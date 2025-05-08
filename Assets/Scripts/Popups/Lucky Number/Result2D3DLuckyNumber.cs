using System;
using System.Collections;
using System.Collections.Generic;
using Globals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Result2D3DLuckyNumber : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> m_BallTMPs;
    [SerializeField] private List<Image> m_BallImgs;
    [SerializeField] private TextMeshProUGUI m_LeftTextTMP, m_RightTextTMP;

    public void SetData(long dateMiliseconds, List<int> ballCodes)
    {
        string dateFormat = DateTimeOffset.FromUnixTimeMilliseconds(dateMiliseconds).DateTime.ToString("dd/MM hh tt");
        string[] strings = dateFormat.Split(' ');
        m_LeftTextTMP.text = strings[0];
        m_RightTextTMP.text = "Lotto " + strings[1] + strings[2];
        for (int i = 0; i < m_BallImgs.Count; i++)
        {
            bool isShow = i < ballCodes.Count;
            m_BallImgs[i].gameObject.SetActive(isShow);
            if (isShow)
            {
                m_BallTMPs[i].text = ballCodes[i].ToString();
                m_BallImgs[i].sprite = Config.LoadLuckyBallById(ballCodes[i]);
            }
        }
    }
}
