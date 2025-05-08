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

public class BaccaratView : GameView
{
    [SerializeField]
    public ChipBaccarat chipPref;

    [SerializeField]
    public GameObject chipContainer;

    [SerializeField]
    public GameObject cardDeckB;

    [SerializeField]
    public List<Card> listCardB = new List<Card>();

    [SerializeField]
    public GameObject cardDeckP;

    [SerializeField]
    public List<Card> listCardP = new List<Card>();

    [SerializeField]
    public GameObject buttonBetBaccarat;

    [SerializeField]
    public List<GameObject> boxBetBaccarat = new List<GameObject>();

    [SerializeField]
    public List<GameObject> listPot = new List<GameObject>();

    [SerializeField]
    public GameObject buttonMenu;

    [SerializeField]
    public GameObject clock;

    [SerializeField]
    public TextMeshProUGUI lbTimeBet;

    [SerializeField]
    public GameObject scorePlayer;

    [SerializeField]
    public TextMeshProUGUI lbScorePlayer;

    [SerializeField]
    public GameObject scoreBanker;

    [SerializeField]
    public TextMeshProUGUI lbScoreBanker;

    [SerializeField]
    public Button btnDoubleBet;

    [SerializeField]
    public Button btnRebet;

    [SerializeField]
    public List<Button> listChipBets = new List<Button>();

    [SerializeField]
    public List<TextMeshProUGUI> listBoxBet = new List<TextMeshProUGUI>();

    [SerializeField]
    public List<TextMeshProUGUI> listBetContainer = new List<TextMeshProUGUI>();

    [SerializeField]
    public SkeletonGraphic ani_win;

    [SerializeField]
    public TextMeshProUGUI lb_waiting;

    [SerializeField]
    public TextMeshProUGUI lb_max_bet;

    [SerializeField]
    public TextMeshProUGUI lb_not_enough_gold;

    [SerializeField]
    public GameObject popupHistoryPrefab;

    [SerializeField]
    public TextMeshProUGUI lb_his_banker;

    [SerializeField]
    public TextMeshProUGUI lb_his_player;

    [SerializeField]
    public TextMeshProUGUI lb_his_tie;

    private HistoryBaccarat popupHistory;

    private JObject saveDT;

    private PlayerViewBaccarat playerViewBaccarat;

    private List<long> listValueChipBets = new List<long>();
    private List<ChipBaccarat> chipPoolTG = new List<ChipBaccarat>();
    private long betValue = 0;
    private int chipBetColorInx = 0;
    private int myChipBetColor = 0;
    private long[] listBet = { 0, 0, 0, 0, 0 };
    private long[] saveListBet = { 0, 0, 0, 0, 0 };
    private long[] saveListMyBet = { 0, 0, 0, 0, 0 };
    private long[] listMyBet = { 0, 0, 0, 0, 0 };
    private long[] listLastBet = { 0, 0, 0, 0, 0 };
    private List<ChipBaccarat> listChipInTable = new List<ChipBaccarat>();
    private List<ChipBaccarat> listChipsPay = new List<ChipBaccarat>();
    private List<int> listCodeCardPlayer = new List<int>();
    private List<int> listCodeCardBanker = new List<int>();
    private List<int> listWinResult = new List<int>();
    private int scorePl = 0, scoreBk = 0, scorePl1 = 0, scoreBk1 = 0;
    private bool checkBeted = false, checkBetDouble = false;
    private List<int> savePotLose = new List<int>();
    [HideInInspector] public List<int> listSaveHistory = new List<int>();

    private void _SetTextTime(int time)
    {
        lbTimeBet.SetText(time.ToString());
        if (time == 3) Config.Vibration();
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

    }

    public int getBetGate(string side)
    {
        switch (side)
        {
            case "Tie":
                return 1;
                break;
            case "Player":
                return 2;
                break;
            case "Banker":
                return 3;
                break;
            case "PlayerPair":
                return 4;
                break;
            case "BankerPair":
                return 5;
                break;
        }
        return 0;
    }

    public string getBetGate2(int side)
    {
        switch (side)
        {
            case 1:
                return "Tie";
                break;
            case 2:
                return "Player";
                break;
            case 3:
                return "Banker";
                break;
            case 4:
                return "PlayerPair";
                break;
            case 5:
                return "BankerPair";
                break;
        }
        return null;
    }

    public void resetGame()
    {
        Array.Copy(listMyBet, listLastBet, 5);
        checkBeted = false;
        setStatusButtonsBet(true, false);

        foreach (ChipBaccarat x in listChipInTable)
        {
            Destroy(x.gameObject);
        }
        listChipInTable.Clear();

        foreach (GameObject x in boxBetBaccarat)
        {
            x.SetActive(false);
        }

        savePotLose.Clear();
        listCodeCardBanker.Clear();
        listCodeCardPlayer.Clear();

        listBet = listBet.Select(x => x * 0).ToArray();
        listMyBet = listMyBet.Select(x => x * 0).ToArray();

        for (int i = 0; i < 5; i++)
        {
            listBoxBet[i].text = "";
            listBetContainer[i].text = "";
        }

        for (int i = 0; i < listPot.Count; i++)
        {
            listPot[i].GetComponent<Button>().interactable = false;
        }

        listWinResult.Clear();
        checkAutoExit();

        lbScoreBanker.text = "";
        lbScorePlayer.text = "";
        ani_win.gameObject.SetActive(false);


        scorePl = 0;
        scorePl1 = 0;
        scoreBk = 0;
        scoreBk1 = 0;

        scoreBanker.SetActive(false);
        scorePlayer.SetActive(false);
        scoreBanker.transform.localPosition = new Vector2(253, 174);
        scorePlayer.transform.localPosition = new Vector2(-253, 174);

        thuBai();
        //Array.Fill(saveListBet, 0);

        HandleGame.nextEvt();
        stateGame = Globals.STATE_GAME.WAITING;
    }

