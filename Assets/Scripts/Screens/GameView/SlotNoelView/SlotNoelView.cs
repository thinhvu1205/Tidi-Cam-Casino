using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using Newtonsoft.Json.Linq;
using Globals;
public class SlotNoelView : BaseSlotGameView
{
    public override void HandlerUpdateUserChips(JObject data)
    {
        lbCurrentChips.setValue((long)data["ag"], false);
    }
    public static SlotNoelView instance;
    public override void setStateBtnSpin()
    {
        base.setStateBtnSpin();
        Globals.Logging.Log("setStateBtnSpin:" + gameState);
        if (gameState == GAME_STATE.SPINNING)
        {
            if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
            {
                animBtnSpin.startingAnimation = "freespin";
                //animBtnSpin.color = Color.gray;
            }
            else if (spintype == SPIN_TYPE.AUTO)
            {
                animBtnSpin.startingAnimation = "stop";
            }
            else
            {
                animBtnSpin.startingAnimation = "autospin";
                animBtnSpin.color = Color.gray;
            }
        }
        else
        {
            animBtnSpin.color = Color.white;
            if (gameState == GAME_STATE.SHOWING_RESULT)
            {
                if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
                {
                    animBtnSpin.startingAnimation = "freespin";
                }
                else
                {
                    if (spintype == SPIN_TYPE.AUTO)
                    {
                        animBtnSpin.startingAnimation = "stop";
                    }
                    else
                    {
                        animBtnSpin.startingAnimation = "autospin";
                    }
                }
            }
            else if (gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME)
            {
                animBtnSpin.startingAnimation = "autospin";
                Debug.Log("listBetRoom.Count=" + listBetRoom.Count);
                if (listBetRoom.Count == 0)
                {
                    animBtnSpin.color = Color.white;
                }
                else if (agPlayer < totalListBetRoom[currentMarkBet] && isFreeSpin == false) //het cmn tien roi.an di.
                {
                    animBtnSpin.color = Color.gray;
                }
                if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
                {
                    animBtnSpin.startingAnimation = "freespin";
                    //animBtnSpin.color = Color.white;
                }
            }
        }
        animBtnSpin.Initialize(true);
    }
    public override void showAnimChipBay()
    {
        Transform transFrom = lbChipWins.transform;
        Transform transTo = lbCurrentChips.transform.parent.Find("icChip").transform;
        coinFly(transFrom, transTo);
    }
}
