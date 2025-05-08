using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTabBet : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI txtBet, txtBetOn;
    [SerializeField]
    private Material gray_bg;

    public void setInfo(int bet, int nUser)
    {
        txtBet.text = Globals.Config.FormatMoney(bet, true);
        txtBetOn.text = txtBet.text;
        //txtUser.text = nUser.ToString();
    }
}
