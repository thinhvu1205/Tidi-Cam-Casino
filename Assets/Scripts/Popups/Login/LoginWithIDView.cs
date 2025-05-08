using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginWithIDView : BaseView
{
    // Start is called before the first frame update
    [SerializeField]
    TMP_InputField edbUserName;

    [SerializeField]
    TMP_InputField edbPassword;
    protected override void Awake()
    {
        base.Awake();
        Globals.Config.password_normal = "";
    }
    // Update is called once per frame
    protected void OnDisable()
    {
        Debug.Log("Disable Login");
        //Globals.Config.password_normal = "";
        edbPassword.text = "";
    }
    public void onClickLogin()
    {
        var strAcc = edbUserName.text;
        var strPass = edbPassword.text;
        Globals.Config.username_normal = strAcc;
        Globals.Config.password_normal = strPass;

        if (strAcc.Equals("") | strPass.Equals(""))
        {
            return;
        }

        UIManager.instance.showWaiting();
        SocketSend.sendLogin(strAcc, strPass, false);
    }
}
