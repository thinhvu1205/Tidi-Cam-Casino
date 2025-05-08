using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Globals;

public class MailView : BaseView
{
    // Start is called before the first frame update
    public static MailView instance;
    [SerializeField]
    ScrollRect scrMail;

    [SerializeField]
    GameObject itemMailPr;
    [SerializeField]
    Toggle btnSelectAll;

    public List<JObject> listMailData = new List<JObject>();
    public List<MailItem> listMailSelected = new List<MailItem>();
    protected override void Awake()
    {
        base.Awake();
        MailView.instance = this;

        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.MAIL);
        UIManager.instance.showWaiting();
        SocketSend.getMail(10);
    }
    protected override void Start()
    {
        base.Start();
        UIManager.instance.lobbyView.removePopupNoti("MAIL_ADMIN");
    }
    // Update is called once per frame

    public void reloadListMail()
    {
        int size = listMailData.Count;
        int sizeItem = scrMail.content.childCount;
        if (size == 0)
        {
            btnSelectAll.isOn = false;
        }
        Globals.Logging.Log("reload ListMail:" + size + "--size size item==" + sizeItem);
        for (int i = 0; i < size; i++)
        {
            JObject data = listMailData[i];
            MailItem item;
            if (i < sizeItem)
            {
                item = scrMail.content.GetChild(i).GetComponent<MailItem>();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(itemMailPr, scrMail.content).GetComponent<MailItem>();
                item.transform.localScale = Vector3.one;

            }
            //Globals.Logging.Log("Mail Data:" + data.ToString());
            item.setInfo(data);
        }
        if (size < sizeItem)
        {
            for (int i = size; i < sizeItem; i++)
            {
                scrMail.content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void onSelectAll(Toggle btnSelect)
    {
        SoundManager.instance.soundClick();
        btnSelectAll.graphic.gameObject.GetComponent<Image>().enabled = true;
        int size = scrMail.content.childCount;
        for (int i = 0; i < size; i++)
        {
            MailItem itemMail = scrMail.content.GetChild(i).gameObject.GetComponent<MailItem>();
            itemMail.btnCheck.isOn = btnSelect.isOn;
        }
    }
    public void checkSelectedMail()
    {

        int size = scrMail.content.childCount;
        bool isAllOn = true;
        for (int i = 0; i < size; i++)
        {
            MailItem itemMail = scrMail.content.GetChild(i).gameObject.GetComponent<MailItem>();
            if (itemMail.btnCheck.isOn == false)
            {
                isAllOn = false;
            }
            else
            {
                listMailSelected.Add(itemMail);
            }
        }
        btnSelectAll.graphic.gameObject.GetComponent<Image>().enabled = isAllOn;
    }
    public void onClickDeleteMail()
    {
        SoundManager.instance.soundClick();
        int sizeContent = scrMail.content.childCount;
        listMailSelected.Clear();
        for (int i = 0; i < sizeContent; i++)
        {
            MailItem itemMail = scrMail.content.GetChild(i).gameObject.GetComponent<MailItem>();
            if (itemMail.btnCheck.isOn)
            {
                listMailSelected.Add(itemMail);
            }
        }
        int size = listMailSelected.Count;

        List<int> arrId = new List<int>();
        for (int i = 0; i < size; i++)
        {
            MailItem mailItem = listMailSelected[i];
            Globals.Logging.Log("Mail Selected:" + mailItem.btnCheck.isOn);
            if (mailItem.btnCheck.isOn)
            {
                arrId.Add((int)mailItem.dataMail["Id"]);
                Globals.Logging.Log("Delete Mail:" + (int)mailItem.dataMail["Id"]);
                //deleteMailData((int)mailItem.dataMail["Id"]);
            }
        }
        SocketSend.deleteMailAdmin(arrId);
        UIManager.instance.showWaiting();
        SocketSend.getMail(10);
    }
    void deleteMailData(int id)
    {
        JObject mailDelete = listMailData.Find(data => (int)data["Id"] == id);
        listMailData.Remove(mailDelete);
        reloadListMail();
    }
    public void showMailDetail(JObject data)
    {
        UIManager.instance.openMailDetail(data);
    }
}
