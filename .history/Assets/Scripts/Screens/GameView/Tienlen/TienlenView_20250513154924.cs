using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using Spine.Unity;
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
    [SerializeField] private GameObject m_ButtonDiscard;
    [SerializeField] private GameObject m_ButtonCancel;
    [SerializeField] public SkeletonGraphic m_AniStart;

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
                players[i].playerView.transform.localScale = new Vector2(0.8f, 0.8f);
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
        Debug.Log("check xem là ai đánh bài");
        // 1. Setup buttons theo state game
        if (stateGame == Globals.STATE_GAME.PLAYING)
        {
            // Enable nút đánh và bỏ lượt
            m_ButtonDiscard.SetActive(true);
            m_ButtonCancel.SetActive(true);
            m_ButtonDiscard.GetComponent<Button>().interactable = true;
            m_ButtonCancel.GetComponent<Button>().interactable = true;

            // Nếu là lượt của người chơi hiện tại
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
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
                    card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
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
                card.transform.localPosition = new Vector3(498f, 50f, 0);
                break;
            case 2: // Người chơi đối diện
                card.transform.localPosition = new Vector3(40f, 250f, 0);
                break;
            case 3: // Người chơi bên trái
                card.transform.localPosition = new Vector3(-498f, 50f, 0);
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
                basePos = new Vector3(0f, -390f, 0);
                break;
            case 1: // Bên phải
                basePos = new Vector3(398f, 0f, 0);
                break;
            case 2: // Đối diện
                basePos = new Vector3(50f, 150f, 0);
                break;
            case 3: // Bên trái
                basePos = new Vector3(-398f, 0f, 0);
                break;
        }

        card.transform.localPosition = new Vector3(
            basePos.x + offset,
            basePos.y,
            basePos.z
        );
    }
    public void handleVTable(JObject jData)
    {
        Debug.Log(jData.ToString() + "handleVTable nhé");
        string dataS = (string)jData["data"];
        JObject data = JObject.Parse(dataS);
        base.handleVTable(dataS);
        connectGame(data);
    }
    public void startGame(JObject data)
    {
        // 1. Show animation bắt đầu game
        m_AniStart.gameObject.SetActive(true);
        m_AniStart.AnimationState.SetAnimation(0, "start", false);
        
        // Tắt animation sau khi chạy xong
        m_AniStart.AnimationState.Complete += (entry) => {
            m_AniStart.gameObject.SetActive(false);
        };

        // 2. Ẩn countdown thời gian
        m_BgStart.SetActive(false);
        m_TimeToStartText.gameObject.SetActive(false);

        // 3. Parse data từ server
        JArray arr = getJArray(data, "arr"); // Mảng bài
        timeTurn = getInt(data, "T"); // Thời gian lượt
        turnNameCurrent = getString(data, "nameturn"); // Tên người đánh đầu
        bool firstRound = getBool(data, "firstRound"); // Lượt đầu tiên

        // 4. Khởi tạo bài cho mỗi người chơi
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            int position = players.IndexOf(player);
            ListCardPlayer[position].Clear();

            // Add 13 lá bài cho mỗi người
            for (int j = 0; j < arr.Count; j++)
            {
                Card card = spawnCard();
                card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                card.setTextureWithCode(0); // Set texture úp bài
                card.gameObject.SetActive(false);

                // Chỉ decode bài của người chơi chính
                if (player == thisPlayer)
                {
                    card.setTextureWithCode((int)arr[j]);
                }

                ListCardPlayer[position].Add(card);
            }
        }

        // 5. Sequence chia bài
        Sequence dealSequence = DOTween.Sequence();
        dealSequence.AppendInterval(1f);
        dealSequence.AppendCallback(() =>
        {
            DealCards(); // Animation chia bài
        });
        dealSequence.AppendInterval(2.8f);
        dealSequence.AppendCallback(() =>
        {
            // Process sau khi chia bài xong
            initPlayerCard(); // Setup vị trí bài

            Player player = getPlayer(turnNameCurrent);
            if (player != null)
            {
                player.playerView.setTurn(true, timeTurn);
            }
            // Show hint nếu là người đánh đầu
            if (turnNameCurrent == thisPlayer.namePl && firstRound)
            {
                ShowFirstTurnHint();
            }

            // Show nút đánh nếu là lượt mình
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonDiscard.GetComponent<Button>().interactable = true;
            }
        });

        dealSequence.Play();
    }

    private void DealCards()
    {
        Debug.Log("xem là chỗ chia bài này xem như nào");
        // Tạo sequence chính
        Sequence dealSequence = DOTween.Sequence();

        // Chia bài cho từng người chơi
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            int position = players.IndexOf(player);
            List<Card> listCard = ListCardPlayer[position];

            // Chia từng lá bài cho người chơi
            for (int j = 0; j < listCard.Count; j++)
            {
                Card card = listCard[j];
                // Reset vị trí bài về giữa bàn
                card.transform.localPosition = new Vector3(0f, 0f, 0);
                card.gameObject.SetActive(true);

                // Tính delay cho mỗi lá
                float delay = 0.1f * (i * 13 + j); // 0.1s cho mỗi lá

                // Tạo sequence cho 1 lá bài
                Sequence cardSequence = DOTween.Sequence();

                if (player == thisPlayer)
                {
                    // Chia bài cho người chơi chính
                    float posX = -((listCard.Count * 30f) / 2) + (j * 30f);
                    cardSequence.AppendInterval(delay)
                        .Append(card.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 0.2f))
                        .Join(card.transform.DOLocalMove(new Vector3(posX, -250f, 0), 0.2f))
                        .SetEase(Ease.OutQuad);
                }
                else
                {
                    // Chia bài cho người chơi khác
                    Vector3 targetPos = Vector3.zero;
                    switch (position)
                    {
                        case 1: // Phải
                            targetPos = new Vector3(498f, 50f, 0);
                            break;
                        case 2: // Trên
                            targetPos = new Vector3(40f, 250f, 0);
                            break;
                        case 3: // Trái  
                            targetPos = new Vector3(-498f, 50f, 0);
                            break;
                    }

                    cardSequence.AppendInterval(delay)
                        .Append(card.transform.DOScale(new Vector3(0.4f, 0.4f, 1), 0.2f))
                        .Join(card.transform.DOLocalMove(targetPos, 0.2f))
                        .SetEase(Ease.OutQuad);
                }

                // Add sequence của lá bài vào sequence chính
                dealSequence.Join(cardSequence);
            }
        }

        // Play sound effect chia bài
        dealSequence.OnStart(() =>
        {
            // TODO: Add sound chia bài
        });

        // Play toàn bộ sequence
        dealSequence.Play();
    }

    private void ShowFirstTurnHint()
    {
        // TODO: Implement animation hint cho người đánh đầu tiên
        // Có thể dùng DOTween để tạo animation nhấp nháy
    }
    protected override void Awake()
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