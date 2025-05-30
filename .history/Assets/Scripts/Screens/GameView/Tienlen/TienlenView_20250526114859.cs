using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        Player player = getPlayer(turnName);
        turnNameCurrent = nextTurn;
        lastTurnName = turnName;
        int position = players.IndexOf(player);

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

        // 3. Process playing cards
        List<Card> listCard = ListCardPlayer[position];
        List<Card> listCardD = ListCardPlayerD[position];

        // 4. Check special combinations and animate
        bool isSpecialHand = false;
        if (check3DoiThong(listCardD) ||
            check4DoiThong(listCardD) ||
            checkTuQuy(listCardD) ||
            checkSetOfTwos(listCardD))
        {
            isSpecialHand = true;
            Sequence specialSequence = DOTween.Sequence();

            specialSequence.AppendInterval(0.1f);
            specialSequence.AppendCallback(() =>
            {
                Debug.Log("có chạy bào chỗ show cái special");
                m_AniCardSpecial.gameObject.SetActive(true);

                if (check3DoiThong(listCardD))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "3 consecutive pairs", false);
                }
                else if (check4DoiThong(listCardD))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "4 consecutive pairs", false);
                    m_AniWinSpecial.gameObject.SetActive(true);
                    m_AniWinSpecial.AnimationState.SetAnimation(0, "dragon", false);
                }
                else if (checkTuQuy(listCardD))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "4 of a kind", false);
                }
                else if (checkSetOfTwos(listCardD))
                {
                    m_AniCardSpecial.AnimationState.SetAnimation(0, "6 pairs", false);
                }
            });

            specialSequence.AppendInterval(2f);
            specialSequence.AppendCallback(() =>
            {
                ProcessPlayCards(player, listCard, listCardD, vtCard, position);
            });
            specialSequence.Play();
        }
        else
        {
            ProcessPlayCards(player, listCard, listCardD, vtCard, position);
        }
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
                    SetDiscardCardPosition(cardToPlay, position, i);
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
                    SetDiscardCardPosition(cardToPlay, position, i);
                    cardToPlay.gameObject.SetActive(true);
                    listCardD.Add(cardToPlay);
                }
            }
        }
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

    private void SetDiscardCardPosition(Card card, int playerIndex, int cardIndex)
    {
        // Reset card state first
        card.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
        card.transform.localRotation = Quaternion.identity;

        Vector3 basePos = new Vector3(0f, 0f, 0);
        float offset = cardIndex * 30f;

        switch (playerIndex)
        {
            case 0: // Người chơi chính
                basePos = new Vector3(0f, -60f, 0);
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
          playerIndex == 1 ? basePos.x - offset : basePos.x + offset,
            basePos.y,
            basePos.z
        );
        card.transform.SetSiblingIndex(playerIndex == 1 ? 20 - cardIndex : cardIndex);


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
                        deckPosition.y - (stackOffset * (i * arr.Count + j)),
                        0
                    );

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
        float dealDelay = 0.04f;

        Sequence dealSequence = DOTween.Sequence();

        for (int cardIndex = 0; cardIndex < 13; cardIndex++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                List<Card> playerCards = ListCardPlayer[i];

                if (cardIndex < playerCards.Count)
                {
                    Card card = playerCards[cardIndex];
                    float delay = (cardIndex * players.Count + i) * dealDelay;

                    Vector3 finalPos = Vector3.zero;
                    float finalScale = 0.45f;

                    if (player == thisPlayer)
                    {
                        float spacing = 60f;
                        float totalWidth = (playerCards.Count - 1) * spacing;
                        float startX = -totalWidth / 2f;
                        finalPos = new Vector3(startX + (cardIndex * spacing), -250f, 0f);
                        finalScale = 0.8f;
                    }
                    else
                    {
                        switch (i)
                        {
                            case 1: finalPos = new Vector3(498f, 80f, 0f); break; // phải
                            case 2: finalPos = new Vector3(40f, 250f, 0f); break; // trên
                            case 3: finalPos = new Vector3(-498f, 80f, 0f); break; // trái
                        }
                    }

                    dealSequence.InsertCallback(delay, () =>
                    {
                        // Hiện lá bài
                        card.gameObject.SetActive(true);
                        // Scale + fade-in + move (combo đẹp)
                        card.transform.DOLocalMove(finalPos, dealTime).SetEase(Ease.OutQuart);
                        card.transform.DOScale(finalScale, dealTime).SetEase(Ease.OutBack);
                    });
                }
            }
        }

        dealSequence.OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                onClickSortCard();
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
            m_TxtPass[indexPos].SetActive(true);
            StartCoroutine(HidePassAfterDelay(m_TxtPass[indexPos], 2f));
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
            m_AniCardSpecial.gameObject.SetActive(false);
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
    private IEnumerator HidePassAfterDelay(GameObject passObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (passObject != null)
        {
            passObject.SetActive(false);
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

    // Hàm xử lý touch bài
    public void OnCardTouch(Card card)
    {
        Debug.Log($"[OnCardTouch] Card touched: {card.code}");

        touchStartPos = Input.mousePosition;
        cardStartPos = card.transform.localPosition;
        selectedCard = card;
        isDragging = true;
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
        float raisedY = -210f;    // Vị trí y khi nâng lên
        float animDuration = 0.2f;

        bool isCurrentlySelected = selectedCards.Contains(card);

        if (isCurrentlySelected)
        {
            // Bỏ chọn lá bài
            selectedCards.Remove(card);
            card.transform.DOLocalMoveY(normalY, animDuration)
                .SetEase(Ease.OutQuint);
        }
        else
        {
            // Chọn lá bài
            selectedCards.Add(card);
            card.transform.DOLocalMoveY(raisedY, animDuration)
                .SetEase(Ease.OutQuint);
        }

        // Enable/disable nút đánh bài
        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
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
            float dealTime = 0.15f;
            Card card = playerCards[i];
            float spacing = 60f;
            float totalWidth = (playerCards.Count - 1) * spacing;
            float startX = -totalWidth / 2f;
            Vector3 finalPos = new Vector3(startX + (i * spacing), -250f, 0f);
            Vector3 finalScale = new Vector3(0.8f, 0.8f, 1f);
            playerCards[i].transform.DOLocalMove(finalPos, dealTime)
           .SetEase(Ease.OutQuint);
            playerCards[i].transform.DOScale(finalScale, dealTime);
            card.transform.SetSiblingIndex(i + 50);
        }

        // Đảm bảo lá đang được chọn luôn nằm trên cùng
        if (selectedCard != null)
        {
            // Z-index cao nhất cho lá được chọn
            // selectedCard.transform.SetSiblingIndex(baseZIndex + playerCards.Count + 1);

            // Nếu lá bài được chọn (nâng lên), điều chỉnh y position
            if (selectedCards.Contains(selectedCard))
            {
                selectedCard.transform.localPosition = new Vector3(
                    selectedCard.transform.localPosition.x,
                    -230f, // Nâng cao hơn 20 đơn vị
                    0
                );
            }
        }
    }
    public void onClickDanhBai()
    {

        JArray arrCard = new JArray();
        for (int i = 0; i < selectedCards.Count; i++)
        {
            arrCard.Add(selectedCards[i].code);
        }
        SocketSend.danhBai(arrCard);

        // 5. Clear selected cards array
        selectedCards.Clear();

        handleButton(false, false, false);


    }
    public void onClickBoLuot()
    {
        SocketSend.boLuot();
        handleButton(false, false, false);
    }

    public void onClickSortCard()
    {
        // Play sort sound
        //  playSound(Globals.SOUND_GAME.CARD_SORT);

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
    public override void handleLTable(JObject data)
    {
        base.handleLTable(data);
        if (players.Count <= 1)
        {
            countDownTimeToStart(0);
        }
    }
    public void handleJTable(string data, int time)
    {
        base.handleJTable(data);
        countDownTimeToStart(time);
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
        playerLose.playerView.effectFlyMoney(agCut);
        playerWin.playerView.effectFlyMoney(-agCut);


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
    public void finishGameTienLen(string strData)
    {
        // 1. Initial setup
        int type = -1;
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
        // Check người chơi nào không đánh được lá bài nào
        foreach (JObject playerData in data)
        {
            string playerName = getString(playerData, "N");
            Player player = getPlayer(playerName);

            if (player != null)
            {
                int playerIndex = players.IndexOf(player);
                List<Card> playerCards = ListCardPlayer[playerIndex];
                List<Card> discardedCards = ListCardPlayerD[playerIndex];

                // Nếu số lá bài trên tay vẫn = 13 (full bài) và chưa đánh lá nào (discarded = 0)
                if (playerCards.Count == 13 && discardedCards.Count == 0)
                {
                    // Play animation cháy bài
                    if (playerIndex >= 0 && playerIndex < m_ListAniChay.Count && m_ListAniChay[playerIndex] != null)
                    {
                        var anim = m_ListAniChay[playerIndex];
                        anim.gameObject.SetActive(true);
                        anim.AnimationState.SetAnimation(0, "animation", true);

                        // Auto hide sau khi animation kết thúc
                        anim.AnimationState.Complete += (entry) =>
                        {
                            anim.gameObject.SetActive(false);
                        };
                    }
                }
            }
        }

        // 2. Setup animation finish
        m_AniFinish.gameObject.SetActive(true);
        m_AniFinish.AnimationState.SetAnimation(0, "animation", false);
        m_AniCardSpecial.gameObject.SetActive(false);

        // 3. Process each player data
        Player playerSpecial = null;
        foreach (JObject playerData in data)
        {
            string playerName = getString(playerData, "N");
            Player player = getPlayer(playerName);

            if (player != null)
            {
                // Update player stats
                player.ag = getLong(playerData, "M");
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

        finishSequence.AppendInterval(1f);

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

            finishSequence.AppendInterval(2.5f);

            // Show all cards
            finishSequence.AppendCallback(() => ShowAllCards(data));
            finishSequence.AppendInterval(2.5f);
        }
        else
        {
            // Normal finish - show cards directly
            finishSequence.AppendCallback(() => ShowAllCards(data));
            finishSequence.AppendInterval(4f);
        }

        // Money exchange
        finishSequence.AppendCallback(() =>
        {
            foreach (JObject playerData in data)
            {
                string playerName = getString(playerData, "N");
                Player player = getPlayer(playerName);
                player.playerView.effectFlyMoney(player.ag);
                player.updateMoney();
            }

        });

        finishSequence.AppendInterval(4f);

        // Final cleanup
        finishSequence.AppendCallback(() =>
        {
            m_AniFinish.gameObject.SetActive(false);
            m_AniCardSpecial.gameObject.SetActive(false);
            m_AniWinSpecial.gameObject.SetActive(false);

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

            if (player != null && player != thisPlayer)
            {
                int playerIndex = players.IndexOf(player);
                List<Card> playerCards = ListCardPlayer[playerIndex];

                // Setup vị trí và scale cho bài
                float spacing = 30f;
                float startX = playerIndex == 1 ? 498f : (playerIndex == 2 ? 40 : -498f); // Bên phải hoặc bên trái

                for (int i = 0; i < playerCards.Count; i++)
                {
                    Card card = playerCards[i];
                    float delay = i * 0.05f; // Delay giữa các lá bài

                    showSequence.Insert(delay, card.transform.DOLocalMoveX((playerIndex == 1 || playerIndex == 2) ? (startX - (i * spacing)) : (startX + (i * spacing)), 0.3f));
                    card.transform.SetSiblingIndex((playerIndex == 1 || playerIndex == 2) ? i : 20 - i);

                    showSequence.Insert(delay, card.transform.DOScale(0.4f, 0.3f));
                    showSequence.Insert(delay, card.transform.DORotate(Vector3.zero, 0.3f));

                    // Hiển thị bài và lật mặt
                    card.gameObject.SetActive(true);
                    card.setTextureWithCode(card.code); // Lật mặt bài lên
                }

                // Check bài cháy (13 lá không đánh được)
                if (playerCards.Count == 13 && ListCardPlayerD[playerIndex].Count == 0)
                {
                    if (playerIndex >= 0 && playerIndex < m_ListAniChay.Count)
                    {
                        var burnAnim = m_ListAniChay[playerIndex];
                        burnAnim.gameObject.SetActive(true);
                        burnAnim.AnimationState.SetAnimation(0, "animation", true);
                    }
                }
            }
        }
        showSequence.Play();
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
        InitAniChay();
    }

    private void InitAniChay()
    {
        // Khởi tạo và setup position cho mỗi animation
        for (int i = 0; i < m_ListAniChay.Count; i++)
        {
            if (m_ListAniChay[i] != null)
            {
                var anim = m_ListAniChay[i];
                anim.gameObject.SetActive(false);

                // Set vị trí animation theo vị trí người chơi
                switch (i)
                {
                    case 0: // Người chơi chính
                        anim.transform.localPosition = new Vector3(0f, -60f, 0f);
                        break;
                    case 1: // Bên phải
                        anim.transform.localPosition = new Vector3(358f, 0f, 0f);
                        break;
                    case 2: // Đối diện
                        anim.transform.localPosition = new Vector3(20f, 120f, 0f);
                        break;
                    case 3: // Bên trái  
                        anim.transform.localPosition = new Vector3(-358f, 0f, 0f);
                        break;
                }
            }
        }
    }
    public static bool check3DoiThong(List<Card> listIn)
    {
        var list = new List<Card>(listIn);
        if (list.Count != 6) return false;

        // Sort theo number
        list.Sort((x, y) => x.N.CompareTo(y.N));

        // Check các đôi liên tiếp 
        for (int i = 0; i < list.Count - 2; i += 2)
        {
            // Check đôi
            if (list[i].N != list[i + 1].N) return false;
            // Check liên tiếp
            if (i < list.Count - 2 && list[i].N + 1 != list[i + 2].N) return false;
        }

        return true;
    }

    // Check 4 đôi thông
    public static bool check4DoiThong(List<Card> listIn)
    {
        var list = new List<Card>(listIn);
        if (list.Count != 8) return false;

        list.Sort((x, y) => x.N.CompareTo(y.N));

        for (int i = 0; i < list.Count - 2; i += 2)
        {
            if (list[i].N != list[i + 1].N) return false;
            if (i < list.Count - 2 && list[i].N + 1 != list[i + 2].N) return false;
        }

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
}