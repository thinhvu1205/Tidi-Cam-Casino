using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;
using Globals;

public class FreeChipView : BaseView
{
    public static FreeChipView instance = null;
    // Start is called before the first frame update
    //[SerializeField] ListViewCtrl scrFreeChip;
    [SerializeField] ScrollRect list_view;
    [SerializeField] TextMeshProUGUI lbNoFree;
    [SerializeField] GameObject item_free, btnClose;
    public int countMailAg = 0;
    public List<FreeChipData> dataFreeChip = new List<FreeChipData>();
    public List<FreeChipData> dataFreeChipAdmin = new List<FreeChipData>();
    private List<GameObject> freeChipPool = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        SocketIOManager.getInstance().emitSIOCCCNew(Globals.Config.formatStr("ClickFreeChip_%s", Globals.CURRENT_VIEW.getCurrentSceneName()));
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.FREECHIP_VIEW);
        dataFreeChip.Clear();
        SocketSend.getMail(12);
        UIManager.instance.lobbyView.removePopupNoti("FREE_CHIP");
    }
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    // Update is called once per frame
    public void loadFreeChip()
    {
        dataFreeChip.Clear();
        countMailAg = 0;
        if (Globals.Promotion.adminMoney > 0)
        {
            FreeChipData free = new FreeChipData();
            free.type = 0;
            free.message = Globals.Config.getTextConfig("txt_chip_receive") + Globals.Config.FormatNumber(Globals.Promotion.adminMoney);
            free.chips = Globals.Promotion.adminMoney;
            free.receiveType = 1;
            dataFreeChip.Add(free);
            countMailAg++;
        }
        if (Globals.Promotion.online > 0)
        {
            var free = new FreeChipData();
            free.type = 3;
            free.message =
            Globals.Config
              .getTextConfig("txt_chip_login_receive") +
             Globals.Config.FormatNumber
              (
                Globals.Promotion.online
              );
            free.chips = Globals.Promotion.online;
            free.receiveType = 3;
        }
        else if (
      Globals.Promotion.online == 0 &&
      Globals.Promotion.time > 0)
        {
            if (Globals.Promotion.onlineCurrent < Globals.Promotion.numberP)
            {
                var free = new FreeChipData();
                free.type = 3;
                free.message =
                 Globals.Config
                  .getTextConfig("txt_chip_not_enough_money") +
                Globals.Config
                  .FormatNumber(
                   Globals.Promotion.agOnline
                  );
                free.chips = Globals.Promotion.agOnline;
                free.receiveType = 69;
            }
        }
        if (Globals.Promotion.upVip > 0)
        {
            var free = new FreeChipData();
            free.type = 2;
            free.message = Globals.Config.getTextConfig("txt_chip_up_vip") + Globals.Config.FormatNumber(Globals.Promotion.upVip);
            free.chips = Globals.Promotion.upVip;
            free.receiveType = 2;
            dataFreeChip.Add(free);
            countMailAg++;
        }
        if (Globals.Promotion.notEnoughMoney > 0)
        {
            var free = new FreeChipData();
            free.type = 1;
            free.message = Globals.Config.getTextConfig("txt_chip_not_enough_money") +
             Globals.Config.FormatNumber(Globals.Promotion.notEnoughMoney);
            free.chips = Globals.Promotion.notEnoughMoney;
            free.receiveType = 0;
            dataFreeChip.Add(free);
            countMailAg++;
        }
        if (Globals.Promotion.giftCode > 0)
        {
            var free = new FreeChipData();
            free.type = 5;
            free.message =
             Globals.Config.getTextConfig("txt_chip_gift_code") +
              Globals.Config.FormatNumber(Globals.Promotion.giftCode);
            free.chips = Globals.Promotion.giftCode;
            free.receiveType = 6;
            dataFreeChip.Add(free);
            countMailAg++;
        }

        if (Globals.User.userMain.NM > 0 || countMailAg > 0)
        {
            //Global.MainView.skeletonFreeChip.setAnimation(0, "open", true);
        }
        else
        {
            //Global.MainView.skeletonFreeChip.setAnimation(0, "animation1", true);
        }
        reloadList();
    }
    public void reloadList()
    {

        Debug.Log("-=-dataFreeChip " + dataFreeChip.Count);
        Debug.Log("-=-dataFreeChipAdmin " + dataFreeChipAdmin.Count);

        List<FreeChipData> listData = new List<FreeChipData>();
        listData.AddRange(dataFreeChip);
        listData.AddRange(dataFreeChipAdmin);
        int dataSize = listData.Count;
        lbNoFree.gameObject.SetActive(!(dataSize > 0));
        //foreach (GameObject item in list_view.content.transform.GetComponentsInChildren<GameObject>())
        //{
        //    putFreeChip(item);
        //}
        for (var i = 0; i < dataSize; i++)
        {
            FreeChipItem item;
            if (i < list_view.content.childCount)
            {
                GameObject ScrCtItem = list_view.content.GetChild(i).gameObject;
                item = ScrCtItem.GetComponent<FreeChipItem>();
            }
            else
            {
                item = getFreeChipPool().GetComponent<FreeChipItem>();
                item.transform.SetParent(list_view.content);
                item.transform.localScale = Vector2.one;
            }
            //item.GetComponent<Image>().enabled = (i % 2) == 0;
            item.gameObject.SetActive(true);
            item.init(
              listData[i].type,
              listData[i].message,
              listData[i].chips,
              listData[i].receiveType,
              i,
              listData[i]

            );
        }
        for (int i = dataSize; i < list_view.content.childCount; i++)
        {
            putFreeChip(list_view.content.transform.GetChild(i).gameObject);
        }
        //dataSize = dataFreeChipAdmin.Count;
        //for (int i = 0, l = dataSize; i < l; i++)
        //{
        //    FreeChipItem item;
        //    if (i < list_view.content.childCount)
        //    {
        //        GameObject ScrCtItem = list_view.content.GetChild(i + index).gameObject;
        //        item = ScrCtItem.GetComponent<FreeChipItem>();
        //    }
        //    else
        //    {
        //        item = getFreeChipPool().GetComponent<FreeChipItem>();
        //        item.transform.SetParent(list_view.content);
        //        item.transform.localScale = Vector2.one;
        //    }
        //    item.gameObject.SetActive(true);
        //    item.init(
        //      dataFreeChipAdmin[i].type,
        //     dataFreeChipAdmin[i].message,
        //     dataFreeChipAdmin[i].chips,
        //      dataFreeChipAdmin[i].receiveType,
        //      i
        //    );
        //}

    }
    private GameObject getFreeChipPool()
    {
        GameObject item;
        if (freeChipPool.Count < 1)
        {
            GameObject go = Instantiate(item_free, list_view.content);
            freeChipPool.Add(go);
        }
        item = freeChipPool[0];
        freeChipPool.RemoveAt(0);
        return item;
    }
    public void putFreeChip(GameObject item)
    {
        item.transform.SetParent(null);
        item.SetActive(false);
        freeChipPool.Add(item);
    }

    public void loadDataFreeChip(JArray listData)
    {
        //scrFreeChip.setDataList(setDataItem, listData);
    }
    public void pushMailAdmin(int type, string mess, int chip, int rec_type)
    {
        var free = new FreeChipData();
        free.type = type;
        free.message = mess;
        free.chips = chip;
        free.receiveType = rec_type;
        dataFreeChipAdmin.Add(free);
    }
    public void clearMailAdmin()
    {
        dataFreeChipAdmin.Clear();
    }
    public void setDataItem(GameObject item, JObject data)
    {
        TextMeshProUGUI lbTitle = item.transform.Find("lbTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI lbDescriptions = item.transform.Find("lbDescriptions").GetComponent<TextMeshProUGUI>();
        lbTitle.text = Globals.Config.FormatMoney2((int)data["AG"]);
        lbDescriptions.text = (string)data["Msg"];
        item.SetActive(true);
    }
    public void onClickReceive()
    {

    }

    public void setShowBack(bool isShow)
    {
        btnClose.SetActive(isShow);
    }
}
public class FreeChipData
{
    public int type = 0;
    public string message = "";
    public int chips = 0;
    public int receiveType = 0;
}

