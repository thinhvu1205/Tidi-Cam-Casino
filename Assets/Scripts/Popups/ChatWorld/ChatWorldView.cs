using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Globals;

public class ChatWorldView : BaseView
{
    // Start is called before the first frame update
    public static ChatWorldView instance = null;
    [SerializeField]
    ScrollRect scrListWorld;
    [SerializeField]
    GameObject itemChatWorld;
    [SerializeField]
    TMP_InputField edbChatWorld, edbSearchID;

    [SerializeField]
    Toggle btnTabPrivate;

    [SerializeField]
    GameObject tabWorld;

    [SerializeField]
    GameObject NodeOption;

    [SerializeField]
    GameObject itemOption;

    [SerializeField]
    GameObject tabPrivate;

    [SerializeField]
    ChatPrivateView tabPrivateView;

    ItemChatWorld currentItemSelect;

    private List<JObject> dataChatWorld = new List<JObject>();
    protected override void Awake()
    {
        base.Awake();

        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.CHATWORLD);
        ChatWorldView.instance = this;

    }
    //protected override void Start()
    //{
    //    base.Start();
    //    for (int i = 0; i < Globals.COMMON_DATA.ListChatWorld.Count; i++)
    //    {
    //        dataChatWorld.Add(Globals.COMMON_DATA.ListChatWorld[i] as JObject);
    //    }
    //    initListChatWorld();
    //}

    // Update is called once per frame
    void initListChatWorld()
    {
        for (int i = 0; i < dataChatWorld.Count; i++)
        {

            JObject data = dataChatWorld[i];
            GameObject itemMess;
            if (i == 0)
            {
                itemMess = scrListWorld.content.GetChild(0).gameObject;
            }
            else
            {
                itemMess = Instantiate(itemChatWorld, scrListWorld.content);
                itemMess.transform.localScale = Vector3.one;
            }
            itemMess.SetActive(true);
            itemMess.GetComponent<ItemChatWorld>().setInfoMess(data);

        }
        scrListWorld.normalizedPosition = Vector2.zero;

    }



    public void addMessage(JObject data)
    {
        JObject messageData = new JObject();
        messageData["Vip"] = data["V"];
        messageData["ID"] = data["ID"];
        messageData["Name"] = data["N"];
        messageData["Avatar"] = data["Avatar"];
        messageData["Data"] = data["D"];
        GameObject itemMess = Instantiate(itemChatWorld, scrListWorld.content);
        itemMess.transform.localScale = Vector3.one;

        itemMess.GetComponent<ItemChatWorld>().setInfoMess(messageData);
        if ((int)data["ID"] == Globals.User.userMain.Userid)
        {
            edbChatWorld.text = "";
        }
    }
    public void sendChatWorld()
    {

        string msg = edbChatWorld.text;
        SocketSend.sendChatWorld(msg, 1);
        edbChatWorld.text = "";
    }

    public void onClickTabWorld()
    {
        tabWorld.SetActive(true);
        tabPrivate.SetActive(false);
    }
    public void onClickTabPrivate()
    {
        tabWorld.SetActive(false);
        tabPrivate.SetActive(true);
        UIManager.instance.showWaiting();
        SocketSend.getMessList();
    }
    public void showOptionChat(ItemChatWorld itemChat)
    {
        currentItemSelect = itemChat;
        NodeOption.SetActive(true);
        itemOption.transform.localScale = new Vector2(0, 0);
        itemOption.transform.DOScale(new Vector2(1, 1), 0.1f);
        Vector3 itemPos = itemChat.transform.position;
        Vector3 itemOptionPos = NodeOption.transform.InverseTransformPoint(itemPos);
        itemOption.transform.localPosition = new Vector2(itemOption.transform.localPosition.x, itemOptionPos.y);
    }
    public void hideOptionChat()
    {
        NodeOption.SetActive(false);
        currentItemSelect = null;
    }
    public void onClickSearchInfo()
    {
        if (currentItemSelect != null)
        {
            string idPlayer = (string)currentItemSelect.dataChat["ID"];
            SocketSend.searchFriend(idPlayer);
            hideOptionChat();

        }
    }
    public void onClickSendGift()
    {
        if (currentItemSelect != null)
        {
            string idPlayer = (string)currentItemSelect.dataChat["ID"];
            SocketSend.searchFriend(idPlayer);
            hideOptionChat();

        }
    }
    public void onClickSearchID()
    {
        string idPlayer = edbSearchID.text;
        if (idPlayer == "") return;
        SocketSend.searchFriend(idPlayer);
    }
    public void onClickChatPriavte()
    {
        btnTabPrivate.isOn = true;
        tabPrivateView.addChatPrivateWithPlayer(currentItemSelect.dataChat);
        hideOptionChat();
    }
    public void showChatPrivate(JObject dataChat)
    {

        Debug.Log("showChatPrivate");
        tabPrivateView.addChatPrivateWithPlayer(dataChat);
    }


}
