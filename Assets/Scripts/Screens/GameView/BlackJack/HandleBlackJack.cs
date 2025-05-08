using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HandleBlackJack
{
    public static void processData(JObject jData)
    {
        var gameView = (BlackJackView)UIManager.instance.gameView;
        if (gameView == null) return;
        string evt = (string)jData["evt"];
        switch (evt)
        {
            case "stable":
                {
                    gameView.handleSTable((string)jData);
                    break;
                }
            case "vtable":
                {
                    gameView.handleVTable((string)jData);
                    break;
                }
            case "ctable":
                {
                    gameView.handleCTable((string)jData);
                    break;
                }
            case "ltable":
                {
                    gameView.handleLTable(jData);
                    break;
                }
            case "start":
                {
                    gameView.TimeAction(jData);
                    break;
                }
            case "insuranceTime":
                {
                    gameView.BuyInsure(jData);
                    break;
                }
            case "playerInsured":
                {
                    gameView.handleBuyInSurePlayer(jData);
                    break;
                }
            case "finish":
                {
                    gameView.handleFinishGame(jData);
                    break;
                }
            case "cards":
                {
                    gameView.handlelc(jData);
                    break;
                }
            case "decisionTurn":
                {
                    gameView.handleTurnPlayer(jData);
                    break;
                }
            case "prepareToStart":
                {
                    gameView.prepareToStart(jData);
                    break;
                }
            case "playerStood":
            case "playerHit":
            case "playerDoubled":
            case "playerSplit":
                {
                    gameView.handleActionPlayer(evt, jData);
                    break;
                }

            case "irFinish":
                {
                    gameView.handleIrFinish(jData);
                    break;
                }
            case "autoExit":
                {
                    gameView.handleAutoExit(jData);
                    break;
                }
            case "doubleErr":
                {
                    // {"evt":"doubleErr","data":"{\"message\":\"notEnoughChip\"}"}
                    UIManager.instance.showToast(Globals.Config.getTextConfig("txt_koduchip").ToUpper());
                    gameView.handleActionErr();
                    break;
                }
            case "splitErr":
                {
                    //{"evt":"splitErr","data":"{\"message\":\"notEnoughChip\"}"}
                    UIManager.instance.showToast(Globals.Config.getTextConfig("txt_koduchip").ToUpper());
                    gameView.handleActionErr();
                    break;
                }
            case "betAccepted":
                {
                    gameView.handleBet(jData);
                    break;
                }
        }
    }
}
