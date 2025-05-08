using System;
using System.Collections;
using System.Collections.Generic;
using Globals;
using TMPro;
using UnityEngine;

public class HistoryBetLuckyNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_BetNumberTMP, m_BetContentTMP;
    private Action _OnDeleteCb;
    private int _betId, _betType;

    #region Button
    public void DoClickDeleteHistory()
    {
        UIManager.instance.showDialog("You will lose 10% of the bet as fee, do you want to cancel bet?", "Confirm", () =>
        {
            SocketSend.SendCancelBetLuckyNumber(_betId, _betType);
        }, "Back", null);
    }
    #endregion

    public void SetData(int betNumber, int betChips, int betId, int betType)
    {
        _betId = betId;
        _betType = betType;
        m_BetNumberTMP.text = betNumber.ToString();
        m_BetContentTMP.text = "Bet " + Config.FormatNumber(betChips) + " chips";
    }
}
