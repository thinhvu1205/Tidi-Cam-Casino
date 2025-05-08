using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleBaccarat
{
    public static void processData(JObject jData)
    {
        var gameView = (BaccaratView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "start":
                {
                    gameView.handleStartGame(jData);
                    break;
                }
            case "betAccepted":
                {
                    gameView.handleBet(jData);
                    break;
                }
            case "finish":
                {
                    gameView.handleFinishGame(jData);
                    break;
                }
            case "tip":
                {
                    gameView.HandlerTip(jData);
                    break;
                }
            case "lc":
                {
                    gameView.handlelc(jData);
                    break;
                }
            case "betError":
                gameView.handleBetError(jData);
                break;
        }
    }

}
