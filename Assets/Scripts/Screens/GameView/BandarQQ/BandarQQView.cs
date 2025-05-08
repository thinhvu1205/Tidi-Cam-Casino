using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Device;
using Screen = UnityEngine.Device.Screen;
using Globals;

public class BandarQQView : GameView

{
    [SerializeField] GameObject PlayerContainer;
    [SerializeField] ChipBandar chipPref;
    [SerializeField] GameObject ChipContainer;
    [SerializeField] GameObject CardContainer;
    [SerializeField] GameObject btnPots;
    [SerializeField] GameObject btnBets;
    [SerializeField] List<Button> listChipBets = new List<Button>();
    [SerializeField] List<Sprite> listXS = new List<Sprite>();
    [SerializeField] List<GameObject> listBoxResult = new List<GameObject>();
    [SerializeField] SkeletonGraphic animStart;
    [SerializeField] SkeletonGraphic animFinish;
    [SerializeField] List<SkeletonGraphic> listAnimResults = new List<SkeletonGraphic>();
    [SerializeField] GameObject prefab_popup_player;
    [SerializeField] GameObject Clock;
    [SerializeField] GameObject PopupHistory;
    [SerializeField] GameObject PopupRule;
    [SerializeField] GameObject PopupTop;
    [SerializeField] TextMeshProUGUI lbTimeRemain;
    [SerializeField] GameObject nodePlayerHide;
    [SerializeField] TextMeshProUGUI lbPlayerHide;
    [SerializeField] DealerInGameView dealerInGame;
    [SerializeField] List<GameObject> listPot = new List<GameObject>();
    [SerializeField] List<GameObject> listTopWin = new List<GameObject>();
    [SerializeField] List<Avatar> listAvatarTop = new List<Avatar>();
    [SerializeField] List<Sprite> listDot = new List<Sprite>();
    [SerializeField] GameObject dot;
    [SerializeField] ScrollRect listDotHistory;
    [SerializeField] GameObject itemResult;
    [SerializeField] ScrollRect listItemHistory;
    [SerializeField] GameObject cardNode;
    [HideInInspector] private int historyCount = 0;
    [HideInInspector] private int lastWin = 0;
    // Win: red = 1; blue = 2; draw = 3;
    private long betValue = 0;
    private List<ChipBandar> chipPool1 = new List<ChipBandar>();
    private List<GameObject> cardPool1 = new List<GameObject>();
    public List<Player> listPlayerHide = new List<Player>();
    private List<long> listValueChipBets = new List<long>();
    private int chipBetColor = 0;
    private int myChipBetColor = 0;
    private bool checkBeted = false;
    private long[] listBet = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private long[] listMyBet = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private long[] listLastBet = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    protected List<ChipBandar> listChipInTable = new List<ChipBandar>();
    protected NodePlayerBandar listPlayer = null;
    private List<CancellationTokenSource> cancelTokList = new List<CancellationTokenSource>();

    // Start is calledQ before the first frame update
    // client g?i {"evt":"bet","M":"500","N":"1"}
    // M: m?c c??c
    // N: � c??c (t? 1 ??n 9)
    // 1 = RED          (merah)
    // 2 = RED_TWIN     (twin_left)
    // 3 = RED_QIU      (qiu_left)
    // 4 = DRAW         (Seri)
    // 5 = QIU_QIU      (qiuqiu)
    // 6 = SIX_TWINS    (six)
    // 7 = BLUE         (biru)
    // 8 = BLUE_QIU     (qiu_right)
    // 9 = BLUE_TWIN    (twin_right)

    protected override void Awake()
    {
        base.Awake();
        disablePot(false);
        btnBets.SetActive(false);
    }
    protected override void Start()
    {
        base.Start();
        //Debug.Log("dasdasdsa  ====   " + thisPlayer.ag);

    }
    public override void OnDestroy()
    {
        base.OnDestroy();

        cancelTokList.ForEach((token) =>
        {
            token?.Cancel();
            token.Dispose();
        });
    }
    public CancellationTokenSource getCancelToken()
    {
        CancellationTokenSource cancelTok = new CancellationTokenSource();
        cancelTokList.Add(cancelTok);
        return cancelTok;
    }
    public void resetGame()
    {
        checkBeted = false;
        Array.Copy(listMyBet, listLastBet, 9);
        listBet = listBet.Select(x => x * 0).ToArray();
        listMyBet = listMyBet.Select(x => x * 0).ToArray();
        updateStagePot();
        for (int i = 0; i < 9; i++)
        {
            listPot[i].transform.Find("press").gameObject.SetActive(false);
        }
        setStateButtonsBet(false, false);
        listAnimResults[0].gameObject.SetActive(false);
        listAnimResults[1].gameObject.SetActive(false);
        listBoxResult[0].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "";
        listBoxResult[1].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "";
        disablePot(false);
        updatePositionPlayerView();
    }