    public void setLbWaitingNextgame()
    {
        //Harap menunggu permainan selanjutnya
        lb_waiting.gameObject.SetActive(true);
        string text1 = lb_waiting.text + ".";
        string text2 = lb_waiting.text + "..";
        string text3 = lb_waiting.text + "...";

        Sequence textSequence = DOTween.Sequence()
                                    .AppendInterval(1.0f)
                                    .AppendCallback(() => lb_waiting.text = text1)
                                    .AppendInterval(1.0f)
                                    .AppendCallback(() => lb_waiting.text = text2)
                                    .AppendInterval(1.0f)
                                    .AppendCallback(() => lb_waiting.text = text3);
        textSequence.SetLoops(-1);
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

    public void handleFinishGame(JObject jData)
    {
        HandleData.DelayHandleLeave = 7f;
        clock.SetActive(false);
        buttonBetBaccarat.SetActive(false);
        for (int i = 0; i < listPot.Count; i++)
        {
            listPot[i].GetComponent<Button>().interactable = false;
        }
        setStatusButtonsBet(!checkBeted, checkBeted);

        Vector2 pos1 = new Vector2(-101, 171);
        Vector2 pos2 = new Vector2(101, 171);
        Vector2 pos3 = new Vector2(-172, 171);
        Vector2 pos4 = new Vector2(172, 171);
        Vector2 pos5 = new Vector2(-254, 171);
        Vector2 pos6 = new Vector2(254, 171);
        Vector2 posThuBai = new Vector2(-296, 301);

        JObject dataFinish = JObject.Parse((string)jData["data"]);
        saveDT = dataFinish;
        var playerCards = (JArray)dataFinish["playerCards"];
        var bankerCards = (JArray)dataFinish["bankerCards"];

        for (int i = 0; i < playerCards.Count; i++)
        {
            if (playerCards[i].GetType() != typeof(JObject)) continue;
            JObject dataCard = (JObject)playerCards[i];
            string value = getString(dataCard, "value");
            string type = getString(dataCard, "type");
            int code = getInt(dataCard, "code");
            scorePl1 += setValueCard(value);
            if (i < playerCards.Count - 1)
            {
                scorePl += setValueCard(value);
            }
            listCodeCardPlayer.Add(code);
        }

        for (int i = 0; i < bankerCards.Count; i++)
        {
            if (bankerCards[i].GetType() != typeof(JObject)) continue;
            JObject dataCard = (JObject)bankerCards[i];
            string value = getString(dataCard, "value");
            string type = getString(dataCard, "type");
            int code = getInt(dataCard, "code");
            scoreBk1 += setValueCard(value);
            if (i < bankerCards.Count - 1)
            {
                scoreBk += setValueCard(value);
            }
            listCodeCardBanker.Add(code);
        }

        scorePl = setScoreCard(scorePl);
        scoreBk = setScoreCard(scoreBk);

        scorePl1 = setScoreCard(scorePl1);
        scoreBk1 = setScoreCard(scoreBk1);

        lbScorePlayer.text = scorePl.ToString();
        lbScoreBanker.text = scoreBk.ToString();

        effChiaBai(listCardP[0], listCodeCardPlayer[0], pos1, true, 0.1f);
        effChiaBai(listCardB[0], listCodeCardBanker[0], pos2, true, 0.3f);
        effChiaBai(listCardP[1], listCodeCardPlayer[1], pos3, true, 0.5f);
        effChiaBai(listCardB[1], listCodeCardBanker[1], pos4, true, 0.7f);

        DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
        {
            scorePlayer.SetActive(true);
            scoreBanker.SetActive(true);
        });

        DOTween.Sequence()
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {

                if (listCodeCardPlayer.Count == 3)
                {
                    DOTween.Sequence()
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            scorePlayer.SetActive(false);
                            effChiaBai(listCardP[2], listCodeCardPlayer[2], pos5, false, 0.8f);
                        })
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            scorePlayer.transform.localPosition = new Vector2(-338, 174);
                            lbScorePlayer.text = scorePl1.ToString();
                            scorePlayer.SetActive(true);
                        });
                }

                if (listCodeCardBanker.Count == 3)
                {
                    DOTween.Sequence()
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            scoreBanker.SetActive(false);
                            effChiaBai(listCardB[2], listCodeCardBanker[2], pos6, false, 1.0f);
                        })
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            scoreBanker.transform.localPosition = new Vector2(338, 174);
                            lbScoreBanker.text = scoreBk1.ToString();
                            scoreBanker.SetActive(true);
                        });
                }
            });

        listSaveHistory = getListInt(dataFinish, "history");

        if (listCodeCardBanker.Count == 3 || listCodeCardPlayer.Count == 3)
        {
            DOTween.Sequence()
                .AppendInterval(4.0f)
                .AppendCallback(() =>
                {
                    handleAniCards();
                    handleWinPots(dataFinish);
                });
        }
        else
        {
            DOTween.Sequence()
                .AppendInterval(2.0f)
                .AppendCallback(() =>
                {
                    handleAniCards();
                    handleWinPots(dataFinish);
                });
        }

        // HandleGame.nextEvt();

    }

    public void effChiaBai(Card card, int code, Vector2 pos, bool axis, float time)
    {
        DOTween.Sequence()
            .AppendInterval(time)
            .AppendCallback(() =>
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.DISPATCH_CARD);
                card.setTextureWithCode(0);
                card.gameObject.transform.localScale = new Vector2(0.38f, 0.4f);
                card.gameObject.transform.localPosition = new Vector2(296, 301);
                card.gameObject.transform.localEulerAngles = new Vector3(0, 0, 64.48f);

                card.gameObject.transform.DOLocalMove(pos, 0.5f);
                card.gameObject.transform.DOLocalRotate(new Vector3(0, 0, axis ? 0 : 90), 0.5f);
                card.gameObject.transform.SetAsLastSibling();
                card.gameObject.SetActive(true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                card.gameObject.transform.DOScale(new Vector2(0f, 0.4f), 0.1f)
                .OnComplete(() =>
                {
                    card.setTextureWithCode(code);
                    card.gameObject.transform.DOScale(new Vector2(0.38f, 0.4f), 0.1f);
                });


            });
    }

    public int setValueCard(string value)
    {
        if (value == "A")
        {
            return 1;
        }
        else if (value == "10" || value == "J" || value == "Q" || value == "K")
            return 0;
        return int.Parse(value);
    }

    public int setScoreCard(int sum)
    {
        if (sum >= 10)
        {
            if (sum % 10 == 0)
                return 0;
            else
                return sum % 10;
        }
        return sum;
    }

    public void setPosLbWinFinish(JObject data)
    {
        bool tie = getBool(data, "tie");//1
        bool player = getBool(data, "player");//2
        bool banker = getBool(data, "banker");//3

        if (tie == true)
        {

            ani_win.gameObject.SetActive(true);
            ani_win.Initialize(true);
            ani_win.AnimationState.SetAnimation(0, "tie", false);
        }

        if (player == true)
        {
            ani_win.gameObject.SetActive(true);
            ani_win.Initialize(true);
            ani_win.AnimationState.SetAnimation(0, "player", false);
        }

        if (banker == true)
        {
            ani_win.gameObject.SetActive(true);
            ani_win.Initialize(true);
            ani_win.AnimationState.SetAnimation(0, "banker", false);
        }

    }

    public void handleWinPots(JObject data)
    {
        Debug.Log("chay vao day  cmmmmm.......");

        bool tie = getBool(data, "tie");//1
        bool player = getBool(data, "player");//2
        bool banker = getBool(data, "banker");//3
        bool playerPair = getBool(data, "playerPair");//4
        bool bankerPair = getBool(data, "bankerPair");//5

        setPosLbWinFinish(data);

        int bankerWinCount = getInt(data, "bankerWinCount");
        int playerWinCount = getInt(data, "playerWinCount");
        int tieWinCount = getInt(data, "tieWinCount");
        int bankerPairCount = getInt(data, "bankerPairCount");
        int playerPairCount = getInt(data, "playerPairCount");

        setInfoCountHis(bankerWinCount, playerWinCount, tieWinCount);

        //handleResultHis(data);

        if (tie == true)
        {
            listWinResult.Add(1);
        }

        if (player == true)
        {
            listWinResult.Add(2);
        }

        if (banker == true)
        {
            listWinResult.Add(3);
        }

        if (playerPair == true)
        {
            listWinResult.Add(4);
        }

        if (bankerPair == true)
        {
            listWinResult.Add(5);
        }

        //Debug.Log("size listwinresult====" + listWinResult.Count);

        listWinResult.ForEach(x =>
        {
            effectWinGate(x);
        });

        DOTween.Sequence()
            .AppendInterval(3.0f)
            .AppendCallback(() =>
            {
                getChipLose();
                ani_win.gameObject.SetActive(false);
                handleChipLoseForPlayers(data);
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                payChipWin();
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                handleChipWinForPlayers(data);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                foreach (Card card in listCardP)
                {
                    card.transform.Find("blink").gameObject.SetActive(false);
                    card.transform.Find("mask").gameObject.SetActive(false);
                }

                foreach (Card card in listCardB)
                {
                    card.transform.Find("blink").gameObject.SetActive(false);
                    card.transform.Find("mask").gameObject.SetActive(false);
                }
            })
            .AppendInterval(1.5f)
            .AppendCallback(() =>
            {
                resetGame();
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                setLbWaitingNextgame();

            });
    }

    public void thuBai()
    {
        foreach (Card card in listCardP)
        {
            int index = listCardP.IndexOf(card);
            DOTween.Sequence()
                    .AppendInterval(index * 0.3f)
                    .AppendCallback(() =>
                    {
                        card.setTextureWithCode(0);
                        card.gameObject.transform.DOLocalMove(new Vector2(-296, 301), 0.5f);
                        card.gameObject.transform.DOLocalRotate(new Vector3(0, 0, -64.48f), 0.5f);
                        card.gameObject.transform.DOScale(new Vector2(0.38f, 0.4f), 0.5f);
                    })
                    .AppendInterval(0.6f)
                    .AppendCallback(() =>
                    {
                        card.gameObject.SetActive(false);
                    });
        }

        foreach (Card card in listCardB)
        {
            int index = listCardB.IndexOf(card);
            DOTween.Sequence()
                    .AppendInterval(index * 0.3f)
                    .AppendCallback(() =>
                    {
                        card.setTextureWithCode(0);
                        card.gameObject.transform.DOLocalMove(new Vector2(-296, 301), 0.5f);
                        card.gameObject.transform.DOLocalRotate(new Vector3(0, 0, -64.48f), 0.5f);
                        card.gameObject.transform.DOScale(new Vector2(0.38f, 0.4f), 0.5f);
                    })
                    .AppendInterval(0.6f)
                    .AppendCallback(() =>
                    {
                        card.gameObject.SetActive(false);
                    });
        }

        scorePlayer.SetActive(false);
        scoreBanker.SetActive(false);
    }

    public void handleAniCards()
    {
        if (scorePl1 > scoreBk1)
        {
            foreach (Card card in listCardP)
            {
                card.anim_border.gameObject.SetActive(true);
                card.anim_border.Initialize(true);
                card.anim_border.AnimationState.SetAnimation(0, "animation", true);
            }

            foreach (Card card in listCardB)
            {
                card.transform.Find("mask").gameObject.SetActive(true);
            }
        }
        else if (scorePl1 < scoreBk1)
        {
            foreach (Card card in listCardB)
            {
                card.anim_border.gameObject.SetActive(true);
                card.anim_border.Initialize(true);
                card.anim_border.AnimationState.SetAnimation(0, "animation", true);
            }

            foreach (Card card in listCardP)
            {
                card.transform.Find("mask").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Card card in listCardP)
            {
                card.anim_border.gameObject.SetActive(true);
                card.anim_border.Initialize(true);
                card.anim_border.AnimationState.SetAnimation(0, "animation", true);
            }

            foreach (Card card in listCardB)
            {
                card.anim_border.gameObject.SetActive(true);
                card.anim_border.Initialize(true);
                card.anim_border.AnimationState.SetAnimation(0, "animation", true);
            }
        }
    }

    public void handleChipWinForPlayers(JObject data)
    {
        List<JObject> inforUser = data["results"].ToObject<List<JObject>>();

        inforUser.ForEach((dataUser) =>
        {
            int idPl = getInt(dataUser, "pid");
            int agPl = getInt(dataUser, "AG");
            int agAdd = getInt(dataUser, "agAdd");
            int agWin = getInt(dataUser, "agWin");

            Player player1 = getPlayerWithID(idPl);
            PlayerViewBaccarat plView = (PlayerViewBaccarat)player1.playerView;

            if (agWin > 0)
            {
                player1.playerView.effectFlyMoney(agWin, 40);
            }

            player1.ag = agPl;
            player1.setAg();
        });
    }

    public void handleChipLoseForPlayers(JObject data)
    {
        bool tie = getBool(data, "tie");
        bool player = getBool(data, "player");
        bool banker = getBool(data, "banker");
        bool playerPair = getBool(data, "playerPair");
        bool bankerPair = getBool(data, "bankerPair");

        List<JObject> inforBet = data["bets"].ToObject<List<JObject>>();

        List<int> listGateBet = new List<int>();

        inforBet.ForEach((dataUser) =>
        {
            int idPl = getInt(dataUser, "pid");
            JObject bankerSide = (JObject)dataUser["bankerSide"];
            JObject playerSide = (JObject)dataUser["playerSide"];
            JObject tieSide = (JObject)dataUser["tieSide"];
            JObject playerPairSide = (JObject)dataUser["playerPairSide"];
            JObject bankerPairSide = (JObject)dataUser["bankerPairSide"];

            List<JObject> inforBetSide = new List<JObject> { bankerSide, playerSide, tieSide, playerPairSide, bankerPairSide };

            for (int i = 0; i < listChipInTable.Count; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    Player objPlayer = players[j];
                    ChipBaccarat chip = listChipInTable[i];
                    Player player1 = getPlayerWithID(idPl);

                    if (!listWinResult.Contains(chip.gateId) && chip.idPl == idPl)
                    {

                        if (chip.idPl != thisPlayer.id)
                        {
                            player1.playerView.agLose -= chip.chipValue;
                        }
                        else
                        {
                            if (!listGateBet.Contains(chip.gateId))
                            {
                                listGateBet.Add(chip.gateId);
                            }
                        }
                    }
                    player1.playerView.effectFlyMoney(player1.playerView.agLose, 40);
                    player1.playerView.agLose = 0;
                }
            }
        });


        for (int i = 0; i < listGateBet.Count; i++)
        {
            Debug.Log("gate bet lose ====" + listGateBet[i]);
            thisPlayer.playerView.agLose -= listMyBet[listGateBet[i] - 1];
        }

        thisPlayer.playerView.effectFlyMoney(thisPlayer.playerView.agLose, 40);
        Debug.Log("aglose thisplayer ===" + thisPlayer.playerView.agLose);
    }

    public void effectWinGate(int index)
    {
        int resultWin = index - 1;

        for (int i = 0; i < listPot.Count; i++)
        {
            if (i == resultWin)
            {
                Button objButton = listPot[i].GetComponent<Button>();
                Sprite spr = objButton.spriteState.pressedSprite;
                GameObject btnPress = objButton.transform.Find("press").gameObject;
                if (btnPress != null)
                {
                    Image btnImgPress = btnPress.GetComponent<Image>();
                    btnImgPress.sprite = spr;
                    btnPress.gameObject.SetActive(true);
                    btnImgPress.enabled = true;
                    Color normalColor = btnImgPress.color;
                    Color noOpacity = new Color(1, 1, 1, 0);
                    DOTween.Sequence()
                        .Append(btnImgPress.DOColor(noOpacity, 0.2f))
                        .Append(btnImgPress.DOColor(normalColor, 0.2f))
                        .SetLoops(5)
                        .OnComplete(() =>
                        {
                            btnPress.gameObject.SetActive(false);
                            btnImgPress.enabled = false;
                        });
                }
            }
        }
    }

    private void getChipLose()
    {

        for (int i = 0; i < listChipInTable.Count; i++)
        {
            ChipBaccarat chip = listChipInTable[i];
            if (!listWinResult.Contains(chip.gateId))
            {

                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.GET_CHIP);

                chip.transform.DOLocalMove(new Vector2(0, 300), 0.5f);
                chip.transform.DOScale(new Vector2(0f, 0f), 0.5f);
                //listChipInTable.Remove(chip);
            }
        }
    }

    public void handlelc(JObject jData)
    {

    }

    public void handleBet(JObject jData)
    {

        JObject data = JObject.Parse((string)jData["data"]);
        setStatusButtonsBet(!checkBeted, checkBeted);
        int pId = getInt(data, "pid");
        string side = getString(data, "side");
        int betGate = getBetGate(side);
        Debug.Log("betGate====" + betGate);
        long chipBet = getLong(data, "chipBet");
        listBet[betGate - 1] += chipBet;

        if (pId == thisPlayer.id)
        {
            listMyBet[betGate - 1] += chipBet;
        }

        Player plBet = getPlayerWithID(pId);
        if (plBet == thisPlayer)
        {
            stateGame = Globals.STATE_GAME.PLAYING;
        }

        PlayerViewBaccarat plView = (PlayerViewBaccarat)plBet.playerView;
        Vector2 posPl = plView.transform.localPosition;
        plBet.ag -= chipBet;
        plBet.setAg();

        chipBetColorInx = listValueChipBets.IndexOf(chipBet);
        ChipBaccarat chip = getChip(chipBetColorInx);
        chip.idPl = pId;
        chip.gateId = betGate;
        chip.chipValue = chipBet;
        chip.transform.localPosition = posPl;
        chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
        chipMoveTo(chip, betGate);
        listChipInTable.Add(chip);
        buttonBetBaccarat.SetActive(true);
        setDisplayBet();
        updateStatePot();

        for (int i = 0; i < 5; i++)
        {
            Debug.Log("value list my bet =====" + listMyBet[i]);
        }
    }
    public void handleStartGame(JObject jData)
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
        clock.SetActive(true);
        buttonBetBaccarat.SetActive(true);
        lb_waiting.gameObject.SetActive(false);
        if (popupHistory != null)
        {
            popupHistory.onClickClose(true);
        }
        //if (popupHistory != null)
        //{
        //    popupHistory.tableHisDetailPopOff();
        //}
        setDisplayBet();
        setStatusButtonsBet(!checkBeted, checkBeted);
        for (int i = 0; i < listPot.Count; i++)
        {
            listPot[i].GetComponent<Button>().interactable = true;
        }
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse((string)jData["data"]);
        int timeStart = (int)(getInt(data, "finishAfter") / 1000);
        timeStart -= 1;
        _SetTextTime(timeStart);
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            timeStart--;
            if (timeStart > 0)
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
            }
            _SetTextTime(timeStart);
        }).SetLoops(timeStart).SetId("SoundClock");

        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerView.setTurn(true, getInt(data, "finishAfter") / 1000 - 1);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.Kill("SoundClock");
    }


    public override void handleSTable(string strData)
    {
        base.handleSTable(strData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(strData);
        saveDT = data;
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
        int timeLeft = (int)Mathf.Round(getInt(data, "timeLeft") / 1000);
        string gamestatus = getString(data, "gameStatus");

        int bankerWinCount = getInt(data, "bankerWinCount");
        int playerWinCount = getInt(data, "playerWinCount");
        int tieWinCount = getInt(data, "tieWinCount");
        int bankerPairCount = getInt(data, "bankerPairCount");
        int playerPairCount = getInt(data, "playerPairCount");

        setInfoCountHis(bankerWinCount, playerWinCount, tieWinCount);

        if (timeLeft > -1 && gamestatus == "BET_TIME")
        {
            setStatusButtonsBet(!checkBeted, checkBeted);
            setDisplayBet();
            clock.SetActive(true);
            buttonBetBaccarat.SetActive(true);

            for (int i = 0; i < listPot.Count; i++)
            {
                listPot[i].GetComponent<Button>().interactable = true;
            }
            _SetTextTime(timeLeft);
            //timeLeft -= 1;

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeLeft--;
                if (timeLeft > 0)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
                }
                _SetTextTime(timeLeft);
            }).SetLoops(timeLeft).SetId("SoundClock");

            JArray playerBet = getJArray(data, "playerBet");
            for (int i = 0; i < playerBet.Count; i++)
            {
                JObject dataPL = (JObject)playerBet[i];
                int pId = getInt(dataPL, "pid");
                int tie = getInt(dataPL, "tie");//1
                int player = getInt(dataPL, "player");//2
                int banker = getInt(dataPL, "banker");//3
                int playerPair = getInt(dataPL, "playerPair");//4
                int bankerPair = getInt(dataPL, "bankerPair");//5
                listBet[0] += tie;
                listBet[1] += player;
                listBet[2] += banker;
                listBet[3] += playerPair;
                listBet[4] += bankerPair;

                saveListBet[0] += listBet[0];
                saveListBet[1] += listBet[1];
                saveListBet[2] += listBet[2];
                saveListBet[3] += listBet[3];
                saveListBet[4] += listBet[4];

                for (int j = 0; j < listBet.Length; j++)
                {
                    if (listBet[j] > 0)
                    {
                        chipBetColorInx = listValueChipBets.IndexOf(listBet[j]);
                        ChipBaccarat chip = getChip(chipBetColorInx);
                        chip.gateId = j + 1;
                        chip.idPl = pId;
                        chip.chipValue = listBet[j];
                        chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                        chipMoveTo(chip, j + 1);
                        listChipInTable.Add(chip);
                    }
                }
                Array.Fill(listBet, 0);
            }
        }
        else
        {
            setLbWaitingNextgame();
            clock.SetActive(false);
            buttonBetBaccarat.SetActive(false);
            for (int i = 0; i < listPot.Count; i++)
            {
                listPot[i].GetComponent<Button>().interactable = false;
            }
        }
        listSaveHistory = getListInt(data, "history");
        for (int i = 0; i < 5; i++)
        {
            //Debug.Log("value lisst save bet ...... ===== " + saveListBet[i]);
            listBetContainer[i].text = saveListBet[i] > 0 ? Globals.Config.FormatMoney(saveListBet[i], true) : "";
            listBet[i] = saveListBet[i];
        }
    }

    public override void handleRJTable(string strData)
    {
        base.handleRJTable(strData);
        JObject data = JObject.Parse(strData);
        saveDT = data;
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
        checkBeted = true;
        int timeRemain = (int)Mathf.Round(getInt(data, "timeLeft") / 1000);

        string gamestatus = getString(data, "gameStatus");

        int bankerWinCount = getInt(data, "bankerWinCount");
        int playerWinCount = getInt(data, "playerWinCount");
        int tieWinCount = getInt(data, "tieWinCount");
        int bankerPairCount = getInt(data, "bankerPairCount");
        int playerPairCount = getInt(data, "playerPairCount");

        setInfoCountHis(bankerWinCount, playerWinCount, tieWinCount);

        if (timeRemain > -1 && gamestatus == "BET_TIME")
        {
            stateGame = Globals.STATE_GAME.PLAYING;

            setStatusButtonsBet(!checkBeted, checkBeted);
            clock.SetActive(true);
            setDisplayBet();
            buttonBetBaccarat.SetActive(true);

            for (int i = 0; i < listPot.Count; i++)
            {
                listPot[i].GetComponent<Button>().interactable = true;
            }
            _SetTextTime(timeRemain);

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeRemain--;
                if (timeRemain > 0)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
                }
                _SetTextTime(timeRemain);
            }).SetLoops(timeRemain).SetId("SoundClock");

            JArray ArrP = getJArray(data, "ArrP");
            JArray playerBet = getJArray(data, "playerBet");

            for (int i = 0; i < ArrP.Count; i++)
            {
                JObject dataPl = (JObject)ArrP[i];
                int pid = getInt(dataPl, "id");
                Player plView = getPlayerWithID(pid);
                string name = getString(dataPl, "N");
                long agPl = getInt(dataPl, "AG");

                for (int j = 0; j < playerBet.Count; j++)
                {
                    JObject dataPL = (JObject)playerBet[j];
                    int pId = getInt(dataPL, "pid");

                    if (pId == pid)
                    {
                        int tie = getInt(dataPL, "tie");//1
                        int player = getInt(dataPL, "player");//2
                        int banker = getInt(dataPL, "banker");//3
                        int playerPair = getInt(dataPL, "playerPair");//4
                        int bankerPair = getInt(dataPL, "bankerPair");//5

                        listBet[0] += tie;
                        listBet[1] += player;
                        listBet[2] += banker;
                        listBet[3] += playerPair;
                        listBet[4] += bankerPair;

                        saveListBet[0] += listBet[0];
                        saveListBet[1] += listBet[1];
                        saveListBet[2] += listBet[2];
                        saveListBet[3] += listBet[3];
                        saveListBet[4] += listBet[4];

                        if (pId == thisPlayer.id)
                        {
                            saveListMyBet[0] += tie;
                            saveListMyBet[1] += player;
                            saveListMyBet[2] += banker;
                            saveListMyBet[3] += playerPair;
                            saveListMyBet[4] += bankerPair;
                        }

                        plView.ag = agPl;
                        plView.setAg();
                    }

                    for (int k = 0; k < listBet.Length; k++)
                    {
                        if (listBet[k] > 0)
                        {
                            chipBetColorInx = listValueChipBets.IndexOf(listBet[k]);
                            Debug.Log("chipBetColorInd=====" + chipBetColorInx);

                            ChipBaccarat chip = getChip(chipBetColorInx);
                            chip.gateId = k + 1;
                            chip.idPl = pId;
                            chip.chipValue = listBet[k];
                            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                            chipMoveTo(chip, k + 1);
                            listChipInTable.Add(chip);
                        }
                    }
                    Array.Fill(listBet, 0);
                }
            }
        }
        else
        {
            setLbWaitingNextgame();
            clock.SetActive(false);
            buttonBetBaccarat.SetActive(false);
            for (int i = 0; i < listPot.Count; i++)
            {
                listPot[i].GetComponent<Button>().interactable = false;
            }
        }
        listSaveHistory = getListInt(data, "history");
        for (int i = 0; i < 5; i++)
        {
            listBoxBet[i].text = saveListMyBet[i] > 0 ? Globals.Config.FormatMoney(saveListMyBet[i], true) : "";
            listBetContainer[i].text = saveListBet[i] > 0 ? Globals.Config.FormatMoney(saveListBet[i], true) : "";
            boxBetBaccarat[i].gameObject.SetActive(true ? saveListMyBet[i] > 0 : false);

            listMyBet[i] += saveListMyBet[i];
            listBet[i] += saveListBet[i];

            Debug.Log("value list my bet ===" + listMyBet[i]);
            Debug.Log("value save list my bet ====" + saveListMyBet[i]);
            //Debug.Log("value save list my bet ====" + saveListMyBet[i]);

        }
    }

    public override void handleCTable(string strData)
    {
        base.handleCTable(strData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(strData);
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
    }
    public override void handleJTable(string objData)
    {
        base.handleJTable(objData);
    }

    public override void handleLTable(JObject objData)
    {
        base.handleLTable(objData);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.REMOVE);
    }

    public void handleBetError(JObject jData)
    {
        JObject data = JObject.Parse((string)jData["data"]);
        DOTween.Sequence()
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                btnDoubleBet.interactable = false;
            });

        String actionResult = getString(data, "actionResult");
        if (actionResult == "MAX_BET")
        {
            for (int i = 0; i < listPot.Count; i++)
            {
                listPot[i].GetComponent<Button>().interactable = false;
            }
            lb_max_bet.gameObject.SetActive(true);
            DOTween.Sequence()
                .AppendInterval(1.0f)
                .AppendCallback(() =>
                {
                    lb_max_bet.gameObject.SetActive(false);
                });
        }
        else
        {
            if (actionResult == "NOT_ENOUGH_AG")
            {
                lb_not_enough_gold.gameObject.SetActive(true);
                DOTween.Sequence()
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        lb_not_enough_gold.gameObject.SetActive(false);
                    });
            }
            else return;
        }
    }

    public override void setGameInfo(int m, int id = 0, int maxBett = 0)
    {
        base.setGameInfo(m, id, maxBett);
        setInfoBet(m);
    }

    public void setInfoBet(int m)
    {
        listValueChipBets = new List<long> { m, m * 5, m * 10, m * 50, m * 100 };
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(listValueChipBets[i], true);
        }
        listChipBets[0].transform.Find("border").gameObject.SetActive(true);
        betValue = listValueChipBets[0];
    }

    public void setDisplayBet()
    {
        //long betValid = 0;
        bool check = false;
        for (int i = 4; i >= 0; i--)
        {
            if (listValueChipBets[i] > thisPlayer.ag)
            {
                listChipBets[i].interactable = false;
                listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, -321);
                listChipBets[i].transform.Find("border").gameObject.SetActive(false);
                check = true;
            }
            else
            {
                // break
                listChipBets[i].interactable = true;
                if (stateGame == Globals.STATE_GAME.PLAYING && check && myChipBetColor >= i)
                {
                    listChipBets[i].transform.Find("border").gameObject.SetActive(true);
                    betValue = listValueChipBets[i];
                    check = false;
                }
                //betValid = listValueChipBets[i];
            }
        }
    }

    public void updateStatePot()
    {
        for (int i = 0; i < 5; i++)
        {
            listBoxBet[i].text = listMyBet[i] > 0 ? Globals.Config.FormatMoney(listMyBet[i], true) : "";
            listBetContainer[i].text = listBet[i] > 0 ? Globals.Config.FormatMoney(listBet[i], true) : "";
            boxBetBaccarat[i].gameObject.SetActive(true ? listMyBet[i] > 0 : false);
        }
    }

    public void payChipWin()
    {

        Vector2 posModel = new Vector2(0, 300);
        for (int i = 0; i < listChipInTable.Count; i++)
        {
            var chip = listChipInTable[i];
            if (listWinResult.Contains(chip.gateId))
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);

                ChipBaccarat chip1 = getChip(chip.chipSprite);
                chip1.transform.localPosition = posModel;
                Vector2 posChip = chip.transform.localPosition;
                posChip.x += 5f;
                posChip.y += 5f;
                chip1.transform.DOLocalMove(posChip, 0.5f);
                chip1.idPl = chip.idPl;
                chip1.gateId = chip.gateId;
                chip1.chipValue = chip.chipValue;
                chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                listChipsPay.Add(chip1);
            }
        }
        listChipInTable.AddRange(listChipsPay);
        listChipsPay.Clear();
        DOTween.Sequence()
            .AppendInterval(0.7f)
            .AppendCallback(() =>
            {
                for (int i = 0; i < listChipInTable.Count; i++)
                {
                    var chip = listChipInTable[i];
                    if (listWinResult.Contains(chip.gateId))
                    {
                        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.WIN);
                        // get player bet win position
                        Player playerBet = getPlayerWithID(chip.idPl);
                        PlayerViewBaccarat plView = (PlayerViewBaccarat)playerBet.playerView;
                        Vector2 posPlayer = plView.transform.localPosition;
                        chip.transform.DOLocalMove(posPlayer, 0.5f);
                    }
                }
            });
    }

    public void onClickChipBet(int chipBet)
    {
        SoundManager.instance.soundClick();

        betValue = listValueChipBets[chipBet];
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.Find("border").gameObject.SetActive(i == chipBet ? true : false);
        }
        chipBetColorInx = chipBet;
        myChipBetColor = chipBet;
    }

    public void onClickBet(string sideBet)
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.BET);

        if (betValue == 0)
        {
            return;
        }
        SocketSend.sendBetBaccarat(betValue, sideBet);
        checkBeted = true;
        int gateBet = getBetGate(sideBet);

        listPot[gateBet - 1].GetComponent<Button>().interactable = false;

        DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    listPot[gateBet - 1].GetComponent<Button>().interactable = true;
                });
    }

    public void onClickDoubleBet()
    {
        SoundManager.instance.soundClick();
        int timeleft = int.Parse(lbTimeBet.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (listMyBet[i] > 0 && listMyBet[i] <= agTable * 100)
                {
                    checkBetDouble = true;
                    SocketSend.sendBetBaccarat(listMyBet[i], getBetGate2(i + 1));
                }
            }
        }
    }

    public void onClickRebet()
    {
        SoundManager.instance.soundClick();

        int timeleft = int.Parse(lbTimeBet.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (listLastBet[i] > 0)
                {
                    checkBeted = true;
                    SocketSend.sendBetBaccarat(listLastBet[i], getBetGate2(i + 1));
                }
            }
        }
    }

    public void setStatusButtonsBet(bool btnrebet, bool btndouble)
    {
        if (!listLastBet.Any(element => element != 0))
        {
            btnrebet = false;
        }
        Debug.Log("setStatusButtonsBet:" + btndouble);
        btnRebet.interactable = btnrebet;
        btnDoubleBet.interactable = btndouble;
    }

    private ChipBaccarat getChip(int chipIndex)
    {
        ChipBaccarat chip;
        if (chipPoolTG.Count > 0)
        {
            chip = chipPoolTG[0];
            chipPoolTG.Remove(chip);
            chip.transform.parent = chipContainer.transform;
            chip.gameObject.SetActive(true);
        }
        else
        {
            chip = Instantiate<ChipBaccarat>(chipPref, chipContainer.transform);
        }
        chip.chipSprite = chipIndex;
        if (chipIndex == -1)
        {
            chipIndex = 5;
        }
        chip.GetComponent<Image>().sprite = chip.listSpriteChip[chipIndex];
        chip.transform.localScale = new Vector2(0.5f, 0.5f);
        return chip;
    }

    public void chipMoveTo(ChipBaccarat chip, int betGate)
    {
        Vector2 posPot = Globals.Config.getPosInOtherNode(listPot[betGate - 1].transform.position, chipContainer);
        chip.transform
            .DOLocalMove(posPot, 0.3f)
            .SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                Vector2 randomPosition = new Vector2(
                    posPot.x + UnityEngine.Random.Range(-30, 30),
                    posPot.y + UnityEngine.Random.Range(-8, 8)
                );

                chip.transform.DOLocalJump(randomPosition, 20, 1, 0.2f);
            });
    }

    public void OnClickShowPopupHistory()
    {
        if (listSaveHistory.Count > 0)
        {
            popupHistory = Instantiate(popupHistoryPrefab, transform).GetComponent<HistoryBaccarat>();
            popupHistory.transform.SetAsLastSibling();

            popupHistory.handleResultHisLayer1(listSaveHistory);
            popupHistory.handleResultHisLayer2(listSaveHistory);
            popupHistory.handleResultBigEyes(listSaveHistory);
            handleResultHis(saveDT);
        }

    }

    public void setInfoCountHis(int cntB, int cntP, int cntT)
    {
        lb_his_banker.text = cntB.ToString();
        lb_his_player.text = cntP.ToString();
        lb_his_tie.text = cntT.ToString();
    }

    public void handleResultHis(JObject data)
    {
        int bankerWinCount = getInt(data, "bankerWinCount");
        int playerWinCount = getInt(data, "playerWinCount");
        int tieWinCount = getInt(data, "tieWinCount");
        int bankerPairCount = getInt(data, "bankerPairCount");
        int playerPairCount = getInt(data, "playerPairCount");

        popupHistory.numWinB += bankerWinCount;
        popupHistory.numWinP += playerWinCount;
        popupHistory.numWinT += tieWinCount;
        popupHistory.numWinBP += bankerPairCount;
        popupHistory.numWinPP += playerPairCount;

        popupHistory.lb_his_player_detail.text = popupHistory.numWinP.ToString();
        popupHistory.lb_his_banker_detail.text = popupHistory.numWinB.ToString();
        popupHistory.lb_his_tie_detail.text = popupHistory.numWinT.ToString();
        popupHistory.lb_his_playerPair.text = popupHistory.numWinPP.ToString();
        popupHistory.lb_his_bankerPair.text = popupHistory.numWinBP.ToString();

    }
}
