using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Globals;
public class SafeView : BaseView
{
    // Start is called before the first frame update
    public static SafeView instance;
    [SerializeField]
    TextMeshProUGUI lbCurrentChips, lbCurrentSafe, lbTypeTrans;

    [SerializeField]
    GameObject InfoView;
    [SerializeField]
    GameObject transactionView;

    [SerializeField]
    GameObject historyView;

    [SerializeField]
    GameObject commonView;

    [SerializeField]
    GameObject buttonContainer;


    [SerializeField]
    TMP_InputField edbChips;

    [SerializeField]
    ScrollRect scrHistory;

    private int typeTrans = -1;//0:push-to-safe,1:withdraw

    protected override void Awake()
    {
        base.Awake();
        SafeView.instance = this;

        SocketIOManager.getInstance().emitSIOCCCNew(Globals.Config.formatStr("ClickShowSafe_%s", Globals.CURRENT_VIEW.getCurrentSceneName()));
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.KET_VIEW);
    }
    protected override void Start()
    {
        base.Start();
        setInfo();
        typeTrans = -1;

    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void setInfo()
    {
        lbCurrentChips.text = Globals.Config.FormatNumber(Globals.User.userMain.AG);
        lbCurrentSafe.text = Globals.Config.FormatNumber(Globals.User.userMain.agSafe);
        if (typeTrans == 0)
        {
            //UIManager.instance.showToast("Push to safe successfully!");
            edbChips.text = "";
        }
        if (typeTrans == 1)
        {
            //UIManager.instance.showToast("Withdraw successfully!");
            edbChips.text = "";
        }
        if (typeTrans == -1)
        {
            buttonContainer.SetActive(true);
            transactionView.SetActive(false);
            commonView.SetActive(true);
            historyView.SetActive(false);
            commonView.SetActive(true);
        }
    }
    public void onClickPushToSafe()
    {
        SoundManager.instance.soundClick();
        typeTrans = 0;
        edbChips.text = "";
        transactionView.SetActive(true);
        commonView.SetActive(false);
        buttonContainer.SetActive(false);
        lbTypeTrans.text = Globals.Config.getTextConfig("push_to_safe");
        //edbChips.GetComponent<EdbController>().isCheckWithAgSafe = false;
        //edbChips.GetComponent<EdbController>().isCheckWithAg = true;
        edbChips.GetComponent<EdbController>().SetCheckNumber(true);
    }
    public void onClickWithDraw()
    {
        SoundManager.instance.soundClick();
        typeTrans = 1;
        edbChips.text = "";
        transactionView.SetActive(true);
        buttonContainer.SetActive(false);
        commonView.SetActive(false);
        lbTypeTrans.text = Globals.Config.getTextConfig("withdraw");
        //edbChips.GetComponent<EdbController>().isCheckWithAgSafe = true;
        edbChips.GetComponent<EdbController>().SetCheckNumber(false);
    }
    public void onClickSendGift()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openSendGift();
    }
    public void onClickHistory()
    {
        SoundManager.instance.soundClick();
        InfoView.SetActive(false);
        historyView.SetActive(true);
        commonView.SetActive(false);
        UIManager.instance.showWaiting();
        SocketSend.getHistorySafe();
    }
    public void onClickBackView()
    {
        SoundManager.instance.soundClick();
        if (typeTrans != -1)
        {
            buttonContainer.SetActive(true);
            transactionView.SetActive(false);
            commonView.SetActive(true);
            typeTrans = -1;
            InfoView.SetActive(true);
        }
        else
        {
            historyView.SetActive(false);
            commonView.SetActive(true);
            InfoView.SetActive(true);
        }
    }

    public void onClickConfirmTrans()
    {

        SoundManager.instance.soundClick();
        if (edbChips.text.Equals("") || edbChips.text.Equals("0"))
        {
            UIManager.instance.showToast(Globals.Config.getTextConfig("error_empty"));
            return;
        }
        long chips = edbChips.gameObject.GetComponent<EdbController>().getLong();

        if (typeTrans == 0)
        {
            SocketSend.sendToSafe(chips);
        }
        else
        {
            if (chips > Globals.User.userMain.agSafe)
            {
                UIManager.instance.showToast(Globals.Config.getTextConfig("msg_warrning_send"));
                return;
            }
            SocketSend.sendWithDraw(chips);
        }
        edbChips.text = "";
    }
    public void reloadHistory(JArray jsonData)
    {
        int size = jsonData.Count;
        for (int i = 0; i < size; i++)
        {
            JObject data = (JObject)jsonData[i];
            GameObject item;
            if (i < scrHistory.content.childCount)
            {
                item = scrHistory.content.GetChild(i).gameObject;

            }
            else
            {
                item = Instantiate(scrHistory.content.GetChild(0), scrHistory.content).gameObject;
                item.transform.localScale = Vector3.one;

            }
            string timeDay = (string)data["timeday"];
            string timeHour = (string)data["timehour"];
            long chipChange = (long)data["chipchange"];
            item.SetActive(true);
            item.transform.Find("lbTimes").GetComponent<TextMeshProUGUI>().text = timeDay + "\n" + timeHour;
            //item.transform.Find("lbContent").GetComponent<TextMeshProUGUI>().text = (string)data["msg"];
            var msgg = (string)data["msg"];
            item.transform.Find("lbContent").GetComponent<TextMeshProUGUI>().text = msgg.Length > 20 ? msgg.Substring(0, 18) + "..." : msgg;
            item.transform.Find("lbChips").GetComponent<TextMeshProUGUI>().text = chipChange > 0 ? ("+" + Globals.Config.FormatMoney(chipChange)) : Globals.Config.FormatNumber(chipChange).ToString();
            item.transform.Find("lbChips").GetComponent<TextMeshProUGUI>().color = chipChange > 0 ? Color.green : Color.red;

        }
        for (int i = size; i < scrHistory.content.childCount; i++)
        {
            scrHistory.content.GetChild(i).gameObject.SetActive(false);
        }
    }


}
