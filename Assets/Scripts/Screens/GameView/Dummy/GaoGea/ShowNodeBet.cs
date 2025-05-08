using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowNodeBet : MonoBehaviour
{
    [SerializeField]
    GameObject btn_Fold;

    [SerializeField]
    TextMeshProUGUI textBtn_Fold;

    [SerializeField]
    TextMeshProUGUI textCurrentChip;

    [SerializeField]
    GameObject btn_Raise;

    [SerializeField]
    TextMeshProUGUI textBtn_Raise;

    [SerializeField]
    GameObject btn_Call;

    [SerializeField]
    TextMeshProUGUI textBtn_Call;

    [SerializeField]
    TextMeshProUGUI textBet;

    [SerializeField]
    Image thanhKeo;

    [SerializeField]
    GameObject btn_Allin_Call;

    [SerializeField]
    List<Button> listButton;

    [SerializeField]
    Slider handleSlider;


    private float valueBet = 0, valueTableAg = 500, valuePot = 0;
    private float valueThisPlayer = 500f;
    private float timeCountDow = 0;
    private bool isClickRaise = false, isCountDow;

    // Start is called before the first frame update

    void Start()
    {
        //handleSlider.onValueChanged.AddListener((vl) =>
        //{
        //    CallBackSilder();
        //});

    }
    private void Update()
    {
        if (isCountDow)
        {
            timeCountDow -= Time.deltaTime;
            if (timeCountDow < 0.5 && isClickRaise)
            {
                btn_Raise.gameObject.SetActive(true);
                if (textBtn_Call.text == Globals.Config.getTextConfig("show_lb_check"))
                {// GameManager.getInstance().getTextConfig("show_lb_check"))
                    SocketSend.sendMakeBetShow("check");
                }
                else
                {
                    SocketSend.sendMakeBetShow("fold");
                }
                isCountDow = false;
                isClickRaise = false;
                gameObject.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        handleSlider.transform.parent.gameObject.SetActive(false);

    }
    // Update is called once per frame
    public void update_slider(int value)
    {
        Globals.Logging.Log("update_slider" + value);
        valueThisPlayer = value;
        resetSlider();
    }
    public void CallBackSilder()
    {
        float valueMoney = 0;
        float _progress = handleSlider.value;

        if (_progress <= 0.7f)
        {

            valueMoney = Mathf.FloorToInt((_progress * valueThisPlayer / 2) * (1 / 0.7f));
            if (_progress <= valueTableAg / valueThisPlayer / 0.7)
            {
                handleSlider.value = valueTableAg / valueThisPlayer / 0.7f;
            }
            Globals.Logging.Log("_progress <= 0.7f valueMoney:" + valueMoney);
        }
        else
        {
            valueMoney = Mathf.FloorToInt(valueThisPlayer / 2 + ((_progress - 0.7f) * valueThisPlayer / 2 * (1 / 0.3f)));
            if (_progress >= 0.99)
            {
                handleSlider.value = 1;
            }
        }
        if (valueMoney <= valueTableAg)
        {
            valueMoney = valueTableAg;
            textBet.text = Globals.Config.FormatMoney((int)valueTableAg);
        }
        else if (valueMoney < valueThisPlayer)
        {
            textBet.text = Globals.Config.FormatMoney((int)valueMoney);
        }
        else
        {
            textBet.text = Globals.Config.FormatMoney((int)Mathf.Floor(valueThisPlayer));
            valueMoney = valueThisPlayer;
        }
        valueBet = valueMoney;

    }
    public void setValueInfo(long valueAgPlayer, int agToCall, int valueTableAgg, int valuePott)
    {
        valueThisPlayer = valueAgPlayer;
        valueTableAg = valueTableAgg;
        valuePot = valuePott;
        valueBet = valueTableAg;
        textBet.text = Globals.Config.FormatMoney((int)valueTableAg);
        if (agToCall >= valueAgPlayer)
        {
            btn_Allin_Call.gameObject.SetActive(true);
            btn_Raise.gameObject.SetActive(false);
            btn_Call.gameObject.SetActive(false);
        }
        else
        {
            btn_Raise.gameObject.SetActive(true);
            btn_Call.gameObject.SetActive(true);
            btn_Allin_Call.gameObject.SetActive(false);
        }

    }
    public void setInfoBtn(string btn_2, int amount = 0)
    {
        if (btn_2 == "Call")
        {
            string str = Globals.Config.FormatMoney(amount);
            textBtn_Call.text = Globals.Config.getTextConfig("show_lb_call") + "(" + str + ")";//GameManager.getInstance().getTextConfig("show_lb_call") + "(" + str + ")"
        }
        else
        {
            textBtn_Call.text = Globals.Config.getTextConfig("show_lb_check");// GameManager.getInstance().getTextConfig("show_lb_check")
        }
    }
    public void onClickRaise()
    {
        // btn_Raise.active = false;
        SoundManager.instance.soundClick();
        isClickRaise = true;
        handleSlider.transform.parent.gameObject.SetActive(true);
      
        resetSlider();
    }
    public void offRaise()
    {
        btn_Raise.gameObject.SetActive(true);
        isClickRaise = false;
        handleSlider.transform.parent.gameObject.SetActive(false);
        resetSlider();
    }
    public void onClickComfirm()
    {
        float _progress = handleSlider.value;
        Globals.Logging.Log("_progress:" + _progress);
        Globals.Logging.Log("valueTableAg:" + valueTableAg);
        Globals.Logging.Log("valueThisPlayer:" + valueThisPlayer);
        SoundManager.instance.soundClick();
        SocketSend.sendMakeBetShow("raise", (int)valueBet);
        btn_Raise.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    public void onClickCall()
    {
        SoundManager.instance.soundClick();
        if (textBtn_Call.text == Globals.Config.getTextConfig("show_lb_check"))
        {// GameManager.getInstance().getTextConfig("show_lb_check")) {
            SocketSend.sendMakeBetShow("check");
        }
        else
        {
            SocketSend.sendMakeBetShow("call");
        }
        gameObject.SetActive(false);
    }
    public void onClickBtnAllIn()
    {
        SoundManager.instance.soundClick();
        btn_Raise.gameObject.SetActive(true);
        SocketSend.sendMakeBetShow("raise", (int)valueThisPlayer);
        gameObject.SetActive(false);
    }
    private void resetSlider()
    {

        handleSlider.value = 0;
        textBet.text = Globals.Config.FormatMoney((int)valueBet);
        List<float> arrbet = new List<float> { 0.5f, 1.0f, 2.0f, 5.0f };
        for (int index = 0; index < arrbet.Count; index++)
        {
            float cBet = valuePot * arrbet[index];
            if (cBet < valueTableAg)
            {
                cBet = valueTableAg;
            }
            listButton[index].interactable = valueThisPlayer > cBet;
        }
    }
    public void onClickBtnAllinForCall()
    {
        SoundManager.instance.soundClick();
        SocketSend.sendMakeBetShow("call");
        gameObject.SetActive(false);
    }
    public void AutoBetIfClickRaise(int time)
    {
        timeCountDow = time;
        isCountDow = true;
    }
    public void SetFalseIsCountDown()
    {
        timeCountDow = 0;
        isCountDow = false;
        isClickRaise = false;
    }
    public void onClickFold()
    {
        SoundManager.instance.soundClick();
        SocketSend.sendMakeBetShow("fold");
        gameObject.SetActive(false);
    }
    public void x1per2Click()
    {
        SoundManager.instance.soundClick();
        float cBet = (valuePot / 2.0f);

        if (cBet < valueTableAg)
        {
            cBet = valueTableAg;
        }
        btn_Raise.gameObject.SetActive(true);
        if (valueThisPlayer > cBet)
        {
            SocketSend.sendMakeBetShow("raise", Mathf.FloorToInt(cBet));
        }
        else
        {
            SocketSend.sendMakeBetShow("raise", (int)valueThisPlayer);
        }
        gameObject.SetActive(false);
    }
    public void x1Click()
    {
        SoundManager.instance.soundClick();
        var cBet = valuePot;

        if (cBet < valueTableAg)
        {
            cBet = valueTableAg;
        }
        btn_Raise.gameObject.SetActive(true);
        if (valueThisPlayer > cBet)
        {
            SocketSend.sendMakeBetShow("raise", Mathf.FloorToInt(cBet));
        }
        else
        {
            SocketSend.sendMakeBetShow("raise", (int)valueThisPlayer);
        }
        gameObject.SetActive(false);
    }
    public void x2Click()
    {
        SoundManager.instance.soundClick();
        var cBet = (valuePot * 2);

        if (cBet < valueTableAg)
        {
            cBet = valueTableAg;
        }
        btn_Raise.gameObject.SetActive(true);
        if (valueThisPlayer > cBet)
        {
            SocketSend.sendMakeBetShow("raise", Mathf.FloorToInt(cBet));
        }
        else
        {
            SocketSend.sendMakeBetShow("raise", (int)valueThisPlayer);
        }
        gameObject.SetActive(false);
    }
    public void x5Click()
    {
        SoundManager.instance.soundClick();
        var cBet = valuePot * 5;

        if (cBet < valueTableAg)
        {
            cBet = valueTableAg;
        }
        gameObject.SetActive(false);
        if (valueThisPlayer > cBet)
        {
            SocketSend.sendMakeBetShow("raise", Mathf.FloorToInt(cBet));
        }
        else
        {
            SocketSend.sendMakeBetShow("raise", (int)valueThisPlayer);
        }
        gameObject.SetActive(false);
    }
}
