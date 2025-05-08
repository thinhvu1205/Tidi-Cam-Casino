using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleBandarQQ 
{
	public static void processData(JObject jData)
	{
		var gameView = (BandarQQView)UIManager.instance.gameView;
		if (gameView == null) return;
		string evt = (string)jData["evt"];
		switch (evt)
		{
			case "startgame":
				{
					gameView.handleStartGame(jData);
                    break;
				}
			case "bet":
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
			case "history":
				{
					gameView.setHistory(jData);
					break;
				}
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
