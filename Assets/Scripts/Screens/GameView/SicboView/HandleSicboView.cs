using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleSicboView
{
    public static void processData(JObject jData)
    {
        var gameView = (HiloView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "start":
                gameView.handleStart();
                break;
            case "lc":
                gameView.handleStartBet((string)jData["data"]);
                break;
            case "bet":
                gameView.handleBet((string)jData["data"]);
                break;
            case "finish":
                gameView.handleFinish((string)jData["data"]);
                break;

        }
    }
}
