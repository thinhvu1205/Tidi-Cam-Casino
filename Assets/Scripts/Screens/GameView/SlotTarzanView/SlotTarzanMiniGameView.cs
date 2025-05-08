using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Newtonsoft.Json.Linq;
using DG.Tweening;
using Spine.Unity;
using Globals;
public class SlotTarzanMiniGameView : BaseView
{
    public static SlotTarzanMiniGameView instance = null;
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI lbPickLeft;
    [SerializeField] TextMeshProUGUI lbTotalWin;
    [SerializeField] Transform itemContainer;
    SlotTarzanView gameView;
    private List<SlotTarzanItemBonus> listItem = new List<SlotTarzanItemBonus>();
    private SlotTarzanItemBonus currentItemPick;
    public int pickLeft = 0;
    public int totalWin = 0;
    public bool canClick = true;

    protected override void Start()
    {
        base.Start();
        SlotTarzanMiniGameView.instance = this;
        listItem.AddRange(itemContainer.GetComponentsInChildren<SlotTarzanItemBonus>());
        listItem.ForEach(itemPick =>
        {
            itemPick.index = listItem.IndexOf(itemPick);
        });
        gameView = (SlotTarzanView)UIManager.instance.gameView;
    }
    public void onClickItem(SlotTarzanItemBonus item)
    {
        if (pickLeft > 0 && canClick)
        {
            if (!item.isOpen)
            {
                pickLeft--;
                lbPickLeft.text = pickLeft.ToString();
                SocketSend.sendSpinSlot(gameView.currentMarkBet, gameView.isFreeSpin, item.index);
                canClick = false;
            }
        }

    }
    public void setInfo(JObject dataBonus, bool isPicking = false)
    {

        pickLeft = (int)dataBonus["numSpin"];
        if (!isPicking)
        {
            lbPickLeft.text = pickLeft + "";
        }
        List<JObject> listData = new List<JObject>();
        JArray views = (JArray)dataBonus["view"];
        foreach (JArray dataView in views)
        {
            foreach (JObject data in dataView)
            {
                listData.Add(data);
            }
        }
        for (int i = 0, l = listData.Count; i < l; i++)
        {
            JObject dataItem = listData[i];
            if ((int)dataItem["id"] == -1)
            {
                listItem[i].Reset();
            }
            else
            {
                if (!listItem[i].isOpen)
                {
                    listItem[i].showResult((int)dataItem["value"], (int)dataItem["id"]);
                }
                else
                {
                    listItem[i].animScore.gameObject.SetActive(false);
                }

            }
        }
    }
    public void addPickTurn(SlotTarzanItemBonus item, int value)
    {
        TextMeshProUGUI lbValueEff = Instantiate(item.lbValue.gameObject, transform).GetComponent<TextMeshProUGUI>();

        lbValueEff.transform.localScale = Vector2.one;
        lbValueEff.transform.localPosition = transform.InverseTransformPoint(item.lbValue.transform.position);
        DOTween.Sequence()
            .Append(lbValueEff.transform.DOLocalMove(lbPickLeft.transform.localPosition, 1.0f).SetEase(Ease.InBack))
            .Join(lbValueEff.transform.DOScale(new Vector2(0.5f, 0.5f), 1.0f))
            .AppendCallback(() =>
            {
                lbPickLeft.text = pickLeft.ToString();
                lbPickLeft.transform.localScale = Vector2.zero;
                lbPickLeft.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutBack);
                Destroy(lbValueEff.gameObject);
            });
    }
    public void updateTotalWin(int value)
    {
        Globals.Config.tweenNumberFromK(lbTotalWin, totalWin + value, totalWin, 0.3f, true);
        totalWin += value;
    }
    public void showPopupResult()
    {
        gameView.showPopupResultMinigame(totalWin);
    }
    public void resetUI()
    {
        totalWin = 0;
        pickLeft = 0;
        lbPickLeft.text = "";
        lbTotalWin.text = "0";
        listItem.ForEach(item =>
        {
            item.Reset();
        });
        onClickClose(false);
    }
}
