using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardController : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransformBkg;

    [SerializeField]
    TextMeshProUGUI textDisplay, txtAction;

    System.Action<string> actionDoneCallback;
    System.Action<string> actionInputCallback;
    int characterLimit = -1;
    public void OnEnable()
    {
        textDisplay.text = "";
    }
    public void setTextAction(string text)
    {
        txtAction.text = text;
    }
    public void setCharacterLimit(int limit)
    {
        characterLimit = limit;
    }
    public void onClickButton(string eventData)
    {
        var str = textDisplay.text;
        if (eventData == "done")
        {
            if(actionDoneCallback != null)
                actionDoneCallback.Invoke(str);
            setShow(false);
        }
        else if (eventData == "del")
        {
            Globals.Logging.Log(str);
            if(str.Length > 0)
            {
                str = str.Remove(str.Length - 1);
            }
            textDisplay.text = str;
            if (actionInputCallback != null)
                actionInputCallback.Invoke(str);
        }
        else
        {
            if (characterLimit > 0 && str.Length >= characterLimit)
            {
                return;
            }
            str += eventData;
            textDisplay.text = str;
            if (actionInputCallback != null)
                actionInputCallback.Invoke(str);
        }
    }

    public void addListernerCallback(System.Action<string> _doneCallback, System.Action<string> _inputCallback)
    {
        actionDoneCallback = _doneCallback;
        actionInputCallback = _inputCallback;
    }

    public void setShow(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void setPortrait(bool isPortrait)
    {
        if (isPortrait)
        {

            var rota = rectTransformBkg.rotation;
            rota.z = 0;
            rectTransformBkg.rotation = rota;
        }
    }
}
