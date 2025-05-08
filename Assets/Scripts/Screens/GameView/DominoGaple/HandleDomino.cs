using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleDomino
{
	public static void processData(JObject jData)
	{
		var gameView = (DominoGapleView)UIManager.instance.gameView;
		if (gameView == null) return;
		string evt = (string)jData["evt"];
		switch (evt)
		{
            case "notiStart":
                {
                    gameView.handleStartGame(jData);
                    break;
                }
            case "lc":
                {
                    gameView.handleLc(jData);
                    break;
                }
            case "disCard":
                {
                    gameView.handleDc(jData);
                    break;
                }
            case "finish":
                {
                    gameView.handleFinishGame(jData);
                    break;
                }
            case "fold":
                {
                    gameView.handleFold(jData);
                    break;
                }
            case "autoDisLastOne":
                {
                    gameView.setStateAutoDisCard(jData);
                    break;
                }
                //case "stable":
                //    {
                //        gameView.handleSTable((string)jData);
                //        break;
                //    }
                //case "vtable":
                //    {
                //        gameView.handleVTable((string)jData);
                //        break;
                //    }
                //case "ctable":
                //    {
                //        gameView.handleCTable((string)jData);
                //        break;
                //    }
                //case "jtable":
                //    {
                //        gameView.handleJTable((string)jData);
                //        break;
                //    }
                //case "ltable":
                //    {
                //        gameView.handleLTable(jData);
                //        break;
                //    }
        }
        }

}
