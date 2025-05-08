using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemHistorySicbo : MonoBehaviour
{
    [SerializeField] private List<Sprite> m_TaiXiuSs;
    [SerializeField] private Image m_BackgroundImg;
    [SerializeField] private TextMeshProUGUI m_NumberTMP;

    public void SetData(int number, bool showTextNumber)
    {
        gameObject.SetActive(true);
        bool isTai = number > 10;
        m_BackgroundImg.sprite = m_TaiXiuSs[isTai ? 0 : 1];
        if (!showTextNumber)
        {
            m_NumberTMP.gameObject.SetActive(false);
            return;
        }
        m_NumberTMP.gameObject.SetActive(true);
        m_NumberTMP.text = number.ToString();
        m_NumberTMP.color = isTai ? Color.white : Color.black;
    }
}
