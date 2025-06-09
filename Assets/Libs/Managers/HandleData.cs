using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Globals;

public class HandleData
{
    public static float DelayHandleLeave = 0;
    public static void handleLoginResponse(string strData)
    {
        Logging.LogWarning("-=- =handleLoginResponse:  " + strData);
        //LoginResponsePacket packet = JsonConvert.DeserializeObject<LoginResponsePacket>(strData);
        LoginResponsePacket packet = JsonUtility.FromJson<LoginResponsePacket>(strData);

        if (packet.status == CMD.OK)
        {
            if (Config.typeLogin == LOGIN_TYPE.NORMAL) Config.setDataUser();
            else if (Config.typeLogin == LOGIN_TYPE.PLAYNOW)
            {
                PlayerPrefs.SetString("USER_PLAYNOW", UIManager.instance.loginView.accPlayNow);
                PlayerPrefs.SetString("PASS_PLAYNOW", UIManager.instance.loginView.passPlayNow);
                PlayerPrefs.Save();
            }

            PlayerPrefs.SetInt("type_login", (int)Config.typeLogin);

            string data = Config.Base64Decode(packet.credentials);
            Logging.LogWarning("-=- =dang nhap thanh cong:  " + data);
            //data = data.Replace("\"VIP\":2", "\"VIP\":0");
            //Logging.LogWarning("-=- =dang nhap thanh cong2:  " + data);
            JObject obj = JObject.Parse(data);
            string strUser = (string)obj["data"];
            Logging.LogWarning(strUser);
            //User.userMain = JsonUtility.FromJson<User>(strUser);
            //{ "evt":"0","data":"{\"Userid\":5311,\"Username\":\"playforfun\",\"Tinyurl\":\"\",\"AG\":321194,\"LQ\":0,\"VIP\":1,\"MVip\":0,\"markLevel\":0,\"PD\":0,\"OD\":16,\"A\":26,\"NM\":0,\"ListDP\":\"500;500;500_0;100;100_0;100;100_0;\",\"NewAccFBInDevice\":0,\"chipbank\":0,\"gameid\":0,\"NumFriendMail\":0,\"gameNo\":0,\"Diamond\":0,\"vippoint\":1,\"vippointMax\":10,\"FacebookName\":\"\",\"displayName\":\"playforfun\",\"LQ0\":0.0,\"CO\":0.0,\"CO0\":0.0,\"LQSMS\":0.0,\"LQIAP\":0.0,\"LQOther\":0.0,\"BLQ1\":0.0,\"BLQ3\":0.0,\"BLQ5\":0.0,\"BLQ7\":0.0,\"AVG7\":0.0,\"Group\":0.0,\"CreateTime\":1620630212017,\"keyObjectInGame\":0,\"idChatContents\":[1,3,13,17,20,24,26,40,43,47],\"UsernameLQ\":\"\"}","time":1636367771387,"auth":""}
            JObject objUser = JObject.Parse(strUser);

            User.userMain = new User();
            User.userMain.Userid = (int)objUser["Userid"];
            User.userMain.Username = (string)objUser["Username"];
            User.userMain.Tinyurl = (string)objUser["Tinyurl"];
            User.userMain.AG = (long)objUser["AG"];
            User.userMain.LQ = (long)objUser["LQ"];
            User.userMain.VIP = (int)objUser["VIP"];
            User.userMain.MVip = (int)objUser["MVip"];
            User.userMain.markLevel = (int)objUser["markLevel"];
            User.userMain.PD = (int)objUser["PD"];
            User.userMain.OD = (int)objUser["OD"];
            User.userMain.Avatar = (int)objUser["A"];
            User.userMain.NM = (int)objUser["NM"] % 100;
            User.userMain.nmAg = (long)objUser["NM"] / 100;
            User.userMain.ListDP = (string)objUser["ListDP"];
            if (objUser["NewAccFBInDevice"] != null)
                User.userMain.NewAccFBInDevice = (int)objUser["NewAccFBInDevice"];

            if (objUser.ContainsKey("lastPlay"))
            {
                Config.lastGameIDSave = (int)objUser["lastPlay"];
                UIManager.instance.lobbyView.setQuickPlayGame(Config.lastGameIDSave);
            }
            User.userMain.agSafe = (long)objUser["chipbank"];
            User.userMain.NumFriendMail = (int)objUser["NumFriendMail"];
            User.userMain.gameNo = (int)objUser["gameNo"];
            if (objUser["Diamond"] != null)
                User.userMain.Diamond = (int)objUser["Diamond"];

            User.userMain.vippoint = (int)objUser["vippoint"];
            User.userMain.vippointMax = (int)objUser["vippointMax"];
            User.userMain.FacebookName = (string)objUser["FacebookName"];
            User.userMain.displayName = (string)objUser["displayName"];
            User.userMain.LQ0 = (float)objUser["LQ0"];
            User.userMain.CO = (float)objUser["CO"];
            User.userMain.CO0 = (float)objUser["CO0"];
            User.userMain.LQSMS = (float)objUser["LQSMS"];
            User.userMain.LQIAP = (float)objUser["LQIAP"];
            User.userMain.LQOther = (float)objUser["LQOther"];
            User.userMain.BLQ1 = (float)objUser["BLQ1"];
            User.userMain.BLQ3 = (float)objUser["BLQ3"];
            User.userMain.BLQ5 = (float)objUser["BLQ5"];
            User.userMain.BLQ7 = (float)objUser["BLQ7"];
            User.userMain.AVG7 = (float)objUser["AVG7"];
            User.userMain.Group = (float)objUser["Group"];
            User.userMain.CreateTime = (long)objUser["CreateTime"];
            if (objUser["keyObjectInGame"] != null)
                User.userMain.keyObjectInGame = (int)objUser["keyObjectInGame"];

            User.userMain.UsernameLQ = (string)objUser["UsernameLQ"];
            User.userMain.isShowMailAg = true;
            if (objUser["uidInvite"] != null)
                User.userMain.uidInvite = (int)objUser["uidInvite"];
            if (objUser["canInputInvite"] != null)
                User.userMain.canInputInvite = (bool)objUser["canInputInvite"];
            if (objUser["timeInputInvite"] != null)
                User.userMain.timeInputInvite = (int)objUser["timeInputInvite"];

            User.userMain.timeInputInviteRemain = DateTimeOffset.Now.ToUnixTimeMilliseconds() + User.userMain.timeInputInvite * 1000;
            User.userMain.lastGameID = (int)objUser["gameid"];

            PlayerPrefs.SetInt("isFirstOpen", 1);
            PlayerPrefs.Save();

            //SocketIOManager.getInstance().startSIO();
            SocketIOManager.getInstance().isSendFirst = true;
            Config.isLoginSuccess = true;
            JObject objLogin = new JObject();
            objLogin["evt"] = "0";
            objLogin["data"] = obj.ToString(Formatting.None);

            SocketIOManager.getInstance().DATAEVT0 = objLogin;
            SocketIOManager.getInstance().emitLogin();
            SocketIOManager.getInstance().emitSIOWithValue(objLogin, "LoginPacket", false);

            if (Config.curGameId == 0) Config.curGameId = (int)objUser["gameid"];
            // LoadConfig.instance.getConfigInfo();
            // LoadConfig.instance.isLoadedConfig = false;
            LoadConfig.instance.getInfoUser(strUser);

            if (Config.typeLogin == LOGIN_TYPE.NORMAL) Config.saveLoginAccount();
            //Logging.Log("emit update info o day nua");
            SocketIOManager.getInstance().emitUpdateInfo();
            Dictionary<string, object> tags = new Dictionary<string, object>();
            if (UIManager.instance.gameView == null) UIManager.instance.showLobbyScreen(true);

            tags.Add("deviceid", Config.deviceId);
            tags.Add("publisher", Config.publisher);
            tags.Add("username", User.userMain.Username);
            tags.Add("displayName", User.userMain.displayName);
            tags.Add("uservip", User.userMain.VIP.ToString());
            tags.Add("userid", User.userMain.Userid.ToString());
            tags.Add("usergold", User.userMain.AG.ToString());
            tags.Add("gameport", Config.OPERATOR.ToString());
            tags.Add("pkgname", Config.package_name);
            tags.Add("version", Config.versionGame);
            tags.Add("userlq", User.userMain.LQ.ToString());
            tags.Add("co", User.userMain.CO.ToString());
            tags.Add("co0", User.userMain.CO0.ToString());
            tags.Add("lqsms", User.userMain.LQSMS.ToString());
            tags.Add("lqiap", User.userMain.LQIAP.ToString());
            tags.Add("lqother", User.userMain.LQOther.ToString());
            tags.Add("blq1", User.userMain.BLQ1.ToString());
            tags.Add("blq3", User.userMain.BLQ3.ToString());
            tags.Add("blq5", User.userMain.BLQ5.ToString());
            tags.Add("blq7", User.userMain.BLQ7.ToString());
            tags.Add("avg7", User.userMain.AVG7.ToString());
            tags.Add("group", User.userMain.Group.ToString());
            OneSignalSDK.OneSignal.Default.SendTags(tags);

            new Thread(new ThreadStart(() =>
            {
                SocketSend.sendSelectG2(Config.curGameId);
                Thread.Sleep(100);
                SocketSend.getInfoSafe();
                Thread.Sleep(100);
                SocketSend.sendPromotion();
                Thread.Sleep(100);
                SocketSend.getMessList();
                Thread.Sleep(100);
                SocketSend.getMail(10);
                Thread.Sleep(100);
                SocketSend.getMail(12);
                // Thread.Sleep(100);
                // SocketSend.getChatWorld();
                // Thread.Sleep(100);
                // SocketSend.getFarmInfo();
                Thread.Sleep(100);
                SocketSend.sendRef();
            })).Start();
        }
        else
        {
            Logging.Log(packet.message);

            UIManager.instance.showMessageBox(packet.message);
            //{ "screenname":null,"pid":0,"status":"DENIED","code":-3,"message":"Username and Password do not match!","credentials":"","classId":11}

            var objData = new JObject();
            objData["codeError"] = packet.code;
            objData["MsgError"] = packet.message;
            SocketIOManager.getInstance().emitSIOWithValue(objData, "LoginPacket", false);
        }
    }


