using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleTienlenView
{
    public static void processData(JObject jData)
    {
        var gameView = (TienlenView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "stable":
                gameView.countDownTimeToStart((int)jData["time"]);
                break;
            case "rtable":
                gameView.handleRTable(jData);
                break;
            case "lc":
                // {"evt":"lc","arr":[25,21,32,14,44,19,20,36,6,35,45,1,39],"T":12,"S":0,"rate":0,"score":0,"nameturn":"fb.105964707280388","deckCount":26,"firstRound":true}
                gameView.startGame(jData);
                break;
            case "dc":
                // {"nameturn":"fb.105964707280388","arr":[4,30,5,18],"evt":"dc","nextturn":"ahiha123","T":0,"newTurn":false}
                string name = (string)jData["nameturn"];
                string nextTurn = (string)jData["nextturn"];
                bool newTurn = (bool)jData["newTurn"];
                JArray arr = (JArray)jData["arr"];
                gameView.danhBai(name, nextTurn, arr, newTurn);
                break;
            case "cc":
                // {"nameturn":"ahiha123","evt":"cc","nextturn":"fb.105964707280388","T":0,"newTurn":true}

                gameView.boLuot((string)jData["nameturn"], (string)jData["nextturn"], (bool)jData["newTurn"]);
                break;
            case "cutCard":
                // {"evt":"cutCard","user":"ahiha123","agUser":1181800,"userCut":"annaly","agUserCut":833200,"ag":200000}
                gameView.cutCard((string)jData["user"], (long)jData["agUser"], (string)jData["userCut"], (long)jData["agUserCut"], (long)jData["ag"]);
                break;
            case "ace":
                // {"evt":"ace","data":"You can\u0027t discard this cards","T":0,"C":0,"rate":0,"score":0,"time":0}
                gameView.danhBaiError((string)jData["data"]);
                break;
            case "finish":
                // {"evt":"finish","data":"[{\"N\":\"ahiha123\",\"M\":-370,\"AG\":965738,\"ArrCard\":[44,45,6,19,32,20,21,35,36,25,39,1,14],\"point\":74,\"rate\":0,\"TypeWin\":-1,\"lstDenLang\":[]},{\"N\":\"fb.105964707280388\",\"M\":351,\"AG\":409,\"ArrCard\":[],\"point\":0,\"rate\":0,\"TypeWin\":0,\"lstDenLang\":[]}]","T":0,"C":0,"rate":0,"score":0,"time":0}
                // cc.log("Tien len : finihs handle");
                gameView.finishGameTienLen((string)jData["data"]);
                break;
            case "uag":
                // {"evt":"uag","data":"[{\"N\":\"fb.173461193643727\",\"AG\":321},{\"N\":\"te.1556959617_460832d0-42e2-469a-abe8-b83b0541a248\",\"AG\":116},{\"N\":\"seth_tha2222\",\"AG\":269}]","T":0,"C":0,"rate":0,"score":0,"time":0}
                gameView.updateMoney((string)jData["data"]);
                break;
        }
    }
}