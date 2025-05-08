using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System.Linq;
using Globals;

public class SicboView : HiloView
{
    protected override void updatePositionPlayerView()
    {
        listPlayerSicbo = new List<Player>();
        for (int i = 0; i < players.Count - 1; i++)
        {
            if (players[i] == null) continue;
            for (int j = i + 1; j < players.Count; j++)
            {
                if (players[j] == null) continue;
                if (players[i].ag < players[j].ag)
                {
                    Player tempP = players[i];
                    players[i] = players[j];
                    players[j] = tempP;
                }
            }
        }
        Player playerP = players.Find(x => x.id == User.userMain.Userid);
        if (playerP != null) thisPlayer = playerP;
        players.Remove(thisPlayer);
        players.Insert(0, thisPlayer);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null || players[i].playerView == null) continue;
            if (i < listPosView.Count) players[i].playerView.transform.localPosition = listPosView[i];
            players[i].playerView.transform.localScale = players[i] == thisPlayer ? new Vector2(0.8f, 0.8f) : new Vector2(0.7f, 0.7f);
            if (i >= 6)
            {
                listPlayerSicbo.Add(players[i]);
                players[i].playerView.gameObject.SetActive(false);
                players[i].playerView.transform.localPosition = avatar_chung.transform.localPosition;
            }
            else
            {
                players[i].updatePlayerView();
                players[i].playerView.gameObject.SetActive(true);
                players[i].updateItemVip(players[i].vip);
            }
        }
    }
}
