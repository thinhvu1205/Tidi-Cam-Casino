using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleGaoGea
{
    public static void processData(JObject jData)
    {
        var gameView = (GaoGeaView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "startGame":
                {
                    gameView.handleStartGame(jData);
                    break;
                }
            case "bc":
                {
                    //gameView.handleBc(jData);
                    break;
                }
            case "cab":
                {
                    gameView.handleCab(jData);
                    break;
                }
            case "show":
                {
                    gameView.handleCab(jData, "Show");
                    break;
                }
            case "fold":
                {
                    gameView.handleCab(jData, "Fold");
                    break;
                }
            case "call":
                {
                    if ((int)jData["agCurrent"] == 0)
                    {
                        gameView.handleCab(jData, "Allin");
                    }
                    else
                        gameView.handleCab(jData, "Call");
                    break;
                }
            case "check":
                {
                    gameView.handleCab(jData, "Check");
                    break;
                }
            case "raise":
                {
                    if ((int)jData["agCurrent"] == 0)
                    {
                        gameView.handleCab(jData, "Allin");
                    }
                    else
                        gameView.handleCab(jData, "Raise");
                    break;
                }
            case "getNext":
                {
                    gameView.setTurn(jData);
                    break;
                }
            case "pstartg":
                {
                    gameView.handleTimeToStart(jData);
                    break;
                }
           
            case "startgame":
                {
                    gameView.handleStartGame(jData);
                    break;
                }
            case "finishgame":
                {
                    gameView.handleFinish(jData);
                    break;
                }
            case "tip":
                {
                    gameView.HandlerTip(jData);
                    break;
                }
            case "autoExit":
                {
                    gameView.handleAutoExit(jData);
                    break;
                }

        }
    }
}
