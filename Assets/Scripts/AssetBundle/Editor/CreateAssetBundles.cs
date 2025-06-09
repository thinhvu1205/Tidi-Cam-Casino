using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using System;

public class CreateAssetBundles
{
    [MenuItem("Asset Bundles/Clear Cache")] private static void _ClearCache() => Debug.Log(Caching.ClearCache() ? "Cache cleared!" : "Cache not cleared!!!");
    [MenuItem("Asset Bundles/Clear Prefs")] private static void _ClearPrefs() => PlayerPrefs.DeleteAll();

    [MenuItem("Asset Bundles/Build/Android")]
    private static void _BuildForAndroid()
    {
        string basePath = _PrepareRootFolder(BundleHandler.PLATFORM.Android);
        if (basePath.Equals("")) return;
        AssetBundleManifest manifestABM = BuildPipeline.BuildAssetBundles(basePath, BuildAssetBundleOptions.None, BuildTarget.Android);
        _BuildCategory(basePath, manifestABM);
    }
    [MenuItem("Asset Bundles/Build/iOS")]
    private static void _BuildForiOS()
    {
        string basePath = _PrepareRootFolder(BundleHandler.PLATFORM.iOS);
        if (basePath.Equals("")) return;
        AssetBundleManifest manifestABM = BuildPipeline.BuildAssetBundles(basePath, BuildAssetBundleOptions.None, BuildTarget.iOS);
        _BuildCategory(basePath, manifestABM);
    }
    private static string _PrepareRootFolder(BundleHandler.PLATFORM platformType)
    {
        string basePath = BundleHandler.BASE_PATH;
        switch (platformType)
        {
            case BundleHandler.PLATFORM.Android:
                {
                    basePath += BundleHandler.PLATFORM.Android.ToString();
                    break;
                }
            case BundleHandler.PLATFORM.iOS:
                {
                    basePath += BundleHandler.PLATFORM.iOS.ToString();
                    break;
                }
            default:
                {
                    basePath = "";
                    break;
                }
        }
        if (basePath.Equals("")) return "";
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
        else
        {
            Directory.Delete(basePath, true);
            Directory.CreateDirectory(basePath);
        }
        return basePath;
    }
    private static void _BuildCategory(string path, AssetBundleManifest manifestABM)
    {
        DirectoryInfo aDI = new(path);
        FileInfo[] infoFIs = aDI.GetFiles();
        List<string> hashedNames = new();
        foreach (FileInfo infoFI in infoFIs) _RenameBundle(manifestABM, AssetDatabase.GetAllAssetBundleNames(), infoFI, hashedNames);
        string categoryContent = "[" + string.Join(",", hashedNames) + "]";
        string categoryPath = path + "/" + BundleHandler.CATEGORY;
        if (File.Exists(categoryPath)) File.Delete(categoryPath);
        File.WriteAllText(categoryPath, categoryContent);
    }
    private static void _RenameBundle(AssetBundleManifest manifestABM, string[] assetBundleNames, FileInfo infoFI, List<string> nodes)
    {
        if (!infoFI.Name.EndsWith(".manifest") && assetBundleNames.Contains(infoFI.Name))
        {
            Hash128 hash = manifestABM.GetAssetBundleHash(infoFI.Name);
            string suffix = BundleHandler.SPLIT + hash.ToString();
            nodes.Add('"' + infoFI.Name + suffix + '"');
            File.Move(infoFI.FullName, infoFI.FullName + suffix);
        }
    }
}
