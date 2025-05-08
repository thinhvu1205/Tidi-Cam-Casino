using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
public class BoxbetSicbo : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public List<TextMeshProUGUI> listLabelBet;
    [SerializeField] public bool isHilo = false;
    [SerializeField] private HiloView gameView;
    [HideInInspector] public long totalBoxBet = 0;
    private List<long> listB = new List<long>();
    private List<long> listT = new List<long>();
    public JArray dataBet = new JArray();
    public JArray dataBetOld = new JArray();
    public JArray dataClickBet = new JArray();
    public long totalBet = 0;

    private long t1 = 0, t2 = 0, t3 = 0, t4 = 0, t5 = 0, t6 = 0, t7 = 0, t8 = 0, t9 = 0, t10 = 0,
        t11 = 0, t12 = 0, t13 = 0, t14 = 0, t15 = 0, t16 = 0, t17 = 0, t18 = 0, t19 = 0, t20 = 0,
        t21 = 0, t22 = 0, t23 = 0, t24 = 0, t25 = 0, t26 = 0, t27 = 0, t28 = 0, t29 = 0;
    void Start()
    {

    }
    private void Awake()
    {
        gameView = SicboView.instance;
        totalBoxBet = 0;
        for (int i = 1; i < 30; i++)
        {
            listB.Add(0);
            listT.Add(0);
        }
    }
    private void setValueLabel()
    {
        totalBoxBet = 0;
        for (int i = 0; i < dataClickBet.Count; i++)
        {
            JObject dataClick = (JObject)dataClickBet[i];
            totalBoxBet += (long)Mathf.Min((int)dataClick["value"], gameView.thisPlayer.ag - totalBoxBet);
            TextMeshProUGUI labelBet = listLabelBet[(int)dataClick["numberBet"] - 1];
            labelBet.gameObject.SetActive(true);
            labelBet.text = Globals.Config.FormatMoney2((int)dataClick["value"], true);
            for (int j = 0; j < dataBetOld.Count; j++)
            {
                JObject dataBet = (JObject)dataBetOld[j];
                if ((int)dataClick["numberBet"] == (int)dataBet["numberBet"])
                {
                    labelBet.text = Globals.Config.FormatMoney2((int)dataClick["value"] + (int)dataBet["value"], true);
                }
            }
        }
        gameView.buttonBet.setStateButtonChip();
        if (gameView.buttonBet.label_totalBet)
        {
            //gameView.buttonBet.label_clear.text = Globals.Config.FormatMoney2(totalBoxBet, true);
            gameView.buttonBet.label_totalBet.text = Globals.Config.FormatMoney2(totalBoxBet, true);
        }
    }
    public void onClickClearBet()
    {
        resetDataClickBet();
        gameView.buttonBet.setStateButtonChip();
        gameView.buttonBet.onClickChip(gameView.chipDealLastMatch.ToString());
    }


    public void onBet(int index, int value)
    {
        int numberBet = index - 1;
        listT[numberBet] += value;
        //long valueBet = listT[numberBet];
        long valueBet = 0;
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
        }
        setValueOnBet(numberBet, valueBet);
    }
    private void setValueOnBet(int numberBet, long value)
    {
        for (int i = 0; i < listLabelBet.Count; i++)
        {
            if (i == numberBet)
            {
                TextMeshProUGUI labelBet = listLabelBet[i];
                labelBet.gameObject.SetActive(true);
                labelBet.text = Globals.Config.FormatMoney(value, true);
            }
        }
    }
    public void resetBoxBet()
    {
        resetDataClickBet();
        dataBetOld.Clear();
        t1 = 0; t2 = 0; t3 = 0; t4 = 0; t5 = 0; t6 = 0; t7 = 0; t8 = 0; t9 = 0; t10 = 0;
        t11 = 0; t12 = 0; t13 = 0; t14 = 0; t15 = 0; t16 = 0; t17 = 0; t18 = 0; t19 = 0; t20 = 0;
        t21 = 0; t22 = 0; t23 = 0; t24 = 0; t25 = 0; t26 = 0; t27 = 0; t28 = 0; t29 = 0;
        for (int i = 0, l = listT.Count; i < l; i++)
        {
            listT[i] = 0;
            listB[i] = 0;
        }
        for (int i = 0; i < listLabelBet.Count; i++)
        {
            TextMeshProUGUI labelBet = listLabelBet[i];
            labelBet.gameObject.SetActive(false);
            labelBet.text = "0";
        }
    }
    public void creatDataBet()
    {
        dataBet = new JArray();
        dataBetOld = new JArray();
        if (t1 > 0) initData(1, t1);
        if (t2 > 0) initData(2, t2);
        if (t3 > 0) initData(3, t3);
        if (t4 > 0) initData(4, t4);
        if (t5 > 0) initData(5, t5);
        if (t6 > 0) initData(6, t6);
        if (t7 > 0) initData(7, t7);
        if (t8 > 0) initData(8, t8);
        if (t9 > 0) initData(9, t9);
        if (t10 > 0) initData(10, t10);
        if (t11 > 0) initData(11, t11);
        if (t12 > 0) initData(12, t12);
        if (t13 > 0) initData(13, t13);
        if (t14 > 0) initData(14, t14);
        if (t15 > 0) initData(15, t15);
        if (t16 > 0) initData(16, t16);
        if (t17 > 0) initData(17, t17);
        if (t18 > 0) initData(18, t18);
        if (t19 > 0) initData(19, t19);
        if (t20 > 0) initData(20, t20);
        if (t21 > 0) initData(21, t21);
        if (t22 > 0) initData(22, t22);
        if (t23 > 0) initData(23, t23);
        if (t24 > 0) initData(24, t24);
        if (t25 > 0) initData(25, t25);
        if (t26 > 0) initData(26, t26);
        if (t27 > 0) initData(27, t27);
        if (t28 > 0) initData(28, t28);
        if (t29 > 0) initData(29, t29);
        //for (int i = 0, l = listT.Count; i < l; i++)
        //{
        //    if (listT[i] > 0)
        //    {
        //        initData(i+1, listT[i]);
        //    }

        //}
    }
    private void initData(int numberBet, long value)
    {
        JObject data = new JObject();
        data["numberBet"] = numberBet;
        data["value"] = value;
        dataBet.Add(data);
        dataBetOld.Add(data);
    }
    public void onClickBet(int index, long value)
    {
        if (!isHilo) //sicbo th??ng
        {
            listB[index] += (long)Mathf.Min(value, gameView.thisPlayer.ag - totalBoxBet);
            Debug.Log("onClickBet listB[index]=" + listB[index]);
        }
        else //hilo thai
        {
            listB[index] += value;
        }
        creatDataClickBet();
        gameView.buttonBet.setStateButtonChip();
    }
    private void creatDataClickBet()
    {
        dataClickBet.Clear();
        for (int i = 0; i < listB.Count; i++)
        {
            //Debug.Log("listB[i]=" + listB[i]);
            if (listB[i] > 0)
            {
                initDataBetSendServer(i + 1, listB[i]);
                //Debug.Log("listB[i]=" + listB[i]);
            }
        }
        //Debug.Log("creatDataClickBet:" + dataClickBet.Count);
        setValueLabel();
    }
    public void initDataBetSendServer(int numberBet, long value)
    {
        JObject data = new JObject();
        data["numberBet"] = numberBet;
        data["value"] = value;
        dataClickBet.Add(data);
    }
    public void resetDataClickBet()
    {
        totalBoxBet = 0;
        gameView.buttonBet.label_totalBet.text = totalBoxBet.ToString();
        //b1 = 0; b2 = 0; b3 = 0; b4 = 0; b5 = 0; b6 = 0; b7 = 0; b8 = 0; b9 = 0; b10 = 0;
        //b11 = 0; b12 = 0; b13 = 0; b14 = 0; b15 = 0; b16 = 0; b17 = 0; b18 = 0; b19 = 0; b20 = 0;
        //b21 = 0; b22 = 0; b23 = 0; b24 = 0; b25 = 0; b26 = 0; b27 = 0; b28 = 0; b29 = 0; b30 = 0; b31 = 0; b32 = 0;
        for (int i = 0; i < listB.Count; i++)
        {
            listB[i] = 0;
        }
        creatDataClickBet();
        for (int i = 0; i < listLabelBet.Count; i++)
        {
            TextMeshProUGUI labelBet = listLabelBet[i];
            labelBet.gameObject.SetActive(false);
            labelBet.text = "0";
        }
        for (int i = 0; i < dataBetOld.Count; i++)
        {
            JObject dataBet = (JObject)dataBetOld[i];
            TextMeshProUGUI labelBet = listLabelBet[(int)dataBet["numberBet"] - 1];
            labelBet.gameObject.SetActive(true);
            labelBet.text = Globals.Config.FormatMoney2((int)dataBet["value"], true);
        }
    }


}