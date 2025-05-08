using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleManager : MonoBehaviour
{
    public static AssetBundleManager instance;

    const string URL_BUNDLE = "http://192.168.1.46:8080/AssetBundles/";
    string URL_BUNDLE_TARGET = "";


    private bool IsChecking = false;
    void Awake()
    {
        instance = this;
        URL_BUNDLE_TARGET = URL_BUNDLE + GetPlatformForAssetBundles();

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private string GetPlatformForAssetBundles()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID)
    return "Android";
#elif (!UNITY_EDITOR && UNITY_IOS)
    return "iOS";
#endif

        return "UnityEditor";
    }

    IEnumerator loadAllMaterials()
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(URL_BUNDLE_TARGET + "/modulesmaterials");
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Globals.Logging.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        }
    }

    [Tooltip("Asset Bundle Name:  Tên của bundle, Asset Name: Tên của gameobject")]
    public void loadPrefab(string assetBundleName, string assetName, UnityAction<GameObject> loadDoneCallback, UnityAction loadErrorCallback = null)
    {
        StartCoroutine(GetAssetBundle(assetBundleName, assetName, null, loadDoneCallback, loadErrorCallback));
    }
    [Tooltip("Asset Bundle Name:  Tên của bundle, Asset Name: Tên của gameobject")]
    public void loadPrefab(string assetBundleName, string assetName, UnityAction<float> loaddingCallback, UnityAction<GameObject> loadDoneCallback, UnityAction loadErrorCallback = null)
    {
        StartCoroutine(GetAssetBundle(assetBundleName, assetName, loaddingCallback, loadDoneCallback, loadErrorCallback));
    }

    IEnumerator GetAssetBundle(string assetBundleName, string assetName, UnityAction<float> loaddingCallback, UnityAction<GameObject> loadDoneCallback, UnityAction loadErrorCallback = null)
    {
//#if UNITY_EDITOR
//        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
//        if (assetPaths.Length == 0)
//        {
//            Globals.Logging.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
//            if (loadErrorCallback != null)
//                loadErrorCallback();
//            yield return null;
//        }

//        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPaths[0], typeof(GameObject)) as GameObject;
//        if (loadDoneCallback == null)
//        {
//            Globals.Logging.LogError("Callback done == null");
//            yield return null;
//        }else
//            loadDoneCallback(prefab);

//#else

        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(URL_BUNDLE_TARGET + assetBundleName);

        if (loaddingCallback == null)
        {
            yield return www.SendWebRequest();
        }
        else
        {
            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                loaddingCallback.Invoke(operation.progress * 100);
                yield return null;
            }
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Globals.Logging.Log(www.error);
            if (loadErrorCallback != null)
                loadErrorCallback();
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            var prefab = bundle.LoadAsset<GameObject>(assetName);

            ////string dataFileName = "WaterVehicles";
            //string tempPath = Path.Combine(Application.persistentDataPath, "AssetData");
            //tempPath = Path.Combine(tempPath, assetBundleName + ".unity3d");

            ////Save
            //var data = DownloadHandlerAssetBundle.
            //save(www., tempPath);
            if (loadDoneCallback == null)
            {
                Globals.Logging.LogError("Callback done == null");
                yield return null;
            }else
                loadDoneCallback(prefab);
        }
        www.Dispose();
//#endif
    }

    IEnumerator downloadAsset()
    {
        string url = "http://url.net/YourAsset.unity3d";

        UnityWebRequest www = UnityWebRequest.Get(url);
        DownloadHandler handle = www.downloadHandler;

        //Send Request and wait
        yield return www.Send();

        if (www.isNetworkError)
        {

            Globals.Logging.Log("Error while Downloading Data: " + www.error);
        }
        else
        {
            Globals.Logging.Log("Success");

            //handle.data

            //Construct path to save it
            string dataFileName = "WaterVehicles";
            string tempPath = Path.Combine(Application.persistentDataPath, "AssetData");
            tempPath = Path.Combine(tempPath, dataFileName + ".unity3d");

            //Save
            save(handle.data, tempPath);
        }
    }

    IEnumerable LoadObject(string path)
    {
        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(path);
        yield return bundle;

        AssetBundle myLoadedAssetBundle = bundle.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Globals.Logging.Log("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest request = myLoadedAssetBundle.LoadAssetAsync<GameObject>("boat");
        yield return request;

        GameObject obj = request.asset as GameObject;
        obj.transform.position = new Vector3(0.08f, -2.345f, 297.54f);
        obj.transform.Rotate(350.41f, 400f, 20f);
        obj.transform.localScale = new Vector3(1.0518f, 0.998f, 1.1793f);

        Instantiate(obj);

        myLoadedAssetBundle.Unload(false);
    }

    void save(byte[] data, string path)
    {
        //Create the Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, data);
            Globals.Logging.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Globals.Logging.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Globals.Logging.LogWarning("Error: " + e.Message);
        }
    }


    //IEnumerator GetAssetBundle(string assetBundleName, string assetName, System.Type type)
    //{
    //    string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
    //    if (assetPaths.Length == 0)
    //    {
    //        Globals.Logging.LogError("There is no asset with name \"" + assetName + "\" in " + assetBundleName);
    //        yield return null;
    //    }
    //    UnityWebRequest www0 = UnityWebRequestAssetBundle.GetAssetBundle("http://192.168.1.46:8080/AssetBundles/Android/modulesmaterials");
    //    yield return www0.SendWebRequest();
    //    UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("http://192.168.1.46:8080/AssetBundles/Android/dialog");
    //    yield return www.SendWebRequest();

    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        Globals.Logging.Log(www.error);
    //    }
    //    else
    //    {
    //        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
    //        var prefab = bundle.LoadAsset<GameObject>("dialog");
    //        //Instantiate(prefab);

    //        //var dialog = Instantiate(prefab, parentPopups).GetComponent<DialogView>();
    //        //dialog.transform.localScale = Vector3.one;
    //        //dialog.setMessage("TestTestTestTestTest");
    //        //dialog.setIsShowButton1(true, "OK", null);
    //        //dialog.setIsShowButton2(false, "", null);
    //        //dialog.setIsShowClose(false, null);
    //    }
    //}



    //IEnumerator DownloadFromRemote(Hash128 remoteHash)
    //{
    //    string uri = "http://myhosting.domain.com/levels";
    //    var request = UnityWebRequestAssetBundle.GetAssetBundle(uri, remoteHash, 0);
    //    yield return request.SendWebRequest();
    //    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
    //}
    //IEnumerator GetLocalData()
    //{
    //    string uri = Path.Combine(Application.streamingAssetsPath, "Android");
    //    var request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
    //    yield return request.SendWebRequest();
    //    AssetBundle manifestBundle = DownloadHandlerAssetBundle.GetContent(request);
    //    AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    //    Hash128 hash = manifest.GetAssetBundleHash("levels");
    //    manifestBundle.Unload(true);
    //    CachedAssetBundle cachedAssetBundle = new CachedAssetBundle("levels", hash);
    //    uri = Path.Combine(Application.streamingAssetsPath, "levels");
    //    request = UnityWebRequestAssetBundle.GetAssetBundle(uri, cachedAssetBundle, 0);
    //    yield return request.SendWebRequest();
    //}
}
