using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Newtonsoft.Json.Linq;
using DG.Tweening;
using Spine.Unity;
using System;
using Unity.Mathematics;
using Globals;

public class BaseSlotGameView : GameView
{
    // Start is called before the first frame update

    [SerializeField]
    public Button btnSpin;

    [SerializeField]
    Button btnMaxBet;

    [SerializeField]
    Button btnPlusBet;

    [SerializeField]
    Button btnMinusBet;

    [SerializeField]
    protected Image bgSpin;

    [SerializeField]
    protected TextMeshProUGUI lbInfoSession, lbCurrentBet;
    [SerializeField]
    protected TextNumberControl lbChipWins;

    [SerializeField]
    protected TextMeshProUGUI lbStateBet; //Bet-Max Bet

    [SerializeField]
    protected TextNumberControl lbCurrentChips; //Bet-Max Bet

    [SerializeField]
    protected TextMeshProUGUI lbFreespinLeft;

    [SerializeField]
    protected TextMeshProUGUI lbStateWin;

    [SerializeField]
    protected Image sprStateWin; //Totalwin-lastwin


    [SerializeField]
    GameObject line2Pr; //Totalwin-lastwin


    [SerializeField]
    protected TextMeshProUGUI lbBigWin;


    [SerializeField]
    protected List<Sprite> listSprStateWin = new List<Sprite>(); //Totalwin-lastwin
    [SerializeField]
    List<Sprite> listSprIcon = new List<Sprite>(); //Totalwin-lastwin

    [SerializeField]
    public List<CollumSpinController> listCollum = new List<CollumSpinController>();
    [SerializeField]
    public SkeletonGraphic animBtnSpin;

    [SerializeField]
    public SkeletonGraphic animBgFreeSpin;

    [SerializeField]
    public SkeletonGraphic animFreespinNum;
    [SerializeField]
    public SkeletonGraphic animEffect;
    [SerializeField]
    public SkeletonGraphic animNearFreeSpin;
    [SerializeField]
    public GameObject lineContainer;
    [SerializeField]
    public GameObject effectContainer;
    [SerializeField]
    public GameObject paylineInfoContainer, collumContainer;
    [SerializeField]
    GameObject rulePr;
    [SerializeField]
    protected GameObject animIconPr;

    [HideInInspector]
    public List<string> listColors = new();
    protected long agPlayer = 0;
    public int currentMarkBet = 0;
    protected int totalLineWin = 0;
    [HideInInspector]
    public int currentIndexStop = -1;
    protected float timeHoldSpin = 0f;
    protected int winType = 0;
    public int freespinLeft = 0;
    protected int currentLineEffect = 0;
    public int countScatter = 0;

    [HideInInspector]
    public bool isSpining = false;
    protected bool isHoldSpin = false;
    public bool isFreeSpin = false;
    protected bool isFiveOfaKind = false;
    protected bool isWinScatter = false;
    protected bool isGetFreeSpin = false;//Phi?n quay ?? ?c 3 scatter
    public bool isInFreeSpin = false;
    protected int countTotalAgFreespin = 0;

    [HideInInspector]
    protected List<int> listBetRoom = new List<int>();
    protected List<int> totalListBetRoom = new List<int>();
    public List<int> winningLines = new List<int>();
    protected List<int> listMarkbet = new List<int>();
    protected List<List<int>> slotViews = new List<List<int>>();
    protected List<GameObject> listLineRect = new List<GameObject>();
    protected List<GameObject> listLineStraight = new List<GameObject>();

    [HideInInspector]
    protected JArray linesDetail = new JArray();
    protected JArray payLines = new JArray();


    protected Sequence seqShowAllLine;
    protected List<Sequence> seqShowOneByOne = new List<Sequence>();

    protected List<GameObject> animIconPool = new List<GameObject>();
    protected List<TweenCallback> listActionHandleSpin = new List<TweenCallback>();//list action show  result c?a spin winscatter,freespin,5ofAkind,Bigwin,Megawin..v.v.

    protected string BIGWIN_ANIMPATH = "GameView/SlotSpine/Noel/big_megawinNoel/skeleton_SkeletonData";
    protected string MEGAWIN_ANIMPATH = "GameView/SlotSpine/Noel/big_megawinNoel/skeleton_SkeletonData";
    protected string FREESPIN_ANIMPATH = "GameView/SlotSpine/freespin/skeleton_SkeletonData";
    protected string FIVEOFAKIND_ANIMPATH = "GameView/SlotSpine/FiveOfAKind/skeleton_SkeletonData";
    protected string ANIM_BIGWIN_NAME = "big";
    protected string ANIM_MEGAWIN_NAME = "mega";
    protected string ANIM_HUGEWIN_NAME = "hugethai";
    protected string ANIM_BG_FREESPIN = "GameView/SlotSpine/freespin/vienBg/skeleton_SkeletonData";
    protected int TYPE_BIGWIN = 1;
    protected int TYPE_MEGA = 2;
    protected int TYPE_HUGEWIN = 3;
    protected int singleLineBet;
    protected bool isSendingSpin = false;
    protected int indexCheck3rdScatter = 3;
    protected List<int> listPosXCollum = new List<int> { -289, -143 };

    protected System.Action effectAnimEndListenter;

    [HideInInspector]
    public enum SPIN_TYPE
    {
        NORMAL,
        AUTO,
        FREE_NORMAL,
        FREE_AUTO
    }
    public enum GAME_STATE
    {
        PREPARE,
        SPINNING,
        SHOWING_RESULT,
        JOIN_GAME
    }

    [HideInInspector]
    public SPIN_TYPE spintype = SPIN_TYPE.NORMAL;
    public GAME_STATE gameState = GAME_STATE.JOIN_GAME;



    protected JObject finishData, dataCtable = null;
    protected Vector2 RECT_SIZE = new Vector2(200, 170);

    [SerializeField]
    Image coinEffectPrefab;
    public void DoClickShop()
    {
        UIManager.instance.openShop();
    }
    protected override void Start()
    {
        base.Start();
        paylineInfoContainer.SetActive(false);
        listColors = new List<string>
        {
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
            "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
            "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
            "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
            "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE"
        };
        // listColors.AddRange(listColors);
        //animEffect.AnimationState.Complete += delegate
        //{
        //    if (effectAnimEndListenter != null)
        //        effectAnimEndListenter();
        //};
    }
    public override void onLeave()
    {
        User.userMain.AG = agPlayer;
        base.onLeave();
    }
    public GameObject getAnimIcon(Transform parent)
    {
        GameObject animIcon;
        if (animIconPool.Count == 0)
        {
            GameObject go = Instantiate(animIconPr, parent);

            animIconPool.Add(go);
        }
        animIcon = animIconPool[0];
        //animIcon.GetComponent<SkeletonGraphic>().allowMultipleCanvasRenderers = true;
        //animIcon.GetComponent<SkeletonGraphic>().TrimRenderers();
        animIconPool.RemoveAt(0);
        animIcon.SetActive(true);
        return animIcon;
    }
    public void removeAnimIcon(GameObject animIcon)
    {
        animIcon.SetActive(false);
        animIconPool.Add(animIcon);
    }

    List<Image> lsCoinPool = new List<Image>();