    public static void handleServiceTransportPacket(string strData)
    {
        ServiceTransportPacket packet = JsonUtility.FromJson<ServiceTransportPacket>(strData);
        packet.str_servicedata = (string)JObject.Parse(strData)["servicedata"];
        string data = Config.Base64Decode(packet.str_servicedata);
        HandleService.processData(JObject.Parse(data));
    }

    public static void handleGameTransportPacket(string strData)
    {
        //Logging.Log("handleGameTransportPacket   " + strData);
        GameTransportPacket packet = JsonUtility.FromJson<GameTransportPacket>(strData);
        packet.str_gamedata = (string)JObject.Parse(strData)["gamedata"];
        string data = Config.Base64Decode(packet.str_gamedata);
        HandleGame.processData(JObject.Parse(data));
    }
    public static void handleForcedLogoutPacket(string strData)
    {
        JObject data = JObject.Parse(strData);
        string message = (string)data["message"];

    }
    public static void handleJoinResponsePacket(string strData)
    {
        //{ "tableid":14,"seat":0,"status":"OK","code":0,"classId":31}
        Debug.Log("handleJoinResponsePacket:" + strData);
        JObject data = JObject.Parse(strData);
        //string message = (string)data["message"];

        if ((string)data["status"] == "OK")
        {

            //    NetworkManager.getInstance().listEvtGame.length = 0;
            //    cc.NGWlog('========================== packet OnShowGame');

            //    let dataJson = { };
            //    dataJson.tableid = packet.tableid;
            //    dataJson.curGameID = Config.curGameId;
            //    require('SMLSocketIO').getInstance().emitSIOWithValue(dataJson, "JoinPacket", false);
            //    require("UIManager").instance.onShowGame();
            Config.tableId = (int)data["tableid"];
            Debug.Log("tableId2=" + Config.tableId);
            JObject dataJson = new JObject();
            dataJson["tableid"] = Config.tableId;
            dataJson["curGameID"] = Config.curGameId;
            SocketIOManager.getInstance().emitSIOWithValue(dataJson, "JoinPacket", false);
            UIManager.instance.showGame();
        }
        else
        {
            //    cc.NGWlog('hide load ben vao` ban` ko thanh cong====')
            //        require('UIManager').instance.onHideLoad();
            //    var _str = Config.getTextConfig(
            //        "show_join_error"
            //    );
            string _str = "";
            switch ((int)data["code"])
            {
                case -4:
                    _str = "";
                    break;
                case -5:
                    _str = Config.getTextConfig(
                        "err_table_another_table"
                    );
                    break;
                case -6:
                    _str = Config.getTextConfig("err_table_full");
                    break;
                case -7:
                    // _str = Config.getTextConfig("err_table_vip");
                    break;
                case -8:
                    _str = Config.getTextConfig("txt_not_enough_money_gl");
                    break;
            }

            //    let dataJson = { };
            //    dataJson.codeError = packet.code;
            //    dataJson.msgError = _str;
            //    require('SMLSocketIO').getInstance().emitSIOWithValue(dataJson, "JoinPacket", false);

            JObject dataJson = new JObject();
            dataJson["codeError"] = data["code"];
            dataJson["msgError"] = _str;
            //if (Config.curGameId != (int)GAMEID.BANDAR_QQ && Config.curGameId == (int)GAMEID.RONGHO) //din case baner het tien join ban van hien thi o game playnow
            //{
            SocketIOManager.getInstance().emitSIOWithValue(dataJson, "JoinPacket", false);
            //}
            if (_str != "")
                UIManager.instance.showMessageBox(_str);
        }
    }

