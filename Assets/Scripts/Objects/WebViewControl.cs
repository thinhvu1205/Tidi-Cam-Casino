using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class WebViewControl : MonoBehaviour
{
  public static WebViewControl instance = null;

  public string Url;
  public Text status;
  [SerializeField] public TextMeshProUGUI lbTitle;
  [SerializeField] public GameObject titleBar;
  [SerializeField] public RectTransform Rect;
  WebViewObject webViewObject;

  private void Awake()
  {
  }
  private void Start()
  {
    WebViewControl.instance = this;
  }
  public void loadUrl(string urlWeb, string title = "")
  {
    Url = urlWeb;
    if (title != "")
    {
      lbTitle.text = title;
      lbTitle.gameObject.SetActive(true);
    }
    StartCoroutine(TaiURL());
  }
  public IEnumerator TaiURL()
  {
    yield return null;
    webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

    webViewObject.Init(
        cb: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallFromJS[{0}]", msg));
          //status.text = msg;
          //status.GetComponent<Animation>().Play();
        },
        err: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallOnError[{0}]", msg));
          //status.text = msg;
          //status.GetComponent<Animation>().Play();
        },
        httpErr: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallOnHttpError[{0}]", msg));
          //status.text = msg;
          //status.GetComponent<Animation>().Play();
        },
        started: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallOnStarted[{0}]", msg));
        },
        hooked: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallOnHooked[{0}]", msg));
        },
        ld: (msg) =>
        {
          Globals.Logging.Log(string.Format("CallOnLoaded[{0}]", msg));
          showWebView();
#if UNITY_EDITOR_OSX || (!UNITY_ANDROID && !UNITY_WEBPLAYER && !UNITY_WEBGL)
                // NOTE: depending on the situation, you might prefer
                // the 'iframe' approach.
                // cf. https://github.com/gree/unity-webview/issues/189
#if true
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        window.location = 'unity:' + msg;
                      }
                    }
                  }
                ");
#else
                webViewObject.EvaluateJS(@"
                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
                    window.Unity = {
                      call: function(msg) {
                        window.webkit.messageHandlers.unityControl.postMessage(msg);
                      }
                    }
                  } else {
                    window.Unity = {
                      call: function(msg) {
                        var iframe = document.createElement('IFRAME');
                        iframe.setAttribute('src', 'unity:' + msg);
                        document.documentElement.appendChild(iframe);
                        iframe.parentNode.removeChild(iframe);
                        iframe = null;
                      }
                    }
                  }
                ");
#endif
#elif UNITY_WEBPLAYER || UNITY_WEBGL
          webViewObject.EvaluateJS(
                  "window.Unity = {" +
                  "   call:function(msg) {" +
                  "       parent.unityWebView.sendMessage('WebViewObject', msg)" +
                  "   }" +
                  "};");
#endif
          webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
        }
        //transparent: false,
        //zoom: true,
        //ua: "custom user agent string",
        //// android
        //androidForceDarkMode: 0,  // 0: follow system setting, 1: force dark off, 2: force dark on
        //// ios
        //enableWKWebView: true,
        //wkContentMode: 0,  // 0: recommended, 1: mobile, 2: desktop
        //wkAllowsLinkPreview: true,
        //// editor
        //separated: false
        );
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        webViewObject.bitmapRefreshCycle = 1;
#endif
    // cf. https://github.com/gree/unity-webview/pull/512
    // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru � Pull Request #512 � gree/unity-webview
    //webViewObject.SetAlertDialogEnabled(false);

    // cf. https://github.com/gree/unity-webview/pull/728
    //webViewObject.SetCameraAccess(true);
    //webViewObject.SetMicrophoneAccess(true);

    // cf. https://github.com/gree/unity-webview/pull/550
    // introduced SetURLPattern(..., hookPattern). by KojiNakamaru � Pull Request #550 � gree/unity-webview
    //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

    // cf. https://github.com/gree/unity-webview/pull/570
    // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 � Pull Request #570 � gree/unity-webview
    //webViewObject.SetBasicAuthInfo("id", "password");

    //webViewObject.SetScrollbarsVisibility(true);


    webViewObject.SetTextZoom(100);  // android only. cf. https://stackoverflow.com/questions/21647641/android-webview-set-font-size-system-default/47017410#47017410
    webViewObject.SetVisibility(true);
    if (UIManager.instance.gameView != null)
    {
      if (UIManager.instance.gameView.transform.eulerAngles.z == 0)
      {

        webViewObject.SetMargins(0, Screen.currentResolution.height - Mathf.FloorToInt(Screen.safeArea.height) + Mathf.FloorToInt(titleBar.GetComponent<RectTransform>().rect.height), 0, 0);
        //webViewObject.SetMargins(0, Mathf.FloorToInt(Rect.rect.height), 0, 0);
      }
      else
      {
        Globals.Logging.Log("Webview:Screen.currentResolution1:" + Screen.currentResolution);
        UIManager.instance.changeOrientation(ScreenOrientation.LandscapeLeft);
        Globals.Logging.Log("Webview:Screen.currentResolution2:" + Screen.currentResolution);
        Globals.Logging.Log("Webview:safe area= " + Screen.safeArea);
        webViewObject.SetMargins(0, (int)titleBar.GetComponent<RectTransform>().sizeDelta.y, 0, 0);
      }
    }
    else
    {
      webViewObject.SetMargins(0, Screen.currentResolution.height - (int)Screen.safeArea.height + (int)titleBar.GetComponent<RectTransform>().sizeDelta.y, 0, 0);
    }


