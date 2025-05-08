using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInvite : MonoBehaviour
{
    [SerializeField]
    Avatar avatar;
    [SerializeField]
    TextMeshProUGUI txtName, txtMoney;
    System.Action callback;
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void setInfo(JObject itData, System.Action _callback)
    {
        callback = _callback;

        //    //var item = new FriendData();
        //    //item.name = data[i].N;
        //    //item.idFriend = data[i].Id;
        //    //item.idAva = data[i].Avatar;
        //    //item.agFriend = data[i].AG;
        //    //item.vip = data[i].V;
        //    //this.data_list.push(item);

        var idAva = (int)itData["Avatar"];
        string fid = "";
        if (itData.ContainsKey("fid"))
        {
            fid = (string)itData["fid"];
        }

        avatar.loadAvatar(idAva, (string)itData["N"], fid);

        //avatar.loadAvatar(idAva, Globals.User.userMain.Username, Globals.User.FacebookID);
        txtName.text = (string)itData["N"];
        txtMoney.text = Globals.Config.FormatMoney((int)itData["AG"]);
    }


    public void onClickInvite()
    {
        callback.Invoke();
    }
}
