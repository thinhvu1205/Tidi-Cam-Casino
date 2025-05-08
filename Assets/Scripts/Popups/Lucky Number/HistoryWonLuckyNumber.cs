using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HistoryWonLuckyNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_DateTMP, m_NumberTMP, m_ChipBetTMP, m_ChipWonTMP;

    public void SetData(long dateMilliseconds, int number, int betChips, int wonChips)
    {
        m_DateTMP.text = DateTimeOffset.FromUnixTimeMilliseconds(dateMilliseconds).DateTime.ToString("dd/MM/yyyy hh tt");
        m_NumberTMP.text = number.ToString();
        m_ChipBetTMP.text = betChips.ToString();
        m_ChipWonTMP.text = wonChips.ToString();
    }
}
