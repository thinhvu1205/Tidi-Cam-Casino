using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using System.Linq;
using Globals;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI profit;
    [SerializeField] TextMeshProUGUI normalWin;
    [SerializeField] TextMeshProUGUI tongits;
    [SerializeField] TextMeshProUGUI secretMelds;
    [SerializeField] TextMeshProUGUI bonusCard;
    [SerializeField] TextMeshProUGUI burnedPlayers;
    //[SerializeField] TextMeshProUGUI butasan;
    [SerializeField] TextMeshProUGUI challenger;
    [SerializeField] TextMeshProUGUI hitpot;
    [SerializeField] TextMeshProUGUI butasan;
    [SerializeField] Avatar ava_player;
    [SerializeField] TextMeshProUGUI name_player;
    [SerializeField] Image ribbon_type;
    [SerializeField] Sprite[] typeRibbon;
    [SerializeField] Sprite[] type_ribbon_phi;

    public void setInfo(JObject data, Player player)
    {
        List<TextMeshProUGUI> labels = new() { profit, normalWin, tongits, secretMelds, bonusCard, burnedPlayers, challenger, hitpot };
        List<int> values = new() { (int)data["M"], (int)data["normalAG"], (int)data["tongits"], (int)data["secretmelds"], (int)data["bonusCards"], (int)data["bunnedPlayers"], (int)data["challengers"], (int)data["HitPot"] };
        bool hasButasan = data["butasan"] != null;
        butasan.gameObject.SetActive(hasButasan);
        if (hasButasan)
        {
            labels.Add(butasan);
            values.Add((int)data["butasan"]);
        }
        score.text = ((int)data["Score"]).ToString();
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] == 0)
            {
                if (i == 0 || i == 3 || i == 4 || i == 8)
                {
                    labels[i].text = values[i].ToString();
                }
                else
                {
                    labels[i].text = "-";
                }
            }
            else if (values[i] < 0)
            {
                labels[i].text = values[i].ToString();
                labels[i].color = Color.yellow;

            }
            else if (values[i] > 0)
            {
                labels[i].text = values[i].ToString();
                labels[i].color = Color.green;
            }
        }

        ava_player.loadAvatar(player.avatar_id, player.displayName, player.fid);
        ava_player.setVip(player.vip);
        string name = player.displayName;
        name_player.text = name;


        int ribbon = getRibbonType((int)data["winFightType"], (int)data["rank"], (int)data["bunnedPlayers"]);
        if (ribbon == 1)
        {
            if ((int)data["tongits"] > 0)
            {
                ribbon = 8;
            }
        }
        //if (ribbon == 7) ribbon_type.rectTransform.anchoredPosition = new Vector2(0, -47);
        string languaSave = PlayerPrefs.GetString("language_client");
        if (languaSave != "EN")
        {
            ribbon_type.sprite = type_ribbon_phi[ribbon];
        }
        else
        {
            ribbon_type.sprite = typeRibbon[ribbon];
        }
        ribbon_type.SetNativeSize();
        if (ribbon == 7) ribbon_type.transform.localPosition = new Vector2(0, -50f);

    }


    public int getRibbonType(int type, int rank, int isburn)
    {
        switch (type)
        {
            case -2:
                return 0;
            case -1:
                if (rank == 1)
                {
                    return 1;
                }
                else
                {
                    if (isburn < 0)
                    {
                        return 7;
                    }
                    return 2;
                }
            case 0:
                return 3;
            case 1:
                return 4;
            case 2:
                return 5;
            case 3:
                return 6;
            default:
                return 0;
        }
    }
}
