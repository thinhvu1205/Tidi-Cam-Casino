using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Globals;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class SocketIOManager
{
    private static SocketIOManager instance = null;
    public JObject DATAEVT0 = null;
    public bool isSendFirst = false;
    private List<JObject> listDataResendForPacket = new();
    private List<string> packetDetail = new(), //evt nào có trong array này thì bắn đủ data (bắn lên "packetDetail")
        blackListBehaviorIgnore = new(), //behaviorI: (behavior Ignore) evt nào có trong đây thì ko bắn lên  (bắn lên "behavior")
        whiteListOnlySendEvt = new(), //packet: evt nào có trong array này thì bắn evt, isSend, timestamp.. (bắn lên "packet")
        listResendData = new(), arrayIDBannerShowed = new();
    private ConnectionStatus connectionStatus = ConnectionStatus.NONE;
    private SocketIOUnity clientIO;
    private string EVENT = "event", REGINFO = "reginfo", LOGIN = "login", BEHAVIOR = "behavior", UPDATE = "update", url_old = "";
    private bool isGetedListFillter, isEmitReginfo;

    public static SocketIOManager getInstance()
    {
        if (instance == null) instance = new SocketIOManager();
        return instance;
    }
    public SocketIOManager() { }
    public void initSml()
    {
        try
        {
            string _blackList = PlayerPrefs.GetString("dataFilter", "");
            if (!_blackList.Equals(""))
            {
                JObject blackList = JObject.Parse(PlayerPrefs.GetString("dataFilter"));
                if (blackList != null)
                {
                    packetDetail = ((JArray)blackList["packetDetail"]).ToObject<List<string>>();
                    blackListBehaviorIgnore = ((JArray)blackList["behaviorI"]).ToObject<List<string>>();
                    whiteListOnlySendEvt = ((JArray)blackList["packet"]).ToObject<List<string>>();
                }
            }
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    public void startSIO()
    {
        try
        {
            Debug.Log("-=-== startSIO " + Config.u_SIO);
            if (!url_old.Equals(Config.u_SIO))
            {
                url_old = Config.u_SIO;
                stopIO();
            }
            if (connectionStatus == ConnectionStatus.CONNECTED || connectionStatus == ConnectionStatus.CONNECTING) return;

            Debug.Log("-=-== start Connect " + Config.u_SIO);
            SocketIOOptions options = new() { IgnoreServerCertificateValidation = true };
            Uri uri = new(Config.u_SIO);
            clientIO = new(uri, options) { JsonSerializer = new NewtonsoftJsonSerializer() };
            connectionStatus = ConnectionStatus.CONNECTING;
            clientIO.OnConnected += (sender, e) =>
            {
                Debug.Log("-=-== CONNECTED SIO ");
                connectionStatus = ConnectionStatus.CONNECTED;
                if (!isEmitReginfo)
                {
                    emitReginfo();
                    isEmitReginfo = true;
                }
                if (isSendFirst)
                    if (Config.isLoginSuccess)
                        emitLogin();
                if (DATAEVT0 != null)
                    if (Config.isLoginSuccess)
                        emitSIOWithValue(DATAEVT0, "LoginPacket", false);
                for (int i = 0; i < listResendData.Count; i++)
                {
                    if (listResendData[i].Contains("ClickLogOut")) continue;
                    emitSIO(listResendData[i]);
                }
                listResendData.Clear();
            };
            clientIO.OnDisconnected += (sender, e) =>
            {
                Debug.Log("SML DISCONNECTED");
                isSendFirst = false;
                isEmitReginfo = false;
                connectionStatus = ConnectionStatus.DISCONNECTED;
            };
            clientIO.OnError += (sender, e) =>
            {
                Debug.Log("SML Connect Error:" + e.ToString());
                isSendFirst = false;
                isEmitReginfo = false;
                connectionStatus = ConnectionStatus.DISCONNECTED;
            };
            clientIO.On(EVENT, data =>
            {
                Debug.Log("SML===============> event:" + data.ToString());
                UnityMainThread.instance.AddJob(() =>
                {
                    string dataStr = data.ToString();
                    handleEvent(dataStr);
                });
            });
            clientIO.Connect();
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    public void stopIO()
    {
        if (clientIO != null) clientIO.Disconnect();
        clientIO = null;
    }
    async Task handleEvent(string strData)
    {
        JArray dataArr = JArray.Parse(strData);
        JToken data = dataArr[0];
        //{ "event":"filter","packetDetail":["0","LoginPacket","JoinPacket","LeavePacket"],"packet":["0","ltv","pctable","selectG2","uag"],"behaviorI":[],"valueGet":[]}
        string evt = (string)data["event"];
        Debug.Log("===============> SIO: handleEvent la " + strData);
        try
        {
            switch (evt)
            {
                case "filter":
                    {
                        Debug.Log("-=-= filter");
                        //PlayerPrefs.SetString("dataFilter", strData);
                        packetDetail = ((JArray)data["packetDetail"]).ToObject<List<string>>();
                        blackListBehaviorIgnore = ((JArray)data["behaviorI"]).ToObject<List<string>>();
                        whiteListOnlySendEvt = ((JArray)data["packet"]).ToObject<List<string>>();
                        isGetedListFillter = true;
                        while (listDataResendForPacket.Count > 0)
                        {
                            JObject resend = listDataResendForPacket[0];
                            emitSIOWithValuePacket((JObject)resend["strData"], (string)resend["namePackage"], (bool)resend["isSend"], (bool)resend["isPacketDetai"], (long)resend["timestamp"]);
                            listDataResendForPacket.RemoveAt(0);
                        }
                        break;
                    }
                case "banner":
                    {
                        if (HandleData.DelayHandleLeave > 0) await Task.Delay((int)(HandleData.DelayHandleLeave + 0.5f) * 1000); //delay thêm 0.5s cho chắc
                        JArray arrData = (JArray)data["data"];
                        JArray arrOnlistFalse = new(), arrOnlistTrue = new(), arrBannerLobby = new();
                        for (int i = 0; i < arrData.Count; i++)
                        {
                            JObject item = (JObject)arrData[i];
                            if (item.ContainsKey("urlImg") && !((string)item["urlImg"]).Equals(""))
                            {
                                if (item.ContainsKey("showByActionType") && (int)item["showByActionType"] == 9)
                                    arrBannerLobby.Add(item);
                                else if (item.ContainsKey("isOnList") && (bool)item["isOnList"])
                                    arrOnlistTrue.Add(item);
                                else
                                    arrOnlistFalse.Add(item);
                            }
                        }
                        if (arrBannerLobby.Count > 0) Config.arrBannerLobby = arrBannerLobby;
                        //UIManager.instance.preLoadBaner(data.data);
                        UIManager.instance.handleBannerIO(arrOnlistFalse);
                        Config.arrOnlistTrue.Merge(arrOnlistTrue);
                        UIManager.instance.updateBannerNews();
                        if (UIManager.instance.lobbyView.gameObject.activeSelf) UIManager.instance.showListBannerOnLobby();
                        break;
                    }
                case "getcf":
                    {
                        break;
                    }
            }
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    // public void testBanner()
    // {
    //     //string str = "{\"event\":\"banner\",\"data\":[{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.lengbear.com/Banner/lq0/1011/btn_recharge.png\",\"pos\":[0.5,0.5],\"urlLink\":\"http://kenh14.vn/\"}],\"_id\":\"5dcba97af89e24167aee37f1\",\"id\":\"5dc3edacdda6164a2693f86e\",\"title\":\"testchọngame\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/demo.png\",\"isOnList\":false,\"showByActionType\":6,\"priority\":1},{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.lengbear.com/Banner/lq0/1011/btn_recharge.png\",\"pos\":[0.5,0.2],\"urlLink\":\"https://vnexpress.net/\"}],\"_id\":\"5dcba97af89e24167aee37f2\",\"id\":\"5dca5cb01442f41a4c8f2242\",\"title\":\"testchọngame\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/demo.png\",\"isOnList\":false,\"showByActionType\":6,\"priority\":2}]}";

    //     string str = "{\"event\":\"banner\",\"data\":[{\"arrButton\":[{\"type\":\"showwebview\",\"urlBtn\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/Banner_Lobby/May25/V0.jpg\",\"urlLink\":\"http://pm.davaogames.com/fortumo?userid=%uid%&price=20\",\"pos\":[0.5,0.5]}],\"_id\":\"62c3a3fe24f9eb0018cf82ed\",\"id\":\"628d96bb56d93d00186cd4f9\",\"title\":\"[May25]NewBanner\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/Banner_Lobby/May25/V0.jpg\",\"isOnList\":false,\"showByActionType\":9,\"priority\":1,\"isShowGameView\":false},{\"arrButton\":[],\"_id\":\"62c3a3fe24f9eb0018cf82ee\",\"id\":\"62be6891d31c8e001a9385d1\",\"title\":\"Invite-01July\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/1.jpg\",\"isOnList\":false,\"showByActionType\":9,\"priority\":2,\"isShowGameView\":false},{\"arrButton\":[{\"type\":\"cashout\",\"urlBtn\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/3.png\",\"pos\":[0.5,0.5]}],\"_id\":\"62c3a3fe24f9eb0018cf82ef\",\"id\":\"62a965db08c8fb001181661d\",\"title\":\"MờiCO-01Jul\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/3.png\",\"isOnList\":false,\"showByActionType\":9,\"priority\":3,\"isShowGameView\":false},{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/4.png\",\"pos\":[0.5,0.5],\"urlLink\":\"https://laropay.net/\"}],\"_id\":\"62c3a3fe24f9eb0018cf82f2\",\"id\":\"62be6b0ed31c8e001a9385d3\",\"title\":\"GiớithiệuLaropay-01Jul\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/tongits_club_online_test/4.png\",\"isOnList\":false,\"showByActionType\":9,\"priority\":4,\"isShowGameView\":false},{\"arrButton\":[],\"_id\":\"62c3a3fe24f9eb0018cf82f1\",\"id\":\"62a94c6808c8fb00118165fc\",\"title\":\"Cảnhbáo-1006-all-15Jun\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/1006_ios/Warning/2.png\",\"isOnList\":true,\"showByActionType\":7,\"priority\":4,\"isShowGameView\":false},{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/1006_ios/HDN/b2.png\",\"urlLink\":\"https://www.facebook.com/bigwinclub.site\",\"pos\":[0.6,0.1]}],\"_id\":\"62c3a3fe24f9eb0018cf82f4\",\"id\":\"62a97e6408c8fb0011816629\",\"title\":\"Hướngdẫnnạp-1006-V0,1-15Jun\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/1006_ios/HDN/i2.png\",\"isOnList\":true,\"showByActionType\":7,\"priority\":7,\"isShowGameView\":false},{\"arrButton\":[{\"type\":\"openlink\",\"urlBtn\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/1006_ios/Laropay/3a.png\",\"urlLink\":\"https://laropay.net/\",\"pos\":[0.5,0.1]}],\"_id\":\"62c3a3fe24f9eb0018cf82f6\",\"id\":\"62a95c2308c8fb0011816602\",\"title\":\"Laropay-1006-V0,5-15Jun\",\"isClose\":true,\"urlImg\":\"https://storage.googleapis.com/cdn.davaogames.com/NewBanner/1006_ios/Laropay/3ai.png\",\"isOnList\":true,\"showByActionType\":7,\"priority\":8,\"isShowGameView\":false}]}";
    //     handleEvent(str);
    // }
    void emitSIO(string strData)
    {
        if (clientIO != null && connectionStatus == ConnectionStatus.CONNECTED)
        {
            Debug.Log("-=-=SML emitSIO  data: " + strData);
            if (!IsJSON(strData)) clientIO.Emit("event", strData);
            else clientIO.EmitStringAsJSON("event", strData);
        }
        else
        {
            //listResendEvent.Add(eventName);
            if (listResendData.Count < 100) listResendData.Add(strData);
        }
    }
    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                JToken obj = JToken.Parse(str);
                return true;
            }
            catch (Exception ex) //some other exception
            {
                Debug.LogError(ex.ToString());
                return false;
            }
        }
        else return false;
    }
    void emitSIOWithMapData(string evtName, Dictionary<string, string> mapData)
    {
        JObject objectVL = new();
        foreach (KeyValuePair<string, string> kvp in mapData)
        {
            //if (kvp.Key == "vip" || kvp.Key == "ag" || kvp.Key == "id")
            //{
            //    objectVL[kvp.Key] = int.Parse(kvp.Value);
            //}
            //else
            //{
            objectVL[kvp.Key] = kvp.Value;
            //}
        }
        //    mapData.forEach((valu, key) => {
        //        objectVL[key] = valu;
        //    });
        objectVL["event"] = evtName;
        objectVL["timestamp"] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        emitSIO(objectVL.ToString());
    }
    public void emitSIOWithValue(JObject objectVL, string namePackage, bool isSend)
    {
        ////packetDetail: evt nào có trong array này thì bắn đủ data (bắn lên "packetDetail")
        emitSIOWithValuePacket(objectVL, namePackage, isSend, true);
        ////packet: evt nào có trong array này thì bắn evt, isSend, timestamp.. (bắn lên "packet")
        emitSIOWithValuePacket(objectVL, namePackage, isSend, false);
    }
    public void emitSIOCCCNew(string strData)
    {
        try
        {
            if (blackListBehaviorIgnore.Contains(strData) || blackListBehaviorIgnore.Contains("all_sio"))
                return;
            Dictionary<string, string> mapDM = new() { { BEHAVIOR, strData } };
            emitSIOWithMapData(BEHAVIOR, mapDM);
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    void emitSIOWithValuePacket(JObject packetValue, string namePackage, bool isSend, bool isPacketDetai, long timeStamp = 0)
    {
        try
        {
            string timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            JObject objectVV = packetValue; //packetValue.slice();
            if (connectionStatus != ConnectionStatus.CONNECTED || !isGetedListFillter)
            {
                JObject objSave = new()
                {
                    ["strData"] = packetValue,
                    ["isSend"] = isSend,
                    ["isPacketDetai"] = isPacketDetai,
                    ["namePackage"] = namePackage,
                    ["timestamp"] = timestamp
                };
                listDataResendForPacket.Add(objSave);
                return;
            }
            string evtt = "";
            if (objectVV.ContainsKey("evt")) evtt = (string)objectVV["evt"];
            else if (objectVV.ContainsKey("idevt")) evtt = (string)objectVV["idevt"];
            else
            {
                evtt = namePackage;
                objectVV["evt"] = evtt;
            }
            if (isPacketDetai)
            {
                if (packetDetail.Contains(evtt) || packetDetail.Contains("all_sio"))
                {
                    objectVV["event"] = "packetDetail";
                    if ((string)packetValue["evt"] == "0") DATAEVT0 = packetValue;
                }
                else
                {
                    //cc.NGWlog("SIO: EVT NAY THUOC DIEN CHINH SACH KO DUOC GUI DI :( -  evt: " + evtt);
                    return;
                }
            }
            else
            {
                if (whiteListOnlySendEvt.Contains(evtt) || whiteListOnlySendEvt.Contains("all_sio"))
                {
                    objectVV = new JObject
                    {
                        ["evt"] = evtt,
                        ["event"] = "packet"
                    };
                }
                else
                {
                    //cc.NGWlog("SIO: =-=-=-=-==== CHIM CUT");
                    return;
                }
            }
            objectVV["packetData"] = namePackage;
            objectVV["isSendData"] = isSend;
            objectVV["timestamp"] = timeStamp == 0 ? DateTimeOffset.Now.ToUnixTimeMilliseconds() : timeStamp;
            emitSIO(objectVV.ToString());
        }
        catch (Exception e) { Debug.LogException(e); }
    }
    //Gui sau' khi connect success --> gui thong tin device
    void emitReginfo()
    {
        //try
        //{
        JObject objectVL = new()
        {
            ["event"] = REGINFO
        };
        //string osName = "web";
        string osName = "Android";
        if (Application.platform == RuntimePlatform.Android) osName = "Android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer) osName = "iOS";

        objectVL["location"] = "WHERE";
        objectVL["pkgname"] = Config.package_name;
        objectVL["versionCode"] = Config.versionGame;
        objectVL["versionName"] = Config.versionNameOS;
        objectVL["versionDevice"] = Config.versionDevice;
        objectVL["os"] = osName;
        objectVL["language"] = Config.language;
        objectVL["model"] = Config.model;
        objectVL["brand"] = Config.brand;

        //JArray jArray = new JArray();
        //jArray.Add(Screen.currentResolution.width);
        //jArray.Add(Screen.currentResolution.height);
        //objectVL["resolution"] = jArray;
        objectVL["time_start"] = Config.TimeOpenApp;
        objectVL["devID"] = Config.deviceId;
        objectVL["operatorID"] = Config.OPERATOR;
        emitSIO(objectVL.ToString());
        //}
        //catch (Exception e)
        //{

        //    Debug.LogException(e);
        //}
    }

    public void emitLogin()
    {
        //// isSendFirst = false;
        ////tracking io khi login success
        Dictionary<string, string> mapDataLogin = new()
        {
            { "event", LOGIN },
            { "gameIP", Config.curServerIp },
            { "verHotUpdate", Config.versionGame },
            { "id", User.userMain.Userid.ToString() },
            { "name", User.userMain.Username },
            { "ag", User.userMain.AG + "" },
            { "vip", User.userMain.VIP + "" },
            { "lq", User.userMain.LQ + "" },
            { "curView", CURRENT_VIEW.getCurrentSceneName() },
            { "gameID", Config.curGameId + "" },
            { "disID", Config.disID + "" }
        };
        emitSIOWithMapData(LOGIN, mapDataLogin);
    }
    public void emitUpdateInfo()
    {
        Dictionary<string, string> mapData = new()
        {
            { "id", User.userMain.Userid + "" },
            { "name", User.userMain.Username },
            { "ag", User.userMain.AG + "" },
            { "vip", User.userMain.VIP + "" },
            { "lq", User.userMain.LQ + "" },
            { "curView", CURRENT_VIEW.getCurrentSceneName() },
            { "gameID", Config.curGameId + "" }
        };
        emitSIOWithMapData(UPDATE, mapData);
    }
    public void logEventSuggestBanner(int type, JObject dataItem)
    {
        Dictionary<string, string> dataMap = new();
        if (type == 1) dataMap["action"] = "close";
        else if (type == 2) dataMap["action"] = "click";
        else if (type == 3) dataMap["action"] = "view";
        dataMap["id"] = (string)dataItem["id"];
        dataMap["urlImg"] = (string)dataItem["urlImg"];
        if (!arrayIDBannerShowed.Contains((string)dataItem["id"])) arrayIDBannerShowed.Add((string)dataItem["id"]);
        emitSIOWithMapData("actionBanner", dataMap);
        if (type == 2)
        {
            JArray arrayDataBannerIO = UIManager.instance.arrayDataBannerIO;
            for (int i = 0; i < arrayDataBannerIO.Count; i++)
            {
                if (dataItem["id"] == arrayDataBannerIO[i]["id"]) { }
                else
                {
                    if (arrayIDBannerShowed.Contains((string)arrayDataBannerIO[i]["id"])) continue;
                    Dictionary<string, string> dataNo = new()
                    {
                        { "action", "notshow" },
                        { "id", (string)arrayDataBannerIO[i]["id"] },
                        { "urlImg", (string)arrayDataBannerIO[i]["urlImg"] }
                    };
                    emitSIOWithMapData("actionBanner", dataNo);
                }
            }
        }
    }
}