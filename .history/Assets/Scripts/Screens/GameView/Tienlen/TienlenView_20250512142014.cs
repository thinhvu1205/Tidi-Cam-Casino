using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class TienlenView : GameView
{
    public static TienlenView instance;
    private int timeToStart;
    [SerializeField] private TextMeshProUGUI m_TimeToStartText;
    [SerializeField] private GameObject m_BgStart;
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
        if (time <= 0 || players.Count == 1)
        {
            m_TimeToStartText.gameObject.SetActive(false);
            m_BgStart.SetActive(false);
            return;
        }
        else
        {
            m_TimeToStartText.gameObject.SetActive(true);
            m_BgStart.SetActive(true);
            TweenCallback callback = () =>
                        {
                            if(timeToStart>0){
                                
                            }
                            m_TimeToStartText.gameObject.SetActive(false);
                            m_BgStart.SetActive(false);
                            timeToStart = time;
                            m_TimeToStartText.text = timeToStart.ToString();
                            timeToStart--;
                        };


            DOTween.Sequence()
                         .AppendCallback(time)
                         .AppendInterval(del)
                         .SetLoops(timeClock + 1);
            if (timeToStart < 0)
            {
                m_TimeToStartText.gameObject.SetActive(false);
                m_BgStart.SetActive(false);
                return;
            }
        }


    }
}