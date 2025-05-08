using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

public class MailDetailView : BaseView
{
    public static MailDetailView instance;
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lbFrom, lbTime, lbMsg;
    protected override void Awake()
    {
        base.Awake();
        MailDetailView.instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setInfo(JObject data)
    {
        lbFrom.text = "From: " + (string)data["From"];
        long timeMili = (long)data["Time"];
        DateTime timeInit = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(timeMili.ToString()));
        string dd = timeInit.ToLocalTime().ToString();
        var regex = new Regex(Regex.Escape(" "));
        lbTime.text = "Time: " + dd;
        lbMsg.text = (string)data["Msg"];
    }
}
