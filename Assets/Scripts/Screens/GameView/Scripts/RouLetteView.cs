using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Globals;
using DG.Tweening;
using Spine.Unity;
using System.Linq;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// using Newtonsoft.Json;
// using Random = UnityEngine.Random;

public class RouLetteView : GameView
{

    [SerializeField] private List<BetOptionsRoulette> listBetOptions;
    [SerializeField] private List<BetButtonRoulette> listBetButtonRoulette;
    [SerializeField] private List<ResultHistory> listResultHistoryInPopup;
    [SerializeField] private Button buttonSpine;
    [SerializeField] private Image imageSpin, imageBall, imagePopupHistory;
    [SerializeField] private SkeletonGraphic animResult, animShowSo;
    [SerializeField] private TextMeshProUGUI textResult, textNumWin, textNumLose, textPercentRed, textPercentBlack;
    [SerializeField] private ChipBetRouLette chipBet;
    [SerializeField] private ResultHistory resultHistory;
    [SerializeField] private Button buttonDouble, buttonDeal, buttonClear, buttonHistory, buttonCloseHistory, buttonRebet;
    [SerializeField] private TextMeshProUGUI textFrameCoin_1, textFrameCoin_2, textFrameCoin, textNumMoney, textNumDeal;
    [SerializeField] private Transform transformResult;
    [SerializeField] private RectTransform transformTabResult, _rectTransformButtonMenu, table_1, table_2;

    HashSet<int> redNumbers = new HashSet<int>
        { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

    private int[] arrWin;
    private long currentBet = 10000;
    private ChipBetRouLette chip;
    private List<BetData> listDataBet = new List<BetData>();
    private Dictionary<List<int>, int> specialBets = new Dictionary<List<int>, int>(new ListComparer())
    {
        { new List<int> { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }, 37 },
        { new List<int> { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }, 38 },
        { new List<int> { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }, 39 },
        { Enumerable.Range(1, 12).ToList(), 40 },
        { Enumerable.Range(13, 12).ToList(), 41 },
        { Enumerable.Range(25, 12).ToList(), 42 },
        { Enumerable.Range(1, 18).ToList(), 43 },
        { Enumerable.Range(1, 36).Where(n => n % 2 == 0).ToList(), 44 },
        { new List<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }, 45 },
        { new List<int> { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 }, 46 },
        { Enumerable.Range(1, 36).Where(n => n % 2 != 0).ToList(), 47 },
        { Enumerable.Range(19, 18).ToList(), 48 }
    };
    private List<BetData> listDataBetForRebet = new List<BetData>();
    private List<BetData> listDataBetForRebetTemp = new List<BetData>();
    private Dictionary<long, long> newDataBetDeal = new Dictionary<long, long>();
    public List<int> listResultHistory = new List<int>();
    public List<Vector2> listPosBallEnd;
    // private List<(int betType, int[] numArr, long betAmount)> listBetValue;
    private int result = 1;
    private int currenIdBet;
    private long totalBetValue;
    private bool isClickRebet = false;
    private long totalBetDeal, agWin, agPlayer;
    public long agRemaining;
    public static RouLetteView instance = null;
    public bool isBetTime = false;

    [Serializable]
    public class BetData
    {
        public int IdBet { get; set; }
        public int BetType { get; set; }
        public List<int> NumArr { get; set; }
        public long BetAmount { get; set; }

        public BetData(int idBet, int betType, List<int> numArr, long betAmount)
        {
            IdBet = idBet;
            BetType = betType;
            NumArr = numArr;
            BetAmount = betAmount;
        }
    }

    void Start()
    {
        isBetTime = false;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        for (int i = 0; i < listBetOptions.Count; i++)
        {
            listBetOptions[i].id = i;
        }

        for (int i = 0; i < listBetButtonRoulette.Count; i++)
        {
            listBetButtonRoulette[i].id = i;
        }

        for (int i = 0; i < listResultHistoryInPopup.Count; i++)
        {
            listResultHistoryInPopup[i].id = i;
        }
        agRemaining = thisPlayer.ag;
        buttonSpine.onClick.AddListener(clickButtonSpin);
        buttonDouble.onClick.AddListener(ClickButtonDouble);
        buttonRebet.onClick.AddListener(ClickButtonRebet);
        buttonDeal.onClick.AddListener(ClickButtonDeal);
        buttonClear.onClick.AddListener(ClickButtonClear);
        buttonHistory.onClick.AddListener(ClickButtonHistory);
        buttonCloseHistory.onClick.AddListener(ClickButtonCloseHistory);
    }

    protected override void Awake()
    {
        base.Awake();
        stateGame = STATE_GAME.VIEWING;
        instance = this;
    }

    public void ProcessResponseData(JObject jData)
    {
        Debug.Log($"Tinh_Evt_DataStart: {jData}");
        switch ((string)jData["evt"])
        {
            case "timeToStart":
                HandleStartGame(jData);
                Debug.Log($"Tinh_Evt_DataStart: {jData}");
                break;
            case "ctable":
                handleCTable(jData.ToString());
                break;
            case "stable":
                handleSTable(jData.ToString());
                break;
            case "vtable":
                handleVTable(jData.ToString());
                break;
            case "jtable":
                handleJTable(jData.ToString());
                break;
            case "rjtable":
                handleRJTable(jData.ToString());
                break;
            case "ltable":
                handleLTable(jData);
                break;
            case "chattable":
                handleChatTable(jData);
                break;
            case "autoExit":
                handleAutoExit(jData);
                break;

            case "spin":
                HandleSpin(jData);
                break;
            case "make_bet":
                HandleMakeBet(jData);
                break;
            case "finish":
                HandleFinishGame(jData);
                break;
        }
    }

    protected override void updatePositionPlayerView()
    {
        int countLoop = Mathf.Min(players.Count, listPosView.Count);
        for (int i = 0; i < countLoop; i++)
        {
            players[i].playerView.transform.localPosition = listPosView[i];
            players[i].updateItemVip(players[i].vip);
        }
    }

    public override void handleCTable(string strData)
    {
        base.handleCTable(strData);
        // Debug.Log($"Tinh_Evt_ctable: {strData}---agTable: {agTable}");
        thisPlayer.playerView.transform.localScale = Vector3.one;
    }

    public override void handleCCTable(JObject data)
    {
        base.handleCCTable(data);
    }

