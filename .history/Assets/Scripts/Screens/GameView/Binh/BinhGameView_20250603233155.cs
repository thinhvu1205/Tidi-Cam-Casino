using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Linq;
using Globals;

public class BinhGameView : GameView
{
    private enum TYPE_CARDS_PUSOY
    {
        NONE = -1, HIGH_CARD, PAIR, TWO_PAIR, THREE_OF_A_KIND, STRAIGHT, FLUSH, FULL_HOUSE, FOUR_OF_A_KIND,
        STRAIGHT_FLUSH, THREE_FLUSHES, THREE_STRAIGHT, SIX_PAIRS, SAME_COLOR, DRAGON, GRAND_DRAGON
    }
    [SerializeField] private List<GameObject> m_TotalPoints, m_BurnedIcons;
    [SerializeField] private List<Image> m_RankImgs, m_SpecialRankImgs, m_HandImgs, m_HintRankImgs;
    [SerializeField] private List<TextMeshProUGUI> m_JackPotNumberTMPs;
    [SerializeField] private List<Sprite> m_RankImageSs, m_CheckIconSs, m_HandBgSs, m_ButtonSs;
    [SerializeField] private List<SkeletonGraphic> m_BurnedCardsSG;
    [SerializeField] private List<Vector2> m_CardsPosV2s;
    [SerializeField]
    private GameObject
        m_Countdown, m_Cards, m_Cards1, m_Chips, m_TimeRemain, m_ScoreBg, m_SortCards, m_Star, m_IconBomb;
    [SerializeField] private Image m_BgTimeRemainImg, m_CheckHand1Img, m_CheckHand2Img, m_CheckHand3Img, m_TextSpecialImg;
    [SerializeField] private Button m_RearrangeBtn, m_SwapHandsBtn, m_DeclareBtn;
    [SerializeField]
    private TextMeshProUGUI
        m_SpecialNameTMP, m_SortHand1TMP, m_SortHand2TMP, m_SortHand3TMP, m_Hand1TMP, m_Hand2TMP, m_Hand3TMP,
        m_ScoreHand1TMP, m_ScoreHand2TMP, m_ScoreHand3TMP, m_JackPotNumTMP;
    [SerializeField] private SkeletonGraphic m_StartSG, m_FinishSG, m_SpecialSG, m_EndSG, m_TankSG, m_BombSG;
    [SerializeField] private Avatar m_SpecialAvatarA;
    [SerializeField] private Card m_PrefabCardC;
    [SerializeField] private ChipBet m_PrefabChipCB;
    [SerializeField] private Animation m_JackpotAnimA;
    [SerializeField] private BinhChosenGroupCards m_ChosenCardsBCGC;
    private const float SCALE_CARD = 0.55f, CARD_HEIGHT = 198f, CARD_WIDTH = 147f, ORG_CARD = 0.8f;
    private List<List<List<Card>>> _HintRankCs = new();
    private List<Card> _CardPoolCs = new(), _FakeCardOnDragCs = new();
    private List<ChipBet> _ChipPoolCBs = new();
    private List<JObject> _DataResultJOs = new();
    private List<int> _ScoreBonus = new();
    private BinhLogicManager _LogicManagerBLM = new();
    private JArray _DataFinishJA = new();
    private Vector2 _CardsCenterV2;
    private TextMeshProUGUI _TimeTMP;
    private Coroutine _ShowArrangingC;
    private int[] _StateButtonIds = new int[5];
    private float _ScreenLeftClamp, _ScreenRightClamp, _ScreenBotClamp, _ScreenTopClamp;
    private bool _IsExit, _IsFinish, _CanClear = true, _CanSortCard;

