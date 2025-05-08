using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoxbetView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lb_chipbet;

    [SerializeField]
    GameObject iconChip;

    public int currentValue = 0, value = 0, valueChange = 0;
    void Start()
    {

    }
    public void setValue(int value)
    {
        lb_chipbet.text = Globals.Config.FormatMoney2(value, true);
        gameObject.SetActive(true);
    }
    public void onShow(int index)
    {
        lb_chipbet.gameObject.SetActive(true);
        transform.gameObject.SetActive(true);
        if (Globals.Config.curGameId == (int)Globals.GAMEID.LUCKY_89) return;

        Vector2 pos = transform.localPosition;
        Vector2 startPos = getStartPosition(index);
        transform.localPosition = startPos;
        // this.lb_chipbet.node.opacity = 0;
        gameObject.GetComponent<Material>().color = Color.clear;

        //DOTween.Sequence()
        //    .Append()

        transform.DOLocalMove(pos, 0.4f).SetEase(Ease.OutCubic);
        //gameObject.GetComponent<Material>().DOFade(Color.white, 0.4f);
        //this.node.runAction(
        //    cc.spawn(
        //        cc.moveTo(0.4, pos).easing(cc.easeCubicActionOut()),
        //        cc.fadeIn(0.4),
        //        cc.sequence(
        //            cc.scaleTo(0.2, 1.2).easing(cc.easeCubicActionOut()),
        //            cc.scaleTo(0.2, 1).easing(cc.easeCubicActionIn()),
        //        )
        //    )
        //)

    }
    private Vector2 getStartPosition(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector2(-115, -173);
            case 1:
                return new Vector2(-420, -173);
            case 2:
                return new Vector2(-513, 39);
            case 3:
                return new Vector2(-314, 255);
            case 4:
                return new Vector2(315, 255);
            case 5:
                return new Vector2(546, 39);
            case 6:
                return new Vector2(428, -173);
            case 7:
                return new Vector2(-3, 223);
        }
        return Vector2.zero;
    }
    public void setValueBoogyi(int valueBet, float delayTime = 0)
    {
        value = valueBet;
        lb_chipbet.text = Globals.Config.FormatMoney(value);
    }
    // Update is called once per frame

}
