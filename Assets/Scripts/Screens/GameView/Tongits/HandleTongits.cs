using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleTongits
{
    // Start is called before the first frame update
    public static void processData(JObject jData) // class nay dung de viet them cac evt rieng cua game binh a nhe. Con may cai chung nhu stable,ctable o ben handleGame co r/
    {
        var gameView = (TongitsView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            //case "watcherChat":
            //    gameView.handleWatcherChat(jData);
            //    break;
            case "tip":
                gameView.HandlerTip(jData);
                break;
            case "autoExit":
                gameView.handleAutoExit(jData);
                break;
            case "timeToStart":
                gameView.handleTimeToStart(jData);
                break;
            case "HITPOT":
                gameView.handleHitpot(jData);
                break;
            case "lc":
                gameView.handleLc(jData);
                break;
            case "bc":
                gameView.handleBc(jData);
                break;
            case "dc":
                gameView.handleDc(jData);
                break;
            case "hc":
                gameView.handleHc(jData);
                break;
            case "sc":
                gameView.handleSc(jData);
                break;
            case "finish":
                gameView.handleFinish(jData);
                break;
            case "fight":
                gameView.handleFight(jData);
                break;
            case "acceptFight":
                gameView.handleAcceptFight(jData);
                break;
            case "bd":
                gameView.handleBd(jData);
                break;
            case "msg":
                gameView.handleMsg(jData);
                break;
            case "xepbai":
                gameView.handleXepBai(jData);
                break;
            case "declare":
                gameView.handleDeclare(jData);
                break;
            case "updateAG":
                gameView.handleUpdateAG(jData);
                break;
            case "bp":
                gameView.handleBp(jData);
                break;
            //case "watcherJoin":
            //    gameView.handleWatcherJoin(jData);
            //    break;
            //case "numberWatcher":
            //    gameView.handleNumberWatcher(jData);
            //    break;
            //case "watcherBet":
            //    gameView.handleWatcherBet(jData);
            //    break;
            //case "watcherLeft":
            //    gameView.handleWatcherLeft(jData);
            //    break;
            //case "startBet":
            //    gameView.handleStartBet(jData);
            //    break;
            case "betInfo":
                gameView.handleBetInfo(jData);
                break;
                //case "betResult":
                //    gameView.handleBetResult(jData);
                //    break;
                //case "watcherDonate":
                //    gameView.handleViewerDonate(jData);
                //    break;
                //case "watcherBetHistory":
                //    gameView.handleListViewerBet(jData);
                //    break;
        }
    }
}
