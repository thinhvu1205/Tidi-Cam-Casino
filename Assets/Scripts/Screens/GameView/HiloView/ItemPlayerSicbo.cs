using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;


public class ItemPlayerSicbo : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI lb_name;

    [SerializeField] TextMeshProUGUI lb_ag;


    [SerializeField] TextMeshProUGUI lb_id;

    [SerializeField] Avatar avatar;


    [SerializeField] VipContainer listIconVip;
    private Player _ThisPLayerP;

    public void setInfo(Player dataPlayer)
    {
        _ThisPLayerP = dataPlayer;
        Debug.Log("dataPlayer.avatar_id:" + dataPlayer.avatar_id);
        Debug.Log("dataPlayer.namePl:" + dataPlayer.namePl);
        Debug.Log("dataPlayer.fid:" + dataPlayer.fid);
        avatar.loadAvatar(dataPlayer.avatar_id, dataPlayer.namePl, dataPlayer.fid);
        avatar.setVip(dataPlayer.vip);
        listIconVip.setVip(dataPlayer.vip);
        string name = dataPlayer.namePl;
        if (name.Length > 15)
        {
            name = name.Substring(0, 15) + "...";
        }
        lb_name.text = name;
        lb_id.text = dataPlayer.id.ToString();
        lb_ag.text = Globals.Config.FormatMoney((int)Mathf.Max(0, dataPlayer.ag));
    }
    public void onClick()
    {
        if (_ThisPLayerP.id == Globals.User.userMain.Userid) return;
        // SocketSend.searchFriend(id.ToString());
        UIManager.instance.gameView.onClickInfoPlayer(_ThisPLayerP, false);
    }

}
