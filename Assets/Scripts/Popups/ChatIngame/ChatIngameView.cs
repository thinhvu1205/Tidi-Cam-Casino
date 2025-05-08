using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class ChatIngameView : BaseView
{
    public static ChatIngameView instance;
    [SerializeField]
    ScrollRect scrText, scrEmo1, scrEmo2;

    [SerializeField]
    GameObject itemChatText, itemChatEmo;

    [SerializeField]
    Image tab1, tab2;
    [SerializeField]
    Image tabEmo1, tabEmo2;

    protected override void Awake()
    {
        base.Awake();
        ChatIngameView.instance = this;
        //chat_text_1004_1
        for (var i = 1; i <= 10; i++)
        {
            string msg = "";
            if (Globals.Config.curGameId == (int)Globals.GAMEID.DOMINO)
            {
                msg = Globals.Config.getTextConfig("chat_text_" + i);
            }
            else
            {
                msg = Globals.Config.getTextConfig(string.Format("chat_text_{0}_{1}", Globals.Config.curGameId, i));
            }
            var item = Instantiate(itemChatText, scrText.content);

            item.transform.GetComponentInChildren<TextMeshProUGUI>().text = msg;

            //item.transform.parent = scrText.content;
            item.transform.localScale = Vector3.one;
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickChatText(msg);
            });
        }

        for (var i = 1; i <= 15; i++)
        {
            var item = Instantiate(itemChatEmo, scrEmo1.content);

            //item.transform.parent = scrEmo2.content;
            item.transform.localScale = Vector3.one;
            string iidd = i + "";
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickChatEmo(iidd);
            });
            var skeEmo = Resources.Load<SkeletonDataAsset>("GameView/Emo/emoticon/e" + i + "/skeleton_SkeletonData");
            var skeletonGraphic = item.GetComponentInChildren<SkeletonGraphic>();
            skeletonGraphic.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            skeletonGraphic.skeletonDataAsset = skeEmo;
            skeletonGraphic.startingAnimation = "animation";
            skeletonGraphic.startingLoop = true;
            skeletonGraphic.Initialize(true);
        }
        for (var i = 16; i <= 24; i++)
        {
            var item = Instantiate(itemChatEmo, scrEmo2.content);
            //item.transform.parent = scrEmo1.content;
            item.transform.localScale = Vector3.one;

            string iidd = i + "";
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickChatEmo(iidd);
            });

            var skeEmo = Resources.Load<SkeletonDataAsset>("GameView/Emo/emoticon/e" + i + "/skeleton_SkeletonData");
            var skeletonGraphic = item.GetComponentInChildren<SkeletonGraphic>();
            skeletonGraphic.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            skeletonGraphic.skeletonDataAsset = skeEmo;
            skeletonGraphic.startingAnimation = "animation";
            skeletonGraphic.startingLoop = true;
            skeletonGraphic.Initialize(true);
        }

        onClickTab("0");
        onClickTabEmo("0");
    }


    void onClickChatText(string text)
    {
        Globals.Logging.Log("onClickChatText: " + text);

        SocketSend.sendChat(Globals.User.userMain.displayName, text);
        hide();
    }

    void onClickChatEmo(string id)
    {
        Globals.Logging.Log("onClickChatEmo: " + id);
        SocketSend.sendChatEmo(Globals.User.userMain.displayName, "", 1, id);
        hide();
    }

    public void onClickTab(string tab)
    {
        bool isTab0 = tab.Equals("0");
        scrText.gameObject.SetActive(isTab0);
        scrEmo1.transform.parent.gameObject.SetActive(!isTab0);

        //tab1.enabled = isTab0;
        tab1.transform.GetChild(1).gameObject.SetActive(isTab0);
        //tab2.enabled = !isTab0;
        tab2.transform.GetChild(1).gameObject.SetActive(!isTab0);
    }

    public void onClickTabEmo(string tab)
    {
        bool isTab0 = tab.Equals("0");
        scrEmo1.gameObject.SetActive(isTab0);
        scrEmo2.gameObject.SetActive(!isTab0);


        //tabEmo1.enabled = isTab0;
        tabEmo1.transform.GetChild(1).gameObject.SetActive(isTab0);
        //tabEmo2.enabled = !isTab0;
        tabEmo2.transform.GetChild(1).gameObject.SetActive(!isTab0);

    }
}
