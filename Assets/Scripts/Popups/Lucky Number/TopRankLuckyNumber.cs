using System.Collections;
using System.Collections.Generic;
using Globals;
using TMPro;
using UnityEngine;

public class TopRankLuckyNumber : MonoBehaviour
{
    [SerializeField] private Transform m_IconsTf;
    [SerializeField] private Avatar m_AvatarA;
    [SerializeField] private TextMeshProUGUI m_NameTMP, m_ScoreTMP, m_RankNumberTMP;

    public void SetData(int rankId, string name, int avatarId, int vip, int score)
    {
        bool showIconRankOnTop3 = rankId < 3;
        m_IconsTf.gameObject.SetActive(showIconRankOnTop3);
        m_RankNumberTMP.gameObject.SetActive(!showIconRankOnTop3);
        if (showIconRankOnTop3) for (int i = 0; i < m_IconsTf.childCount; i++) m_IconsTf.GetChild(i).gameObject.SetActive(i == rankId);
        else m_RankNumberTMP.text = (rankId + 1).ToString();
        m_NameTMP.text = name;
        m_AvatarA.setSpriteWithID(avatarId);
        m_AvatarA.setVip(vip);
        m_ScoreTMP.text = "Win <b><color=#EAF400>" + Config.FormatMoney(score, true) + "</color></b> chips.";
    }
}
