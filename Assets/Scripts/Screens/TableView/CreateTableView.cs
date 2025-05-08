using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class CreateTableView : BaseView
{
    // Start is called before the first frame update
    [SerializeField] TMP_InputField edbNameRoom, edbPassword;
    [SerializeField] Slider sliderRoom;
    [SerializeField] Button btnCreate;
    [SerializeField] TextMeshProUGUI lbValueRoom;
    //private List<int> listRoomValue = new List<int>();

    private int currentRoomValue = 0;

    [SerializeField] GameObject btnInput;
    protected override void Awake()
    {
        base.Awake();
        btnInput.SetActive(false);
        if (TableView.instance != null && TableView.instance.isHorizontal)
        {
            background.transform.localEulerAngles = new Vector3(0, 0, 270);
            btnInput.SetActive(true);
        }
    }
    protected override void Start()
    {
        base.Start();

        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.CREATE_TABLE_GAME);
        if (TableView.instance.listDataRoomBet.Count > 0)
        {
            sliderRoom.minValue = (float)1 / TableView.instance.listDataRoomBet.Count;
            currentRoomValue = (int)TableView.instance.listDataRoomBet[0]["mark"];
        }
    }

    // Update is called once per frame
    public void onClickCreate()
    {
        if (edbPassword.text == "")
        {
            SocketSend.sendCreateTable(currentRoomValue);
        }
        else
        {
            //SocketSend.sendCreateTable(currentRoomValue);
            SocketSend.sendCreateTableWithPass(currentRoomValue, "", edbPassword.text);
        }
        onClickClose();
    }

    public void handleSliderRoomBet()
    {
        for (int i = 0, l = TableView.instance.listDataRoomBet.Count; i < l; i++)
        {
            var _dataItem = TableView.instance.listDataRoomBet[i];
            if (sliderRoom.value >= (float)(i + 1.0f) / l)
            {
                if (Globals.User.userMain.AG >= (int)_dataItem["minAgCon"])
                {
                    btnCreate.interactable = true;
                }
                else
                {
                    btnCreate.interactable = false;
                }
                currentRoomValue = (int)_dataItem["mark"];
                lbValueRoom.text = Globals.Config.FormatMoney(currentRoomValue);

            }

        }

    }

    KeyboardController keyboardController;
    public void onClickInput()
    {
        edbPassword.text = "";
        if (keyboardController != null)
        {
            keyboardController.setShow(true);
        }
        else
        {
            keyboardController = UIManager.instance.showKeyboardCustom(transform);
        }

        keyboardController.setTextAction(Globals.Config.getTextConfig("txt_done").ToUpper());
        keyboardController.setCharacterLimit(edbPassword.characterLimit);
        keyboardController.addListernerCallback((str) =>
        {
            edbPassword.text = str;
        }, (strIn) =>
        {
            edbPassword.text = strIn;
        });
    }
}