    protected override void updatePositionPlayerView()
    {
        listPlayerHide = new List<Player>();
        int size = 7;
        if (size == 7 && players.Count > size)
            size = 6;
        //players.Sort((a, b) =>
        //{
        //    return b.ag.CompareTo(a.ag);
        //});
        for (int i = 0; i < players.Count; i++)
        {
            if (thisPlayer == players[i])
            {
                players.RemoveAt(i);
            }
        }
        //players = [thisPlayer].concat(players);
        players.Insert(0, thisPlayer);
        for (var i = 0; i < players.Count; i++)
        {
            var index = getDynamicIndex(getIndexOf(players[i]));
            players[i]._indexDynamic = index;
            if (index >= size)
            {
                players[i].playerView.transform.localPosition = listPosView[6];
                listPlayerHide.Add(players[i]);
                players[i].playerView.gameObject.SetActive(false);
            }
            else
            {
                players[i].playerView.transform.localPosition = listPosView[index];
                players[i].playerView.gameObject.SetActive(true);
            }
            players[i].updateItemVip(players[i].vip);
        }
        lbPlayerHide.text = listPlayerHide.Count.ToString() + '+';
        nodePlayerHide.SetActive(listPlayerHide.Count > 0);
    }

    public void handleStartGame(JObject data)
    {
        // *********
        resetGame();
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.DISPATCH_CARD);
        effstart();
        // *********

        int timeStart = (int)data["timeOut"];

