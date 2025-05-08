using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LuckyNumberView : BaseView
{
    public static LuckyNumberView Instance;
    [Serializable]
    private struct Tab
    {
        public GameObject m_BgSelect;
        public void ShowHideBgSelect(bool show) => m_BgSelect.SetActive(show);
    }
    [SerializeField] private List<Tab> m_ButtonTabTs;
    [SerializeField] private GameObject m_PanelRule, m_PanelHistory;
    [SerializeField]
    private Transform m_BetBallsTf, m_PrefabTopRankTf, m_Prefab1DResultTf, m_Prefab2D3DResultTf, m_PrefabBetHistoryTf, m_PrefabWonHistoryTf;
    [SerializeField] private TextMeshProUGUI m_ChipTMP;
    [SerializeField] private TMP_InputField m_1DChipInputIF, m_2DChipInputIF, m_3DChipInputIF, m_2DNumberInputIF, m_3DNumberInputIF;
    [SerializeField] private ScrollRect m_TopRankSR, m_1DResultsSR, m_2DResultsSR, m_3DResultsSR, m_BetHistorySR, m_WonHistorySR;
    private Dictionary<int, HistoryBetLuckyNumber> _CurrentHistoryBets = new();
    private const float SCALE_VALUE = 1.1f, SCALE_DURATION = .1f;
    private int _count1DBetChip, _count2DBetChip, _count3DBetChip, _1DBetNumber, _2DBetNumber, _3DBetNumber, _currentTabId;

    #region Button
    public void DoClickButtonTab(int tabId)
    {
        for (int i = 0; i < m_ButtonTabTs.Count; i++)
        {
            m_ButtonTabTs[i].ShowHideBgSelect(i == tabId);
            m_1DResultsSR.transform.parent.gameObject.SetActive(tabId == 0);
            m_2DResultsSR.transform.parent.gameObject.SetActive(tabId == 1);
            m_3DResultsSR.transform.parent.gameObject.SetActive(tabId == 2);
        }
        _currentTabId = tabId + 1;
        SocketSend.SendGetNumber(_currentTabId);
        CallAsyncFunction(ShowResults());
        async Awaitable ShowResults()
        {
            Vector2 scaleV2 = Vector2.one * SCALE_VALUE;
            Transform contentTf = null;
            if (_currentTabId == 1) contentTf = m_1DResultsSR.content.transform;
            else if (_currentTabId == 2) contentTf = m_2DResultsSR.content.transform;
            else if (_currentTabId == 3) contentTf = m_3DResultsSR.content.transform;
            contentTf.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            foreach (Transform tf in contentTf) tf.gameObject.SetActive(false);
            foreach (Transform tf in contentTf)
            {
                tf.localScale = scaleV2;
                tf.gameObject.SetActive(true);
                await Awaitable.WaitForSecondsAsync(.05f);
                tf.DOScale(1, SCALE_DURATION);
            }
        }
    }
    public void DoClickBall(int ballId)
    {
        for (int i = 0; i < m_BetBallsTf.childCount; i++)
        {
            m_BetBallsTf.GetChild(i).GetComponent<Image>().sprite = Config.LoadLuckyBallById(i == ballId ? ballId : -1);
            _1DBetNumber = ballId;
        }
    }
    public void DoClickCreateBet1D()
    {
        SocketSend.SendCreateBetLuckyNumber(_currentTabId, _1DBetNumber, _count1DBetChip);
    }
    public void DoClickCreateBet2D()
    {
        SocketSend.SendCreateBetLuckyNumber(_currentTabId, _2DBetNumber, _count2DBetChip);
    }
    public void DoClickCreateBet3D()
    {
        SocketSend.SendCreateBetLuckyNumber(_currentTabId, _3DBetNumber, _count3DBetChip);
    }
    public void DoClickButtonRule()
    {
        m_PanelRule.SetActive(true);
    }
    public void DoClickButtonCloseRule()
    {
        m_PanelRule.SetActive(false);
    }
    public void DoClickButtonHistory()
    {
        SocketSend.SendWonHistory();
    }
    public void DoClickButtonCloseHistory()
    {
        m_PanelHistory.SetActive(false);
    }
    public void DoClickButtonShop()
    {
        UIManager.instance.openShop();
    }
    #endregion

    public void HandleGetStatusLuckyNumber(JObject data)
    {
        JArray dataTopRanks = JArray.Parse((string)data["data"]);
        for (int i = 0; i < dataTopRanks.Count; i++)
        {
            JToken item = dataTopRanks[i];
            TopRankLuckyNumber trln = Instantiate(m_PrefabTopRankTf, m_TopRankSR.content.transform).GetComponent<TopRankLuckyNumber>();
            trln.SetData(i, (string)item["N"], (int)item["AV"], (int)item["VIP"], (int)item["AG"]);
            trln.gameObject.SetActive(true);

        }
    }
    public void HandleGetLotteryByType(JObject data)
    {
        CallAsyncFunction(HandleData());
        async Awaitable HandleData()
        {
            _CurrentHistoryBets.Clear();
            foreach (Transform tf in m_BetHistorySR.content.transform) Destroy(tf.gameObject);
            JArray historyBets = JArray.Parse((string)data["data"]);
            for (int i = 0; i < historyBets.Count; i++)
            {
                JToken item = historyBets[i];
                HistoryBetLuckyNumber hbln = Instantiate(m_PrefabBetHistoryTf, m_BetHistorySR.content.transform).GetComponent<HistoryBetLuckyNumber>();
                hbln.SetData((int)item["StrNumber"], (int)item["ChipsBet"], (int)item["Id"], (int)item["TypeLottery"]);
                hbln.transform.localScale = Vector2.one * SCALE_VALUE;
                hbln.gameObject.SetActive(true);

                await Awaitable.WaitForSecondsAsync(.05f);
                hbln.gameObject.transform.DOScale(1, SCALE_DURATION);
                _CurrentHistoryBets.Add((int)item["Id"], hbln);
            }
        }
    }
    public void HandleGetLotteryResults(JObject data)
    {
        foreach (Transform tf in m_1DResultsSR.content.transform) Destroy(tf.gameObject);
        foreach (Transform tf in m_2DResultsSR.content.transform) Destroy(tf.gameObject);
        foreach (Transform tf in m_3DResultsSR.content.transform) Destroy(tf.gameObject);
        JObject results = JObject.Parse((string)data["data"]);
        JArray todayResults = (JArray)results["today"], othersResults = (JArray)results["others"];
        CallAsyncFunction(GenTabResultsData(todayResults));
        CallAsyncFunction(GenTabResultsData(othersResults));
        DoClickButtonTab(0);
        async Awaitable GenTabResultsData(JArray inputArray)
        {
            foreach (JToken item in inputArray)
            {
                if (!string.IsNullOrEmpty((string)item["strNumber"]))
                {
                    Vector2 scaleV2 = Vector2.one * SCALE_VALUE;
                    List<int> numbers = GetListResultsFromANumber((int)item["strNumber"]);
                    Transform r1tf = Instantiate(m_Prefab1DResultTf, m_1DResultsSR.content.transform);
                    r1tf.GetComponent<Result1DLuckyNumber>().SetData((long)item["CreateTime"], numbers.Last());
                    r1tf.localScale = scaleV2;
                    r1tf.gameObject.SetActive(true);

                    Transform r2tf = Instantiate(m_Prefab2D3DResultTf, m_2DResultsSR.content.transform);
                    r2tf.GetComponent<Result2D3DLuckyNumber>().SetData((long)item["CreateTime"], numbers);
                    r2tf.localScale = scaleV2;
                    r2tf.gameObject.SetActive(true);

                    Transform r3tf = Instantiate(m_Prefab2D3DResultTf, m_3DResultsSR.content.transform);
                    r3tf.GetComponent<Result2D3DLuckyNumber>().SetData((long)item["CreateTime"], numbers);
                    r3tf.localScale = scaleV2;
                    r3tf.gameObject.SetActive(true);

                    await Awaitable.WaitForSecondsAsync(.05f);
                    r1tf.DOScale(1, SCALE_DURATION);
                    r2tf.DOScale(1, SCALE_DURATION);
                    r3tf.DOScale(1, SCALE_DURATION);
                }
            }
        }
    }
    public void HandleCreateBet(JObject data)
    {
        JObject rawData = JObject.Parse((string)data["data"]);
        HistoryBetLuckyNumber hbln = Instantiate(m_PrefabBetHistoryTf, m_BetHistorySR.content.transform).GetComponent<HistoryBetLuckyNumber>();
        hbln.SetData((int)rawData["StrNumber"], (int)rawData["ChipsBet"], (int)rawData["Id"], (int)rawData["TypeLottery"]);
        hbln.gameObject.SetActive(true);
        hbln.transform.SetSiblingIndex(0);

        Dictionary<int, HistoryBetLuckyNumber> tempDict = new();
        tempDict.AddRange(_CurrentHistoryBets);
        _CurrentHistoryBets.Clear();
        _CurrentHistoryBets.Add((int)rawData["Id"], hbln);
        _CurrentHistoryBets.AddRange(tempDict);
        User.userMain.AG = (long)rawData["ag"];
        m_ChipTMP.text = Config.FormatNumber(User.userMain.AG);
    }
    public void HandleCancelBet(JObject data)
    {
        JObject rawData = JObject.Parse((string)data["data"]);
        if (((string)rawData["status"]).ToUpper().Equals("OK"))
        {
            foreach (KeyValuePair<int, HistoryBetLuckyNumber> kvp in _CurrentHistoryBets)
            {
                if (kvp.Key == (int)rawData["Id"])
                {
                    Destroy(kvp.Value.gameObject);
                    _CurrentHistoryBets.Remove(kvp.Key);
                    break;
                }
            }
        }
        User.userMain.AG = (int)rawData["ag"];
        m_ChipTMP.text = Config.FormatNumber(User.userMain.AG);
    }
    public void HandleWonHistory(JObject data)
    {
        m_PanelHistory.SetActive(true);
        foreach (Transform tf in m_WonHistorySR.content.transform) Destroy(tf.gameObject);
        JArray rawData = JArray.Parse((string)data["data"]);
        foreach (JToken item in rawData)
        {
            HistoryWonLuckyNumber hwln = Instantiate(m_PrefabWonHistoryTf, m_WonHistorySR.content.transform).GetComponent<HistoryWonLuckyNumber>();
            hwln.SetData((long)item["CreateTime"], (int)item["StrNumber"], (int)item["ChipsBet"], (int)item["ChipsWin"]);
            hwln.gameObject.SetActive(true);

        }
    }
    private List<int> GetListResultsFromANumber(int inputResult)
    {
        int firstNumber = inputResult / 100;
        int secondNumber = inputResult % 100 / 10;
        int thirdNumber = inputResult - firstNumber * 100 - secondNumber * 10;
        return new List<int>() { firstNumber, secondNumber, thirdNumber };
    }
    private async void CallAsyncFunction(Awaitable function)
    {
        try { await function; }
        // catch (Exception e) { Debug.LogError("Async Error: " + e.Message); }
        catch (Exception e) { if (e.GetType() != typeof(MissingReferenceException)) Debug.LogError("Error on calling async function: " + e.Message); }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        Instance = null;
    }
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        SocketSend.SendGetStatusLuckyNumber();
        _currentTabId = 1;
        DoClickBall(0);
        SocketSend.SendGetResultLuckyNumber();
        m_ChipTMP.text = Config.FormatNumber(User.userMain.AG);
        AddCheckCbToChipInputField(m_1DChipInputIF, 1);
        AddCheckCbToChipInputField(m_2DChipInputIF, 2);
        AddCheckCbToChipInputField(m_3DChipInputIF, 3);
        AddCheckCbToNumberInputField(m_2DNumberInputIF, 2);
        AddCheckCbToNumberInputField(m_3DNumberInputIF, 3);

        void AddCheckCbToChipInputField(TMP_InputField inputField, int tabId)
        {
            inputField.onValueChanged.AddListener(value =>
            {
                int.TryParse(value, out int betChip);
                int result = (int)Mathf.Min(betChip, User.userMain.AG);
                inputField.text = result <= 0 ? "" : result.ToString();
                if (tabId == 1) _count1DBetChip = result;
                else if (tabId == 2) _count2DBetChip = result;
                else if (tabId == 3) _count3DBetChip = result;
            });
        }
        void AddCheckCbToNumberInputField(TMP_InputField inputField, int tabId)
        {
            inputField.onValueChanged.AddListener(value =>
            {
                int.TryParse(value, out int betNumber);
                if (tabId == 2)
                {
                    int result = Mathf.Clamp(betNumber, 0, 99);
                    inputField.text = result <= 0 ? "" : result.ToString();
                    _2DBetNumber = result;
                }
                else if (tabId == 3)
                {
                    int result = Mathf.Clamp(betNumber, 0, 999);
                    inputField.text = result <= 0 ? "" : result.ToString();
                    _3DBetNumber = result;
                }
            });
        }
    }
}
