using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemPlayerDragonTiger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI lb_name;

    [SerializeField] TextMeshProUGUI lb_ag;


    [SerializeField] TextMeshProUGUI lb_id;

    [SerializeField] Avatar avatar;


    [SerializeField] VipContainer listIconVip;

    private int id = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setInfo(Player dataPlayer)
    {
        Debug.Log("dataPlayer.avatar_id:" + dataPlayer.avatar_id);
        Debug.Log("dataPlayer.namePl:" + dataPlayer.namePl);
        Debug.Log("dataPlayer.fid:" + dataPlayer.fid);
        avatar.loadAvatar(dataPlayer.avatar_id, dataPlayer.namePl, dataPlayer.fid);
        avatar.setVip(dataPlayer.vip);
        listIconVip.setVip(dataPlayer.vip);
        id = dataPlayer.id;
        string name = dataPlayer.namePl;
        if (name.Length > 15)
        {
            name = name.Substring(0, 15) + "...";
        }
        lb_name.text = name;
        lb_id.text = id.ToString();
        lb_ag.text = Globals.Config.FormatMoney(dataPlayer.ag);
    }
    public void onClick()
    {
        if (id == Globals.User.userMain.Userid) return;
        SocketSend.searchFriend(id.ToString());
    }

}
