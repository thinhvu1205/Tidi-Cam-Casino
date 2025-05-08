using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PotView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI money;
    [SerializeField] TextMeshProUGUI round;

    public void SetInfo(int moneyy, int roundd)
    {
        money.gameObject.transform.DOScale(new Vector2(1.3f, 1.3f), 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            if (moneyy >= 100000)
                money.text = Globals.Config.FormatMoney2(moneyy, true);
            else
                money.text = Globals.Config.FormatMoney(moneyy);
            money.gameObject.transform.DOScale(Vector2.one, 0.2f).SetEase(Ease.InOutCubic);
        });
        round.gameObject.transform.DOScale(new Vector2(1.3f, 1.3f), 0.2f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            round.text = "Round " + roundd;
            round.gameObject.transform.DOScale(Vector2.one, 0.2f).SetEase(Ease.InOutCubic);
        });
        ;
    }
}