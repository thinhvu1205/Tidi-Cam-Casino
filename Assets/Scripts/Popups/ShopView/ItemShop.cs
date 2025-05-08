using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemShop : MonoBehaviour
{
    JObject dataItem;
    //ShopView shopView;
    System.Action callback;

    [SerializeField]
    Image imgBkg;

    [SerializeField]
    TextMeshProUGUI txtPrice, txtRate, txtBonus, txtAmout;

    [SerializeField]
    Image imgIcon;
    [SerializeField]
    List<Sprite> listSfIcon;


    [SerializeField]
    GameObject line;

    public void setInfo(JObject _dataItem, int index, System.Action _callback, bool isEnd)
    {
        //shopView = _shopView;
        callback = _callback;
        dataItem = _dataItem;

        var bonus = (string)_dataItem["txtBonus"];
        txtPrice.text = (string)_dataItem["txtChip"];
        txtRate.text = (string)_dataItem["txtPromo"];
        txtBonus.text = bonus.Equals("0%") ? "" : bonus;
        txtBonus.transform.gameObject.SetActive(!txtBonus.text.Equals(""));

        var txtBuy = (string)_dataItem["txtBuy"];
       if (txtBuy.Contains("USD"))
        {
            txtAmout.text = Globals.Config.convertStringToNumber(txtBuy).ToString().Replace(",", ".") + "$";
        }
        else
        {
            txtAmout.text = txtBuy;
        }
        //txtAmout.text = (string)_dataItem["txtBuy"];

        if (index > listSfIcon.Count - 1)
        {
            imgIcon.sprite = listSfIcon[listSfIcon.Count - 1];
        }
        else
        {
            imgIcon.sprite = listSfIcon[index];
        }
        imgIcon.SetNativeSize();
        line.SetActive(!isEnd);
    }

    public void onClickBuy()
    {
        //shopView.onBuy(dataItem);
        SoundManager.instance.soundClick();
        callback();
    }
}