    #region Button
    public void onClickRuleJP()
    {
        UIManager.instance.openRuleJPBinh();
    }
    public void onClickShowCard()
    {
        SoundManager.instance.playEffectFromPath(SOUND_GAME.CLICK);
        _IsExit = false;
        m_SortCards.SetActive(false);
        List<int> vtCard = new();
        thisPlayer.vectorCard = sortChi(thisPlayer.vectorCard);
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++) vtCard.Add(thisPlayer.vectorCard[i].code);
        SocketSend.sendBinhShowCard(vtCard, _IsExit);
    }
    public void onClickDoiChi()
    {
        SoundManager.instance.playEffectFromPath(SOUND_GAME.CLICK);
        cleanMarkCard();
        m_SwapHandsBtn.interactable = false;
        for (int i = 0; i < 5; i++)
        {
            int id1 = 3 + i, id2 = 8 + i;
            thisPlayer.vectorCard[id1].transform.DOLocalMove(getPositionSortCard(id2), 0.2f);
            if (i < 4) thisPlayer.vectorCard[id2].transform.DOLocalMove(getPositionSortCard(id1), 0.2f);
            else thisPlayer.vectorCard[id2].transform.DOLocalMove(getPositionSortCard(id1), 0.2f).OnComplete(() => { m_SwapHandsBtn.interactable = true; });
            Card temp = thisPlayer.vectorCard[id1];
            thisPlayer.vectorCard[id1] = thisPlayer.vectorCard[id2];
            thisPlayer.vectorCard[id2] = temp;
        }
        splitChi(thisPlayer);
        updateTextBinh();
    }
    public void onClickHint(string customEventData)
    {
        int index = int.Parse(customEventData);
        if (_StateButtonIds[index] == 0) return;
        showIcStar(true, index);
        showStar(_HintRankCs[index]);
    }
    public void onClickDeclare()
    {
        _IsExit = false;
        SoundManager.instance.playEffectFromPath(SOUND_GAME.CLICK);
        List<int> vtCard = new();
        thisPlayer.vectorCard = sortChi(thisPlayer.vectorCard);
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++) vtCard.Add(thisPlayer.vectorCard[i].code);
        SocketSend.sendBinhDeclare(vtCard, _IsExit);
    }
    public void onClickXepLai()
    {
        _IsExit = false;
        SoundManager.instance.playEffectFromPath(SOUND_GAME.CLICK);
        SocketSend.sendBinhXepLai();
    }
    #endregion
    public void ProcessResponseData(JObject jData)
    {
        switch ((string)jData["evt"])
        {
            case "countdowntostart":
                _HandleCountdownToStart(jData);
                break;
            case "lc":
                _HandleLC(jData);
                break;
            case "declare":
                _HandleDeclare(jData);
                break;
            case "fsc":
                _HandleCompareHands(jData);
                break;
            case "ufsc":
                _HandleReshuffle(jData);
                break;
            case "finish":
                _HandleFinishGame(jData);
                break;
            case "am":
                break;
        }
    }
    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        JObject data = JObject.Parse(objData);
        // JArray ArrP = getJArray(data, "ArrP");
        if (data["bonusScore"] != null) _ScoreBonus = getListInt(data, "bonusScore");
        if (data["result"] != null)
        {
            _IsFinish = true;
            ViewIng((JObject)data["result"]);
        }
    }
    public override void handleSTable(string objData)
    {
        resetGameDisplay();
        base.handleSTable(objData);
        JObject data = JObject.Parse(objData);
        // JArray ArrP = getJArray(data, "ArrP");
        if (data["bonusScore"] != null) _ScoreBonus = getListInt(data, "bonusScore");
        if (data["result"] != null)
        {
            _IsFinish = true;
            ViewIng((JObject)data["result"]);
        }
    }
    public override void handleVTable(string objData)
    {
        base.handleVTable(objData);
        connectGame(objData);
        JObject data = JObject.Parse(objData);
        // JArray ArrP = getJArray(data, "ArrP");
        if (data["bonusScore"] != null) _ScoreBonus = getListInt(data, "bonusScore");
        if (data["result"] != null)
        {
            _IsFinish = true;
            ViewIng((JObject)data["result"]);
        }
        else
        {
            countDown((int)data["T"]);
            initPlayerCard();
            _IsFinish = false;
        }
    }
    public override void handleRJTable(string objData)
    {
        base.handleRJTable(objData);
        connectGame(objData);
        JObject data = JObject.Parse(objData);
        // JArray ArrP = getJArray(data, "ArrP");
        if (data["bonusScore"] != null) _ScoreBonus = getListInt(data, "bonusScore");
        if (data["result"] != null)
        {
            _IsFinish = true;
            ViewIng((JObject)data["result"]);
        }
        else
        {
            countDown((int)data["T"] - 5);
            initPlayerCard();
            _IsFinish = false;
        }
    }
    public override void handleFinishGame()
    {
        base.handleFinishGame();
        HandleGame.nextEvt();
        checkAutoExit();
    }
    private void _HandleCountdownToStart(JObject data)
    {
        cleanTable();
        for (int i = 0; i < players.Count; i++) clearAllCard(players[i]);
        _CanClear = false;
        stateGame = STATE_GAME.WAITING;
        SoundManager.instance.playEffectFromPath(SOUND_GAME.START_GAME);
        int timeStart = (int)data["data"];
        resetGameDisplay();
        m_Countdown.SetActive(true);
        TextMeshProUGUI countDownTMP = m_Countdown.transform.GetComponentInChildren<TextMeshProUGUI>();
        countDownTMP.text = timeStart.ToString();
        StartCoroutine(countDown());
        _TimeTMP.color = Color.green;

        IEnumerator countDown()
        {
            while (timeStart - 1 >= 0)
            {
                countDownTMP.text = timeStart.ToString();
                checkAutoExit();
                timeStart--;
                yield return new WaitForSeconds(1f);
                if (timeStart <= 0) m_Countdown.SetActive(false);
            }
        }
    }
    private async void _HandleLC(JObject data)
    {
        SoundManager.instance.playEffectFromPath(SOUND_GAME.START_GAME);
        updatePositionPlayerView();
        m_StartSG.gameObject.SetActive(true);
        m_StartSG.AnimationState.SetAnimation(0, "start", false);
        m_StartSG.Initialize(true);
        m_StartSG.AnimationState.Complete += delegate { m_StartSG.gameObject.SetActive(false); };
        stateGame = STATE_GAME.PLAYING;
        _IsFinish = false;
        int time = (int)data["T"];
        // get this player's cards
        string strArr = (string)data["data"];
        strArr = strArr.Remove(0, 1);
        strArr = strArr.Remove(strArr.Length - 1);
        List<int> arr = strArr?.Split(',').Select(Int32.Parse).ToList();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].vectorCard.Clear();
            Vector2 posCard = m_CardsPosV2s[getDynamicIndex(getIndexOf(players[i]))];
            for (int j = 0; j < arr.Count; j++)
            {
                Card card = getCard();
                card.transform.localScale = new Vector2(SCALE_CARD, SCALE_CARD);
                card.SetPivot(.5f, -.5f);
                card.gameObject.SetActive(false);
                Vector2 pos = getPositionPlayerCard(j, posCard);
                Quaternion newRotation = Quaternion.Euler(0, 0, j < 3 ? 15 : 30);
                card.transform.SetLocalPositionAndRotation(pos, newRotation);
                if (players[i] == thisPlayer)
                {
                    card.decodeCard(arr[j]);
                    card.setTextureWithCode(arr[j]);
                }
                else card.setTextureWithCode(0);
                players[i].vectorCard.Add(card);
            }
        }
        await Awaitable.WaitForSecondsAsync(1.5f);
        chiabai();
        countDown(time - 4);
        await Awaitable.WaitForSecondsAsync(1f);
        initSortLayer();
    }
    private void _HandleDeclare(JObject data)
    {
        if (getPlayer(getString(data, "Name")) == thisPlayer)
        {
            moveBack();
            cleanMarkCard();
            hideSortLayer();
        }
    }
    private void _HandleCompareHands(JObject data)
    {
        Player playerP = getPlayer(getString(data, "Name"));
        if (playerP != thisPlayer)
        {
            for (int i = 0; i < playerP.vectorCard.Count; i++)
            {
                Card cardC = playerP.vectorCard[i];
                cardC.transform.DOScale(SCALE_CARD + 0.05f, 0.3f).SetEase(Ease.InOutSine).SetDelay(0.2f)
                    .OnComplete(() => { cardC.transform.DOScale(SCALE_CARD, 0.3f).SetEase(Ease.InOutSine); });
            }
            playerP.mauBinhSoBai = true;
            int index = getDynamicIndex(getIndexOf(playerP));
            m_RankImgs[index].gameObject.SetActive(true);
            m_RankImgs[index].sprite = m_RankImageSs[13];
            m_RankImgs[index].SetNativeSize();
        }
        else
        {
            moveBack();
            cleanMarkCard();
            hideSortLayer();
        }
    }
    private void _HandleReshuffle(JObject data)
    {
        Player playerP = getPlayer(getString(data, "Name"));
        if (playerP != thisPlayer)
        {
            playerP.mauBinhSoBai = false;
            int indexPos = getDynamicIndex(getIndexOf(playerP));
            m_RankImgs[indexPos].gameObject.SetActive(true);
            m_RankImgs[indexPos].sprite = m_RankImageSs[13];
            m_RankImgs[indexPos].SetNativeSize();
        }
        else initSortLayer();
    }
    private async void _HandleFinishGame(JObject data)
    {
        _IsExit = true;
        SoundManager.instance.playEffectFromPath(SOUND_GAME.ALERT);
        if (m_SortCards.activeSelf) hideSortLayer();
        stopCountDown();
        m_RearrangeBtn.gameObject.SetActive(false);
        m_FinishSG.gameObject.SetActive(true);
        m_FinishSG.AnimationState.SetAnimation(0, "compare", false);
        m_FinishSG.Initialize(true);
        m_FinishSG.AnimationState.Complete += delegate { m_FinishSG.gameObject.SetActive(false); };
        for (int i = 0; i < 4; i++) m_RankImgs[i].gameObject.SetActive(false);
        JArray listPl = JArray.Parse(getString(data, "data"));
        _DataFinishJA = listPl;
        _DataResultJOs = new();
        for (int i = 0; i < listPl.Count; i++)
        {
            JObject jpl = (JObject)listPl[i];
            string name = getString(jpl, "N");
            Player playerP = getPlayer(name);
            if (playerP == null) continue;
            playerP.mauBinh_M = (int)jpl["M"];
            playerP.mauBinh_BL = getBool(jpl, "BL");
            playerP.mauBinh_MB = (int)jpl["MB"];
            playerP.scoreChi1 = (int)jpl["hesochi1"];
            playerP.scoreChi2 = (int)jpl["hesochi2"];
            playerP.scoreChi3 = (int)jpl["hesochi3"];
            playerP.bonusChi1 = (int)jpl["bonuschi1"];
            playerP.bonusChi2 = (int)jpl["bonuschi2"];
            playerP.bonusChi3 = (int)jpl["bonuschi3"];
            playerP.jcards = getListInt(jpl, "ArrCard");
            for (int j = 0; j < playerP.jcards.Count; j++) playerP.vectorCard[j].decodeCard(playerP.jcards[j]);
            JObject playerData = new()
            {
                ["name"] = name,
                ["ArrWin"] = getJArray(jpl, "ArrWin")
            };
            _DataResultJOs.Add(playerData);
            int jackPot = (int)jpl["jackPot"];
            if (jackPot > 0 && playerP == thisPlayer)
            {
                playerP.playerView.chipJackpot = jackPot;
                StartCoroutine(ShowJackpotWin(jackPot));
            }
        }
        int num = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == thisPlayer && stateGame == STATE_GAME.VIEWING) continue;
            if (!players[i].mauBinh_BL && players[i].mauBinh_MB == 0) num++;
        }
        // Prepare finish
        await Awaitable.WaitForSecondsAsync(1.5f);
        m_StartSG.gameObject.SetActive(false);
        if (stateGame == STATE_GAME.PLAYING)
        {
            for (int i = 0; i < thisPlayer.vectorCard.Count; i++)
            {
                Card cardC = thisPlayer.vectorCard[i];
                cardC.transform.DOScale(new Vector2(0, SCALE_CARD), 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    cardC.setTextureWithCode(0);
                    cardC.transform.DOScale(new Vector2(SCALE_CARD, SCALE_CARD), 0.2f).SetEase(Ease.InOutSine);
                });
            }
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            players[i].totalPoint = 0;
            players[i].mauBinhSoBai = false;
            players[i].is_ready = false;
            players[i].vectorCard = sortChi(players[i].vectorCard);
            splitChi(players[i]);
            for (int j = players[i].vectorCard.Count - 1; j >= 0; j--)
                players[i].vectorCard[j].transform.localRotation = Quaternion.Euler(0, 0, setRotationCard(j));
        }
        // showBinhSpecial
        await Awaitable.WaitForSecondsAsync(2f);
        if (num < players.Count)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
                if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) showCardSpecial(players[i]);
            }
            if (thisPlayer.mauBinh_MB > 0) showAnimationSpecialFull();
            await Awaitable.WaitForSecondsAsync(2f);
        }
        if (num == 1)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
                if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
                for (int j = 0; j < players[i].vectorCard.Count; j++)
                {
                    Card cardC = players[i].vectorCard[j];
                    cardC.transform.DOScale(new Vector2(0, SCALE_CARD), 0.05f).SetEase(Ease.InOutSine)
                        .OnComplete(() => { cardC.transform.DOScale(new Vector2(SCALE_CARD, SCALE_CARD), 0.15f).SetEase(Ease.InOutSine); });
                    cardC.setTextureWithCode(cardC.encodeCard());
                }
            }
            await Awaitable.WaitForSecondsAsync(2f);
        }
        if (num > 1)
        {
            if (this != null) showChi3();
            await Awaitable.WaitForSecondsAsync(3.5f);
            if (this != null) showChi2();
            await Awaitable.WaitForSecondsAsync(3.5f);
            if (this != null) showChi1();
            await Awaitable.WaitForSecondsAsync(2f);
            for (int i = 0; i < 4; i++) m_RankImgs[i].gameObject.SetActive(false);
            await Awaitable.WaitForSecondsAsync(2f);
        }
        doEndGameFlow();
    }
    public void UpdateJackPot()
    {
        foreach (TextMeshProUGUI jackPot in m_JackPotNumberTMPs) jackPot.gameObject.SetActive(false);
        string str = UIManager.instance.PusoyJackPot.ToString();
        for (int i = 0; i < 11 - UIManager.instance.PusoyJackPot.ToString().Length; i++) str = "0" + str;
        for (int i = 0; i < str.Length; i++)
        {
            m_JackPotNumberTMPs[i].gameObject.SetActive(true);
            m_JackPotNumberTMPs[i].text = str[i].ToString();
            StartCoroutine(animateJackPot(m_JackPotNumberTMPs[i].gameObject));
        }
    }
    private IEnumerator animateJackPot(GameObject node)
    {
        float duration = 0.15f, time = 0;
        Vector3 initialScale = node.transform.localScale, targetScale = initialScale * 1.2f;
        while (time < duration)
        {
            time += Time.deltaTime;
            node.transform.localScale = Vector3.Lerp(initialScale, targetScale, time / duration);
            yield return null;
        }
        time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            node.transform.localScale = Vector3.Lerp(targetScale, initialScale, time / duration);
            yield return null;
        }
        node.transform.localScale = initialScale;
    }
    void resetGameDisplay()
    {
        for (int i = 0; i < players.Count; i++) clearAllCard(players[i]);
        for (int i = 0; i < 4; i++)
        {
            m_SpecialRankImgs[i].gameObject.SetActive(false);
            m_TotalPoints[i].SetActive(false);
            m_BurnedCardsSG[i].gameObject.SetActive(false);
            m_BurnedIcons[i].gameObject.SetActive(false);
        }
        handleFinishGame();
    }
    public void ViewIng(JObject dataGame)
    {
        _DataFinishJA = JArray.Parse(getString(dataGame, "data").Replace("\\\"", "\\"));
        _DataResultJOs = new();
        for (int i = 0; i < _DataFinishJA.Count; i++)
        {
            JObject jpl = (JObject)_DataFinishJA[i];
            string name = getString(jpl, "N");
            Player playerP = getPlayer(name);
            if (playerP == null) continue;
            playerP.mauBinh_M = (int)jpl["M"];
            playerP.mauBinh_BL = (bool)jpl["BL"];
            playerP.mauBinh_MB = (int)jpl["MB"];
            playerP.scoreChi1 = (int)jpl["hesochi1"];
            playerP.scoreChi2 = (int)jpl["hesochi2"];
            playerP.scoreChi3 = (int)jpl["hesochi3"];
            playerP.bonusChi1 = (int)jpl["bonuschi1"];
            playerP.bonusChi2 = (int)jpl["bonuschi2"];
            playerP.bonusChi3 = (int)jpl["bonuschi3"];
            playerP.ag = (int)jpl["AG"] - playerP.mauBinh_M;
            playerP.setAg();
            List<int> jcards = getListInt(jpl, "ArrCard");
            for (int j = 0; j < jcards.Count; j++) playerP.vectorCard[j].decodeCard(jcards[j]);
            JObject playerData = new()
            {
                ["name"] = name,
                ["ArrWin"] = getJArray(jpl, "ArrWin")
            };
            _DataResultJOs.Add(playerData);
        }
        for (int i = 0; i < players.Count; i++)
        {
            Player playerP = players[i];
            if (stateGame == STATE_GAME.VIEWING && playerP == thisPlayer) continue;
            int idPos = getDynamicIndex(getIndexOf(playerP));
            if (playerP.mauBinh_BL)
            {
                SoundManager.instance.playEffectFromPath(SOUND_GAME.CLOCK_TICK);
                m_BurnedCardsSG[idPos].gameObject.SetActive(true);
                m_BurnedCardsSG[idPos].Initialize(true);
                m_BurnedCardsSG[idPos].AnimationState.SetAnimation(0, "animation", false);
                m_BurnedCardsSG[idPos].AnimationState.Complete += delegate
                {
                    m_BurnedCardsSG[idPos].gameObject.SetActive(false);
                    m_BurnedIcons[idPos].SetActive(true);
                };
            }
            else
            {
                int mb = playerP.mauBinh_MB;
                JObject data = _DataResultJOs.Find(x => getString(x, "name").Equals(playerP.namePl));
                JArray arrWin = getJArray(data, "ArrWin");
                foreach (JToken item in arrWin)
                {
                    int score = (int)item["Score"];
                    playerP.totalPoint += score;
                    Player thisPlayerP = players.Find(x => x.namePl.Equals((string)item["Name"]));
                    if (thisPlayerP == null) continue;
                    thisPlayerP.totalPoint -= score;
                }
                if (mb > 0)
                {
                    int path = -1;
                    if (mb == 15) path = 8;
                    else if (mb == 14) path = 1;
                    else if (mb == 13) path = 14;
                    else if (mb == 12) path = 15;
                    else if (mb == 11) path = 23;
                    else if (mb == 10) path = 20;
                    if (path >= 0)
                    {
                        m_SpecialRankImgs[idPos].gameObject.SetActive(true);
                        m_SpecialRankImgs[idPos].transform.localScale = Vector2.zero;
                        m_SpecialRankImgs[idPos].sprite = m_RankImageSs[path];
                        m_SpecialRankImgs[idPos].transform.DOScale(1f, 0.3f).SetEase(Ease.InOutSine);
                        m_SpecialRankImgs[idPos].SetNativeSize();
                    }
                }
                else playerP.totalPoint += playerP.scoreChi1 + playerP.scoreChi2 + playerP.scoreChi3 + playerP.bonusChi1 + playerP.bonusChi2 + playerP.bonusChi3;
            }
        }
        for (var i = 0; i < players.Count; i++) setPointTotal(players[i], getDynamicIndex(getIndexOf(players[i])));
        initPlayerCard();
        showExchangeMoney();
    }
    public void chiabai()
    {
        SoundManager.instance.playEffectFromPath(SOUND_GAME.CARD_FLIP_1);
        float biggerScale = 1.1f * SCALE_CARD;
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = players[i].vectorCard.Count - 1; j >= 0; j--)
            {
                Card cardC = players[i].vectorCard[j];
                cardC.gameObject.SetActive(true);
                Quaternion newRotation = Quaternion.Euler(0, 0, setRotationCard(j));
                cardC.transform.localScale = new Vector3(SCALE_CARD, SCALE_CARD);
                cardC.transform.DORotate(newRotation.eulerAngles, 0.3f).SetEase(Ease.InOutSine);
                cardC.transform.DOScale(biggerScale, .15f).OnComplete(() => cardC.transform.DOScale(SCALE_CARD, .15f));
            }
        }
        splitChi(thisPlayer);
    }
    public float checkWinAll()
    {
        bool isViewing = stateGame == STATE_GAME.VIEWING;
        if (players.Count <= 3 && isViewing) return 0;
        int sizeSapLang = players.Count - 1;
        if (isViewing) sizeSapLang = players.Count - 2;
        if (sizeSapLang == 1) return 0;
        float time = 0;
        for (int i = 0; i < players.Count; i++)
        {
            JArray data = null;
            for (int j = 0; j < _DataResultJOs.Count; j++)
            {
                if (players[i].namePl.Equals(getString(_DataResultJOs[j], "name")))
                {
                    data = getJArray(_DataResultJOs[j], "ArrWin");
                    break;
                }
            }
            players[i].isSapLang = data != null && data.Count == sizeSapLang;
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].mauBinh_MB > 0 || (players[i] == thisPlayer && isViewing)) continue;
            JArray data = null;
            for (int j = 0; j < _DataResultJOs.Count; j++)
            {
                if (players[i].namePl.Equals(getString(_DataResultJOs[j], "name")))
                {
                    data = getJArray(_DataResultJOs[j], "ArrWin");
                    break;
                }
            }
            if (data == null) continue;
            if (data.Count == sizeSapLang)
            {
                for (int id = 0; id < players.Count; id++)
                {
                    if (players[i] == players[id] || (players[id] == thisPlayer && isViewing)) continue;
                    int point = 0;
                    for (int j = 0; j < data.Count; j++)
                    {
                        JObject winData = (JObject)data[j];
                        Player playerP = getPlayer(getString(winData, "Name"));
                        if (players[id] == playerP)
                        {
                            point = (int)winData["Score"];
                            break;
                        }
                    }
                    if (!players[id].isSapLang)
                    {
                        players[i].totalPoint += point;
                        players[id].totalPoint -= point;
                        int fromPos = getDynamicIndex(getIndexOf(players[i])), toPos = getDynamicIndex(getIndexOf(players[id]));
                        setPointTotal(players[i], fromPos);
                        setPointTotal(players[id], toPos);
                        StartCoroutine(soundBoom(0.4f));
                        SkeletonGraphic animation = Instantiate(m_EndSG, transform);
                        animation.gameObject.SetActive(true);
                        animation.transform.localPosition = m_CardsPosV2s[toPos] + new Vector2(0, -60);
                        animation.Initialize(true);
                        animation.AnimationState.SetAnimation(0, "animation", false);
                        animation.AnimationState.Complete += delegate { Destroy(animation.gameObject); };
                        time = 3.5f;
                    }
                }
            }
        }
        return time;
    }
    private IEnumerator soundBoom(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this != null) SoundManager.instance.playEffectFromPath(SOUND_CHAT.BOOM);
    }
    async Awaitable checkSapHam(float delayTime)
    {
        await Awaitable.WaitForSecondsAsync(delayTime);
        foreach (Player playerP in players)
        {
            if (stateGame == STATE_GAME.VIEWING && playerP == thisPlayer) continue;
            if (playerP.mauBinh_BL || playerP.mauBinh_MB > 0 || playerP.isSapLang) continue;
            List<Player> targetPs = new();
            JArray data = null;
            foreach (JObject playerResult in _DataResultJOs)
            {
                if (playerP.namePl.Equals(getString(playerResult, "name")))
                {
                    data = getJArray(playerResult, "ArrWin");
                    break;
                }
            }
            if (data == null) continue;
            foreach (JToken winData in data) targetPs.Add(getPlayer(getString((JObject)winData, "Name")));
            if (targetPs.Count > 0 && targetPs.Count <= 2)
            {
                createShootingEffect(playerP, targetPs);
                await Awaitable.WaitForSecondsAsync(targetPs.Count == 1 ? 2f : 4f);
            }
        }
        await Awaitable.WaitForSecondsAsync(.5f);
        _CanClear = true;
    }
    async void createShootingEffect(Player shooterP, List<Player> targetPs)
    {
        int startPosId = getDynamicIndex(getIndexOf(shooterP));
        Vector2 posTank = m_CardsPosV2s[startPosId] + new Vector2(0, 100);
        for (int i = 0; i < targetPs.Count; i++)
        {
            int toPos = getDynamicIndex(getIndexOf(targetPs[i]));
            string anim = "";
            if (startPosId == 0)
            {
                switch (toPos)
                {
                    case 3: anim = "A1"; break;
                    case 2: anim = "A2"; break;
                    case 1: anim = "A3"; break;
                }
            }
            else if (startPosId == 1)
            {
                switch (toPos)
                {
                    case 0: anim = "D4"; break;
                    case 3: anim = "D5"; break;
                    case 2: anim = "D6"; break;
                }
            }
            else if (startPosId == 2)
            {
                switch (toPos)
                {
                    case 0: anim = "A2"; break;
                    case 1: anim = "A1"; break;
                    case 3: anim = "A3"; break;
                }
            }
            else if (startPosId == 3)
            {
                switch (toPos)
                {
                    case 0: anim = "D6"; break;
                    case 1: anim = "D5"; break;
                    case 2: anim = "D4"; break;
                }
            }
            if (i != 0) await Awaitable.WaitForSecondsAsync(1.9f);
            SkeletonGraphic animation = Instantiate(m_TankSG, transform);
            animation.gameObject.SetActive(true);
            animation.transform.localPosition = posTank;
            if (startPosId == 1 || startPosId == 2) animation.transform.localRotation = Quaternion.Euler(0, 0, 180);
            animation.Initialize(true);
            animation.AnimationState.SetAnimation(0, anim, false);
            animation.AnimationState.Complete += delegate { StartCoroutine(turnOffAnimTank(1f, animation)); };
            await Awaitable.WaitForSecondsAsync(.5f);
            createBomEffect(shooterP, targetPs[i]);
        }
    }
    private IEnumerator turnOffAnimTank(float delay, SkeletonGraphic animation)
    {
        yield return new WaitForSeconds(delay);
        Destroy(animation.gameObject);
    }
    async void createBomEffect(Player shooterP, Player targetP)
    {
        int fromPos = getDynamicIndex(getIndexOf(shooterP)), toPos = getDynamicIndex(getIndexOf(targetP));
        Vector2 tankV2 = m_CardsPosV2s[fromPos] + new Vector2(0, 100), bombV2 = m_CardsPosV2s[toPos] + new Vector2(0, 100);
        GameObject bomb = Instantiate(m_IconBomb, transform);
        bomb.transform.SetSiblingIndex(bomb.transform.parent.childCount - 2);
        bomb.transform.localPosition = tankV2;
        bomb.SetActive(true);
        bomb.transform.DOLocalMove(bombV2, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            Destroy(bomb);
            SkeletonGraphic bombSG = Instantiate(m_BombSG, transform);
            bombSG.gameObject.SetActive(true);
            bombSG.transform.localPosition = bombV2;
            bombSG.Initialize(true);
            bombSG.AnimationState.SetAnimation(0, "animation", false);
            bombSG.AnimationState.Complete += delegate { StartCoroutine(turnOffAnimBom(0.5f, bombSG)); };
            if (this != null) SoundManager.instance.playEffectFromPath(SOUND_CHAT.BOOM);
        });
        int point = 0;
        foreach (JObject data in _DataResultJOs)
        {
            if (getString(data, "name").Equals(shooterP.namePl))
            {
                JArray arr = getJArray(data, "ArrWin");
                if (arr == null) return;
                if (arr.Count == 0) return;
                JObject arrObj = (JObject)arr[0];
                if (arrObj.ContainsKey("Score") && arrObj["Score"] != null) point = (int)arrObj["Score"];
                else point = 0;
                break;
            }
        }
        await Awaitable.WaitForSecondsAsync(1f);
        shooterP.totalPoint += point;
        targetP.totalPoint -= point;
        setPointTotal(shooterP, fromPos);
        await Awaitable.WaitForSecondsAsync(.5f);
        setPointTotal(targetP, toPos);
    }
    private IEnumerator turnOffAnimBom(float delay, SkeletonGraphic animSG)
    {
        yield return new WaitForSeconds(delay);
        Destroy(animSG.gameObject);
    }
    void showCardSpecial(Player playerP)
    {
        for (int i = 0; i < playerP.vectorCard.Count; i++)
        {
            Card cardC = playerP.vectorCard[i];
            cardC.setTextureWithCode(cardC.encodeCard());
            cardC.transform.DOScale(new Vector2(0, SCALE_CARD), 0.05f).SetEase(Ease.InOutSine)
                .OnComplete(() => { cardC.transform.DOScale(new Vector2(SCALE_CARD, SCALE_CARD), 0.15f).SetEase(Ease.InOutSine); });
        }
        int indexPos = getDynamicIndex(getIndexOf(playerP));
        if (playerP.mauBinh_BL)
        {
            SoundManager.instance.playEffectFromPath(SOUND_GAME.CLOCK_TICK);
            m_BurnedCardsSG[indexPos].gameObject.SetActive(true);
            m_BurnedCardsSG[indexPos].Initialize(true);
            m_BurnedCardsSG[indexPos].AnimationState.SetAnimation(0, "animation", false);
            m_BurnedCardsSG[indexPos].AnimationState.Complete += delegate
            {
                m_BurnedCardsSG[indexPos].gameObject.SetActive(false);
                m_BurnedIcons[indexPos].SetActive(true);
            };
        }
        else
        {
            int mb = playerP.mauBinh_MB, idPos = getDynamicIndex(getIndexOf(playerP)), point = 0;
            for (int i = 0; i < _DataResultJOs.Count; i++)
            {
                if (getString(_DataResultJOs[i], "name").Equals(playerP.namePl))
                {
                    JArray arrWin = getJArray(_DataResultJOs[i], "ArrWin");
                    if (arrWin.Count == 0) return;
                    point = (int)arrWin[0]["Score"];
                    break;
                }
            }
            for (int i = 0; i < players.Count; i++)
            {
                if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
                if (players[i] != playerP)
                {
                    if (players[i].mauBinh_MB > 0)
                    {
                        if (playerP.mauBinh_MB > players[i].mauBinh_MB)
                        {
                            playerP.totalPoint += point;
                            players[i].totalPoint -= point;
                        }
                    }
                    else
                    {
                        playerP.totalPoint += point;
                        players[i].totalPoint -= point;
                    }
                    setPointTotal(playerP, idPos);
                    setPointTotal(players[i], getDynamicIndex(getIndexOf(players[i])));
                }
            }
            int path = -1;
            if (mb == 15) path = 8;
            else if (mb == 14) path = 1;
            else if (mb == 13) path = 14;
            else if (mb == 12) path = 15;
            else if (mb == 11) path = 23;
            else if (mb == 10) path = 20;
            if (path >= 0)
            {
                m_SpecialRankImgs[idPos].gameObject.SetActive(true);
                m_SpecialRankImgs[idPos].transform.localScale = Vector2.zero;
                m_SpecialRankImgs[idPos].sprite = m_RankImageSs[path];
                m_SpecialRankImgs[idPos].transform.DOScale(1f, 0.3f).SetEase(Ease.InOutSine);
                m_SpecialRankImgs[idPos].SetNativeSize();
            }
        }
    }
    void showAnimationSpecialFull()
    {
        SoundManager.instance.playEffectFromPath(SOUND_BINH.WIN_BANKER);
        m_SpecialAvatarA.loadAvatar(thisPlayer.avatar_id, thisPlayer.displayName, thisPlayer.fid);
        int mb = thisPlayer.mauBinh_MB;
        string path = "";
        if (mb == 15) path = "granddragon";
        else if (mb == 14) path = "dragon";
        else if (mb == 13) path = "samecolour";
        else if (mb == 12) path = "6pairs";
        else if (mb == 11) path = "3straights";
        else if (mb == 10) path = "3flushes";
        m_SpecialSG.transform.parent.gameObject.SetActive(true);
        m_SpecialSG.gameObject.SetActive(true);
        m_SpecialSG.Initialize(true);
        m_SpecialSG.AnimationState.SetAnimation(0, path, false);
        m_SpecialSG.AnimationState.Complete += delegate
        {
            m_SpecialSG.gameObject.SetActive(false);
            m_SpecialSG.transform.parent.gameObject.SetActive(false);
        };
        m_SpecialNameTMP.text = thisPlayer.displayName.Length > 20 ? thisPlayer.displayName[..20] + "..." : thisPlayer.displayName;
    }
    async void showChi1()
    {
        if (this != null) SoundManager.instance.playEffectFromPath(thisPlayer.scoreChi1 + thisPlayer.bonusChi1 > 0 ? SOUND_BINH.COMPARE_WIN : SOUND_BINH.COMPARE_LOSE);
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            int totalChiAndBonus = players[i].scoreChi1 + players[i].bonusChi1;
            players[i].totalPoint += totalChiAndBonus;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            Vector2 posV2 = m_CardsPosV2s[idPos];
            int[] zValues = new int[8];
            for (int j = 0; j < 8; j++) zValues[j] = players[i].vectorCard[j].transform.GetSiblingIndex();
            for (int j = 0; j < players[i].vectorChi2.Count; j++)
            {
                players[i].vectorChi2[j].setDark(true);
                players[i].vectorChi2[j].transform.SetSiblingIndex(zValues[j]);
            }
            for (int j = 0; j < players[i].vectorChi1.Count; j++)
            {
                Card cardC = players[i].vectorChi1[j];
                cardC.setTextureWithCode(cardC.encodeCard());
                cardC.setDark(false);
                Quaternion newRotationQ = Quaternion.Euler(0, 0, setRotationCard(j));
                Vector2 playerCardsV2 = getPositionPlayerCard(j, posV2), showCardsV2 = getPositionShowCard(j, posV2);
                cardC.transform.rotation = Quaternion.identity;
                int idInVectorCard = j;
                cardC.transform.DOLocalMove(showCardsV2, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    DOTween.Sequence().AppendInterval(2f).Append(cardC.transform.DORotate(newRotationQ.eulerAngles, 0.2f))
                        .Append(cardC.transform.DOLocalMove(playerCardsV2, 0.2f).SetEase(Ease.InOutCubic))
                        .OnComplete(() => { cardC.transform.SetSiblingIndex(zValues[idInVectorCard]); });
                });
            }
            m_RankImgs[idPos].gameObject.SetActive(true);
            m_RankImgs[idPos].transform.localPosition = posV2 + new Vector2(0, 80);
            m_RankImgs[idPos].sprite = m_RankImageSs[getFileName(getMauBinhRank(players[i].vectorChi1), totalChiAndBonus > 0)];
            m_RankImgs[idPos].SetNativeSize();
            DOTween.Sequence().AppendInterval(2.5f).AppendCallback(() => m_RankImgs[idPos].gameObject.SetActive(false));
        }
        await Awaitable.WaitForSecondsAsync(2.5f);
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            if (players[i] == thisPlayer) setPointAtChi(m_Hand1TMP, m_ScoreHand1TMP, getMauBinhRank(players[i].vectorChi1), players[i].scoreChi1, players[i].bonusChi1);
            setPointTotal(players[i], getDynamicIndex(getIndexOf(players[i])));
            for (int j = 0; j < players[i].vectorCard.Count; j++) players[i].vectorCard[j].setDark(false);
        }
    }
    async void showChi2()
    {
        if (this != null) SoundManager.instance.playEffectFromPath(thisPlayer.scoreChi2 + thisPlayer.bonusChi2 > 0 ? SOUND_BINH.COMPARE_WIN : SOUND_BINH.COMPARE_LOSE);
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            int totalChiAndBonus = players[i].scoreChi2 + players[i].bonusChi2;
            players[i].totalPoint += totalChiAndBonus;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            Vector2 posV2 = m_CardsPosV2s[idPos];
            int[] zValues = new int[10];
            for (int j = 3; j < 13; j++) zValues[j - 3] = players[i].vectorCard[j].transform.GetSiblingIndex();
            for (int j = 0; j < players[i].vectorChi3.Count; j++)
            {
                players[i].vectorChi3[j].setDark(true);
                players[i].vectorChi3[j].transform.SetSiblingIndex(zValues[j]);
            }
            for (int j = 0; j < players[i].vectorChi2.Count; j++)
            {
                Card cardC = players[i].vectorChi2[j];
                cardC.setTextureWithCode(cardC.encodeCard());
                cardC.setDark(false);
                Quaternion newRotationQ = Quaternion.Euler(0, 0, setRotationCard(j + 3));
                Vector2 playerCardsV2 = getPositionPlayerCard(j + 3, posV2), showCardsV2 = getPositionShowCard(j + 3, posV2);
                cardC.transform.rotation = Quaternion.identity;
                int idInVectorCard = j;
                cardC.transform.DOLocalMove(showCardsV2, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    DOTween.Sequence().AppendInterval(2f).Append(cardC.transform.DORotate(newRotationQ.eulerAngles, 0.2f))
                        .Append(cardC.transform.DOLocalMove(playerCardsV2, 0.2f).SetEase(Ease.InOutCubic))
                        .OnComplete(() => { cardC.transform.SetSiblingIndex(zValues[idInVectorCard]); });
                });
            }
            m_RankImgs[idPos].gameObject.SetActive(true);
            m_RankImgs[idPos].transform.localPosition = posV2 + new Vector2(0, 200);
            m_RankImgs[idPos].sprite = m_RankImageSs[getFileName(getMauBinhRank(players[i].vectorChi2), totalChiAndBonus > 0)];
            m_RankImgs[idPos].SetNativeSize();
            DOTween.Sequence().AppendInterval(2.5f).AppendCallback(() => m_RankImgs[idPos].gameObject.SetActive(false));
        }
        await Awaitable.WaitForSecondsAsync(2.5f);
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            if (players[i] == thisPlayer) setPointAtChi(m_Hand2TMP, m_ScoreHand2TMP, getMauBinhRank(players[i].vectorChi2), players[i].scoreChi2, players[i].bonusChi2);
            setPointTotal(players[i], getDynamicIndex(getIndexOf(players[i])));
        }
    }
    async void showChi3()
    {
        m_SpecialSG.transform.parent.gameObject.SetActive(false);
        if (this != null) SoundManager.instance.playEffectFromPath(thisPlayer.scoreChi3 + thisPlayer.bonusChi3 > 0 ? SOUND_BINH.COMPARE_WIN : SOUND_BINH.COMPARE_LOSE);
        m_ScoreBg.SetActive(!(stateGame == STATE_GAME.VIEWING || thisPlayer.mauBinh_MB >= 10 || thisPlayer.mauBinh_BL));
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            int totalChiAndBonus = players[i].scoreChi3 + players[i].bonusChi3;
            players[i].totalPoint += totalChiAndBonus;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            Vector2 posV2 = m_CardsPosV2s[idPos];
            for (int j = 0; j < players[i].vectorChi3.Count; j++)
            {
                Card cardC = players[i].vectorChi3[j];
                cardC.setTextureWithCode(cardC.encodeCard());
                cardC.setDark(false);
                Quaternion newRotationQ = Quaternion.Euler(0, 0, setRotationCard(j + 8));
                Vector2 playerCardsV2 = getPositionPlayerCard(j + 8, posV2), showCardsV2 = getPositionShowCard(j + 8, posV2);
                cardC.transform.rotation = Quaternion.identity;
                cardC.transform.DOLocalMove(showCardsV2, 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    DOTween.Sequence().AppendInterval(2f).Append(cardC.transform.DORotate(newRotationQ.eulerAngles, 0.2f))
                        .Append(cardC.transform.DOLocalMove(playerCardsV2, 0.2f).SetEase(Ease.InOutCubic));
                });
            }
            m_RankImgs[idPos].gameObject.SetActive(true);
            m_RankImgs[idPos].transform.localPosition = posV2 + new Vector2(0, 150);
            m_RankImgs[idPos].sprite = m_RankImageSs[getFileName(getMauBinhRank(players[i].vectorChi3), totalChiAndBonus > 0)];
            m_RankImgs[idPos].SetNativeSize();
            DOTween.Sequence().AppendInterval(2.5f).AppendCallback(() => m_RankImgs[idPos].gameObject.SetActive(false));
        }
        await Awaitable.WaitForSecondsAsync(2.5f);
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            if (players[i].mauBinh_BL || players[i].mauBinh_MB > 0) continue;
            if (players[i] == thisPlayer) setPointAtChi(m_Hand3TMP, m_ScoreHand3TMP, getMauBinhRank(players[i].vectorChi3), players[i].scoreChi3, players[i].bonusChi3);
            setPointTotal(players[i], getDynamicIndex(getIndexOf(players[i])));
        }
    }
    int getFileName(int rank, bool isBest)
    {
        int fileName = -1;
        switch (rank)
        {
            case (int)TYPE_CARDS_PUSOY.HIGH_CARD:
                fileName = 9;
                break;
            case (int)TYPE_CARDS_PUSOY.PAIR:
                fileName = 11;
                break;
            case (int)TYPE_CARDS_PUSOY.TWO_PAIR:
                fileName = 21;
                break;
            case (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND:
                fileName = 24;
                break;
            case (int)TYPE_CARDS_PUSOY.STRAIGHT:
                fileName = 16;
                break;
            case (int)TYPE_CARDS_PUSOY.FLUSH:
                fileName = 2;
                break;
            case (int)TYPE_CARDS_PUSOY.FULL_HOUSE:
                fileName = 6;
                break;
            case (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND:
                fileName = 4;
                break;
            case (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH:
                fileName = 18;
                break;
        }
        if (!isBest) fileName++;
        return fileName;
    }
    public void initSortLayer()
    {
        _CanSortCard = true;
        m_SortCards.SetActive(true);
        m_RearrangeBtn.gameObject.SetActive(false);
        foreach (Card cardC in _FakeCardOnDragCs) cardC.gameObject.SetActive(false);
        cleanMarkCard();
        showStar(new List<List<Card>>());
        updateTextBinh();
        showHintForPlayer();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == thisPlayer) continue;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            m_RankImgs[idPos].gameObject.SetActive(true);
            m_RankImgs[idPos].transform.localPosition = m_CardsPosV2s[idPos] + new Vector2(0, 120);
            m_RankImgs[idPos].sprite = m_RankImageSs[0];
            m_RankImgs[idPos].SetNativeSize();
        }
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++)
        {
            Card cardC = thisPlayer.vectorCard[i];
            cardC.transform.parent = m_Cards1.transform;
            cardC.SetPivot(.5f, .5f);
            cardC.StopAllCoroutines();
            cardC.transform.DOLocalMove(getPositionSortCard(i), 0.2f).SetEase(Ease.InOutCubic);
            cardC.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic);
            cardC.transform.DOScale(ORG_CARD, 0.2f).SetEase(Ease.InOutSine);
        }
        m_ChosenCardsBCGC.transform.SetAsLastSibling();

    }
    public void hideSortLayer()
    {
        _CanSortCard = false;
        m_SortCards.SetActive(false);
        m_RearrangeBtn.gameObject.SetActive(true);
        hideStar();
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++)
        {
            Card card = thisPlayer.vectorCard[i];
            card.transform.parent = m_Cards.transform;
            card.SetPivot(.5f, -.5f);
            card.transform.DOLocalMove(getPositionPlayerCard(i, m_CardsPosV2s[0]), 0.25f).SetEase(Ease.InOutCubic);
            card.transform.DORotate(Quaternion.Euler(0, 0, setRotationCard(i)).eulerAngles, 0.25f).SetEase(Ease.InOutCubic);
            card.transform.DOScale(SCALE_CARD, 0.1f).SetEase(Ease.InOutSine);
        }
    }
    public List<Card> sortChi(List<Card> cardCs)
    {
        List<Card> chi1Cs = new(), chi2Cs = new(), chi3Cs = new();
        int[] zValues = new int[13];
        for (int i = 0; i < cardCs.Count; i++)
        {
            if (i < 3) chi1Cs.Add(cardCs[i]);
            else if (i < 8) chi2Cs.Add(cardCs[i]);
            else chi3Cs.Add(cardCs[i]);
            zValues[i] = cardCs[i].transform.GetSiblingIndex();
        }
        chi1Cs.Sort((x, y) => y.N - x.N);
        chi2Cs.Sort((x, y) => y.N - x.N);
        chi3Cs.Sort((x, y) => y.N - x.N);
        if (_LogicManagerBLM.checkDoi(chi1Cs, true)) chi1Cs = listDoi(chi1Cs);

        if (_LogicManagerBLM.checkThungPhaSanh(chi2Cs, out List<bool> chi2ThungAndSanh, 5)) {/* bản thân chi2Cs đã sort từ đầu theo thùng*/ }
        else if (_LogicManagerBLM.checkTuQuy(chi2Cs, true)) chi2Cs = listTuQuy(chi2Cs);
        else if (_LogicManagerBLM.checkCulu(chi2Cs, out bool isChi2SamCo, true)) chi2Cs = listCuLu(chi2Cs);
        else if (chi2ThungAndSanh[0]) {/* bản thân chi2Cs đã sort từ đầu theo thùng*/}
        else if (chi2ThungAndSanh[1]) chi2Cs = listSanh(chi2Cs);
        else if (isChi2SamCo) chi2Cs = listSamCo(chi2Cs);
        else if (_LogicManagerBLM.checkThu(chi2Cs, true)) chi2Cs = listThu(chi2Cs);
        else if (_LogicManagerBLM.checkDoi(chi2Cs, true)) chi2Cs = listDoi(chi2Cs);

        if (_LogicManagerBLM.checkThungPhaSanh(chi3Cs, out List<bool> chi3ThungAndSanh, 5)) {/* bản thân chi3Cs đã sort từ đầu theo thùng*/}
        else if (_LogicManagerBLM.checkTuQuy(chi3Cs, true)) chi3Cs = listTuQuy(chi3Cs);
        else if (_LogicManagerBLM.checkCulu(chi3Cs, out bool isChi3SamCo, true)) chi3Cs = listCuLu(chi3Cs);
        else if (chi3ThungAndSanh[0]) {/* bản thân chi3Cs đã sort từ đầu theo thùng*/}
        else if (chi3ThungAndSanh[1]) chi3Cs = listSanh(chi3Cs);
        else if (isChi3SamCo) chi3Cs = listSamCo(chi3Cs);
        else if (_LogicManagerBLM.checkThu(chi3Cs, true)) chi3Cs = listThu(chi3Cs);
        else if (_LogicManagerBLM.checkDoi(chi3Cs, true)) chi3Cs = listDoi(chi3Cs);

        List<Card> resultCs = new();
        resultCs.AddRange(chi1Cs);
        resultCs.AddRange(chi2Cs);
        resultCs.AddRange(chi3Cs);
        for (int i = 0; i < resultCs.Count; i++) resultCs[i].transform.SetSiblingIndex(zValues[i]);
        return resultCs;
    }
    List<Card> listDoi(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), outputCs = new();
        for (int i = tempCs.Count - 1; i > 0; i--)
        {
            if (tempCs[i].N == tempCs[i - 1].N)
            {
                outputCs.Add(tempCs[i]);
                outputCs.Add(tempCs[i - 1]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                break;
            }
        }
        outputCs.AddRange(tempCs);
        return outputCs;
    }
    List<Card> listThu(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), outputCs = new();
        // tempCs.Sort((x, y) => x.N.CompareTo(y.N));
        tempCs.Reverse();
        for (int i = tempCs.Count - 1; i > 0; i--)
        {
            if (tempCs[i].N == tempCs[i - 1].N)
            {
                outputCs.Add(tempCs[i]);
                outputCs.Add(tempCs[i - 1]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                break;
            }
        }
        for (int i = tempCs.Count - 1; i > 0; i--)
        {
            if (tempCs[i].N == tempCs[i - 1].N)
            {
                outputCs.Add(tempCs[i]);
                outputCs.Add(tempCs[i - 1]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                break;
            }
        }
        outputCs.AddRange(tempCs);
        return outputCs;
    }
    List<Card> listSamCo(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), outputCs = new();
        for (int i = tempCs.Count - 1; i > 1; i--)
        {
            if (tempCs[i].N == tempCs[i - 1].N && tempCs[i].N == tempCs[i - 2].N)
            {
                outputCs.Add(tempCs[i]);
                outputCs.Add(tempCs[i - 1]);
                outputCs.Add(tempCs[i - 2]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                tempCs.RemoveAt(i - 2);
                break;
            }
        }
        outputCs.AddRange(tempCs);
        return outputCs;
    }
    List<Card> listSanh(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), outputCs = new();
        Card firstC = tempCs[0], lastC = tempCs[tempCs.Count - 1];
        if (lastC.N == 2 && firstC.N == 14)
        {
            for (int j = 1; j < tempCs.Count; j++) outputCs.Add(tempCs[j]);
            outputCs.Add(firstC);
        }
        else outputCs.AddRange(tempCs);
        return outputCs;
    }
    List<Card> listCuLu(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), ouputCs = new();
        // tempCs.Sort((x, y) => x.N.CompareTo(y.N));
        tempCs.Reverse();
        for (int i = tempCs.Count - 1; i > 1; i--)
        {
            if (tempCs[i].N == tempCs[i - 1].N && tempCs[i].N == tempCs[i - 2].N)
            {
                ouputCs.Add(tempCs[i]);
                ouputCs.Add(tempCs[i - 1]);
                ouputCs.Add(tempCs[i - 2]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                tempCs.RemoveAt(i - 2);
                break;
            }
        }
        ouputCs.AddRange(tempCs);
        return ouputCs;
    }
    List<Card> listTuQuy(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs), outputCs = new();
        for (int i = tempCs.Count - 1; i > 2; i--)
        {
            if (tempCs[i].N == tempCs[i - 3].N)
            {
                outputCs.Add(tempCs[i]);
                outputCs.Add(tempCs[i - 1]);
                outputCs.Add(tempCs[i - 2]);
                outputCs.Add(tempCs[i - 3]);
                tempCs.RemoveAt(i);
                tempCs.RemoveAt(i - 1);
                tempCs.RemoveAt(i - 2);
                tempCs.RemoveAt(i - 3);
                break;
            }
        }
        outputCs.AddRange(tempCs);
        return outputCs;
    }
    public void moveBack()
    {
        //for (int i = 0; i < movePos.Count; i++)
        //{
        //    int pos = movePos[i];
        //    var card = thisPlayer.vectorCard[pos];
        //    card.transform.localPosition = oldCardPos[i];
        //    card.transform.SetSiblingIndex((int)GAME_ZORDER.Z_MENU_VIEW + 70 + pos);
        //}
    }
    public void cleanTable()
    {
        for (int i = 0; i < 4; i++)
        {
            m_SpecialRankImgs[i].gameObject.SetActive(false);
            m_TotalPoints[i].SetActive(false);
        }
        m_ScoreBg.SetActive(false);
        m_ScoreHand1TMP.gameObject.SetActive(false);
        m_ScoreHand2TMP.gameObject.SetActive(false);
        m_ScoreHand3TMP.gameObject.SetActive(false);
        m_Hand1TMP.gameObject.SetActive(false);
        m_Hand2TMP.gameObject.SetActive(false);
        m_Hand3TMP.gameObject.SetActive(false);
        updatePositionPlayerView();
    }
    public void connectGame(string stringData)
    {
        JArray ArrP = getJArray(JObject.Parse(stringData), "ArrP");
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject data = (JObject)ArrP[i];
            Player playerP = getPlayerWithID(getInt(data, "id"));
            playerP.mauBinhSoBai = !(bool)data["A"];
            clearAllCard(playerP);
            List<int> listCards = getListInt(data, "Arr");
            for (int j = 0; j < listCards.Count; j++)
            {
                Card card = getCard();
                card.decodeCard(listCards[j]);
                playerP.vectorCard.Add(card);
            }
        }
    }
    IEnumerator ShowJackpotWin(int chip)
    {
        Transform parentTf = m_JackPotNumTMP.transform.parent;
        parentTf.gameObject.SetActive(true);
        parentTf.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
        m_JackPotNumTMP.text = chip.ToString();
        yield return new WaitForSeconds(4f);
        parentTf.gameObject.SetActive(false);
    }
    public void initPlayerCard()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (stateGame == STATE_GAME.VIEWING && players[i] == thisPlayer) continue;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            Vector2 posV2 = m_CardsPosV2s[idPos];
            for (int j = 0; j < players[i].vectorCard.Count; j++)
            {
                Card cardC = players[i].vectorCard[j];
                cardC.SetPivot(.5f, -.5f);
                cardC.transform.localScale = new Vector2(SCALE_CARD, SCALE_CARD);
                cardC.transform.localPosition = getPositionPlayerCard(j, posV2);
                cardC.transform.localRotation = Quaternion.Euler(0, 0, setRotationCard(j));
                cardC.setTextureWithCode(cardC.code);
            }
            if (players[i] != thisPlayer && !_IsFinish)
            {
                m_RankImgs[idPos].gameObject.SetActive(true);
                m_RankImgs[idPos].transform.localPosition = posV2 + new Vector2(0, 120);
                m_RankImgs[idPos].sprite = players[i].mauBinhSoBai ? m_RankImageSs[13] : m_RankImageSs[0];
                m_RankImgs[idPos].SetNativeSize();
            }
            splitChi(players[i]);
        }
        if (stateGame == STATE_GAME.PLAYING) onClickXepLai();
    }

    public async void doEndGameFlow()
    {
        for (int i = 0; i < 4; i++) m_RankImgs[i].gameObject.SetActive(false);
        m_SpecialSG.transform.parent.gameObject.SetActive(false);
        foreach (Player playerP in players)
        {
            foreach (Card cardC in playerP.vectorChi1) cardC.setDark(false);
            foreach (Card cardC in playerP.vectorChi2) cardC.setDark(false);
            foreach (Card cardC in playerP.vectorChi3) cardC.setDark(false);
        }
        await checkSapHam(checkWinAll());
        StartCoroutine(EndGameSequence());
    }
    private IEnumerator EndGameSequence()
    {
        yield return new WaitForSeconds(1f);
        showExchangeMoney();
        yield return new WaitForSeconds(5f);
        handleFinishGame();
    }
    public async void showExchangeMoney()
    {
        m_ScoreBg.SetActive(false);
        m_ScoreHand1TMP.gameObject.SetActive(false);
        m_ScoreHand2TMP.gameObject.SetActive(false);
        m_ScoreHand3TMP.gameObject.SetActive(false);
        m_Hand1TMP.gameObject.SetActive(false);
        m_Hand2TMP.gameObject.SetActive(false);
        m_Hand3TMP.gameObject.SetActive(false);
        if (!_CanClear || _IsFinish) await Awaitable.WaitForSecondsAsync(4f);
        for (int i = 0; i < 4; i++)
        {
            m_SpecialRankImgs[i].gameObject.SetActive(false);
            m_TotalPoints[i].SetActive(false);
        }
        await Awaitable.WaitForSecondsAsync(1f);
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null || (players[i] == thisPlayer && stateGame == STATE_GAME.VIEWING)) continue;
            int idPos = getDynamicIndex(getIndexOf(players[i]));
            m_BurnedCardsSG[idPos].gameObject.SetActive(false);
            m_BurnedIcons[idPos].SetActive(false);
            clearAllCard(players[i]);
        }
        showWinLose();
    }
    public async void showWinLose()
    {
        Vector2 posV2 = new();
        List<ChipBet> chipCBs = new();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null || (players[i] == thisPlayer && stateGame == STATE_GAME.VIEWING)) continue;
            if (players[i].mauBinh_M < 0)
            {
                if (players[i] == thisPlayer) if (this != null) SoundManager.instance.playEffectFromPath(SOUND_GAME.LOSE);
                players[i].playerView.setEffectLose(false);
                players[i].playerView.effectFlyMoney(players[i].mauBinh_M, 40);
                for (int j = 0; j < _DataFinishJA.Count; j++)
                {
                    JObject data = (JObject)_DataFinishJA[j];
                    if (players[i].namePl.Equals(getString(data, "N")))
                    {
                        players[i].ag = (long)data["AG"];
                        players[i].setAg();
                        players[i].playerView.chipJackpot = 0;
                    }
                }
                if (this != null) SoundManager.instance.playEffectFromPath(SOUND_GAME.THROW_CHIP);
                for (int j = 0; j < 10; j++)
                {
                    posV2.x = UnityEngine.Random.Range(0, 80) - 40;
                    posV2.y = UnityEngine.Random.Range(0, 80) - 40;
                    ChipBet chipCB = getChip();
                    chipCBs.Add(chipCB);
                    chipCB.transform.localPosition = players[i].playerView.transform.localPosition;
                    chipCB.transform.DOLocalMove(posV2, 0.8f).SetDelay(0.075f * j).SetEase(Ease.InOutCubic);
                }
            }
        }
        await Awaitable.WaitForSecondsAsync(2f);
        for (int i = 0; i < chipCBs.Count; i++) putChip(chipCBs[i]);
        for (int i = 0; i < players.Count; i++)
        {
            Player playerP = players[i];
            if (playerP == null || (playerP == thisPlayer && stateGame == STATE_GAME.VIEWING)) continue;
            if (playerP.mauBinh_M > 0)
            {
                if (this != null) SoundManager.instance.playEffectFromPath(SOUND_GAME.THROW_CHIP);
                if (playerP == thisPlayer) if (this != null) SoundManager.instance.playEffectFromPath(SOUND_GAME.WIN);
                bool checkWin = false;
                for (int j = 0; j < 10; j++)
                {
                    posV2.x = UnityEngine.Random.Range(0, 80) - 40;
                    posV2.y = UnityEngine.Random.Range(0, 80) - 40;
                    ChipBet chipCB = getChip();
                    chipCB.transform.localPosition = posV2;
                    chipCB.transform.DOLocalMove(playerP.playerView.transform.localPosition, 0.8f).SetEase(Ease.InOutCubic).SetDelay(0.075f * j).OnComplete(() =>
                    {
                        putChip(chipCB);
                        if (!checkWin)
                        {
                            checkWin = true;
                            playerP.playerView.setEffectWin("", false);
                            playerP.playerView.effectFlyMoney(playerP.mauBinh_M, 40);
                            for (int k = 0; k < _DataFinishJA.Count; k++)
                            {
                                JObject data = (JObject)_DataFinishJA[k];
                                if (playerP.namePl.Equals(getString(data, "N")))
                                {
                                    playerP.ag = (long)data["AG"];
                                    playerP.setAg();
                                    playerP.playerView.chipJackpot = 0;
                                }
                            }
                        }
                    });
                }
            }
        }
        stateGame = STATE_GAME.WAITING;
    }
    void setPointAtChi(TextMeshProUGUI pointTMP, TextMeshProUGUI scoreTMP, int rank, int point, int bonus)
    {
        int score = point + bonus;
        string text = "";
        if (rank == (int)TYPE_CARDS_PUSOY.HIGH_CARD) text = Config.getTextConfig("binh_mauthau");
        else if (rank == (int)TYPE_CARDS_PUSOY.PAIR) text = Config.getTextConfig("binh_doi");
        else if (rank == (int)TYPE_CARDS_PUSOY.TWO_PAIR) text = Config.getTextConfig("binh_thu");
        else if (rank == (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND) text = Config.getTextConfig("binh_xam");
        else if (rank == (int)TYPE_CARDS_PUSOY.STRAIGHT) text = Config.getTextConfig("binh_sanh");
        else if (rank == (int)TYPE_CARDS_PUSOY.FLUSH) text = Config.getTextConfig("binh_thung");
        else if (rank == (int)TYPE_CARDS_PUSOY.FULL_HOUSE) text = Config.getTextConfig("binh_culu");
        else if (rank == (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND) text = Config.getTextConfig("binh_tuquy");
        else if (rank == (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH) text = Config.getTextConfig("binh_tps");
        pointTMP.gameObject.SetActive(true);
        scoreTMP.gameObject.SetActive(true);
        pointTMP.text = text;
        string stringScore = string.Format("{0}", Mathf.Abs(point));
        if (point > 0) stringScore = "+" + stringScore;
        else if (point < 0) stringScore = "-" + stringScore;
        if (bonus != 0)
        {
            string mark = "";
            if (bonus > 0) mark = "+";
            else mark = "-";
            stringScore += "(" + mark + Mathf.Abs(bonus) + ")";
        }
        scoreTMP.text = stringScore;
        scoreTMP.color = score > 0 ? Color.yellow : Color.white;
    }
    public void countDown(int time)
    {
        m_BgTimeRemainImg.DOKill();
        _TimeTMP.text = time + "";
        m_BgTimeRemainImg.gameObject.SetActive(true);
        m_BgTimeRemainImg.fillAmount = 1;
        m_BgTimeRemainImg.DOFillAmount(0, time);
        if (_ShowArrangingC != null) StopCoroutine(_ShowArrangingC);
        _ShowArrangingC = StartCoroutine(countDown());

        IEnumerator countDown()
        {
            while (time >= 0)
            {
                if (time == 4 && m_SortCards.activeSelf) Config.Vibration();
                updateBinhTime(time);
                _TimeTMP.text = time.ToString();
                time--;
                yield return new WaitForSeconds(1f);
            }
        }
    }
    public void stopCountDown()
    {
        _TimeTMP.color = Color.green;
        m_BgTimeRemainImg.gameObject.SetActive(false);
    }
    public void setPointTotal(Player playerP, int id)
    {
        if (stateGame == STATE_GAME.VIEWING && playerP == thisPlayer) return;
        TextMeshProUGUI totalPoint = m_TotalPoints[id].transform.Find("textPoint").GetComponent<TextMeshProUGUI>();
        totalPoint.text = playerP.totalPoint > 0 ? "+" + playerP.totalPoint.ToString() : playerP.totalPoint.ToString();
        totalPoint.color = playerP.totalPoint >= 0 ? Color.yellow : Color.white;
        m_TotalPoints[id].SetActive(true);
        m_TotalPoints[id].transform.DOScale(1.5f, 0.2f).SetEase(Ease.InOutSine).OnComplete(() => m_TotalPoints[id].transform.DOScale(1, 0.15f).SetEase(Ease.InOutSine));
    }
    void splitChi(Player playerP)
    {
        if (playerP == null) return;
        playerP.vectorChi1 = new();
        playerP.vectorChi2 = new();
        playerP.vectorChi3 = new();
        int chi = 1;
        for (int i = 0; i < playerP.vectorCard.Count; i++)
        {
            if (chi == 1) playerP.vectorChi1.Add(playerP.vectorCard[i]);
            else if (chi == 2) playerP.vectorChi2.Add(playerP.vectorCard[i]);
            else if (chi == 3) playerP.vectorChi3.Add(playerP.vectorCard[i]);
            if (i == 2 || i == 7) chi++;
        }
    }
    public void updateBinhTime(int time)
    {
        if (time > 0)
        {
            if (time <= 10) _TimeTMP.color = Color.red;
            else if (time <= 20) _TimeTMP.color = Color.yellow;
            else _TimeTMP.color = Color.green;
            for (int i = 0; i < players.Count; i++) if (players[i] != thisPlayer && !players[i].mauBinhSoBai) swapCard(players[i], time);
        }
        else
        {
            stopCountDown();
            if (_CanSortCard)
            {
                List<int> vtCard = new();
                thisPlayer.vectorCard = sortChi(thisPlayer.vectorCard);
                for (int i = 0; i < thisPlayer.vectorCard.Count; i++) vtCard.Add(thisPlayer.vectorCard[i].code);
                SocketSend.sendBinhShowCard(vtCard, _IsExit);
            }
        }
    }
    void updateTextBinh()
    {
        int mb = checkMauBinh(), verifyChi1 = compareRank(thisPlayer.vectorChi1, thisPlayer.vectorChi2),
            verifyChi2 = compareRank(thisPlayer.vectorChi2, thisPlayer.vectorChi3), rankChi1 = getMauBinhRank(thisPlayer.vectorChi1),
            rankChi2 = getMauBinhRank(thisPlayer.vectorChi2), rankChi3 = getMauBinhRank(thisPlayer.vectorChi3);
        cleanMarkCard();
        if (verifyChi1 > 0)
        {
            m_HandImgs[0].sprite = m_HandBgSs[0];
            m_CheckHand1Img.sprite = m_CheckIconSs[1];
            foreach (Card card in thisPlayer.vectorChi1) card.setDark(true);
        }
        if (verifyChi2 > 0)
        {
            m_HandImgs[1].sprite = m_HandBgSs[0];
            m_CheckHand2Img.sprite = m_CheckIconSs[1];
            foreach (Card card in thisPlayer.vectorChi2) card.setDark(true);
        }
        if (verifyChi1 <= 0 && verifyChi2 <= 0)
        {
            m_HandImgs[0].sprite = m_HandImgs[1].sprite = m_HandImgs[2].sprite = m_HandBgSs[1];
            m_CheckHand1Img.sprite = m_CheckHand2Img.sprite = m_CheckHand3Img.sprite = m_CheckIconSs[0];
        }
        if (verifyChi1 <= 0 && verifyChi2 <= 0)
        {
            SetTextMauBinh(rankChi1, 1);
            SetTextMauBinh(rankChi2, 2);
            SetTextMauBinh(rankChi3, 3);
        }
        if (mb == (int)TYPE_CARDS_PUSOY.NONE)
        {
            m_DeclareBtn.gameObject.SetActive(false);
            m_TextSpecialImg.gameObject.SetActive(false);
            if (verifyChi1 <= 0 && verifyChi2 <= 0)
            {
                SetTextMauBinh(rankChi1, 1);
                SetTextMauBinh(rankChi2, 2);
                SetTextMauBinh(rankChi3, 3);
            }
            else
            {
                m_SortHand1TMP.text = "";
                m_SortHand2TMP.text = Config.getTextConfig("binh_lung");
                m_SortHand2TMP.color = Color.red;
                m_SortHand3TMP.text = "";
            }
        }
        else
        {
            m_SortHand1TMP.text = m_SortHand2TMP.text = m_SortHand3TMP.text = "";
            m_DeclareBtn.gameObject.SetActive(true);
            m_TextSpecialImg.gameObject.SetActive(true);
            if (mb == (int)TYPE_CARDS_PUSOY.GRAND_DRAGON) m_TextSpecialImg.sprite = m_RankImageSs[8];
            else if (mb == (int)TYPE_CARDS_PUSOY.DRAGON) m_TextSpecialImg.sprite = m_RankImageSs[1];
            else if (mb == (int)TYPE_CARDS_PUSOY.SAME_COLOR) m_TextSpecialImg.sprite = m_RankImageSs[14];
            else if (mb == (int)TYPE_CARDS_PUSOY.SIX_PAIRS) m_TextSpecialImg.sprite = m_RankImageSs[15];
            else if (mb == (int)TYPE_CARDS_PUSOY.THREE_STRAIGHT) m_TextSpecialImg.sprite = m_RankImageSs[23];
            else if (mb == (int)TYPE_CARDS_PUSOY.THREE_FLUSHES) m_TextSpecialImg.sprite = m_RankImageSs[20];
            m_TextSpecialImg.SetNativeSize();
        }
    }
    void cleanMarkCard()
    {
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < players[i].vectorCard.Count; j++)
            {
                players[i].vectorCard[j].setDark(false);
                players[i].vectorCard[j].showBorder(false);
            }
        }
        for (int i = 0; i < _CardPoolCs.Count; i++)
        {
            _CardPoolCs[i].setDark(false);
            _CardPoolCs[i].showBorder(false);
        }
    }
    int compareRank(List<Card> card1Cs, List<Card> card2Cs)
    {
        List<int> value1s = GetListCardValue(card1Cs), value2s = GetListCardValue(card2Cs);
        int rank1 = getMauBinhRank(card1Cs), rank2 = getMauBinhRank(card2Cs);
        if (rank1 == rank2)
        {
            if (rank1 == (int)TYPE_CARDS_PUSOY.HIGH_CARD) return CompareMauThau(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.PAIR) return CompareDoi(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.TWO_PAIR) return CompareThu(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND) return CompareSamCo(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.STRAIGHT) return CompareSanh(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.FLUSH) return CompareThung(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.FULL_HOUSE) return CompareCuLu(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND) return CompareTuQuy(value1s, value2s);
            else if (rank1 == (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH) return CompareThungPhaSanh(value1s, value2s);
        }
        else if (rank1 > rank2) return 1;
        return -1;
    }
    int CompareMauThau(List<int> list1, List<int> list2)
    {
        for (int i = list1.Count - 1; i >= 0; i--)
        {
            if (list1[i] > list2[i]) return 1;
            else if (list1[i] < list2[i]) return -1;
        }
        return 0;
    }
    int CompareDoi(List<int> list1, List<int> list2)
    {
        int value1 = 0, value2 = 0;
        for (int i = 0; i < list1.Count - 1; i++)
        {
            if (list1[i] == list1[i + 1])
            {
                value1 = list1[i];
                break;
            }
        }
        for (int i = 0; i < list2.Count - 1; i++)
        {
            if (list2[i] == list2[i + 1])
            {
                value2 = list2[i];
                break;
            }
        }
        if (value1 > value2) return 1;
        else if (value1 < value2) return -1;
        else return CompareMauThau(list1, list2);
    }
    int CompareThu(List<int> list1, List<int> list2)
    {
        if (list1[1] != list1[2] && list1[2] != list1[3])
        {
            int temp = list1[2];
            list1.RemoveAt(2);
            list1.Insert(0, temp);
        }
        else if (list1[3] != list1[4])
        {
            int temp = list1[4];
            list1.RemoveAt(4);
            list1.Insert(0, temp);
        }
        if (list2[1] != list2[2] && list2[2] != list2[3])
        {
            int temp = list2[2];
            list2.RemoveAt(2);
            list2.Insert(0, temp);
        }
        else if (list2[3] != list2[4])
        {
            int temp = list2[4];
            list2.RemoveAt(4);
            list2.Insert(0, temp);
        }
        return CompareMauThau(list1, list2);
    }
    int CompareSamCo(List<int> list1, List<int> list2)
    {
        int value1 = 0, value2 = 0;
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] == list1[i + 1] && list1[i] == list1[i + 2])
            {
                value1 = list1[i];
                break;
            }
        }
        for (int i = 0; i < list2.Count; i++)
        {
            if (list2[i] == list2[i + 1] && list2[i] == list2[i + 2])
            {
                value2 = list2[i];
                break;
            }
        }
        if (value1 > value2) return 1;
        return -1;
    }
    int CompareSanh(List<int> list1, List<int> list2)
    {
        if (list1[0] == 2 && list1[4] == 14)
        {
            list1.RemoveAt(4);
            list1.Insert(0, 1);
        }
        if (list2[0] == 2 && list2[4] == 14)
        {
            list2.RemoveAt(4);
            list2.Insert(0, 1);
        }
        return CompareMauThau(list1, list2);
    }
    int CompareThung(List<int> list1, List<int> list2)
    {
        return CompareMauThau(list1, list2);
    }
    int CompareCuLu(List<int> list1, List<int> list2)
    {
        return CompareSamCo(list1, list2);
    }
    int CompareTuQuy(List<int> list1, List<int> list2)
    {
        return CompareDoi(list1, list2);
    }
    int CompareThungPhaSanh(List<int> list1, List<int> list2)
    {
        return CompareThung(list1, list2);
    }
    List<int> GetListCardValue(List<Card> listCard)
    {
        List<int> value = new();
        if (listCard.Count == 3)
        {
            value.Add(-2);
            value.Add(-1);
        }
        for (int i = 0; i < listCard.Count; i++) value.Add(listCard[i].N);
        value.Sort();
        return value;
    }
    int getMauBinhRank(List<Card> listCard)
    {
        if (_LogicManagerBLM.checkThungPhaSanh(listCard, out List<bool> isThungAndSanh, 5)) return (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH;
        else if (_LogicManagerBLM.checkTuQuy(listCard)) return (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND;
        else if (_LogicManagerBLM.checkCulu(listCard, out bool isSamCo, false)) return (int)TYPE_CARDS_PUSOY.FULL_HOUSE;
        else if (_LogicManagerBLM.checkThung(listCard, 5)) return (int)TYPE_CARDS_PUSOY.FLUSH;
        else if (_LogicManagerBLM.checkSanh(listCard, 5)) return (int)TYPE_CARDS_PUSOY.STRAIGHT;
        else if (_LogicManagerBLM.checkSamCo(listCard)) return (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND;
        else if (_LogicManagerBLM.checkThu(listCard)) return (int)TYPE_CARDS_PUSOY.TWO_PAIR;
        else if (_LogicManagerBLM.checkDoi(listCard)) return (int)TYPE_CARDS_PUSOY.PAIR;
        return (int)TYPE_CARDS_PUSOY.HIGH_CARD;
    }
    int checkMauBinh()
    {
        if (_LogicManagerBLM.checkBinhGrandDragon(thisPlayer.vectorCard)) return (int)TYPE_CARDS_PUSOY.GRAND_DRAGON;
        else if (_LogicManagerBLM.checkBinhDragon(thisPlayer.vectorCard)) return (int)TYPE_CARDS_PUSOY.DRAGON;
        else if (_LogicManagerBLM.checkBinhSameColor(thisPlayer.vectorCard)) return (int)TYPE_CARDS_PUSOY.SAME_COLOR;
        else if (_LogicManagerBLM.checkBinhSixPairs(thisPlayer.vectorCard)) return (int)TYPE_CARDS_PUSOY.SIX_PAIRS;
        else if (_LogicManagerBLM.checkBinhThreeStraights(thisPlayer.vectorChi1, thisPlayer.vectorChi2, thisPlayer.vectorChi3)) return (int)TYPE_CARDS_PUSOY.THREE_STRAIGHT;
        else if (_LogicManagerBLM.checkBinhThreeFlushes(thisPlayer.vectorChi1, thisPlayer.vectorChi2, thisPlayer.vectorChi3)) return (int)TYPE_CARDS_PUSOY.THREE_FLUSHES;
        return (int)TYPE_CARDS_PUSOY.NONE;
    }

    void SetTextMauBinh(int rankchi, int chi)
    {
        string text = "";
        bool hasColor = false;
        if (chi == 1)
        {
            if (rankchi == (int)TYPE_CARDS_PUSOY.HIGH_CARD) text = Config.getTextConfig("binh_mauthau");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.PAIR) text = Config.getTextConfig("binh_doi");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND)
            {
                text += Config.getTextConfig("binh_xam");
                if (_ScoreBonus.Count > 0) text += "(+" + Mathf.Abs(_ScoreBonus[0]) + ")";
                hasColor = true;
            }
            m_SortHand1TMP.text = text;
            m_SortHand1TMP.color = hasColor ? Color.yellow : Color.white;
        }
        else if (chi == 2)
        {
            if (rankchi == (int)TYPE_CARDS_PUSOY.HIGH_CARD) text = Config.getTextConfig("binh_mauthau");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.PAIR) text = Config.getTextConfig("binh_doi");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.TWO_PAIR) text = Config.getTextConfig("binh_thu");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND) text = Config.getTextConfig("binh_xam");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.STRAIGHT) text = Config.getTextConfig("binh_sanh");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FLUSH) text = Config.getTextConfig("binh_thung");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FULL_HOUSE)
            {
                text += Config.getTextConfig("binh_culu");
                if (_ScoreBonus.Count > 0) text += " (+" + _ScoreBonus[2] + ")";
                hasColor = true;
            }
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND)
            {
                text += Config.getTextConfig("binh_tuquy");
                if (_ScoreBonus.Count > 0) text += " (+" + _ScoreBonus[3] + ")";
                hasColor = true;
            }
            else if (rankchi == (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH)
            {
                text += Config.getTextConfig("binh_tps");
                if (_ScoreBonus.Count > 0) text += " (+" + _ScoreBonus[5] + ")";
                hasColor = true;
            }
            m_SortHand2TMP.text = text;
            m_SortHand2TMP.color = hasColor ? Color.yellow : Color.white;
        }
        else if (chi == 3)
        {
            if (rankchi == (int)TYPE_CARDS_PUSOY.HIGH_CARD) text = Config.getTextConfig("binh_mauthau");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.PAIR) text = Config.getTextConfig("binh_doi");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.TWO_PAIR) text = Config.getTextConfig("binh_thu");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.THREE_OF_A_KIND) text = Config.getTextConfig("binh_xam");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.STRAIGHT) text = Config.getTextConfig("binh_sanh");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FLUSH) text = Config.getTextConfig("binh_thung");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FULL_HOUSE) text = Config.getTextConfig("binh_culu");
            else if (rankchi == (int)TYPE_CARDS_PUSOY.FOUR_OF_A_KIND)
            {
                text += Config.getTextConfig("binh_tuquy");
                if (_ScoreBonus.Count > 0) text += " (+" + _ScoreBonus[8] + ")";
                hasColor = true;
            }
            else if (rankchi == (int)TYPE_CARDS_PUSOY.STRAIGHT_FLUSH)
            {
                text += Config.getTextConfig("binh_tps");
                if (_ScoreBonus.Count > 0) text += " (+" + _ScoreBonus[10] + ")";
                hasColor = true;
            }
            m_SortHand3TMP.text = text;
            m_SortHand3TMP.color = hasColor ? Color.yellow : Color.white;
        }
    }
    public void showStar(List<List<Card>> listCs)
    {
        foreach (Card cardTemp in thisPlayer.vectorCard) cardTemp.showStarBinh(false);
        if (listCs.Count == 0) return;
        for (int i = 0; i < listCs.Count; i++) foreach (Card cardTemp in listCs[i]) cardTemp.showStarBinh(true, i);
    }
    public void hideStar()
    {
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++) thisPlayer.vectorCard[i].showStarBinh(false);
    }
    public void showHintForPlayer()
    {
        List<Card> cardCs = new(thisPlayer.vectorCard);
        List<List<Card>> listTPS = GetHintTPS(cardCs), listTuQuy = GetHintTuQuy(cardCs),
            listCuLu = GetHintCuLu(cardCs), listThung = GetHintThung(cardCs), listSanh = GetHintSanh(cardCs);
        _HintRankCs = new List<List<List<Card>>> { listTPS, listTuQuy, listCuLu, listThung, listSanh };
        int first = -1;
        // straight flush(0), four of a kind(1), full house(2), flush(3), straight(4)
        if (listTPS.Count == 0)
        {
            m_HintRankImgs[0].sprite = m_ButtonSs[0];
            _StateButtonIds[0] = 0;
        }
        else
        {
            m_HintRankImgs[0].sprite = m_ButtonSs[1];
            if (first == -1) first = 0;
            _StateButtonIds[0] = 1;
        }
        if (listTuQuy.Count == 0)
        {
            m_HintRankImgs[1].sprite = m_ButtonSs[0];
            _StateButtonIds[1] = 0;
        }
        else
        {
            m_HintRankImgs[1].sprite = m_ButtonSs[1];
            if (first == -1) first = 1;
            _StateButtonIds[1] = 1;
        }
        if (listCuLu.Count == 0)
        {
            m_HintRankImgs[2].sprite = m_ButtonSs[0];
            _StateButtonIds[2] = 0;
        }
        else
        {
            m_HintRankImgs[2].sprite = m_ButtonSs[1];
            if (first == -1) first = 2;
            _StateButtonIds[2] = 1;
        }
        if (listThung.Count == 0)
        {
            m_HintRankImgs[3].sprite = m_ButtonSs[0];
            _StateButtonIds[3] = 0;
        }
        else
        {
            m_HintRankImgs[3].sprite = m_ButtonSs[1];
            if (first == -1) first = 3;
            _StateButtonIds[3] = 1;
        }
        if (listSanh.Count == 0)
        {
            m_HintRankImgs[4].sprite = m_ButtonSs[0];
            _StateButtonIds[4] = 0;
        }
        else
        {
            m_HintRankImgs[4].sprite = m_ButtonSs[1];
            if (first == -1) first = 4;
            _StateButtonIds[4] = 1;
        }
        showIcStar(false);
        if (first >= 0 && first < 5)
        {
            showIcStar(true, first);
            showStar(_HintRankCs[first]);
        }
    }
    void showIcStar(bool isShow, int index = 0)
    {
        m_Star.SetActive(isShow);
        if (!isShow) return;
        m_Star.transform.position = new Vector3(m_Star.transform.position.x, m_HintRankImgs[index].transform.position.y, m_Star.transform.position.z);
    }
    public List<List<Card>> GetHintTPS(List<Card> cardCs)
    {
        List<Card> tempCs = cardCs.ToList();
        List<List<Card>> resultCs = new();
        for (int suit = 4; suit >= 1; suit--)
        {
            List<Card> testCs = tempCs.Where(card => card.S == suit).ToList();
            List<Card> sanhCs = GetSanh(ref testCs);
            while (sanhCs.Count >= 5)
            {
                resultCs.Add(sanhCs);
                sanhCs = GetSanh(ref testCs);
            }
        }
        return resultCs;
    }
    public List<List<Card>> GetHintTuQuy(List<Card> cardCs)
    {
        List<Card> tempCs = cardCs.ToList();
        List<List<Card>> resultCs = new();
        int lastN = 15;
        tempCs = tempCs.OrderByDescending(card => card.N).ToList();
        foreach (Card cardTemp in tempCs)
        {
            if (cardTemp.N >= lastN) continue;
            lastN = cardTemp.N;
            List<Card> testCs = tempCs.Where(card => card.N == lastN).ToList();
            if (testCs.Count == 4) resultCs.Add(testCs);
        }
        return resultCs;
    }
    List<List<Card>> GetHintCuLu(List<Card> cardCs)
    {
        List<Card> tempCs = new(cardCs);
        List<List<Card>> resultCs = new();
        int lastN = 15;
        tempCs.Sort((x, y) => y.N.CompareTo(x.N));
        List<List<Card>> samCoCs = new();
        foreach (Card cardTemp in tempCs)
        {
            if (cardTemp.N >= lastN) continue;
            lastN = cardTemp.N;
            List<Card> testCs = tempCs.Where(card => card.N == lastN).ToList();
            if (testCs.Count >= 3)
            {
                if (testCs.Count == 4)
                {
                    testCs.Sort((x, y) => x.S.CompareTo(y.S));
                    testCs.RemoveAt(0);
                }
                samCoCs.Add(testCs);
            }
        }
        tempCs.Sort((x, y) => x.N.CompareTo(y.N));
        lastN = 0;
        List<List<Card>> doiCs = new();
        foreach (Card cardTemp in tempCs)
        {
            if (cardTemp.N <= lastN) continue;
            lastN = cardTemp.N;
            List<Card> test = tempCs.Where(card => card.N == lastN).ToList();
            if (test.Count == 2) doiCs.Add(test);
        }
        int length = Math.Min(doiCs.Count, samCoCs.Count);
        for (int i = 0; i < length; i++) resultCs.Add(samCoCs[i].Concat(doiCs[i]).ToList());
        if (samCoCs.Count >= 2 && resultCs.Count == 0)
        {
            List<Card> temp = new(samCoCs[samCoCs.Count - 1]);
            temp.RemoveAt(0);
            resultCs.Add(samCoCs[0].Concat(temp).ToList());
        }
        return resultCs;
    }
    public List<List<Card>> GetHintThung(List<Card> cardCs)
    {
        List<Card> tempCs = cardCs.ToList();
        List<List<Card>> resultCs = new();
        for (int suit = 4; suit >= 1; suit--)
        {
            List<Card> testCs = tempCs.Where(card => card.S == suit).OrderByDescending(card => card.N).ToList();
            while (testCs.Count >= 5)
            {
                resultCs.Add(testCs.Take(5).ToList());
                testCs = testCs.Skip(5).ToList();
            }
        }
        return resultCs;
    }
    public List<List<Card>> GetHintSanh(List<Card> cardCs)
    {
        List<Card> tempCs = cardCs.ToList();
        List<List<Card>> resultCs = new();
        tempCs.Sort((x, y) => y.S.CompareTo(x.S));
        tempCs.Sort((x, y) => y.N.CompareTo(x.N));
        List<Card> testCs = GetSanh(ref tempCs);
        while (testCs.Count >= 5)
        {
            resultCs.Add(new List<Card>(testCs));
            testCs = GetSanh(ref tempCs);
        }
        return resultCs;
    }
    public List<Card> GetSanh(ref List<Card> cardCs)
    {
        List<Card> sanhCs = new();
        if (!_LogicManagerBLM.checkSanh(cardCs, 5)) return sanhCs;
        cardCs = cardCs.OrderBy(x => x.N).ToList();
        int id;
        for (int i = cardCs.Count - 1; i > 3; i--)
        {
            id = 1;
            sanhCs.Clear();
            Card card1C = cardCs[i];
            sanhCs.Add(card1C);
            for (int j = i - 1; j >= 0; j--)
            {
                Card card2C = cardCs[j];
                if (card2C.N == card1C.N - 1)
                {
                    sanhCs.Add(card2C);
                    id++;
                    card1C = card2C;
                    if (id == 5)
                    {
                        for (int k = 0; k < sanhCs.Count; k++)
                        {
                            for (int l = 0; l < cardCs.Count; l++)
                            {
                                if (sanhCs[k].code == cardCs[l].code)
                                {
                                    cardCs.RemoveAt(l);
                                    break;
                                }
                            }
                        }
                        sanhCs = sanhCs.OrderBy(x => x.N).ToList();
                        return sanhCs;
                    }
                }
            }
        }
        sanhCs.Clear();
        id = 2;
        Card lastC = cardCs[cardCs.Count - 1], firstC = cardCs[0];
        if (lastC.N == 14 && firstC.N == 2)
        {
            sanhCs.Add(lastC);
            sanhCs.Add(firstC);
            lastC = firstC;
            for (int i = 1; i < cardCs.Count; i++)
            {
                firstC = cardCs[i];
                if (firstC.N == lastC.N + 1)
                {
                    sanhCs.Add(firstC);
                    id++;
                    lastC = firstC;
                    if (id == 5) break;
                }
            }
        }
        for (int i = 0; i < sanhCs.Count; i++)
        {
            for (int j = 0; j < cardCs.Count; j++)
            {
                if (sanhCs[i].code == cardCs[j].code)
                {
                    cardCs.RemoveAt(j);
                    break;
                }
            }
        }
        return sanhCs;
    }
    public List<Card> GetThung(ref List<Card> cardCs)
    {
        List<Card> thungCs = new();
        if (!_LogicManagerBLM.checkThung(cardCs, 5)) return thungCs;
        cardCs = cardCs.OrderBy(x => x.N).ToList();
        int id;
        for (int suit = 1; suit <= 4; suit++)
        {
            id = 0;
            thungCs.Clear();
            for (int i = cardCs.Count - 1; i >= 0; i--)
            {
                Card card1C = cardCs[i];
                if (card1C.S == suit)
                {
                    thungCs.Add(card1C);
                    id++;
                    if (id == 5)
                    {
                        for (int j = 0; j < thungCs.Count; j++)
                        {
                            for (int k = 0; k < cardCs.Count; k++)
                            {
                                if (thungCs[j].code == cardCs[k].code)
                                {
                                    cardCs.RemoveAt(k);
                                    break;
                                }
                            }
                        }
                        thungCs = thungCs.OrderBy(x => x.N).ToList();
                        return thungCs;
                    }
                }
            }
        }
        return thungCs;
    }
    public List<Card> GetCuLu(ref List<Card> cardCs)
    {
        List<Card> culuCs = new();
        if (!_LogicManagerBLM.checkCulu(cardCs, out bool isSamCo, false)) return culuCs;
        cardCs = cardCs.OrderBy(x => x.N).ToList();
        for (int i = cardCs.Count - 1; i > 1; i--)
        {
            if (cardCs[i].N == cardCs[i].N)
            {
                culuCs.Add(cardCs[i]);
                culuCs.Add(cardCs[i - 1]);
                culuCs.Add(cardCs[i - 2]);
                break;
            }
        }
        for (int i = 0; i < culuCs.Count; i++)
        {
            for (int j = 0; j < cardCs.Count; j++)
            {
                if (culuCs[i].code == cardCs[j].code)
                {
                    cardCs.RemoveAt(j);
                    break;
                }
            }
        }
        for (int i = cardCs.Count - 1; i > 0; i--)
        {
            if (cardCs[i].N == cardCs[i - 1].N)
            {
                culuCs.Add(cardCs[i]);
                culuCs.Add(cardCs[i - 1]);
                break;
            }
        }
        for (int i = 0; i < culuCs.Count; i++)
        {
            for (int j = 0; j < cardCs.Count; j++)
            {
                if (culuCs[i].code == cardCs[j].code)
                {
                    cardCs.RemoveAt(j);
                    break;
                }
            }
        }
        culuCs = culuCs.OrderBy(x => x.N).ToList();
        return culuCs;
    }
    public List<Card> GetTuQuy(ref List<Card> cardCs)
    {
        List<Card> tuquyCs = new();
        if (!_LogicManagerBLM.checkTuQuy(cardCs)) return tuquyCs;
        cardCs = cardCs.OrderBy(x => x.N).ToList();
        for (int i = cardCs.Count - 1; i > 0; i--)
        {
            if (cardCs[i].N == cardCs[i - 3].N)
            {
                tuquyCs.Add(cardCs[i]);
                tuquyCs.Add(cardCs[i - 1]);
                tuquyCs.Add(cardCs[i - 2]);
                tuquyCs.Add(cardCs[i - 3]);
                break;
            }
        }
        if (tuquyCs[0].N == cardCs[0].N) tuquyCs.Add(cardCs[12]);
        else tuquyCs.Add(cardCs[0]);
        for (int i = 0; i < tuquyCs.Count; i++)
        {
            for (int j = 0; j < cardCs.Count; j++)
            {
                if (tuquyCs[i].code == cardCs[j].code)
                {
                    cardCs.RemoveAt(j);
                    break;
                }
            }
        }
        return tuquyCs;
    }
    public void swapCard(Player playerP, int time)
    {
        if (playerP.timeSwapCard == 0) playerP.timeSwapCard = time - UnityEngine.Random.Range(1, 4);
        if (time <= playerP.timeSwapCard && m_BgTimeRemainImg.IsActive())
        {
            playerP.timeSwapCard = time - UnityEngine.Random.Range(1, 3);
            int id1 = UnityEngine.Random.Range(0, 13), id2 = UnityEngine.Random.Range(0, 13);
            Card card1C = playerP.vectorCard[id1], card2C = playerP.vectorCard[id2];
            Vector3 pos1V3 = card2C.gameObject.transform.localPosition, pos2V3 = card1C.gameObject.transform.localPosition;
            Quaternion rot1Q = card2C.gameObject.transform.localRotation, rot2Q = card1C.gameObject.transform.localRotation;
            int zValue1 = card2C.transform.GetSiblingIndex(), zValue2 = card1C.transform.GetSiblingIndex();
            card1C.transform.DOLocalMove(pos1V3, 0.2f).SetEase(Ease.InOutCubic);
            card1C.transform.DORotate(rot1Q.eulerAngles, 0.2f).SetEase(Ease.InOutCubic);
            card1C.transform.SetSiblingIndex(zValue1);
            card2C.transform.DOLocalMove(pos2V3, 0.2f).SetEase(Ease.InOutCubic);
            card2C.transform.DORotate(rot2Q.eulerAngles, 0.2f).SetEase(Ease.InOutCubic);
            card2C.transform.SetSiblingIndex(zValue2);
            Card tempC = playerP.vectorCard[id1];
            playerP.vectorCard[id1] = playerP.vectorCard[id2];
            playerP.vectorCard[id2] = tempC;
        }
    }
    private Card getCard()
    {
        Card cardC;
        if (_CardPoolCs.Count > 0)
        {
            cardC = _CardPoolCs[0];
            _CardPoolCs.Remove(cardC);
            cardC.transform.SetParent(m_Cards.transform);
        }
        else cardC = Instantiate(m_PrefabCardC, m_Cards.transform);
        cardC.gameObject.SetActive(true);
        return cardC;
    }
    private void putCard(Card card)
    {
        _CardPoolCs.Add(card);
        card.transform.SetParent(null);
        card.gameObject.SetActive(false);
    }
    public void clearAllCard(Player player)
    {
        if (!_CanClear) return;
        for (int i = 0; i < player.vectorCard.Count; i++) putCard(player.vectorCard[i]);
        player.vectorCard.Clear();
        player.vectorChi1.Clear();
        player.vectorChi2.Clear();
        player.vectorChi3.Clear();
    }
    private ChipBet getChip()
    {
        ChipBet chipCB;
        if (_ChipPoolCBs.Count > 0)
        {
            chipCB = _ChipPoolCBs[0];
            _ChipPoolCBs.Remove(chipCB);
            chipCB.transform.parent = m_Chips.transform;
        }
        else chipCB = Instantiate(m_PrefabChipCB, m_Chips.transform);
        chipCB.gameObject.SetActive(true);
        chipCB.transform.localScale = new Vector2(0.5f, 0.5f);
        return chipCB;
    }
    private void putChip(ChipBet chip)
    {
        _ChipPoolCBs.Add(chip);
        chip.transform.SetParent(null);
        chip.gameObject.SetActive(false);
    }
    public Vector2 getPositionPlayerCard(int id, Vector2 cardV2)
    {
        Vector2 posV2 = new(0, 0);
        float y = CARD_HEIGHT * 0.32f;
        switch (id)
        {
            case 0:
            case 1:
            case 2:
                posV2 = cardV2 + new Vector2(0, y);
                break;
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                posV2 = cardV2;
                break;
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
                posV2 = cardV2 - new Vector2(0, y);
                break;
            default:
                break;
        }
        return posV2;
    }
    public Vector2 getPositionSortCard(int id)
    {
        float x = CARD_WIDTH * ORG_CARD, y = CARD_HEIGHT * ORG_CARD + 20;
        Vector2 posV2 = new Vector2(-130, 40);
        switch (id)
        {
            case 0:
                posV2 += new Vector2(0, y);
                posV2 -= new Vector2(2 * x, 0);
                break;
            case 1:
                posV2 += new Vector2(0, y);
                posV2 -= new Vector2(x, 0);
                break;
            case 2:
                posV2 += new Vector2(0, y);
                break;
            case 3:
                posV2 -= new Vector2(2 * x, 0);
                break;
            case 4:
                posV2 -= new Vector2(x, 0);
                break;
            case 5:
                break;
            case 6:
                posV2 += new Vector2(x, 0);
                break;
            case 7:
                posV2 += new Vector2(2 * x, 0);
                break;
            case 8:
                posV2 -= new Vector2(0, y);
                posV2 -= new Vector2(2 * x, 0);
                break;
            case 9:
                posV2 -= new Vector2(0, y);
                posV2 -= new Vector2(x, 0);
                break;
            case 10:
                posV2 -= new Vector2(0, y);
                break;
            case 11:
                posV2 += new Vector2(x, 0);
                posV2 -= new Vector2(0, y);
                break;
            case 12:
                posV2 += new Vector2(2 * x, 0);
                posV2 -= new Vector2(0, y);
                break;
            default:
                break;
        }
        return posV2;
    }
    Vector2 getPositionShowCard(int id, Vector2 cardV2)
    {
        Vector2 pos = cardV2;
        float x = CARD_WIDTH * 0.3f, y = CARD_HEIGHT * 0.3f, del_y = 20;
        switch (id)
        {
            case 0:
                pos += new Vector2(-x, y + del_y);
                break;
            case 1:
                pos += new Vector2(0, y + del_y);
                break;
            case 2:
                pos += new Vector2(x, y + del_y);
                break;
            case 3:
                pos -= new Vector2(2 * x, 0);
                break;
            case 4:
                pos -= new Vector2(x, 0);
                break;
            case 5:
                // No modification for case 5
                break;
            case 6:
                pos += new Vector2(x, 0);
                break;
            case 7:
                pos += new Vector2(2 * x, 0);
                break;
            case 8:
                pos -= new Vector2(2 * x, y);
                break;
            case 9:
                pos -= new Vector2(x, y);
                break;
            case 10:
                pos -= new Vector2(0, y);
                break;
            case 11:
                pos += new Vector2(x, -y);
                break;
            case 12:
                pos += new Vector2(2 * x, -y);
                break;
            default:
                break;
        }
        return pos;
    }
    public int setRotationCard(int id)
    {
        int x = 0;
        switch (id)
        {
            case 3:
            case 8:
                x = 30;
                break;
            case 0:
            case 4:
            case 9:
                x = 15;
                break;
            case 1:
            case 5:
            case 10:
                x = 0;
                break;
            case 2:
            case 6:
            case 11:
                x = -15;
                break;
            case 7:
            case 12:
                x = -30;
                break;
            default:
                break;
        }
        return x;
    }
    private void OnPointerUp(PointerEventData eventData)
    {
        Card cardC = null;
        List<RaycastResult> rrs = new();
        EventSystem.current.RaycastAll(eventData, rrs);
        foreach (RaycastResult item in rrs) if (item.gameObject.TryGetComponent(out cardC)) break;
        if (cardC == null) return;
        if (!thisPlayer.vectorCard.Contains(cardC)) return;
        int cardId = thisPlayer.vectorCard.IndexOf(cardC);
        if (m_ChosenCardsBCGC.ChosenIds.Count >= 5 && !m_ChosenCardsBCGC.ChosenIds.Contains(cardId))
        {
            foreach (Card card in thisPlayer.vectorCard) card.showBorder(false);
            m_ChosenCardsBCGC.ChosenIds.Clear();
        }
        bool isAlreadyChosen = m_ChosenCardsBCGC.ChosenIds.Contains(cardId);
        if (isAlreadyChosen) m_ChosenCardsBCGC.ChosenIds.Remove(cardId);
        else m_ChosenCardsBCGC.ChosenIds.Add(cardId);
        cardC.showBorder(!isAlreadyChosen);
    }
    private void OnPointerDown(PointerEventData eventData) { }
    void OnBeginDrag(PointerEventData eventData)
    {
        if (m_ChosenCardsBCGC.ChosenIds.Count <= 0)
        {
            Card cardC = null;
            List<RaycastResult> rrs = new();
            EventSystem.current.RaycastAll(eventData, rrs);
            foreach (RaycastResult item in rrs) if (item.gameObject.TryGetComponent<Card>(out cardC)) break;
            if (cardC == null) return;
            if (!thisPlayer.vectorCard.Contains(cardC)) return;
            int cardId = thisPlayer.vectorCard.IndexOf(cardC);
            bool isChosen = m_ChosenCardsBCGC.ChosenIds.Contains(cardId);
            if (isChosen) m_ChosenCardsBCGC.ChosenIds.Remove(cardId);
            else m_ChosenCardsBCGC.ChosenIds.Add(cardId);
            cardC.showBorder(!isChosen);
        }
        m_ChosenCardsBCGC.FakeChosenCs.Clear();
        foreach (int id in m_ChosenCardsBCGC.ChosenIds)
        {
            Card cardC = thisPlayer.vectorCard[id];
            cardC.showBorder(false);
            Card fakeCardC = null;
            foreach (Transform tf in m_Cards1.transform)
            {
                if (!tf.gameObject.activeSelf)
                {
                    fakeCardC = tf.GetComponent<Card>();
                    break;
                }
            }
            if (fakeCardC == null)
            {
                fakeCardC = Instantiate(m_PrefabCardC, m_Cards1.transform);
                _FakeCardOnDragCs.Add(fakeCardC);
            }

            fakeCardC.transform.SetSiblingIndex(m_Cards1.transform.childCount - 2); // cái cuối là Raycast Manager
            fakeCardC.transform.localScale = cardC.transform.localScale;
            fakeCardC.transform.localPosition = cardC.transform.localPosition;
            fakeCardC.setTextureWithCode(cardC.code);
            fakeCardC.showBorder(false);
            fakeCardC.gameObject.SetActive(true);
            m_ChosenCardsBCGC.FakeChosenCs.Add(fakeCardC);
        }
        _CardsCenterV2 = Vector2.zero;
        foreach (Card card in m_ChosenCardsBCGC.FakeChosenCs) _CardsCenterV2 += (Vector2)card.transform.localPosition;
        _CardsCenterV2 /= m_ChosenCardsBCGC.FakeChosenCs.Count;
    }
    void OnDrag(PointerEventData eventData)
    {
        int countFakeCards = m_ChosenCardsBCGC.FakeChosenCs.Count;
        if (countFakeCards <= 0 || countFakeCards > 5) return;
        Vector2 posTouch = m_Cards1.transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
        Mathf.Clamp(posTouch.x, _ScreenLeftClamp, _ScreenRightClamp);
        Mathf.Clamp(posTouch.y, _ScreenBotClamp, _ScreenTopClamp);
        Vector2 offset = posTouch - _CardsCenterV2;
        _CardsCenterV2 = posTouch;
        foreach (Card card in m_ChosenCardsBCGC.FakeChosenCs) card.transform.localPosition += (Vector3)offset;
        m_ChosenCardsBCGC.TargetIds = _GetChiNearGroupCards(_CardsCenterV2);
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++) thisPlayer.vectorCard[i].showBorder(false);
        if (m_ChosenCardsBCGC.TargetIds.Count < countFakeCards) return;
        m_ChosenCardsBCGC.TargetIds = m_ChosenCardsBCGC.TargetIds.OrderBy(id => Mathf.Abs(thisPlayer.vectorCard[id].transform.localPosition.x - posTouch.x)).ToList();
        for (int i = m_ChosenCardsBCGC.TargetIds.Count - 1; i >= countFakeCards; i--) m_ChosenCardsBCGC.TargetIds.RemoveAt(i);
        foreach (int id in m_ChosenCardsBCGC.TargetIds) thisPlayer.vectorCard[id].showBorder(true);
    }
    void OnEndDrag(PointerEventData eventData)
    {
        m_ChosenCardsBCGC.TargetIds = m_ChosenCardsBCGC.TargetIds.OrderBy(x => x).ToList();
        if (m_ChosenCardsBCGC.TargetIds.Count > 0)
        {
            if (m_ChosenCardsBCGC.TargetIds.Count >= m_ChosenCardsBCGC.FakeChosenCs.Count)
            {
                List<int> sameIds = new();
                foreach (int id in m_ChosenCardsBCGC.ChosenIds) if (m_ChosenCardsBCGC.TargetIds.Contains(id)) sameIds.Add(id);
                foreach (int id in sameIds)
                {
                    m_ChosenCardsBCGC.ChosenIds.Remove(id);
                    m_ChosenCardsBCGC.TargetIds.Remove(id);
                }
                for (int i = 0; i < m_ChosenCardsBCGC.ChosenIds.Count; i++)
                {
                    Card card1 = thisPlayer.vectorCard[m_ChosenCardsBCGC.ChosenIds[i]], card2 = thisPlayer.vectorCard[m_ChosenCardsBCGC.TargetIds[i]];
                    Vector3 card1V3 = card1.transform.localPosition, card2V3 = card2.transform.localPosition;
                    card1.transform.localPosition = card2V3;
                    card2.transform.localPosition = card1V3;
                    SwapIndex(ref thisPlayer.vectorCard, m_ChosenCardsBCGC.ChosenIds[i], m_ChosenCardsBCGC.TargetIds[i]);
                }
                splitChi(thisPlayer);
                updateTextBinh();
            }
        }
        foreach (Card card in m_ChosenCardsBCGC.FakeChosenCs) card.gameObject.SetActive(false);
        foreach (Card card in thisPlayer.vectorCard) card.showBorder(false);
        m_ChosenCardsBCGC.ChosenIds.Clear();
    }
    private void SwapIndex(ref List<Card> cardCs, int n, int m)
    {
        (cardCs[m], cardCs[n]) = (cardCs[n], cardCs[m]);
    }
    private List<int> _GetChiNearGroupCards(Vector2 posTouch)
    {
        float offsetLimit = 2f * CARD_WIDTH * SCALE_CARD;
        for (int i = 0; i < thisPlayer.vectorCard.Count; i++)
        {
            Vector2 cardV2 = thisPlayer.vectorCard[i].transform.localPosition;
            float distanceX = Mathf.Abs(cardV2.x - posTouch.x), distanceY = Mathf.Abs(cardV2.y - posTouch.y);
            if (distanceX <= offsetLimit && distanceY <= offsetLimit)
            {
                if (i <= 2) return new() { 0, 1, 2 };
                else if (i <= 7) return new() { 3, 4, 5, 6, 7 };
                else return new() { 8, 9, 10, 11, 12 };
            }
        }
        return new List<int>();
    }
    protected override void Start()
    {
        base.Start();
        m_ChosenCardsBCGC.SetCallBacks(OnPointerUp, OnBeginDrag, OnDrag, OnEndDrag, OnPointerDown);
        UpdateJackPot();
        m_JackpotAnimA.Stop();
        m_JackpotAnimA.gameObject.SetActive(true);
        Vector3 posV3 = m_JackpotAnimA.transform.localPosition;
        SocketSend.sendUpdateJackpot((int)GAMEID.PUSOY);
        m_JackpotAnimA.Play();
        DOTween.Sequence().AppendCallback(() => { SocketSend.sendUpdateJackpot(Config.curGameId); }).AppendInterval(1.5f)
            .SetLoops(-1).SetId("updateJackpot");

    }
    protected override void Awake()
    {
        base.Awake();
        _TimeTMP = m_TimeRemain.transform.GetComponentInChildren<TextMeshProUGUI>();
        _ScreenLeftClamp = -Screen.currentResolution.width / 2 + 40;
        _ScreenRightClamp = Screen.currentResolution.width / 2 - 40;
        _ScreenBotClamp = -Screen.currentResolution.height / 2 + 65;
        _ScreenTopClamp = Screen.currentResolution.height / 2 - 78;
    }
}
