using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using Globals;

public class ExchangeView : BaseView
{
    public static ExchangeView instance;
    [SerializeField] List<Sprite> spTab;
    [SerializeField] GameObject tabTop, itemEx, itemAgency, itemHistory;
    [SerializeField] Transform m_PrefabHistoryTf, m_HistoryTf;
    [SerializeField] TextMeshProUGUI lbChips, m_RewardTMP, m_HistoryTMP;
    [SerializeField] BaseView popupInput;
    [SerializeField] ScrollRect scrContentRedeem, scrContentAgency, scrContentHistory, scrTabs, scrTabsHis;
    [SerializeField] private InputField m_PhoneIF, m_ConfirmPhoneIF;

    private List<JObject> listDataHis = new List<JObject>();
    private JObject firstTabHistItem, curDataTabNap;
    private JArray dataCO;
    private string typeTabHistory = "";
    private int indexTabHis = 0, indexTabNap = 1;
    private float _contentHeight = 0;

    #region Button
    public void onConfirmCashOut()
    {
        SoundManager.instance.soundClick();
        //require('SMLSocketIO').getInstance().emitSIOCCC(cc.js.formatStr("onConfirmCashOut_%s", require('GameManager').getInstance().getCurrentSceneName()));
        var value = valueCO;
        var typeName = typeNet;
        var phoneNumber = m_PhoneIF.text;
        var phoneNumberRetype = m_ConfirmPhoneIF.text;

        if (phoneNumber.Equals("") || phoneNumberRetype.Equals(""))
            UIManager.instance.showMessageBox(Globals.Config.formatStr(Globals.Config.getTextConfig("txt_notEmty"), typeNet.Equals("Mobile") ? Globals.Config.getTextConfig("txt_phone_numnber") : (string)rewardData["TypeName"], ""));
        else if (!phoneNumber.Equals(phoneNumberRetype))
            UIManager.instance.showMessageBox(Globals.Config.formatStr(Globals.Config.getTextConfig("txt_notSame"), typeNet.Equals("Mobile") ? Globals.Config.getTextConfig("txt_phone_numnber") : (string)rewardData["TypeName"]));
        else
        {
            m_PhoneIF.text = "";
            m_ConfirmPhoneIF.text = "";
            SocketSend.sendCashOut(value, phoneNumber, typeName);
            UIManager.instance.showWaiting();
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        instance = this;
        SocketSend.SendGiftsHistory();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SocketIOManager.getInstance().emitSIOCCCNew(Globals.Config.formatStr("ClickShowExchange_%s", Globals.CURRENT_VIEW.getCurrentSceneName()));
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.DT_VIEW);
        Debug.Log("-==infoDT  " + Globals.Config.infoDT);
        LoadConfig.instance.getInfoEX(updateInfo);
        lbChips.text = Globals.Config.FormatNumber(Globals.User.userMain.AG);
    }
    public async void HandleGiftHistory(JObject data)
    {
        JArray content = (JArray)data["content"];
        foreach (Transform tf in m_HistoryTf) Destroy(tf.gameObject);
        for (int i = 0; i < content.Count; i++)
        {
            Transform tf = Instantiate(m_PrefabHistoryTf, m_HistoryTf);
            tf.gameObject.SetActive(true);
            tf.GetChild(0).GetComponent<TextMeshProUGUI>().text = DateTimeOffset.FromUnixTimeMilliseconds((long)content[i]["time"]).DateTime.ToString("dd/MM/yyyy hh:mm:ss tt");
            tf.GetChild(1).GetComponent<TextMeshProUGUI>().text = (string)content[i]["content"];

        }
        await ScrollHistory();
        async Awaitable ScrollHistory()
        {
            try
            {
                await Awaitable.NextFrameAsync();
                await Awaitable.NextFrameAsync();
                _contentHeight = m_HistoryTf.GetComponent<RectTransform>().rect.height;
                float viewportheight = m_HistoryTf.parent.GetComponent<RectTransform>().rect.height;
                while (true)
                {
                    if (m_HistoryTf.localPosition.y > (_contentHeight - viewportheight)) m_HistoryTf.localPosition = Vector3.zero;
                    await Awaitable.FixedUpdateAsync();
                    m_HistoryTf.localPosition += Time.fixedDeltaTime * new Vector3(0, 100, 0);
                }
            }
            catch
            {

            }
        }
    }
    public void HandleUpdateHistory(JObject data)
    {
        Transform tf = Instantiate(m_PrefabHistoryTf, m_HistoryTf);
        tf.gameObject.SetActive(true);
        tf.GetChild(0).GetComponent<TextMeshProUGUI>().text = DateTimeOffset.FromUnixTimeMilliseconds((long)data["time"]).DateTime.ToString();
        tf.GetChild(1).GetComponent<TextMeshProUGUI>().text = (string)data["content"];
        _contentHeight += tf.GetComponent<RectTransform>().rect.height;

    }
    public void UpdateAg()
    {
        lbChips.text = Globals.Config.FormatNumber(Globals.User.userMain.AG);
    }
    void updateInfo(string strData)
    {
        Globals.Logging.Log("updateInfo EX   " + strData);
        //[{ "title":"Truemoney","type":"phil","child":[{ "title":"truemoney","TypeName":"truemoney","title_img":"https://cdn.topbangkokclub.com/api/public/dl/VbfRjo1c/co/Truemoney.png","textBox":[{ "key_placeHolder":"txt_enter_text_gc"},{ "key_placeHolder":"txt_conf_text_gc"}]}],"items":[{ "ag":1000000,"m":50},{ "ag":2000000,"m":100},{ "ag":4000000,"m":200},{ "ag":10000000,"m":500},{ "ag":20000000,"m":1000},{ "ag":40000000,"m":2000},{ "ag":100000000,"m":5000},{ "ag":200000000,"m":10000}]}]
        dataCO = JArray.Parse(strData);

        SetDataButtons();
    }

