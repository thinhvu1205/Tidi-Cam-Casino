using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class TextNumberControl : MonoBehaviour
{
    // Start is called befo
    // re the first frame update
    public enum TYPE_FORMAT
    {
        NUMBER = 0,
        MONEY = 1,
        MONEY_FROM_K = 2,
        NUMBER_FROM_M = 3
    }
    [SerializeField] TextMeshProUGUI tmPro;
    [SerializeField] bool isFormatMoney = false;
    [SerializeField]
    public float scale = 1;
    [SerializeField]
    public TYPE_FORMAT FormatType = TYPE_FORMAT.NUMBER;

    private string text; // field
    public System.Action callback;

    public string GetTextNumber() { return tmPro.text; }
    public string Text   // property
    {
        get { return text; }   // get method
        set { setText(value); }  // set method
    }
    [HideInInspector]
    private long number = 0;
    private void Awake()
    {
        tmPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void ResetValue()
    {
        //number = 0;
        DOTween.Kill(tmPro.transform);
        tmPro.text = "0";
        text = "0";
        tmPro.transform.localScale = new Vector2(scale, scale);
        number = 0;
    }
    private void setText(string value)
    {
        text = value;
        tmPro.text = text;
        number = Globals.Config.splitToLong(value);
    }
    public void setValue(long value, bool isRun = false, float timeRun = 0.5f, string msg = "", System.Action cb = null)
    {
        callback = cb;
        long startNumber = number;
        if (msg != "")
        {
            Debug.Log(msg + value);
            Debug.Log(msg + "StartNUmber:" + startNumber);
        }
        if (value == number)
        {
            isRun = false;
        }
        if (isRun && value != number)
        {
            DOTween.To(() => startNumber, x => startNumber = x, value, timeRun)
                //.SetEase(Ease.InSine)
                .OnUpdate(() =>
                {
                    tmPro.text = formatValue(startNumber);
                })
                .OnComplete(() =>
                {
                    tmPro.text = formatValue(value);
                    number = value;

                    callback?.Invoke();
                    callback = null;
                })
                .SetId("tweenNumber");
            Vector2 normalScale = new Vector2(scale, scale);
            Vector2 biggerScale = new Vector2(scale + 0.2f, scale + 0.2f);
            DOTween.Kill(tmPro.transform);
            DOTween.Sequence()
            .Append(tmPro.transform.DOScale(biggerScale, timeRun * 0.45f))
            .AppendInterval(timeRun * 0.45f)
            .Append(tmPro.transform.DOScale(normalScale, timeRun * 0.1f)).SetId("tweenScale");
        }
        else
        {
            tmPro.text = Globals.Config.FormatNumber(value);
            number = value;
        }
        number = value;
    }
    private string formatValue(long number)
    {
        string valueStr = "0";
        if (FormatType == TYPE_FORMAT.NUMBER)
        {
            valueStr = Globals.Config.FormatNumber(number);
        }
        else if (FormatType == TYPE_FORMAT.MONEY)
        {
            valueStr = Globals.Config.FormatMoney(number);
        }
        else if (FormatType == TYPE_FORMAT.MONEY_FROM_K)
        {
            valueStr = Globals.Config.FormatMoney(number, true);
        }
        else if (FormatType == TYPE_FORMAT.NUMBER_FROM_M)
        {
            if (number > 1000000)
            {
                valueStr = Globals.Config.FormatMoney(number);
            }
            else
            {
                valueStr = Globals.Config.FormatNumber(number);
            }
        }
        return valueStr;
    }
    public void setLastValue()
    {
        if (callback != null)
        {
            callback();
            callback = null;
        }
        DOTween.Kill(tmPro.transform);
        DOTween.Kill("tweenNumber");
        DOTween.Kill("tweenScale");
        tmPro.text = Globals.Config.FormatNumber(number);
    }
}
