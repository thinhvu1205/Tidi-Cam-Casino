using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;

public class ProfileView : BaseView
{
    // Start is called before the first frame update
    public static ProfileView instance;
    [SerializeField] GameObject AvatarPr, btnChangeName, btnRegister, btnChangePass, refContainer, btnEdit, btnSave, btnCancel;
    [SerializeField] TextMeshProUGUI lbName, lbChips, lbAgSafe, lbId, lbIdWallet, lbTimeRemainRef;
    [SerializeField] Button btnSendGift, btnConfirmRef, btnSlectTypeCO;
    [SerializeField] Avatar _avatar;
    [SerializeField] VipContainer VipContainer;
    [SerializeField] ScrollRect scrListAvatar;
    [SerializeField] TMP_InputFieldWithEmoji edbStatus;
    [SerializeField] TMP_InputField edbRef, edbPhone;
    [SerializeField] WalletInfo walletInfo;
    public DropBoxCustom dropBox;

    private List<JObject> dataWalletInfo = new(), dataSendSave = new();
    private long currentTime = 0;
    private bool isChangeStatus = false;
    private int currentAvatarId = 0;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    protected override void Start()
    {
        base.Start();
        currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        setInfo();
        dropBox.Hide();
        dropBox.setCallback(onClickSelect);
        //updateInfoCO();
        //setShowHideButtonCO(true, false, false);
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void setInfo()
    {
        lbName.text = User.userMain.displayName;
        //Config.effectTextRunInMask(lbName);
        lbChips.text = Config.FormatNumber(User.userMain.AG);
        lbAgSafe.text = Config.FormatNumber(User.userMain.agSafe);
        lbId.text = "ID: " + User.userMain.Userid.ToString();
        currentAvatarId = User.userMain.Avatar;
        _avatar.loadAvatar(User.userMain.Avatar, User.userMain.Username, User.FacebookID);
        VipContainer.setVip(User.userMain.VIP);
        _avatar.setVip(User.userMain.VIP);
        loadListAvatar();
        btnSendGift.gameObject.SetActive(Config.ketT);
        SocketSend.getStatus();

        lbAgSafe.transform.parent.gameObject.SetActive(Config.ket);
        updateStateChangeName();
        btnConfirmRef.gameObject.SetActive(User.userMain.canInputInvite);
        Debug.Log(" uidInvite:" + User.userMain.uidInvite);
        if (User.userMain.timeInputInvite > 0)
        {

            refContainer.SetActive(true);
            countTimeRemainRef();
            edbRef.interactable = true;
        }
        else
        {
            if (User.userMain.uidInvite != 0)
            {
                refContainer.SetActive(true);
                edbRef.text = User.userMain.uidInvite.ToString();
                edbRef.interactable = false;
            }
            else
            {
                refContainer.SetActive(false);
            }
            lbTimeRemainRef.text = "";
            DOTween.Kill(DOTWEEN_TAG.PROFILE_COUNTTIME);
        }
        SocketSend.getWalletInfo();
        SocketSend.checkUpdateWallet();
    }
    private void countTimeRemainRef()
    {
        DOTween.Sequence()
            .AppendCallback(() =>
            {
                currentTime += 1000;
                long deltaTime = User.userMain.timeInputInviteRemain - currentTime;
                lbTimeRemainRef.text = Config.convertSeccondToDDHHMMSS((int)deltaTime / 1000);
                if (currentTime < 1000)
                {
                    DOTween.Kill(DOTWEEN_TAG.PROFILE_COUNTTIME);
                    refContainer.SetActive(false);
                }
            })
            .AppendInterval(1.0f)
            .SetLoops(-1)
            .SetId(1911);
    }
    public void onAddFriendIdRefSuccess()
    {
        DOTween.Kill(DOTWEEN_TAG.PROFILE_COUNTTIME);
        User.userMain.timeInputInvite = 0;
        User.userMain.timeInputInviteRemain = 0;
        User.userMain.uidInvite = Config.splitToInt(edbRef.text);
        btnConfirmRef.gameObject.SetActive(false);
        edbRef.interactable = false;
        setInfo();

    }
    public void updateStateChangeName()
    {
        if (Config.typeLogin == LOGIN_TYPE.FACEBOOK
            || Config.typeLogin == LOGIN_TYPE.APPLE_ID
            || !User.userMain.Username.ToUpper().Contains("TE."))
        {
            btnChangeName.SetActive(false); //ko doi ten

            if (!User.userMain.Username.ToUpper().Contains("TE."))
            {
                btnChangePass.SetActive(true);
                btnRegister.SetActive(false);
            }
            if (Config.typeLogin == LOGIN_TYPE.FACEBOOK)
            {
                btnChangePass.SetActive(false);//ko doi pass
            }
        }
        else
        {
            if (Config.typeLogin == LOGIN_TYPE.PLAYNOW && User.userMain.Username.ToUpper().Contains("TE."))
            {
                btnChangePass.SetActive(false);
                btnRegister.SetActive(true);
            }
            else
            {
                btnChangePass.SetActive(true);
                btnRegister.SetActive(false);
            }
            btnChangeName.SetActive(true);
        }
    }
    public void updateAg()
    {
        lbChips.text = Config.FormatNumber(User.userMain.AG);

    }
    public void updateStatus(string status)
    {
        if (status.Contains("คุณกำลังคิดอะไรอยู่"))
        {
            edbStatus.placeholder.GetComponent<TextMeshProUGUI>().text = status;
        }
        else
            edbStatus.text = status;

        if (isChangeStatus)
        {
            //UIManager.instance.showToast("Change status successfully!");
            isChangeStatus = false;
        }

    }
    public void onClickRuleRefence()
    {
        if (Config.url_rule_refGuide != "")
        {
            UIManager.instance.showWebView(Config.url_rule_refGuide, Config.getTextConfig("txt_rule"));
        }
    }
    private void loadListAvatar()
    {
        for (int i = 0; i < 36; i++)
        {
            GameObject avatarItem;
            if (i < scrListAvatar.content.childCount)
            {
                avatarItem = scrListAvatar.content.GetChild(i).gameObject;
            }
            else
            {
                avatarItem = Instantiate(AvatarPr, scrListAvatar.content);
                avatarItem.transform.localScale = AvatarPr.transform.localScale;

            }
            avatarItem.SetActive(true);
            avatarItem.GetComponent<Avatar>().setSpriteWithID((i + 1));
            avatarItem.GetComponent<Avatar>().idAvt = (i + 1);
            avatarItem.GetComponent<Avatar>().setVip(Globals.User.userMain.VIP);
        }
    }
    public void onClickQuit()
    {
        SoundManager.instance.soundClick();
        Application.Quit();
    }
    public void onClickLogout()
    {
        SoundManager.instance.soundClick();
        SocketIOManager.getInstance().emitSIOCCCNew("ClickLogOut");
        SocketSend.sendLogOut();
        Config.typeLogin = LOGIN_TYPE.NONE;
        PlayerPrefs.SetInt("type_login", (int)LOGIN_TYPE.NONE);
        PlayerPrefs.Save();
        UIManager.instance.showLoginScreen(false);
    }
    public void onClickChangeAvatar(Avatar avatarItem)
    {
        SoundManager.instance.soundClick();
        SocketSend.changeAvatar(avatarItem.idAvt);
        currentAvatarId = avatarItem.idAvt;
    }
    public void onChangeAvatar()
    {
        User.userMain.Avatar = currentAvatarId;
        _avatar.loadAvatar(User.userMain.Avatar, User.userMain.Username, "");
    }
    public void onClickChangePass()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openChangePass();
    }

