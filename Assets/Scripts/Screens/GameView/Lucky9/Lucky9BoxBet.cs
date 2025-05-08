using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;


public class Lucky9BoxBet : MonoBehaviour
{
    public TextMeshProUGUI lb_chipbet;
    public int totalValue = 0;

    public void onBet(int ag)
    {
        totalValue += ag;
        Debug.Log(totalValue);
        lb_chipbet.text = Globals.Config.FormatMoney2(totalValue);
        Debug.Log(lb_chipbet.text);
    }

    public void onReset()
    {
        totalValue = 0;
        gameObject.SetActive(false);
    }
}
