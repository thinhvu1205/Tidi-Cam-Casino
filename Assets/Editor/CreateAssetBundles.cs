using UnityEngine;
using UnityEditor;
using System.IO;
public class CreateAssetBundles
{
    [MenuItem("DevGamePro/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        if (EditorUtility.DisplayDialog("Confirm", "Do you want to build asset bundle for " + EditorUserBuildSettings.activeBuildTarget.ToString() + "?", "Yes", "No"))
        {
            string assetBundleDirectory = "Assets/AssetBundles/" + EditorUserBuildSettings.activeBuildTarget;
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }

    [MenuItem("DevGamePro/Clear All")]
    static void ClearAllCache()
    {
        if (EditorUtility.DisplayDialog("Confirm", "Do you want Clear All? " + EditorUserBuildSettings.activeBuildTarget.ToString() + "?", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
        }
    }


    [MenuItem("DevGamePro/Clear PlayerPrefs")]
    static public void AssetBundleClearPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Confirm", "Do you want Clear PlayerPrefs? " + EditorUserBuildSettings.activeBuildTarget.ToString() + "?", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    [MenuItem("DevGamePro/Clear Cache AssetBundles")]
    static public void AssetBundleClearCache()
    {
        if (EditorUtility.DisplayDialog("Confirm", "Do you want Clear Cache AssetBundles? " + EditorUserBuildSettings.activeBuildTarget.ToString() + "?", "Yes", "No"))
        {
            Caching.ClearCache();
        }
    }
}
