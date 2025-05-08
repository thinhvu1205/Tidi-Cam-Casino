using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System.Threading.Tasks;
using System.Linq;
using Globals;

public class Lucky9View : GameView
{
    [SerializeField] List<Lucky9BoxBet> listBoxBets;
    [SerializeField] GameObject startTime, iconBanker, cardContainer, popupRule, chipContainer, funnyMessage, ruleGame, cardStackNode;
    [SerializeField] Card cardPref;
    [SerializeField] ChipLucky9 chipLucky9Prefab;
    [SerializeField] Lucky9Hitpot hitPot;
    [SerializeField] Lucky9NodeBet betManager;
    [SerializeField] Lucky9HandControl handControl;
    [SerializeField] Lucky9ScoreResult bg_Score_Result;
    [SerializeField] Lucky9NodeBtn nodeButtons;
    [SerializeField] Lucky9RateItem rateItem;
    [SerializeField] ItemChatInGame itemChat;
    [SerializeField] AnimHitpot animHitpot;
    private List<Card> _CardCs = new();
    private List<ChipLucky9> _ChipCL9s = new();
    private JArray _PlayerDataJA = new();
    private const float _PLAYER_SCALE = 0.45f, _OTHER_SCALE = 0.4f;
    private int _CurBankerId, _CurDealerId;
    private float _CardScale = 0.4f;
    private bool _IsDrawTurn, _LcDone = false;

    protected override void updatePositionPlayerView()
    {
        iconBanker.SetActive(false);
        for (int i = 0; i < players.Count; i++)
        {
            int index = getDynamicIndex(getIndexOf(players[i]));
            players[i].playerView.transform.localPosition = listPosView[index];
            players[i]._indexDynamic = index;
            players[i].playerView.enableHitpot(true);
            if (players[i].id == _CurBankerId)
            {
                iconBanker.SetActive(true);
                iconBanker.transform.localPosition = getBankerPos(players[i]._indexDynamic);
            }
            players[i].updateItemVip(players[i].vip);
        }
    }

