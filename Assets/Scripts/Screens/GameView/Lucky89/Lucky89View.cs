using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lucky89View : GameView
{
    public enum SCORE
    {
        LUCKY_8 = 5088, LUCKY_9 = 5099, THREE_OF_A_KIND = 4000, FACE_CARDS = 3000, STRAIGHT_FLUSH = 2000, FLUSH = 1000
    }
    [SerializeField] private List<TextMeshProUGUI> m_BetOptionTMPs;
    [SerializeField] private GameObject m_Actions, m_TickDraw, m_TickDontDraw;
    [SerializeField] private Transform m_PrefabChipTf, m_ChipsTf;
    [SerializeField] private TextMeshProUGUI m_TipChipsTMP, m_TipThanksTMP;
    [SerializeField] private SkeletonGraphic m_BeginGameSG, m_DealerSG;
    [SerializeField] private PlayerViewLucky89 m_DealerPVL89;
    private List<int> _BetValues = new();
    private Action _WaitForFinishCompleteCb = null;
    private const float CARD_FLYING_DURATION = .5f, CARD_ROTATING_DURATION = .3f, WIN_CHIP_DURATION = .5f, LOSE_CHIP_DURATION = .5f;
    private bool _IsMyDrawTime;
    private bool? _DrawACard = null;

    #region Button
    public void DoClickBetButton(int buttonId)
    {
        playSound(SOUND_GAME.CLICK);
        SocketSend.SendBetLucky89((int)Mathf.Min(User.userMain.AG, _BetValues[buttonId]));
    }
    public void DoClickDraw()
    {
        playSound(SOUND_GAME.CLICK);
        if (!_IsMyDrawTime) _SetTickDraw(true, 1);
        else SocketSend.SendDrawACardLucky89(1);
    }
    public void DoClickDontDraw()
    {
        playSound(SOUND_GAME.CLICK);
        if (!_IsMyDrawTime) _SetTickDraw(true, -1);
        else SocketSend.SendDrawACardLucky89(0);
    }
    public void DoClickTip()
    {
        playSound(SOUND_GAME.CLICK);
        SocketSend.sendTip();
    }
    public override void onClickRule()
    {
        playSound(SOUND_GAME.CLICK);
        Application.OpenURL("https://n.cfg.davaogames.com/rule/index.html?gameid=8802&list=true");
    }
    #endregion
    public void ProcessResponseData(JObject jData)
    {
        switch ((string)jData["evt"])
        {
            case "startdealer":
                _HandleStartGame(jData);
                break;
            case "bm":
                _HandleAnyoneBets(jData);
                break;
            case "lc":
                _HandleReceiveMyCards(jData);
                break;
            case "pokpok":
                _HandleAnyoneReceivesLuckyCards(jData);
                break;
            case "timeout":
                _HandleAnyoneTimeOut(jData);
                break;
            case "bc":
                _HandleAnyoneDrawsCard(jData);
                break;
            case "finish":
                _HandleFinishGame(jData);
                break;
            case "tip":
                HandlerTip(jData);
                break;
        }
    }
    public override void setGameInfo(int m, int id = 0, int maxBett = 0)
    {
        base.setGameInfo(m, id, maxBett);
        if (_BetValues.Count > 0) return;
        List<int> coefficients = new() { 1, 5, 10, 50, 100 };
        foreach (int coefficient in coefficients) _BetValues.Add(agTable * coefficient);
    }
    protected override void updatePositionPlayerView()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerView.transform.localPosition = listPosView[i];
            players[i].updateItemVip(players[i].vip);
        }
    }
    public override void handleCTable(string strData)
    {
        base.handleCTable(strData);
    }
    public override void handleCCTable(JObject data)
    {
        if (_WaitForFinishCompleteCb != null) _WaitForFinishCompleteCb += () => base.handleCCTable(data);
        else base.handleCCTable(data);
    }
    public override void handleVTable(string strData)
    {
        stateGame = STATE_GAME.VIEWING;
        JObject data = JObject.Parse(strData);
        setGameInfo(m: (int)data["M"], id: (int)data["Id"], maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        JArray dataDealerCards = (JArray)data["ArrDealer"];
        List<int> dealerCardCodes = new();
        foreach (JToken item in dataDealerCards) dealerCardCodes.Add((int)item);
        _DistributeCardsToAPlayer(m_DealerPVL89, dealerCardCodes, (int)data["rateDealer"], (int)data["scoreDealer"]);
        for (int i = 0; i < players.Count; i++) Destroy(players[i].playerView.gameObject);
        players.Clear();
        thisPlayer = new()
        {
            playerView = createPlayerView(),
            id = User.userMain.Userid,
            namePl = User.userMain.Username,
            displayName = User.userMain.displayName,
            ag = User.userMain.AG,
            agBet = 0,
            vip = User.userMain.VIP,
            avatar_id = User.userMain.Avatar,
            is_ready = true,
        };
        thisPlayer.fid = User.userMain.Tinyurl.IndexOf("fb.") != -1 ? User.userMain.Tinyurl.Substring(3) : thisPlayer.fid;
        thisPlayer.playerView.setDark(true);
        players.Add(thisPlayer);
        JArray dataPlayers = (JArray)data["ArrP"];
        for (int i = 0; i < dataPlayers.Count; i++)
        {
            Player player = new();
            readDataPlayer(player, (JObject)dataPlayers[i]);
            player.playerView = createPlayerView();
            player.agBet = (int)dataPlayers[i]["AGC"];
            players.Add(player);
        }
        for (int i = 0; i < players.Count; i++)
        {
            PlayerViewLucky89 pv = (PlayerViewLucky89)players[i].playerView;
            players[i].updatePlayerView();
            pv.SetBetPosition(i).ShowHideBetChips(players[i].agBet > 0, players[i].agBet);
        }
        updatePositionPlayerView();
        for (int i = 0; i < dataPlayers.Count; i++)
        {
            JArray dataPlayerCards = (JArray)dataPlayers[i]["Arr"];
            List<int> playerCardCodes = new();
            foreach (JToken item in dataPlayerCards) playerCardCodes.Add((int)item);
            _DistributeCardsToAPlayer((PlayerViewLucky89)players[i + 1].playerView, playerCardCodes, (int)dataPlayers[i]["rate"], (int)dataPlayers[i]["score"]);
        }
    }
    public override void handleSTable(string strData)
    {
        _WaitForFinishCompleteCb = () =>
        {
            JObject data = JObject.Parse(strData);
            setGameInfo(m: (int)data["M"], id: (int)data["Id"], maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
            for (int i = 0; i < players.Count; i++) Destroy(players[i].playerView.gameObject);
            players.Clear();
            JArray dataPlayers = (JArray)data["ArrP"];
            for (int i = 0; i < dataPlayers.Count; i++)
            {
                Player player = new();
                readDataPlayer(player, (JObject)dataPlayers[i]);
                player.playerView = createPlayerView();
                if (player.id == User.userMain.Userid)
                {
                    thisPlayer = player;
                    players.Insert(0, thisPlayer);
                }
                else players.Add(player);
            }
            for (int i = 0; i < players.Count; i++)
            {
                PlayerViewLucky89 pv = (PlayerViewLucky89)players[i].playerView;
                players[i].updatePlayerView();
                pv.SetBetPosition(i).ShowHideBetChips(players[i].agBet > 0, players[i].agBet).HideAllCards().UpdateCardsParentPositionAndRotation();
            }
            updatePositionPlayerView();
        };
        if (stateGame != STATE_GAME.VIEWING)
        { // có trường hợp mới vào view vtable mà stable trả về ngay sau finish nếu chạy luôn stable sẽ không diễn finish
            _WaitForFinishCompleteCb?.Invoke();
            _WaitForFinishCompleteCb = null;
        }
        stateGame = STATE_GAME.WAITING;
    }
    public override void handleJTable(string strData)
    {
        if (_WaitForFinishCompleteCb != null) _WaitForFinishCompleteCb += () => base.handleJTable(strData);
        else base.handleJTable(strData);
    }
    public override void handleRJTable(string strData)
    {
        stateGame = STATE_GAME.PLAYING;
        _ShowAnimOnBegin(false);
        JObject data = JObject.Parse(strData);
        setGameInfo(m: (int)data["M"], id: (int)data["Id"], maxBett: data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        JArray dataDealerCards = (JArray)data["ArrDealer"];
        for (int i = 0; i < players.Count; i++) Destroy(players[i].playerView.gameObject);
        players.Clear();
        JArray dataPlayers = (JArray)data["ArrP"];
        for (int i = 0; i < dataPlayers.Count; i++)
        {
            Player player = new();
            readDataPlayer(player, (JObject)dataPlayers[i]);
            player.playerView = createPlayerView();
            player.agBet = (int)dataPlayers[i]["AGC"];
            if (player.id == User.userMain.Userid)
            {
                thisPlayer = player;
                players.Insert(0, player);
            }
            else players.Add(player);
        }
        for (int i = 0; i < players.Count; i++)
        {
            PlayerViewLucky89 pv = (PlayerViewLucky89)players[i].playerView;
            players[i].updatePlayerView();
            pv.SetBetPosition(i).ShowHideBetChips(players[i].agBet > 0, players[i].agBet).HideAllCards().UpdateCardsParentPositionAndRotation();
        }
        updatePositionPlayerView();
        List<DataPlayer> dps = new();
        bool distributeCards = false;
        for (int i = 0; i < dataPlayers.Count; i++)
        {
            JArray dataPlayerCards = (JArray)dataPlayers[i]["Arr"];
            DataPlayer dp = new()
            {
                PlayerP = players.Find(x => x.id == (int)dataPlayers[i]["id"]),
                rate = (int)dataPlayers[i]["rate"],
                score = (int)dataPlayers[i]["score"],
            };
            foreach (JToken item in dataPlayerCards) dp.cardCodes.Add((int)item);
            dps.Add(dp);
            if (dp.PlayerP == thisPlayer)
                foreach (int num in dp.cardCodes)
                    if (num > 0)
                    {
                        distributeCards = true;
                        break;
                    }
        }
        if (distributeCards)
        {
            List<int> dealerCardCodes = new();
            foreach (JToken item in dataDealerCards) dealerCardCodes.Add((int)item);
            _DistributeCardsToAPlayer(m_DealerPVL89, dealerCardCodes, (int)data["rateDealer"], (int)data["scoreDealer"]);
            foreach (DataPlayer dp in dps)
            {
                PlayerViewLucky89 playerView = (PlayerViewLucky89)(players.Find(x => x == dp.PlayerP).playerView);
                _DistributeCardsToAPlayer(playerView, dp.cardCodes, dp.rate, dp.score);
            }
        }
    }
    public override void handleLTable(JObject data)
    {
        if (_WaitForFinishCompleteCb != null) _WaitForFinishCompleteCb += () => base.handleLTable(data);
        else base.handleLTable(data);
    }
    private void _HandleStartGame(JObject data)
    {
        StartCoroutine(handleData());
        IEnumerator handleData()
        {
            playSound(SOUND_GAME.START_GAME);
            stateGame = STATE_GAME.PLAYING;
            m_DealerPVL89.ShowHideBetChips(false).ShowAnimResult(false, 0).ShowScore(false, 0).ShowRate(0).HideAllCards();
            foreach (Player p in players) ((PlayerViewLucky89)p.playerView).ShowHideBetChips(false).ShowAnimResult(false, 0).ShowScore(false, 0).ShowRate(0).HideAllCards();
            m_Actions.SetActive(false);
            _SetTickDraw(true, 0);
            yield return new WaitForSeconds((int)m_BeginGameSG.SkeletonData.FindAnimation(_ShowAnimOnBegin(true)).Duration);
            foreach (Player player in players)
            {
                if (player.id == User.userMain.Userid) player.setTurn(true, (float)data["T"] / 1000, timeVibrate: 3f);
                else player.setTurn(true, (float)data["T"] / 1000, timeVibrate: -1f);
            }
            for (int i = 0; i < m_BetOptionTMPs.Count; i++)
            {
                Transform tf = m_BetOptionTMPs[i].transform;
                m_BetOptionTMPs[i].text = i == m_BetOptionTMPs.Count - 1 ? "Max Bet" : Config.FormatMoney3(_BetValues[i]);
                tf.parent.gameObject.SetActive(true);
                tf.DOLocalJump(tf.localPosition, 20f, 1, .1f);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
    private void _HandleAnyoneBets(JObject data)
    {
        playSound(SOUND_GAME.CLICK);
        Player player = players.Find(x => x.namePl.Equals((string)data["N"]));
        if (player == null) return;
        int betChips = (int)data["AG"];
        ((PlayerViewLucky89)player.playerView).ShowHideBetChips(true, betChips);
        player.setTurn(false);
        player.ag -= ((PlayerViewLucky89)player.playerView).GetBetValue();
        player.updatePlayerView();
        if (player == thisPlayer) foreach (TextMeshProUGUI tmp in m_BetOptionTMPs) tmp.transform.parent.gameObject.SetActive(false);
    }
    private void _HandleReceiveMyCards(JObject data)
    {
        StartCoroutine(handleData());
        //======================================================
        void getAllACard(int myCardCode, bool updateCardsParent)
        {
            StartCoroutine(_DrawCard(m_DealerPVL89, 0));
            if (updateCardsParent) m_DealerPVL89.UpdateCardsParentPositionAndRotation();
            foreach (Player player in players)
            {
                bool isMe = player == thisPlayer;
                PlayerViewLucky89 playerView = (PlayerViewLucky89)player.playerView;
                StartCoroutine(_DrawCard(playerView, isMe ? myCardCode : 0));
                if (updateCardsParent) playerView.UpdateCardsParentPositionAndRotation();
            }
        }
        IEnumerator handleData()
        { // delay tổng bao nhiêu lâu thì HandleAnyoneReceivesLuckyCards() phải delay bấy nhiêu tránh xung đột tween rotate
            JArray dataMyCards = (JArray)data["arr"];
            getAllACard((int)dataMyCards[0], false);
            _SetTickDraw(true, 0);
            yield return new WaitForSeconds(CARD_FLYING_DURATION);
            getAllACard((int)dataMyCards[1], true);
            yield return new WaitForSeconds(CARD_FLYING_DURATION);
            PlayerViewLucky89 thisPVL89 = (PlayerViewLucky89)thisPlayer.playerView;
            int myScore = (int)data["score"]; // riêng service lc này thì lucky8 lucky9 sv lại trả về 8, 9
            if (myScore == 8) myScore = (int)SCORE.LUCKY_8;
            else if (myScore == 9) myScore = (int)SCORE.LUCKY_9;
            // m_Actions.SetActive(myScore < (int)SCORE.LUCKY_8);
            thisPVL89.ShowRate((int)data["rate"]).ShowScore(true, myScore);
        }
    }
    private void _HandleAnyoneReceivesLuckyCards(JObject data)
    {
        StartCoroutine(handleData());
        //======================================================
        IEnumerator handleData()
        {
            yield return new WaitForSeconds(2 * CARD_FLYING_DURATION);
            JArray dataLuckyPlayers = JArray.Parse((string)data["data"]);
            bool isDealerLucky = false, isThisPlayerLucky = false;
            foreach (JToken dataLucky in dataLuckyPlayers)
            {
                Player playerP = players.Find(x => x.namePl.Equals((string)dataLucky["N"]));
                PlayerViewLucky89 pvl89 = playerP == null ? m_DealerPVL89 : (PlayerViewLucky89)playerP.playerView;
                JArray dataCards = (JArray)dataLucky["arr"];
                List<Card> cardCs = pvl89.GetListCards();
                for (int i = 0; i < dataCards.Count; i++) _RevealACard(cardCs[i], (int)dataCards[i], cardCs[i].transform.localEulerAngles);
                int score = (int)dataLucky["score"];
                pvl89.ShowRate((int)dataLucky["rate"]).ShowScore(true, score);
                if (score >= (int)SCORE.LUCKY_8)
                {
                    if (pvl89 == m_DealerPVL89) isDealerLucky = true;
                    else if (playerP.id == User.userMain.Userid) isThisPlayerLucky = true;
                }
            }
            if (stateGame == STATE_GAME.VIEWING) yield break;
            m_Actions.SetActive(!isDealerLucky && !isThisPlayerLucky);
        }
    }
    private void _HandleAnyoneTimeOut(JObject data)
    {
        StartCoroutine(handleData());
        //======================================================
        IEnumerator handleData()
        {
            Player player = players.Find(x => x.namePl.Equals((string)data["NN"]));
            float time = (float)data["T"] / 1000;
            if (player != null)
            {
                if (player == thisPlayer)
                {
                    float timeVibrate = -1f;
                    if (_DrawACard == true) SocketSend.SendDrawACardLucky89(1);
                    else if (_DrawACard == false) SocketSend.SendDrawACardLucky89(0);
                    else timeVibrate = 3f;
                    player.setTurn(true, time, timeVibrate: timeVibrate);
                    _IsMyDrawTime = true;
                    m_Actions.SetActive(true);
                    _SetTickDraw(false, 0);
                    yield return new WaitForSeconds(time);
                    _IsMyDrawTime = false;
                    m_Actions.SetActive(false);
                }
                else player.setTurn(true, time, timeVibrate: -1f);
            }
        }
    }
    private void _HandleAnyoneDrawsCard(JObject data)
    {
        Player player = players.Find(x => x.namePl.Equals((string)data["N"]));
        bool isDealer = player == null;
        bool isMe = !isDealer && player == thisPlayer;
        PlayerViewLucky89 playerView = isDealer ? m_DealerPVL89 : (PlayerViewLucky89)player.playerView;
        if (!isDealer) player.setTurn(false);
        else m_DealerPVL89.setTurn(false);
        int cardCode = (int)data["C"];
        if (cardCode > 0)
        {
            StartCoroutine(_DrawCard(playerView, isMe ? cardCode : 0));
            playerView.UpdateCardsParentPositionAndRotation();
        }
        if (isMe)
        {
            m_Actions.SetActive(false);
            _SetTickDraw(false, 0);
            playerView.ShowScore(true, (int)data["score"]).ShowRate((int)data["rate"]);
        }
    }
    private void _HandleFinishGame(JObject data)
    {
        StartCoroutine(handleData());
        //======================================================
        IEnumerator handleData()
        {
            HandleData.DelayHandleLeave = 5f;
            stateGame = STATE_GAME.WAITING;
            m_Actions.SetActive(false);
            _SetTickDraw(false, 0);
            List<Action> playerWinCbs = new(), playerLoseCbs = new();
            Action finalDealerCb = null;
            JArray dataResults = JArray.Parse((string)data["data"]);
            foreach (JToken result in dataResults)
            {
                Player player = players.Find(x => x.namePl.Equals((string)result["N"]));
                PlayerViewLucky89 playerView = null;
                if (player == null) playerView = m_DealerPVL89;
                else playerView = (PlayerViewLucky89)player.playerView;
                if ((int)result["S"] < (int)SCORE.LUCKY_8)
                {
                    List<Card> cardCs = playerView.GetListCards();
                    JArray dataCards = (JArray)result["ArrCard"];
                    for (int i = 0; i < dataCards.Count; i++)
                        _RevealACard(cardCs[i], (int)dataCards[i], cardCs[i].transform.localEulerAngles);
                    playerView.ShowScore(true, (int)result["S"]).ShowRate((int)result["rate"]);
                }
                long chips = (long)result["M"];
                playerView.ShowAnimResult(true, chips);
                if (playerView != m_DealerPVL89)
                {
                    if (chips > 0) playerWinCbs.Add(() => StartCoroutine(playerWinChips(player.id, chips, (long)result["AG"])));
                    else if (chips < 0) playerLoseCbs.Add(() => StartCoroutine(playerLoseChips(player.id, chips, (long)result["AG"])));
                    else
                    {
                        player.ag += ((PlayerViewLucky89)player.playerView).GetBetValue();
                        player.updatePlayerView();
                    }
                }
                else finalDealerCb = () => m_DealerPVL89.effectFlyMoney(chips);
            }
            foreach (Action cb in playerLoseCbs) cb.Invoke();
            if (playerLoseCbs.Count > 0) yield return new WaitForSeconds(2 * LOSE_CHIP_DURATION + 1);
            foreach (Action cb in playerWinCbs) cb.Invoke();
            if (playerWinCbs.Count > 0) yield return new WaitForSeconds(3 * WIN_CHIP_DURATION);
            finalDealerCb.Invoke();
            yield return new WaitForSeconds(1);
            _WaitForFinishCompleteCb?.Invoke();
            _WaitForFinishCompleteCb = null;
            checkAutoExit();
        }
        IEnumerator playerLoseChips(int pId, long changedChips, long currentChips)
        {
            if (changedChips >= 0) yield break;
            for (int i = 0; i < 3; i++)
            {
                Transform chipTf = null;
                foreach (Transform childTf in m_ChipsTf)
                    if (!childTf.gameObject.activeSelf)
                    {
                        chipTf = childTf;
                        break;
                    }
                if (chipTf == null) chipTf = Instantiate(m_PrefabChipTf, m_ChipsTf);
                playSound(SOUND_GAME.GET_CHIP);
                chipTf.gameObject.SetActive(true);
                chipTf.position = (Vector2)(players.Find(x => x.id == pId).playerView.transform.position) + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
                chipTf.DOMove(m_DealerPVL89.transform.position, 2 * LOSE_CHIP_DURATION).SetEase(Ease.OutQuad).OnComplete(() => chipTf.gameObject.SetActive(false));
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(LOSE_CHIP_DURATION);
            Player player = players.Find(x => x.id == pId);
            if (player.id == User.userMain.Userid) User.userMain.AG = currentChips;
            player.playerView.effectFlyMoney(changedChips);
            player.ag = currentChips;
            player.updatePlayerView();
        }
        IEnumerator playerWinChips(int pId, long changedChips, long currentChips)
        {
            if (changedChips <= 0) yield break;
            for (int i = 0; i < 3; i++)
            {
                Transform chipTf = null;
                foreach (Transform childTf in m_ChipsTf)
                    if (!childTf.gameObject.activeSelf)
                    {
                        chipTf = childTf;
                        break;
                    }
                if (chipTf == null) chipTf = Instantiate(m_PrefabChipTf, m_ChipsTf);
                playSound(SOUND_GAME.GET_CHIP);
                chipTf.gameObject.SetActive(true);
                chipTf.position = (Vector2)m_DealerPVL89.transform.position;
                chipTf.DOMove((Vector2)m_ChipsTf.position + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)), WIN_CHIP_DURATION)
                    .SetEase(Ease.OutQuart).OnComplete(async () =>
                    {
                        await Awaitable.WaitForSecondsAsync(WIN_CHIP_DURATION);
                        chipTf.DOMove((Vector2)(players.Find(x => x.id == pId).playerView.transform.position), WIN_CHIP_DURATION)
                            .OnComplete(() => chipTf.gameObject.SetActive(false));
                    });
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(3 * WIN_CHIP_DURATION);
            Player player = players.Find(x => x.id == pId);
            if (player.id == User.userMain.Userid) User.userMain.AG = currentChips;
            player.playerView.effectFlyMoney(changedChips);
            playSound(SOUND_GAME.REWARD);
            player.ag = currentChips;
            player.updatePlayerView();
        }
    }
    public override void HandlerTip(JObject data)
    {
        if (User.userMain.AG < agTable) return;
        int chips = (int)data["AGTip"];
        thisPlayer.playerView.effectFlyMoney(-chips);
        User.userMain.AG -= chips;
        thisPlayer.ag = User.userMain.AG;
        thisPlayer.updatePlayerView();
        m_DealerPVL89.effectFlyMoney(chips);
        m_DealerSG.AnimationState.SetAnimation(0, "kiss", false);
        StartCoroutine(ShowThanksDialog());

        IEnumerator ShowThanksDialog()
        {
            GameObject parentObject = m_TipChipsTMP.transform.parent.gameObject;
            m_TipChipsTMP.text = Config.FormatNumber(chips);
            string playerName = (string)data["N"];
            m_TipThanksTMP.text = (playerName.Length >= 7 ? ((string)data["N"]).Substring(0, 7) + "..., " : playerName + ", ") + Globals.Config.getTextConfig("tip_thanks_1");
            parentObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            parentObject.SetActive(false);
        }
    }
    private string _ShowAnimOnBegin(bool isStart = true)
    {
        string animName = isStart ? "start" : "continue";
        m_BeginGameSG.gameObject.SetActive(true);
        m_BeginGameSG.AnimationState.SetAnimation(0, animName, false);
        return animName;
    }
    private void _SetTickDraw(bool show, int draw)
    {
        m_TickDontDraw.transform.parent.gameObject.SetActive(show);
        m_TickDraw.transform.parent.gameObject.SetActive(show);
        m_TickDraw.SetActive(draw > 0);
        m_TickDontDraw.SetActive(draw < 0);
        if (show && draw > 0) _DrawACard = true;
        else if (show && draw < 0) _DrawACard = false;
        else _DrawACard = null;
    }
    private void _RevealACard(Card cardC, int cardCode, Vector3 targetRotV3)
    {
        cardC.DOComplete();
        if (cardCode <= 0)
        {
            cardC.setTextureWithCode(0);
            cardC.transform.DOLocalRotate(targetRotV3, CARD_ROTATING_DURATION).SetEase(Ease.InQuad);
        }
        else
        {
            playSound(SOUND_GAME.CARD_FLIP_1);
            cardC.transform.DOLocalRotate(new Vector3(0, 90, 0) + targetRotV3, CARD_ROTATING_DURATION).SetEase(Ease.InQuad).OnComplete(() =>
            {
                cardC.setTextureWithCode(cardCode);
                cardC.transform.DOLocalRotate(targetRotV3, CARD_ROTATING_DURATION).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    cardC.transform.localEulerAngles = new(cardC.transform.localEulerAngles.x, 0, cardC.transform.localEulerAngles.z);
                });
            });
        }
    }
    private void _MoveACard(Transform cardTf, Vector2 targetPosV2)
    {
        cardTf.DOComplete();
        cardTf.DOLocalMove(targetPosV2, CARD_FLYING_DURATION).SetEase(Ease.OutQuad);
    }
    private IEnumerator _DrawCard(PlayerViewLucky89 playerView, int cardCode = 0)
    {
        playSound(SOUND_GAME.CARD_FLIP_1);
        Card cardC = playerView.GetACard();
        if (cardC == null) yield break;
        Transform cardParentTf = cardC.transform.parent;
        RectTransform cardRT = cardC.GetComponent<RectTransform>();
        Vector2 targetPosV2 = cardRT.anchoredPosition;
        Vector3 targetRotV3 = cardRT.transform.localEulerAngles;
        cardRT.SetParent(transform);
        cardRT.anchoredPosition = Vector2.zero;
        cardRT.localRotation = Quaternion.identity;
        cardRT.gameObject.SetActive(true);
        cardRT.SetParent(cardParentTf);
        _MoveACard(cardRT, targetPosV2);
        _RevealACard(cardC, cardCode, targetRotV3);
        yield return new WaitForSeconds(CARD_FLYING_DURATION);
    }
    private void _DistributeCardsToAPlayer(PlayerViewLucky89 playerView, List<int> codes, int rate, int score)
    {
        playSound(SOUND_GAME.DISPATCH_CARD);
        playerView.HideAllCards();
        List<Card> cardCs = playerView.GetListCards();
        for (int i = 0; i < cardCs.Count; i++) if (i < codes.Count) StartCoroutine(_DrawCard(playerView, codes[i]));
        int totalCode = 0;
        foreach (int code in codes) totalCode += code;
        playerView.UpdateCardsParentPositionAndRotation().ShowRate(totalCode > 0 ? rate : 0).ShowScore(totalCode > 0, score);
    }
    protected override void Start()
    {
        base.Start();
        m_BeginGameSG.AnimationState.Complete += (x) => m_BeginGameSG.gameObject.SetActive(false);
        m_BeginGameSG.gameObject.SetActive(false);
        m_DealerSG.AnimationState.Complete += (x) => m_DealerSG.AnimationState.SetAnimation(0, "normal", true);
    }
    private class DataPlayer
    {
        public List<int> cardCodes = new();
        public Player PlayerP;
        public int rate, score;
    }
}
