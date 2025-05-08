using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;
using System;
using System.Linq;
using Globals;
using System.Threading;

public class DummyDataCustom
{
    public List<Card> cardLeft = new List<Card>();
    public List<Card> lsLayoff = new List<Card>();
    public JArray allLayoff = new JArray();

    public List<List<Card>> allMeld = new List<List<Card>>();

    public void LogToString()
    {
        var str = "***********allMeld";
        allMeld.ForEach((arr) =>
        {
            arr.ForEach((it) =>
            {
                str += " " + it.N + it.getSuitInVN();
            });
            str += "\n";
        });
        str += "***********cardLeft";
        cardLeft.ForEach(it => str += " " + it.N + it.getSuitInVN());

        str += "***********allLayoff";
        str += allLayoff.ToString();
        Logging.Log(str);
    }

    public void LogCodeToString()
    {
        var str = "CODE***********allMeld";
        allMeld.ForEach((arr) =>
        {
            arr.ForEach((it) =>
            {
                str += " " + it.code;
            });
            str += "\n";
        });
        str += "***********cardLeft";
        cardLeft.ForEach(it => str += " " + it.code);

        str += "***********allLayoff";
        str += allLayoff.ToString();
        Logging.Log(str);
    }
}

public class DummyView : GameView
{
    //public enum STATE_MODE
    //{
    //    1 eat, 2 meld, 3 layoff, 4 knock out, 5 show special
    //}
    Vector3 PLAYERSCALE = new Vector3(0.8f, 0.8f, 0.8f);
    Vector3 OTHERSCALE = new Vector3(0.3f, 0.3f, 0.3f);
    Vector3 TABLESCALE = new Vector3(0.4f, 0.4f, 0.4f);

    [SerializeField]
    SkeletonGraphic animStart;
    [SerializeField]
    Button btnSort;

    [HideInInspector]
    public int currenMode = 0; // 0 normal, 1 eat, 2 meld, 3 layoff
    [SerializeField]
    GameObject nodeOrder = null, dropPanel = null, nodeArrow = null;

    [SerializeField]
    BaseView nodeRulePot;

    [SerializeField]
    DummyResult nodeResult;

    [SerializeField]
    TextMeshProUGUI cardDeck, txtPot, txtRoundPot;

    [SerializeField]
    List<GameObject> listBgDropZone = new List<GameObject>();

    [SerializeField]
    List<GameObject> listDropLayout = new List<GameObject>();

    [SerializeField]
    List<SkeletonDataAsset> listAnim = new List<SkeletonDataAsset>();
    [SerializeField]
    List<SkeletonGraphic> listNodeAnim = new List<SkeletonGraphic>();
    [SerializeField]
    SkeletonGraphic lblTextDown = null;

    [SerializeField]
    GameObject nodeAnim = null;
    [SerializeField]
    DummyItemTag nodeItemTag = null;
    [SerializeField]
    List<GameObject> listBoxResult = new List<GameObject>();
    [SerializeField]
    List<TextMeshProUGUI> listBoxResult_lb = new List<TextMeshProUGUI>();

    [SerializeField]
    Transform hitpot = null, BgHitpot = null;

    List<Card> listDumpCard = new List<Card>();

    [HideInInspector]
    public List<Card> selectedCards = new List<Card>();
    [HideInInspector]
    public List<Card> selectedDumps = new List<Card>();
    [HideInInspector]
    public List<Card> selectedLayoff = new List<Card>();

    int currentSort = 0;
    Player currentPlayer = null;
    bool isState1 = true;
    List<List<Card>> allPossibleMeld = new List<List<Card>>();
    List<Card> listCanEat = new List<Card>();
    string currentDataLayoff = null;
    //	resultPanel = nodeResult.getComponent('DummyResult');
    DummyDataCustom dataKnockOut;// = new DummyDataCustom();
    List<List<Card>> dataKnockOut_AllMeld = new List<List<Card>>();
    //	listTagKnock = [];
    bool isKnockOut = false;
    int roundHitpot = 0, currentTopCard = 0;
    //	result = [];
    JObject lastWin = null;
    List<bool> listAniDummy = new List<bool>() { false, false, false, false };
    //	langLocal = cc.sys.localStorage.getItem("language_client");
    //	lblTextDown.getComponent(sp.Skevaron).skevaronData = langLocal == LANGUAGE_TEXT_CONFIG.LANG_THAI? listAnim[9] : listAnim[8];

    [SerializeField]
    TextMeshProUGUI CountDownTime;

    [SerializeField]
    DummyButton buttonManager;
    JArray result = new JArray();

    Card cardTempOnFocus = null;
    bool isHoldCard = false;
    //float distance = 0;
    //int idTouch = -1;


    [SerializeField]
    Transform cardParent;
    [SerializeField]
    Transform clockWise;

    protected override void Awake()
    {
        base.Awake();
        updateCardDeck(0);
        setInfoPot(0);
        nodeOrder.SetActive(false);
        buttonManager.onHide();

        btnSort.gameObject.SetActive(false);
    }

    void onClickCardMe(Card card)
    {
        Logging.Log("onClickCardMe currenMode " + currenMode);
        if (currenMode == 4) return;
        if (isHoldCard) return;
        cardTempOnFocus = card;
        handleCard(card);
    }

    void onClickCardDump(Card card)
    {
        if (currenMode == 4) return;
        cardTempOnFocus = null;
        handleDumpedCard(card);
    }

    void OnBeginDrag(PointerEventData eventData, Card card)
    {
        Logging.Log("OnBeginDrag currenMode " + currenMode);
        if (currenMode == 4) return;
        isHoldCard = false;
        //cardTempOnFocus = card;
        //if (!cardTempOnFocus.isAllowTouch || currenMode == 4)
        //{
        //    cardTempOnFocus = null;
        //    return;
        //}
    }

    void OnDrag(PointerEventData eventData, Card card)
    {
        if (currenMode == 4) return;

        isHoldCard = true;

        var posTouch = eventData.position - new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
        var pos = card.transform.localPosition;
        pos.x = -posTouch.y;

        var posMin = getMyCardPosition(0).x - card.GetComponent<RectTransform>().rect.width * .5f * PLAYERSCALE.x;
        var posMax = getMyCardPosition(thisPlayer.vectorCard.Count - 1).x + card.GetComponent<RectTransform>().rect.width * .5f * PLAYERSCALE.x;
        if (pos.x > posMax)
            pos.x = posMax;
        if (pos.x < posMin)
            pos.x = posMin;
        card.transform.localPosition = pos;

        var indexTemp = thisPlayer.vectorCard.IndexOf(card);
        if (eventData.delta.y > 0)
        {
            var index1Temp = indexTemp - 1;
            if (index1Temp >= 0)
            {
                var cardTemp = thisPlayer.vectorCard[index1Temp];
                if (cardTemp.transform.localPosition.x > pos.x)
                {
                    var posC = getMyCardPosition(indexTemp);
                    //cardTemp.transform.localPosition = posC;
                    cardTemp.transform.DOLocalMove(posC, 0.2f);
                    var temp = thisPlayer.vectorCard[indexTemp];
                    thisPlayer.vectorCard[indexTemp] = thisPlayer.vectorCard[index1Temp];
                    thisPlayer.vectorCard[index1Temp] = temp;

                    var tempZindex = cardTemp.transform.GetSiblingIndex();
                    cardTemp.transform.SetSiblingIndex(card.transform.GetSiblingIndex());
                    card.transform.SetSiblingIndex(tempZindex);
                }
            }
        }
        else
        {
            var index1Temp = indexTemp + 1;
            if (index1Temp < thisPlayer.vectorCard.Count)
            {
                var cardTemp = thisPlayer.vectorCard[index1Temp];
                if (cardTemp.transform.localPosition.x < pos.x)
                {
                    //isHoldCard = true;
                    var posC = getMyCardPosition(indexTemp);
                    //cardTemp.transform.localPosition = posC;

                    cardTemp.transform.DOLocalMove(posC, 0.2f);

                    var temp = thisPlayer.vectorCard[indexTemp];
                    thisPlayer.vectorCard[indexTemp] = thisPlayer.vectorCard[index1Temp];
                    thisPlayer.vectorCard[index1Temp] = temp;

                    var tempZindex = cardTemp.transform.GetSiblingIndex();
                    cardTemp.transform.SetSiblingIndex(card.transform.GetSiblingIndex());
                    card.transform.SetSiblingIndex(tempZindex);
                }
            }
        }
    }

    void OnEndDrag(PointerEventData eventData, Card card)
    {
        if (currenMode == 4) return;
        if (isHoldCard)
            resetPosCard();

        isHoldCard = false;
    }

    private Guid uid_onClickSort;
    //uid_foldCardUp = Guid.NewGuid();
    public void onClickSort()
    {
        if (currenMode == 4) return;
        SoundManager.instance.soundClick();
        btnSort.interactable = false;
        buttonManager.showDummyWarn(false);
        DOTween.Kill(uid_onClickSort);
        DOTween.Sequence().AppendInterval(0.4f).AppendCallback(() =>
        {
            btnSort.interactable = true;
        }).id = uid_onClickSort;
        var vectorCard = thisPlayer.vectorCard;

        if (currentSort == 0)
        {
            currentSort = 1;
            thisPlayer.vectorCard.Sort((a, b) =>
            {
                return a.N == b.N ? a.S - b.S : a.N - b.N;
            });
        }
        else
        {
            currentSort = 0;
            thisPlayer.vectorCard.Sort((a, b) =>
            {
                return a.S == b.S ? a.N - b.N : a.S - b.S;
            });
        }
        selectedCards.Clear();
        resetPosCard();
        checkAllowButton();
    }

    public void onClickHistory()
    {
        nodeResult.transform.SetAsLastSibling();
        nodeResult.onShow(result);
    }

    public void onClickPot()
    {
        nodeRulePot.transform.SetAsLastSibling();
        nodeRulePot.show();
    }

    public override void handleVTable(string strData)
    {
        resetGame();
        base.handleVTable(strData);
        UIManager.instance.showToast(Config.getTextConfig("txt_view_table"), transform);
        setDataView(strData);

    }

    public override void handleRJTable(string strData)
    {
        resetGame();
        base.handleRJTable(strData);
        JObject data = JObject.Parse(strData);
        thisPlayer.isSpecialQb2T = !(bool)data["canShow"];
        setDataView(strData);
    }

    void setDataView(string strData)
    {
        JObject data = JObject.Parse(strData);
        roundHitpot = (int)data["round"];
        currentTopCard = (int)data["topCard"];
        JArray ArrP = (JArray)data["ArrP"];
        foreach (JObject playerData in ArrP)
        {
            var player = getPlayerWithID((int)playerData["id"]);
            var len = ArrP.Count;

            JArray lstMeld = (JArray)playerData["lstMeld"];
            foreach (JObject meldData in lstMeld)
            {
                var panel = Instantiate(dropPanel, listDropLayout[player._indexDynamic].transform);
                panel.transform.localScale = Vector3.one;

                panel.GetComponent<Button>().onClick.AddListener(() =>
                {
                    onClickDGroup(player._indexDynamic + "-" + (int)meldData["idMeld"]);
                });

                List<Card> listDrop = new List<Card>();
                JArray arrMeld = (JArray)meldData["arrMeld"];
                foreach (int cardCode in arrMeld)
                {
                    var cardTemp = getCard(panel.transform);
                    //cardTemp.transform.localScale = Vector3.one;
                    var pos = cardTemp.transform.localPosition;
                    pos.y = 0;
                    cardTemp.transform.localPosition = pos;
                    cardTemp.setTextureWithCode(cardCode, true);
                    if (cardCode == currentTopCard) cardTemp.showTarget(true);
                    listDrop.Add(cardTemp);
                };
                player.vectorCardD2.Add(listDrop);

            }

            sortCardMeld(listDropLayout[player._indexDynamic].transform);

            JArray ArrCard = (JArray)playerData["Arr"];
            if (player != thisPlayer)
            {
                createOtherCard(player, ArrCard.Count);
            }
            else
            {
                // spawn card for thisplayer
                btnSort.gameObject.SetActive(true);
                for (var i = 0; i < ArrCard.Count; i++)
                {
                    var cardTemp = getCard(cardParent);
                    cardTemp.transform.localScale = PLAYERSCALE;
                    cardTemp.setTextureWithCode((int)ArrCard[i]);
                    var pos = getMyCardPosition(i);
                    cardTemp.transform.SetSiblingIndex(i);
                    thisPlayer.vectorCard.Add(cardTemp);
                    cardTemp.transform.localPosition = pos;
                    cardTemp.isAllowTouch = true;
                    cardTemp.addButtonClick(onClickCardMe);
                    cardTemp.setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
                }
            }

            ((PlayerViewDummy)player.playerView).updateKaengPoint((int)playerData["point"]);
        }
        isState1 = !((bool)data["isDraw"]);
        //cc.NGWlog("=====>", data.gameStatus, '><', data.gameStatus != "FINISHED")


        if (!((string)data["gameStatus"]).Equals("FINISHED")) setPlayerTurn((int)data["CN"], (int)data["CT"]);
        updateCardDeck((int)data["deckSize"]);
        JArray listCardShow = (JArray)data["listCardShow"];

        for (var i = 0; i < listCardShow.Count; i++)
        {
            var cardCode = (int)listCardShow[i];
            var cardTemp = getCard(cardParent);
            cardTemp.setTextureWithCode(cardCode);
            if (cardCode == currentTopCard) cardTemp.showTarget(true);

            var pos = getDumpCardPosition(i);
            listDumpCard.Add(cardTemp);
            cardTemp.transform.localScale = TABLESCALE;
            cardTemp.transform.localPosition = pos;
            cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
            cardTemp.isAllowTouch = false;
        }
        if (currentPlayer == thisPlayer)
            checkAllowButton(true);


        resetDumpCardPosition();
        JArray lstWinning = (JArray)data["lstWinning"];

        if (lstWinning.Count > 0)
        {
            hitpot.GetChild(0).gameObject.SetActive(true);
        }
        else
            hitpot.GetChild(0).gameObject.SetActive(false);

        setInfoPot((int)data["pot"]);

        for (var i = 0; i < ArrP.Count; i++)
        {
            var playerData = (JObject)ArrP[i];


            if ((int)playerData["numberMatchWin"] > 0)
            {
                lastWin = new JObject();
                lastWin["numberMatchWin"] = (int)playerData["numberMatchWin"];
                lastWin["id"] = (int)playerData["id"];
                lastWin["pname"] = (string)playerData["displayName"];

                hitpot.GetChild(0).gameObject.SetActive(true);
                var player = getPlayerWithID((int)playerData["id"]);
                ((PlayerViewDummy)player.playerView).updatePotDummy(1);
            }
        }


        if (data.ContainsKey("isClockwise"))
            showPlayOrder((bool)data["isClockwise"], false);
    }

    public override void handleSTable(string strData)
    {
        //	cc.NGWlog('!==> handle stable', strData)
        resetGame();
        base.handleSTable(strData);
        //	_super(strData);
        //	// setpot
        var data = JObject.Parse(strData);
        roundHitpot = (int)data["round"];
        JArray ArrP = (JArray)data["ArrP"];

        for (var i = 0; i < ArrP.Count; i++)
        {
            JObject plData = (JObject)ArrP[i];

            var player = getPlayerWithID((int)plData["id"]);
            if ((int)plData["numberMatchWin"] != 0)
            {
                ((PlayerViewDummy)player.playerView).updatePotDummy(1);
                hitpot.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                ((PlayerViewDummy)player.playerView).updatePotDummy(0);
            }
        }
    }

    public override void handleLTable(JObject data)
    {
        if (players.Count <= 1)
        {
            CountDownTime.gameObject.SetActive(false);
        }

        if (lastWin != null)
            if (lastWin.ContainsKey("pname") && ((string)lastWin["pname"]).Equals((string)data["Name"]))
            {
                hitpot.GetChild(0).gameObject.SetActive(false);
                hitpot.GetChild(1).gameObject.SetActive(false);
            }
        base.handleLTable(data);
    }