        setDisplayBet();
        lbTimeRemain.text = timeStart.ToString();
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            Debug.Log("Clock");
            timeStart--;
            lbTimeRemain.text = timeStart + "";
            if (Clock.gameObject.activeSelf)
            {
                SoundManager.instance.playEffectFromPath(timeStart > 3 ? Globals.SOUND_GAME.TICKTOK : Globals.SOUND_GAME.CLOCK_HURRY);
            }
        }).SetLoops(timeStart);
        DOTween.Sequence().AppendInterval(4.0f).AppendCallback(() =>
        {
            setStateButtonsBet(true, false);
            disablePot(true);
            btnBets.SetActive(true);
        });
        animStart.gameObject.SetActive(true);
        animStart.AnimationState.SetAnimation(0, "bettime", false);
        animStart.Initialize(true);
        animStart.AnimationState.Complete += delegate
        {
            animStart.gameObject.SetActive(false);
        };

        // hide score, dominos
        listBoxResult.ForEach(box =>
        {
            box.transform.Find("dominos").Find("domino1up").gameObject.SetActive(true);
            box.transform.Find("dominos").Find("domino2up").gameObject.SetActive(true);
            box.transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text = "";
        });

    }

    public async void handleFinishGame(JObject data)
    {
        stateGame = Globals.STATE_GAME.PLAYING;
        Clock.gameObject.SetActive(false);
        setStateButtonsBet(!checkBeted, checkBeted);
        btnBets.SetActive(false);
        disablePot(false);
        sendBandarHistory();
        List<JObject> inforDominos = data["listDomino"].ToObject<List<JObject>>();
        Vector2 posRed = listBoxResult[0].transform.Find("dominos").localPosition;
        Vector2 posBlue = listBoxResult[1].transform.Find("dominos").localPosition;
        Vector2 posFinishRed = Globals.Config.getPosInOtherNode(animFinish.transform.Find("red").position, listBoxResult[0]);
        Vector2 posFinishBlue = Globals.Config.getPosInOtherNode(animFinish.transform.Find("blue").position, listBoxResult[1]);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CARD_FLIP_2);
        listBoxResult[0].transform.Find("dominos").transform.DOLocalMove(posFinishRed, 1.5f).SetEase(Ease.InOutCubic);
        listBoxResult[0].transform.Find("dominos").transform.DOScale(1.5f, 1.5f);
        listBoxResult[1].transform.Find("dominos").transform.DOLocalMove(posFinishBlue, 1.5f).SetEase(Ease.InOutCubic);
        listBoxResult[1].transform.Find("dominos").transform.DOScale(1.5f, 1.5f);
        await Task.Delay(1500, getCancelToken().Token);
        animFinish.gameObject.SetActive(true);
        inforDominos.ForEach((dataDomino) =>
        {
            List<int> listDomino = getListInt(dataDomino, "listId");
            int type = getInt(dataDomino, "type");
            int score = getInt(dataDomino, "score");
            listBoxResult[type - 1].transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text = score.ToString();
            setImageDomino(listBoxResult[type - 1].transform.Find("dominos").Find("domino1").Find("top"), listDomino[0] / 7);
            setImageDomino(listBoxResult[type - 1].transform.Find("dominos").Find("domino1").Find("bottom"), listDomino[0] % 7);
            setImageDomino(listBoxResult[type - 1].transform.Find("dominos").Find("domino2").Find("top"), listDomino[1] / 7);
            setImageDomino(listBoxResult[type - 1].transform.Find("dominos").Find("domino2").Find("bottom"), listDomino[1] % 7);
            listBoxResult[type - 1].transform.Find("dominos").Find("domino1up").gameObject.SetActive(false);
            listBoxResult[type - 1].transform.Find("dominos").Find("domino2up").gameObject.SetActive(false);
        });
        SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.REWARD);
        await Task.Delay(2000);
        animFinish.gameObject.SetActive(false);

        listBoxResult[0].transform.Find("dominos").transform.DOLocalMove(posRed, 1f);
        listBoxResult[0].transform.Find("dominos").transform.DOScale(1f, 1f);
        listBoxResult[1].transform.Find("dominos").transform.DOLocalMove(posBlue, 1f);
        listBoxResult[1].transform.Find("dominos").transform.DOScale(1f, 1f);
        await Task.Delay(1000);
        List<int> listPotsWin = getListInt(data, "listResult");
        setPotsWin(listPotsWin);
        // anim boxs result
        if (int.Parse(listBoxResult[1].transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text)
            > int.Parse(listBoxResult[0].transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text))
        {
            listAnimResults[1].gameObject.SetActive(true);
        }
        if (int.Parse(listBoxResult[1].transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text)
            < int.Parse(listBoxResult[0].transform.Find("dominos").GetComponentInChildren<TextMeshProUGUI>().text))
        {
            listAnimResults[0].gameObject.SetActive(true);
        }
        if (listPotsWin.Contains(2))
        {
            listBoxResult[0].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "TWIN";
        }
        else if (listPotsWin.Contains(3))
        {
            listBoxResult[0].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "QIU";
        }
        if (listPotsWin.Contains(9))
        {
            listBoxResult[1].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "TWIN";
        }
        else if (listPotsWin.Contains(8))
        {
            listBoxResult[1].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "QIU";
        }
        // Note: get chip lose and refund chip win
        bool check = getChipLose(listPotsWin);
        if (check)
        {
            SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIP_WIN);
            // Note: waiting if the game have to get chip lose
            await Task.Delay(700, getCancelToken().Token);
        }
        returnChipWin(listPotsWin);
        await Task.Delay(1000, getCancelToken().Token);
        List<JObject> inforPlayers = JArray.Parse((string)data["data"]).ToObject<List<JObject>>();
        bool isPlayer = false;
        long betWin = 0;
        inforPlayers.Sort((a, b) =>
        {
            long bet1 = getLong(a, "M");
            long bet2 = getLong(b, "M");
            return bet2.CompareTo(bet1);
        });
        inforPlayers.ForEach((dataPl) =>
        {
            int id = getInt(dataPl, "uid");
            long ag = getLong(dataPl, "AG");
            long bet = getLong(dataPl, "M");
            Player player1 = getPlayerWithID(id);
            PlayerViewBandar plView = (PlayerViewBandar)player1.playerView;
            if (bet > 0 && player1.playerView.gameObject.activeSelf)
                player1.playerView.effectFlyMoney(bet, 26);
            if (bet < 0 && player1.playerView.gameObject.activeSelf)
                player1.playerView.setEffectLose(false);
            SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIP_WIN);
            player1.ag = ag;
            player1.setAg();
            if (id == thisPlayer.id && bet > 0)
            {
                betWin = bet;
                isPlayer = true;
            }
        });
        await Task.Delay(1000, getCancelToken().Token);
        PopupTop.transform.Find("bkg_popup").Find("chip_text").GetComponentInChildren<TextMeshProUGUI>().text
            = "+0";
        setPopupTop(inforPlayers);
        if (isPlayer)
        {
            PopupTop.transform.Find("bkg_popup").Find("chip_text").GetComponentInChildren<TextMeshProUGUI>().text
            = '+' + Globals.Config.FormatMoney(betWin);
            PopupTop.SetActive(true);
        }
        await Task.Delay(4000, getCancelToken().Token);
        listAnimResults[0].gameObject.SetActive(false);
        listAnimResults[1].gameObject.SetActive(false);
        PopupTop.SetActive(false);
        for (int i = 0; i < listPotsWin.Count; i++)
        {
            listPot[listPotsWin[i] - 1].transform.Find("press").gameObject.SetActive(false);
        }
        listBoxResult[0].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "";
        listBoxResult[1].transform.Find("lb").GetComponent<TextMeshProUGUI>().text = "";
        updatePositionPlayerView();
        checkAutoExit();
        stateGame = Globals.STATE_GAME.WAITING;
        Array.Copy(listMyBet, listLastBet, 9);
        HandleGame.nextEvt();

    }

    public void handleBet(JObject data)
    {
        //{"evt":"bet","uid":5919184,"Num":"1","M":"3200000"}
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);
        int id = (int)data["uid"];
        int betGate = (int)data["Num"];
        long chipBet = (long)data["M"];
        listBet[betGate - 1] += chipBet;
        if (id == thisPlayer.id)
        {
            listMyBet[betGate - 1] += chipBet;
        }
        setStateButtonsBet(!checkBeted, checkBeted);
        Player playerBet = getPlayerWithID(id);
        PlayerViewBandar plView = (PlayerViewBandar)playerBet.playerView;
        Vector2 posPlayer = plView.transform.localPosition;
        playerBet.ag -= chipBet;
        playerBet.setAg();
        // Note: set Random position on potGate for chipbet
        chipBetColor = listValueChipBets.IndexOf(chipBet);
        if (chipBetColor < 0)
        {
            long[] chipArray = new long[5];
            for (int i = 4; i >= 0; i--)
            {
                chipArray[i] = chipBet / listValueChipBets[i];
                chipBet = chipBet % listValueChipBets[i];
                for (var j = 0; j < chipArray[i]; j++)
                {
                    ChipBandar chip = getChip(i);
                    chip.gateid = betGate;
                    chip.playerid = id;
                    chip.chipValue = listValueChipBets[i];
                    chip.transform.localPosition = posPlayer;
                    chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                    Vector2 posPot = getRandomPositionPot(betGate);
                    chip.transform.DOLocalMove(posPot, 0.5f);
                    listChipInTable.Add(chip);
                }
            }
        }
        else
        {
            ChipBandar chip = getChip(chipBetColor);
            chip.gateid = betGate;
            chip.playerid = id;
            chip.chipValue = chipBet;
            //getPlayerView(playerBet).listChipBetPl.Add(chip);
            chip.transform.localPosition = posPlayer;
            chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
            Vector2 posPot = getRandomPositionPot(betGate);
            chip.transform.DOLocalMove(posPot, 0.5f);
            listChipInTable.Add(chip);
        }
        if (chipBet > 0 && playerBet.playerView.gameObject.activeSelf)
            playerBet.playerView.effectFlyMoney(-chipBet, 26);
        setDisplayBet();
        updateStagePot();
    }

    public override void handleSTable(string objData)
    {
        base.handleSTable(objData);
        Globals.Logging.Log("handleStable:" + objData);
        JObject data = JObject.Parse(objData);

        thisPlayer.playerView.setPosThanhBarThisPlayer();

        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
        //--------------- CREAT CHIP BETTED --------------//
        if (getInt(data, "Time") > 1)
        {
            JArray ArrP = getJArray(data, "ArrP");
            for (int i = 0; i < ArrP.Count; i++)
            {
                JObject dataPlayer = (JObject)ArrP[i];
                Player player = getPlayerWithID(getInt(dataPlayer, "id"));
                JArray Arr = getJArray(dataPlayer, "Arr");
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    int id = (int)dataChip["uid"];
                    int betGate = (int)dataChip["N"];
                    long chipBet = (long)dataChip["M"];
                    listBet[betGate - 1] += chipBet;
                    chipBetColor = listValueChipBets.IndexOf(chipBet);
                    if (chipBetColor < 0)
                    {
                        long[] chipArray = new long[5];
                        for (int k = 4; k >= 0; k--)
                        {
                            chipArray[k] = chipBet / listValueChipBets[k];
                            chipBet = chipBet % listValueChipBets[k];
                            for (var h = 0; h < chipArray[k]; h++)
                            {
                                ChipBandar chip1 = getChip(k);
                                chip1.gateid = betGate;
                                chip1.playerid = id;
                                chip1.chipValue = listValueChipBets[k];
                                chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip1.chipValue, true);
                                Vector2 posPot1 = getRandomPositionPot(betGate);
                                chip1.transform.localPosition = posPot1;
                                listChipInTable.Add(chip1);
                            }
                        }
                    }
                    else
                    {
                        ChipBandar chip1 = getChip(chipBetColor);
                        chip1.gateid = betGate;
                        chip1.playerid = id;
                        chip1.chipValue = chipBet;
                        chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
                        Vector2 posPot1 = getRandomPositionPot(betGate);
                        chip1.transform.localPosition = posPot1;
                        listChipInTable.Add(chip1);
                    }
                }
                updateStagePot();
            }
        }
        //--------------- END CREAT CHIP BETTED --------------//

        ////--------------- STABLE WHEN STARTED -----------//
        if (getInt(data, "Time") > 1)
        {
            int timeStart = getInt(data, "Time");
            Clock.SetActive(true);
            setStateButtonsBet(!checkBeted, checkBeted);
            setStateButtonsBet(false, false);
            btnBets.SetActive(true);
            setDisplayBet();
            disablePot(true);
            lbTimeRemain.text = timeStart.ToString();
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeStart--;
                lbTimeRemain.text = timeStart + "";
                if (Clock.gameObject.activeSelf)
                {
                    SoundManager.instance.playEffectFromPath(timeStart > 3 ? Globals.SOUND_GAME.TICKTOK : Globals.SOUND_GAME.CLOCK_HURRY);
                }
            }).SetLoops(timeStart);
        }
        else
        {
            disablePot(false);
            Clock.SetActive(false);
            btnBets.SetActive(false);
        }
        sendBandarHistory();
        updateStagePot();
        ////------------- END ------------//
    }

    public override void handleRJTable(string strData)
    {
        base.handleRJTable(strData);
        JObject data = JObject.Parse(strData);
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 5, agTable * 10, agTable * 50, agTable * 100 };
        if (getInt(data, "Time") > 1)
        {
            stateGame = Globals.STATE_GAME.PLAYING;
            JArray ArrP = getJArray(data, "ArrP");
            for (int i = 0; i < ArrP.Count; i++)
            {
                JObject dataPlayer = (JObject)ArrP[i];
                Player player = getPlayerWithID(getInt(dataPlayer, "id"));
                JArray Arr = getJArray(dataPlayer, "Arr");
                long totalBet = 0;
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    int id = (int)dataChip["uid"];
                    int betGate = (int)dataChip["N"];
                    long chipBet = (long)dataChip["M"];
                    totalBet += chipBet;
                    listBet[betGate - 1] += chipBet;
                    if (id == thisPlayer.id)
                    {
                        listMyBet[betGate - 1] += chipBet;
                    }
                    chipBetColor = listValueChipBets.IndexOf(chipBet);
                    if (chipBetColor < 0)
                    {
                        long[] chipArray = new long[5];
                        for (int k = 4; k >= 0; k--)
                        {
                            chipArray[k] = chipBet / listValueChipBets[k];
                            chipBet = chipBet % listValueChipBets[k];
                            for (var h = 0; h < chipArray[k]; h++)
                            {
                                ChipBandar chip1 = getChip(k);
                                chip1.gateid = betGate;
                                chip1.playerid = id;
                                chip1.chipValue = listValueChipBets[k];
                                chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip1.chipValue, true);
                                Vector2 posPot1 = getRandomPositionPot(betGate);
                                chip1.transform.localPosition = posPot1;
                                listChipInTable.Add(chip1);
                            }
                        }
                    }
                    else
                    {
                        ChipBandar chip1 = getChip(chipBetColor);
                        chip1.gateid = betGate;
                        chip1.playerid = id;
                        chip1.chipValue = chipBet;
                        chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
                        Vector2 posPot1 = getRandomPositionPot(betGate);
                        chip1.transform.localPosition = posPot1;
                        listChipInTable.Add(chip1);
                    }
                }
                if (player == thisPlayer)
                {
                    thisPlayer.ag -= totalBet;
                    thisPlayer.setAg();
                }
            }
        }
        updateStagePot();
        //--------------- END CREAT CHIP BETTED --------------//

        if (getInt(data, "Time") > 1)
        {
            int timeStart = getInt(data, "Time");
            Clock.SetActive(true);
            setStateButtonsBet(!checkBeted, checkBeted);
            setStateButtonsBet(false, false);
            btnBets.SetActive(true);
            setDisplayBet();
            disablePot(true);
            lbTimeRemain.text = timeStart.ToString();
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeStart--;
                lbTimeRemain.text = timeStart + "";
                if (Clock.gameObject.activeSelf)
                {
                    SoundManager.instance.playEffectFromPath(timeStart > 3 ? Globals.SOUND_GAME.TICKTOK : Globals.SOUND_GAME.CLOCK_HURRY);
                }
            }).SetLoops(timeStart);
        }
        else
        {
            disablePot(false);
            Clock.SetActive(false);
            btnBets.SetActive(false);
        }
        sendBandarHistory();
    }

    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(objData);

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
        //playSound(Globals.SOUND_GAME.REMOVE);
    }

    public override void HandlerTip(JObject data)
    {
        // {"evt":"tip", "N":"sugavip2k3", "AGTip":10000}
        playSound(Globals.SOUND_GAME.TIP);
        data["displayName"] = Globals.User.userMain.displayName;
        Player playerTip = getPlayer(getString(data, "N"));

        //require('SoundManager1').instance.dynamicallyPlayMusic(ResDefine.tipAudio);
        //EffectMoneyChange(-data.AGTip, players[i].ag, players[i]._playerView.lbAg);
        int AGTip = getInt(data, "AGTip");
        playerTip.playerView.effectFlyMoney(-AGTip, 30);
        playerTip.ag -= AGTip;
        playerTip.setAg();
        if (playerTip == thisPlayer)
        {
            Globals.User.userMain.AG -= AGTip;
        }
        ChipBandar temp = getChip(0);
        temp.chipValue = AGTip;
        temp.transform.localPosition = playerTip.playerView.transform.localPosition;
        temp.transform.localScale = new Vector2(0.4f, 0.4f);
        DOTween.Sequence()
            .AppendInterval(0.2f)
            .Append(temp.transform.DOLocalMove(temp.transform.localPosition + new Vector3(0, 80), 0.2f))
            .AppendInterval(0.3f)
            .Append(temp.transform.DOLocalMove(dealerInGame.transform.localPosition, 1.0f).SetEase(Ease.InOutSine))
            .AppendCallback(() =>
            {
                putChip(temp);
            });
        DOTween.Sequence()
            .AppendInterval(2.0f)
            .AppendCallback(() =>
            {
                dealerInGame.show(playerTip.namePl, AGTip);
            });
    }

    public async void effstart()
    {
        Vector2 posDefault = new Vector2(-Screen.width / 2, Screen.height / 2);
        Vector2 posCard = new Vector2(338, 0);
        List<GameObject> listCardInTable = new List<GameObject>();
        listBoxResult[0].transform.Find("dominos").gameObject.SetActive(false);
        listBoxResult[1].transform.Find("dominos").gameObject.SetActive(false);
        for (int i = 0; i < 14; i++)
        {
            GameObject card = getCard();
            card.transform.localPosition = posDefault;
            posCard.x = 338 - 52 * i;
            card.transform.DOLocalMove(posCard, 1f);
            listCardInTable.Add(card);
            await Task.Delay(100, getCancelToken().Token);
        }
        int blue = UnityEngine.Random.Range(0, 11);
        int red = UnityEngine.Random.Range(blue + 2, 13);
        await Task.Delay(1200, getCancelToken().Token);
        Vector2 posBlue = new Vector2(338 - 52 * blue, 20);
        Vector2 posBlue1 = new Vector2(286 - 52 * blue, 20);
        listCardInTable[blue].transform.DOLocalMove(posBlue, 0.3f);
        listCardInTable[blue + 1].transform.DOLocalMove(posBlue1, 0.3f);
        Vector2 posRed = new Vector2(338 - 52 * red, 20);
        Vector2 posRed1 = new Vector2(286 - 52 * red, 20);
        listCardInTable[red].transform.DOLocalMove(posRed, 0.3f);
        listCardInTable[red + 1].transform.DOLocalMove(posRed1, 0.3f);
        await Task.Delay(500, getCancelToken().Token);
        //Globals.Config.getPosInOtherNode(listPot[betGate - 1].transform.position, ChipContainer);
        posBlue = Globals.Config.getPosInOtherNode(listBoxResult[1].transform.Find("dominos").Find("domino2").position, CardContainer);
        posBlue1 = Globals.Config.getPosInOtherNode(listBoxResult[1].transform.Find("dominos").Find("domino1").position, CardContainer);
        posRed = Globals.Config.getPosInOtherNode(listBoxResult[0].transform.Find("dominos").Find("domino1").position, CardContainer);
        posRed1 = Globals.Config.getPosInOtherNode(listBoxResult[0].transform.Find("dominos").Find("domino2").position, CardContainer);
        listCardInTable[blue].transform.DOLocalMove(posBlue, 0.7f);
        listCardInTable[blue + 1].transform.DOLocalMove(posBlue1, 0.7f);
        listCardInTable[red].transform.DOLocalMove(posRed, 0.7f);
        listCardInTable[red + 1].transform.DOLocalMove(posRed1, 0.7f);
        await Task.Delay(1000, getCancelToken().Token);
        listBoxResult[0].transform.Find("dominos").gameObject.SetActive(true);
        listBoxResult[1].transform.Find("dominos").gameObject.SetActive(true);
        for (int i = 13; i >= 0; i--)
        {
            putCard(listCardInTable[i]);
        }
        Clock.SetActive(true);
    }

    public void setHistory(JObject list)
    {
        Debug.Log("SetHistory:");
        List<JObject> listHistory = list["data"].ToObject<List<JObject>>();
        List<JObject> inforDominos = new List<JObject>();
        int count = 0;
        listHistory.ForEach(data =>
        {
            count++;
            List<JObject> inforDominos = data["results"].ToObject<List<JObject>>();
            List<int> listPotsWin = getListInt(data, "wins");
            if (count > historyCount || (count == historyCount && count == 50))
            {
                GameObject item = Instantiate(itemResult, listItemHistory.content.transform);
                item.transform.SetAsFirstSibling();

                GameObject itemdot = Instantiate(dot, listDotHistory.content.transform);

                if (listPotsWin.Contains(1))
                {
                    itemdot.GetComponent<Image>().sprite = listDot[0];
                    item.transform.Find("red").Find("win").gameObject.SetActive(true);
                    setLineHistory(item.transform.Find("red").gameObject);
                    lastWin = 1;
                }
                else if (listPotsWin.Contains(7))
                {
                    item.transform.Find("blue").Find("win").gameObject.SetActive(true);
                    itemdot.GetComponent<Image>().sprite = listDot[2];
                    setLineHistory(item.transform.Find("blue").gameObject);
                    lastWin = 2;
                }
                else
                {
                    item.transform.Find("draw").gameObject.SetActive(true);
                    setLineHistory(item.transform.Find("draw").gameObject);
                    itemdot.GetComponent<Image>().sprite = listDot[1];
                    lastWin = 3;
                }
                if (listPotsWin.Contains(2))
                    item.transform.Find("textRed").GetComponentInChildren<TextMeshProUGUI>().text = "T";
                else if (listPotsWin.Contains(3))
                    item.transform.Find("textRed").GetComponentInChildren<TextMeshProUGUI>().text = "Q";
                if (listPotsWin.Contains(9))
                    item.transform.Find("textBlue").GetComponentInChildren<TextMeshProUGUI>().text = "T";
                else if (listPotsWin.Contains(8))
                    item.transform.Find("textBlue").GetComponentInChildren<TextMeshProUGUI>().text = "Q";
                item.SetActive(true);
                itemdot.SetActive(true);
                inforDominos.ForEach((dataDomino) =>
                {
                    int type = getInt(dataDomino, "type");
                    int score = getInt(dataDomino, "score");
                    if (type == 1)
                        item.transform.Find("red").Find("text").GetComponentInChildren<TextMeshProUGUI>().text = score.ToString();
                    else
                        item.transform.Find("blue").Find("text").GetComponentInChildren<TextMeshProUGUI>().text = score.ToString();
                });
            }
        });
        listDotHistory.DOHorizontalNormalizedPos(1.0f, 0.2f).SetEase(Ease.OutSine);
        historyCount = listHistory.Count;
    }

    public void setLineHistory(GameObject item)
    {
        if (lastWin == 1)
            item.transform.Find("red").gameObject.SetActive(true);
        else if (lastWin == 2)
            item.transform.Find("blue").gameObject.SetActive(true);
        else if (lastWin == 3)
            item.transform.Find("green").gameObject.SetActive(true);
    }

    public void setPopupTop(List<JObject> inforPlayers)
    {
        for (int i = 0; i < 3; i++)
        {
            listTopWin[i].SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            if (i < inforPlayers.Count)
            {
                int id = getInt(inforPlayers[i], "uid");
                long bet = getLong(inforPlayers[i], "M");
                if (bet <= 0)
                    break;
                Player topPlayer = getPlayerWithID(id);
                listTopWin[i].transform.Find("name").GetComponent<TextMeshProUGUI>().text = topPlayer.displayName;
                listTopWin[i].transform.Find("chip_text").GetComponent<TextMeshProUGUI>().text = Globals.Config.FormatMoney(bet, true);
                listAvatarTop[i].loadAvatar(topPlayer.avatar_id, topPlayer.namePl, topPlayer.fid);
                listTopWin[i].SetActive(true);
            }
        }
    }

    public void showPopupTop()
    {
        PopupTop.SetActive(true);
        SoundManager.instance.soundClick();
    }
    public void hidePopupTop()
    {
        SoundManager.instance.soundClick();
        PopupTop.SetActive(false);
    }

    public void showPopupHistory()
    {
        SoundManager.instance.soundClick();
        float x = PopupHistory.transform.parent.GetComponent<RectTransform>().rect.width / 2 - PopupHistory.GetComponent<RectTransform>().sizeDelta.x / 2;
        Vector2 pos = new Vector2(x, 0);
        PopupHistory.transform.DOLocalMove(pos, 0.5f);
    }

    public void hidePopupHistory()
    {
        SoundManager.instance.soundClick();
        Vector2 pos = new Vector2(Screen.width / 2 + PopupHistory.GetComponent<RectTransform>().sizeDelta.x / 2, 0);
        PopupHistory.transform.DOLocalMove(pos, 0.5f);
    }

    public override void onClickRule()
    {
        Vector2 pos = new Vector2(525, 0);
        PopupRule.transform.DOLocalMove(pos, 0.8f);
    }

    public void hideRule()
    {
        Vector2 pos = new Vector2(1300, 0);
        PopupRule.transform.DOLocalMove(pos, 0.8f);
    }

    public void onClickDoubleBet()
    {
        SoundManager.instance.soundClick();
        int timeleft = int.Parse(lbTimeRemain.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 9; i++)
            {
                if (listMyBet[i] > 0)
                {
                    SocketSend.sendBetBandar(listMyBet[i], i + 1);
                }
            }
        }
    }

    public void onClickReBet()
    {
        SoundManager.instance.soundClick();
        int timeleft = int.Parse(lbTimeRemain.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 9; i++)
            {
                if (listLastBet[i] > 0)
                {
                    checkBeted = true;
                    SocketSend.sendBetBandar(listLastBet[i], i + 1);
                }
            }
        }
        stateGame = Globals.STATE_GAME.PLAYING;
    }

    public void setStateButtonsBet(bool btnrebet, bool btndouble)
    {
        if (!listLastBet.Any(element => element != 0))
        {
            btnrebet = false;
        }
        btnBets.transform.Find("btn_rebet").GetComponent<Button>().interactable = btnrebet;
        btnBets.transform.Find("btn_double").GetComponent<Button>().interactable = btndouble;
    }


    public Vector2 getRandomPositionPot(int betGate)
    {
        Vector2 posPot = Globals.Config.getPosInOtherNode(listPot[betGate - 1].transform.position, ChipContainer);
        RectTransform rectfPot = listPot[betGate - 1].GetComponent<RectTransform>();
        posPot.x = UnityEngine.Random.Range(0, 2) < 0.5 ? posPot.x + UnityEngine.Random.Range(0, 20) * (int)rectfPot.rect.width / 100 : posPot.x - UnityEngine.Random.Range(0, 20) * (int)rectfPot.rect.width / 100;
        posPot.y = UnityEngine.Random.Range(0, 2) < 0.5 ? posPot.y + UnityEngine.Random.Range(0, 20) * (int)rectfPot.rect.height / 100 : posPot.y - UnityEngine.Random.Range(0, 20) * (int)rectfPot.rect.height / 100;
        return posPot;
    }

    public void updateStagePot()
    {
        for (int i = 0; i < 9; i++)
        {
            listPot[i].transform.Find("bet").GetComponentInChildren<TextMeshProUGUI>().text = listBet[i] > 0 ? Globals.Config.FormatMoney(listBet[i], true) : "";
            listPot[i].transform.Find("myBet").GetComponentInChildren<TextMeshProUGUI>().text = listMyBet[i] > 0 ? Globals.Config.FormatMoney(listMyBet[i], true) : "";
        }
    }

    public void setImageDomino(Transform dominoImage, int k)
    {
        dominoImage.GetComponent<Image>().sprite = listXS[k];
        dominoImage.GetComponent<Image>().SetNativeSize();
    }

    public bool getChipLose(List<int> listPotsWin)
    {
        int count = 0;
        Vector2 posModel = new Vector2(0, 220);
        for (int i = 0; i < listChipInTable.Count; i++)
        {
            var chip = listChipInTable[i];
            if (!listPotsWin.Contains(chip.gateid))
            {
                count++;
                chip.transform.DOLocalMove(posModel, 0.5f).OnComplete(() => putChip(chip));
                listChipInTable.Remove(chip);
                i--;
            }
        }
        return count > 0;
    }

    public async void returnChipWin(List<int> listPotsWin)
    {
        Vector2 posModel = new Vector2(0, 220);
        int count = listChipInTable.Count;
        //ChipBandar[] chipBands = new ChipBandar[count];
        for (int i = 0; i < count; i++)
        {
            var chip = listChipInTable[i];
            ChipBandar chip1 = getChip(chip.chipSprite);
            chip1.transform.localPosition = posModel;
            Vector2 posChip = chip.transform.localPosition;
            posChip.x += 5f;
            posChip.y += 5f;
            chip1.transform.DOLocalMove(posChip, 0.5f);
            chip1.playerid = chip.playerid;
            listChipInTable.Add(chip1);
        }
        SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIP_WIN);
        await Task.Delay(700, getCancelToken().Token);
        for (int i = 0; i < listChipInTable.Count; i++)
        {
            var chip = listChipInTable[i];
            // get player bet win position
            Player playerBet = getPlayerWithID(chip.playerid);
            PlayerViewBandar plView = (PlayerViewBandar)playerBet.playerView;
            Vector2 posPlayer = plView.transform.localPosition;
            chip.transform.DOLocalMove(posPlayer, 0.5f).OnComplete(() => putChip(chip));
        }
        listChipInTable.Clear();
    }

    public void disablePot(bool s)
    {
        for (int j = 0; j < 9; j++)
        {
            listPot[j].GetComponent<Button>().interactable = s;
        }
    }

    public async void setPotsWin(List<int> listPotsWin)
    {
        await Task.Delay(700, getCancelToken().Token);
        for (int j = 0; j < 15; j++)
        {
            for (int i = 0; i < listPotsWin.Count; i++)
            {
                listPot[listPotsWin[i] - 1].transform.Find("press").gameObject.SetActive(
                    !listPot[listPotsWin[i] - 1].transform.Find("press").gameObject.activeSelf);
            }
            await Task.Delay(280, getCancelToken().Token);
        }
    }

    public void sendBandarHistory()
    {
        JObject data = new JObject();
        data["evt"] = "history";
        WebSocketManager.getInstance().sendDataGame(data.ToString(Newtonsoft.Json.Formatting.None));
    }

    //private PlayerViewBandar getPlayerView(Player player)
    //{
    //    if (player != null)
    //    {
    //        return (PlayerViewBandar)player.playerView;
    //    }
    //    return null;
    //}

    public override PlayerView createPlayerView()
    {
        var plView = Instantiate(playerViewPrefab, PlayerContainer.transform);//.GetComponent<PlayerView>();
        plView.transform.SetSiblingIndex((int)Globals.ZODER_VIEW.PLAYER);

        return plView.GetComponent<PlayerViewBandar>();
    }

    public override void setGameInfo(int m, int id = 0, int maxBet = 0)
    {
        base.setGameInfo(m, id, maxBet);
        setInfoBet(m);
    }

    public void setInfoBet(int m)
    {
        listValueChipBets = new List<long> { m, m * 5, m * 10, m * 50, m * 100 };
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(listValueChipBets[i], true);
            //listChipBets[i].interactable = listValueChipBets[i] <= (int)thisPlayer.ag;
        }
        listChipBets[0].transform.localPosition = new Vector2(listChipBets[0].transform.localPosition.x, 20);
        betValue = listValueChipBets[0];
    }

    public void setDisplayBet()
    {
        bool check = false;
        //listChipBets.ForEach(btn => {
        //    btn.interactable = true;
        //});
        for (int i = 4; i >= 0; i--)
        {
            if (listValueChipBets[i] > thisPlayer.ag)
            {
                listChipBets[i].interactable = false;
                listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, 0);
                check = true;
            }
            else
            {
                // break
                listChipBets[i].interactable = true;
                if (stateGame == Globals.STATE_GAME.PLAYING && check && myChipBetColor >= i)
                {
                    listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, 20);
                    betValue = listValueChipBets[i];
                    check = false;
                }
            }
        }
    }


    public void onClickChipBet(int chipBet)
    {
        SoundManager.instance.soundClick();
        betValue = listValueChipBets[chipBet];
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, i == chipBet ? 20 : 0);
        }
        chipBetColor = chipBet;
        myChipBetColor = chipBet;
    }

    public void onClickBet(int gateBet)
    {
        SocketSend.sendBetBandar(betValue, gateBet);
        checkBeted = true;
        stateGame = Globals.STATE_GAME.PLAYING;
    }
    private ChipBandar getChip(int chipSprite)
    {
        ChipBandar chip;
        if (chipPool1.Count > 0)
        {
            chip = chipPool1[0];
            chipPool1.Remove(chip);
            chip.transform.parent = ChipContainer.transform;
            chip.gameObject.SetActive(true);
        }
        else
        {
            chip = Instantiate<ChipBandar>(chipPref, ChipContainer.transform);
        }
        chip.chipSprite = chipSprite;
        chip.GetComponent<Image>().sprite = chip.listSpriteChip[chipSprite];
        chip.transform.localScale = new Vector2(0.4f, 0.4f);
        return chip;
    }
    private void putChip(ChipBandar chip)
    {
        chipPool1.Add(chip);
        chip.transform.SetParent(null);
        chip.gameObject.SetActive(false);
    }
    // Update is called once per frame

    private GameObject getCard()
    {
        GameObject card;
        if (cardPool1.Count > 0)
        {
            card = cardPool1[0];
            cardPool1.Remove(card);
            card.transform.parent = CardContainer.transform;
        }
        else
        {
            card = Instantiate(cardNode, CardContainer.transform);

        }
        card.SetActive(true);
        return card;
    }

    private void putCard(GameObject card)
    {
        cardPool1.Add(card);
        card.transform.SetParent(null);
        card.SetActive(false);
    }

    public void onClickShowPlayer()
    {
        //if (listPlayer == null || buttonBet != null)
        updatePositionPlayerView();
        listPlayer = Instantiate(prefab_popup_player, transform).GetComponent<NodePlayerBandar>();
        listPlayer.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_MENU_VIEW);

    }

}