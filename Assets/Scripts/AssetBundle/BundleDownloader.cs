using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleJSON;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BundleDownloader : MonoBehaviour
{
    public const string STORED_BUNDLE_URL = "storedBundleUrl";
    [SerializeField] private Image m_ProgressImg;
    [SerializeField] private TextMeshProUGUI m_ProgressTMPUI;
    private List<Coroutine> _SentGettingBundleCs = new();
    private List<BundleVersion> _NewDataBVs = new(), _StoredDataBVs = new();
    private Action _OnFailCb, _OnCompleteCb;
    private int _TotalBundles, _CachedBundles;

    public void CheckAndDownloadAssets(string url, Action onFailCb = null, Action onCompleteCb = null)
    {
        if (url.Equals("")) return;
        if (!url[^1].Equals('/')) url += "/"; // if it does not end with "/" then add it
#if UNITY_ANDROID
        string platformFolder = "/" + BundleHandler.PLATFORM.Android.ToString() + "/";
#elif UNITY_IOS
        string platformFolder = "/" + BundleHandler.PLATFORM.iOS.ToString() + "/";
#endif
        if (!url.EndsWith(platformFolder)) url += platformFolder.Remove(0, 1);
        PlayerPrefs.SetString(STORED_BUNDLE_URL, url);
        PlayerPrefs.Save();
        AssetBundle.UnloadAllAssetBundles(true);
        if (!url.Contains("://")) url = "file:///" + url;
        _OnFailCb = onFailCb;
        _OnCompleteCb = onCompleteCb;
        StartCoroutine(_GetAssetBundles(url));
    }
    public void SetProgressValue(float value) => m_ProgressImg.fillAmount = (_CachedBundles + value) / _TotalBundles;
    public void SetProgressText(string content) => m_ProgressTMPUI.text = content;
    private IEnumerator _GetAssetBundles(string url)
    {
        using UnityWebRequest aUWR = UnityWebRequest.Get(url + BundleHandler.CATEGORY); // get new category content from server
        yield return aUWR.SendWebRequest();

        if (aUWR.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("|   ) )=3 Get category fail: " + aUWR.result.ToString() + " / " + aUWR.error + " / Path: " + aUWR.uri);
            _OnFailCb?.Invoke();
        }
        else
        {
            BundleHandler.MAIN.ClearAssetsDictionary();
            string newContent = aUWR.downloadHandler.text, storedPath = Application.persistentDataPath + "/" + BundleHandler.CATEGORY;
            if (!_TryParseCategory(_NewDataBVs, _TryParseJsonArray(newContent), url))
            {
                Debug.Log("|   ) )=3 Wrong latest bundle info!");
                _OnFailCb?.Invoke();
                yield break;
            }
            if (File.Exists(storedPath))
            {
                if (!_TryParseCategory(_StoredDataBVs, _TryParseJsonArray(File.ReadAllText(storedPath)), ""))
                {
                    Debug.Log("|   ) )=3 Wrong stored bundles info, clear all cached bundles!");
                    Caching.ClearCache();
                    File.Delete(storedPath);
                }
            }
            File.WriteAllText(storedPath, newContent);
            _ClearOldCachedBundleVersions();
            _TotalBundles = _NewDataBVs.Count;
            _CachedBundles = 0;
            if (_TotalBundles > 0)
            {
                _SetProgressUI(0);
                _SentGettingBundleCs.Add(StartCoroutine(_LoadAssetBundles()));
            }
            else _CompleteLoadingAssets();
        }
    }
    private IEnumerator _LoadAssetBundles()
    {
        if (_NewDataBVs.Count > 0)
        {
            BundleVersion thisBV = _NewDataBVs[0];
            while (!Caching.ready) yield return null;
            using UnityWebRequest aUWR = UnityWebRequestAssetBundle.GetAssetBundle(thisBV.Url, thisBV.HashH128, 0);
            thisBV.State = BundleVersion.STATE.Downloading;
            aUWR.SendWebRequest();
            while (!aUWR.isDone)
            {
                _SetProgressUI(aUWR.downloadProgress);
                yield return null;
            }
            if (aUWR.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("|   ) )=3 Error getting asset bundle: " + aUWR.error + " | " + aUWR.url);
                thisBV.State = BundleVersion.STATE.Cloud;
                _OnFailCb?.Invoke();
                foreach (Coroutine sentRequestC in _SentGettingBundleCs) StopCoroutine(sentRequestC);
            }
            else
            {
                thisBV.BundleAB = DownloadHandlerAssetBundle.GetContent(aUWR);
                thisBV.State = BundleVersion.STATE.Downloaded;
                thisBV.AssetNamesHS = thisBV.BundleAB.GetAllAssetNames().ToHashSet();
                BundleHandler.MAIN.AddToLocalMap(thisBV);
                _NewDataBVs.Remove(thisBV);
                _CachedBundles += 1;
                _SentGettingBundleCs.Add(StartCoroutine(_LoadAssetBundles()));
            }
        }
        else _CompleteLoadingAssets();
    }
    private JSONArray _TryParseJsonArray(string input)
    {
        try { return JSON.Parse(input).AsArray; }
        catch (Exception e) { Debug.Log("|   ) )=3 Error parsing array: " + e); return null; }
    }
    private bool _TryParseCategory(List<BundleVersion> storedBVs, JSONArray categoryJA, string url)
    {
        try
        {
            storedBVs.Clear();
            for (int i = 0; i < categoryJA.Count; i++)
            {
                string[] split = categoryJA[i].Value.Split(BundleHandler.SPLIT, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length <= 1) continue;
                storedBVs.Add(new() { Name = split[0], HashH128 = Hash128.Parse(split[1]), Url = url + categoryJA[i].Value });
            }
            return true;
        }
        catch (Exception e) { Debug.Log("|   ) )=3 Fail to parse Category content!!! " + e); return false; }
    }
    private void _ClearOldCachedBundleVersions()
    {
        foreach (BundleVersion aBV in _StoredDataBVs)
            if (_NewDataBVs.Find(x => x.Name.Equals(aBV.Name) && x.HashH128 != aBV.HashH128) != null)
                Caching.ClearCachedVersion(aBV.Name, aBV.HashH128);
    }
    private void _SetProgressUI(float value)
    {
        SetProgressValue(value);
        SetProgressText((value >= 1 ? _TotalBundles : _CachedBundles) + "/" + _TotalBundles);
    }
    private void _CompleteLoadingAssets()
    {
        Debug.Log("|   ) )=3 Complete Loading AssetBundles");
        _SetProgressUI(1);
        _OnCompleteCb?.Invoke();
    }
}
