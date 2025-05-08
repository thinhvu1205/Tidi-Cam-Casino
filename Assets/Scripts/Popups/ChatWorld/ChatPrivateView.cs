using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Globals;
public class ChatPrivateView : MonoBehaviour
{
    // Start is called before the first frame update
    public static ChatPrivateView instance = null;
    [SerializeField]
    ScrollRect scrListMessage;

    [SerializeField]
    ScrollRect scrListFriend;
    [SerializeField]
    GameObject itemListFriend;

    [SerializeField]
    GameObject itemMessage;
    [SerializeField]
    GameObject itemFrPr;


    [SerializeField]
    TMP_InputFieldWithEmoji edbMessage;

    [SerializeField]
    TMP_InputField edbSearch;

    [SerializeField]
    GameObject ic_notify;
    [SerializeField]
    Button btnDelete;
    [SerializeField]
    Button btnSelect;
    [SerializeField]
    GameObject selectTick;
    [SerializeField]
    GameObject NodeOption;
    [SerializeField]
    GameObject itemOption;

    private int currentFriendID = 0;
    private GameObject currentItemFriend = null;
    private string nameFriend = "";
    private JArray listMessageData = new JArray();
    private bool isSelectAll = false;
    private bool isSelectDelete = false;
    //private List<int> listSelectedMessage = new List<int>();
    private JArray listMessagePlayer = new JArray();
    private JObject dataPrivatePlayer;
    private bool isFirstSelectPrivatePlayer = true;
    private bool isFirstClickItemPlayer = true;
    ItemChatWorld currentItemSelect;

    List<GameObject> listSelectDelete = new List<GameObject>();

    List<ItemFriend> listItemFriend = new List<ItemFriend>();

    public Sprite _spriteAvatarSelect = null;

