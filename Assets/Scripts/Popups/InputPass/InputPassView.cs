using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPassView : BaseView
{
    [SerializeField]
    TMP_InputField edbPass;
    int tableID = -1;

    [SerializeField] GameObject btnInput;

    bool isOne = true;
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
    protected override void OnEnable()
    {
        base.OnEnable();
        isOne = true;
    }
    public void setTableID(int _tableID){
        tableID = _tableID;
    }

    public void onClickConfirm() {
        SoundManager.instance.soundClick();
        string pass = edbPass.text;
        if (pass.Equals(""))
        {
            UIManager.instance.showMessageBox(Globals.Config.getTextConfig("error_empty"));
            return;
        }
        if (isOne)
        {
            isOne = false;
            SocketSend.sendJoinTableWithPass(tableID, pass);
        }
        hide();
    }

    private void OnDisable()
    {
        tableID = -1;
    }

    KeyboardController keyboardController;
    public void onClickInput()
    {
        edbPass.text = "";
        if (keyboardController != null)
        {
            keyboardController.setShow(true);
        }
        else
        {
            keyboardController = UIManager.instance.showKeyboardCustom(transform);
        }

        keyboardController.setTextAction(Globals.Config.getTextConfig("txt_done").ToUpper());
        keyboardController.setCharacterLimit(edbPass.characterLimit);
        keyboardController.addListernerCallback((str) =>
        {
            edbPass.text = str;
        }, (strIn) =>
        {
            edbPass.text = strIn;
        });
    }
}
