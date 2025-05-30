using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] public SkeletonGraphic m_AniWinSpecial;
    [SerializeField] public Image m_AvatarSpecial;
    [SerializeField] public TextMeshProUGUI m_NameWin;
    [SerializeField] public List<GameObject> m_TxtPass;
    [SerializeField] public GameObject m_BtnSort;
    [SerializeField] public List<SkeletonGraphic> m_ListAniChay;
    [SerializeField] private GameObject m_PrefabChip; // Prefab của chip
    [SerializeField] private Transform m_ContainerChip; // Container chứa các chip

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

    private int typeSort = 2; // Thêm biến để track kiểu sắp xếp
    private Queue<GameObject> chipPool = new Queue<GameObject>(); // Pool các chip
    private int initialPoolSize = 50; // Số lượng chip khởi tạo ban đầu
    private List<GameObject> vtChipFinish = new List<GameObject>();

    private List<Card> listCardSuggest = new List<Card>(); // List chứa các lá bài được gợi ý

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
            else
            {
                players[i].playerView.gameObject.SetActive(false);
            }
        }
    }
    public override void handleSTable(string data)
    {
        base.handleSTable(data);
    }
    public void countDownTimeToStart(int time)
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
            playerD.setTurn(true, timeTurn);
        }
        initPlayerCard();

    }

    public Card spawnCard()
    {
        Debug.Log("[spawnCard] Spawning new card");

        // Check tái sử dụng
        foreach (var cardList in ListCardPlayer)
        {
            foreach (var card in cardList)
            {
                if (card != null && !card.gameObject.activeSelf)
                {
                    Debug.Log("[spawnCard] Reusing existing card");
                    AddCardTouchEvents(card);
                    return card;
                }
            }
        }

        // Tạo mới
        Debug.Log("[spawnCard] Creating new card");
        Card cardTemp = getCard();
        if (cardTemp == null)
        {
            Debug.LogError("[spawnCard] Failed to create card!");
            return null;
        }

        cardTemp.setTextureWithCode(0);
        cardTemp.transform.localPosition = new Vector2(0f, 20f);
        cardTemp.transform.SetParent(m_ContainerCards);

        AddCardTouchEvents(cardTemp);
        Debug.Log("[spawnCard] New card created successfully");

        return cardTemp;
    }

    private void AddCardTouchEvents(Card card)
    {
        Debug.Log($"[AddCardTouchEvents] Adding events to card {card.code}");

        // Check component Image
        Image img = card.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError($"[AddCardTouchEvents] Card {card.code} missing Image component!");
            return;
        }
        img.raycastTarget = true;
        Debug.Log($"[AddCardTouchEvents] Card {card.code} raycastTarget = {img.raycastTarget}");

        // Check nếu là bài của người chơi chính
        if (!ListCardPlayer[players.IndexOf(thisPlayer)].Contains(card))
        {
            Debug.Log($"[AddCardTouchEvents] Card {card.code} not belongs to main player, skipping...");
            return;
        }

        EventTrigger trigger = card.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            Debug.Log($"[AddCardTouchEvents] Adding new EventTrigger to card {card.code}");
            trigger = card.gameObject.AddComponent<EventTrigger>();
        }
        else
        {
            Debug.Log($"[AddCardTouchEvents] Clearing existing triggers on card {card.code}");
            trigger.triggers.Clear();
        }

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) =>
        {
            Debug.Log($"[PointerDown] Card {card.code} pressed");
            OnCardTouch(card);
        });
        trigger.triggers.Add(entryDown);

        // Drag
        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) =>
        {
            if (isDragging && selectedCard == card)
            {
                Debug.Log($"[Drag] Card {card.code} being dragged");
                // ... existing drag code ...
            }
        });
        trigger.triggers.Add(entryDrag);

        // EndDrag
        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) =>
        {
            Debug.Log($"[EndDrag] Card {card.code} drag ended");
            // ... existing end drag code ...
        });
        trigger.triggers.Add(entryEndDrag);

        // Check CanvasGroup
        CanvasGroup cg = card.gameObject.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Debug.Log($"[AddCardTouchEvents] Adding CanvasGroup to card {card.code}");
            cg = card.gameObject.AddComponent<CanvasGroup>();
        }
        cg.blocksRaycasts = true;
        cg.interactable = true;
        Debug.Log($"[AddCardTouchEvents] Card {card.code} setup complete");
    }
    public void danhBai(string turnName, string nextTurn, JArray vtCard, bool newTurn)
    {

        // 1. Initial setup
        //  playSound(Globals.SOUND_GAME.CARD_DISCARD);
        playSound(Globals.SOUND_GAME.CARD_FLIP_1);
        Player player = getPlayer(turnName);
        turnNameCurrent = nextTurn;
        lastTurnName = turnName;
        int position = players.IndexOf(player);
        Player playerTurnCurr = getPlayer(lastTurnName);
        if (playerTurnCurr == thisPlayer)
        {
            handleButton(false, false, false);
        }
        // 2. Reset animation và clear bài cũ
        m_AniCardSpecial.gameObject.SetActive(false);

        foreach (var cardList in ListCardPlayerD)
        {
            if (cardList != null)
            {
                for (int i = cardList.Count - 1; i >= 0; i--)
                {
                    if (cardList[i] != null)
                    {
                        cardList[i].gameObject.SetActive(false);
                    }
                    cardList.RemoveAt(i);
                }
            }
        }
        foreach (var img in m_TxtPass)
        {
            img.SetActive(false);
        }
        List<Card> listCard = ListCardPlayer[position];
        List<Card> listCardD = ListCardPlayerD[position];
        ProcessPlayCards(player, listCard, listCardD, vtCard, position);
        if (position == 0)
        {
            RearrangeCards();
        }
        player.setTurn(false, 0);
        Player nextPlayer = getPlayer(nextTurn);
        if (nextPlayer != null)
        {
            nextPlayer.setTurn(true, timeTurn);
            if (nextPlayer == thisPlayer)
            {
                handleButton(true, true, false);
            }
        }
    }

    private void ProcessPlayCards(Player player, List<Card> listCard, List<Card> listCardD, JArray vtCard, int position)
    {
        if (player == thisPlayer)
        {
            for (int i = 0; i < vtCard.Count; i++)
            {
                Card cardToPlay = listCard.Find(c => c.code == (int)vtCard[i]);
                if (cardToPlay != null)
                {
                    listCard.Remove(cardToPlay);
                    listCardD.Add(cardToPlay);
                    cardToPlay.transform.DOScale(0.4f, 0.2f);
                    cardToPlay.gameObject.SetActive(true);

                }
            }

        }
        else
        {
            for (int i = 0; i < vtCard.Count; i++)
            {
                if (listCard.Count > 0)
                {
                    Card cardToPlay = listCard[0];
                    listCard.RemoveAt(0);
                    cardToPlay.setTextureWithCode((int)vtCard[i]);
                    cardToPlay.transform.DOScale(0.4f, 0.2f);
                    cardToPlay.gameObject.SetActive(true);
                    listCardD.Add(cardToPlay);
                }
            }
        }
        SetDiscardCardPosition(listCardD, position);
    }
    public void danhBaiError(string error)
    {
        UIManager.instance.showToast(error);
        selectedCards.Clear();
        RearrangeCards();
        if (turnNameCurrent == thisPlayer.namePl)
        {
            handleButton(true, true, false);
        }
    }

    public void initPlayerCard()
    {
        Debug.Log("check xem là ai đánh bài");
        // 1. Setup buttons theo state game
        if (stateGame == Globals.STATE_GAME.PLAYING)
        {
            if (turnNameCurrent == thisPlayer.namePl)
            {
                handleButton(true, true, false);
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
                SetDiscardCardPosition(listCardD, position);
            }


        }
    }

    // Helper method cho vị trí bài
    private void SetCardPositionByPlayerIndex(Card card, int playerIndex)
    {
        switch (playerIndex)
        {
            case 1:
                card.transform.localPosition = new Vector3(498f, 80f, 0);
                break;
            case 2:
                card.transform.localPosition = new Vector3(40f, 250f, 0);
                break;
            case 3:
                card.transform.localPosition = new Vector3(-498f, 80f, 0);
                break;
        }
    }

    private void SetDiscardCardPosition(List<Card> cards, int playerIndex)
    {
        bool isSpecialHand = false;
        if (check3DoiThong(cards) ||
           check4DoiThong(cards) ||
           checkTuQuy(cards) ||
           checkSetOfTwos(cards))
        {
            isSpecialHand = true;
        }
        if (isSpecialHand)
        {
            Sequence specialSequence = DOTween.Sequence();

            specialSequence.AppendInterval(0.1f);
            specialSequence.AppendCallback(() =>
            {
                Debug.Log("có chạy vào chỗ show cái special");
                m_AniCardSpecial.gameObject.SetActive(true);

                if (check3DoiThong(cards))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "3 consecutive pairs", false);
                }
                else if (check4DoiThong(cards))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "4 consecutive pairs", false);
                }
                else if (checkTuQuy(cards))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "4 of a kind", false);
                }
                else if (checkSetOfTwos(cards))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "set of twos", false);
                }
            });

            specialSequence.AppendInterval(2f);
            specialSequence.AppendCallback(() =>
            {
                // Callback sau hiệu ứng đặc biệt (nếu cần)
            });
            specialSequence.Play();
        }

        Vector3 basePos = Vector3.zero;
        switch (playerIndex)
        {
            case 0:
                basePos = new Vector3(0f, -60f, 0);
                break;
            case 1:
                basePos = new Vector3(358f, 0f, 0);
                break;
            case 2:
                basePos = new Vector3(20f, 120f, 0);
                break;
            case 3:
                basePos = new Vector3(-358f, 0f, 0);
                break;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            EventTrigger trigger = card.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                trigger.triggers.Clear();
                Destroy(trigger);
            }


            Image img = card.GetComponent<Image>();
            if (img != null)
            {
                img.raycastTarget = false;
            }
            CanvasGroup cg = card.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }

            float offset = i * 30f;

            card.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            card.transform.localRotation = Quaternion.identity;

            Vector3 finalPos = new Vector3(
                playerIndex == 1 ? basePos.x - offset : basePos.x + offset,
                basePos.y,
                basePos.z
            );
            if (isSpecialHand)
            {
                Sequence seq = DOTween.Sequence();

                // Bước 1: Scale lên nhiều hơn và xoay chậm hơn
                seq.Append(card.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f));
                seq.Join(card.transform.DOLocalRotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.LocalAxisAdd));

                // Bước 2: Quay về scale và rotation ban đầu
                seq.Append(card.transform.DOScale(new Vector3(0.4f, 0.4f, 1f), 0.25f));
                seq.Join(card.transform.DOLocalRotate(Vector3.zero, 0.25f, RotateMode.Fast));

                // Bước 3: Di chuyển về vị trí cuối
                seq.Append(card.transform.DOLocalMove(finalPos, 0.3f).SetEase(Ease.OutBack));
            }
            else
            {
                card.transform.DOLocalMove(finalPos, 0.2f)
                    .SetEase(Ease.InExpo);
            }



            card.transform.SetSiblingIndex(playerIndex == 1 ? 20 - i : i);
            m_AniCardSpecial.gameObject.SetActive(false);
        }
    }


    public override void handleVTable(string data)
    {
        JObject dataS = JObject.Parse(data);
        base.handleVTable(data);
        connectGame(dataS);
    }
    public void startGame(JObject data)
    {
        typeSort = 2;
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
        mainSequence.AppendInterval(1f);

        // Setup và chia bài
        mainSequence.AppendCallback(() =>
        {
            m_AniStart.gameObject.SetActive(false);
            Vector3 deckPosition = new Vector3(0f, 20f, 0f); // Vị trí bộ bài giữa bàn
            float stackOffset = 0.16f; // Khoảng cách giữa các lá trong stack

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
                        deckPosition.y - (stackOffset * (i * arr.Count + j)),
                        0
                    );

                    card.transform.Rotate(0, 0, 90);
                    // Thêm vào list trước
                    ListCardPlayer[position].Add(card);

                    // Sau đó mới setup events và texture
                    AddCardTouchEvents(card);

                    if (player == thisPlayer)
                        card.setTextureWithCode((int)arr[j]);
                    else
                        card.setTextureWithCode(0);

                    card.gameObject.SetActive(true);
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
            Player player = getPlayer(turnNameCurrent);
            if (player != null)
            {
                player.setTurn(true, timeTurn);
            }
            // Enable nút điều khiển nếu là lượt của người chơi
            if (turnNameCurrent == thisPlayer.namePl)
            {
                handleButton(true, true, false);
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
        float dealTime = 0.25f;
        float dealDelay = 0.05f;

        Sequence dealSequence = DOTween.Sequence();

        for (int cardIndex = 0; cardIndex < 13; cardIndex++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                List<Card> playerCards = ListCardPlayer[players.IndexOf(player)];

                if (cardIndex < playerCards.Count)
                {
                    playSound(Globals.SOUND_GAME.CARD_FLIP_1);
                    Card card = playerCards[cardIndex];
                    float delay = (cardIndex * players.Count + i) * dealDelay;
                    float spacing = 60f;
                    float totalWidth = (playerCards.Count - 1) * spacing;
                    float startX = -totalWidth / 2f;
                    Vector3 finalPos = new Vector3(startX + (cardIndex * spacing), -250f, 0f);

                    dealSequence.InsertCallback(delay, () =>
                    {
                        card.gameObject.SetActive(true);

                        if (player == thisPlayer)
                        {
                            card.transform.DOLocalMove(finalPos, dealTime)
                                .SetEase(Ease.OutQuint);
                            card.transform.DOScale(0.8f, dealTime);
                            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        }
                        else
                        {
                            Vector3 finalPos = Vector3.zero;
                            switch (players.IndexOf(player))
                            {
                                case 1: // Phải
                                    finalPos = new Vector3(498f, 80f, 0);
                                    break;
                                case 2: // Trên
                                    finalPos = new Vector3(40f, 250f, 0);
                                    break;
                                case 3: // Trái
                                    finalPos = new Vector3(-498f, 80f, 0);
                                    break;
                            }
                            card.transform.DOLocalMove(finalPos, dealTime)
                                .SetEase(Ease.OutQuint);
                            card.transform.DOScale(0.45f, dealTime);
                            card.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        }
                    });
                }
            }
        }

        dealSequence.OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                SortCard();
                if (turnNameCurrent == thisPlayer.namePl)
                {
                    handleButton(true, true, false);
                }
            });
        });
        dealSequence.Play();
    }
    public void boLuot(string turnName, string nextTurn, bool newTurn)
    {
        // 1. Play sound và lấy thông tin player
        playSound(Globals.SOUND_GAME.FOLD);
        Player player = getPlayer(turnName);
        turnNameCurrent = nextTurn;

        // 2. Ẩn nút control nếu là người chơi hiện tại
        if (player == thisPlayer)
        {
            selectedCards.Clear();
            handleButton(false, false, false);
        }

        // 3. Show sprite "Pass" cho người vừa bỏ lượt
        int indexPos = players.IndexOf(player);
        if (indexPos >= 0 && indexPos < m_TxtPass.Count)
        {
            GameObject obj = m_TxtPass[indexPos];
            obj.SetActive(true);

            // Reset trạng thái ban đầu
            obj.transform.localScale = Vector3.zero;

            // Tăng scale lên 1 và alpha từ 0 -> 1
            obj.transform.DOScale(0.7f, 0.5f).SetEase(Ease.OutBack);

            // Nếu có CanvasGroup để điều chỉnh alpha
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1f, 0.5f);
            }

            // Auto hide sau 1s
            DOVirtual.DelayedCall(1f, () =>
            {
                if (m_TxtPass[indexPos] != null)
                {
                    obj.SetActive(false);
                }
            });
        }

        if (newTurn)
        {
            // Clear last turn name và bài đã đánh
            lastTurnName = "";

            // Clear bài đã đánh của tất cả người chơi
            foreach (var cardList in ListCardPlayerD)
            {
                if (cardList != null)
                {
                    for (int i = cardList.Count - 1; i >= 0; i--)
                    {
                        if (cardList[i] != null)
                        {
                            cardList[i].gameObject.SetActive(false);
                        }
                        cardList.RemoveAt(i);
                    }
                }
            }



            // Show nút đánh nếu là lượt mình
            if (turnNameCurrent == thisPlayer.namePl)
            {
                handleButton(false, true, true);
            }

            // Ẩn animation đặc biệt

        }
        else
        {
            // Show nút control nếu là lượt mình 
            if (turnNameCurrent == thisPlayer.namePl)
            {
                handleButton(true, true, false);
            }
        }

        // 4. Set turn cho người chơi tiếp theo
        player.setTurn(false, 0);
        Player nextPlayer = getPlayer(nextTurn);
        if (nextPlayer != null)
        {
            nextPlayer.setTurn(true, timeTurn);
        }
    }
    public void handleButton(bool isCancel, bool isDiscard, bool isCenter)
    {
        m_ButtonCancel.SetActive(isCancel);
        m_ButtonDiscard.SetActive(isDiscard);
        m_ButtonCancel.GetComponent<Button>().interactable = isCancel;
        m_ButtonDiscard.GetComponent<Button>().interactable = isDiscard;
        if (!isCenter)
        {
            m_ButtonDiscard.transform.localPosition = new Vector3(100, m_ButtonDiscard.transform.localPosition.y, 0);
        }
        else
        {
            m_ButtonDiscard.transform.localPosition = new Vector3(0, m_ButtonDiscard.transform.localPosition.y, 0);
        }

    }
    public void OnCardTouch(Card card)
    {
        Debug.Log($"[OnCardTouch] Card touched: {card.code}");

        // if (turnNameCurrent != thisPlayer.namePl)
        // {
        //     Debug.Log("[OnCardTouch] Not player's turn, ignoring touch");
        //     return;
        // }
        touchStartPos = Input.mousePosition;
        cardStartPos = card.transform.localPosition;
        selectedCard = card;
        isDragging = true;
    }

    private void SuggestCards(Card selectedCard)
    {
        // 1. Lấy thông tin bài của người chơi trước
        Player lastPlayer = getPlayer(lastTurnName);
        if (lastPlayer == null) return;

        List<Card> lastCards = ListCardPlayerD[players.IndexOf(lastPlayer)];
        if (lastCards.Count == 0) return;

        // 2. Phân tích loại bài đã đánh
        bool isDouble = checkDoiTL(lastCards);
        bool isTriple = checkXamTL(lastCards);
        bool isStraight = checkSanhTL(lastCards);
        bool isFlushStraight = checkThungPhaSanhTL(lastCards);

        // 3. Lấy bài người chơi hiện tại
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];
        listCardSuggest.Clear();
        ClearSuggestion();

        if (isDouble || isTriple)
        {
            int targetNumber = lastCards[0].N;

            // Nhóm bài theo số, chọn nhóm lớn hơn target và đủ số lượng
            var grouped = playerCards
                .GroupBy(c => c.N)
                .Where(g => g.Key > targetNumber)
                .Where(g => (isDouble && g.Count() >= 2) || (isTriple && g.Count() >= 3));

            // Với sám cô cần ưu tiên sám 2 > sám A > ...
            if (isTriple)
            {
                // Sắp xếp lại các nhóm sám theo quy tắc đặc biệt
                grouped = grouped.OrderByDescending(g =>
                {
                    if (g.Key == 2) return 100; // sám 2 cao nhất
                    if (g.Key == 14) return 99; // sám A kế tiếp (A=14)
                    return g.Key; // các lá khác
                });
            }
            else
            {
                // Đôi thì sort bình thường theo số lớn hơn
                grouped = grouped.OrderBy(g => g.Key);
            }

            foreach (var group in grouped)
            {
                var cards = group.Take(isDouble ? 2 : 3).ToList();
                if (cards.Contains(selectedCard))
                {
                    listCardSuggest.AddRange(cards);
                    break;
                }
            }
        }
        else if (isStraight || isFlushStraight)
        {
            int length = lastCards.Count;
            int targetNumber = lastCards[length - 1].N;

            // Tạo bản sao và sort bài theo số
            List<Card> sortedCards = new List<Card>(playerCards);
            sortedCards.Sort((a, b) => a.N.CompareTo(b.N));

            for (int i = 0; i <= sortedCards.Count - length; i++)
            {
                List<Card> seq = sortedCards.Skip(i).Take(length).ToList();

                // Bỏ sảnh có 2 vì 2 không nằm trong sảnh
                if (seq.Any(card => card.N == 2)) continue;

                bool isValid = true;
                for (int j = 1; j < length; j++)
                {
                    if (seq[j].N != seq[j - 1].N + 1 ||
                        (isFlushStraight && seq[j].S != seq[0].S))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid) continue;

                if (seq.Last().N > targetNumber && seq.Contains(selectedCard))
                {
                    listCardSuggest.AddRange(seq);
                    break;
                }
            }
        }
        else
        {
            // Đánh lẻ - chỉ gợi ý selectedCard nếu lớn hơn lá đã đánh
            if (selectedCard.N > lastCards[0].N)
            {
                listCardSuggest.Add(selectedCard);
            }
        }

        Debug.Log($"{listCardSuggest.Count} lá bài gợi ý, có chứa lá chọn: {listCardSuggest.Contains(selectedCard)}");

        // 4. Highlight bộ bài được gợi ý
        HighlightSuggestedCards(selectedCard);
    }



    private void HighlightSuggestedCards(Card selectedCard)
    {
        // Chỉ chạy khi đúng 1 lá bài được chọn và nâng lên
        if (selectedCards.Count != 1 || !selectedCards.Contains(selectedCard))
            return;

        // Nếu gợi ý có selectedCard, thì highlight phần còn lại
        if (listCardSuggest.Contains(selectedCard))
        {
            // Lấy ra các lá khác trong gợi ý (trừ selectedCard)
            var otherSuggestedCards = listCardSuggest
                .Where(card => card != selectedCard && !selectedCards.Contains(card))
                .ToList();

            // Add các lá còn lại vào danh sách selectedCards
            foreach (var card in otherSuggestedCards)
            {
                selectedCards.Add(card);
                card.transform.DOLocalMoveY(-210f, 0.2f)
                    .SetEase(Ease.OutQuint);
            }

            Debug.Log($"[HighlightSuggestedCards] Gợi ý bổ sung {otherSuggestedCards.Count} lá bài");
        }

        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
        Debug.Log($"[HighlightSuggestedCards] Tổng số bài được chọn: {selectedCards.Count}");
    }


    private void ClearSuggestion()
    {
        listCardSuggest.Clear();
        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
        Debug.Log("[ClearSuggestion] Suggestion cleared");
    }
    private void Update()
    {
        if (!isDragging || selectedCard == null) return;

        if (Input.GetMouseButton(0))
        {
            Vector3 dragPos = Input.mousePosition;
            float dragDistance = Vector3.Distance(dragPos, touchStartPos);

            if (dragDistance > dragThreshold)
            {
                Debug.Log($"[Update] Dragging - Distance: {dragDistance}, Threshold: {dragThreshold}");

                Vector3 newPos = cardStartPos;
                newPos.x += (dragPos.x - touchStartPos.x);
                newPos.x = Mathf.Clamp(newPos.x, -400f, 400f);

                Debug.Log($"[Update] New card position: {newPos}");
                selectedCard.transform.localPosition = newPos;
                HandleCardSwapping();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("[Update] Mouse button released");
            isDragging = false;

            float releaseDistance = Vector3.Distance(Input.mousePosition, touchStartPos);
            Debug.Log($"[Update] Release distance: {releaseDistance}");

            if (releaseDistance <= dragThreshold)
            {
                Debug.Log("[Update] Detected as tap - toggling selection");
                ToggleCardSelection(selectedCard);
            }
            else
            {
                Debug.Log("[Update] Detected as drag - rearranging cards");
                RearrangeCards();
            }
            selectedCard = null;
        }
    }

    // Toggle chọn/bỏ chọn bài
    private void ToggleCardSelection(Card card)
    {
        float normalY = -250f;    // Vị trí y bình thường
        float raisedY = -210f;    // Vị trí y khi nâng lên (nâng 40 đơn vị)
        float animDuration = 0.1f; // Thời gian animation

        // Chỉ xử lý nếu đang là lượt của người chơi
        // if (turnNameCurrent != thisPlayer.namePl)
        // {
        //     Debug.Log("[ToggleCardSelection] Not player's turn");
        //     return;
        // }

        // Check xem lá bài có đang được chọn không
        bool isCurrentlySelected = selectedCards.Contains(card);
        bool isCurrentlyRaised = Mathf.Approximately(card.transform.localPosition.y, raisedY);

        // Nếu lá bài đang được chọn (đang nâng lên) -> hạ xuống
        if (isCurrentlySelected && isCurrentlyRaised)
        {
            selectedCards.Remove(card);
            card.transform.DOLocalMoveY(normalY, animDuration)
                .SetEase(Ease.OutQuint);
        }
        // Nếu lá bài chưa được chọn -> nâng lên
        else if (!isCurrentlySelected)
        {
            selectedCards.Add(card);
            card.transform.DOLocalMoveY(raisedY, animDuration)
                .SetEase(Ease.OutQuint);
            if (selectedCards.Count == 1)
            {
                SuggestCards(card);
            }
        }

        // Enable/disable nút đánh bài
        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;

        // Play sound effect khi click
        // playSound(Globals.SOUND_GAME.CARD_SELECT);

        Debug.Log($"[ToggleCardSelection] Card {card.code} - Selected: {selectedCards.Contains(card)}");
    }

    // Xử lý hoán đổi vị trí bài khi kéo
    private void HandleCardSwapping()
    {
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];
        int currentIndex = playerCards.IndexOf(selectedCard);

        // Sử dụng spacing và scale như trong chia bài
        float spacing = 60f;
        float totalWidth = (playerCards.Count - 1) * spacing;
        float startX = -totalWidth / 2f;

        float cardX = selectedCard.transform.localPosition.x;
        int newIndex = Mathf.RoundToInt((cardX - startX) / spacing);
        newIndex = Mathf.Clamp(newIndex, 0, playerCards.Count - 1);

        Debug.Log($"[HandleCardSwapping] Current index: {currentIndex}, New index: {newIndex}");

        if (newIndex != currentIndex)
        {
            Debug.Log($"[HandleCardSwapping] Swapping cards from index {currentIndex} to {newIndex}");

            // Cập nhật thứ tự trong list
            playerCards.RemoveAt(currentIndex);
            playerCards.Insert(newIndex, selectedCard);

            // Sắp xếp lại
            RearrangeCards();
        }
    }

    // Sắp xếp lại vị trí các lá bài
    private void RearrangeCards()
    {
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];
        for (int i = 0; i < playerCards.Count; i++)
        {
            Card card = playerCards[i];
            float dealTime = 0.15f;
            float spacing = 60f;
            float totalWidth = (playerCards.Count - 1) * spacing;
            float startX = -totalWidth / 2f;

            // Kiểm tra xem lá bài có được chọn không
            bool isSelected = selectedCards.Contains(card);
            float targetY = isSelected ? -210f : -250f;

            Vector3 finalPos = new Vector3(startX + (i * spacing), targetY, 0f);
            Vector3 finalScale = new Vector3(0.8f, 0.8f, 1f);

            // Animation di chuyển
            card.transform.DOLocalMove(finalPos, dealTime)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                {
                    // Nếu lá bài ở vị trí thấp, đảm bảo nó không nằm trong danh sách chọn
                    if (card.transform.localPosition.y <= -250f && selectedCards.Contains(card))
                    {
                        selectedCards.Remove(card);
                        // Update trạng thái nút đánh bài
                        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
                    }
                });

            card.transform.DOScale(finalScale, dealTime);
            card.transform.SetSiblingIndex(i + 50);
        }
    }
    public void onClickDanhBai()
    {
        SoundManager.instance.soundClick();
        JArray arrCard = new JArray();

        for (int i = 0; i < selectedCards.Count; i++)
        {
            Card card = selectedCards[i];
            arrCard.Add(card.code);
        }
        SocketSend.danhBai(arrCard);
        selectedCards.Clear();
        handleButton(false, false, false);
    }


    public void onClickBoLuot()
    {
        SocketSend.boLuot();
        handleButton(false, false, false);
    }
    public void OnClickSortCard()
    {
        SoundManager.instance.soundClick();
        SortCard();
    }

    public void SortCard()
    {
        typeSort = (typeSort + 1) % 3;

        // Lấy list bài của người chơi chính 
        List<Card> playerCards = ListCardPlayer[players.IndexOf(thisPlayer)];

        // Sắp xếp theo kiểu tương ứng
        switch (typeSort)
        {
            case 0: // Sắp tăng dần theo số + chất
                playerCards.Sort((x, y) =>
                {
                    int result = x.N.CompareTo(y.N);
                    return result != 0 ? result : x.S.CompareTo(y.S);
                });
                break;

            case 1: // Sắp giảm dần theo số + chất 
                playerCards.Sort((x, y) =>
                {
                    int result = y.N.CompareTo(x.N);
                    return result != 0 ? result : x.S.CompareTo(y.S);
                });
                break;

            case 2: // Sắp theo chất + số
                playerCards.Sort((x, y) =>
                {
                    int result = x.S.CompareTo(y.S);
                    return result != 0 ? result : x.N.CompareTo(y.N);
                });
                break;
        }

        // Disable nút sort tạm thời để tránh spam
        m_BtnSort.GetComponent<Button>().interactable = false;
        DOVirtual.DelayedCall(0.3f, () =>
        {
            m_BtnSort.GetComponent<Button>().interactable = true;
        });

        // Sắp xếp lại vị trí các lá bài
        RearrangeCards();
        Debug.Log($"Cards sorted using type {typeSort}");
    }

    public override void handleJTable(string data)
    {
        base.handleJTable(data);
    }
    public override void handleRJTable(string data)
    {
        base.handleRJTable(data);
        JObject dataS = JObject.Parse(data);
        connectGame(dataS);
    }
    public void cutCard(string nameLose, long agPlayerLose, string nameWin, long agPlayerWin, long agCut)
    {
        Player playerLose = getPlayer(nameLose);
        Player playerWin = getPlayer(nameWin);
        playerLose.ag = agPlayerLose;
        playerWin.ag = agPlayerWin;
        playerLose.updateMoney();
        playerWin.updateMoney();
        playerLose.playerView.effectFlyMoney(-agCut);
        playerWin.playerView.effectFlyMoney(agCut);


    }
    public void updateMoney(string data)
    {
        JObject dataS = JObject.Parse(data);
        Player player = getPlayer(getString(dataS, "N"));
        if (player != null)
        {
            player.ag = getLong(dataS, "AG");
            player.updateMoney();
        }
    }
    private void FadeOutAndInCards()
    {
        foreach (var cardList in ListCardPlayer)
        {
            if (cardList != null)
            {
                foreach (var card in cardList)
                {
                    if (card != null)
                    {
                        CanvasGroup cg = card.GetComponent<CanvasGroup>();
                        if (cg == null)
                        {
                            cg = card.gameObject.AddComponent<CanvasGroup>();
                        }

                        // Set alpha về 0 ngay lập tức
                        cg.alpha = 0f;

                        // Sau 2.3s thì alpha về 1
                        DOVirtual.DelayedCall(2f, () =>
                        {
                            cg.alpha = 1f;
                        });
                    }
                }
            }
        }
    }

    public void finishGameTienLen(string strData)
    {
        // 1. Initial setup
        int type = -1;
        long moneyWin = 0;
        playSound(Globals.SOUND_GAME.ALERT);
        isFinish = true;
        JArray data = JArray.Parse(strData);
        foreach (var cardList in ListCardPlayerD)
        {
            if (cardList != null)
            {
                for (int i = cardList.Count - 1; i >= 0; i--)
                {
                    if (cardList[i] != null)
                    {
                        cardList[i].gameObject.SetActive(false);
                    }
                    cardList.RemoveAt(i);
                }
            }
        }
        FadeOutAndInCards();

        m_AniFinish.gameObject.SetActive(true);
        m_AniFinish.AnimationState.SetAnimation(0, "animation", true);
        m_AniCardSpecial.gameObject.SetActive(false);
        DOVirtual.DelayedCall(2.3f, () =>
        {
            if (m_AniFinish != null)
            {
                m_AniFinish.gameObject.SetActive(false);
            }
        });
        // 3. Process each player data
        Player playerSpecial = null;
        foreach (JObject playerData in data)
        {
            string playerName = getString(playerData, "N");
            Player player = getPlayer(playerName);

            if (player != null)
            {
                // Update player stats
                player.ag = getLong(playerData, "AG");
                player.point = getInt(playerData, "point");
                int typeWin = getInt(playerData, "TypeWin");

                // Set cards
                JArray arrCard = getJArray(playerData, "ArrCard");
                List<Card> playerCards = ListCardPlayer[players.IndexOf(player)];
                for (int i = 0; i < arrCard.Count; i++)
                {
                    playerCards[i].setTextureWithCode((int)arrCard[i]);
                }

                // Check special win
                if (typeWin > 0)
                {
                    type = typeWin;
                    playerSpecial = player;
                    m_AvatarSpecial.sprite = player.playerView.avatar.image.sprite;
                    m_NameWin.text = player.displayName;
                }
            }
        }

        // 4. Create finish sequence
        Sequence finishSequence = DOTween.Sequence();

        finishSequence.AppendInterval(0.5f);

        // Prepare finish
        finishSequence.AppendCallback(() =>
        {

            handleButton(false, false, false);
        });

        finishSequence.AppendInterval(3f);

        if (playerSpecial != null)
        {
            // Special win sequence
            finishSequence.AppendCallback(() =>
            {
                m_AniCardSpecial.gameObject.SetActive(true);
                switch (type)
                {
                    case 1: m_AniWinSpecial.AnimationState.SetAnimation(0, "four 2s", false); break;
                    case 2: m_AniWinSpecial.AnimationState.SetAnimation(0, "dragon", false); break;
                    case 3: m_AniWinSpecial.AnimationState.SetAnimation(0, "6 pairs", false); break;
                    case 4: m_AniWinSpecial.AnimationState.SetAnimation(0, "four triples", false); break;
                    case 5: m_AniWinSpecial.AnimationState.SetAnimation(0, "Fiveconsecutive", false); break;
                    case 6: m_AniWinSpecial.AnimationState.SetAnimation(0, "four 3s", false); break;
                }
            });

            finishSequence.AppendInterval(2f);

            // Show all cards
            finishSequence.AppendCallback(() => ShowAllCards(data));
            finishSequence.AppendInterval(1.5f);
        }
        else
        {
            // Normal finish - show cards directly
            finishSequence.AppendCallback(() => ShowAllCards(data));
            finishSequence.AppendInterval(3f);
        }

        // Money exchange trong finishGameTienLen
        finishSequence.AppendCallback(() =>
        {
            Vector3 centerPos = new Vector3(0, 0, 0); // Vị trí center
            Player winPlayer = null;

            // First pass: Xác định người thắng/thua và tạo chip bay từ người thua về center
            foreach (JObject playerData in data)
            {
                string playerName = getString(playerData, "N");
                Player player = getPlayer(playerName);
                long money = getLong(playerData, "M");

                if (money < 0)
                {
                    playSound(Globals.SOUND_GAME.LOSE);
                    player.playerView.effectFlyMoney(money);

                    float radius = 100f;
                    float delayStep = 0.08f; // delay giữa mỗi chip

                    Sequence chipSequence = DOTween.Sequence(); // Gộp tất cả chip vào 1 sequence

                    for (int i = 0; i < 8; i++)
                    {
                        GameObject chip = GetChipFromPool();
                        vtChipFinish.Add(chip);

                        // Vị trí xuất phát
                        chip.transform.position = player.playerView.transform.position;

                        // Tạo vị trí đích ngẫu nhiên quanh (0,0)
                        float angle = Random.Range(0f, Mathf.PI * 2);
                        float distance = Random.Range(0f, radius);
                        Vector2 randomOffset = new Vector2(
                            Mathf.Cos(angle) * distance,
                            Mathf.Sin(angle) * distance
                        );

                        Vector3 destination = new Vector3(randomOffset.x, randomOffset.y, 0f);

                        // Tạo tween riêng cho chip này
                        Tween moveTween = chip.transform.DOLocalMove(destination, 0.8f).SetEase(Ease.OutQuint);

                        // Thêm vào sequence với độ trễ tăng dần
                        chipSequence.Insert(i * delayStep, moveTween);
                    }
                    playSound(Globals.SOUND_GAME.THROW_CHIP);
                    chipSequence.Play(); // Bắt đầu chạy chuỗi hiệu ứng
                }

                else if (money > 0)
                {
                    moneyWin = money;
                    winPlayer = player;
                }
            }

            // Second pass: Sau 3s, tất cả chip từ center bay đến người thắng
            if (winPlayer != null)
            {
                DOVirtual.DelayedCall(2.5f, () =>
                {
                    playSound(Globals.SOUND_GAME.WIN);
                    winPlayer.playerView.effectFlyMoney(moneyWin);
                    PlayerViewTienlen playerViewTienlen = getPlayerView(winPlayer);
                    playerViewTienlen.ShowAniWin();
                    // Animation các chip bay từ center đến người thắng
                    playSound(Globals.SOUND_HILO.CHIP_WINNER);
                    for (int i = 0; i < vtChipFinish.Count; i++)
                    {
                        var chip = vtChipFinish[i];
                        float delay = i * 0.06f;
                        Vector3 positionPlayer = winPlayer.playerView.transform.position;

                        Sequence chipSequence = DOTween.Sequence();
                        chipSequence.AppendInterval(delay)
                            .Append(chip.transform
                                .DOMove(positionPlayer, 0.3f)
                                .SetEase(Ease.OutQuint))
                            .AppendCallback(() =>
                            {
                                ReturnChipToPool(chip);
                            });
                    }
                    vtChipFinish.Clear();
                });
            }

            // Update money display
            foreach (var player in players)
            {
                player.updateMoney();
            }
        });

        finishSequence.AppendInterval(5f);

        finishSequence.AppendCallback(() =>
        {
            m_AniFinish.gameObject.SetActive(false);
            m_AniCardSpecial.gameObject.SetActive(false);
            m_AniWinSpecial.gameObject.SetActive(false);

            // Tắt tất cả animation cháy
            foreach (var anim in m_ListAniChay)
            {
                if (anim != null)
                {
                    anim.gameObject.SetActive(false);
                }
            }

            selectedCards.Clear();
            turnNameCurrent = "";
            lastTurnName = "";
            isFinish = false;

            HandleFinishGame();
        });

        finishSequence.Play();
    }
    public void HandleFinishGame()
    {
        foreach (var cardList in ListCardPlayer)
        {
            if (cardList != null)
            {
                for (int i = cardList.Count - 1; i >= 0; i--)
                {
                    if (cardList[i] != null)
                    {
                        cardList[i].gameObject.SetActive(false);
                    }
                    cardList.RemoveAt(i);
                }
            }
        }
    }

    private void ShowAllCards(JArray data)
    {
        // Animation sequence cho việc lật bài
        Sequence showSequence = DOTween.Sequence();

        foreach (JObject playerData in data)
        {
            string playerName = getString(playerData, "N");
            Player player = getPlayer(playerName);
            int playerIndex = players.IndexOf(player);
            List<Card> playerCards = ListCardPlayer[playerIndex];
            if (playerCards.Count == 13 && ListCardPlayerD[playerIndex].Count == 0)
            {
                Debug.Log("có cháy bài");
                if (playerIndex >= 0 && playerIndex < m_ListAniChay.Count)
                {
                    var burnAnim = m_ListAniChay[playerIndex];
                    burnAnim.gameObject.SetActive(true);
                    burnAnim.AnimationState.SetAnimation(0, "animation", true);
                }
            }
            if (player == thisPlayer)
            {
                RearrangeCards();
                continue;
            }
            float spacing = 30f;
            float startX = playerIndex == 1 ? 498f : (playerIndex == 2 ? 40f : -498f);
            for (int i = 0; i < playerCards.Count; i++)
            {
                Card card = playerCards[i];
                float delay = i * 0.08f; // Delay giữa các lá bài
                showSequence.Insert(delay, card.transform.DOLocalMoveX((playerIndex == 1 || playerIndex == 2) ? startX - (i * spacing) : startX + (i * spacing), 0.1f));
                showSequence.Insert(delay, card.transform.DOScale(0.4f, 0.05f));
                card.transform.SetSiblingIndex((playerIndex == 1 || playerIndex == 2) ? 20 - i : i); // Đặt thứ tự hiển thị                                                                                       // Hiển thị bài và lật mặt
                card.gameObject.SetActive(true);
                card.setTextureWithCode(card.code); // Lật mặt bài lên
            }
        }
        showSequence.Play();
    }

    private void InitChipPool()
    {
        // Tạo sẵn một số lượng chip và cho vào pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject chip = Instantiate(m_PrefabChip, m_ContainerChip);
            chip.SetActive(false);
            chipPool.Enqueue(chip);
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

        // Khởi tạo animations cháy bài
        InitChipPool();
    }
    public static bool check3DoiThong(List<Card> listIn)
    {
        Debug.Log("⏺ Kiểm tra 3 đôi thông");

        var list = new List<Card>(listIn);

        // Yêu cầu đủ 6 lá
        if (list.Count != 6)
            return false;

        // Không được chứa quân 2 (giả sử N = 15 là quân 2)
        if (list.Any(card => card.N == 2))
        {
            Debug.Log("❌ Có chứa quân 2, không hợp lệ");
            return false;
        }

        // Sắp xếp tăng dần theo số
        list.Sort((x, y) => x.N.CompareTo(y.N));

        // Kiểm tra từng cặp đôi và tính liên tiếp
        for (int i = 0; i < 6; i += 2)
        {
            // Mỗi đôi phải là 2 lá bằng nhau
            if (list[i].N != list[i + 1].N)
            {
                Debug.Log($"❌ Không phải đôi tại vị trí {i}: {list[i].N}, {list[i + 1].N}");
                return false;
            }

            // Đảm bảo các đôi là liên tiếp nhau
            if (i < 4 && list[i].N + 1 != list[i + 2].N)
            {
                Debug.Log($"❌ Các đôi không liên tiếp: {list[i].N} -> {list[i + 2].N}");
                return false;
            }
        }

        Debug.Log("✅ Là 3 đôi thông hợp lệ");
        return true;
    }


    // Check 4 đôi thông
    public static bool check4DoiThong(List<Card> listIn)
    {
        Debug.Log("⏺ Kiểm tra 4 đôi thông");

        var list = new List<Card>(listIn);

        // Đúng 8 lá
        if (list.Count != 8)
            return false;

        // Không cho chứa quân 2 (giả sử N = 15)
        if (list.Any(card => card.N == 2))
        {
            Debug.Log("❌ Có chứa quân 2, không hợp lệ");
            return false;
        }

        // Sắp xếp theo số
        list.Sort((x, y) => x.N.CompareTo(y.N));

        // Kiểm tra từng đôi và tính liên tiếp
        for (int i = 0; i < 8; i += 2)
        {
            // Mỗi đôi phải là 2 lá bằng nhau
            if (list[i].N != list[i + 1].N)
            {
                Debug.Log($"❌ Không phải đôi tại vị trí {i}: {list[i].N}, {list[i + 1].N}");
                return false;
            }

            // Đảm bảo đôi hiện tại liên tiếp đôi sau (trừ đôi cuối)
            if (i < 6 && list[i].N + 1 != list[i + 2].N)
            {
                Debug.Log($"❌ Không liên tiếp giữa {list[i].N} và {list[i + 2].N}");
                return false;
            }
        }

        Debug.Log("✅ Là 4 đôi thông hợp lệ");
        return true;
    }


    // Check tứ quý
    public static bool checkTuQuy(List<Card> list)
    {
        if (list.Count < 4) return false;
        list.Sort((x, y) => x.N.CompareTo(y.N));

        for (int i = 0; i < list.Count - 1; i++)
        {
            int count = 0;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[j].N == list[i].N) count++;
            }
            if (count == 3) return true;
        }

        return false;
    }


    // Check bộ quân 2
    public static bool checkSetOfTwos(List<Card> list)
    {
        Debug.Log("⏺ Kiểm tra bộ quân 2");
        if (list.Count > 3 || list.Count < 2) return false;

        foreach (var card in list)
        {
            if (card.N != 2) return false;
        }
        return true;
    }

    // Check đôi Tiến Lên
    public static bool checkDoiTL(List<Card> listIn)
    {
        if (listIn.Count != 2) return false;
        return listIn[0].N == listIn[1].N;
    }

    // Check sám Tiến Lên
    public static bool checkXamTL(List<Card> listIn)
    {
        if (listIn.Count != 3) return false;
        return listIn[0].N == listIn[1].N && listIn[1].N == listIn[2].N;
    }

    // Check thùng phá sảnh
    public static bool checkThungPhaSanhTL(List<Card> list)
    {
        if (!checkSanhTL(list)) return false;

        // Check cùng chất
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].S != list[i + 1].S) return false;
        }
        return true;
    }

    // Check sảnh
    public static bool checkSanhTL(List<Card> list)
    {
        if (list.Count < 3) return false;

        list.Sort((x, y) => x.N.CompareTo(y.N));

        // Check liên tiếp
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].N + 1 != list[i + 1].N) return false;
        }
        return true;
    }

    private GameObject GetChipFromPool()
    {
        GameObject chip;
        if (chipPool.Count > 0)
        {
            chip = chipPool.Dequeue();
        }
        else
        {
            chip = Instantiate(m_PrefabChip, m_ContainerChip);
        }
        chip.SetActive(true);
        return chip;
    }

    private void ReturnChipToPool(GameObject chip)
    {
        if (chip != null)
        {
            chip.SetActive(false);
            chipPool.Enqueue(chip);
        }
    }
    private PlayerViewTienlen getPlayerView(Player player)
    {
        if (player != null)
        {
            return (PlayerViewTienlen)player.playerView;
        }
        return null;

    }
}