using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlackJackView : GameView
{
    [SerializeField] public GameObject PlayerContainer;
    [SerializeField] public ChipBlackJack chipPref;
    [SerializeField] public GameObject chipContainer;
    [SerializeField] public List<Button> listChipBets = new List<Button>();
    [SerializeField] public GameObject btn_bet;
    [SerializeField] public GameObject clock;
    [SerializeField] public Image uiFill;
    [SerializeField] public TextMeshProUGUI lbTimeBet;
    [SerializeField] public TextMeshProUGUI lbClear;
    [SerializeField] public TextMeshProUGUI lbDeal;
    [SerializeField] public Button btn_clear;
    [SerializeField] public Button btn_deal;
    [SerializeField] public Button btn_doubleBet;
    [SerializeField] public Button btn_rebet;
    [SerializeField] public List<GameObject> listBoxCard = new List<GameObject>();
    [SerializeField] public GameObject cardDeckLeft;
    [SerializeField] public GameObject cardDeckRight;
    [SerializeField] public GameObject cardDealer;
    [SerializeField] public GameObject scoreBox;
    [SerializeField] public List<GameObject> listScoreBoxs = new List<GameObject>();
    [SerializeField] public GameObject scoreBox_dealer;
    [SerializeField] public Card cardPrefab;
    [SerializeField] public List<GameObject> listCards = new List<GameObject>();
    [SerializeField] public List<GameObject> listDotsWait2 = new List<GameObject>();
    [SerializeField] public List<GameObject> listDotsWait3 = new List<GameObject>();
    [SerializeField] public List<GameObject> listDots = new List<GameObject>();
    [SerializeField] public List<GameObject> listLights = new List<GameObject>();
    [SerializeField] public Button btn_split;
    [SerializeField] public Button btn_stand;
    [SerializeField] public Button btn_hit;
    [SerializeField] public Button btn_double;
    [SerializeField] public List<GameObject> listBoxSecondHand = new List<GameObject>();
    [SerializeField] public List<GameObject> listCardsSecondHand = new List<GameObject>();
    [SerializeField] public List<SkeletonGraphic> listAniBlackJack = new List<SkeletonGraphic>();
    [SerializeField] public List<GameObject> listTextStandAction = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextHitAction = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextDoubleAction = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextSplitAction = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextWin = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextLose = new List<GameObject>();
    [SerializeField] public List<GameObject> listTextPush = new List<GameObject>();
    [SerializeField] public List<SkeletonGraphic> listAniBust = new List<SkeletonGraphic>();
    [SerializeField] public List<GameObject> listTextBust = new List<GameObject>();
    [SerializeField] public List<SkeletonGraphic> listAniWow = new List<SkeletonGraphic>();
    [SerializeField] public SkeletonGraphic aniBustDealer;
    [SerializeField] public SkeletonGraphic aniWowDealer;
    [SerializeField] public SkeletonGraphic checkBaiUp;
    [SerializeField] public GameObject textBustDealer;
    [SerializeField] public GameObject textWinDealer;
    [SerializeField] public GameObject textLoseDealer;
    [SerializeField] public GameObject textPushDealer;

    [SerializeField] public GameObject popupBuyInsure;
    [SerializeField] public Image uiFill1;
    [SerializeField] public TextMeshProUGUI lbTimeBet1;
    [SerializeField] public List<GameObject> listChipBetInsure = new List<GameObject>();

    [HideInInspector] private long betValue = 0;
    private List<long> listValueChipBets = new List<long>();
    private int chipBetColorInx = 0;
    private int myChipBetColor = 0;
    private long curChipBet = 0;
    private long isLastBet = 0;
    private long isSaveLastBet = 0;
    private long isMaxBet = 0;
    private long isMinBet = 0;
    private long curMoney = 0;
    private long saveCurChipBC0 = 0;
    private long saveCurChipBC1 = 0;
    private long saveCurChipBC2 = 0;
    private bool isDealerBusted = false;

    private List<ChipBlackJack> listChipsPay = new List<ChipBlackJack>();

    private List<ChipBlackJack> listChipBC0 = new List<ChipBlackJack>();
    private List<ChipBlackJack> listChipBC1 = new List<ChipBlackJack>();
    private List<ChipBlackJack> listChipBC2 = new List<ChipBlackJack>();
    private List<List<ChipBlackJack>> listChipInTable = new List<List<ChipBlackJack>>();

    private List<Card> listCardDealer = new List<Card>();
    private List<Card> listCardInTable = new List<Card>();
    private List<Card> listCardInFirstHand = new List<Card>();
    private List<Card> listCardInSecondHand = new List<Card>();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void updatePositionPlayerView()
    {
        for (int i = 0; i < players.Count; i++)
        {
            int index = getDynamicIndex(getIndexOf(players[i]));
            players[i]._indexDynamic = index;
            if (index < listPosView.Count)
            {
                players[i].playerView.transform.localPosition = listPosView[index];// new Vector2(listPosView[index].x / 1280 * Screen.width, listPosView[index].y / 720 * Screen.height);
                players[i].playerView.gameObject.SetActive(true);
            }
            players[i].updateItemVip(players[i].vip);
        }
    }

    public override PlayerView createPlayerView()
    {
        var plView = Instantiate(playerViewPrefab, PlayerContainer.transform);//.GetComponent<PlayerView>();
        plView.transform.SetSiblingIndex((int)Globals.ZODER_VIEW.PLAYER);

        //plView.transform.localScale = new Vector2(0.9f, 0.8f);
        return plView.GetComponent<PlayerViewBlackJack>();
    }

    public void resetGame()
    {
        stateGame = Globals.STATE_GAME.WAITING;
        saveCurChipBC0 = 0;
        saveCurChipBC1 = 0;
        saveCurChipBC2 = 0;
        isLastBet = 0;
        uiFill.fillAmount = 1f;

        foreach (GameObject x in listChipBetInsure)
        {
            x.SetActive(false);
            x.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        btn_split.interactable = true;
        btn_stand.interactable = true;
        btn_hit.interactable = true;
        btn_double.interactable = true;

        textBustDealer.SetActive(false);
        listTextBust[0].gameObject.SetActive(false);
        listTextBust[1].gameObject.SetActive(false);
        listTextBust[2].gameObject.SetActive(false);

        scoreBox_dealer.transform.Find("imgBust").gameObject.SetActive(false);
        scoreBox_dealer.transform.Find("imgWow").gameObject.SetActive(false);

        listScoreBoxs[0].transform.Find("imgBust").gameObject.SetActive(false);
        listScoreBoxs[0].transform.Find("imgWow").gameObject.SetActive(false);

        listScoreBoxs[1].transform.Find("imgBust").gameObject.SetActive(false);
        listScoreBoxs[1].transform.Find("imgWow").gameObject.SetActive(false);

        listScoreBoxs[2].transform.Find("imgBust").gameObject.SetActive(false);
        listScoreBoxs[2].transform.Find("imgWow").gameObject.SetActive(false);

        foreach (Card x in listCardInTable)
        {
            Destroy(x.gameObject);
        }
        listCardInTable.Clear();

        foreach (ChipBlackJack x in listChipInTable[0])
        {
            Destroy(x.gameObject);
        }
        listChipInTable[0].Clear();

        foreach (ChipBlackJack x in listChipInTable[1])
        {
            Destroy(x.gameObject);
        }
        listChipInTable[1].Clear();

        foreach (ChipBlackJack x in listChipInTable[2])
        {
            Destroy(x.gameObject);
        }
        listChipInTable[2].Clear();


        foreach (Card x in listCardDealer)
        {
            Destroy(x.gameObject);
        }
        listCardDealer.Clear();

        foreach (GameObject x in listLights)
        {
            x.SetActive(false);
        }

        foreach (GameObject x in listDots)
        {
            x.SetActive(false);
        }

        foreach (Player player in players)
        {
            foreach (Card x in player.playerView.cards)
            {
                Destroy(x.gameObject);
            }
            player.playerView.cards.Clear();
        };

        cardDealer.transform.DOScale(new Vector2(1f, 1f), 0.1f);
        scoreBox_dealer.SetActive(false);
        scoreBox_dealer.GetComponentInChildren<TextMeshProUGUI>().text = "";

        scoreBox_dealer.transform.localPosition = new Vector2(50, 195);
        listScoreBoxs[0].transform.localPosition = new Vector2(47, 110);
        listScoreBoxs[1].transform.localPosition = new Vector2(47, 110);
        listScoreBoxs[2].transform.localPosition = new Vector2(47, 110);

        foreach (ChipBlackJack chip in listChipBC0)
        {
            Destroy(chip.gameObject);
        }
        listChipBC0.Clear();

        foreach (ChipBlackJack chip in listChipBC1)
        {
            Destroy(chip.gameObject);
        }
        listChipBC1.Clear();

        foreach (ChipBlackJack chip in listChipBC2)
        {
            Destroy(chip.gameObject);
        }
        listChipBC2.Clear();

        for (int i = 0; i < listBoxCard.Count; i++)
        {
            listBoxCard[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        checkAutoExit();
    }


    public override void handleSTable(string strData)
    {
        base.handleSTable(strData);

        stateGame = Globals.STATE_GAME.WAITING;

        clock.gameObject.SetActive(false);
        btn_bet.SetActive(false);
        btn_deal.gameObject.SetActive(false);
        btn_clear.gameObject.SetActive(false);

        thisPlayer.playerView.setPosThanhBarThisPlayer();
        setStatusButtonsBet();

        for (int i = 0; i < players.Count; i++)
        {
            int indexPlayer = getDynamicIndex(getIndexOf(players[i]));
            listDots[indexPlayer].SetActive(true);
        }

        JObject jData = JObject.Parse(strData);
        agTable = (int)jData["M"];

        var stringTemp = agTable.ToString();

        if (agTable == 2)
        {
            listValueChipBets = new List<long>() { 2, 10, 50, 100, 500 };
        }
        else
        {
            if (stringTemp[0].Equals('1'))
            {
                listValueChipBets = new List<long>() { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
            }
            else
            {
                listValueChipBets = new List<long>() { agTable, agTable * 2, agTable * 10, agTable * 20, agTable * 100 };
            }
        }
        setDisplayBet();
        handleDotsWaiting();
    }

    public override void handleVTable(string strData)
    {
        base.handleVTable(strData);

        stateGame = Globals.STATE_GAME.VIEWING;
        clock.gameObject.SetActive(false);
        btn_bet.SetActive(false);
        btn_deal.gameObject.SetActive(false);
        btn_clear.gameObject.SetActive(false);


        thisPlayer.playerView.setPosThanhBarThisPlayer();
        setStatusButtonsBet();

        JObject jData = JObject.Parse(strData);
        agTable = (int)jData["M"];

        var stringTemp = agTable.ToString();

        if (agTable == 2)
        {
            listValueChipBets = new List<long>() { 2, 10, 50, 100, 500 };
        }
        else
        {
            if (stringTemp[0].Equals('1'))
            {
                listValueChipBets = new List<long>() { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
            }
            else
            {
                listValueChipBets = new List<long>() { agTable, agTable * 2, agTable * 10, agTable * 20, agTable * 100 };
            }
        }

        int codeCardDealer = (int)jData["dealersOpenCard"];
        Debug.Log("card dealer ===== " + cardDealer);

        chiaBaiDealer(codeCardDealer, cardDealer);
        chiaBaiDealer(0, cardDealer);

        JArray arrP = (JArray)jData["ArrP"];
        Debug.Log("count arrP " + arrP.Count);
        viewIng(arrP);

        setDisplayBet();
        handleDotsWaiting();
    }

    public void viewIng(JArray arrP)
    {
        for (int i = 0; i < arrP.Count; i++)
        {
            JObject objPlayer = (JObject)arrP[i];
            int pid = (int)objPlayer["id"];

            Debug.Log("id player: " + pid);

            int AG = (int)objPlayer["AG"];
            bool isInsurance = (bool)objPlayer["insurance"];
            int chipInsuranceAG = (int)objPlayer["insuranceAG"];

            Player player = getPlayerWithID(pid);
            int indexPlayer = getDynamicIndex(getIndexOf(player));

            JArray playerHands = (JArray)objPlayer["playerHands"];

            Debug.Log("count playerHands " + playerHands.Count);//2

            for (int j = 0; j < playerHands.Count; j++)
            {
                if (playerHands[j] is JObject)
                {
                    JObject playerHand = (JObject)playerHands[j];

                    int chipBet = (int)playerHand["chip"];

                    int scoreBox = (int)playerHand["score"];
                    player.playerView.setAg(player.ag -= chipBet);

                    //bool stand = (bool)playerHands[0]["stand"];
                    //bool busted = (bool)playerHands[0]["busted"];
                    int hand = (int)playerHand["hand"];

                    GameObject boxCardPlayer = listCards[indexPlayer];
                    JArray cardsCode = (JArray)playerHand["cardsCode"];

                    Debug.Log("cardsCode player count==== " + cardsCode.Count);

                    for (int k = 0; k < cardsCode.Count; k++)
                    {
                        chiaBaiPlayer((int)cardsCode[k], boxCardPlayer, player, indexPlayer);
                    }

                    listDots[indexPlayer].SetActive(true);
                    listScoreBoxs[indexPlayer].SetActive(true);
                    listScoreBoxs[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = scoreBox.ToString();

                    //if (stand == true)
                    //{
                    //    listTextStandAction[indexPlayer].SetActive(true);
                    //}
                    //if (busted == true)
                    //{
                    //    listAniBust[indexPlayer].gameObject.SetActive(true);
                    //    listTextBust[indexPlayer].SetActive(true);
                    //}

                    Vector2 posPl = player.playerView.transform.localPosition;

                    chipBetColorInx = listValueChipBets.IndexOf(chipBet);
                    ChipBlackJack chip = setChip(chipBetColorInx);
                    chip.idPl = pid;
                    chip.chipValue = chipBet;
                    chip.transform.localPosition = posPl;
                    chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
                    chipMoveTo(chip, player, chipBet);
                }

            }

            if (isInsurance == true)
            {
                listChipBetInsure[indexPlayer].SetActive(true);
                listChipBetInsure[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipInsuranceAG, true);
            }
        }
    }

    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(objData);

        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        JObject jData = JObject.Parse(objData);
        agTable = (int)jData["M"];

        var stringTemp = agTable.ToString();

        if (agTable == 2)
        {
            listValueChipBets = new List<long>() { 2, 10, 50, 100, 500 };
        }
        else
        {
            if (stringTemp[0].Equals('1'))
            {
                listValueChipBets = new List<long>() { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
            }
            else
            {
                listValueChipBets = new List<long>() { agTable, agTable * 2, agTable * 10, agTable * 20, agTable * 100 };
            }
        }
    }

    public override void handleLTable(JObject data)
    {
        base.handleLTable(data);
    }

    public void TimeAction(JObject jData)
    {
        stateGame = Globals.STATE_GAME.PLAYING;

        setStatusButtonsBet();

        clock.gameObject.SetActive(true);
        btn_deal.gameObject.SetActive(true);
        btn_clear.gameObject.SetActive(true);
        btn_bet.SetActive(true);
        btn_clear.interactable = false;
        btn_deal.interactable = false;

        for (int i = 0; i < players.Count; i++)
        {
            int indexPlayer = getDynamicIndex(getIndexOf(players[i]));
            listDots[indexPlayer].SetActive(true);
        }

        //{ "evt":"start","data":"{\"time\":12000}"}
        JObject data = JObject.Parse((string)jData["data"]);
        int timeStart = (int)(getInt(data, "time") / 1000);
        int duration = timeStart;
        Debug.Log("duration === " + duration);
        lbTimeBet.text = timeStart.ToString();
        uiFill.DOFillAmount(0, timeStart);

        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            duration--;
            if (duration > 0)
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
            }
            lbTimeBet.text = duration + "";
        }).SetLoops(duration).SetId("SoundClock");

        curMoney = thisPlayer.ag;

        handleDotsWaiting();
        resetLabel();
        setDisplayBet();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.Kill("SoundClock");
    }

    public void BuyInsure(JObject data)
    {
        //"evt":"insuranceTime","data":"{"time":5000}"
        if (thisPlayer == null)
        {
            popupBuyInsure.SetActive(false);
        }
        popupBuyInsure.SetActive(true);
        JObject BiData = JObject.Parse((string)data["data"]);

        int timeStart = (int)(getInt(BiData, "time") / 1000);
        int duration = timeStart;
        Debug.Log("duration === " + duration);
        lbTimeBet1.text = timeStart.ToString();
        uiFill1.DOFillAmount(0, timeStart);

        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            duration--;
            if (duration > 0)
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
            }
            if (duration == 0)
            {
                popupBuyInsure.SetActive(false);
            }
            lbTimeBet1.text = duration + "";
        }).SetLoops(duration).SetId("SoundClock");

    }

    public void onClickButtonYes()
    {
        SocketSend.sendBlackjackInsure();
        popupBuyInsure.SetActive(false);
    }

    public void handleBuyInSurePlayer(JObject data)
    {
        //"evt":"playerInsured", "data":"{\"chip\":250,\"pid\":1013342}"
        JObject jData = JObject.Parse((string)data["data"]);
        int chipBet = (int)jData["chip"];
        int pid = (int)jData["pid"];
        Player player = getPlayerWithID(pid);
        int indexPlayer = getDynamicIndex(getIndexOf(player));

        listChipBetInsure[indexPlayer].SetActive(true);
        listChipBetInsure[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet);

        //ani lac la bai
        DOTween.Sequence()
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            checkBaiUp.gameObject.SetActive(true);
            checkBaiUp.Initialize(true);
            checkBaiUp.AnimationState.SetAnimation(0, "animation", false);
        })
        .AppendInterval(2.0f)
        .AppendCallback(() =>
        {
            //get chip insure va fly chip player
            listChipBetInsure[indexPlayer].transform.Find("chipInsure").DOLocalMove(new Vector2(0, 300), 1.0f);
            listChipBetInsure[indexPlayer].transform.Find("chipInsure").DOScale(new Vector2(0, 0), 1.2f);

            player.playerView.effectFlyMoney(-chipBet, 30);
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            listChipBetInsure[indexPlayer].SetActive(false);
        });
    }

    public void onClickClosePopupInsure()
    {
        popupBuyInsure.SetActive(false);
    }

    public void handleFinishGame(JObject fdata)
    {
        //{
        //    "evt":"finish",
        //    "data":"{
        //    "packets":[{"pid":5925158,"agAdd":-100,"AG":44036,"slot":0,"hand0":" - 100"}],
        //    "timeToStart":15500,
        //    "dealerCards":[{"value":"2","type":"SPADE","code":1},{"value":"2","type":"SPADE","code":1},{"value":"2","type":"DIAMOND","code":27},{"value":"6","type":"DIAMOND","code":31},{"value":"6","type":"CLUB","code":18}],
        //    "cardsCode":[1,1,27,31,18],
        //    "score":[2,4,6,12,18]
        //            }"
        //}

        btn_split.gameObject.SetActive(false);
        btn_stand.gameObject.SetActive(false);
        btn_hit.gameObject.SetActive(false);
        btn_double.gameObject.SetActive(false);

        setStatusButtonsBet();

        hideAllLights();
        foreach (GameObject x in listDots)
        {
            x.SetActive(false);
        }

        JObject data = JObject.Parse((string)fdata["data"]);

        int timeRemain = (int)data["timeToStart"];
        JArray listCardsCode = (JArray)data["cardsCode"];
        JArray listScore = (JArray)data["score"];
        JArray packetsPlayer = (JArray)data["packets"];


        DOTween.Sequence()
        .AppendInterval(1f)
        .AppendCallback(() =>
        {
            cardDealer.transform.DOScale(new Vector2(1.5f, 1.5f), 0.2f);
            scoreBox_dealer.transform.DOScale(new Vector2(1.5f, 1.5f), 0.2f);
            scoreBox_dealer.transform.localPosition = new Vector2(75, 218);

            checkBaiUp.gameObject.SetActive(false);
        })
        .AppendInterval(1f)
        .AppendCallback(() =>
        {
            listCardDealer[1].gameObject.transform.DOScale(new Vector2(0f, 0.5f), 0.2f)
            .OnComplete(() =>
            {
                listCardDealer[1].setTextureWithCode((int)listCardsCode[1]);
                listCardDealer[1].gameObject.transform.DOScale(new Vector2(0.5f, 0.5f), 0.2f);
            });
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            scoreBox_dealer.SetActive(true);
            scoreBox_dealer.transform.GetComponentInChildren<TextMeshProUGUI>().text = listScore[1].ToString();
        })
        .AppendInterval(1.5f)
        .AppendCallback(() =>
        {
            for (int i = 2; i < listCardsCode.Count; i++)
            {
                int count = i;
                DOTween.Sequence()
                .AppendInterval(1.0f * i)
                .AppendCallback(() =>
                {
                    chiaBaiDealer((int)listCardsCode[count], cardDealer);
                    sortCardPlayer(listCardDealer);
                    scoreBox_dealer.SetActive(false);
                })
                .AppendInterval(0.25f * i)
                .AppendCallback(() =>
                {
                    scoreBox_dealer.SetActive(true);
                    scoreBox_dealer.GetComponentInChildren<TextMeshProUGUI>().text = listScore[count].ToString();
                    scoreBox_dealer.transform.localPosition = new Vector2(75 + (listCardDealer.Count - 2) * 23, 218);
                });
            }
        })
        .AppendInterval(1.25f * (listCardsCode.Count - 1))
        .AppendCallback(() =>
        {
            checkResultScoreDealer((int)listScore[listScore.Count - 1]);
        })
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            checkResultDealerAndPlayer(packetsPlayer, (int)listScore[listScore.Count - 1]);
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            hideTextAndAniEndGame();
        })
        .AppendInterval(3.0f)
        .AppendCallback(() =>
        {
            thuBai();
        })
        .AppendInterval(1.5f)
        .AppendCallback(() =>
        {
            resetGame();
        });
    }

    public void hideTextAndAniEndGame()
    {
        foreach (SkeletonGraphic x in listAniBlackJack)
        {
            x.gameObject.SetActive(false);
        }

        foreach (GameObject x in listTextBust)
        {
            x.SetActive(false);
        }
        foreach (GameObject x in listTextWin)
        {
            x.SetActive(false);
        }
        foreach (GameObject x in listTextLose)
        {
            x.SetActive(false);
        }
        foreach (GameObject x in listTextPush)
        {
            x.SetActive(false);
        }
        foreach (GameObject x in listScoreBoxs)
        {
            x.SetActive(false);
            x.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        scoreBox_dealer.SetActive(false);
        textBustDealer.SetActive(false);
        textWinDealer.SetActive(false);
        textLoseDealer.SetActive(false);
        textPushDealer.SetActive(false);

        foreach (Card card in listCardInTable)
        {
            card.transform.Find("mask").gameObject.SetActive(false);
            card.transform.Find("blink").gameObject.SetActive(false);
        }
    }

    public void checkResultScoreDealer(int scoreDealer)
    {
        if (scoreDealer > 21)
        {
            isDealerBusted = true;
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                aniBustDealer.gameObject.SetActive(true);
                aniBustDealer.AnimationState.SetAnimation(0, "animation", false);
                aniBustDealer.Initialize(true);
                scoreBox_dealer.transform.Find("imgBust").gameObject.SetActive(true);

                foreach (Card card in listCardDealer)
                {
                    card.transform.Find("mask").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                aniBustDealer.gameObject.SetActive(false);
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                cardDealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                scoreBox_dealer.transform.DOLocalMove(new Vector2(50 + (listCardDealer.Count - 2) * 13, 195), 0.1f);
                scoreBox_dealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                sortCardPlayer(listCardDealer);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                textBustDealer.gameObject.SetActive(true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                textLoseDealer.SetActive(true);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                textLoseDealer.SetActive(false);
            });
        }

        if (scoreDealer == 21)
        {
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                aniWowDealer.gameObject.SetActive(true);
                aniWowDealer.AnimationState.SetAnimation(0, "animation", false);
                aniWowDealer.Initialize(true);
                scoreBox_dealer.transform.Find("imgWow").gameObject.SetActive(true);

                foreach (Card card in listCardDealer)
                {
                    card.transform.Find("blink").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                aniWowDealer.gameObject.SetActive(false);
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                cardDealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                scoreBox_dealer.transform.DOLocalMove(new Vector2(50 + (listCardDealer.Count - 2) * 13, 195), 0.1f);
                scoreBox_dealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                sortCardPlayer(listCardDealer);
            });
        }

        if (scoreDealer < 21)
        {
            cardDealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
            scoreBox_dealer.transform.DOLocalMove(new Vector2(50 + (listCardDealer.Count - 2) * 13, 195), 0.1f);
            scoreBox_dealer.transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
            sortCardPlayer(listCardDealer);
        }
    }

    public void checkResultDealerAndPlayer(JArray packetsPlayer, int scoreDealer)
    {
        if (packetsPlayer.Count == 0)
        {
            textWinDealer.SetActive(true);
        }

        for (int i = 0; i < packetsPlayer.Count; i++)
        {
            JObject dataPlayer = (JObject)packetsPlayer[i];
            int pid = (int)dataPlayer["pid"];
            int agAdd = (int)dataPlayer["agAdd"];
            int AG = (int)dataPlayer["AG"];

            Player player = getPlayerWithID(pid);

            int indexPlayer = getDynamicIndex(getIndexOf(player));

            string textScoreBox = listScoreBoxs[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text;
            string[] parts = textScoreBox.Split("/");

            int scoreBoxPlayer = int.Parse(parts[0]);
            Debug.Log("scoreBoxPlayer: " + i + "=== " + scoreBoxPlayer);


            if (isDealerBusted || scoreDealer < scoreBoxPlayer)
            {
                //Debug.Log("scoreDealer 2");
                textLoseDealer.SetActive(true);
                listTextWin[indexPlayer].SetActive(true);
                payChipWin(indexPlayer, agAdd, player);
            }
            else if (!isDealerBusted && (scoreDealer > scoreBoxPlayer || scoreBoxPlayer > 21))
            {
                //Debug.Log("scoreDealer 11");
                //Debug.Log("scoreBoxPlayer==== " + scoreBoxPlayer);
                textWinDealer.SetActive(true);
                //Debug.Log("scoreDealer 12");
                listTextLose[indexPlayer].SetActive(true);
                //Debug.Log("scoreDealer 13");
                getChipLose(indexPlayer);
            }
            else if (scoreDealer == scoreBoxPlayer)
            {
                //Debug.Log("scoreDealer 3");
                listTextPush[indexPlayer].SetActive(true);
                pushChip(indexPlayer, player);
            }
            player.ag = AG;
            player.setAg();
        }
    }

    public void payChipWin(int indexPlayer, int agAdd, Player pl)
    {

        Debug.Log("index PLayer: " + indexPlayer);

        Vector2 posModel = new Vector2(0, 300);
        for (int i = 0; i < listChipInTable[indexPlayer].Count; i++)
        {
            var chip = listChipInTable[indexPlayer][i];
            if (agAdd > 0)
            {
                //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);

                ChipBlackJack chip1 = setChip(chip.chipSprite);
                chip1.transform.localPosition = posModel;
                Vector2 posChip = chip.transform.localPosition;
                posChip.y += 50f;
                chip1.transform.DOLocalMove(posChip, 0.5f);
                chip1.idPl = chip.idPl;
                chip1.chipValue = chip.chipValue;
                chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                listChipsPay.Add(chip1);
            }
        }
        listChipInTable[indexPlayer].AddRange(listChipsPay);
        listChipsPay.Clear();
        DOTween.Sequence()
            .AppendInterval(1.5f)
            .AppendCallback(() =>
            {
                for (int i = 0; i < listChipInTable[indexPlayer].Count; i++)
                {
                    var chip = listChipInTable[indexPlayer][i];
                    if (agAdd > 0)
                    {
                        //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.WIN);
                        // get player bet win position
                        PlayerViewBlackJack plView = (PlayerViewBlackJack)pl.playerView;
                        Vector2 posPlayer = plView.transform.localPosition;
                        chip.transform.DOLocalMove(posPlayer, 0.6f);
                        listBoxCard[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        pl.playerView.effectFlyMoney(agAdd, 30);
                    }
                }
            });
    }

    public void pushChip(int indexPlayer, Player pl)
    {
        Debug.Log("index PLayer: " + indexPlayer);
        listBoxCard[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = "";

        for (int i = 0; i < listChipInTable[indexPlayer].Count; i++)
        {
            ChipBlackJack chip = listChipInTable[indexPlayer][i];
            //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.GET_CHIP);
            PlayerViewBlackJack plView = (PlayerViewBlackJack)pl.playerView;
            Vector2 posPlayer = plView.transform.localPosition;
            chip.transform.DOLocalMove(posPlayer, 0.6f);
        }
    }

    public void thuBai()
    {
        Debug.Log("length list bai in table: " + listCardInTable.Count);
        foreach (Card card in listCardInTable)
        {
            int index = listCardInTable.IndexOf(card);
            card.transform.SetParent(cardDeckRight.transform);
            DOTween.Sequence()
                    .AppendInterval(index * 0.2f)
                    .AppendCallback(() =>
                    {
                        card.setTextureWithCode(0);
                        card.gameObject.transform.DOLocalMove(cardDeckLeft.transform.localPosition, 0.5f);
                        card.gameObject.transform.DOLocalRotate(new Vector3(0, 0, -64.48f), 0.5f);
                        card.gameObject.transform.DOScale(new Vector2(0.38f, 0.4f), 0.5f);
                    })
                    .AppendInterval(0.6f)
                    .AppendCallback(() =>
                    {
                        card.gameObject.SetActive(false);
                    });
        }
    }

    public void handlelc(JObject jData)
    {
        //{ "dealersOpenCard":{ "value":"5","type":"SPADE","code":4},
        //"dealersOpenCardCode":4,"dealerScore":5,
        //"playerCard":[
        //              { "pid":6042108,"cardJsons":[{ "value":"10","type":"HEART","code":48},
        //                                           { "value":"6","type":"DIAMOND","code":31}
        //                                          ],
        //                "cardsCode":[48,31],"score":"16","slot":0
        //              },
        //              { "pid":1013239,"cardJsons":[{ "value":"9","type":"SPADE","code":8},
        //                                           { "value":"A","type":"CLUB","code":26}
        //                                          ],
        //                "cardsCode":[8,26],"score":"10/20","slot":1
        //              }
        //             ],
        //"time":2500}
        stateGame = Globals.STATE_GAME.PLAYING;
        clock.gameObject.SetActive(false);
        btn_bet.SetActive(false);
        btn_deal.gameObject.SetActive(false);
        btn_clear.gameObject.SetActive(false);
        foreach (GameObject x in listDots)
        {
            x.SetActive(false);
        }

        string fakeJData = "{\"dealersOpenCard\":{\"value\":\"6\",\"type\":\"DIAMOND\",\"code\":31},\"dealersOpenCardCode\":31,\"dealerScore\":6,\"playerCard\":[{\"pid\":5925158,\"cardJsons\":[{\"value\":\"4\",\"type\":\"SPADE\",\"code\":3},{\"value\":\"4\",\"type\":\"HEART\",\"code\":42}],\"cardsCode\":[3,42],\"score\":\"8\",\"slot\":1}],\"time\":2500}";
        JObject dataLc = JObject.Parse(fakeJData);

        //JObject dataLc = JObject.Parse((string)jData["data"]);
        JObject dealer = (JObject)dataLc["dealersOpenCard"];
        JArray playerCard = (JArray)dataLc["playerCard"];
        int codeCardDealer = (int)dealer["code"];

        DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
        {
            chiaBaiDealer(codeCardDealer, cardDealer);
        })
        .AppendInterval(0.4f * playerCard.Count + 0.2f)
        .AppendCallback(() =>
        {
            chiaBaiDealer(0, cardDealer);
        });

        for (int i = 0; i < playerCard.Count; i++)
        {
            JObject dataCardPl = (JObject)playerCard[i];
            int pid = (int)dataCardPl["pid"];
            Player pl = getPlayerWithID(pid);
            int indexPlayer = getDynamicIndex(getIndexOf(pl));

            GameObject boxCardPlayer = listCards[indexPlayer];
            JArray cardsCode = (JArray)dataCardPl["cardsCode"];


            DOTween.Sequence().AppendInterval(0.15f).AppendCallback(() =>
            {
                chiaBaiPlayer((int)cardsCode[0], boxCardPlayer, pl, indexPlayer);
            })
            .AppendInterval(0.4f * (i + 1))
            .AppendCallback(() =>
            {
                chiaBaiPlayer((int)cardsCode[1], boxCardPlayer, pl, indexPlayer);
            })
            ;
            DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
            {
                setStatus(pl.playerView.cards[0], pl.playerView.cards[1]);
            });

            string score = (string)dataCardPl["score"];
            Debug.Log("score Box " + i + ":" + score);

            DOTween.Sequence()
                .AppendInterval(playerCard.Count * 0.4f + 0.1f * i + 1.2f)
                .AppendCallback(() =>
                {
                    listScoreBoxs[indexPlayer].SetActive(true);
                    listScoreBoxs[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = score;
                });
        }
    }

    public void chiaBaiPlayer(int code, GameObject obj, Player player, int indexPlayer)
    {
        Card card_deal = Instantiate(cardPrefab.gameObject, cardDeckRight.transform).GetComponent<Card>();

        player.playerView.cards.Add(card_deal);
        listCardInTable.Add(card_deal);
        card_deal.setTextureWithCode(0);
        card_deal.transform.localScale = new Vector2(0f, 0f);
        card_deal.transform.localPosition = new Vector2(307, 300);
        card_deal.transform.localEulerAngles = new Vector3(0, 0, 64.48f);
        card_deal.transform.SetParent(obj.transform);
        card_deal.transform.SetAsLastSibling();
        card_deal.gameObject.SetActive(true);
        List<Vector2> listPosCard = getPosSortCard(player.playerView.cards.Count);

        DOTween.Sequence()
            .AppendCallback(() =>
            {
                sortCardPlayer(player.playerView.cards);
                card_deal.transform.DOScale(new Vector2(0.5f, 0.5f), 0.5f);
                card_deal.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);

            })
            .AppendInterval(0.7f)
            .AppendCallback(() =>
            {
                card_deal.transform.DOScale(new Vector2(0f, 0.5f), 0.1f)
                .OnComplete(() =>
                {
                    card_deal.setTextureWithCode(code);
                    card_deal.transform.DOScale(new Vector2(0.5f, 0.5f), 0.1f);
                });
            });
    }

    public void chiaBaiDealer(int code, GameObject obj)
    {
        Card card_deal = Instantiate(cardPrefab.gameObject, cardDeckRight.transform).GetComponent<Card>();

        listCardDealer.Add(card_deal);
        listCardInTable.Add(card_deal);
        card_deal.setTextureWithCode(0);
        card_deal.transform.localScale = new Vector2(0f, 0f);
        card_deal.transform.localPosition = new Vector2(307, 300);
        card_deal.transform.localEulerAngles = new Vector3(0, 0, 64.48f);
        card_deal.transform.SetParent(obj.transform);
        card_deal.transform.SetAsLastSibling();
        card_deal.gameObject.SetActive(true);
        List<Vector2> listPosCard = getPosSortCard(listCardDealer.Count);

        DOTween.Sequence()
            .AppendCallback(() =>
            {
                sortCardPlayer(listCardDealer);
                card_deal.transform.DOScale(new Vector2(0.5f, 0.5f), 0.5f);
                card_deal.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);

            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                card_deal.transform.DOScale(new Vector2(0f, 0.4f), 0.1f)
                .OnComplete(() =>
                {
                    card_deal.setTextureWithCode(code);
                    card_deal.transform.DOScale(new Vector2(0.5f, 0.5f), 0.1f);
                });
            });
    }

    public void sortCardPlayer(List<Card> list, bool isEffectMove = true)
    {

        for (int i = 0; i < list.Count; i++)
        {
            Vector2 pos = new Vector2((i - ((int)(list.Count / 2) + (list.Count % 2 == 0 ? -0.5f : 0))) * 25f, 0);
            if (!isEffectMove)
            {
                list[i].transform.localPosition = pos;
            }
            else
            {
                Debug.Log("sortCardPlayer:" + pos.x);
                list[i].transform.DOLocalMove(pos, 0.5f);
            }

            //Debug.Log("DKMMMM-------x===" + list[i].transform.localPosition.x);
        }
    }
    public void sortCardPlayer2(List<Card> list, bool isEffectMove = true)
    {

        for (int i = 0; i < list.Count - 1; i++)
        {
            Vector2 pos = new Vector2((i - ((int)(list.Count / 2) + (list.Count % 2 == 0 ? -0.5f : 0))) * 25f, 0);
            if (!isEffectMove)
            {
                list[i].transform.localPosition = pos;
            }
            else
            {
                Debug.Log("sortCardPlayer:" + pos.x);
                list[i].transform.DOLocalMove(pos, 0.5f);
            }

            //Debug.Log("DKMMMM-------x===" + list[i].transform.localPosition.x);
        }
    }
    public List<Vector2> getPosSortCard(int sizeCard)
    {
        List<Vector2> listPos = new List<Vector2>();
        for (int i = 0; i < sizeCard; i++)
        {
            listPos.Add(new Vector2((i - ((int)(sizeCard / 2) + (sizeCard % 2 == 0 ? -0.5f : 0))) * 25f, 0));
        }
        return listPos;
    }

    public void handleTurnPlayer(JObject jdata)
    {
        //"evt":"decisionTurn",
        //"data":"{"pid":6042108,"hand":0,"slot":2,"time":12000,"blackjack":false,"AG":0,"agAdd":0}"
        JObject data = JObject.Parse((string)jdata["data"]);
        Debug.Log("data:  " + data);
        int pid = (int)data["pid"];
        Player pl = getPlayerWithID(pid);
        int indexPlayer = getDynamicIndex(getIndexOf(pl));
        int hand = (int)data["hand"];
        int slotPL = (int)data["slot"];
        bool blackjack = (bool)data["blackjack"];
        int timeRemain = (int)data["time"] / 1000;
        hideAllLights();
        listLights[indexPlayer].SetActive(true);

        pl.playerView.setTurn(true, timeRemain - 1);
        if (pl != thisPlayer)
        {
            if (timeRemain == 0 || blackjack == true)
            {
                pl.playerView.setTurn(false);

                Debug.Log("time remain player: " + timeRemain);

                listAniBlackJack[indexPlayer].gameObject.SetActive(true);
                listAniBlackJack[indexPlayer].AnimationState.SetAnimation(0, "animation", false);
                listAniBlackJack[indexPlayer].Initialize(true);
            }

            listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
            listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
            btn_split.gameObject.SetActive(false);
            btn_stand.gameObject.SetActive(false);
            btn_hit.gameObject.SetActive(false);
            btn_double.gameObject.SetActive(false);
        }

        if (pl == thisPlayer)
        {
            if (timeRemain == 0 || blackjack == true)
            {
                pl.playerView.setTurn(false);
                Debug.Log("time remain player: " + timeRemain);


                listAniBlackJack[indexPlayer].gameObject.SetActive(true);
                listAniBlackJack[indexPlayer].AnimationState.SetAnimation(0, "animation", false);
                listAniBlackJack[indexPlayer].Initialize(true);

                listCards[0].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.2f);
                listScoreBoxs[0].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.2f);
            }
            else
            {
                listCards[0].transform.DOLocalMove(new Vector2(0, 120), 0.2f);
                listCards[0].transform.DOScale(new Vector2(1.5f, 1.5f), 0.2f);
                listScoreBoxs[0].transform.DOLocalMove(new Vector2(75, 190), 0.2f);
                listScoreBoxs[0].transform.DOScale(new Vector2(1.5f, 1.5f), 0.2f);

                btn_split.gameObject.SetActive(true);
                btn_stand.gameObject.SetActive(true);
                btn_hit.gameObject.SetActive(true);
                btn_double.gameObject.SetActive(true);
            }
        }
    }

    public void hideAllLights()
    {
        foreach (GameObject x in listLights)
        {
            x.SetActive(false);
        }
    }

    public void handleActionPlayer(String evt, JObject jdata)
    {
        JObject data = JObject.Parse((string)jdata["data"]);
        Debug.Log(data);
        int pid = (int)data["pid"];
        Player pl = getPlayerWithID(pid);
        int indexPlayer = getDynamicIndex(getIndexOf(pl));
        int agBet = (int)data["agBet"];
        Vector3 newPos1 = new Vector3(-12.5f, 64, 0);

        switch (evt)
        {
            //{ "evt":"playerStood",
            //"data":"{\"pid\":5925158,\"name\":\"tuntun34\",\"hand\":0,\"slot\":0,\"AG\":46393,\"agBet\":100}"}
            case "playerStood":
                Debug.Log("chay vao stand.......");

                flyTextActionOnClick(listTextStandAction, indexPlayer);

                pl.playerView.setTurn(false, 0);

                if (pl != thisPlayer)
                {
                    btn_split.gameObject.SetActive(false);
                    btn_stand.gameObject.SetActive(false);
                    btn_hit.gameObject.SetActive(false);
                    btn_double.gameObject.SetActive(false);
                    listScoreBoxs[indexPlayer].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                }
                sortCardPlayer(pl.playerView.cards);

                if (pl == thisPlayer)
                {
                    listCards[0].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                    listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.2f);
                    listScoreBoxs[0].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                    listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.2f);

                }
                break;

            //{ "evt":"playerHit",
            //"data":"{\"pid\":5925158,\"AG\":46693,\"name\":\"tuntun34\",\"card\":{\"value\":\"4\",\"type\":\"HEART\",\"code\":42},\"cardsCode\":42,\"score\":\"22\",\"hand\":0,\"slot\":0,\"agBet\":100,\"busted\":true,\"agAdd\":-100}"}
            case "playerHit":
                Debug.Log("chay vao hit......");
                int cardCode = (int)data["cardsCode"];
                string score = (string)data["score"];
                bool isBusted = (bool)data["busted"];
                int agAdd = (int)data["agAdd"];
                Debug.Log("agADD === " + agAdd);

                listTextHitAction[indexPlayer].SetActive(true);
                btn_hit.interactable = false;
                DOTween.Sequence()
                .Append(listTextHitAction[indexPlayer].transform.DOLocalMove(new Vector2(0, 80), 1f).SetRelative(true))
                .Join(listTextHitAction[indexPlayer].transform.DORotate(new Vector3(0, 0, 20), 0.25f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo))
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    btn_hit.interactable = true;
                    listTextHitAction[indexPlayer].SetActive(false);
                    listTextHitAction[indexPlayer].transform.localPosition = Vector2.zero;
                });

                chiaBaiPlayer(cardCode, listCards[indexPlayer], pl, indexPlayer);

                if (pl == thisPlayer)
                {
                    handleBustAndBlackJackOfThisPlayer(isBusted, pl, score, agAdd);
                    btn_double.interactable = false;
                }
                else
                {
                    handleBustAndBlackJackOfOtherPlayer(isBusted, pl, score, indexPlayer, agAdd);
                }
                break;

            //{ "evt":"playerDoubled",
            //"data":"{\"pid\":5925158,\"name\":\"tuntun34\",\"agBet\":200,\"card\":{\"value\":\"6\",\"type\":\"DIAMOND\",\"code\":31},\"cardsCode\":31,\"hand\":0,\"score\":17,\"busted\":false,\"agAdd\":0,\"AG\":0}"}
            case "playerDoubled":
                Debug.Log("chay vao double......");

                handleChipDouble(pl, indexPlayer, pid);
                flyTextActionOnClick(listTextDoubleAction, indexPlayer);

                pl.playerView.setTurn(false, 0);
                int cardCode2 = (int)data["cardsCode"];
                string score2 = (string)data["score"];
                bool isBusted1 = (bool)data["busted"];
                int agAdd2 = (int)data["agAdd"];
                Debug.Log("agADD === " + agAdd2);
                chiaBaiPlayer(cardCode2, listCards[indexPlayer], pl, indexPlayer);

                if (pl == thisPlayer)
                {
                    handleBustAndBlackJackOfThisPlayer(isBusted1, pl, score2, agAdd2);

                    DOTween.Sequence()
                   .AppendInterval(1.5f)
                   .AppendCallback(() =>
                   {
                       listCards[0].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                       listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                       listScoreBoxs[0].transform.DOLocalMove(new Vector2(60, 110), 0.1f);
                       listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                       sortCardPlayer(pl.playerView.cards);
                   });
                }
                else
                {
                    handleBustAndBlackJackOfOtherPlayer(isBusted1, pl, score2, indexPlayer, agAdd2);
                }
                break;

            case "playerSplit":
                {
                    //"evt":"playerSplit",
                    //"data":"{"pid":1231283,"name":"Jing Ca�ares L Dawal","slot":1,
                    //          "firstHand":{"chip":100,"hand":0,"cards":[{"value":"5","type":"CLUB","code":17},{"value":"9","type":"CLUB","code":21}],"cardsCode":[17,21],"score":"14"},
                    //          "secondHand":{"chip":100,"hand":1,"cards":[{"value":"5","type":"HEART","code":43},{"value":"2","type":"SPADE","code":1}],"cardsCode":[43,1],"score":"7"},
                    //          "time":13500}"}
                    Debug.Log("chay vao split.....");

                    flyTextActionOnClick(listTextSplitAction, indexPlayer);

                    JObject firstHand = (JObject)data["firstHand"];
                    JObject secondHand = (JObject)data["secondHand"];
                    int time = (int)data["time"];

                    int chip1 = (int)firstHand["chip"];
                    int chip2 = (int)secondHand["chip"];

                    int hand1 = (int)firstHand["hand"];
                    int hand2 = (int)secondHand["hand"];

                    JArray cardsCode1 = (JArray)firstHand["cardsCode"];
                    JArray cardsCode2 = (JArray)secondHand["cardsCode"];

                    int scoreBox1 = (int)firstHand["score"];
                    int scoreBox2 = (int)secondHand["score"];

                    listBoxSecondHand[indexPlayer].SetActive(true);
                    foreach (Card x in pl.playerView.cards)
                    {
                        pl.playerView.cards.Remove(x);
                    }
                    pl.playerView.cards.Clear();

                    for (int i = 0; i < cardsCode1.Count; i++)
                    {
                        Card card_deal = Instantiate(cardPrefab.gameObject, listCards[indexPlayer].transform).GetComponent<Card>();

                        listCardInFirstHand.Add(card_deal);
                        listCardInTable.Add(card_deal);
                        card_deal.setTextureWithCode((int)cardsCode1[i]);
                        card_deal.transform.localScale = new Vector2(0.5f, 0.5f);
                        card_deal.transform.SetAsLastSibling();
                        card_deal.gameObject.SetActive(true);
                    }

                    for (int i = 0; i < cardsCode2.Count; i++)
                    {
                        Card card_deal = Instantiate(cardPrefab.gameObject, listCards[indexPlayer].transform).GetComponent<Card>();

                        listCardInSecondHand.Add(card_deal);
                        listCardInTable.Add(card_deal);
                        card_deal.setTextureWithCode((int)cardsCode2[i]);
                        card_deal.transform.localScale = new Vector2(0.5f, 0.5f);
                        card_deal.transform.SetAsLastSibling();
                        card_deal.gameObject.SetActive(true);
                    }

                    sortCardPlayer(listCardInFirstHand);
                    sortCardPlayer(listCardInSecondHand);
                    listScoreBoxs[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = scoreBox1.ToString();
                    listBoxSecondHand[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = scoreBox2.ToString();

                    break;
                }
        }
    }

    public void flyTextActionOnClick(List<GameObject> listText, int indexPlayer)
    {

        listText[indexPlayer].SetActive(true);
        DOTween.Sequence()
        .Append(listText[indexPlayer].transform.DOLocalMove(new Vector2(0, 80), 1f).SetRelative(true))
        .Join(listText[indexPlayer].transform.DORotate(new Vector3(0, 0, 20), 0.25f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo))
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            listText[indexPlayer].SetActive(false);
            listText[indexPlayer].transform.localPosition = Vector2.zero;
        });
    }

    public void handleChipDouble(Player pl, int indexPlayer, int pid)
    {
        Vector2 posPl = pl.playerView.transform.localPosition;

        if (indexPlayer == 0)
        {
            saveCurChipBC0 *= 2;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC0);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC0;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC0, true);
            chipMoveTo(chip, pl, saveCurChipBC0);
            pl.playerView.effectFlyMoney(-(int)(saveCurChipBC0 / 2), 30);
            pl.ag -= (int)(saveCurChipBC0 / 2);
            pl.setAg();

        }
        else if (indexPlayer == 1)
        {
            saveCurChipBC1 *= 2;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC1);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC1;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC1, true);
            chipMoveTo(chip, pl, saveCurChipBC1);
            pl.playerView.effectFlyMoney(-(int)(saveCurChipBC1 / 2), 30);
            pl.ag -= (int)(saveCurChipBC1 / 2);
            pl.setAg();
        }
        else
        {
            saveCurChipBC2 *= 2;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC2);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC2;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC2, true);
            chipMoveTo(chip, pl, saveCurChipBC2);
            pl.playerView.effectFlyMoney(-(int)(saveCurChipBC2 / 2), 30);
            pl.ag -= (int)(saveCurChipBC2 / 2);
            pl.setAg();
        }
    }

    public void handleBustAndBlackJackOfThisPlayer(bool isBusted, Player pl, string score, int agAdd)
    {
        listScoreBoxs[0].SetActive(false);
        DOTween
        .Sequence()
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            listScoreBoxs[0].SetActive(true);
            listScoreBoxs[0].GetComponentInChildren<TextMeshProUGUI>().text = score;

            listScoreBoxs[0].transform.localPosition = new Vector2(75 + (pl.playerView.cards.Count - 2) * 20, 190);
        });

        if (isBusted == true)
        {
            pl.playerView.setTurn(false);
            //ani busst
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniBust[0].gameObject.SetActive(true);
                listAniBust[0].transform.localPosition = new Vector2(0, 120);
                listAniBust[0].AnimationState.SetAnimation(0, "animation", false);
                listAniBust[0].Initialize(true);
                listScoreBoxs[0].transform.Find("imgBust").gameObject.SetActive(true);

                foreach (Card card in pl.playerView.cards)
                {
                    card.transform.Find("mask").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniBust[0].gameObject.SetActive(false);
                listAniBust[0].transform.localPosition = Vector2.zero;

                btn_split.gameObject.SetActive(false);
                btn_stand.gameObject.SetActive(false);
                btn_hit.gameObject.SetActive(false);
                btn_double.gameObject.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listCards[0].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                listScoreBoxs[0].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);

                sortCardPlayer(pl.playerView.cards);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                listTextBust[0].gameObject.SetActive(true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listTextLose[0].SetActive(true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                //thu chip va fly chip
                getChipLose(0);
                pl.playerView.effectFlyMoney(agAdd, 30);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                listTextLose[0].SetActive(false);
            })
            ;
        }

        if (score == "21" || score == "11/21")
        {
            pl.playerView.setTurn(false);
            //ani wow, card blink
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniWow[0].gameObject.SetActive(true);
                listAniWow[0].transform.localPosition = new Vector2(0, 120);
                listAniWow[0].AnimationState.SetAnimation(0, "animation", false);
                listAniWow[0].Initialize(true);
                listScoreBoxs[0].transform.Find("imgWow").gameObject.SetActive(true);

                foreach (Card card in pl.playerView.cards)
                {
                    Debug.Log("chay vao blink la bai............");
                    card.transform.Find("blink").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniWow[0].gameObject.SetActive(false);
                listAniWow[0].transform.localPosition = Vector2.zero;

                btn_split.gameObject.SetActive(false);
                btn_stand.gameObject.SetActive(false);
                btn_hit.gameObject.SetActive(false);
                btn_double.gameObject.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listCards[0].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                listScoreBoxs[0].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                listCards[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                listScoreBoxs[0].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);

                sortCardPlayer(pl.playerView.cards);
            })
            //tra chip va cuoi tran push chip ve player
            //.AppendInterval(1.0f)
            //.AppendCallback(() =>
            //{
            //    Vector2 posModel = new Vector2(0, 300);
            //    for (int i = 0; i < listChipInTable[0].Count; i++)
            //    {
            //        var chip = listChipInTable[0][i];
            //        if (int.Parse(listBoxCard[0].GetComponentInChildren<TextMeshProUGUI>().text) > 0)
            //        {
            //            //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);

            //            ChipBlackJack chip1 = setChip(chip.chipSprite);
            //            chip1.transform.localPosition = posModel;
            //            Vector2 posChip = chip.transform.localPosition;
            //            posChip.y += 50f;
            //            chip1.transform.DOLocalMove(posChip, 0.5f);
            //            chip1.idPl = chip.idPl;
            //            chip1.chipValue = chip.chipValue;
            //            chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);

            //            listChipInTable[0].Add(chip1);
            //        }
            //    }
            //})
            ;
        }
    }

    public void handleBustAndBlackJackOfOtherPlayer(bool isBusted, Player pl, string score, int indexPlayer, int agAdd)
    {
        listScoreBoxs[indexPlayer].SetActive(false);

        DOTween
        .Sequence()
        .AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            listScoreBoxs[indexPlayer].SetActive(true);
            listScoreBoxs[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = score;
            listScoreBoxs[indexPlayer].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);

            sortCardPlayer(pl.playerView.cards);
        });

        if (isBusted == true)
        {
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniBust[indexPlayer].gameObject.SetActive(true);
                listAniBust[indexPlayer].AnimationState.SetAnimation(0, "animation", false);
                listAniBust[indexPlayer].Initialize(true);
                listScoreBoxs[indexPlayer].transform.Find("imgBust").gameObject.SetActive(true);

                foreach (Card card in pl.playerView.cards)
                {
                    card.transform.Find("mask").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniBust[indexPlayer].gameObject.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listScoreBoxs[indexPlayer].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                sortCardPlayer(pl.playerView.cards);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                listTextBust[indexPlayer].gameObject.SetActive(true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listTextLose[indexPlayer].SetActive(true);

            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                //thu chip va fly chip
                getChipLose(indexPlayer);
                pl.playerView.effectFlyMoney(agAdd, 30);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                listTextLose[indexPlayer].SetActive(false);
            })
            ;
        }

        if (score == "21" || score == "11/21")
        {
            //ani wow, card blink
            DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniWow[indexPlayer].gameObject.SetActive(true);
                listAniWow[indexPlayer].AnimationState.SetAnimation(0, "animation", false);
                listAniWow[indexPlayer].Initialize(true);
                listScoreBoxs[indexPlayer].transform.Find("imgWow").gameObject.SetActive(true);

                foreach (Card card in pl.playerView.cards)
                {
                    Debug.Log("chay vao blink la bai............");
                    card.transform.Find("blink").gameObject.SetActive(true);
                }
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listAniWow[indexPlayer].gameObject.SetActive(false);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                listCards[indexPlayer].transform.DOLocalMove(new Vector2(0, 64), 0.1f);
                listCards[indexPlayer].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);
                listScoreBoxs[indexPlayer].transform.DOLocalMove(new Vector2(47 + (pl.playerView.cards.Count - 2) * 13, 110), 0.1f);
                listScoreBoxs[indexPlayer].transform.DOScale(new Vector2(1.0f, 1.0f), 0.1f);

                sortCardPlayer(pl.playerView.cards);
            });
            //.AppendInterval(1.0f)
            //.AppendCallback(() =>
            //{
            //    Vector2 posModel = new Vector2(0, 300);
            //    for (int i = 0; i < listChipInTable[indexPlayer].Count; i++)
            //    {
            //        var chip = listChipInTable[indexPlayer][i];
            //        if (int.Parse(listBoxCard[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text) > 0)
            //        {
            //            //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);
            //            ChipBlackJack chip1 = setChip(chip.chipSprite);
            //            chip1.transform.localPosition = posModel;
            //            Vector2 posChip = chip.transform.localPosition;
            //            posChip.y += 50f;
            //            chip1.transform.DOLocalMove(posChip, 0.5f);
            //            chip1.idPl = chip.idPl;
            //            chip1.chipValue = chip.chipValue;
            //            chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);

            //            listChipInTable[indexPlayer].Add(chip1);
            //        }
            //    }
            //})
            //;
        }
    }

    public void getChipLose(int indexPlayer)
    {

        //Debug.Log("index PLayer: " + indexPlayer);
        listBoxCard[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = "";

        //Debug.Log("list chip in table of index: " + indexPlayer + "=== " + listChipInTable[indexPlayer].Count);
        for (int i = 0; i < listChipInTable[indexPlayer].Count; i++)
        {
            ChipBlackJack chip = listChipInTable[indexPlayer][i];
            //SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.GET_CHIP);
            chip.transform.DOLocalMove(new Vector2(0, 300), 1.0f);
            chip.transform.DOScale(new Vector2(0f, 0f), 1.2f);
        }
    }

    public void onClickButtonPlay(int num)
    {
        switch (num)
        {
            case 0:
                Debug.Log("gui evt double......");
                SocketSend.sendBlackjackActionPlay("double");
                break;
            case 1:
                Debug.Log("gui evt split......");
                SocketSend.sendBlackjackActionPlay("split");
                break;
            case 2:
                Debug.Log("gui evt hit......");
                SocketSend.sendBlackjackActionPlay("hit");
                break;
            case 3:
                Debug.Log("gui evt stand......");
                SocketSend.sendBlackjackActionPlay("stand");
                break;
        }
    }

    public void setStatus(Card card1, Card card2)
    {
        Debug.Log("card1 N=" + card1.N);
        Debug.Log("card2 N=" + card2.N);
        if (card1.N == card2.N || (card1.N > 9 && card2.N > 9))
        {
            btn_split.interactable = true;
        }
        else
        {
            btn_split.interactable = false;
        }
    }

    public void prepareToStart(JObject data)
    {
        //{ "evt":"prepareToStart","data":"{"time":6000}"}
        btn_clear.gameObject.SetActive(false);
        btn_deal.gameObject.SetActive(false);
        btn_bet.SetActive(false);
    }

    public void handleDotsWaiting()
    {
        DOTween.Kill("dotWaitTime");
        DOTween.Sequence()
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                SwitchDots(1, 4, 2, 5, 0, 3);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                SwitchDots(0, 3, 2, 5, 1, 4);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                SwitchDots(0, 3, 1, 4, 2, 5);
            })
            .SetLoops(-1)
            .SetId("dotWaitTime");
    }

    public void SwitchDots(int activeIndex1, int inactiveIndex1, int activeIndex2, int inactiveIndex2, int activeIndex3, int inactiveIndex3)
    {
        listDotsWait2[activeIndex1].SetActive(true);
        listDotsWait2[inactiveIndex1].SetActive(false);

        listDotsWait2[activeIndex2].SetActive(true);
        listDotsWait2[inactiveIndex2].SetActive(false);

        listDotsWait2[activeIndex3].SetActive(false);
        listDotsWait2[inactiveIndex3].SetActive(true);

        listDotsWait3[activeIndex1].SetActive(true);
        listDotsWait3[inactiveIndex1].SetActive(false);

        listDotsWait3[activeIndex2].SetActive(true);
        listDotsWait3[inactiveIndex2].SetActive(false);

        listDotsWait3[activeIndex3].SetActive(false);
        listDotsWait3[inactiveIndex3].SetActive(true);
    }


    public void handleIrFinish(JObject data)
    {
        //
    }

    //    evt":"betAccepted",
    //    "data":"{\"pid\":2033368,\"agBet\":100,\"name\":\"RaquelLSCullum\"}"
    public void handleBet(JObject jData)
    {
        if (stateGame == Globals.STATE_GAME.VIEWING)
        {
            btn_bet.SetActive(false);
        }
        else
        {
            btn_bet.SetActive(true);
        }

        JObject data = JObject.Parse((string)jData["data"]);
        int pid = getInt(data, "pid");
        int chipBet = getInt(data, "agBet");
        string name = getString(data, "name");

        Debug.Log("chip bet player....." + chipBet);

        Player plBet = getPlayerWithID(pid);
        int indexPlayer = getDynamicIndex(getIndexOf(plBet));

        if (plBet == thisPlayer)
        {
            stateGame = Globals.STATE_GAME.PLAYING;
        }

        PlayerViewBlackJack plView = (PlayerViewBlackJack)plBet.playerView;
        Vector2 posPl = plView.transform.localPosition;
        plBet.ag -= chipBet;
        plBet.setAg();

        plView.effectFlyMoney(-chipBet, 30);

        if (indexPlayer == 0)
        {
            saveCurChipBC0 += chipBet;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC0);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC0;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC0, true);
            chipMoveTo(chip, plBet, saveCurChipBC0);
        }
        else if (indexPlayer == 1)
        {
            saveCurChipBC1 += chipBet;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC1);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC1;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC1, true);
            chipMoveTo(chip, plBet, saveCurChipBC1);
        }
        else
        {
            saveCurChipBC2 += chipBet;
            chipBetColorInx = listValueChipBets.IndexOf(saveCurChipBC2);
            ChipBlackJack chip = setChip(chipBetColorInx);
            chip.idPl = pid;
            chip.chipValue = saveCurChipBC2;
            chip.transform.localPosition = posPl;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(saveCurChipBC2, true);
            chipMoveTo(chip, plBet, saveCurChipBC2);
        }

        Debug.Log("Length listChipBC0: " + listChipBC0.Count);
        Debug.Log("Length listChipBC1: " + listChipBC1.Count);
        Debug.Log("Length listChipBC2: " + listChipBC2.Count);

        for (int i = 0; i < listChipInTable.Count; i++)
        {
            Debug.Log("count index list=== " + listChipInTable[i].Count);
        }

        setDisplayBet();
    }

    //public void InstantiateTextFly(int chip, PlayerViewBlackJack player)
    //{
    //    if (chip > 0)
    //    {
    //        player.textFly.font = font[1];
    //        player.textFly.text = "+" + Globals.Config.FormatMoney(chip);
    //    }
    //    else
    //    {
    //        player.textFly.font = font[0];
    //        player.textFly.text = Globals.Config.FormatMoney(chip);
    //    }

    //    DOTween.Sequence()
    //    .AppendInterval(0.5f).AppendCallback(() =>
    //    {
    //        player.textFly.transform.DOLocalMove(new Vector2(0, 80), 0.5f);
    //    })
    //    .SetEase(Ease.OutBack)
    //    .AppendInterval(0.5f)
    //    .AppendCallback(() =>
    //    {
    //        player.textFly.text = "";
    //        player.textFly.transform.localPosition = Vector2.zero;
    //    });
    //}

    //public void handleChipLoseForPlayers(JObject data)
    //{
    //    List<JObject> inforUser = data["listUser"].ToObject<List<JObject>>();

    //    inforUser.ForEach((dataUser) =>
    //    {
    //        int idPl = getInt(dataUser, "pid");
    //        int agPl = getInt(dataUser, "ag");
    //        int agAdd = getInt(dataUser, "agadd");
    //        int agLose = getInt(dataUser, "agLose");

    //        Player player1 = getPlayerWithID(idPl);
    //        PlayerViewDragonTiger plView = (PlayerViewDragonTiger)player1.playerView;
    //        if (agLose < 0 && player1.playerView.gameObject.activeSelf)
    //        {
    //            player1.playerView.effectFlyMoney(agLose, 30);
    //        }

    //        player1.ag = agPl;
    //        player1.setAg();
    //    });
    //}

    public ChipBlackJack setChip(int chipIndex)
    {
        ChipBlackJack chip;
        chip = Instantiate<ChipBlackJack>(chipPref, chipContainer.transform);
        chip.chipSprite = chipIndex;
        if (chipIndex == -1)
        {
            chipIndex = 5;
        }
        chip.GetComponent<Image>().sprite = chip.listSpriteChip[chipIndex];
        chip.transform.localScale = new Vector2(0.6f, 0.6f);
        return chip;
    }

    private void chipMoveTo(ChipBlackJack chip, Player player, long valueBet)
    {
        //Debug.Log("move chip >>>>>");
        int indexPlayer = getDynamicIndex(getIndexOf(player));
        Debug.Log("index player: " + indexPlayer);

        GameObject boxCardPlayer = listBoxCard[indexPlayer];
        listBoxCard[indexPlayer].GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(valueBet, true);

        if (indexPlayer == 0)
        {
            listChipBC0.Add(chip);
        }
        else if (indexPlayer == 1)
        {
            listChipBC2.Add(chip);
        }
        else
        {
            listChipBC1.Add(chip);
        }
        listChipInTable.Add(listChipBC0);
        listChipInTable.Add(listChipBC2);
        listChipInTable.Add(listChipBC1);

        Vector2 posBoxCard = boxCardPlayer.transform.localPosition + new Vector3(0, 12, 0);

        chip.transform
            .DOLocalMove(posBoxCard, 0.3f)
            .SetEase(Ease.InSine);
    }

    public void handleActionErr()
    {

    }

    public override void setGameInfo(int m, int id = 0, int maxBet = 0)
    {
        base.setGameInfo(m, id, maxBet);
        setInfoBet(m);
    }

    public void setInfoBet(int m)
    {
        isMaxBet = m * 100;
        isMinBet = m;

        Debug.Log("minBet ===== " + isMinBet);
        Debug.Log("maxBet =====" + isMaxBet);

        string mc = m.ToString();
        if (mc[0].Equals('1'))
        {
            listValueChipBets = new List<long>() { m, m * 5, m * 10, m * 50, m * 100 };
        }
        else
        {
            listValueChipBets = new List<long>() { m, m * 2, m * 10, m * 20, m * 100 };
        }
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(listValueChipBets[i], true);
            //listChipBets[i].interactable = listValueChipBets[i] <= (int)thisPlayer.ag;
        }
        listChipBets[0].transform.Find("border").gameObject.SetActive(true);
        betValue = listValueChipBets[0];
    }

    public void setDisplayBet()
    {
        bool check = false;
        for (int i = 4; i >= 0; i--)
        {
            if (listValueChipBets[i] > curMoney)
            {
                listChipBets[i].interactable = false;
                listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, -310);
                listChipBets[i].transform.Find("border").gameObject.SetActive(false);
                check = true;
            }
            else
            {
                listChipBets[i].interactable = true;
                if (stateGame == Globals.STATE_GAME.PLAYING && check && myChipBetColor >= i)
                {
                    listChipBets[i].transform.Find("border").gameObject.SetActive(true);
                    betValue = listValueChipBets[i];
                    check = false;
                }
            }
        }
    }

    public void onClickChipBet(int chipBet)
    {
        SoundManager.instance.soundClick();
        btn_clear.gameObject.SetActive(true);
        btn_deal.gameObject.SetActive(true);
        betValue = listValueChipBets[chipBet];
        curChipBet += listValueChipBets[chipBet];
        isSaveLastBet += curChipBet;
        isLastBet += listValueChipBets[chipBet];
        curMoney -= listValueChipBets[chipBet];

        btn_clear.interactable = true;
        btn_deal.interactable = true;

        Debug.Log("curChipBet === " + curChipBet);
        Debug.Log("isLastBet === " + isLastBet);

        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.Find("border").gameObject.SetActive(i == chipBet ? true : false);
        }
        chipBetColorInx = chipBet;
        myChipBetColor = chipBet;

        setDisplayBet();
        setlbChip();
    }

    public void onClickDoubleBet()
    {
        Debug.Log("chay vao day .....");
        if (isLastBet > 0 && isLastBet <= agTable * 100)
        {
            curChipBet += isLastBet;
            isSaveLastBet += isLastBet;
            isLastBet *= 2;
        }
        onClickDeal();
        Debug.Log("islastbet ==== " + isLastBet);
        setDisplayBet();
        setlbChip();
    }

    public void onClickRebet()
    {
        btn_rebet.interactable = false;
        if (isSaveLastBet > 0 && isSaveLastBet <= agTable * 100)
        {
            curChipBet += isSaveLastBet;
            isLastBet += isSaveLastBet;
        }
        onClickDeal();
        Debug.Log("islastbet ==== " + isSaveLastBet);
        setDisplayBet();
        setlbChip();
    }

    public void setStatusButtonsBet()
    {
        if (isSaveLastBet > 0)
        {
            btn_rebet.interactable = true;
        }
        else
        {
            btn_rebet.interactable = false;
        }
        btn_doubleBet.interactable = true;
    }

    public void onClickDeal()
    {
        if (curChipBet == 0) return;
        btn_clear.interactable = false;
        btn_deal.interactable = false;
        btn_rebet.interactable = false;
        btn_doubleBet.interactable = true;
        Debug.Log("chipFinishBet la:   " + curChipBet);
        SocketSend.sendBetBlackJack(curChipBet);
        curChipBet = 0;
        resetLabel();
    }

    public void onClickClear()
    {
        btn_clear.interactable = false;
        btn_deal.interactable = false;
        curMoney += curChipBet;
        curChipBet = 0;
        resetLabel();
        setDisplayBet();
    }

    public void setlbChip()
    {
        lbClear.text = Globals.Config.FormatMoney(curChipBet, true);
        lbDeal.text = Globals.Config.FormatMoney(curChipBet, true);
    }

    public void resetLabel()
    {
        Debug.Log("chay vao ham resetlabel");
        lbClear.text = "0";
        lbDeal.text = "0";
    }



}