    public override void handleVTable(string strData)
    {
        stateGame = STATE_GAME.VIEWING;
        JObject data = JObject.Parse(strData);
        setGameInfo(m: (int)data["M"], id: (int)data["Id"],
            maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        for (int i = 0; i < players.Count; i++)
            if (players[i].playerView != null)
                Destroy(players[i].playerView.gameObject);
        players.Clear();
        thisPlayer = new()
        {
            playerView = createPlayerView(),
            id = User.userMain.Userid,
            namePl = User.userMain.Username,
            displayName = User.userMain.displayName,
            ag = User.userMain.AG,
            vip = User.userMain.VIP,
            avatar_id = User.userMain.Avatar,
            is_ready = true,
        };
        thisPlayer.fid = User.userMain.Tinyurl.IndexOf("fb.") != -1
            ? User.userMain.Tinyurl.Substring(3)
            : thisPlayer.fid;
        thisPlayer.updatePlayerView();
        thisPlayer.playerView.setDark(true);
        players.Add(thisPlayer);
        JArray listPlayer = (JArray)data["ArrP"];
        int countShowAvatars = Mathf.Min(listPlayer.Count, listPosView.Count - 1); //trừ chỗ cho user 
        for (int i = 0; i < listPlayer.Count; i++)
        {
            Player player = new();
            readDataPlayer(player, (JObject)listPlayer[i]);
            player.setHost(i == 0);
            if (i < countShowAvatars)
            {
                player.playerView = createPlayerView();
                player.updatePlayerView();
            }

            players.Add(player);
        }

        updatePositionPlayerView();
        for (int i = 0; i < listPlayer.Count; i++)
        {
            if (i < countShowAvatars)
            {
                JArray bets = (JArray)listPlayer[i]["ArrCuoc"];
                foreach (JToken bet in bets)
                {
                    Player p = players.Find(x => x.id == (int)listPlayer[i]["id"]);
                }
            }
        }

        int timeLeft = (int)data["timeLeft"];
    }

    public override void handleSTable(string strData)
    {
        stateGame = STATE_GAME.WAITING;
        JObject data = JObject.Parse(strData);
        JArray dataWinHistory = (JArray)data["boxWinHistory"];
        setGameInfo(m: (int)data["M"], id: (int)data["Id"],
            maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        for (int i = 0; i < players.Count; i++)
            if (players[i].playerView != null)
                Destroy(players[i].playerView.gameObject);
        // có trường hợp đang chạy handleVTable chưa xong do async mà chạy luôn handleSTable nên phải giữ lại cái sabongbetchips
        List<Tuple<int, Dictionary<int, List<Transform>>>> tempList = new();
        foreach (Player player in players) tempList.Add(new(player.id, player.sabongBetChips));
        players.Clear();
        JArray listPlayer = (JArray)data["ArrP"];
        int count = 0,
            countShowAvatars =
                Mathf.Min(listPlayer.Count, listPosView.Count) - 1; //trừ chỗ cho user là listPlayer.Last()
        for (int i = 0; i < listPlayer.Count; i++)
        {
            Player player = new();
            readDataPlayer(player, (JObject)listPlayer[i]);
            player.is_ready = true;
            player.setHost(i == 0);
            bool isShowAvatar = count + 1 <= countShowAvatars, isThisUser = player.id == User.userMain.Userid;
            if (isShowAvatar || isThisUser)
            {
                player.playerView = createPlayerView();
                player.playerView.setDark(false);
                player.updatePlayerView();
                if (isShowAvatar) count++;
            }

            if (isThisUser)
            {
                thisPlayer = player;
                players.Insert(0, thisPlayer);
            }
            else players.Add(player);
        }

        foreach (Player player in players)
            foreach (Tuple<int, Dictionary<int, List<Transform>>> item in tempList)
                if (player.id == item.Item1)
                    player.sabongBetChips = item.Item2;
        thisPlayer.playerView.transform.localScale = Vector3.one;
        updatePositionPlayerView();
    }

    public override void handleJTable(string strData)
    {
        JObject listPlayer = JObject.Parse(strData);
        Player player = new();
        readDataPlayer(player, listPlayer);
        if (players.Count < listPosView.Count)
        {
            player.playerView = createPlayerView();
            player.updatePlayerView();
        }

        players.Add(player);
        updatePositionPlayerView();
    }

    public override void setGameInfo(int m, int id = 0, int maxBett = 0)
    {
        base.setGameInfo(m, id, maxBett);
        if (listBetButtonRoulette.Count > 0) return;
    }

    public override void handleRJTable(string strData)
    {
        stateGame = STATE_GAME.PLAYING;
        JObject data = JObject.Parse(strData);
        JArray dataWinHistory = (JArray)data["boxWinHistory"];
        setGameInfo(m: (int)data["M"], id: (int)data["Id"],
            maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        for (int i = 0; i < players.Count; i++)
            if (players[i].playerView != null)
                Destroy(players[i].playerView.gameObject);
        players.Clear();
        JArray listPlayer = (JArray)data["ArrP"];
        int count = 0, countShowAvatars = Mathf.Min(listPlayer.Count, listPosView.Count);
        for (int i = 0; i < listPlayer.Count; i++)
        {
            Player playerP = new();
            readDataPlayer(playerP, (JObject)listPlayer[i]);
            playerP.is_ready = true;
            playerP.setHost(i == 0);
            if (playerP.id == User.userMain.Userid)
            {
                thisPlayer = playerP;
                players.Insert(0, thisPlayer);
            }
            else players.Add(playerP);
        }

        foreach (Player playerP in players)
        {
            if (count + 1 <= countShowAvatars)
            {
                playerP.playerView = createPlayerView();
                playerP.playerView.setDark(false);
                playerP.updatePlayerView();
                count++;
            }
        }

        thisPlayer.playerView.transform.localScale = Vector3.one;
        updatePositionPlayerView();
        for (int i = 0; i < listPlayer.Count; i++)
        {
            JArray bets = (JArray)listPlayer[i]["ArrCuoc"];
            foreach (JToken bet in bets)
            {
                Player player = players.Find(x => x.id == (int)listPlayer[i]["id"]);
            }
        }

        int timeLeft = (int)data["timeLeft"];
    }

    public override void handleLTable(JObject data)
    {
        string namePl = (string)data["Name"];
        Player player = getPlayer(namePl);
        if (player == null) return;
        if (player != thisPlayer)
        {
            players.Remove(player);
            if (player.playerView != null) Destroy(player.playerView.gameObject);
            for (int i = 0; i < players.Count; i++)
            {
                if (i < listPosView.Count && players[i].playerView == null)
                {
                    players[i].playerView = createPlayerView();
                    players[i].setDark(false);
                    players[i].updatePlayerView();
                }
            }

            updatePositionPlayerView();
        }
    }

    public void HandleFinishGame(JObject data)
    {
        isBetTime = false;
        HandleData.DelayHandleLeave = 10f;
        int result = data["result"]?.Value<int>() ?? 0;
        this.result = result;
        listResultHistory.Insert(0, result);
        Debug.Log($"TinhListResultHistory: [{string.Join(", ", listResultHistory)}]");
        string rawData = data["data"]?.Value<string>();
        if (!string.IsNullOrEmpty(rawData))
        {
            JArray jsonArray = JArray.Parse(rawData);
            if (jsonArray.Count > 0)
            {
                JObject playerData = (JObject)jsonArray[0];
                int agWin = playerData["agWin"]?.Value<int>() ?? 0;
                this.agWin = agWin;
                int agPlayer = playerData["ag"]?.Value<int>() ?? 0;
                this.agPlayer = agPlayer;
                Debug.Log($"tinh=> agWin:{this.agWin}//agPLayer:{this.agPlayer}");
            }
        }

        for (int i = 0; i < listResultHistory.Count; i++)
        {
            listResultHistoryInPopup[i].imageResult.gameObject.SetActive(true);
            if (listResultHistory[i] == 0)
            {
                listResultHistoryInPopup[i].imageResult.sprite = listResultHistoryInPopup[i].images[0];
            }
            else
            {
                if (redNumbers.Contains(listResultHistory[i]))
                {
                    listResultHistoryInPopup[i].imageResult.sprite = listResultHistoryInPopup[i].images[1];
                }
                else
                {
                    listResultHistoryInPopup[i].imageResult.sprite = listResultHistoryInPopup[i].images[2];
                }
            }

            listResultHistoryInPopup[i].textResult.text = $"{listResultHistory[i]}";
        }
        int nonZeroCount = listResultHistory.Count(num => num != 0);
        int x = listResultHistory.Count(num => num != 0 && redNumbers.Contains(num));
        float percentRed = nonZeroCount > 0 ? (float)x / nonZeroCount : 0;
        float percentBlack = nonZeroCount > 0 ? 100 - (percentRed * 100) : 0;

        textPercentRed.text = $"{percentRed * 100:0}%";
        textPercentBlack.text = $"{percentBlack:0}%";
    }

    public void HandleMakeBet(JObject data)
    {
        Debug.Log($"{data}");
    }

    public void HandleSpin(JObject data)
    {
        Debug.Log($"Tinh_Spin");
    }

    public void HandleStartGame(JObject data)
    {
        isBetTime = true;
        playSound(SOUND_GAME.START_GAME);
        stateGame = STATE_GAME.WAITING;
        _OnStartGame();
    }

    private void clickButtonSpin()
    {
        playSound(SOUND_GAME.CLICK);
        for (int i = 0; i < listBetOptions.Count; i++)
        {
            listBetOptions[i].buttonBetOption.interactable = false;
        }
        buttonHistory.interactable = false;
        Tweener tweener = null;
        tweener = table_1.DOAnchorPosX(1280, 1).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            tweener.Kill();
            RotateSpinAndBall();
        });
        table_2.DOAnchorPosX(0, 1).SetEase(Ease.InOutQuad);
        _rectTransformButtonMenu.DOAnchorPosX(240, 0.5f);
        transformTabResult.DOAnchorPosX(-232, 0.75f).SetEase(Ease.InOutQuad);
        SocketSend.sendSpinRoulette();
        listDataBetForRebetTemp.Clear();
        listDataBetForRebetTemp.AddRange(listDataBetForRebet);
        listDataBetForRebet.Clear();
        // listDataBet.Clear();
    }

    private void ReStartGame()
    {
        buttonRebet.interactable = true;
        newDataBetDeal.Clear();
        listDataBet.Clear();
        totalBetDeal = 0;
        textFrameCoin.text = $"{0}";
        textFrameCoin_1.text = $"{0}";
        textFrameCoin_2.text = $"{0}";
        thisPlayer.ag = agPlayer;
        thisPlayer.setAg();
        UpdateButtonBet(thisPlayer.ag);
        buttonDouble.interactable = false;
        Debug.Log(listBetOptions.Count + "xóa sạch bàn chơi");
        for (int i = 0; i < listBetOptions.Count; i++)
        {
            listBetOptions[i].buttonBetOption.interactable = true;
        }
        animShowSo.gameObject.SetActive(false);
        foreach (var betOption in listBetOptions)
        {
            int id = listBetOptions.IndexOf(betOption);
            int childCount = betOption.transform.childCount;

            // Xử lý riêng cho các ô từ 0-48 và ô còn lại
            if (id < 49)
            {
                if (childCount > 1)
                {
                    for (int i = childCount - 1; i >= 1; i--)
                    {
                        ClearChip(betOption.transform.GetChild(i));
                    }
                }
            }
            else
            {
                if (childCount > 0)
                {
                    for (int i = childCount - 1; i >= 0; i--)
                    {
                        ClearChip(betOption.transform.GetChild(i));
                    }
                }
            }
        }
    }

    private void ClearChip(Transform chipTransform)
    {
        ChipBetRouLette chip = chipTransform.GetComponent<ChipBetRouLette>();
        if (chip != null)
        {
            CanvasGroup canvasGroup = chipTransform.gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = chipTransform.gameObject.AddComponent<CanvasGroup>();
            }

            chipTransform.DOScale(Vector3.one, 0.5f);
            chipTransform.DOLocalMove(new Vector3(0, 32, 0), 0.5f);
            canvasGroup.DOFade(0, 1f).OnComplete(() => Destroy(chipTransform.gameObject));
        }
    }