    public static async void handleLeaveResponsePacket(string strData)
    {
        Logging.Log("handleLeaveResponsePacket  " + strData);
        Logging.Log("-=-= " + Config.curGameId);
        if (DelayHandleLeave > 0)
        {
            await Awaitable.WaitForSecondsAsync(DelayHandleLeave);
            DelayHandleLeave = 0f;
        }
        JObject packet = JObject.Parse(strData);
        //string message = (string)data["message"];
        if ((string)packet["status"] == "OK")
        {
            if (UIManager.instance.gameView != null)
            {
                if (!Config.listGameSlot.Contains(Config.curGameId))
                {
                    HandleGame.handleLeave();
                    return;
                }
                if (UIManager.instance.gameView.dataLeave != null)
                {
                    SocketIOManager.getInstance().emitSIOWithValue(UIManager.instance.gameView.dataLeave, "LeavePacket", false);

                }
                UIManager.instance.gameView.dataLeave = null;
                UIManager.instance.gameView.destroyThis();
                UIManager.instance.gameView = null;

            }

            //if (TableView.instance
            //    && Config.curGameId != (int)GAMEID.SLOTNOEL
            //    && Config.curGameId != (int)GAMEID.SLOTTARZAN)
            if (Config.isShowTableWithGameId(Config.curGameId) && User.userMain.VIP >= 1)
            {
                UIManager.instance.openTableView();
            }
            else
            {
                UIManager.instance.showLobbyScreen();
            }
        }
        else
        {
            JObject dataJson = new JObject();
            dataJson["codeError"] = packet["code"];
            dataJson["msgError"] = packet["status"];
            SocketIOManager.getInstance().emitSIOWithValue(dataJson, "LeavePacket", false);
        }
    }
}
