using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MineFindingView : GameView
{
    private enum MODE { NORMAL, AUTO }
    [SerializeField] private List<MysteriousSlot> m_SlotMSs = new();
    [SerializeField] private List<BetBonusButton> m_BetBBBs = new();
    [SerializeField]
    private GameObject m_BgModeNormal, m_BgModeAuto, m_StartNormal, m_PanelSetting, m_PanelRule, m_Withdraw, m_StopAuto, m_PrefabHistory,
        m_ClosePanels, m_SoundOn, m_SoundOff, m_MusicOn, m_MusicOff, m_TopRight;
    [SerializeField] private Button m_StartAutoBtn, m_RandomBtn, m_NormalModeBtn, m_AutoModeBtn, m_PrefabBaseBetBtn, m_MinusBetBtn, m_PlusBetBtn, m_BaseBetBtn;
    [SerializeField] private TextMeshProUGUI m_WithdrawTMP, m_StopAutoTMP, m_BetTMP;
    [SerializeField] private ScrollRect m_BaseBetsSR, m_HistorySR, m_BetsSR;
    [SerializeField] private TMP_InputField m_TurnsIF, m_WinIF, m_LoseIF;
    [SerializeField] private TMP_Dropdown m_BombsDd;
    [SerializeField] private TextNumberControl m_ChipsTNC;
    private const int MIN_TURN = 1, MIN_WIN_LOSE = 0, MAX_WIN_LOSE = 100;
    private const float AFK_TIME = 60f, SELECT_TIME = .5f;
    private List<List<float>> _RatesByBomb = new();
    private List<BaseBetButton> _BaseBetBtns = new();
    private List<int> _ChosenSlotIds = new();
    private MODE _PlayMode = MODE.NORMAL;
    private RectTransform _BetsRT;
    private string _ErrorOnWithdraw = ""; // update từ handleCtable, bên sv thêm vào đây tránh bug phát sinh
    private long _BaseBet, _FinalChangedChips, _AdjustBet;
    private int _CountBombs, _SetTurn = MIN_TURN, _SetWin = MIN_WIN_LOSE, _SetLose = MIN_WIN_LOSE;
    private float _BetBonus, _ElapsedAfkTime, _ElapsedSelectSlotTime;
    private bool _IsStartAuto, _IsEndAuto, _IsNormalReady, _IsBackOnPlaying;

    #region Button
    public void DoClickBack()
    {
        playSound(SOUND_GAME.CLICK);
        if (stateGame == STATE_GAME.PLAYING)
        {
            UIManager.instance.showMessageBox("Exit when playing will complete your turn, continue?", () =>
            {
                _IsBackOnPlaying = true;
                DoClickWithdraw();
            }, true);
            return;
        }
        SocketSend.sendExitGame();
    }
    public void DoClickButtonNormal()
    {
        playSound(SOUND_GAME.CLICK);
        _SetUIByPlayMode(MODE.NORMAL);
    }
    public void DoClickButtonAuto()
    {
        playSound(SOUND_GAME.CLICK);
        _SetUIByPlayMode(MODE.AUTO);
    }
    public void DoClickStartNormal()
    {
        playSound(SOUND_GAME.CLICK);
        SocketSend.SendManualStartMines(_CountBombs, _BaseBet);
    }
    public void DoClickIncreaseBet()
    {
        m_BetTMP.text = Config.FormatMoney3(_BaseBet += _AdjustBet);
        _EnableAdjustBets(true).playSound(SOUND_GAME.CLICK);
    }
    public void DoClickDecreaseBet()
    {
        m_BetTMP.text = Config.FormatMoney3(_BaseBet -= _AdjustBet);
        _EnableAdjustBets(true).playSound(SOUND_GAME.CLICK);
    }
    public void DoClickRandomChoose()
    {
        if (_PlayMode == MODE.NORMAL && !_IsNormalReady || _ElapsedSelectSlotTime >= 0) return;
        playSound(SOUND_GAME.CLICK);
        _ElapsedSelectSlotTime = 0;
        int rnd;
        do rnd = UnityEngine.Random.Range(m_SlotMSs[0].GetId(), m_SlotMSs[m_SlotMSs.Count - 1].GetId() + 1);
        while (_ChosenSlotIds.Contains(rnd));
        SocketSend.SendSelectCellMines(rnd);
    }
    public void DoClickStartAuto()
    {
        playSound(SOUND_GAME.CLICK);
        _IsStartAuto = true;
        SocketSend.SendAutoStartMines(_CountBombs, _BaseBet, _SetTurn, _SetWin, _SetLose);
    }
    public void DoClickWithdraw()
    {
        playSound(SOUND_GAME.CLICK);
        if (_ChosenSlotIds.Count <= 0) UIManager.instance.showToast(_ErrorOnWithdraw);
        else SocketSend.SendWithdrawMines();
    }
    public void DoClickButtonSetting()
    {
        playSound(SOUND_GAME.CLICK);
        _Turn(m_ClosePanels, true).m_PanelSetting.transform.DOLocalMove((Vector2)m_TopRight.GetComponent<RectTransform>().localPosition - new Vector2(0, 155), .2f).SetEase(Ease.OutQuad);
    }
    public void DoClickButtonSound()
    {
        Config.isSound = !Config.isSound;
        if (Config.isSound) SoundManager.instance.soundClick();
        Config.updateConfigSetting();
        _TurnButtonSetting(m_SoundOn, m_SoundOff, Config.isSound);
    }
    public void DoClickButtonMusic()
    {
        Config.isMusic = !Config.isMusic;
        SoundManager.instance.soundClick();
        Config.updateConfigSetting();
        SoundManager.instance.playMusic();
        _TurnButtonSetting(m_MusicOn, m_MusicOff, Config.isMusic);
    }
    public void DoClickButtonRule()
    {
        playSound(SOUND_GAME.CLICK);
        _Turn(m_ClosePanels, true).m_PanelRule.transform.DOLocalMove(new Vector2(0, 0), .2f).SetEase(Ease.OutQuad);
    }
    public void DoClickButtonBaseBet()
    {
        playSound(SOUND_GAME.CLICK);
        _Turn(m_ClosePanels, true)._Turn(m_BaseBetsSR.gameObject, !m_BaseBetsSR.gameObject.activeSelf);
    }
    public void DoClickCloseMultiplePanels()
    {
        playSound(SOUND_GAME.CLICK);
        _Turn(m_ClosePanels, false)._Turn(m_BaseBetsSR.gameObject, false);
        m_PanelSetting.transform.DOLocalMove(new Vector2(2000, 155), .2f).SetEase(Ease.OutQuad);
        m_PanelRule.transform.DOLocalMove(new Vector2(0, 2000), .2f).SetEase(Ease.OutQuad);
    }
    #endregion

    public void ProcessResponseData(JObject data)
    {
        switch ((string)data["evt"])
        {
            case "errorClient":
                _HandleErrorCode(data);
                break;
            case "history":
                _HandleHistory(data);
                break;
            //normal mode
            case "sgameResult":
                _HandleStartNormal(data);
                break;
            case "cellResult":
                _HandleSelectCellSlot((int)data["cellPosition"], (long)data["currentWinAmount"]);
                break;
            case "gameResult":
                _HandleEndNormal(data);
                break;
            //auto mode
            case "atStartGame":
                StartCoroutine(_HandleStartAuto(data));
                break;
            case "autoCellResult":
                _HandleSelectCellSlot((int)data["cellPosition"], (long)data["currentWinAmount"], (int)data["numberOfTurns"]);
                break;
            case "autoPlayResult":
                _HandleEndAuto(data);
                break;
            case "atFinalResult":
                _IsEndAuto = true;
                _HandleEndAuto(data);
                _IsEndAuto = false;
                break;
        }
    }
    public override void handleCTable(string strData)
    {
        JObject rawData = JObject.Parse(strData);
        _ErrorOnWithdraw = (string)rawData["errorWithdrawMgs"];
        _AdjustBet = (long)rawData["amountAdjustBet"];
        JArray baseBets = (JArray)rawData["betAmountList"];
        foreach (BaseBetButton bbb in _BaseBetBtns) Destroy(bbb.BetBtn.gameObject);
        _BaseBetBtns.Clear();
        for (int i = 0; i < baseBets.Count; i++) _BaseBetBtns.Add(new() { Value = (int)baseBets[i] });
        _BaseBetBtns.Sort((a, b) => a.Value.CompareTo(b.Value));
        for (int i = 0; i < _BaseBetBtns.Count; i++)
        {
            int id = i;
            Button btn = Instantiate(m_PrefabBaseBetBtn, m_BaseBetsSR.content);

            _BaseBetBtns[id].BetBtn = btn;
            btn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = Config.FormatMoney3(_BaseBetBtns[id].Value);
            btn.onClick.AddListener(() => { _Turn(m_ClosePanels, false)._Turn(m_BaseBetsSR.gameObject, false)._SetBaseBet(_BaseBetBtns[id].Value); });
            _Turn(btn.gameObject, true);
        }
        JArray rates = (JArray)rawData["rewardRatesByBomb"];
        _RatesByBomb.Clear();
        foreach (JToken item1 in rates)
        {
            List<float> listRates = new();
            foreach (JToken item2 in (JArray)item1) listRates.Add((float)item2 / 100);
            _RatesByBomb.Add(listRates);
        }
        int countBombDdValue = m_BombsDd.value;
        m_BombsDd.value = -1;
        m_BombsDd.value = countBombDdValue;
        if (User.userMain.AG < _BaseBetBtns[0].Value) _SetBaseBet(User.userMain.AG);
        else _BaseBetBtns[0].BetBtn.onClick.Invoke();
        _EnableAdjustBets(true)._EnableBaseBets(true);
    }
    public override void handleRJTable(string strData) {/*không có trường hợp rejoin, để đây nhỡ sv lỗi có trả về thì k sao */}
    private void _HandleErrorCode(JObject data)
    {
        UIManager.instance.showToast((string)data["msg"]);
    }
    private void _HandleStartNormal(JObject data)
    {
        stateGame = STATE_GAME.PLAYING;
        playSound(SOUND_GAME.START_GAME);
        _ChosenSlotIds.Clear();
        _BetBonus = 0;
        m_WithdrawTMP.text = "0";
        _EnableBaseBets(false)._EnableModes(false)._EnableAdjustBets(false)._Enable(m_BombsDd, false)._Enable(m_RandomBtn, true)
            ._Turn(m_StartNormal, false)._Turn(m_Withdraw, true);
        _BetsRT.anchoredPosition = new Vector2(-220, 0);
        m_BetsSR.enabled = false;
        long agNow = (long)data["currentAgBalance"];
        User.userMain.AG = agNow;
        m_ChipsTNC.setValue(User.userMain.AG, true);
        StartCoroutine(_EnableMysteriousSlots());
    }
    private void _HandleEndNormal(JObject data)
    {
        m_BetsSR.enabled = true;
        foreach (BetBonusButton bbb in m_BetBBBs) bbb.TurnUnselect(true);
        JObject results = (JObject)data["finalBoard"];
        _ChosenSlotIds.Add((int)data["cellPosition"]);
        foreach (JProperty item in results.Properties())
        {
            MysteriousSlot ms = m_SlotMSs.Find(x => x.GetId() == int.Parse(item.Name));
            bool isBomb = (bool)item.Value, isSelected = _ChosenSlotIds.Contains(ms.GetId());
            ms.TurnUnchosable(false).TurnChosable(false).TurnChosenGold(!isBomb && isSelected).TurnUnchosenGold(!isBomb && !isSelected)
                .TurnUnchosenBomb(isBomb && !isSelected).TurnChosenBomb(isBomb && isSelected).transform.localScale = Vector3.one;
        }
        bool isWin = (bool)data["isWin"];
        if (!isWin)
        {
            playSound(SOUND_GAME.EXPLODE);
            playSound(SOUND_GAME.LOSE);
        }
        else playSound(SOUND_GAME.WIN);
        long chipsNow = (long)data["agBalance"];
        UIManager.instance.showDialog(
            (isWin ? "Win " : "Lose ") + Config.FormatMoney3(isWin ? chipsNow - User.userMain.AG : _BaseBet) + " chips.", "OK",
            () =>
            {
                stateGame = STATE_GAME.VIEWING;
                if (chipsNow <= 0 || _IsBackOnPlaying)
                {
                    _IsBackOnPlaying = false;
                    SocketSend.sendExitGame();
                }
            }
        );
        User.userMain.AG = chipsNow;
        m_ChipsTNC.setValue(User.userMain.AG, true);
        _RecalculateBaseBet(chipsNow)._EnableBaseBets(true)._EnableAdjustBets(true)._EnableModes(true)._Enable(m_BombsDd, true)
            ._Enable(m_RandomBtn, false)._Turn(m_StartNormal, true)._Turn(m_Withdraw, false);
        _IsNormalReady = false;
    }
    private IEnumerator _HandleStartAuto(JObject data)
    {
        stateGame = STATE_GAME.PLAYING;
        if (_IsStartAuto) playSound(SOUND_GAME.START_GAME);
        _IsStartAuto = false;
        _ChosenSlotIds.Clear();
        _BetBonus = 0;
        m_StopAutoTMP.text = "0";
        _EnableBaseBets(false)._EnableModes(false)._EnableAdjustBets(false)._Enable(m_BombsDd, false)._Enable(m_StartAutoBtn, false)
            ._Enable(m_TurnsIF, false)._Enable(m_WinIF, false)._Enable(m_LoseIF, false)._Turn(m_StopAuto, true);
        _BetsRT.anchoredPosition = new Vector2(-180, 0);
        m_BetsSR.enabled = false;
        long agNow = (long)data["currentAgBalance"];
        _FinalChangedChips += agNow - User.userMain.AG;
        User.userMain.AG = agNow;
        m_ChipsTNC.setValue(User.userMain.AG, true);
        StartCoroutine(_EnableMysteriousSlots());
        while (true)
        {
            yield return new WaitForSecondsRealtime(.75f);
            DoClickRandomChoose();
        }
    }
    private void _HandleEndAuto(JObject data)
    {
        m_BetsSR.enabled = true;
        StopAllCoroutines();
        JObject results = (JObject)data["finalBoard"];
        int cellPos = (int)data["cellPosition"];
        if (!_ChosenSlotIds.Contains(cellPos)) _ChosenSlotIds.Add((int)data["cellPosition"]);
        _BetBonus = _RatesByBomb[_CountBombs - 1][_ChosenSlotIds.Count - 1];
        for (int i = 0; i < m_BetBBBs.Count; i++)
        {
            bool isThis = m_BetBBBs[i].gameObject.activeSelf && m_BetBBBs[i].GetBetBonus() == _BetBonus;
            m_BetBBBs[i].TurnUnselect(!isThis);
            if (isThis) _BetsRT.anchoredPosition = new Vector2(-m_BetBBBs[i].GetComponent<RectTransform>().localPosition.x - 180, _BetsRT.anchoredPosition.y);
        }
        foreach (JProperty item in results.Properties())
        {
            MysteriousSlot ms = m_SlotMSs.Find(x => x.GetId() == int.Parse(item.Name));
            bool isBomb = (bool)item.Value, isChosen = _ChosenSlotIds.Contains(ms.GetId());
            ms.TurnUnchosable(false).TurnChosable(false).TurnUnchosenBomb(isBomb && !isChosen).TurnChosenBomb(isBomb && isChosen)
                .TurnChosenGold(!isBomb && isChosen).TurnUnchosenGold(!isBomb && !isChosen).transform.localScale = Vector3.one;
        }
        if (!(bool)data["isWin"]) playSound(SOUND_GAME.EXPLODE);
        long chipsNow = (long)data["currentAgBalance"], currentWinAmount = chipsNow - User.userMain.AG;
        m_StopAutoTMP.text = currentWinAmount < 1000 ? currentWinAmount.ToString() : Config.FormatMoney3(currentWinAmount);
        _FinalChangedChips += currentWinAmount;
        User.userMain.AG = chipsNow;
        m_ChipsTNC.setValue(User.userMain.AG, true);
        if (!_IsEndAuto)
            _UpdateAutoInputs(data["numberOfTurns"].ToString(), data["numberOfRoundsWin"].ToString(), data["numberOfRoundsLost"].ToString());
        else
        {
            foreach (BetBonusButton bbb in m_BetBBBs) bbb.TurnUnselect(true);
            bool hasProfit = _FinalChangedChips >= 0;
            UIManager.instance.showDialog((hasProfit ? "Win " : "Lose ") + Config.FormatMoney3((long)Mathf.Abs(_FinalChangedChips)) + " chips.", "OK",
                () =>
                {
                    stateGame = STATE_GAME.VIEWING;
                    if (chipsNow <= 0 || _IsBackOnPlaying)
                    {
                        _IsBackOnPlaying = false;
                        SocketSend.sendExitGame();
                    }
                }
            );
            _RecalculateBaseBet(chipsNow)._EnableBaseBets(true)._EnableAdjustBets(true)._EnableModes(true)._Enable(m_BombsDd, true)
                ._Enable(m_StartAutoBtn, true)._Enable(m_TurnsIF, true)._Enable(m_WinIF, true)._Enable(m_LoseIF, true)._Turn(m_StopAuto, false);
            _UpdateAutoInputs(_GetMaxTurn().ToString());
            playSound(hasProfit ? SOUND_GAME.WIN : SOUND_GAME.LOSE);
            _FinalChangedChips = 0;
        }
    }
    private void _HandleHistory(JObject data)
    {
        JArray arrayData = JArray.Parse((string)data["data"]);
        foreach (Transform tf in m_HistorySR.content) Destroy(tf.gameObject);
        for (int i = arrayData.Count - 1; i >= 0; i--)
        {
            JToken item = arrayData[i];
            string date = DateTimeOffset.FromUnixTimeMilliseconds((long)item["timestamp"]).DateTime.ToString("dd/MM");
            Transform tf = Instantiate(m_PrefabHistory, m_HistorySR.content).transform;

            tf.gameObject.SetActive(true);
            tf.GetChild(1).GetComponent<TextMeshProUGUI>().text = date;
            tf.GetChild(2).GetComponent<TextMeshProUGUI>().text = Config.FormatMoney3((long)item["amountBet"]).ToString();
            tf.GetChild(3).GetComponent<TextMeshProUGUI>().text = item["numberOfBombs"].ToString();
            tf.GetChild(4).GetComponent<TextMeshProUGUI>().text = item["times"].ToString();
            tf.GetChild(5).GetComponent<TextMeshProUGUI>().text = (bool)item["isWin"] ? "Win" : "Lose";
        }
    }
    private MineFindingView _RecalculateBaseBet(long chipsNow)
    {
        if (chipsNow < _BaseBet) _SetBaseBet((long)Mathf.Min(chipsNow, _BaseBetBtns[0].Value));
        else
        {
            if (_BaseBet < _BaseBetBtns[0].Value) _SetBaseBet((long)Mathf.Min(chipsNow, _BaseBetBtns[0].Value));
            else _SetBaseBet(_BaseBet);
        }
        return this;
    }
    private void _SetBaseBet(long baseBet)
    {
        _BaseBet = baseBet;
        _FinalChangedChips = 0;
        m_BetTMP.text = Config.FormatMoney3(_BaseBet);
        foreach (BaseBetButton bbb in _BaseBetBtns) bbb.IsChosen = bbb.Value == baseBet;
    }
    private void _HandleSelectCellSlot(int cellId, long currentWinAmount, int countTurn = -1)
    {
        _ElapsedAfkTime = 0; // tránh bị check afk lúc đang chơi auto
        MysteriousSlot ms = m_SlotMSs.Find(x => x.GetId() == cellId);
        ms.TurnUnchosable(false).TurnChosable(false).TurnChosenBomb(false).TurnUnchosenBomb(false).TurnChosenGold(true).TurnUnchosenGold(false)
            .transform.DOScaleX(-1f, .25f).SetEase(Ease.Linear);
        _ChosenSlotIds.Add(ms.GetId());
        _BetBonus = _RatesByBomb[_CountBombs - 1][_ChosenSlotIds.Count - 1];
        for (int i = 0; i < m_BetBBBs.Count; i++)
        {
            bool isThis = m_BetBBBs[i].gameObject.activeSelf && m_BetBBBs[i].GetBetBonus() == _BetBonus;
            m_BetBBBs[i].TurnUnselect(!isThis);
            if (isThis) _BetsRT.anchoredPosition = new Vector2(-m_BetBBBs[i].GetComponent<RectTransform>().localPosition.x - 220, _BetsRT.anchoredPosition.y);
        }
        m_WithdrawTMP.text = m_StopAutoTMP.text = currentWinAmount < 1000 ? currentWinAmount.ToString() : Config.FormatMoney3(currentWinAmount);
        if (countTurn >= 0) m_TurnsIF.text = countTurn.ToString();
    }
    private void _SetUIByPlayMode(MODE mode)
    {
        _PlayMode = mode;
        bool isNormal = _PlayMode == MODE.NORMAL, isAuto = _PlayMode == MODE.AUTO;
        _Turn(m_BgModeNormal, isNormal)._Turn(m_BgModeAuto, isAuto)._Turn(m_NormalModeBtn.gameObject, isAuto)._Turn(m_AutoModeBtn.gameObject, isNormal);
        _Turn(m_StartNormal, isNormal)._Turn(m_Withdraw, false)._Turn(m_RandomBtn.gameObject, isNormal)._Turn(m_StartAutoBtn.gameObject, isAuto)
            ._Turn(m_StopAuto, false)._Turn(m_TurnsIF.gameObject, isAuto)._Turn(m_WinIF.gameObject, isAuto)._Turn(m_LoseIF.gameObject, isAuto);
    }
    private void _UpdateAutoInputs(string textTurn = "", string textWin = "", string textLose = "")
    {
        m_TurnsIF.text = textTurn;
        m_WinIF.text = textWin;
        m_LoseIF.text = textLose;
    }
    private IEnumerator _EnableMysteriousSlots()
    {
        for (int slice = 0; slice < 9; slice++)
        {
            for (int j = 0, k = slice; j < 5 && k >= 0; j++, k--)
            {
                if (k > 4)
                {
                    j = k - 4;
                    k = 4;
                }
                MysteriousSlot ms = m_SlotMSs[j * 5 + k];
                ms.transform.localScale = Vector3.one;
                float time = .1f;
                ms.transform.DOScale(.5f, time).OnComplete(() =>
                {
                    ms.transform.DOScale(1, time);
                    ms.TurnUnchosable(false).TurnChosable(true).TurnChosenBomb(false).TurnUnchosenBomb(false);
                });
            }
            yield return new WaitForSecondsRealtime(.05f);
        }
        if (_PlayMode == MODE.NORMAL) _IsNormalReady = true;
    }
    private MineFindingView _TurnButtonSetting(GameObject onGO, GameObject offGO, bool on) { _Turn(onGO, on)._Turn(offGO, !on); return this; }
    private int _GetMaxTurn() { return m_SlotMSs.Count - _CountBombs; }
    private MineFindingView _EnableAdjustBets(bool enable)
    {
        long minusBet = _BaseBet - _AdjustBet, plusBet = _BaseBet + _AdjustBet;
        bool canMinus = _BaseBetBtns[0].Value <= minusBet, canPlus = User.userMain.AG >= plusBet;
        _Enable(m_MinusBetBtn, enable && canMinus)._Enable(m_PlusBetBtn, enable && canPlus);
        return this;
    }
    private MineFindingView _EnableBaseBets(bool enable)
    {
        m_BaseBetBtn.interactable = enable;
        if (enable) foreach (BaseBetButton bbb in _BaseBetBtns) _Enable(bbb.BetBtn, bbb.Value <= User.userMain.AG);
        return this;
    }
    private MineFindingView _EnableModes(bool enable) { _Enable(m_NormalModeBtn, enable)._Enable(m_AutoModeBtn, enable); return this; }
    private MineFindingView _Enable(Component obj, bool enable)
    {
        if (obj is Button) obj.GetComponent<Button>().interactable = enable;
        else if (obj is TMP_Dropdown) obj.GetComponent<TMP_Dropdown>().interactable = enable;
        else if (obj is TMP_InputField) obj.GetComponent<TMP_InputField>().interactable = enable;
        return this;
    }
    private MineFindingView _Turn(GameObject obj, bool show) { obj.SetActive(show); return this; }

    protected override void Update()
    {
        if (Input.touchCount <= 0 && !Input.anyKeyDown)
        {
            _ElapsedAfkTime += Time.deltaTime;
            if (_ElapsedAfkTime >= AFK_TIME)
            {
                _ElapsedAfkTime = 0;
                SocketSend.sendExitGame();
            }
        }
        else _ElapsedAfkTime = 0;
        if (_ElapsedSelectSlotTime >= 0)
        {
            _ElapsedSelectSlotTime += Time.deltaTime;
            if (_ElapsedSelectSlotTime >= SELECT_TIME) _ElapsedSelectSlotTime = -1;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        _TurnButtonSetting(m_MusicOn, m_MusicOff, Config.isMusic)._TurnButtonSetting(m_SoundOn, m_SoundOff, Config.isSound);
        setUpMysteriousSlots();
        setUpCountBombDropdown();
        setUpInputFields();
        DoClickButtonNormal();
        m_ChipsTNC.setValue(User.userMain.AG, true);
        _BetsRT = m_BetsSR.content.GetComponent<RectTransform>();
        void setUpMysteriousSlots()
        {
            for (int i = 0; i < m_SlotMSs.Count; i++)
            {
                int id = i;
                m_SlotMSs[id].SetId(id).SetOnclickCB(() =>
                {
                    if (_PlayMode != MODE.NORMAL || (_PlayMode == MODE.NORMAL && !_IsNormalReady)) return;
                    playSound(SOUND_GAME.CLICK);
                    if (_ElapsedSelectSlotTime >= 0) return;
                    _ElapsedSelectSlotTime = 0;
                    SocketSend.SendSelectCellMines(m_SlotMSs[id].GetId());
                });
            }
        }
        void setUpCountBombDropdown() => m_BombsDd.onValueChanged.AddListener((id) =>
            {
                if (id < 0) return;
                int.TryParse(m_BombsDd.options[id].text, out _CountBombs);
                for (int i = 0; i < m_BetBBBs.Count; i++)
                {
                    bool show = i < _RatesByBomb[_CountBombs - 1].Count;
                    _Turn(m_BetBBBs[i].gameObject, show);
                    if (show) m_BetBBBs[i].SetData(_RatesByBomb[_CountBombs - 1][i]);
                }
                m_TurnsIF.text = _GetMaxTurn().ToString();
            });
        void setUpInputFields()
        {
            m_TurnsIF.onValueChanged.AddListener(value =>
            {
                if (!int.TryParse(value, out int num))
                {
                    _SetTurn = 0;
                    return;
                }
                if (m_TurnsIF.interactable) _SetTurn = Mathf.Clamp(num, Mathf.Max(MIN_TURN, num), _GetMaxTurn());
                m_TurnsIF.text = m_TurnsIF.interactable ? _SetTurn.ToString() : num.ToString();
            });
            m_WinIF.onValueChanged.AddListener(value =>
            {
                if (!int.TryParse(value, out int num))
                {
                    _SetWin = MIN_WIN_LOSE;
                    return;
                }
                if (m_WinIF.interactable) _SetWin = Mathf.Clamp(num, MIN_WIN_LOSE + 1, MAX_WIN_LOSE);
                m_WinIF.text = m_WinIF.interactable ? _SetWin.ToString() : (num > MAX_WIN_LOSE ? "" : num.ToString());
            });
            m_LoseIF.onValueChanged.AddListener(value =>
            {
                if (!int.TryParse(value, out int num))
                {
                    _SetLose = MIN_WIN_LOSE;
                    return;
                }
                if (m_LoseIF.interactable) _SetLose = Mathf.Clamp(num, MIN_WIN_LOSE + 1, MAX_WIN_LOSE);
                m_LoseIF.text = m_LoseIF.interactable ? _SetLose.ToString() : (num > MAX_WIN_LOSE ? "" : num.ToString());
            });
        }
    }
    private class BaseBetButton
    {
        public Button BetBtn;
        public long Value;
        public bool IsChosen;
    }
}