    public void onClickChangeName()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openChangePass();
    }
    public void onClickSendGift()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openSendGift();
        onClickClose();
    }
    public void onClickChangeStatus()
    {
        SoundManager.instance.soundClick();
        isChangeStatus = true;
        SocketSend.changeStatus(edbStatus.text);
    }
    public void onClickAddChips()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openShop();
        this.onClickClose();
    }
    public void onOpenSafe()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openSafeView();
        this.onClickClose();
    }
    public void ondEdbStatusChange()
    {
        SoundManager.instance.soundClick();
        if (edbStatus.text != "")
        {
            SocketSend.changeStatus(edbStatus.text);
        }
    }
    public void onClickConfirmRef()
    {
        SoundManager.instance.soundClick();
        if (edbRef.text != "")
        {
            SocketSend.sendIdReferFriend(int.Parse(edbRef.text));
            UIManager.instance.showWaiting();
        }
    }

    public void OnClickDropBox()
    {
        SoundManager.instance.soundClick();
        if (dropBox.isActiveAndEnabled)
        {
            dropBox.Hide();
        }
        else
        {
            dropBox.Show();
        }

    }

    void setShowHideButtonCO(bool isEdit, bool isSave, bool isCancel)
    {
        Debug.Log("setShowHideButtonCO");
        btnEdit.SetActive(isEdit);
        btnSave.SetActive(isSave);
        btnCancel.SetActive(isCancel);
        edbPhone.enabled = !isEdit;
        btnSlectTypeCO.interactable = !isEdit;
    }

    int indexType = 0;

    void onClickSelect(int index)
    {
        indexType = index;
        WalletItem itemW = dropBox.listBtnDropbox[index].GetComponent<WalletItem>();
        lbIdWallet.text = itemW.idWallet;
    }

    public void onClickEdit()
    {
        SoundManager.instance.soundClick();
        //setShowHideButtonCO(false, true, true);
    }

    public void OnClickSavePhone()
    {
        SoundManager.instance.soundClick();
        string strAdd = edbPhone.text;
        if (strAdd.Equals("")) return;
        JObject dataSave = new JObject();

        var strData = PlayerPrefs.GetString("data_type_co", "");
        if (!strData.Equals(""))
        {
            dataSave = JObject.Parse(strData);
        }
        dataSave["index"] = indexType;
        dataSave["phone"] = strAdd;
        PlayerPrefs.SetString("data_type_co", dataSave.ToString());
        PlayerPrefs.Save();
        //setShowHideButtonCO(true, false, false);
    }

    void updateInfoCO()
    {
        var strData = PlayerPrefs.GetString("data_type_co", "");
        if (strData.Equals(""))
        {
            edbPhone.text = "";
            return;
        }
        JObject dataSave = JObject.Parse(strData);
        dropBox.SetSlectWithIndex((int)dataSave["index"]);
        edbPhone.text = (string)dataSave["phone"];
    }

    public void OnClickCancelPhone()
    {
        SoundManager.instance.soundClick();
        updateInfoCO();

        //setShowHideButtonCO(true, false, false);
    }
    public void handleWalletInfo(JObject jsonData)
    {
        Debug.Log("handleWalletInfo");
        List<JObject> data = jsonData["data"].ToObject<List<JObject>>();
        Debug.Log("data size=" + data.Count);
        dataWalletInfo = data;
        walletInfo.gameObject.SetActive(data.Count > 0);
        walletInfo.setInfo(data);

    }
    public void handleUPdateWallet(JObject jsonData)
    {
        //{"evt":"checkUpdateWallet","data":[{"walletId":"123456799","type":"GcaSH","urlImg":"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/cashout/gcash.png?inline\u003dtrue"}],"showUpdate":true}

        List<JObject> data = jsonData["data"].ToObject<List<JObject>>();
        Debug.Log("handleUPdateWallet" + jsonData.ToString());
        if (data.Count > 0 && (bool)jsonData["showUpdate"])
        {
            List<JObject> dataSend = new List<JObject>();
            for (int i = 0; i < data.Count; i++)
            {
                JObject dtIt = data[i];
                JObject itemID = dataWalletInfo.Find(data =>
                {
                    return ((string)data["type"]).ToUpper() == ((string)dtIt["type"]).ToUpper();
                });
                JObject dataAdd = new JObject();

                if (itemID != null)
                {
                    if ((string)itemID["walletId"] == (string)dtIt["walletId"])
                    {
                        continue;
                    }

                    dataAdd["isUpdate"] = true;
                    dataAdd["data"] = dtIt;
                    dataSend.Add(dataAdd);

                }
                else
                {
                    dataAdd["isUpdate"] = false;
                    dataAdd["data"] = dtIt;
                    dataSend.Add(dataAdd);
                }
                checkUpdateSaveWalletID(dataSend);
            }
        }
    }
    public void checkUpdateSaveWalletID(List<JObject> dataSend)
    {

        if (dataSend.Count <= 0) return;
        JObject dataItem = dataSend[0];
        dataSend.RemoveAt(0);
        //WalletInfo _walletInfo = walletInfo;
        JObject dataIn = (JObject)dataItem["data"];
        string msg = Config.getTextConfig(((bool)dataItem["isUpdate"] ? "txt_noti_update_id" : "txt_noti_save_id"));//.Replace("%s", );
        msg = Globals.Config.formatStr(msg, ((string)dataIn["type"]).ToUpper(), (string)dataIn["walletId"]);
        if (dataSend.Count <= 0)
        {

            UIManager.instance.showDialog(msg, Config.getTextConfig("txt_ok"), () =>
            {

                dataIn["isUpdate"] = true;
                dataSendSave.Add(dataIn);
                SocketSend.updateWallet(dataSendSave);
            }, Config.getTextConfig("label_cancel"), () =>
            {
                dataIn["isUpdate"] = false;
                dataSendSave.Add(dataIn);
                SocketSend.updateWallet(dataSendSave);
            });
            dropBox.gameObject.SetActive(false);
        }
        else
        {
            UIManager.instance.showDialog(msg, Config.getTextConfig("label_cancel"), () =>
            {
                dataIn["isUpdate"] = false;
                dataSendSave.Add(dataIn);
                checkUpdateSaveWalletID(dataSend);
            }, Config.getTextConfig("txt_ok"), () =>
            {
                dataIn["isUpdate"] = true;
                dataSendSave.Add(dataIn);
                checkUpdateSaveWalletID(dataSend);
            });
            dropBox.gameObject.SetActive(false);
        }
    }

}
