using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;
using DG.Tweening;

public class FreeChipItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image freechipIcon;
    [SerializeField] TextMeshProUGUI lbMessage, lbChip, lb_time;
    [SerializeField] Button btnReceive;
    [SerializeField] List<Sprite> listIconType = new List<Sprite>();
    Sequence seqCountTime;
    private int type = 0, type_receive = 0, index_arr = 0;
    public int chip = 0;
    private FreeChipData dataItem;
    enum TYPE_FREECHIP
    {
        NORMAL = 1,
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Awake()
    {
        countDownTime1();
    }
    private void countDownTime1()
    {
        if (Globals.Promotion.time <= 0)
        {
            //node.stopAllActions();
            return;
        }

        string ho = Mathf.Floor((Globals.Promotion.time / 3600) % 24) + "";
        string mi = Mathf.Floor((Globals.Promotion.time / 60) % 60) + "";
        string se = Mathf.Floor(Globals.Promotion.time % 60) + "";

        if (ho.Length < 2) ho = "0" + ho;
        if (mi.Length < 2) mi = "0" + mi;
        if (se.Length < 2) se = "0" + se;

        var _time = ho + ":" + mi + ":" + se;
        lb_time.text = _time;
    }
    public void init(int typeItem, string message, int numChip, int receiveType, int index, FreeChipData data)
    {
        type = typeItem;
        type_receive = receiveType;
        index_arr = index;
        chip = numChip;
        dataItem = data;
        freechipIcon.sprite = listIconType[type];
        lbMessage.text = message;
        lbChip.text = Globals.Config.FormatNumber(numChip);
     
        if (receiveType == 69 && type == 3)
        {
            btnReceive.gameObject.SetActive(false);
            lb_time.gameObject.SetActive(true);
            seqCountTime.AppendInterval(1.0f).AppendCallback(() =>
            {
                countDownTime1();
            }).SetLoops(-1);
        }
        else
        {
            btnReceive.gameObject.SetActive(true);
            lb_time.gameObject.SetActive(false);
        }
        //cc.NGWlog('CONTENT SIZE LA ', lbMessage.node.getContentSize());
        if (lbMessage.preferredWidth > 640)
        {
            lbMessage.fontSize = 20;
        }
    }
    public void onClickReceive()
    {

        SoundManager.instance.soundClick();
        //FreeChipView.instance.putFreeChip(gameObject);
        FreeChipView.instance.countMailAg--;
        if (type < 6)
        {
            SocketSend.sendPromotinGold(type_receive, chip);
            if (FreeChipView.instance.transform.parent != null)
            {
                FreeChipView.instance.dataFreeChip.RemoveAt(index_arr);
            }
        }
        else if (type == 7 || type == 8)
        {
            List<int> data = new List<int> { type_receive };
            SocketSend.getArrGold(data);
            Globals.User.userMain.nmAg--;
            if (FreeChipView.instance.transform.parent != null)
            {
                FreeChipView.instance.dataFreeChipAdmin.Remove(dataItem);
            }
        }
       
        Debug.Log("Item Chip=" + chip+"--Item:"+transform.name);
        string msg = Globals.Config.getTextConfig("nhan_ag_tu_ngan_hang");
        UIManager.instance.showMessageBox(Globals.Config.formatStr(msg, Globals.Config.FormatNumber(chip)));
        FreeChipView.instance.reloadList();
        SocketSend.sendUAG();
        SocketSend.sendPromotion();
        UIManager.instance.updateMailAndMessageNoti();
        if (Globals.User.userMain.nmAg > 0 || FreeChipView.instance.countMailAg > 0)
        {
            //Global.MainView.skeletonFreeChip.setAnimation(0, "open", true);
        }
        else
        {
            //Global.MainView.skeletonFreeChip.setAnimation(0, "animation1", true);
        }

    }
}
