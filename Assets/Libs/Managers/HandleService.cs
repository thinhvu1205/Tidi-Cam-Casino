using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Globals;
using Socket.Quobject.EngineIoClientDotNet.Modules;
using System;

public class HandleService
{
    public static void processData(JObject jsonData)
    {

        SocketIOManager.getInstance().emitSIOWithValue(jsonData, "ServiceTransportPacket", false);
        if (jsonData.ContainsKey("evt"))
        {
            string evt = (string)jsonData["evt"];
            Globals.Logging.Log("--------------------------------------------------->EVT: " + evt + " <------------------------------------------->\n" + jsonData);
            switch (evt)
            {
                case "promotion_info":
                    //// "evt":"promotion_info","P":0,"A":0,"UV":0,"O":80,"V":0,"C":0,"T":0,"VC":0,"VM":3,"OC":0,"OM":6,"NV":3000,"NO":80,"NIV":10000,"InviteMark":500,"InviteNum":40,"OnlinePolicy":"{\"numberP\":6,\"timeWaiting\":[60,60,60,60,60,60],\"chipBonus\":[80,80,160,80,80,80]}"
                    //cc.NGWlog("jsondata.p: " + jsonData.P);
                    ////cc.NGWlog("GM.pro: ", GameManager.getInstance().promotionInfo);
                    //GameManager.getInstance().setPromotionInfo(jsonData);
                    //Global.MainView.lbTimeOnline.node.stopAllActions();
                    //Global.MainView.setTimeGetMoney();
                    //
                    return;
                    Globals.Promotion.setPromotionInfo(jsonData);
                    UIManager.instance.updateMailAndMessageNoti();
                    UIManager.instance.setTimeOnline();
                    if (DailyBonusView.instance != null)
                    {
                        DailyBonusView.instance.setInfo();
                    }
                    if (FreeChipView.instance != null)
                    {
                        FreeChipView.instance.loadFreeChip();
                    }
                    break;
                case "promotion_online":

                    break;
                case "reconnect":
                    Globals.User.userMain.lastGameID = (int)jsonData["gameid"];
                    Globals.Config.curGameId = (int)jsonData["gameid"];
                    break;
                case "promotion":
                    break;
                case "addInviteFriendID":
                    bool isSuccess = (bool)jsonData["isSuccess"];
                    UIManager.instance.hideWatting();
                    if (isSuccess)
                    {
                        Globals.User.userMain.canInputInvite = false;
                        UIManager.instance.updateCanInviteFriend();
                        UIManager.instance.showToast((string)jsonData["data"]);
                        if (ProfileView.instance)
                            ProfileView.instance.onAddFriendIdRefSuccess();
                    }
                    else
                    {
                        UIManager.instance.showToast((string)jsonData["data"]);
                    }
                    break;
                case "giftDetail":
                    if (ExchangeView.instance != null) ExchangeView.instance.HandleUpdateHistory(jsonData);
                    break;
                case "historyGiftfDetail":
                    if (ExchangeView.instance != null) ExchangeView.instance.HandleGiftHistory(jsonData);
                    break;
                case "dp":
                    {
                        var agDp = (long)jsonData["AG"];
                        SocketIOManager.getInstance().emitUpdateInfo();
                        break;
                    }
                case "GiftCode":
                    {
                        int gold = (int)jsonData["G"];
                        string msg = (string)jsonData["Msg"];
                        if (gold > 0)
                        {
                            Globals.User.userMain.AG += gold;
                            System.Action cb1 = () =>
                            {
                                UIManager.instance.lobbyView.onClickFreechip();
                                if (GiftCodeView.instance != null && GiftCodeView.instance.gameObject.activeSelf)
                                {
                                    GiftCodeView.instance.onClickClose();
                                }
                            };
                            SocketSend.sendPromotion();
                            UIManager.instance.showDialog(msg, Globals.Config.getTextConfig("txt_free_chip"), cb1, Globals.Config.getTextConfig("ok"));
                        }
                        else
                        {
                            UIManager.instance.showMessageBox(msg);
                        }

                        break;
                    }

                case "toprich":

                    break;
                case "followlist":
                    //if (jsonData.data !== "")
                    //{
                    //    var dat = JSON.parse(jsonData.data);
                    //    Global.FriendPopView.receviceData(dat);
                    //}
                    //else
                    //{
                    //    Global.FriendPopView._listFriends.length = [];
                    //}
                    break;
                case "follow":
                    //    if (!jsonData.data) return;
                    //    require('GameManager').getInstance().onShowToast(require('GameManager').getInstance().getTextConfig('friend_add_success'));
                    break;
                case "10":
                    {
                        if (jsonData.ContainsKey("data"))
                        {
                            UIManager.instance.showMessageBox((string)jsonData["data"]);
                        }
                        else
                        {
                            //UIManager.instance.showMessageBox((string)jsonData["Cmd"]);
                            UIManager.instance.showDialog((string)jsonData["Cmd"], Globals.Config.getTextConfig("ok"), () =>
                            {
                                //if (Globals.Config.curGameId == (int)Globals.GAMEID.SLOT50LINE ||
                                //        Globals.Config.curGameId == (int)Globals.GAMEID.SLOT20FRUIT)
                                //{
                                //    GameManager.getInstance().onReconnect();
                                //}
                            });
                        }
                        break;
                    }
                case "changea":
                    //var status = jsonData.error;
                    //if (status == 0)
                    //{
                    //    if (Global.ProfileView.node.getParent() !== null)
                    //        Global.ProfileView.setAvatar();
                    //    Global.MainView.setAvatar();
                    //    // GameManager.getInstance().onShowConfirmDialog(GameManager.getInstance().getTextConfig('success_change_ava'), null, true, 2);
                    //}
                    int status = (int)jsonData["error"];
                    Globals.Logging.Log("status avatar:" + status);

                    if (status == 0)
                    {
                        UIManager.instance.updateAvatar();
                    }
                    break;
                case "promotion_online_2":
                    //if (!jsonData.result) break;

                    //                    {
                    //                        "evt": "promotion_online_2",
                    //  "result": "{\"result\":26,\"vip\":2,\"turn\":1,\"baseAg\":1000,\"vipBonus\":20,\"turnBonus\":10,\"AG\":1300}"
                    //}
                    if (!jsonData.ContainsKey("result") || ((string)jsonData["result"]).Equals("")) break;
                    ////{"evt":"promotion_online_2","result":"0;0"}
                    ////{"evt":"promotion_online_2","result":"{\"result\":25,\"vip\":1,\"turn\":0,\"baseAg\":1800,\"vipBonus\":10,\"turnBonus\":0,\"AG\":1980}"}
                    //Global.ChipOnline.onStopSpin(jsonData.result);

                    if (DailyBonusView.instance)
                    {

                        JObject result = JObject.Parse((string)jsonData["result"]);
                        DailyBonusView.instance.handleResult(result);
                    }

                    break;
                case "promotion_daily_2":

                    if (DailyBonusView.instance != null && DailyBonusView.instance.gameObject.activeSelf)
                    {
                        DailyBonusView.instance.handleResult(JObject.Parse((string)jsonData["result"]));
                    }
                    break;
                case "20":


                    Globals.User.userMain.listMailAdmin.Clear();
                    JArray listMail = JArray.Parse((string)jsonData["data"]);
                    for (int i = 0, l = listMail.Count; i < l; i++)
                    {
                        JObject data = new JObject((JObject)listMail[i]);
                        data["S"] = (bool)data["S"] ? 1 : 0;
                        Globals.User.userMain.listMailAdmin.Add(data);
                    }
                    Globals.User.userMain.listMailAdmin.Sort((a, b) =>
                    {
                        if ((int)a["S"] != (int)b["S"])
                        {
                            return (int)a["S"] - (int)b["S"];
                        }
                        else
                        {
                            if ((long)a["Time"] > (long)b["Time"]) return -1;
                            else return 1;
                        }
                    });
                    Globals.User.userMain.mailUnRead = Globals.User.userMain.listMailAdmin.FindAll(data => (int)data["S"] == 0).Count;
                    if (MailView.instance != null && MailView.instance.gameObject.activeSelf)
                    {
                        MailView.instance.listMailData = Globals.User.userMain.listMailAdmin;
                        MailView.instance.reloadListMail();
                    }

                    UIManager.instance.updateMailAndMessageNoti();
                    break;
                case "22":

                    if (FreeChipView.instance != null && FreeChipView.instance.gameObject.activeSelf)
                    {
                        FreeChipView.instance.dataFreeChipAdmin.Clear();
                    }
                    JArray listFreeChip = JArray.Parse((string)jsonData["data"]);
                    foreach (JObject data in listFreeChip)
                    {
                        JObject item = new JObject();
                        if (data["DT"] != null)
                        {
                            item["moneyType"] = data["DT"];
                            item["idMsg"] = data["Id"];
                            item["t"] = data["T"];
                            item["vip"] = data["Vip"];
                            item["from"] = data["From"];
                            item["to"] = data["To"];
                            item["gold"] = data["AG"];
                            item["i"] = data["I"];
                            item["msg"] = data["Msg"];
                            item["time"] = data["Time"];
                            item["s"] = (bool)data["S"] ? 0 : 1;
                            item["d"] = data["D"];
                            if (((string)item["from"]).ToLower() == "admin")
                            {
                                Globals.Config.mail20.Add(item);
                            }
                            ;
                            if (FreeChipView.instance != null && FreeChipView.instance.gameObject.activeSelf)
                            {
                                FreeChipView.instance.pushMailAdmin(7, (string)item["msg"], (int)item["gold"], (int)item["idMsg"]);
                            }
                        }
                    }

                    Globals.User.userMain.nmAg = listFreeChip.Count;

                    UIManager.instance.updateMailAndMessageNoti();
                    if (FreeChipView.instance != null && FreeChipView.instance.gameObject.activeSelf)
                    {
                        FreeChipView.instance.loadFreeChip();
                    }
                    else
                    {
                        UIManager.instance.checkAlertMail(true);
                    }
                    break;

                case "messagelist":
                    if (jsonData.ContainsKey("data"))
                    {
                        List<JObject> listMes = new List<JObject>();
                        JArray listFriend = new JArray();
                        if ((string)jsonData["data"] != "")
                        {
                            listMes = JArray.Parse((string)jsonData["data"]).ToObject<List<JObject>>();
                        }
                        Globals.User.userMain.messageUnRead = listMes.FindAll(data => (int)data["count"] != 0).Count;
                        if (ChatPrivateView.instance != null && ChatPrivateView.instance.gameObject.activeSelf)
                        {
                            if ((string)jsonData["data"] != "")
                            {
                                listFriend = JArray.Parse((string)jsonData["data"]);
                            }
                            ChatPrivateView.instance.initListFriend(listFriend);

                        }
                        UIManager.instance.updateMailAndMessageNoti();
                    }

                    break;
                case "followfind":
                    {
                        //require("UIManager").instance.onHideLoad();
                        //if (!jsonData.data) return;
                        //var status = jsonData.status;
                        //if (status == true)
                        //{
                        //    Global.FriendProfilePop.recivceData(jsonData);
                        //}
                        //else
                        //{
                        //    GameManager.getInstance().onShowConfirmDialog(jsonData.data);
                        //}
                        //                    {
                        //                        "evt": "followfind",
                        //  "data": "เพื่อนไม่มีอยู่จริง โปรดลองอีกครั้ง!",
                        //  "status": false
                        //}

                        bool _status = (bool)jsonData["status"];
                        if (_status)
                        {
                            JObject dataUser = JObject.Parse((string)jsonData["data"]);

                            if (FriendInfoView.instance != null && FriendInfoView.instance.gameObject.activeSelf)
                            {
                                FriendInfoView.instance.setInfo(dataUser);
                            }
                            else
                            {
                                UIManager.instance.openFriendInfo();
                                FriendInfoView.instance.setInfo(dataUser);
                            }
                        }
                        else
                        {
                            UIManager.instance.showMessageBox((string)jsonData["data"]);
                        }

                        break;
                    }
                case "messagedetail":

                    if ((string)jsonData["data"] != "")
                    {
                        if (ChatPrivateView.instance != null && ChatPrivateView.instance.gameObject.activeSelf)
                        {
                            JObject data = JObject.Parse((string)jsonData["data"]);
                            JArray listMessage = (JArray)data["lsMessage"];

                            ChatPrivateView.instance.loadListMessage(listMessage);
                        }
                    }
                    else
                    {
                        //{
                        //    "evt": "messagelist",
                        //"data": "[{\"fromid\":388638,\"toid\":526342,\"count\":0,\"fromname\":\"tictctoe123\",\"toname\":\"TBK.526342\",\"vip\":2,\"avatar\":1,\"title\":\"hahaa\",\"id\":372754,\"msgtime\":1671598633330,\"fid\":0,\"from_avatar\":0,\"to_avatar\":1},{\"fromid\":388638,\"toid\":527111,\"count\":0,\"fromname\":\"tictctoe123\",\"toname\":\"TBK.527111\",\"vip\":2,\"avatar\":1,\"title\":\"hi\",\"id\":372753,\"msgtime\":1671598554320,\"fid\":0,\"from_avatar\":0,\"to_avatar\":1},{\"fromid\":388638,\"toid\":23044,\"count\":0,\"fromname\":\"tictctoe123\",\"toname\":\"Saranya Temdee\",\"vip\":4,\"avatar\":999,\"title\":\"helo\",\"id\":372747,\"msgtime\":1671598380640,\"fid\":2024880997679987,\"from_avatar\":0,\"to_avatar\":999}]"
                        //}
                        if (ChatPrivateView.instance != null && ChatPrivateView.instance.gameObject.activeSelf)
                            ChatPrivateView.instance.loadListMessage(new JArray());
                    }
                    break;
                case "message":
                    {
                        if (!((string)jsonData["data"]).Equals(""))
                        {
                            JObject dataMess = JObject.Parse((string)jsonData["data"]);
                            JObject messageData = new JObject();
                            messageData["Vip"] = dataMess["vip"];
                            messageData["Name"] = dataMess["fromname"];
                            messageData["Avatar"] = dataMess["avatar"];
                            messageData["Data"] = dataMess["msg"];
                            messageData["FaceID"] = dataMess["fid"];
                            messageData["time"] = dataMess["timemsg"];
                            messageData["ID"] = dataMess["fromid"];
                            if (ChatPrivateView.instance != null && ChatPrivateView.instance.gameObject.activeSelf)
                            {
                                ChatPrivateView.instance.addMessage(messageData);
                            }
                            else if (UIManager.instance.lobbyView.gameObject.activeSelf && UIManager.instance.gameView == null)
                            {
                                UIManager.instance.setNotiMessage(true);
                                UIManager.instance.showDialog(Globals.Config.getTextConfig("has_mail"), Globals.Config.getTextConfig("txt_ok"), () =>
                                {
                                    if (TableView.instance != null) TableView.instance.onClickClose();
                                    UIManager.instance.destroyAllPopup();
                                    // UIManager.instance.lobbyView.onShowChatWorld(true);
                                }, Globals.Config.getTextConfig("label_cancel"));
                            }
                            else
                            {
                                UIManager.instance.showToast(Globals.Config.getTextConfig("has_mail"));
                                UIManager.instance.setNotiMessage(true);
                            }
                        }

                        break;
                    }

                case "ltv":
                    {
                        Debug.Log("ltv: ");
                        if (jsonData["data"] != null)
                        {
                            UIManager.instance.lobbyView.isClicked = false;
                            JArray listLtv = JArray.Parse((string)jsonData["data"]);
                            if (listLtv.Count <= 0) return;
                            if (TableView.instance != null)
                            {
                                TableView.instance.listDataRoomBet = listLtv;
                                TableView.instance.handleLtv(listLtv);
                            }
                        }
                        //                    {
                        //                        "evt": "ltv",
                        //  "data": "[{\"mark\":100,\"ag\":2000,\"agPn\":0,\"agD\":0,\"minAgCon\":2000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":1500,\"agRaiseFee\":2000,\"fee\":1.5},{\"mark\":1000,\"ag\":20000,\"agPn\":0,\"agD\":0,\"minAgCon\":20000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":15000,\"agRaiseFee\":20000,\"fee\":1.5},{\"mark\":5000,\"ag\":100000,\"agPn\":0,\"agD\":0,\"minAgCon\":100000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":75000,\"agRaiseFee\":100000,\"fee\":1.5},{\"mark\":10000,\"ag\":200000,\"agPn\":0,\"agD\":0,\"minAgCon\":200000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":150000,\"agRaiseFee\":150000,\"fee\":1.5},{\"mark\":50000,\"ag\":1000000,\"agPn\":0,\"agD\":0,\"minAgCon\":1000000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":750000,\"agRaiseFee\":1000000,\"fee\":1.5},{\"mark\":100000,\"ag\":2000000,\"agPn\":0,\"agD\":0,\"minAgCon\":2000000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":1500000,\"agRaiseFee\":2000000,\"fee\":1.5},{\"mark\":200000,\"ag\":4000000,\"agPn\":0,\"agD\":0,\"minAgCon\":4000000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":3000000,\"agRaiseFee\":3000000,\"fee\":1.5},{\"mark\":500000,\"ag\":10000000,\"agPn\":0,\"agD\":0,\"minAgCon\":10000000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":7500000,\"agRaiseFee\":10000000,\"fee\":1.5},{\"mark\":1000000,\"ag\":20000000,\"agPn\":0,\"agD\":0,\"minAgCon\":20000000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":15000000,\"agRaiseFee\":20000000,\"fee\":1.5}]"
                        //}

                        //cc.NGWlog("--------------------------------------------------------------->ltv");
                        //// setTimeout(() => {
                        ////     require('GameManager').getInstance().is_ltv = true;
                        ////     require('UIManager').instance.onHideLoadLtv();
                        //// },100);
                        //setTimeout(() => {
                        //    require('GameManager').getInstance().is_ltv = true;
                        //}, 1000)

                        //    if (!jsonData.data) return;
                        //cc.NGWlog("--------------------------------------------------------------->onShowLobby 2");
                        //Global.LobbyView.recivceData(jsonData);
                        break;
                    }
                case "roomVip":
                    //cc.NGWlog("---------------------------------------------> hanlde data roomVip");
                    //if (!jsonData.data) return;
                    //cc.NGWlog("--------------------------------------------------------------->onShowLobby");
                    if (TableView.instance)
                        TableView.instance.handleListTable(jsonData);
                    break;

                case "roomTable":
                    //cc.NGWlog("---------------------------------------------> hanlde data roomVip new");
                    //if (!jsonData.data) return;
                    //cc.NGWlog("--------------------------------------------------------------->onShowLobby");
                    //Global.LobbyView.recivceDataRoomVip(jsonData);
                    if (TableView.instance)
                        TableView.instance.handleListTable(jsonData);
                    break;

                case "checkPass":
                    ////{"evt":"checkPass","checked":true,"tid":2619}
                    ////dar lkdfdf = jsonData.heekd;
                    var isChecked = (bool)jsonData["checked"];
                    //var idCheck = jsonData.tid;
                    if (isChecked)
                    {
                        UIManager.instance.openInputPass((int)jsonData["tid"]);
                        //    Global.LobbyView.onShowCheckPass();
                        //    GameManager.getInstance().checkPassId = idCheck;
                    }


                    break;

                case "getChatWorld":
                    {
                        //// get ListChat
                        ///
                        if (MainChatWorld.instance)
                        {
                            MainChatWorld.instance.setInfo(jsonData);
                        }

                        Globals.COMMON_DATA.ListChatWorld = JArray.Parse((string)jsonData["data"]);
                        break;
                    }

                case "getChatGame":
                    //// get data chat game
                    //Global.LobbyView.receiveDataChatGame(jsonData)

                    break;
                case "16": //chat world message
                    {

                        //                        "evt": "16",
                        //  "T": 1,
                        //  "N": "TBK.245999",
                        //  "D": "ขายไม่ถึง10คงไม่น่าแบนหลอกมั้ง",
                        //  "V": 2,
                        //  "Avatar": 3,
                        //  "Ag": 867490,
                        //  "ID": 245999,
                        //  "fbid": 0,
                        //  "level": 0,
                        //  "time": 1646809196447
                        //}
                        JObject messageData = new JObject();
                        messageData["Vip"] = jsonData["V"];
                        messageData["ID"] = jsonData["ID"];
                        messageData["Name"] = jsonData["N"];
                        messageData["Avatar"] = jsonData["Avatar"];
                        messageData["Data"] = jsonData["D"];
                        messageData["FaceID"] = jsonData["fbid"];
                        messageData["time"] = jsonData["time"];
                        messageData["level"] = jsonData["level"];
                        messageData["Ag"] = jsonData["Ag"];
                        Globals.COMMON_DATA.ListChatWorld.Add(messageData);
                        if (ChatWorldView.instance != null)
                        {
                            ChatWorldView.instance.addMessage(jsonData);
                        }
                        break;
                    }

                case "ivp":
                    Globals.Logging.Log("-=-= IVP laf loiwf mowif");
                    if (UIManager.instance.gameView != null)
                    {
                        if (InviteView.instance != null)
                        {
                            Globals.Logging.Log("Invite data:" + jsonData.ToString());
                            InviteView.instance.receiveData(jsonData);
                        }
                    }
                    else if (TableView.instance != null && Globals.Config.invitePlayGame)
                    {
                        TableView.instance.showInvite(jsonData);
                    }

                    break;
                case "topgamer_new":
                    {
                        JArray dataListTop = (JArray)jsonData["list"];
                        if (LeaderBoardView.instance != null && LeaderBoardView.instance.gameObject.activeSelf)
                        {
                            if (LeaderBoardView.instance.timeRemain == -1)
                            {
                                JObject dataLast = (JObject)dataListTop[dataListTop.Count - 1];
                                LeaderBoardView.instance.setTimeRemain((long)dataLast["timeEnd"]);
                            }
                            LeaderBoardView.instance.reloadList(dataListTop);
                        }
                        break;
                    }
                case "salert":
                    {

                        // jsonData= JObject.Parse("{\"evt\":\"salert\",\"data\":\"ขอแสดงความยินดี! ตุณ ตา'ไนท์ท' ข้าว หลาม'ซิ่ง'ง แลกรางวัลสำเร็จ 5000.0 บาท\",\"gameId\":0,\"title\":\"ตา'ไนท์ท' ข้าว หลาม'ซิ่ง'ง\",\"content\":\"แลกเปลี่ยน 5000.0 baht สำเหร็จแล้ว\",\"urlAvatar\":\"fb.560999282058245\",\"isfb\":true,\"vip\":5}");

                        if (Globals.Config.show_new_alert)
                        {

                            if (Globals.Config.list_Alert.Count < 20)
                            {
                                if (jsonData.ContainsKey("data"))
                                {

                                    Globals.Config.list_Alert.Add(jsonData);
                                    UIManager.instance.showAlertMessage(jsonData);
                                }
                                if (jsonData.ContainsKey("content"))
                                {
                                    Globals.Config.list_AlertShort.Add(jsonData);
                                    AlertShort.Instance.checkShowAlertShort();
                                    //DOTween.Kill("checkAlertSort");
                                    //DOTween.Sequence().AppendCallback(() =>
                                    //{
                                    //    Debug.Log("checkShowAlertShort");
                                    //    AlertShort.instance.checkShowAlertShort();
                                    //})
                                    //   .AppendInterval(4.5f)
                                    //   .SetLoops(-1)
                                    //   .SetId("checkAlertSort");
                                }
                            }
                        }
                        //UIManager.instance.showAlertMessage(jsonData);

                        break;
                    }
                case "SAON":
                    {
                        //// arr.includes
                        //GameManager.getInstance().list_alert.push(jsonData.Cmd);
                        //if (!require('UIManager').instance.alertView._strSAON.includes(jsonData.Cmd)) require('UIManager').instance.alertView._strSAON.push(jsonData.Cmd);
                        //cc.NGWlog("length saon la== " + require('UIManager').instance.alertView._strSAON.length);
                        //if (!GameManager.getInstance().is_show_alert) require('UIManager').instance.alertView.showAlert();

                        break;
                    }
                case "uag":
                    {
                        //                    {
                        //                        "evt": "uag",
                        //  "ag": 16685598,
                        //  "lq": 21,
                        //  "vip": 2,
                        //  "dm": 0,
                        //  "mvip": 0,
                        //  "lqago": 0,
                        //  "vippoint": 10,
                        //  "vippointMax": 500,
                        //  "curLevelExp": 0,
                        //  "levelUser": 0,
                        //  "maxLevelExp": 0,
                        //  "diamond": 0
                        //}

                        Globals.User.userMain.AG = (long)jsonData["ag"];
                        Globals.User.userMain.VIP = (int)jsonData["vip"];
                        Globals.User.userMain.LQ = (int)jsonData["lq"];
                        //GameManager.getInstance().user.ag = jsonData.ag;
                        //GameManager.getInstance().user.vip = jsonData.vip;
                        //GameManager.getInstance().user.lq = jsonData.lq;
                        //require('UIManager').instance.updateChipUser();
                        //require('SMLSocketIO').getInstance().emitUpdateInfo();
                        // Tracking:: sendTrackBanner(3, _idBanner, paybanner);
                        UIManager.instance.updateAG();
                        SocketIOManager.getInstance().emitUpdateInfo();
                        if (TableView.instance != null)
                        {
                            TableView.instance.reloadLtv();
                        }
                        if (ExchangeView.instance != null)
                        {
                            ExchangeView.instance.UpdateAg();
                        }
                        break;
                    }
                case "uvip":
                    {
                        //GameManager.getInstance().userUpVip(jsonData);
                        Globals.User.userMain.VIP = 1;
                        Globals.User.userMain.AG += (long)jsonData["AG"];
                        UIManager.instance.updateVip();

                        SocketIOManager.getInstance().emitUpdateInfo();
                        UIManager.instance.showMessageBox(Globals.Config.getTextConfig("archive_vip1"));


                        if (ProfileView.instance)
                        {
                            ProfileView.instance.setInfo();
                        }
                        break;
                    }
                case "changepass":
                    {
                        //Global.RegisterPopup.btn_confirm.interactable = true;

                        //if (jsonData.error == 1)
                        //{
                        //    GameManager.getInstance().userChangePass();

                        //}
                        //else
                        //{
                        //    GameManager.getInstance().onShowConfirmDialog(GameManager.getInstance().getTextConfig('error_change_pass'));
                        //}
                        if ((int)jsonData["error"] == 1)
                        {
                            UIManager.instance.showDialog(Globals.Config.getTextConfig("change_pass_succes"), Globals.Config.getTextConfig("ok"), () =>
                            {
                                //this.onShowConfirmDialog(this.getTextConfig('change_pass_succes'), () => {
                                //    UIManager.instance.onLogout();
                                //});

                                //if (this.user_name == cc.sys.localStorage.getItem("USER_PLAYNOW"))
                                //{
                                //    cc.sys.localStorage.setItem("PASS_PLAYNOW", this.passwordToBeChanged);
                                //}

                                //if (this.user_name == cc.sys.localStorage.getItem("user_name"))
                                //{
                                //    cc.sys.localStorage.setItem("password", this.passwordToBeChanged);
                                //}

                                //Global.RegisterPopup.onClose();

                                if (ChangeNameView.intance)
                                {
                                    ChangeNameView.intance.saveData();
                                    ChangeNameView.intance.hide();
                                }
                                Globals.Config.typeLogin = Globals.LOGIN_TYPE.PLAYNOW;
                                if (ProfileView.instance)
                                {
                                    ProfileView.instance.setInfo();
                                }
                                WebSocketManager.getInstance().UserLogout = true;
                                UIManager.instance.showLoginScreen(true);
                            });
                        }
                        else
                        {
                            UIManager.instance.showMessageBox(Globals.Config.getTextConfig("error_change_pass"));
                        }
                        break;
                    }
                case "RUF":
                    {
                        //Global.RegisterPopup.btn_confirm.interactable = true;

                        //if (jsonData.hasOwnProperty("U"))
                        //{
                        //    GameManager.getInstance().userChangename(jsonData);
                        //}
                        if (jsonData.ContainsKey("U"))
                        {
                            Globals.User.userMain.Username = (string)jsonData["U"];
                            Globals.User.userMain.displayName = (string)jsonData["U"];
                            //cc.NGWlog("!> user change name", jsonData);
                            //Global.MainView.updateName();
                            //Global.ProfileView.updateName();

                            UIManager.instance.showDialog(
                                Globals.Config.getTextConfig("change_name_success") + " " + jsonData["U"], Globals.Config.getTextConfig("ok"),
                                () =>
                                {
                                    WebSocketManager.getInstance().UserLogout = true;
                                    UIManager.instance.showLoginScreen(true);
                                }
                            );
                            UIManager.instance.updateInfo();
                            if (ChangeNameView.intance)
                            {
                                ChangeNameView.intance.saveData();
                                ChangeNameView.intance.hide();
                            }

                            if (ProfileView.instance)
                            {
                                ProfileView.instance.setInfo();
                            }
                        }
                        break;
                    }

                case "updateObjectGame":
                    {
                        //                      {
                        //                          "evt": "updateObjectGame",
                        //"status": true,
                        //"key": 80
                        // }
                        if ((bool)jsonData["status"])
                        {
                            if (UIManager.instance.gameView != null)
                            {
                                UIManager.instance.gameView.updateItemVip(jsonData);
                            }
                        }
                        else
                        {
                            UIManager.instance.showToast((string)jsonData["msg"]);
                        }
                        //if (require('GameManager').getInstance().gameView !== null)
                        //{
                        //    require('GameManager').getInstance().gameView.updateObjectInGame(jsonData);
                        //}
                        break;
                    }
                case "iapResult":
                    {
                        ////{"evt":"iapResult","msg":"លេខកូដនេះត្រូវបានប្រើរួចហើយ។","verified":"false","goldPlus":0,"signature":"tKRMRLaaD3tDrrrGxwR58laVFW36bLvEyryT/kTK6ovSiG23y3SaO8q19kY25r1RuV2T2FAUFxx1EXqnQ5ofgMHFN4gxP8Nm70HbkP7s/ni2jMNRfujzH2hVF51rpJdyrtpGLNDqChZsJaOcw+RTDIDl3eetzrQbeacmf3N3YlF2xSo7MBPSZ9EyRPBq/ru5QWFLGencGT6Szy1AlJcxlS2lraMBL/6LA+NXIaG0wwyVeZOiohI4ky/NuTkKKyilmCw7xpVQ5IC4SwKkVMBSRgxDuNAsoX9D5LUufZa2Qx+y5NMoYXabjftl"}
                        var chip = (int)jsonData["goldPlus"];
                        var msg = (string)jsonData["msg"];
                        var signature = (string)jsonData["signature"];

                        if (chip > 0)
                        {
                            Globals.User.userMain.AG += chip;
                            UIManager.instance.updateAG();
                            SocketSend.sendUAG();
                        }
                        UIManager.instance.showMessageBox(msg);

                        var key_iap = Globals.User.userMain.Userid + "_iap_count";
                        var countIAP = PlayerPrefs.GetInt(key_iap, 0);
                        //var key_signdata = Globals.User.userMain.Userid + "_signdata_" + countIAP;
                        //var key_signature = Globals.User.userMain.Userid + "_signature_" + countIAP;
                        //PlayerPrefs.SetString(key_signdata, _signdata);
                        //PlayerPrefs.SetString(key_signature, _signature);
                        //countIAP++;
                        //PlayerPrefs.SetInt(key_iap, countIAP);
                        //GameManager.getInstance().onShowConfirmDialog(msg);

                        //var key_iap = require("GameManager").getInstance().user.id.toString() + "_iap_count";
                        //var countIAP = cc.sys.localStorage.getItem(key_iap);
                        //if (countIAP === null || typeof(countIAP) === "undefined")
                        //{
                        //    countIAP = 0;
                        //}
                        for (var i = 0; i < countIAP; i++)
                        {
                            var key_signdata = Globals.User.userMain.Userid + "_signdata_" + i;
                            var key_signature = Globals.User.userMain.Userid + "_signature_" + i;
                            var _signature = PlayerPrefs.GetString(key_signature);

                            if (_signature == signature)
                            {
                                PlayerPrefs.DeleteKey(key_signdata);
                                PlayerPrefs.DeleteKey(key_signature);
                                countIAP--;
                                PlayerPrefs.SetInt(key_iap, countIAP);
                                break;
                            }
                        }

                        SocketIOManager.getInstance().emitUpdateInfo();
                        break;
                    }
                case "iap_ios":
                    {
                        var chip = (int)jsonData["goldPlus"];
                        var msg = (string)jsonData["msg"];
                        var receipt = (string)jsonData["receipt"];

                        if (chip > 0)
                        {
                            Globals.User.userMain.AG += chip;
                            UIManager.instance.updateAG();
                            SocketSend.sendUAG();
                        }
                        UIManager.instance.showMessageBox(msg);
                        var key_iap = Globals.User.userMain.Userid + "_iap_count";
                        var countIAP = PlayerPrefs.GetInt(key_iap, 0);
                        for (var i = 0; i < countIAP; i++)
                        {
                            var key_receipt = Globals.User.userMain.Userid + "_receipt_" + i;
                            var _receipt = PlayerPrefs.GetString(key_receipt);

                            if (_receipt == receipt)
                            {
                                PlayerPrefs.DeleteKey(_receipt);
                                countIAP--;
                                PlayerPrefs.SetInt(key_iap, countIAP);
                                break;
                            }
                        }
                        SocketIOManager.getInstance().emitUpdateInfo();
                        break;
                    }
                case "jackpot":
                    //if (GameManager.getInstance().gameView != null)
                    //{
                    //    GameManager.getInstance().handleJackPot(jsonData);
                    //}
                    break;
                case "jackpotwin":
                    {
                        //GameManager.getInstance().handleJackPotWin(jsonData);
                        break;
                    }
                case "updatejackpot":
                    {
                        //GameManager.getInstance().handleUpdateJackPot(jsonData);
                        if (jsonData != null && jsonData.ContainsKey("M"))
                        {
                            UIManager.instance.PusoyJackPot = (long)jsonData["M"];
                        }
                        UIManager.instance.lobbyView.UpdateJackpotPusoy();
                        if (TableView.instance != null && TableView.instance.gameObject.activeSelf)
                        {
                            TableView.instance.UpdateJackpot();
                        }
                        if (Config.curGameId == (int)GAMEID.PUSOY && UIManager.instance.gameView != null)
                        {
                            ((BinhGameView)UIManager.instance.gameView).UpdateJackPot();
                        }
                        // if (Config.curGameId == (int)GAMEID.KARTU_QIU && UIManager.instance.gameView != null)
                        // {
                        //     ((BorkKDengView)UIManager.instance.gameView).handleUpdateJackpot(jsonData);
                        // }
                        break;
                    }
                case "jackpothistory":
                    {
                        //cc.NGWlog('!=> jackpothistory', jsonData)
                        //GameManager.getInstance().handleJackPotHis(jsonData);
                        if (BinhJackpotView.instance != null && BinhJackpotView.instance.gameObject.activeSelf)
                        {
                            BinhJackpotView.instance.SetInfo(jsonData);
                        }
                        break;
                    }
                case "cashOutHistory":
                    {
                        UIManager.instance.hideWatting();
                        if (ExchangeView.instance != null)
                        {
                            ExchangeView.instance.reloadListItemHistory(((JArray)jsonData["data"]).ToObject<List<JObject>>());
                        }
                        //if (Global.ExchangeView.node.getParent() !== null)
                        //{

                        //    Global.ExchangeView.listHistoryDt = jsonData.data;
                        //    Global.ExchangeView.listHistoryDtMobile = jsonData.dataMobile;
                        //    if (Global.ExchangeView.typeNet == 'Mobile')
                        //    {
                        //        Global.ExchangeView.curListDataItem = jsonData.dataMobile;
                        //    }
                        //    else
                        //    {
                        //        Global.ExchangeView.curListDataItem = jsonData.data;
                        //    }

                        //    Global.ExchangeView.reloadListView();
                        //    require('UIManager').instance.onHideLoad();
                        //}
                        break;
                    }
                case "getgift":
                    {
                        if (ExchangeView.instance != null)
                        {
                            ExchangeView.instance.cashOutReturn(jsonData);
                        }
                        //if (Global.ExchangeView.node.getParent() !== null)
                        //{
                        //    Global.ExchangeView.cashOutReturn(jsonData);
                        //    require('UIManager').instance.onHideLoad();
                        //}
                        break;
                    }
                case "rejectCashout":
                    {
                        if ((int)jsonData["status"] == 0)
                        {


                            UIManager.instance.showWaiting();
                            DOTween.Sequence().AppendInterval(2.5f).AppendCallback(() =>
                            {
                                SocketSend.getMail(12);
                                SocketSend.sendDTHistory();
                                if ((string)jsonData["msg"] != "")
                                    UIManager.instance.showMessageBox((string)jsonData["msg"]);
                            });
                        }

                        // if(jsonData.status)
                        //     require('NetworkManager').getInstance().sendDTHistory(require('GameManager').getInstance().user.id);
                        break;
                    }

                case "autoExit":
                    {
                        ////require("GameManager").getInstance().onShowToast(jsonData.data);
                        //require("GameManager").getInstance().gameView.handleAutoExit(jsonData);
                        if (UIManager.instance.gameView != null)
                        {
                            UIManager.instance.gameView.handleAutoExit(jsonData);
                        }
                        break;
                    }
                case "shareImageFb":
                    {
                        //GameManager.getInstance().onShowConfirmDialog(jsonData.Msg);
                        //require('NetworkManager').getInstance().getMail(12);
                        break;
                    }
                case "getWalletInfo":
                    {
                        if (ProfileView.instance != null)
                        {
                            //jsonData = JObject.Parse("{\"evt\":\"getWalletInfo\",\"data\":[{\"walletId\":\"123456789\",\"type\":\"GCASH\",\"urlImg\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/cashout/gcash.png?inline=true\"},{\"walletId\":\"9079836578435\",\"type\":\"GCASH\",\"urlImg\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/cashout/gcash.png?inline=true\"}]}");
                            ProfileView.instance.handleWalletInfo(jsonData);
                        }
                        break;
                    }
                case "checkUpdateWallet":
                    {
                        if (ProfileView.instance != null)
                        {
                            //jsonData = JObject.Parse("{\"evt\":\"checkUpdateWallet\",\"data\":[{\"walletId\":\"123456799\",\"type\":\"GcaSH\",\"urlImg\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/cashout/gcash.png?inline\u003dtrue\"}],\"showUpdate\":true}");
                            ProfileView.instance.handleUPdateWallet(jsonData);
                        }
                        break;
                    }
                case "payment_success":
                    //GameManager.getInstance().userNapTienSuccess(jsonData);
                    break;

                /* LOTO */
                case "lottery_topgame":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleGetStatusLuckyNumber(jsonData);
                    break;
                case "lottery_lotos":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleGetLotteryByType(jsonData);
                    break;
                case "lottery_results":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleGetLotteryResults(jsonData);
                    break;
                case "lottery_create":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleCreateBet(jsonData);
                    break;
                case "lottery_cancel":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleCancelBet(jsonData);
                    break;
                case "lottery_history":
                    if (LuckyNumberView.Instance != null) LuckyNumberView.Instance.HandleWonHistory(jsonData);
                    break;
                /* END LOTO */
                ///-------SLOT SIXIANG--------//
                case Globals.ACTION_SLOT_SIXIANG.getInfo:
                case Globals.ACTION_SLOT_SIXIANG.normalSpin:
                case Globals.ACTION_SLOT_SIXIANG.getBonusGames:
                case Globals.ACTION_SLOT_SIXIANG.scatterSpin:
                case Globals.ACTION_SLOT_SIXIANG.buyBonusGame:
                case Globals.ACTION_SLOT_SIXIANG.dragonPearlSpin:
                case Globals.ACTION_SLOT_SIXIANG.rapidPay:
                case Globals.ACTION_SLOT_SIXIANG.goldPick:
                case Globals.ACTION_SLOT_SIXIANG.luckyDraw:
                case Globals.ACTION_SLOT_SIXIANG.selectBonusGame:
                    handleGameSiXiang(jsonData);
                    break;
                case "farmInfo":
                    {
                        Globals.Config.dataVipFarm = jsonData;
                        UIManager.instance.SetDataVipFarmList();
                        float farmPercent = (float)jsonData["farmPercent"];
                        if (farmPercent >= 100f)
                        {
                            if (UIManager.instance.gameView != null) return;
                            if (Config.is_First_CheckVIPFarms)
                            {
                                Config.is_First_CheckVIPFarms = false;
                                if (VipFarmView.instance == null)
                                    UIManager.instance.openConfirmVipFarm();
                            }
                            else if (TableView.instance != null && TableView.instance.gameObject.activeSelf)
                            {
                                if (VipFarmView.instance == null)
                                    UIManager.instance.openConfirmVipFarm();
                            }
                            else if (UIManager.instance.lobbyView.gameObject.activeSelf)
                            {
                                if (VipFarmView.instance == null)
                                    UIManager.instance.openConfirmVipFarm();
                            }
                        }
                        break;
                    }
                case "farmReward":
                    {
                        if (VipFarmView.instance != null)
                        {
                            VipFarmView.instance.HandleReward(jsonData);
                        }
                        break;
                    }
                case "deleteAccount":
                    if ((bool)jsonData["isSuccess"])
                    {
                        //cc.sys.localStorage.setItem("isLogOut", "true");
                        if (SettingView.instance != null)
                        {
                            SettingView.instance.handleDeleteAcount();
                        }
                        // GameManager.getInstance().onShowConfirmDialog("true");
                    }
                    else
                    {
                        // GameManager.getInstance().onShowConfirmDialog("false");
                    }
                    break;
            }
        }
        else if (jsonData.ContainsKey("idevt"))
        {

            int idevt = (int)jsonData["idevt"];
            Globals.Logging.Log("------------------------------------------------>ID EVT:" + idevt + "<------------------------------------------->\n" + jsonData);
            switch (idevt)
            {
                case 300:
                    //cc.NGWlog("===========> chip in ket ::: " + jsonData.chip);
                    //GameManager.getInstance().user.agSafe = jsonData.chip;
                    //Global.KetView.lbAgSafe.string = GameManager.getInstance().formatNumber(
                    //    GameManager.getInstance().user.agSafe
                    //);
                    //Global.MainView.lbSafe.string = GameManager.getInstance().formatNumber(
                    //    GameManager.getInstance().user.agSafe);
                    //require('UIManager').instance.onHideLoad();
                    Globals.Logging.Log("-=- update bank   " + jsonData.ToString());
                    Globals.User.userMain.agSafe = (long)jsonData["chip"];
                    UIManager.instance.updateAGSafe();
                    break;
                case 301://send to safe {"idevt":301,"status":true,"chipbank":11558654,"chipuser":84600}

                    if ((bool)jsonData["status"] == true)
                    {
                        Globals.User.userMain.AG = (long)jsonData["chipuser"];
                        Globals.User.userMain.agSafe = (long)jsonData["chipbank"];
                        if (SafeView.instance != null)
                        {
                            SafeView.instance.setInfo();
                        }

                        UIManager.instance.updateAG();
                        UIManager.instance.updateAGSafe();
                    }
                    else
                    {
                        UIManager.instance.showDialog((string)jsonData["msg"], Globals.Config.getTextConfig("ok"));
                    }
                    break;
                case 302://get from safe
                    if ((bool)jsonData["status"] == true)
                    {
                        Globals.User.userMain.AG = (long)jsonData["chipuser"];
                        Globals.User.userMain.agSafe = (long)jsonData["chipbank"];
                        if (SafeView.instance != null)
                        {
                            SafeView.instance.setInfo();
                        }
                        UIManager.instance.updateAG();
                        UIManager.instance.updateAGSafe();
                    }
                    else
                    {
                        UIManager.instance.showToast((string)jsonData["msg"]);
                    }
                    break;

                case 303://{"idevt":303,"status":true,"msg":"Send gift to your friend successfully!","chipbank":10999801,"chipuser":645000}
                    //if (jsonData.status)
                    //{
                    //    GameManager.getInstance().user.agSafe = jsonData.chipbank;
                    //    GameManager.getInstance().user.ag = jsonData.chipuser
                    //    Global.KetView.lbAgSafe.string = GameManager.getInstance().formatNumber(
                    //        GameManager.getInstance().user.agSafe
                    //    );
                    //    Global.KetView.lbAg.string = GameManager.getInstance().formatNumber(
                    //        GameManager.getInstance().user.ag
                    //    );
                    //    Global.KetView.lbAgSafe.string = GameManager.getInstance().formatNumber(
                    //        GameManager.getInstance().user.agSafe
                    //    );
                    //    require('NetworkManager').getInstance().sendUAG();
                    //    GameManager.getInstance().onShowConfirmDialog(jsonData.msg);
                    //}
                    //else
                    //    GameManager.getInstance().onShowConfirmDialog(jsonData.msg);
                    break;

                case 304:
                    //if (jsonData.msg != null)
                    //{
                    //    GameManager.getInstance().onShowConfirmDialog(jsonData.msg);
                    //}
                    //require('NetworkManager').getInstance().getInfoSafe();
                    break;

                case 500://get his safe
                    //cc.NGWlog("=======> history safe");
                    //GameManager.getInstance().list_data_history_safe = [];
                    //if (!jsonData.data) return;
                    //var data = JSON.parse(jsonData.data);
                    //for (var i = 0; i < data.length; i++)
                    //{
                    //    var item = new HistorySafeData();
                    //    item.timeday = data[i].timeday;
                    //    item.timehour = data[i].timehour;
                    //    item.content = data[i].msg;
                    //    item.chipchange = data[i].chipchange;
                    //    item.chip = data[i].chip;
                    //    GameManager.getInstance().list_data_history_safe.push(item);
                    //}
                    //cc.NGWlog('-hihihihi------->' + GameManager.getInstance().list_data_history_safe.length);
                    //if (Global.GiftView.node.getParent() !== null)
                    //{
                    //    Global.GiftView.reloadHistory();
                    //}
                    //if (Global.KetView.node.getParent() !== null)
                    //{
                    //    Global.KetView.reloadHistory();
                    //}
                    UIManager.instance.hideWatting();
                    if (SendGiftView.instance != null && SendGiftView.instance.gameObject.activeSelf)
                    {
                        JArray dataHis = JArray.Parse((string)jsonData["data"]);
                        SendGiftView.instance.reloadHistory(dataHis);
                    }
                    if (SafeView.instance != null)
                    {
                        JArray dataHis = JArray.Parse((string)jsonData["data"]);
                        SafeView.instance.reloadHistory(dataHis);
                    }
                    break;
                case 400:
                    //{"idevt":400,"data":"[{\"id\":311827,\"name\":\"frocker\",\"avatar\":9,\"faceid\":2009684709344701,\"vip\":10,\"chip\":760000,\"status\":\"📌Chipsအား Viberမွသာ ေရာင္းခ်ေပးသည္📌Bill 1,000ks\\u003d6.5M, Wave/OK$ 1000ks\\u003d8.5M📌\",\"online\":false},{\"id\":380703,\"name\":\"koko_kyi\",\"avatar\":3,\"faceid\":0,\"vip\":10,\"chip\":0,\"status\":\"မာနမင္​းသား\",\"online\":true},{\"id\":98788,\"name\":\"piggy_lady\",\"avatar\":10,\"faceid\":567752770291410,\"vip\":10,\"chip\":1950000,\"status\":\"1000\\u003d7m wave 110m\\\"အေခ်းအဌားမလုပ္ပါ\\\" vip pointတက္သည္\",\"online\":false},{\"id\":354062,\"name\":\"saw_thu_ra\",\"avatar\":2,\"faceid\":179839392689481,\"vip\":10,\"chip\":0,\"status\":\"Player\",\"online\":false},{\"id\":100100,\"name\":\"authorised_shweyang\",\"avatar\":7,\"faceid\":0,\"vip\":10,\"chip\":300000,\"status\":\"...\",\"online\":false},{\"id\":325538,\"name\":\"zawpyan\",\"avatar\":5,\"faceid\":0,\"vip\":9,\"chip\":0,\"status\":\"ကတိေတြ လြယ္လြယ္ လာမေပးနဲ႔။feelက်ဲတယ္။\",\"online\":false},{\"id\":134468,\"name\":\"station\",\"avatar\":5,\"faceid\":0,\"vip\":9,\"chip\":2221030,\"status\":\"\",\"online\":false},{\"id\":596996,\"name\":\"7-rainbow\",\"avatar\":12,\"faceid\":0,\"vip\":9,\"chip\":49000,\"status\":\"...\",\"online\":false},{\"id\":588256,\"name\":\"thin_ei_phyu\",\"avatar\":1,\"faceid\":404829843373013,\"vip\":8,\"chip\":0,\"status\":\"\",\"online\":false},{\"id\":516239,\"name\":\"f_seller\",\"avatar\":2,\"faceid\":2030492887263883,\"vip\":8,\"chip\":0,\"status\":\"💰Chips Seller💰\",\"online\":false},{\"id\":679764,\"name\":\"rainwine\",\"avatar\":9,\"faceid\":0,\"vip\":8,\"chip\":900000,\"status\":\"ျပန္ေရာင္းခ်င္တယ္\",\"online\":false},{\"id\":465764,\"name\":\"nay_da_na\",\"avatar\":1,\"faceid\":171080700420707,\"vip\":7,\"chip\":0,\"status\":\"...\",\"online\":false},{\"id\":606363,\"name\":\"1z2a1y1r\",\"avatar\":2,\"faceid\":125782501688239,\"vip\":7,\"chip\":47000,\"status\":\"...\",\"online\":true},{\"id\":372262,\"name\":\"blue123\",\"avatar\":10,\"faceid\":0,\"vip\":7,\"chip\":61870000,\"status\":\"\",\"online\":false},{\"id\":202617,\"name\":\"owlfiter\",\"avatar\":9,\"faceid\":0,\"vip\":7,\"chip\":0,\"status\":\"ခ်ိစ္​​ေရာင္​းမည္​ ၀၉၇၇၉၂၆၈၈၂၉ \",\"online\":true},{\"id\":487688,\"name\":\"ngaye99\",\"avatar\":1,\"faceid\":0,\"vip\":7,\"chip\":0,\"status\":\"\\\" မေကာင္းဘူးထင္ရင္ ကင္းေအာင္ေန \\\"\",\"online\":false},{\"id\":313932,\"name\":\"aye_mya\",\"avatar\":12,\"faceid\":462451614185878,\"vip\":7,\"chip\":0,\"status\":\"...\",\"online\":false},{\"id\":526229,\"name\":\"real_b_fri\",\"avatar\":999,\"faceid\":254193898724202,\"vip\":7,\"chip\":979400,\"status\":\"ျပဳံးတယ္ ဘယ္ေလာက္ကုန္ကုန္\",\"online\":false},{\"id\":157352,\"name\":\"mg_poker\",\"avatar\":3,\"faceid\":220703818516511,\"vip\":7,\"chip\":0,\"status\":\"ေစတနာသည္လုတိုင္းနွင့္မတန္ပါ  က်ြန္ုပ္မရွိခင္ကသင္ကူညီဖူးပါသလား\",\"online\":false},{\"id\":352988,\"name\":\"neverdie\",\"avatar\":999,\"faceid\":110174849889516,\"vip\":7,\"chip\":0,\"status\":\"​ေစတနာသည္.....လူတိုင္းနဲ႔မတန္​\",\"online\":false},{\"id\":199969,\"name\":\"tuetue000\",\"avatar\":8,\"faceid\":0,\"vip\":7,\"chip\":150199969,\"status\":\"\",\"online\":true},{\"id\":649727,\"name\":\"waiphyo.min\",\"avatar\":1,\"faceid\":112117816399676,\"vip\":7,\"chip\":0,\"status\":\"မငိုပါနဲ႔ကေလးရယ္\",\"online\":false},{\"id\":491640,\"name\":\"blank.verse1984\",\"avatar\":999,\"faceid\":102364497355636,\"vip\":7,\"chip\":0,\"status\":\"ပင္လယ္ထဲမွာ လိုင္းမမွီဘူးေနာ္ တစ္ခါတစ္ရံေလာက္ပဲလိုင္းမိတယ္\",\"online\":false},{\"id\":4481,\"name\":\"b1joker\",\"avatar\":999,\"faceid\":1950185488387403,\"vip\":6,\"chip\":133366,\"status\":\"...\",\"online\":false},{\"id\":139042,\"name\":\"paingsoeoo\",\"avatar\":999,\"faceid\":230231111085082,\"vip\":6,\"chip\":0,\"status\":\"လာမ​ေတာင္​းပါန​ွင္​့။မ​ေပးနိုင္​ပါ။မ​ေရာင္​းပါ။ ဆဲမွ ရိုင္​းတယ္​မ​ေျပာပါနွင္​့။\",\"online\":false},{\"id\":527043,\"name\":\"moemoewim\",\"avatar\":2,\"faceid\":211290439724203,\"vip\":6,\"chip\":0,\"status\":\"...\",\"online\":false},{\"id\":594036,\"name\":\"may_shel\",\"avatar\":0,\"faceid\":857294354658078,\"vip\":6,\"chip\":1770400,\"status\":\"...\",\"online\":false},{\"id\":100581,\"name\":\"aungthurawin\",\"avatar\":1,\"faceid\":0,\"vip\":6,\"chip\":547960,\"status\":\"...\",\"online\":false},{\"id\":182348,\"name\":\"ko.par.gyi\",\"avatar\":8,\"faceid\":0,\"vip\":6,\"chip\":120000,\"status\":\"...of\",\"online\":false},{\"id\":528343,\"name\":\"trueman11111\",\"avatar\":3,\"faceid\":0,\"vip\":6,\"chip\":0,\"status\":\"FUCK ADMIN FUCK GAME\",\"online\":false}]"}
                    // var dat = JSON.parse(jsonData.data);
                    // if (Global.TopRichView.instance !== null) {
                    //     Global.TopRichView.arrVip = [];
                    //     for (let i = 0; i < dat.length; i++) {
                    //         const itemDat = dat[i];
                    //         var topData = new TopData();
                    //         topData.name = itemDat.name;
                    //         topData.chip = itemDat.chip;
                    //         topData.av = itemDat.avatar;
                    //         topData.vip = itemDat.vip;
                    //         topData.id = itemDat.id;
                    //         topData.fId = itemDat.faceid;
                    //         topData.status = itemDat.status;
                    //         if (topData.status === "" || typeof topData.status == 'undefined') {
                    //             topData.status = "...";
                    //         }
                    //         Global.TopRichView.arrVip.push(topData);
                    //     }
                    //     //  Global.TopRichView.updateList();
                    // }
                    break;
                case 200:
                    {
                        if (ProfileView.instance != null && ProfileView.instance.gameObject.activeSelf)
                        {
                            ProfileView.instance.updateStatus((string)jsonData["status"]);
                        }
                        break;
                    }

                case 202: //change name
                          //Global.RegisterPopup.btn_confirm.interactable = true;
                          //GameManager.getInstance().userRegister(jsonData);
                          //                    {
                          //                        "idevt": 202,
                          //  "status": true,
                          //  "msg": "การลงทะเบียนสำเร็จ"
                          //}
                    if ((bool)jsonData["status"])
                    {
                        Globals.User.userMain.displayName = Globals.Config.user_name_temp;
                        //Globals.Config.user_name = Globals.Config.user_name_temp;
                        //Globals.Config.user_pass = Globals.Config.user_pass_temp;

                        Globals.User.userMain.Username = Globals.Config.user_name_temp;
                        PlayerPrefs.SetInt("isReg", 1);
                        UIManager.instance.updateInfo();
                        if (ChangeNameView.intance)
                        {
                            ChangeNameView.intance.saveData();
                            ChangeNameView.intance.hide();
                        }

                        if (ProfileView.instance)
                        {
                            ProfileView.instance.setInfo();
                        }

                        SocketIOManager.getInstance().emitUpdateInfo();
                    }

                    UIManager.instance.showMessageBox((string)jsonData["msg"]);
                    break;
                case 201:
                    {
                        if ((bool)jsonData["result"] == true && ProfileView.instance != null)
                        {
                            ProfileView.instance.updateStatus((string)jsonData["status"]);
                        }
                        break;
                    }
                case 800:

                    bool status = (bool)jsonData["status"];
                    if (status)
                    {
                        Globals.User.userMain.AG = (long)jsonData["AG"];
                        UIManager.instance.hideWatting();
                        UIManager.instance.updateAG();
                        if (SendGiftView.instance != null && SendGiftView.instance.gameObject.activeSelf)
                        {
                            SendGiftView.instance.onSuccessSendGift();
                        }
                        if (SafeView.instance != null)
                        {
                            SafeView.instance.setInfo();
                        }
                    }

                    UIManager.instance.showMessageBox((string)jsonData["msg"]);
                    break;
                case 801:

                    UIManager.instance.userHasNewMailAdmin();
                    break;


            }
        }
    }
    public static void handleGameSiXiang(JObject data)
    {
        JObject dataGame = JObject.Parse((string)data["data"]);
        if (SiXiangView.Instance == null)
        {
            Debug.Log("clm chua co game sixiang la sao");
        }
        switch ((string)data["evt"])
        {
            case Globals.ACTION_SLOT_SIXIANG.getInfo:
                //dataGame = JObject.Parse(SiXiangFakeData.Instance.getInfoDragonPearl);
                SiXiangView.Instance.handleGetInfo(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.getBonusGames:
                SiXiangView.Instance.handleBonusInfo(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.normalSpin:
                SiXiangView.Instance.handleNormalSpin(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.scatterSpin:
                SiXiangView.Instance.handleScatterSpin(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.buyBonusGame:
                SiXiangView.Instance.handleBuyBonusGame(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.dragonPearlSpin:
                SiXiangView.Instance.handleDragonPealsSpin(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.rapidPay:
                SiXiangView.Instance.handleRapidPay(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.goldPick:
                SiXiangView.Instance.handleGoldPick(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.luckyDraw:
                SiXiangView.Instance.handleLuckyDraw(dataGame);
                break;
            case Globals.ACTION_SLOT_SIXIANG.selectBonusGame:
                SiXiangView.Instance.handleSelectBonusGame(dataGame);
                break;
        }
    }
}
