using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeNameView : BaseView
{
    public static ChangeNameView intance;
    //InputField edb_username, edb_password, edb_retype;
    [SerializeField] TMP_InputField edb_username, edb_password, edb_retype;
    [SerializeField] private GameObject m_ChangePass, m_Register;

    [SerializeField]
    TextMeshProUGUI lb_Confirm, lbBonus;

    [SerializeField]
    GameObject tagBonus;

    [SerializeField]
    Button btn_confirm;

    protected override void Awake()
    {
        intance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        setInfo();
    }
    void setInfo()
    {
        edb_username.text = "";
        edb_password.text = "";
        edb_retype.text = "";
        btn_confirm.interactable = true;

        if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.NORMAL)
        {
            m_ChangePass.SetActive(true);
            m_Register.SetActive(false);
            tagBonus.SetActive(false);
            edb_password.gameObject.SetActive(true);
            edb_retype.gameObject.SetActive(true);

            edb_username.contentType = TMP_InputField.ContentType.Password;
            edb_password.contentType = TMP_InputField.ContentType.Password;
            edb_retype.contentType = TMP_InputField.ContentType.Password;

            edb_username.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("old_pw");
            edb_password.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("new_pw");
            edb_retype.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_re_pass");
            lb_Confirm.text = Globals.Config.getTextConfig("ok");
        }
        else if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.FACEBOOK || Globals.Config.typeLogin == Globals.LOGIN_TYPE.APPLE_ID || Globals.Config.typeLogin == Globals.LOGIN_TYPE.FACEBOOK_INSTANT)
        {
            m_ChangePass.SetActive(true);
            m_Register.SetActive(false);
            //tagBonus.SetActive(Globals.Config.agRename>0);
            tagBonus.SetActive(false);
            Globals.Logging.Log("vao day la doi ten");
            lbBonus.text = "+" + Globals.Config.FormatNumber(Globals.Config.agRename);

            edb_password.gameObject.SetActive(false);
            edb_retype.gameObject.SetActive(false);
            edb_username.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_name");

            edb_username.contentType = TMP_InputField.ContentType.Standard;

            lb_Confirm.text = Globals.Config.getTextConfig("ok");
        }
        else if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW)
        {
            if (PlayerPrefs.GetInt("isReg", 0) == 0)
            {
                m_ChangePass.SetActive(false);
                m_Register.SetActive(true);
                tagBonus.SetActive(false);
                lbBonus.text = "+" + Globals.Config.FormatNumber(Globals.Config.agRename);

                edb_password.gameObject.SetActive(true);
                edb_retype.gameObject.SetActive(true);
                edb_username.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_name");
                edb_password.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_pass");
                edb_retype.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_re_pass");

                edb_username.contentType = TMP_InputField.ContentType.Standard;
                edb_password.contentType = TMP_InputField.ContentType.Password;
                edb_retype.contentType = TMP_InputField.ContentType.Password;

                lb_Confirm.text = Globals.Config.getTextConfig("register");
            }
            else
            {
                m_ChangePass.SetActive(true);
                m_Register.SetActive(false);
                tagBonus.SetActive(false);

                edb_password.gameObject.SetActive(true);
                edb_retype.gameObject.SetActive(true);
                edb_username.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("old_pw");
                edb_password.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("new_pw");
                edb_retype.placeholder.GetComponent<TextMeshProUGUI>().text = Globals.Config.getTextConfig("login_re_pass");

                edb_username.contentType = TMP_InputField.ContentType.Password;
                edb_password.contentType = TMP_InputField.ContentType.Password;
                edb_retype.contentType = TMP_InputField.ContentType.Password;

                lb_Confirm.text = Globals.Config.getTextConfig("ok");
            }
        }
    }



    public void onConfirm()
    {
        SoundManager.instance.soundClick();
        var txt_username = edb_username.text;
        var txt_password = edb_password.text;
        var txt_retype = edb_retype.text;

        Globals.Config.user_name_temp = txt_username;
        Globals.Config.user_pass_temp = txt_password;

        if ((edb_username.gameObject.activeInHierarchy && txt_username.Equals(""))
            || (edb_password.gameObject.activeInHierarchy && txt_password.Equals(""))
            || (edb_retype.gameObject.activeInHierarchy && txt_retype.Equals("")))
        {
            UIManager.instance.showMessageBox(Globals.Config.getTextConfig("txt_empty_noti"));
            return;
        }

        if (txt_username != "" && Globals.Config.typeLogin == Globals.LOGIN_TYPE.FACEBOOK || Globals.Config.typeLogin == Globals.LOGIN_TYPE.APPLE_ID || Globals.Config.typeLogin == Globals.LOGIN_TYPE.FACEBOOK_INSTANT)
        {
            SocketSend.sendChangeName(txt_username);
            //btn_confirm.interactable = false;
        }
        else if (txt_password != "" && txt_password == txt_retype && Globals.Config.typeLogin == Globals.LOGIN_TYPE.NORMAL)
        {

            if (txt_password == txt_username)
            {
                UIManager.instance.showMessageBox(Globals.Config.getTextConfig("enter_new_pass"));
                return;
            }

            //GameManager.getInstance().passwordToBeChanged = edb_password.string;
            SocketSend.changePassword(txt_username, txt_password);
            //btn_confirm.interactable = false;
        }
        else if (txt_password != "" && txt_password == txt_retype && Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW)
        {
            if (PlayerPrefs.GetInt("isReg", 0) == 0)
            {
                //GameManager.getInstance().userNameRegTempl = edb_username.string;
                //GameManager.getInstance().passRegTemple = edb_password.string;
                SocketSend.sendRegister(txt_username, txt_password, txt_retype);
                //btn_confirm.interactable = false;
            }
            else
            {

                if (txt_password == txt_username)
                {
                    UIManager.instance.showMessageBox(Globals.Config.getTextConfig("enter_new_pass"));
                    return;
                }

                //GameManager.getInstance().passwordToBeChanged = edb_password.string;
                SocketSend.changePassword(txt_username, txt_password);
                //btn_confirm.interactable = false;
            }
        }
        else if (txt_password != txt_retype && (Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW || Globals.Config.typeLogin == Globals.LOGIN_TYPE.NORMAL))
        {
            UIManager.instance.showMessageBox(Globals.Config.getTextConfig("reg_not_pass"));
        }
    }

    public void saveData()
    {

        if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW || Globals.Config.typeLogin == Globals.LOGIN_TYPE.NORMAL)
        {
            if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW)
            {
                if (edb_username.contentType != TMP_InputField.ContentType.Password)
                    PlayerPrefs.SetString("USER_PLAYNOW", edb_username.text);
                PlayerPrefs.SetString("PASS_PLAYNOW", edb_password.text);
            }
            else
            {

                PlayerPrefs.SetString("user_pass", edb_password.text);
            }
            PlayerPrefs.Save();
        }
    }
}
