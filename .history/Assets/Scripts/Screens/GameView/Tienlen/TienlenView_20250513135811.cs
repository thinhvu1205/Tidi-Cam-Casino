using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TienlenView : GameView
{
    public static TienlenView instance;
    private int timeToStart = 0;
    [SerializeField] private TextMeshProUGUI m_TimeToStartText;
    [SerializeField] private GameObject m_BgStart;
    [SerializeField] private Transform m_ContainerCards;
    [SerializeField] private GameObject m_Discard;
    [SerializeField] private GameObject m_ButtonCancel;
    public List<List<Card>> ListCardPlayer = new List<List<Card>>();
    public List<List<Card>> ListCardPlayerD = new List<List<Card>>();
    private string turnNameCurrent = "";

    private string lastTurnName = "";
    private int timeTurn = 0;
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
        string dataS = (string)jData["data"];
        JObject data = JObject.Parse(dataS);
        base.handleSTable(dataS);
        countDownTimeToStart(time);
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
            timeToStart = time;
            m_TimeToStartText.gameObject.SetActive(true);
            m_BgStart.SetActive(true);
            TweenCallback callback = () =>
                        {
                            if (timeToStart > 0)
                            {
                                m_TimeToStartText.text = timeToStart.ToString();
                                timeToStart--;
                            }
                            else
                            {
                                m_TimeToStartText.gameObject.SetActive(false);
                                m_BgStart.SetActive(false);
                            }
                        };
            DOTween.Sequence()
                         .AppendCallback(callback)
                         .AppendInterval(1f)
                         .SetLoops(timeToStart + 1);
        }
    }
    private void connectGame(JObject data)
    {
        JArray ArrP = getJArray(data, "ArrP");
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            Player player = getPlayerWithID(getInt(dataPlayer, "id"));
            JArray Arr = getJArray(dataPlayer, "Arr");
            int position = players.IndexOf(player);
            List<Card> listCard = ListCardPlayer[position];
            for (int j = 0; j < Arr.Count; j++)
            {
                int cardCode = (int)Arr[j];
                Card card = spawnCard();
                card.setTextureWithCode(cardCode);
                card.gameObject.SetActive(true);
                listCard.Add(card);
            }
        }
        turnNameCurrent = (string)data["CN"];
        lastTurnName = (string)data["lp"];
        timeTurn = (int)data["T"];
        Player playerD = getPlayer(lastTurnName);
        int positionD = players.IndexOf(playerD);
        if (playerD != null)
        {
            List<Card> listCardD = ListCardPlayerD[positionD];
            JArray Arr = getJArray(data, "CardsInTurn");
            for (int j = 0; j < Arr.Count; j++)
            {
                int cardCode = (int)Arr[j];
                Card card = spawnCard();
                card.setTextureWithCode(cardCode);
                card.gameObject.SetActive(true);
                listCardD.Add(card);
            }

        }
        initPlayerCard();
    }

    public Card spawnCard()
    {
        foreach (var cardList in ListCardPlayer)
        {
            foreach (var card in cardList)
            {
                if (card != null && !card.gameObject.activeSelf)
                {
                    return card; // Tái sử dụng lá bài
                }
            }
        }
        Card cardTemp = getCard();
        cardTemp.setTextureWithCode(0);
        cardTemp.transform.localPosition = new Vector2(0f, 300f);
        cardTemp.transform.SetParent(m_ContainerCards);
        return cardTemp;
    }
    public void initPlayerCard()
    {
        // 1. Setup buttons theo state game
        if (stateGame == Globals.STATE_GAME.PLAYING)
        {
            // Enable nút đánh và bỏ lượt
            m_Discard.SetActive(true); 
            m_ButtonCancel.SetActive(true);
            m_Discard.GetComponent<Button>().interactable = true;
            m_ButtonCancel.GetComponent<Button>().interactable = true;

            // Nếu là lượt của người chơi hiện tại
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_Discard.SetActive(true);
                m_ButtonCancel.SetActive(true);
            }
        }

        // 2. Setup bài cho từng người chơi
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            
            // Skip nếu đang xem và là người chơi chính
            if (player == thisPlayer && stateGame == Globals.STATE_GAME.VIEWING)
                continue;

            int position = players.IndexOf(player);
            List<Card> listCard = ListCardPlayer[position];
            List<Card> listCardD = ListCardPlayerD[position];

            // 3. Hiển thị bài trên tay
            foreach (Card card in listCard)
            {
                card.gameObject.SetActive(true);
                
                if (player == thisPlayer)
                {
                    // Setup bài người chơi chính
                    card.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                    // Tính toán vị trí căn giữa
                    float posX = -((listCard.Count * 30f) / 2) + (listCard.IndexOf(card) * 30f);
                    card.transform.localPosition = new Vector3(posX, -250f, 0);
                }
                else
                {
                    // Setup bài người chơi khác
                    card.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    SetCardPositionByPlayerIndex(card, position);
                }
            }

            // 4. Hiển thị bài đã đánh
            foreach (Card card in listCardD)
            {
                card.gameObject.SetActive(true);
                card.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                SetDiscardCardPosition(card, position, listCardD.IndexOf(card));
            }

           
        }
    }

    // Helper method cho vị trí bài
    private void SetCardPositionByPlayerIndex(Card card, int playerIndex)
    {
        switch (playerIndex)
        {
            case 1: // Người chơi bên phải
                card.transform.localPosition = new Vector3(350f, 0f, 0);
                break;
            case 2: // Người chơi đối diện
                card.transform.localPosition = new Vector3(0f, 250f, 0);
                break;
            case 3: // Người chơi bên trái
                card.transform.localPosition = new Vector3(-350f, 0f, 0);
                break;
        }
    }

    private void SetDiscardCardPosition(Card card, int playerIndex, int cardIndex)
    {
        Vector3 basePos = new Vector3(0f, 0f, 0);
        float offset = cardIndex * 30f;

        switch (playerIndex) 
        {
            case 0: // Người chơi chính
                basePos = new Vector3(0f, -150f, 0);
                break;
            case 1: // Bên phải
                basePos = new Vector3(250f, 0f, 0);
                break;
            case 2: // Đối diện
                basePos = new Vector3(0f, 150f, 0);
                break;
            case 3: // Bên trái
                basePos = new Vector3(-250f, 0f, 0);
                break;
        }

        card.transform.localPosition = new Vector3(
            basePos.x + offset,
            basePos.y,
            basePos.z
        );
    }

    void Awake()
    {
        base.Awake();
        instance = this;
        for (int i = 0; i < 4; i++)
        {
            ListCardPlayer.Add(new List<Card>());
            ListCardPlayerD.Add(new List<Card>());
        }
    }
}