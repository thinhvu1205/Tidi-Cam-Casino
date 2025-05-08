using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChipBetSicbo : MonoBehaviour
{
    // Start is called before the first frame update
    public int pid = 0, numberBet = 0, chipDeal = 0, chipBetSicbo = 0;
    private List<int> listValue;
    public List<List<ChipBetSicbo>> listChipBetOnGate = new List<List<ChipBetSicbo>>();
    private List<int> listWinResult = new List<int>();
    [SerializeField] List<Sprite> listSprChip;


    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void resetValue()
    {
        pid = 0;
        numberBet = 0;
        chipDeal = 0;
        chipBetSicbo = 0;
    }
    private void Awake()
    {
        listValue = HiloView.instance.listValue;
    }
    public void chipMoveTo(Vector2 pos, bool isActive = true, TweenCallback cb = null)
    {
        DOTween.Sequence()
                .Append(transform.DOLocalMove(pos, 0.3f).SetEase(Ease.InSine)).OnComplete(() =>
                //.Append(transform.DOLocalJump(pos, 20, 3, 0.5f)).SetEase(Ease.InSine).OnComplete(() =>
                {
                    gameObject.SetActive(isActive);
                    if (cb != null)
                    {
                        DOTween.Sequence().AppendCallback(cb);
                        cb = null;
                    }
                });
    }
    public void setChip(int value, int numberBetValue)
    {

        int chipBet = value;

        chipDeal += value;
        //Globals.Logging.Log("Set Chip:" + chipDeal);
        numberBet = numberBetValue;
        chipBetSicbo = value;
        Sprite spr = null;
        for (int i = 0; i < listValue.Count; i++)
        {
            if (chipBet >= listValue[i])
            {
                spr = listSprChip[i];
            }
        }
        if (spr == null)
        {
            spr = listSprChip[5];
        }
        transform.localScale = new Vector2(0.5f, 0.5f);
        GetComponent<Image>().sprite = spr;
        TextMeshProUGUI labelText = transform.Find("lbText").GetComponent<TextMeshProUGUI>();
        labelText.text = Globals.Config.FormatMoney(chipBet, true);
    }
    public void chipMoveToPlayer(float delayTime, Vector2 pos, bool isActive = true)
    {


        DOTween.Sequence()
            .AppendInterval(delayTime)
            .Append(transform.DOLocalMove(pos, 0.4f).SetEase(Ease.OutSine))
        //.Append(transform.DOLocalJump(pos, 20, 3, 0.5f).SetEase((Ease.InSine)))
        .AppendCallback(() =>
        {
            gameObject.SetActive(isActive);
           
        });
    }
}
