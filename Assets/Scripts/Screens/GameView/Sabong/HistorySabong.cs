using System;
using System.Collections;
using System.Collections.Generic;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistorySabong : BaseView
{
    private static HistorySabong _Instance;
    private HistorySabong() { }
    [SerializeField] private List<Transform> m_ResultColumnsTfs;
    [SerializeField] private Transform m_PrefabResultTf, m_RecentResultsTf;
    [SerializeField] private Image m_MeronBgImg, m_WalaBgImg;
    [SerializeField] private TextMeshProUGUI m_MeronPercentTMP, m_WalaPercentTMP;
    private const int RESULTS_IN_A_ROW = 20, RESULTS_IN_A_COLUMN = 5;

    #region Button
    public void DoClickClose()
    {
        _Instance.gameObject.SetActive(false);
    }
    #endregion

    private void Init(List<int> results)
    {
        foreach (Transform tf in m_RecentResultsTf) tf.gameObject.SetActive(false);
        foreach (Transform columnTf in m_ResultColumnsTfs)
            foreach (Transform childTf in columnTf)
                childTf.gameObject.SetActive(false);
        List<int> recentResults = new();
        for (int i = Math.Max(0, results.Count - RESULTS_IN_A_ROW); i < results.Count; i++) recentResults.Add(results[i]);
        for (int i = 0; i < recentResults.Count; i++) ShowResult(m_RecentResultsTf.GetChild(i), recentResults[i]);

        List<List<int>> resultsEachColumn = new();
        List<int> resultsAColumn = new();
        int countMeron = 0, countWala = 0, countDraw = 0;
        for (int i = 0; i < results.Count; i++)
        {
            if (i == 0)
            {
                resultsAColumn.Add(results[i]);
            }
            else
            {
                if (results[i] == results[i - 1])
                {
                    if (resultsAColumn.Count < RESULTS_IN_A_COLUMN) resultsAColumn.Add(results[i]);
                    else
                    {
                        List<int> temp = new();
                        foreach (int num in resultsAColumn) temp.Add(num);
                        resultsEachColumn.Add(temp);
                        resultsAColumn.Clear();
                        resultsAColumn.Add(results[i]);
                    }
                }
                else
                {
                    List<int> temp = new();
                    foreach (int num in resultsAColumn) temp.Add(num);
                    resultsEachColumn.Add(temp);
                    resultsAColumn.Clear();
                    resultsAColumn.Add(results[i]);
                }
            }
            if (i == results.Count - 1) resultsEachColumn.Add(resultsAColumn);
        }
        for (int i = 0; i < resultsEachColumn.Count; i++)
        {
            if (resultsEachColumn.Count > RESULTS_IN_A_ROW)
            {
                resultsEachColumn.RemoveAt(0);
                i--;
            }
        }
        for (int i = 0; i < resultsEachColumn.Count; i++)
            for (int j = 0; j < resultsEachColumn[i].Count; j++)
                ShowResult(m_ResultColumnsTfs[i].GetChild(j), resultsEachColumn[i][j]);
        foreach (List<int> item in resultsEachColumn)
        {
            foreach (int num in item)
            {
                if (num == 1) countMeron++;
                else if (num == 7) countWala++;
                else countDraw++;
            }
        }
        int total = countMeron + countDraw + countWala;
        float percentMeron = (float)countMeron / total;
        float percentWala = (float)countWala / total;
        m_MeronBgImg.fillAmount = percentMeron;
        m_WalaBgImg.fillAmount = percentWala;
        m_MeronPercentTMP.text = string.Format("{0:N2}", percentMeron * 100) + "%";
        m_WalaPercentTMP.text = string.Format("{0:N2}", percentWala * 100) + "%";
    }
    private void ShowResult(Transform tf, int result)
    {
        tf.gameObject.SetActive(true);
        tf.GetChild(0).gameObject.SetActive(result == 1); //meron
        tf.GetChild(1).gameObject.SetActive(result == 7); //wala
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        _Instance = null;
    }
    public static HistorySabong Show(HistorySabong prefab, Transform parentTf, List<int> results)
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(prefab, parentTf);
            _Instance.transform.localScale = Vector3.one;
            _Instance.transform.localPosition = Vector3.zero;

        }
        else
        {
            _Instance.gameObject.SetActive(true);
        }
        _Instance.Init(results);
        return _Instance;
    }
}
