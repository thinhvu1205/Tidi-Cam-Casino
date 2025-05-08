using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleKeangView
{
	public static void processData(JObject jData)
	{
		var gameView = (KeangView)UIManager.instance.gameView;
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
			case "bc":
				{
					gameView.handleBc(jData);
					break;
				}
			case "pot":
				{
					gameView.handlePot(jData);
					break;
				}
			case "dc":
				{
					gameView.handleDc(jData);
					break;
				}
			case "finish":
				{
					gameView.handleFinish(jData);
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
