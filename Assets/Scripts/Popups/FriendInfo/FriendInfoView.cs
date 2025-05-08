using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
public class FriendInfoView : BaseView
{

    public static FriendInfoView instance = null;
    [SerializeField]
    TextMeshProUGUI lbNameUser, lbChips, lbUserId, lbStatus;

    [SerializeField]
    Avatar avatar;

    [SerializeField]
    VipContainer vipContainer;
    [HideInInspector]
    public string idFriend;
    JObject dataFriend = new JObject();

    [SerializeField]
    GameObject btnMessage, btnSendGift;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        instance = this;
        transform.eulerAngles = new Vector3(0, 0, 0);
        //if (UIManager.instance.gameView && (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO
        //    || Globals.Config.curGameId == (int)Globals.GAMEID.DUMMY
        //    || Globals.Config.curGameId == (int)Globals.GAMEID.KEANG))
        //{
        //  transform.eulerAngles = new Vector3(0, 0, -90);
        //}
    }

    public void setInfo(JObject jsonData)
    {
        //        {
        //            "evt": "followfind",
        //  "data": "{\"name\":\"ตา แลนน\",\"namelq\":\"\",\"avatar\":2,\"online\":0,\"vip\":2,\"ag\":1300,\"uid\":245943,\"idtable\":0,\"status\":\"ว่าไงวัยรุ่น\",\"level\":0,\"fbid\":717288562770412}",
        //  "status": true
        //}
        dataFriend = jsonData;
        string name = (string)jsonData["name"];
        int avatarId = (int)jsonData["avatar"];
        string userId = (string)jsonData["uid"];
        string fbId = (string)jsonData["fbid"];
        int chip = (int)jsonData["ag"];
        int vip = (int)jsonData["vip"];
        lbNameUser.text = name;
        lbUserId.text = "ID: " + userId;
        idFriend = userId;
        lbChips.text = Globals.Config.FormatNumber(chip);
        avatar.loadAvatar(avatarId, name, fbId);
        avatar.setVip(vip);
        vipContainer.setVip(vip);
        lbStatus.text = (string)jsonData["status"];

        btnMessage.SetActive(Globals.User.userMain.Userid.ToString() != userId);
        btnSendGift.SetActive(Globals.User.userMain.Userid.ToString() != userId);
        if (UIManager.instance.gameView != null)
        {
            btnMessage.SetActive(false);
            btnSendGift.SetActive(false);
        }
    }
    public void onClickSendMessage()
    {
        JObject dataChat = new JObject();
        dataChat["Name"] = dataFriend["name"];
        dataChat["ID"] = dataFriend["uid"];
        dataChat["Avatar"] = dataFriend["avatar"];
        dataChat["FaceID"] = dataFriend["fbid"];
        dataChat["vip"] = dataFriend["vip"];
        if (ChatWorldView.instance != null)
        {
            PlayerPrefs.DeleteKey("dataPrivatePlayer");
            ChatWorldView.instance.showChatPrivate(dataChat);
        }
        else
        {
            PlayerPrefs.SetString("dataPrivatePlayer", dataChat.ToString());
            UIManager.instance.clickTabChatWorld();
            if (LeaderBoardView.instance != null)
            {
                LeaderBoardView.instance.onClickClose();
            }
        }
        hide();
    }
    public void onClickSendGift()
    {
        UIManager.instance.openSendGift(idFriend);
    }
}
