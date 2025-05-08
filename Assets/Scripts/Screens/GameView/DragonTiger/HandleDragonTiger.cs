using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleDragonTiger
{
    public static void processData(JObject jData)
    {
        var gameView = (DragonTigerView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "start":
                {
                    gameView.handleStartGame(jData);
                    break;
                }
            case "bet":
                {
                    gameView.handleBet(jData);
                    break;
                }
            case "lc":
                {
                    gameView.handleLc(jData);
                    break;
                }
            case "finish":
                {
                    gameView.handleFinishGame(jData);
                    break;
                }
            //case "tip":
            //    {
            //        gameView.HandlerTip(jData);
            //        break;
            //    }
                //case "stable":
                //	{
                //		gameView.handleSTable((string)jData);
                //		break;
                //	}
                //case "ctable":
                //	{
                //		gameView.handleCTable((string)jData);
                //		break;
                //	}
                //case "jtable":
                //	{
                //		gameView.handleJTable((string)jData);
                //		break;
                //	}
                //case "ltable":
                //	{
                //		gameView.handleLTable(jData);
                //		break;
                //	}
        }
    }
}
