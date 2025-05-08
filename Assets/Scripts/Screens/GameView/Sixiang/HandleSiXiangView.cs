using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleSiXiangView 
{
    public static void processData(JObject jData)
    {
        var gameView = (HiloView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {

        }
    }
}
