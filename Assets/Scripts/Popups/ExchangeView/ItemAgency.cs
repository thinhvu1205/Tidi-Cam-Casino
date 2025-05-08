using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemAgency : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtID, txtName, txtPhone;
    JObject dataItem;

    public void setInfo(JObject dt)
    {
        this.dataItem = dt;
        //      {
        //          "id": "1862315",
        //  "name": "Agency Jason",
        //  "tel": "09396196724",
        //  "msg_fb": "http://bit.ly/jason-agency"
        //}

        txtID.text = (string)dt["id"];
        txtName.text = (string)dt["name"];
        txtPhone.text = (string)dt["tel"];
    }

    public void onClickCall()
    {
        SoundManager.instance.soundClick();
        Application.OpenURL("tel://[" + (string)dataItem["tel"] + "]");
    }
    public void onClickMess()
    {
        SoundManager.instance.soundClick();
        Application.OpenURL((string)dataItem["msg_fb"]);
    }
}
