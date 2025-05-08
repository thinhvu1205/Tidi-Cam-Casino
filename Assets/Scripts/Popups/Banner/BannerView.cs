using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Globals;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BannerView : BaseView
{
    [SerializeField]
    GameObject btnClose;

    [SerializeField]
    Image imageBanner;

    System.Action callbaclClick = null;
    bool isNotShowNext = false;
    public bool isBannerType9 = false;
    public JObject data;
    //[SerializeField]
    //bool isBannerLobby = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        if (background != null)
        {
            if (TableView.instance != null && TableView.instance.isHorizontal && ShopView.instance == null)
            {
                background.transform.localEulerAngles = new Vector3(0, 0, 270);
            }
            else
            {
                background.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    public async void setInfo(JObject _data, bool isMark = true, System.Action _callbaclClick = null, Sprite imageS = null)
    {
        data = _data;
        //Debug.Log("DataBanner=" + data.ToString());
        //[{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.lengbear.com/Banner/lq0/1011/btn_recharge.png\",\"pos\":[0.5,0.5],\"urlLink\":\"http://kenh14.vn/\"}],\"_id\":\"5dcba97af89e24167aee37f1\",\"id\":\"5dc3edacdda6164a2693f86e\",\"title\":\"test chọn game\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.ngwcasino.com/Test/1.jpg\",\"isOnList\":false,\"showByActionType\":6,\"priority\":1}
        if (UIManager.instance.gameView != null && !isBannerType9)
        {
            Debug.Log(" Co game view----> Destroy Banner");
            Destroy(gameObject);
            return;
        }
        if (isBannerType9)
        {
            Debug.Log(" ADd banner type9");
        }

        callbaclClick = _callbaclClick;
        var mask = transform.Find("mask");
        if (mask != null)
            mask.gameObject.SetActive(isMark);

        JArray arrButton = (JArray)data["arrButton"];

        string urlImg = (string)data["urlImg"];

        bool isClose = (bool)data["isClose"];
        if (this == null) return;
        imageBanner.sprite = imageS == null ? await Globals.Config.GetRemoteSprite(urlImg, true) : imageS;
        if (btnClose != null) btnClose.SetActive(isClose);
        if (this == null || gameObject == null || imageBanner.IsDestroyed()) return;
        if (UIManager.instance.gameView != null && !isBannerType9)
        {
            Debug.Log(" Co game view----> Destroy Banner");
            Destroy(gameObject);
            return;
        }
        imageBanner.SetNativeSize();
        if (btnClose != null)
        {
            RectTransform closeButtonRT = btnClose.GetComponent<RectTransform>();
            closeButtonRT.anchorMax = new Vector2(1, 1);
            closeButtonRT.anchorMin = new Vector2(1, 1);
        }
        var scale = 1.0f;
        if (imageBanner.rectTransform.rect.width >= 1280)
        {
            scale = 1280f / imageBanner.rectTransform.rect.width - 0.1f;
        }
        imageBanner.transform.localScale = new Vector3(scale, scale, scale);
        for (var i = 0; i < arrButton.Count; i++)
        {
            var dtBtn = arrButton[i];
            List<float> posss = ((JArray)dtBtn["pos"]).ToObject<List<float>>();

            //Texture2D tex = await Globals.Config.GetRemoteTexture((string)dtBtn["urlBtn"]);
            Sprite spr = await Globals.Config.GetRemoteSprite((string)dtBtn["urlBtn"], true);
            if (this == null) return;
            if (UIManager.instance.gameView != null && !isBannerType9)
            {
                Destroy(gameObject);
                return;
            }
            if (spr != null)
            {
                //var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                var btnView = Globals.Config.createNodeButton(spr);
                btnView.GetComponent<Image>().SetNativeSize();
                btnView.transform.SetParent(imageBanner.transform, false);
                btnView.transform.localScale = Vector3.one;
                if (imageBanner.rectTransform.rect.width == float.NaN)
                {
                    imageBanner.rectTransform.sizeDelta = new Vector2(270, 479);
                }
                Vector2 posBtn = new Vector3(imageBanner.rectTransform.rect.width * (posss[0] - 0.5f), imageBanner.rectTransform.rect.height * (posss[1] - 0.5f));
                btnView.transform.localPosition = posBtn;
                btnView.transform.localEulerAngles = new Vector3(0, 0, 0);
                btnView.onClick.RemoveAllListeners();
                btnView.onClick.AddListener(() =>
                {

                    SocketIOManager.getInstance().logEventSuggestBanner(2, data);
                    //}
                    string type = (string)dtBtn["type"];
                    Debug.Log("Type Banner Click-" + type);
                    switch (type)
                    {
                        case "openlink":
                            {
                                //Debug.Log("Chi chau open link=====" + (string)dtBtn["urlLink"]);
                                Application.OpenURL((string)dtBtn["urlLink"]);
                                break;
                            }
                        case "showwebview":
                            {
                                string url = (string)dtBtn["urlLink"];
                                url = url.Replace("%userid%", Globals.User.userMain.Userid.ToString());
                                url = url.Replace("%uid%", Globals.User.userMain.Userid.ToString());
                                url = url.Replace("%dm%", "0");
                                UIManager.instance.showWebView(url);
                                break;
                            }
                        case "ok":
                            { //chi de thong bao

                                //this.Baner.onClose();

                                onClickClose(true);
                                break;
                            }
                        case "pm":
                            {
                                UIManager.instance.openShop((string)dtBtn["titlePM"]);
                                break;
                            }
                        case "playnow":
                            {
                                int gameID = (int)dtBtn["gameID"];

                                UIManager.instance.lobbyView.onClickGameFromBanner(gameID);
                                isNotShowNext = true;
                                onClickClose(true);
                                break;
                            }
                        case "force":
                            { //buoc ra khoi cuoc doi bo
                                Application.Quit();
                                break;
                            }
                        case "update":
                            //              cc.sys.openURL(dataBtn.urlLink);
                            //isTypeUpdate = true;

                            Application.OpenURL((string)dtBtn["urlLink"]);
                            break;
                        case "cashout": // open CO
                                        //              cc.NGWlog('chay vao  btn  cash out', require("ConfigManager").getInstance().is_dt);
                            if (Config.IsBuildStore)
                            {

                            }
                            else
                            {
                                if (Globals.Config.is_dt)
                                {
                                    //cc.NGWlog('chay vao  btn  cash out 22222');
                                    UIManager.instance.openEx();
                                }
                            }
                            break;
                        case "topgame": // top game
                                        //require("UIManager").instance.onShowTopGame(dataBtn.gameID);
                                        //UIManager.instance.lobbyView.hideMore();
                            UIManager.instance.openLeaderBoard((int)dtBtn["gameID"]);
                            break;
                        case "gofarms":
                            UIManager.instance.openVipFarm();
                            break;
                    }
                    callbaclClick?.Invoke();
                });
            }
        }

    }

    public new void onClickClose(bool isDestroy = true)
    {
        base.onClickClose(isDestroy);
        UIManager.instance.nextBanner(isNotShowNext);
        SocketIOManager.getInstance().logEventSuggestBanner(1, data);
    }
    //private IEnumerator loadBanner(Image _sprite = null, string url = "", Action callback = null)
    //{
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
    //    yield return request.SendWebRequest();
    //    if (request.result==UnityWebRequest.Result.ConnectionError || request.result==UnityWebRequest.Result.ProtocolError)
    //        Debug.Log(request.error);
    //    else
    //        _sprite.sprite = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //}

}
