using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

public class MailItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lbTitle, lbDesc, lbTime;
    public Toggle btnCheck;
    public GameObject icUnRead;
    public JObject dataMail = new JObject();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setInfo(JObject data)
    {
        dataMail = data;
        string title = (string)data["From"];
        string desc = (string)data["Msg"];
        long timeMili = (long)data["Time"];
        DateTime timeInit = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(timeMili.ToString()));
        string dd = timeInit.ToLocalTime().ToString();
        var regex = new Regex(Regex.Escape(" "));
        lbTime.text = regex.Replace(dd, "\n", 1);
        lbDesc.text = desc.Length > 40 ? (desc.Substring(0, 37) + "...") : desc;
        lbTitle.text = title;
        icUnRead.SetActive((int)dataMail["S"] == 0);
        btnCheck.isOn = false;
    }
    public void onSelectMail(Toggle btnSelected)
    {
        MailView.instance.checkSelectedMail();
    }
    public void deleteMail()
    {

    }
    public void clickMailDetail()
    {
        MailView.instance.showMailDetail(dataMail);
        SocketSend.sendReadMail((int)dataMail["Id"]);
        SocketSend.getMail(10);
    }

}
