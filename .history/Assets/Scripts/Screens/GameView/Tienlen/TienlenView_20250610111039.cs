using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] private Transform m_ContainerChip;
    [SerializeField] private List<GameObject> m_ListScore;
    [SerializeField] private List<TextMeshProUGUI> m_NumberCardLast;

    public List<List<Card>> ListCardPlayer = new List<List<Card>>();
    public List<List<Card>> ListCardPlayerD = new List<List<Card>>();

    private string turnNameCurrent = "";

    private string lastTurnName = "";
    private int timeTurn = 0;
    private bool isPlayBool = false;

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
    private List<string> ListPlayerLeave = new List<string>();
    protected override void updatePositionPlayerView()
    {

        players.Remove(thisPlayer);
        players.Insert(0, thisPlayer);
        for (int i = 0; i < players.Count; i++)
        {

            if (i < listPosView.Count)
            {
                players[i].playerView.transform.localScale = new Vector2(0.8f, 0.8f);
                players[i].updatePlayerView();
                players[i].playerView.gameObject.SetActive(true);
                players[i].updateItemVip(players[i].vip);
                players[i].playerView.transform.localPosition = listPosView[i];
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
        isPlayBool = true;
        JArray ArrP = getJArray(data, "ArrP");

        // Clear existing cards first
        foreach (List<Card> cardList in ListCardPlayer)
        {
            cardList.Clear();
        }

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
                if (card != null)
                {
                    card.setTextureWithCode(cardCode);
                    card.gameObject.SetActive(true);
                    listCard.Add(card);

                    // Add touch events after adding to list
                    if (player == thisPlayer)
                    {
                        Debug.Log("Adding touch events for card: " + cardCode);
                        AddCardTouchEvents(card);
                    }
                }
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

        // Force rearrange cards after setup
        if (thisPlayer != null)
        {
            RearrangeCards();
        }
    }

    public Card spawnCard()
    {

        // Check tái sử dụng
        foreach (List<Card> cardList in ListCardPlayer)
        {
            foreach (Card card in cardList)
            {
                if (card != null && !card.gameObject.activeSelf)
                {
                    AddCardTouchEvents(card);
                    return card;
                }
            }
        }
        Card cardTemp = getCard();
        if (cardTemp == null)
        {
            return null;
        }

        cardTemp.setTextureWithCode(0);
        cardTemp.transform.localPosition = new Vector2(0f, 20f);
        cardTemp.transform.SetParent(m_ContainerCards);

        AddCardTouchEvents(cardTemp);
        return cardTemp;
    }

    private void AddCardTouchEvents(Card card)
    {
        if (card == null) return;

        // Check component Image
        Image img = card.GetComponent<Image>();
        if (img == null)
        {
            img = card.gameObject.AddComponent<Image>();
        }
        img.raycastTarget = true;

        // Check nếu là bài của người chơi chính
        int playerIndex = players.IndexOf(thisPlayer);
        if (playerIndex < 0 || playerIndex >= ListCardPlayer.Count || !ListCardPlayer[playerIndex].Contains(card))
        {
            Debug.Log("Card not in player's hand: " + card.code);
            return;
        }

        // Add EventTrigger
        EventTrigger trigger = card.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = card.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.Clear();

        // PointerDown
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) =>
        {

            OnCardTouch(card);

        });
        trigger.triggers.Add(entryDown);

        // Drag
        EventTrigger.Entry entryDrag = new EventTrigger.Entry();
        entryDrag.eventID = EventTriggerType.Drag;
        entryDrag.callback.AddListener((data) =>
        {
        });
        trigger.triggers.Add(entryDrag);

        // EndDrag
        EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
        entryEndDrag.eventID = EventTriggerType.EndDrag;
        entryEndDrag.callback.AddListener((data) =>
        {
        });
        trigger.triggers.Add(entryEndDrag);

        // Add CanvasGroup
        CanvasGroup cg = card.gameObject.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = card.gameObject.AddComponent<CanvasGroup>();
        }
        cg.blocksRaycasts = true;
        cg.interactable = true;

        Debug.Log("Added touch events for card: " + card.code);
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

        foreach (List<Card> cardList in ListCardPlayerD)
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
        foreach (GameObject img in m_TxtPass)
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
        setNumberCardLast();
    }
    private void setNumberCardLast(bool isFinish = false)
    {
        for (int i = 1; i < players.Count; i++)
        {
            if (ListCardPlayer[i].Count == 0 || isFinish || agTable > 1000)
            {
                m_NumberCardLast[i - 1].text = "";
            }
            else
            {
                m_NumberCardLast[i - 1].text = ListCardPlayer[i].Count.ToString();
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
    }

    public void initPlayerCard()
    {
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
            SetDiscardCardPosition(listCardD, position);
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
        List<int> cardCodeSpecial = new List<int>();
        bool isSpecialHand =
            check3DoiThong(cards) ||
            check4DoiThong(cards) ||
            checkTuQuy(cards) ||
            checkSetOfTwos(cards);

        Vector3 basePos = playerIndex switch
        {
            0 => new Vector3(0f, -60f, 0),
            1 => new Vector3(358f, 0f, 0),
            2 => new Vector3(20f, 120f, 0),
            3 => new Vector3(-358f, 0f, 0),
            _ => Vector3.zero
        };

        float spacing = 30f;

        // ✅ Hiện anim đặc biệt nếu có
        if (isSpecialHand)
        {
            m_AniCardSpecial.gameObject.SetActive(true);

            if (check3DoiThong(cards))
                m_AniCardSpecial.AnimationState.SetAnimation(0, "3 consecutive pairs", false);
            else if (check4DoiThong(cards))
                m_AniCardSpecial.AnimationState.SetAnimation(0, "4 consecutive pairs", false);
            else if (checkTuQuy(cards))
                m_AniCardSpecial.AnimationState.SetAnimation(0, "4 of a kind", false);
            else if (checkSetOfTwos(cards))
                m_AniCardSpecial.AnimationState.SetAnimation(0, "set of twos", false);

            DOVirtual.DelayedCall(2f, () =>
            {
                m_AniCardSpecial.gameObject.SetActive(false);
            });
            foreach (Card card in cards)
            {
                cardCodeSpecial.Add(card.code);
                card.setTextureWithCode(0);
            }
        }
        else
        {
            m_AniCardSpecial.gameObject.SetActive(false);
        }

        // ✅ Vị trí giữa bàn không chồng lá (dùng cho animation đặc biệt)
        float centerSpacing = 50f;
        float totalWidthCenter = (cards.Count - 1) * centerSpacing;
        Vector3 centerOrigin = Vector3.zero - new Vector3(totalWidthCenter / 2f, 0f, 0f);

        // Tính điểm giữa dãy bài cho player 0 và 2
        Vector3 discardStartPos = basePos;
        if (playerIndex == 0 || playerIndex == 2)
        {
            float totalWidthDiscard = (cards.Count - 1) * spacing;
            discardStartPos = basePos - new Vector3(totalWidthDiscard / 2f, 0f, 0f);
        }

        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];

            Vector3 finalPos;
            if (playerIndex == 0 || playerIndex == 2)
            {
                // Xếp thẳng hàng từ discardStartPos theo spacing, căn giữa dãy bài
                finalPos = discardStartPos + new Vector3(i * spacing, 0f, 0f);
            }
            else if (playerIndex == 1)
            {

                finalPos = new Vector3(basePos.x - i * spacing, basePos.y, basePos.z);
            }
            else
            {
                // Player 3 xếp từ basePos cộng offset sang phải
                finalPos = new Vector3(basePos.x + i * spacing, basePos.y, basePos.z);
            }

            Vector3 centerPos = centerOrigin + new Vector3(i * centerSpacing, 0f, 0f);

            card.transform.localRotation = Quaternion.identity;
            card.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            float delay = i * 0.03f;
            if (isSpecialHand)
            {
                int index = cardCodeSpecial[i];
                card.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                // bài úp quay 180°
                DOVirtual.DelayedCall(delay, () =>
 {
     float animTime = 0.3f;
     float moveToFinalTime = 0.3f;

     Sequence seq = DOTween.Sequence();

     seq.Insert(0f, DOVirtual.DelayedCall(0f, () => card.setEffect_Twinkle(true, 1.6f)));

     seq.Join(card.transform.DOLocalMove(centerPos, animTime).SetEase(Ease.OutQuad));
     seq.Join(card.transform.DOScale(Vector3.one, animTime));
     seq.Join(card.transform.DOLocalRotate(new Vector3(0f, 89f, 0f), animTime / 3f).SetEase(Ease.InOutSine));
     seq.AppendCallback(() =>
     {
         card.setTextureWithCode(index); // Đổi mặt tại 90°
     });
     // Xoay tiếp từ 90 -> 0
     seq.Append(card.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), animTime * 2 / 3f).SetEase(Ease.InOutSine));

     // 🏁 Move về vị trí cuối + scale nhỏ lại
     seq.AppendInterval(0.2f);
     seq.Append(
         DOTween.Sequence()
             .Join(card.transform.DOLocalMove(finalPos, moveToFinalTime).SetEase(Ease.OutBack))
             .Join(card.transform.DOScale(new Vector3(0.4f, 0.4f, 1f), moveToFinalTime))
     );

     // 🌀 Reset xoay (nếu dư)
     seq.Append(card.transform.DOLocalRotate(Vector3.zero, 0.1f));
 });

            }
            else
            {
                // Bình thường
                Sequence moveSeq = DOTween.Sequence();
                moveSeq.AppendInterval(delay);
                moveSeq.Append(card.transform.DOLocalMove(finalPos, 0.25f).SetEase(Ease.OutQuad));
            }

            // Set sibling index
            int siblingIndex = (playerIndex == 1)
                ? cards.Count - 1 - i
                : i;
            card.transform.SetSiblingIndex(siblingIndex);
            EventTrigger trigger = card.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                trigger.triggers.Clear();
                UnityEngine.Object.Destroy(trigger);
            }

            Image img = card.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;

            CanvasGroup cg = card.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.blocksRaycasts = false;
                cg.interactable = false;
            }
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
        isPlayBool = true;
        typeSort = 2;
        JArray arr = getJArray(data, "arr");
        timeTurn = getInt(data, "T");
        turnNameCurrent = getString(data, "nameturn");
        bool firstRound = getBool(data, "firstRound");
        TweenCallback start = () =>
        {
            playSound(Globals.SOUND_HILO.START_GAME);
            m_AniStart.gameObject.SetActive(true);
            m_AniStart.AnimationState.SetAnimation(0, "start", false);
            m_BgStart.SetActive(false);
        };
        Sequence mainSequence = DOTween.Sequence().AppendCallback(start);
        mainSequence.AppendInterval(0.8f);
        mainSequence.AppendCallback(() =>
                        {
                            m_AniStart.gameObject.SetActive(false);
                            Vector3 deckPosition = new Vector3(0f, 20f, 0f); // Vị trí bộ bài giữa bàn
                            float stackOffset = 0.16f; // Khoảng cách giữa các lá trong stack
                            for (int i = 0; i < players.Count; i++)
                            {
                                Player player = players[i];
                                int position = players.IndexOf(player);
                                ListCardPlayer[position].Clear();

                                for (int j = 0; j < arr.Count; j++)
                                {
                                    Card card = spawnCard();
                                    card.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                                    card.transform.localPosition = new Vector3(
                                        deckPosition.x,
                                        deckPosition.y - (stackOffset * (i * arr.Count + j)),
                                        0
                                    );

                                    card.transform.Rotate(0, 0, 90);
                                    ListCardPlayer[position].Add(card);
                                    AddCardTouchEvents(card);

                                    if (player == thisPlayer)
                                        card.setTextureWithCode((int)arr[j]);
                                    else
                                        card.setTextureWithCode(0);

                                    card.gameObject.SetActive(true);
                                }
                            }
                            chiaBai();

                        });
        mainSequence.Play();
    }





    private void chiaBai()
    {
        float dealTime = 0.25f;
        float dealDelay = 0.05f;

        Sequence dealSequence = DOTween.Sequence();

        for (int cardIndex = 0; cardIndex < 13; cardIndex++)
        {
            for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
            {

                int cardOrder = cardIndex * players.Count + playerIndex;
                float delay = cardOrder * dealDelay;

                Player player = players[playerIndex];
                List<Card> playerCards = ListCardPlayer[playerIndex];

                if (cardIndex >= playerCards.Count)
                    continue;

                Card card = playerCards[cardIndex];

                // Tính vị trí và scale cuối cùng
                Vector3 finalPos;
                Vector3 finalScale;

                if (player == thisPlayer)
                {
                    float spacing = 60f;
                    float totalWidth = (playerCards.Count - 1) * spacing;
                    float startX = -totalWidth / 2f;
                    finalPos = new Vector3(startX + (cardIndex * spacing), -250f, 0f);
                    finalScale = Vector3.one * 0.8f;
                }
                else
                {

                    switch (playerIndex)
                    {
                        case 1: finalPos = new Vector3(498f, 80f, 0); break;   // Phải
                        case 2: finalPos = new Vector3(40f, 250f, 0); break;   // Trên
                        case 3: finalPos = new Vector3(-498f, 80f, 0); break;  // Trái
                        default: finalPos = Vector3.zero; break;
                    }
                    finalScale = Vector3.one * 0.45f;
                }

                // Capture để tránh lỗi closure
                Card capturedCard = card;
                Vector3 capturedPos = finalPos;
                Vector3 capturedScale = finalScale;

                dealSequence.InsertCallback(delay, () =>
                {
                    playSound(Globals.SOUND_GAME.CARD_FLIP_1);
                    capturedCard.gameObject.SetActive(true);

                    // Góc nghiêng ban đầu
                    capturedCard.transform.localRotation = Quaternion.Euler(0, 0, 30f);

                    // Tween đến vị trí & xoay về đúng góc
                    capturedCard.transform.DOLocalMove(capturedPos, dealTime).SetEase(Ease.OutQuint);
                    capturedCard.transform.DOScale(capturedScale, dealTime).SetEase(Ease.OutQuint);
                    capturedCard.transform.DOLocalRotate(Vector3.zero, dealTime).SetEase(Ease.OutQuint);
                });
            }
        }

        dealSequence.OnComplete(() =>
        {

            SortCard();
            setNumberCardLast();
            Player playerFirst = getPlayer(turnNameCurrent);
            if (playerFirst != null)
            {
                playerFirst.setTurn(true, timeTurn);
            }
            if (turnNameCurrent == thisPlayer.namePl)
            {
                handleButton(false, true, true);
            }

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


        if (newTurn)
        {
            // Clear last turn name và bài đã đánh
            lastTurnName = "";

            // Clear bài đã đánh của tất cả người chơi
            foreach (List<Card> cardList in ListCardPlayerD)
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
        if (indexPos >= 0 && indexPos < m_TxtPass.Count)
        {
            GameObject obj = m_TxtPass[indexPos];

            // Delay 0.5s trước khi hiển thị và bắt đầu hiệu ứng
            DOVirtual.DelayedCall(0.3f, () =>
            {
                obj.SetActive(true);

                // Reset trạng thái ban đầu
                obj.transform.localScale = Vector3.zero;

                // Tăng scale lên 0.7 và alpha từ 0 -> 1
                obj.transform.DOScale(0.7f, 0.5f).SetEase(Ease.OutBack);

                // Nếu có CanvasGroup để điều chỉnh alpha
                CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.DOFade(1f, 0.5f);
                }

                // Auto hide sau 1s kể từ lúc hiện
                DOVirtual.DelayedCall(1f, () =>
                {
                    if (m_TxtPass[indexPos] != null)
                    {
                        obj.SetActive(false);
                    }
                });
            });
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
            m_ButtonCancel.SetActive(false);
            m_ButtonDiscard.transform.localPosition = new Vector3(0, m_ButtonDiscard.transform.localPosition.y, 0);
        }

    }
    public void OnCardTouch(Card card)
    {
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
                List<Card> cards = group.Take(isDouble ? 2 : 3).ToList();
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
            List<Card> otherSuggestedCards = listCardSuggest
                .Where(card => card != selectedCard && !selectedCards.Contains(card))
                .ToList();

            // Add các lá còn lại vào danh sách selectedCards
            foreach (Card card in otherSuggestedCards)
            {
                selectedCards.Add(card);
                card.transform.DOLocalMoveY(-210f, 0.2f)
                    .SetEase(Ease.OutQuint);
            }
        }

        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
    }


    private void ClearSuggestion()
    {
        listCardSuggest.Clear();
        m_ButtonDiscard.GetComponent<Button>().interactable = selectedCards.Count > 0;
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
                Vector3 newPos = cardStartPos;
                newPos.x += (dragPos.x - touchStartPos.x);
                newPos.x = Mathf.Clamp(newPos.x, -400f, 400f);
                selectedCard.transform.localPosition = newPos;
                HandleCardSwapping();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            float releaseDistance = Vector3.Distance(Input.mousePosition, touchStartPos);
            if (releaseDistance <= dragThreshold)
            {
                ToggleCardSelection(selectedCard);
            }
            else
            {
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

        if (newIndex != currentIndex)
        {

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
    }

    public override void handleRJTable(string data)
    {
        isPlayBool = true;
        base.handleRJTable(data);
        JObject dataS = JObject.Parse(data);
        connectGame(dataS);
        RearrangeCards();
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
        foreach (List<Card> cardList in ListCardPlayer)
        {
            if (cardList != null)
            {
                foreach (Card card in cardList)
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
        setNumberCardLast(true);
        HandleData.DelayHandleLeave = 8f;
        // 1. Initial setup
        int type = -1;
        long moneyWin = 0;
        playSound(Globals.SOUND_GAME.ALERT);
        JArray data = JArray.Parse(strData);
        foreach (List<Card> cardList in ListCardPlayerD)
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
        SkeletonGraphic skeleton = m_AniFinish.GetComponent<SkeletonGraphic>();
        skeleton.timeScale = 1.4f;
        m_AniFinish.AnimationState.SetAnimation(0, "animation", true);
        m_AniCardSpecial.gameObject.SetActive(false);
        DOVirtual.DelayedCall(1.8f, () =>
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
        Sequence finishSequence = DOTween.Sequence();

        finishSequence.AppendInterval(0.5f);
        finishSequence.AppendCallback(() =>
        {
            handleButton(false, false, false);
        });

        finishSequence.AppendInterval(2f);
        finishSequence.AppendCallback(() => ShowAllCards(data));
        finishSequence.AppendInterval(1.5f);
        finishSequence.AppendCallback(() =>
        {
            Vector3 centerPos = new Vector3(0, 0, 0);
            Player winPlayer = null;
            foreach (JObject playerData in data)
            {
                string playerName = getString(playerData, "N");
                Player player = getPlayer(playerName);
                long money = getLong(playerData, "M");

                if (money < 0)
                {
                    playSound(Globals.SOUND_GAME.LOSE);
                    player.playerView.effectFlyMoney(money);

                    float radius = 50f;
                    float delayStep = 0.04f; // delay giữa mỗi chip

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
            if (winPlayer != null)
            {
                DOVirtual.DelayedCall(2.2f, () =>
                {
                    playSound(Globals.SOUND_GAME.WIN);
                    winPlayer.playerView.effectFlyMoney(moneyWin);
                    PlayerViewTienlen playerViewTienlen = getPlayerView(winPlayer);
                    playerViewTienlen.ShowAniWin();
                    // Animation các chip bay từ center đến người thắng
                    playSound(Globals.SOUND_HILO.CHIP_WINNER);
                    for (int i = 0; i < vtChipFinish.Count; i++)
                    {
                        GameObject chip = vtChipFinish[i];
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
            foreach (Player player in players)
            {
                player.updateMoney();
            }

        });

        finishSequence.AppendInterval(3.6f);

        finishSequence.AppendCallback(() =>
        {

            m_AniFinish.gameObject.SetActive(false);
            m_AniCardSpecial.gameObject.SetActive(false);
            m_AniWinSpecial.gameObject.SetActive(false);

            // Tắt tất cả animation cháy
            foreach (SkeletonGraphic anim in m_ListAniChay)
            {
                if (anim != null)
                {
                    anim.gameObject.SetActive(false);
                }
            }
            foreach (GameObject score in m_ListScore)
            {
                if (score != null)
                {
                    score.SetActive(false);
                }
            }
            isPlayBool = false;
            selectedCards.Clear();
            turnNameCurrent = "";
            lastTurnName = "";
            HandleFinishGame();

        });

        finishSequence.Play();
    }
    public void HandleFinishGame()
    {

        foreach (Transform chip in m_ContainerChip)
        {
            chip.gameObject.SetActive(false);
        }
        foreach (List<Card> cardList in ListCardPlayer)
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
        foreach (string player in ListPlayerLeave)
        {
            if (players.Contains(getPlayer(player)))
            {
                removePlayer(player);
            }

        }
        ListPlayerLeave.Clear();
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
                if (playerIndex >= 0 && playerIndex < m_ListAniChay.Count)
                {
                    SkeletonGraphic burnAnim = m_ListAniChay[playerIndex];
                    burnAnim.gameObject.SetActive(true);
                    burnAnim.AnimationState.SetAnimation(0, "animation", true);
                }
            }
            if ((int)playerData["point"] != 0)
            {
                m_ListScore[playerIndex].SetActive(true);
                m_ListScore[playerIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ((int)playerData["point"]).ToString();
            }
            if (player == thisPlayer)
            {
                continue;
            }
            float spacing = 30f;
            float startX = playerIndex == 1 ? 498f : (playerIndex == 2 ? 40f : -498f);
            List<(Card card, float xPos)> cardWithPositions = new List<(Card, float)>();

            for (int i = 0; i < playerCards.Count; i++)
            {
                Card card = playerCards[i];
                float delay = i * 0.01f;

                // Tính vị trí X mục tiêu
                float xPos = (playerIndex == 1 || playerIndex == 2)
                    ? startX - (i * spacing)
                    : startX + (i * spacing);

                // Thêm vào danh sách tạm để sắp thứ tự z-index sau
                cardWithPositions.Add((card, xPos));

                // Tạo animation
                showSequence.Insert(delay, card.transform.DOLocalMoveX(xPos, 0.1f));
                showSequence.Insert(delay, card.transform.DOScale(0.4f, 0.05f));

                card.gameObject.SetActive(true);
                card.setTextureWithCode(card.code); // Lật mặt bài
            }

            // Sắp xếp theo vị trí thực tế từ trái qua phải (x tăng dần)
            cardWithPositions.Sort((a, b) => a.xPos.CompareTo(b.xPos));

            // Đặt z-index theo thứ tự trái → phải
            for (int i = 0; i < cardWithPositions.Count; i++)
            {
                cardWithPositions[i].card.transform.SetSiblingIndex(i);
            }

        }
        showSequence.Play();
    }

    public override void handleLTable(JObject data)
    {
        Debug.Log("có chạy vào hàm ltable");
        string namePl = (string)data["Name"];
        Player player = getPlayer(namePl);
        if (isPlayBool && player != thisPlayer)
        {
            ListPlayerLeave.Add(namePl);
            Debug.Log("chưa đc thoát bàn");
            return;
        }
        Debug.Log("thoát bàn");

        if (player == null) return;

        if (player != thisPlayer)
        {
            removePlayer(namePl);
        }

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
        isPlayBool = false;
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

        List<Card> list = new List<Card>(listIn);

        // Yêu cầu đủ 6 lá
        if (list.Count != 6)
            return false;

        // Không được chứa quân 2 (giả sử N = 15 là quân 2)
        if (list.Any(card => card.N == 2))
        {
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
                return false;
            }

            // Đảm bảo các đôi là liên tiếp nhau
            if (i < 4 && list[i].N + 1 != list[i + 2].N)
            {
                return false;
            }
        }
        return true;
    }


    // Check 4 đôi thông
    public static bool check4DoiThong(List<Card> listIn)
    {
        List<Card> list = new List<Card>(listIn);

        // Đúng 8 lá
        if (list.Count != 8)
            return false;

        // Không cho chứa quân 2 (giả sử N = 15)
        if (list.Any(card => card.N == 2))
        {
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
                return false;
            }

            // Đảm bảo đôi hiện tại liên tiếp đôi sau (trừ đôi cuối)
            if (i < 6 && list[i].N + 1 != list[i + 2].N)
            {
                return false;
            }
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