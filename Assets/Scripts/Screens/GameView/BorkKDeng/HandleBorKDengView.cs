using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleBorKDengView
{
	public static void processData(JObject jData)
	{
		var gameView = (BorkKDengView)UIManager.instance.gameView;
		if (gameView == null) return;
		string evt = (string)jData["evt"];
		switch (evt)
		{
			case "startGame":
				{
					gameView.handleStartGame(jData);
					break;
				}
			case "lc":
				{
					gameView.handleLc(jData);
					break;
				}
			case "startBet":
                {
					gameView.handleStartBet(jData);
					break;
				}
			case "bm":
                {
					gameView.handleBm(jData);
					break;
				}
			case "pokpok":
				{
					gameView.handlePokpok(jData);
					break;
				}
			case "timeout":
				{
					gameView.handleTimeOut(jData);
					break;
				}
			case "bc":
				{
					gameView.handleBc(jData);
					break;
				}
			case "finish":
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
			case "leave_dealer":
				{
					gameView.handleLeaveDealer(jData);
					break;
				}
			case "dealer":
				{
					gameView.handleDealer(jData);
					break;
				}
            case "findDealer":
                {
                    gameView.handleFindDealer(jData);
                    break;
                }

            case "uag":
				{
					gameView.handleUAG(jData);
					break;
				}

		}
	}
}