    protected void coinFly(Transform transFrom, Transform transTo, int count = 10, float timeInterval = 0.1f)
    {

        var parentCoin = transform.Find("CoinLayer");
        for (var i = 0; i < 5; i++)
        {
            DOTween.Sequence().AppendInterval(i * 0.1f).AppendCallback(() =>
            {
                Image obj;
                if (lsCoinPool.Count > 0)
                {
                    obj = lsCoinPool[0];
                    lsCoinPool.RemoveAt(0);
                }
                else
                {
                    obj = Instantiate(coinEffectPrefab, parentCoin).GetComponent<Image>();

                }
                obj.gameObject.SetActive(true);
                obj.GetComponent<Animator>().Play("idle");
                effectCoinFly(obj, transFrom, transTo);
            });

        }

    }

    void effectCoinFly(Image coinEffect, Transform transFrom, Transform transTo)
    {

        coinEffect.transform.position = transFrom.position;
        coinEffect.transform.DOJump(transTo.position, 1, 1, 2).SetEase(Ease.InOutCubic);
        var cc = coinEffect.color;
        cc.a = .2f;
        coinEffect.color = cc;
        coinEffect.DOFade(1, .75f);

        DOTween.Sequence()
            .AppendInterval(.5f)
            .Append(coinEffect.transform.DOScale(2, 0.25f))
            .AppendInterval(0.15f)
            .Append(coinEffect.transform.DOScale(1, 0.25f))//0.85
            .AppendInterval(0.6f)
            .Append(coinEffect.DOFade(0, .25f)).AppendCallback(() =>
            {
                coinEffect.gameObject.SetActive(false);
                lsCoinPool.Add(coinEffect);
                //Destroy(coinEffect.gameObject);
            });
    }

