using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Globals;
public class SendGiftView : BaseView
{

    public static SendGiftView instance;
    [SerializeField]
    GameObject tabSendGift;

    [SerializeField]
    GameObject tabHistory;

    [SerializeField]
    TMP_InputField edbChip;

    [SerializeField]
    public TMP_InputField edbID;

    [SerializeField]
    TextMeshProUGUI lbFee, lbCurrentChip;

    [SerializeField]
    ScrollRect scrHistory;

    [SerializeField]
    Toggle btnTabSendGift;

    [SerializeField]
    Toggle btnTabHistory;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        SocketIOManager.getInstance().emitSIOCCCNew(Globals.Config.formatStr("ClickShowGift_%s", Globals.CURRENT_VIEW.getCurrentSceneName()));
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.SEND_GIFT_VIEW);
        SendGiftView.instance = this;
    }
    protected override void Start()
    {
        base.Start();
        setInfo();
    }
    public void updateChip()
    {
        lbCurrentChip.text = Globals.Config.FormatNumber(Globals.User.userMain.AG);
    }
    // Update is called once per frame
    private void setInfo()
    {
        updateChip();
        lbFee.text = Globals.Config.getTextConfig("fee") + Globals.Config.ketPhe + "%";
    }

    public void onClickSend()
    {
        string friendID = edbID.text;
        int chipSend = Globals.Config.splitToInt(edbChip.text);
        if (friendID == "" || chipSend == 0 || edbChip.text == "")
        {
            UIManager.instance.showToast(Globals.Config.getTextConfig("error_empty"));
            return;
        }
        UIManager.instance.showWaiting();
        SocketSend.sendGift(int.Parse(friendID), chipSend);
    }
    public void onClickTabSendGift(bool isOn)
    {
        if (tabSendGift.activeSelf == false)
        {
            tabSendGift.SetActive(true);
            tabHistory.SetActive(false);
        }
    }
    public void onClickTabHistory(bool isOn)
    {
        if (tabHistory.activeSelf == false)
        {
            tabSendGift.SetActive(false);
            tabHistory.SetActive(true);
            UIManager.instance.showWaiting();
            SocketSend.getHistorySafe();
        }

    }
    public void onSuccessSendGift()
    {
        edbChip.text = "";

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
            int chipChange = (int)data["chipchange"];
            item.SetActive(true);
            item.transform.Find("lbTimes").GetComponent<TextMeshProUGUI>().text = timeDay + "\n" + timeHour;
            var msgg = (string)data["msg"];
            Globals.Logging.Log("-=-= msg  " + msgg);
            item.transform.Find("lbContent").GetComponent<TextMeshProUGUI>().text = msgg.Length > 20 ? msgg.Substring(0, 18) + "..." : msgg;
            item.transform.Find("lbChips").GetComponent<TextMeshProUGUI>().text = chipChange > 0 ? ("+" + Globals.Config.FormatNumber(chipChange)) : Globals.Config.FormatNumber(chipChange);
            item.transform.Find("lbChips").GetComponent<TextMeshProUGUI>().color = chipChange > 0 ? Color.green : Color.red;
        }
        for (int i = size; i < scrHistory.content.childCount; i++)
        {
            scrHistory.content.GetChild(i).gameObject.SetActive(false);
        }

    }
    public void onEdbChange()
    {
        string textNumber = edbChip.text;
        if (textNumber.Equals(""))
        {
            textNumber = "0";
        }

        var Number = Globals.Config.splitToLong(textNumber);

        if (Globals.User.userMain.AG < Number)
        {
            Number = Globals.User.userMain.AG;
        }

        edbChip.text = Globals.Config.FormatNumber(Number);
    }
}
