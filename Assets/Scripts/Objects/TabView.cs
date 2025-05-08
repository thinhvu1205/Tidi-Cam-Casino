using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txt_off, txt_on;
    [SerializeField]
    GameObject objOn;

    public JObject data;
    Action<TabView> callback;
    public void setInfo(string title, Action<TabView> _callback)
    {
        txt_off.text = title;
        txt_on.text = title;
        callback = _callback;
    }

    public void SetActiveTab(bool isActive)
    {
        objOn.SetActive(isActive);
    }

    public void OnClickTab()
    {
        if(callback != null)
        {
            callback.Invoke(this);
        }
    }
}
