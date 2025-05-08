using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemEx : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtChip, txtPrize;
    System.Action callback;
    public void setInfo(JObject dt, System.Action _callback)
    {
        //      {
        //          "ag": 1000000,
        //  "m": 50
        //},
        callback = _callback;
        txtChip.text = Globals.Config.FormatNumber((int)dt["ag"]);
        txtPrize.text = Globals.Config.FormatNumber((int)dt["m"]);
    }

    public void onClickConfirm()
    {
        SoundManager.instance.soundClick();
        callback.Invoke();
    }
}
