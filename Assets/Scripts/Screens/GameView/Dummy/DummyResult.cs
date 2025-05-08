using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class DummyResult : BaseView
{
    //mask: cc.Node,
    //    bg: cc.Node,
    //    listItem: cc.Node,
    //    itemInfo: cc.Node,
    [SerializeField]
    ScrollRect listItem;
    [SerializeField]
    GameObject itemInfo;

    public void onShow(JArray data)
    {
        try
        {
            for (var i = 0; i < data.Count; i++)
            {
                DummyItemResult item;
                if (i < listItem.content.childCount)
                {
                    item = listItem.content.GetChild(i).GetComponent<DummyItemResult>();
                }
                else
                {
                    item = Instantiate(itemInfo, listItem.content).GetComponent<DummyItemResult>();

                }
                item.gameObject.SetActive(true);
                item.setData((JObject)data[i]);
            }
            for (var i = data.Count; i < listItem.content.childCount; i++)
            {
                listItem.content.GetChild(i).gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Globals.Logging.LogException(e);
        }

        show();
    }
}
