using System.Collections;
using Globals;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{
    // https://console.cloud.google.com/storage/browser/kh9;tab=objects?forceOnBucketsSortingFiltering=true&inv=1&invt=AbzYXw&project=myanmar-199404&prefix=&forceOnObjectsSortingFiltering=false
    [SerializeField] private BundleDownloader m_BundleBD;

    private void Awake()
    {
        // D:/Unity projects/Tidi-Phil-Win777/Assets/AssetBundles;
        // https://storage.googleapis.com/kh9/AssetBundles/
        string storedUrl = PlayerPrefs.GetString(BundleDownloader.STORED_BUNDLE_URL, "");
        m_BundleBD.CheckAndDownloadAssets(storedUrl,
            () =>
            {
                m_BundleBD.SetProgressText("Retrying ...");
                StartCoroutine(retry());
            },
            () =>
            {
                SceneManager.LoadScene("MainScene");
            });

        IEnumerator retry()
        {
            while (Config.Bundle_URL.Equals("")) yield return new WaitForSeconds(3f);
            m_BundleBD.CheckAndDownloadAssets(Config.Bundle_URL,
                () =>
                {
                    m_BundleBD.SetProgressText("Fail to get assets!");
                },
                () =>
                {
                    SceneManager.LoadScene("MainScene");
                });
        }

    }
}
