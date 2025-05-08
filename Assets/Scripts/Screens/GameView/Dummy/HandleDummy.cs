using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HandleDummy
{
    public static void processData(JObject jData)
    {
        var gameView = (DummyView) UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];

		Globals.Logging.Log("-=-=EVT Game DUMMY  " + evt);
		UIManager.instance.sendLog(jData.ToString(), false);

		try
		{
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

				case "disCard":
					{
						gameView.handleDiscard(jData);
						break;
					}

				case "draw":
					{
						gameView.handleDraw(jData);
						break;
					}

				case "eat":
					{
						gameView.handleEat(jData);
						break;
					}

				case "meld":
					{
						gameView.handleMeld(jData);
						break;
					}

				case "layoff":
					{
						gameView.handleLayoff(jData);
						break;
					}
				case "show":
					{
						gameView.handleShow(jData);
						break;
					}
				case "finish":
					{
						gameView.handleFinish((JArray)jData["data"], (int)jData["agPot"]);
						break;
					}

				case "stupidCard":
					{
						gameView.handleStupid(jData);
						break;
					}

				case "knockOut":
					{
						gameView.handleKnockOut((JObject)jData["data"]);
						break;
					}

				case "tip":
					{
						gameView.HandlerTip(jData);
						break;
					}

				case "potInfo":
					{
						gameView.handlePotInfo(jData);
						break;
					}

			}
		}catch(System.Exception e)
        {
			Globals.Logging.LogException(e);
        }
    }
}
