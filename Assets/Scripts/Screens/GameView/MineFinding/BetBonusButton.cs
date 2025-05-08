using Globals;
using TMPro;
using UnityEngine;

public class BetBonusButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_BetTMP;
    [SerializeField] private GameObject m_BgUnselect;
    private float betBonus;

    public void SetData(float _betBonus)
    {
        betBonus = _betBonus;
        m_BetTMP.text = _betBonus < 100 ? betBonus.ToString() : Config.FormatMoney2((long)betBonus, true);
        TurnUnselect(true);
    }
    public void TurnUnselect(bool show)
    {
        m_BgUnselect.SetActive(show);
    }
    public float GetBetBonus() { return betBonus; }
}
