using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleCheckShow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Button toggleShow;

    [SerializeField]
    List<Toggle> listToggle;

    [SerializeField]
    TextMeshProUGUI textToggleFold;

    [SerializeField]
    TextMeshProUGUI textToggleCall;

    [SerializeField]
    TextMeshProUGUI textToggleCallAny;

    public bool allowCheck = true;

    void Start()
    {

    }

    // Update is called once per frame
    public void resetStatusToggle()
    {
        for (int i = 0; i < listToggle.Count; i++)
        {
            listToggle[i].isOn = false;
        }
        allowCheck = true;
    }
    public void setStatus(bool isFold, bool isShow = false)
    {

        for (int i = 0; i < listToggle.Count; i++)
        {
            listToggle[i].gameObject.SetActive(!isFold);
        }
        toggleShow.gameObject.SetActive(isFold && !isShow);
    }
    public void setInfo(int chipForCall, int chipBoxBet = 0, long ChipPlayer = 0)
    {
        //Globals.Logging.Log("Toggle SetInfo:chipForCall=" + chipForCall);
        //Globals.Logging.Log("Toggle SetInfo:chipBoxBet=" + chipBoxBet);
        //Globals.Logging.Log("Toggle SetInfo:ChipPlayer=" + ChipPlayer);
        gameObject.SetActive(ChipPlayer > 0);
        setStatus(false);
        int temp = chipBoxBet;
        if (chipForCall <= 0 || chipForCall == temp)
        {
            textToggleFold.text = Globals.Config.getTextConfig("show_lb_fold_check");
            textToggleCall.text = Globals.Config.getTextConfig("show_lb_call");
        }
        else
        {
            listToggle[1].isOn = (listToggle[1].isOn && textToggleCall.text == Globals.Config.getTextConfig("show_lb_call") + "(" + Globals.Config.FormatMoney(chipForCall - temp) + ")");
            textToggleFold.text = Globals.Config.getTextConfig("show_lb_fold");
            textToggleCall.text = Globals.Config.formatStr("%s(%s)", Globals.Config.getTextConfig("show_lb_call"), Globals.Config.FormatMoney(chipForCall - temp));
        };
        textToggleCallAny.text = Globals.Config.getTextConfig("show_lb_call_any");
        if (chipForCall >= ChipPlayer + temp)
        {
            listToggle[1].gameObject.SetActive(false);
            textToggleCallAny.text = Globals.Config.getTextConfig("show_lb_allin");//require('GameManager').getInstance().getTextConfig("show_lb_allin");
        }
        else
        {
            listToggle[1].gameObject.SetActive(true);
        }
    }
    public bool readInfoToggle()
    {
        for (int i = 0; i < listToggle.Count; i++)
        {
            if (listToggle[i].isOn)
            {
                if (i == 0)
                {
                    if (textToggleFold.text == Globals.Config.getTextConfig("show_lb_fold") || allowCheck == false)
                    {
                        Debug.Log("readInfoToggle--->1");
                        SocketSend.sendMakeBetShow("fold");
                        setStatus(true);
                    }
                    else
                    {
                        Debug.Log("readInfoToggle--->2");
                        SocketSend.sendMakeBetShow("check");
                    }
                }
                else
                {
                    if (textToggleCallAny.text == Globals.Config.getTextConfig("show_lb_allin"))
                    {
                        Debug.Log("readInfoToggle--->3");
                        SocketSend.sendMakeBetShow("call");
                    }
                    else
                    {
                        if (textToggleCall.text == Globals.Config.getTextConfig("show_lb_call") && allowCheck == true)
                        {//require('GameManager').getInstance().getTextConfig("show_lb_call")) {
                            Debug.Log("readInfoToggle--->4");
                            SocketSend.sendMakeBetShow("check");
                        }
                        else
                        {
                            Debug.Log("readInfoToggle--->5");
                            SocketSend.sendMakeBetShow("call");
                        }
                    }
                }

                listToggle[i].isOn = false;
                gameObject.SetActive(false);
                return true;
            }
        }
        gameObject.SetActive(false);
        return false;
    }
    public void onClickToggleShow()
    {
        SocketSend.sendMakeBetShow("show");
        toggleShow.gameObject.SetActive(false);
        SoundManager.instance.soundClick();
    }
    public void onClickToggleFold()
    {
        SoundManager.instance.soundClick();
        //require('SMLSocketIO').getInstance().emitSIOCCC(cc.js.formatStr("ClickToggleFold_%s", require('GameManager').getInstance().getCurrentSceneName()));
    }
    public void onClickToggleCall()
    {
        SoundManager.instance.soundClick();
        //require('SMLSocketIO').getInstance().emitSIOCCC(cc.js.formatStr("ClickToggleCall%s", require('GameManager').getInstance().getCurrentSceneName()));
    }
    public void onClickToggleCallAny()
    {
        SoundManager.instance.soundClick();
        //require('SMLSocketIO').getInstance().emitSIOCCC(cc.js.formatStr("ClickToggleCallAny%s", require('GameManager').getInstance().getCurrentSceneName()));
    }
}
