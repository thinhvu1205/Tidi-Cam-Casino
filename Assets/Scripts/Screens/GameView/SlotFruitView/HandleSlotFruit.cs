using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HandleSlotFruit
{
    public static void processData(JObject jData)
    {
        var gameView = (SlotFruitView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];

        Globals.Logging.Log("-=-=EVT Game SlotInCa  " + evt);

        switch (evt)
        {
            case "slotViews":
                {
                    gameView.handleSpin(jData);
                    break;
                }
        }
    }
}