    public void handleStartGame(JObject data)
    {
        stateGame = STATE_GAME.WAITING;
        SoundManager.instance.playEffectFromPath(SOUND_GAME.ALL_IN);
        int timeStart = (int)data["time"];
        startTime.SetActive(true);
        startTime.transform.GetComponentInChildren<TextMeshProUGUI>().text = timeStart.ToString();
        resetGame();
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            timeStart--;
            checkAutoExit();
            startTime.transform.GetComponentInChildren<TextMeshProUGUI>().text = timeStart + "";
        }).SetLoops(timeStart).OnComplete(() =>
        {
            startTime.SetActive(false);
        });
        checkAutoExit();
        cardStackNode.transform.Find("card1").GetComponent<Card>().setTextureWithCode(0);
    }

    public async override void handleRJTable(string strData)
    {
        Debug.Log("!==> handle rjtable: " + strData);
        base.handleRJTable(strData);

        var data = JObject.Parse(strData);
        setTableView(data);
        _LcDone = true;
        if ((int)data["Status"] == 0)
        {
            StartCoroutine(ResetGameWithDelay(2.0f));
        }
    }

    public override void handleVTable(string strData)
    {
        Debug.Log("!==> handle vtable: " + strData);
        base.handleVTable(strData);
        UIManager.instance.showToast(Config.getTextConfig("txt_view_table"), transform);
        stateGame = STATE_GAME.VIEWING;
        JObject data = JObject.Parse(strData);
        setTableView(data);
        SoundManager.instance.playEffectFromPath(SOUND_GAME.ALL_IN);
    }

    public override void handleLTable(JObject data)
    {
        // {"evt":"ltable","Name":"Susan Omayan Huliganga","errorCode":0}
        base.handleLTable(data);
        var index = -1;
        for (int i = 0; i < _PlayerDataJA.Count; i++)
        {
            JObject dataPlayer = (JObject)_PlayerDataJA[i];
            if (getString(data, "Name") == getString(dataPlayer, "N"))
            {
                index = i;
            }
        }
        if (index == -1) return;
        var nameID = "";
        if (_PlayerDataJA.Count > 0)
        {
            JObject dataPl = (JObject)_PlayerDataJA[0];
            nameID = dataPl.ContainsKey("pid") ? "pid" : "id";
        }
        setInforHitPot(_PlayerDataJA, nameID);
    }

    public int getMaxBetTable()
    {
        return maxbet;
    }

    public override void handleCCTable(JObject data)
    {
        stateGame = STATE_GAME.WAITING;
        var name = getString(data, "Name");
        var player = getPlayer(name);
        if (player == null) return;
        for (var i = 0; i < players.Count; i++)
        {
            var pl = players[i];
            if (pl == player)
                pl.setHost(true);
            else
                pl.setHost(false);
        }
    }

    public override void cleanGame()
    {
        base.cleanGame();
        SocketSend.sendUAG();
    }


    public override void handleSTable(string strData)
    {
        Debug.Log("!==> handle stable: " + strData);
        resetGame();
        base.handleSTable(strData);
        JObject data = JObject.Parse(strData);
        JArray arrP = getJArray(data, "ArrP");
        setInforHitPot(arrP, "id");
        hitPot.updateMoney((int)data["pot"]);
    }

    private void setTableView(JObject data)
    {
        // Set Banker
        if ((int)data["BankerId"] != 0)
        {
            _CurBankerId = (int)data["BankerId"];
            Player banker = getPlayerWithID(_CurBankerId);
            iconBanker.SetActive(true);
            iconBanker.transform.localScale = Vector3.one;
            CanvasGroup canvasGroup = iconBanker.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = iconBanker.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1.0f;
            iconBanker.transform.localPosition = getBankerPos(banker._indexDynamic);
        }

        // Set Dealer
        if ((int)data["DealerId"] != 0)
        {
            _CurDealerId = (int)data["DealerId"];
            foreach (var player in players)
            {
                player.playerView.showDealer(player.id == _CurBankerId, player._indexDynamic <= 5);
            }
        }

        // Set card player
        JArray arrP = getJArray(data, "ArrP");
        foreach (JObject playerData in arrP)
        {
            Player player = getPlayerWithID((int)playerData["id"]);
            if (playerData.ContainsKey("arrCard"))
            {
                player.arrCodeCard.Clear();
                bool isShowPoint = true;
                List<int> arrCardData = getListInt(playerData, "arrCard");
                foreach (var cardCode in arrCardData)
                {
                    var cardTemp = spawnCard();
                    cardTemp.transform.SetParent(player.playerView.nodeCard.transform);
                    cardTemp.transform.localPosition = Vector3.zero;
                    cardTemp.transform.SetSiblingIndex(0);
                    player.vectorCard.Add(cardTemp);
                    cardTemp.setTextureWithCode(cardCode);
                    if (cardCode == 0) isShowPoint = false;
                    player.arrCodeCard.Add(cardCode);

                    if (player == thisPlayer)
                    {
                        cardTemp.transform.DOScale(_PLAYER_SCALE, 0.4f);
                    }
                }

                player.point = (int)playerData["score"];
                player.isSpecial = (bool)playerData["isLucky9"];
                player.rate = (int)playerData["isRate"];
                if (isShowPoint) showPlayerPoint(player, player.point, player.rate, player.isSpecial);
                sortCardPlayer(player, arrCardData);
            }
            else sortCardPlayer(player);

            if ((int)playerData["markBet"] > 0)
            {
                int index = player._indexDynamic;
                Lucky9BoxBet boxBet = listBoxBets[index];
                boxBet.gameObject.SetActive(true);
                boxBet.totalValue = (int)playerData["markBet"];
                boxBet.onBet(0);
            }

            if ((bool)playerData["isTurn"] && (int)data["timeLeft"] > 0)
            {
                if (player == thisPlayer)
                {
                    player.playerView.setTurn(true, (int)data["timeLeft"], _isMe: true, timeVibrate: 3f);
                    if (!(bool)playerData["isTakeCard"] && playerData["arrCard"] != null)
                    {
                        nodeButtons.gameObject.SetActive(true);
                        nodeButtons.onShow((int)data["timeLeft"]);
                    }

                    if (playerData["arrCard"] == null && thisPlayer.id != _CurBankerId)
                    {
                        betManager.onShow((int)data["timeLeft"]);
                    }
                }
                else player.playerView.setTurn(true, (int)data["timeLeft"], _isMe: false, timeVibrate: -1f);
            }

            if ((bool)playerData["isTakeCard"])
            {
                if (playerData["arrCard"].Count() == 2)
                {
                    showFunnyMessage(player, "im good");
                }
            }
        }

        if ((int)data["idFirstCard"] != 0)
        {
            Transform cardTransform = cardStackNode.transform.Find("card1");
            if (cardTransform != null)
            {
                Card firstCard = cardTransform.GetComponent<Card>();
                if (firstCard == null)
                {
                    initCardStack();
                    cardTransform = cardStackNode.transform.Find("card1");
                    firstCard = cardTransform.GetComponent<Card>();

                }
                firstCard.setTextureWithCode((int)data["idFirstCard"]);
            }
        }

        stateGame = (STATE_GAME)(int)data["Status"];
        setInforHitPot(arrP, "id");
        checkHaveHitPot(arrP, "id");
        hitPot.updateMoney((int)data["pot"]);
    }

    private IEnumerator ResetGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resetGame();
    }

    public void handleIsBanker(JObject data)
    {
        maxbet = (int)data["maxBet"];
        bool isBankerPlaying = getBool(data, "isPlaying");
        if (stateGame != STATE_GAME.VIEWING)
            stateGame = isBankerPlaying ? stateGame = STATE_GAME.PLAYING : stateGame = STATE_GAME.VIEWING;
        iconBanker.SetActive(false);
        var lastBanker = _CurBankerId;
        var banker = getPlayerWithID(getInt(data, "pid"));
        if (banker == null) return;
        iconBanker.SetActive(true);
        _CurBankerId = getInt(data, "pid");
        if (_CurBankerId != lastBanker)
        {
            iconBanker.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            CanvasGroup canvasGroup = iconBanker.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = iconBanker.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 180 / 255f;
            iconBanker.transform.localPosition = getBankerPos(banker._indexDynamic);
            StopAllCoroutines();
            StartCoroutine(AnimateIconBanker(iconBanker.transform, canvasGroup));
        }
        StartCoroutine(HandleBetTimeWrapper((int)data["time"]));
    }

    private IEnumerator AnimateIconBanker(Transform iconTransform, CanvasGroup canvasGroup)
    {
        float duration = 0.3f;
        Vector3 targetScale = Vector3.one;
        float targetAlpha = 1f;
        float elapsedTime = 0f;
        Vector3 initialScale = iconTransform.localScale;
        float initialAlpha = canvasGroup.alpha;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            iconTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, t);
            yield return null;
        }
        iconTransform.localScale = targetScale;
        canvasGroup.alpha = targetAlpha;
    }

    Vector2 getBankerPos(int index)
    {
        return listBoxBets[index].transform.localPosition;
    }

    private IEnumerator HandleBetTimeWrapper(int time)
    {
        yield return new WaitForSeconds(1f);
        handleBetTime(time);
        HandleGame.nextEvt();
    }

    public void handleBetTime(int time = 7)
    {
        foreach (Player player in players)
        {
            if (player == thisPlayer)
            {
                if (stateGame == STATE_GAME.PLAYING && _CurBankerId != player.id)
                {
                    player.playerView.setTurn(true, time, _isMe: true, timeVibrate: 3f);
                    betManager.onShow(time);
                }
            }
            else if (_CurBankerId != player.id)
            {
                player.playerView.setTurn(true, time, _isMe: false, timeVibrate: -1f);
            }
        }
    }

    public void onClickRuleHitpot()
    {
        popupRule.gameObject.SetActive(!popupRule.gameObject.activeSelf);
    }

    public async void handleIsDealer(JObject data)
    {
        revealFirstCardCoroutine((int)data["C"]);
        await Task.Delay(1400);
        setDealerAfterDelay((int)data["pid"]);
    }

    private async void setDealerAfterDelay(int dealerId)
    {
        _CurDealerId = dealerId;
        foreach (var player in players)
        {
            player.playerView.showDealer(player.id == _CurDealerId, player._indexDynamic <= 8);
        }
        await Task.Delay(600);
        handleLc();
    }

    private void revealFirstCardCoroutine(int code)
    {
        cardStackNode.transform.DOLocalMove(new Vector2(0, 0), 0.6f).SetEase(Ease.InOutCubic);
        cardStackNode.transform.DOScale(new Vector3(2, 2, 2), 0.6f).SetEase(Ease.InOutCubic);

        var cardTemp = getCard();
        cardTemp.transform.SetParent(cardStackNode.transform);
        cardTemp.transform.localPosition = new Vector2(-2, 10);
        cardTemp.transform.localRotation = Quaternion.Euler(0, 0, -2);
        cardTemp.transform.localScale = new Vector2(0.4f, 0.4f);
        cardTemp.transform.SetSiblingIndex(0);
        foldCardUp(cardTemp, code, 0.4f);
    }

    async void foldCardUp(Card card, int code, float time, float scale = 0.4f, int delay = 200)
    {
        await Task.Delay(delay);
        Vector2 scaleCard = new Vector2(0.01f, scale);
        card.transform.DOScale(scaleCard, time / 2f).OnComplete(() =>
        {
            card.setTextureWithCode(code);
            card.transform.DOScale(0.4f, time / 2f).SetEase(Ease.InOutCubic);
        });
        Quaternion newRotation = Quaternion.Euler(0, 10, 0);
        card.transform.DOLocalRotate(newRotation.eulerAngles, time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            newRotation = Quaternion.Euler(0, -10, 0);
            card.transform.localRotation = newRotation;
            card.transform.DOLocalRotate(Vector3.zero, time / 2).SetEase(Ease.InOutCubic);
        });
        card.transform.DOLocalMove(new Vector2(-60, 30), time / 2).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            card.transform.SetSiblingIndex(10);
            card.transform.DOLocalMove(new Vector2(3, 10), time / 2).OnComplete(() =>
            {
                newRotation = Quaternion.Euler(5, 5, 5);
                card.transform.DOLocalRotate(newRotation.eulerAngles, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    cardStackNode.transform.Find("card1").GetComponent<Card>().setTextureWithCode(code);
                    removeRateCard(card);
                    putCard(card);
                });
            });
        });

    }

    public void handleBc(JObject data)
    {
        // {"evt":"bc","pid":1815671,"C":14,"score":5}
        var player = getPlayerWithID(getInt(data, "pid"));
        int code = 0;
        bool checkCode = true;
        if (data.ContainsKey("C"))
            code = getInt(data, "C");
        else
            checkCode = false;
        if (checkCode)
        {
            List<int> arrC = new List<int> { 0, 0, 0 };
            Card cardTemp = spawnCard();
            Vector2 pos = cardStackNode.transform.localPosition;
            cardTemp.transform.localPosition = player.playerView.transform.localPosition;
            cardTemp.transform.rotation = Quaternion.Euler(0, 0, -2);
            cardTemp.transform.SetSiblingIndex(0);
            player.vectorCard.Add(cardTemp);

            Debug.Log($"!==> player draw {player.id}");

            cardTemp.transform.SetParent(player.playerView.nodeCard.transform, false);

            if (player == thisPlayer)
            {
                arrC = new List<int>();
                thisPlayer.arrCodeCard.Add(code);
                arrC = thisPlayer.arrCodeCard;
                Debug.Log($"!==> player card after get {string.Join(", ", arrC)}");
                cardTemp.transform.DOScale(_PLAYER_SCALE, 0.4f);
            }

            player.point = (int)data["score"];
            player.rate = (int)data["isRate"];
            sortCardPlayer(player, arrC);
            showFunnyMessage(player, "hirit");

            if (player == thisPlayer)
            {
                showPlayerPoint(player, player.point, thisPlayer.rate);
            }
            SoundManager.instance.playEffectFromPath(SOUND_GAME.CARD_FLIP_2);
        }
        else
        {
            showFunnyMessage(player, "im good");
            SoundManager.instance.playEffectFromPath(SOUND_GAME.DROP);

        }
        player.playerView.setTurn(false);
    }

    public async void handleLc()
    {
        _LcDone = false;
        List<Player> listDealCard = new List<Player>();
        Player dealer = getPlayerWithID(_CurDealerId);
        listDealCard.Add(dealer);
        if (stateGame == STATE_GAME.PLAYING && thisPlayer != dealer)
        {
            listDealCard.Add(thisPlayer);
        }
        foreach (Player player in players)
        {
            if (player.id != _CurDealerId && player.id != thisPlayer.id)
            {
                listDealCard.Add(player);
            }
        }
        listDealCard = sortListDealCard(listDealCard);
        Debug.Log($"!==> dealing card, isplaying {stateGame == STATE_GAME.PLAYING}, {listDealCard}");
        int len = listDealCard.Count;
        for (int i = 0; i < len * 2; i++)
        {
            bool isRotateLeft = i < len;
            int index = i < len ? i : i - len;
            await Task.Delay(250);
            SoundManager.instance.playEffectFromPath(SOUND_GAME.CARD_FLIP_1);
            var cardTemp = spawnCard();
            cardTemp.transform.SetParent(listDealCard[index].playerView.nodeCard.transform, false);
            cardTemp.transform.localPosition = Config.getPosInOtherNode(cardStackNode.transform.localPosition, listDealCard[index].playerView.nodeCard);
            cardTemp.transform.localRotation = Quaternion.Euler(0, 0, -2);
            float angle = isRotateLeft ? -10 : 10;
            cardTemp.transform.DOLocalMove(new Vector2(0, 0), 0.4f).SetEase(Ease.InOutBack);
            cardTemp.transform.DOLocalRotate(new Vector3(0, 0, 360 + angle), 0.4f, RotateMode.Fast);
            listDealCard[index].vectorCard.Add(cardTemp);
            if (listDealCard[index] == thisPlayer)
            {
                cardTemp.transform.DOScale(Vector3.one * _PLAYER_SCALE, 0.4f);
            }
            if (i == len * 2 - 1)
            {
                await Task.Delay(500);
                cardStackNode.transform.DOLocalMove(new Vector2(0, 130), 0.6f).SetEase(Ease.InOutCubic);
                cardStackNode.transform.DOScale(1, 0.6f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    HandleGame.nextEvt();
                    _LcDone = true;
                });
            }
        }
    }

    private List<Player> sortListDealCard(List<Player> listDealCard)
    {
        listDealCard.Sort((x, y) =>
        {
            if (x._indexDynamic > y._indexDynamic) return 1;
            else return -1;
        });
        List<Player> result = new List<Player>();
        Player dealer = getPlayerWithID(_CurDealerId);
        int splitPoint = listDealCard.IndexOf(dealer);

        if (splitPoint <= 0) return listDealCard;
        for (int i = splitPoint; i < listDealCard.Count; i++)
        {
            result.Add(listDealCard[i]);
        }
        for (int i = 0; i < splitPoint; i++)
        {
            result.Add(listDealCard[i]);
        }
        return result;
    }

    public async void handleTwoCard(JObject data)
    {
        Debug.Log("!==> handle two card " + data);
        thisPlayer.arrCodeCard = getListInt(data, "arrCard");
        thisPlayer.isSpecial = getBool(data, "isLucky9");
        thisPlayer.point = getInt(data, "score");
        thisPlayer.rate = getInt(data, "isRate");
        while (!_LcDone)
        {
            await Task.Delay(200);
            Debug.Log("chua chia bai xong");
            if (this == null) return;
        }
        handlePlayerTurn(data);
    }

    void handlePlayerTurn(JObject data)
    {
        Debug.Log("!==> now is player turn, check is playing " + (stateGame == STATE_GAME.PLAYING));
        _IsDrawTurn = true;

        foreach (Player player in players)
        {
            sortCardPlayer(player);

            if (player.id != _CurBankerId)
            {
                if (player == thisPlayer && stateGame == STATE_GAME.PLAYING)
                {
                    player.playerView.setTurn(true, (int)data["time"], _isMe: true, timeVibrate: 3f);
                    onShowFoldCard((int)data["time"]);
                }
                else if (player != thisPlayer)
                {
                    player.playerView.setTurn(true, (int)data["time"], _isMe: false, timeVibrate: -1f);
                }
            }
        }

        if (stateGame == STATE_GAME.PLAYING && _CurBankerId == thisPlayer.id)
        {
            _IsDrawTurn = false;
            onShowFoldCard((int)data["time"]);
        }
    }

    public void handleBankerTurn(JObject data)
    {
        // {"evt":"bankerTurn","time":10}
        _IsDrawTurn = false;
        handControl.onHide();
        nodeButtons.onHide();
        foreach (var player in players)
        {
            player.playerView.setTurn(false);
        }

        Player banker = getPlayerWithID(_CurBankerId);
        if (banker == null) return;
        Debug.Log("!==> player score: " + banker.point);

        if (banker == thisPlayer)
        {
            banker.playerView.setTurn(true, (int)data["time"], _isMe: true, timeVibrate: 3f);
            if (thisPlayer.isSpecial)
            {
                SocketSend.sendReiveCard(0);
                nodeButtons.gameObject.SetActive(false);
            }
            else
            {
                nodeButtons.gameObject.SetActive(true);
                nodeButtons.onShow((int)data["time"]);
            }
        }
        else banker.playerView.setTurn(true, (int)data["time"], _isMe: false, timeVibrate: -1f);
    }

    public void handleUAG(string strData)
    {
        Debug.Log("!>! handle uag");
        var data = JObject.Parse(strData);
        JArray ArrP = JArray.Parse(getString(data, "data"));
        foreach (JObject item in ArrP)
        {
            Player player = getPlayer(getString(item, "N"));
            if (player == null) continue;

            var ag = (int)item["AG"];
            player.ag = ag;
            player.updateMoney();
        }
    }

    void removeRateCard(Card card)
    {
        var rate = card.transform.Find("rate");
        if (rate != null)
            Destroy(rate.gameObject);
    }

    public void sortCardPlayer(Player player, List<int> arrC = null)
    {
        if (player == null)
            return;
        int len = player.vectorCard.Count;
        if (len == 0)
            return;
        float scale = player == thisPlayer ? _PLAYER_SCALE : _OTHER_SCALE;
        float midScale = player == thisPlayer ? 0.6f : _OTHER_SCALE;
        for (int i = 0; i < len; i++)
        {
            Card cardTemp = player.vectorCard[i];
            cardTemp.transform.SetAsLastSibling();
            float angle = 20 * i - 10 * (len - 1);
            float posX = 30 * i - 15 * (len - 1);
            float posY = 0;
            if (len >= 3)
            {
                posY = i == 1 ? 5 : 0;
            }
            removeRateCard(cardTemp);
            cardTemp.StopAllCoroutines();
            cardTemp.transform.DOLocalMove(Vector2.zero, 0.3f).SetEase(Ease.OutCubic);
            cardTemp.transform.DOScale(new Vector2(midScale, scale), 0.3f).OnComplete(() =>
            {
                cardTemp.transform.DOLocalMove(new Vector2(posX, posY), 0.3f).SetEase(Ease.OutBack);
                cardTemp.transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);
                cardTemp.transform.DOLocalRotate(new Vector3(0, 0, -angle), 0.3f).SetEase(Ease.InOutCubic);
            });
            if (arrC != null)
            {
                if (arrC.Count == len && len > 0)
                {
                    cardTemp.setTextureWithCode(arrC[i]);
                }
                if (i == len - 1 && player.rate > 1 && arrC[i] > 0)
                {
                    Lucky9RateItem rateObj = Instantiate(rateItem, cardTemp.transform).GetComponent<Lucky9RateItem>();
                    rateObj.setRate(player.rate);
                    rateObj.transform.localScale = player == thisPlayer ? new Vector3(1.3f, 1.3f, 1.3f) : new Vector3(1.7f, 1.7f, 1.7f);
                    rateObj.transform.localPosition = new Vector3(cardTemp.GetComponent<RectTransform>().rect.width / 2, cardTemp.GetComponent<RectTransform>().rect.height / 2, 0);
                    rateObj.name = "rate";

                }
            }
        }
    }

    public async void onShowFoldCard(int time)
    {
        if (thisPlayer.arrCodeCard.Count == 0) return;
        int timeOp = time - 4; //fix cung time = 10;
        handControl.gameObject.transform.SetSiblingIndex((int)GAME_ZORDER.Z_MENU_VIEW);

        foreach (var cardTemp in thisPlayer.vectorCard)
        {
            cardTemp.transform.DOLocalMove(new Vector3(0, -200, 0), 0.4f).SetEase(Ease.InOutCubic);
        }

        // Call hand control
        handControl.gameObject.SetActive(true);
        handControl.onShow(thisPlayer.arrCodeCard, timeOp);
    }

    public void onHideFoldCard(float timeLeft)
    {
        if (_IsDrawTurn && !thisPlayer.isSpecial)
        {
            nodeButtons.gameObject.SetActive(true);
            nodeButtons.onShow(timeLeft);
        }
        sortCardPlayer(thisPlayer, thisPlayer.arrCodeCard);
        showPlayerPoint(thisPlayer, thisPlayer.point, thisPlayer.rate, thisPlayer.isSpecial);
        notifyNanCard();
    }

    public void showPlayerPoint(Player player, int score = 0, int rate = 0, bool isSpecial = false)
    {
        if (player == null) return;
        Transform viewNode = player.playerView.transform;
        if (viewNode == null) return;
        GameObject effectLk9 = player.playerView.lucky9Ani;
        effectLk9.SetActive(true);
        SkeletonGraphic effect = effectLk9.GetComponent<SkeletonGraphic>();

        if (isSpecial)
        {
            effect.transform.localPosition = new Vector3(0, 75, 0);
            effect.AnimationState.SetAnimation(0, "lucky9", false);
            effect.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            effect.transform.localPosition = new Vector3(effect.transform.localPosition.x, 50, effect.transform.localPosition.z);
            effect.timeScale = 1;

            if (player == thisPlayer && _CurBankerId != thisPlayer.id)
            {
                player.playerView.setTurn(false);
                nodeButtons.onHide();
            }
            SoundManager.instance.playEffectFromPath(SOUND_DOMINO.SHOW_RESULTS);
            return;
        }

        Transform nodePoint = viewNode.Find("nodePoint");
        nodePoint.localPosition = new Vector3(viewNode.localPosition.x <= 0 ? 75 : -75, nodePoint.localPosition.y, nodePoint.localPosition.z);

        Lucky9ScoreResult scorePanel;
        if (nodePoint.childCount == 0)
        {
            scorePanel = Instantiate(bg_Score_Result, nodePoint);

        }
        else
        {
            scorePanel = nodePoint.GetChild(0).GetComponent<Lucky9ScoreResult>();
        }
        effect.timeScale = 2;
        effect.transform.localScale = Vector3.one;
        effect.transform.localPosition = new Vector3(nodePoint.localPosition.x, 75, effect.transform.localPosition.z);
        effect.gameObject.SetActive(true);
        effect.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(runEffectSequence(effect, effectLk9));
        scorePanel.gameObject.SetActive(false);
        StartCoroutine(showScorePanelCoroutine(scorePanel, score, rate));
        SoundManager.instance.playEffectFromPath(SOUND_GAME.DROP);
    }


    private IEnumerator runEffectSequence(SkeletonGraphic effect, GameObject effectLk9)
    {
        yield return new WaitForSeconds(0.3f);
        effect.GetComponent<CanvasGroup>().alpha = 1;
        effect.AnimationState.SetAnimation(0, "point", false);
        effect.Initialize(true);
        yield return new WaitForSeconds(2f);
        effect.gameObject.SetActive(true);
        effectLk9.SetActive(false);
    }

    private IEnumerator showScorePanelCoroutine(Lucky9ScoreResult scorePanel, int score, int rate)
    {
        yield return new WaitForSeconds(0.8f);
        scorePanel.gameObject.SetActive(true);
        scorePanel.onShow(score, rate);
    }

    void notifyNanCard()
    {
        SocketSend.notifyNanCard();
    }

    public void handleNotifyLucky9(JObject data)
    {
        Player player = getPlayerWithID((int)data["pid"]);
        if (player == null) return;
        if (player == thisPlayer) return;
        player.point = (int)data["score"];
        player.arrCodeCard = getListInt(data, "arrCard");
        player.rate = (int)data["isRate"];
        player.isSpecial = getBool(data, "isLucky9");
        sortCardPlayer(player, player.arrCodeCard);
        showPlayerPoint(player, player.point, player.rate, player.isSpecial);
        player.playerView.setTurn(false);
    }
    public void resetHitPot()
    {
        foreach (var player in players)
        {
            player.playerView.setHitpot(0);
        }
        hitPot.updateMoney(0);
        hitPot.setInfo(new List<Dictionary<string, object>>());
    }

    public void showFunnyMessage(Player player, string msg)
    {
        if (string.IsNullOrEmpty(msg) || player == null) return;

        GameObject mess = Instantiate(funnyMessage, cardContainer.transform);

        mess.SetActive(true);
        mess.transform.localPosition = player.playerView.transform.localPosition;

        if (msg == "hirit")
        {
            mess.transform.localPosition = new Vector3(mess.transform.localPosition.x, mess.transform.localPosition.y + 10, mess.transform.localPosition.z);
        }

        mess.transform.SetSiblingIndex((int)GAME_ZORDER.Z_BUTTON);

        mess.GetComponent<SkeletonGraphic>().startingAnimation = msg;
        mess.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, msg, false);
        mess.GetComponent<SkeletonGraphic>().Initialize(true);
        mess.GetComponent<SkeletonGraphic>().AnimationState.Complete += delegate
        {
            Destroy(mess);
        };

    }

    public void handleBm(JObject data)
    {
        // {"evt":"bm","pid":"2981145","agBet":"50","ag":"..."}
        var player = getPlayerWithID(getInt(data, "pid"));
        var agBet = getInt(data, "agBet");
        if (player == null) return;
        var index = player._indexDynamic;
        var viewPos = listPosView[index];
        var boxBet = listBoxBets[index];
        player.ag = getLong(data, "ag");
        player.setAg();
        SoundManager.instance.playEffectFromPath(SOUND_GAME.THROW_CHIP);
        boxBet.gameObject.SetActive(true);
        boxBet.onBet(agBet);
        if (player.ag == 0)
        {
            player.playerView.setTurn(false);
            var bubble = Instantiate(itemChat, transform);

            bubble.transform.localPosition = player.playerView.transform.localPosition + new Vector3(0, 40, 0);
            bubble.setMsg("All In!!!", "", player.playerView);
        }
        if (player == thisPlayer)
        {
            betManager.totalBet += agBet;
            betManager.lastBet = betManager.totalBet;
            betManager.checkButtons();
        }
        for (var j = 0; j < 5; j++)
        {
            ChipLucky9 chip = getChip();
            chip.transform.localPosition = player.playerView.transform.localPosition;
            chip.transform.DOLocalMove(boxBet.transform.localPosition, 0.5f).SetDelay(0.1f * j).OnComplete(() =>
            {
                putChip(chip);
            });
        }
    }

    public async void handleFinish(JObject data)
    {
        betManager.resetTotalBet();
        nodeButtons.onHide();
        JArray ArrP = JArray.Parse(getString(data, "data"));
        Debug.Log("!==> handle finish " + ArrP);
        List<Player> listHandlePlayer = new List<Player>();

        _IsDrawTurn = false;
        handControl.onHide();

        foreach (var player in players)
        {
            player.playerView.setTurn(false);
        }

        foreach (JObject playerData in ArrP)
        {
            var player = getPlayerWithID((int)playerData["pid"]);
            player.point = (int)playerData["score"];
            player.arrCodeCard = getListInt(playerData, "arrCard");
            player.rate = (int)playerData["isRate"];
            player.isSpecial = getBool(playerData, "isLucky9");
            listHandlePlayer.Add(player);
        }

        listHandlePlayer = listHandlePlayer.OrderByDescending(p => p._indexDynamic).ToList();
        int bankerIndex = listHandlePlayer.FindIndex(p => p.id == _CurBankerId);

        List<Player> listPlayer2nd = listHandlePlayer.Skip(bankerIndex + 1).ToList();
        listHandlePlayer = listPlayer2nd.Concat(listHandlePlayer).ToList();

        for (int i = 0; i < listHandlePlayer.Count; i++)
        {
            Player player = listHandlePlayer[i];
            sortCardPlayer(player, player.arrCodeCard);
            showPlayerPoint(player, player.point, player.rate, player.isSpecial);
            await Awaitable.WaitForSecondsAsync(.5f);
        }

        // float timeDelay = listHandlePlayer.Count / 10;
        setInforHitPot(ArrP, "pid");
        if (checkHaveHitPot(ArrP, "pid"))
        {
            showAnimWinHitPot();
            await Awaitable.WaitForSecondsAsync(3.5f);
        }
        resolveMoneyFinish(ArrP);
    }


    public void setHitpot(JArray arrP)
    {
        setInforHitPot(arrP, "pid");
        if (checkHaveHitPot(arrP, "pid"))
        {
            showAnimWinHitPot();
        }
    }

    public bool checkHaveHitPot(JArray arrP, string nameId)
    {
        bool isHaveHitPot = false;

        foreach (JObject playerData in arrP)
        {
            if (!playerData.ContainsKey("hitPot"))
                return isHaveHitPot;
            if ((int)playerData["hitPot"] > 0)
            {
                isHaveHitPot = true;
                var player = getPlayerWithID((int)playerData[nameId]);
                PlayerData dataPlayer = new PlayerData((int)playerData["hitPot"], player);
                StartCoroutine(flyMoney(player, 2f, (int)playerData["hitPot"]));
                animHitpot.addPlayerHitPot(dataPlayer);
            }
        }
        return isHaveHitPot;
    }

    private IEnumerator flyMoney(Player player, float delay, int money)
    {
        yield return new WaitForSeconds(delay);
        player.playerView.effectFlyMoney(money, 30);
    }

    private void setInforHitPot(JArray arrP, string nameID)
    {
        int pot = 0;
        _PlayerDataJA = arrP;
        List<Dictionary<string, object>> listPlayer = new List<Dictionary<string, object>>();

        foreach (JObject playerData in arrP)
        {
            if (!playerData.ContainsKey(nameID))
                return;
            Player player = getPlayerWithID((int)playerData[nameID]);
            if (player == null) return;

            int point = (int)playerData["hitPotPoint"];
            player.playerView.setHitpot(point);
            if (point == pot)
            {
                listPlayer.Add(getPlayerDataHitPot(pot, player));
            }
            else if (point > pot)
            {
                listPlayer.Clear();
                pot = point;
                listPlayer.Add(getPlayerDataHitPot(pot, player));
            }
        }

        hitPot.setInfo(listPlayer);
    }

    public Dictionary<string, object> getPlayerDataHitPot(int pot, Player player)
    {
        var playerData = new Dictionary<string, object>
        {
            { "Pot", pot },
            { "AvatarId", player.avatar_id },
            { "Fid", player.fid },
            { "PlayerName", player.displayName }
        };
        return playerData;
    }

    private void showAnimWinHitPot()
    {
        animHitpot.show();
        StartCoroutine(hideAnimAfterDelay(3.0f));
        _PlayerDataJA = new JArray();
        resetHitPot();
    }

    private IEnumerator hideAnimAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animHitpot.hide();
    }

    private void resolveMoneyFinish(JArray arrP)
    {
        Debug.Log("!==> resolve money finish");
        Player banker = getPlayerWithID(_CurBankerId);
        int agBankerWin = 0;
        long agBanker = banker != null ? banker.ag : 0;
        JArray listWin = new JArray();
        JArray listLose = new JArray();

        foreach (JObject playerData in arrP)
        {
            Player player = getPlayerWithID((int)playerData["pid"]);
            int status = (int)playerData["status"];
            if (status == 1) player.playerView.setEffectWin("", false);
            else if (status == 0) player.playerView.setEffectDraw(false);
            else player.playerView.setEffectLose(false);
            if (player != banker)
            {
                if (status == -1) listLose.Add(playerData);
                else if (status == 0 || status == 1) listWin.Add(playerData);
            }
            else
            {
                agBankerWin = (int)playerData["agTT"];
                agBanker = (long)playerData["agNew"];
            }

            if (player == thisPlayer)
            {
                if (status == 1) SoundManager.instance.playEffectFromPath(SOUND_GAME.WIN);
                else SoundManager.instance.playEffectFromPath(SOUND_GAME.LOSE);
            }
        }
        float timeLose = arrP.Count > 2 ? 2f : 1f;
        StartCoroutine(handleListLose(listLose, banker, 0));
        StartCoroutine(handleListWin(listWin, banker, timeLose));
        StartCoroutine(handleListArrP(arrP, timeLose + 1f));
        StartCoroutine(handleEnd(timeLose + 2f));
    }

    private IEnumerator handleListLose(JArray data, Player banker, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var playerData in data)
        {
            var player = getPlayerWithID((int)playerData["pid"]);
            if (banker != null)
                doChipFlyEffect(player, banker);
        }
    }

    private IEnumerator handleListWin(JArray data, Player banker, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var playerData in data)
        {
            var player = getPlayerWithID((int)playerData["pid"]);
            if ((int)playerData["agTT"] > 0)
            {
                if (banker != null)
                    doChipFlyEffect(banker, player);
            }
        }
    }

    private IEnumerator handleListArrP(JArray data, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var playerData in data)
        {
            Player player = getPlayerWithID((int)playerData["pid"]);
            if (player == null) continue;
            player.playerView.effectFlyMoney((int)playerData["agTT"], 40);
            //player.playerView.setupEffectChangeMoney(player.ag, (long)playerData["agNew"]);
            player.ag = (long)playerData["agNew"];
            player.setAg();
        }
    }

    private IEnumerator handleEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        stateGame = STATE_GAME.WAITING;
        HandleGame.nextEvt();
        resetGame();
        checkAutoExit();
    }

    public void doChipFlyEffect(Player playerLose, Player playerWin)
    {
        if (playerLose == null || playerWin == null) return;

        Vector2 basePos = listPosView[playerLose._indexDynamic];
        Vector2 destPos = listPosView[playerWin._indexDynamic];
        SoundManager.instance.playEffectFromPath(SOUND_GAME.THROW_CHIP);

        for (int i = 0; i < 20; i++)
        {
            StartCoroutine(ChipFlyCoroutine(basePos, destPos, i * 0.015f));
        }
    }

    private IEnumerator ChipFlyCoroutine(Vector3 basePos, Vector3 destPos, float delay)
    {
        yield return new WaitForSeconds(delay);

        float offSet1 = Random.Range(-20f, 20f);
        float offSet2 = Random.Range(-20f, 20f);
        float randMid = Random.Range(-60f, 60f);

        ChipLucky9 chipBet = getChip();
        chipBet.transform.localPosition = new Vector3(basePos.x + offSet1, basePos.y, basePos.z);
        chipBet.gameObject.SetActive(true);
        chipBet.transform.DOLocalMove(new Vector2(randMid, 20 + offSet1), 0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            chipBet.transform.DOLocalMove(new Vector2(destPos.x + offSet2, destPos.y), 0.4f).SetDelay(1f).OnComplete(() =>
            {
                putChip(chipBet);
            });
        });
        if (delay == 0)
            SoundManager.instance.playEffectFromPath(SOUND_GAME.THROW_CHIP);
    }

    public override void onClickRule()
    {
        Vector2 pos = transform.Find("guidePos").localPosition;
        ruleGame.SetActive(true);
        ruleGame.transform.Find("background").DOLocalMove(pos, 0.4f);
    }

    public void onCloseRule()
    {
        Vector2 pos = transform.Find("guidePos1").localPosition;
        ruleGame.transform.Find("background").DOLocalMove(pos, 0.4f).OnComplete(() =>
        {
            ruleGame.SetActive(false);
        });
    }

    void resetGame()
    {
        iconBanker.SetActive(false);
        foreach (Lucky9BoxBet boxBet in listBoxBets)
        {
            boxBet.onReset();
        }

        foreach (var player in players)
        {
            player.playerView.setTurn(false);
            player.playerView.showDealer(false);
            foreach (var cardTemp in player.vectorCard)
            {
                removeRateCard(cardTemp);
                putCard(cardTemp);
            }
            player.vectorCard.Clear();
            hidePlayerPoint(player);
            player.arrCodeCard.Clear();
            player.point = 0;
        }

        StopAllCoroutines(); // Assuming BetManager uses coroutines
        betManager.onHide();
        nodeButtons.onHide();

        //listCardFold.Clear();
        _IsDrawTurn = false;
    }



    public void hidePlayerPoint(Player player)
    {
        if (player == null) return;
        var viewNode = player.playerView;
        if (viewNode == null) return;

        var nodePoint = viewNode.transform.Find("nodePoint");
        var effect = viewNode.lucky9Ani;
        if (effect != null)
        {
            effect.SetActive(false);
        }

        if (nodePoint != null && nodePoint.childCount > 0)
        {
            var scoreResult = nodePoint.GetChild(0).GetComponent<Lucky9ScoreResult>();
            if (scoreResult != null)
            {
                scoreResult.onHide();
            }
        }
    }

    public void handleAddPot(JObject data)
    {
        var addPot = getInt(data, "addPot");
        var pot = getInt(data, "Pot");
        stateGame = STATE_GAME.PLAYING;
        foreach (var player in players)
        {
            for (var j = 0; j < 5; j++)
            {
                ChipLucky9 chip = getChip();
                chip.transform.localPosition = player.playerView.transform.localPosition;
                chip.transform.DOLocalMove(hitPot.transform.localPosition, 0.3f).SetDelay(0.075f * j).OnComplete(() => putChip(chip));
            }
            player.ag -= addPot;
            player.setAg();
            player.playerView.effectFlyMoney(-addPot, 40);
        }
        hitPot.updateMoney(pot);
    }

    public void effChiaBai(Card card, int code, Vector2 pos, float time, Vector3 rotation)
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.DISPATCH_CARD);
        if (code == 0) card.SetTextureBackCard();
        else card.setTextureWithCode(code);
        card.gameObject.transform.localScale = new Vector3(_CardScale, _CardScale, _CardScale);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(card.gameObject.transform.DOLocalMove(pos, 0.5f));
        sequence.Join(card.gameObject.transform.DORotate(rotation, 0.5f));
        card.gameObject.transform.SetAsLastSibling();
        card.gameObject.SetActive(true);
    }

    public void putChip(ChipLucky9 chip)
    {
        _ChipCL9s.Add(chip);
        chip.transform.SetParent(chipContainer.transform);
        chip.gameObject.SetActive(false);
    }

    public ChipLucky9 getChip()
    {
        ChipLucky9 chip;
        if (chipPool.Count > 0)
        {
            chip = _ChipCL9s[0];
            _ChipCL9s.Remove(chip);
            chip.transform.SetParent(chipContainer.transform);
        }
        else
        {
            chip = Instantiate(chipLucky9Prefab, chipContainer.transform);

        }
        chip.gameObject.SetActive(true);
        chip.transform.localScale = new Vector2(.8f, .8f);
        chip.gameView = this;
        return chip;
    }

    void initCardStack()
    {
        Transform cardTransform = cardStackNode.transform.Find("card1");
        if (cardTransform != null) return;
        Vector2[] posList = { new Vector2(-2, 10), new Vector2(3, 10) };
        for (int i = 0; i < 2; i++)
        {
            Card cardTemp = spawnCard();
            cardTemp.transform.SetParent(cardStackNode.transform, false);
            cardTemp.name = "card" + i;
            cardTemp.transform.localPosition = posList[i];
            cardTemp.transform.localRotation = Quaternion.Euler(0, 0, i == 1 ? 5 : -2);
            cardTemp.transform.SetSiblingIndex(i + 1);
        }
    }

    void initNodeBet()
    {
        betManager.setParentNodeBet(this);
    }

    void initHandControl()
    {
        handControl.gameView = this;
        handControl.gameObject.SetActive(false);
    }

    public Card spawnCard()
    {
        Card cardTemp = getCard();
        cardTemp.setTextureWithCode(0);
        cardTemp.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        cardTemp.transform.localPosition = new Vector2(-340f, 120f);
        return cardTemp;
    }

    private Card getCard()
    {
        Card card;
        if (_CardCs.Count > 0)
        {
            card = _CardCs[0];
            _CardCs.Remove(card);
            card.transform.SetParent(cardContainer.transform);
        }
        else
        {
            card = Instantiate(cardPref, cardContainer.transform);

        }
        card.setTextureWithCode(0);
        card.gameObject.SetActive(true);
        return card;
    }

    private void putCard(Card card)
    {
        if (!_CardCs.Contains(card))
            _CardCs.Add(card);
        card.transform.SetParent(null);
        card.gameObject.SetActive(false);
    }

    public void OnClickBtnShowPopupRule()
    {
        popupRule.SetActive(true);
    }
    protected override void Start()
    {
        base.Start();
        initCardStack();
        initNodeBet();
        initHandControl();
    }
}
