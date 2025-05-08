using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro;
using Newtonsoft.Json.Linq;
using System;

public class HistorySicbo : BaseView
{
    private enum TYPE { TAI, XIU }
    [SerializeField] private Transform m_TopTableTf, m_BottomTableTf;
    [SerializeField] private Image m_BgTopTableXiuImg, m_BgBottomTableXiuImg;
    [SerializeField] private TextMeshProUGUI m_TopTableXiuTMP, m_TopTableTaiTMP, m_BottomTableXiuTMP, m_BottomTableTaiTMP;
    private List<List<ItemHistorySicbo>> _TopColumnIHSs = new(), _BottomColumnIHSs = new();

    public void handleDataHistory(List<List<int>> data)
    {
        Debug.Log("dataHistory size=" + data.Count); // [[2, 5, 6], [1, 4, 6], [2, 4, 5]] 
        List<HistoryData> sumTotalHDs = new();
        int countXiuTotal = 0;
        for (int i = data.Count - 1; i >= 0; i--)
        {
            int sum = 0;
            foreach (int number in data[i]) sum += number;
            bool isTai = sum > 10;
            sumTotalHDs.Insert(0, new() { Sum = sum, Type = isTai ? TYPE.TAI : TYPE.XIU });
            if (!isTai) countXiuTotal++;
            if (sumTotalHDs.Count > 100) break; // max 100 ô
        }
        if (sumTotalHDs.Count <= 0)
        {
            m_BgTopTableXiuImg.fillAmount = m_BgBottomTableXiuImg.fillAmount = .5f;
            m_TopTableXiuTMP.text = m_TopTableTaiTMP.text = m_BottomTableXiuTMP.text = m_BottomTableTaiTMP.text = "50%";
            return;
        }
        float totalXiuPercent = Mathf.FloorToInt((float)countXiuTotal / sumTotalHDs.Count * 100);
        m_BgBottomTableXiuImg.fillAmount = totalXiuPercent / 100;
        m_BottomTableXiuTMP.text = totalXiuPercent + " %";
        m_BottomTableTaiTMP.text = 100 - totalXiuPercent + " %";

        List<List<HistoryData>> topTableHDs = new();
        int idNow = sumTotalHDs.Count - 1, countXiuTopTable = 0, countTaiTopTable = 0;
        while (idNow >= 0)
        {
            if (sumTotalHDs[idNow].Type == TYPE.TAI) countTaiTopTable++;
            else countXiuTopTable++;
            List<HistoryData> sameTypeHDs = new() { sumTotalHDs[idNow] };
            while (idNow > 0 && sumTotalHDs[idNow].Type == sumTotalHDs[idNow - 1].Type)
            {
                sameTypeHDs.Insert(0, sumTotalHDs[idNow - 1]);
                if (sumTotalHDs[idNow - 1].Type == TYPE.TAI) countTaiTopTable++;
                else countXiuTopTable++;
                idNow--;
            }
            topTableHDs.Insert(0, sameTypeHDs);
            if (topTableHDs.Count >= 20) break;
            idNow--;
        }
        float topTableXiuPercent = Mathf.FloorToInt((float)countXiuTopTable / (countXiuTopTable + countTaiTopTable) * 100);
        m_BgTopTableXiuImg.fillAmount = topTableXiuPercent / 100;
        m_TopTableXiuTMP.text = topTableXiuPercent + " %";
        m_TopTableTaiTMP.text = 100 - topTableXiuPercent + " %";

        idNow = topTableHDs.Count - 1;
        int lastShownId = topTableHDs[idNow].Count - 1;
        for (int i = _TopColumnIHSs.Count - 1; i >= 0; i--)
        {
            bool showColumn = idNow >= 0;
            _TopColumnIHSs[i][0].transform.parent.gameObject.SetActive(showColumn); // show theo cột
            if (!showColumn) continue;
            for (int j = _TopColumnIHSs[i].Count - 1; j >= 0; j--)
            {
                bool showItem = j <= lastShownId % 5;
                _TopColumnIHSs[i][j].gameObject.SetActive(showItem); // show theo hàng
                if (!showItem) continue;
                _TopColumnIHSs[i][j].SetData(topTableHDs[idNow][lastShownId].Sum, true);
                lastShownId--;
            }
            if (lastShownId < 0)
            {
                idNow--;
                if (idNow >= 0) lastShownId = topTableHDs[idNow].Count - 1;
            }
        }
        for (int i = 0; i < _BottomColumnIHSs.Count; i++)
        {
            bool isColumnShown = i < Mathf.CeilToInt((float)sumTotalHDs.Count / 5);
            _BottomColumnIHSs[i][0].transform.parent.gameObject.SetActive(isColumnShown);
            if (!isColumnShown) continue;
            for (int j = 0; j < _BottomColumnIHSs[i].Count; j++)
            {
                int idInTotal = i * 5 + j;
                bool isItemShown = idInTotal < sumTotalHDs.Count;
                _BottomColumnIHSs[i][j].gameObject.SetActive(isItemShown);
                if (!isItemShown) continue;
                _BottomColumnIHSs[i][j].SetData(sumTotalHDs[idInTotal].Sum, false);
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        foreach (Transform columnTf in m_TopTableTf)
        {
            List<ItemHistorySicbo> columnsIHSs = new();
            foreach (Transform itemTf in columnTf) columnsIHSs.Add(itemTf.GetComponent<ItemHistorySicbo>());
            _TopColumnIHSs.Add(columnsIHSs);
        }
        foreach (Transform columnTf in m_BottomTableTf)
        {
            List<ItemHistorySicbo> columnsIHSs = new();
            foreach (Transform itemTf in columnTf) columnsIHSs.Add(itemTf.GetComponent<ItemHistorySicbo>());
            _BottomColumnIHSs.Add(columnsIHSs);
        }
    }
    private class HistoryData
    {
        public int Sum;
        public TYPE Type;
    }
}
