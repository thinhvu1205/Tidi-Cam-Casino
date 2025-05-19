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
    [SerializeField] public SkeletonGraphic m_AniFinish;
    [SerializeField] public SkeletonGraphic m_AniCardSpecial;
    [SerializeField] public Image m_AvatarSpecial;
    [SerializeField] public TextMeshProUGUI m_NameWin;

    public List<List<Card>> ListCardPlayer = new List<List<Card>>();
    public List<List<Card>> ListCardPlayerD = new List<List<Card>>();

    private string turnNameCurrent = "";

    private string lastTurnName = "";
    private int timeTurn = 0;
    private bool isFinish = false;

    // Thêm các biến quản lý touch
    private Card selectedCard = null;
    private Vector3 touchStartPos;
    private Vector3 cardStartPos;
    private bool isDragging = false;
    private float cardSpacing = 30f;
    private float dragThreshold = 20f; // Ngưỡng để xác định drag hay tap
    private List<Card> selectedCards = new List<Card>();

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
        base.handleCTable((string)jData["data"]);
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
        playerD.setTurn(true, timeTurn);
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
        cardTemp.transform.localPosition = new Vector2(0f, 20f);
        cardTemp.transform.SetParent(m_ContainerCards);
        return cardTemp;
    }
    public void initPlayerCard()
    {
        Debug.Log("check xem là ai đánh bài");
        // 1. Setup buttons theo state game
        if (stateGame == Globals.STATE_GAME.PLAYING)
        {
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonCancel.SetActive(true);
                m_ButtonDiscard.GetComponent<Button>().interactable = true;
                m_ButtonCancel.GetComponent<Button>().interactable = true;

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
                    Debug.Log("có set bài cho người chơi chính");
                    // Setup bài người chơi chính
                    card.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                    // Tính toán vị trí căn giữa
                    float posX = -((listCard.Count * 30f) / 2) + (listCard.IndexOf(card) * 30f);
                    card.transform.localPosition = new Vector3(posX, -250f, 0);
                }
                else
                {
                    // Setup bài người chơi khác
                    card.transform.localScale = new Vector3(0.45f, 0.45f, 1);
                    SetCardPositionByPlayerIndex(card, position);
                }
            }

            // 4. Hiển thị bài đã đánh
            foreach (Card card in listCardD)
            {
                card.gameObject.SetActive(true);
                card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                SetDiscardCardPosition(card, position, listCardD.IndexOf(card));
            }


        }
    }

    // Helper method cho vị trí bài
    private void SetCardPositionByPlayerIndex(Card card, int playerIndex)
    {
        switch (playerIndex)
        {
            case 1:
                card.transform.localPosition = new Vector3(498f, 50f, 0);
                break;
            case 2:
                card.transform.localPosition = new Vector3(40f, 250f, 0);
                break;
            case 3:
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
                basePos = new Vector3(0f, -310f, 0);
                break;
            case 1: // Bên phải
                basePos = new Vector3(358f, 0f, 0);
                break;
            case 2: // Đối diện
                basePos = new Vector3(20f, 120f, 0);
                break;
            case 3: // Bên trái
                basePos = new Vector3(-358f, 0f, 0);
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
        JArray arr = getJArray(data, "arr");
        timeTurn = getInt(data, "T");
        turnNameCurrent = getString(data, "nameturn");
        bool firstRound = getBool(data, "firstRound");
        // 1. Initial setup

        TweenCallback start = () =>
        {
            playSound(Globals.SOUND_HILO.START_GAME);
            beforeStartGame();
            m_AniStart.gameObject.SetActive(true);
            m_AniStart.AnimationState.SetAnimation(0, "start", false);
            m_BgStart.SetActive(false);
        };
        Sequence mainSequence = DOTween.Sequence().AppendCallback(start);

        // Delay đầu game
        mainSequence.AppendInterval(1.5f);

        // Setup và chia bài
        mainSequence.AppendCallback(() =>
        {
              m_AniStart.gameObject.SetActive(false);
            Vector3 deckPosition = new Vector3(0f, 20f, 0f); // Vị trí bộ bài giữa bàn
            float stackOffset = 0.1f; // Khoảng cách giữa các lá trong stack

            // Setup bài cho mỗi người chơi
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                int position = players.IndexOf(player);
                ListCardPlayer[position].Clear();

                for (int j = 0; j < arr.Count; j++)
                {
                    Card card = spawnCard();
                    // Setup bài trong stack ở giữa
                    card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                    card.transform.localPosition = new Vector3(
                        deckPosition.x,
                        deckPosition.y - (stackOffset * (i * arr.Count + j)), // Stack các lá
                        0
                    );

                    // Set texture (úp/ngửa)
                    if (player == thisPlayer)
                        card.setTextureWithCode((int)arr[j]);
                    else
                        card.setTextureWithCode(0);

                    card.gameObject.SetActive(true);
                    ListCardPlayer[position].Add(card);
                }
            }

            // Gọi chiaBai() để animate từ stack -> vị trí người chơi
            chiaBai();
        });

        // Delay để đợi animation chia bài
        mainSequence.AppendInterval(3f);

        // Final setup
        mainSequence.AppendCallback(() =>
        {
            // Enable nút điều khiển nếu là lượt của người chơi
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonCancel.SetActive(true);
            }

            // Show hint nếu là lượt đầu
            if (firstRound && turnNameCurrent == thisPlayer.namePl)
            {
                ShowFirstTurnHint();
            }
        });

        // Play toàn bộ sequence
        mainSequence.Play();
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
        // Vị trí bắt đầu của bộ bài ở giữa
        Vector3 deckPosition = new Vector3(0f, 20f, 0f);
        float dealTime = 0.15f;
        float dealDelay = 0.05f;

        // Sequence chính cho việc chia bài
        Sequence dealSequence = DOTween.Sequence();

        // Chia bài theo thứ tự: mỗi người 1 lá, lặp lại 13 lần
        for (int cardIndex = 0; cardIndex < 13; cardIndex++)
        {
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {
                Player player = players[playerIndex];
                int position = players.IndexOf(player);
                List<Card> playerCards = ListCardPlayer[position];

                if (cardIndex < playerCards.Count)
                {
                    Card card = playerCards[cardIndex];
                    float delay = (cardIndex * players.Count + playerIndex) * dealDelay;

                    // Lưu các giá trị cho callback
                    int capturedCardIndex = cardIndex;
                    int capturedPosition = position;

                    dealSequence.InsertCallback(delay, () =>
                    {
                        // Hiện lá bài và đặt về vị trí bộ bài
                        card.gameObject.SetActive(true);
                        card.transform.localPosition = deckPosition;
                        card.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

                        // Tính toán vị trí đích
                        Vector3 finalPos;
                        Vector3 finalScale;

                        if (player == thisPlayer)
                        {
                            float spacing = 60f;
                            float totalWidth = (playerCards.Count - 1) * spacing;
                            float startX = -totalWidth / 2f;
                            finalPos = new Vector3(startX + (capturedCardIndex * spacing), -250f, 0f);
                            finalScale = new Vector3(0.8f, 0.8f, 1f);
                        }
                        else
                        {
                            switch (capturedPosition)
                            {
                                case 1: // Phải
                                    finalPos = new Vector3(498f, 80f, 0f);
                                    break;
                                case 2: // Trên
                                    finalPos = new Vector3(40f, 250f, 0f);
                                    break;
                                case 3: // Trái
                                    finalPos = new Vector3(-498f, 80f, 0f);
                                    break;
                                default:
                                    finalPos = Vector3.zero;
                                    break;
                            }
                            finalScale = new Vector3(0.45f, 0.45f, 1f);
                        }

                        // Animation bay từ bộ bài đến vị trí người chơi
                        card.transform.DOLocalMove(finalPos, dealTime)
                            .SetEase(Ease.OutQuint);
                        card.transform.DOScale(finalScale, dealTime);

                        // Sound effect
                        playSound(Globals.SOUND_GAME.CARD_FLIP_1);
                    });
                }
            }
        }

        // Callback khi chia xong
        float totalDuration = (13 * players.Count * dealDelay) + dealTime;
        dealSequence.InsertCallback(totalDuration, () =>
        {
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonCancel.SetActive(true);
            }
        });

        dealSequence.Play();
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
        for (int i = 0; i < 4; i++)
        {
            foreach (Card card in ListCardPlayerD[i])
            {
                card.gameObject.SetActive(false);
            }
            ListCardPlayerD[i].Clear();
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
            // Với người chơi khác - chuyển bài từ tay xuống bàn
            for (int i = 0; i < vtCard.Count; i++)
            {
                Card card = listCard[0];
                card.setTextureWithCode((int)vtCard[i]);
                card.gameObject.SetActive(true); // Hiện lá bài mới
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
            card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
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

    public void boLuot(string turnName, string nextTurn, bool newTurn)
    {
        // 1. Play sound và setup thông tin lượt
        playSound(Globals.SOUND_GAME.FOLD);
        Player player = getPlayer(turnName);  // Người bỏ lượt
        turnNameCurrent = nextTurn;   // Set người đánh tiếp theo

        // 2. Ẩn nút nếu là người chơi chính
        if (player == thisPlayer)
        {
            m_ButtonCancel.SetActive(false);
            m_ButtonDiscard.SetActive(false);
        }

        // 3. Xử lý newTurn (lượt đánh mới)
        if (newTurn)
        {
            // Show nút đánh bài nếu là lượt mình
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonDiscard.SetActive(true);
                m_ButtonDiscard.GetComponent<Button>().interactable = true;
                // Set vị trí nút về giữa
                m_ButtonDiscard.transform.localPosition = new Vector3(0, m_ButtonDiscard.transform.localPosition.y, 0);
            }

            // Clear bài trên bàn của tất cả người chơi
            for (int i = 0; i < players.Count; i++)
            {
                int position = players.IndexOf(players[i]);
                ListCardPlayerD[position].Clear();
            }

            // Reset các state
            lastTurnName = "";
            m_AniStart.gameObject.SetActive(false);

            // TODO: Ẩn tất cả sprite "Bỏ lượt"
            // Cần thêm UI cho sprite "Bỏ lượt"
        }
        else
        {
            // Show nút đánh và bỏ lượt nếu là lượt mình
            if (turnNameCurrent == thisPlayer.namePl)
            {
                m_ButtonCancel.SetActive(true);
                m_ButtonDiscard.SetActive(true);

                m_ButtonCancel.GetComponent<Button>().interactable = true;
                m_ButtonDiscard.GetComponent<Button>().interactable = true;

                // Set vị trí nút sang phải
                m_ButtonDiscard.transform.localPosition = new Vector3(140, m_ButtonDiscard.transform.localPosition.y, 0);
            }

            // TODO: Show sprite "Bỏ lượt" cho người vừa bỏ
            // Cần thêm animation scale cho sprite
        }

        // 4. Set turn cho người tiếp theo
        Player nextPlayer = getPlayer(nextTurn);
        if (nextPlayer != null)
        {
            nextPlayer.setTurn(true, timeTurn);
        }
    }

    public void finishGameTienLen(JArray data)
    {
        // 1. Play sound và setup
        isFinish = true;

        // 2. Show animation finish
        m_AniStart.gameObject.SetActive(false);
        m_AniFinish.gameObject.SetActive(true);
        m_AniFinish.AnimationState.SetAnimation(0, "animation", true);
        m_AniCardSpecial.gameObject.SetActive(false);

        // 3. Xử lý data từng người chơi
        Player playerSpecial = null;
        for (int i = 0; i < data.Count; i++)
        {
            JObject playerData = (JObject)data[i];
            Player player = getPlayer(getString(playerData, "N"));
            if (player != null)
            {
                // Lưu thông tin kết quả
                //player.money = getLong(playerData, "M");         // Tiền thắng/thua
                player.point = getInt(playerData, "point");      // Điểm
                                                                 // player.typeWin = getInt(playerData, "TypeWin");  // Kiểu thắng đặc biệt

                // Decode và hiển thị bài
                JArray arrCard = getJArray(playerData, "ArrCard");
                List<Card> listCard = ListCardPlayer[players.IndexOf(player)];
                for (int j = 0; j < arrCard.Count; j++)
                {
                    if (j < listCard.Count)
                    {
                        listCard[j].setTextureWithCode((int)arrCard[j]);
                        listCard[j].gameObject.SetActive(true);
                    }
                }

                // Nếu là người thắng đặc biệt
                // if (player.typeWin > 0)
                // {
                //     playerSpecial = player;
                //     // Copy avatar và tên
                //     m_AvatarSpecial.sprite = player.playerView.m_Avatar.sprite;
                //     m_NameWin.text = player.displayName;
                // }
            }
        }

        // 4. Animation sequence
        Sequence finishSequence = DOTween.Sequence();

        // Delay đầu
        finishSequence.AppendInterval(0.5f);

        // Prepare finish
        finishSequence.AppendCallback(() =>
        {
            // Ẩn UI gameplay
            m_ButtonDiscard.SetActive(false);
            m_ButtonCancel.SetActive(false);
        });

        // Animation thắng đặc biệt nếu có
        if (playerSpecial != null)
        {
            finishSequence.AppendCallback(() =>
            {
                m_AniCardSpecial.gameObject.SetActive(true);
                // m_AniCardSpecial.AnimationState.SetAnimation(0, GetSpecialWinAnimation(playerSpecial.typeWin), false);
            });
            finishSequence.AppendInterval(2f);
        }

        // Show bài tất cả người chơi
        finishSequence.AppendCallback(() =>
        {
            foreach (var player in players)
            {
                List<Card> listCard = ListCardPlayer[players.IndexOf(player)];
                foreach (Card card in listCard)
                {
                    if (player != thisPlayer)
                    {
                        // Lật bài người khác
                        card.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
                    }
                }
            }
        });

        // Animation tiền bay
        finishSequence.AppendInterval(0.5f);
        finishSequence.AppendCallback(() =>
        {
            foreach (var player in players)
            {
                // if (player.money != 0)
                // {
                //     // Animation tiền bay từ/đến người chơi
                //     player.playerView.effectFlyMoney(player.money);
                //     player.playerView.updateMoney();
                // }
            }
        });

        // Kết thúc game
        finishSequence.AppendInterval(1f);
        finishSequence.AppendCallback(() =>
        {
            handleFinishGame();
        });

        finishSequence.Play();
    }

    private string GetSpecialWinAnimation(int typeWin)
    {
        switch (typeWin)
        {
            case 1: return "toi_trang";
            case 2: return "chat_heo";
            case 3: return "sanh_rong";
            default: return "normal";
        }
    }

    // Hàm xử lý touch bài
    public void OnCardTouch(Card card)
    {
        // Chỉ xử lý touch khi là lượt của mình và không phải đang chia bài
        if (turnNameCurrent != thisPlayer.namePl ) return;

        touchStartPos = Input.mousePosition;
        cardStartPos = card.transform.localPosition;
        selectedCard = card;
        isDragging = true;

        // Animation hover card
        card.transform.DOLocalMoveY(card.transform.localPosition.y + 20f, 0.2f);
    }

    // Update để xử lý drag
    private void Update()
    {
        if (!isDragging || selectedCard == null) return;

        if (Input.GetMouseButton(0))
        {
            // Xử lý kéo thả
            Vector3 dragPos = Input.mousePosition;
            float dragDistance = Vector3.Distance(dragPos, touchStartPos);

            if (dragDistance > dragThreshold)
            {
                // Đang kéo - Cập nhật vị trí card
                Vector3 newPos = cardStartPos;
                newPos.x += (dragPos.x - touchStartPos.x);
                // Giới hạn vùng có thể kéo
                newPos.x = Mathf.Clamp(newPos.x, -400f, 400f);
                selectedCard.transform.localPosition = newPos;

                // Xử lý đổi vị trí các lá bài
                HandleCardSwapping();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Kết thúc drag
            isDragging = false;

            if (Vector3.Distance(Input.mousePosition, touchStartPos) <= dragThreshold)
            {
                // Là tap - toggle chọn bài
                ToggleCardSelection(selectedCard);
            }
            else
            {
                // Là drag - sắp xếp lại vị trí bài
                RearrangeCards();
            }

            selectedCard.transform.DOLocalMoveY(-250f, 0.2f);
            selectedCard = null;
        }
    }

    // Toggle chọn/bỏ chọn bài
    private void ToggleCardSelection(Card card)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            card.transform.DOLocalMoveY(-250f, 0.2f);
        }
        else
        {
            selectedCards.Add(card);
            card.transform.DOLocalMoveY(-230f, 0.2f);
        }
        
        // Enable/disable nút đánh bài
        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
    }

    // Xử lý hoán đổi vị trí bài khi kéo
    private void HandleCardSwapping()
    {
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];
        int currentIndex = playerCards.IndexOf(selectedCard);
        
        // Tìm vị trí mới dựa trên khoảng cách
        float cardX = selectedCard.transform.localPosition.x;
        int newIndex = Mathf.RoundToInt((cardX + (playerCards.Count * cardSpacing / 2)) / cardSpacing);
        newIndex = Mathf.Clamp(newIndex, 0, playerCards.Count - 1);

        if (newIndex != currentIndex)
        {
            // Đổi vị trí trong list
            playerCards.RemoveAt(currentIndex);
            playerCards.Insert(newIndex, selectedCard);

            // Animation các lá bài khác
            RearrangeCards();
        }
    }

    // Sắp xếp lại vị trí các lá bài
    private void RearrangeCards()
    {
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];
        float startX = -(playerCards.Count * cardSpacing / 2);

        for (int i = 0; i < playerCards.Count; i++)
        {
            Card card = playerCards[i];
            Vector3 newPos = new Vector3(startX + (i * cardSpacing), -250f, 0);
            if (card != selectedCard)
            {
                card.transform.DOLocalMove(newPos, 0.2f);
            }
            else
            {
                card.transform.localPosition = newPos;
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