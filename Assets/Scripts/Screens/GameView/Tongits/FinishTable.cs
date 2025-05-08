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

public class FinishTable : MonoBehaviour
{
    [SerializeField] Button dark_bg;
    [SerializeField] GameObject table_label;
    [SerializeField] PlayerInfo player_info;
    [SerializeField] Button btn_close;
    [SerializeField] GameObject title;
    [SerializeField] GameObject butasan;

    List<PlayerInfo> infoPlayers = new List<PlayerInfo>();

    public void showTable(JArray data, List<Player> players)
    {
        dark_bg.gameObject.SetActive(true);

        Vector2 initialPosition = new Vector2(-200f, 180f);
        Vector2 targetPosition = new Vector2(-50f, 180f);
        table_label.transform.localPosition = initialPosition;
        table_label.transform.DOLocalMove(targetPosition, 0.4f).SetEase(Ease.InOutCubic);
        int rank3Count = 1;
        for (int i = 0; i < data.Count; i++)
        {
            JObject item = (JObject)data[i];
            if ((int)item["rank"] == 1 || (int)item["rank"] == 2)
            {
                PlayerInfo info = spawnPlayerInfo(item, players[i]);
                Vector2 pos = getPlayerInfoPosition((int)data[i]["rank"]);
                info.transform.localPosition = new Vector3(-200, pos.y, 0);
                info.transform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutCubic);

            }
            else
            {
                if ((int)item["rank"] == 3)
                {
                    rank3Count++;
                    if (rank3Count == 3)
                    {
                        PlayerInfo info = spawnPlayerInfo(item, players[i]);
                        Vector2 pos = getPlayerInfoPosition(2);
                        info.transform.localPosition = new Vector3(-200, pos.y, 0);
                        info.transform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutCubic);
                    }
                    else
                    {
                        PlayerInfo info = spawnPlayerInfo(item, players[i]);
                        Vector2 pos;
                        if (data.Count < 3)
                            pos = getPlayerInfoPosition(2);
                        else
                            pos = getPlayerInfoPosition((int)item["rank"]);
                        info.transform.localPosition = new Vector3(-200, pos.y, 0);
                        info.transform.DOLocalMove(pos, 0.4f).SetEase(Ease.InOutCubic);
                    }
                }
            }
        }
    }

    public PlayerInfo spawnPlayerInfo(JObject data, Player player)
    {
        PlayerInfo infoObject = Instantiate(player_info, transform);

        infoPlayers.Add(infoObject);
        infoObject.setInfo(data, player);
        butasan.SetActive(data["butasan"] != null);
        return infoObject;
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    public Vector2 getPlayerInfoPosition(int rank)
    {
        Debug.Log("!>@ rank " + rank);
        switch (rank)
        {
            case 1:
                return new Vector2(0, 110);

            case 2:
                return new Vector2(0, -5);

            case 3:
                return new Vector2(0, -120);

            default:
                return Vector2.zero;
        }
    }
}
