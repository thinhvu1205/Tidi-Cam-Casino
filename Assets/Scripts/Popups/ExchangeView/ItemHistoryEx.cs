using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemHistoryEx : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtTime, txtPrice, txtAmount, txtMobile, txtStatus;
    [SerializeField]
    Button btnCancel;

    JObject dataItem;

    string addZero(int i)
    {
        return (i < 10 ? "0" : "") + i;
    }

    public void setInfo(JObject _dataItem, int chip)
    {
        //    {
        //        "id": 1,
        //  "CashValue": 50.0,
        //  "GcashId": "123",
        //  "typeName": "Gcash",
        //  "CreateTime": 1621239907437,
        //  "status": 2
        //}
        dataItem = _dataItem;
        DateTime time_ = new DateTime(1970, 1, 1).AddMilliseconds((double)dataItem["CreateTime"]);
        time_ = time_.ToLocalTime();
        txtTime.text = addZero(time_.Day) + "/" + addZero(time_.Month) + "/" + time_.Year + "\n" + addZero(time_.Hour) + ":" + addZero(time_.Minute);
        int cash = (int)dataItem["CashValue"];
        txtAmount.text = Globals.Config.FormatNumber(cash);
        txtPrice.text = Globals.Config.FormatNumber(cash * 20000); // sv bảo fix rate là 20000
        //Debug.Log("txtPrice:" + chip);
        this.txtMobile.text = (string)dataItem["GcashId"];

        if (txtMobile.text.Length > 15)
            txtMobile.text = txtMobile.text.Substring(0, 12) + "...";
        if ((int)dataItem["status"] == 0)
        {
            txtStatus.text = Globals.Config.getTextConfig("label_cancel");
            btnCancel.gameObject.SetActive(true);
            //this.btnStatus.getComponent(cc.Sprite).spriteFrame = this.sfCancel
        }
        else if ((int)dataItem["status"] == 1)
        {
            txtStatus.text = Globals.Config.getTextConfig("txt_done");
            btnCancel.gameObject.SetActive(false);
        }
        else if ((int)dataItem["status"] == 4)
        {
            txtStatus.text = Globals.Config.getTextConfig("txt_pending");
            btnCancel.gameObject.SetActive(false);
        }
        else
        {
            txtStatus.text = Globals.Config.getTextConfig("txt_canceled");
            btnCancel.gameObject.SetActive(false);
        }
    }

    public void onClickCancel()
    {
        UIManager.instance.showDialog(Globals.Config.getTextConfig("txt_cancel_CO_noti"), Globals.Config.getTextConfig("ok"), () =>
        {
            SocketSend.sendRejectCashout((int)dataItem["status"], (int)dataItem["id"]);
        }, Globals.Config.getTextConfig("label_cancel"));
    }
}
