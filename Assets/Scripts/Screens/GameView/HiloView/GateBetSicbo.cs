using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Globals;

public class GateBetSicbo : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<Button> listButtonBet;

    [SerializeField]
    public List<TextMeshProUGUI> listLabelGateBet;

    [SerializeField]
    public GameObject chip_bet_prefab;

    [SerializeField]
    public GameObject mask;

    [SerializeField]
    public Sprite noImg;
    [SerializeField]
    private bool isHilo = false;

    private HiloView sicboGameView;
    public List<JObject> dataWin = new List<JObject>();
    public List<int> listWinResult = new List<int>();
    private int t1 = 0, t2 = 0, t3 = 0, t4 = 0, t5 = 0, t6 = 0, t7 = 0, t8 = 0, t9 = 0, t10 = 0;
    private int t11 = 0, t12 = 0, t13 = 0, t14 = 0, t15 = 0, t16 = 0, t17 = 0, t18 = 0, t19 = 0, t20 = 0;
    private int t21 = 0, t22 = 0, t23 = 0, t24 = 0, t25 = 0, t26 = 0, t27 = 0, t28 = 0, t29 = 0, t30 = 0, t31 = 0, t32 = 0;
    private List<List<ChipBetSicbo>> listChipBetOnGate = new List<List<ChipBetSicbo>>();
    private List<ChipBetSicbo> chipPool = new List<ChipBetSicbo>();
    private List<int> listValueGateWin = new List<int>();
    private List<ChipBetSicbo> listChipPerGate = new List<ChipBetSicbo>();
    private List<ChipBetSicbo> listChipWinPerGate = new List<ChipBetSicbo>();
    private List<int> listGateTotalValue = new List<int>();
    void Start()
    {
        listButtonBet.ForEach(btn =>
        {
            btn.GetComponent<Image>().sprite = noImg;
        });
        for (int i = 0; i <= (isHilo ? 32 : 29); i++)
        {
            ChipBetSicbo initChip = new ChipBetSicbo();
            listChipPerGate.Add(initChip);
            listChipWinPerGate.Add(initChip);
        }
    }
    private void OnDisable()
    {
        chipPool.ForEach(chip =>
        {
            Destroy(chip.gameObject);
        });
    }
    public void removeChipInTable()
    {
        listChipPerGate.ForEach(objChip =>
        {
            if (objChip != null)
            {
                objChip.gameObject.SetActive(false);
            }
        });
        listChipWinPerGate.ForEach(objChip =>
        {
            if (objChip != null)
            {
                objChip.gameObject.SetActive(false);
            }
        });
    }
    public void resetOCuoc()
    {
        for (int i = 0; i < listButtonBet.Count; i++)
        {
            Button objButton = listButtonBet[i];
            Sprite spr = objButton.GetComponent<Image>().sprite;
            GameObject btnPress = objButton.transform.Find("press").gameObject;
            if (btnPress != null)
            {
                //btnPress.GetComponent<Image>().sprite = spr;
                btnPress.SetActive(true);
                btnPress.GetComponent<Image>().enabled = false;
            }
        }
    }
    private void Awake()
    {
        sicboGameView = HiloView.instance;
        resetGateBet();
        setStateButtonBet(true);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void setStateButtonBet(bool isActive = false)
    {
        mask.SetActive(isActive);
    }
    public void resetGateBet()
    {
        resetOCuoc();
        listWinResult.Clear();
        dataWin.Clear();

        t1 = 0; t2 = 0; t3 = 0; t4 = 0; t5 = 0; t6 = 0; t7 = 0; t8 = 0; t9 = 0; t10 = 0;
        t11 = 0; t12 = 0; t13 = 0; t14 = 0; t15 = 0; t16 = 0; t17 = 0; t18 = 0; t19 = 0; t20 = 0;
        t21 = 0; t22 = 0; t23 = 0; t24 = 0; t25 = 0; t26 = 0; t27 = 0; t28 = 0; t29 = 0; t30 = 0; t31 = 0; t32 = 0;


        listLabelGateBet.ForEach(lb =>
        {
            lb.gameObject.SetActive(false);
            lb.text = "0";
        });
        listGateTotalValue.Clear();
        listValueGateWin.Clear();
        for (int i = 0; i < (isHilo ? 32 : 29); i++)
        {
            List<ChipBetSicbo> tempList = new List<ChipBetSicbo>();
            listChipBetOnGate.Add(tempList);
            listGateTotalValue.Add(0);
            listValueGateWin.Add(0);
        }
        listGateTotalValue.Add(0);
        listValueGateWin.Add(0);
    }

    public Vector2 getPositionGateBet(int index)
    {

        //hilo
        if (index < 1 || index > listButtonBet.Count) return Vector2.zero; ;
        for (int i = 0; i < listButtonBet.Count; i++)
        {
            Button objButton = listButtonBet[i];
            if (i == index - 1)
            {
                return objButton.transform.localPosition;
            }

        }
        return Vector2.zero;
    }
    private ChipBetSicbo creatChipBet(int valueChip, int numberBet)
    {
        ChipBetSicbo chipBet;

        if (chipPool.Count == 0)
        {
            ChipBetSicbo cbs = Instantiate(chip_bet_prefab, transform.Find(numberBet + "")).GetComponent<ChipBetSicbo>();

            chipPool.Add(cbs);
        }
        chipBet = chipPool[0];
        chipPool.RemoveAt(0);
        chipBet.transform.SetParent(transform.Find(numberBet + ""));
        chipBet.transform.localScale = Vector2.one;
        chipBet.transform.SetSiblingIndex(50);
        chipBet.gameObject.SetActive(true);
        chipBet.setChip(valueChip, numberBet);
        DOTween.Kill(chipBet.transform, true);
        return chipBet;
    }

    public int setValueGateBet(int index, int value)
    {
        int valueBet = 0;
        switch (index)
        {
            case 1:
                t1 += value;
                valueBet = t1;
                break;
            case 2:
                t2 += value;
                valueBet = t2;
                break;
            case 3:
                t3 += value;
                valueBet = t3;
                break;
            case 4:
                t4 += value;
                valueBet = t4;
                break;
            case 5:
                t5 += value;
                valueBet = t5;
                break;
            case 6:
                t6 += value;
                valueBet = t6;
                break;
            case 7:
                t7 += value;
                valueBet = t7;
                break;
            case 8:
                t8 += value;
                valueBet = t8;
                break;
            case 9:
                t9 += value;
                valueBet = t9;
                break;
            case 10:
                t10 += value;
                valueBet = t10;
                break;
            case 11:
                t11 += value;
                valueBet = t11;
                break;
            case 12:
                t12 += value;
                valueBet = t12;
                break;
            case 13:
                t13 += value;
                valueBet = t13;
                break;
            case 14:
                t14 += value;
                valueBet = t14;
                break;
            case 15:
                t15 += value;
                valueBet = t15;
                break;
            case 16:
                t16 += value;
                valueBet = t16;
                break;
            case 17:
                t17 += value;
                valueBet = t17;
                break;
            case 18:
                t18 += value;
                valueBet = t18;
                break;
            case 19:
                t19 += value;
                valueBet = t19;
                break;
            case 20:
                t20 += value;
                valueBet = t20;
                break;
            case 21:
                t21 += value;
                valueBet = t21;
                break;
            case 22:
                t22 += value;
                valueBet = t22;
                break;
            case 23:
                t23 += value;
                valueBet = t23;
                break;
            case 24:
                t24 += value;
                valueBet = t24;
                break;
            case 25:
                t25 += value;
                valueBet = t25;
                break;
            case 26:
                t26 += value;
                valueBet = t26;
                break;
            case 27:
                t27 += value;
                valueBet = t27;
                break;
            case 28:
                t28 += value;
                valueBet = t28;
                break;
            case 29:
                t29 += value;
                valueBet = t29;
                break;
            case 30:
                t30 += value;
                valueBet = t30;
                break;
            case 31:
                t31 += value;
                valueBet = t31;
                break;
            case 32:
                t32 += value;
                valueBet = t32;
                break;
        }
        setValueLabelBet(index, valueBet);
        return valueBet;
    }
    private void setValueLabelBet(int numberBet, int value)
    {
        for (int i = 0; i < listLabelGateBet.Count; i++)
        {
            if (i == numberBet - 1)
            {
                TextMeshProUGUI labelBet = listLabelGateBet[i];
                labelBet.gameObject.SetActive(true);
                labelBet.text = Globals.Config.FormatMoney(value, true);
            }
        }
    }
    public void setValueChipForGate(int gateID, int value)
    {
        listGateTotalValue[gateID] += value;
        if (listChipPerGate[gateID] == null)
        {
            listChipPerGate[gateID] = creatChipBet(value, gateID);
        }

        listChipPerGate[gateID].gameObject.SetActive(true);
        listChipPerGate[gateID].gameObject.transform.localPosition = Vector2.zero;
        listChipPerGate[gateID].setChip(listGateTotalValue[gateID], gateID);
        listLabelGateBet[gateID - 1].gameObject.SetActive(true);
        listLabelGateBet[gateID - 1].text = Globals.Config.FormatMoney(listGateTotalValue[gateID], true);
    }
    public void effectWinGate(int index)
    {
        int resultWin = index - 1;
        listWinResult.Add(resultWin);

        for (int i = 0; i < listButtonBet.Count; i++)
        {
            if (i == resultWin)
            {
                Button objButton = listButtonBet[i];
                Sprite spr = objButton.spriteState.pressedSprite;
                GameObject btnPress = objButton.transform.Find("press").gameObject;
                if (btnPress != null)
                {
                    Image btnImgPress = btnPress.GetComponent<Image>();
                    btnImgPress.sprite = spr;
                    btnPress.gameObject.SetActive(true);
                    btnImgPress.enabled = true;
                    Color normalColor = btnImgPress.color;
                    Color noOpacity = new Color(1, 1, 1, 0);
                    DOTween.Sequence()
                        .Append(btnImgPress.DOColor(noOpacity, 0.1f))
                        .Append(btnImgPress.DOColor(normalColor, 0.1f))
                        .SetLoops(10)
                        .OnComplete(() =>
                        {
                            btnPress.gameObject.SetActive(false);
                            btnImgPress.enabled = false;
                        });
                }
            }
        }
    }
    public void getChipLose()
    {

        for (int i = 0; i < listChipPerGate.Count; i++)
        {
            //Globals.Logging.Log(i + "======>" + listChipPerGate[i] + "--value:" + listGateTotalValue[i] + "----listWinResult=");
            if (!listWinResult.Contains(i - 1) && listChipPerGate[i] != null && listGateTotalValue[i] != 0)  //check xem i cos tồn tại trong mảng win ko va gate do co dat tien ko;
            {
                ChipBetSicbo chipLoseEff = Instantiate(listChipPerGate[i].gameObject, listChipPerGate[i].transform.parent).GetComponent<ChipBetSicbo>();

                Vector2 pos = sicboGameView.dealer.transform.localPosition;
                Vector2 orgPos = sicboGameView.transform.TransformPoint(pos);
                Vector2 posReal = listChipPerGate[i].transform.parent.InverseTransformPoint(orgPos);
                TweenCallback cb = () =>
                {
                    Destroy(chipLoseEff.gameObject);
                };
                chipLoseEff.chipMoveTo(posReal, false, cb);
                listChipPerGate[i].gameObject.SetActive(false);

            }
        }
    }
    public void createChipWin(int gateID) // hien dong chip tong win ben canh dong chip tong cuoc cua gate do
    {
        int value = listValueGateWin[gateID];
        if (listValueGateWin[gateID] > 0)
        {
            if (listChipWinPerGate[gateID] == null)
            {
                listChipWinPerGate[gateID] = creatChipBet(value, gateID);
            }
            else
            {

            }
            listChipWinPerGate[gateID].setChip(value, gateID);
            Vector2 posGate = getPositionGateBet(gateID);
            posGate = new Vector2(posGate.x + (posGate.x > 0 ? -35 : 35), posGate.y);
            listChipWinPerGate[gateID].transform.localPosition = listChipWinPerGate[gateID].transform.parent.InverseTransformPoint(transform.TransformPoint(posGate));
            listChipWinPerGate[gateID].gameObject.SetActive(true);
        }

    }
    public int setValueGateWin(int index, int value)
    {
        int valueWin = 0;

        for (int i = 0; i < listValueGateWin.Count; i++)
        {
            if (index == i)
            {
                listValueGateWin[i] += value;
                valueWin = listValueGateWin[i];
            }
        }
        return valueWin;
    }
    public void creatDataChipWin()
    {
        dataWin.Clear();
        for (int i = 0; i < listValueGateWin.Count; i++)
        {
            if (listValueGateWin[i] > 0)
            {
                initData(i, listValueGateWin[i]);
            }
        }
    }
    private void initData(int numberWin, int value)
    {
        JObject data = new JObject();
        data["numberWin"] = numberWin;
        data["value"] = value;
        dataWin.Add(data);
    }
}