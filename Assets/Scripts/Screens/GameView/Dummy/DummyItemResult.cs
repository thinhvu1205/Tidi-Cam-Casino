using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DummyItemResult : MonoBehaviour
{
    //avatar: cc.Node,
    //    lb_name: cc.Label,
    //    lb_score: cc.Label,
    //    lb_money: cc.Label,
    //    ic_rank: cc.Sprite,
    //    lb_rank: cc.Label,
    //    listIcRank: [cc.SpriteFrame],
    [SerializeField]
    Avatar avatar;
    [SerializeField]
    TextMeshProUGUI lb_name, lb_score, lb_money, lb_rank, lb_point, lb_money_hit;
    [SerializeField]
    Image ic_rank;
    [SerializeField]
    List<Sprite> listIcRank;// = new List<Sprite>();

    public void setData(JObject data)
    {
        //Globals.Logging.Log("set dataa   " + data.ToString());
        //        {
        //            "id": 171286,
        //  "score": 200,
        //  "money": 1960,
        //  "hitpot": null,
        //  "rank": 1
        //}
        //var player = (JObject)data["player"];
        lb_name.text = (string)data["displayName"];
        Globals.Config.effectTextRunInMask(lb_name);
        avatar.loadAvatar((int)data["avatar_id"], (string)data["pname"], (string)data["fid"]);
        lb_score.text = Globals.Config.FormatNumber((int)data["score"]);
        lb_point.text = Globals.Config.FormatNumber((int)data["point"]);
        var totalMoney = (int)data["money"] + (int)data["hitpot"];
        lb_money.text = Globals.Config.FormatNumber(totalMoney);
        lb_money_hit.text = Globals.Config.FormatNumber((int)data["hitpot"]);

        int rank = (int)data["rank"];
        ic_rank.gameObject.SetActive(rank <= 2);
        if (rank <= 2)
            ic_rank.sprite = listIcRank[rank - 1];

        lb_rank.text = rank + "";
        transform.localScale = new Vector3(1, 0, 1);
        transform.DOScaleY(1, (rank - 1) * .2f);
    }
}
