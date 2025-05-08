using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.EventSystems;
using Globals;
public class GaoGeaView : GameView
{
    // Start is called before the first frame update
    [SerializeField]
    public SkeletonGraphic anim_start;

    [SerializeField]
    public SkeletonGraphic ani_dealer;

    [SerializeField]
    public SkeletonDataAsset aniWin;

    [SerializeField]
    public GameObject bg_arrow_swap;

    [SerializeField]
    public GameObject NodeBetPf;

    [SerializeField]
    public GameObject chipEffect;

    [SerializeField]
    public PotShow Pot;

    [SerializeField]
    public GameObject textChipReturn;

    [SerializeField]
    public List<Font> font;

    [SerializeField]
    public DealerInGameView DealerInGame;

    [SerializeField]
    public GameObject lbChangeCard, cardStack;

    [SerializeField]
    public GameObject buttonCheckTogglePf;

    [SerializeField]
    public GameObject chip_Tip;

    [SerializeField]
    public GameObject NodeCountDownPf;

    [SerializeField]
    public GameObject BoxBetPf;

    [SerializeField]
    public GameObject Table;


    [SerializeField]
    List<Sprite> listImgWinlose;

    [SerializeField]
    List<Sprite> listImgWinloseThai;

    [SerializeField]
    public GameObject chipReturnPf;

    [SerializeField]
    public GameObject bg_Score_Result;

    [SerializeField]
    public GameObject nodeRule;

    [SerializeField]
    public List<Vector2> listPosBoxBet;

    [SerializeField]
    public List<Vector2> listPosCard;
    [SerializeField]
    public GameObject CardContainer;
    [SerializeField]
    public GameObject PlayerContainer;
    [SerializeField]
    public TextMeshProUGUI lbStateGame;

    [HideInInspector]
    private bool isRJ, isShow = false;
    private int _scoreP = 0, _rateP = 0;
    private bool isAllIn = false;
    protected List<GameObject> ShanClubScorePool = new List<GameObject>();
    protected List<GameObject> efWinLosePool = new List<GameObject>();
    protected List<GameObject> chipEffectPool = new List<GameObject>();
    //private Vector2 sizeDefine = new Vector2(Screen.width, Screen.height);
    private ShowNodeBet NodeBet;
    private CountDownTime NodeChangeTime;
    private List<List<GameObject>> playerCards = new List<List<GameObject>>();
    private CountDownTime nodeTimeToStart;
    private ToggleCheckShow buttonCheckToggle;
    private List<string> listDataFake = new List<string>();
    private Vector2 CARD_SIZE = new Vector2(147, 198);



