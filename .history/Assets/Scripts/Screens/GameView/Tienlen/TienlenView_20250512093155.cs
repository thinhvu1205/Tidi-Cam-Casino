using Newtonsoft.Json.Linq;
using UnityEngine;

public class TienlenView : GameView
{
    public static TienlenView instance;
    protected override void updatePositionPlayerView()
    {
        players.Remove(thisPlayer);
        players.Insert(0, thisPlayer);
        for (int i = 0; i < players.Count; i++)
        {
            if (i < listPosView.Count)
            {
                players[i].playerView.transform.localPosition = listPosView[i];
                players[i].playerView.transform.localScale = players[i] == thisPlayer ? new Vector2(0.8f, 0.8f) : new Vector2(0.7f, 0.7f);
                players[i].updatePlayerView();
                players[i].playerView.gameObject.SetActive(true);
                players[i].updateItemVip(players[i].vip);
            }
        }
    }
    public void handleCTable(JObject jData){
Debug.Log(jData.ToString()+"handleCTable nhé");
    }
}