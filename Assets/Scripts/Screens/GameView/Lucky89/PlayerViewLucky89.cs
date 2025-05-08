using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Globals;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class PlayerViewLucky89 : PlayerView
{
    public enum BetInfoPosition { NONE, ABOVE, RIGHT, BELLOW, LEFT }
    [SerializeField] private List<Card> m_CardCs;
    [SerializeField] private List<GameObject> m_Rates;
    [SerializeField] private SkeletonGraphic m_LuckySG, m_WinSg, m_LoseSg, m_DrawSG;
    [SerializeField] private TextMeshProUGUI m_BetTMP, m_ScoreTMP;
    private BetInfoPosition _BetInfoBIP = BetInfoPosition.ABOVE;
    private int _BetValue;

    public PlayerViewLucky89 ShowAnimResult(bool show, long changedChips)
    {
        bool isWin = changedChips > 0, isDraw = changedChips == 0, isLose = changedChips < 0;
        m_WinSg.gameObject.SetActive(show && isWin);
        m_DrawSG.gameObject.SetActive(show && isDraw);
        m_LoseSg.gameObject.SetActive(show && isLose);
        if (!show) return this;
        if (isWin) m_WinSg.AnimationState.SetAnimation(0, "win", false);
        if (isDraw) m_DrawSG.AnimationState.SetAnimation(0, "draw", false);
        if (isLose) m_LoseSg.AnimationState.SetAnimation(0, "lose", false);
        return this;
    }
    public PlayerViewLucky89 HideAllCards()
    {
        foreach (Card card in m_CardCs) card.gameObject.SetActive(false);
        return this;
    }
    public PlayerViewLucky89 UpdateCardsParentPositionAndRotation()
    {
        Card thirdCard = m_CardCs.Last();
        bool isShowAll = thirdCard.gameObject.activeSelf;
        RectTransform cardsParentRT = thirdCard.transform.parent.GetComponent<RectTransform>();
        float tweenDuration = .2f;
        cardsParentRT.DOLocalMoveX(isShowAll ? 0 : 10, tweenDuration);
        cardsParentRT.DOLocalRotate(new Vector3(0, 0, isShowAll ? 0 : -15), tweenDuration);
        return this;
    }
    public List<Card> GetListCards() { return m_CardCs; }
    public Card GetACard()
    {
        foreach (Card cardC in m_CardCs) if (!cardC.gameObject.activeSelf) return cardC;
        return null;
    }
    public PlayerViewLucky89 ShowScore(bool show, int score)
    {
        bool isLucky = score >= (int)Lucky89View.SCORE.LUCKY_8;
        m_LuckySG.gameObject.SetActive(show && isLucky);
        m_ScoreTMP.transform.parent.gameObject.SetActive(show && !isLucky);
        if (!show) return this;
        if (score >= (int)Lucky89View.SCORE.LUCKY_9) m_LuckySG.AnimationState.SetAnimation(0, "lucky9", false);
        else if (score >= (int)Lucky89View.SCORE.LUCKY_8) m_LuckySG.AnimationState.SetAnimation(0, "lucky8", false);
        else if (score >= (int)Lucky89View.SCORE.THREE_OF_A_KIND) m_ScoreTMP.text = "Three of a kind";
        else if (score >= (int)Lucky89View.SCORE.FACE_CARDS) m_ScoreTMP.text = "Face cards";
        else if (score >= (int)Lucky89View.SCORE.STRAIGHT_FLUSH) m_ScoreTMP.text = "Straight flush";
        else if (score >= (int)Lucky89View.SCORE.FLUSH) m_ScoreTMP.text = "Flush";
        else m_ScoreTMP.text = score + " points";
        return this;
    }
    public PlayerViewLucky89 ShowRate(int rate)
    {
        if (rate < 2) foreach (GameObject go in m_Rates) go.SetActive(false);
        else for (int i = 0; i < m_Rates.Count; i++) m_Rates[i].SetActive(i == rate - 2);
        return this;
    }
    public PlayerViewLucky89 SetBetPosition(int idPlayerview)
    {
        switch (idPlayerview)
        {
            case 0:
            case 1:
            case 6:
                _BetInfoBIP = BetInfoPosition.ABOVE;
                break;
            case 2:
                _BetInfoBIP = BetInfoPosition.RIGHT;
                break;
            case 3:
            case 4:
                _BetInfoBIP = BetInfoPosition.BELLOW;
                break;
            case 5:
                _BetInfoBIP = BetInfoPosition.LEFT;
                break;
        }
        return this;
    }
    public PlayerViewLucky89 ShowHideBetChips(bool show, int betValue = 0)
    {
        _BetValue = betValue;
        RectTransform rt = m_BetTMP.transform.parent.GetComponent<RectTransform>();
        rt.gameObject.SetActive(show);
        if (_BetInfoBIP == BetInfoPosition.ABOVE) rt.anchoredPosition = new Vector2(5, 70);
        else if (_BetInfoBIP == BetInfoPosition.RIGHT) rt.anchoredPosition = new Vector2(130, -5);
        else if (_BetInfoBIP == BetInfoPosition.BELLOW) rt.anchoredPosition = new Vector2(5, -130);
        else if (_BetInfoBIP == BetInfoPosition.LEFT) rt.anchoredPosition = new Vector2(-130, -5);
        else rt.anchoredPosition = Vector2.zero;
        if (_BetValue > 0) m_BetTMP.text = Config.FormatMoney(_BetValue, true).ToString();
        return this;
    }
    public int GetBetValue() { return _BetValue; }
}
