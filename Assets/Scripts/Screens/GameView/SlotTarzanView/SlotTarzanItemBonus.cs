using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Spine.Unity;
public class SlotTarzanItemBonus : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public SkeletonGraphic animScore;
    [SerializeField] public TextMeshProUGUI lbValue;
    public int index = 0;
    public bool isOpen = false;
    void Start()
    {

    }

    // Update is called once per frame
    public void showResult(int value, int idItem)
    {
        Globals.Logging.Log(value);
        isOpen = true;

        animScore.gameObject.SetActive(true);
        animScore.Initialize(true);
        animScore.AnimationState.SetAnimation(0, "animation", true);
        animScore.AnimationState.Complete += delegate
        {
            //animScore.gameObject.SetActive(false);
        };
        lbValue.text = Globals.Config.FormatMoney2(value, true);
        if (idItem == 0 || idItem == 15 || idItem == 16)
        {
            lbValue.text = "+" + value;
        }
        else
        {
            SlotTarzanMiniGameView.instance.updateTotalWin(value);
        }
        lbValue.gameObject.SetActive(true);
        lbValue.transform.localScale = Vector2.zero;
        lbValue.transform
            .DOScale(new Vector2(1.0f, 1.0f), 0.1f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                if (idItem == 0 || idItem == 15 || idItem == 16)
                {
                    SlotTarzanMiniGameView.instance.addPickTurn(this, value);
                }
                if (SlotTarzanMiniGameView.instance.pickLeft == 0 && idItem != 0 && idItem != 15 && idItem != 16)
                {
                    DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
                    {
                        SlotTarzanMiniGameView.instance.showPopupResult();
                    });
                }
                SlotTarzanMiniGameView.instance.canClick = true;
            });

    }
    public void Reset()
    {
        isOpen = false;
        lbValue.text = "?";
        animScore.gameObject.SetActive(false);
    }

}
