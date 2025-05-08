using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Newtonsoft.Json.Linq;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using System.Threading.Tasks;
using Globals;

public class SlotJuicyGardenView : BaseSlotGameView
{
    public static SlotJuicyGardenView instance;
    [SerializeField] public GameObject holdPackageContainer;
    [SerializeField] protected TextNumberControl lbJpGrand, lbJpMajor, lbJpMinor, lbJpMini;
    [SerializeField] protected TextMeshProUGUI lbTotalPackageValue;
    [SerializeField]
    protected SkeletonGraphic
    animPopupGetFreeSpin, animPopupResultPackage, animJackpot, animToltalMoneyPackage, animEffectPackage;
    [SerializeField] protected DialogView popupGetFruitRain;
    [SerializeField] protected Material lbJpMaterial_grand, lbJpMaterial_major, lbJpMaterial_minor, lbJpMaterial_mini;
    public int typeBonus = 0;
    public bool isBonusGame = false, allowSelectBonus = false;
    private int rateJPGrand = 0, rateJPMajor = 0, rateJPMinor = 0, rateJPMini = 0, totalFreeSpinGet;
    private long valueJPGrand = 0, valueJPMajor = 0, valueJPMinor = 0, valueJPMini = 0, jpGrandPlayer = 0, jpMajorPlayer = 0, totalPackageValue = 0;

    protected string BGMONEYPACKAGE_ANIMPATH = "GameView/SlotSpine/JuicyGarden/EffectPackage/skeleton_SkeletonData";
    protected string RESULT_BONUSGAME_ANIMPATH = "GameView/SlotSpine/JuicyGarden/EndGame/skeleton_SkeletonData";
    protected string JACKPOT_ANIMPATH = "GameView/SlotSpine/JuicyGarden/Jackpot/skeleton_SkeletonData";

