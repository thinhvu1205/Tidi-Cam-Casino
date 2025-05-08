using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTable : MonoBehaviour
{
    [SerializeField]
    List<Image> listPlayer;

    [SerializeField]
    List<Sprite> listSpPlayer;

    [SerializeField]
    TextMeshProUGUI txtAg;
    [SerializeField]
    TextMeshProUGUI txtName;
    [SerializeField]
    TextMeshProUGUI txtID;
    //[SerializeField]
    //Button btnJoin;
    [SerializeField]
    GameObject objFull, m_ButtonJoin;
    System.Action callback;
    JObject dataItem;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setInfo(JObject _dataItem, System.Action _callback)
    {
        dataItem = _dataItem;
        callback = _callback;
        int sizeTable = (int)dataItem["size"];
        for (var i = 0; i < listPlayer.Count; i++)
        {
            listPlayer[i].sprite = i <= (int)dataItem["player"] - 1 ? listSpPlayer[1] : listSpPlayer[0];
            listPlayer[i].gameObject.SetActive(!(i >= sizeTable));
            listPlayer[i].SetNativeSize();
        }
        objFull.SetActive((int)dataItem["player"] == (int)dataItem["size"]);
        m_ButtonJoin.SetActive(!objFull.activeSelf);
        txtAg.text = Globals.Config.FormatMoney((long)dataItem["mark"], true);
        //txtName.text = (string)dataItem["N"];
        JArray arrN = (JArray)_dataItem["ArrName"];
        List<string> arrName = arrN.ToObject<List<string>>();
        string tableName = "";
        foreach (string name in arrName)
        {
            string tbName = name;
            if (name.Length > 10)
            {
                tbName = name.Substring(0, 7) + "..., ";
            }
            tableName += tbName;
        }
        txtName.text = tableName;
        txtID.text = (int)dataItem["id"] + "";
        //var gameId = Globals.Config.curGameId;
        //if (gameId == (int)Globals.GAMEID.TONGITS || gameId == (int)Globals.GAMEID.TONGITS_JOKER)
        //{
        //    txtAg.text = hitpot;
        //}
    }

    public void onClickJoin()
    {
        if (callback != null)
        {
            callback.Invoke();
        }
    }
}
