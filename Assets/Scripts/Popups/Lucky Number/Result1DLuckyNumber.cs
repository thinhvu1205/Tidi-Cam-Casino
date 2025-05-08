using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Result1DLuckyNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_DateTMP, m_ResultTMP;

    public void SetData(long dateMiliseconds, int result)
    {
        m_DateTMP.text = DateTimeOffset.FromUnixTimeMilliseconds(dateMiliseconds).DateTime.ToString("dd/MM/yyyy");
        m_ResultTMP.text = result.ToString();
    }
}
