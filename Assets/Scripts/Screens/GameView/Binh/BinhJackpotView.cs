using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Newtonsoft.Json.Linq;
using Globals;

public class BinhJackpotView : BaseView
{
    public static BinhJackpotView instance = null;
    [SerializeField]
    public ScrollRect scrRuleJackPot;
    [SerializeField]
    public GameObject itemRule;
    [SerializeField]
    public ScrollRect scrHistoryJackPot;
    [SerializeField]
    public GameObject itemHistory;
    [SerializeField]
    public GameObject tabRule;
    [SerializeField]
    public GameObject tabHistory;
    [SerializeField]
    public GameObject btnRule;
    [SerializeField]
    public GameObject btnHis;
    [SerializeField]
    public Image imgRuleDemo;
    [SerializeField]
    public TextMeshProUGUI lbTextRule;
    [SerializeField]
    public TextMeshProUGUI lbExample;
    [SerializeField]
    public List<Sprite> listSpriteBtn = new List<Sprite>();


    protected override void Awake()
    {
        base.Awake();
        instance = this;
        SocketSend.sendJackpotHistory();
        OnClickRule();
    }
    void OnEnable()
    {
        base.OnEnable();

        //var listDataJp = ConfigManager.Instance.listDataJp;


        //var dataJp = listDataJp.Find(data => data.gameid == gameId);
        //if (dataJp == null) return;

        //var bonusVip = ConfigManager.Instance.listBonusVip;
        //bonusVip = dataJp.bonus_vip;
        //lbTextRule.text = dataJp.text;
        //lbExample.text = dataJp.textExample;

        //scrJackPot.content.DestroyAllChildren();

        //for (int i = 0; i < bonusVip.Length; i++)
        //{
        //    string strData = bonusVip[i];
        //    var item = Instantiate(item_history_jackpot_prefab).GetComponent<ItemRuleJackPot>();
        //    item.transform.SetParent(scrJackPot.content, false);
        //    item.SetInfo(i, strData);
        //}

        //OnPopOn();
    }

    public void SetInfo(JObject data)
    {
        Debug.Log("SetInfo Jackpot Binh----" + data.ToString());
        JObject dataJP = JObject.Parse((string)data["data"]);
        List<JObject> lswin = dataJP["lswin"].ToObject<List<JObject>>();

        for (var i = 0; i < lswin.Count; i++)
        {
            JObject dataPl = (JObject)lswin[i];
            GameObject item = Instantiate(itemHistory, scrHistoryJackPot.content.transform);

            long epochTime = (long)dataPl["timeWin"];
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(epochTime);
            DateTime dateTime = dateTimeOffset.DateTime;
            item.transform.Find("textTime").GetComponent<TextMeshProUGUI>().text = dateTime.ToString();
            item.transform.Find("textPlayer").GetComponent<TextMeshProUGUI>().text
                = ObjectParse.getString(dataPl, "username");
            item.transform.Find("textReward").GetComponent<TextMeshProUGUI>().text
                = ObjectParse.getString(dataPl, "markWin");
            item.SetActive(true);
        }
    }

    //void OnClose()
    //{
    //    OnPopOff();
    //    SoundManager1.instance.playButton();
    //    toggleHistory.isOn = false;
    //    toggleRule.isOn = true;
    //    OnClickRule();
    //    GameManager.Instance.SetCurView(GameManager.Instance.curGameViewId);
    //}

    //void UpdateHistory()
    //{
    //    var data = GameManager.Instance.list_data_jackpot_history;
    //    GameObject item;
    //    var itemPool = UIManager.instance.historyJackPotPool;

    //    foreach (Transform child in listHistory.content)
    //    {
    //        child.gameObject.SetActive(false);
    //    }

    //    for (int i = 0; i < data.Count; i++)
    //    {
    //        var dataHis = data[i];
    //        item = listHistory.content.GetChild(i).gameObject;

    //        if (item == null)
    //        {
    //            if (itemPool.Count < 1)
    //            {
    //                itemPool.Put(Instantiate(Global.ItemHistoryJackPot.node));
    //            }

    //            item = itemPool.Get();
    //            item.transform.SetParent(listHistory.content, false);
    //        }

    //        item.SetActive(true);
    //        var time_ = new System.DateTime(dataHis.timeWin);
    //        var _time = time_.Day + "." + (time_.Month + 1) + "." + time_.Year +
    //            "\n" + time_.Hour + ":" + time_.Minute + ":" + time_.Second;

    //        item.GetComponent<ItemHistoryJackPot>().SetInfo(_time, dataHis.userName, dataHis.reward, i);
    //    }
    //}

    public void OnClickRule()
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLICK);
        btnRule.GetComponent<Image>().sprite = listSpriteBtn[0];
        btnHis.GetComponent<Image>().sprite = listSpriteBtn[3];
        tabRule.SetActive(true);
        tabHistory.SetActive(false);
    }

    public void OnClickHistory()
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLICK);
        btnRule.GetComponent<Image>().sprite = listSpriteBtn[1];
        btnHis.GetComponent<Image>().sprite = listSpriteBtn[2];
        tabRule.SetActive(false);
        tabHistory.SetActive(true);
    }
}
