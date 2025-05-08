using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DummyButton : MonoBehaviour
{
    //[SerializeField]
    public Button btnEat, btnDraw, btnMeld, btnLayoff, btnDiscard, btnKnockOut, btnOk, btnCancel, btnDarkKnock;
    [SerializeField]
    GameObject nodeState1, nodeState2, nodeCheck, dummyWarn;
    int currentState = 0;
    [SerializeField]
    TextMeshProUGUI confirmSprite;
    [SerializeField]
    GameObject ico2tQb;

    //[SerializeField]
    //List<Sprite> listSprite = new List<Sprite>();

    [SerializeField]
    DummyView gameView;

    void OnEnable()
    {
        Globals.Logging.Log("DummyButton OnEnable");
        btnEat.interactable = false;
        btnMeld.interactable = false;
        btnLayoff.interactable = false;

        btnKnockOut.gameObject.SetActive(false);
        btnDarkKnock.gameObject.SetActive(false);
    }

    public void onHide()
    {
        gameObject.SetActive(false);
    }

    public void clearButton(List<Button> listBtn)
    {
        listBtn.ForEach(btn =>
        {
            btn.interactable = false;
            btn.gameObject.SetActive(false);
        });
    }
    public void showButtonByState(int state)
    {
        // 0 - eat draw
        // 1 = meld layoff discard
        // 2 = ok cancel
        dummyWarn.SetActive(false);
        nodeState1.SetActive(state == 0);
        nodeState2.SetActive(state == 1);
        nodeCheck.SetActive(state == 2);
    }

    public void disableAllButton()
    {

    }

    public void onShow(bool isState1 = true)
    {
        gameObject.SetActive(true);

        btnEat.interactable = false;
        btnMeld.interactable = false;
        btnLayoff.interactable = false;

        btnKnockOut.gameObject.SetActive(false);
        btnDarkKnock.gameObject.SetActive(false);

        //node.zIndex = GAME_ZORDER.Z_CARD;
        //transform.SetSiblingIndex((int)Globals.ZODER_VIEW.CARD);
        if (isState1)
        {
            currentState = 0;
            showButtonByState(0);
            btnDraw.interactable = true;
        }
        else
        {
            currentState = 1;
            showButtonByState(1);
        }

        gameView.checkAllowButton();
        gameView.checkAllowButton(true);
    }

    public void onClickEnterMode(int mode)
    {
        Globals.Logging.Log("onClickEnterMode  " + mode);
        // 1 eat, 2 meld, 3 layoff, 4 knock out, 5 show special
        //var mode = parseInt(data)
        gameView.onEnterMode(mode);
        // show ok cancel button
        showButtonByState(2);
        gameView.checkAllowButton();
        var lb = "";

        ico2tQb.SetActive(false);
        switch (mode - 1)
        {
            case 0:
                lb = Globals.Config.getTextConfig("txt_dummy_eat");
                break;
            case 1:
                lb = Globals.Config.getTextConfig("txt_dummy_meld");
                break;
            case 2:
                lb = Globals.Config.getTextConfig("txt_dummy_deposit");
                break;
            case 3:
                lb = Globals.Config.getTextConfig("txt_dummy_knockout");
                break;
            case 4:
                lb = "";//Globals.Config.getTextConfig("txt_dummy_show2q");
                btnOk.interactable = true;
                ico2tQb.SetActive(true);
                break;
            default:
                lb = Globals.Config.getTextConfig("txt_ok");
                break;
        }
        confirmSprite.text = lb;
    }


    public void onClickDraw()
    {
        SocketSend.sendDummyDraw();
        onHide();
    }

    public void onClickDiscard()
    {
        var arr = gameView.selectedCards;
        if (arr.Count > 1 || arr.Count == 0) return;
        SocketSend.sendDummyDiscard(arr[0].code);
        onHide();
    }

    public void onClickCancel()
    {
        var isShowSpecial = gameView.currenMode == 5 ? true : false;
        btnDiscard.interactable = false;
        gameView.onEnterMode(0);
        showButtonByState(currentState);
        Globals.Logging.Log("!==> isShowSpecial" + isShowSpecial);
        if (isShowSpecial) gameView.checkAllowButton(true);
    }

    public void onClickOk()
    {
        var vectorCard = gameView.selectedCards;
        var litsDumped = gameView.selectedDumps;
        Globals.Logging.Log("!=> click send" + gameView.currenMode);

        switch (gameView.currenMode)
        {
            case 1:
                {
                    // eat
                    Globals.Logging.Log("!=> click send mode 1 - eat");
                    JArray arrHand = new JArray();
                    JArray arrEat = new JArray();
                    vectorCard.ForEach(c =>
                    {
                        arrHand.Add(c.code);
                    });
                    litsDumped.ForEach(c =>
                    {
                        arrEat.Add(c.code);
                    });
                    SocketSend.sendDummyEat(arrHand, arrEat);
                    break;
                }
            case 2:
                {
                    // meld
                    Globals.Logging.Log("!=> click send mode 2 - meld");
                    JArray arrHand = new JArray();
                    vectorCard.ForEach(c =>
                    {
                        arrHand.Add(c.code);
                    });
                    SocketSend.sendDummyMeld(arrHand);
                    break;
                }
            case 3:
                // layoff
                Globals.Logging.Log("!=> click send mode 3 - layoff");

                var data = gameView.getDataLayoff();

                if (data == null) return;
                SocketSend.sendDummyLayoff(data);
                break;
            case 4:
                // knockout
                Globals.Logging.Log("!=> click send mode 4 - knockout");

                gameView.sendDataKnockOut();
                break;
            case 5:
                // show special card
                Globals.Logging.Log("!=> click send mode 5 - show special card");
                SocketSend.sendDummyShow();
                break;
        }
        gameView.onEnterMode(0);
        onHide();
    }

    public void showDummyWarn(bool isShow)
    {
        if (isShow && currentState == 1)
        {
            //if (dummyWarn.activeInHierarchy) return;
            dummyWarn.SetActive(true);
            var pos = dummyWarn.transform.parent.InverseTransformPoint(btnDiscard.transform.parent.TransformPoint(btnDiscard.transform.localPosition));
            var poss = dummyWarn.transform.localPosition;
            poss.x = pos.x;
            dummyWarn.transform.localPosition = poss;
            //dummyWarn.DOKill();
            //dummyWarn.GetComponent<CanvasGroup>().alpha = 0;
            //dummyWarn.stopAllActions();
            //dummyWarn.active = true;
            //dummyWarn.setScale(0);
            //dummyWarn.x = btnDiscard.node.x + 93;
            //dummyWarn.runAction(cc.spawn(
            //    cc.scavaro(0.2, 1).easing(cc.easeBackOut()),
            //    cc.fadeIn(0.2),


            //))
        }
        else
        {
            dummyWarn.SetActive(false);
            //if (!dummyWarn.activeInHierarchy) return;
            //dummyWarn.stopAllActions();
            //dummyWarn.runAction(cc.sequence(
            //    cc.spawn(
            //        cc.scavaro(0.1, 0),
            //        cc.fadeOut(0.1),


            //    ),
            //    cc.callFunc(() =>
            //    {
            //        dummyWarn.active = false;
            //    })
            //))

        }
    }

    public bool getIsShow()
    {
        return gameObject.activeInHierarchy;
    }
}
