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
        playSound(Globals.SOUND_HILO.START_GAME);
        beforeStartGame();

        // 2. Setup animation và UI
        m_AniStart.gameObject.SetActive(true);
        m_AniStart.AnimationState.SetAnimation(0, "start", false);
        m_BgStart.SetActive(false);

        // 3. Parse data từ server
        JArray arr = getJArray(data, "arr");
        timeTurn = getInt(data, "T");
        turnNameCurrent = getString(data, "nameturn");
        bool firstRound = getBool(data, "firstRound");

        // 4. Setup bài cho mỗi người chơi
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
                card.setTextureWithCode(0);
                card.gameObject.SetActive(false);
                card.transform.rotation = Quaternion.Euler(0, 0, -90);

                // Chỉ decode bài của người chơi chính
                if (player == thisPlayer)
                {
                    card.setTextureWithCode((int)arr[j]);
                }

                ListCardPlayer[position].Add(card);
            }

            // Set số lá bài ban đầu
            //player.numberCard = player == thisPlayer ? 13 : 0;
        }

        // 5. Sequence chia bài và xử lý sau khi chia xong
        Sequence gameSequence = DOTween.Sequence();

        gameSequence.AppendInterval(1f);
        gameSequence.AppendCallback(() =>
        {
            chiaBai();
        });

        gameSequence.AppendInterval(2.8f);
        gameSequence.AppendCallback(() =>
        {
            // Process sau khi chia bài xong
            initPlayerCard();

            Player thisPlayer = getPlayer(turnNameCurrent);
            if (thisPlayer != null)
            {
                thisPlayer.setTurn(true, timeTurn);
            }

            if (turnNameCurrent == thisPlayer.namePl && firstRound)
            {
                ShowFirstTurnHint();
            }

            // Show nút đánh bài nếu là lượt mình
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonDiscard.GetComponent<Button>().interactable = true;
                m_ButtonCancel.SetActive(true);
                m_ButtonCancel.GetComponent<Button>().interactable = true;
            }
        });

        gameSequence.Play();
    }

    private void ShowFirstTurnHint()
    {
        // TODO: Implement animation hint người đánh đầu tiên 
    }

    private void beforeStartGame()
    {
        // TODO: Reset các state game về ban đầu
    }

    private void chiaBai()
    {
        // 1. Tắt animation start game
        m_AniStart.gameObject.SetActive(false);

        // 2. Loop qua từng người chơi
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            int position = players.IndexOf(player);
            List<Card> listCard = ListCardPlayer[position];
            float timedelay = 0;

            // 3. Loop qua từng lá bài
            for (int j = 0; j < listCard.Count; j++)
            {
                Card card = listCard[j];
                card.gameObject.SetActive(true);
                // Set góc xoay -90 độ
                card.transform.rotation = Quaternion.Euler(0, 0, -90);

                if (player == thisPlayer)
                {
                    // Xử lý bài người chơi chính
                    float posX = -((listCard.Count * 30f) / 2) + (j * 30f);
                    Vector3 targetPos = new Vector3(posX, -250f, 0);

                    // Animation cho bài người chơi chính
                    Sequence cardSequence = DOTween.Sequence();

                    cardSequence.AppendInterval(timedelay)
                        // Di chuyển và scale
                        .Append(card.transform.DOLocalMove(targetPos, 0.4f))
                        .Join(card.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 0.4f))
                        .Join(card.transform.DORotate(Vector3.zero, 0.3f))
                        // Animation lật bài
                        .Append(card.transform.DOScaleX(0, 0.15f))
                        .Join(card.transform.DOLocalRotate(new Vector3(0, -20f, 0), 0.15f))
                        .AppendCallback(() =>
                        {
                            // Set texture mặt bài
                            card.setTextureWithCode(card.code);
                            card.transform.localEulerAngles = new Vector3(0, 20f, 0);
                        })
                        .Append(card.transform.DOScaleX(0.7f, 0.15f))
                        .Join(card.transform.DOLocalRotate(Vector3.zero, 0.15f));

                    cardSequence.Play();
                }
                else
                {
                    // Xử lý bài người chơi khác
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

                    // Animation đơn giản cho người chơi khác
                    Sequence cardSequence = DOTween.Sequence();
                    cardSequence.AppendInterval(timedelay)
                        .Append(card.transform.DOLocalMove(targetPos, 0.2f))
                        .Join(card.transform.DORotate(Vector3.zero, 0.3f))
                        .AppendCallback(() =>
                        {
                            // Update số lá bài trực tiếp
                           // player.numberCard++;
                        });

                    cardSequence.Play();
                }

                timedelay += 0.15f; // Delay giữa các lá
            }
        }
    }

    public void danhBai(string turnName, string nextTurn, JArray vtCard, bool newTurn) 
    {
        // 1. Play sound và setup thông tin turn
      //  playSound(Globals.SOUND_HILO.DEAL_ONECARD);
        Player player = getPlayer(turnName); // Người đánh bài
        turnNameCurrent = nextTurn;  // Set người đánh tiếp theo
        lastTurnName = turnName;     // Set người vừa đánh

        // 2. Reset animation đặc biệt
        m_AniStart.gameObject.SetActive(false);
        m_AniStart.transform.SetSiblingIndex(1001);

        // 3. Clear bài cũ trên bàn của tất cả người chơi
        for (int i = 0; i < players.Count; i++) 
        {
            int positionD = players.IndexOf(players[i]);
            ListCardPlayerD[positionD].Clear();
        }

        // 4. Ẩn nút đánh và bỏ lượt nếu là người chơi hiện tại
        if (player == thisPlayer)
        {
            m_ButtonCancel.SetActive(false);
            m_ButtonDiscard.SetActive(false);
        }

        // 5. Xử lý bài đánh ra
        int position = players.IndexOf(player);
        List<Card> listCard = ListCardPlayer[position];
        List<Card> listCardD = ListCardPlayerD[position];

        if (player == thisPlayer)
        {
            // Với người chơi chính - chuyển bài từ tay sang bài đánh
            for (int i = 0; i < vtCard.Count; i++)
            {
                int cardCode = (int)vtCard[i];
                for (int j = 0; j < listCard.Count; j++)
                {
                    if (listCard[j].code == cardCode)
                    {
                        Card card = listCard[j];
                       // card.isTouch = false;
                        listCardD.Add(card);
                        listCard.RemoveAt(j);
                        break;
                    }
                }
            }
        }
        else
        {
            // Với người chơi khác - tạo bài mới và decode
            for (int i = 0; i < vtCard.Count; i++)
            {
                Card card = listCard[0];
                card.setTextureWithCode((int)vtCard[i]);
                listCardD.Add(card);
                listCard.RemoveAt(0);
            }
        }

        // 6. Update số lá bài
       // player.numberCard -= vtCard.Count;

        // 7. Animation đánh bài
        // TODO: Thêm check bài đặc biệt và animation tương ứng
        
        // Animation đánh bài thường
        for (int i = 0; i < listCardD.Count; i++)
        {
            Card card = listCardD[i];
            card.gameObject.SetActive(true);
            card.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            SetDiscardCardPosition(card, position, i);
        }

        // 8. Set turn cho người tiếp theo
        Player nextPlayer = getPlayer(nextTurn);
        if (nextPlayer != null)
        {
            nextPlayer.setTurn(true, timeTurn);
            if (nextPlayer == thisPlayer)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonDiscard.GetComponent<Button>().interactable = true;
                m_ButtonCancel.SetActive(true); 
                m_ButtonCancel.GetComponent<Button>().interactable = true;
            }
        }
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