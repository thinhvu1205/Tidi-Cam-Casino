using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class PopupFeedBack : BaseView
{
    [SerializeField]
    TMP_InputField tMP_Input;
    // Start is called before the first frame update
    protected override void Start()
    {
        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.FEEDBACK_VIEW);
    }


    public void onClickSend()
    {
        SoundManager.instance.soundClick();
        var str = tMP_Input.text;
        if(str != "")
        SocketSend.sendFeedback(str);
        this.hide();
    }
}