    protected override void Awake()
    {
        base.Awake();
        FREESPIN_ANIMPATH = "GameView/SlotSpine/JuicyGarden/AnimBox/skeleton_SkeletonData";
        ANIM_BG_FREESPIN = "GameView/SlotSpine/JuicyGarden/BgFreeSpin/skeleton_SkeletonData";
        BIGWIN_ANIMPATH = "GameView/SlotSpine/JuicyGarden/big_megawinJuicy/skeleton_SkeletonData";
        MEGAWIN_ANIMPATH = "GameView/SlotSpine/JuicyGarden/big_megawinJuicy/skeleton_SkeletonData";
        RECT_SIZE = new Vector2(120, 120);
    }
    public override void handleCTable(string data)
    {
        //data = GetFakeDataJuicy.instance.getdataCtableGioBeforeEnd();
        Globals.Logging.Log("HandleCtable Juicy:" + data);
        dataCtable = JObject.Parse(data);
        isBonusGame = getBool(dataCtable, "isBonusGame");
        payLines = (JArray)dataCtable["payLine"];
        for (int i = 1; i < 5; i++)
        {
            CollumSpinController col = Instantiate(listCollum[0], collumContainer.transform).GetComponent<CollumSpinController>();

            col.transform.name = "Collum" + (i + 1);
            col.collumIndex = i;
            if (i == 4)
            {
                col.isLastCollum = true;
            }
            listCollum.Add(col);
        }
        JArray views = getJArray(dataCtable, "views").ToObject<JArray>();
        for (int i = 0; i < views.Count; i++)
        {
            var arr = (JArray)views[i];
            for (int j = 0; j < arr.Count; j++)
            {
                if ((int)views[i][j]["id"] == -1)
                {
                    views[i][j]["id"] = Random.Range(0, 11);
                }
                //if (!(bool)dataCtable["isBonusGame"] && !(bool)dataCtable["isFreeGame"])
                //{
                //    int idRan = Random.Range(0, 13);
                //    views[i][j]["id"] = idRan;
                //}

            }
        }
        dataCtable["views"] = views;

        //getJArray(dataCtable, "views") = views;
        freespinLeft = getInt(dataCtable, "freeSpinCount");
        DOTween.Sequence().AppendInterval(0.05f).AppendCallback(() =>
        {
            setStartView((JArray)dataCtable["views"]);
        });
        JArray arrP = (JArray)dataCtable["ArrP"];
        JObject dataPlayer = (JObject)arrP[0];
        agPlayer = (long)dataPlayer["AG"];
        lbCurrentChips.Text = Globals.Config.FormatNumber(agPlayer);
        listMarkbet = ((JArray)dataCtable["MarkBet"]).ToObject<List<int>>();
        listBetRoom = getListBetRoom();

        totalFreeSpinGet = getInt(dataCtable, "freeSpinCount");
        isFreeSpin = freespinLeft > 0;
        singleLineBet = getInt(dataCtable, "singleLineBet");
        isInFreeSpin = getBool(dataCtable, "isFreeGame");
        updateCurrentMarkBet();
        if (isFreeSpin)
        {
            currentMarkBet = listMarkbet.IndexOf(singleLineBet);
            lbFreespinLeft.text = Globals.Config.getTextConfig("txt_freespinRM") + ": " + freespinLeft;
            animFreespinNum.gameObject.SetActive(true);
            animBgFreeSpin.gameObject.SetActive(true);
            animBgFreeSpin.skeletonDataAsset = UIManager.instance.loadSkeletonData(ANIM_BG_FREESPIN);
            animBgFreeSpin.allowMultipleCanvasRenderers = true;
            animBgFreeSpin.Initialize(true);
            animBgFreeSpin.AnimationState.SetAnimation(0, "animation", true);

        }

        if (listBetRoom.Count > 0)
        {
            if (listBetRoom.Count - 1 >= currentMarkBet)
            {
                Debug.Log("currentMarkBet=" + currentMarkBet);
                lbCurrentBet.text = Globals.Config.FormatMoney2(listBetRoom[currentMarkBet]);
                lbStateBet.text = currentMarkBet == listBetRoom.Count - 1 ? Globals.Config.getTextConfig("txt_max_bet") : Globals.Config.getTextConfig("txt_bet");
            }
            else
            {
                lbCurrentBet.text = Globals.Config.FormatMoney2(totalListBetRoom[currentMarkBet]);
            }
        }
        else
        {
            if (!isFreeSpin)
            {
                lbInfoSession.text = Globals.Config.getTextConfig("msg_warrning_send");
                lbCurrentBet.text = Globals.Config.FormatMoney2(totalListBetRoom[currentMarkBet]);
            }
            else
            {
                lbCurrentBet.text = Globals.Config.FormatMoney2(totalListBetRoom[currentMarkBet]);
            }
        }
        setSpinType();
        setStateBtnSpin();
        rateJPGrand = getInt(dataCtable, "rateJPGrand");
        rateJPMajor = getInt(dataCtable, "rateJPMajor");
        rateJPMinor = getInt(dataCtable, "rateJPMinor");
        rateJPMini = getInt(dataCtable, "rateJPMini");
        setJackpotValue(dataCtable);
        allowSelectBonus = getBool(dataCtable, "allowSelectBonus");
        if (allowSelectBonus)
        {
            showFreeSpin();
        }

    }
    public override void HandlerUpdateUserChips(JObject data)
    {
        lbCurrentChips.setValue((long)data["ag"], false);
    }
    protected override void setSpinType()
    {
        if (isFreeSpin)
        {
            //if (!isGetFreeSpin && isBonusGame || (isInFreeSpin)) //dang trong turn bonus game thi quay auto
            //{
            //    spintype = SPIN_TYPE.FREE_NORMAL;
            //}
            //else //neu turn do dc freespin(allowBonus)  hoac 6 gio thi dung het tat ca lai bat dau bam quay;
            //{
            //    if (spintype == SPIN_TYPE.AUTO)
            //    {
            //        spintype = SPIN_TYPE.FREE_AUTO;
            //    }
            //    else if (spintype == SPIN_TYPE.NORMAL)
            //    {
            //        spintype = SPIN_TYPE.FREE_NORMAL;
            //    }

            //}
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
            if (spintype == SPIN_TYPE.FREE_AUTO)
            {
                spintype = SPIN_TYPE.AUTO;
            }
            else if (spintype == SPIN_TYPE.FREE_NORMAL)
            {
                spintype = SPIN_TYPE.NORMAL;
            }
        }
        Globals.Logging.Log("Spintype=" + spintype);
    }
    protected override void resetSlotView()
    {
        Globals.Logging.Log("Reset Slot View JUICY");
        if (finishData != null)
        {
            setDarkAllCollum(getInt((JObject)finishData["selectBonus"], "typeBonus") == 4 || getInt((JObject)finishData["selectBonus"], "typeBonus") == 5);
        }

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
        lbInfoSession.text = Globals.Config.formatStr(Globals.Config.getTextConfig("txt_playing_lines"), payLines.Count);
        paylineInfoContainer.SetActive(false);
        listLineStraight.Clear();
        gameState = GAME_STATE.PREPARE;
        isFiveOfaKind = false;
        isWinScatter = false;
        listActionHandleSpin.Clear();
        slotViews.Clear();

        if (finishData != null && isInFreeSpin == true && isFreeSpin == false)
        {
            countTotalAgFreespin = 0;
        }

        setStateBtnSpin();
        if (freespinLeft > 0)
        {

            if (Globals.Config.curGameId == (int)Globals.GAMEID.SLOTTARZAN)
            {
                lbFreespinLeft.gameObject.SetActive(true);
                lbFreespinLeft.text = freespinLeft.ToString();
            }
            else
            {
                animFreespinNum.gameObject.SetActive(true);
                lbFreespinLeft.text = Globals.Config.getTextConfig("txt_freespinRM") + ": " + freespinLeft;
            }
        }
        else
        {
            if (animBgFreeSpin != null)
            {
                animBgFreeSpin.gameObject.SetActive(false);
            }
            if (Globals.Config.curGameId != (int)Globals.GAMEID.SLOTTARZAN)
            {
                animFreespinNum.gameObject.SetActive(false);
            }
            else
            {
                lbFreespinLeft.gameObject.SetActive(false);
            }
        }
        Debug.Log("ResetSlot JuicyDone");
    }
    private void showBgTotalMoneyPackage()
    {
        lbTotalPackageValue.text = "0";
        animToltalMoneyPackage.gameObject.SetActive(true);
        animToltalMoneyPackage.skeletonDataAsset = UIManager.instance.loadSkeletonData(BGMONEYPACKAGE_ANIMPATH);
        animToltalMoneyPackage.Initialize(true);
        animToltalMoneyPackage.AnimationState.SetAnimation(0, "box_chip", true);
        transform.Find("bgTable/NodeJackPotNumber").gameObject.SetActive(false);
    }
    private void showEffectPackageWithIndex(int index, long value, TweenCallback cb = null)
    {
        totalPackageValue += value;
        Globals.Config.tweenNumberToNumber(lbTotalPackageValue, totalPackageValue, totalPackageValue - value);
        animEffectPackage.gameObject.SetActive(true);
        animEffectPackage.skeletonDataAsset = UIManager.instance.loadSkeletonData(BGMONEYPACKAGE_ANIMPATH);
        animEffectPackage.Initialize(true);
        Debug.Log("totalPackageValue=" + totalPackageValue);

        animEffectPackage.AnimationState.Complete += delegate
        {
            handleActionResult();
        };
        animEffectPackage.AnimationState.SetAnimation(0, index.ToString(), false);

    }
    public override void setStateBtnSpin()
    {
        //base.setStateBtnSpin();
        //Globals.Logging.Log("setStateBtnSpin:" + gameState + "----spintype==" + spintype);
        if (gameState == GAME_STATE.SPINNING)
        {

            if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
            {
                animBtnSpin.startingAnimation = "freespin";
            }
            else if (spintype == SPIN_TYPE.AUTO)
            {
                animBtnSpin.startingAnimation = "stop";
            }
            else if (spintype == SPIN_TYPE.NORMAL)
            {
                bool isGet6Gio = (!isBonusGame && typeBonus == 5); //bat dau dc 6 gio
                if (isGetFreeSpin || isGet6Gio)
                {
                    animBtnSpin.startingAnimation = "freespin";
                }
            }
            else
            {
                animBtnSpin.startingAnimation = "autospin";
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
                if (listBetRoom.Count == 0)
                {
                }
                else  //het cmn tien roi.an di.
                {
                    if (currentMarkBet <= listBetRoom.Count - 1)
                    {
                        if (agPlayer < listBetRoom[currentMarkBet])
                            animBtnSpin.color = Color.gray;
                    }

                }
                if (spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL)
                {
                    animBtnSpin.startingAnimation = "freespin";
                    animBtnSpin.color = Color.white;
                }
                if (spintype == SPIN_TYPE.NORMAL)
                {
                    bool isGet6Gio = (!isBonusGame && typeBonus == 5); //bat dau dc 6 gio
                    if (isGetFreeSpin || isGet6Gio)
                    {
                        animBtnSpin.startingAnimation = "freespin";
                        animBtnSpin.color = Color.white;
                    }

                }
            }
        }
        animBtnSpin.Initialize(true);
    }
    private int indexPick = 0;
    public override void handleSpin(JObject data)
    {
        isSendingSpin = false;
        finishData = data;

        JObject jackPot = (JObject)finishData["jackPot"];
        string typeJP = getString(jackPot, "typeJP");
        long winJP = getLong(jackPot, "winJP");
        if (typeJP != "NONE" && typeJP != "")
        {
            setValueJPToSlotViews(typeJP, winJP);
        }

        allowSelectBonus = getBool(finishData, "allowSelectBonus");
        setFinishView((JArray)finishData["slotViews"]);
        isFreeSpin = (int)finishData["freeSpinLeft"] > 0;
        freespinLeft = (int)finishData["freeSpinLeft"];
        winningLines = finishData["winningLine"].ToObject<List<int>>();
        linesDetail = (JArray)finishData["lineDetail"];
        winType = (int)finishData["winType"];
        isInFreeSpin = getBool(finishData, "freeSpin");
        JObject selectBonus = (JObject)finishData["selectBonus"];
        typeBonus = getInt(selectBonus, "typeBonus");
        isBonusGame = getBool(finishData, "isBonusGame");
        if (isBonusGame && freespinLeft == 0)//turn cuoi, check lai tong tien xem co duoc big,mega win ko vi server turn cuoi tra ve ko chuan
        {
            long totalPkgVl = getLong(finishData, "creditWin");
            if (totalPkgVl > 50 * totalListBetRoom[currentMarkBet])
            {
                winType = 2;
            }
            else if (totalPkgVl > 20 * totalListBetRoom[currentMarkBet])
            {
                winType = 1;
            }
            else
            {
                winType = 0;
            }
        }
        checkFiveOfAKind();
        isWinScatter = checkWinScatter();
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f).AppendCallback(() =>
        {
            listCollum[0].isStop = true;
            currentIndexStop = 0;
        });

    }
    private long getTotalPkgVl(string typeJP, long jpValue)
    {
        long totalPkgVl = 0;
        JArray slotViews = getJArray(finishData, "slotViews");
        foreach (JArray views in slotViews)
        {
            foreach (JObject idView in views)
            {
                if (getInt(idView, "id") > 13)
                {
                    if (typeJP == "JP_MINI" || typeJP == "JP_MINOR")
                    {
                        totalPkgVl += jpValue;
                    }
                    else if (typeJP == "JP_MAJOR")
                    {
                        totalPkgVl += jpValue + jpMajorPlayer;
                    }

                }
                else
                {
                    totalPkgVl += getLong(idView, "value");
                }
            }
        }
        return totalPkgVl;
    }
    private void setValueJPToSlotViews(string typeJP, long jpValue)
    {
        JArray slotViews = getJArray(finishData, "slotViews");
        foreach (JArray views in slotViews)
        {
            foreach (JObject idView in views)
            {
                if (getInt(idView, "id") > 13)
                {
                    if (typeJP == "JP_MINI" || typeJP == "JP_MINOR")
                    {
                        idView["value"] = jpValue;
                    }
                    else if (typeJP == "JP_MAJOR")
                    {
                        idView["value"] = jpValue + jpMajorPlayer;
                    }
                }
            }
        }
        finishData["slotViews"] = slotViews;

    }
    public override void onStopSpin() //dung het cot cuoi cung.bat dau check data show result c?c th?
    {

        if (checkConditionHolderPackage())
        {
            TweenCallback acCreateHolderPkgView = () =>
            {

                createHolderPackageView();
                handleActionResult();
            };
            listActionHandleSpin.Add(acCreateHolderPkgView);
        }
        if (freespinLeft == 0)
        {
            if (animBgFreeSpin != null)
            {
                animBgFreeSpin.gameObject.SetActive(false);
            }
            animFreespinNum.gameObject.SetActive(false);

        }
        if (!isInFreeSpin) //khong co freespin
        {
            if ((long)finishData["agWin"] != 0 && !isBonusGame)
            {
                //Globals.Config.tweenNumberTo(lbChipWins, (long)finishData["agWin"], 0, 0.3f, true);
                lbChipWins.setValue((long)finishData["agWin"], true);
                if ((long)finishData["agWin"] > 0)
                {
                    sprStateWin.sprite = listSprStateWin[0];
                    sprStateWin.SetNativeSize();
                }
            }
        }
        else if (isInFreeSpin == true && !isFreeSpin) //freespin turn cuoi cung
        {
            countTotalAgFreespin += (int)finishData["agWin"];
            TweenCallback acShowTotalWinFreeSpin = () =>
            {
                //Globals.Config.tweenNumberTo(lbChipWins, countTotalAgFreespin, countTotalAgFreespin - (int)finishData["agWin"]);
                lbChipWins.setValue(countTotalAgFreespin, true);
                sprStateWin.sprite = listSprStateWin[1];
                sprStateWin.SetNativeSize();
                //Globals.Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer);
                lbCurrentChips.setValue((long)finishData["AG"], true);
                showAnimChipBay();
                handleActionResult();
            };

            if (countTotalAgFreespin > 50 * totalListBetRoom[currentMarkBet])
            {
                winType = 2;
            }
            else if (countTotalAgFreespin > 20 * totalListBetRoom[currentMarkBet])
            {
                winType = 1;
            }
            else
            {
                winType = 0;
            }
            if (countTotalAgFreespin > 0)
            {
                listActionHandleSpin.Add(acShowTotalWinFreeSpin);
            }
        }
        else //dang trong freespin
        {
            Debug.Log("Dang Trong FreeSpin");
            countTotalAgFreespin += (int)finishData["agWin"];
            //Globals.Config.tweenNumberTo(lbChipWins, countTotalAgFreespin, countTotalAgFreespin - (int)finishData["agWin"]);
            lbChipWins.setValue(countTotalAgFreespin, true);
            sprStateWin.sprite = listSprStateWin[1];
            sprStateWin.SetNativeSize();
        }
        if (isBonusGame && freespinLeft == 0)
        {//choi gi? ??n turn cu?i c?ng.Bat dau show effect ?n gi?
            Debug.Log("Package turn cuoi");
            TweenCallback acShowEffectPackage = () =>
            {
                totalPackageValue = 0;
                lbTotalPackageValue.gameObject.SetActive(true);
                showBgTotalMoneyPackage();
                JArray slotV = getJArray(finishData, "slotViews");
                List<JObject> viewsPackage = new List<JObject>();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        JArray views = (JArray)slotV[j];
                        JObject dataItem = (JObject)views[i];
                        dataItem["index"] = (i * 5) + j + 1;
                        viewsPackage.Add(dataItem);
                    }
                }
                viewsPackage = viewsPackage.FindAll(view =>
                {
                    return getInt(view, "value") > 0;
                });
                int indexPackage = 0;
                for (int i = 0, l = viewsPackage.Count; i < l; i++)
                {
                    int value = getInt(viewsPackage[i], "value");
                    int index = getInt(viewsPackage[i], "index");
                    DOTween.Sequence()
                    .AppendInterval(1.0f * i)
                    .AppendCallback(() =>
                    {
                        indexPackage++;
                        showEffectPackageWithIndex(index, value);
                    });
                }

            };
            listActionHandleSpin.Add(acShowEffectPackage);
            JObject jackPot = (JObject)finishData["jackPot"];
            string typeJP = getString(jackPot, "typeJP");
            if (typeJP != "NONE" && typeJP != "")
            {
                TweenCallback acShowJackpot = () =>
                {
                    showJackpotAnim();
                };
                listActionHandleSpin.Add(showJackpotAnim);
            }
        }
        TweenCallback acCheckNextSpin = () =>
        {
            //dang quay thuong auto ma dc freespin-> dung lai.
            //dang quay freespin m? dc freespin->quay tiep.
            Globals.Logging.Log("acCheckNextSpin:SpinType= " + spintype);
            setSpinType();
            refreshBetRoomAndMarkbet();

            if (isBonusGame && freespinLeft == 0)
            {
                for (int i = 0, l = holdPackageContainer.transform.childCount; i < l; i++)
                {
                    Transform itemHold = holdPackageContainer.transform.GetChild(i);
                    itemHold.gameObject.SetActive(false);
                    if (itemHold.childCount != 0)
                    {
                        UIManager.instance.destroyAllChildren(itemHold);
                    }
                }
                foreach (CollumSpinController col in listCollum)
                {
                    SlotJuicyGardenCollumController colJuicy = (SlotJuicyGardenCollumController)col;
                    colJuicy.setDarkItem(false);
                }
                lbTotalPackageValue.gameObject.SetActive(false);
                animToltalMoneyPackage.gameObject.SetActive(false);
                transform.Find("bgTable/NodeJackPotNumber").gameObject.SetActive(true);
                animEffectPackage.gameObject.SetActive(false);
            }
            if (spintype == SPIN_TYPE.AUTO || spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL) //check luot tiep theo neu dang auto hoac dang trong freesin
            {
                Debug.Log(" VBao Day");
                resetSlotView();
                if ((spintype == SPIN_TYPE.FREE_AUTO || spintype == SPIN_TYPE.FREE_NORMAL || listBetRoom.Count > 0) && !isGetFreeSpin)
                {
                    onClickSpin();
                }
                else if (isGetFreeSpin || isInFreeSpin == true) //dc freespin trong luc freepsin ->quay tiep.
                {
                    onClickSpin();
                }
                else
                {
                    setStateBtnSpin();
                }
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
            if (winningLines.Count > 0 && !isFreeSpin)
            {
                //Globals.Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer);
                lbCurrentChips.setValue((long)finishData["AG"], true);
                refreshBetRoomAndMarkbet();
                showAnimChipBay();
            }
            showOneByOneLine();
        };

        ///------------------CHECK SHOW WIN FREESPIN--------------------//
        if (isWinScatter)
        {
            TweenCallback acShowWinScatter = () =>
            {
                showWinScatter();
            };
            listActionHandleSpin.Add(acShowWinScatter);
        }

        ///------------------CHECK SHOW FIVE OF A KIND--------------------//
        if (isFiveOfaKind)
        {
            TweenCallback acShowFiveOfAkind = () =>
            {
                showFiveOfaKind();
            };
            listActionHandleSpin.Add(acShowFiveOfAkind);
        }


        if (!isBonusGame && typeBonus == 5)
        {
            TweenCallback acShowFruitRain = () =>
            {
                showPopupGetFruitRain();
            };
            listActionHandleSpin.Add(acShowFruitRain);
        }

        ///------------------CHECK SHOW ALL LINE--------------------//
        if (winningLines.Count > 0)
        {
            TweenCallback acShowAllWinLine = () =>
            {
                showAllWinline();
            };
            listActionHandleSpin.Add(acShowAllWinLine);
        }

        ///------------------CHECK SHOW TYPE WIN--------------------//

        if (winType == TYPE_BIGWIN)
        {
            TweenCallback acShowBigWin = () =>
            {
                showBigWin();
            };
            if (!isFreeSpin)
            {
                listActionHandleSpin.Add(acShowBigWin);
            }
        }
        else if (winType == TYPE_MEGA)
        {
            TweenCallback acShowMegaWin = () =>
            {
                showMegaWin();
            };
            if (!isFreeSpin)
            {
                listActionHandleSpin.Add(acShowMegaWin);
            }

        }
        if (isBonusGame && freespinLeft == 0)
        {//choi gi? ??n turn cu?i c?ng.Bat dau show effect ?n gi?
            TweenCallback showResultPackage = () =>
            {
                //Globals.Config.tweenNumberTo(lbChipWins, (long)finishData["agWin"], 0, 0.3f, true);
                lbChipWins.setValue((long)finishData["agWin"], true);
                if ((long)finishData["agWin"] > 0)
                {
                    sprStateWin.sprite = listSprStateWin[0];
                    sprStateWin.SetNativeSize();
                }
                showPopupResultPackage();
            };
            listActionHandleSpin.Add(showResultPackage);
        }
        ///------------------CHECK SHOW ONE BY ONE--------------------//
        if (winningLines.Count > 0) //co line win va quay thuong thi moi show an tung line
        {
            TweenCallback acShowAnimChipBay = () =>
            {
                //Globals.Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer);
                lbCurrentChips.setValue((long)finishData["AG"], true);
                refreshBetRoomAndMarkbet();
                showAnimChipBay();
                handleActionResult();
            };
            Debug.Log("SpinType=" + spintype);
            Debug.Log("isInFreeSpin=" + isInFreeSpin);
            if (spintype == SPIN_TYPE.NORMAL)
            {
                if (freespinLeft == 0)
                {
                    listActionHandleSpin.Add(acShowOneWinLine);
                }
                else
                {

                    if (!isInFreeSpin)
                    {
                        listActionHandleSpin.Add(acShowAnimChipBay);
                    }

                }
            }
            else if (spintype == SPIN_TYPE.AUTO || spintype == SPIN_TYPE.FREE_AUTO)
            {
                if (winningLines.Count == 1) listActionHandleSpin.Add(() => { showOneByOneLine(); });
                if (isInFreeSpin == false) listActionHandleSpin.Add(acShowAnimChipBay);
            }

        }
        else if (winningLines.Count == 0 && getLong(finishData, "agWin") > 0)
        {

            TweenCallback acShowAnimChipBay = () =>
            {
                //Globals.Config.tweenNumberToNumber(lbCurrentChips, (long)finishData["AG"], agPlayer);
                lbCurrentChips.setValue((long)finishData["AG"], true);
                refreshBetRoomAndMarkbet();
                showAnimChipBay();
                handleActionResult();
            };
            listActionHandleSpin.Add(acShowAnimChipBay);
        }
        ///------------------CHECK SHOW FREESPIN--------------------//

        if (isGetFreeSpin)
        {
            TweenCallback acShowFreeSpin = () =>
            {
                showFreeSpin();
            };
            listActionHandleSpin.Add(acShowFreeSpin);

        }
        listActionHandleSpin.Add(acCheckNextSpin);
        handleActionResult();
        setJackpotValue(finishData);
    }
    private void createHolderPackageView()
    {
        for (int i = 0; i < listCollum.Count; i++)
        {
            SlotJuicyGardenItemSpin itemSpin = (SlotJuicyGardenItemSpin)listCollum[i].itemResult;
            int index = 0;
            foreach (long idValue in itemSpin.arrValuePackage)
            {
                if (itemSpin.listIdIcon[index] >= 13) //id phai la gio va dang trong freespin
                {
                    createHoldPackage(convertIndex(index, i), itemSpin.listSprItem[index].gameObject);
                }
                index++;
            }
        }
    }
    private int convertIndex(int index, int collumIndex)
    {

        int indexNew = index * 5 + collumIndex;
        return indexNew;
    }
    public bool checkConditionHolderPackage()
    {
        // cac case show giu gio
        // 1. Turn do dc 6 gio.Bat dau giu 6 gio do tren man hinh va quay tiep.
        //2.Dang trong bonus Fruit Rain Game.Ra gio nao giu luon gio do.
        JObject selectBonus = new JObject();
        JObject dataGame = new JObject();
        if (finishData != null)
        {
            selectBonus = (JObject)finishData["selectBonus"];
            dataGame = finishData;
        }
        else
        {
            selectBonus = (JObject)dataCtable["selectBonus"];
            dataGame = dataCtable;
        }
        typeBonus = getInt(selectBonus, "typeBonus");
        bool isSetHolderPackage = false;
        if (isBonusGame)
        {
            //Globals.Logging.Log("allowSelectBonus=" + getBool(dataGame, "allowSelectBonus"));
            if (allowSelectBonus == true && typeBonus != 5) //chua chon thunng.vao lai game.neu ca case chon gio thi create giu gio.con ko thi thoi.
            {
                isSetHolderPackage = false;
            }
            else
            {

                isSetHolderPackage = true;
            }

        }
        if (!isBonusGame && typeBonus == 5)
        {
            isSetHolderPackage = true;
        }
        return isSetHolderPackage;
    }
    public override void showAnimChipBay()
    {
        Transform transFrom = lbChipWins.transform;
        Transform transTo = lbCurrentChips.transform.parent.Find("icChip").transform;
        coinFly(transFrom, transTo);
        agPlayer = (long)finishData["AG"];
    }
    public override void onClickSpin()
    {
        base.onClickSpin();
        animBgFreeSpin.transform.localScale = new Vector2(1.25f, 1.05f);
        if (isBonusGame && freespinLeft > 0)
        {
            listCollum.ForEach(col =>
            {
                col.setDarkAllCollum(true);
            });
        }
    }
    protected override void checkFiveOfAKind()
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
                    List<int> collumView = slotViews[j];
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
    protected override void showBigWin()
    {
        playSound(Globals.SOUND_SLOT.BIG_WIN);
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);
        lbBigWin.gameObject.SetActive(true);
        lbBigWin.transform.localScale = new Vector2(1.5f, 1.5f);
        if (isInFreeSpin == true && isFreeSpin == false) //vua quay het freespin turn cuoi cung;
        {
            Globals.Config.tweenNumberToNumber(lbBigWin, (long)countTotalAgFreespin, 0, 3.0f);
        }
        else
        {
            Globals.Config.tweenNumberToNumber(lbBigWin, getLong(finishData, "agWin"), 0, 3.0f);
        }

        animEffect.transform.localScale = new Vector2(.75f, .75f);
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(BIGWIN_ANIMPATH);
        animEffect.Initialize(true);
        animEffect.AnimationState.Complete += delegate
        {
            effectAnimEndListenter();
        };
        animEffect.AnimationState.SetAnimation(0, ANIM_BIGWIN_NAME, false);
        effectAnimEndListenter = () =>
        {
            animEffect.gameObject.SetActive(false);
            effectContainer.SetActive(false);
            gameState = GAME_STATE.SHOWING_RESULT;
            handleActionResult();
        };
        var myAnimation = animEffect.Skeleton.Data.FindAnimation(ANIM_BIGWIN_NAME);
        float timeEff = myAnimation.Duration;
        DOTween.Sequence().AppendInterval(timeEff - 0.65f).Append(lbBigWin.transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack));

    }
    protected override void showMegaWin()
    {
        playSound(Globals.SOUND_SLOT.MEGA_WIN);
        effectContainer.SetActive(true);
        animEffect.gameObject.SetActive(true);
        lbBigWin.gameObject.SetActive(true);
        lbBigWin.transform.localScale = new Vector2(1.5f, 1.5f);
        if (isInFreeSpin == true && isFreeSpin == false) //vua quay het freespin turn cuoi cung;
        {
            Globals.Config.tweenNumberToNumber(lbBigWin, (long)countTotalAgFreespin, 0, 5.0f);
        }
        else
        {
            Globals.Config.tweenNumberToNumber(lbBigWin, getLong(finishData, "agWin"), 0, 5.0f);
        }
        animEffect.skeletonDataAsset = UIManager.instance.loadSkeletonData(BIGWIN_ANIMPATH);
        animEffect.transform.localScale = new Vector2(.75f, .75f);
        animEffect.Initialize(true);
        animEffect.AnimationState.SetAnimation(0, ANIM_MEGAWIN_NAME, false);
        animEffect.AnimationState.Complete += delegate
        {
            effectAnimEndListenter();
        };
        effectAnimEndListenter = () =>
        {
            effectContainer.SetActive(false);
            animEffect.gameObject.SetActive(false);
            gameState = GAME_STATE.SHOWING_RESULT;
            handleActionResult();

        };
        var myAnimation = animEffect.Skeleton.Data.FindAnimation(ANIM_MEGAWIN_NAME);
        float timeEff = myAnimation.Duration;
        DOTween.Sequence().AppendInterval(timeEff - 0.65f).Append(lbBigWin.transform.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack));
    }
    private void showJackpotAnim()
    {
        JObject jackPot = (JObject)finishData["jackPot"];
        string typeJP = getString(jackPot, "typeJP");
        long value = getLong(jackPot, "winJP");
        animJackpot.gameObject.SetActive(true);
        effectContainer.SetActive(true);
        animJackpot.transform.localScale = new Vector2(0.8f, 0.8f);
        animJackpot.skeletonDataAsset = UIManager.instance.loadSkeletonData(JACKPOT_ANIMPATH);
        animJackpot.Initialize(true);

        Debug.Log("showJackpotAnim");
        switch (typeJP)
        {
            case "JP_GRAND":
                typeJP = "JP_grand";
                value += jpGrandPlayer;
                break;
            case "JP_MINI":
                typeJP = "JP_mini";
                break;
            case "JP_MAJOR":
                typeJP = "JP_major";
                value += jpMajorPlayer;
                break;
            case "JP_MINOR":
                typeJP = "JP_minor";
                break;
        }
        try
        {
            animJackpot.AnimationState.SetAnimation(0, typeJP, true);
            TextMeshProUGUI lbJackpot = animJackpot.transform.GetComponentInChildren<TextMeshProUGUI>();
            lbJackpot.text = Globals.Config.FormatMoney2(value);
            Globals.Config.tweenNumberToNumber(lbJackpot, value, 0, 1.0f);
            lbJackpot.transform.localScale = new Vector2(0.8f, 0.8f);
            DOTween.Sequence()
           .Append(animJackpot.transform.DOScale(new Vector2(1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack))
           .Join(lbJackpot.transform.DOScale(new Vector2(1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack))
           .AppendInterval(2.5f)
           .Append(animJackpot.transform.DOScale(new Vector2(1.5f, 1.5f), 0.3f).SetEase(Ease.InBack))
           .AppendCallback(() =>
           {
               Debug.Log("Show JP xong!");
               effectContainer.SetActive(false);
               animJackpot.gameObject.SetActive(false);
               handleActionResult();
           });
        }
        catch (System.Exception err)
        {
            Debug.Log(err);
        }


    }
    private void showPopupResultPackage()
    {
        playSound(Globals.SOUND_SLOT.FREESPIN);
        animPopupResultPackage.gameObject.SetActive(true);
        animPopupResultPackage.skeletonDataAsset = UIManager.instance.loadSkeletonData(RESULT_BONUSGAME_ANIMPATH);
        animPopupResultPackage.transform.Find("lbPackageValueWin").GetComponent<TextMeshProUGUI>().text = Globals.Config.FormatNumber(getLong(finishData, "agWin"));
        effectContainer.SetActive(true);
        animPopupResultPackage.Initialize(true);
        animPopupResultPackage.AnimationState.SetAnimation(0, "eng", true);
        animPopupResultPackage.transform.localScale = new Vector2(0.8f, 0.8f);
        animPopupResultPackage.transform.DOScale(new Vector2(1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
    }
    public void onClickClosePopupResultPackage()
    {
        animPopupResultPackage.transform.DOScale(new Vector2(0.8f, 0.8f), 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            effectContainer.SetActive(false);
            animPopupResultPackage.gameObject.SetActive(false);
            handleActionResult();
        });
    }
    protected override void setFinishView(JArray dataFinishView)
    {
        slotViews.Clear();
        foreach (JArray data in dataFinishView)
        {
            List<int> collumView = new List<int>();
            foreach (JObject item in data) collumView.Add(getInt(item, "id"));
            slotViews.Add(collumView);
        }
        for (int i = 0; i < dataFinishView.Count; i++)
        {
            JArray data = (JArray)dataFinishView[i];
            listCollum[i].setFinishView(data);
        }
    }
    public override bool checkWinScatter()
    {
        // int numberScatter = slotViews.FindAll(arr => arr.Contains(12)).Count;
        isGetFreeSpin = allowSelectBonus;
        return allowSelectBonus;
    }
    public void createHoldPackage(int index, GameObject itemTemplate)
    {
        //Debug.Log("createHoldPackage:" + index);
        Transform itemContainer = holdPackageContainer.transform.GetChild(index);
        itemContainer.gameObject.SetActive(true);
        if (itemContainer.transform.childCount > 0)
        {
            //Debug.Log("createHoldPackage vitri :" + index+" co child roi");
            //UIManager.instance.destroyAllChildren(itemContainer);

            return;
        }
        GameObject itemPackage = Instantiate(itemTemplate, itemContainer);

        Vector2 posInWorld = itemTemplate.GetComponent<RectTransform>().position;
        itemPackage.transform.localPosition = itemContainer.InverseTransformPoint(posInWorld);
        itemPackage.transform.localScale = itemPackage.transform.localScale * new Vector2(0.95f, 0.95f);
        itemPackage.GetComponent<Image>().color = Color.white;
        itemPackage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        itemPackage.SetActive(true);
        //itemPackage.transform.GetChild(0).localScale = Vector2.one;
    }
    protected override void showFreeSpin()
    {
        playSound(Globals.SOUND_SLOT.FREESPIN);
        animPopupGetFreeSpin.transform.Find("NodeWild").gameObject.SetActive(false);
        animPopupGetFreeSpin.transform.Find("NodeFruitRain").gameObject.SetActive(false);
        animPopupGetFreeSpin.gameObject.SetActive(true);
        animPopupGetFreeSpin.skeletonDataAsset = UIManager.instance.loadSkeletonData(FREESPIN_ANIMPATH);

        effectContainer.SetActive(true);

        animPopupGetFreeSpin.Initialize(true);
        animPopupGetFreeSpin.AnimationState.SetAnimation(0, "thung", true);
        animPopupGetFreeSpin.transform.Find("btnBoxLeft").GetComponent<Button>().interactable = true;
        animPopupGetFreeSpin.transform.Find("btnBoxRight").GetComponent<Button>().interactable = true;
        Globals.Logging.Log("showFreeSpin");
    }
    private void setJackpotValue(JObject data)
    {

        jpGrandPlayer = getLong(data, "jpGrandPlayer");
        jpMajorPlayer = getLong(data, "jpMajorPlayer");


        long valueJPGrandNew = (long)listMarkbet[currentMarkBet] * (long)rateJPGrand + jpGrandPlayer;
        long valueJPMajorNew = (long)listMarkbet[currentMarkBet] * (long)rateJPMajor + jpMajorPlayer;
        long valueJPMinorNew = listMarkbet[currentMarkBet] * rateJPMinor;
        long valueJPMiniNew = listMarkbet[currentMarkBet] * rateJPMini;
        //Globals.Config.tweenNumberToNumber(lbJpGrand, valueJPGrandNew, valueJPGrand);
        //Globals.Config.tweenNumberToNumber(lbJpMajor, valueJPMajorNew, valueJPMajor);
        //Globals.Config.tweenNumberToNumber(lbJpMinor, valueJPMinorNew, valueJPMinor);
        //Globals.Config.tweenNumberToNumber(lbJpMini, valueJPMiniNew, valueJPMini);
        lbJpGrand.setValue(valueJPGrandNew, true, 0.2f);
        lbJpMajor.setValue(valueJPMajorNew, true, 0.2f);
        lbJpMinor.setValue(valueJPMinorNew, true, 0.2f);
        lbJpMini.setValue(valueJPMiniNew, true, 0.2f);
        valueJPGrand = valueJPGrandNew;
        valueJPMajor = valueJPMajorNew;
        valueJPMinor = valueJPMinorNew;
        valueJPMini = valueJPMiniNew;

    }
    public void chooseBox(string type)
    {
        animPopupGetFreeSpin.Initialize(true);
        animPopupGetFreeSpin.AnimationState.SetAnimation(0, "thung" + type + "2", false);
        animPopupGetFreeSpin.transform.Find("btnBoxLeft").GetComponent<Button>().interactable = false;
        animPopupGetFreeSpin.transform.Find("btnBoxRight").GetComponent<Button>().interactable = false;
        JObject selectBonus;
        if (finishData != null)
        {
            selectBonus = (JObject)finishData["selectBonus"];
        }
        else
        {
            selectBonus = (JObject)dataCtable["selectBonus"];
        }

        int typeBonus = getInt(selectBonus, "typeBonus");
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() =>
        {

            Transform nodeWild = animPopupGetFreeSpin.transform.Find("NodeWild");
            Transform nodeFruitRain = animPopupGetFreeSpin.transform.Find("NodeFruitRain");
            if (typeBonus < 4)
            {
                int numFreeSpin = 6;
                int numWild = 50;
                if (typeBonus == 2)
                {
                    numFreeSpin = 9;
                    numWild = 100;
                }
                else if (typeBonus == 3)
                {
                    numFreeSpin = 15;
                    numWild = 200;
                }

                nodeWild.gameObject.SetActive(true);
                nodeFruitRain.gameObject.SetActive(false);
                effectContainer.SetActive(true);
                nodeWild.localScale = Vector2.zero;
                nodeWild.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutBack);
                TextMeshProUGUI lbFreeSpinNum = nodeWild.Find("lbFreeSpinNum").GetComponent<TextMeshProUGUI>();
                Globals.Config.tweenNumberToNumber(lbFreeSpinNum, numFreeSpin, 0, 1.0f);
                TextMeshProUGUI lbNumWild = nodeWild.Find("lbAddedWild").GetComponent<TextMeshProUGUI>();
                Globals.Config.tweenNumberToNumber(lbNumWild, numWild, 0, 1.0f);

                //.text = Globals.Config.getTextConfig("txt_add") + " " + numWild;
            }
            else if (typeBonus == 4)
            {
                nodeWild.gameObject.SetActive(false);
                nodeFruitRain.gameObject.SetActive(true);
                nodeFruitRain.Find("titleTypeFruitRain").localScale = Vector2.zero;
                nodeFruitRain.Find("titleTypeFruitRain").gameObject.SetActive(true);
                nodeFruitRain.Find("titleTypeFruitRain").DOScale(new Vector2(1.0f, 1.0f), 0.3f).SetEase(Ease.OutBack);
            }

        });

        animPopupGetFreeSpin.AnimationState.Complete += delegate
        {
            animPopupGetFreeSpin.gameObject.SetActive(false);
            effectContainer.SetActive(false);
            if (typeBonus == 4)
            {
                showPopupGetFruitRain();
            }
            else
            {
                handleActionResult();
            }
        };
    }
    private void showPopupGetFruitRain()
    {
        Debug.Log("showPopupGetFruitRain");
        effectContainer.gameObject.SetActive(true);
        popupGetFruitRain.gameObject.SetActive(true);
        popupGetFruitRain.show();
        if (spintype == SPIN_TYPE.AUTO || spintype == SPIN_TYPE.FREE_AUTO)
        {
            DOTween.Sequence()
                      .AppendInterval(10.0f)
                      .AppendCallback(() =>
                      {
                          if (popupGetFruitRain.gameObject.activeSelf)
                          {
                              closePopupGetFruitRain();
                          }
                      });
        }

    }
    public void closePopupGetFruitRain()
    {
        System.Action cb = () =>
        {
            effectContainer.gameObject.SetActive(false);
            handleActionResult();
            Debug.Log("closePopupGetFruitRain");
        };
        popupGetFruitRain.hide(false, cb);
    }
    public override void changeBetRoom(string type)
    {
        base.changeBetRoom(type);
        if (isFreeSpin || listBetRoom.Count == 0) return;//free thi k change muc bet;
        if (gameState == GAME_STATE.SPINNING)
        {
            return;
        }
        JObject data = finishData != null ? finishData : dataCtable;
        setJackpotValue(data);

    }
    protected override async void setStartView(JArray dataStartView)
    {

        if (!checkConditionHolderPackage())
        {
            for (int i = 0, size = dataStartView.Count; i < size; i++)
            {
                JArray dataStartViewCol = (JArray)dataStartView[i];
                for (int j = 0; j < dataStartViewCol.Count; j++)
                {
                    dataStartViewCol[j]["id"] = Random.Range(0, 12);
                }
            }
        }

        for (int i = 0, size = dataStartView.Count; i < size; i++)
        {
            listCollum[i].setStartView((JArray)dataStartView[i], this);
        }
        await Task.Delay(500);
        if (checkConditionHolderPackage())
        {
            createHolderPackageView();
        }

    }
    protected override void refreshBetRoomAndMarkbet()
    {
        Debug.Log("refreshBetRoomAndMarkbet isInFreeSpin=" + isInFreeSpin);
        Debug.Log("refreshBetRoomAndMarkbet isBonusGame=" + isBonusGame);
        if ((!isInFreeSpin && !isBonusGame) || (isInFreeSpin == true && !isFreeSpin) || (!isInFreeSpin && isBonusGame)) //ko co free ,bonus hoac free turn cuoi
        {
            agPlayer = (long)finishData["AG"];
            listBetRoom = getListBetRoom();

            setCurrentMarkBet();
        }
    }
}