    // Update is called once per frame
    public new void Update()
    {
        if (isHoldSpin)
        {
            timeHoldSpin += Time.deltaTime;
            if (timeHoldSpin > 1.3f)
            {
                if (agPlayer < totalListBetRoom[currentMarkBet])
                {
                    lbInfoSession.text = Config.getTextConfig("msg_warrning_send");
                    return;
                }
                spintype = SPIN_TYPE.AUTO;
                setSpinType();
                if (gameState != GAME_STATE.SPINNING)
                {
                    resetSlotView();
                }

                onClickSpin();
                isHoldSpin = false;
            }
        }
    }
    protected void sendSpin()
    {
        if (!isSendingSpin)
        {
            isSendingSpin = true;
            gameState = GAME_STATE.SPINNING;
            SocketSend.sendSpinSlot(listMarkbet[currentMarkBet], isFreeSpin);
        }
    }
    public virtual void onClickSpin()
    {
        SoundManager.instance.soundClick();
        if (listCollum.Count < 5)
        {
            return;
        }
        Config.Vibration();
        if (spintype == SPIN_TYPE.NORMAL)
        {
            if (gameState == GAME_STATE.SPINNING)// dang spin normal thi thoi ko cho click
            {
                if (spintype == SPIN_TYPE.AUTO)
                {
                    spintype = SPIN_TYPE.NORMAL;
                    setStateBtnSpin();
                }
                return;
            }
            else if (gameState == GAME_STATE.SHOWING_RESULT || gameState == GAME_STATE.PREPARE) //dang show result ma click Spin(stop) thi kill het action;
            {
                //resetSlotView();
                handleActionResult();
                if (gameState == GAME_STATE.SHOWING_RESULT)
                {
                    return;
                }
            }
        }
        else //auto
        {
            if (gameState == GAME_STATE.SPINNING)
            {
                if (freespinLeft == 0)//dang auto bt k co freespin thi moi doi trang thai sang normal
                {
                    spintype = SPIN_TYPE.NORMAL;
                    setStateBtnSpin();
                    return;
                }
                else //.Con k co freespin la auto;
                {
                    return;
                }
            }
        }

        playSound(SOUND_SLOT.WHEEL);
        lbInfoSession.text = Config.formatStr(Config.getTextConfig("txt_playing_lines"), payLines.Count);
        if (!isFreeSpin)
        {
            //Config.tweenNumberToNumber(lbCurrentChips, agPlayer - Convert.ToInt64(listBetRoom[currentMarkBet]), agPlayer);
            if (currentMarkBet >= 0 && currentMarkBet < listBetRoom.Count)
            {
                lbCurrentChips.setValue(agPlayer - listBetRoom[currentMarkBet], true, 0.5f, "currentChip:");
                agPlayer -= (long)listBetRoom[currentMarkBet];
            }
        }
        else
        {
            if (animBgFreeSpin != null)
            {
                animBgFreeSpin.gameObject.SetActive(true);
                if (Config.curGameId == (int)GAMEID.SLOT_INCA)
                {
                    animBgFreeSpin.transform.localScale = Vector2.one * 1.4f;
                }
                if (animBgFreeSpin.skeletonDataAsset == null)
                {
                    animBgFreeSpin.skeletonDataAsset = UIManager.instance.loadSkeletonData(ANIM_BG_FREESPIN);
                }
                animBgFreeSpin.Initialize(true);
                animBgFreeSpin.AnimationState.SetAnimation(0, "animation", true);
                lbFreespinLeft.text = Config.getTextConfig("txt_freespinRM") + ": " + freespinLeft;
            }
        }
        for (int i = 0; i < listCollum.Count; i++)
        {
            listCollum[i].startSpin(this);
        }
        sendSpin();
        setStateBtnSpin();
        if (!isInFreeSpin && isFreeSpin)
        {
            Debug.Log("Vao 1");
            if (lbStateWin != null)
            {
                lbStateWin.text = Config.getTextConfig("txt_total_win");
            }
            else
            {
                sprStateWin.sprite = listSprStateWin[1];
                sprStateWin.SetNativeSize();
            }

            lbChipWins.ResetValue();
        }
        else if (isInFreeSpin && isFreeSpin)
        {
            Debug.Log("Vao 2");
            if (lbStateWin != null)
            {
                lbStateWin.text = Config.getTextConfig("txt_total_win");
            }
            else
            {
                sprStateWin.sprite = listSprStateWin[1];
                sprStateWin.SetNativeSize();
            }

        }
        else
        {
            Debug.Log("Vao 3");
            if (lbStateWin != null)
            {
                lbStateWin.text = Config.getTextConfig("txt_last_win");
            }
            else
            {
                sprStateWin.sprite = listSprStateWin[2];
                sprStateWin.SetNativeSize();
            }

        }
    }
    public void setCurrentMarkBet()
    {
        Debug.Log("setCurrentMarkBet:" + currentMarkBet);
        if (currentMarkBet >= listBetRoom.Count && listBetRoom.Count > 0)
        {
            currentMarkBet = listBetRoom.Count - 1;
        }
        if (isFreeSpin || isInFreeSpin)
        {
            if (singleLineBet > 0 && gameState == GAME_STATE.JOIN_GAME)
            {
                currentMarkBet = listMarkbet.IndexOf(singleLineBet);
            }
        }
        lbCurrentBet.text = Config.FormatMoney2(totalListBetRoom[currentMarkBet], true);
    }
    protected virtual void refreshBetRoomAndMarkbet()
    {
        Debug.Log("refreshBetRoomAndMarkbet isInFreeSpin=" + isInFreeSpin);
        if (!isInFreeSpin || (isInFreeSpin && !isFreeSpin))
        {
            agPlayer = (long)finishData["AG"];
            listBetRoom = getListBetRoom();
            setCurrentMarkBet();
        }
    }
    public virtual void onStopSpin() //dung het cot cuoi cung.bat dau check data show result c?c th?
    {
        freespinLeft = (int)finishData["freeSpinLeft"];
        isWinScatter = checkWinScatter();
        if (freespinLeft <= 0)
        {
            if (animBgFreeSpin != null) animBgFreeSpin.gameObject.SetActive(false);
            if ((winType == 0 && !isInFreeSpin))////spin thuong hoac spin turn do dc frespin nhung freespin nay ko trong luc dang freespin. thi moi show update tien con khong thi het freespin moi update
            {
                //Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer, 0.5f);
                //lbCurrentChips.setValue(getLong(finishData, "AG"), true);
            }
        }
        if (!isInFreeSpin) //khong co freespin
        {
            if ((long)finishData["agWin"] != 0)
            {
                lbChipWins.setValue((long)finishData["agWin"], true);
                if (lbStateWin != null) lbStateWin.text = Config.getTextConfig("txt_win");
                else
                {
                    sprStateWin.sprite = listSprStateWin[0];
                    sprStateWin.SetNativeSize();
                }
            }
        }
        else if (isInFreeSpin && isFreeSpin == false) //freespin turn cuoi cung
        {
            countTotalAgFreespin += (int)finishData["agWin"];
            TweenCallback acShowTotalWinFreeSpin = () =>
            {
                lbChipWins.setValue(countTotalAgFreespin, true);
                if (lbStateWin != null) lbStateWin.text = Config.getTextConfig("txt_total_win");
                else
                {
                    sprStateWin.sprite = listSprStateWin[1];
                    sprStateWin.SetNativeSize();
                }
                showAnimChipBay();
                //Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer, 0.5f);
                lbCurrentChips.setValue((long)finishData["AG"], true);
                handleActionResult();
            };
            if (countTotalAgFreespin > payLines.Count * totalListBetRoom[currentMarkBet]) winType = 1;
            if (countTotalAgFreespin > 50 * totalListBetRoom[currentMarkBet]) winType = 2;
            if (countTotalAgFreespin > 0) listActionHandleSpin.Add(acShowTotalWinFreeSpin);
        }
        else //dang trong freespin
        {
            countTotalAgFreespin += (int)finishData["agWin"];
            lbChipWins.setValue(countTotalAgFreespin, true);
        }
        TweenCallback acCheckNextSpin = () =>
        {//dang quay thuong auto ma dc freespin-> dung lai, dang quay freespin ma dc freespin->quay tiep.
            setSpinType();
            refreshBetRoomAndMarkbet();
            if (spintype == SPIN_TYPE.AUTO || spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
            {//check luot tiep theo neu dang auto hoac dang trong freesin
                resetSlotView();
                if ((spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL || listBetRoom.Count > 0) && !isGetFreeSpin)
                    onClickSpin();
                else if (isGetFreeSpin || isInFreeSpin == true) onClickSpin(); //dc freespin trong luc freepsin ->quay tiep
                else setStateBtnSpin();
            }
            else
            {
                resetSlotView();
                setStateBtnSpin();
            }
        };
        TweenCallback acShowOneWinLine = () =>
        {
            gameState = GAME_STATE.SHOWING_RESULT;
            if (!isFreeSpin)
            {
                lbCurrentChips.setValue((long)finishData["AG"], true);
                refreshBetRoomAndMarkbet();
                showAnimChipBay();
                setStateBtnSpin();
            }
            showOneByOneLine();
        };
        ///------------------CHECK SHOW WIN FREESPIN--------------------//
        if (isWinScatter)
        {
            TweenCallback acShowWinScatter = () => { showWinScatter(); };
            listActionHandleSpin.Add(acShowWinScatter);
        }
        ///------------------CHECK SHOW FIVE OF A KIND--------------------//
        if (isFiveOfaKind)
        {
            TweenCallback acShowFiveOfAkind = () => { showFiveOfaKind(); };
            listActionHandleSpin.Add(acShowFiveOfAkind);
        }
        ///------------------CHECK SHOW FREESPIN--------------------//
        if (isGetFreeSpin)
        {
            TweenCallback acShowFreeSpin = () => { showFreeSpin(); };
            listActionHandleSpin.Add(acShowFreeSpin);
        }
        ///------------------CHECK SHOW ALL LINE--------------------//=
        if (winningLines.Count > 0)
        {
            TweenCallback acShowAllWinLine = () => { showAllWinline(); };
            listActionHandleSpin.Add(acShowAllWinLine);
        }
        ///------------------CHECK SHOW TYPE WIN--------------------//
        if (winType == TYPE_BIGWIN)
        {
            TweenCallback acShowBigWin = () => { showBigWin(); };
            if (!isFreeSpin) listActionHandleSpin.Add(acShowBigWin);
        }
        else if (winType == TYPE_HUGEWIN)
        {
            TweenCallback acShowHugeWin = () => { showHugeWin(); };
            if (!isFreeSpin) listActionHandleSpin.Add(acShowHugeWin);
        }
        else if (winType == TYPE_MEGA)
        {
            TweenCallback acShowMegaWin = () => { showMegaWin(); };
            if (!isFreeSpin) listActionHandleSpin.Add(acShowMegaWin);
        }
        ///------------------CHECK SHOW ONE BY ONE--------------------//
        if (winningLines.Count > 0 || getInt(finishData, "agWin") > 0) //co line win va quay thuong thi moi show an tung line
        {
            TweenCallback acShowAnimChipBay = () =>
            {
                //Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer, 0.5f);
                lbCurrentChips.setValue(getLong(finishData, "AG"), true);
                refreshBetRoomAndMarkbet();
                showAnimChipBay();
                handleActionResult();
            };
            if (spintype == SPIN_TYPE.NORMAL)
            {
                if (freespinLeft == 0) listActionHandleSpin.Add(acShowOneWinLine);
                if (isGetFreeSpin && !isInFreeSpin) listActionHandleSpin.Add(acShowAnimChipBay);
            }
            else if (spintype == SPIN_TYPE.AUTO || spintype == SPIN_TYPE.FREE_AUTO)
            {
                if (winningLines.Count == 1) listActionHandleSpin.Add(() => { showOneByOneLine(); });
                if (!isInFreeSpin) listActionHandleSpin.Add(acShowAnimChipBay);
            }
        }
        listActionHandleSpin.Add(acCheckNextSpin);
        handleActionResult();
    }

    protected void handleActionResult()
    {
        if (listActionHandleSpin.Count > 0)
        {
            DOTween.Sequence().AppendCallback(listActionHandleSpin[0]);
            listActionHandleSpin.RemoveAt(0);
        }
    }
    public virtual void showAnimChipBay()
    {

    }
    protected virtual void resetSlotView()
    {
        Debug.Log("ResetSlovView:" + gameState);
        setDarkAllItem(false);
        seqShowAllLine.Kill();
        foreach (Sequence seq in seqShowOneByOne)
        {
            seq.Kill();
        }
        foreach (GameObject lineRect in listLineRect)
        {
            Destroy(lineRect);
        }
        listLineRect.Clear();
        foreach (GameObject lineStraight in listLineStraight)
        {
            Destroy(lineStraight);
        }
        lbInfoSession.gameObject.SetActive(true);
        lbInfoSession.text = Config.formatStr(Config.getTextConfig("txt_playing_lines"), payLines.Count);
        paylineInfoContainer.SetActive(false);
        listLineStraight.Clear();
        gameState = GAME_STATE.PREPARE;

        isFiveOfaKind = false;
        isWinScatter = false;
        listActionHandleSpin.Clear();
        slotViews.Clear();
        countScatter = 0;

        if (finishData != null)
        {
            if (isInFreeSpin && !isFreeSpin)
            {
                countTotalAgFreespin = 0;
            }
        }
        setStateBtnSpin();
        if (freespinLeft > 0)
        {
            if (Config.curGameId == (int)GAMEID.SLOTTARZAN)
            {
                lbFreespinLeft.gameObject.SetActive(true);
                lbFreespinLeft.text = freespinLeft.ToString();
            }
            else
            {
                animFreespinNum.gameObject.SetActive(true);
                lbFreespinLeft.text = Config.getTextConfig("txt_freespinRM") + ": " + freespinLeft;
            }
        }
        else
        {
            if (animBgFreeSpin != null)
            {
                animBgFreeSpin.gameObject.SetActive(false);
            }
            if (Config.curGameId != (int)GAMEID.SLOTTARZAN)
            {
                animFreespinNum.gameObject.SetActive(false);
            }
            else
            {
                lbFreespinLeft.gameObject.SetActive(false);
            }
        }
    }
    public virtual void setStateBtnSpin()
    {

    }
    protected virtual void checkFiveOfAKind()
    {
        for (int i = 0, l = winningLines.Count; i < l; i++)
        {
            if (!isFiveOfaKind)
            {
                List<int> listIdInLine = new List<int>();
                List<int> lineWinID = getPaylineWithID(winningLines[i]);
                int numberItem = 0;
                for (int j = 0; j < lineWinID.Count; j++)
                {
                    JArray resultView = (JArray)finishData["slotViews"];
                    List<int> collumView = resultView[j].ToObject<List<int>>();
                    listIdInLine.Add(collumView[lineWinID[j]]);
                    numberItem = checkNumberOfItemInLine(listIdInLine);
                    if (numberItem == 5)
                    {
                        isFiveOfaKind = true;
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }
    }
    public override void handleCTable(string data)
    {
        //JObject dataFake = JObject.Parse("{\"evt\":\"ctable\",\"data\":{\"N\":\"Poker[318]\",\"M\":10,\"ArrP\":[{\"id\":8240,\"N\":\"hienndm\",\"Url\":\"\",\"AG\":123,\"LQ\":0,\"VIP\":3,\"isStart\":true,\"IK\":0,\"sIP\":\"10.148.0.4\",\"Av\":4,\"FId\":0,\"GId\":0,\"UserType\":1,\"TotalAG\":0,\"timeToStart\":0,\"CO\":0.0,\"CO0\":0.0}],\"Id\":318,\"V\":0,\"S\":1,\"issd\":true,\"views\":[[2,7,8],[7,0,9],[6,0,3],[0,6,1],[2,7,3]],\"payLine\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[0,0,1,2,2],[2,2,1,0,0],[1,0,1,2,1],[1,2,1,0,1],[1,0,0,1,0],[1,2,2,1,2],[0,1,0,0,1],[2,1,2,2,1],[0,2,0,2,0],[2,0,2,0,2],[1,0,2,0,1],[1,2,0,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,2,2,2,0]],\"payTable\":[[0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0,2,2,2,10,2],[5,5,5,10,10,10,15,15,25,25,50,250,5],[25,25,25,50,50,50,75,75,125,125,250,2500,0],[125,125,125,250,250,250,500,500,750,750,1250,10000,0]],\"freeSpinCount\":1,\"level\":{\"levelUser\":0,\"curLevelExp\":0,\"maxLevelExp\":0,\"agUser\":123},\"MarkBet\":[1,5,50,500,2500,5000,10000,25000,50000,100000,250000,500000],\"singleLineBet\":5000}}");
        //dataCtable = (JObject)dataFake["data"];
        dataCtable = JObject.Parse(data);
        payLines = (JArray)dataCtable["payLine"];
        for (int i = 1; i < 5; i++)
        {
            CollumSpinController col = Instantiate(listCollum[0], collumContainer.transform).GetComponent<CollumSpinController>();

            col.transform.name = "Collum" + (i + 1);
            if (i == 4)
            {
                col.isLastCollum = true;
            }
            listCollum.Add(col);
        }
        JArray views = (JArray)dataCtable["views"];
        setStartView(views);
        JArray arrP = (JArray)dataCtable["ArrP"];
        JObject dataPlayer = (JObject)arrP[0];
        agPlayer = (long)dataPlayer["AG"];
        //lbCurrentChips.text = Config.FormatNumber(agPlayer);
        lbCurrentChips.Text = Config.FormatNumber(agPlayer);
        listMarkbet = ((JArray)dataCtable["MarkBet"]).ToObject<List<int>>();
        listBetRoom = getListBetRoom();
        freespinLeft = getInt(dataCtable, "freeSpinCount");
        isFreeSpin = freespinLeft > 0;
        isInFreeSpin = isFreeSpin;
        singleLineBet = getInt(dataCtable, "singleLineBet");

        updateCurrentMarkBet();

        if (isFreeSpin)
        {
            setCurrentMarkBet();
            if (Config.curGameId != (int)GAMEID.SLOTTARZAN)
            {
                animFreespinNum.gameObject.SetActive(true);
                lbFreespinLeft.text = Config.getTextConfig("txt_freespinRM") + ": " + freespinLeft;
            }
            else
            {
                lbFreespinLeft.gameObject.SetActive(freespinLeft > 0);
                lbFreespinLeft.text = "" + freespinLeft;
            }

        }
        lbCurrentBet.text = Config.FormatMoney2(totalListBetRoom[currentMarkBet], true);
        if (listBetRoom.Count > 0)
        {
            lbStateBet.text = currentMarkBet >= listBetRoom.Count - 1 ? Config.getTextConfig("txt_max_bet") : Config.getTextConfig("txt_bet");
        }
        else
        {
            if (!isFreeSpin)
            {
                lbInfoSession.text = Config.getTextConfig("msg_warrning_send");
            }
            else
            {

            }

        }
        setSpinType();
        setStateBtnSpin();
    }
    public override void handleSpin(JObject data)
    {
        isSendingSpin = false;
        finishData = data;

        //get freespin
        //if (indexSpin == 1)


        //indexSpin++;
        //5ofakind + bigwin
        //finishData = JObject.Parse("{\"evt\":\"slotViews\",\"slotViews\":[[7,10,9],[7,7,0],[7,9,8],[11,10,1],[5,7,8]],\"creditWin\":1150,\"winningLine\":[1,11],\"lineDetail\":[{\"lineId\":1,\"win\":7500},{\"lineId\":11,\"win\":50000}],\"freeSpinLeft\":0,\"winType\":2,\"freeSpin\":false,\"agWin\":57500,\"AG\":145113,\"MarkBet\":[1,5,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");

        //win scatter
        //finishData = JObject.Parse("{\"evt\":\"slotViews\",\"slotViews\":[[1,7,1],[12,7,7],[12,7,10],[1,7,3],[6,9,0]],\"creditWin\":130,\"winningLine\":[],\"lineDetail\":[],\"freeSpinLeft\":0,\"winType\":0,\"freeSpin\":false,\"agWin\":20000,\"AG\":148063,\"MarkBet\":[1,5,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");

        //finishData = JObject.Parse(" {\"evt\":\"slotViews\",\"slotViews\":[[6,0,8],[11,11,5],[10,1,0],[10,8,9],[8,2,11]],\"creditWin\":26,\"winningLine\":[4,12,14,15,18],\"lineDetail\":[{\"lineId\":4,\"win\":20},{\"lineId\":12,\"win\":20},{\"lineId\":14,\"win\":20},{\"lineId\":15,\"win\":50},{\"lineId\":18,\"win\":20}],\"freeSpinLeft\":0,\"winType\":0,\"freeSpin\":false,\"agWin\":130,\"AG\":15262,\"MarkBet\":[1,5,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");
        //finishData = JObject.Parse("{\"evt\":\"slotViews\",\"slotViews\":[[4,2,7],[2,12,4],[1,12,11],[2,12,8],[0,0,0]],\"creditWin\":70,\"winningLine\":[15,19],\"lineDetail\":[{\"lineId\":15,\"win\":50000},{\"lineId\":19,\"win\":20000}],\"freeSpinLeft\":15,\"winType\":0,\"freeSpin\":false,\"agWin\":70000,\"AG\":574384,\"level\":{\"levelUser\":0,\"curLevelExp\":0,\"maxLevelExp\":0,\"agUser\":574384},\"MarkBet\":[1,5,10,25,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");
        //if (indexSpin == 0)
        //{
        //    finishData = JObject.Parse("{\"evt\":\"slotViews\",\"slotViews\":[[1,7,1],[12,7,7],[1,7,10],[4,7,3],[6,9,0]],\"creditWin\":130,\"winningLine\":[],\"lineDetail\":[],\"freeSpinLeft\":0,\"winType\":0,\"freeSpin\":true,\"agWin\":0,\"AG\":18000,\"MarkBet\":[1,5,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");
        //}
        //else if (indexSpin == 1)
        //{
        //finishData = JObject.Parse("{\"evt\":\"slotViews\",\"slotViews\":[[1,7,1],[1,7,7],[12,7,10],[12,7,3],[6,9,0]],\"creditWin\":130,\"winningLine\":[],\"lineDetail\":[],\"freeSpinLeft\":0,\"winType\":0,\"freeSpin\":true,\"agWin\":2000,\"AG\":150063,\"MarkBet\":[1,5,50,500,1000,5000,10000,25000,50000,100000,250000,500000]}");
        //}
        //indexSpin++;
        setFinishView((JArray)finishData["slotViews"]);
        winningLines = finishData["winningLine"].ToObject<List<int>>();
        linesDetail = (JArray)finishData["lineDetail"];
        winType = (int)finishData["winType"];
        isFreeSpin = (int)finishData["freeSpinLeft"] > 0;

        isInFreeSpin = getBool(finishData, "freeSpin");
        if ((int)finishData["freeSpinLeft"] == 0)
        {
            animFreespinNum.gameObject.SetActive(false);
        }
        checkFiveOfAKind();

        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f).AppendCallback(() =>
        {
            listCollum[0].isStop = true;
            currentIndexStop = 0;
        });

    }
    public void forceFreeSpin()
    {
        //isFirstFreeSpin = !isFirstFreeSpin;
        //UIManager.instance.showToast("isForcefree:" + isFirstFreeSpin);
    }
    public virtual bool checkWinScatter()
    {
        int numberScatter = slotViews.FindAll(arr => arr.Contains(12)).Count;
        isGetFreeSpin = (int)finishData["winType"] >= 100;
        return numberScatter >= 2;
    }
    public virtual void showNearFreeSpin(int indexCol)
    {
        animNearFreeSpin.gameObject.SetActive(true);
        animNearFreeSpin.transform.localPosition = new Vector2(animNearFreeSpin.transform.parent.InverseTransformPoint(listCollum[indexCol].transform.position).x, animNearFreeSpin.transform.localPosition.y);

    }
    public virtual void offAnimNearFreeSpin()
    {
        animNearFreeSpin.gameObject.SetActive(false);
    }
    protected void showWinScatter()
    {
        List<int> scatterColumnIds = new List<int>();
        slotViews.ForEach(arr => { if (arr.Contains(12)) scatterColumnIds.Add(slotViews.IndexOf(arr)); });
        if (Config.curGameId == (int)GAMEID.SLOT_JUICY_GARDEN && scatterColumnIds.Count >= 3)
        {
            for (int i = 0; i < scatterColumnIds.Count; i++)
            {
                List<int> rowIds = new() { scatterColumnIds[i] };
                for (int j = i + 1; j < scatterColumnIds.Count; j++)
                {
                    if (scatterColumnIds[j] == scatterColumnIds[j - 1] + 1) rowIds.Add(scatterColumnIds[j]);
                    else break;
                }
                if (rowIds.Count >= 3)
                {
                    foreach (int id in rowIds) listCollum[id].itemResult.showScatterAnim();
                    break;
                }
            }
        }
        else foreach (int id in scatterColumnIds) listCollum[id].itemResult.showScatterAnim();
        DOTween.Sequence().AppendInterval(3.5f).AppendCallback(() =>
        {
            setDarkAllItem(false);
            handleActionResult();
        });
    }
    public virtual void showAllWinline()
    {
        lbInfoSession.gameObject.SetActive(false);
        setDarkAllItem(true);
        setStateBtnSpin();
        for (int i = 0; i < winningLines.Count; i++) // ve truoc line roi active false di.
        {
            int lineId = winningLines[i];
            List<int> lineWinID = getPaylineWithID(winningLines[i]);
            ColorUtility.TryParseHtmlString(listColors[lineId % listColors.Count], out Color colorLine);
            List<Vector2> listPos = new List<Vector2>();
            for (int j = 0; j < lineWinID.Count; j++)
            {
                Vector2 positionItem = listCollum[j].getPositionItemAtIndex(lineWinID[j]);
                Vector2 posisionInContainer = lineContainer.transform.InverseTransformPoint(positionItem);
                listPos.Add(posisionInContainer);
            }
            drawLines(listPos, colorLine);
        }

        //show tung line len;
        int index = 0;
        for (int i = 0, l = listLineStraight.Count; i < l; i++)
        {
            var objLL = listLineStraight[i];
            seqShowAllLine = DOTween.Sequence();
            seqShowAllLine.AppendInterval(0.1f * i)
            .AppendCallback(() =>
            {
                objLL.SetActive(true);
            })
            .AppendInterval(1.5f)
            .AppendCallback(() =>
            {

                if (index == l - 1)
                {
                    //show den line cuoi cung thi chuyen sang show one by one
                    foreach (GameObject line in listLineStraight)
                    {
                        Destroy(line);
                    }
                    if (winType != 0)
                    {
                        gameState = GAME_STATE.SHOWING_RESULT;
                    }

                    setDarkAllItem(false);
                    handleActionResult();
                }
                index++;

            });
        }
    }
    public virtual void showOneByOneLine()
    {
        int index = 0;

        for (int i = 0, l = winningLines.Count; i < l; i++)
        {

            int lineId = winningLines[i];
            List<int> lineWinID = getPaylineWithID(lineId);
            Color colorLine = new Color();
            ColorUtility.TryParseHtmlString(listColors[lineId], out colorLine);
            Sequence s = DOTween.Sequence();
            seqShowOneByOne.Add(s);
            s.AppendInterval(2.0f * i)
            .AppendCallback(() =>
            {
                if (transform == null)
                {
                    DOTween.Kill(s);

                }
                else
                {
                    playSound(SOUND_SLOT.SHOW_LINE);
                    setDarkAllItem(true);
                    createRect(lineWinID, colorLine);
                    showPaylinesInfo(lineWinID, index);
                }

            })
            .AppendInterval(2.0f)
            .AppendCallback(() =>
            {
                foreach (GameObject line in listLineRect)
                {
                    Destroy(line);
                }
                listLineRect.Clear();
            })
            //.AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                if (index == winningLines.Count - 1)
                {
                    handleActionResult();
                    if (spintype == SPIN_TYPE.NORMAL)
                    {
                        setDarkAllItem(false);
                        resetSlotView();
                        index++;
                    }
                }
            });
        }
    }
    protected virtual void showPaylinesInfo(List<int> lineWinID, int indexLine)
    {
        List<int> listIdInLine = new List<int>();
        for (int j = 0; j < lineWinID.Count; j++)
        {
            List<int> collumView = slotViews[j];
            listIdInLine.Add(collumView[lineWinID[j]]);
        }

        int numberItem = checkNumberOfItemInLine(listIdInLine);
        paylineInfoContainer.SetActive(true);
        Transform paylinesTrans = paylineInfoContainer.transform;
        Transform iconContainer = paylinesTrans.Find("iconContainer");
        TextMeshProUGUI lbPayWin = paylineInfoContainer.transform.Find("lbPayWin").gameObject.GetComponentInChildren<TextMeshProUGUI>();
        JObject dataDetail = (JObject)linesDetail[indexLine];
        lbPayWin.text = Config.formatStr(Config.getTextConfig("txt_pays_chip"), Config.FormatNumber((int)dataDetail["win"]));// "pays " + Config.FormatNumber((int)dataDetail["win"]) + " chips";
        List<GameObject> listIcon = new List<GameObject>();
        listIcon.Add(iconContainer.GetChild(0).gameObject);
        listIcon.Add(iconContainer.GetChild(1).gameObject);
        listIcon.Add(iconContainer.GetChild(2).gameObject);
        listIcon.Add(iconContainer.GetChild(3).gameObject);
        listIcon.Add(iconContainer.GetChild(4).gameObject);
        for (int i = 0; i < 5; i++)
        {
            if (i < numberItem)
            {
                listIcon[i].SetActive(true);
                listIcon[i].GetComponent<Image>().sprite = listSprIcon[getIdWin(listIdInLine)];
                listIcon[i].GetComponent<Image>().SetNativeSize();
                //listIcon[i].transform.localScale = new Vector2(0.5f, 0.5f);
            }
            else
            {
                listIcon[i].SetActive(false);
            }
        }
        listIcon.Clear();

    }
    protected int getIdWin(List<int> listId)
    {

        //Logging.Log(idWin);
        return listId.Find(id => id != 11);
    }
    protected void showEffectLine()
    {
        int lineId = winningLines[currentLineEffect];
        List<int> lineWinID = getPaylineWithID(winningLines[currentLineEffect]);
        Color colorLine = new Color();
        ColorUtility.TryParseHtmlString(listColors[lineId], out colorLine);

        Sequence s = DOTween.Sequence();
        seqShowOneByOne.Add(s);
        s.AppendInterval(1.0f * currentLineEffect)
        .AppendCallback(() =>
        {
            setDarkAllItem(true);
            createRect(lineWinID, colorLine);
        })
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            foreach (GameObject line in listLineRect)
            {
                Destroy(line);
            }
            listLineRect.Clear();
        })
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {

            if (currentLineEffect == winningLines.Count - 1)
            {
                setDarkAllItem(false);
            }

        });
    }
    public void showNextLineEffect()
    {
        currentLineEffect++;
        showEffectLine();
    }
    protected virtual void showBigWin()
    {
        playSound(SOUND_SLOT.BIG_WIN);
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);

        lbBigWin.transform.parent.gameObject.SetActive(true);
        lbBigWin.gameObject.SetActive(true);
        if (isInFreeSpin == true && isFreeSpin == false) //vua quay het freespin turn cuoi cung;
        {
            Config.tweenNumberTo(lbBigWin, countTotalAgFreespin, 0, 3.0f);
        }
        else
        {
            Config.tweenNumberTo(lbBigWin, getInt(finishData, "agWin"), 0, 2.0f);
        }

        animEffect.TrimRenderers();
        animEffect.transform.localScale = new Vector2(0.9f, 0.9f);
        animEffect.transform.localPosition = new Vector2(0, -70);
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(BIGWIN_ANIMPATH);
        animEffect.Initialize(true);
        animEffect.AnimationState.Complete += delegate
        {
            effectAnimEndListenter();
        };
        animEffect.AnimationState.SetAnimation(0, ANIM_BIGWIN_NAME, false);
        effectAnimEndListenter = () =>
        {
            Debug.Log(" Chay vào end show big");
            lbBigWin.transform.parent.gameObject.SetActive(false);
            effectContainer.SetActive(false);
            gameState = GAME_STATE.SHOWING_RESULT;
            handleActionResult();
            Color cl = Color.black;
            effectContainer.GetComponent<Image>().color = new Color(cl.r, cl.g, cl.b, 0.5f);

        };

    }
    protected virtual void showMegaWin()
    {
        playSound(SOUND_SLOT.MEGA_WIN);
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);
        //TextMeshProUGUI lbChipWin = animEffect.transform.Find("bgChip/lbBigWin").GetComponent<TextMeshProUGUI>();
        lbBigWin.transform.parent.gameObject.SetActive(true);
        lbBigWin.gameObject.SetActive(true);
        if (isInFreeSpin == true && isFreeSpin == false) //vua quay het freespin turn cuoi cung;
        {
            Config.tweenNumberTo(lbBigWin, countTotalAgFreespin, 0, 2.0f);
        }
        else
        {
            Config.tweenNumberTo(lbBigWin, getInt(finishData, "agWin"), 0, 2.0f);
        }
        //Config.tweenNumberTo(lbBigWin, 100000, 0, 3.0f);
        animEffect.TrimRenderers();
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(BIGWIN_ANIMPATH);
        animEffect.transform.localScale = new Vector2(0.9f, 0.9f);
        animEffect.transform.localPosition = new Vector2(0, -70);
        animEffect.Initialize(true);
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
    }
    protected virtual void showHugeWin()
    {

    }
    protected virtual void showFreeSpin()
    {
        playSound(SOUND_SLOT.FREESPIN);
        effectContainer.SetActive(true);
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(FREESPIN_ANIMPATH);
        animEffect.Initialize(true);
        animEffect.gameObject.SetActive(true);
        animEffect.transform.localPosition = new Vector2(0, 0);
        animEffect.AnimationState.Complete += delegate
        {
            effectAnimEndListenter();
        };
        animEffect.TrimRenderers();
        animEffect.gameObject.GetComponent<RectTransform>().localScale = new Vector2(0.8f, 0.8f);
        animEffect.AnimationState.SetAnimation(0, "eng", false);
        effectAnimEndListenter = () =>
        {
            effectContainer.SetActive(false);
            handleActionResult();
        };
    }
    protected void showFiveOfaKind()
    {
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(FIVEOFAKIND_ANIMPATH);
        animEffect.transform.localScale = new Vector2(1.0f, 1.0f);
        animEffect.transform.localPosition = new Vector2(0, 0);
        animEffect.Initialize(true);
        animEffect.AnimationState.Complete += delegate
        {
            effectAnimEndListenter();
        };
        animEffect.TrimRenderers();
        lbBigWin.gameObject.SetActive(false);
        animEffect.gameObject.GetComponent<RectTransform>().localScale = new Vector2(0.8f, 0.8f);
        animEffect.AnimationState.SetAnimation(0, "animation", false);
        effectAnimEndListenter = () =>
        {
            effectContainer.SetActive(false);
            handleActionResult();
        };
    }
    protected void createRect(List<int> lineWinID, Color colorLine)
    {
        List<Vector2> listPosRect = new List<Vector2>();
        List<int> listIdInLine = new List<int>();

        for (int j = 0; j < lineWinID.Count; j++)
        {

            Vector2 positionItem = listCollum[j].getPositionItemAtIndex(lineWinID[j]);
            //Logging.Log("createRect3 getPositionItemAtIndex:"+ (lineWinID[j]);
            Vector2 posisionInContainer = lineContainer.transform.InverseTransformPoint(positionItem);
            listPosRect.Add(posisionInContainer);
            List<int> collumView = slotViews[j];// resultView[j].ToObject<List<int>>();
            listIdInLine.Add(collumView[lineWinID[j]]);
        }

        int numberItem = checkNumberOfItemInLine(listIdInLine);
        for (int j = 0; j < numberItem; j++)
        {
            int indexShowIcon = lineWinID[j];
            listCollum[j].setDarkItem(false, indexShowIcon);
        }
        drawRect(listPosRect, colorLine, numberItem);


    }
    protected void createLineRect(Vector2 startPos, Color colorLine)
    {
        GameObject lineRect = Instantiate(line2Pr, lineContainer.transform);

        RectTransform rt = lineRect.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        LineController lineComp = lineRect.GetComponent<LineController>();
        lineComp.drawRect(startPos, RECT_SIZE, colorLine);
        listLineRect.Add(lineRect);
    }
    protected void drawLines(List<Vector2> listPos, Color colorLine)
    {
        Vector2 startPos = new Vector2(listPos[0].x - 90, listPos[0].y);
        Vector2 lastPos = new Vector2(listPos[listPos.Count - 1].x + 90, listPos[listPos.Count - 1].y);
        listPos.Insert(0, startPos);
        listPos.Add(lastPos);
        GameObject lineStraight = Instantiate(line2Pr, lineContainer.transform);

        RectTransform rt = lineStraight.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        lineStraight.SetActive(false);
        listLineStraight.Add(lineStraight);
        LineController lineComp = lineStraight.GetComponent<LineController>();
        lineComp.drawLine(listPos, colorLine);

    }
    public List<int> getPaylineWithID(int idLine)
    {
        return payLines[idLine].ToObject<List<int>>();
    }
    protected void drawLines2(List<Vector2> listPos, Color colorLine)
    {
        GameObject lineStraight = Instantiate(line2Pr, lineContainer.transform);

        lineStraight.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        //lineStraight.SetActive(false);
        listLineRect.Add(lineStraight);
        LineController lineComp = lineStraight.GetComponent<LineController>();
        lineComp.drawLine(listPos, colorLine);
    }
    protected void drawRect(List<Vector2> listPosRect, Color colorLine, int numberItem)
    {

        List<Vector2> listPosRemain = new List<Vector2>();
        for (int i = 0, l = listPosRect.Count; i < l; i++)
        {
            Vector2 startPos = listPosRect[i];
            if (i < numberItem) // ve cac hinh vuong
            {
                createLineRect(startPos, colorLine);
            }

            if (i < l - 1)
            {
                Vector2 nextPos = listPosRect[i + 1];

                if (Mathf.Abs(nextPos.y - startPos.y) > 200)// check 2 o vuong cach nhau 1 moi ve line noi tiep;
                {
                    if (i < numberItem - 1)
                    {
                        List<Vector2> listPos = new List<Vector2>();
                        Vector2 firstIntersectPos = getCrossPoint(listPosRect[i], listPosRect[i + 1]);
                        Vector2 nextIntersectPos = getCrossPoint(listPosRect[i + 1], listPosRect[i]);
                        listPos.Add(firstIntersectPos);
                        listPos.Add(nextIntersectPos);
                        drawLines2(listPos, colorLine);
                    }
                }

            }
            if (i >= numberItem && listPosRemain.Count == 0) // t?nh position ?i?m b?t ??u v? line t? ? cu?i c?ng ??n m?p ph?i
            {
                Vector2 previousPos = listPosRect[i - 1];
                Vector2 startPosLineRemain = new Vector2();
                if (math.abs(previousPos.y - startPos.y) < 100)
                {
                    startPosLineRemain = new Vector2(listPosRect[i - 1].x + RECT_SIZE.x / 2, listPosRect[i - 1].y);
                }
                else
                {
                    bool isTwoItemSpace = (Mathf.Abs(startPos.y - previousPos.y) > 200);
                    if (previousPos.y < startPos.y)
                    {
                        startPosLineRemain = isTwoItemSpace ? new Vector2(previousPos.x, previousPos.y + RECT_SIZE.y / 2) : new Vector2(previousPos.x + RECT_SIZE.x / 2, previousPos.y + RECT_SIZE.y / 2);
                    }
                    else
                    {
                        startPosLineRemain = isTwoItemSpace ? new Vector2(previousPos.x, previousPos.y - RECT_SIZE.y / 2) : new Vector2(previousPos.x + RECT_SIZE.x / 2, previousPos.y - RECT_SIZE.y / 2);
                    }

                }
                listPosRemain.Add(startPosLineRemain);
            }
            if (i >= numberItem)
            {
                listPosRemain.Add(listPosRect[i]);
            }
        }
        listPosRemain.Add(new Vector2(listPosRect[listPosRect.Count - 1].x + RECT_SIZE.x / 2, listPosRect[listPosRect.Count - 1].y)); //add them 1 doan cuoi cung ve tu tam icon cuoi ra mep phai
        drawLines2(listPosRemain, colorLine);
    }
    protected Vector2 getCrossPoint(Vector2 vec1, Vector2 vec2)
    {

        int delta = vec1.y > vec2.y ? -1 : 1;
        Vector2 posA = new Vector2(vec1.x - RECT_SIZE.x / 2, vec1.y + (RECT_SIZE.y / 2) * delta);
        Vector2 posB = new Vector2(posA.x + RECT_SIZE.x, posA.y);
        if (Mathf.Abs(vec1.y - vec2.y) < 1)
        {
            posA = new Vector2(vec1.x + RECT_SIZE.x / 2, vec1.y + RECT_SIZE.y / 2);
            posB = new Vector2(vec1.x + RECT_SIZE.x / 2, vec1.y - RECT_SIZE.y / 2);
        }
        //Vector2 getCrossPoint =Config.pIntersectPoint(posA, posB, vec1, vec2);
        Vector2 getCrossPoint = new Vector2(posA.x + RECT_SIZE.x / 2, posA.y);
        //Logging.Log("startPos:" + vec1);
        //Logging.Log("nextPos:" + vec2);
        //Logging.Log("posA:" + posA);
        //Logging.Log("posB:" + posB);
        //Logging.Log("getCrossPoint:" + getCrossPoint);
        return getCrossPoint;
    }
    protected virtual int checkNumberOfItemInLine(List<int> listId)
    {


        int count = 1;
        int idCheck = listId[0];
        for (int i = 1, l = listId.Count; i < l; i++)
        {
            if (idCheck != 11)
            {
                if (listId[i] == idCheck || listId[i] == 11)
                    count++;
                else break;
            }
            else
            {
                idCheck = listId[i];
                count++;
            }
        }
        return count;
    }

    public void setDarkAllCollum(bool state)
    {
        for (int i = 0, l = listCollum.Count; i < l; i++)
        {
            listCollum[i].setDarkAllCollum(state);
        }
    }
    protected virtual void setStartView(JArray dataStartView)
    {
        for (int i = 0, size = dataStartView.Count; i < size; i++)
        {
            List<int> viewCollum = dataStartView[i].ToObject<List<int>>();
            listCollum[i].setStartView(viewCollum, this);
        }
    }
    protected virtual void setFinishView(JArray dataFinishView)
    {
        slotViews.Clear();
        slotViews = dataFinishView.ToObject<List<List<int>>>();
        for (int i = 0; i < slotViews.Count; i++)
        {
            List<int> viewCollum = slotViews[i];
            listCollum[i].setFinishView(viewCollum);
        }
    }
    protected List<int> getListBetRoom()
    {
        List<int> listBetRoom = new List<int>();
        totalListBetRoom = new List<int>();
        for (int i = 0; i < listMarkbet.Count; i++)
        {
            if (listMarkbet[i] * payLines.Count <= agPlayer)
            {
                listBetRoom.Add(listMarkbet[i] * payLines.Count);

            }
            totalListBetRoom.Add(listMarkbet[i] * payLines.Count);
        }

        return listBetRoom;
    }
    public void nextColStop()
    {
        //if (currentIndexStop < 4)
        //{
        if (countScatter >= 2 && currentIndexStop == indexCheck3rdScatter - 1 && Config.curGameId != (int)GAMEID.SLOT_JUICY_GARDEN)
        {
            DOTween.Sequence()
                .AppendInterval(2.0f)
                .AppendCallback(() =>
                {
                    currentIndexStop++;
                    if (currentIndexStop < listCollum.Count)
                    {
                        listCollum[currentIndexStop].isStop = true;
                    }
                });
            showNearFreeSpin(currentIndexStop + 1);
            listCollum[currentIndexStop + 1].isNearFreeSpin = true;
        }
        else
        {
            currentIndexStop++;
            if (currentIndexStop < listCollum.Count)
            {
                listCollum[currentIndexStop].isStop = true;
            }
        }


        //}
    }
    public virtual void changeBetRoom(string type)
    {
        SoundManager.instance.soundClick();
        if (isFreeSpin) return;//free thi k change muc bet;
        if (listBetRoom.Count == 0)
        {
            string textShow = Config.getTextConfig("txt_not_enough_money_gl");
            string textBtn2 = Config.getTextConfig("shop");
            string textBtn3 = Config.getTextConfig("label_cancel");
            UIManager.instance.showDialog(textShow, textBtn2, () =>
            {
                UIManager.instance.openShop();
            }, textBtn3);
            return;
        }
        if (gameState == GAME_STATE.SPINNING)
        {
            return;
        }
        if (type == "plus")
        {
            currentMarkBet = currentMarkBet == listBetRoom.Count - 1 ? 0 : currentMarkBet + 1;//vuot qua index thi quay ve 0;
        }
        else if (type == "minus")
        {
            currentMarkBet = currentMarkBet == 0 ? listBetRoom.Count - 1 : currentMarkBet - 1;//vuot qua index thi quay ve 0;
        }
        else
        {
            if (currentMarkBet == listBetRoom.Count - 1)
            {
                onClickSpin();
                return;
            }
            currentMarkBet = listBetRoom.Count - 1;
        }
        listBetRoom.ForEach(item =>
        {
            Debug.Log("listBetRoom:" + item);
        });
        lbCurrentBet.text = Config.FormatMoney2(listBetRoom[currentMarkBet], true);
        if (currentMarkBet == listBetRoom.Count - 1)
        {
            lbStateBet.text = Config.getTextConfig("txt_max_bet");
        }
        else
        {
            lbStateBet.text = Config.getTextConfig("txt_bet");
        }
    }
    protected void updateCurrentMarkBet()
    {
        //for (int i = 0, size = listBetRoom.Count; i < size; i++)
        //{
        //    Debug.Log("listBetRoom[i]=" + listBetRoom[i]);
        //    if (listBetRoom[i] <= agPlayer / 20)
        //    {
        //        currentMarkBet = i;
        //    }
        //}
        for (int i = 0; i < listMarkbet.Count; i++)
        {
            int markbet = listMarkbet[i];
            if (markbet * 20 < agPlayer / 20)
            {
                currentMarkBet = i;
            }
        }
    }
    public void onSpinTriggerDown()
    {

        timeHoldSpin = 0;
        isHoldSpin = true;

    }
    public virtual void setDarkAllItem(bool state)
    {

        if (bgSpin != null)
        {
            bgSpin.color = state ? Color.gray : Color.white;
        }

        for (int i = 0, l = listCollum.Count; i < l; i++)
        {
            listCollum[i].setDarkItem(state);
        }
    }

    public virtual void onSpinTriggerUp()
    {
        isHoldSpin = false;

        if (timeHoldSpin < 1.3f)
        {
            if (gameState != GAME_STATE.SPINNING) //dang dung.chua quay
            {
                if (isFreeSpin == false && (listBetRoom.Count == 0 || agPlayer < totalListBetRoom[currentMarkBet]))
                {
                    resetSlotView();
                    lbInfoSession.text = Config.getTextConfig("msg_warrning_send");
                    string textShow = Config.getTextConfig("txt_not_enough_money_gl");
                    string textBtn2 = Config.getTextConfig("shop");
                    string textBtn3 = Config.getTextConfig("label_cancel");
                    UIManager.instance.showDialog(textShow, textBtn2, () =>
                    {
                        UIManager.instance.openShop();
                    }, textBtn3);
                    return;
                }
                if ((spintype == SPIN_TYPE.FREE_NORMAL || spintype == SPIN_TYPE.FREE_AUTO) && (gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.SHOWING_RESULT) && !isGetFreeSpin)//neu turn do dc freespin thi van cho click
                {
                    Logging.Log("dang free spin.Deo cho click:gameState=" + gameState);
                    return;
                }
                spintype = SPIN_TYPE.NORMAL;
                setSpinType();
                if (gameState == GAME_STATE.SHOWING_RESULT)
                {
                    listActionHandleSpin = new List<TweenCallback> { listActionHandleSpin[listActionHandleSpin.Count - 1] };
                    handleActionResult();
                }
                else
                {
                    onClickSpin();
                }
            }
            else
            {
                spintype = SPIN_TYPE.NORMAL;
                setSpinType();
                setStateBtnSpin();
            }

        }
        else
        {
            if (agPlayer < totalListBetRoom[currentMarkBet])
            {
                lbInfoSession.text = Config.getTextConfig("msg_warrning_send");
                return;
            }
            spintype = SPIN_TYPE.AUTO;
            setSpinType();
        }

    }
    protected virtual void setSpinType()
    {
        //Logging.Log("setSpinType freespinLeft ==" + freespinLeft);
        if (freespinLeft > 0)
        {
            //Logging.Log("isGetFreeSpin==" + isGetFreeSpin);
            if (!isGetFreeSpin)
            {
                if (spintype == SPIN_TYPE.AUTO)
                {
                    spintype = SPIN_TYPE.FREE_AUTO;
                }
                else if (spintype == SPIN_TYPE.NORMAL)
                {
                    spintype = SPIN_TYPE.FREE_NORMAL;
                }
            }
            else
            {
                if (spintype == SPIN_TYPE.NORMAL)
                {
                    spintype = SPIN_TYPE.FREE_NORMAL;
                }
                else if (spintype == SPIN_TYPE.AUTO)
                {
                    spintype = SPIN_TYPE.FREE_AUTO;
                }
            }

        }
        else
        {
            if (spintype == SPIN_TYPE.FREE_AUTO)
            {
                spintype = SPIN_TYPE.AUTO;
            }
            else if (spintype == SPIN_TYPE.FREE_NORMAL)
            {
                spintype = SPIN_TYPE.NORMAL;
            }
        }
    }
    public void onSpinHold()
    {
        Logging.Log("onSpinHold:");
    }
    public override void onClickRule()
    {
        GameObject ruleView = Instantiate(rulePr, transform);

    }

}