    private void ClickButtonRebet()
    {
        isClickRebet = true;

        playSound(SOUND_GAME.CLICK);
        Debug.Log(totalBetValue+" xem chỗnayf");
        Debug.Log(listDataBetForRebetTemp.Sum(bet => bet.BetAmount)+" "+listDataBetForRebetTemp.Count);
        totalBetValue = listDataBetForRebetTemp.Sum(bet => bet.BetAmount)+totalBetValue;
        textFrameCoin.text = Globals.Config.FormatMoney(totalBetValue+totalBetDeal, true);
        textFrameCoin_1.text = textFrameCoin_2.text = Globals.Config.FormatMoney(totalBetValue, true);
        buttonDeal.interactable = true;
        buttonClear.interactable = true;
        buttonDouble.interactable = true;

        listDataBet.AddRange(listDataBetForRebetTemp);

        foreach (var bet in listDataBetForRebetTemp)
        {
            if (specialBets.TryGetValue(bet.NumArr, out int betOption))
            {
                var chipClone = Instantiate(chipBet, listBetOptions[betOption].transform);
                chipClone.Init(currenIdBet, bet.BetAmount);
            }
            else
            {
                var chipClone = Instantiate(chipBet, listBetOptions[bet.IdBet].transform);
                chipClone.Init(currenIdBet, bet.BetAmount);
            }
        }
    }