    protected override void Awake()
    {
        base.Awake();
        NodeBet = null;
        NodeChangeTime = null;
        buttonCheckToggle = null;
        nodeTimeToStart = null;
        buttonCheckToggle = Instantiate(buttonCheckTogglePf, transform).GetComponent<ToggleCheckShow>();

        buttonCheckToggle.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
        buttonCheckToggle.gameObject.SetActive(false);
        playerCards = new List<List<GameObject>>() { new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>(), new List<GameObject>() };
    }
    protected override void Start()
    {
        base.Start();
        //listDataFake = new List<string>
        //{
        //    "{\"evt\":\"rjtable\",\"data\":\"{\\\"M\\\":100,\\\"ArrP\\\":[{\\\"id\\\":394867,\\\"N\\\":\\\"Nattaporn Lamjantuk\\\",\\\"AG\\\":1731,\\\"LQ\\\":1000,\\\"VIP\\\":2,\\\"Av\\\":8,\\\"FId\\\":564635725044069,\\\"UserType\\\":11,\\\"isPlaying\\\":true,\\\"cards\\\":[0,0,0],\\\"lastPlay\\\":1,\\\"agBet\\\":0,\\\"typeCard\\\":6,\\\"score\\\":-1,\\\"autoExit\\\":false,\\\"keyObjectInGame\\\":0},{\\\"id\\\":378086,\\\"N\\\":\\\"นายวัณโณ ชินราช\\\",\\\"AG\\\":4078,\\\"LQ\\\":1000,\\\"VIP\\\":0,\\\"Av\\\":4,\\\"FId\\\":125514200151285,\\\"UserType\\\":11,\\\"isPlaying\\\":true,\\\"cards\\\":[0,0,0],\\\"lastPlay\\\":1,\\\"agBet\\\":0,\\\"typeCard\\\":6,\\\"score\\\":-1,\\\"autoExit\\\":false,\\\"keyObjectInGame\\\":0},{\\\"id\\\":408696,\\\"N\\\":\\\"คง เดิม\\\",\\\"AG\\\":5295,\\\"LQ\\\":0,\\\"VIP\\\":1,\\\"Av\\\":0,\\\"FId\\\":1718971121787561,\\\"UserType\\\":2,\\\"isPlaying\\\":true,\\\"cards\\\":[0,0,0],\\\"lastPlay\\\":1,\\\"agBet\\\":0,\\\"typeCard\\\":6,\\\"score\\\":-1,\\\"autoExit\\\":false,\\\"keyObjectInGame\\\":0},{\\\"id\\\":8240,\\\"N\\\":\\\"hienndm\\\",\\\"AG\\\":99299,\\\"LQ\\\":0,\\\"VIP\\\":3,\\\"Av\\\":12,\\\"FId\\\":0,\\\"UserType\\\":1,\\\"isPlaying\\\":false,\\\"cards\\\":[50,30,1],\\\"lastPlay\\\":4,\\\"agBet\\\":0,\\\"typeCard\\\":5,\\\"score\\\":7,\\\"autoExit\\\":false,\\\"keyObjectInGame\\\":0},{\\\"id\\\":5363,\\\"N\\\":\\\"nanaboy12\\\",\\\"AG\\\":133180,\\\"LQ\\\":21,\\\"VIP\\\":2,\\\"Av\\\":3,\\\"FId\\\":0,\\\"UserType\\\":1,\\\"isPlaying\\\":true,\\\"cards\\\":[0,0,0],\\\"lastPlay\\\":0,\\\"agBet\\\":0,\\\"typeCard\\\":6,\\\"score\\\":-1,\\\"autoExit\\\":false,\\\"keyObjectInGame\\\":0}],\\\"Id\\\":7954,\\\"V\\\":0,\\\"AG\\\":1000,\\\"S\\\":9,\\\"currentTurn\\\":5363,\\\"timeLeave\\\":3,\\\"pot\\\":500,\\\"idDealer\\\":394867}\"}",
        //    "{\"evt\":\"raise\",\"id\":384538,\"agBet\":863,\"agCurrent\":5511,\"pot\":1163,\"agRaise\":863}",
        //    "{\"evt\":\"startgame\",\"pot\":150,\"currentBet\":0,\"cards\":[13,12,6],\"typeCard\":4,\"score\":0,\"idDealer\":101065,\"agPutPot\":50}",
        //    "{\"evt\":\"getNext\",\"pid\":101065,\"time\":15,\"agToCall\":0,\"allowCheck\":true}",
        //    "{\"evt\":\"raise\",\"id\":101065,\"agBet\":200,\"agCurrent\":1969,\"pot\":350,\"agRaise\":200}",
        //    "{\"evt\":\"getNext\",\"pid\":8240,\"time\":10,\"agToCall\":200,\"allowCheck\":false}",
        //    "{\"evt\":\"call\",\"id\":8240,\"agBet\":200,\"agCurrent\":41229,\"pot\":550,\"agRaise\":0}",
        //    "{\"evt\":\"getNext\",\"pid\":111434,\"time\":10,\"agToCall\":200,\"allowCheck\":false}",
        //    "{\"evt\":\"fold\",\"id\":111434,\"agBet\":0,\"agCurrent\":0,\"pot\":0,\"agRaise\":0}",
        //    "{\"evt\":\"finishgame\",\"idwinner\":8240,\"data\":[{\"id\":111434,\"ag\":6695,\"agWin\":0,\"agLost\":0,\"cards\":[0,0,0],\"typeCard\":5,\"score\":0},{\"id\":101065,\"ag\":1969,\"agWin\":0,\"agLost\":200,\"cards\":[39,10,33],\"typeCard\":5,\"score\":9},{\"id\":8240,\"ag\":41768,\"agWin\":539,\"agLost\":200,\"cards\":[13,12,6],\"typeCard\":4,\"score\":0}]}",
        //    "{\"evt\":\"pstartg\",\"time\":5}",
        //    "{\"evt\":\"startgame\",\"pot\":150,\"currentBet\":0,\"cards\":[34,7,46],\"typeCard\":5,\"score\":5,\"idDealer\":8240,\"agPutPot\":50}"
        //};
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        chipPool.ForEach(chip =>
        {
            Destroy(chip);
        });
        if ((Globals.Config.typeLogin == Globals.LOGIN_TYPE.PLAYNOW && Globals.User.userMain.VIP == 0) || Globals.Config.typeLogin == Globals.LOGIN_TYPE.FACEBOOK)
        {
            UIManager.instance.showListBannerOnLobby();
        }

    }
    public void onClickFake(Button btnFake)
    {
        string strData = listDataFake[0];
        Globals.Logging.Log("Fake strData:" + strData);

        //string strDataNext = listDataFake[1];
        JObject dataFake = JObject.Parse(strData);
        //JObject dataFakeNext = JObject.Parse(strDataNext);
        //btnFake.GetComponentInChildren<TextMeshProUGUI>().text = getString(dataFakeNext, "evt");
        HandleGame.processData(dataFake);
        listDataFake.RemoveAt(0);
    }
    public override void HandlerTip(JObject data)
    {
        playSound(Globals.SOUND_GAME.TIP);
        data["displayName"] = Globals.User.userMain.displayName;
        Player playerTip = getPlayer(getString(data, "N"));
        int AGTip = getInt(data, "AGTip");
        playerTip.playerView.effectFlyMoney(-AGTip, 40);
        playerTip.ag -= AGTip;
        playerTip.setAg();
        if (playerTip == thisPlayer)
        {
            Globals.User.userMain.AG -= AGTip;
        }
        for (int j = 0; j < 2; j++) // sinh ra 4 chip                     //  lay tien nguoi` thua
        {
            ChipBet temp = getChipBet(transform);
            temp.transform.localPosition = playerTip.playerView.transform.localPosition;
            temp.transform.localScale = new Vector2(0.5f, 0.5f);
            DOTween.Sequence()
                .AppendInterval(j * 0.2f)
                .Append(temp.transform.DOLocalMove(temp.transform.localPosition + new Vector3(0, 80), 0.2f))
                .AppendInterval(0.3f)
                .Append(temp.transform.DOLocalMove(DealerInGame.transform.localPosition, 1.0f).SetEase(Ease.InOutSine))
                .AppendCallback(() =>
                {
                    removerChip(temp);
                });
        }
        DOTween.Sequence()
            .AppendInterval(2.0f)
            .AppendCallback(() =>
            {
                DealerInGame.show(playerTip.namePl, AGTip);
            });

    }
    public override void handleSTable(string strData)
    {
        base.handleSTable(strData);
        thisPlayer.is_ready = true;

        var data = JObject.Parse(strData);
        JArray listPlayer = getJArray(data, "ArrP");
        resetViewGame(listPlayer);

    }
    public override void handleRJTable(string strData)
    {
        base.handleRJTable(strData);
        isRJ = true;
        var data = JObject.Parse(strData);
        var listPlayer = getJArray(data, "ArrP");
        //myChipCur = getInt(data, "AG");
        viewIng(listPlayer, data);
        for (int i = 0; i < listPlayer.Count; i++)
        {
            JObject dataPl = (JObject)listPlayer[i];
            int maxUserBet = 0;
            int agBet = getInt(dataPl, "agBet");
            for (int j = 0; j < listPlayer.Count; j++) maxUserBet = maxUserBet > agBet ? maxUserBet : agBet;
            Player player = getPlayerWithID(getInt(dataPl, "id"));
            if (player == thisPlayer && getInt(data, "currentTurn") > 0)
                if (getInt(dataPl, "id") == getInt(data, "currentTurn"))
                {
                    NodeBet.gameObject.SetActive(true);
                    if (maxUserBet > agBet)
                        NodeBet.setInfoBtn("Call", maxUserBet - agBet);
                    else
                        NodeBet.setInfoBtn("Check");
                    //Globals.Logging.Log("setValueInfo:player ag==" + player.ag);
                    NodeBet.setValueInfo(player.ag, (maxUserBet - agBet), agTable, Pot.getValue());

                    //setTimeout(() => {
                    //    NodeBet.node.active = false;
                    //}, (15 - data.timeLeave) * 1000);
                    DOTween.Sequence()
                        .AppendInterval((15 - getInt(data, "timeLeave")))
                        .AppendCallback(() =>
                        {
                            NodeBet.gameObject.SetActive(false);
                        });
                }
                else
                {
                    bool isPlaying = getBool(dataPl, "isPlaying");
                    NodeBet.gameObject.SetActive(false);
                    buttonCheckToggle.gameObject.SetActive(true);
                    int valueToggleCheck = getPlayerView(thisPlayer).boxbet.chip;
                    buttonCheckToggle.resetStatusToggle();
                    buttonCheckToggle.setInfo(maxUserBet, valueToggleCheck, thisPlayer.ag);
                    buttonCheckToggle.setStatus(!isPlaying);
                }
        }
    }
    public override void cleanGame()
    {
        base.cleanGame();
        efWinLosePool.Clear();
        chipEffectPool.Clear();
    }
    // Update is called once per frame
    public void handleTimeToStart(JObject strData)
    {
        showTextWaiting(false);
        stateGame = Globals.STATE_GAME.WAITING;
        resetViewGame();


        isAllIn = false;

        int temp = getInt(strData, "time");
        nodeTimeToStart = Instantiate(NodeCountDownPf, transform).GetComponent<CountDownTime>();

        nodeTimeToStart.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_CARD);
        nodeTimeToStart.setInfo(temp, 1);
    }
    public override void handleVTable(string strData)
    {
        base.handleVTable(strData);
        showTextWaiting(true);
        thisPlayer.is_ready = false;
        var data = JObject.Parse(strData);
        JArray listPlayer = getJArray(data, "ArrP");
        viewIng(listPlayer, data);
    }

    private void OpenCardViewing(float timeDelay, int id, int score, int rate, List<int> arrC)
    {
        DOTween.Sequence()
            .AppendInterval(timeDelay)
            .AppendCallback(() =>
            {
                openCardPlayer(id, arrC, score, rate);
            });
    }
    private void openCardPlayer(int id, List<int> arrC, int score = -1, int rate = 0)
    {
        playSound(Globals.SOUND_GAME.CARD_FLIP_1);
        Player player = getPlayerWithID(id);
        var indexPDyn = player._indexDynamic;
        if (indexPDyn == 0 && !isRJ) isShow = !isShow;
        List<Card> vectorCard = player.vectorCard;
        int length = vectorCard.Count;
        //int offset = 30;
        //int offsetRotation = 15;
        //int rotate = 0;
        int PosY = indexPDyn == 0 ? 0 : 15;
        int index = 0;
        float cardScale = indexPDyn == 0 ? 0.5f : 0.5f;
        Vector2 posCard0 = Vector2.zero;
        for (int i = 0; i < length; i++)
        {
            JObject data = getCardPositionNAngle(indexPDyn, i, cardScale, player);
            int deltaX = indexPDyn == 0 ? 0 : (indexPDyn > 4 ? 30 : -30);
            Vector2 posCard = new Vector2(getFloat(data, "posX") + deltaX, getFloat(data, "posY") + PosY);//pos nay tinh trong CardContainer cua table;

            Card card = vectorCard[i];
            Transform cardTf = card.transform;
            //cardTf.localPosition = new Vector2(cardTf.localPosition.x, vectorCard[0].transform.localPosition.y);
            posCard = cardTf.parent.InverseTransformPoint(CardContainer.transform.TransformPoint(posCard)); //pos nay tinh trong CardContainer cua player vi luc chia card xong add vao Container cua player roi
            if (i == 0)
            {
                posCard0 = posCard;
            }
            //cardTf.DOLocalRotate(new Vector3(cardTf.localRotation.x,cardTf.localRotation.y, rotate), 0.2f);
            cardTf.DOScale(new Vector2(0.01f, cardScale), 0.2f).OnComplete(() =>
               {
                   cardTf.DOScale(new Vector2(cardScale, cardScale), 0.2f);
               });
            cardTf.DOLocalRotate(new Vector3(cardTf.localRotation.x, -15, cardTf.localRotation.z), 0.2f).OnComplete(() =>
             {
                 cardTf.DOLocalRotate(Vector3.zero, 0.2f);
             });

            DOTween.Sequence()
                   .AppendInterval(0.2f)
                   .AppendCallback(() =>
                   {
                       card.setTextureWithCode(arrC[index]);
                       index++;
                   });
            cardTf.DOLocalMove(new Vector2(posCard.x, posCard0.y), 0.4f);
            //rotate += offsetRotation;
        }
        if (score != -1 && rate != 0 && arrC[0] != 0) SetEfftResult(player, score, rate);
    }
    private int HamTinhDiem(int diem)
    {
        if (diem >= 1000 && diem < 2000) diem = 11;
        else if (diem >= 2000 && diem < 3000) diem = 12;
        else if (diem >= 3000 && diem < 4000) diem = 13;
        else if (diem >= 4000 && diem < 5000) diem = 14;
        else if (diem >= 5000)
        {
            diem = diem % 5000;
            diem = diem % 10;
        }
        return diem;
    }
    private void SetEfftResult(Player player, int score, int rate, float timeDel = 0)
    {
        ///  let _this =this;

        PlayerViewGaoGea playerV = getPlayerView(player);
        int arr_index = player._indexDynamic;
        if (playerV.resultScore == null)
        {
            playerV.resultScore = Instantiate(bg_Score_Result, playerV.transform).GetComponent<ShowResultScore>();

            int positionY = -18;
            int positionX = arr_index == 0 ? 0 : (arr_index > 4 ? -95 : 95);
            if (arr_index == 0)
            {
                positionY = -10;
            }
            Vector2 posResultScore = playerV.transform.InverseTransformPoint(playerV.cardContainer.transform.TransformPoint(playerV.cardContainer.transform.GetChild(1).transform.localPosition));
            if (player == thisPlayer)
            {
                playerV.resultScore.transform.localPosition = new Vector2(158, -10);
            }
            playerV.resultScore.gameObject.SetActive(false);
        }
        if (timeDel == 0)
        {
            playerV.resultScore.gameObject.SetActive(true);
            playerV.resultScore.setResult(score, rate);
        }
        else
        {
            DOTween.Sequence()
                .AppendInterval(timeDel)
                .AppendCallback(() =>
                {
                    playerV.resultScore.gameObject.SetActive(true);
                    playerV.resultScore.setResult(score, rate);
                });
        }
    }
    private void viewIng(JArray listPlayer, JObject data)
    {
        if (buttonCheckToggle == null)
        {
            buttonCheckToggle = Instantiate(buttonCheckTogglePf, Table.transform).GetComponent<ToggleCheckShow>();

            buttonCheckToggle.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
        }
        buttonCheckToggle.gameObject.SetActive(false);
        if (getInt(data, "currentTurn") > 0)
        {
            Player player = getPlayerWithID(getInt(data, "currentTurn"));

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].playerView != null)
                {
                    players[i].playerView.setTurn(false, 0);

                }
            }
            player.setTurn(true, 15 - getInt(data, "timeLeave"));

        }
        Pot.setValue(getInt(data, "pot"), .5f);
        if (NodeBet == null)
        {
            NodeBet = Instantiate(NodeBetPf, Table.transform).GetComponent<ShowNodeBet>();

            NodeBet.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
        }
        NodeBet.gameObject.SetActive(false);
        for (int i = 0; i < listPlayer.Count; i++)
        {
            JObject dataPl = (JObject)listPlayer[i];
            Player player = getPlayerWithID(getInt(dataPl, "id"));
            int indexP = players.IndexOf(player);
            List<int> cards = getListInt(dataPl, "cards");
            int score = getInt(dataPl, "score");
            int typeCard = getInt(dataPl, "typeCard");
            bool isPlaying = getBool(dataPl, "isPlaying");
            player.isFold = !isPlaying;

            if (player == thisPlayer)
            {
                if (isPlaying == false)
                {
                    cards = new List<int> { 0, 0, 0 };
                }
                _scoreP = score;
                _rateP = typeCard;
                for (int j = 0; j < cards.Count; j++)
                {
                    ChiaCardPlayer(indexP, cards[j], j);
                }
                //SetEfftResult(player, _scoreP, _rateP);
            }
            else
            {
                for (int j = 0; j < cards.Count; j++)
                {
                    ChiaCardPlayer(indexP, 0, j);
                }
            }
            // chưa tới lượt = 0
            // check = 1
            // call = 2
            // raise = 3
            // fold = 4
            int lastPlay = getInt(dataPl, "lastPlay");
            int agBet = getInt(dataPl, "agBet");
            int ag = getInt(dataPl, "AG");
            switch (lastPlay)
            {
                case 1:
                    InstantiateBoxBet(player, agBet, "Check");
                    break;
                case 2:
                    if (ag == 0)
                    {
                        InstantiateBoxBet(player, agBet, "Allin");
                        player.playerView.setEffectAllIn(true);
                    }
                    else
                        InstantiateBoxBet(player, agBet, "Call");
                    break;
                case 3:
                    if (ag == 0)
                    {
                        InstantiateBoxBet(player, agBet, "Allin");
                        player.playerView.setEffectAllIn(true);
                    }
                    else
                        InstantiateBoxBet(player, agBet, "Raise");
                    break;
                case 4:
                    InstantiateBoxBet(player, agBet, "Fold");
                    DOTween.Sequence().AppendInterval(0.01f).AppendCallback(() => { FoldPlayer(player, player._indexDynamic); });

                    break;
                default:
                    break;
            }
            if (cards[0] != 0)
            {
                int id = getInt(dataPl, "id");
                OpenCardViewing(1, id, score, typeCard, cards);

            }
        }
        if (isRJ)
        {
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                isRJ = false;
            });
        }
    }
    private void FoldPlayer(Player player, int index)
    {
        player.isFold = true;
        player.playerView.setDark(true);
        for (int i = 0; i < player.vectorCard.Count; i++)
        {
            // player.vectorCard[i].setTextureWithCode(0);
            // player.vectorCard[i].setDark(true, spriteFrameMask);
            FoldDown(index, player.vectorCard[i], i * 0.1f);
        }
        PlayerViewGaoGea playerV = getPlayerView(player);
        int dyn_id = player._indexDynamic;
        if (playerV.resultScore == null)
        {
            playerV.resultScore = Instantiate(bg_Score_Result, playerV.transform).GetComponent<ShowResultScore>();

            playerV.resultScore.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BET);
            int positionY = dyn_id == 0 ? 15 : 10;
            int positionX = dyn_id == 0 ? 0 : (dyn_id > 4 ? 30 : -30);
            //Vector2 posResultScore = playerV.transform.InverseTransformPoint(Table.transform.TransformPoint(new Vector2(listPosCard[dyn_id].x + positionX, listPosCard[dyn_id].y + positionY)));
            Vector2 posResultScore = playerV.transform.InverseTransformPoint(playerV.cardContainer.transform.GetChild(1).transform.localPosition);
            if (player == thisPlayer)
            {
                playerV.resultScore.transform.localPosition = new Vector2(158, -10);
            }

        }
        ((PlayerViewGaoGea)player.playerView).resultScore.setResult(-1, 0);//
        if (player == thisPlayer)
        {
            //hien nut show
        }
    }
    private void FoldDown(int index, Card card, float delay)
    {

        Transform cardTF = card.transform;
        int sk1 = 0;
        int sk2 = 0;
        if (index <= 2)
        {
            sk1 = -15;
            sk2 = 15;
        }
        else
        {
            sk1 = 15;
            sk2 = -15;
        }
        float scale = index == 0 ? 0.55f : 0.35f;
        //float cardRotateZ = cardTF.localEulerAngles.z;
        Vector2 posFold = cardTF.parent.InverseTransformPoint(CardContainer.transform.TransformPoint(listPosCard[index]));
        float timeEff = 0.45f;
        float timeEff1 = 0.15f;
        if (stateGame == Globals.STATE_GAME.VIEWING)
        {
            timeEff = timeEff1 = 0.0f;
            delay = 0.0f;

        }
        cardTF.localPosition = new Vector2(cardTF.localPosition.x, Mathf.Floor(posFold.y + 30));
        //cardTF.DOLocalMove(new Vector2(cardTF.localPosition.x, Mathf.Floor(posFold.y + 30)), timeEff);
        DOTween.Sequence()
            .AppendInterval(delay)
            .Append(cardTF.DOScale(new Vector2(0, timeEff), timeEff1)).SetEase(Ease.OutCubic)
            .Join(cardTF.DOLocalMove(new Vector2(cardTF.localPosition.x, Mathf.Floor(posFold.y + 30)), timeEff))
            .AppendCallback(() =>
            {
                card.setTextureWithCode(0);
                card.setDark(true);
            })
            .Append(cardTF.DOScale(new Vector2(scale, scale), timeEff1).SetEase(Ease.OutCubic));
        DOTween.Sequence()
            .AppendInterval(delay)
            .Append(cardTF.DOLocalRotate(new Vector3(0, 0, sk1), timeEff1).SetEase(Ease.OutCubic))
            .AppendCallback(() =>
            {
                cardTF.localRotation = Quaternion.Euler(0, sk2, 0);
            })
            .Append(cardTF.DOLocalRotate(new Vector3(0, 0, 0), timeEff1).SetEase(Ease.OutCubic));
    }
    private void foldUp(int index, Card card, int code)
    {
        Transform cardTf = card.transform;
        int sk1 = 0;
        int sk2 = 0;
        if (index <= 2)
        {
            sk1 = -15;
            sk2 = 15;
        }
        else
        {
            sk1 = 15;
            sk2 = -15;
        }

        DOTween.Sequence()
             .Append(cardTf.DOScale(new Vector2(0, 0.65f), 0.2f))
             .AppendCallback(() =>
             {
                 card.setTextureWithCode(code);
             })
             .Append(cardTf.DOScale(new Vector2(0.45f, 0.45f), 0.2f));

        DOTween.Sequence()
            .Append(cardTf.DOLocalRotate(new Vector3(0, sk1, 0), 0.2f))
            .AppendCallback(() =>
            {
                cardTf.localRotation = Quaternion.Euler(cardTf.localRotation.x, sk2, cardTf.localRotation.z);
            })
            .Append(cardTf.DOLocalRotate(Vector3.zero, 0.2f));

    }

    public void InstantiateResultText(int indexP, string typeCard)
    {
        GameObject node = new GameObject();
        TextMeshProUGUI componentSprite = node.AddComponent<TextMeshProUGUI>();
        componentSprite.text = typeCard;// require("GameManager").getInstance().getTextConfig(typeCard);
        Player player = players[indexP];
        int length = player.vectorCard.Count;
        int dynamixIndex = player._indexDynamic;
        float cardScale = 0.5f;
        //   if(indexP == 0) cardScale = 0.7;
        float widthCardNode = 149 * cardScale / 2;
        Vector2 cardBegin = listPosCard[dynamixIndex];
        node.transform.SetParent(Table.transform);
        node.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
        if (dynamixIndex > 2)
        {
            node.transform.localPosition = new Vector2((cardBegin.x - (((length - 2) / 2) * widthCardNode)), cardBegin.y - ((200 * cardScale) / 4));
        }
        else
        {
            node.transform.localPosition = new Vector2((cardBegin.x + (((length - 2) / 2) * widthCardNode)), cardBegin.y - ((200 * cardScale) / 4));
        }
        //node.runAction(cc.sequence(cc.delayTime(4), cc.removeSelf()));
        DOTween.Sequence()
            .AppendInterval(4.0f)
            .AppendCallback(() =>
            {
                Destroy(node);
            });
    }
    public void handleFinish(JObject jData)
    {
        // {
        // 	"evt": "finishgame",
        // 	"idwinner": 5563,
        // 	"data": [{"id":700,"ag":10808,"agWin":0,"agLost":0,"cards":[13,5,4],"typeCard":4,"score":2},
        // 			 {"id":968,"ag":102550,"agWin":0,"agLost":0,"cards":[24,7,3],"typeCard":5,"score":2},
        // 			 {"id":5563,"ag":340771,"agWin":294,"agLost":0,"cards":[38,34,19],"typeCard":5,"score":6}]
        // }
        isRJ = false;
        JArray data = getJArray(jData, "data");
        buttonCheckToggle.gameObject.SetActive(false);
        if (NodeBet != null) NodeBet.gameObject.SetActive(false);
        // Node_Btn_Ac_Card.active = false;
        Pot.setValue(0, .5f);
        for (int i = 0; i < data.Count; i++)
        {
            JObject dataPl = (JObject)(data[i]);
            int idPl = getInt(dataPl, "id");
            Player pl = getPlayerWithID(idPl);
            PlayerViewGaoGea playerV = getPlayerView(pl);
            int chip = getInt(dataPl, "agWin");
            int totalChip = getInt(dataPl, "ag");
            int score = getInt(dataPl, "score");
            int rate = getInt(dataPl, "typeCard"); // 0 //
            List<int> cards = getListInt(dataPl, "cards");
            if (pl != null)
            {
                //Globals.Logging.Log("openCardFinish:" + cards[0] + "," + cards[1] + "," + cards[2]);
                openCardPlayer(idPl, cards, score, rate);
                DOTween.Sequence()
                    .AppendInterval(2.0f)
                    .AppendCallback(() =>
                    {
                        int dyn_id = pl._indexDynamic;
                        if (playerV.resultScore == null)
                        {
                            playerV.resultScore = Instantiate(bg_Score_Result, playerV.transform).GetComponent<ShowResultScore>();

                            playerV.resultScore.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BET);
                            int positionY = dyn_id == 0 ? 15 : 10;
                            int positionX = dyn_id == 0 ? 0 : (dyn_id > 4 ? 30 : -30);
                            Vector2 posResultScore = playerV.transform.InverseTransformPoint(playerV.cardContainer.transform.GetChild(1).transform.localPosition);//TransformPoint(new Vector2(listPosCard[dyn_id].x + positionX, listPosCard[dyn_id].y + positionY)));
                            if (pl == thisPlayer)
                            {
                                playerV.resultScore.transform.localPosition = new Vector2(158, -10);
                            }
                        }
                        playerV.resultScore.gameObject.SetActive(cards[0] != 0);
                        if (chip > 0)
                        {
                            playerV.setEffectWin("win_thai", false);
                            if (pl == thisPlayer)
                            {
                                playSound(Globals.SOUND_GAME.WIN);
                            }
                        }
                        else
                        {
                            playerV.setEffectLose(false);
                            //playSound(Globals.SOUND_GAME.GET_CHIP);
                            if (pl == thisPlayer)
                            {
                                playSound(Globals.SOUND_GAME.LOSE);
                            }
                        }
                    })
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        pl.ag = totalChip;
                        pl.setAg();
                        if (chip > 0)
                        {
                            playSound(Globals.SOUND_GAME.THROW_CHIP);
                            pl.playerView.effectFlyMoney(chip, 30);
                        }
                    });
            }

        }
        DOTween.Sequence()
                  .AppendInterval(6.0f)
                  .AppendCallback(() =>
                  {
                      stateGame = Globals.STATE_GAME.WAITING;
                      resetViewGame(data);
                      HandleGame.nextEvt();
                  });
    }

    private void resetViewGame(JArray data = null)
    {
        Globals.Logging.Log("resetViewGame:" + data);
        if (buttonCheckToggle != null) buttonCheckToggle.resetStatusToggle();
        isShow = false;
        isRJ = false;

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            player.setDark(false);
            PlayerViewGaoGea playerV = getPlayerView(player);
            player.isFold = false;
            player.vectorCard.Clear();
            if (playerV.boxbet != null)
            {
                playerV.boxbet.gameObject.SetActive(false);
                playerV.boxbet.chip = 0;
            }
            if (playerV.resultScore != null)
            {
                playerV.resultScore.gameObject.SetActive(false);
            }
            playerV.animResult.gameObject.SetActive(false);
            playerV.isShowCardFold = false;
        }
        handleCardsFinish();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerView.setEffectAllIn(false);
            if (players[i].isFold)
            {
                players[i].isFold = false;
                players[i].playerView.setDark(false);
            }
        };
        checkAutoExit();
    }
    private void handleCardsFinish()
    {
        recallCards();
    }
    private void recallCards()
    {
        Globals.Logging.Log("recallCards:" + playerCards.FindAll(plCards => plCards.Count > 0).Count);
        if (playerCards.FindAll(plCards => plCards.Count > 0).Count > 0)
        {
            playSound(Globals.SOUND_GAME.DISPATCH_CARD);
        }
        float delay = 0;
        int zView = 0;

        for (int i = 0; i < playerCards.Count; i++)
        {
            int len = playerCards[i].Count;
            List<GameObject> listCard = playerCards[i];
            while (playerCards[i].Count > 0)
            {
                GameObject card = playerCards[i][0];

                playerCards[i].Remove(card);
                moveCardsFinish(card, delay, zView);
                zView++;
                delay += 0.1f;
            }
        }


        for (int i = 0; i < players.Count; i++)
        {
            players[i].vectorCard.Clear();
        }

    }
    private void moveCardsFinish(GameObject card, float delay, int zView)
    {
        DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() =>
            {
                Vector2 posRemove = card.transform.parent.transform.InverseTransformPoint(cardStack.transform.position);
                card.GetComponent<Card>().setDark(false);
                card.transform.DOLocalMove(posRemove, 0.4f).SetEase(Ease.OutCubic);
                card.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_CARD + zView);
                card.transform.DOScale(new Vector2(0.04f, 0.4f), 0.15f).OnComplete(() =>
                {
                    card.transform.DOScale(new Vector2(0.4f, 0.4f), 0.15f);
                });

                DOTween.Sequence()
                .Append(card.transform.DOLocalRotate(new Vector3(0, -15f, 0), 0.15f))
                .AppendCallback(() =>
                {
                    card.transform.localRotation = Quaternion.Euler(0, -15, 0);
                })
                .Append(card.transform.DOLocalRotate(Vector3.zero, 0.15f));

                DOTween.Sequence()
                .AppendInterval(0.15f)
                .AppendCallback(() =>
                {
                    card.GetComponent<Card>().setTextureWithCode(0);
                })
                .AppendInterval(0.45f)
                .AppendCallback(() =>
                {
                    removerCard(card.GetComponent<Card>());
                });
            });

    }
    public void handleCab(JObject data, string status = "")
    {
        /*
        {"evt":"show","id":5442,"cards":[11,31,2],"agBet":0,"agCurrent":0,"pot":0,"agRaise":0}
        */
        if (status == "Show")
        {
            //
            PlayerViewGaoGea playerV = getPlayerView(getPlayerWithID((getInt(data, "id"))));
            if (playerV == thisPlayer.playerView)
            {
                openCardPlayer(getInt(data, "id"), getListInt(data, "cards"), _scoreP, _rateP);

            }

            playerV.isShowCardFold = true;
            return;
        }
        if (NodeChangeTime != null && NodeChangeTime.gameObject != null)
        {
            Destroy(NodeChangeTime.gameObject);
            NodeChangeTime = null;
        }

        if (status == "Raise" || status == "Call" || status == "Allin")
        {
            Pot.setValue(getInt(data, "pot"), .5f);
            if (status == "Allin")
            {
                playSound(Globals.SOUND_GAME.ALL_IN);
            }
            else
            {
                playSound(Globals.SOUND_GAME.BET);
            }

        }
        setCurentTurn(data, status);
        if (!thisPlayer.is_ready || isAllIn)
        {
            buttonCheckToggle.gameObject.SetActive(false);
        }
        else if (thisPlayer.isFold)
        {
            buttonCheckToggle.gameObject.SetActive(true);
            buttonCheckToggle.setStatus(true, isShow);
        }
        else
        {
            int valueToggleCheck = 0;
            if (getPlayerView(players[0]).boxbet != null)
            {
                valueToggleCheck = getPlayerView(players[0]).boxbet.chip;
            }
            Player player = getPlayerWithID(getInt(data, "id"));
            PlayerViewGaoGea playerV = getPlayerView(player);
            int valueBig = playerV.boxbet.chip;
            foreach (Player pl in players)
            {
                BoxBetShow boxbet = getPlayerView(pl).boxbet;
                if (boxbet != null)
                {
                    if (boxbet.chip > playerV.boxbet.chip)
                    {
                        valueBig = boxbet.chip;
                    }
                }
            }
            buttonCheckToggle.setInfo(valueBig, valueToggleCheck, thisPlayer.ag);
            if (buttonCheckToggle != null) buttonCheckToggle.gameObject.SetActive(true);
        }
        Player pls = getPlayerWithID(getInt(data, "id"));
        PlayerViewGaoGea plV = getPlayerView(pls);
        BoxBetShow bb = plV.boxbet;
        if (bb != null)
        {
            bb.transform.localPosition = listPosBoxBet[pls._indexDynamic];
        }

    }
    public void setTurn(JObject data)
    {

        Player player = getPlayerWithID(getInt(data, "pid"));

        for (int i = 0; i < players.Count; i++)
        {
            players[i].setTurn(false, 0);
        }
        // {"evt":"getNext","pid":560,"time":15,"agToCall":0,"allowCheck":true}
        if (player == thisPlayer)
        {
            buttonCheckToggle.allowCheck = getBool(data, "allowCheck");
            if (!buttonCheckToggle.readInfoToggle())
            {
                player.setTurn(true, getInt(data, "time"));
                if (NodeBet == null)
                {
                    NodeBet = Instantiate(NodeBetPf, Table.transform).GetComponent<ShowNodeBet>();

                    NodeBet.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
                }
                NodeBet.gameObject.SetActive(true);
                //lbChangeCard.SetActive(false);
                //bg_arrow_swap.SetActive(false);
                NodeBet.AutoBetIfClickRaise(getInt(data, "time"));
                if (buttonCheckToggle != null) buttonCheckToggle.gameObject.SetActive(false);
                int agToCall = getInt(data, "agToCall");
                if (agToCall != 0)
                {
                    NodeBet.setInfoBtn("Call", agToCall);
                    //Globals.Logging.Log("setValueInfo:player ag==" + player.ag);
                    NodeBet.setValueInfo(player.ag, agToCall, agTable, Pot.getValue());

                }
                else
                {
                    NodeBet.setInfoBtn("Check");
                    //Globals.Logging.Log("setValueInfo:player ag==" + player.ag);
                    NodeBet.setValueInfo(player.ag, agToCall, agTable, Pot.getValue());
                }

            }
        }
        else
        {
            player.setTurn(true, getInt(data, "time"));

            if (NodeBet == null) return;
            NodeBet.SetFalseIsCountDown();
            NodeBet.gameObject.SetActive(false);
        }
    }

    public void setCurentTurn(JObject data, string status)
    {
        /*
        {"evt":"check","id":5442,"agBet":0,"agCurrent":0,"pot":0,"agRaise":0}
        */
        if (status == "")
        {
            return;
        }
        if (NodeBet != null) NodeBet.gameObject.SetActive(false);
        Player player = getPlayerWithID(getInt(data, "id"));
        if (status == "Allin")
            player.playerView.setEffectAllIn(true);
        int indexDynamic = player._indexDynamic;
        player.setTurn(false, 0);
        InstantiateBoxBet(player, getInt(data, "agBet"), status);
        if (status == "Fold")
        {
            playSound(Globals.SOUND_GAME.FOLD);
            FoldPlayer(player, indexDynamic);
        }
        int valueBoxBex = getInt(data, "agBet");
        int agRaise = getInt(data, "agRaise");
        //player.ag -= valueBoxBex;
        //player.setAg();
        player.playerView.effectFlyMoney(-valueBoxBex, 30);
        int numberChip = 0;
        if (valueBoxBex <= 0)
        {

        }
        else if (valueBoxBex <= agTable)
        {
            numberChip = 1;
        }
        else if (valueBoxBex <= 2 * agTable)
        {
            numberChip = 2;
        }
        else if (valueBoxBex <= 3 * agTable)
        {
            numberChip = 3;
        }
        else
        {
            numberChip = 4;
        }
        Vector2 target = listPosBoxBet[indexDynamic];// listPosView[indexDynamic].getChildByName("box_bet_bg").position;

        Vector2 vPos = listPosView[indexDynamic];
        for (int i = 0; i < numberChip; i++)
        {
            Vector2 vTemp = new Vector2(target.x, target.y);
            if (chipEffectPool.Count < 1)
            {
                GameObject go = Instantiate(chipEffect, Table.transform);

                chipEffectPool.Add(go);
            }
            GameObject temp = chipEffectPool[0];
            chipEffectPool.RemoveAt(0);
            temp.transform.localPosition = vPos;
            temp.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
            //let tempAc1 = cc.fadeOut(0.8).easing(cc.easeIn(6))

            TweenCallback tempAc1 = (() =>
              {
                  temp.GetComponent<CanvasGroup>().DOFade(0, 0.8f).SetEase(Ease.InSine);
              });

            TweenCallback tempAc2 = () =>
             {
                 temp.transform.DOLocalMove(vTemp, 0.5f).SetEase(Ease.OutSine);
             };
            TweenCallback temoAc3 = () =>
            {
                DOTween.Sequence().AppendCallback(tempAc1);
                DOTween.Sequence().AppendCallback(tempAc2);
            };
            DOTween.Sequence()
                .AppendInterval(i * 0.01f)
                .AppendCallback(temoAc3)
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    temp.transform.SetParent(null);
                    chipEffectPool.Add(temp);

                });
        }
        if (status == "Raise" || status == "Call" || status == "Allin")
        {
            if (player == thisPlayer)
            {
                Globals.User.userMain.AG = getInt(data, "agCurrent");
            }
            player.ag = getInt(data, "agCurrent");
            player.setAg();
        }
    }

    private void onclickCancel()
    {
        SoundManager.instance.soundClick();
        thisPlayer.vectorCard[3].setDark(false);
        SocketSend.sendChangeCard(false);
        //lbChangeCard.SetActive(false);
        //bg_arrow_swap.SetActive(false);
    }
    private PlayerViewGaoGea getPlayerView(Player player)
    {
        return (PlayerViewGaoGea)player.playerView;
    }
    private void InstantiateBoxBet(Player player, int chip, string status)
    {
        int indexDynamic = player._indexDynamic;
        PlayerViewGaoGea playerV = getPlayerView(player);
        if (playerV.boxbet == null)
        {
            playerV.boxbet = Instantiate(BoxBetPf, Table.transform).GetComponent<BoxBetShow>();

            playerV.boxbet.transform.SetParent(Table.transform);
            playerV.boxbet.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_CARD);
            playerV.boxbet.transform.localPosition = listPosBoxBet[indexDynamic];
        }

        if (indexDynamic == 0 && status == "Allin")
        {
            isAllIn = true;
        }
        playerV.boxbet.gameObject.SetActive(true);
        playerV.boxbet.setInfo(status, indexDynamic, chip);
    }
    private void ChiaCardPlayer(int indexP, int cardCode, int indexCard)
    {
        Card cardTemp = getCard(CardContainer.transform, 0.5f);
        Transform cardTempTf = cardTemp.transform;
        cardTempTf.localPosition = cardStack.transform.localPosition;
        int dynamicIndex = players[indexP]._indexDynamic;
        players[indexP].vectorCard.Add(cardTemp);
        playerCards[dynamicIndex].Add(cardTemp.gameObject);
        int currentCard = playerCards[dynamicIndex].Count - 1;
        int length = currentCard + 1;
        float timeEff = stateGame == Globals.STATE_GAME.VIEWING ? 0.0f : 0.2f;
        float timeEffMove = stateGame == Globals.STATE_GAME.VIEWING ? 0.0f : 0.6f;
        if (isRJ)
        {
            timeEff = timeEffMove = 0.0f;
        }
        if (timeEff != 0)
        {
            playSound(Globals.SOUND_GAME.CARD_FLIP_1);
        }
        if (dynamicIndex == 0)
        {
            if (timeEffMove != 0)
            {
                cardTempTf.DOScale(new Vector2(0.05f, 0.7f), timeEff).OnComplete(() =>
                {
                    cardTempTf.DOScale(new Vector2(0.7f, 0.7f), timeEff);

                });
                cardTempTf.DOLocalRotate(new Vector3(0, 15, 0), timeEff).OnComplete(() =>
                {
                    cardTempTf.localRotation = Quaternion.Euler(0, -15.0f, 0);
                    cardTempTf.DOLocalRotate(Vector3.zero, timeEff);
                });
                cardTempTf.DOLocalMove(new Vector2(0, 50), timeEffMove);
                DOTween.Sequence().AppendInterval(timeEff).AppendCallback(() =>
                {
                    cardTemp.setTextureWithCode(cardCode);
                })
                 .AppendInterval(timeEff * 2)
                 .AppendCallback(() =>
                 {
                     cardTempTf.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_BUTTON);
                     cardTempTf.DOScale(new Vector2(0.55f, 0.55f), timeEff * 2);
                     handleCardPosition(players[indexP], dynamicIndex, length, 0.55f);

                     if (indexCard == 2)
                     {
                         SetEfftResult(thisPlayer, _scoreP, _rateP, 0.6f);
                     }
                 });
            }
            else
            {
                cardTempTf.localScale = new Vector2(0.55f, 0.55f);
                cardTemp.setTextureWithCode(cardCode);
                handleCardPosition(players[indexP], dynamicIndex, length, 0.55f);
                if (indexCard == 2 && cardCode != 0)
                {
                    SetEfftResult(thisPlayer, _scoreP, _rateP, 0.6f);
                }

            }

        }
        else
        {
            handleCardPosition(players[indexP], dynamicIndex, playerCards[dynamicIndex].Count, 0.35f);
        }
    }
    private void handleCardPosition(Player player, int indexD, int indexC, float scale = 0.45f)
    {
        float timeEffect = 0.6f;
        PlayerViewGaoGea playerV = getPlayerView(player);
        if (stateGame == Globals.STATE_GAME.VIEWING || isRJ)
        {
            timeEffect = 0;
        }
        for (int i = 0; i < indexC; i++)
        {
            GameObject card = playerCards[indexD][i];
            JObject data = getCardPositionNAngle(indexD, i, scale, player);

            Vector2 posReal = new Vector2((float)data["posX"], (float)data["posY"]);
            Vector2 posOut = CardContainer.transform.TransformPoint(posReal);
            Vector2 posIn = playerV.cardContainer.transform.InverseTransformPoint(posOut);
            card.transform.SetParent(playerV.cardContainer.transform);
            card.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_CARD);
            card.transform.DOLocalMove(posIn, timeEffect).SetEase(Ease.OutCubic);
            card.transform.DOLocalRotate(new Vector3(0, 0, (float)data["angle"]), timeEffect).SetEase(Ease.OutCubic);
            card.transform.DOScale(new Vector2(scale, scale), timeEffect).SetEase(Ease.OutCubic);
        }
    }
    private JObject getCardPositionNAngle(int indexD, int indexC, float scale = 0.4f, Player player = null)
    {
        List<GameObject> Cstack = playerCards[indexD];
        float tile = scale / 0.4f;
        float angle = 20 * indexC - 10 * (Cstack.Count - 1);
        //float offset = indexD == 0 ? 30 * indexC - 15 * (Cstack.Count - 1) : 30 * indexC - 7 * (Cstack.Count - 1);
        float offset = 25 * indexC - 15 * (Cstack.Count - 1);
        offset = offset * tile;
        Vector2 hand = getHandPosition(indexD);
        int offY = 0;
        if (Cstack.Count > 2 && player.isFold == false)
        {
            offY = 6;
        }
        //offY = 0;
        //angle = 0;
        Vector2 pos;
        if (indexC == 1)
        {
            pos = new Vector2(hand.x + offset, hand.y);
        }
        else
        {
            pos = new Vector2(hand.x + offset, hand.y - offY);
        }

        JObject result = new JObject();
        result["angle"] = -angle;
        result["posX"] = pos.x;
        result["posY"] = pos.y;
        return result;
    }
    private Vector2 getHandPosition(int index)
    {
        return new Vector2(listPosCard[index].x, listPosCard[index].y + 30);
    }

    public void handleStartGame(JObject data)
    {
        showTextWaiting(false);
        playSound(Globals.SOUND_GAME.START_GAME);
        stateGame = Globals.STATE_GAME.PLAYING;
        DOTween.Sequence()
           .AppendInterval(2.0f)
           .AppendCallback(() =>
           {
               HandleGame.nextEvt();
           });
        DealerInGame.dispatchCard();
        DOTween.Sequence().SetDelay(2.0f).AppendCallback(() =>
        {
            if (buttonCheckToggle != null)
            {
                if (NodeBet == null || (NodeBet != null && NodeBet.gameObject.activeSelf == false))
                {
                    buttonCheckToggle.gameObject.SetActive(true);
                    int valueToggleCheck = 0;
                    if (getPlayerView(thisPlayer).boxbet != null)
                    {
                        valueToggleCheck = getPlayerView(thisPlayer).boxbet.chip;
                        getPlayerView(thisPlayer).boxbet.transform.localPosition = listPosBoxBet[thisPlayer._indexDynamic];
                    }
                    buttonCheckToggle.resetStatusToggle();
                    buttonCheckToggle.setInfo(0, valueToggleCheck, thisPlayer.ag);
                }
            }
        });

        Pot.setValue(getInt(data, "pot"), .5f);
        stateGame = Globals.STATE_GAME.PLAYING;
        List<int> arrCodes = getListInt(data, "cards");
        _scoreP = getInt(data, "score");
        _rateP = getInt(data, "typeCard");
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            player.is_ready = true;
            int indexP = i;
            if (player.id == getInt(data, "idDealer"))
            {

                bool isLeft = false;
                bool isUp = false;
                if (indexP == 0)
                {
                    isUp = true;
                }
                else if (indexP > 4)
                {
                    isLeft = true;
                }
                player.playerView.showDealer(true, isLeft, isUp);
            }
            else
            {
                player.playerView.showDealer(false);
            }
            player.is_ready = true;
            int indexD = player._indexDynamic;
            int agPutPot = getInt(data, "agPutPot");
            player.ag -= agPutPot;
            player.setAg();
            player.playerView.effectFlyMoney(-agPutPot, 30);
            EffectMoneyChange(-agPutPot, player.ag, player.playerView.txtMoney);
            player.vectorCard.Clear();
            int index = 0;
            for (int j = 0; j < 3; j++)
            {
                int codeC = indexD == 0 ? arrCodes[j] : 0;
                DOTween.Sequence()
                    .AppendInterval(j * 0.6f)
                    .AppendCallback(() =>
                    {
                        ChiaCardPlayer(indexP, codeC, index);
                        index++;

                    });
            }
        }
    }
    public override void onClickRule()
    {
        nodeRule.GetComponent<BaseView>().show();
    }
    public void onCloseRule()
    {
        nodeRule.GetComponent<BaseView>().onClickClose();
    }
    private void EffectMoneyChange(int amountChange, long _valueSet, TextMeshProUGUI label, bool format = false, int speed = 1, int duration = 1)
    {
        DOTween.Sequence()
            .Append(label.transform.DOScale(new Vector2(1.2f, 1.2f), 0.2f))
            .AppendCallback(() =>
            {
                var number = _valueSet;
                number += amountChange;
                label.text = Globals.Config.FormatNumber(number);
            })
            .Append(label.transform.DOScale(Vector2.one, 0.2f));

    }
    public override PlayerView createPlayerView()
    {
        var plView = Instantiate(playerViewPrefab, PlayerContainer.transform);//.GetComponent<PlayerView>();

        plView.transform.SetSiblingIndex((int)Globals.ZODER_VIEW.PLAYER);
        return plView.GetComponent<PlayerViewGaoGea>();
    }
    private void showTextWaiting(bool state)
    {
        if (!state)
        {
            lbStateGame.gameObject.SetActive(false);
        }
        else
        {
            lbStateGame.text = "";
            lbStateGame.gameObject.SetActive(true);
            //lbStateGame.DOText("Watting for the next round...", 0.3f).SetEase(Ease.OutSine);//hien
            UIManager.instance.DOTextTmp(lbStateGame, Globals.Config.getTextConfig("txt_view_table"));

        }
    }


    public void onClickDemo()
    {
        for (var i = 1; i < listBtnInvite.Count; i++)
        {
            var pl = createPlayerView();
            pl.transform.localPosition = listBtnInvite[i].transform.localPosition;
            pl.updateItemVip(1, 1);

            var boxbet = Instantiate(BoxBetPf, Table.transform).GetComponent<BoxBetShow>();

            boxbet.transform.SetParent(Table.transform);
            boxbet.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_CARD);
            boxbet.transform.localPosition = listPosBoxBet[i];
        }
    }

}