    private void Awake()
    {
        instance = this;
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.CHAT_FRIEND);

    }
    void Start()
    {
        btnDelete.interactable = isSelectDelete;
        UIManager.instance.showWaiting();
        SocketSend.getMessList();
        UIManager.instance.lobbyView.removePopupNoti("CHAT_PRIVATE");
    }

    // Update is called once per frame
    void Update()
    {

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
    private void OnDisable()
    {
        dataPrivatePlayer = null;
    }
    public void initListFriend(JArray listFriend)
    {

        UIManager.instance.hideWatting();

        if (PlayerPrefs.GetString("dataPrivatePlayer") != null && PlayerPrefs.GetString("dataPrivatePlayer") != "")
        {
            dataPrivatePlayer = JObject.Parse(PlayerPrefs.GetString("dataPrivatePlayer"));
        }

        if (dataPrivatePlayer != null)
        {
            Debug.Log("dataPrivatePlayer" + dataPrivatePlayer.ToString());
            JObject dataChat = null;
            foreach (JObject data in listFriend)
            {
                if ((int)data["fromid"] == (int)dataPrivatePlayer["ID"] || (int)data["toid"] == (int)dataPrivatePlayer["ID"])
                {
                    dataChat = data;
                }
            }
            if (dataChat == null)
            {
                dataChat = new JObject();
                dataChat["toid"] = dataPrivatePlayer["ID"];
                dataChat["fromname"] = Globals.User.userMain.displayName;
                dataChat["toname"] = dataPrivatePlayer["Name"];
                dataChat["avatar"] = dataPrivatePlayer["Avatar"];
                dataChat["fid"] = dataPrivatePlayer["FaceID"];
                dataChat["vip"] = dataPrivatePlayer["vip"];
                dataChat["count"] = 0;
                listFriend.AddFirst(dataChat);
                Debug.Log("add vao dau");
            }
        }
        listMessagePlayer = listFriend;
        int sizeListFr = listFriend.Count;
        int sizeItem = scrListFriend.content.childCount;
        listItemFriend.Clear();
        for (int i = 0; i < sizeListFr; i++)
        {
            JObject objData = (JObject)listFriend[i];
            string name = "";
            int idFr = 0;
            if (((string)objData["fromname"]).ToLower().Equals(Globals.User.userMain.displayName.ToLower()))
            {
                name = (string)objData["toname"];
                idFr = (int)objData["toid"];
            }
            else
            {
                name = (string)objData["fromname"];
                idFr = (int)objData["fromid"];
            }
            GameObject item = addItemPlayer(name, idFr, (int)objData["avatar"], (string)objData["fid"], i, (int)objData["vip"]);
            ItemFriend itComp = item.GetComponent<ItemFriend>();
            itComp.data = objData;
            itComp.setInfo();
            listItemFriend.Add(itComp);
        }
        if (sizeListFr < sizeItem)
        {
            for (int i = sizeListFr; i < sizeItem; i++)
            {
                scrListFriend.content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    private GameObject addItemPlayer(string namePL, int idFr, int avatar, string fbId, int index, int vip)
    {
        GameObject item;
        if (index < scrListFriend.content.childCount)
        {
            item = scrListFriend.content.GetChild(index).gameObject;
        }
        else
        {
            item = Instantiate(itemListFriend, scrListFriend.content);
            item.transform.localScale = Vector3.one;

        }
        item.SetActive(true);
        ItemFriend itComp = item.GetComponent<ItemFriend>();
        itComp.avtCtrl.loadAvatar(avatar, namePL, fbId);
        itComp.avtCtrl.setVip(vip);
        itComp.lbName.text = namePL.Length > 10 ? namePL.Substring(0, 7) + "..." : namePL;
        itComp.idFriend = idFr;
        item.GetComponent<Button>().onClick.RemoveAllListeners();
        item.GetComponent<Button>().onClick.AddListener(() =>
        {

            if (isSelectDelete)
            {
                if (!listSelectDelete.Contains(item))
                {
                    listSelectDelete.Add(item);
                    item.GetComponent<ItemFriend>().setSelect(true);
                    btnDelete.interactable = true;
                }
                else
                {
                    item.GetComponent<ItemFriend>().setSelect(false);
                    listSelectDelete.Remove(item);
                    btnDelete.interactable = listSelectDelete.Count > 0;

                }

                return;
            }
            if (currentItemFriend == item)
            {
                return;
            }

            onClickMessDetail(idFr);
            currentFriendID = idFr;
            Debug.Log("currentFriendID:" + currentFriendID);
            nameFriend = namePL;
            if (currentItemFriend != null)
            {
                Debug.Log("currentItemFriend != null");
                currentItemFriend.GetComponent<ItemFriend>().setSelect(false);
                btnDelete.interactable = listSelectDelete.Count > 0;

            }
            currentItemFriend = item;
            _spriteAvatarSelect = currentItemFriend.GetComponent<ItemFriend>().avtCtrl.image.sprite;
            currentItemFriend.GetComponent<ItemFriend>().setSelect(true);
            itComp.ic_nofity.SetActive(false);
            //}
        });

        if (dataPrivatePlayer != null && namePL == (string)dataPrivatePlayer["Name"])
        {
            Debug.Log("co dataprivate");
            if (isFirstClickItemPlayer)
            {
                if (currentItemFriend != null)
                {
                    currentItemFriend.GetComponent<ItemFriend>().setSelect(false);
                }
                currentItemFriend = null;
                item.GetComponent<Button>().onClick.Invoke();
                isFirstClickItemPlayer = false;
                PlayerPrefs.DeleteKey("dataPrivatePlayer");
            }
        }
        return item;
    }
    public void onClickMessDetail(int idFr)
    {
        Debug.Log("onClickMessDetail:" + idFr);
        SocketSend.getMessList();
        SocketSend.getMessageDetail(idFr);


    }
    public void loadListMessage(JArray listMessage)
    {
        listMessageData = listMessage;
        int sizeListMessage = listMessage.Count;
        int sizeContentChild = scrListMessage.content.childCount;
        for (int i = 0; i < sizeListMessage; i++)
        {
            GameObject item;
            if (i < sizeContentChild)
            {
                item = scrListMessage.content.GetChild(i).gameObject;
            }
            else
            {
                item = Instantiate(itemMessage, scrListMessage.content);
                item.transform.localScale = Vector3.one;
            }
            item.SetActive(true);
            JObject jsonData = (JObject)listMessage[i];
            JObject messageData = new JObject();
            messageData["Vip"] = jsonData["vip"];
            messageData["Name"] = jsonData["fromname"];
            messageData["Avatar"] = (int)jsonData["avatar"];
            messageData["Data"] = jsonData["msg"];
            messageData["FaceID"] = jsonData["fid"];
            messageData["time"] = jsonData["timemsg"];
            messageData["ID"] = jsonData["fromid"];

            item.GetComponent<ItemChatWorld>().setInfoMess(messageData);
        }
        if (sizeListMessage < sizeContentChild)
        {
            for (int i = sizeListMessage; i < sizeContentChild; i++)
            {
                scrListMessage.content.GetChild(i).gameObject.SetActive(false);
            }
        }
        RectTransform contentRect = scrListMessage.content.GetComponent<RectTransform>();
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
        {
            //scrListMessage.normalizedPosition = new Vector2(0, 0);
            scrListMessage.DOVerticalNormalizedPos(0, 0.2f);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 10000);
        });
    }
    public void onClickSendMessage()
    {
        if (currentFriendID != 0)
        {
            string msg = edbMessage.text;
            SocketSend.sendMessage(currentFriendID, msg, nameFriend);
        }
        else
        {
            edbMessage.text = "";
        }
    }
    public void addMessage(JObject jsonData)
    {
        //Doan nay phai check xem dang mo tin nhan ma co thang nhan tin den thi xem thang do co phai thang minh dang xem k hay thang khac de add message vao scrollview.Thang khac nhan thi thoi chi notify len thoi k add message.
        int fromID = (int)jsonData["ID"];
        if (fromID == currentFriendID || fromID == Globals.User.userMain.Userid)
        {
            GameObject itemMess;
            if (listMessageData.Count < scrListMessage.content.childCount)
            {
                itemMess = scrListMessage.content.GetChild(listMessageData.Count).gameObject;
            }
            else
            {
                itemMess = Instantiate(itemMessage, scrListMessage.content);
                itemMess.transform.localScale = Vector3.one;
            }
            listMessageData.Add(jsonData);
            itemMess.SetActive(true);
            itemMess.GetComponent<ItemChatWorld>().setInfoMess(jsonData);
            if ((int)jsonData["ID"] == Globals.User.userMain.Userid)
            {
                edbMessage.text = "";
                scrListMessage.normalizedPosition = new Vector2(0, 0);
            }
        }
        else
        {
            int size = scrListFriend.content.childCount;
            for (int i = 0; i < size; i++)
            {
                GameObject itemFriend = scrListFriend.content.GetChild(i).gameObject;
                ItemFriend itComp = itemFriend.GetComponent<ItemFriend>();
                if (itComp.idFriend == fromID)
                {
                    itemFriend.transform.SetSiblingIndex(0);
                    itComp.ic_nofity.SetActive(true);
                }

            }
            ic_notify.SetActive(true);
            //notify co tin nhan moi ra day
        }
    }
    public void onClickSearchFriend()
    {
        string idFr = edbSearch.text;
        if (!idFr.Equals(""))
        {
            SocketSend.searchFriend(idFr);
        }
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
            UIManager.instance.openSendGift(idPlayer);
            hideOptionChat();

        }
    }
    public void onClickSelectAll(Toggle btnSelect)
    {

    }
    public void onClickSelect()
    {
        isFirstClickItemPlayer = true;
        isSelectDelete = !isSelectDelete;
        btnDelete.interactable = false;
        selectTick.SetActive(isSelectDelete);
        listSelectDelete.Clear();
        int sizeFriendItem = scrListFriend.content.childCount;
        for (int i = 0; i < sizeFriendItem; i++)
        {
            var itemFriend = scrListFriend.content.GetChild(i).GetComponent<ItemFriend>();

            if (itemFriend.gameObject.activeSelf)
            {
                itemFriend.setSelect(false);

            }
        }
        if (!isSelectDelete && currentItemFriend != null)
        {
            currentItemFriend.GetComponent<ItemFriend>().setSelect(true);
        }
    }

    public void onClickDelete()
    {
        if (!isSelectDelete) return;
        for (int i = 0; i < listSelectDelete.Count; i++)
        {
            int idFriend = listSelectDelete[i].GetComponent<ItemFriend>().idFriend;
            SocketSend.deleteMessage(idFriend);
            removeMessagePlayerData(idFriend);
        }
        int size = scrListFriend.content.childCount;
        for (int i = 0; i < size; i++)
        {
            var itemFriend = scrListFriend.content.GetChild(i).GetComponent<ItemFriend>();
            if (itemFriend.gameObject.activeSelf && itemFriend.isSelect)
            {
                itemFriend.gameObject.SetActive(false);
            }
        }
        int sizeMess = scrListMessage.content.childCount;
        for (int i = 0; i < sizeMess; i++)
        {
            GameObject itemFriend = scrListMessage.content.GetChild(i).gameObject;
            itemFriend.SetActive(false);
        }
        selectTick.SetActive(false);
        btnDelete.interactable = false;
        isSelectDelete = false;

    }
    private void removeMessagePlayerData(int idFriend)
    {
        int messUnread = 0;
        for (int i = 0; i < listMessagePlayer.Count; i++)
        {
            JObject objData = (JObject)listMessagePlayer[i];
            if ((int)objData["count"] != 0)
            {
                messUnread++;
            }
            int idFr = 0;
            if (((string)objData["fromname"]).ToLower().Equals(Globals.User.userMain.displayName.ToLower()))
            {
                idFr = (int)objData["toid"];
            }
            else
            {
                idFr = (int)objData["fromid"];
            }
            if (idFr == idFriend)
            {
                listMessagePlayer.RemoveAt(i);
                i--;
            }
        }
        List<JObject> listMessage = listMessagePlayer.ToObject<List<JObject>>();
        Globals.User.userMain.messageUnRead = listMessage.FindAll(data => (int)data["count"] != 0).Count;
        UIManager.instance.updateMailAndMessageNoti();
    }
    public void addChatPrivateWithPlayer(JObject dataPlayer)
    {
        int idFr = (int)dataPlayer["ID"];
        ItemFriend itemFriend = listItemFriend.Find(item => item.idFriend == idFr);
        if (itemFriend != null)
        {
            itemFriend.gameObject.SetActive(true);
            itemFriend.GetComponent<Button>().onClick.Invoke();

        }
        else
        {
            dataPrivatePlayer = dataPlayer;
            isFirstClickItemPlayer = true;
            SocketSend.getMessList();
        }

    }
    public GameObject addChatPrivate()
    {
        string name = (string)dataPrivatePlayer["Name"];
        int idFr = (int)dataPrivatePlayer["ID"];
        int Avatar = (int)dataPrivatePlayer["Avatar"];
        string FaceID = (string)dataPrivatePlayer["FaceID"];

        int vip = (int)dataPrivatePlayer["vip"];
        return addItemPlayer(name, idFr, Avatar, FaceID, listMessagePlayer.Count, vip);
    }
}