    private void ClickButtonDeal()
    {
        playSound(SOUND_GAME.CLICK);
 
        buttonDeal.interactable = false;
        buttonClear.interactable = false;
        totalBetDeal += totalBetValue;
        textFrameCoin_1.text = textFrameCoin_2.text = "0";
        textFrameCoin.text = Globals.Config.FormatMoney(totalBetDeal, true);
        ShowTextNumDeal();
        foreach (var betData in listDataBet)
        {
            foreach (var id in betData.NumArr)
            {
                if (newDataBetDeal.ContainsKey(id))
                {
                    newDataBetDeal[id] += betData.BetAmount;
                }
                else
                {
                    newDataBetDeal[id] = betData.BetAmount;
                }
            }
        }
        string jsonDataBet = JsonConvert.SerializeObject(listDataBet, Formatting.Indented,
          new JsonSerializerSettings
          {
              ContractResolver = new CamelCasePropertyNamesContractResolver()
          });
        foreach (var bet in listDataBet)
        {
            var existingBet = listDataBetForRebet
                .FirstOrDefault(b => b.BetType == bet.BetType && b.NumArr.SequenceEqual(bet.NumArr));

            if (existingBet != null)
            {
                existingBet.BetAmount += bet.BetAmount;
            }
            else
            {
                listDataBetForRebet.Add(new BetData(bet.IdBet, bet.BetType, new List<int>(bet.NumArr), bet.BetAmount));
            }
        }
        foreach (var betOption in listBetOptions)
        {
            foreach (Transform child in betOption.transform)
            {
                ChipBetRouLette chip = child.GetComponent<ChipBetRouLette>();
                if (chip != null)
                {
                    chip.isDealed = true;
                }
            }
        }
        SocketSend.sendBetRoulette(jsonDataBet);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            thisPlayer.ag -= totalBetValue;
            totalBetValue = 0;
            thisPlayer.setAg();
            jsonDataBet = JsonConvert.SerializeObject(listDataBet, Formatting.Indented,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            Debug.Log($"jsonString: {jsonDataBet}");
        });
    }

    private void ShowTextNumDeal()
    {
        textNumDeal.transform.localPosition = new Vector3(32, 0, 0);
        textNumDeal.gameObject.SetActive(true);
        textNumDeal.text = $"{-totalBetValue}";
        textNumDeal.transform.DOLocalMoveY(140, 2f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { textNumDeal.gameObject.SetActive(false); });
    }

    private void ClickButtonCloseHistory()
    {
        playSound(SOUND_GAME.CLICK);
        imagePopupHistory.gameObject.SetActive(false);
    }

    private void ClickButtonHistory()
    {
        playSound(SOUND_GAME.CLICK);
        imagePopupHistory.gameObject.SetActive(true);
        if (listResultHistory.Count != 0)
        {
            listResultHistoryInPopup[0].animVongSang.gameObject.SetActive(true);
            listResultHistoryInPopup[0].animVongSang.AnimationState.SetAnimation(0, "khung1", true);
        }
        else
        {
            textPercentBlack.text = $"0%";
            textPercentRed.text = $"0%";
        }
    }

    private void ClickButtonClear()
{
    playSound(SOUND_GAME.CLICK);
    buttonDeal.interactable = false;
    buttonClear.interactable = false;

    // Chỉ xóa những chip chưa được deal
    foreach (var betOption in listBetOptions)
    {
        int childCount = betOption.transform.childCount;

        for (int i = childCount - 1; i >= 0; i--) 
        {
            Transform child = betOption.transform.GetChild(i);
            ChipBetRouLette chip = child.GetComponent<ChipBetRouLette>();
            if (chip != null && !chip.isDealed) // Chỉ xóa chip chưa deal
            {
                ClearChip(child);
            }
        }
    }

    // Lọc ra những cược đã deal
    var dealtBets = listDataBet.Where(bet => 
        listBetOptions[bet.IdBet].transform.GetComponentsInChildren<ChipBetRouLette>()
            .Any(chip => chip.isDealed)).ToList();

    // Clear listDataBet và chỉ giữ lại những cược đã deal
    listDataBet.Clear();
    listDataBet.AddRange(dealtBets);

    // Reset totalBetValue về 0 vì những cược chưa deal đã bị xóa
    totalBetValue = 0;

    // Update UI
    textFrameCoin.text = Globals.Config.FormatMoney(totalBetDeal, true);
    textFrameCoin_1.text = textFrameCoin_2.text = "0";

    UpdateButtonBet(thisPlayer.ag);
}

    public void ClickButtonDouble()
    {
        playSound(SOUND_GAME.CLICK);

        // Validate double
        if (listDataBet.Count == 0)
        {
            showNoti(3); // Không có cược để nhân đôi
            return;
        }

        long afterDoubleBet = (totalBetDeal + totalBetValue) * 2;

        // Check max bet
        if (afterDoubleBet > agTable * 200)
        {
            showNoti(1, $"{agTable * 200}");
            return;
        }

        // Check đủ tiền
        if (totalBetValue * 2 > agRemaining)
        {
            showNoti(2);
            return;
        }

        // Logic double bet giữ nguyên
        buttonClear.interactable = true;
        buttonDeal.interactable = true;

        List<BetData> clonedBets = new List<BetData>();
        foreach (var bet in listDataBet)
        {
            clonedBets.Add(new BetData(bet.IdBet, bet.BetType, bet.NumArr, bet.BetAmount));
        }
        if (totalBetValue == 0)
        {
            totalBetValue = totalBetDeal;
            textFrameCoin_1.text = textFrameCoin_2.text = Globals.Config.FormatMoney(totalBetValue, true);
            textFrameCoin.text = Globals.Config.FormatMoney(totalBetDeal * 2, true);
        }
        else
        {
            Debug.Log(" có chạy vào đây");
            totalBetValue *= 2;
            textFrameCoin_1.text = textFrameCoin_2.text = Globals.Config.FormatMoney(totalBetValue, true);
            textFrameCoin.text = Globals.Config.FormatMoney(totalBetValue + totalBetDeal, true);
        }

        foreach (var bet in clonedBets)
        {
            if (specialBets.TryGetValue(bet.NumArr, out int betOption))
            {
                var chipClone = Instantiate(chipBet, listBetOptions[betOption].transform);
                chipClone.Init(currenIdBet, bet.BetAmount);
            }
            else
            {
                var chipClone = Instantiate(chipBet, listBetOptions[bet.IdBet].transform);
                chipClone.Init(currenIdBet, bet.BetAmount);
            }
        }
        Debug.Log($"TinhClickDouble: {JsonConvert.SerializeObject(listDataBet, Formatting.Indented)}");
    }

    public class ListComparer : IEqualityComparer<List<int>>
    {
        public bool Equals(List<int> x, List<int> y)
        {
            return x != null && y != null && x.SequenceEqual(y);
        }

        public int GetHashCode(List<int> obj)
        {
            if (obj == null) return 0;
            int hash = 17;
            foreach (var num in obj)
            {
                hash = hash * 31 + num.GetHashCode();
            }
            return hash;
        }
    }

    private void UpdateButtonBet(long agRemaining)
    {
        long tienConLai = agRemaining - totalBetValue;

        Debug.Log($"Update Button Bet: Money Left={tienConLai}");

        List<int> coefficients = new() { 1, 5, 10, 50, 100 };
        List<BetButtonRoulette> listBetButtonRouletteTemp = new List<BetButtonRoulette>();
        for (int i = 0; i < listBetButtonRoulette.Count; i++)
        {
            long chipValue = agTable * coefficients[i];
            bool canUseChip = chipValue <= tienConLai;
            listBetButtonRoulette[i].button.interactable = canUseChip;
            if (canUseChip)
            {
                listBetButtonRouletteTemp.Add(listBetButtonRoulette[i]);
            }
        }

        int maxCurrenIdBet = 0;
        Debug.Log(listBetButtonRouletteTemp.Count + " xem giá trị count list");
        if (listBetButtonRouletteTemp.Count > 0)
        {
            Debug.Log("có chạy vào đây này");
            foreach (var betButton in listBetButtonRouletteTemp)
            {
                int index = listBetButtonRoulette.IndexOf(betButton);
                if (currenIdBet < index)
                {
                    continue;
                }
                if (index >= maxCurrenIdBet)
                {
                    maxCurrenIdBet = index;
                }
            }
            currenIdBet = maxCurrenIdBet;
        }
        else
        {
            Debug.Log("có chạy vào đây");
            currenIdBet = 0;
        }
        SelectButtonBet(currenIdBet);
        buttonDeal.interactable = totalBetValue > 0;
        buttonClear.interactable = totalBetValue > 0;
        buttonDouble.interactable =  (totalBetValue * 2 <= tienConLai);
    }

    private void showNoti(int type, string context = null)
    {
        switch (type)
        {
            case 0:
                UIManager.instance.showToast("Ang chip ang hindi sapat para pumusta"); // Not enough chips to bet
                break;
            case 1:
                UIManager.instance.showToast($"Pinakamataas ng pagtaya: {context}"); // Maximum bet limit
                break;
            case 2:
                UIManager.instance.showToast("Hindi sapat ang iyong chips!!!"); // Your chips are not enough
                break;
            case 3:
                UIManager.instance.showToast("Walang taya na dodoblehin"); // No bet to double
                break;
        }
    }

    public void ClickButtonSendBet(int idBetOption)
    {
        Debug.Log("idBet ô cược" + idBetOption.ToString());
        if (!isBetTime) return;

        List<int> coefficients = new() { 1, 5, 10, 50, 100 };
        long chipValue = agTable * coefficients[currenIdBet];
        if (thisPlayer.ag - totalBetValue - chipValue < 0)
        {
            showNoti(0);
            return;
        }
        long totalBetAfter = totalBetDeal + totalBetValue + chipValue;
        buttonDeal.interactable = true;
        buttonClear.interactable = true;
        buttonDouble.interactable = true;
        playSound(SOUND_ROULETTE.coinAdd);
        List<int> betOne = new List<int> { 121, 122, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156 };
        List<int> betFour = new List<int> { 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120 };
        List<int> betThree = new List<int> { 85, 97, 109, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134 };
        int betType = (idBetOption >= 0 && idBetOption <= 36) ? 0 :
            betFour.Contains(idBetOption) ? 4 :
            (idBetOption >= 37 && idBetOption <= 42) ? 5 :
            (idBetOption >= 43 && idBetOption <= 48) ? 6 :
            betThree.Contains(idBetOption) ? 2 :
            ((idBetOption >= 49 && idBetOption <= 84) || betOne.Contains(idBetOption)) ? 1 : -1;
        List<int> numArr = new List<int>();
        switch (idBetOption)
        {
            case 37: numArr = new List<int> { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }; break;
            case 38: numArr = new List<int> { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }; break;
            case 39: numArr = new List<int> { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }; break;
            case 40: numArr = Enumerable.Range(1, 12).ToList(); break; // 1 - 12
            case 41: numArr = Enumerable.Range(13, 12).ToList(); break; // 13 - 24
            case 42: numArr = Enumerable.Range(25, 12).ToList(); break; // 25 - 36
            case 43: numArr = Enumerable.Range(1, 18).ToList(); break; // 1 - 18
            case 44: numArr = Enumerable.Range(1, 36).Where(n => n % 2 == 0).ToList(); break; // Số chẵn từ 1 - 36
            case 45:
                numArr = new List<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }; break;
            case 46:
                numArr = new List<int> { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 }; break;
            case 47: numArr = Enumerable.Range(1, 36).Where(n => n % 2 != 0).ToList(); break; // Số lẻ từ 1 - 36
            case 48: numArr = Enumerable.Range(19, 18).ToList(); break; // 19 - 36
            case 49: numArr = new List<int> { 0, 3 }; break;
            case 50: numArr = new List<int> { 3, 6 }; break;
            case 51: numArr = new List<int> { 6, 9 }; break;
            case 52: numArr = new List<int> { 9, 12 }; break;
            case 53: numArr = new List<int> { 12, 15 }; break;
            case 54: numArr = new List<int> { 15, 18 }; break;
            case 55: numArr = new List<int> { 18, 21 }; break;
            case 56: numArr = new List<int> { 21, 24 }; break;
            case 57: numArr = new List<int> { 24, 27 }; break;
            case 58: numArr = new List<int> { 27, 30 }; break;
            case 59: numArr = new List<int> { 30, 33 }; break;
            case 60: numArr = new List<int> { 33, 36 }; break;
            case 61: numArr = new List<int> { 0, 2 }; break;
            case 62: numArr = new List<int> { 2, 5 }; break;
            case 63: numArr = new List<int> { 5, 8 }; break;
            case 64: numArr = new List<int> { 8, 11 }; break;
            case 65: numArr = new List<int> { 11, 14 }; break;
            case 66: numArr = new List<int> { 14, 17 }; break;
            case 67: numArr = new List<int> { 17, 20 }; break;
            case 68: numArr = new List<int> { 20, 23 }; break;
            case 69: numArr = new List<int> { 23, 26 }; break;
            case 70: numArr = new List<int> { 26, 29 }; break;
            case 71: numArr = new List<int> { 29, 32 }; break;
            case 72: numArr = new List<int> { 32, 35 }; break;
            case 73: numArr = new List<int> { 0, 1 }; break;
            case 74: numArr = new List<int> { 1, 4 }; break;
            case 75: numArr = new List<int> { 4, 7 }; break;
            case 76: numArr = new List<int> { 7, 10 }; break;
            case 77: numArr = new List<int> { 10, 13 }; break;
            case 78: numArr = new List<int> { 13, 16 }; break;
            case 79: numArr = new List<int> { 16, 19 }; break;
            case 80: numArr = new List<int> { 19, 22 }; break;
            case 81: numArr = new List<int> { 22, 25 }; break;
            case 82: numArr = new List<int> { 25, 28 }; break;
            case 83: numArr = new List<int> { 28, 31 }; break;
            case 84: numArr = new List<int> { 31, 34 }; break;
            case 85: numArr = new List<int> { 0, 2, 3 }; break;
            case 86: numArr = new List<int> { 2, 3, 5, 6 }; break;
            case 87: numArr = new List<int> { 5, 6, 8, 9 }; break;
            case 88: numArr = new List<int> { 8, 9, 11, 12 }; break;
            case 89: numArr = new List<int> { 11, 12, 14, 15 }; break;
            case 90: numArr = new List<int> { 14, 15, 17, 18 }; break;
            case 91: numArr = new List<int> { 17, 18, 20, 21 }; break;
            case 92: numArr = new List<int> { 20, 21, 23, 24 }; break;
            case 93: numArr = new List<int> { 23, 24, 26, 27 }; break;
            case 94: numArr = new List<int> { 26, 27, 29, 30 }; break;
            case 95: numArr = new List<int> { 29, 30, 32, 33 }; break;
            case 96: numArr = new List<int> { 32, 33, 35, 36 }; break;
            case 97: numArr = new List<int> { 0, 1, 2 }; break;
            case 98: numArr = new List<int> { 1, 2, 4, 5 }; break;
            case 99: numArr = new List<int> { 4, 5, 7, 8 }; break;
            case 100: numArr = new List<int> { 7, 8, 10, 11 }; break;
            case 101: numArr = new List<int> { 10, 11, 13, 14 }; break;
            case 102: numArr = new List<int> { 13, 14, 16, 17 }; break;
            case 103: numArr = new List<int> { 16, 17, 19, 20 }; break;
            case 104: numArr = new List<int> { 19, 20, 22, 23 }; break;
            case 105: numArr = new List<int> { 22, 23, 25, 26 }; break;
            case 106: numArr = new List<int> { 25, 26, 28, 29 }; break;
            case 107: numArr = new List<int> { 28, 29, 31, 32 }; break;
            case 108: numArr = new List<int> { 31, 32, 34, 35 }; break;
            case 109: numArr = new List<int> { 0, 1, 2, 3 }; break;
            case 110: numArr = new List<int> { 1, 2, 3, 4, 5, 6 }; break;
            case 111: numArr = new List<int> { 4, 5, 6, 7, 8, 9 }; break;
            case 112: numArr = new List<int> { 7, 8, 9, 10, 11, 12 }; break;
            case 113: numArr = new List<int> { 10, 11, 12, 13, 14, 15 }; break;
            case 114: numArr = new List<int> { 13, 14, 15, 16, 17, 18 }; break;
            case 115: numArr = new List<int> { 16, 17, 18, 19, 20, 21 }; break;
            case 116: numArr = new List<int> { 19, 20, 21, 22, 23, 24 }; break;
            case 117: numArr = new List<int> { 22, 23, 24, 25, 26, 27 }; break;
            case 118: numArr = new List<int> { 25, 26, 27, 28, 29, 30 }; break;
            case 119: numArr = new List<int> { 28, 29, 30, 31, 32, 33 }; break;
            case 120: numArr = new List<int> { 31, 32, 33, 34, 35, 36 }; break;
            case 121: numArr = new List<int> { 2, 3 }; break;
            case 122: numArr = new List<int> { 1, 2 }; break;
            case 123: numArr = new List<int> { 1, 2, 3 }; break;
            case 124: numArr = new List<int> { 4, 5, 6 }; break;
            case 125: numArr = new List<int> { 7, 8, 9 }; break;
            case 126: numArr = new List<int> { 10, 11, 12 }; break;
            case 127: numArr = new List<int> { 13, 14, 15 }; break;
            case 128: numArr = new List<int> { 16, 17, 18 }; break;
            case 129: numArr = new List<int> { 19, 20, 21 }; break;
            case 130: numArr = new List<int> { 22, 23, 24 }; break;
            case 131: numArr = new List<int> { 25, 26, 27 }; break;
            case 132: numArr = new List<int> { 28, 29, 30 }; break;
            case 133: numArr = new List<int> { 31, 32, 33 }; break;
            case 134: numArr = new List<int> { 34, 35, 36 }; break;
            case 135: numArr = new List<int> { 4, 5 }; break;
            case 136: numArr = new List<int> { 7, 8 }; break;
            case 137: numArr = new List<int> { 10, 11 }; break;
            case 138: numArr = new List<int> { 13, 14 }; break;
            case 139: numArr = new List<int> { 16, 17 }; break;
            case 140: numArr = new List<int> { 19, 20 }; break;
            case 141: numArr = new List<int> { 22, 23 }; break;
            case 142: numArr = new List<int> { 25, 26 }; break;
            case 143: numArr = new List<int> { 28, 29 }; break;
            case 144: numArr = new List<int> { 31, 32 }; break;
            case 145: numArr = new List<int> { 34, 35 }; break;
            case 146: numArr = new List<int> { 5, 6 }; break;
            case 147: numArr = new List<int> { 8, 9 }; break;
            case 148: numArr = new List<int> { 11, 12 }; break;
            case 149: numArr = new List<int> { 14, 15 }; break;
            case 150: numArr = new List<int> { 17, 18 }; break;
            case 151: numArr = new List<int> { 20, 21 }; break;
            case 152: numArr = new List<int> { 23, 24 }; break;
            case 153: numArr = new List<int> { 26, 27 }; break;
            case 154: numArr = new List<int> { 29, 30 }; break;
            case 155: numArr = new List<int> { 32, 33 }; break;
            case 156: numArr = new List<int> { 35, 36 }; break;
            default: numArr = new List<int> { idBetOption }; break;
        }

        var existingBet = listDataBet.FirstOrDefault(b => b.NumArr.SequenceEqual(numArr));

        if (existingBet != null)
        {
            existingBet.BetAmount += chipValue;
        }
        else
        {
            listDataBet.Add(new BetData(idBetOption, betType, numArr, chipValue));
        }

        totalBetValue += chipValue;
        textFrameCoin.text = Globals.Config.FormatMoney(totalBetValue + totalBetDeal, true);
        textFrameCoin_1.text = textFrameCoin_2.text = Globals.Config.FormatMoney(totalBetValue, true);
        long totalBetAtOption = listDataBet.FirstOrDefault(t => t.NumArr.SequenceEqual(numArr))?.BetAmount ?? 0;
        if (newDataBetDeal.ContainsKey(idBetOption))
        {
            totalBetAtOption += newDataBetDeal[idBetOption];
        }
        UpdateButtonBet(thisPlayer.ag);
        if (agRemaining < agTable) return;
        textNumMoney.transform.SetParent(listBetOptions[idBetOption].transform);
        ShowTextNumMoney(Vector3.zero + new Vector3(0, 20, 0), Vector3.zero + new Vector3(0, 50, 0), totalBetAtOption);

        chip = Instantiate(chipBet, listBetOptions[idBetOption].transform);
        chip.Init(currenIdBet, chipValue);
    }

    private void ShowTextNumMoney(Vector3 posStart, Vector3 posEnd, long num)
    {
        if (textNumMoney == null) return;

        TextMeshProUGUI newTextNumMoney = Instantiate(textNumMoney, textNumMoney.transform.parent);
        newTextNumMoney.gameObject.SetActive(true);
        newTextNumMoney.transform.localPosition = posStart;
        newTextNumMoney.text = Globals.Config.FormatMoney(num, true);

        newTextNumMoney.transform.DOLocalMove(posEnd, 0.5f).OnComplete(() => { Destroy(newTextNumMoney.gameObject); });
    }

    private async void _OnStartGame()
    {
        for (int i = 0; i < listBetOptions.Count; i++)
        {
            listBetOptions[i].id = i;
        }
        UpdateButtonBet(thisPlayer.ag);
        SetTextButtonBetRoulette();
    }

    private async void _CallAsyncFunction(Awaitable function)
    {
        try
        {
            await function;
        }
        catch (Exception e)
        {
            if (e.GetType() != typeof(MissingReferenceException))
                Debug.LogError("Error on calling async function: " + e.Message);
        }
        // catch (Exception e) { Debug.LogError("Error on calling async function: " + e.Message); }
    }

    private void OnDisable()
    {
        stateGame = STATE_GAME.VIEWING;
    }

    //TODO: dung override Starrt khi o mainscene
    // protected override void Start()
    // {
    //     base.Start();
    //     buttonSpine.onClick.AddListener(clickButtonSpin);
    // }
    public void GroupBetOptionHorizontal(int num, bool active)
    {
        for (int i = 0; i < 13; i++)
        {
            foreach (var item in listBetOptions[num].imageChoosing)
            {
                item.gameObject.SetActive(active);
            }
            num += 3;
        }
    }

    public void GroupBetOptionGrid(int numStart, int numEnd, bool active)
    {
        for (int i = numStart; i <= numEnd; i++)
        {
            foreach (var item in listBetOptions[i].imageChoosing)
            {
                item.gameObject.SetActive(active);
            }
        }
    }

    public void GroupBetOptionEvenOdd(bool isEven, bool active)
    {
        for (int i = 1; i <= 36; i++)
        {
            if ((isEven && i % 2 == 0) || (!isEven && i % 2 != 0))
            {
                foreach (var item in listBetOptions[i].imageChoosing)
                {
                    item.gameObject.SetActive(active);
                }
            }
        }
    }

    public void GroupBetOptionRedBlack(bool isRed, bool active)
    {
        for (int i = 1; i <= 36; i++)
        {
            bool isRedNumber = redNumbers.Contains(i);
            if (isRed == isRedNumber)
            {
                foreach (var item in listBetOptions[i].imageChoosing)
                {
                    item.gameObject.SetActive(active);
                }
            }
        }
    }

    private void SetTextButtonBetRoulette()
    {
        List<int> coefficients = new() { 1, 5, 10, 50, 100 };
        for (int i = 0; i < listBetButtonRoulette.Count; i++)
        {
            int value = agTable * coefficients[i];
            listBetButtonRoulette[i].textbet.text = FormatNumber(value);
        }
    }

    private string FormatNumber(long value)
    {
        if (value >= 1000000)
            return (value / 1000000f).ToString("0.#") + "M";
        if (value >= 1000)
            return (value / 1000f).ToString("0.#") + "K";
        return value.ToString();
    }

    public void SelectButtonBet(int id)
    {
        currenIdBet = id;
        List<int> coefficients = new() { 1, 5, 10, 50, 100 };
        currentBet = agTable * coefficients[id];
        Debug.Log($"TinhCurrentBet: {currentBet}");
        for (int i = 0; i < listBetButtonRoulette.Count; i++)
        {
            bool isSelected = (i == id);
            listBetButtonRoulette[i].imageBorder.gameObject.SetActive(isSelected);
            listBetButtonRoulette[i].button.transform.localScale =
                isSelected ? new Vector3(1.25f, 1.25f, 1.25f) : Vector3.one;
        }
        for (int i = 0; i < listBetOptions.Count; i++)
        {
            listBetOptions[i].buttonBetOption.interactable = true;
        }

    }

    private void RotateSpinAndBall()
    {
        Vector2 spinCenter = imageSpin.rectTransform.localPosition;
        Vector2 ballCenter = imageBall.rectTransform.localPosition;
        float initialRadius = Vector2.Distance(ballCenter, spinCenter);
        Vector3 startPosBall = new Vector3(296, 0, 0);
        imageBall.transform.localPosition = startPosBall;
        imageSpin.rectTransform.DORotate(new Vector3(0, 0, 100), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                imageBall.rectTransform
                    .DORotate(new Vector3(0, 0, -720), 5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear);
                imageBall.rectTransform
                    .DOLocalPath(GetCirclePath(spinCenter, initialRadius, 1080), 5f, PathType.CatmullRom)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { ReduceRadiusAndSpin(spinCenter, initialRadius, 2, 3f); });

                imageSpin.rectTransform.DORotate(new Vector3(0, 0, -720), 5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        imageSpin.rectTransform.DORotate(new Vector3(0, 0, -1440), 7f, RotateMode.FastBeyond360)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                Debug.Log("SpinDone");
                                PlayResultAnimation(result);
                            });
                    });
            });
    }

    private Vector3[] GetCirclePath(Vector2 center, float radius, float totalDegrees)
    {
        int segments = 100;
        Vector3[] path = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * (totalDegrees / segments));
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            path[i] = new Vector3(x, y, 0);
        }

        return path;
    }

    private void ReduceRadiusAndSpin(Vector2 center, float initialRadius, int numRounds, float duration)
    {
        float totalDegrees = 360f * numRounds;
        float timeStep = duration / numRounds / 10;

        DOTween.To(() => initialRadius, x => initialRadius = x, initialRadius * 0.5f, duration)
            .SetEase(Ease.InOutQuad);

        imageBall.rectTransform.DOLocalPath(GetShrinkingCirclePath(center, initialRadius, totalDegrees), duration,
                PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Vector2 targetPos = listPosBallEnd[result];
                Vector2 direction = (targetPos - Vector2.zero).normalized;
                Vector2 offsetPos = targetPos - direction * 50f;

                imageBall.transform
                    .DOLocalMove(new Vector3(offsetPos.x, offsetPos.y, 0), 0.5f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        imageBall.transform.SetParent(imageSpin.transform);
                    });
            });
    }

     private Vector3[] GetShrinkingCirclePath(Vector2 center, float initialRadius, float totalDegrees)
    {
        int segments = 100;
        Vector3[] path = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float radius = Mathf.Lerp(initialRadius, initialRadius * 0.5f, t);
            float angle = Mathf.Deg2Rad * (t * totalDegrees);
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            path[i] = new Vector3(x, y, 0);
        }

        return path;
    }
    private void PlayResultAnimation(int result)
    {
        animResult.gameObject.SetActive(true);

        string animationName = result == 0 ? "green" : (redNumbers.Contains(result) ? "red" : "black");
        playSound(SOUND_ROULETTE.showResult);
        textResult.gameObject.SetActive(true);
        textResult.text = $"{result}";
        animResult.AnimationState.SetAnimation(0, animationName, false).Complete += (entry) =>
        {
            buttonHistory.interactable = true;
            listDataBet.Clear();
            textResult.gameObject.SetActive(false);
            Tweener tweener = null;
            tweener = table_1.DOAnchorPosX(0, 1).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                Debug.Log($"OnMoveComplete");
                tweener.Kill();
                ShowReSult();
            });
            table_2.DOAnchorPosX(-2400, 1f).SetEase(Ease.InOutQuad);
            _rectTransformButtonMenu.DOAnchorPosX(60, 0.5f);
            transformTabResult.DOAnchorPosX(-44, 0.75f).SetEase(Ease.InOutQuad);
        };
        ResultHistory resultClone = Instantiate(resultHistory, transformResult);
        if (result == 0)
        {
            resultClone.Init(result, 0, false);
        }
        else
        {
            if (redNumbers.Contains(result))
            {
                resultClone.Init(result, 1, false);
            }
            else
            {
                resultClone.Init(result, 2, false);
            }
        }

        for (int i = 0; i < transformResult.childCount; i++)
        {
            Transform child = transformResult.GetChild(i);
            if (i == transformResult.childCount - 1)
            {
                child.localScale = Vector3.one;
            }
            else
            {
                child.localScale = new Vector3(0.75f, 0.75f, 1f);
            }
        }
    }

    private void ShowAnimSo()
    {
        if (totalBetDeal != 0)
        {
            if (agWin > 0)
            {
                animShowSo.gameObject.SetActive(true);
                animShowSo.AnimationState.SetAnimation(0, "win", true);
                textNumWin.gameObject.SetActive(true);
                textNumLose.gameObject.SetActive(false);
                textNumWin.text = $"+{FormatNumber(agWin)}";
                playSound(SOUND_GAME.WIN);
            }
            else
            {
                animShowSo.gameObject.SetActive(true);
                animShowSo.AnimationState.SetAnimation(0, "lose", false);
                textNumWin.gameObject.SetActive(false);
                textNumLose.transform.localPosition = new Vector3(108, 40, 0);
                textNumLose.gameObject.SetActive(true);
                textNumLose.transform.DOLocalMoveY(10, 0.5f);
                textNumLose.text = $"-{FormatNumber(totalBetDeal)}";
                playSound(SOUND_GAME.LOSE);
            }
        }
        else
        {
            animShowSo.gameObject.SetActive(false);
        }

    }
    private void ShowReSult()
    {
        // 1. Kiểm tra các cược đặc biệt từ specialBets
        foreach (var specialBet in specialBets)
        {
            if (specialBet.Key.Contains(result))
            {
                // Highlight ô cược đặc biệt thắng
                HighlightWinningBet(specialBet.Value);
            }
        }

        // 2. Kiểm tra cược số trực tiếp
        if (result >= 0 && result <= 36)
        {
            HighlightWinningBet(result);
        }

        // Delay 2s rồi mới gọi ShowAnimSo
        DOVirtual.DelayedCall(5f, () =>
        {
            ShowAnimSo();

            // Delay tiếp 3s sau khi show animation mới restart game
            DOVirtual.DelayedCall(2f, () =>
            {
                ReStartGame();
                playSound(SOUND_GAME.THROW_CHIP);
            });
        });
    }


    private void HighlightWinningBet(int betId)
    {
        if (betId >= 0 && betId < listBetOptions.Count)
        {
            var img = listBetOptions[betId].imageChoosing[0];
            img.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < 4; i++)
            {
                sequence.AppendCallback(() => img.gameObject.SetActive(true));
                sequence.AppendInterval(0.4f);
                sequence.AppendCallback(() => img.gameObject.SetActive(false));
                sequence.AppendInterval(0.4f);
            }

            sequence.Play();
        }

    }
}