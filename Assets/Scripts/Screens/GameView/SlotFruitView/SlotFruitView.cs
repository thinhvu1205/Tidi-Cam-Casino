using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using Globals;
public class SlotFruitView : BaseSlotGameView
{
    public static SlotFruitView instance;
    // Start is called before the first frame update
    [SerializeField] public Sprite sprBtnStop, sprBtnFreeSpin;
    [SerializeField] public Image imgBtnSpin;
    public override void HandlerUpdateUserChips(JObject data)
    {
        lbCurrentChips.setValue((long)data["ag"], false);
    }

    protected override void Start()
    {
        base.Start();
        SlotFruitView.instance = this;
        RECT_SIZE = new Vector2(130, 130f);
        BIGWIN_ANIMPATH = "GameView/SlotSpine/Common/Bigwin/skeleton_SkeletonData";
        MEGAWIN_ANIMPATH = "GameView/SlotSpine/Common/Bigwin/skeleton_SkeletonData";
        ANIM_BIGWIN_NAME = "bigwin";
        ANIM_MEGAWIN_NAME = "megawin";
    }

    public override void showAnimChipBay()
    {
        Transform transFrom = lbChipWins.transform;
        Transform transTo = lbCurrentChips.transform.parent.Find("icChip").transform;
        coinFly(transFrom, transTo);
        agPlayer = (long)finishData["AG"];
    }

    public override void setStateBtnSpin()
    {
        base.setStateBtnSpin();
        Debug.Log("setStateBtnSpin:" + gameState);
        animBtnSpin.color = Color.white;
        animBtnSpin.gameObject.SetActive(true);
        if (gameState == GAME_STATE.SPINNING)
        {
            if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
            {
                //animBtnSpin.startingAnimation = "freespin";
                animBtnSpin.gameObject.SetActive(false);
                imgBtnSpin.sprite = sprBtnFreeSpin;
                imgBtnSpin.color = Color.white;
            }
            else if (spintype == SPIN_TYPE.AUTO)
            {
                //animBtnSpin.startingAnimation = "autospin";
                animBtnSpin.gameObject.SetActive(false);
                imgBtnSpin.sprite = sprBtnStop;
                imgBtnSpin.color = Color.white;
            }
            else
            {
                animBtnSpin.startingAnimation = "eng";
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
                    animBtnSpin.gameObject.SetActive(false);
                    imgBtnSpin.sprite = sprBtnFreeSpin;
                    imgBtnSpin.color = Color.white;
                }
                else
                {
                    if (spintype == SPIN_TYPE.AUTO)
                    {
                        //animBtnSpin.startingAnimation = "autospin";
                        animBtnSpin.gameObject.SetActive(false);
                        imgBtnSpin.sprite = sprBtnStop;
                        imgBtnSpin.color = Color.white;
                    }
                    else
                    {
                        animBtnSpin.startingAnimation = "eng";
                        animBtnSpin.color = Color.white;
                        animBtnSpin.gameObject.SetActive(true);

                    }
                }
            }
            else if (gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME)
            {
                animBtnSpin.startingAnimation = "eng";
                animBtnSpin.color = Color.white;
                if (listBetRoom.Count == 0)
                {
                    //animBtnSpin.color = Color.gray;
                }
                else if (agPlayer < totalListBetRoom[currentMarkBet]) //het cmn tien roi.an di.
                {
                    //animBtnSpin.color = Color.gray;
                }
                if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
                {
                    animBtnSpin.gameObject.SetActive(false);
                    imgBtnSpin.sprite = sprBtnFreeSpin;
                    imgBtnSpin.color = Color.white;
                }
            }
        }
        animBtnSpin.Initialize(true);
    }
    protected override void showBigWin()
    {
        base.showBigWin();
        Color colorLine = new Color();
        ColorUtility.TryParseHtmlString("#037CED", out colorLine);
        animEffect.transform.localPosition = new Vector2(0, 0);
        effectContainer.GetComponent<Image>().color = new Color(colorLine.r, colorLine.g, colorLine.b, 0.5f);
        animEffect.transform.localScale = Vector2.one;
    }
    protected override void showMegaWin()
    {
        playSound(Globals.SOUND_SLOT.MEGA_WIN);
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);
        //TextMeshProUGUI lbChipWin = animEffect.transform.Find("bgChip/lbBigWin").GetComponent<TextMeshProUGUI>();
        lbBigWin.transform.parent.gameObject.SetActive(true);
        lbBigWin.gameObject.SetActive(true);
        if (isInFreeSpin == true && isFreeSpin == false) //vua quay het freespin turn cuoi cung;
        {
            Globals.Config.tweenNumberTo(lbBigWin, countTotalAgFreespin, 0, 4.0f);
        }
        else
        {
            Globals.Config.tweenNumberTo(lbBigWin, getInt(finishData, "agWin"), 0, 4.0f);
        }
        //Globals.Config.tweenNumberTo(lbBigWin, 100000, 0, 3.0f);
        animEffect.TrimRenderers();
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(BIGWIN_ANIMPATH);
        animEffect.transform.localScale = new Vector2(0.9f, 0.9f);
        animEffect.transform.localPosition = new Vector2(0, -70);
        animEffect.Initialize(true);
        animEffect.AnimationState.SetAnimation(0, ANIM_BIGWIN_NAME, false);
        animEffect.AnimationState.Complete += delegate
        {
            //effectAnimEndListenter();
            animEffect.AnimationState.SetAnimation(0, ANIM_MEGAWIN_NAME, false);
            animEffect.AnimationState.Complete += delegate
            {
                effectAnimEndListenter();
            };
            effectAnimEndListenter = () =>
            {
                effectContainer.SetActive(false);
                lbBigWin.transform.parent.gameObject.SetActive(false);
                gameState = GAME_STATE.SHOWING_RESULT;
                handleActionResult();
                Color cl = Color.black;
                effectContainer.GetComponent<Image>().color = new Color(cl.r, cl.g, cl.b, 0.5f);
            };
        };

        animEffect.transform.localPosition = new Vector2(0, 0);
        Color colorLine = new Color();
        ColorUtility.TryParseHtmlString("#037CED", out colorLine);
        animEffect.transform.localScale = Vector2.one;
        effectContainer.GetComponent<Image>().color = new Color(colorLine.r, colorLine.g, colorLine.b, 0.5f);

    }
}
