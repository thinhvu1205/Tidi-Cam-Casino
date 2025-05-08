using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using System;
using Globals;

public class LoginView : BaseView
{
    [SerializeField] Transform m_InputTf;
    [SerializeField] TMP_InputField m_AccountTMPIF, m_PasswordTMPIF, m_IpTMPIF;
    [SerializeField] GameObject m_CheckTest, m_ButtonLogin, m_ButtonCreateAccount, m_ButtonPlayGuest;
    public string accPlayNow = "";
    public string passPlayNow = "";
    bool isLoginingFB = false;

    protected override void Start()
    {
        base.Start();
        if (!Config.username_normal.Equals("")) m_AccountTMPIF.text = Config.username_normal;
        // if (!FB.IsInitialized) FB.Init(InitCallback, OnHideUnity); // Initialize the Facebook SDK
        // else FB.ActivateApp(); // Already initialized, signal an app activation App Event
        StartCoroutine(login());
        IEnumerator login()
        {
            while (Config.curServerIp.Equals("")) yield return new WaitForSeconds(1f);
            if (Config.typeLogin == LOGIN_TYPE.PLAYNOW || Config.typeLogin == LOGIN_TYPE.NONE)
                UIManager.instance.loginView.onClickPlayNow();
            else if (Config.typeLogin == LOGIN_TYPE.NORMAL)
                UIManager.instance.loginView.reconnect();
        }
    }
    public bool IsInputsClear() { return m_AccountTMPIF.text.Equals(""); }
    public void reconnect()
    {
        UIManager.instance.showWaiting();
        switch (Globals.Config.typeLogin)
        {
            case Globals.LOGIN_TYPE.NORMAL:
                {
                    SocketSend.sendLogin(Globals.Config.user_name, Globals.Config.user_pass, false);
                    break;
                }
            case Globals.LOGIN_TYPE.FACEBOOK:
                {
                    onClickLoginFB();
                    //SocketSend.sendLogin("", aToken.TokenString, false);
                    break;
                }
            case Globals.LOGIN_TYPE.PLAYNOW:
                {
                    SocketSend.onPlayNow();
                    break;
                }
            default:
                {
                    Globals.Logging.Log("dclm Xem lai di nhe !!!");
                    break;
                }
        }

    }
    protected override void OnEnable()
    {
        Globals.Config.arrBannerLobby.Clear();
        Globals.Config.arrOnlistTrue.Clear();
        // if (UIManager.instance == null)
        // {
        //     LobbyView lobbyView = transform.parent.Find("LobbyView").GetComponent<LobbyView>();
        //     lobbyView.resetLogout();
        // }
        // else
        // {
        //     UIManager.instance.lobbyView.resetLogout();
        // }

        Globals.Config.invitePlayGame = true;
        Globals.Config.isLoginSuccess = false;
        SoundManager.instance.pauseMusic();
        Globals.Config.getDataUser();

        if (Globals.Config.typeLogin == Globals.LOGIN_TYPE.NORMAL)
        {
            //ipfAcc.text = Globals.Config.username_normal;
            //ipfPass.text = Globals.Config.password_normal;
            if (!Globals.Config.username_normal.Equals("")) m_AccountTMPIF.text = Globals.Config.username_normal;
            //if (Globals.Config.password_normal != "")
            //ipfPass.text = Globals.Config.password_normal;
        }
        //Globals.Config.typeLogin = Globals.LOGIN_TYPE.NONE;
        //PlayerPrefs.SetInt("type_login",(int)Globals.LOGIN_TYPE.NONE);
        //PlayerPrefs.Save();
        if (UIManager.instance != null)
            UIManager.instance.destroyAllPopup();
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.LOGIN_VIEW);
        checkTest();
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
            if (isLoginingFB)
            {
                onClickLoginFB();
            }
        }
        else
        {
            Globals.Logging.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;

        }
    }

    //List<int> listTest = new List<int>() { 1, 22, 13, 4, 51, 26, 7,48, 39 };


    public void onClickLoginFB()
    {
        SoundManager.instance.soundClick();
        isLoginingFB = true;
        if (!FB.IsInitialized)
        {
            Debug.Log("-=- !FB.IsInitialized");
            return;
        }
        Globals.Config.typeLogin = Globals.LOGIN_TYPE.FACEBOOK;
        if (FB.IsLoggedIn)
        {
            try
            {
                if (checkTokenExpirated())
                {
                    var aToken = AccessToken.CurrentAccessToken;


                    Globals.User.AccessToken = aToken.TokenString;
                    Globals.User.FacebookID = aToken.UserId;

                    // Print current access token's User ID
                    Globals.Logging.Log("token:  " + aToken.TokenString + " UserId: " + Globals.User.FacebookID);
                    SocketSend.sendLogin("", aToken.TokenString, false);
                }
                else
                {
                    var perms = new List<string>() { "public_profile" };
                    FB.LogInWithReadPermissions(perms, AuthCallback);
                }
            }
            catch (Exception e)
            {
                Debug.Log("errorFB=" + e);
            }

        }
        else
        {
            var perms = new List<string>() { "public_profile" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        isLoginingFB = false;
    }

    bool checkTokenExpirated()
    {
        var timeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var timeEnd = new DateTimeOffset(AccessToken.CurrentAccessToken.ExpirationTime).ToUnixTimeMilliseconds();
        return timeEnd >= timeNow;
    }
    public void onClickLoginToken()
    {
        isLoginingFB = true;
        if (!FB.IsInitialized)
        {
            Debug.Log("-=- !FB.IsInitialized");
            return;
        }
        Globals.Config.typeLogin = Globals.LOGIN_TYPE.FACEBOOK;
        Globals.User.AccessToken = "GGQVlZANVVFYzVPYUdrcEJES01SSjF2MHgyQUNrNmtvTUp5eHFycTd2dy1tR1VyeHQ4SEZAjaGN2OHB5dkprOFdicU8zbDZAITXVoTmNQVmxFZAlduR1dKZAnZAXMG05SDVtMFhsUGQ0ZAFR4MU5ZAQUFFaVl3SWJvZAU9zOFpNcWdCbTdFNnQwQTZAJU3pCRngwNXNqaHpuS2tPUS1B";

        // Print current access token's User ID
        //Globals.Logging.Log("token:  " + aToken.TokenString + " UserId: " + aToken.UserId);
        SocketSend.sendLogin("", Globals.User.AccessToken, false);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            Globals.Logging.Log("Error: " + result.Error);
        }
        else if (result.Cancelled)
        {
            Globals.Logging.Log("Cancelled: Access Token could not be retrieved");
        }
        else
        {
            // Successfully logged user in
            // A popup notification will appear that says "Logged in as <User Name>"
            Globals.Logging.Log("Success: " + result.AccessToken.UserId);
            //fbid = result.AccessToken.UserId;
            Globals.User.FacebookID = result.AccessToken.UserId;
            Globals.Logging.Log("FacebookID: " + Globals.User.FacebookID);
        }
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;
            Globals.User.AccessToken = aToken.TokenString;

            // Print current access token's User ID
            Globals.Logging.Log("token:  " + aToken.TokenString + " UserId: " + aToken.UserId);

            SocketSend.sendLogin("", aToken.TokenString, false);
        }
        else
        {
            Globals.Logging.Log("User cancelled login");
        }
    }

    public void onClickPlayNow()
    {
        // PlayerPrefs.SetInt("isFirstOpen", 1);
        // PlayerPrefs.Save();
        Globals.Config.typeLogin = Globals.LOGIN_TYPE.PLAYNOW;
        SoundManager.instance.soundClick();

        UIManager.instance.showWaiting();

        //PlayerPrefs.DeleteKey("USER_PLAYNOW");
        //PlayerPrefs.DeleteKey("PASS_PLAYNOW");
        SocketSend.onPlayNow();
    }

    public void onClickLogin()
    {
        SoundManager.instance.soundClick();
        var strAcc = m_AccountTMPIF.text;
        var strPass = m_PasswordTMPIF.text;
        if (strAcc.Equals("") || strPass.Equals(""))
        {
            UIManager.instance.showToast("Username and Password must not be empty");
            return;
        }
        Globals.Config.user_name = strAcc;
        Globals.Config.user_pass = strPass;

        UIManager.instance.showWaiting();
        Globals.Config.typeLogin = Globals.LOGIN_TYPE.NORMAL;
        SocketSend.sendLogin(strAcc, strPass, false);
    }
    public void onClickRegister()
    {
        SoundManager.instance.soundClick();
        var strAcc = m_AccountTMPIF.text;
        var strPass = m_PasswordTMPIF.text;
        if (strAcc.Equals("") || strPass.Equals(""))
        {
            UIManager.instance.showToast("Username and Password must not be empty!");
            return;
        }
        Globals.Config.user_name = strAcc;
        Globals.Config.user_pass = strPass;
        UIManager.instance.showWaiting();
        Globals.Config.typeLogin = Globals.LOGIN_TYPE.NORMAL;
        SocketSend.sendLogin(strAcc, strPass, true);
    }

    void checkTest()
    {
        Globals.Config.isSvTest = PlayerPrefs.GetInt("is_sv_test", 0) == 1;
        m_CheckTest.SetActive(Globals.Config.isSvTest);
    }

    public void onClickTest()
    {
        m_CheckTest.SetActive(!m_CheckTest.activeSelf);
        Globals.Config.isSvTest = m_CheckTest.activeSelf;
        PlayerPrefs.SetInt("is_sv_test", Globals.Config.isSvTest ? 1 : 0);
    }

    public void onClickSet()
    {
        //http://192.168.1.132:8080
        if (m_IpTMPIF.text.Equals("")) return;
        Globals.Config.url_log = "http://192.168.1." + m_IpTMPIF.text + ":3000";
    }
}
