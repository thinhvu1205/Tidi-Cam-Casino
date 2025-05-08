using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class DialogView : BaseView
{
    [SerializeField]
    GameObject btn_1;
    [SerializeField]
    GameObject btn_2;

    [SerializeField]
    GameObject btn_close;


    [SerializeField]
    TextMeshProUGUI lb_content;

    System.Action callback1 = null;
    System.Action callback2 = null;
    System.Action callbackClose = null;


    protected override void OnEnable()
    {
        base.OnEnable();
        if (TableView.instance != null && TableView.instance.isHorizontal)
        {
            background.transform.localEulerAngles = new Vector3(0, 0, 270);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            background.transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void setMessage(string msg)
    {
        lb_content.text = msg;
    }

    public string getMessage()
    {
        return lb_content.text;
    }
    private void OnDisable()
    {
        if (!UIManager.instance.dialogPool.Contains(this))
            UIManager.instance.dialogPool.Add(this);

        UIManager.instance.listDialogOne.Remove(this);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (UIManager.instance.dialogPool.Contains(this))
            UIManager.instance.dialogPool.Remove(this);

        UIManager.instance.listDialogOne.Remove(this);
    }
    public void setIsShowButton1(bool IsShow, string nameBtn = "", System.Action cb = null)
    {
        btn_1.SetActive(IsShow);
        callback1 = cb;
        if (!IsShow) return; ;
        if (nameBtn != "")
            btn_1.GetComponentInChildren<TextMeshProUGUI>().text = nameBtn;
    }

    public void setIsShowButton2(bool IsShow, string nameBtn = "", System.Action cb = null)
    {
        btn_2.SetActive(IsShow);
        callback2 = cb;
        if (!IsShow) return; ;
        if (nameBtn != "")
            btn_2.GetComponentInChildren<TextMeshProUGUI>().text = nameBtn;
    }

    public void setIsShowClose(bool isShowClose, System.Action _callbackClose = null)
    {
        btn_close.SetActive(isShowClose);
        callbackClose = _callbackClose;
    }

    public void onClick1()
    {
        SoundManager.instance.soundClick();
        hide(false, callback1);
    }

    public void onClick2()
    {
        SoundManager.instance.soundClick();
        hide(false, callback2);
    }

    public void setLanscape()
    {
        var mask = transform.Find("mask");
        var sizz = mask.GetComponent<RectTransform>().sizeDelta;
        sizz.x = Screen.height;
        sizz.y = Screen.width;
        mask.GetComponent<RectTransform>().sizeDelta = sizz;
        background.transform.localEulerAngles = new Vector3(0, 0, 0);

    }
}