    async void SetDataButtons()
    {
        if (dataCO.Count <= 0) return;
        if (dataCO.Count <= 0) return;
        JObject objData = (JObject)dataCO[0];
        m_RewardTMP.text = ((string)objData["title"]).ToUpper();
        GameObject go = m_RewardTMP.transform.parent.gameObject;
        go.GetComponent<Button>().onClick.AddListener(() => DoClickButton(go, objData));
        if (!((string)objData["type"]).Equals("agency"))
        {
            m_HistoryTMP.text = Globals.Config.getTextConfig("history").ToUpper();
            GameObject historyObj = m_HistoryTMP.transform.parent.gameObject;
            historyObj.GetComponent<Button>().onClick.AddListener(() => DoClickButton(historyObj, null));
        }
        if (((string)objData["title"]).Equals("reward")) await genTabTop((JArray)objData["child"]);
        DoClickButton(go, objData);
    }

    async Task genTabTop(JArray arrayData)
    {
        scrTabs.enabled = arrayData.Count > 4;
        JObject item0 = null;
        var indSelect = 0;
        for (var i = 0; i < arrayData.Count; i++)
        {
            JObject obItem = (JObject)arrayData[i];

            if (i == 0) { item0 = obItem; indSelect = i; }
            Globals.Logging.Log(obItem);
            string title = (string)obItem["TypeName"];
            string title_img = (string)obItem["title_img"];

            GameObject btn = Instantiate(tabTop, scrTabs.content);

            var bkg = btn.transform.Find("Bkg").GetComponent<Image>();
            bkg.sprite = spTab[(i == 0 || i >= arrayData.Count - 1) ? 0 : 1];
            if (i >= arrayData.Count - 1)
            {
                bkg.transform.localScale = new Vector3(-1, 1, 1);
                btn.transform.Find("Line").gameObject.SetActive(false);
            }
            var txt = btn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            txt.text = "";

            var spLogo = btn.transform.Find("Icon").GetComponent<Image>();
            spLogo.gameObject.SetActive(false);
            if (title_img.Equals(""))
            {
                txt.text = title.ToUpper();
            }
            else
            {
                Sprite spr = await Globals.Config.GetRemoteSprite(title_img);
                if (spr != null)
                {
                    spLogo.sprite = spr;
                    if (spLogo != null && spLogo.sprite != null)
                    {
                        spLogo.gameObject.SetActive(true);
                        spLogo.SetNativeSize();
                    }
                    else
                    {
                        txt.text = title.ToUpper();
                    }
                }

            }
            btn.transform.localScale = Vector3.one;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickTab(btn.gameObject, obItem);
            });

        }

        if (item0 == null && arrayData.Count > 0)
        {
            indSelect = 0;
            item0 = (JObject)arrayData[0];
        }
        if (scrTabs.content.childCount > indSelect)
        {
            Globals.Logging.Log("item   " + item0.ToString());
            onClickTab(scrTabs.content.GetChild(indSelect).gameObject, item0);
            curDataTabNap = item0;
        }
        genTabHis(arrayData);
    }
    private async void genTabHis(JArray arrayData)
    {
        scrTabsHis.enabled = arrayData.Count > 4;
        JObject item0 = null;
        indexTabHis = 0;
        for (var i = 0; i < arrayData.Count; i++)
        {
            JObject obItem = (JObject)arrayData[i];

            if (i == 0) { item0 = obItem; indexTabHis = i; }
            Globals.Logging.Log(obItem);
            string title = (string)obItem["TypeName"];
            string title_img = (string)obItem["title_img"];

            GameObject btn = Instantiate(tabTop, scrTabsHis.content);


            var bkg = btn.transform.Find("Bkg").GetComponent<Image>();
            bkg.sprite = spTab[(i == 0 || i >= arrayData.Count - 1) ? 0 : 1];
            if (i >= arrayData.Count - 1)
            {
                bkg.transform.localScale = new Vector3(-1, 1, 1);
                btn.transform.Find("Line").gameObject.SetActive(false);
            }
            var txt = btn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            txt.text = "";

            var spLogo = btn.transform.Find("Icon").GetComponent<Image>();
            spLogo.gameObject.SetActive(false);
            if (title_img.Equals(""))
            {
                txt.text = title.ToUpper();
            }
            else
            {
                Sprite spr = await Globals.Config.GetRemoteSprite(title_img);
                if (spr != null)
                {
                    spLogo.sprite = spr;
                    if (spLogo != null && spLogo.sprite != null)
                    {
                        spLogo.gameObject.SetActive(true);
                        spLogo.SetNativeSize();
                    }
                    else
                    {
                        txt.text = title.ToUpper();
                    }
                }

            }
            btn.transform.localScale = Vector3.one;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickTabHis(btn.gameObject, obItem);
            });

            if (typeTabHistory == (string)obItem["TypeName"])
            {
                firstTabHistItem = obItem;
                indexTabHis = i;
            }
        }
    }
    void onClickTabHis(GameObject evv, JObject dataItem)
    {
        SoundManager.instance.soundClick();
        for (var i = 0; i < scrTabsHis.content.childCount; i++)
        {
            var bkg = scrTabsHis.content.GetChild(i).transform.Find("Bkg");
            bkg.gameObject.SetActive(evv == scrTabsHis.content.GetChild(i).gameObject);
            if (evv == scrTabsHis.content.GetChild(i).gameObject)
            {
                indexTabNap = i;
            }
        }
        if (dataItem["TypeName"] != null) typeTabHistory = (string)dataItem["TypeName"];
        else
        {
            JArray tabNamesJA = (JArray)dataItem["child"];
            typeTabHistory = (string)tabNamesJA[indexTabNap]["TypeName"];
        }
        curDataTabNap = dataItem;
        if (listDataHis.Count > 0)
        {
            reloadListItemHistory(listDataHis);
        }
    }

    JObject rewardData = null;
    void onClickTab(GameObject evv, JObject dataItem)
    {
        SoundManager.instance.soundClick();
        rewardData = dataItem;
        for (var i = 0; i < scrTabs.content.childCount; i++)
        {
            var bkg = scrTabs.content.GetChild(i).transform.Find("Bkg");
            bkg.gameObject.SetActive(evv == scrTabs.content.GetChild(i).gameObject);
            if (evv == scrTabs.content.GetChild(i).gameObject)
            {
                indexTabHis = i;
                indexTabNap = i;
            }
        }
        typeTabHistory = (string)dataItem["TypeName"];
        firstTabHistItem = dataItem;
        reloadListItem(rewardData);
    }

    void DoClickButton(GameObject obj, JObject objDataItem)
    {
        SoundManager.instance.soundClick();
        GameObject rewardGo = m_RewardTMP.transform.parent.gameObject;
        GameObject historyGo = m_HistoryTMP.transform.parent.gameObject;
        rewardGo.SetActive(obj != rewardGo);
        historyGo.SetActive(obj != historyGo);
        if (objDataItem == null && obj == historyGo)
        {
            scrContentRedeem.transform.parent.gameObject.SetActive(false);
            scrContentAgency.transform.parent.gameObject.SetActive(false);
            scrContentHistory.transform.parent.gameObject.SetActive(true);
            onClickTabHis(scrTabsHis.content.GetChild(indexTabHis).gameObject, firstTabHistItem);
            SocketSend.sendDTHistory();
        }
        else if (((string)objDataItem["type"]).Equals("agency"))
        {
            typeNet = (string)objDataItem["type"];
            scrContentRedeem.transform.parent.gameObject.SetActive(false);
            scrContentAgency.transform.parent.gameObject.SetActive(true);
            scrContentHistory.transform.parent.gameObject.SetActive(false);
            reloadListItem(objDataItem);
        }
        else
        {
            typeNet = (string)curDataTabNap["TypeName"];
            scrContentRedeem.transform.parent.gameObject.SetActive(true);
            scrContentAgency.transform.parent.gameObject.SetActive(false);
            scrContentHistory.transform.parent.gameObject.SetActive(false);
            if (indexTabNap != -1) onClickTab(scrTabs.content.GetChild(indexTabNap).gameObject, objDataItem);
        }
    }

    void reloadListItem(JObject objDataItem)
    {
        if (objDataItem != null)
        {
            //[{ "title":"Truemoney","type":"phil","child":[{ "title":"truemoney","TypeName":"truemoney","title_img":"https://storage.googleapis.com/cdn.topbangkokclub.com/shop/Truemoney.png?v=1","textBox":[{ "key_placeHolder":"txt_enter_text_gc"},{ "key_placeHolder":"txt_conf_text_gc"}]}],"items":[{ "ag":1000000,"m":50},{ "ag":2000000,"m":100},{ "ag":4000000,"m":200},{ "ag":10000000,"m":500},{ "ag":20000000,"m":1000},{ "ag":40000000,"m":2000},{ "ag":100000000,"m":5000},{ "ag":200000000,"m":10000}]},{ "type":"agency","title":"agency","items":[{ "id":"1862315","name":"Agency Jason","tel":"09396196724","msg_fb":"http://bit.ly/jason-agency"}]}]
            JArray items = new JArray(); ;
            Transform parent;
            Globals.Logging.Log("type  " + objDataItem["typeName"]);
            Debug.Log("-=-= " + objDataItem.ToString());
            if (objDataItem["TypeName"] != null) typeNet = (string)objDataItem["TypeName"];
            else
            {
                JArray tabNamesJA = (JArray)objDataItem["child"];
                typeNet = (string)tabNamesJA[indexTabNap]["TypeName"];
            }
            bool isAgency = objDataItem.ContainsKey("type") && ((string)objDataItem["type"]).Equals("agency");
            items = (JArray)objDataItem["items"];
            parent = isAgency ? scrContentAgency.content : scrContentRedeem.content;
            if (items == null || items.Count <= 0) return;
            Debug.Log("-=-= itemss  " + items.ToString());

            for (var i = 0; i < items.Count; i++)
            {
                JObject dt = (JObject)items[i];
                GameObject item = i < parent.childCount ? parent.GetChild(i).gameObject : Instantiate(isAgency ? itemAgency : itemEx, parent);
                if (isAgency) item.GetComponent<ItemAgency>().setInfo(dt);
                else item.GetComponent<ItemEx>().setInfo(dt, () => onChooseCashOut((int)dt["ag"], (int)dt["m"]));
                item.SetActive(true);
                item.transform.SetParent(parent);
                item.transform.localScale = Vector3.one;
            }
            for (var i = items.Count; i < parent.childCount; i++) parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void reloadListItemHistory(List<JObject> listItem)
    {
        listDataHis = listItem;
        for (int i = 0; i < scrContentHistory.content.childCount; i++)
        {
            scrContentHistory.content.GetChild(i).gameObject.SetActive(false);
        }
        for (var i = 0; i < listDataHis.Count; i++)
        {
            string typeNameItem = (string)listDataHis[i]["typeName"];
            if (typeNameItem.Equals(typeTabHistory))
            {
                GameObject objItem;
                if (i < scrContentHistory.content.childCount)
                {
                    objItem = scrContentHistory.content.GetChild(i).gameObject;
                }
                else
                {
                    objItem = Instantiate(itemHistory, scrContentHistory.content);

                }
                objItem.SetActive(true);
                objItem.transform.SetParent(scrContentHistory.content);
                objItem.transform.localScale = Vector3.one;

                objItem.GetComponent<ItemHistoryEx>().setInfo(listDataHis[i], (int)listDataHis[i]["CashValue"]);
            }
        }
    }

    int valueCO;
    string typeNet;
    void onChooseCashOut(int ag, int value)
    {
        SoundManager.instance.soundClick();
        Debug.Log("typenet ==" + typeNet);
        Debug.Log("Current Tab=" + indexTabNap);
        if (Globals.User.userMain.AG < ag)
        {
            UIManager.instance.showMessageBox(Globals.Config.getTextConfig("txt_koduchip"));
        }
        else
        {
            popupInput.show();
            if (rewardData != null)
            {
                JArray textBox = null;
                if (rewardData["textBox"] != null) textBox = (JArray)rewardData["textBox"];
                else textBox = (JArray)rewardData["child"][indexTabNap]["textBox"];
                m_PhoneIF.placeholder.GetComponent<Text>().text = Config.getTextConfig((string)textBox[0]["key_placeHolder"]);
                m_ConfirmPhoneIF.placeholder.GetComponent<Text>().text = Config.getTextConfig((string)textBox[1]["key_placeHolder"]);
            }
        }

        valueCO = value;
    }
    public void cashOutReturn(JObject data)
    {
        Globals.Logging.Log("-=-=-=-=cashOutReturn  " + data.ToString());
        UIManager.instance.showMessageBox((string)data["data"]);
        if ((bool)data["status"])
        {
            m_PhoneIF.text = "";
            m_ConfirmPhoneIF.text = "";
            SocketSend.sendUAG();
            popupInput.hide(false);
            DoClickButton(m_HistoryTMP.transform.parent.gameObject, null);

        }
    }
}