    int timeStart = 0;
    public void handleStartGame(JObject data)
    {
        //	// {"evt":"startGame","timeAction":3
        UIManager.instance.gameView.stateGame = STATE_GAME.WAITING;

        resetGame();
        if (nodeResult.getIsShow())
            nodeResult.hide(false);
        if (!data.ContainsKey("timeAction")) return;
        CountDownTime.gameObject.SetActive(true);
        timeStart = (int)data["timeAction"];
        CountDownTime.text = timeStart + "";
        CountDownTime.transform.localScale = new Vector3(3, 3, 3);
        CountDownTime.transform.DOScale(1, 0.3f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1);
        seq.AppendCallback(() =>
        {
            timeStart--;
            if (timeStart <= 0)
            {
                animStart.gameObject.SetActive(true);
                animStart.AnimationState.SetAnimation(0, "starttime_thai", false);
                animStart.Initialize(true);

                DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
                {
                    animStart.gameObject.SetActive(false);
                });
                CountDownTime.gameObject.SetActive(false);
                return;
            }
            CountDownTime.text = timeStart + "";
            CountDownTime.transform.localScale = new Vector3(3, 3, 3);
            CountDownTime.transform.DOScale(1, 0.3f);
        });
        seq.SetLoops(timeStart);
        seq.SetAutoKill(true);
    }

    void resetGame()
    {
        foreach (var player in players)
        {
            player.isFold = false;
            player.isSpecial = false;
            foreach (var cardTemp in player.vectorCard)
            {
                removerCard(cardTemp);
            }
            foreach (var arr in player.vectorCardD2)
            {
                foreach (var cardTemp in arr)
                {
                    removerCard(cardTemp);
                }
            }

            player.vectorCard.Clear();
            player.vectorCardD2.Clear();
            ((PlayerViewDummy)player.playerView).updateKaengPoint(-1, true);
            UIManager.instance.destroyAllChildren(listDropLayout[player._indexDynamic].transform);
            UIManager.instance.destroyAllChildren(cardParent);
            hitpot.parent = BgHitpot;
            hitpot.localPosition = new Vector3(-79.8f, 0, 0);
            hitpot.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "03", true);
            dataKnockOut = new DummyDataCustom();
        }

        foreach (var cardTemp in listDumpCard)
        {
            removerCard(cardTemp);
        }
        foreach (var bkg in listBoxResult)
        {
            UIManager.instance.destroyAllChildren(bkg.transform);
            bkg.SetActive(false);
        }

        //isSendDataKnockOut = false;
        isKnockOut = false;
        listDumpCard.Clear();
        isState1 = true;
        btnSort.gameObject.SetActive(false);
        currentTopCard = 0;
        currenMode = 0;
        updateCardDeck(0);
        showDropZone(-1);
    }

    public void handleLc(JObject data)
    {

        //{"evt":"lc","data":{"arrCard":[39,23,9,8,52,5,31,21,32,33,25],"isClockwise":false,"topCard":51},"timeAction":3,"nextTurn":1765662,"cardDeck":29,"turnTime":7}
        //var data = strData.data;
        resetGame();
        if (nodeResult.getIsShow())
            nodeResult.hide(false);
        btnSort.gameObject.SetActive(false);
        stateGame = STATE_GAME.PLAYING;
        JObject dataDT = (JObject)data["data"];
        JArray arrCard = (JArray)dataDT["arrCard"];
        var len = arrCard.Count;
        currentPlayer = null;
        foreach (var player in players)
        {
            player.isSpecialQb2T = false;
            player.isFold = false;
            if (player == thisPlayer)
            {
                for (var i = 0; i < len; i++)
                {
                    var cardTemp = getCard(cardParent);
                    cardTemp.transform.localScale = PLAYERSCALE;
                    cardTemp.isAllowTouch = false;
                    var pos = getMyCardPosition(i);
                    cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
                    thisPlayer.vectorCard.Add(cardTemp);
                    cardTemp.transform.localPosition = new Vector2(pos.x, pos.y + 120);
                    var idCard = (int)arrCard[i];

                    cardTemp.transform.DOKill();
                    Sequence seq = DOTween.Sequence();
                    seq.AppendInterval(0.1f * i + 1);
                    seq.Append(cardTemp.transform.DOLocalMove(pos, 0.6f));
                    seq.AppendCallback(() =>
                    {
                        foldCardUp(cardTemp, idCard);
                    });

                    cardTemp.addButtonClick(onClickCardMe);

                    cardTemp.setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
                }
            }
            else
            {
                createOtherCard(player, len);
            }
            ((PlayerViewDummy)player.playerView).updateKaengPoint(0);
        }
        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(1.0f);
        seq2.AppendCallback(() =>
        {
            updateCardDeck((int)data["cardDeck"]);
            handleFirstCard((int)dataDT["topCard"]);
        });
        seq2.AppendInterval(2.0f);
        seq2.AppendCallback(() =>
        {
            btnSort.gameObject.SetActive(true);
            currentSort = 0;
            onClickSort();
        });
        seq2.AppendInterval(0.5f);
        seq2.AppendCallback(() =>
        {
            setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        });

        showPlayOrder((bool)dataDT["isClockwise"]);
    }

    void showPlayOrder(bool isClockwise, bool isAnim = true)
    {

        clockWise.localScale = new Vector3(isClockwise ? -1 : 1, 1, 1);
        if (!isAnim) return;
        var caOp = nodeOrder.GetComponent<CanvasGroup>();
        if (caOp == null)
        {
            caOp = nodeOrder.AddComponent<CanvasGroup>();
        }
        caOp.DOKill();

        caOp.alpha = 0;
        nodeOrder.transform.localScale = new Vector3(isClockwise ? -1 : 1, 1, 1);

        var angle = isClockwise ? -360 : 360;
        nodeOrder.transform.rotation = new Quaternion(0, 0, 0, 0);
        nodeOrder.transform.DOLocalRotate(new Vector3(0, 0, angle), 1, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
        Sequence seq = DOTween.Sequence();
        seq.Append(caOp.DOFade(1.0f, 0.2f));
        seq.AppendInterval(1f);
        seq.Append(caOp.DOFade(0, 0.4f));
        seq.AppendCallback(() =>
        {
            nodeOrder.SetActive(false);
        });
    }

    void showDropZone(int index)
    {
        foreach (var bg in listBgDropZone)
        {
            bg.SetActive(false);
        }
        if (index < 0) return;
        listBgDropZone[index].SetActive(true);
        if (currentPlayer == null || currentPlayer._indexDynamic != index)
        {
            var oppp = listBgDropZone[index].GetComponent<CanvasGroup>();
            if (oppp == null)
            {
                oppp = listBgDropZone[index].AddComponent<CanvasGroup>();
            }
            oppp.alpha = 0;
            oppp.DOFade(1, 4f);
            return;
        }
    }

    void setPlayerTurn(int pId, int time = 7)
    {
        Logging.Log("-=-  setPlayerTurn");
        foreach (var pl in players)
        {
            pl.setTurn(false);
        }
        var player = getPlayerWithID(pId);
        if (player == null) return;
        player.setTurn(true, time);
        showDropZone(player._indexDynamic);
        if (player == currentPlayer)
        {
            if (player.isFold == false) player.isFold = true;
        }
        currentPlayer = player;
        if (player == thisPlayer && stateGame == STATE_GAME.PLAYING)
        {
            Logging.Log("-=-= di vao day buttonManager.gameObject.SetActive(true) " + isState1);
            buttonManager.gameObject.SetActive(true);
            buttonManager.onShow(isState1);
            checkAllowButton(true);
            currenMode = 0;
            var dmm = checkIsLcSpecial(player);
            Debug.Log("-=-= checkIsLcSpecial   " + dmm);
            Debug.Log("-=-= player.isSpecialQb2T   " + player.isSpecialQb2T);
            if (dmm && !player.isSpecialQb2T)
            {
                player.isSpecialQb2T = true;
                DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
                {
                    buttonManager.onClickEnterMode(5);
                });
                return;
            }

            player.isSpecialQb2T = true;
        }
        else
        {
            isState1 = true;
            buttonManager.onHide();
        }

        Logging.Log("-=-=buttonManager   " + buttonManager.getIsShow());
        if (buttonManager.getIsShow())
        {
            Logging.Log("-=-=buttonManager   đang ẩn");
            onEnterMode(0);
        }
        showArrow(false);
    }

    public void handleDraw(JObject data)
    {
        //// {"evt":"draw","data":51,"pid":1765715,"nextTurn":1765715,"cardDeck":28,"turnTime":7}
        //require('SoundManager1').instance.dynamicallyPlayMusic(ResDefine.sound_fold);
        var player = getPlayerWithID((int)data["pid"]);
        var _cardTemp1 = getCard(cardParent);

        _cardTemp1.transform.localScale = TABLESCALE;

        var pos = cardDeck.transform.parent.localPosition;
        _cardTemp1.transform.localPosition = pos;

        DOTween.Sequence().Append(_cardTemp1.transform.DOLocalMoveY(pos.y - 80, 0.3f)).AppendCallback(() =>
        {
            removerCard(_cardTemp1);
        });
        if (player == thisPlayer)
        {
            if (isState1 == true) isState1 = false;
            selectedCards.Clear();
            resetPosCard();
            var _cardTemp2 = getCard(cardParent);
            _cardTemp2.setTextureWithCode((int)data["data"]);
            thisPlayer.vectorCard.Add(_cardTemp2);
            _cardTemp2.addButtonClick(onClickCardMe);

            _cardTemp2.setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
            _cardTemp2.transform.localScale = PLAYERSCALE;
            var cardPos = getMyCardPosition(thisPlayer.vectorCard.Count - 1);

            _cardTemp2.transform.localPosition = new Vector3(cardPos.x, cardPos.y + 150);
            _cardTemp2.transform.SetSiblingIndex((int)GAME_ZORDER.Z_CARD + thisPlayer.vectorCard.Count);
            DOTween.Sequence().AppendInterval(0.3f)
               .Append(_cardTemp2.transform.DOLocalMove(cardPos, .2f).SetEase(Ease.OutCubic))
               .AppendCallback(() =>
               {
                   _cardTemp2.isAllowTouch = true;
                   checkAllowButton(true);
               }).AppendInterval(0.1f).AppendCallback(() =>
               {
                   selectedCards.Clear();
                   resetPosCard();
               });
        }
        else
        {
            var coutCa = ((PlayerViewDummy)player.playerView).getCardCount() + 1;
            ((PlayerViewDummy)player.playerView).updateNumCard(coutCa, Vector3.one, false);
        }

        updateCardDeck((int)data["cardDeck"]);

        //DOTween.Sequence().AppendInterval(1).AppendCallback(() => {
        setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        //});
    }

    public void handleDiscard(JObject data)
    {
        //        {
        //            "evt": "disCard",
        //  "data": 34,
        //  "pid": 8072,
        //  "nextTurn": 5271,
        //  "cardHand": 11,
        //  "showCard": [
        //    52,
        //    34
        //  ],
        //  "turnTime": 12
        //}
        //// {"evt":"disCard","data":30,"pid":1765715,"nextTurn":1815863,"cardHand":12,"showCard":[14,30],"turnTime":7}
        ///
        var vectorCard = thisPlayer.vectorCard;
        var player = getPlayerWithID((int)data["pid"]);
        if (player == thisPlayer)
        {
            selectedCards.Clear();
            for (var i = 0; i < vectorCard.Count; i++)
            {
                if (vectorCard[i].code == (int)data["data"])
                {
                    var card = vectorCard[i];
                    card.transform.DOKill();
                    Sequence seq = DOTween.Sequence();
                    seq.Append(card.transform.DOLocalMoveY(card.transform.localPosition.y + 150, 0.3f).SetEase(Ease.OutCubic));
                    seq.AppendCallback(() =>
                    {
                        removerCard(card);
                    });
                    vectorCard.RemoveAt(i);
                    break;
                }
            }
            resetPosCard();
            showTagKnock(false, -1, Vector2.zero);
        }
        else
        {
            updateCardOnHand(player, (int)data["cardHand"]);
            if (player.isSpecial && ((int)data["data"] == 11 || (int)data["data"] == 14))
            {
                //List<Card> allCard = new List<Card>(player.vectorCard);//[...player.vectorCard]
                for (var i = 0; i < player.vectorCard.Count; i++)
                {
                    if (player.vectorCard[i].code == (int)data["data"])
                    {
                        var card = player.vectorCard[i];
                        player.vectorCard.RemoveAt(i);
                        removerCard(card);
                        i--;
                    }
                }
            }
        }

        var time = player == thisPlayer ? 0.2f : 0;
        DOTween.Sequence().AppendInterval(time).AppendCallback(() =>
        {
            var cardTemp = getCard(cardParent);
            cardTemp.setTextureWithCode((int)data["data"]);
            if ((int)data["data"] == currentTopCard) cardTemp.showTarget(true);
            var pos = getDumpCardPosition(listDumpCard.Count - 1);
            listDumpCard.Add(cardTemp);
            var zOrder = (int)GAME_ZORDER.Z_CARD + listDumpCard.Count - 1;

            if ((int)data["cardHand"] < 1)
            {
                var zonePos = listBgDropZone[player._indexDynamic].transform.localPosition;
                pos = new Vector2(zonePos.x, zonePos.y + 67);

                zOrder = (int)GAME_ZORDER.Z_CARD + 20;
            }
            cardTemp.transform.localScale = TABLESCALE;
            cardTemp.transform.localPosition = pos;
            cardTemp.transform.SetSiblingIndex(zOrder);
            cardTemp.isAllowTouch = false;
            cardTemp.showSmoke(true);

            cardTemp.addButtonClick(onClickCardDump);
            checkAllowButton(true);
            if ((int)data["cardHand"] > 0)
            {
                setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
                resetDumpCardPosition();
            }
        });
    }

    public void handleLayoff(JObject data2)
    {
        //        {
        //            "evt": "layoff",
        //  "data": {
        //                "idCardSend": 46,
        //    "idMeld": 2,
        //    "sendtoPid": 5271,
        //    "point": 95
        //  },
        //  "pid": 5271,
        //  "nextTurn": 5271,
        //  "cardHand": 2,
        //  "turnTime": 12
        //}
        // {"evt":"layoff","data":{"idCardSend":29,"idMeld":0,"sendtoPid":4220315,"point":5},"pid":4220315,"nextTurn":4220315,"cardHand":10,"turnTime":7}
        var playerSend = getPlayerWithID((int)data2["pid"]);
        playSound(SOUND_DUMMY.MELD);
        JObject data = (JObject)data2["data"];
        ((PlayerViewDummy)playerSend.playerView).updateKaengPoint((int)data["point"]);

        var player = getPlayerWithID((int)data["sendtoPid"]);

        var nodePanel = listDropLayout[player._indexDynamic].transform.GetChild((int)data["idMeld"]);
        var cardTemp = getCard(nodePanel);
        cardTemp.setTextureWithCode((int)data["idCardSend"], true);
        if ((int)data["idCardSend"] == currentTopCard) cardTemp.showTarget(true);
        //var poss = cardTemp.transform.localPosition;
        //poss.y = 0;
        //cardTemp.transform.localPosition = poss;
        //cardTemp.transform.localScale = Vector3.zero;
        cardTemp.setColor(playerSend == thisPlayer ? 1 : 2);
        updateCardOnHand(playerSend, (int)data2["cardHand"]);
        var arrayCard = player.vectorCardD2[(int)data["idMeld"]];
        arrayCard.Add(cardTemp);
        arrayCard.Sort((a, b) =>
        {
            return a.S.CompareTo(b.S);
        });
        arrayCard.Sort((a, b) =>
        {
            return a.N.CompareTo(b.N);
        });

        if (playerSend == thisPlayer)
        {
            foreach (var _cardTemp in playerSend.vectorCard)
            {
                if (_cardTemp.code == (int)data["idCardSend"])
                {
                    playerSend.vectorCard.Remove(_cardTemp);
                    DOTween.Sequence().Append(_cardTemp.transform.DOLocalMove(new Vector3(_cardTemp.transform.localPosition.x, _cardTemp.transform.localPosition.y + 60), 0.2f)).AppendCallback(() =>
                    {
                        removerCard(_cardTemp);
                    });
                    break;
                }
            }
            selectedCards.Clear();
            selectedDumps.Clear();
            resetPosCard();
        }
        else
        {
            if (playerSend.isSpecial == true && ((int)data["idCardSend"] == 11 || (int)data["idCardSend"] == 14))
            {
                //var allCard = [...playerSend.vectorCard]
                var allCard = new List<Card>(playerSend.vectorCard);
                foreach (var cardTemp2 in allCard)
                {
                    if (cardTemp2.code == (int)data["idCardSend"])
                    {
                        playerSend.vectorCard.Remove(cardTemp2);
                        removerCard(cardTemp2);
                    }
                }
            }
        }

        //Logging.Log("-=-= 2 vectorCard D2");
        //player.vectorCardD2.ForEach(it => Logging.Log(it.Count));
        sortCardMeld(nodePanel.transform.parent);
        setPlayerTurn((int)data2["nextTurn"], (int)data2["turnTime"]);
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
        {
            showTextDown(player._indexDynamic, (int)data["idMeld"], 3);
        });
    }

    public void handleMeld(JObject data)
    {
        var meldData = (JObject)data["data"];
        var player = getPlayerWithID((int)meldData["pid"]);
        handleDropCardTable(data, false);
        ((PlayerViewDummy)player.playerView).updateKaengPoint((int)meldData["point"]);
        if (player == thisPlayer)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.3f);
            seq.AppendCallback(() =>
            {
                selectedCards.Clear();
                selectedDumps.Clear();
                resetPosCard();
            });
        }
        else
        {
            updateCardOnHand(player, (int)data["cardHand"]);
        }
        setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
    }

    public void handleEat(JObject data)
    {
        // {"evt":"eat","data":{"pid":1815671,"point":15,"nextTurn":1815671,"meldObject":{"idMeld":0,"arrMeld":[30,32,31]}},"nextTurn":1815671,"cardHand":11,"showCard":[10,34,11,20,49],"turnTime":7}

        var meldData = (JObject)data["data"];
        var addToHands = ((JArray)meldData["addToHands"]).ToObject<List<int>>();
        var meldObject = (JObject)meldData["meldObject"];
        var arrMeld = ((JArray)meldObject["arrMeld"]).ToObject<List<int>>();//an,ha phom
        var showCard = ((JArray)data["showCard"]).ToObject<List<int>>();

        var player = getPlayerWithID((int)meldData["pid"]);
        foreach (var code in arrMeld)
        {
            if (code == currentTopCard)
            {
                showAnim(false, 4, player._indexDynamic);
                showTag(player, 6, "+50");
                break;
            }
        }
        handleDropCardTable(data, true);
        ((PlayerViewDummy)player.playerView).updateKaengPoint((int)meldData["point"]);
        for (var i = 0; i < listDumpCard.Count; i++)
        {
            var cardTemp = listDumpCard[i];
            if (showCard.FindIndex(x => x == cardTemp.code) == -1)
            {
                listDumpCard.RemoveAt(i);
                i--;
                var pos = cardTemp.transform.localPosition;
                DOTween.Sequence().Append(cardTemp.transform.DOLocalMove(new Vector3(pos.x, pos.y + 40), 0.2f)).AppendCallback(() =>
                {
                    removerCard(cardTemp);
                });
            }
        }

        if (player == thisPlayer)
        {
            if (isState1) isState1 = false;
            var len = addToHands.Count;
            var currentLength = player.vectorCard.Count;
            for (var i = 0; i < len; i++)
            {
                var cardTemp = getCard(cardParent);
                var pos = getMyCardPosition(currentLength + i);
                cardTemp.transform.localPosition = new Vector3(pos.x, pos.y + 150);
                cardTemp.transform.localScale = PLAYERSCALE;
                cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + player.vectorCard.Count);
                cardTemp.setTextureWithCode(addToHands[i]);
                if (addToHands[i] == currentTopCard) cardTemp.showTarget(true);
                player.vectorCard.Add(cardTemp);
                cardTemp.addButtonClick(onClickCardMe);

                cardTemp.setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
                DOTween.Sequence().AppendInterval(0.1f).Append(cardTemp.transform.DOLocalMove(pos, 0.2f)).AppendCallback(() =>
                {
                    cardTemp.isAllowTouch = true;
                });
            }
            DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
            {
                selectedCards.Clear();
                selectedDumps.Clear();
                resetPosCard();
            });

        }
        else
        {
            updateCardOnHand(player, (int)data["cardHand"]);
        }
        //DOTween.Sequence().AppendInterval(1).AppendCallback(() => {
        setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        //});

    }
    void sortCardMeld(Transform layout)
    {
        DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
        {
            var sizeParent = layout.GetComponent<RectTransform>().sizeDelta;
            var rate = layout.localPosition.x < 0 ? -1 : 1;//GetComponent<RectTransform>().pivot.x == 0 ? -1 : 1;
            var px = rate * sizeParent.x * 0.5f;
            var pxMax = -rate * sizeParent.x * 0.5f;
            var py = sizeParent.y * 0.5f;
            for (var i = 0; i < layout.childCount; i++)
            {
                var child = layout.GetChild(i);
                var sizeChild = child.GetComponent<RectTransform>().sizeDelta;

                if (i == 0)
                {
                    py -= sizeChild.y * .5f;
                }
                px += -rate * sizeChild.x * 0.5f;
                if (rate == 1 ? (px - sizeChild.x * 0.5f) < pxMax : (px + sizeChild.x * 0.5f) > pxMax)
                {
                    px = rate * sizeParent.x * 0.5f;
                    px += -rate * sizeChild.x * 0.5f;
                    py -= (sizeChild.y + 20);
                }
                child.localPosition = new Vector3(px, py, 0);
                px += -rate * (sizeChild.x * 0.5f + 20);

                List<Card> listTemp = new List<Card>();
                for (var j = 0; j < child.childCount; j++)
                {
                    var card = child.GetChild(j).GetComponent<Card>();
                    listTemp.Add(card);
                }

                listTemp.Sort((a, b) =>
                {
                    return a.S.CompareTo(b.S);
                });
                listTemp.Sort((a, b) =>
                {
                    return a.N.CompareTo(b.N);
                });

                for (var j = 0; j < listTemp.Count; j++)
                {
                    var poss = listTemp[j].transform.localPosition;
                    poss.y = 0;
                    listTemp[j].transform.localPosition = poss;
                    listTemp[j].transform.SetSiblingIndex(j);
                }
            }
        });
    }


    public void handleDropCardTable(JObject data, bool isEat = true)
    {
        var meldData = (JObject)data["data"];
        var player = getPlayerWithID((int)meldData["pid"]);
        playSound(SOUND_DUMMY.MELD);
        var arrCode = ((JObject)meldData["meldObject"])["arrMeld"].ToObject<List<int>>();

        //arrCode.Sort((a, b) =>
        //{
        //    int N1 = 0, S1 = 0;
        //    int N2 = 0, S2 = 0;
        //    Config.decodeCard(a, ref N1, ref S1);
        //    Config.decodeCard(b, ref N2, ref S2);
        //    return S1.CompareTo(S2);
        //});

        //arrCode.Sort((a, b) =>
        //{
        //    int N1 = 0, S1 = 0;
        //    int N2 = 0, S2 = 0;
        //    Config.decodeCard(a, ref N1, ref S1);
        //    Config.decodeCard(b, ref N2, ref S2);
        //    return N1.CompareTo(N2);
        //});
        List<Card> listDrop = new List<Card>();
        var layout = listDropLayout[player._indexDynamic];
        var panel = Instantiate(dropPanel, layout.transform);

        panel.GetComponent<Button>().onClick.AddListener(() =>
        {
            onClickDGroup(player._indexDynamic + "-" + meldData["meldObject"]["idMeld"]);
        });

        for (var i = 0; i < arrCode.Count; i++)
        {
            var code = arrCode[i];
            //int N1 = 0, S1 = 0;
            //Config.decodeCard(code, ref N1, ref S1);
            //Logging.Log("-=-= code: " + code + " N: " + N1 + " S: " + S1);

            for (var j = 0; j < listDumpCard.Count; j++)
            {
                var cardTem = listDumpCard[j];

                if (cardTem.code == code)
                {
                    listDumpCard.RemoveAt(j);
                    j--;
                    DOTween.Sequence().Append(cardTem.transform.DOLocalMoveY(cardTem.transform.localPosition.y + 40, 0.2f)).AppendCallback(() =>
                    {
                        removerCard(cardTem);
                    });
                    cardTem.transform.DOLocalMoveY(cardTem.transform.localPosition.y + 40, 0.2f);
                    break;
                }
            }

            if (player == thisPlayer)
            {
                for (var j = 0; j < player.vectorCard.Count; j++)
                {
                    var cardTem = player.vectorCard[j];
                    if (cardTem != null && cardTem.code == code)
                    {
                        player.vectorCard.RemoveAt(j);
                        j--;
                        DOTween.Sequence().Append(cardTem.transform.DOLocalMoveY(cardTem.transform.localPosition.y + 60, 0.2f)).AppendCallback(() =>
                        {
                            removerCard(cardTem);
                        });
                        break;
                    }
                }
            }
            else
            {
                if (player.isSpecial && (code == 11 || code == 14))
                {
                    for (var k = 0; k < player.vectorCard.Count; k++)
                    {
                        var cardTem = player.vectorCard[k];
                        if (cardTem.code == code)
                        {
                            player.vectorCard.RemoveAt(k);
                            removerCard(cardTem);
                            k--;
                        }
                    }
                }
            }

            var cardTemp = getCard(panel.transform);
            cardTemp.transform.localScale = Vector3.one;
            var poss = cardTemp.transform.localPosition;
            poss.y = 0;
            cardTemp.transform.localPosition = poss;
            cardTemp.setTextureWithCode(code, true);
            cardTemp.transform.SetSiblingIndex(i);
            if (code == currentTopCard) cardTemp.showTarget(true);
            listDrop.Add(cardTemp);
        }

        listDrop.Sort((a, b) =>
        {
            return a.S - b.S;
        });
        listDrop.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        sortCardMeld(layout.transform);
        resetDumpCardPosition();
        player.vectorCardD2.Add(listDrop);
        DOTween.Sequence().AppendInterval(0.5F).AppendCallback(() =>
        {
            showTextDown(player._indexDynamic, (int)meldData["meldObject"]["idMeld"], isEat ? 1 : 2);
        });
    }

    public void onClickDGroup(string customEventData)
    {
        Logging.Log("onClickDGroup   " + customEventData);// 0-1, 0-2
        if (currenMode != 3 || selectedCards.Count != 1) return;
        var indexPlayer = int.Parse(customEventData[0] + "");
        var idMeld = int.Parse(customEventData[2] + "");
        List<Card> check = new List<Card>();
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player._indexDynamic == indexPlayer)
            {
                //check.AddRange(player.vectorCardD2[idMeld]);
                check = player.vectorCardD2[idMeld];
                break;
            }
        }
        Logging.Log("onClickDGroup  list check ");
        logListCard(check);
        List<Card> mergeArray = new List<Card>();
        mergeArray.Add(selectedCards[0]);
        mergeArray.AddRange(check);

        Logging.Log("onClickDGroup  list mergeArray ");
        logListCard(mergeArray);

        if (!checkValidMeld(mergeArray)) return;
        currentDataLayoff = customEventData;
        selectedLayoff = check;

        Logging.Log("onClickDGroup  list selectedLayoff ");
        logListCard(selectedLayoff);
        highLightLayoffMeld(new List<List<Card>>() { selectedLayoff });

    }

    public void handleStupid(JObject data, bool noupdateKaengPoint = true)
    {
        //// {"evt":"stupidCard","pid":1765715,"point":-150,"addPoint":-50}
        ///{
        //        "evt": "stupidCard",
        //  "pid": 8072,
        //  "addPoint": -50,
        //  "point": -200,
        //  "type": "FULL_DUMMY"
        //}
        var addPoint = (int)data["addPoint"];
        Logging.Log("-=-=addPoint  " + addPoint);
        var player = getPlayerWithID((int)data["pid"]);
        if (noupdateKaengPoint) ((PlayerViewDummy)player.playerView).updateKaengPoint((int)data["point"]);
        if (listAniDummy[player._indexDynamic])
        {
            DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
            {
                handleStupid(data, false);
            });
            return;
        }
        else
        {
            listAniDummy[player._indexDynamic] = true;
            DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
            {
                listAniDummy[player._indexDynamic] = false;
            });
        }
        var type = new List<int>();
        switch ((string)data["type"])
        {
            case "FULL_DUMMY":
                type.Add(5);
                break;
            case "DUMMY":
                type.Add(4);
                break;
            case "EATSPECIALCARD":
                if ((string)data["kindCard"] == "BODY") type.Add(7);
                else if ((string)data["kindCard"] == "SPETO") type.Add(8);
                else if ((string)data["kindCard"] == "BODY_SPETO")
                {
                    type.Add(7);
                    type.Add(8);
                    int anim2 = ((string)data["type"] == "FULL_DUMMY") ? 3 : 1;

                    Sequence seqq2 = DOTween.Sequence();
                    for (var i = 0; i < type.Count; i++)
                    {
                        var ii = i;
                        seqq2.AppendCallback(() =>
                        {
                            showTag(player, type[ii], (addPoint / 2) + "");
                            showAnim(false, anim2, player._indexDynamic);
                        });
                        if (i < type.Count - 1)
                            seqq2.AppendInterval(2);
                    }

                    return;
                }
                break;
            case "SUCKED":
                type.Add(9);
                break;
            case "SENDSPECIALCARD":
                type.Add(10);
                break;
            default:
                break;
        }


        if (type.Count > 0)
        {
            int anim = ((string)data["type"] == "FULL_DUMMY") ? 3 : 1;
            //var listAction = [];
            Sequence seqq = DOTween.Sequence();
            for (var i = 0; i < type.Count; i++)
            {
                var ii = i;
                seqq.AppendCallback(() =>
                {
                    showTag(player, type[ii], addPoint + "");
                    showAnim(false, anim, player._indexDynamic);
                });
                if (i < type.Count - 1)
                    seqq.AppendInterval(2);
            }
        }
    }

    public void handleKnockOut(JObject data)
    {
        //// {"evt":"knockOut","data":{"pid":1765662,"bonus":50,"point":110,"type":"KNOCK_OUT"}}
        isKnockOut = true;
        var player = getPlayerWithID((int)data["pid"]);
        ((PlayerViewDummy)player.playerView).updateKaengPoint((int)data["point"]);
        int type = -1;
        var anim = 0;

        foreach (var pl in players) pl.setTurn(false);
        buttonManager.onHide();
        var str = (string)data["type"];

        string sound = null;
        if (str == "COLOR_KNOCK_OUT")
        {
            type = 1;
            anim = 5;
            sound = SOUND_DUMMY.KNOCK_OUT;

        }
        else if (str == "DARK_KNOCK_OUT")
        {
            type = 2;
            anim = 5;
            sound = SOUND_DUMMY.KNOCK_DARK;

        }
        else if (str == "DARK_COLOR_KNOCK_OUT")
        {
            type = 3;
            anim = 5;
            sound = SOUND_DUMMY.KNOCK_DARK_COLOR;

        }

        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
        {
            if (type != -1) showTag(player, type, (int)data["bonus"] + "");

            if (anim == 5) showAnim(false, anim, player._indexDynamic);
            if (sound != null)
                playSound(sound);
        });
        var time = anim == 5 ? 3.5f : 2;
        DOTween.Sequence().AppendInterval(time).AppendCallback(() =>
        {
            showAnim(false, 0, player._indexDynamic);
        });
    }

    public void handleFinish(JArray _sortedData, int agPot)
    {
        //{ "evt":"finish","data":[{ "uid":2666,"M":7203,"ag":28697,"point":245,"diff":735,"arrCard":[],"isBurn":false,"hitPot":784,"numberMatchWin":0},{ "uid":8240,"M":-7350,"ag":3176,"point":-490,"diff":-735,"arrCard":[27,25,44,17,11,23,48,15,22,45,43],"isBurn":true,"hitPot":0,"numberMatchWin":0}],"timeAction":10,"agPot":0}
        //dev
        //_sortedData = new JArray("[{\"uid\":388638,\"M\":7203,\"ag\":2282500,\"point\":245,\"diff\":735,\"arrCard\":[],\"isBurn\":false,\"hitPot\":784,\"numberMatchWin\":0},{\"uid\":8240,\"M\":-7350,\"ag\":3176,\"point\":-490,\"diff\":-735,\"arrCard\":[27,25,44,17,11,23,48,15,22,45,43],\"isBurn\":true,\"hitPot\":0,\"numberMatchWin\":0}]");
        //agPot = 0;
        result.Clear();

        var sortedData = new JArray(_sortedData.OrderByDescending(p => (int)p["M"]));
        DOTween.Sequence().AppendCallback(() =>
        {
            stateGame = STATE_GAME.WAITING;
            foreach (var pl in players) pl.setTurn(false);
            if (!isKnockOut)
            {
                DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
                {
                    showAnim(true, 7, -1);
                });
            }

            //var scoreHighest = new List<int>();
            var indexRank = 0;
            var currentScore = 0;
            for (var i = 0; i < sortedData.Count; i++)
            {
                var playerData = (JObject)sortedData[i];
                var player = getPlayerWithID((int)playerData["uid"]);

                player.playerView.effectFlyMoney((int)playerData["M"]);
                burnPlayer(player, playerData);
                if (player != thisPlayer)
                {
                    foreach (var c in player.vectorCard)
                    {
                        removerCard(c);
                    }
                    player.vectorCard.Clear();
                    ((PlayerViewDummy)player.playerView).updateNumCard(-1, Vector3.zero, false);
                    showResultCardPlayers(playerData, player._indexDynamic - 1);
                }
                //scoreHighest.Add((int)playerData["point"]);
                player.ag = (int)playerData["ag"];
                player.setAg();

                if ((int)playerData["point"] != currentScore)
                {
                    currentScore = (int)playerData["point"];
                    indexRank++;
                }

                var objData = new JObject();
                objData["id"] = player.id;
                objData["displayName"] = player.displayName;
                objData["pname"] = player.namePl;
                objData["fid"] = player.fid;
                objData["avatar_id"] = player.avatar_id;
                objData["score"] = playerData["diff"];
                objData["point"] = playerData["point"];
                objData["money"] = playerData["M"];
                objData["hitpot"] = playerData["hitPot"];
                objData["rank"] = indexRank;

                result.Add(objData);
                ((PlayerViewDummy)player.playerView).updateKaengPoint((int)playerData["point"]);
                if (i == 0)
                {
                    ((PlayerViewDummy)player.playerView).updatePotDummy(1);
                    var vlHitpot = (int)playerData["hitPot"];
                    if (vlHitpot > 0)
                    {
                        //Logging.Log("-=- an hit pot");
                        hitpot.GetChild(1).gameObject.SetActive(true);
                        ((PlayerViewDummy)player.playerView).updatePotDummy(2);

                        hitpot.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                        hitpot.GetChild(2).gameObject.SetActive(false);
                        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
                        {
                            player.playerView.effectFlyMoney((int)playerData["hitPot"]);
                            player.playerView.setAg((int)playerData["ag"] + (int)playerData["hitPot"]);
                            hitpot.parent = transform;
                            hitpot.SetAsLastSibling();
                            hitpot.localPosition = new Vector3(0, 15);
                            BgHitpot.gameObject.SetActive(true);
                            hitpot.GetChild(0).gameObject.SetActive(false);
                            hitpot.GetChild(1).gameObject.SetActive(false);
                            hitpot.GetChild(2).gameObject.SetActive(true);
                            hitpot.GetChild(2).GetComponent<TextMeshProUGUI>().text = Config.FormatNumber(vlHitpot);
                            var animHitPot = hitpot.GetComponent<SkeletonGraphic>();
                            animHitPot.AnimationState.SetAnimation(0, "05_open_chest", false);
                        }).AppendInterval(1.8f).AppendCallback(() =>
                        {
                            hitpot.GetChild(2).gameObject.SetActive(false);
                        });
                    }
                }
                else
                {
                    ((PlayerViewDummy)player.playerView).updatePotDummy(0);
                }
            }

            if (agPot == 0)
            {
                DOTween.Sequence().AppendInterval(4).AppendCallback(() =>
                {
                    resetHitpot();
                    foreach (var player in players)
                    {
                        ((PlayerViewDummy)player.playerView).updatePotDummy(0);
                    }
                });
            }
            else
                hitpot.GetChild(0).gameObject.SetActive(true);
            buttonManager.onHide();
            DOTween.Sequence().AppendInterval(4.5f).AppendCallback(() =>
            {
                nodeResult.onShow(result);
                nodeResult.transform.SetAsLastSibling();
            });
        }).AppendInterval(6).AppendCallback(() =>
        {
            if (Config.isBackGame)
            {
                SocketSend.sendExitGame();
            }
            resetGame();
        });

        for (var i = 0; i < sortedData.Count; i++)
        {
            var dataplayer = (JObject)sortedData[i];

            if ((int)dataplayer["numberMatchWin"] == 1)
            {
                var player = getPlayerWithID((int)dataplayer["uid"]);
                lastWin = new JObject();
                lastWin["numberMatchWin"] = 1;
                lastWin["pname"] = player.displayName;
                lastWin["id"] = (int)dataplayer["uid"];
                hitpot.GetChild(0).gameObject.SetActive(true);
            }
        }

    }
    void resetPosCard()
    {
        for (var i = 0; i < thisPlayer.vectorCard.Count; i++)
        {
            cardMoveBack(thisPlayer.vectorCard[i], 0.3f);
            thisPlayer.vectorCard[i].transform.localScale = PLAYERSCALE;
            thisPlayer.vectorCard[i].setDark(false);
            thisPlayer.vectorCard[i].setEffect_Twinkle(false, -1);
            thisPlayer.vectorCard[i].transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
        }
        checkAllowButton(true);

    }


    public void handleFirstCard(int code)
    {
        var cardTemp = getCard(cardParent);
        cardTemp.transform.localScale = TABLESCALE;
        cardTemp.isAllowTouch = false;
        listDumpCard.Add(cardTemp);
        var len = listDumpCard.Count;
        var pos = getDumpCardPosition(len - 1);

        cardTemp.transform.localPosition = cardDeck.transform.parent.localPosition;//dev
        cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD);
        currentTopCard = code;
        Sequence seq = DOTween.Sequence();
        seq.Append(cardTemp.transform.DOLocalMove(pos, 0.3f));
        seq.AppendCallback(() =>
        {
            foldCardUp(cardTemp, code, 0.4f);
        });
        seq.AppendInterval(0.4f);
        seq.AppendCallback(() =>
        {
            cardTemp.isAllowTouch = true;
            cardTemp.showTarget(true);
        });

        cardTemp.addButtonClick(onClickCardDump);
    }

    void updateCardDeck(int number)
    {
        if (number <= 0)
        {
            cardDeck.transform.parent.gameObject.SetActive(false);
            return;
        }
        cardDeck.transform.parent.gameObject.SetActive(true);
        cardDeck.text = number + "";
    }

    Vector3 getMyCardPosition(int index)
    {
        var len = thisPlayer.vectorCard.Count;
        var basePos = new Vector2(-320, -280);
        var offSet = len <= 13 ? 65 : 720 / len;
        return new Vector3(basePos.x + offSet * index, basePos.y, 0);
    }

    void resetDumpCardPosition()
    {
        selectedDumps.Clear();
        for (var i = 0; i < listDumpCard.Count; i++)
        {
            var pos = getDumpCardPosition(i);
            var cardTemp = listDumpCard[i];
            cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
            cardTemp.setDark(false);
            cardTemp.isAllowTouch = false;
            cardTemp.transform.DOLocalMove(pos, .2f);
        }
    }

    Vector2 getDumpCardPosition(int index)
    {
        var len = listDumpCard.Count;
        var basePos = new Vector2(-350, 27);
        var offSet = len < 12 ? 60 : 720 / len;
        return new Vector3(basePos.x + offSet * index, basePos.y, 0);
    }
    Vector2 getResultBoxCardPosition(int index, int length)
    {
        var basePos = new Vector2(-170, 0);
        var offSet = length <= 13 ? 20 : 350 / length;
        return new Vector2(basePos.x + offSet * index, basePos.y);
    }


    void foldCardUp(Card cardTemp, int code, float time = 0.6f, bool isAnim = true)
    {
        if (cardTemp == null) return;
        var scale = cardTemp.transform.localScale;
        cardTemp.transform.DOKill();
        var seqFoldCardUp = DOTween.Sequence();
        seqFoldCardUp.Append(cardTemp.transform.DOScaleX(0, time / 2));
        seqFoldCardUp.AppendCallback(() =>
        {
            cardTemp.setTextureWithCode(code);
            if (code == currentTopCard) cardTemp.showTarget(true);
            if (isAnim) cardTemp.setEffect_Twinkle(true, 0.8f);
        });
        seqFoldCardUp.Append(cardTemp.transform.DOScaleX(scale.x, time / 2));
    }

    void handleDumpedCard(Card cardTemp)
    {
        //cc.NGWlog('!=> select card on dumped ', cardTemp.nameCard)

        if (currenMode != 1) return;
        if (selectedDumps.IndexOf(cardTemp) >= 0)
        {
            handleUnCheckDumpCard(cardTemp);
        }
        else
        {
            handleCheckDumpCard(cardTemp);
        }
        checkAllowButton();
    }

    void handleCheckDumpCard(Card _cardTemp, bool isAuto = true)
    {
        if (listDumpCard.FirstOrDefault(it => it.code == _cardTemp.code) == null || !_cardTemp.isAllowTouch) return;
        if (currenMode == 1 && isAuto)
        {
            autoPickSuggestCard(_cardTemp);
            return;
        }
        selectedDumps.Add(_cardTemp);
        var pos = getDumpCardPosition(listDumpCard.FindIndex(it => it.code == _cardTemp.code));
        //_cardTemp.node.runAction(cc.moveTo(0.1, cc.v2(pos.x, pos.y + 20)))
        //_cardTemp.DOKill();

        _cardTemp.transform.DOKill();
        _cardTemp.transform.DOLocalMoveY(pos.y + 35, 0.1f);
        //pos.y += 35;
        //_cardTemp.transform.localPosition = pos;
    }

    void handleUnCheckDumpCard(Card _cardTemp)
    {
        if (selectedDumps.FirstOrDefault(it => it.code == _cardTemp.code) == null) return;
        selectedDumps.Remove(_cardTemp);
        var pos = getDumpCardPosition(listDumpCard.FindIndex(it => it.code == _cardTemp.code));
        //_cardTemp.node.runAction(cc.moveTo(0.1, pos))
        _cardTemp.transform.DOKill();
        _cardTemp.transform.DOLocalMove(pos, 0.1f);

    }

    void handleCard(Card cardTemp)
    {
        if (thisPlayer.vectorCard.IndexOf(cardTemp) < 0) return;
        if (selectedCards.IndexOf(cardTemp) >= 0)
        {
            Logging.Log("handleCard handleUnSelectCard ");
            handleUnSelectCard(cardTemp);
        }
        else
        {
            Logging.Log("handleCard handleSelectCard ");
            handleSelectCard(cardTemp);
        }
        checkAllowButton();
    }

    void handleSelectCard(Card cardTemp, bool isAuto = true)
    {
        if (selectedCards.IndexOf(cardTemp) >= 0) return;
        if (currenMode != 0 && isAuto && currenMode < 4)
        {
            autoPickSuggestCard(cardTemp);

            return;
        }
        if (currenMode == 0)
        {
            //selectedCards.forEach(cardTemp => {
            //foreach (var cardTemp2 in selectedCards)
            for (var i = 0; i < selectedCards.Count; i++)
            {
                handleUnSelectCard(selectedCards[i]);
                i--;
            }
            selectedCards.Clear();
        }
        selectedCards.Add(cardTemp);
        cardMoveBack(cardTemp, 0.1f);
    }

    void handleUnSelectCard(Card cardTemp, bool isTouch = true)
    {
        var index = selectedCards.IndexOf(cardTemp);

        Logging.Log("-=-=handleUnSelectCard " + index);
        if (index == -1) return;
        selectedCards.Remove(cardTemp);
        cardMoveBack(cardTemp, 0.1f);
        if (currenMode == 3 && isTouch)
        {
            highLightLayoffMeld(new List<List<Card>>());
        }

    }
    //const float idSeqCardMoveBack = 10001;
    void cardMoveBack(Card cardTemp, float time = 0.2f)
    {
        //if (cardTemp == null || cardTemp.code == 0 || cardTemp.getOpacity() < 255) return;
        if (cardTemp == null || cardTemp.code == 0) return;

        DOTween.Kill(cardTemp.transform);
        //DOTween.Kill(idSeqCardMoveBack);
        cardTemp.isAllowTouch = false;
        var index = thisPlayer.vectorCard.IndexOf(cardTemp);
        var pos = getMyCardPosition(index);
        //cardTemp.transform.DOKill();

        //var isSelect = selectedCards.Contains(cardTemp);
        var isSelect = selectedCards.IndexOf(cardTemp) >= 0 ? true : false;
        if (isSelect)
        {
            pos.y += 35;//= -275;
        }

        if (currenMode != 0 && currenMode < 4 && thisPlayer.getIsTurn())
        {
            cardTemp.setDark(!isSelect);
        }
        else
        {
            cardTemp.setDark(false);
        }
        cardTemp.transform.DOLocalMove(pos, time).SetEase(Ease.OutCubic);
        cardTemp.isAllowTouch = true;
    }
    public void checkAllowButton(bool isCheckMainBtn = false)
    {
        if (currentPlayer != thisPlayer) return;
        var vectorCard = thisPlayer.vectorCard;
        List<Card> mergeArray = new List<Card>();
        switch (currenMode)
        {
            case 0:
                {
                    //if (!isSendDataKnockOut)
                    //{
                    if (isCheckMainBtn)
                    {

                        var dmm = checkKnockOut();
                        buttonManager.btnDarkKnock.gameObject.SetActive(dmm && thisPlayer.vectorCardD2.Count < 1);

                        buttonManager.btnEat.interactable = checkEat();
                        buttonManager.btnMeld.interactable = checkMeld();
                        buttonManager.btnLayoff.interactable = checkLayoff();
                        buttonManager.btnKnockOut.gameObject.SetActive(dmm);
                    }
                    if (!isState1)
                    {
                        var isAllow = selectedCards.Count == 1;
                        buttonManager.btnDiscard.interactable = isAllow;
                        if (isAllow)
                        {
                            var isDummy = checkIsDummyCard();
                            buttonManager.showDummyWarn(isDummy);
                        }
                        else
                        {
                            buttonManager.showDummyWarn(false);
                        }
                    }
                    //}
                    break;
                }

            case 1:
                {
                    // eat
                    //mergeArray = [...selectedCards, ...selectedDumps];
                    mergeArray.AddRange(selectedCards);
                    mergeArray.AddRange(selectedDumps);

                    var isAllow = checkValidEatMeld(mergeArray) && selectedCards.Count > 0 && checkValidMeld(mergeArray);
                    buttonManager.btnOk.interactable = isAllow;
                    break;
                }
            case 2:
                {
                    // meld
                    if (selectedCards.Count == vectorCard.Count)
                        buttonManager.btnOk.interactable = false;
                    else
                        buttonManager.btnOk.interactable = checkValidMeld(selectedCards);

                    break;
                }
            case 3:
                {
                    var isSelected = selectedCards.Count > 0;
                    mergeArray.AddRange(selectedCards);
                    mergeArray.AddRange(selectedLayoff);
                    var isActive = isSelected && checkValidMeld(mergeArray);

                    buttonManager.btnOk.interactable = isActive;
                    break;
                }
            case 4:
                // knockout
                buttonManager.btnOk.interactable = dataKnockOut.cardLeft.Count <= 1;
                btnSort.interactable = false;
                break;
        }
    }

    bool checkMeld()
    {
        if (thisPlayer.vectorCardD2.Count == 0 || thisPlayer.vectorCard.Count <= 3) return false;
        var checkMeld = findPossibleMeld(thisPlayer.vectorCard);
        var cardLeft = new List<Card>();
        var allMeld = new List<List<Card>>();//[...checkMeld.meldSame, ...checkMeld.meldStraight]
        allMeld.AddRange(checkMeld["meldSame"]);
        allMeld.AddRange(checkMeld["meldStraight"]);

        foreach (var cardTemp in thisPlayer.vectorCard)
        {
            if (checkCardSingle(cardTemp, allMeld, new JArray()))
                cardLeft.Add(cardTemp);

        }
        if (cardLeft.Count == 0)
        {
            foreach (var array in allMeld)
            {
                if (array.Count > 3 && cardLeft.Count == 0)
                {
                    //array.sort(compareValues('N'));
                    array.Sort((a, b) =>
                    {
                        return a.N.CompareTo(b.N);
                    });
                    var card = array[0];
                    array.RemoveAt(array.IndexOf(card));
                    cardLeft.Add(card);
                }
            }
        }
        if (checkMeld["meldSame"].Count > 0 || checkMeld["meldStraight"].Count > 0)
        {
            if (cardLeft.Count > 0 || ((checkMeld["meldSame"].Count + checkMeld["meldSame"].Count) > 1))
            {
                return true;
            }
        }
        return false;
    }

    bool checkLayoff()
    {
        if (thisPlayer.vectorCardD2.Count == 0) return false;
        foreach (var player in players)
        {
            foreach (var array in player.vectorCardD2)
            {
                //Logging.Log("-=-= 3 vectorCard D2 " + player.displayName);
                //player.vectorCardD2.ForEach(it => Logging.Log(it.Count));
                for (var i = 0; i < thisPlayer.vectorCard.Count; i++)
                {
                    List<Card> checkArr = new List<Card>();
                    var cardTemp = thisPlayer.vectorCard[i];
                    checkArr.Add(cardTemp);
                    checkArr.AddRange(new List<Card>(array));
                    checkArr.Sort((a, b) =>
                    {
                        return a.S.CompareTo(b.S);
                    }); checkArr.Sort((a, b) =>
                    {
                        return a.N.CompareTo(b.N);
                    });
                    if (checkValidMeld(checkArr)) return true;
                }
            }
        }
        return false;
    }

    bool checkEat()
    {
        List<Card> checkArray = new List<Card>();
        checkArray.AddRange(listDumpCard);
        checkArray.AddRange(thisPlayer.vectorCard);
        var check = findPossibleMeld(checkArray, false);

        List<List<Card>> meldSame = check["meldSame"];
        List<List<Card>> meldStraight = check["meldStraight"];

        if (meldSame.Count == 0 && meldStraight.Count == 0) return false;
        foreach (var array in meldSame)
        {
            if (checkValidEatMeld(array)) return true;
        }

        foreach (var array in meldStraight)
        {
            if (checkValidEatMeld(array)) return true;
        }
        return false;
    }
    public void onEnterMode(int mode)
    {
        foreach (var cardTemp in thisPlayer.vectorCard)
        {
            cardTemp.transform.localScale = PLAYERSCALE;
        }
        var vectorCard = new List<Card>(thisPlayer.vectorCard);
        currenMode = mode;
        selectedCards.Clear();
        selectedDumps.Clear();
        resetDumpCardPosition();
        if (mode != 4) resetPosCard();
        showTagKnock(false, -1, Vector2.zero);
        currentDataLayoff = null;
        buttonManager.showDummyWarn(false);

        List<List<Card>> allMeld = new List<List<Card>>();
        IDictionary<string, List<List<Card>>> checkMeld;
        showArrow(false);
        if (mode != 3)
        {
            foreach (var player in players)
            {
                foreach (var array in player.vectorCardD2)
                {
                    foreach (var cardTemp in array)
                    {
                        cardTemp.setDark(false);
                    }
                }
            }
        }
        btnSort.interactable = true;
        Logging.Log("-=-=onEnterMode mode  " + mode);
        switch (mode)
        {
            case 0:
                // normal
                allPossibleMeld.Clear();
                listCanEat.Clear();
                break;
            case 1:
                {
                    // eat
                    listCanEat.Clear();
                    List<Card> arrayCheck = new List<Card>();
                    arrayCheck.AddRange(listDumpCard);
                    arrayCheck.AddRange(vectorCard);

                    Logging.Log(arrayCheck.Count + " = " + listDumpCard.Count + " + " + vectorCard.Count);


                    var stre = "";
                    arrayCheck.ForEach(it => stre += "," + it.code);
                    Logging.Log(stre);

                    checkMeld = findPossibleMeld(arrayCheck, false);
                    allMeld.Clear();

                    var meldSame = checkMeld["meldSame"];

                    var str = "";
                    for (var ii = 0; ii < meldSame.Count; ii++)
                    {
                        str += "cound:  " + meldSame[ii].Count;
                        meldSame[ii].ForEach(it => str += it.N + it.getSuitInVN());
                        str += "\n";
                    }
                    Logging.Log("-=-= onEnterMode   " + str);

                    allMeld.AddRange(meldSame);
                    allMeld.AddRange(checkMeld["meldStraight"]);

                    foreach (var array in allMeld)
                    {
                        if (checkValidEatMeld(array))
                        {
                            allPossibleMeld.Add(array);
                        }
                    }

                    foreach (var array in allPossibleMeld)
                    {
                        foreach (var cardTemp in array)
                        {
                            if (listDumpCard.FindIndex(it => it.code == cardTemp.code) >= 0 &&
                                listCanEat.FindIndex(it => it.code == cardTemp.code) < 0)
                            {
                                listCanEat.Add(cardTemp);
                                cardTemp.isAllowTouch = true;
                            }
                        }
                    }

                    Logging.Log("=--=mode = 1 listCanEat " + listCanEat.Count);

                    dohighLightCard(listDumpCard, listCanEat);
                    dohighLightCard(vectorCard, new List<Card>() { });
                    //handleCheckDumpCard(listCanEat[listCanEat.Count - 1]);

                    Logging.Log("=--=mode = 1 listDumpCard " + listDumpCard.Count);
                    for (var i = 0; i < listDumpCard.Count; i++)
                    {
                        var cardTemp = listDumpCard[i];
                        if (listCanEat.FirstOrDefault(it => it.code == cardTemp.code) != null)
                        {
                            handleCheckDumpCard(listCanEat[listCanEat.FindIndex(it => it.code == cardTemp.code)]);
                            break;
                        }
                    }
                    break;
                }
            case 2:
                {
                    //meld
                    allMeld.Clear();
                    //checkMeld.Clear();
                    checkMeld = findPossibleMeld(vectorCard);
                    allMeld.AddRange(checkMeld["meldSame"]);
                    allMeld.AddRange(checkMeld["meldStraight"]);
                    allPossibleMeld.AddRange(allMeld);
                    if (allMeld.Count == 0)
                    {
                        return;
                    }
                    var arraySuggest = allMeld[0];
                    for (var i = 0; i < allMeld.Count; i++)
                    {
                        if (getPoint(allMeld[i]) > getPoint(arraySuggest))
                        {
                            arraySuggest = allMeld[i];
                        }
                    }

                    var arraySelect = new List<Card>(arraySuggest);
                    if (vectorCard.Count == arraySelect.Count && arraySelect.Count > 3)
                    {
                        arraySelect.Sort((a, b) =>
                        {
                            return a.N.CompareTo(b.N);
                        });
                        arraySelect.RemoveAt(0);
                    }
                    dohighLightCard(vectorCard, arraySelect);
                    break;
                }

            case 3:
                {
                    // layoff

                    Logging.Log("-=-== check layoff enter mode");
                    foreach (var player in players)
                    {
                        foreach (var array in player.vectorCardD2)
                        {
                            foreach (var cardTemp in array)
                            {
                                cardTemp.setDark(true);
                            }
                        }
                    }

                    Card suggestMine = null;
                    var sortedCard = new List<Card>(vectorCard);
                    sortedCard.Sort((a, b) =>
                    {
                        return a.S.CompareTo(b.S);
                    });
                    sortedCard.Sort((a, b) =>
                    {
                        return a.N.CompareTo(b.N);
                    });

                    foreach (var cardTemp in sortedCard)
                    {
                        foreach (var player in players)
                        {
                            foreach (var array in player.vectorCardD2)
                            {
                                //Logging.Log("-=-= 4 vectorCard D2 " + player.displayName);
                                //player.vectorCardD2.ForEach(it => Logging.Log(it.Count));
                                var testArray = new List<Card>();
                                testArray.Add(cardTemp);
                                testArray.AddRange(new List<Card>(array));
                                if (checkValidMeld(testArray))
                                {
                                    suggestMine = cardTemp;
                                    currentDataLayoff = player._indexDynamic + "-" + player.vectorCardD2.IndexOf(array);
                                }
                            }
                        }
                    }
                    dohighLightCard(vectorCard, new List<Card>() { });
                    Logging.Log("-=-=suggestMine code: " + suggestMine.code + " N: " + suggestMine.N + " S: " + suggestMine.S + " Name: " + suggestMine.getSuitInVN());
                    autoPickSuggestCard(suggestMine);
                    break;
                }
            case 4:// knockOut
                {
                    //isSendDataKnockOut = false;
                    checkAllowButton();
                    btnSort.interactable = false;
                    showCardKnockOut();
                    break;
                }
        }
    }


    void autoPickSuggestCard(Card _cardTemp)
    {
        if (currenMode == 1)
        {
            Logging.Log("-=-=  autoPickSuggestCard currenMode == 1  " + _cardTemp.N + _cardTemp.getSuitInVN());
            Logging.Log("-=-=selectedCards    " + selectedCards.Count);
            selectedCards.ForEach(it => Logging.Log(" " + it.N + it.getSuitInVN()));

            List<List<Card>> listPossible = new List<List<Card>>();
            List<Card> result = new List<Card>();
            var strrrr = "";
            foreach (var array in allPossibleMeld)
            {
                array.ForEach(it => strrrr += "\n" + it.N + it.getSuitInVN());
                strrrr += "-=-=-=-=-==***\n";
                if (array.FindIndex(it => it.code == _cardTemp.code) != -1)
                {
                    Logging.Log("-=-= them 1 mạng");
                    listPossible.Add(array);
                }
            }

            Logging.Log(" -=-= ls Phom co " + strrrr);
            var str = "";
            for (var ii = 0; ii < listPossible.Count; ii++)
            {
                listPossible[ii].ForEach(it => str += "\n" + it.N + it.getSuitInVN());
                str += "-=-=-=-=-==\n";
            }
            Logging.Log(str);
            List<Card> listCardSelect = new List<Card>();
            listCardSelect.Add(_cardTemp);
            listCardSelect.AddRange(selectedCards);
            listCardSelect.AddRange(selectedDumps);

            List<Card> playerSelect = new List<Card>();
            playerSelect.Add(_cardTemp);
            playerSelect.AddRange(selectedCards);

            if (checkValidMeld(listCardSelect))
            {
                Logging.Log("-==-listCardSelect DONE");
                if (thisPlayer.vectorCard.IndexOf(_cardTemp) >= 0)
                {
                    handleSelectCard(_cardTemp, false);
                }
                else
                {
                    handleCheckDumpCard(_cardTemp, false);
                }
                return;
            }
            var strrrr2 = "";
            Logging.Log("-==-playerSelect " + playerSelect.Count);
            playerSelect.ForEach(it => strrrr2 += it.N + it.getSuitInVN() + "  ");
            Logging.Log(strrrr2);
            Logging.Log("-==-listPossible " + listPossible.Count);

            for (var i = 0; i < listPossible.Count; i++)
            {
                if (playerSelect.Count >= 2)
                {
                    var checkListSelectCard = true;
                    for (var j = 0; j < playerSelect.Count; j++)
                    {
                        checkListSelectCard = checkListSelectCard && (listPossible[i].FindIndex(cardTemp => cardTemp.code == playerSelect[j].code) != -1);
                    }
                    Logging.Log("checkListSelectCard   " + checkListSelectCard);
                    if (checkListSelectCard)
                    {
                        result.Clear();
                        result = new List<Card>(listPossible[i]);
                        break;
                    }
                }
                if (getPoint(listPossible[i]) > getPoint(result))
                {
                    Logging.Log("Diem lon hon   ");
                    if (checkValidEatMeld(listPossible[i]))
                    {
                        result.Clear();
                        result = new List<Card>(listPossible[i]);

                    }
                }
            }

            var lastSelect = new List<Card>(selectedCards);
            selectedCards.Clear();


            foreach (var cardTemp in lastSelect)
            {
                if (result.FirstOrDefault(it => it.code == cardTemp.code) == null)
                {
                    cardMoveBack(cardTemp, 0.1f);
                }
            }
            lastSelect.Clear();
            lastSelect = new List<Card>(selectedDumps);
            selectedDumps.Clear();
            foreach (var cardTemp in lastSelect)
            {
                cardTemp.transform.DOKill();
                var pos = getDumpCardPosition(listDumpCard.FindIndex(it => it.code == cardTemp.code));
                if (result.FirstOrDefault(it => it.code == cardTemp.code) == null)
                {
                    cardTemp.transform.DOLocalMove(pos, 0.1f);
                }
            }

            if (result.Count == 0)
            {
                Logging.Log("-=-=result.Count == 0 ");
                if (listDumpCard.FirstOrDefault(it => it.code == _cardTemp.code) != null)
                {
                    handleCheckDumpCard(_cardTemp, false);
                }
                else
                {
                    handleSelectCard(_cardTemp, false);
                }
                return;
            }

            foreach (var cardTemp in result)
            {
                //if (listDumpCard.IndexOf(cardTemp) >= 0)
                if (listDumpCard.FirstOrDefault(it => it.code == cardTemp.code) != null)
                {
                    handleCheckDumpCard(cardTemp, false);
                }
                else
                {
                    handleSelectCard(cardTemp, false);
                }
            }

        }
        else if (currenMode == 2)
        {
            var len = allPossibleMeld.Count;
            if (len <= 0) return;
            List<Card> result = new List<Card>();
            Card card = null;
            selectedCards.ForEach(it => Logging.Log(it.N + it.getSuitInVN()));

            //var str = "";
            //for (var i = 0; i < len; i++)
            //{
            //    var arr = allPossibleMeld[i];
            //    arr.ForEach(it => str += (it.N + it.getSuitInVN()) + " ");
            //    str += "\n";
            //}
            //Logging.Log(str);
            if (selectedCards.Count == 1) card = selectedCards[0];
            if (card != null)
            {
                for (var i = 0; i < len; i++)
                {
                    var arrDM = allPossibleMeld[i];
                    var check = (arrDM.FindIndex(cardTemp => cardTemp.code == _cardTemp.code) != -1) && (arrDM.FindIndex(cardTemp => cardTemp.code == card.code) != -1);
                    if (check)
                    {
                        if (getPoint(arrDM) > getPoint(result))
                        {
                            result.Clear();
                            result = new List<Card>(arrDM);

                        }
                    }
                }
            }
            if (result.Count == 0)
                for (var i = 0; i < len; i++)
                {
                    var arrDM = allPossibleMeld[i];
                    var check = arrDM.FindIndex(cardTemp => cardTemp.code == _cardTemp.code) != -1;

                    if (check)
                    {
                        if (getPoint(arrDM) > getPoint(result))
                        {
                            result.Clear();
                            result = new List<Card>(arrDM);

                        }
                    }
                }
            var lastSelect = new List<Card>(selectedCards);
            selectedCards.Clear();

            Logging.Log("-=-= selectedCards   " + selectedCards.Count);

            foreach (var cardTemp in lastSelect)
            {
                if (result.IndexOf(cardTemp) < 0)
                    cardMoveBack(cardTemp, 0.1f);

            }
            if (result.Count == 0)
            {
                handleSelectCard(_cardTemp, false);
                return;
            }
            foreach (var cardTemp in result)
            {
                handleSelectCard(cardTemp, false);
            }
        }
        else if (currenMode == 3)
        {
            selectedLayoff.Clear();
            List<List<Card>> suggestTheir = new List<List<Card>>();

            foreach (var player in players)
            {
                foreach (var array in player.vectorCardD2)
                {
                    List<Card> testArray = new List<Card>();
                    testArray.Add(_cardTemp);
                    testArray.AddRange(new List<Card>(array));
                    if (checkValidMeld(testArray))
                    {
                        Logging.Log("-=-= co thanh phan " + array.Count);
                        suggestTheir.Add(array);
                    }
                }
            }
            if (selectedCards.Count > 0)
            {
                for (var i = 0; i < selectedCards.Count; i++)
                {
                    handleUnSelectCard(selectedCards[i], false);
                    i--;
                }
            }
            highLightLayoffMeld(suggestTheir);
            selectedCards.Clear();
            handleSelectCard(_cardTemp, false);
        }
    }

    void dohighLightCard(List<Card> arrayAll, List<Card> arraySuggest)
    {
        foreach (var cardTemp in arrayAll)
        {
            if (arraySuggest.FindIndex(card => card.code == cardTemp.code) != -1)
            {
                if (currenMode == 1)
                {
                    cardTemp.setDark(false);
                    cardTemp.isAllowTouch = true;
                    //var pos = cardTemp.transform.localPosition;
                    //pos.y += 35;
                    //cardTemp.transform.localPosition = pos;
                }
                if (currenMode >= 2)
                {
                    handleSelectCard(cardTemp, false);
                }

            }
            else
            {
                cardTemp.setDark(true);
                if (currenMode == 1)
                {
                    cardTemp.isAllowTouch = false;
                }
            }
        }
    }


    IDictionary<string, List<List<Card>>> findPossibleMeld(List<Card> _arrayCard, bool isJump = true)
    {
        //Logging.Log(" -=-=findPossibleMeld0 " + _arrayCard.Count);
        List<List<Card>> temp = new List<List<Card>>();
        temp.Add(new List<Card>());
        temp.Add(new List<Card>());
        temp.Add(new List<Card>());
        temp.Add(new List<Card>());

        foreach (var cardTemp in _arrayCard)
        { //nhom card theo chat
            var index = cardTemp.S - 1;
            temp[index].Add(cardTemp);
        }

        foreach (var array in temp)
        {
            array.Sort((a, b) =>
            {
                return a.N.CompareTo(b.N);
            });
        }

        List<Card> arrayCard = new List<Card>();
        arrayCard.AddRange(temp[0]);
        arrayCard.AddRange(temp[1]);
        arrayCard.AddRange(temp[2]);
        arrayCard.AddRange(temp[3]);
        var listSame = findSameMeld(arrayCard, isJump);
        var listStraight = findStraightMeld(arrayCard, isJump);
        IDictionary<string, List<List<Card>>> results = new Dictionary<string, List<List<Card>>>();
        results.Add("meldSame", listSame);
        results.Add("meldStraight", listStraight);

        return results;
    }

    List<List<Card>> findStraightMeld(List<Card> _listCard, bool isJump = true)
    {
        var arrayCard = new List<Card>(_listCard);
        List<List<Card>> result = new List<List<Card>>();
        var index = 0;
        while (index < arrayCard.Count - 1)
        {
            List<Card> testArray = new List<Card>();
            testArray.Add(arrayCard[index]);

            for (var i = index + 1; i < arrayCard.Count; i++)
            {
                testArray.Add(arrayCard[i]);
                if (!checkStraightCard(testArray, false))
                {
                    testArray.RemoveAt(testArray.Count - 1);
                    break;
                }
                if (testArray.Count >= 3 && !isJump)
                {
                    result.Add(testArray);
                }
            }
            if (testArray.Count >= 3 && isJump) result.Add(testArray);
            var jump = isJump ? testArray.Count : 1;
            index += jump;
        }

        return result;
    }

    bool checkStraightCard(List<Card> _listCard, bool isCheckLength = true)
    {
        var listCard = new List<Card>(_listCard);
        if (listCard.Count < 3 && isCheckLength) return false;
        listCard.Sort((a, b) =>
        {
            return a.N.CompareTo(b.N);
        });
        for (var i = 1; i < listCard.Count; i++)
        {
            var nBase = listCard[0].N;
            if (nBase + i != listCard[i].N || listCard[0].S != listCard[i].S)
            {
                return false;
            }
        }
        return true;
    }

    List<List<Card>> findSameMeld(List<Card> _listCard, bool isJump = true)
    {
        //dev
        var arrayCard = new List<Card>(_listCard);
        List<List<Card>> result = new List<List<Card>>();
        var index = 0;
        arrayCard.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        while (index < arrayCard.Count - 1)
        {
            List<Card> testArray = new List<Card>();
            testArray.Add(arrayCard[index]);

            for (var i = index + 1; i < arrayCard.Count; i++)
            {
                testArray.Add(arrayCard[i]);
                if (!checkSameCard(testArray, false))
                {
                    testArray.RemoveAt(testArray.Count - 1);
                    break;
                }
            }
            if (testArray.Count >= 3)
            {
                result.Add(testArray);
                // tim to hop chap 3 cua test array co length = 4
                if (testArray.Count > 3 && !isJump)
                {
                    foreach (var cardSkip in testArray)
                    {
                        List<Card> subArray = new List<Card>();

                        foreach (var cardTemp in testArray)
                        {
                            if (cardTemp != cardSkip)
                            {
                                subArray.Add(cardTemp);
                            }
                        }
                        result.Add(subArray);
                    }
                }
            }
            var jump = testArray.Count;
            index += jump;
        }
        return result;
    }

    bool checkSameCard(List<Card> _listCard, bool isCheckLength = true)
    {
        var listCard = new List<Card>(_listCard);
        if (listCard.Count < 3 && isCheckLength) return false;
        for (var i = 1; i < listCard.Count; i++)
        {
            if (listCard[0].N != listCard[i].N) return false;

        }
        return true;
    }

    int getPoint(List<Card> arrayCard)
    {
        var point = 0;
        foreach (var cardTemp in arrayCard)
        {
            if (cardTemp.N < 10)
            {
                point += 5;
            }
            else if (cardTemp.N == 14)
            {
                point += 15;
            }
            else
            {
                point += 10;
            }
        }
        return point;
    }

    bool checkValidEatMeld(List<Card> _array)
    {
        var array = new List<Card>(_array);
        var isYour = false;
        var isDump = false;
        var lowest = listDumpCard.Count - 1;
        for (var i = 0; i < array.Count; i++)
        {
            var cardTemp = array[i];
            if (thisPlayer.vectorCard.IndexOf(cardTemp) >= 0) isYour = true;
            var idDump = listDumpCard.IndexOf(cardTemp);

            if (idDump >= 0)
            {
                isDump = true;
                if (idDump < lowest) lowest = idDump;
            }
        }
        // check phan om len khi ma an bo do
        List<Card> listCheck = new List<Card>();
        for (var i = 0; i < listDumpCard.Count; i++)
        {
            if (i >= lowest) listCheck.Add(listDumpCard[i]);
        }
        // check xem sau khi an con con nao de danh khong
        listCheck.AddRange(thisPlayer.vectorCard);
        foreach (var cardTemp in array)
        {
            listCheck.Remove(cardTemp);
        }
        if (listCheck.Count == 0) return false;

        return isYour && isDump;
    }

    bool checkValidMeld(List<Card> array)
    {
        if (array.Count < 3) return false;
        if (checkSameCard(array) || checkStraightCard(array)) return true;
        return false;
    }

    void highLightLayoffMeld(List<List<Card>> suggest)
    {
        if (suggest.Count == 0)
        {
            showArrow(false);
        }

        foreach (var player in players)
        {
            foreach (var array in player.vectorCardD2)
            {

                //Logging.Log("-=-= 6 vectorCard D2 " + player.displayName);
                //player.vectorCardD2.ForEach(it => Logging.Log(it.Count));

                Logging.Log("-=-= suggest trước khi fail loi mom " + suggest.IndexOf(array));
                if (suggest.IndexOf(array) != -1)
                {
                    Logging.Log("-=-= suggest  fail loi mom");
                    foreach (var cardTemp in array)
                    {
                        cardTemp.setDark(false);
                    }
                    selectedLayoff = new List<Card>(array);
                    showArrow(true, player._indexDynamic, player.vectorCardD2.IndexOf(array));
                    currentDataLayoff = player._indexDynamic + "-" + player.vectorCardD2.IndexOf(array);
                }
                else
                {
                    var isAllow = false;

                    List<Card> testArray = new List<Card>();
                    testArray.AddRange(selectedCards);
                    testArray.AddRange(new List<Card>(array));

                    testArray.Sort((a, b) =>
                    {
                        return a.S - b.S;
                    });
                    testArray.Sort((a, b) =>
                    {
                        return a.N - b.N;
                    });

                    isAllow = checkValidMeld(testArray);
                    if (selectedCards.Count != 1) isAllow = false;
                    foreach (var cardTemp in array)
                    {
                        cardTemp.setDark(!isAllow);
                    };
                }
            }
        }
    }
    void showArrow(bool isShow, int idPlayer = -1, int idMeld = -1)
    {
        if (!isShow)
        {
            nodeArrow.SetActive(false);
            return;

        }

        if (thisPlayer != null && !thisPlayer.getIsTurn()) return;

        nodeArrow.SetActive(true);
        nodeArrow.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + 50);
        var nodePanel = listDropLayout[idPlayer].transform.GetChild(idMeld);

        var pos = listDropLayout[idPlayer].transform.TransformPoint(nodePanel.localPosition);
        var truePos = transform.InverseTransformPoint(pos);
        nodeArrow.transform.localPosition = new Vector3(truePos.x, truePos.y + 70);
    }

    public JObject getDataLayoff()
    {
        if (currentDataLayoff == null || selectedCards.Count != 1) return null;
        var str = currentDataLayoff;
        var indexPlayer = int.Parse(str[0] + "");
        int playerID = 0;
        foreach (var player in players)
        {
            if (player._indexDynamic == indexPlayer)
            {
                playerID = player.id;
                break;
            }
        }
        var idMeld = int.Parse(str[2] + "");
        var data = new JObject();
        data["idCardSend"] = selectedCards[0].code;
        data["idMeld"] = idMeld;
        data["sendtoPlayer"] = playerID;
        return data;
    }

    void updateCardOnHand(Player player, int number)
    {
        if (player == null || player == thisPlayer) return;
        ((PlayerViewDummy)player.playerView).updateNumCard(number, Vector3.zero, false);
    }

    void showAnim(bool isAll, int type, int pIndex)
    {
        nodeAnim.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + 50);
        var animName = "animation";
        if (type == 7 || type == 0)
        {
            animName = "thai";//Config.language == "EN" ? "eng" : "thai";
        }
        string sound = null;
        switch (type)
        {
            case 0:
                sound = SOUND_DUMMY.KNOCK_OUT;
                break;
            case 1:
                sound = SOUND_DUMMY.DUMMY;
                break;
            case 2:
                sound = SOUND_DUMMY.BURNED;
                break;
            case 3:
                sound = SOUND_DUMMY.FULL_DUMMY;
                break;
            case 4:
                sound = SOUND_DUMMY.KEEP_HEAD;
                break;
            case 6:
                sound = SOUND_DUMMY.SHOW;
                break;
            case 7:
                sound = SOUND_DUMMY.FINISH;
                break;

        }
        if (sound != null)
            playSound(sound);

        if (isAll)
        {
            var anim = listNodeAnim[4];
            anim.gameObject.SetActive(true);
            anim.transform.DOKill();
            for (var i = 0; i < 4; i++)
            {
                //listNodeAnim[i].skeletonDataAsset = null;
                listNodeAnim[i].gameObject.SetActive(false);
            }
            anim.skeletonDataAsset = listAnim[type];

            anim.Initialize(true);
            anim.AnimationState.SetAnimation(0, animName, false);
            DOTween.Sequence().AppendInterval(3).AppendCallback(() =>
            {
                anim.gameObject.SetActive(false);
            });

            var pos = anim.transform.localPosition;
            if (type >= 1 && type <= 6)
            {
                pos.y = 0;
            }
            else
            {
                pos.y = 15;
            }
            anim.transform.localPosition = pos;
            return;
        }
        else
        {
            //listNodeAnim[4].skeletonDataAsset = null;
            listNodeAnim[4].gameObject.SetActive(false);
            if (pIndex < 0 && pIndex >= 4) return;
            var anim = listNodeAnim[pIndex];
            anim.gameObject.SetActive(true);
            //anim.transform.DOKill();
            anim.skeletonDataAsset = listAnim[type];
            anim.Initialize(true);
            anim.AnimationState.SetAnimation(0, animName, false);
            DOTween.Sequence().AppendInterval(3).AppendCallback(() =>
            {
                anim.gameObject.SetActive(false);
            });

            var pos = anim.transform.localPosition;
            if (type >= 1 && type <= 6)
            {
                pos.y = (pIndex == 1 || pIndex == 2) ? 185 : -45;
            }
            else
            {
                pos.y = (pIndex == 1 || pIndex == 2) ? 115 : -115;
            }
            anim.transform.localPosition = pos;
        }
    }

    void showTag(Player player, int type, string value)
    {
        var item = Instantiate(nodeItemTag, transform).GetComponent<DummyItemTag>();

        item.gameObject.SetActive(true);
        item.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + 51);
        item.onShow(player, type, value);
    }

    void showTextDown(int idPlayer, int idMeld, int type)
    {
        var itemTextDown = Instantiate(lblTextDown, transform);

        itemTextDown.gameObject.SetActive(true);
        itemTextDown.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + 10);
        itemTextDown.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        var nodePanel = listDropLayout[idPlayer].transform.GetChild(idMeld);
        var pos = listDropLayout[idPlayer].transform.TransformPoint(nodePanel.localPosition);
        var truePos = transform.InverseTransformPoint(pos);
        itemTextDown.transform.localPosition = truePos;
        itemTextDown.gameObject.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, type + "", false);
        DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
        {
            itemTextDown.DOFade(0, .5f);
        }).AppendInterval(0.5f).AppendCallback(() =>
        {
            Destroy(itemTextDown.gameObject);
        });
    }

    bool checkIsDummyCard()
    {
        var cardTemp = selectedCards[0];
        //check an card tren ban
        //var checkArray = [...listDumpCard];
        var checkArray = new List<Card>(listDumpCard);
        checkArray.Add(cardTemp);
        var check = findPossibleMeld(checkArray);

        List<List<Card>> allCheck = new List<List<Card>>();
        allCheck.AddRange(check["meldSame"]);
        allCheck.AddRange(check["meldStraight"]);
        foreach (var array in allCheck)
        {
            if (array.IndexOf(cardTemp) >= 0)
                return true;
        }
        //check gui
        foreach (var player in players)
        {
            foreach (var array in player.vectorCardD2)
            {

                //Logging.Log("-=-= 7 vectorCard D2 " + player.displayName);
                //player.vectorCardD2.ForEach(it => Logging.Log(it.Count));
                //var testArray = [cardTemp];
                //testArray = [...testArray, ...array];
                List<Card> testArray = new List<Card>();
                testArray.Add(cardTemp);
                testArray.AddRange(new List<Card>(array));
                if (checkValidMeld(testArray)) return true;
            }
        }
        return false;
    }

    bool checkKnockOut()
    {
        dataKnockOut = simulateKnockOut();

        Logging.Log("-=-= checkKnockOut ");
        if (dataKnockOut.cardLeft.Count > 1) return false;
        if (dataKnockOut.cardLeft.Count == 0 && dataKnockOut.allLayoff.Count == 0) return false;
        return true;
    }

    DummyDataCustom simulateAllLayoff(List<Card> _array)
    {
        var vectorCard = new List<Card>(_array);
        vectorCard.Sort((a, b) =>
        {
            return a.S - b.S;
        });
        vectorCard.Sort((a, b) =>
        {
            return a.N - b.N;
        });

        var cardLeft = vectorCard; //xep cac sanh theo chat
        JArray dataLayoff = new JArray();
        foreach (var player in players)
        {
            foreach (var array in player.vectorCardD2)
            { //lay ra cac bo tren ban (co ca cua mk)
                var isStop = false;
                var temp = new List<Card>(vectorCard); //card cua mk
                var testArray = new List<Card>(array);
                while (isStop == false)
                {
                    isStop = true;
                    testArray.Sort((a, b) =>
                    {
                        return a.S - b.S;
                    });
                    testArray.Sort((a, b) =>
                    {
                        return a.N - b.N;
                    });

                    for (var k = 0; k < temp.Count; k++)
                    {
                        var cardTemp = temp[k];
                        var testArrayTemp = new List<Card>(testArray);

                        testArrayTemp.Add(cardTemp);

                        if (checkValidMeld(testArrayTemp))
                        { //check xem co gui dc k
                            testArray.Add(cardTemp);
                            var index = cardLeft.FindIndex(c => c.code == cardTemp.code);
                            if (index >= 0) cardLeft.RemoveAt(index); //card gui dc thi xoa khoi list dang check
                            //temp.RemoveAt(k);
                            temp.Remove(cardTemp);
                            JObject data = new JObject();
                            data["idCardSend"] = cardTemp.code;
                            data["idMeld"] = player.vectorCardD2.IndexOf(array);
                            data["sendtoPlayer"] = player.id;
                            dataLayoff.Add(data);
                            isStop = false;
                            k--;
                        }
                    }
                }

            }
        }
        DummyDataCustom result = new DummyDataCustom();
        result.cardLeft = cardLeft;
        result.allLayoff = dataLayoff;

        return result;
    }

    List<List<Card>> findOverLapArray(Card cardTemp, List<List<Card>> array)
    {
        List<List<Card>> result = new List<List<Card>>();
        for (var i = 0; i < array.Count; i++)
        {
            if (array[i].FindIndex(c => c.code == cardTemp.code) >= 0) result.Add(array[i]);
        }
        return result;
    }
    List<List<Card>> resolveOverLapMeld(List<List<Card>> listArray, List<List<Card>> allMeld, JArray allLayoff)
    {
        List<Card> temp = new List<Card>();
        foreach (var arr in listArray)
            temp.AddRange(arr);

        List<Card> array = new List<Card>();
        foreach (var cardTemp in temp)
        {
            if (array.FindIndex(c => c.code == cardTemp.code) < 0) array.Add(cardTemp);
        }

        List<Card> cardLeft = new List<Card>();
        List<Card> checkCard = new List<Card>();

        // xam truoc day sau
        List<List<Card>> option1 = new List<List<Card>>();
        array.Sort((a, b) =>
        {
            return a.S - b.S;
        });
        array.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        var listSameMeld = findSameMeld(array, true);
        if (listSameMeld.Count > 0)
            option1.Add(listSameMeld[0]);

        //array.forEach(cardTemp => {
        foreach (var cardTemp in array)
        {
            if (listSameMeld.Count == 0 || listSameMeld[0].FindIndex(c => c.code == cardTemp.code) < 0) checkCard.Add(cardTemp);

        }
        foreach (var cardTemp in array)
        {
            var checkCardLeft = false;
            foreach (var arr in allMeld)
            {
                if (arr.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCardLeft = true;
            }
            if (!checkCardLeft)
            {
                if (allLayoff.Count > 0)
                    checkCardLeft = (allLayoff.FirstOrDefault(data => (int)data["idCardSend"] == cardTemp.code)) != null || checkCardLeft;
            }
            if (checkCardLeft && checkCard.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCard.Remove(cardTemp);

        }
        if (checkCard.Count == 0) return option1;

        cardLeft.Clear();
        foreach (var cardTemp in array)
        {
            if (listSameMeld.Count == 0 || listSameMeld[0].FirstOrDefault(it => it.code == cardTemp.code) == null) cardLeft.Add(cardTemp);

        }

        cardLeft.Sort((a, b) =>
        {
            return a.S - b.S;
        });

        var listStraightMeldCardLeft = findStraightMeld(cardLeft, true);
        if (listStraightMeldCardLeft.Count > 0)
            option1.Add(listStraightMeldCardLeft[0]);

        checkCard.Clear();
        foreach (var cardTemp in array)
        {
            if ((listSameMeld.Count <= 0 || listSameMeld[0].FirstOrDefault(it => it.code == cardTemp.code) == null) &&
                (listStraightMeldCardLeft.Count <= 0 || listStraightMeldCardLeft[0].FirstOrDefault(it => it.code == cardTemp.code) == null)) checkCard.Add(cardTemp);
        }
        if (checkCard.Count == 0) return option1;
        foreach (var cardTemp in array)
        {
            var checkCardLeft = false;
            foreach (var arr in allMeld)
            {
                if (arr.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCardLeft = true;
            }
            if (!checkCardLeft)
            {
                if (allLayoff.Count > 0)
                    checkCardLeft = (allLayoff.FirstOrDefault(data => (int)data["idCardSend"] == cardTemp.code) != null) || checkCardLeft;
            }

            if (checkCardLeft && checkCard.Contains(cardTemp)) checkCard.Remove(cardTemp);

        }
        if (checkCard.Count == 0) return option1;
        var score1 = checkCard.Count;

        List<List<Card>> option2 = new List<List<Card>>();

        array.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        array.Sort((a, b) =>
        {
            return a.S - b.S;
        });


        var listStraightMeld = findStraightMeld(array, true);
        if (listStraightMeld.Count > 0)
            option2.Add(listStraightMeld[0]);

        checkCard.Clear();
        foreach (var cardTemp in array)
        {
            if (listStraightMeld.Count == 0 || listStraightMeld[0].FindIndex(c => c.code == cardTemp.code) < 0)
                checkCard.Add(cardTemp);

        }
        foreach (var cardTemp in array)
        {
            var checkCardLeft = false;
            foreach (var arr in allMeld)
            {
                if (arr.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCardLeft = true;
            }
            if (!checkCardLeft)
            {
                if (allLayoff.Count > 0)
                    checkCardLeft = (allLayoff.FirstOrDefault(data => (int)data["idCardSend"] == cardTemp.code) != null) || checkCardLeft;
            }
            if (checkCardLeft && checkCard.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCard.Remove(cardTemp);

        }
        if (checkCard.Count == 0) return option2;
        cardLeft.Clear();
        foreach (var cardTemp in array)
        {
            if (listStraightMeld.Count == 0 || listStraightMeld[0].FirstOrDefault(it => it.code == cardTemp.code) == null) cardLeft.Add(cardTemp);

        }

        cardLeft.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        var listSameMeldCardLeft = findSameMeld(cardLeft, true);
        if (listSameMeldCardLeft.Count > 0)
            option2.Add(listSameMeldCardLeft[0]);

        checkCard.Clear();

        foreach (var cardTemp in array)
        {
            if ((listSameMeldCardLeft.Count <= 0 || listSameMeldCardLeft[0].FirstOrDefault(it => it.code == cardTemp.code) == null) &&
                (listStraightMeld.Count <= 0 || listStraightMeld[0].FirstOrDefault(it => it.code == cardTemp.code) == null)) checkCard.Add(cardTemp);

        }
        if (checkCard.Count == 0) return option2;
        foreach (var cardTemp in array)
        {
            var checkCardLeft = false;
            foreach (var arr in allMeld)
            {
                if (arr.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCardLeft = true;
            }
            if (!checkCardLeft)
            {
                if (allLayoff.Count > 0)
                    checkCardLeft = (allLayoff.FirstOrDefault(data => (int)data["idCardSend"] == cardTemp.code) != null) || checkCardLeft;
            }
            if (checkCardLeft && checkCard.FirstOrDefault(it => it.code == cardTemp.code) != null) checkCard.Remove(cardTemp);

        }
        if (checkCard.Count == 0) return option2;
        var score2 = checkCard.Count;
        if (score1 <= score2) return option1;
        return option2;
    }
    JArray findOverLapLayoff(JObject _data, JArray listData)
    {
        JArray result = new JArray();
        foreach (var data in listData)
        {
            if (_data["idCardSend"] == data["idCardSend"])
            {
                result.Add(data);
            }
        }

        return result;
    }

    JObject resolveOverLapLayoff(JArray listData, JArray allData)
    {
        JObject bestCase = new JObject();
        var bestCount = 0;
        foreach (JObject data1 in listData)
        {
            var count = 0;
            foreach (JObject data2 in allData)
            {
                if (data2["idMeld"] == data1["idMeld"] && data2["sendtoPlayer"] == data1["sendtoPlayer"])
                {
                    count++;
                }
            }
            if (count > bestCount)
            {
                bestCount = count;
                bestCase = data1;
            }
        }
        //cc.NGWlog('!==> resolve layoff overlap, best case', bestCount, bestCase)
        return bestCase;
    }

    /*-=-=-==start new code==-=-=-=-*/
    JArray simulateAllLayoff(List<Card> _array, ref List<Card> lsCardFree, ref List<List<Card>> lsLayoff)
    {
        var vectorCard = new List<Card>(_array);

        vectorCard.Sort((a, b) =>
        {
            return a.N == b.N ? a.S - b.S : a.N - b.N;
        });
        //var cardLeft = vectorCard; //xep cac sanh theo chat
        JArray dataLayoff = new JArray();

        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            for (var j = 0; j < player.vectorCardD2.Count; j++)
            {
                var temp = new List<Card>(vectorCard); //card cua player
                var testArray = new List<Card>(player.vectorCardD2[j]);
                JArray dataLayoffTemp = new JArray();
                var lsLayoffTemp = new List<Card>(); //card cua player
                for (var k = 0; k < temp.Count; k++)
                {
                    var cardTemp = temp[k];
                    var testArrayTemp = new List<Card>(testArray);

                    testArrayTemp.Add(cardTemp);

                    if (checkValidMeld(testArrayTemp))
                    { //check xem co gui dc k
                        testArray.Add(cardTemp);
                        var index = vectorCard.FindIndex(c => c.code == cardTemp.code);
                        if (index >= 0)
                        {
                            lsLayoffTemp.Add(cardTemp);
                            vectorCard.RemoveAt(index); //card gui dc thi xoa khoi list dang check
                        }

                        temp.Remove(cardTemp);
                        JObject data = new JObject();
                        data["idCardSend"] = cardTemp.code;
                        data["idMeld"] = j;
                        data["sendtoPlayer"] = player.id;
                        data["info"] = cardTemp.LogInfo();
                        dataLayoffTemp.Add(data);
                        k = -1;
                    }
                }

                if (lsLayoffTemp.Count > 0)
                {
                    lsLayoff.Add(lsLayoffTemp);
                    dataLayoff.Add(dataLayoffTemp);
                }
            }
        }
        lsCardFree.AddRange(vectorCard);
        return dataLayoff;
    }

    JArray simulateAllLayoff(List<Card> _array, ref List<Card> lsCardFree, ref List<Card> lsLayoff)
    {
        var vectorCard = new List<Card>();
        vectorCard.AddRange(_array);

        vectorCard.Sort((a, b) =>
        {
            return a.N == b.N ? a.S - b.S : a.N - b.N;
        });
        //var cardLeft = vectorCard; //xep cac sanh theo chat
        JArray dataLayoff = new JArray();

        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            for (var j = 0; j < player.vectorCardD2.Count; j++)
            {
                var temp = new List<Card>(vectorCard); //card cua player
                var testArray = new List<Card>(player.vectorCardD2[j]);

                for (var k = 0; k < temp.Count; k++)
                {
                    var cardTemp = temp[k];
                    var testArrayTemp = new List<Card>(testArray);

                    testArrayTemp.Add(cardTemp);

                    if (checkValidMeld(testArrayTemp))
                    { //check xem co gui dc k
                        testArray.Add(cardTemp);
                        var index = vectorCard.FindIndex(c => c.code == cardTemp.code);
                        if (index >= 0)
                        {
                            lsLayoff.Add(cardTemp);
                            vectorCard.RemoveAt(index); //card gui dc thi xoa khoi list dang check
                        }

                        temp.Remove(cardTemp);
                        JObject data = new JObject();
                        data["idCardSend"] = cardTemp.code;
                        data["idMeld"] = j;
                        data["sendtoPlayer"] = player.id;
                        data["info"] = cardTemp.LogInfo();
                        dataLayoff.Add(data);
                        k = -1;
                    }
                }
            }
        }
        lsCardFree.AddRange(vectorCard);
        return dataLayoff;
    }

    List<List<Card>> findStraightMeld(List<Card> _listCard, ref List<Card> lsCardFree)
    {
        List<List<Card>> result = new List<List<Card>>();
        if (_listCard.Count < 3)
        {
            lsCardFree.AddRange(_listCard);
            return result;
        }
        var arrayCard = new List<Card>(_listCard);
        arrayCard.Sort((x, y) =>
        {
            if (x.N != y.N) return x.N - y.N;
            else return x.S - y.S;
        });

        for (var i = 0; i < arrayCard.Count - 1; i++)
        {
            var NFirst = arrayCard[i].N;
            List<Card> testArray = new List<Card>();
            testArray.Add(arrayCard[i]);
            for (var j = i + 1; j < arrayCard.Count; j++)
            {
                if (arrayCard[i].S == arrayCard[j].S && NFirst == arrayCard[j].N - 1)
                {
                    NFirst = arrayCard[j].N;
                    testArray.Add(arrayCard[j]);
                }
            }
            if (testArray.Count >= 3)
            {
                result.Add(testArray);
                foreach (var c in testArray)
                {
                    arrayCard.Remove(c);
                }
                i = -1;
            }
        }

        lsCardFree.AddRange(arrayCard);

        return result;
    }

    List<List<Card>> findSameMeld(List<Card> _listCard, ref List<Card> lsCardFree)
    {
        var arrayCard = new List<Card>(_listCard);
        List<List<Card>> result = new List<List<Card>>();

        arrayCard.Sort((a, b) =>
        {
            return a.N - b.N;
        });
        var cardGroup = arrayCard.GroupBy(c => c.N);
        foreach (var group in cardGroup)
        {
            if (group.Count() >= 3)
            {
                result.Add(group.ToList());
            }
            else
            {
                lsCardFree.AddRange(group.ToList());
            }
        }
        return result;
    }

    DummyDataCustom simulateKnockOutSame(ref bool isKnock)
    {

        DummyDataCustom result = new DummyDataCustom();

        var cuntLoop = 0;
        isKnock = false;
        List<Card> arr = new List<Card>();
        arr.AddRange(thisPlayer.vectorCard);
        arr.Sort((x, y) =>
        {
            if (x.N != y.N) return x.N - y.N;
            else return x.S - y.S;
        });

        var lsCardFree = new List<Card>();
        var lsCardFree2 = new List<Card>();


        var lsMeldSame = findSameMeld(arr, ref lsCardFree2);
        lsMeldSame.Sort((x, y) =>
        {
            return x[0].N - y[0].N;
        });

        var lsAllMeld = new List<List<Card>>();
        var lsCardLayoff = new List<Card>();
        JArray checkLayoff;

        var lsCardFreeBackUp = new List<Card>();
        {
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<Card>();
            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree2), ref _lsCardFree, ref _lsCardLayoff);


            if (_lsCardFree.Count <= 1)
            {
                lsCardFree = _lsCardFree;

                var _lsCardFree2 = new List<Card>();
                lsAllMeld = lsMeldSame;
                lsCardLayoff = _lsCardLayoff;
                checkLayoff = _checkLayoff;
                Logging.Log(" goto CHECK_AGAIN -1");
                goto CHECK_AGAIN;
            }
        }

        {
            for (var i = 0; i < lsMeldSame.Count; i++)
            {
                var _lsCardFree2 = new List<Card>();
                var _lsCardFree = new List<Card>(lsCardFree2);
                _lsCardFree.AddRange(lsMeldSame[i]);
                var _lsMeldStraight = findStraightMeld(_lsCardFree, ref _lsCardFree2);

                var _lsCardFree22 = new List<Card>();
                var _lsCardLayoff = new List<Card>();
                var _checkLayoff = simulateAllLayoff(new List<Card>(_lsCardFree2), ref _lsCardFree22, ref _lsCardLayoff);
                if (_lsCardFree22.Count <= 1)
                {
                    lsMeldSame.RemoveAt(i);
                    lsAllMeld = lsMeldSame;
                    lsAllMeld.AddRange(_lsMeldStraight);
                    lsCardLayoff = _lsCardLayoff;
                    checkLayoff = _checkLayoff;
                    lsCardFree = _lsCardFree22;

                    Logging.Log(" goto CHECK_AGAIN -211");
                    goto CHECK_AGAIN;
                }
            }
        }

        var lsMeldStraight = findStraightMeld(lsCardFree2, ref lsCardFree);
        {
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<Card>();
            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree), ref _lsCardFree, ref _lsCardLayoff);


            if (_lsCardFree.Count <= 1)
            {
                lsCardFree = _lsCardFree;

                lsAllMeld.AddRange(lsMeldSame);
                lsAllMeld.AddRange(lsMeldStraight);

                lsCardLayoff = _lsCardLayoff;
                checkLayoff = _checkLayoff;
                Logging.Log(" goto CHECK_AGAIN -1");
                goto CHECK_AGAIN;
            }
        }
        lsAllMeld.AddRange(lsMeldSame);
        lsAllMeld.AddRange(lsMeldStraight);

        lsCardFreeBackUp.AddRange(lsCardFree);

        lsCardFree.Clear();
        checkLayoff = simulateAllLayoff(lsCardFreeBackUp, ref lsCardFree, ref lsCardLayoff); //check cac card gui dc
        if (lsAllMeld.Count == 0 && lsCardFree.Count <= 1 && (lsCardLayoff.Count + lsCardFree.Count) == arr.Count)
        {
            Logging.Log("simulateKnockOutSame CHECK_AGAIN  0");
            goto CHECK_AGAIN;
        }

        if (lsCardFree.Count >= thisPlayer.vectorCard.Count)
        {
            result.cardLeft = lsCardFree;
            isKnock = false;
            Logging.Log("simulateKnockOutSame  0");
            return result;
        }
    CHECK_AGAIN:
        ////u cmnr
        if (lsCardFree.Count == 0)
        {
            cuntLoop++;
            if (checkLayoff.Count > 0)
            {
                if (checkLayoff.Count > 3)
                {
                    var lsCardFree3 = new List<Card>();
                    var lsCardFree33 = new List<Card>();
                    var lsMeldSameTemp = findSameMeld(new List<Card>(lsCardLayoff), ref lsCardFree3);
                    var lsMeldStraightTemp = findStraightMeld(lsCardFree3, ref lsCardFree33);
                    if (lsMeldSameTemp.Count > 0 || lsMeldStraightTemp.Count > 0)
                    {
                        bool isOK = false;
                        if (lsCardFree33.Count != 0)
                        {
                            var _lsCardFree = new List<Card>();
                            var _lsCardLayoff = new List<Card>();
                            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree33), ref _lsCardFree, ref _lsCardLayoff);
                            if (_lsCardFree.Count <= 1)
                            {
                                isOK = true;
                                lsCardFree33 = _lsCardFree;
                                checkLayoff = _checkLayoff;
                                lsCardLayoff = _lsCardLayoff;
                            }
                        }
                        else
                        {
                            //gửi đc hết
                            if (lsAllMeld.FirstOrDefault(x => x.Count > 3) != null || lsMeldSameTemp.FirstOrDefault(x => x.Count > 3) != null || lsMeldStraightTemp.FirstOrDefault(x => x.Count > 3) != null)
                            {
                                checkLayoff.Clear();
                                lsCardLayoff.Clear();
                                isOK = true;
                            }
                            else
                            {
                                for (var i = 0; i < lsMeldSameTemp.Count; i++)
                                {
                                    var _lsCardFree = new List<Card>();
                                    var _lsCardLayoff = new List<Card>();
                                    var checkLayofff22 = simulateAllLayoff(new List<Card>(lsMeldSameTemp[i]), ref _lsCardFree, ref _lsCardLayoff);
                                    if (_lsCardLayoff.Count == lsMeldSameTemp[i].Count)
                                    {
                                        lsMeldSameTemp.RemoveAt(i);
                                        checkLayoff = checkLayofff22;
                                        lsCardLayoff = _lsCardLayoff;
                                        isOK = true;
                                        break;
                                    }
                                }
                                if (!isOK)
                                {
                                    for (var i = 0; i < lsMeldStraightTemp.Count; i++)
                                    {
                                        var _lsCardFree = new List<Card>();
                                        var _lsCardLayoff = new List<Card>();
                                        var checkLayofff22 = simulateAllLayoff(new List<Card>(lsMeldStraightTemp[i]), ref _lsCardFree, ref _lsCardLayoff);
                                        if (_lsCardLayoff.Count == lsMeldStraightTemp[i].Count)
                                        {
                                            lsMeldStraightTemp.RemoveAt(i);

                                            checkLayoff = checkLayofff22;
                                            lsCardLayoff = _lsCardLayoff;
                                            isOK = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (isOK)
                        {
                            lsCardFree = lsCardFree33;
                            if (lsMeldSameTemp.Count > 0)
                                lsAllMeld.AddRange(lsMeldSameTemp);
                            if (lsMeldStraightTemp.Count > 0)
                                lsAllMeld.AddRange(lsMeldStraightTemp);
                            goto CHECK_AGAIN;
                        }
                    }
                }

                result.allMeld = lsAllMeld;
                result.allLayoff = checkLayoff;
                result.cardLeft = lsCardFree;

                isKnock = true;
                Logging.Log("simulateKnockOutSame  1");
                return result;
            }
            else if (lsAllMeld.FirstOrDefault(x => x.Count > 3) != null)
            {
                var lsCardFree3 = new List<Card>();
                var lsCardLayoff3 = new List<Card>();
                var checkAllLayoff = simulateAllLayoff(arr, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
                for (var i = 0; i < lsAllMeld.Count; i++)
                {
                    if (lsAllMeld[i].Count <= 3) continue;
                    for (var j = 0; j < lsAllMeld[i].Count; j++)
                    {
                        var ll = checkAllLayoff.FirstOrDefault(c => (int)c["idCardSend"] == lsAllMeld[i][j].code);
                        //var ll = lsCardLayoff3.FirstOrDefault(c =>c.code == lsAllMeld[i][j].code);

                        if (ll != null)
                        {
                            var pl = getPlayerWithID((int)ll["sendtoPlayer"]);
                            var lsCaTemp = new List<Card>(pl.vectorCardD2[(int)ll["idMeld"]]);
                            var cc = lsCardLayoff3.FirstOrDefault(c => c.code == (int)ll["idCardSend"]);
                            lsCaTemp.Add(cc);
                            if (!checkValidMeld(lsCaTemp)) continue;
                            if (checkSameCard(lsAllMeld[i]))
                            {
                                result.allLayoff.Add(ll);
                                lsAllMeld[i].RemoveAt(j);
                                result.allMeld = lsAllMeld;
                                isKnock = true;
                                Logging.Log("simulateKnockOutSame  2");
                                return result;
                            }
                            else if (j == 0 || j == lsAllMeld[i].Count - 1)
                            {
                                result.allLayoff.Add(ll);
                                lsAllMeld[i].RemoveAt(j);
                                result.allMeld = lsAllMeld;
                                isKnock = true;
                                Logging.Log("simulateKnockOutSame  3");
                                return result;
                            }
                            else
                            {
                                if (lsAllMeld[i].Count < 6)
                                {
                                    continue;
                                }

                                if (j >= 3 && j <= lsAllMeld[i].Count - 3)
                                {
                                    lsAllMeld[i].RemoveAt(j);
                                    var lsTemo = new List<Card>();
                                    var newMel = findStraightMeld(lsAllMeld[i], ref lsTemo);

                                    lsAllMeld[i] = newMel[0];
                                    newMel.RemoveAt(0);
                                    lsAllMeld.AddRange(newMel);

                                    result.allLayoff.Add(ll);
                                    result.allMeld = lsAllMeld;
                                    isKnock = true;
                                    Logging.Log("simulateKnockOutSame  4");
                                    return result;
                                }
                            }
                        }
                    }
                }

                for (var i = 0; i < lsAllMeld.Count; i++)
                {
                    if (lsAllMeld[i].Count <= 3) continue;
                    //result.cardLeft.Add(lsAllMeld[i][0]);

                    var arrr = new JArray();
                    var data = new JObject();
                    data["idCardSend"] = lsAllMeld[i][0].code;
                    data["idMeld"] = thisPlayer.vectorCardD2.Count;
                    data["sendtoPlayer"] = thisPlayer.id;
                    arrr.Add(data);
                    result.allLayoff = arrr;

                    lsAllMeld[i].RemoveAt(0);
                    result.allMeld = lsAllMeld;
                    isKnock = true;
                    Logging.Log("simulateKnockOutSame  5");
                    return result;
                }
            }
            else
            {
                if (lsCardFree.Count <= 0 && checkLayoff.Count <= 0)
                {
                    isKnockOut = false;
                    result.allMeld = lsAllMeld;
                    Logging.Log("simulateKnockOutSame  6");
                    return result;
                }
                //phá phỏm xem ù ko
                bool issHas = false;
                for (var i = 0; i < lsCardFreeBackUp.Count; i++)
                {
                    if (issHas) break;
                    foreach (var ls in lsAllMeld)
                    {
                        if (issHas) break;
                        if (ls.FirstOrDefault(x => x.code == lsCardFreeBackUp[i].code) != null)
                        {
                            //ls
                            var lsTest = new List<Card>();
                            lsTest.AddRange(ls);

                            var lsMeldSameTest = new List<List<Card>>();
                            lsMeldSameTest.AddRange(lsAllMeld);

                            //check test xem đủ đk ko
                            for (var j = 0; j < lsTest.Count; j++)
                            {
                                if (issHas) break;
                                var ccTest = lsTest[j];
                                if (ccTest.code == lsCardFreeBackUp[i].code) continue;
                                foreach (var ls2 in lsMeldSameTest)
                                {
                                    ls2.Add(ccTest);
                                    if (checkValidMeld(ls2))
                                    {
                                        lsTest.Remove(ccTest);
                                        j = -1;
                                    }
                                    else
                                    {
                                        ls2.Remove(ccTest);
                                    }
                                }
                                if (lsTest.Count == 1)//đủ đk
                                {
                                    break;
                                }
                            }

                            if (lsTest.Count == 1)//đủ đk
                            {
                                //lsCardFree.AddRange(lsCardFree);

                                lsCardFree = Add2RangeCustom(lsCardFree, lsCardFree);
                                lsAllMeld.Remove(ls);
                                result.cardLeft = lsCardFree;
                                result.allMeld = lsAllMeld;
                                Logging.Log("simulateKnockOutSame  7");
                                return result;
                            }
                        }
                    }
                }

                if (lsCardFree.Count <= 0 && checkLayoff.Count <= 0) isKnock = false;
                result.cardLeft = lsCardFree;
                result.allMeld = lsAllMeld;
                Logging.Log("simulateKnockOutSame  8");
                return result;
            }
        }
        else if (lsCardFree.Count == 1)
        {
            cuntLoop++;
            {
                var lsCardFree3 = new List<Card>();
                var lsCardFree33 = new List<Card>();
                var lsMeldSameTemp = findSameMeld(new List<Card>(lsCardLayoff), ref lsCardFree3);
                var lsMeldStraightTemp = findStraightMeld(lsCardFree3, ref lsCardFree33);
                if (lsMeldSameTemp.Count > 0 || lsMeldStraightTemp.Count > 0)
                {
                    bool isOK = false;
                    if (lsCardFree33.Count != 0)
                    {
                        var _lsCardFree = new List<Card>();
                        var _lsCardLayoff = new List<Card>();
                        var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree33), ref _lsCardFree, ref _lsCardLayoff);
                        if (_lsCardFree.Count == 0 && _lsCardLayoff.Count > 0)
                        {
                            isOK = true;
                            checkLayoff = _checkLayoff;
                            lsCardLayoff = _lsCardLayoff;
                        }
                    }
                    else
                    {
                        checkLayoff.Clear();
                        lsCardLayoff.Clear();
                        isOK = true;
                    }

                    if (isOK)
                    {
                        lsAllMeld.AddRange(lsMeldSameTemp);
                        lsAllMeld.AddRange(lsMeldStraightTemp);
                    }
                }
            }
            result.allMeld = lsAllMeld;
            result.allLayoff = checkLayoff;
            result.cardLeft = lsCardFree;

            isKnock = true;

            Logging.Log("simulateKnockOutStraight 0");
            return result;
        }

        {
            var listTemp = new List<Card>();
            listTemp.AddRange(lsCardFree);
            listTemp.AddRange(lsCardLayoff);

            var listTemp2 = new List<Card>();
            for (var i = 0; i < listTemp.Count; i++)
            {
                for (var j = 0; j < lsAllMeld.Count; j++)
                {
                    if (lsAllMeld[j].Count <= 3) continue;
                    var ls = lsAllMeld[j].FirstOrDefault(x => x.S == listTemp[i].S);
                    if (ls != null)
                    {
                        listTemp2 = Add2RangeCustom(listTemp2, lsAllMeld[j]);
                    }
                }
            }

            listTemp.AddRange(listTemp2);

            var listFreeTemp = new List<Card>();
            var lsMeldBonus = findStraightMeld(listTemp, ref listFreeTemp);

            for (var i = 0; i < lsMeldBonus.Count; i++)
            {
                var lssss = lsMeldBonus[i];

                Dictionary<int, List<List<Card>>> dicBackup = new Dictionary<int, List<List<Card>>>();
                Dictionary<int, List<Card>> dicCardBackup = new Dictionary<int, List<Card>>();
                for (var j = 0; j < lssss.Count; j++)
                {
                    for (var k = 0; k < lsAllMeld.Count; k++)
                    {
                        //if (lsAllMeld[k].Count <= 3) continue;
                        var ls = lsAllMeld[k].FirstOrDefault(x => x.code == lssss[j].code);

                        if (ls != null)
                        {
                            lsAllMeld[k].Remove(ls);
                            if (dicCardBackup.ContainsKey(k))
                            {
                                if (!dicCardBackup[k].Contains(ls))
                                {
                                    dicCardBackup[k].Add(ls);
                                }
                            }
                            else
                            {
                                dicCardBackup.Add(k, new List<Card>() { ls });
                            }

                            if (!checkValidMeld(lsAllMeld[k]))
                            {
                                var lsTemo = new List<Card>();
                                var newMel = findStraightMeld(lsAllMeld[k], ref lsTemo);
                                if (lsTemo.Count == 0)
                                {
                                    dicBackup.Add(k, newMel);
                                }
                                else
                                {
                                    //dicCardBackup.Remove(k);
                                    //lsAllMeld[k].Add(ls);
                                    lsAllMeld[k] = Add2RangeCustom(lsAllMeld[k], dicCardBackup[k]);
                                    dicCardBackup.Remove(k);
                                    lsAllMeld[k].Sort((x, y) =>
                                    {
                                        if (x.N != y.N) return x.N - y.N;
                                        else return x.S - y.S;
                                    });
                                    var cc = lssss.FirstOrDefault(x => x.code == ls.code);
                                    if (cc != null)
                                    {
                                        lssss.Remove(cc);
                                        j--;
                                    }
                                }

                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                if (checkValidMeld(lssss) && dicCardBackup.Count > 0)
                {
                    foreach (var gg in dicBackup)
                    {
                        if (gg.Value.Count > 1)
                        {
                            lsAllMeld[gg.Key] = gg.Value[0];
                            for (var ijij = 1; ijij < gg.Value.Count; ijij++)
                            {
                                lsAllMeld.Add(gg.Value[ijij]);
                            }
                        }
                    }
                    foreach (var gg in dicCardBackup)
                    {
                        if (checkValidMeld(gg.Value))
                        {
                            lsAllMeld.Add(gg.Value);
                        }
                    }
                    for (var j = 0; j < lssss.Count; j++)
                    {
                        var cc = lsCardFree.FirstOrDefault(x => x.code == lssss[j].code);
                        if (cc != null)
                        {
                            lsCardFree.Remove(cc);
                        }
                        var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lssss[j].code);
                        if (cc2 != null)
                        {
                            lsCardLayoff.Remove(cc2);
                        }
                        var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lssss[j].code);
                        if (cc3 != null)
                        {
                            checkLayoff.Remove(cc3);
                        }
                    }
                    lsAllMeld.Add(lssss);
                    lsMeldBonus.Remove(lssss);
                    i = lsMeldBonus.Count;
                }
                else
                {
                    foreach (var gg in dicCardBackup)
                    {
                        //gg.Key
                        //lsAllMeld[gg.Key].AddRange(gg.Value);
                        lsAllMeld[gg.Key] = Add2RangeCustom(lsAllMeld[gg.Key], gg.Value);
                        lsAllMeld[gg.Key].Sort((x, y) =>
                        {
                            if (x.N != y.N) return x.N - y.N;
                            else return x.S - y.S;
                        });
                    }
                }
            }
            var lsCardLayoff22 = new List<Card>();
            var lsCardFree22 = new List<Card>();
            var checkLayoffTep = simulateAllLayoff(lsCardLayoff, ref lsCardFree22, ref lsCardLayoff22); //check cac card gui dc
            for (var i = 0; i < lsCardFree22.Count; i++)
            {
                if (lsCardFree.FirstOrDefault(x => x.code == lsCardFree22[i].code) == null)
                {
                    var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lsCardFree22[i].code);
                    if (cc2 != null)
                    {
                        lsCardLayoff.Remove(cc2);
                    }
                    var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lsCardFree22[i].code);
                    if (cc3 != null)
                    {
                        checkLayoff.Remove(cc3);
                    }
                    lsCardFree.Add(lsCardFree22[i]);
                }
            }
            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log("simulateKnockOutSame CHECK_AGAIN  6");
                goto CHECK_AGAIN;
            }
        }

        var lsCardFree4 = new List<Card>();
        var lsCardLayoff4 = new List<Card>();
        lsCardFree.AddRange(lsCardLayoff);
        var checkLayoff2 = simulateAllLayoff(lsCardFree, ref lsCardFree4, ref lsCardLayoff4); //check cac card gui dc

        if (lsCardFree4.Count <= 1)
        {
            isKnock = true;
        }

        result.allMeld = lsAllMeld;
        result.allLayoff = checkLayoff2;
        result.cardLeft = lsCardFree4;


        Logging.Log("simulateKnockOutSame  10");
        return result;
    }

    string logListCard(List<Card> list, bool isLog = true)
    {
        var str = "";
        for (var i = 0; i < list.Count; i++)
        {
            str += list[i].LogInfo() + " ";
        }
        if (isLog)
            Logging.Log("logListCard \n" + str);

        return str;
    }

    List<Card> Add2RangeCustom(List<Card> lsCheck, List<Card> lsIn)
    {
        List<Card> lsReslt = new List<Card>();
        lsReslt.AddRange(lsCheck);
        lsReslt.AddRange(lsIn.FindAll(x => lsReslt.FirstOrDefault(y => y.code == x.code) == null));
        return lsReslt;
    }

    JArray Add2RangeCustom(JArray lsCheck, JArray lsIn)
    {
        JArray lsReslt = new JArray(lsCheck);
        for (var i = 0; i < lsIn.Count; i++)
        {
            if (lsReslt.FirstOrDefault(x => (int)x["idCardSend"] == (int)lsIn[i]["idCardSend"]) == null)
            {
                lsReslt.Add(lsIn[i]);
            }
        }

        return lsReslt;
    }

    DummyDataCustom simulateKnockOutStraight(ref bool isKnock)
    {
        DummyDataCustom result = new DummyDataCustom();

        var cuntLoop = 0;
        isKnock = false;
        List<Card> arr = new List<Card>();
        arr.AddRange(thisPlayer.vectorCard);
        arr.Sort((x, y) =>
        {
            if (x.N != y.N) return x.N - y.N;
            else return x.S - y.S;
        });

        var lsCardFree = new List<Card>();
        var lsCardFree2 = new List<Card>();


        var lsMeldStraight = findStraightMeld(arr, ref lsCardFree2);
        var lsCardLayoff = new List<Card>();
        var lsAllMeld = new List<List<Card>>();
        JArray checkLayoff;

        var lsCardFreeBackUp = new List<Card>();
        {
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<Card>();
            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree2), ref _lsCardFree, ref _lsCardLayoff);

            if (_lsCardFree.Count <= 1)
            {
                lsCardFree = _lsCardFree;

                var _lsCardFree2 = new List<Card>();
                lsAllMeld = lsMeldStraight;
                lsCardLayoff = _lsCardLayoff;
                checkLayoff = _checkLayoff;
                Logging.Log(" goto CHECK_AGAIN 101");
                goto CHECK_AGAIN;
            }
        }

        var lsMeldSame = findSameMeld(lsCardFree2, ref lsCardFree);
        lsMeldSame.Sort((x, y) =>
        {
            return x[0].N - y[0].N;
        });
        {
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<Card>();
            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree), ref _lsCardFree, ref _lsCardLayoff);


            if (_lsCardFree.Count <= 1)
            {
                lsCardFree = _lsCardFree;
                lsAllMeld.AddRange(lsMeldStraight);
                lsAllMeld.AddRange(lsMeldSame);

                lsCardLayoff = _lsCardLayoff;
                checkLayoff = _checkLayoff;
                Logging.Log(" goto CHECK_AGAIN -1");
                goto CHECK_AGAIN;
            }
        }


        lsAllMeld.AddRange(lsMeldStraight);
        lsAllMeld.AddRange(lsMeldSame);


        lsCardFreeBackUp.AddRange(lsCardFree);

        lsCardFree.Clear();

        checkLayoff = simulateAllLayoff(lsCardFreeBackUp, ref lsCardFree, ref lsCardLayoff); //check cac card gui dc
        if (lsAllMeld.Count == 0 && lsCardFree.Count <= 1 && (lsCardLayoff.Count + lsCardFree.Count) == arr.Count)
        {
            goto CHECK_AGAIN;
        }

        if (lsCardFree.Count >= thisPlayer.vectorCard.Count)
        {
            result.cardLeft = lsCardFree;
            isKnock = false;
            return result;
        }
    CHECK_AGAIN:
        if (lsCardFree.Count == 0)
        {
            cuntLoop++;
            if (checkLayoff.Count > 0)
            {
                if (checkLayoff.Count > 3)
                {
                    var lsCardFree3 = new List<Card>();
                    var lsCardFree33 = new List<Card>();
                    var lsMeldSameTemp = findSameMeld(new List<Card>(lsCardLayoff), ref lsCardFree3);
                    var lsMeldStraightTemp = findStraightMeld(lsCardFree3, ref lsCardFree33);
                    if (lsMeldSameTemp.Count > 0 || lsMeldStraightTemp.Count > 0)
                    {
                        bool isOK = false;
                        if (lsCardFree33.Count != 0)
                        {
                            var _lsCardFree = new List<Card>();
                            var _lsCardLayoff = new List<Card>();
                            var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree33), ref _lsCardFree, ref _lsCardLayoff);
                            if (_lsCardFree.Count <= 1)
                            {
                                isOK = true;
                                lsCardFree33 = _lsCardFree;
                                checkLayoff = _checkLayoff;
                                lsCardLayoff = _lsCardLayoff;
                            }
                        }
                        else
                        {
                            //gửi đc hết
                            if (lsAllMeld.FirstOrDefault(x => x.Count > 3) != null || lsMeldSameTemp.FirstOrDefault(x => x.Count > 3) != null || lsMeldStraightTemp.FirstOrDefault(x => x.Count > 3) != null)
                            {
                                checkLayoff.Clear();
                                lsCardLayoff.Clear();
                                isOK = true;
                            }
                            else
                            {
                                for (var i = 0; i < lsMeldSameTemp.Count; i++)
                                {
                                    var _lsCardFree = new List<Card>();
                                    var _lsCardLayoff = new List<Card>();
                                    var checkLayofff22 = simulateAllLayoff(new List<Card>(lsMeldSameTemp[i]), ref _lsCardFree, ref _lsCardLayoff);
                                    if (_lsCardLayoff.Count == lsMeldSameTemp[i].Count)
                                    {
                                        lsMeldSameTemp.RemoveAt(i);
                                        checkLayoff = checkLayofff22;
                                        lsCardLayoff = _lsCardLayoff;
                                        isOK = true;
                                        break;
                                    }
                                }
                                if (!isOK)
                                {
                                    for (var i = 0; i < lsMeldStraightTemp.Count; i++)
                                    {
                                        var _lsCardFree = new List<Card>();
                                        var _lsCardLayoff = new List<Card>();
                                        var checkLayofff22 = simulateAllLayoff(new List<Card>(lsMeldStraightTemp[i]), ref _lsCardFree, ref _lsCardLayoff);
                                        if (_lsCardLayoff.Count == lsMeldStraightTemp[i].Count)
                                        {
                                            lsMeldStraightTemp.RemoveAt(i);

                                            checkLayoff = checkLayofff22;
                                            lsCardLayoff = _lsCardLayoff;
                                            isOK = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (isOK)
                        {
                            lsCardFree = lsCardFree33;
                            if (lsMeldSameTemp.Count > 0)
                                lsAllMeld.AddRange(lsMeldSameTemp);
                            if (lsMeldStraightTemp.Count > 0)
                                lsAllMeld.AddRange(lsMeldStraightTemp);
                            goto CHECK_AGAIN;
                        }
                    }
                }

                result.allMeld = lsAllMeld;
                result.allLayoff = checkLayoff;
                result.cardLeft = lsCardFree;

                isKnock = true;
                Logging.Log("simulateKnockOutSame  1");
                return result;


            }
            else if (lsAllMeld.FirstOrDefault(x => x.Count > 3) != null)
            {
                var lsCardFree3 = new List<Card>();
                var lsCardLayoff3 = new List<Card>();
                var checkAllLayoff = simulateAllLayoff(arr, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
                for (var i = 0; i < lsAllMeld.Count; i++)
                {
                    if (lsAllMeld[i].Count <= 3) continue;
                    for (var j = 0; j < lsAllMeld[i].Count; j++)
                    {
                        var ll = checkAllLayoff.FirstOrDefault(c => (int)c["idCardSend"] == lsAllMeld[i][j].code);
                        if (ll != null)
                        {
                            var pl = getPlayerWithID((int)ll["sendtoPlayer"]);
                            var lsCaTemp = new List<Card>(pl.vectorCardD2[(int)ll["idMeld"]]);
                            var cc = lsCardLayoff3.FirstOrDefault(c => c.code == (int)ll["idCardSend"]);
                            lsCaTemp.Add(cc);
                            if (!checkValidMeld(lsCaTemp)) continue;

                            if (checkSameCard(lsAllMeld[i]))
                            {
                                result.allLayoff.Add(ll);
                                lsAllMeld[i].RemoveAt(j);
                                result.allMeld = lsAllMeld;
                                isKnock = true;
                                return result;
                            }
                            else if (j == 0 || j == lsAllMeld[i].Count - 1)
                            {
                                result.allLayoff.Add(ll);
                                lsAllMeld[i].RemoveAt(j);
                                result.allMeld = lsAllMeld;
                                isKnock = true;
                                return result;
                            }
                            else
                            {
                                if (lsAllMeld[i].Count < 6)
                                {
                                    continue;
                                }

                                if (j >= 3 && j <= lsAllMeld[i].Count - 3)
                                {
                                    lsAllMeld[i].RemoveAt(j);
                                    var lsTemo = new List<Card>();
                                    var newMel = findStraightMeld(lsAllMeld[i], ref lsTemo);

                                    lsAllMeld[i] = newMel[0];
                                    newMel.RemoveAt(0);
                                    lsAllMeld.AddRange(newMel);

                                    result.allLayoff.Add(ll);
                                    result.allMeld = lsAllMeld;
                                    isKnock = true;
                                    return result;
                                }
                            }
                        }
                    }
                }

                for (var i = 0; i < lsAllMeld.Count; i++)
                {
                    if (lsAllMeld[i].Count <= 3) continue;
                    //result.cardLeft.Add(lsAllMeld[i][0]);

                    var arrr = new JArray();
                    var data = new JObject();
                    data["idCardSend"] = lsAllMeld[i][0].code;
                    data["idMeld"] = thisPlayer.vectorCardD2.Count;
                    data["sendtoPlayer"] = thisPlayer.id;
                    arrr.Add(data);
                    result.allLayoff = arrr;

                    lsAllMeld[i].RemoveAt(0);
                    result.allMeld = lsAllMeld;
                    isKnock = true;
                    Logging.Log("simulateKnockOutStraight 3");
                    return result;
                }
            }
            else
            {
                if (lsCardFree.Count <= 0 && checkLayoff.Count <= 0)
                {
                    isKnock = false;
                    for (var i = 0; i < lsAllMeld.Count; i++)
                    {
                        var lsCardFree909 = new List<Card>();
                        var lsCardLayoff909 = new List<Card>();
                        var chekL = simulateAllLayoff(lsAllMeld[i], ref lsCardFree909, ref lsCardLayoff909);
                        if (lsCardFree909.Count == 0)
                        {
                            isKnock = true;
                            checkLayoff = chekL;
                            lsAllMeld.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    result.allLayoff = checkLayoff;
                    result.allMeld = lsAllMeld;
                    Logging.Log("simulateKnockOutStraight 555  " + isKnock);
                    return result;
                }
                //phá phỏm xem ù ko
                //bool issHas = false;
                for (var i = 0; i < lsCardFreeBackUp.Count; i++)
                {
                    //if (issHas) break;
                    foreach (var ls in lsAllMeld)
                    {
                        //if (issHas) break;
                        if (ls.FirstOrDefault(x => x.code == lsCardFreeBackUp[i].code) != null)
                        {
                            //ls
                            var lsTest = new List<Card>();
                            lsTest.AddRange(ls);

                            var lsMeldSameTest = new List<List<Card>>();
                            lsMeldSameTest.AddRange(lsAllMeld);

                            //check test xem đủ đk ko
                            for (var j = 0; j < lsTest.Count; j++)
                            {
                                //if (issHas) break;
                                var ccTest = lsTest[j];
                                if (ccTest.code == lsCardFreeBackUp[i].code) continue;
                                foreach (var ls2 in lsMeldSameTest)
                                {
                                    ls2.Add(ccTest);
                                    if (checkValidMeld(ls2))
                                    {
                                        lsTest.Remove(ccTest);
                                        j = -1;
                                    }
                                    else
                                    {
                                        ls2.Remove(ccTest);
                                    }
                                }
                                if (lsTest.Count == 1)//đủ đk
                                {
                                    break;
                                }
                            }

                            if (lsTest.Count == 1)//đủ đk
                            {
                                lsCardFree.AddRange(lsTest);
                                lsAllMeld.Remove(ls);
                                result.cardLeft = lsCardFree;
                                result.allMeld = lsAllMeld;
                                Logging.Log("simulateKnockOutStraight 4");
                                return result;
                            }
                        }
                    }
                }

                result.cardLeft = lsCardFree;
                result.allMeld = lsAllMeld;
                Logging.Log("simulateKnockOutStraight 5");
                return result;
            }
        }
        else if (lsCardFree.Count == 1)
        {
            cuntLoop++;
            {
                var lsCardFree3 = new List<Card>();
                var lsCardFree33 = new List<Card>();
                var lsMeldSameTemp = findSameMeld(new List<Card>(lsCardLayoff), ref lsCardFree3);

                var lsMeldStraightTemp = findStraightMeld(lsCardFree3, ref lsCardFree33);
                if (lsMeldSameTemp.Count > 0 || lsMeldStraightTemp.Count > 0)
                {
                    bool isOK = false;
                    if (lsCardFree33.Count != 0)
                    {
                        var _lsCardFree = new List<Card>();
                        var _lsCardLayoff = new List<Card>();
                        var _checkLayoff = simulateAllLayoff(new List<Card>(lsCardFree33), ref _lsCardFree, ref _lsCardLayoff);
                        if (_lsCardFree.Count == 0 && _lsCardLayoff.Count > 0)
                        {
                            isOK = true;
                            checkLayoff = _checkLayoff;
                            lsCardLayoff = _lsCardLayoff;
                        }
                    }
                    else
                    {
                        checkLayoff.Clear();
                        lsCardLayoff.Clear();
                        isOK = true;
                    }

                    if (isOK)
                    {
                        lsAllMeld.AddRange(lsMeldSameTemp);
                        lsAllMeld.AddRange(lsMeldStraightTemp);
                    }
                }
            }
            result.allMeld = lsAllMeld;
            result.allLayoff = checkLayoff;
            result.cardLeft = lsCardFree;

            isKnock = true;

            Logging.Log("simulateKnockOutStraight 12323");
            return result;
        }



        {
            DummyDataCustom resultTemp = getMeldKnock(lsAllMeld, lsCardFree, new List<Card>(), new JArray(), true);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            if (resultTemp.allLayoff.Count != 0)
            {
                lsCardLayoff = Add2RangeCustom(lsCardLayoff, resultTemp.lsLayoff);
                checkLayoff = Add2RangeCustom(checkLayoff, resultTemp.allLayoff);
            }

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 0");
                goto CHECK_AGAIN;
            }

            resultTemp = getMeldKnock(lsAllMeld, lsCardFree, new List<Card>(), new JArray(), false);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            if (resultTemp.allLayoff.Count != 0)
            {
                lsCardLayoff = Add2RangeCustom(lsCardLayoff, resultTemp.lsLayoff);
                checkLayoff = Add2RangeCustom(checkLayoff, resultTemp.allLayoff);
            }

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 1");
                goto CHECK_AGAIN;
            }

            resultTemp = getMeldKnock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, true);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            lsCardLayoff = resultTemp.lsLayoff;
            checkLayoff = resultTemp.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 2");
                goto CHECK_AGAIN;
            }

            resultTemp = getMeldKnock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, false);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;

            lsCardLayoff = resultTemp.lsLayoff;
            checkLayoff = resultTemp.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 3");
                goto CHECK_AGAIN;
            }

            DummyDataCustom resultTemp22 = getMeldKnock2(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, true);
            lsAllMeld = resultTemp22.allMeld;
            lsCardFree = resultTemp22.cardLeft;
            lsCardLayoff = resultTemp22.lsLayoff;
            checkLayoff = resultTemp22.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 4");
                goto CHECK_AGAIN;
            }

            DummyDataCustom resultTemp23 = getMeldKnock2(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, false);
            lsAllMeld = resultTemp23.allMeld;
            lsCardFree = resultTemp23.cardLeft;
            lsCardLayoff = resultTemp23.lsLayoff;
            checkLayoff = resultTemp23.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 5");
                goto CHECK_AGAIN;
            }
        }



        var lsCardFree4 = new List<Card>();
        var lsCardLayoff4 = new List<Card>();
        lsCardFree.AddRange(lsCardLayoff);
        var checkLayoff2 = simulateAllLayoff(lsCardFree, ref lsCardFree4, ref lsCardLayoff4); //check cac card gui dc

        lsCardFree = lsCardFree4;
        lsCardLayoff = lsCardLayoff4;
        checkLayoff = checkLayoff2;

        if (lsCardFree.Count <= 1)
        {
            Logging.Log(" goto CHECK_AGAIN 7");
            goto CHECK_AGAIN;
        }

        {
            DummyDataCustom resultTemp = getDropMeldKock(lsAllMeld, lsCardFree, new List<Card>(), new JArray(), true);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            //if (lsCardLayoff.Count == 0)
            //{
            //    lsCardLayoff = resultTemp.lsLayoff;
            //    checkLayoff = resultTemp.allLayoff;
            //}
            //if (lsCardLayoff.Count == 0)
            if (resultTemp.allLayoff.Count != 0)
            {
                //lsCardLayoff = resultTemp.lsLayoff;
                //checkLayoff = resultTemp.allLayoff;
                lsCardLayoff = Add2RangeCustom(lsCardLayoff, resultTemp.lsLayoff);
                checkLayoff = Add2RangeCustom(checkLayoff, resultTemp.allLayoff);
            }
            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 10");
                goto CHECK_AGAIN;
            }

            resultTemp = getDropMeldKock(lsAllMeld, lsCardFree, new List<Card>(), new JArray(), false);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            //if (lsCardLayoff.Count == 0)
            if (resultTemp.allLayoff.Count != 0)
            {
                //lsCardLayoff = resultTemp.lsLayoff;
                //checkLayoff = resultTemp.allLayoff;
                lsCardLayoff = Add2RangeCustom(lsCardLayoff, resultTemp.lsLayoff);
                checkLayoff = Add2RangeCustom(checkLayoff, resultTemp.allLayoff);
            }
            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 11");
                goto CHECK_AGAIN;
            }


            resultTemp = getDropMeldKock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, true);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            lsCardLayoff = resultTemp.lsLayoff;
            checkLayoff = resultTemp.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 12");
                goto CHECK_AGAIN;
            }

            resultTemp = getDropMeldKock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, false);

            lsAllMeld = resultTemp.allMeld;
            lsCardFree = resultTemp.cardLeft;
            lsCardLayoff = resultTemp.lsLayoff;
            checkLayoff = resultTemp.allLayoff;

            if (lsCardFree.Count <= 1 && cuntLoop < 4)
            {
                Logging.Log(" goto CHECK_AGAIN 13");
                goto CHECK_AGAIN;
            }
        }


        DummyDataCustom resultTemp332 = getMeldKnock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, true, true);

        lsAllMeld = resultTemp332.allMeld;
        lsCardFree = resultTemp332.cardLeft;
        lsCardLayoff = resultTemp332.lsLayoff;
        checkLayoff = resultTemp332.allLayoff;

        if (lsCardFree.Count <= 1 && cuntLoop < 4)
        {
            Logging.Log(" goto CHECK_AGAIN 20");
            goto CHECK_AGAIN;
        }

        resultTemp332 = getMeldKnock(lsAllMeld, lsCardFree, lsCardLayoff, checkLayoff, false, true);

        lsAllMeld = resultTemp332.allMeld;
        lsCardFree = resultTemp332.cardLeft;
        lsCardLayoff = resultTemp332.lsLayoff;
        checkLayoff = resultTemp332.allLayoff;

        if (lsCardFree.Count <= 1 && cuntLoop < 4)
        {
            Logging.Log(" goto CHECK_AGAIN 30");
            goto CHECK_AGAIN;
        }

        {
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<Card>();
            var _checkLayoff = simulateAllLayoff(arr, ref _lsCardFree, ref _lsCardLayoff);


            if (_lsCardFree.Count <= 1)
            {
                lsCardFree = _lsCardFree;

                var _lsCardFree2 = new List<Card>();
                var meld1 = findStraightMeld(_lsCardLayoff, ref _lsCardFree2);
                var countFre = _lsCardFree2.Count;
                var _lsCardFree3 = new List<Card>();
                var _lsCardLayoff2 = new List<Card>();
                var _checkLayoff2 = simulateAllLayoff(_lsCardFree2, ref _lsCardFree2, ref _lsCardLayoff2);
                if (_checkLayoff2.Count == countFre)
                {
                    lsAllMeld = meld1;
                    lsCardLayoff = _lsCardLayoff2;
                    checkLayoff = _checkLayoff2;
                }

                Logging.Log(" goto CHECK_AGAIN 31");
                goto CHECK_AGAIN;
            }
        }
        {
            //            ***********allMeld 7tep 8tep 9tep 10tep 11tep 12tep 13tep
            // 11co 12co 13co
            // 3bich 3tep 3co
            // 14ro 14tep 14co
            // 6ro 6co 6tep
            //*********** cardLeft 9ro 13ro 7ro*********** allLayoff[]

            // ***********allMeld 3bich 3tep 3co
            // 6tep 6ro 6co
            // 13tep 13ro 13co
            // 14ro 14tep 14co
            // 7tep 8tep 9tep 10tep 11tep 12tep
            // *********** cardLeft 7ro 9ro*********** allLayoff[
            //  {
            //                "idCardSend": 49,
            //    "idMeld": 1,
            //    "sendtoPlayer": 5283,
            //    "info": "11co "
            //  },
            //  {
            //                "idCardSend": 50,
            //    "idMeld": 1,
            //    "sendtoPlayer": 5283,
            //    "info": "12co "
            //  }
            //]
            var _lsCardFree = new List<Card>();
            var _lsCardLayoff = new List<List<Card>>();

            var _checkLayoff2 = simulateAllLayoff(arr, ref _lsCardFree, ref _lsCardLayoff);

        }
        result.allMeld = lsAllMeld;
        result.allLayoff = checkLayoff;
        result.cardLeft = lsCardFree;

        Logging.Log("simulateKnockOutStraight 7");
        return result;
    }

    List<Card> removeListCardInList(List<List<Card>> _lsAllMeld, List<Card> lsRemove)
    {
        for (var i = 0; i < _lsAllMeld.Count; i++)
        {
            for (var j = 0; j < _lsAllMeld[i].Count; j++)
            {
                var c = lsRemove.FirstOrDefault(x => x.code == _lsAllMeld[i][j].code);
                if (c != null)
                {
                    lsRemove.Remove(c);
                }
            }
        }

        return lsRemove;
    }

    JArray updateArrayWithList(JArray jArray, List<Card> lsCheck)
    {
        JArray jArrayResult = new JArray();
        for (var i = 0; i < lsCheck.Count; i++)
        {
            var cc = jArray.FirstOrDefault(x => (int)x["idCardSend"] == lsCheck[i].code);
            if (cc != null)
            {
                jArrayResult.Add(cc);
            }
        }
        return jArrayResult;
    }

    DummyDataCustom getDropMeldKock(List<List<Card>> _lsAllMeld, List<Card> _lsCardFree, List<Card> _lsCardLayoff, JArray _checkLayoff, bool groupN)
    {
        DummyDataCustom result = new DummyDataCustom();

        //phá phỏm => ù
        var listAllMeldTemp = new List<List<Card>>(_lsAllMeld);
        var listFreeTemp0 = new List<Card>(_lsCardFree);
        var lsCardLayoff0 = new List<Card>(_lsCardLayoff);
        var checkLayoff0 = new JArray(_checkLayoff);
        var listFreeTemp = new List<Card>();
        var listTemp = new List<Card>();

        listTemp = Add2RangeCustom(listTemp, _lsCardFree);
        listTemp = Add2RangeCustom(listTemp, _lsCardLayoff);

        List<List<Card>> grroupCard = new List<List<Card>>();

        var cardGroup = groupN ? listTemp.GroupBy(c => c.N).ToList() : listTemp.GroupBy(c => c.S).ToList();
        foreach (var group in cardGroup)
        {
            grroupCard.Add(group.ToList());
        }
        for (var j = 0; j < grroupCard.Count; j++)
        {
            for (var i = 0; i < listAllMeldTemp.Count; i++)
            {
                var melTemm = listAllMeldTemp[i];
                bool isHas = false;
                for (var l = 0; l < melTemm.Count; l++)
                {
                    grroupCard[j].Add(melTemm[l]);
                    if (checkValidMeld(grroupCard[j]))
                    {
                        melTemm.RemoveAt(l);
                        listAllMeldTemp.Add(grroupCard[j]);
                        if (!checkValidMeld(melTemm))
                        {
                            listAllMeldTemp.RemoveAt(i);
                            listFreeTemp = Add2RangeCustom(listFreeTemp, melTemm);
                        }

                        for (var k = 0; k < grroupCard[j].Count; k++)
                        {
                            var cc = listFreeTemp0.FirstOrDefault(x => x.code == grroupCard[j][k].code);
                            if (cc != null)
                            {
                                listFreeTemp0.Remove(cc);
                            }
                            var cc2 = lsCardLayoff0.FirstOrDefault(x => x.code == grroupCard[j][k].code);
                            if (cc2 != null)
                            {
                                lsCardLayoff0.Remove(cc2);
                            }
                            var cc3 = checkLayoff0.FirstOrDefault(x => (int)x["idCardSend"] == grroupCard[j][k].code);
                            if (cc3 != null)
                            {
                                checkLayoff0.Remove(cc3);
                            }
                        }
                        grroupCard.RemoveAt(j);
                        j = -1;
                        isHas = true;
                        break;
                    }
                    else
                    {
                        grroupCard[j].Remove(melTemm[l]);
                    }
                }
                if (isHas)
                {

                    break;
                }
            }
        }
        for (var j = 0; j < listFreeTemp.Count; j++)
        {
            for (var i = 0; i < listAllMeldTemp.Count; i++)
            {
                var melTemm = listAllMeldTemp[i];
                melTemm.Add(listFreeTemp[j]);
                if (checkValidMeld(melTemm))
                {
                    var cc = listFreeTemp0.FirstOrDefault(x => x.code == listFreeTemp[j].code);
                    if (cc != null)
                    {
                        listFreeTemp0.Remove(cc);
                    }
                    var cc2 = lsCardLayoff0.FirstOrDefault(x => x.code == listFreeTemp[j].code);
                    if (cc2 != null)
                    {
                        lsCardLayoff0.Remove(cc2);
                    }
                    var cc3 = checkLayoff0.FirstOrDefault(x => (int)x["idCardSend"] == listFreeTemp[j].code);
                    if (cc3 != null)
                    {
                        checkLayoff0.Remove(cc3);
                    }
                    listFreeTemp.RemoveAt(j);
                    j--;
                    break;
                }
                else
                {
                    melTemm.Remove(listFreeTemp[j]);
                }
            }
        }

        listFreeTemp = Add2RangeCustom(listFreeTemp, listFreeTemp0);

        result.allMeld = listAllMeldTemp;
        result.cardLeft = listFreeTemp;
        result.lsLayoff = lsCardLayoff0;
        result.allLayoff = checkLayoff0;

        return result;
    }

    DummyDataCustom getMeldKnock(List<List<Card>> _lsAllMeld, List<Card> _lsCardFree, List<Card> _lsCardLayoff, JArray _checkLayoff, bool isRevese, bool isCheckStraigh = false)
    {
        List<List<Card>> lsAllMeld = new List<List<Card>>(_lsAllMeld);
        List<Card> lsCardFree = new List<Card>(_lsCardFree);
        List<Card> lsCardLayoff = new List<Card>(_lsCardLayoff);
        JArray checkLayoff = new JArray(_checkLayoff);

        DummyDataCustom result = new DummyDataCustom();
        var listTemp = new List<Card>();
        listTemp = Add2RangeCustom(listTemp, lsCardFree);
        listTemp = Add2RangeCustom(listTemp, lsCardLayoff);

        var listTemp2 = new List<Card>();
        for (var i = 0; i < listTemp.Count; i++)
        {
            for (var j = 0; j < lsAllMeld.Count; j++)
            {
                //if (lsAllMeld[j].Count <= 3) continue;
                var ls = lsAllMeld[j].FirstOrDefault(x => x.N == listTemp[i].N);
                if (ls != null)
                {
                    listTemp2 = Add2RangeCustom(listTemp2, lsAllMeld[j]);
                }
            }
        }

        List<List<Card>> lsMeldBonus = new List<List<Card>>();
        if (listTemp2.Count <= 0)
        {
            lsCardFree.Clear();
            lsCardLayoff.Clear();
            lsMeldBonus = findSameMeld(listTemp, ref lsCardFree);
            if (lsMeldBonus.Count > 0)
            {
                lsAllMeld.AddRange(lsMeldBonus);
            }
            var lsCardFree222 = new List<Card>();
            checkLayoff = simulateAllLayoff(lsCardFree, ref lsCardFree222, ref lsCardLayoff);

            result.allMeld = lsAllMeld;
            result.cardLeft = lsCardFree222;
            result.allLayoff = checkLayoff;
            result.lsLayoff = lsCardLayoff;

            return result;
        }


        listTemp = Add2RangeCustom(listTemp, listTemp2);
        var listFreeTemp = new List<Card>();
        lsMeldBonus = findSameMeld(listTemp, ref listFreeTemp);
        if (isCheckStraigh)
        {
            var listFreeTemp22 = new List<Card>();
            var lsMeldBonus2 = findStraightMeld(listFreeTemp, ref listFreeTemp22);
            lsMeldBonus.AddRange(lsMeldBonus2);
            listFreeTemp = listFreeTemp22;
        }

        var lsCardFree223 = new List<Card>();
        var lsCardLayoff223 = new List<Card>();
        var checkLayoff223 = simulateAllLayoff(listFreeTemp, ref lsCardFree223, ref lsCardLayoff223);

        if (lsCardFree223.Count <= 1)
        {
            for (var i = 0; i < lsMeldBonus.Count; i++)
            {
                if (!checkSameCard(lsMeldBonus[i])) continue;
                for (var j = 0; j < lsAllMeld.Count; j++)
                {
                    var cc = lsAllMeld[j].FirstOrDefault(x => x.N == lsMeldBonus[i][0].N);
                    if (cc != null)
                    {
                        lsAllMeld.RemoveAt(j);
                        break;
                    }
                }
            }

            lsAllMeld.AddRange(lsMeldBonus);
            result.allMeld = lsAllMeld;
            result.cardLeft = lsCardFree223;
            result.lsLayoff = removeListCardInList(lsAllMeld, lsCardLayoff223);
            result.allLayoff = updateArrayWithList(checkLayoff223, result.lsLayoff);
            return result;
        }

        for (var i = (isRevese ? 0 : lsMeldBonus.Count - 1); (isRevese ? i < lsMeldBonus.Count : i >= 0); i += (isRevese ? 1 : -1))
        {
            var lssss = lsMeldBonus[i];
            bool isHas = false;
            for (var j = 0; j < lssss.Count; j++)
            {
                var lssJ = lssss[j];
                if (lsCardFree.FirstOrDefault(x => x.code == lssJ.code) != null)
                {
                    isHas = true;
                    break;
                }
            }
            if (!isHas) continue;

            Dictionary<int, List<List<Card>>> dicBackup = new Dictionary<int, List<List<Card>>>();
            Dictionary<int, List<Card>> dicCardBackup = new Dictionary<int, List<Card>>();
            Dictionary<int, List<Card>> dicCardCanLayoffBackup = new Dictionary<int, List<Card>>();
            Dictionary<int, JArray> dicJArrayCanLayoffBackup = new Dictionary<int, JArray>();
            for (var j = 0; j < lssss.Count; j++)
            {
                var lssJ = lssss[j];
                if (lsCardFree.FirstOrDefault(x => x.code == lssJ.code) != null)
                {
                    continue;
                }

                for (var k = 0; k < lsAllMeld.Count; k++)
                {
                    //if (lsAllMeld[k].Count <= 3) continue;
                    var ls = lsAllMeld[k].FirstOrDefault(x => x.code == lssJ.code);

                    if (ls != null)
                    {
                        lsAllMeld[k].Remove(ls);
                        if (dicCardBackup.ContainsKey(k))
                        {
                            if (!dicCardBackup[k].Contains(ls))
                            {
                                dicCardBackup[k].Add(ls);
                            }
                        }
                        else
                        {
                            dicCardBackup.Add(k, new List<Card>() { ls });
                        }

                        if (!checkValidMeld(lsAllMeld[k]))
                        {
                            var lsTemo = new List<Card>();
                            var newMel = findStraightMeld(lsAllMeld[k], ref lsTemo);
                            if (lsTemo.Count == 0)
                            {
                                dicBackup.Add(k, newMel);
                            }
                            else
                            {
                                var lsCardFree333 = new List<Card>();
                                var lsCardLayoff333 = new List<Card>();
                                var checkLayoff333 = simulateAllLayoff(lsAllMeld[k], ref lsCardFree333, ref lsCardLayoff333);
                                if (lsCardFree333.Count == 0)
                                {
                                    if (!dicCardCanLayoffBackup.ContainsKey(k))
                                    {
                                        dicCardCanLayoffBackup.Add(k, lsAllMeld[k]);
                                        dicJArrayCanLayoffBackup.Add(k, checkLayoff333);
                                    }
                                    else
                                    {
                                        dicCardCanLayoffBackup[k] = Add2RangeCustom(dicCardCanLayoffBackup[k], lsAllMeld[k]);
                                        dicJArrayCanLayoffBackup[k] = Add2RangeCustom(dicJArrayCanLayoffBackup[k], checkLayoff333);
                                    }
                                }
                                else
                                {
                                    dicCardBackup.Remove(k);
                                    lsAllMeld[k].Add(ls);
                                    lsAllMeld[k].Sort((x, y) =>
                                    {
                                        if (x.N != y.N) return x.N - y.N;
                                        else return x.S - y.S;
                                    });
                                    lssss.Remove(lssJ);
                                    j--;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }

            if (checkValidMeld(lssss) && dicCardBackup.Count > 0)
            {
                foreach (var gg in dicBackup)
                {
                    if (gg.Value.Count > 1)
                    {
                        lsAllMeld[gg.Key] = gg.Value[0];
                        for (var ijij = 1; ijij < gg.Value.Count; ijij++)
                        {
                            lsAllMeld.Add(gg.Value[ijij]);
                        }
                    }
                }
                foreach (var gg in dicCardCanLayoffBackup)
                {
                    lsCardLayoff = Add2RangeCustom(lsCardLayoff, gg.Value);
                    for (var iio = 0; iio < lsCardLayoff.Count; iio++)
                    {
                        var mel = lsAllMeld.FirstOrDefault(x => x.FirstOrDefault(y => y.code == lsCardLayoff[iio].code));
                        if (mel != null)
                        {
                            var c = mel.FirstOrDefault(x => x.code == lsCardLayoff[iio].code);
                            if (c != null)
                            {
                                mel.Remove(c);
                            }
                        }
                    }

                }
                foreach (var gg in dicJArrayCanLayoffBackup)
                {
                    checkLayoff = Add2RangeCustom(checkLayoff, gg.Value);
                }
                for (var j = 0; j < lssss.Count; j++)
                {
                    var cc = lsCardFree.FirstOrDefault(x => x.code == lssss[j].code);
                    if (cc != null)
                    {
                        lsCardFree.Remove(cc);
                    }

                    var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lssss[j].code);
                    if (cc2 != null)
                    {
                        lsCardLayoff.Remove(cc2);
                    }
                    var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lssss[j].code);
                    if (cc3 != null)
                    {
                        checkLayoff.Remove(cc3);
                    }
                }

                lsAllMeld.Add(lssss);
                lsMeldBonus.Remove(lssss);
                i = (isRevese ? -1 : lsMeldBonus.Count);
            }
            else
            {
                foreach (var gg in dicCardBackup)
                {
                    lsAllMeld[gg.Key] = Add2RangeCustom(lsAllMeld[gg.Key], gg.Value);
                    lsAllMeld[gg.Key].Sort((x, y) =>
                    {
                        if (x.N != y.N) return x.N - y.N;
                        else return x.S - y.S;
                    });
                }
            }
        }

        var lsCardLayoff22 = new List<Card>();
        var lsCardFree22 = new List<Card>();
        var checkLayoffTep = simulateAllLayoff(lsCardLayoff, ref lsCardFree22, ref lsCardLayoff22); //check cac card gui dc

        for (var i = 0; i < lsCardFree22.Count; i++)
        {
            if (lsCardFree.FirstOrDefault(x => x.code == lsCardFree22[i].code) == null)
            {
                var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lsCardFree22[i].code);
                if (cc2 != null)
                {
                    lsCardLayoff.Remove(cc2);
                }
                var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lsCardFree22[i].code);
                if (cc3 != null)
                {
                    checkLayoff.Remove(cc3);
                }
                lsCardFree.Add(lsCardFree22[i]);
            }
        }

        for (var i = 0; i < lsAllMeld.Count; i++)
        {
            if (lsAllMeld[i].Count <= 0)
            {
                lsAllMeld.RemoveAt(i);
                i--;
            }
        }

        result.allMeld = lsAllMeld;
        result.cardLeft = lsCardFree;
        result.allLayoff = checkLayoff;
        result.lsLayoff = lsCardLayoff;


        return result;
    }

    DummyDataCustom getMeldKnock2(List<List<Card>> _lsAllMeld, List<Card> _lsCardFree, List<Card> _lsCardLayoff, JArray _checkLayoff, bool isRevese)
    {
        List<List<Card>> lsAllMeld = new List<List<Card>>(_lsAllMeld);
        List<Card> lsCardFree = new List<Card>(_lsCardFree);
        List<Card> lsCardLayoff = new List<Card>(_lsCardLayoff);
        JArray checkLayoff = new JArray(_checkLayoff);
        DummyDataCustom result = new DummyDataCustom();


        var listTemp = new List<Card>();

        listTemp = Add2RangeCustom(listTemp, lsCardFree);
        listTemp = Add2RangeCustom(listTemp, lsCardLayoff);

        var listTemp2 = new List<Card>();
        for (var i = 0; i < listTemp.Count; i++)
        {
            for (var j = 0; j < lsAllMeld.Count; j++)
            {
                if (lsAllMeld[j].Count <= 3) continue;
                var ls = lsAllMeld[j].FirstOrDefault(x => x.N == listTemp[i].N);
                if (ls != null)
                {
                    listTemp2 = Add2RangeCustom(listTemp2, lsAllMeld[j]);
                }
            }
        }

        List<List<Card>> lsMeldBonus = new List<List<Card>>();
        if (listTemp2.Count <= 0)
        {
            lsCardFree.Clear();
            lsCardLayoff.Clear();
            lsMeldBonus = findSameMeld(listTemp, ref lsCardFree);
            if (lsMeldBonus.Count > 0)
            {
                lsAllMeld.AddRange(lsMeldBonus);
            }
            var lsCardFree222 = new List<Card>();
            checkLayoff = simulateAllLayoff(lsCardFree, ref lsCardFree222, ref lsCardLayoff);

            result.allMeld = lsAllMeld;
            result.cardLeft = lsCardFree222;
            result.allLayoff = checkLayoff;
            result.lsLayoff = lsCardLayoff;

            return result;
        }

        //listTemp.AddRange(listTemp2);
        listTemp = Add2RangeCustom(listTemp, listTemp2);

        var listFreeTemp = new List<Card>();
        lsMeldBonus = findSameMeld(listTemp, ref listFreeTemp);
        var lsCardFree223 = new List<Card>();
        var lsCardLayoff223 = new List<Card>();
        var checkLayoff223 = simulateAllLayoff(listFreeTemp, ref lsCardFree223, ref lsCardLayoff223);

        if (lsCardFree223.Count <= 1)
        {
            for (var i = 0; i < lsMeldBonus.Count; i++)
            {
                for (var j = 0; j < lsAllMeld.Count; j++)
                {
                    var cc = lsAllMeld[j].FirstOrDefault(x => x.N == lsMeldBonus[i][0].N);
                    if (cc != null)
                    {
                        lsAllMeld.RemoveAt(j);
                        break;
                    }
                }
            }
            lsAllMeld.AddRange(lsMeldBonus);
            result.allMeld = lsAllMeld;
            result.cardLeft = lsCardFree223;

            result.lsLayoff = removeListCardInList(lsAllMeld, lsCardLayoff223);
            result.allLayoff = updateArrayWithList(checkLayoff223, result.lsLayoff);
            //result.allLayoff = checkLayoff223;
            //result.lsLayoff = lsCardLayoff223;
            return result;
        }

        var lsCardFreeTemp = new List<Card>();

        for (var i = (isRevese ? 0 : lsMeldBonus.Count - 1); (isRevese ? i < lsMeldBonus.Count : i >= 0); i += (isRevese ? 1 : -1))
        {
            var lssss = lsMeldBonus[i];
            bool isHas = false;
            for (var j = 0; j < lssss.Count; j++)
            {
                var lssJ = lssss[j];
                if (lsCardFree.FirstOrDefault(x => x.code == lssJ.code) != null)
                {
                    isHas = true;
                    break;
                }
            }
            if (!isHas) continue;

            Dictionary<int, List<List<Card>>> dicBackup = new Dictionary<int, List<List<Card>>>();
            Dictionary<int, List<Card>> dicCardBackup = new Dictionary<int, List<Card>>();
            for (var j = 0; j < lssss.Count; j++)
            {
                var lssJ = lssss[j];
                if (lsCardFree.FirstOrDefault(x => x.code == lssJ.code) != null)
                {
                    continue;
                }

                for (var k = 0; k < lsAllMeld.Count; k++)
                {
                    //if (lsAllMeld[k].Count <= 3) continue;
                    var ls = lsAllMeld[k].FirstOrDefault(x => x.code == lssJ.code);

                    if (ls != null)
                    {
                        lsAllMeld[k].Remove(ls);
                        if (dicCardBackup.ContainsKey(k))
                        {
                            if (!dicCardBackup[k].Contains(ls))
                            {
                                dicCardBackup[k].Add(ls);
                            }
                        }
                        else
                        {
                            dicCardBackup.Add(k, new List<Card>() { ls });
                        }

                        if (!checkValidMeld(lsAllMeld[k]))
                        {
                            var lsTemo = new List<Card>();
                            var newMel = findStraightMeld(lsAllMeld[k], ref lsTemo);
                            if (lsTemo.Count == 0)
                            {
                                dicBackup.Add(k, newMel);
                            }
                            else
                            {
                                for (var ll = 0; ll < lsTemo.Count; ll++)
                                {
                                    var cccc2 = lsCardFreeTemp.FirstOrDefault(x => x.code == lsTemo[ll].code);
                                    if (cccc2 == null)
                                    {
                                        lsCardFreeTemp.Add(lsTemo[ll]);
                                    }
                                    var cccc = lsAllMeld[k].FirstOrDefault(x => x.code == lsTemo[ll].code);
                                    if (cccc != null)
                                    {
                                        lsAllMeld[k].Remove(cccc);
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }

            if (checkValidMeld(lssss) && dicCardBackup.Count > 0)
            {
                foreach (var gg in dicBackup)
                {
                    if (gg.Value.Count > 1)
                    {
                        lsAllMeld[gg.Key] = gg.Value[0];
                        for (var ijij = 1; ijij < gg.Value.Count; ijij++)
                        {
                            lsAllMeld.Add(gg.Value[ijij]);
                        }
                    }
                }
                for (var j = 0; j < lssss.Count; j++)
                {
                    var cc = lsCardFree.FirstOrDefault(x => x.code == lssss[j].code);
                    if (cc != null)
                    {
                        lsCardFree.Remove(cc);
                    }
                    var cc1 = lsCardFreeTemp.FirstOrDefault(x => x.code == lssss[j].code);
                    if (cc1 != null)
                    {
                        lsCardFreeTemp.Remove(cc1);
                    }
                    var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lssss[j].code);
                    if (cc2 != null)
                    {
                        lsCardLayoff.Remove(cc2);
                    }
                    var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lssss[j].code);
                    if (cc3 != null)
                    {
                        checkLayoff.Remove(cc3);
                    }
                }
                lsAllMeld.Add(lssss);
                lsMeldBonus.Remove(lssss);
                i = (isRevese ? -1 : lsMeldBonus.Count);
            }
            else
            {
                foreach (var gg in dicCardBackup)
                {
                    //gg.Key
                    //lsAllMeld[gg.Key].AddRange(gg.Value);
                    lsAllMeld[gg.Key] = Add2RangeCustom(lsAllMeld[gg.Key], gg.Value);
                    lsAllMeld[gg.Key].Sort((x, y) =>
                    {
                        if (x.N != y.N) return x.N - y.N;
                        else return x.S - y.S;
                    });
                }
            }
        }

        var lsCardLayoff22 = new List<Card>();
        var lsCardFree22 = new List<Card>();
        var checkLayoffTep = simulateAllLayoff(lsCardLayoff, ref lsCardFree22, ref lsCardLayoff22); //check cac card gui dc
        for (var i = 0; i < lsCardFree22.Count; i++)
        {
            if (lsCardFree.FirstOrDefault(x => x.code == lsCardFree22[i].code) == null)
            {
                var cc2 = lsCardLayoff.FirstOrDefault(x => x.code == lsCardFree22[i].code);
                if (cc2 != null)
                {
                    lsCardLayoff.Remove(cc2);
                }
                var cc3 = checkLayoff.FirstOrDefault(x => (int)x["idCardSend"] == lsCardFree22[i].code);
                if (cc3 != null)
                {
                    checkLayoff.Remove(cc3);
                }

                lsCardFree.Add(lsCardFree22[i]);
            }
        }
        for (var i = 0; i < lsAllMeld.Count; i++)
        {
            if (lsAllMeld[i].Count <= 0)
            {
                lsAllMeld.RemoveAt(i);
                i--;
            }
        }
        lsCardFree.AddRange(lsCardFreeTemp);

        result.allMeld = lsAllMeld;
        result.cardLeft = lsCardFree;
        result.allLayoff = checkLayoff;
        result.lsLayoff = lsCardLayoff;


        return result;
    }

    DummyDataCustom simulateKnockOut()
    {
        bool isKnock = false;
        DummyDataCustom result = simulateKnockOutSame(ref isKnock);
        Logging.Log("simulateKnockOut  Same " + isKnock);
        result.LogToString();
        if (!isKnock)
        {
            DummyDataCustom result2 = simulateKnockOutStraight(ref isKnock);
            Logging.Log("simulateKnockOut  Straight " + isKnock);
            result2.LogToString();
            if (isKnock)
            {
                return result2;
            }
            else if (result.cardLeft.Count > result2.cardLeft.Count)
            {
                return result2;
            }
        }

        return result;
    }


    //DummyDataCustom simulateKnockOut()
    //{
    //    var checkLayoff = this.simulateAllLayoff(this.thisPlayer.vectorCard); //check cac card gui dc
    //    var checkMeld = this.findPossibleMeld(this.thisPlayer.vectorCard); //cac phom tren tay

    //    //let allMeld = [...checkMeld.meldSame, ...checkMeld.meldStraight];
    //    var allMeld = new List<List<Card>>();
    //    allMeld.AddRange(checkMeld["meldSame"]);
    //    allMeld.AddRange(checkMeld["meldStraight"]);

    //    //loai bo truong hop phom chung card
    //    var checkOverLap = true;
    //    while (checkOverLap)
    //    {
    //        var temp = new List<List<Card>>();
    //        var countOver = 0;
    //        //for (let cardTemp of this.thisPlayer.vectorCard)
    //        for (var i = 0; i < this.thisPlayer.vectorCard.Count; i++)
    //        {
    //            var cardTemp = this.thisPlayer.vectorCard[i];
    //            var overLap = this.findOverLapArray(cardTemp, allMeld);
    //            if (overLap.Count > 1)
    //            {
    //                overLap.ForEach(element =>
    //                {
    //                    allMeld.RemoveAt(allMeld.IndexOf(element));

    //                });
    //                var reduce = this.resolveOverLapMeld(overLap, allMeld, checkLayoff.allLayoff);

    //                //cc.NGWlog('!=> after reduce', reduce)

    //                reduce.ForEach(array =>
    //                {
    //                    temp.Add(array);
    //                });
    //                countOver++;
    //            }
    //        }
    //        checkOverLap = countOver != 0;
    //        allMeld.InsertRange(0, temp);
    //        //allMeld = [...temp, ...allMeld];
    //    }
    //    var cardfree = new List<Card>();
    //    this.thisPlayer.vectorCard.ForEach(cardTemp =>
    //    {
    //        if (this.checkCardSingle(cardTemp, allMeld, new JArray())) cardfree.Add(cardTemp);

    //    });
    //    var checkMeldFree = this.findPossibleMeld(cardfree); //cac phom tren tay
    //                                                         //allMeld = [...allMeld, ...checkMeldFree.meldStraight, ...checkMeldFree.meldSame];

    //    allMeld.AddRange(checkMeld["meldStraight"]);
    //    allMeld.AddRange(checkMeld["meldSame"]);
    //    this.thisPlayer.vectorCard.ForEach(cardTemp =>
    //    {
    //        if (this.checkCardSingle(cardTemp, allMeld, new JArray())) cardfree.Add(cardTemp);

    //    });
    //    cardfree.ForEach(cardTemp =>
    //    {
    //        //for (let array of allMeld)
    //        for (var i = 0; i < allMeld.Count; i++)
    //        {
    //            var array = allMeld[i];
    //            var test = new List<Card>(allMeld[i]);
    //            test.Add(cardTemp);
    //            if (this.checkValidMeld(test))
    //            {
    //                array.Add(cardTemp);
    //            }
    //        }
    //    });
    //    //tim cac la gui dc
    //    var listData = checkLayoff.allLayoff;

    //    var allLayoff = new JArray();
    //    var listResolve = new List<int>();
    //    for (var i = 0; i < listData.Count; i++)
    //    {
    //        if (listResolve.Find(obj => obj == (int)listData[i]["idCardSend"]) != -1) continue;
    //        listResolve.Add((int)listData[i]["idCardSend"]);
    //        var overLap = this.findOverLapLayoff((JObject)listData[i], listData);
    //        if (overLap.Count > 1)
    //        {
    //            allLayoff.Add(this.resolveOverLapLayoff(overLap, listData));
    //        }
    //        else
    //        {
    //            allLayoff.Add(listData[i]);
    //        }
    //    }
    //    //check la gui di co lam mat phom knock k
    //    var listSplice = new JArray();
    //    //allLayoff.ForEach(data => {
    //    for (var j = 0; j < allLayoff.Count; j++)
    //    {
    //        var data = allLayoff[j];
    //        //for (let array of allMeld)
    //        for (var i = 0; i < allMeld.Count; i++)
    //        {
    //            var array = allMeld[i];
    //            var test = new List<Card>(array);
    //            var card = test.Find(cardTemp => cardTemp.code == (int)data["idCardSend"]);

    //            if (card != null)
    //            {
    //                array.RemoveAt(test.IndexOf(card));
    //                if (this.checkValidMeld(test))
    //                {
    //                    array.RemoveAt(array.IndexOf(card));

    //                }
    //                else
    //                {
    //                    //allLayoff.forEach(dataSub => {
    //                    foreach (var dataSub in allLayoff)
    //                    {
    //                        if ((int)dataSub["idMeld"] == (int)data["idMeld"] && dataSub["sendtoPlayer"] == data["sendtoPlayer"])
    //                        {
    //                            if (listSplice.IndexOf(dataSub) < 0) listSplice.Add(dataSub);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        //});
    //    }

    //    //listSplice.forEach(data => {
    //    foreach (var data in listSplice)
    //    {
    //        allLayoff.RemoveAt(allLayoff.IndexOf(data));
    //    }
    //    //lay log
    //    //let listLog = [];
    //    //allMeld.forEach(array => {
    //    //    listLog.push(array.map(card => card.nameCard))

    //    //});
    //    // tim card k trong phom va k gui dc
    //    var cardLeft = new List<Card>();
    //    this.thisPlayer.vectorCard.ForEach(cardTemp =>
    //    {
    //        if (this.checkCardSingle(cardTemp, allMeld, allLayoff)) cardLeft.Add(cardTemp);

    //    });
    //    //let lastTest = [...cardLeft];
    //    var lastTest = new List<Card>(cardLeft);
    //    var safeLock = this.simulateAllLayoff(lastTest); //tim cac la gui dc
    //    cardLeft = safeLock.cardLeft;
    //    //allLayoff = [...allLayoff, ...safeLock.dataLayoff]
    //    allLayoff = Add2RangeCustom(allLayoff, safeLock.allLayoff);

    //    //loai bo trung lap
    //    //allLayoff.ForEach(data_i => {
    //    foreach (var data_i in allLayoff)
    //    {
    //        //allLayoff.forEach(data_j => {
    //        foreach (var data_j in allLayoff)
    //        {
    //            if (data_i["idCardSend"] == data_j["idCardSend"] && allLayoff.IndexOf(data_i) != allLayoff.IndexOf(data_j))
    //            {
    //                var length = allLayoff.Count;
    //                var i = allLayoff.IndexOf(data_i);
    //                var data_i_plus = i < length - 1 ? allLayoff[i + 1] : allLayoff[i - 1];
    //                var data_i_sub = i < 1 ? allLayoff[i + 1] : allLayoff[i - 1];
    //                if (data_i_plus["idMeld"] == data_i["idMeld"] || data_i_sub["idMeld"] == data_i["idMeld"])
    //                    allLayoff.RemoveAt(allLayoff.IndexOf(data_j));
    //                else allLayoff.RemoveAt(allLayoff.IndexOf(data_i));
    //            }
    //        }
    //    }
    //    //cc.NGWlog('!=>>>>> simulate knockout meld ', listLog, '\nlayoff ', allLayoff, '\ncard left ', cardLeft);
    //    //cc.NGWlog('!=>>>>> simulate knockout allMeld ', allMeld);
    //    cardLeft.ForEach(cardTemp =>
    //    {
    //        foreach (var array in allMeld)
    //        {
    //            var test = new List<Card>(array);
    //            test.Add(cardTemp);
    //            if (this.checkValidMeld(test))
    //            {
    //                array.Add(cardTemp);
    //                cardLeft.RemoveAt(cardLeft.IndexOf(cardTemp));
    //            }
    //        }
    //    });
    //    //allLayoff.forEach(cardTemp =>
    //    foreach (var cardTemp in allLayoff)
    //    {
    //        foreach (var array in allMeld)
    //        {
    //            //let test = [...array];

    //            var test = new List<Card>(array);
    //            test.Add(cardTemp);
    //            if (this.checkValidMeld(test))
    //            {
    //                array.Add(cardTemp);
    //                allLayoff.RemoveAt(allLayoff.IndexOf(cardTemp));
    //            }
    //        }
    //    }
    //    //tach phom thanh la le de u k
    //    DummyDataCustom result = new DummyDataCustom();
    //    result.allMeld = allMeld;
    //    result.allLayoff = allLayoff;
    //    result.cardLeft = cardLeft;
    //    //          = {
    //    //          allMeld: allMeld,
    //    //	allLayoff: allLayoff,
    //    //	cardLeft: cardLeft,
    //    //}
    //    //tach 1 card trong phom daithanh la le de knock dc
    //    if (cardLeft.Count == 0 && allLayoff.Count == 0)
    //    {
    //        for (var i = 0; i < allMeld.Count; i++)
    //        {
    //            if (allLayoff.Count == 1) break;
    //            var array = allMeld[i];
    //            if (array.Count > 3)
    //            {
    //                for (var j = 0; j < array.Count; j++)
    //                {
    //                    var card = array[j];
    //                    var test = new List<Card>(array);
    //                    test.RemoveAt(test.IndexOf(card));
    //                    if (this.checkValidMeld(test))
    //                    {
    //                        array.RemoveAt(array.IndexOf(card));
    //                        result.cardLeft.Add(card);
    //                        return result;
    //                    }
    //                }

    //            }
    //        }
    //    }
    //    //pha phom xem u dc k
    //    if (cardLeft.Count == 0 && allLayoff.Count == 0)
    //    {
    //        for (var i = 0; i < allMeld.Count; i++)
    //        {
    //            var array = allMeld[i];
    //            var checkBreakMeld = this.simulateAllLayoff(array);
    //            if (checkBreakMeld.cardLeft.Count > 1) continue;
    //            allMeld.RemoveAt(allMeld.IndexOf(array));
    //            allLayoff = Add2RangeCustom(allLayoff, checkBreakMeld.allLayoff);
    //            //allLayoff = [...allLayoff, ...checkBreakMeld.dataLayoff];
    //            //cardLeft = [...cardLeft, ...checkBreakMeld.cardLeft];
    //            cardLeft = Add2RangeCustom(cardLeft, checkBreakMeld.cardLeft);
    //            break;
    //        }
    //    }
    //    result.allMeld = allMeld;
    //    result.allLayoff = allLayoff;
    //    result.cardLeft = cardLeft;
    //    return result;
    //}

    bool isCompareList(List<Card> listA, List<Card> listB)
    {
        if (listA.Count != listB.Count) return false;
        bool isEqua = true;
        for (var i = 0; i < listA.Count; i++)
        {
            var cc = listB.FirstOrDefault(x => x.code == listA[i].code);
            if (cc == null)
            {
                isEqua = false;
                break;
            }
        }


        return isEqua;
    }

    void listCardToLayoff() { }

    DummyDataCustom simulateKnockOutBacup()
    {
        DummyDataCustom result = new DummyDataCustom();
        List<Card> arr = new List<Card>();
        arr.AddRange(thisPlayer.vectorCard);
        arr.Sort((x, y) =>
        {
            if (x.N != y.N) return x.N - y.N;
            else return x.S - y.S;
        });
        var strLog = "";
        for (var i = 0; i < arr.Count; i++)
        {
            strLog += arr[i].LogInfo();
        }

        Logging.Log(strLog);

        // chưa tối ưu cái layoff

        var lsCardFree = new List<Card>();
        var lsCardFree2 = new List<Card>();

        var lsMeldSame = findSameMeld(arr, ref lsCardFree2);
        var lsMeldStraight = findStraightMeld(lsCardFree2, ref lsCardFree);


    CHECK_AGAIN:
        ////u cmnr
        if (lsCardFree.Count == 0)
        {
            var lsCardFree3 = new List<Card>();
            var lsCardLayoff3 = new List<Card>();
            var checkAllLayoff = simulateAllLayoff(arr, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
            for (var i = 0; i < lsMeldSame.Count; i++)
            {
                if (lsMeldSame[i].Count <= 3) continue;
                for (var j = 0; j < lsMeldSame[i].Count; j++)
                {
                    var ll = checkAllLayoff.FirstOrDefault(c => (int)c["idCardSend"] == lsMeldSame[i][j].code);
                    if (ll != null)
                    {
                        result.allLayoff.Add(ll);
                        lsMeldSame[i].RemoveAt(j);
                        result.allMeld = lsMeldSame;
                        return result;
                    }
                }
            }

            for (var i = 0; i < lsMeldSame.Count; i++)
            {
                if (lsMeldSame[i].Count <= 3) continue;
                result.cardLeft.Add(lsMeldSame[i][0]);
                lsMeldSame[i].RemoveAt(0);
                result.allMeld = lsMeldSame;
                return result;
            }
        }
        else if (lsCardFree.Count == 1)
        {
            var lsCardFree3 = new List<Card>();
            var lsCardLayoff3 = new List<Card>();
            result.allMeld = lsMeldSame;
            var checkLayoff = simulateAllLayoff(lsCardFree2, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
            if (checkLayoff.Count > 0)
            {
                result.allLayoff = checkLayoff;
            }
            else
            {
                result.cardLeft = lsCardFree2;
            }
            return result;
        }

    CHECK_AGAIN2:

        lsCardFree2.Clear();
        lsCardFree.Clear();

        lsMeldStraight = findStraightMeld(arr, ref lsCardFree2);
        lsMeldSame = findSameMeld(lsCardFree2, ref lsCardFree);

        if (lsMeldSame.Count == 0 && lsCardFree.Count >= 2)
        {
            var llofff = simulateAllLayoff(lsCardFree);
            if (llofff.allLayoff.Count == lsCardFree.Count)
            {
                result.allLayoff = llofff.allLayoff;
                result.allMeld = lsMeldStraight;
                return result;
            }
        }
        if (lsCardFree.Count == 0)
        {
            var lsCardFree3 = new List<Card>();
            var lsCardLayoff3 = new List<Card>();
            var checkAllLayoff = simulateAllLayoff(arr, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
            for (var i = 0; i < lsMeldStraight.Count; i++)
            {
                if (lsMeldStraight[i].Count <= 3) continue;
                for (var j = 0; j < lsMeldStraight[i].Count; j++)
                {
                    var ll = checkAllLayoff.FirstOrDefault(c => (int)c["idCardSend"] == lsMeldStraight[i][j].code);
                    if (ll != null)
                    {
                        result.allLayoff.Add(ll);
                        lsMeldSame[i].RemoveAt(j);
                        result.allMeld = lsMeldSame;
                        return result;
                    }
                }
            }

            for (var i = 0; i < lsMeldStraight.Count; i++)
            {
                if (lsMeldStraight[i].Count <= 3) continue;
                result.cardLeft.Add(lsMeldStraight[i][0]);
                lsMeldStraight[i].RemoveAt(0);
                result.allMeld = lsMeldStraight;
                return result;
            }
        }
        else if (lsCardFree.Count == 1)
        {
            var lsCardFree3 = new List<Card>();
            var lsCardLayoff3 = new List<Card>();
            result.allMeld = lsMeldStraight;
            var checkLayoff = simulateAllLayoff(lsCardFree, ref lsCardFree3, ref lsCardLayoff3); //check cac card gui dc
            if (checkLayoff.Count > 0)
            {
                result.allLayoff = checkLayoff;
            }
            else
            {
                result.cardLeft = lsCardFree;
            }
            return result;
        }


        // chua check layoff

        lsCardFree2.Clear();
        lsCardFree.Clear();

        lsMeldSame = findSameMeld(arr, ref lsCardFree2);
        lsMeldStraight = findStraightMeld(arr, ref lsCardFree);
        //th 1 check theo same meld
        var lsMeld4 = lsMeldSame.FindAll(x => x.Count > 3);
        for (var i = 0; i < lsMeldStraight.Count; i++)
        {
            for (var j = 0; j < lsCardFree2.Count; j++)
            {
                var isHas = lsMeldStraight[i].FirstOrDefault(x => x.code == lsCardFree2[j].code) != null;
                if (isHas)
                {
                    var listSameTemp = new List<Card>();
                    listSameTemp.Add(lsCardFree2[j]);
                    foreach (var it in lsMeldStraight[i])
                    {
                        var lsss = lsMeld4.FirstOrDefault(x => x.FirstOrDefault(y => y.code == it.code));
                        if (lsss != null)
                        {
                            listSameTemp.Add(it);
                        }
                    }

                    if (checkValidMeld(listSameTemp))
                    {
                        lsCardFree2.RemoveAt(j);
                        foreach (var ls in listSameTemp)
                        {
                            var lsss = lsMeld4.FirstOrDefault(x => x.FirstOrDefault(y => y.code == ls.code));
                            if (lsss != null)
                            {
                                lsss.Remove(ls);
                            }
                        }
                        lsMeldSame.Add(listSameTemp);
                        j--;
                    }
                }
            }
        }

        if (lsCardFree.Count <= 1)
        {
            goto CHECK_AGAIN;
        }
        //th 1 check theo straight meld
        lsMeld4 = lsMeldStraight.FindAll(x => x.Count > 3);
        for (var i = 0; i < lsMeldSame.Count; i++)
        {
            for (var j = 0; j < lsCardFree2.Count; j++)
            {
                var isHas = lsMeldSame[i].FirstOrDefault(x => x.code == lsCardFree2[j].code) != null;
                if (isHas)
                {
                    var listSameTemp = new List<Card>();
                    listSameTemp.Add(lsCardFree2[j]);
                    foreach (var it in lsMeldSame[i])
                    {
                        var lsss = lsMeld4.FirstOrDefault(x => x.FirstOrDefault(y => y.code == it.code));
                        if (lsss != null)
                        {
                            listSameTemp.Add(it);
                        }
                    }

                    if (checkValidMeld(listSameTemp))
                    {
                        lsCardFree2.RemoveAt(j);
                        foreach (var ls in listSameTemp)
                        {
                            var lsss = lsMeld4.FirstOrDefault(x => x.FirstOrDefault(y => y.code == ls.code));
                            if (lsss != null)
                            {
                                lsss.Remove(ls);
                            }
                        }
                        lsMeldStraight.Add(listSameTemp);
                        j--;
                    }
                }
            }
        }
        if (lsCardFree.Count <= 1)
        {
            goto CHECK_AGAIN2;
        }


        result.allMeld = lsMeldSame.Count >= lsMeldStraight.Count ? lsMeldSame : lsMeldStraight;
        result.cardLeft = lsCardFree;
        return result;
    }
    /*-=-=-==end new code==-=-=-=-*/


    bool checkCardSingle(Card cardTemp, List<List<Card>> allMeld, JArray allLayoff)
    {
        //allLayoff: [{ idCardSend: 19, idMeld: 0, sendtoPlayer: 182523}]
        foreach (var array in allMeld)
        {
            if (array.FindIndex(card => card.code == cardTemp.code) != -1) return false;
        }

        for (var i = 0; i < allLayoff.Count; i++)
        {
            if ((int)allLayoff[i]["idCardSend"] == cardTemp.code) return false;
        }
        return true;
    }

    [SerializeField]
    RectTransform cardP1, cardP2;

    void showCardKnockOut()
    {//dev

        DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        {
            for (var i = 0; i < thisPlayer.vectorCard.Count; i++)
            {
                thisPlayer.vectorCard[i].DOKill();
                thisPlayer.vectorCard[i].transform.DOKill();
            }

            List<Card> lsCardsResultLayoff = new List<Card>();

            //dataKnockOut = simulateKnockOut();
            Logging.Log("-=-=-=-=-=-=-=showCardKnockOut");
            dataKnockOut.LogToString();
            var lsCardsResultMeld = new List<List<Card>>(dataKnockOut.allMeld);
            var allLayoff = new JArray(dataKnockOut.allLayoff);

            var cardLeft = new List<Card>(dataKnockOut.cardLeft);
            Card cleft = cardLeft.Count > 0 ? cardLeft[0] : null;
            ///// case phom thì bo gui
            List<Card> cardsThis = new List<Card>();
            cardsThis.AddRange(thisPlayer.vectorCard);


            var isSmall12 = cardsThis.Count < 12;

            var posY = getMyCardPosition(0).y;
            foreach (var lo in allLayoff)
            {
                var c = cardsThis.FirstOrDefault(cc => cc.code == (int)lo["idCardSend"]);
                if (c != null)
                {
                    lsCardsResultLayoff.Add(c);
                }
            }

            float rateDistance = .5f;
            var rateDiscard = PLAYERSCALE.x;
            if (cardsThis.Count > 12 && cardsThis.Count <= 24)
                rateDiscard = PLAYERSCALE.x - 0.2f;
            else if (cardsThis.Count > 24)
            {
                rateDistance = .35f;
                rateDiscard = PLAYERSCALE.x - 0.3f;
            }
            float totalWidth = 0f;
            float delDis0 = 30f;
            float delDis1 = 5f;
            float delDistance = 5f;
            for (var i = 0; i < lsCardsResultMeld.Count; i++)
            {
                var arr = lsCardsResultMeld[i];
                var isSame = checkSameCard(arr);
                for (var j = 0; j < arr.Count; j++)
                {
                    var card = arr[j];
                    if (j == 0)
                    {
                        totalWidth += card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance + (i == 0 ? 0 : card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance * rateDistance);
                    }
                    else if (j == 1 || isSame || isSmall12)
                    {
                        totalWidth += delDis0;
                    }
                    else
                    {
                        totalWidth += delDis1;
                    }
                }
                totalWidth += delDistance;
            }
            for (var i = 0; i < lsCardsResultLayoff.Count; i++)
            {
                var card = lsCardsResultLayoff[i];
                totalWidth += card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance + delDistance;
            }
            totalWidth += delDistance;
            if (cleft != null)
            {
                totalWidth += 2 * cleft.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance;
            }
            //spDemo.sizeDelta = new Vector2(totalWidth, 40);
            //var pos = spDemo.localPosition;
            //pos.x = 0;
            //spDemo.localPosition = pos;
            float posXRun = -totalWidth * 0.5f;
            var zIndexRun = 0;
            for (var i = 0; i < lsCardsResultMeld.Count; i++)
            {
                float pxxxx = 0.0f;
                //float pyyy = 0;
                var arr = lsCardsResultMeld[i];
                var isSame = checkSameCard(arr);
                for (var j = 0; j < arr.Count; j++)
                {
                    var card = arr[j];
                    if (j == 0)
                    {
                        posXRun += card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance + (i == 0 ? 0 : card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance * rateDistance);
                    }
                    else if (j == 1 || isSame || isSmall12)
                    {
                        posXRun += delDis0;
                    }
                    else
                    {
                        posXRun += delDis1;
                    }
                    pxxxx += posXRun;
                    card.transform.SetSiblingIndex(zIndexRun);
                    zIndexRun++;
                    card.transform.localScale = new Vector3(rateDiscard, rateDiscard, rateDiscard);
                    card.transform.DOLocalMove(new Vector3(posXRun, posY), 0.2f);
                }
                showTagKnock(true, 2, new Vector2(pxxxx / arr.Count, posY), rateDiscard / PLAYERSCALE.x);
                posXRun += delDistance;
            }

            float pxx = 0;
            float pyy = 0;
            for (var i = 0; i < lsCardsResultLayoff.Count; i++)
            {
                var card = lsCardsResultLayoff[i];
                posXRun += card.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance;
                posXRun += delDistance;
                card.transform.SetSiblingIndex(zIndexRun);
                zIndexRun++;
                card.transform.localScale = new Vector3(rateDiscard, rateDiscard, rateDiscard);
                card.transform.DOLocalMove(new Vector3(posXRun, posY), 0.2f);
                pxx += posXRun;
                pyy = card.transform.localPosition.y;
                //showTagKnock(true, 3, new Vector2(posXRun, card.transform.localPosition.y), rateDiscard / PLAYERSCALE.x);
            }
            if (lsCardsResultLayoff.Count > 0)
                showTagKnock(true, 3, new Vector2(pxx / lsCardsResultLayoff.Count, pyy), rateDiscard / PLAYERSCALE.x);
            if (cleft != null)
            {
                posXRun += cleft.GetComponent<RectTransform>().rect.width * rateDiscard * rateDistance + delDistance;
                cleft.transform.SetSiblingIndex(zIndexRun);
                cleft.transform.localScale = new Vector3(rateDiscard, rateDiscard, rateDiscard);
                cleft.transform.DOLocalMove(new Vector3(posXRun, posY), 0.2f);
                showTagKnock(true, 4, new Vector2(posXRun, cleft.transform.localPosition.y), rateDiscard / PLAYERSCALE.x);
            }

        });
    }

    Vector2 getKnockPosition(float indexCard, int idGroup, float scale = -11)
    {
        if (scale == -11) scale = PLAYERSCALE.x;
        var len = thisPlayer.vectorCard.Count;
        var basePos = new Vector2(-400, -300);
        var offSet = len < 20 ? 40 : 30;
        var offSetGroup = 40;
        if (len < 13) offSetGroup = 80;
        else if (len < 20) offSetGroup = 60;
        else if (len < 25) offSetGroup = 40;
        else offSetGroup = 40;
        // var offSetGroup = len >= 13 ? (len >= 20 ? (len >= 25 ? (len >= 30 ? 30 : 40) : 40) : 60) : 80;
        return new Vector2(basePos.x + (offSet * indexCard + offSetGroup * idGroup) * (scale / PLAYERSCALE.x), basePos.y);
    }

    //getKnockPosition(indexCard, idGroup, scale = PLAYERSCALE)
    //{
    //    var len = thisPlayer.vectorCard.Count;
    //    var basePos = cc.v2(-400, -300);
    //    var offSet = len < 20 ? 40 : 30;
    //    var offSetGroup = 40;
    //    if (len < 13) offSetGroup = 80;
    //    else if (len < 20) offSetGroup = 60;
    //    else if (len < 25) offSetGroup = 40;
    //    else offSetGroup = 40;
    //    // var offSetGroup = len >= 13 ? (len >= 20 ? (len >= 25 ? (len >= 30 ? 30 : 40) : 40) : 60) : 80;
    //    return cc.v2(basePos.x + (offSet * indexCard + offSetGroup * idGroup) * (scale / PLAYERSCALE), basePos.y);
    //},

    List<GameObject> listTagKnock = new List<GameObject>();
    void showTagKnock(bool isShow, int type, Vector2 pos, float scale = 1.0f)
    {
        //// 2 - meld
        //// 3 - layoff
        //// 4 - discard
        if (!isShow)
        {
            //listTagKnock.forEach(tag =>
            //{
            //    tag.destroy();
            //});
            foreach (var ite in listTagKnock)
                Destroy(ite);
            listTagKnock.Clear();
            return;
        }
        //cc.NGWlog('!=> show tag knock', pos)

        var itemTextDown = Instantiate(lblTextDown.gameObject, transform);

        itemTextDown.SetActive(true);
        itemTextDown.transform.localScale = new Vector3(scale, scale, scale);

        listTagKnock.Add(itemTextDown);

        itemTextDown.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + 50);
        itemTextDown.transform.localPosition = pos;
        itemTextDown.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, type.ToString(), false);
    }

    public void sendDataKnockOut()
    {
        //Logging.Log(" sendDataKnockOut 0  ");
        //dataKnockOut.LogToString();
        //dataKnockOut = simulateKnockOut();
        Logging.Log(" sendDataKnockOut 1  ");
        dataKnockOut.LogToString();
        var allMeld = new List<List<Card>>(dataKnockOut.allMeld);
        var allLayoff = new JArray(dataKnockOut.allLayoff);
        var cardLeft = new List<Card>(dataKnockOut.cardLeft);


        Logging.Log(" thisPlayer.vectorCardD2   " + thisPlayer.vectorCardD2.Count);
        if (thisPlayer.vectorCardD2.Count < 1)
        {
            var dataKnock = new JObject();
            dataKnock["evt"] = "darkKnockOut";
            dataKnock["data"] = new JArray();


            for (var i = 0; i < allMeld.Count; i++)
            {
                var meld = allMeld[i];
                JArray arrCard = new JArray();
                foreach (var cc in meld)
                {
                    arrCard.Add(cc.code);
                }
                var dataMeld = new JObject();
                dataMeld["evt"] = "meld";
                dataMeld["arrCard"] = arrCard;
                ((JArray)dataKnock["data"]).Add(dataMeld);

            }

            var _temp = allLayoff.OrderBy(it => (int)it["idMeld"]);
            _temp = _temp.OrderBy(it => (int)it["sendtoPlayer"]);

            foreach (var ll in _temp)
            {
                var dataSend = new JObject();
                dataSend["evt"] = "layoff";
                dataSend["idCardSend"] = ll["idCardSend"];
                dataSend["idMeld"] = ll["idMeld"];
                dataSend["sendtoPlayer"] = ll["sendtoPlayer"];
                ((JArray)dataKnock["data"]).Add(dataSend);
            }


            if (cardLeft.Count == 1)
            {
                var dataDisCard = new JObject();
                dataDisCard["evt"] = "disCard";
                dataDisCard["id"] = cardLeft[0].code;
                ((JArray)dataKnock["data"]).Add(dataDisCard);

            }

            SocketSend.sendDummyKnock(dataKnock);
        }
        else
        {
            //new Thread(new ThreadStart(() =>
            //{
            Logging.Log(" allMeld   " + allMeld.Count);
            var indexSend = 0;
            for (var i = 0; i < allMeld.Count; i++)
            {
                var meldI = allMeld[i];

                JArray arrCode = new JArray();
                for (var j = 0; j < meldI.Count; j++)
                {
                    arrCode.Add(meldI[j].code);
                }
                Logging.Log("-=-= sendDataKnockOut Meld " + arrCode.ToString());
                SocketSend.sendDummyMeld(arrCode);
                indexSend++;
                //Thread.Sleep(50);
            }

            var _temp = allLayoff.OrderBy(it => (int)it["idMeld"]);
            _temp = _temp.OrderBy(it => (int)it["sendtoPlayer"]);
            var lisTe = _temp.ToList();
            for (var i = 0; i < lisTe.Count; i++)
            {
                var teI = (JObject)lisTe[i];
                Logging.Log("-=-= sendDataKnockOut Layoff " + i + " data " + teI.ToString());
                SocketSend.sendDummyLayoff(teI);
                indexSend++;
                //Thread.Sleep(50);
            }

            Logging.Log(" cardLeft   " + cardLeft.Count);
            if (cardLeft.Count == 1)
            {
                var cCode = cardLeft[0].code;
                Logging.Log(" cardLeft cCode  " + cCode);
                SocketSend.sendDummyDiscard(cCode);
            }
            //})).Start();
            /*
            var indexSend = 0;
            Sequence seq = DOTween.Sequence();
            for (var i = 0; i < allMeld.Count; i++)
            {
                var meldI = allMeld[i];
                seq.AppendInterval(0.01f * indexSend).AppendCallback(() =>
                {
                    JArray arrCode = new JArray();
                    for (var j = 0; j < meldI.Count; j++)
                    {
                        arrCode.Add(meldI[j].code);
                    }
                    Logging.Log("-=-= sendDataKnockOut Meld " + arrCode.ToString());
                    SocketSend.sendDummyMeld(arrCode);
                });
                indexSend++;
            }

            Logging.Log(" allLayoff   " + allLayoff.Count);
            var _temp = allLayoff.OrderBy(it => (int)it["idMeld"]);
            _temp = _temp.OrderBy(it => (int)it["sendtoPlayer"]);
            var lisTe = _temp.ToList();
            Logging.Log("-=-= sendDataKnockOut Layoff -1");
            for (var i = 0; i < lisTe.Count; i++)
            {
                var teI = (JObject)lisTe[i];
                seq.AppendInterval(0.01f * indexSend).AppendCallback(() =>
                {
                    Logging.Log("-=-= sendDataKnockOut Layoff " + i);
                    SocketSend.sendDummyLayoff(teI);
                });
                indexSend++;
            }

            Logging.Log(" cardLeft   " + cardLeft.Count);
            if (cardLeft.Count == 1)
            {
                var cCode = cardLeft[0].code;
                seq.AppendInterval(0.01f * indexSend).AppendCallback(() =>
                {
                    SocketSend.sendDummyDiscard(cCode);
                });
            }
            */
        }
    }

    public void handleShow(JObject data)
    {
        //// {"evt":"show","data":[11,14],"pid":1815863,"point":50,"addPoint":50}
        var player = getPlayerWithID((int)data["pid"]);
        player.isSpecial = true;
        ((PlayerViewDummy)player.playerView).updateKaengPoint((int)data["point"]);
        // player.playerView.updatePotDummy();
        showAnim(false, 6, player._indexDynamic);
        showTag(player, 11, "+" + (int)data["point"]);
        createOtherCard(player, 0, true);
        setPlayerTurn((int)data["pid"]);
    }

    public void handlePotInfo(JObject data)
    {
        //{ "evt":"potInfo","addPot":200,"agPot":400,"agInfoLst":[{ "uid":5276,"ag":29612338},{ "uid":974,"ag":11983}],"round":1}
        JArray agInfoLst = (JArray)data["agInfoLst"];
        //addPot = data.addPot

        //agPot = data.agPot

        foreach (JObject playerData in agInfoLst)
        {
            var pl = getPlayerWithID((int)playerData["uid"]);
            if (pl != null)
            {
                pl.ag = (int)playerData["ag"];
                pl.setAg();
                pl.playerView.effectFlyMoney(-(int)data["addPot"]);
            }
        }
        roundHitpot = (int)data["round"];
        setInfoPot((int)data["agPot"]);
    }

    void setInfoPot(int agPot)
    {
        //if (agPot < 1000000)
        //{
        //    txtPot.text = Config.FormatNumber(agPot);
        //}
        //else
        //{
        txtPot.text = Config.FormatMoney2(agPot);
        //}
        if (agPot != 0)
        {
            txtRoundPot.text = Config.getTextConfig("txt_round") + " " + roundHitpot;
        }
    }

    void resetHitpot()
    {
        setInfoPot(0);
        hitpot.parent = BgHitpot;
        hitpot.localPosition = new Vector3(-79.8f, 0, 0);
        BgHitpot.gameObject.SetActive(true);
        hitpot.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "03", true);
        hitpot.GetChild(0).gameObject.SetActive(false);
        hitpot.GetChild(1).gameObject.SetActive(false);
    }
    void createOtherCard(Player player, int number, bool isSpecial = false)
    {
        if (player == thisPlayer) return;
        //var posPlayer = player.playerView.transform.localPosition;
        //var sizePlayer = player.playerView.GetComponent<RectTransform>().sizeDelta;
        if (isSpecial == true)
        {
            var codes = new List<int>() { 11, 14 };

            var xPos = -10;
            var angle = -5;
            foreach (var code in codes)
            {
                var cardTemp = getCard(player.playerView.transform);
                cardTemp.transform.localScale = OTHERSCALE;
                cardTemp.isAllowTouch = false;
                player.vectorCard.Add(cardTemp);
                cardTemp.transform.localPosition = new Vector3(xPos, 0);

                cardTemp.setTextureWithCode(code);

                xPos += 20;
                angle += 10;
            }
            return;
        }

        var cardTemp2 = getCard(player.playerView.transform);
        cardTemp2.transform.localScale = OTHERSCALE;
        cardTemp2.isAllowTouch = false;
        cardTemp2.transform.localPosition = new Vector3(player._indexDynamic < 2 ? 55 : -55, -10);
        player.vectorCard.Add(cardTemp2);
        cardTemp2.transform.SetSiblingIndex((int)ZODER_VIEW.CARD);

        ((PlayerViewDummy)player.playerView).updateNumCard(number, cardTemp2.transform.localPosition);
    }

    bool checkIsLcSpecial(Player player)
    {
        Debug.Log("-=-=checkIsLcSpecial 0 ");
        if (player != thisPlayer) return false;
        Debug.Log("-=-=checkIsLcSpecial 1 ");
        if (player.isFold) return false;
        Debug.Log("-=-=checkIsLcSpecial 2 ");

        var count = 0;
        foreach (var cardTemp in player.vectorCard)
        {
            if (cardTemp.code == 11 || cardTemp.code == 14) count++;
        }
        Debug.Log("-=-=checkIsLcSpecial 3 ");
        if (count == 2) return true;
        Debug.Log("-=-=checkIsLcSpecial 4 ");
        return false;
    }

    void burnPlayer(Player player, JObject playerData)
    {
        if ((bool)playerData["isBurn"])
        {
            showAnim(false, 2, player._indexDynamic);
            showTag(player, 0, "-x2");
        }
    }

    void showResultCardPlayers(JObject playerData, int indexYourPlayers)
    {
        if (playerData.ContainsKey("arrCard") && ((JArray)playerData["arrCard"]).Count == 0) return;
        //cc.NGWlog("FinishGAme", indexYourPlayers);
        var boxResult = listBoxResult[indexYourPlayers];
        boxResult.SetActive(true);

        var arrCard = (JArray)playerData["arrCard"];
        for (var i = 0; i < arrCard.Count; i++)
        {
            var cardTemp = getCard(listBoxResult[indexYourPlayers].transform);
            //cardTemp.node.setScale(0.3);
            cardTemp.transform.localScale = new Vector3(0.3f, 0.3f);
            cardTemp.isAllowTouch = false;
            var pos = getResultBoxCardPosition(i, arrCard.Count);
            //cardTemp.node.zIndex = GAME_ZORDER.Z_CARD + i;
            cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
            //cardTemp.node.rotation = 10;
            cardTemp.transform.localPosition = pos;
            var idCard = (int)arrCard[i];
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.1f * i + 1f);
            seq.AppendCallback(() =>
            {
                foldCardUp(cardTemp, idCard);
            });
        }

        var lable = listBoxResult_lb[indexYourPlayers];

        lable.gameObject.SetActive(true);
        lable.text = "-" + checkPointResultCardLeft(arrCard);

        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(7);
        seq2.AppendCallback(() =>
        {
            boxResult.SetActive(false);
            lable.gameObject.SetActive(false);
            UIManager.instance.destroyAllChildren(boxResult.transform);

        });
    }

    int checkPointResultCardLeft(JArray arrayCard)
    {
        if (arrayCard.Count == 0) return -1;
        var resultCardLeft = 0;
        for (var i = 0; i < arrayCard.Count; i++)
        {
            var chat = (((int)arrayCard[i] - 1) / 13) + 1; //>=1 <=4
            var so = (((int)arrayCard[i] - 1) % 13) + 2; // >=2 , <=14 // A=14
                                                         // case 1 'bich';
                                                         // case 2 'tep';
                                                         // case 3 'ro';
                                                         // case 4 'co';
                                                         //cc.log('checkPointResultCardLeft==>', so, '--', chat)

            if ((chat == 1 && so == 12) || (chat == 2 && so == 2)) // case 2 tep, Q bich
                resultCardLeft += 50;
            else if (so == 14) // case A
                resultCardLeft += 15;
            else if (1 < so && so < 10) // case 2->9
                resultCardLeft += 5;
            else if (9 < so && so < 14) // case 10->K
                resultCardLeft += 10;
            //else cc.log('loi code card') //loi

        }
        return resultCardLeft;
    }


    int iden = 0;

    List<int> ArrNMe;
    List<int> ArrSMe;

    List<List<int>> ArrNMeP;
    List<List<int>> ArrSMeP;

    List<List<int>> ArrNOtherP;
    List<List<int>> ArrSOtherP;

    int indexx = 1;
    public void onClickChangeCard()
    {
        switch (indexx)
        {
            case 1:
                {
                    //ArrNMe = new List<int>() { 2, 3, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 4, 5, 6, 7, 8, 9, 10, 11, 12, 9, 10, 11, 12, 13, 14, 8, 9, 10, 11, 12, 13, 14, 14 };
                    //ArrSMe = new List<int>() { 1, 1, 4, 4, 4, 4, 4, 4, 04, 04, 04, 04, 04, 2, 2, 2, 2, 2, 2, 02, 02, 02, 1, 01, 01, 01, 01, 01, 3, 3, 03, 03, 03, 03, 02, 3 };

                    //ArrNMeP = new List<List<int>>() { new List<int>() { 3, 4, 5, 6, 7} };
                    //ArrSMeP = new List<List<int>>() { new List<int>() { 3,3,3,3,3 } };

                    //ArrNOtherP = new List<List<int>>() { new List<int>() { 2,2,2 }, new List<int>() { 4,5,6,7,8 } };
                    //ArrSOtherP = new List<List<int>>() { new List<int>() { 2,3,4}, new List<int>() { 1,1,1,1,1 } };

                    ArrNMe = new List<int>() { 10, 11, 12, 13 };
                    ArrSMe = new List<int>() { 2, 2, 2, 2 };

                    ArrNMeP = new List<List<int>>() { };
                    ArrSMeP = new List<List<int>>() { };


                    ArrNOtherP = new List<List<int>>() { };
                    ArrSOtherP = new List<List<int>>() { };
                    break;
                }
            case 2:
                {
                    ArrNMe = new List<int>() { 2, 3, 3, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 11, 11, 12, 12, 12, 12, 13, 14 };
                    ArrSMe = new List<int>() { 1, 1, 3, 1, 3, 1, 3, 4, 1, 3, 4, 1, 3, 4, 1, 4, 1, 4, 1, 2, 1, 2, 3, 4, 1, 2, 3, 4, 1, 1 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 3, 4, 5, 6, 7, 8 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 2, 2, 2, 2, 2, 2 } };


                    ArrNOtherP = new List<List<int>>() { new List<int>() { 8, 9, 10 }, new List<int>() { 13, 13, 13 }, new List<int>() { 2, 3, 4 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 3, 3, 3 }, new List<int>() { 1, 3, 4 }, new List<int>() { 4, 4, 4 } };
                    break;
                }

            case 3:
                {
                    ArrNMe = new List<int>() { 2, 2, 2, 5, 5, 5, 7, 7, 7, 8, 8, 8, 8, 10, 11, 11, 11, 12, 13, 13, 13 };
                    ArrSMe = new List<int>() { 4, 3, 2, 3, 4, 2, 4, 2, 1, 1, 4, 2, 3, 02, 01, 02, 03, 01, 03, 04, 01 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 2, 3, 4 }, new List<int>() { 14, 14, 14 }, new List<int>() { 9, 9, 9 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 1, 1 }, new List<int>() { 1, 3, 4 }, new List<int>() { 2, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 10, 10, 10 }, new List<int>() { 6, 6, 6 }, new List<int>() { 4, 4, 4 }, new List<int>() { 12, 12, 12 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 3, 4 }, new List<int>() { 1, 2, 4 }, new List<int>() { 2, 3, 4 }, new List<int>() { 2, 3, 4 } };
                    break;
                }
            case 4:
                {
                    ArrNMe = new List<int>() { 2, 3, 3, 4, 5, 6, 7, 8, 9, 10, 12, 12, 12, 13, 13, 13, 14, 14, 14 };
                    ArrSMe = new List<int>() { 2, 1, 2, 2, 3, 3, 3, 3, 3, 3, 4, 2, 3, 2, 4, 3, 1, 2, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 6, 7, 8, 9 }, new List<int>() { 11, 11, 11, 11 }, new List<int>() { 2, 3, 4 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 4, 4, 4, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 4, 4, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 4, 5, 6 }, new List<int>() { 12, 12, 12 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 1, 1 }, new List<int>() { 2, 2, 2 } };
                    break;
                }

            case 5:
                {
                    ArrNMe = new List<int>() { 9, 10, 10, 10, 10, 11, 11, 11, 11, 12, 12, 12, 12, 13, 13, 13, 13 };
                    ArrSMe = new List<int>() { 2, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 5, 5, 5, 5 }, new List<int>() { 2, 2, 2, 2 }, new List<int>() { 8, 8, 8, 8 }, new List<int>() { 4, 4, 4 }, new List<int>() { 7, 7, 7 }, new List<int>() { 9, 9, 9 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 2, 3, 4 }, new List<int>() { 1, 2, 3 }, new List<int>() { 1, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 3, 3, 3 }, new List<int>() { 6, 6, 6 }, new List<int>() { 14, 14, 14, 14 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 3, 4 }, new List<int>() { 1, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };
                    break;
                }

            case 6:
                {
                    ArrNMe = new List<int>() { 3, 4, 3, 4, 5, 6, 7, 2, 3, 4, 5, 6, 3, 4, 5, 6 };
                    ArrSMe = new List<int>() { 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4 };

                    //ArrNMeP = new List<List<int>>() { new List<int>() { 10, 11, 12, 13, 14 }, new List<int>() { 2, 2, 2 }, new List<int>() { 8, 9, 10, 11 }, new List<int>() { 5, 6, 7, 8, 9 }, new List<int>() { 13, 13, 13 }, new List<int>() { 14, 14, 14 }, new List<int>() { 7, 8, 9, 10 } };
                    //ArrSMeP = new List<List<int>>() { new List<int>() { 1, 1, 1, 1, 1 }, new List<int>() { 1, 2, 4 }, new List<int>() { 2, 2, 2, 2 }, new List<int>() { 1, 1, 1, 1, 1 }, new List<int>() { 1, 3, 4 }, new List<int>() { 1, 3, 4 }, new List<int>() { 4, 4, 4, 4 } };
                    ArrNMeP = new List<List<int>>() { new List<int>() { 10, 11, 12, 13, 14 }, new List<int>() { 8, 9, 10, 11 }, new List<int>() { 5, 6, 7, 8, 9 }, new List<int>() { 13, 13, 13 }, new List<int>() { 14, 14, 14 }, new List<int>() { 7, 8, 9, 10 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 1, 1, 1, 1 }, new List<int>() { 2, 2, 2, 2 }, new List<int>() { 1, 1, 1, 1, 1 }, new List<int>() { 1, 3, 4 }, new List<int>() { 1, 3, 4 }, new List<int>() { 4, 4, 4, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 8, 9, 10, 11 }, new List<int>() { 12, 12, 12 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 3, 3, 3, 3 }, new List<int>() { 2, 3, 4 } };
                    break;
                }
            case 7:
                {

                    ArrNMe = new List<int>() { 2, 4, 2, 4, 6, 7, 8, 12, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 14, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    ArrSMe = new List<int>() { 1, 1, 2, 2, 2, 2, 2, 02, 3, 3, 3, 3, 3, 3, 3, 03, 03, 03, 03, 4, 4, 4, 4, 4, 4, 4, 04, 04, 04 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 13, 13, 13, 13 }, new List<int>() { 8, 9, 10, 11, 12 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 1, 1, 1, 1 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 5, 6, 7 }, new List<int>() { 9, 10, 11 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 1, 1 }, new List<int>() { 2, 2, 2 } };

                    break;
                }
            case 8:
                {

                    ArrNMe = new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 5, 6, 7, 8, 9, 3 };
                    ArrSMe = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 1 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 10, 11, 12, 13, 14 }, new List<int>() { 5, 6, 7, 8, 9, 10, 11, 12 }, new List<int>() { 8, 9, 10, 11, 12, 13, 14 }, new List<int>() { 2, 3, 4 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 3, 3, 3, 3, 3 }, new List<int>() { 2, 2, 2, 2, 2, 2, 2, 2 }, new List<int>() { 4, 4, 4, 4, 4, 4, 4 }, new List<int>() { 3, 3, 3 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 3, 4, 5 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 4, 4, 4 } };

                    break;
                }
            case 9:
                {


                    ArrNMe = new List<int>() { 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 6, 7, 11, 8, 9, 9, 10, 10, 12, 13, 14, 14, 14, 14 };
                    ArrSMe = new List<int>() { 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 1, 1, 1, 3, 1, 3, 1, 3, 3, 3, 1, 2, 3, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 11, 12, 13 }, new List<int>() { 5, 5, 5 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 2, 2, 2 }, new List<int>() { 1, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 6, 7, 8 }, new List<int>() { 6, 7, 8, 9, 10 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 4, 4, 4 }, new List<int>() { 2, 2, 2, 2, 2 } };

                    break;
                }
            case 10:
                {

                    ArrNMe = new List<int>() { 9, 9, 7, 10, 5, 6, 5, 7, 6, 8, 8, 5, 9, 7, 7, 6, 9, 10, 6, 8, 8 };
                    ArrSMe = new List<int>() { 3, 4, 4, 02, 1, 4, 2, 3, 2, 2, 4, 4, 1, 2, 1, 1, 2, 01, 3, 3, 1 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 3, 3, 3, 3 }, new List<int>() { 14, 14, 14, 14 }, new List<int>() { 2, 2, 2 }, new List<int>() { 4, 4, 4, 4 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 13, 13, 13 }, new List<int>() { 12, 12, 12, 12 }, new List<int>() { 11, 11, 11, 11 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };

                    break;
                }
            case 11:
                {

                    ArrNMe = new List<int>() { 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 13, 13, 13 };
                    ArrSMe = new List<int>() { 2, 3, 4, 2, 3, 4, 2, 3, 4, 2, 3, 4, 2, 3, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 14, 14, 14 }, new List<int>() { 4, 4, 4 }, new List<int>() { 8, 8, 8, 8 }, new List<int>() { 6, 6, 6 }, new List<int>() { 9, 10, 11, 12 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 3, 4, }, new List<int>() { 1, 2, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 2, 3, 4 }, new List<int>() { 1, 1, 1, 1 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 7, 7, 7, 7 }, new List<int>() { 5, 5, 5, 5 }, new List<int>() { 3, 3, 3, 3 }, new List<int>() { 2, 2, 2, 2 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };

                    break;
                }
            case 12:
                {

                    ArrNMe = new List<int>() { 5, 7, 8, 9, 9, 10, 10, 11, 11, 11, 13, 13, 13 };
                    ArrSMe = new List<int>() { 2, 2, 2, 2, 3, 3, 2, 2, 3, 4, 2, 3, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 2, 3, 4 }, new List<int>() { 3, 3, 3 }, new List<int>() { 4, 5, 6, 7 }, new List<int>() { 14, 14, 14, 14 }, new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, new List<int>() { 12, 12, 12 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 4, 4, 4 }, new List<int>() { 1, 2, 3 }, new List<int>() { 3, 3, 3, 3 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, new List<int>() { 2, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 5, 6, 7, 8, 9, 10 }, new List<int>() { 2, 2, 2 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 4, 4, 4, 4, 4, 4 }, new List<int>() { 1, 2, 3 } };
                    break;
                }
            case 13:
                {
                    ArrNMe = new List<int>() { 3, 3, 8, 8, 8, 9, 9, 9, 10, 10, 10, 10, 11, 11, 11, 11, 12, 12, 12, 13, 13, 13, 13, 14, 14, 3 };
                    ArrSMe = new List<int>() { 2, 3, 1, 2, 4, 2, 3, 4, 01, 02, 03, 04, 01, 02, 03, 04, 01, 02, 04, 01, 02, 03, 04, 02, 03, 4 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 4, 4, 4, 4 }, new List<int>() { 6, 6, 6, 6 }, new List<int>() { 7, 7, 7, 7 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 2, 2, 2, 2 }, new List<int>() { 5, 5, 5, 5 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 2, 3, 4 } };
                    break;
                }
            case 14:
                {
                    ArrNMe = new List<int>() { 2, 4, 2, 4, 6, 7, 8, 12, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    ArrSMe = new List<int>() { 1, 1, 2, 2, 2, 2, 2, 02, 3, 3, 3, 3, 3, 3, 3, 03, 03, 03, 03, 4, 4, 4, 4, 4, 4, 4, 04, 04, 04 };

                    ArrNMeP = new List<List<int>>() { new List<int>() { 13, 13, 13, 13 }, new List<int>() { 8, 9, 10, 11, 12 } };
                    ArrSMeP = new List<List<int>>() { new List<int>() { 1, 2, 3, 4 }, new List<int>() { 1, 1, 1, 1, 1 } };

                    ArrNOtherP = new List<List<int>>() { new List<int>() { 5, 6, 7 }, new List<int>() { 9, 10, 11 } };
                    ArrSOtherP = new List<List<int>>() { new List<int>() { 1, 1, 1 }, new List<int>() { 2, 2, 2 } };
                    break;
                }
        }
        foreach (var ite in listTagKnock)
            Destroy(ite);
        listTagKnock.Clear();
        resetGame();
        iden = 1;
        indexx++;
        if (indexx > 14)
        {
            indexx = 1;
        }

        Logging.Log("index thu " + indexx);

    }
    int encodeCard(int S, int N)
    {
        return 13 * (S - 1) + N - 1;
    }

    public void onClickDemo()
    {
        //List<Card> arr = new List<Card>();
        //var ArrNMe = new List<int>() { 14, 2, 5, 3 };
        //var ArrSMe = new List<int>() { 3, 1, 3, 3 };
        //for (var i = 0; i < 4; i++)
        //{
        //    var c = getCard();
        //    c.setTextureWithCode(encodeCard(ArrSMe[i], ArrNMe[i]));
        //    arr.Add(c);
        //}
        //List<Card> lsCardFree2 = new List<Card>();
        //List<Card> lsCardFree = new List<Card>();

        //var lsMeldStraight = findStraightMeld(arr, ref lsCardFree2);
        //var lsMeldSame = findSameMeld(lsCardFree2, ref lsCardFree);

        //var lsAllMeld = new List<List<Card>>();
        //lsAllMeld.AddRange(lsMeldStraight);
        //lsAllMeld.AddRange(lsMeldSame);
        //var lsCardFreeBackUp = new List<Card>();
        //lsCardFreeBackUp.AddRange(lsCardFree);

        //lsCardFree.Clear();
        //var lsCardLayoff = new List<Card>();
        //var checkLayoff = simulateAllLayoff(lsCardFreeBackUp, ref lsCardFree, ref lsCardLayoff); //check cac card gui dc

        //hitpot.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
        //hitpot.GetChild(2).gameObject.SetActive(false);
        //DOTween.Sequence().AppendInterval(1).AppendCallback(() =>
        //{
        //    //player.playerView.effectFlyMoney((int)playerData["hitPot"]);
        //    //player.playerView.setAg((int)playerData["ag"] + (int)playerData["hitPot"]);
        //    hitpot.parent = transform;
        //    hitpot.SetAsLastSibling();
        //    hitpot.localPosition = new Vector3(0, 15);
        //    BgHitpot.gameObject.SetActive(true);
        //    hitpot.GetChild(0).gameObject.SetActive(false);
        //    hitpot.GetChild(1).gameObject.SetActive(false);
        //    hitpot.GetChild(2).gameObject.SetActive(true);
        //    hitpot.GetChild(2).GetComponent<TextMeshProUGUI>().text = Config.FormatNumber(7890);
        //    var animHitPot = hitpot.GetComponent<SkeletonGraphic>();
        //    animHitPot.AnimationState.SetAnimation(0, "05_open_chest", false);
        //}).AppendInterval(2).AppendCallback(()=>
        //{
        //    hitpot.GetChild(2).gameObject.SetActive(false);
        //});
        //return;
        if (iden == 0)
        {
            var strData = "{\"evt\":\"stable\",\"data\":\"{\\\"M\\\":100,\\\"ArrP\\\":[{\\\"id\\\":5283,\\\"N\\\":\\\"anhhihi99\\\",\\\"AG\\\":5489051,\\\"VIP\\\":2,\\\"Av\\\":12,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"displayName\\\":\\\"anhhihi99\\\",\\\"numberMatchWin\\\":0,\\\"keyObjectInGame\\\":0,\\\"Tinyurl\\\":\\\"\\\"},{\\\"id\\\":388638,\\\"N\\\":\\\"tictctoe123\\\",\\\"AG\\\":1989236,\\\"VIP\\\":1,\\\"Av\\\":21,\\\"FId\\\":0,\\\"UserType\\\":0,\\\"TotalAG\\\":0,\\\"timeToStart\\\":0,\\\"displayName\\\":\\\"tictctoe123\\\",\\\"numberMatchWin\\\":0,\\\"keyObjectInGame\\\":0,\\\"Tinyurl\\\":\\\"\\\"}],\\\"Id\\\":238,\\\"AG\\\":50000,\\\"S\\\":4,\\\"pot\\\":0,\\\"lstWinning\\\":[],\\\"round\\\":1}\"}";
            HandleGame.processData(JObject.Parse(strData));
        }
        else if (iden == 1)
        {
            for (var i = 0; i < ArrSMe.Count; i++)
            {
                var cardTemp = getCard(cardParent);
                var coo = encodeCard(ArrSMe[i], ArrNMe[i]);
                cardTemp.setTextureWithCode(coo);
                cardTemp.transform.localScale = PLAYERSCALE;
                cardTemp.isAllowTouch = false;
                cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
                cardTemp.name = cardTemp.LogInfo();
                thisPlayer.vectorCard.Add(cardTemp);

                onClickSort();
                this.btnSort.gameObject.SetActive(true);
            }

            for (var i = 0; i < ArrSMeP.Count; i++)
            {
                List<Card> lsTemp = new List<Card>();
                for (var j = 0; j < ArrSMeP[i].Count; j++)
                {
                    var cardTemp = getCard(cardP1);
                    var coo = encodeCard(ArrSMeP[i][j], ArrNMeP[i][j]);
                    cardTemp.setTextureWithCode(coo);
                    cardTemp.transform.localScale = PLAYERSCALE;
                    cardTemp.isAllowTouch = false;
                    cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
                    lsTemp.Add(cardTemp);
                }
                thisPlayer.vectorCardD2.Add(lsTemp);
            }

            var pl = getPlayerWithID(5283);

            for (var i = 0; i < ArrSOtherP.Count; i++)
            {
                List<Card> lsTemp = new List<Card>();
                for (var j = 0; j < ArrSOtherP[i].Count; j++)
                {
                    var cardTemp = getCard(cardP2);
                    var coo = encodeCard(ArrSOtherP[i][j], ArrNOtherP[i][j]);
                    cardTemp.setTextureWithCode(coo);
                    cardTemp.transform.localScale = PLAYERSCALE;
                    cardTemp.isAllowTouch = false;
                    cardTemp.transform.SetSiblingIndex((int)ZODER_VIEW.CARD + i);
                    lsTemp.Add(cardTemp);
                }
                pl.vectorCardD2.Add(lsTemp);
            }


            //return;
            //dataKnockOut = simulateKnockOut();
            bool isCheckKnboo = checkKnockOut();
            dataKnockOut.LogToString();

            Logging.Log("-=-=isCheckKnboo   " + isCheckKnboo);
        }
        else if (iden == 2)
            showCardKnockOut();
        else
        {
            sendDataKnockOut();
        }
        iden++;
    }

    public void onClickClearLog()
    {
        UIManager.instance.sendLog("", true);
    }

}