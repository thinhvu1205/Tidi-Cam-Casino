using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class TienlenView : GameView
{
    public static TienlenView instance;
    private int timeToStart;
   [SerializeField] private TextMeshProUGUI m_TimeToStartText;
   [SerializeField] private GameObject m_
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
    public void handleCTable(JObject jData)
    {
        Debug.Log(jData.ToString() + "handleCTable nhé");
    }
    public void handleSTable(JObject jData)
    {
        Debug.Log(jData.ToString() + "handleSTable nhé");
        int time = (int)jData["time"];
        JObject data = (JObject)jData["data"];
    }
    private void countDownTimeToStart(int time)
    {
        if (time > 0)
        {
            timeToStart = time;
            m_TimeToStartText.text = time.ToString();
            m_TimeToStartText.gameObject.SetActive(true);
            m_TimeToStartText.transform.localScale = Vector3.one;
            m_TimeToStartText.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            m_TimeToStartText.gameObject.SetActive(false);
        }
    }
}