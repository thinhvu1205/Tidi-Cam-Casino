using Globals;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GroupMenuView : BaseView
{
    public static GroupMenuView instance;
    [SerializeField] GameObject m_LeaveButton;
    [SerializeField] Button btnSetting;
    [SerializeField] Button btnChangeTable;
    [SerializeField] Button btnFightTongits;
    [SerializeField] Button btnMusic;
    [SerializeField] Button btnSound;
    [SerializeField] Button btnRule;
    [SerializeField] List<Sprite> listCheck = new List<Sprite>();

    public void onClickSwitchTable()
    {
        SoundManager.instance.soundClick();
        if (UIManager.instance.gameView.stateGame == Globals.STATE_GAME.PLAYING)
        {
            UIManager.instance.showToast(Globals.Config.getTextConfig("txt_intable"));
        }
        else
        {
            //Global.MainView._isClickGame = false;
            Globals.Config.isChangeTable = true;
            onClickBack();
        }
        hide();
    }
    protected override void Start()
    {
        GroupMenuView.instance = this;
        base.Start();
        var curGameId = Globals.Config.curGameId;
        if (curGameId == (int)Globals.GAMEID.KEANG || curGameId == (int)Globals.GAMEID.DUMMY)
        {
            btnSetting.gameObject.SetActive(false);
        }
        if (curGameId == (int)Globals.GAMEID.SLOT20FRUIT || curGameId == (int)Globals.GAMEID.SLOTNOEL || (curGameId == (int)Globals.GAMEID.SLOTTARZAN) || (curGameId == (int)Globals.GAMEID.SLOT_JUICY_GARDEN) || (curGameId == (int)Globals.GAMEID.SLOT_SIXIANG) || (curGameId == (int)Globals.GAMEID.SLOT_INCA))
        {
            btnChangeTable.gameObject.SetActive(false);
        }
        if (curGameId == (int)Globals.GAMEID.RONGHO)
        {
            btnRule.gameObject.SetActive(false);
        }
        if (curGameId == (int)Globals.GAMEID.TONGITS || curGameId == (int)Globals.GAMEID.TONGITS_OLD || curGameId == (int)Globals.GAMEID.TONGITS11 || curGameId == (int)Globals.GAMEID.TONGITS_JOKER)
        {
            btnFightTongits.transform.Find("on").GetComponent<Image>().sprite = TongitsView.IsFight ? listCheck[0] : listCheck[1];
            btnSetting.gameObject.SetActive(false);
            btnFightTongits.gameObject.SetActive(true);
            btnMusic.gameObject.SetActive(true);
            btnSound.gameObject.SetActive(true);
            btnSound.transform.Find("on").GetComponent<Image>().sprite = Globals.Config.isSound ? listCheck[0] : listCheck[1];
            btnMusic.transform.Find("on").GetComponent<Image>().sprite = Globals.Config.isMusic ? listCheck[0] : listCheck[1];
        }

        background.GetComponent<LayoutSizeControl>().updateSizeContent();
        var sizee2 = background.GetComponent<RectTransform>().sizeDelta;
        setOriginPosition(-transform.parent.GetComponent<RectTransform>().rect.width * .5f + sizee2.x * .5f + 10f, 720.0f * .5f - sizee2.y * .5f - 30f);
        show();
    }

    public void onClickRule()
    {
        SoundManager.instance.soundClick();
        hide();
        var curGameId = Globals.Config.curGameId;
        var urlRule = Globals.Config.url_rule.Replace("%gameid%", curGameId + "");
        //var langLocal = cc.sys.localStorage.getItem("language_client");
        //var language = langLocal == LANGUAGE_TEXT_CONFIG.LANG_EN ? "en" : "thai"
        var language = "thai";
        urlRule = urlRule.Replace("%language%", language);
        // https://conf.topbangkokclub.com/rule/index.html?gameid=%gameid%&language=%language%&list=true
        List<int> listGameOther = new List<int> { (int)Globals.GAMEID.SLOT20FRUIT, (int)Globals.GAMEID.SLOT_SIXIANG, (int)Globals.GAMEID.SLOT_INCA, (int)Globals.GAMEID.SLOTNOEL, (int)Globals.GAMEID.SLOTTARZAN, (int)Globals.GAMEID.LUCKY9, (int)Globals.GAMEID.SICBO, (int)Globals.GAMEID.SABONG, (int)Globals.GAMEID.SLOT_INCA, (int)Globals.GAMEID.GAOGEA, (int)Globals.GAMEID.SLOT_JUICY_GARDEN, (int)Globals.GAMEID.BANDAR_QQ, (int)Globals.GAMEID.LUCKY9 };
        if (listGameOther.Contains(curGameId))
        {
            UIManager.instance.gameView.onClickRule();
        }
        else
        {
            //require("Util").onCallWebView(urlRule);
            UIManager.instance.showWebView(urlRule);

        }
    }

    public void onClickSetting()
    {
        SoundManager.instance.soundClick();
        hide();
        UIManager.instance.openSetting();
    }

    public void onClickFightConfirm()
    {
        SoundManager.instance.soundClick();
        TongitsView.IsFight = !TongitsView.IsFight;
        btnFightTongits.transform.Find("on").GetComponent<Image>().sprite = TongitsView.IsFight ? listCheck[0] : listCheck[1];
    }
    public void onClickSound()
    {
        Globals.Config.isSound = !Globals.Config.isSound;
        if (Globals.Config.isSound)
        {
            SoundManager.instance.soundClick();
        }
        Globals.Config.updateConfigSetting();
        btnSound.transform.Find("on").GetComponent<Image>().sprite = Globals.Config.isSound ? listCheck[0] : listCheck[1];
    }
    public void onClickMusic()
    {
        Globals.Config.isMusic = !Globals.Config.isMusic;
        SoundManager.instance.soundClick();
        Globals.Config.updateConfigSetting();
        SoundManager.instance.playMusic();
        btnMusic.transform.Find("on").GetComponent<Image>().sprite = Globals.Config.isMusic ? listCheck[0] : listCheck[1];
    }

    public void onClickBack()
    {
        SoundManager.instance.soundClick();
        if (Globals.Config.curGameId == (int)Globals.GAMEID.SLOTNOEL || Globals.Config.curGameId == (int)Globals.GAMEID.SLOTTARZAN || Globals.Config.curGameId == (int)Globals.GAMEID.SLOT_SIXIANG) //cac game playnow
        {

            hide();
            if (Globals.Config.curGameId == (int)Globals.GAMEID.SLOT_SIXIANG)
            {
                SocketSend.sendExitSlotSixiang(Globals.ACTION_SLOT_SIXIANG.exitGame);
                //string dataLTable = "{\"evt\":\"ltable\",\"Name\":\"${Globals.User.userMain.displayName}\",\"errorCode\":0}";
                JObject dataLTable = new JObject();

                dataLTable["evt"] = "ltable";
                dataLTable["Name"] = Globals.User.userMain.displayName;
                dataLTable["errorCode"] = 0;
                JObject dataLeave = new JObject();
                dataLeave["tableid"] = Globals.Config.tableId;
                dataLeave["curGameID"] = (int)Globals.GAMEID.SLOT_SIXIANG;
                dataLeave["stake"] = 0;
                dataLeave["reason"] = 0;
                //UIManager.instance.gameView.dataLeave=dataLeave;
                HandleGame.processData(dataLTable);
                JObject dataLeavePackage = new JObject();
                dataLeavePackage["tableid"] = Globals.Config.tableId;
                dataLeavePackage["status"] = "OK";
                dataLeavePackage["code"] = 0;
                dataLeavePackage["classId"] = 37;
                HandleData.handleLeaveResponsePacket(dataLeavePackage.ToString());

            }
            else
            {
                SocketSend.sendExitGame();
            }
            return;
        }
        else
        {
            Globals.Logging.Log("Chay vao day!! gameView.stateGame=" + UIManager.instance.gameView.stateGame);
            if (UIManager.instance.gameView.stateGame == Globals.STATE_GAME.PLAYING)
            {
                Globals.Config.isBackGame = !Globals.Config.isBackGame;
                UIManager.instance.gameView.thisPlayer.playerView.setExit(Globals.Config.isBackGame);
                string msg = Globals.Config.isBackGame ? Globals.Config.getTextConfig("wait_game_end_to_leave") : Globals.Config.getTextConfig("minidice_unsign_leave_table");
                UIManager.instance.showToast(msg);
                Debug.Log("back game " + Globals.Config.isBackGame);

            }
            else//con moi 1 minh minh thi cung cho thoat
            {
                SocketSend.sendExitGame();
            }
            hide();
        }
    }
    public void onClickQuit()
    {
        SoundManager.instance.soundClick();
        onClickBack();
    }
}
