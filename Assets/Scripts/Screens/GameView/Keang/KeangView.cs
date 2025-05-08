using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Spine.Unity;
using System.Linq;
using UnityEngine.EventSystems;
using Globals;

public class KeangView : GameView, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lbGameTime;

    [SerializeField]
    TextMeshProUGUI lbPotTotal;
    [SerializeField]
    TextMeshProUGUI lbPotWin;

    [SerializeField]
    List<Avatar> listAvatarHitPot;

    [SerializeField]
    GameObject nodePot;

    [SerializeField]
    TextMeshProUGUI lbCardDeck;

    [SerializeField]
    GameObject cardDeck;

    [SerializeField]
    GameObject table;
    [SerializeField]
    Image sprProgressPot;
    [SerializeField]
    Avatar avatarPot;
    [SerializeField]
    List<GameObject> nodeCardPlayers;

    [SerializeField]
    GameObject nodeButton;
    [SerializeField]
    public GameObject dropCardPanel;
    [SerializeField]
    public BaseView rulePot;
    [SerializeField]
    public List<Vector2> listPosCardTable = new List<Vector2>();
    private List<Vector3> listAngleCardTable = new List<Vector3> { new Vector3(0, 0, 10), new Vector3(0, 0, -10), new Vector3(0, 0, 10), new Vector3(0, 0, -10), new Vector3(0, 0, 10) };

    [SerializeField]
    KeangButton buttonManager;
    [SerializeField]
    TextMeshProUGUI lbStateGame;

    private List<Card> canMatchArray = new List<Card>();
    public List<Card> selectedCards = new List<Card>();

    public SkeletonGraphic animKeang;
    public SkeletonGraphic animHitPot;
    public GameObject nodeLightning;
    public SkeletonGraphic animLightNing;
    [SerializeField]
    public SkeletonDataAsset dataAnimLightning;

    private int agPot = 0;
    private int cardDeckNum = 52;
    private int nextTurnId = 0;
    private int currentIndexFake = 0;
    private List<string> listDataFake = new List<string>();
    private List<Card> lastCardsDispatch = new List<Card>();
    private List<float> listProgressPot = new List<float> { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
    private List<Vector2> posDc = new List<Vector2> { new Vector2(-50, -50), new Vector2(-226, -6), new Vector2(-178, 129), new Vector2(178, 129), new Vector2(226, -6) };
    private Vector2 CARD_SIZE = new Vector2(147, 198);
    private JObject dataFinish = new JObject();
    List<Player> listPlayerHitPot = new List<Player>();


    private Player currentPlayer;
    private float PLAYERSCALE = 0.8f;
    private float OTHERSCALE = 0.4f;
    private float TABLESCALE = 0.4f;
    private bool isRJ = false;
    private int idTouch = -1, currentCountPot = 0;
    private Sequence seqShowAvatarHitPot = null;
    protected override void Awake()
    {
        base.Awake();
        checkButton(false);
        buttonManager.setController(this);
    }
    protected override void Start()
    {
        base.Start();
        //transform.eulerAngles = new Vector3(0, 0, 90);
        //listDataFake.Add("{\"evt\":\"rjtable\",\"data\":\"{\\\"pot\\\":350,\\\"M\\\":100,\\\"ArrP\\\":[{\\\"id\\\":394551,\\\"N\\\":\\\"ดิษ คัดลอกได้เลย\\\",\\\"AG\\\":3986,\\\"VIP\\\":2,\\\"Arr\\\":[0,0,0,0,0],\\\"Av\\\":4,\\\"FId\\\":168234172348421,\\\"UserType\\\":0,\\\"displayName\\\":\\\"ดิษ คัดลอกได้เลย\\\",\\\"lastDis\\\":[],\\\"levechop\\\":0,\\\"Dealer\\\":false,\\\"pot\\\":1,\\\"Tinyurl\\\":\\\"fb.168234172348421\\\",\\\"keyObjectInGame\\\":0},{\\\"id\\\":146913,\\\"N\\\":\\\"กุ้งนาง ทองขอด\\\",\\\"AG\\\":3687,\\\"VIP\\\":0,\\\"Arr\\\":[0,0,0,0,0,0],\\\"Av\\\":10,\\\"FId\\\":718306969550168,\\\"UserType\\\":0,\\\"displayName\\\":\\\"กุ้งนาง ทองขอด\\\",\\\"lastDis\\\":[],\\\"levechop\\\":0,\\\"Dealer\\\":true,\\\"pot\\\":1,\\\"Tinyurl\\\":\\\"fb.718306969550168\\\",\\\"keyObjectInGame\\\":0},{\\\"id\\\":8240,\\\"N\\\":\\\"hienndm\\\",\\\"AG\\\":1719,\\\"VIP\\\":3,\\\"Arr\\\":[3,43,44,8,35],\\\"Av\\\":12,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"displayName\\\":\\\"hienndm\\\",\\\"levechop\\\":0,\\\"Dealer\\\":false,\\\"pot\\\":0,\\\"Tinyurl\\\":\\\"\\\",\\\"keyObjectInGame\\\":0}],\\\"Id\\\":5268,\\\"S\\\":5,\\\"CN\\\":\\\"กุ้งนาง ทองขอด\\\",\\\"CT\\\":4,\\\"TotalAG\\\":0,\\\"rate\\\":0,\\\"score\\\":0,\\\"checkBoc\\\":true,\\\"idchop\\\":0,\\\"isFinish\\\":false,\\\"sizeDeck\\\":36}\",\"timeAction\":0,\"diem\":0,\"pid\":0}");
        //listDataFake.Add("{\"evt\":\"vtable\",\"data\":\"{\\\"pot\\\":500,\\\"M\\\":100,\\\"ArrP\\\":[{\\\"id\\\":182943,\\\"N\\\":\\\"Thu Ya\\\",\\\"AG\\\":3097,\\\"VIP\\\":3,\\\"Arr\\\":[0,0,0,0],\\\"Av\\\":1,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"displayName\\\":\\\"Thu Ya\\\",\\\"lastDis\\\":[17,30],\\\"levechop\\\":0,\\\"Dealer\\\":false,\\\"pot\\\":0,\\\"Tinyurl\\\":\\\"fb.991157401837853\\\",\\\"keyObjectInGame\\\":0},{\\\"id\\\":229523,\\\"N\\\":\\\"Suphanat Ketyu\\\",\\\"AG\\\":4605,\\\"VIP\\\":3,\\\"Arr\\\":[0,0,0],\\\"Av\\\":6,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"displayName\\\":\\\"Suphanat Ketyu\\\",\\\"lastDis\\\":[44,31],\\\"levechop\\\":0,\\\"Dealer\\\":true,\\\"pot\\\":4,\\\"Tinyurl\\\":\\\"fb.704401790713809\\\",\\\"keyObjectInGame\\\":0}],\\\"Id\\\":99,\\\"S\\\":5,\\\"CN\\\":\\\"Thu Ya\\\",\\\"CT\\\":9,\\\"TotalAG\\\":0,\\\"rate\\\":0,\\\"score\\\":0,\\\"checkBoc\\\":false,\\\"idchop\\\":0,\\\"isFinish\\\":false,\\\"sizeDeck\\\":38}\"}");
        //listDataFake.Add("{\"evt\":\"stable\",\"data\":\"{\\\"M\\\":100,\\\"ArrP\\\":[{\\\"id\\\":116988,\\\"N\\\":\\\"เปเล่ โมดิฟาย\\u0027ย\\\",\\\"AG\\\":4350,\\\"VIP\\\":6,\\\"Av\\\":999,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"LQ0\\\":0.0,\\\"displayName\\\":\\\"เปเล่ โมดิฟาย\\u0027ย\\\",\\\"pot\\\":2,\\\"keyObjectInGame\\\":60},{\\\"id\\\":47931,\\\"N\\\":\\\"ซี นายย.\\\",\\\"AG\\\":2923,\\\"VIP\\\":3,\\\"Av\\\":10,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"LQ0\\\":0.0,\\\"displayName\\\":\\\"ซี นายย.\\\",\\\"pot\\\":0,\\\"keyObjectInGame\\\":0},{\\\"id\\\":362369,\\\"N\\\":\\\"fb.136582508948578\\\",\\\"AG\\\":3079,\\\"VIP\\\":1,\\\"Av\\\":0,\\\"FId\\\":136582508948578,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"LQ0\\\":0.0,\\\"displayName\\\":\\\"Surerat Insee\\\",\\\"pot\\\":0,\\\"keyObjectInGame\\\":0},{\\\"id\\\":8240,\\\"N\\\":\\\"hienndm\\\",\\\"AG\\\":41738,\\\"VIP\\\":3,\\\"Av\\\":4,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"LQ0\\\":0.0,\\\"displayName\\\":\\\"hienndm\\\",\\\"pot\\\":0,\\\"keyObjectInGame\\\":0}],\\\"Id\\\":3898,\\\"AG\\\":500,\\\"S\\\":5,\\\"pot\\\":300}\",\"timeAction\":0,\"diem\":0,\"pid\":0}");
        //listDataFake.Add("{\"evt\":\"startGame\",\"time\":5}");
        //listDataFake.Add("{\"evt\":\"pot\",\"listId\":[116988,47931,362369,8240],\"agPot\":50,\"Pot\":500}");
        //listDataFake.Add("{\"evt\":\"lc\",\"NName\":116988,\"Arr\":[42,32,22,25,39],\"time\":10,\"CT\":10,\"sizeDeck\":32,\"sizeuser\":4,\"CO\":0.0}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":116988,\"sizeDeck\":31}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[5],\"idnext\":229523,\"pid\":182943,\"isChop\":true,\"idChop\":229523,\"agchopadd\":49,\"agchopout\":-50,\"levechop\":1,\"minSizeCardsUser\":3}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":229523,\"sizeDeck\":37}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[38],\"idnext\":182943,\"pid\":229523,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":3}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":362369,\"sizeDeck\":29}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[7,46],\"idnext\":8240,\"pid\":362369,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":4}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":36,\"pid\":8240,\"sizeDeck\":28}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[39],\"idnext\":116988,\"pid\":8240,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":4}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":116988,\"sizeDeck\":27}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[48],\"idnext\":47931,\"pid\":116988,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":4}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":47931,\"sizeDeck\":26}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[33,20],\"idnext\":362369,\"pid\":47931,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":3}");
        //listDataFake.Add("{\"evt\":\"bc\",\"C\":0,\"pid\":362369,\"sizeDeck\":25}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[23],\"idnext\":8240,\"pid\":362369,\"isChop\":false,\"idChop\":0,\"agchopadd\":0,\"agchopout\":0,\"levechop\":0,\"minSizeCardsUser\":3}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[36],\"idnext\":116988,\"pid\":8240,\"isChop\":true,\"idChop\":362369,\"agchopadd\":49,\"agchopout\":-50,\"levechop\":1,\"minSizeCardsUser\":3}");
        //listDataFake.Add("{\"evt\":\"dc\",\"arrC\":[36],\"idnext\":116988,\"pid\":8240,\"isChop\":true,\"idChop\":362369,\"agchopadd\":49,\"agchopout\":-50,\"levechop\":1,\"minSizeCardsUser\":3}");

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void onClickFake(Button btnFake)
    {
        string strData = listDataFake[0];
        Globals.Logging.Log("Fake strData:" + strData);

        //string strDataNext = listDataFake[1];
        JObject dataFake = JObject.Parse(strData);
        //JObject dataFakeNext = JObject.Parse(strDataNext);
        //btnFake.GetComponentInChildren<TextMeshProUGUI>().text = getString(dataFakeNext, "evt");
        HandleGame.processData(dataFake);
        listDataFake.RemoveAt(0);
    }
    public override void handleSTable(string strData)
    {
        base.handleSTable(strData);
        JObject data = JObject.Parse(strData);
        agPot = (int)data["pot"];
        cardDeckNum = 52;
        lbCardDeck.text = cardDeckNum.ToString();
        Globals.Config.tweenNumberTo(lbPotTotal, agPot);
        JArray arrP = (JArray)data["ArrP"];
        Player playerHitPot = null;
        int countPot = 0;
        for (int i = 0; i < arrP.Count; i++)
        {
            JObject dataPl = (JObject)arrP[i];
            Player player = getPlayerWithID((int)dataPl["id"]);
            getPlayerView(player).setPotCount((int)dataPl["pot"]);
            if ((int)dataPl["pot"] > countPot)
            {
                countPot = (int)dataPl["pot"];
                playerHitPot = player;
            }
        }
        getListPlayerHitPot();
        setPotProgress(countPot, listPlayerHitPot);

    }
    //private void getListPlayerHitPot(string strData)
    private void getListPlayerHitPot()
    {
        listPlayerHitPot.Clear();
        //JObject data = JObject.Parse(strData);
        int countPot = 0;
        JArray arrP = new JArray();
        //if (data.ContainsKey("ArrP"))
        //{
        //    arrP = (JArray)data["ArrP"];
        //}
        //else if (data.ContainsKey("arrP"))
        //{
        //    arrP = (JArray)data["arrP"];
        //}
        //foreach (JObject dataPl in arrP)
        //{
        //    if (dataPl.ContainsKey("pot") && (int)dataPl["pot"] > countPot)
        //    {
        //        countPot = (int)dataPl["pot"];
        //    }
        //    else if (dataPl.ContainsKey("countPot") && (int)dataPl["countPot"] > countPot)
        //    {
        //        countPot = (int)dataPl["countPot"];
        //    }
        //}
        //foreach (JObject dataPl in arrP)
        //{
        //    Player player = dataPl.ContainsKey("id") ? getPlayerWithID((int)dataPl["id"]) : getPlayerWithID((int)dataPl["pid"]);
        //    if (dataPl.ContainsKey("pot") && (int)dataPl["pot"] == countPot && countPot > 0)
        //    {
        //        listPlayerHitPot.Add(player);
        //    }
        //    else if (dataPl.ContainsKey("countPot") && (int)dataPl["countPot"] == countPot && countPot > 0)
        //    {
        //        listPlayerHitPot.Add(player);
        //    }
        //}
        foreach (Player pl in players)
        {
            countPot = (getPlayerView(pl).potCount > countPot) ? getPlayerView(pl).potCount : countPot;
        }
        currentCountPot = countPot;
        foreach (Player pl in players)
        {
            countPot = (getPlayerView(pl).potCount > countPot) ? getPlayerView(pl).potCount : countPot;
            if (getPlayerView(pl).potCount == countPot && countPot > 0)
            {
                listPlayerHitPot.Add(pl);
            }
        }

    }
    public override void handleCTable(string strData)
    {
        base.handleCTable(strData);
        sprProgressPot.fillAmount = 0;
    }
    public override void handleJTable(string strData)
    {
        base.handleJTable(strData);
        var dataPl = JObject.Parse(strData);
        PlayerViewKeang playerJ = getPlayerView(getPlayerWithID((int)dataPl["id"]));
        playerJ.setPotCount(0);
    }
    public override void handleRJTable(string strData)
    {
        base.handleRJTable(strData);
        isRJ = true;
        int indexPl = getIndexOfPlayer(thisPlayer);
        int indexLastPl = indexPl == 0 ? players.Count - 1 : indexPl - 1;
        int nameLastCard = -1;
        JObject data = JObject.Parse(strData);
        agPot = (int)data["pot"];
        Globals.Config.tweenNumberTo(lbPotTotal, agPot);
        cardDeckNum = (int)data["sizeDeck"];
        lbCardDeck.text = cardDeckNum.ToString();
        cardDeck.gameObject.SetActive(true);
        JArray arrP = (JArray)data["ArrP"];
        int countPot = 0;
        if (getBool(data, "isFinish") == false)
        {
            for (int i = 0; i < arrP.Count; i++)
            {
                JObject dataPl = (JObject)arrP[i];
                Player player = getPlayerWithID((int)dataPl["id"]);
                bool isLeft = player._indexDynamic < 3 ? true : false;
                player.setDealer(getBool(dataPl, "Dealer"), isLeft, false);
                JArray arr = (JArray)dataPl["Arr"];
                List<int> arrIDCard = arr.ToObject<List<int>>();
                player.arrCodeCard = arrIDCard;
                dealCardPlayer(player, arrIDCard.Count);
                getPlayerView(player).setPotCount((int)dataPl["pot"]);
                Globals.Logging.Log(i);
                if (dataPl.ContainsKey("lastDis"))
                {
                    List<int> arrDis = getListInt(dataPl, "lastDis");// (JArray)dataPl["lastDis"];
                    //List<int> arrIDCardDis = arrDis.ToObject<List<int>>();

                    if (arrDis.Count != 0)
                    {
                        setCardOnTable(player, arrDis);
                        if (stateGame == Globals.STATE_GAME.PLAYING && arrP.IndexOf(dataPl) == indexLastPl)
                            nameLastCard = arrDis[arrDis.Count - 1];
                    }

                }
                if ((int)dataPl["pot"] > countPot)
                {
                    countPot = (int)dataPl["pot"];
                }

            }
            getListPlayerHitPot();
            setPotProgress(currentCountPot, listPlayerHitPot);
            Player playerCN = getPlayer((string)data["CN"]);
            setPlayerTurn(playerCN, (int)data["CT"]);
            int isBoc = (bool)data["checkBoc"] ? 2 : 1;
            if (playerCN == thisPlayer)
            {
                checkButton(true, isBoc, nameLastCard, (int)data["idchop"]);
            }
        }

    }
    public override void handleVTable(string strData)
    {
        base.handleVTable(strData);
        UIManager.instance.showToast(Globals.Config.getTextConfig("txt_view_table"), transform);
        int indexPl = players.IndexOf(thisPlayer);
        int indexLastPl = indexPl == 0 ? players.Count - 1 : indexPl - 1;
        JObject data = JObject.Parse(strData);
        agPot = (int)data["pot"];
        Globals.Config.tweenNumberTo(lbPotTotal, agPot);
        cardDeckNum = (int)data["sizeDeck"];
        lbCardDeck.text = cardDeckNum.ToString();
        cardDeck.gameObject.SetActive(true);
        JArray arrP = (JArray)data["ArrP"];
        bool isFinish = (bool)data["isFinish"];
        int countPot = 0;
        for (int i = 0; i < arrP.Count; i++)
        {
            JObject dataPl = (JObject)arrP[i];
            Player player = getPlayerWithID((int)dataPl["id"]);
            if (isFinish == false)
            {
                bool isLeft = player._indexDynamic < 3 ? true : false;
                player.setDealer(getBool(dataPl, "Dealer"), isLeft, false);
                JArray arr = (JArray)dataPl["Arr"];
                List<int> arrIDCard = arr.ToObject<List<int>>();
                dealCardPlayer(player, arrIDCard.Count);
                player.arrCodeCard = arrIDCard;
                if (dataPl.GetValue("lastDis") != null)
                {
                    JArray arrDis = (JArray)dataPl["lastDis"];
                    List<int> arrIDCardDis = arrDis.ToObject<List<int>>();
                    if (arrIDCardDis.Count != 0)
                    {
                        setCardOnTable(player, arrIDCardDis);
                    }

                }
                if (player.namePl == (string)data["CN"])
                {
                    setPlayerTurn(player, (int)data["CT"]);
                }
            }
            getPlayerView(player).setPotCount((int)dataPl["pot"]);
            if ((int)dataPl["pot"] > countPot)
            {
                countPot = (int)dataPl["pot"];
            }
        }
        getPlayerView(thisPlayer).setDark(true);
        getListPlayerHitPot();
        setPotProgress(currentCountPot, listPlayerHitPot);
    }
    private void resetGameView()
    {
        lastCardsDispatch.Clear();
        stateGame = Globals.STATE_GAME.WAITING;
        foreach (Player player in players)
        {
            PlayerViewKeang playerV = getPlayerView(player);
            player.setDealer(false);
            playerV.showScore(false);
            foreach (Card card in player.vectorCard)
            {
                removerCard(card);
            }

            foreach (Card card in playerV.cardOnTables)
            {
                removerCard(card);
            }
            player.vectorCard.Clear();
            player.arrCodeCard.Clear();
            playerV.cardOnTables.Clear();
            string name = "panel" + player._indexDynamic;
            Transform cardPanel = table.transform.Find(name);

            if (cardPanel != null)
            {
                clearLastDropedCard(cardPanel);
            }
            if (agPot == 0)
            {
                playerV.setPotCount(0);
                setPotProgress(-1);
                lbPotTotal.text = "0";
            }
        }
        dataFinish = null;
        cardDeckNum = 52;
        lbCardDeck.text = "52";
        animKeang.gameObject.SetActive(false);
        checkButton(false);
        HandleGame.nextEvt();
        lbStateGame.gameObject.SetActive(true);
        UIManager.instance.DOTextTmp(lbStateGame, Globals.Config.getTextConfig("txt_waiting_game"));
        if (currentCountPot == 5)
        {
            setPotProgress(-1);
        }
        checkAutoExit();
    }
    private void setPotProgress(int countPot = -1, List<Player> playerHitPot = null)
    {
        if (countPot != -1)
        {
            sprProgressPot.DOFillAmount(listProgressPot[countPot], 0.3f);
        }
        else
        {
            avatarPot.setDefault();
            sprProgressPot.DOFillAmount(listProgressPot[0], 0.3f);
        }
        if (playerHitPot != null && playerHitPot.Count > 0)
        {
            if (playerHitPot.Count == 1)
            {
                avatarPot.loadAvatar(playerHitPot[0].avatar_id, playerHitPot[0].displayName, playerHitPot[0].fid);
                if (seqShowAvatarHitPot != null)
                {
                    seqShowAvatarHitPot.Kill();
                }
            }
            else
            {
                int currentIndexPlOnPot = 0;
                if (seqShowAvatarHitPot != null)
                {
                    seqShowAvatarHitPot.Kill();
                }
                seqShowAvatarHitPot = DOTween.Sequence();
                seqShowAvatarHitPot.AppendCallback(() =>
                {
                    avatarPot.effectShowAvatar(playerHitPot[currentIndexPlOnPot].playerView.getAvatarSprite());
                });
                seqShowAvatarHitPot.AppendInterval(2.5f);
                seqShowAvatarHitPot.AppendCallback(() =>
                {
                    currentIndexPlOnPot++;
                    if (currentIndexPlOnPot >= playerHitPot.Count)
                        currentIndexPlOnPot = 0;
                });
                seqShowAvatarHitPot.SetLoops(-1);
            }
        }

    }
    public void handleFinish(JObject dataF)
    {
        playSound(Globals.SOUND_GAME.ALERT);
        JObject data = JObject.Parse((string)dataF["data"]);
        checkButton(false);
        cardDeck.SetActive(false);
        dataFinish = data;
        JArray arrP = (JArray)data["arrP"];
        int countPot = 0;
        Player playerHitPot = null;
        int idkaeng = (int)data["idkaeng"];
        bool isHitPot = false;

        for (int i = 0; i < arrP.Count; i++)
        {
            JObject dataPl = (JObject)arrP[i];
            Player player = getPlayerWithID((int)dataPl["pid"]);
            player.setTurn(false);
            PlayerViewKeang playerV = getPlayerView(player);
            JArray arr = (JArray)dataPl["arr"];
            List<int> arrIDCard = arr.ToObject<List<int>>();
            player.arrCodeCard = arrIDCard;
            player.score = (int)dataPl["score"];
            playerV.potCount = (int)dataPl["countPot"];
            if ((int)dataPl["countPot"] > countPot)
            {
                countPot = (int)dataPl["countPot"];
                playerHitPot = player;
            }
            if ((bool)dataPl["potNow"] == true)
            {
                isHitPot = true;
                agPot = 0;
            }
        }
        float timeShowHitPot = isHitPot == true ? 2.5f : 0.0f;
        float timeResolveWinLose = 6.0f;
        getListPlayerHitPot();
        // co 2 case 1 thang keang sau do hit pot hien big win hitpot va win norrmal(ko keang) sau do hien bigwin hitpot
        if (idkaeng != 0)
        {
            PlayerViewKeang playerVK = getPlayerView(getPlayerWithID(idkaeng));
            showAnimKeang(playerVK, countPot, playerHitPot, isHitPot);
        }
        else
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    resolvePlayerWinLose();
                    setPotProgress(currentCountPot, listPlayerHitPot);
                })
                .AppendInterval(timeResolveWinLose)
                 .AppendCallback(() =>
                 {
                     if (isHitPot)//neu ko co thang nao hitpot thi show tra chip winlose luon.time=0;
                     {
                         showAnimHitPot();
                     }
                 })
                .AppendInterval(timeShowHitPot).AppendCallback(() =>
             {
                 resetGameView();
             });
        }


    }
    private void showAnimHitPot()
    {
        playSound(Globals.SOUND_GAME.REWARD);
        animHitPot.gameObject.SetActive(true);
        animHitPot.Initialize(true);
        animHitPot.AnimationState.SetAnimation(0, "thai", true);

        JArray arrP = (JArray)dataFinish["arrP"];
        int potWin = 0;
        lbPotWin.text = "0";
        int count = 0;
        for (int i = 0; i < arrP.Count; i++)
        {
            JObject dataPl = (JObject)arrP[i];
            Player player = getPlayerWithID((int)dataPl["pid"]);
            player.setTurn(false);
            PlayerViewKeang playerV = getPlayerView(player);
            potWin += (int)dataPl["agPot"];
            if ((int)dataPl["agPot"] > 0)
            {
                DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
                {
                    player.ag = (int)dataPl["ag"];
                    player.setAg();
                    playerV.effectFlyMoney((int)dataPl["agPot"]);
                });
            }
            if ((int)dataPl["agPot"] != 0)
            {
                listAvatarHitPot[count].gameObject.SetActive(true);
                listAvatarHitPot[count].setSpriteFrame(playerV.getAvatarSprite());
                listAvatarHitPot[count].GetComponentInChildren<TextMeshProUGUI>().text = player.displayName;
                count++;
            }
        }
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
        {
            animHitPot.transform.Find("AvatarContainer").gameObject.SetActive(true);
            Globals.Config.tweenNumberTo(lbPotWin, potWin, 0, 1.0f);

        });

        DOTween.Sequence()
               .AppendInterval(3.0f)
               .AppendCallback(() =>
               {
                   animHitPot.gameObject.SetActive(false);
                   animHitPot.transform.Find("AvatarContainer").gameObject.SetActive(false);
                   foreach (Avatar avt in listAvatarHitPot)
                   {
                       avt.gameObject.SetActive(false);
                   }
               });


    }
    private void showAnimKeang(PlayerViewKeang playerV, int countPot, Player playerHitPot, bool isHitPot = false)
    {
        float timeShowHitPot = isHitPot == true ? 2.5f : 0.0f;
        float timeShowWinLose = 6.0f;
        Vector2 posPlayer = playerV.transform.localPosition;
        animKeang.transform.parent.SetSiblingIndex((int)Globals.ZODER_VIEW.EFFECT);
        animKeang.transform.localScale = new Vector2(1.0f, 1.0f);
        animKeang.gameObject.SetActive(true);
        animKeang.transform.localPosition = posPlayer;
        animKeang.Initialize(true);
        animKeang.AnimationState.SetAnimation(0, "animation", true);

        animKeang.transform.DOLocalMove(new Vector2(0, 100), 1.0f).SetDelay(1.5f)
           .OnComplete(() =>
           {
               DOTween.Sequence()
               .AppendCallback(() =>
               {
                   resolvePlayerWinLose();
                   setPotProgress(currentCountPot, listPlayerHitPot);
               })
               .AppendInterval(timeShowWinLose)
               .AppendCallback(() =>
               {
                   if (isHitPot)
                       showAnimHitPot();
               });
           });

        animKeang.transform.DOScale(new Vector2(0, 0), 0.3f).SetEase(Ease.InBack).SetDelay(timeShowWinLose + timeShowHitPot).OnComplete(() =>
          {
              resetGameView();
          });
    }
    private void resolvePlayerWinLose()
    {
        JArray arrP = (JArray)dataFinish["arrP"];
        for (int i = 0; i < arrP.Count; i++)
        {
            JObject dataPl = (JObject)arrP[i];
            Player player = getPlayerWithID((int)dataPl["pid"]);
            autoSortPlayerCard(player);
            PlayerViewKeang playerV = getPlayerView(player);
            playerV.showScore(true, (int)dataPl["score"], (player == thisPlayer));
            DOTween.Sequence()
                .AppendInterval(2.0f)
                .AppendCallback(() =>
                {
                    playerV.setPotCount((int)dataPl["countPot"]);
                    int agChange = (int)dataPl["agchange"];
                    if (agChange > 0)
                    {
                        playerV.setEffectWin("win_thai");
                        playSound(Globals.SOUND_GAME.THROW_CHIP);
                        if (player == thisPlayer)
                        {
                            playSound(Globals.SOUND_GAME.WIN);
                        }
                    }
                    else
                    {
                        playSound(Globals.SOUND_GAME.GET_CHIP);
                        playerV.setEffectLose();
                        if (player == thisPlayer)
                        {
                            playSound(Globals.SOUND_GAME.LOSE);
                        }
                    }
                    DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() =>
                    {
                        player.ag += agChange;
                        player.setAg();
                        playerV.effectFlyMoney(agChange);
                        playerV.animResult.gameObject.SetActive(false);
                    });
                });
        }
    }
    public void handleLc(JObject data)
    {
        lbStateGame.gameObject.SetActive(false);
        playSound(Globals.SOUND_GAME.DISPATCH_CARD);
        stateGame = Globals.STATE_GAME.PLAYING;
        cardDeck.SetActive(true);
        cardDeck.transform.DOScale(cardDeck.transform.localScale + new Vector3(0.2f, 0.2f), 1.0f).OnComplete(() =>
        {
            cardDeck.transform.localScale = new Vector2(0.4f, .4f);
        });

        nextTurnId = (int)data["NName"];
        JArray cardOnHandIds = (JArray)data["Arr"];
        List<int> arrIds = cardOnHandIds.ToObject<List<int>>();
        int timeTurn = (int)data["CT"];
        for (int i = 0, l = players.Count; i < l; i++)
        {

            if (players[i] == thisPlayer) players[i].arrCodeCard = arrIds;
            else players[i].arrCodeCard = new List<int> { 0, 0, 0, 0, 0 };
            dealCardPlayer(players[i], arrIds.Count);
            bool isLeft = players[i]._indexDynamic < 3 ? true : false;
            if (players[i].id == nextTurnId)
            {
                players[i].setDealer(true, isLeft, false);
            }
        }
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            Player playerNext = getPlayerWithID(nextTurnId);
            setPlayerTurn(playerNext, timeTurn);
            if (nextTurnId == thisPlayer.id)
                checkButton(true, 1);
            HandleGame.nextEvt();
        });

    }
    public void dealCardPlayer(Player player, int len)
    {
        int index = getDynamicIndex(getIndexOf(player));
        GameObject nodeCard = nodeCardPlayers[index];
        PlayerViewKeang playerV = getPlayerView(player);
        float timeDeal = stateGame == Globals.STATE_GAME.PLAYING ? 0.4f : 0.0f;
        timeDeal = isRJ ? 0 : timeDeal;
        float scale = player == thisPlayer ? PLAYERSCALE : OTHERSCALE;
        for (int i = 0; i < len; i++)
        {

            if (stateGame == Globals.STATE_GAME.PLAYING && isRJ == false)
            {
                DOTween.Sequence()
                .AppendInterval(0.2f * i)
                .AppendCallback(() =>
                {
                    Card cardTemp = getCard(nodeCard.transform, 0.3f);
                    Vector2 startPos = nodeCard.transform.InverseTransformPoint(cardDeck.transform.position);
                    cardTemp.transform.localPosition = startPos;
                    player.vectorCard.Add(cardTemp);
                    float posX = Random.Range(0.0f, 1.0f) * 40 - 20;
                    float posY = Random.Range(0.0f, 1.0f) * 20 - 10;
                    float angle = i % 2 == 0 ? Random.Range(0.0f, 1.0f) * 10 : Random.Range(0.0f, 1.0f) * -10;
                    cardTemp.transform.DOLocalMove(new Vector2(posX, posY), timeDeal).SetEase(Ease.OutBack);
                    cardTemp.transform.DOLocalRotate(new Vector3(0, 0, angle), timeDeal).SetEase(Ease.OutBack);
                    cardTemp.transform.DOScale(new Vector2(scale, scale), timeDeal).SetEase(Ease.OutCubic);
                    cardDeckNum--;
                    lbCardDeck.text = cardDeckNum.ToString();
                    if (player == thisPlayer)
                    {
                        cardTemp.addButtonClick(() =>
                        {
                            onCardClick(cardTemp);
                        });
                    }
                });
            }
            else
            {
                Card cardTemp = getCard(nodeCard.transform, 0.3f);
                Vector2 startPos = nodeCard.transform.InverseTransformPoint(cardDeck.transform.position);
                cardTemp.transform.localPosition = startPos;
                cardTemp.transform.localScale = new Vector2(scale, scale);
                player.vectorCard.Add(cardTemp);
                cardDeckNum--;
                lbCardDeck.text = cardDeckNum.ToString();
                if (player == thisPlayer)
                {
                    //cardTemp.addButtonClick(onCardClick(cardTemp));
                    cardTemp.addButtonClick(() =>
                    {
                        onCardClick(cardTemp);
                    });
                }
            }
        }
        if (stateGame == Globals.STATE_GAME.PLAYING && isRJ == false)
        {
            DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
            {
                autoSortPlayerCard(player);
            });
        }
        else
        {
            DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
            {
                autoSortPlayerCard(player);
            }).AppendInterval(0.5f).AppendCallback(() =>
            {
                isRJ = false;
            });
        }

    }
    public void onClickRulePot()
    {
        rulePot.show();
        rulePot.transform.SetAsLastSibling();
    }
    private void autoSortPlayerCard(Player player)
    {
        int len = player.vectorCard.Count;
        bool isPlayer = player == thisPlayer ? true : false;
        int index = 0;
        float timeEffect = stateGame == Globals.STATE_GAME.PLAYING ? 0.2f : 0.0f;
        Ease easing = stateGame == Globals.STATE_GAME.PLAYING ? Ease.OutCubic : Ease.Unset;
        if (player.arrCodeCard.Count > 0)
        {
            if (player.arrCodeCard[0] != 0)
            {
                playSound(Globals.SOUND_GAME.CARD_FLIP_2);
            }
        }
        if (player.arrCodeCard.Count > 0)
            for (int i = 0; i < len; i++)
            {
                Card cardTemp = player.vectorCard[i];
                JObject cal = getCardPosition(i, len, isPlayer);

                //cardTemp.node.stopAllActions();
                Transform cardTf = cardTemp.transform;
                cardTemp.showBorder(false);
                cardTemp.isAllowTouch = false;
                if (stateGame == Globals.STATE_GAME.VIEWING || isRJ == true) //set luon ,k co hieu ung gi het
                {
                    cardTf.localEulerAngles = new Vector3(0, 0, (float)cal["angle"]);
                    cardTf.localPosition = new Vector2((float)cal["posX"], (float)cal["posY"]);
                    if (player.arrCodeCard.Count != 0)
                    {
                        cardTemp.setTextureWithCode(player.arrCodeCard[i]);
                        cardTemp.isAllowTouch = true;
                    }
                    if (thisPlayer.arrCodeCard.Count != 0)
                    {
                        int score = getPlayerPoint(thisPlayer.vectorCard);
                        getPlayerView(thisPlayer).showScore(true, score, true);
                    }
                }
                else
                {
                    DOTween.Sequence()
                    .Append(cardTf.DOLocalMove(Vector2.zero, timeEffect).SetEase(easing))
                    .Join(cardTf.DOLocalRotate(Vector3.zero, timeEffect).SetEase(easing))
                    .AppendInterval(timeEffect)
                    .AppendCallback(() =>
                    {
                        if (player.arrCodeCard.Count != 0)
                        {
                            cardTemp.setTextureWithCode(player.arrCodeCard[index]);
                        }
                        index++;
                        if (index == len)
                        {
                            if (thisPlayer.arrCodeCard.Count != 0)
                            {
                                int score = getPlayerPoint(thisPlayer.vectorCard);
                                getPlayerView(thisPlayer).showScore(true, score, true);
                            }
                        }
                    })
                    .Append(cardTf.DOLocalMove(new Vector2((float)cal["posX"], (float)cal["posY"]), timeEffect * 2).SetEase(easing))
                    .Join(cardTf.DOLocalRotate(new Vector3(0, 0, (float)cal["angle"]), timeEffect * 2).SetEase(easing))
                    .AppendCallback(() =>
                    {
                        cardTemp.isAllowTouch = true;
                    });
                }
            }
    }
    public override void handleLTable(JObject data)
    {
        base.handleLTable(data);
        getListPlayerHitPot();
        setPotProgress(currentCountPot, listPlayerHitPot);
        //JObject dataPLeft= arrP.FirstOrDefault( item =>getString(item,"");

    }
    public void handleBc(JObject data)
    {
        playSound(Globals.SOUND_GAME.CARD_FLIP_2);
        Player playerBc = getPlayerWithID((int)data["pid"]);
        int index = getDynamicIndex(getIndexOf(playerBc));
        GameObject nodeCard = nodeCardPlayers[index];
        cardDeckNum = (int)data["sizeDeck"];
        lbCardDeck.text = cardDeckNum.ToString();
        PlayerViewKeang playerV = (PlayerViewKeang)playerBc.playerView;
        int cardId = (int)data["C"];
        Card card = getCard(nodeCard.transform, 0.3f);
        card.addButtonClick(() =>
        {
            onCardClick(card);
        });
        card.transform.localPosition = cardDeck.transform.localPosition;
        card.setTextureWithCode(cardId);
        playerBc.vectorCard.Add(card);
        Vector2 startPos = nodeCard.transform.InverseTransformPoint(cardDeck.transform.position);
        card.transform.localPosition = startPos;
        if (playerBc == thisPlayer)
        {
            checkButton(true, 2);
            Transform cardTf = card.transform;
            DOTween.Sequence()
                .Append(cardTf.DOScale(new Vector2(PLAYERSCALE, PLAYERSCALE), 0.2f).SetEase(Ease.OutCubic))
                .Join(cardTf.DOLocalMove(new Vector2(-50, 250), 0.2f).SetEase(Ease.OutCubic));

            DOTween.Sequence()
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    normalSortPlayerCard(playerBc);
                })
                .AppendInterval(0.3f)
                .AppendCallback(() =>
                {
                    if (currentPlayer == thisPlayer)
                        autoPickCardsHint();
                });
            //cardTemp.node.stopAllActions();
        }
        else
        {
            normalSortPlayerCard(playerBc);
        }

    }
    private void normalSortPlayerCard(Player player)
    {
        int len = player.vectorCard.Count;
        bool isPlayer = player == thisPlayer ? true : false;
        if (isPlayer)
        {
            sortPlayerCardByValue(player);
            selectedCards = new List<Card>();
            int score = getPlayerPoint(thisPlayer.vectorCard);
            getPlayerView(player).showScore(true, score, (player == thisPlayer));
        }
        float scale = isPlayer ? PLAYERSCALE : OTHERSCALE;

        for (int i = 0; i < len; i++)
        {
            Card cardTemp = player.vectorCard[i];
            JObject cal = getCardPosition(i, len, isPlayer);
            cardTemp.showBorder(false);
            cardTemp.transform.SetSiblingIndex(i);
            //cardTemp.node.stopAllActions();
            cardTemp.isAllowTouch = false;
            Transform cardTempTf = cardTemp.transform;
            DOTween.Sequence()
                .Append(cardTempTf.DOLocalMove(new Vector2((float)cal["posX"], (float)cal["posY"]), 0.3f).SetEase(Ease.OutCubic))
                .Join(cardTempTf.DOLocalRotate(new Vector3(0, 0, (float)cal["angle"]), 0.3f).SetEase(Ease.OutCubic))
                .Join(cardTempTf.DOScale(new Vector2(scale, scale), 0.3f))
                .AppendCallback(() =>
                {
                    cardTemp.isAllowTouch = true;
                })
                .AppendInterval(0.3f);
        }
        checkAllowButton();
    }
    private void sortPlayerCardByValue(Player player)
    {
        List<Card> vectorCard = player.vectorCard;
        sortCardS(vectorCard);
        sortCardN(vectorCard);
    }
    private void showLightningEffect(Vector2 pos1, Vector2 pos2)
    {
        playSound(Globals.SOUND_GAME.EXPLODE);

        float x = Mathf.Abs(pos2.x - pos1.x);

        float y = Mathf.Abs(pos2.y - pos1.y);


        float angle = (Mathf.Atan(x / y) * 180) / Mathf.PI;
        nodeLightning.SetActive(true);
        Transform nodeLightningTf = nodeLightning.transform;
        nodeLightningTf.localPosition = pos1;

        if (pos2.y > pos1.y && pos2.x > pos1.x)
        {
        }
        else if (pos2.y > pos1.y && pos2.x < pos1.x)
        {
            angle = angle * -1;
        }
        else if (pos2.y < pos1.y && pos2.x > pos1.x)
        {
            angle = 180 - angle;
        }
        else if (pos2.y < pos1.y && pos2.x < pos1.x)
        {
            angle = 180 + angle;
        }

        if (pos2.y == pos1.y)
        {
            angle = pos2.x > pos1.x ? 90 : -90;
        }
        if (pos1.x == pos2.x)
        {
            angle = pos1.y > pos2.y ? -180 : 0;
        }
        float distance = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));

        nodeLightningTf.localScale = new Vector2(nodeLightningTf.localScale.x, distance / 200);
        nodeLightningTf.SetSiblingIndex((int)Globals.ZODER_VIEW.PLAYER + 1);
        nodeLightningTf.localEulerAngles = new Vector3(0, 0, -angle);

        animLightNing.skeletonDataAsset = dataAnimLightning;
        animLightNing.Initialize(true);
        animLightNing.AnimationState.SetAnimation(0, "animation", true);

        animLightNing.AnimationState.Complete += delegate
        {
            nodeLightning.SetActive(false);
            HandleGame.nextEvt();// case nay phai de lay evt dc vi roi vao case dc la cuoi roi finish luon.phai delay de dien chat bai xong roi moi dien finish .ok?
        };
    }
    public void handleDc(JObject data)
    {
        playSound(Globals.SOUND_GAME.CARD_FLIP_2);
        JArray cards = (JArray)data["arrC"];
        List<int> arrIds = cards.ToObject<List<int>>();
        Player player = getPlayerWithID((int)data["pid"]);
        setPlayerTurn(getPlayerWithID((int)data["idnext"]), 10);
        List<Card> listDrop = new List<Card>();
        if (player == thisPlayer)
        {
            checkButton(false);
            for (int i = 0; i < arrIds.Count; i++)
            {
                Card cardTemp = player.vectorCard.First(card => card.code == arrIds[i]);
                listDrop.Add(cardTemp);
                player.vectorCard.Remove(cardTemp);
                lastCardsDispatch.Add(cardTemp);
            }
        }
        else
        {
            for (int i = 0; i < arrIds.Count; i++)
            {
                if (player.vectorCard.Count > 0)
                {
                    Card cardTemp = player.vectorCard[player.vectorCard.Count - 1];
                    player.vectorCard.RemoveAt(player.vectorCard.Count - 1);
                    listDrop.Add(cardTemp);
                    cardTemp.setTextureWithCode(arrIds[i]);
                    lastCardsDispatch.Add(cardTemp);
                    if ((int)data["idnext"] == thisPlayer.id)
                        checkButton(true, 1, cardTemp.N, (int)data["idChop"]);
                }


            }
        }
        doCardDropEffect(listDrop, player, data);
        normalSortPlayerCard(player);

    }
    private void doCardDropEffect(List<Card> listDrop, Player player, JObject data)
    {

        PlayerViewKeang playerV = getPlayerView(player);
        playerV.cardOnTables.Clear();
        playerV.cardOnTables = listDrop;
        bool isPlayer = player == thisPlayer ? true : false;
        float scale = isPlayer ? PLAYERSCALE : OTHERSCALE * 1.4f;
        Vector2 tempPos = Vector2.zero;
        int delta = 1;
        int pIndex = getDynamicIndex(getIndexOf(player));

        if (pIndex == 0)
        {
            tempPos = new Vector2(0, -30);

        }
        else if (pIndex < 3)
        {
            tempPos.x = listPosCardTable[pIndex].x + 50;

            tempPos.y = listPosCardTable[pIndex].y + 50;

        }
        else
        {
            delta = -1;
            tempPos.x = listPosCardTable[pIndex].x - 50;

            tempPos.y = listPosCardTable[pIndex].y + 50;

        }
        string name = "panel" + pIndex;
        Transform check = table.transform.Find(name); //this.node.getChildByName(name)

        GameObject panel = Instantiate(dropCardPanel, table.transform);
        panel.name = name;

        panel.transform.localPosition = tempPos;
        int len = listDrop.Count;
        GameObject nodeCard = listDrop[0].transform.parent.gameObject;
        for (int i = 0; i < len; i++)
        {
            Card cardTemp = listDrop[i];
            Transform cardTempTf = cardTemp.transform;
            JObject cal = getCardDropPosition(i, len);
            Vector2 pos = cardTemp.transform.position;
            cardTempTf.SetParent(null);
            cardTempTf.SetParent(panel.transform);
            cardTempTf.SetSiblingIndex(i);
            cardTempTf.localPosition = Vector2.zero;
            cardTemp.showBorder(false);

            // cc.NGWlog('!==>??? do drop card for card', cardTemp.node)
            cardTemp.transform.localPosition = panel.transform.InverseTransformPoint(pos);
            Vector3[] bezier = new[] { cardTemp.transform.localPosition, new Vector3((float)cal["posX"] + 5.0f * delta, (float)cal["posY"], 0), new Vector3((float)cal["posX"], (float)cal["posY"], 0) };
            DOTween.Sequence()
                .Append(cardTempTf.DOLocalPath(bezier, 0.4f, PathType.Linear, PathMode.Ignore, 5))
                .Join(cardTempTf.DOLocalRotate(new Vector3(0, 0, (float)cal["angle"]), 0.3f).SetEase(Ease.OutCubic))
                .Join(cardTempTf.DOScale(new Vector2(scale, scale), 0.3f))
                .AppendInterval(0.1f)
                .Append(cardTempTf.DOScale(new Vector2(TABLESCALE, TABLESCALE), 0.3f).SetEase(Ease.InCubic))
                .AppendCallback(() =>
                {
                    cardTemp.showSmoke(true);
                })
                .Append(cardTempTf.DOScale(new Vector2(TABLESCALE + 0.025f, TABLESCALE + 0.025f), 0.15f).SetEase(Ease.OutCubic))
                .Append(cardTempTf.DOScale(new Vector2(TABLESCALE, TABLESCALE), 0.15f).SetEase(Ease.InCubic));
        }
        panel.transform.DOLocalMove(listPosCardTable[pIndex], 0.3f).SetDelay(0.4f).SetEase(Ease.InCubic);
        panel.transform.DOLocalRotate(listAngleCardTable[pIndex], 0.3f).SetDelay(0.4f).OnComplete(() =>
        {
            if (!(bool)data["isChop"])
            {
                HandleGame.nextEvt();
            }
            else
            {
                handleChop(data);
            }
            if (check) clearLastDropedCard(check.transform);
        });

    }
    private void handleChop(JObject data)
    {
        Globals.Logging.Log("!==> handle chop" + (int)data["agchopout"]);
        bool isChop = (bool)data["isChop"];
        int levelchop = (int)data["levechop"];
        int idNext = (int)data["idnext"];
        int idChopped = (int)data["idChop"];
        JArray cards = (JArray)data["arrC"];
        List<int> arrIds = cards.ToObject<List<int>>();
        //Globals.Logging.Log("Thang Bi Chat: Tien Before1:" + getPlayerWithID(idChopped).ag);
        PlayerViewKeang playerChopped = getPlayerView(getPlayerWithID(idChopped));// thang bi chat
        Globals.Logging.Log("playerChopped.cardOnTables:" + playerChopped.cardOnTables.Count);
        playerChopped.cardOnTables[playerChopped.cardOnTables.Count - 1].setFire(levelchop);
        //Globals.Logging.Log("Thang Bi Chat: Tien Before3:" + getPlayerWithID(idChopped).ag);
        getPlayerWithID(idChopped).ag += (int)data["agchopout"];
        //Globals.Logging.Log("Thang Bi Chat: Tien After:" + getPlayerWithID(idChopped).ag);
        getPlayerWithID(idChopped).setAg();
        playerChopped.effectFlyMoney((int)data["agchopout"]);
        int idChopper = (int)data["pid"];
        PlayerViewKeang playerChopper = getPlayerView(getPlayerWithID(idChopper));// thang chat
        getPlayerWithID(idChopper).ag += (int)data["agchopadd"];
        getPlayerWithID(idChopper).setAg();
        playerChopper.effectFlyMoney((int)data["agchopadd"]);
        Vector2 pos1 = listPosCardTable[getPlayerWithID(idChopper)._indexDynamic];
        Vector2 pos2 = listPosCardTable[getPlayerWithID(idChopped)._indexDynamic];
        showLightningEffect(pos1, pos2);
        //DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        //{
        //    Globals.Logging.Log("")
        //    HandleGame.nextEvt();
        //});
    }
    private void clearLastDropedCard(Transform parentNode)
    {
        if (parentNode == null) return;
        foreach (Transform cardTemp in parentNode)
        {
            if (cardTemp != null)
            {
                removerCard(cardTemp.GetComponent<Card>());
            }
        }
        Destroy(parentNode.gameObject);

    }
    private JObject getCardDropPosition(int indexCard, int len)
    {
        float angle, posX, posY, radian;
        float delta = (len - 1) > 0 ? 10 / (len - 1) : 0;
        float delta2 = (len - 1) > 0 ? 8 * (len - 1) : 0;
        angle = -5 + delta * indexCard;
        posX = 17 * indexCard - delta2;
        radian = Mathf.Abs((Mathf.PI / 180) * angle);
        posY = 10 - Mathf.Abs(posX * Mathf.Sin(radian));
        if (len <= 1) angle = 0;
        JObject posDrop = new JObject();

        posDrop["angle"] = -angle;
        posDrop["posX"] = posX;
        posDrop["posY"] = posY;
        return posDrop;
    }
    private void playerThrowChip(Player player, Vector2 posTo)
    {
        Globals.Logging.Log("playerThrowChip");
        int indexPlayer = getDynamicIndex(getIndexOf(player));
        PlayerViewKeang playerView = (PlayerViewKeang)player.playerView;
        for (int i = 0; i < 5; i++)
        {
            ChipBet chip = getChipBet(table.transform);
            chip.transform.localPosition = playerView.transform.localPosition;
            chip.init(0, 0.35f);
            chip.move(posTo, i * 0.1f, () =>
            {
                removerChip(chip);
            });
        }
    }
    private void setCardOnTable(Player player, List<int> arrIds)
    {

        List<Card> listDrop = new List<Card>();
        for (int i = 0; i < arrIds.Count; i++)
        {
            Card card = getCard(table.transform, TABLESCALE);
            listDrop.Add(card);
            lastCardsDispatch.Add(card);
            card.setTextureWithCode(arrIds[i]);
        }
        int pIndex = getDynamicIndex(getIndexOf(player));

        string name = "panel" + pIndex;
        GameObject panel = Instantiate(dropCardPanel, table.transform);

        panel.name = name;
        getPlayerView(player).cardOnTables = listDrop;
        int len = listDrop.Count;
        for (int i = 0; i < len; i++)
        {
            Card cardTemp = listDrop[i];
            Transform cardTempTf = cardTemp.transform;
            JObject cal = getCardDropPosition(i, len);
            cardTempTf.SetParent(null);
            cardTempTf.SetParent(panel.transform);
            cardTempTf.SetSiblingIndex(i);
            cardTempTf.localPosition = Vector2.zero;
            cardTemp.showBorder(false);
            cardTemp.transform.localPosition = new Vector2((float)cal["posX"], (float)cal["posY"]);
            cardTemp.transform.localScale = new Vector2(TABLESCALE, TABLESCALE);
        }
        panel.transform.localPosition = listPosCardTable[pIndex];
        panel.transform.localEulerAngles = listAngleCardTable[pIndex];
    }

    private void shortCardOnHand(Player player, List<int> arrIds = null, bool isRunEffect = true)
    {
        PlayerViewKeang playerV = getPlayerView(player);
        float initAngle = 0;
        float cardScale = 0.5f;
        Vector2 playerPos = playerV.transform.localPosition;
        Vector2 initCardPos = new Vector2(playerPos.x - 63, playerPos.y + 50);
        if (player == thisPlayer)
        {
            initCardPos = new Vector2(playerPos.x + 200, playerPos.y - 60);
            cardScale = 1.0f;
        }
        List<Card> listCard = playerV.cards;
        for (int i = 0, l = listCard.Count; i < l; i++)
        {

            Vector2 cardPos = new Vector2(initCardPos.x + (i * 30) * (cardScale / 0.5f), initCardPos.y);
            Card card = listCard[i];
            Vector3 cardAngle = new Vector3(0, 0);
            if (l % 2 != 0)
            {
                cardPos = new Vector2(cardPos.x, initCardPos.y - (Mathf.Abs(i - l / 2) * 10) * (cardScale / 0.5f));
                if (i == l / 2)
                {
                    cardPos = new Vector2(cardPos.x, cardPos.y - 5 * (cardScale / 0.5f));
                }
                cardAngle = new Vector3(0, 0, initAngle + (l / 2 - i) * 10);
            }
            else
            {
                //initAngle = (5 * l)/2;
                if (i >= l / 2)
                {
                    cardAngle = new Vector3(0, 0, initAngle - (i + 1 - l / 2) * 5);
                    cardPos = new Vector2(cardPos.x, initCardPos.y - (i + 1 - l / 2) * 6 * (cardScale / 0.5f));
                }
                else
                {
                    cardAngle = new Vector3(0, 0, initAngle + (l / 2 - i) * 5);
                    cardPos = new Vector2(cardPos.x, initCardPos.y - (l / 2 - i) * 6 * (cardScale / 0.5f));
                }

            }
            float timeEffect = 0.5f;
            if (!isRunEffect)
            {
                timeEffect = 0;
            }
            card.transform.DOScale(new Vector2(cardScale, cardScale), timeEffect).SetEase(Ease.OutBack);
            card.transform.DOLocalRotate(cardAngle, timeEffect).SetEase(Ease.OutBack);
            card.transform.DOLocalMove(cardPos, timeEffect).SetEase(Ease.InSine);
            if (player == thisPlayer && arrIds != null)
            {
                card.setTextureWithCode(arrIds[i]);
            }

        }
    }
    private void playerShowCard(Player player, List<int> arrId)
    {
        PlayerViewKeang playerV = getPlayerView(player);
        List<Card> arrCards = playerV.cards;
        float initAngle = 0;
        Vector2 playerPos = playerV.transform.localPosition;
        Vector2 initCardPos = new Vector2(playerPos.x - 63, playerPos.y + 50);
        if (player == thisPlayer)
        {
            initCardPos = new Vector2(playerPos.x + 200, playerPos.y - 60);
        }
        for (int i = 0; i < arrCards.Count; i++)
        {
            Card card = arrCards[i];
            //card.transform.localScale = new Vector2(initAngle, initAngle);
            card.transform.localPosition = initCardPos;
            card.transform.localEulerAngles = new Vector3(0, 0, initAngle);
            card.setTextureWithCode(arrId[i]);
        }
        shortCardOnHand(player);
    }
    private PlayerViewKeang getPlayerView(Player player)
    {
        if (player != null)
        {
            return (PlayerViewKeang)player.playerView;
        }
        return null;
    }
    public void handleStartGame(JObject data)
    {
        playSound(Globals.SOUND_GAME.ALL_IN);
        lbStateGame.gameObject.SetActive(false);
        int timeCountDown = (int)data["time"];
        lbGameTime.gameObject.SetActive(true);
        lbGameTime.text = timeCountDown.ToString();
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                if (timeCountDown == 1)
                {
                    lbGameTime.gameObject.SetActive(false);
                    seq.Kill();
                }
                //playSound(Globals.SOUND_GAME.CLOCK_TICK);
                timeCountDown--;
                lbGameTime.text = timeCountDown.ToString();
                lbGameTime.transform.DOScale(0f, 0);
                lbGameTime.transform.DOScale(0.75f, 1.0f).SetEase(Ease.OutBack);

            })
            .SetLoops(timeCountDown);
    }

    public void handlePot(JObject data)
    {
        playSound(Globals.SOUND_GAME.BET);
        agPot = (int)data["Pot"];
        Globals.Config.tweenNumberTo(lbPotTotal, agPot);
        foreach (Player player in players)
        {
            Vector2 posTo = table.transform.InverseTransformPoint(nodePot.transform.Find("icChip").transform.position);
            playerThrowChip(player, posTo);
            PlayerViewKeang playerV = getPlayerView(player);
            if (playerV != null)
            {
                playerV.effectFlyMoney(-(int)data["agPot"]);
                player.ag -= (int)data["agPot"];
                player.setAg();
            }
        }
    }
    private void setPlayerTurn(Player player, int time)
    {
        foreach (Player pl in players)
        {
            pl.setTurn(false);
        }
        currentPlayer = player;
        if (player != thisPlayer)
            checkButton(false);
        player.setTurn(true, time);

    }
    private void checkButton(bool isShow = false, int isBoc = 1, int cardName = -1, int idChop = 0)
    {
        if (!isShow || currentPlayer != thisPlayer)
        {
            nodeButton.SetActive(false);
            return;
        }
        nodeButton.SetActive(true);
        // case can match
        List<Card> isMatch = new List<Card>();
        List<int> listActive = new List<int> { 0 };
        canMatchArray.Clear();
        if (cardName != -1) isMatch = checkMatch(cardName);
        listActive.Add(isBoc);

        if (isBoc == 2)
        {
            //listActive.shift();
            listActive.RemoveAt(0);
        }
        if (isBoc == 1)
        {
            buttonManager.allowPush(1, true);
        }

        if (isMatch.Count > 0 && idChop != this.thisPlayer.id)
        {
            listActive.Add(3);
            autoPickCardsHint();
        }
        buttonManager.showButton(listActive);
        checkAllowButton();
    }
    private List<Card> checkMatch(int cardName = -1)
    {
        List<Card> result = new List<Card>();
        if (cardName == -1) return result;
        List<Card> vectorCard = thisPlayer.vectorCard;
        canMatchArray = new List<Card>();
        for (int i = 0; i < vectorCard.Count; i++)
        {
            if (vectorCard[i].N == cardName)
            {
                result.Add(vectorCard[i]);
                canMatchArray.Add(vectorCard[i]);

            }
        }
        if (this.canMatchArray.Count > 1)
        {
            sortCardS(canMatchArray);
            sortCardN(canMatchArray);
        }
        return result;
    }
    private List<Card> sortCardN(List<Card> listCard)
    {
        listCard.Sort((x, y) =>
        {
            return x.N - y.N;
        });
        return listCard;

    }
    private List<Card> sortCardS(List<Card> listCard)
    {
        listCard.Sort((x, y) =>
        {
            return x.S - y.S;
        });
        return listCard;
    }
    private void autoPickCardsHint()
    {
        List<Card> vectorCard = thisPlayer.vectorCard;
        if (canMatchArray.Count > 0)
        {
            handleCardTouch(canMatchArray[0], true);
            return;
        }
        List<Card> uniqueVectorCard = new List<Card>();
        foreach (Card cardTemp in vectorCard)
        {
            if (!uniqueVectorCard.Find(card => card.N == cardTemp.N))
            {
                uniqueVectorCard.Add(cardTemp);
            }
        }

        Card max = uniqueVectorCard[0];
        List<Card> arrayMax = vectorCard.FindAll(card => card.N == max.N);

        foreach (Card cardTemp in uniqueVectorCard)
        {
            List<Card> arrayNormal = vectorCard.FindAll(card => card.N == cardTemp.N);

            int pointMax = getPlayerPoint(arrayMax);
            int pointNormal = getPlayerPoint(arrayNormal);
            if (pointNormal > pointMax)
            {
                max = cardTemp;
                arrayMax = vectorCard.FindAll(card => card.N == max.N);

            }
            else if (pointNormal == pointMax)
            {
                if (arrayNormal.Count > arrayMax.Count)
                {
                    max = cardTemp;
                    arrayMax = vectorCard.FindAll(card => card.N == max.N);

                }
                else if (arrayNormal.Count == arrayMax.Count)
                {
                    if (cardTemp.N >= max.N)
                    {
                        max = cardTemp;
                        arrayMax = vectorCard.FindAll(card => card.N == max.N);
                    }
                }
            }
        }

        Globals.Logging.Log("!==> do auto pick normal" + max);

        handleCardTouch(max, true);

    }
    private int getPlayerPoint(List<Card> arrayCard)
    {
        int point = 0;
        foreach (Card cardTemp in arrayCard)
        {
            int cardValue = cardTemp.N <= 10 ? cardTemp.N : 10;
            point += cardValue;
        }
        return point;
    }
    private void handleCardTouch(Card cardTemp, bool isOverWrite = false)
    {
        List<Card> vectorCard = thisPlayer.vectorCard;
        int indexCard = vectorCard.IndexOf(cardTemp);

        if (indexCard == -1 || cardTemp.code == 0) return;
        if (!cardTemp)
        {
            return;
        }
        bool isSelected = selectedCards.IndexOf(cardTemp) != -1 ? true : false;
        if (isSelected && isOverWrite == false)
        {
            handleUnselectCard(cardTemp);
        }
        else
        {
            handleSelectCard(cardTemp);
            autoSelectSameCards(cardTemp);
        }
        checkAllowButton();
    }
    private void autoSelectSameCards(Card _cardTemp)
    {
        int uniCode = _cardTemp.N;
        int len = thisPlayer.vectorCard.Count;

        foreach (Card cardTemp in thisPlayer.vectorCard)
        {
            if (cardTemp != _cardTemp && cardTemp.N == uniCode)
            {
                handleSelectCard(cardTemp);
            }
            else if (cardTemp != _cardTemp && cardTemp.N != uniCode && selectedCards.IndexOf(cardTemp) != -1)
            {
                handleUnselectCard(cardTemp);
            }
        }

    }
    private void checkAllowButton()
    {
        bool isAllow;
        if (canMatchArray.Count > 0)
        {
            Globals.Logging.Log("!==> check allow match");

            if (selectedCards.Count > 0)
            {
                isAllow = this.compareListCard(canMatchArray, selectedCards);
                buttonManager.allowPush(3, isAllow);
            }
            else
            {
                buttonManager.allowPush(3, false);
            }
        }
        else
        {
            Globals.Logging.Log("!==> check allow normal");

            isAllow = selectedCards.Count > 0 ? true : false;
            buttonManager.allowPush(2, isAllow);
        }
    }
    private bool compareListCard(List<Card> arr1, List<Card> arr2)
    {
        if (arr2.Count > arr1.Count) return false;
        for (int i = 0; i < arr2.Count; i++)
        {
            if (arr1.IndexOf(arr2[i]) < 0)
            {
                return false;
            }
        }
        return true;
    }
    private void handleSelectCard(Card cardTemp)
    {
        if (selectedCards.IndexOf(cardTemp) != -1) return;
        JObject orgPos = getCardPosition(thisPlayer.vectorCard.IndexOf(cardTemp), thisPlayer.vectorCard.Count);

        float radian = Mathf.Abs((Mathf.PI / 180) * (float)orgPos["angle"]);
        int isRevert = (float)orgPos["posX"] < 0 ? -1 : 1;
        float posX = (30 * Mathf.Sin(radian)) * isRevert + (float)orgPos["posX"];
        float posY = (30 * Mathf.Cos(radian)) + (float)orgPos["posY"];
        cardTemp.isAllowTouch = true;
        //cardTemp.gameObject.stopAllActions();
        cardTemp.showBorder(true);
        selectedCards.Add(cardTemp);
        cardTemp.transform.DOLocalMove(new Vector2(posX, posY), 0.1f).SetEase(Ease.OutCubic);
    }
    private void handleUnselectCard(Card cardTemp)
    {
        JObject orgPos = getCardPosition(thisPlayer.vectorCard.IndexOf(cardTemp), thisPlayer.vectorCard.Count);

        selectedCards.RemoveAt(selectedCards.IndexOf(cardTemp));
        //cardTemp.node.stopAllActions();
        cardTemp.showBorder(false);
        //cardTemp.gam.runAction(cc.moveTo(0.1, orgPos.pos).easing(cc.easeCubicActionOut()));
        cardTemp.transform.DOLocalMove(new Vector2((float)orgPos["posX"], (float)orgPos["posY"]), 0.1f).SetEase(Ease.OutCubic);
    }
    private JObject getCardPosition(int indexCard, int len, bool isPlayer = true)
    {
        float angle, posX, posY, radian;
        int delta1 = (len - 1) > 0 ? (30 / (len - 1)) : 0;
        int delta2 = (len - 1) > 0 ? (40 / (len - 1)) : 0;
        if (!isPlayer)
        {
            angle = -15 + delta1 * indexCard;
            posX = 18 * indexCard - 9 * (len - 1);
            radian = Mathf.Abs((Mathf.PI / 180) * angle);
            posY = -Mathf.Abs(posX * Mathf.Sin(radian));
        }
        else
        {
            angle = -20 + delta2 * indexCard;
            posX = 50 * indexCard - 25 * (len - 1);
            radian = Mathf.Abs((Mathf.PI / 180) * angle);
            posY = -Mathf.Abs(posX * Mathf.Sin(radian));

        }
        if (len <= 3) angle = angle / 2;
        if (len <= 1) angle = 0;
        JObject cardPosObj = new JObject();
        cardPosObj["angle"] = -angle;
        cardPosObj["posX"] = posX;
        cardPosObj["posY"] = posY;
        return cardPosObj;
    }
    private int findTouchedCard(PointerEventData eventData)
    {
        var len = thisPlayer.vectorCard.Count;
        for (var i = len - 1; i >= 0; i--)
        {
            var cardTemp = thisPlayer.vectorCard[i];
            if (Globals.Config.checkContainBoundingBox(cardTemp.gameObject, eventData))
            {
                return i;
            }
        }
        return -1;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        idTouch = findTouchedCard(eventData);
        Globals.Logging.Log("Id Touch:" + idTouch);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
    public override void onCardClick(Card card)
    {
        base.onCardClick(card);
        handleCardTouch(card);

    }

}
