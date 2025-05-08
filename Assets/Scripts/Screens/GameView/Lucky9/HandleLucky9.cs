using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleLucky9
{
    // Start is called before the first frame update
    public static void processData(JObject jData) // class nay dung de viet them cac evt rieng cua game binh a nhe. Con may cai chung nhu stable,ctable o ben handleGame co r/
    {
        var gameView = (Lucky9View)UIManager.instance.gameView; 
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "startGame":
                gameView.handleStartGame(jData);
                break;
            case "isBanker":
                gameView.handleIsBanker(jData);
                break;
            case "potStatus":
                gameView.handleAddPot(jData);
                break;
            case "isDealer":
                gameView.handleIsDealer(jData);
                break;
            case "bm":
                gameView.handleBm(jData);
                break;
            case "bc":
                gameView.handleBc(jData);
                break;
            case "twoCard":
                gameView.handleTwoCard(jData);
                break;
            case "notifyLucky9":
                gameView.handleNotifyLucky9(jData);
                break;
            case "finish":
                gameView.handleFinish(jData);
                break;
            case "bankerTurn":
                gameView.handleBankerTurn(jData);
                break;
            case "uag":
                gameView.handleUAG((string)jData["data"]);
                break;
        }
    }
}
