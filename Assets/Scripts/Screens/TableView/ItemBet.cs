using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ItemBet : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtBet, txtUser;

    [SerializeField]
    Image bkg;
    [SerializeField]
    List<Sprite> lsBkg;
    JObject dataItem;
    [SerializeField]
    protected Material Mat_yellow, Mat_green, Mat_gray;

    public void setInfo(JObject _dataItem, int ndex)
    {
        gameObject.name = "" + (int)_dataItem["mark"];
        dataItem = _dataItem;
        //{\"mark\":100,\"ag\":2000,\"agPn\":0,\"agD\":0,\"minAgCon\":2000,\"maxAgCon\":0,\"currplay\":0,\"room\":0,\"minChipbanker\":0,\"maxBet\":0,\"agLeft\":1500,\"agRaiseFee\":2000,\"fee\":1.5}

        //Globals.Logging.Log("-=- mark  " + (int)_dataItem["mark"]);
        txtBet.text = Globals.Config.FormatMoney((int)_dataItem["mark"], true);
        txtUser.text = Globals.Config.FormatNumber((int)_dataItem["currplay"]);
        bkg.sprite = lsBkg[ndex % 2];
        Color colorLine = new Color();
        if (_dataItem.ContainsKey("minAgCon"))
        {
            if (Globals.User.userMain.AG >= (int)_dataItem["minAgCon"])
            {
                //bkg.sprite = lsBkg[1];
                bkg.color = Color.white;
                txtBet.color = Color.white;

                //ColorUtility.TryParseHtmlString("#F2A433", out colorLine);
                //txtBet.GetComponent<Outline>().effectColor = colorLine;
                //txtBet.outlineColor = colorLine;
                // txtBet.fontMaterial = ndex%2==0? Mat_green: Mat_yellow;
            }
            else
            {
                bkg.sprite = lsBkg[2];

                //bkg.color = Color.gray;
                //txtBet.color = Color.gray;
                //ColorUtility.TryParseHtmlString("#986C2C", out colorLine);
                //txtBet.GetComponent<Outline>().effectColor = colorLine;
                //txtBet.outlineColor = colorLine;
                // txtBet.fontMaterial = Mat_gray;
            }
        }

    }

    public void onClick()
    {
        if (!TableView.instance.isSelectGame)
        {
            TableView.instance.isSelectGame = true;
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                TableView.instance.isSelectGame = false;
            });
            if (Globals.User.userMain.AG <= 0 || Globals.User.userMain.AG < (int)dataItem["minAgCon"])
            {
                UIManager.instance.showPopupWhenNotEnoughChip();
                return;
            }
            SocketSend.sendChangeTable((int)dataItem["mark"], 0);
        }
        else
        {
            Debug.Log("Spam");
        }
    }
}
