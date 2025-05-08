using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiftCodeView : BaseView
{
    public static GiftCodeView instance;
    [SerializeField]
    TMP_InputField edbGiftcode;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        GiftCodeView.instance = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onClickConfirm()
    {
        SoundManager.instance.soundClick();
        string giftcode = edbGiftcode.text;
        if (giftcode.Equals(""))
        {
            UIManager.instance.showToast(Globals.Config.getTextConfig("txt_empty_noti"));
            return;
        }
        SocketSend.sendGiftCode(giftcode);
        
    }
}
