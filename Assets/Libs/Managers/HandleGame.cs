using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Globals;
public class HandleGame
{
    public static List<JObject> listDelayEvt = new List<JObject>();
    public static void processData(JObject jData)
    {
        var gameView = UIManager.instance.gameView;
        if (gameView == null)
        {
            Globals.Logging.Log("processData---> Chua co GameView----->\n                             " + (string)jData["evt"]);
            return;
        }
        string evt = (string)jData["evt"];
        List<string> listEvtNotDelay = new List<string> { "chattable", "dealer", "findDealer", "leave_dealer" };
        if (evt.Equals("ltable"))
        {

            JObject data = jData;
            var tableId = Globals.Config.tableId;
            var namePl = "";
            if (data.ContainsKey("Name"))
            {
                namePl = (string)data["Name"];
            }
            else if (data.ContainsKey("name"))
            {
                namePl = (string)data["name"];
            }

            if (namePl == Globals.User.userMain.Username || namePl == (tableId + ""))
            {
                JObject dataLeave = new JObject();
                dataLeave["tableid"] = tableId;
                dataLeave["curGameID"] = Globals.Config.curGameId;
                dataLeave["stake"] = Globals.Config.tableMark;
                dataLeave["reason"] = data.ContainsKey("errorCode") ? data["errorCode"] : 0;
                UIManager.instance.gameView.dataLeave = dataLeave;
                Debug.Log("set data Leave");
            }
        }
        if (listDelayEvt.Count != 0 && !listEvtNotDelay.Contains(evt)) //
        {
            Globals.Logging.Log("-------------Exist Evt Delay In List--->Add To Delay--->" + evt);
            Globals.Logging.Log("-------------Data<---------------\n" + jData.ToString());
            listDelayEvt.Add(jData);
            return;
        }

        if (gameView.delayEvents.Contains(evt)) //check xem co su dung delay evt ko
        {
            Globals.Logging.Log("-------This is Delay Evt------Add To Delay--->" + evt);
            listDelayEvt.Add(jData);
        }
        resolveData(jData);
    }
    private static void resolveData(JObject jData)
    {

        var gameView = UIManager.instance.gameView;
        if (gameView == null) return;
        SocketIOManager.getInstance().emitSIOWithValue(jData, "GameTransportPacket", false);
        string evt = (string)jData["evt"];
        Globals.Logging.Log("<-------------------------EVT:" + evt + "------------------------->\n" + jData.ToString().Replace("\n", "").Replace(" ", ""));
        switch (evt)
        {
            case "ctable":
                gameView.handleCTable((string)jData["data"]);
                break;
            case "cctable":
                gameView.handleCCTable(jData);
                break;
            case "stable":
                gameView.handleSTable((string)jData["data"]);
                break;
            case "vtable":
                gameView.handleVTable((string)jData["data"]);
                break;
            case "jtable":
                gameView.handleJTable((string)jData["data"]);
                break;
            case "ltable":
                //{ "errorCode":-2,"evt":"ltable","Name":"tictctoe123"}
                var data = jData;
                if (Globals.Config.curGameId == (int)Globals.GAMEID.BLACKJACK)
                {
                    data = JObject.Parse((string)jData["data"]);//JSON.parse(dataJson.data);
                }

                var tableId = Globals.Config.tableId;
                var namePl = "";
                if (data.ContainsKey("Name"))
                {
                    namePl = (string)data["Name"];
                }
                else if (data.ContainsKey("name"))
                {
                    namePl = (string)data["name"];
                }
                if (namePl == Globals.User.userMain.Username || namePl == (tableId + ""))
                {
                    JObject dataLeave = new JObject();
                    dataLeave["tableid"] = tableId;
                    dataLeave["curGameID"] = Globals.Config.curGameId;
                    dataLeave["stake"] = Globals.Config.tableMark;
                    dataLeave["reason"] = data.ContainsKey("errorCode") ? data["errorCode"] : 0;
                    //Globals.Logging.LogError("dataLeave  " + dataLeave.ToString());
                    UIManager.instance.gameView.dataLeave = dataLeave;
                    Debug.Log("dataLeave=" + UIManager.instance.gameView.dataLeave.ToString());
                    SocketIOManager.getInstance().emitSIOWithValue(UIManager.instance.gameView.dataLeave, "LeavePacket", false);
                }
                ;
                gameView.handleLTable(jData);
                break;
            case "rjtable":
                gameView.handleRJTable((string)jData["data"]);
                break;
            case "chattable":
                gameView.handleChatTable(jData);
                break;
            case "autoExit":
                gameView.handleAutoExit(jData);
                break;
            case "spin":
                gameView.handleSpin(jData);
                break;
            case "updateObjectGame":
                JObject jsonData = JObject.Parse((string)jData["data"]);
                gameView.updateItemVip(jsonData);
                break;
            case "updateChip":
                User.userMain.AG = (long)jData["ag"];
                gameView.HandlerUpdateUserChips(jData);
                break;
            default:
                {
                    break;
                }
        }
        switch (Globals.Config.curGameId)
        {
            case (int)Globals.GAMEID.DUMMY:
                {
                    HandleDummy.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SLOTNOEL:
                {
                    HandleSlotNoelView.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SLOTTARZAN:
                {
                    HandleSlotTarzanView.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SLOT_JUICY_GARDEN:
                {
                    HandleSlotJuicyGarden.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SLOT_INCA:
                {
                    HandleSlotInCa.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SLOT20FRUIT:
                {
                    HandleSlotFruit.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.KARTU_QIU:
                {
                    HandleBorKDengView.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.KEANG:
                {
                    HandleKeangView.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.GAOGEA:
                {
                    HandleGaoGea.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SICBO:
                {
                    HandleSicboView.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.BANDAR_QQ:
                {
                    HandleBandarQQ.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.RONGHO:
                {
                    HandleDragonTiger.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.PUSOY:
                {
                    ((BinhGameView)gameView).ProcessResponseData(jData);
                    break;
                }
            case (int)Globals.GAMEID.DOMINO:
                {
                    HandleDomino.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.BACCARAT:
                {
                    HandleBaccarat.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.BLACKJACK:
                {
                    HandleBlackJack.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.TONGITS_OLD:
                {
                    HandleTongits.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.LUCKY9:
                {
                    HandleLucky9.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.SABONG:
                {
                    ((SabongGameView)gameView).ProcessResponseData(jData);
                    break;
                }
            case (int)Globals.GAMEID.LUCKY_89:
                {
                    ((Lucky89View)gameView).ProcessResponseData(jData);
                    break;
                }
            case (int)Globals.GAMEID.TONGITS:
                {
                    HandleTongits.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.TONGITS_JOKER:
                {
                    HandleTongits.processData(jData);
                    break;
                }
            case (int)Globals.GAMEID.MINE_FINDING:
                {
                    ((MineFindingView)gameView).ProcessResponseData(jData);
                    break;
                }
            case (int) Globals.GAMEID.TIENLEN:
                {
                    HandleTienlenView.processData(jData);
                    break;
                }
        }
    }
    public static void nextEvt()
    {
        if (listDelayEvt.Count != 0)
        {
            listDelayEvt.RemoveAt(0);
        }
        //xoa thang evt delay dau tien vi xong thang nay moi call next evt;
        //foreach (JObject jData in listDelayEvt)
        //{
        //    //Globals.Logging.Log("next Evt:" + (string)jData["evt"]);
        //    resolveData(jData);
        //}
        while (listDelayEvt.Count > 0)
        {
            JObject jData = listDelayEvt[0];
            resolveData(jData);
            listDelayEvt.RemoveAt(0);
        }

        //DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
        //{
        //    listDelayEvt.Clear();
        //});
    }
    public static void handleLeave()
    {
        UIManager.instance.gameView.onLeave();
    }
}