#if !UNITY_WEBPLAYER && !UNITY_WEBGL
        if (Url.StartsWith("http"))
        {
            webViewObject.LoadURL(Url.Replace(" ", "%20"));
        }
        else
        {
            var exts = new string[]{
                ".jpg",
                ".js",
                ".html"  // should be last
            };
            foreach (var ext in exts)
            {
                var url = Url.Replace(".html", ext);
                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
                byte[] result = null;
                if (src.Contains("://"))
                {  // for Android
#if UNITY_2018_4_OR_NEWER
                    // NOTE: a more complete code that utilizes UnityWebRequest can be found in https://github.com/gree/unity-webview/commit/2a07e82f760a8495aa3a77a23453f384869caba7#diff-4379160fa4c2a287f414c07eb10ee36d
                    var unityWebRequest = UnityWebRequest.Get(src);
                    yield return unityWebRequest.SendWebRequest();
                    result = unityWebRequest.downloadHandler.data;
#else
                    var www = new WWW(src);
                    yield return www;
                    result = www.bytes;
#endif
                }
                else
                {
                    result = System.IO.File.ReadAllBytes(src);
                }
                System.IO.File.WriteAllBytes(dst, result);
                if (ext == ".html")
                {
                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
                    break;
                }
            }
        }
#else
    if (Url.StartsWith("http"))
    {
      webViewObject.LoadURL(Url.Replace(" ", "%20"));
    }
    else
    {
      webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
    }
#endif
  }

  private void showWebView()
  {
    return;
    //if (UIManager.instance.gameView != null)
    //{
    //    if (UIManager.instance.gameView.transform.eulerAngles.z == 0)
    //    {
    //        webViewObject.SetMargins(0, Screen.currentResolution.height- (int)Screen.safeArea.height + (int)titleBar.GetComponent<RectTransform>().sizeDelta.y, 0, 0);
    //    }
    //    else
    //    {
    //        Globals.Logging.Log("Screen.currentResolution1:" + Screen.currentResolution);
    //        Globals.Logging.Log("Webview size1=" + GetComponent<RectTransform>().rect);
    //        UIManager.instance.changeOrientation(ScreenOrientation.LandscapeLeft);
    //        float tile = (float)Screen.currentResolution.width / 720;
    //        Globals.Logging.Log("Screen.currentResolution2:" + Screen.currentResolution);
    //        Globals.Logging.Log("safe area= " + Screen.safeArea);
    //        webViewObject.SetMargins(0, (int)titleBar.GetComponent<RectTransform>().sizeDelta.y, 0, 0);
    //        Globals.Logging.Log("Webview size2=" + GetComponent<RectTransform>().rect);
    //    }
    //}
    //else
    //{
    //    webViewObject.SetMargins(0, Screen.currentResolution.height - (int)Screen.safeArea.height + (int)titleBar.GetComponent<RectTransform>().sizeDelta.y, 0, 0);
    //}
  }
  public void closeWebView()
  {
    Destroy(gameObject);
    //if (Screen.orientation != ScreenOrientation.Portrait)
    //{
    //    UIManager.instance.changeOrientation(ScreenOrientation.Portrait);
    //}
  }
  private void OnDestroy()
  {
    SocketSend.sendUAG();
    var g = GameObject.Find("WebViewObject");
    if (g != null)
    {
      Destroy(g);
    }
    //if (Screen.orientation != ScreenOrientation.Portrait)
    //    UIManager.instance.changeOrientation(ScreenOrientation.Portrait);
  }

}
