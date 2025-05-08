using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using Globals;

public class DragonTigerView : GameView
{
    [SerializeField] public GameObject PlayerContainer;
    [SerializeField] public ChipDragonTiger chipPref;
    [SerializeField] public GameObject chipContainer;
    [SerializeField] public GameObject btnPots;
    //[SerializeField] public GameObject btnPotsNormal;
    [SerializeField] public GameObject btnBets;
    [SerializeField] public List<Button> listChipBets = new List<Button>();
    [SerializeField] public List<SkeletonGraphic> listAniStart = new List<SkeletonGraphic>();
    [SerializeField] public GameObject aniWin;
    [SerializeField] public TextMeshProUGUI lbWinChip;
    [SerializeField] public GameObject aniLose;
    [SerializeField] public TextMeshProUGUI lbLoseChip;
    [SerializeField] public GameObject prefab_popup_player;
    [SerializeField] public GameObject clock;
    [SerializeField] public GameObject buttonHistory;
    [SerializeField] public GameObject buttonMenu;
    [SerializeField] public DealerInGameView dealerInGame;
    [SerializeField] public TextMeshProUGUI lbTimeRemain;
    [SerializeField] public GameObject dots;
    [SerializeField] public List<GameObject> listDots = new List<GameObject>();
    [SerializeField] private Transform parentMiniHis;
    [SerializeField] GameObject nodePlayerHide;
    [SerializeField] TextMeshProUGUI lbPlayerHide;
    [SerializeField] List<GameObject> listPot = new List<GameObject>();
    [SerializeField] List<GameObject> listPotNormal = new List<GameObject>();
    [SerializeField] Card cardDraPref;
    [SerializeField] Card cardTigPref;
    [SerializeField] Image bkgAniStart;
    [SerializeField] GameObject popupHistoryPrefab;
    [SerializeField] GameObject roundDot;
    [SerializeField] GameObject imageVS;
    [SerializeField] TextMeshProUGUI txtWaiting;

    private HistoryDragonTiger popupHistory;

    public List<Player> listPlayerHide = new List<Player>();

    [HideInInspector] private long betValue = 0;
    private List<long> listValueChipBets = new List<long>();
    private List<ChipDragonTiger> chipPoolTG = new List<ChipDragonTiger>();
    private long[] listBet = { 0, 0, 0, 0, 0, 0, 0 };
    private long[] listMyBet = { 0, 0, 0, 0, 0, 0, 0 };
    private long[] listLastBet = { 0, 0, 0, 0, 0, 0, 0 };
    private int chipBetColorInx = 0;
    private int myChipBetColor = 0;
    private List<ChipDragonTiger> listChipInTable = new List<ChipDragonTiger>();
    private List<ChipDragonTiger> saveListChipInTable = new List<ChipDragonTiger>();
    private List<int> listWinResult = new List<int>();
    private List<ChipDragonTiger> listLoseResult = new List<ChipDragonTiger>();
    private List<ChipDragonTiger> listChipsPay = new List<ChipDragonTiger>();
    private List<int> listDotsAfterUpdate = new List<int>();
    private JObject finishDT;
    private JObject saveDT;
    private bool checkBeted = false;
    private int saveNumDr = 0;
    private int saveNumTi = 0;

    protected NodePlayerDragonTiger listPlayer = null;
    private List<int> listSaveHistory = new List<int>();


    protected override void Start()
    {
        base.Start();
    }

    public void resetGame()
    {
        txtWaiting.gameObject.SetActive(true);
        checkBeted = false;
        Array.Copy(listMyBet, listLastBet, 7);
        setStatusButtonsBet(true, false);
        for (int i = 0; i < listChipInTable.Count(); i++)
        {
            Destroy(listChipInTable[i].gameObject);
        }
        listChipInTable.Clear();

        listBet = listBet.Select(x => x * 0).ToArray();
        listMyBet = listMyBet.Select(x => x * 0).ToArray();

        for (int i = 0; i < 7; i++)
        {
            listPot[i].transform.Find("bet").GetComponentInChildren<TextMeshProUGUI>().text = "";
            listPot[i].transform.Find("mybet").GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        btnPots.SetActive(true);
        //btnPotsNormal.SetActive(false);
        listWinResult.Clear();
        checkAutoExit();
        aniWin.SetActive(false);
        aniLose.SetActive(false);
        lbWinChip.text = "";
        lbLoseChip.text = "";

        setAnimDTNormal();
        setAniVsHide();
        cardDraPref.transform.Find("animTwinkle").gameObject.SetActive(false);
        cardDraPref.transform.Find("blink").gameObject.SetActive(false);
        cardDraPref.transform.Find("mask").gameObject.SetActive(false);
        cardDraPref.transform.Find("vetcao").gameObject.SetActive(false);

        cardTigPref.transform.Find("animTwinkle").gameObject.SetActive(false);
        cardTigPref.transform.Find("blink").gameObject.SetActive(false);
        cardTigPref.transform.Find("mask").gameObject.SetActive(false);
        cardTigPref.transform.Find("vetcao").gameObject.SetActive(false);

        disablePot(false);

        stateGame = Globals.STATE_GAME.WAITING;
    }

    protected override void updatePositionPlayerView()
    {
        listPlayerHide = new List<Player>();
        int size = 6;
        for (int i = 0; i < players.Count; i++)
        {
            if (thisPlayer == players[i])
            {
                players.RemoveAt(i);
            }
        }
        players.Insert(0, thisPlayer);
        for (var i = 0; i < players.Count; i++)
        {
            var index = getDynamicIndex(getIndexOf(players[i]));
            players[i]._indexDynamic = index;
            if (index >= size)
            {
                listPlayerHide.Add(players[i]);
                players[i].playerView.gameObject.SetActive(false);
                players[i].playerView.transform.localPosition = nodePlayerHide.transform.localPosition;
            }
            else
            {
                players[i].playerView.transform.localPosition = listPosView[index];
                players[i].playerView.gameObject.SetActive(true);
            }
            players[i].updateItemVip(players[i].vip);
        }
        lbPlayerHide.text = listPlayerHide.Count.ToString() + '+';
        nodePlayerHide.SetActive(listPlayerHide.Count >= 0);
    }

    public void handleStartGame(JObject dataStart)
    {
        showAnimDTstart();
        txtWaiting.gameObject.SetActive(false);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
    }

    private void showAnimDTstart()
    {
        bkgAniStart.gameObject.SetActive(true);
        bkgAniStart.transform.SetAsLastSibling();
        bkgAniStart.enabled = true;

        for (int i = 0; i < listAniStart.Count - 1; i++)
        {
            listAniStart[i].gameObject.SetActive(true);
            listAniStart[i].AnimationState.SetAnimation(0, "challenge", false);
            listAniStart[i].Initialize(true);
            listAniStart[i].AnimationState.Complete += delegate
            {
                listAniStart[i].gameObject.SetActive(false);
            };
            listAniStart[i].gameObject.transform.localScale = new Vector2(1f, 1f);
        }

        listAniStart[0].gameObject.transform.localPosition = new Vector2(-14, -6);
        listAniStart[1].gameObject.transform.localPosition = new Vector2(-27, -4);

        SkeletonGraphic aniVS = listAniStart[2];
        aniVS.gameObject.SetActive(true);
        aniVS.AnimationState.SetAnimation(0, "animation", false);
        aniVS.Initialize(true);
        aniVS.gameObject.transform.localScale = new Vector2(1.2f, 1.2f);
        aniVS.gameObject.transform.localPosition = new Vector2(-7, -2);
    }

    private void moveAnimDTNormal()
    {
        bkgAniStart.enabled = false;
        //popupHistory.gameObject.transform.SetAsLastSibling();

        for (int i = 0; i < listAniStart.Count - 1; i++)
        {
            listAniStart[i].gameObject.SetActive(true);
            listAniStart[i].Initialize(true);
            listAniStart[i].AnimationState.SetAnimation(0, "normal", true);

            listAniStart[i].gameObject.transform.DOScale(new Vector2(0.7f, 0.7f), 0.5f);
        }
        listAniStart[2].gameObject.SetActive(false);
        listAniStart[0].gameObject.transform.DOLocalMove(new Vector2(-140, 207), 0.5f);
        listAniStart[1].gameObject.transform.DOLocalMove(new Vector2(360, 207), 0.5f);
    }

    private void setAnimDTNormal()
    {
        bkgAniStart.gameObject.SetActive(true);
        bkgAniStart.enabled = false;
        for (int i = 0; i < listAniStart.Count - 1; i++)
        {
            listAniStart[i].gameObject.SetActive(true);
            listAniStart[i].Initialize(true);
            listAniStart[i].AnimationState.SetAnimation(0, "normal", true);
            listAniStart[i].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
        }
        listAniStart[0].gameObject.transform.localPosition = new Vector2(-140, 207);
        listAniStart[1].gameObject.transform.localPosition = new Vector2(360, 207);
        listAniStart[2].gameObject.SetActive(false);
    }
    public void handleLc(JObject dataLc)
    {
        moveAnimDTNormal();
        clock.gameObject.SetActive(false);
        btnBets.SetActive(false);
        btnPots.SetActive(true);
        //btnPotsNormal.SetActive(false);

        JObject data = JObject.Parse((string)dataLc["data"]);

        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() =>
        {
            int idCardDr = getInt(data, "IdCardDr");
            int idCardTi = getInt(data, "IdCardTi");

            cardDraPref.setTextureWithCode(0);
            cardTigPref.setTextureWithCode(0);

            cardDraPref.gameObject.transform.localScale = new Vector2(0.4f, 0.4f);
            cardTigPref.gameObject.transform.localScale = new Vector2(0.4f, 0.4f);

            cardDraPref.gameObject.transform.localPosition = new Vector2(-6, -1);
            cardTigPref.gameObject.transform.localPosition = new Vector2(135, -1);

            dealerInGame.ani_dealer.Initialize(true);
            dealerInGame.ani_dealer.timeScale = 0.8f;
            dealerInGame.ani_dealer.AnimationState.SetAnimation(0, "chiabai", true);
            DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
            {
                dealerInGame.ani_dealer.timeScale = 1.0f;
                dealerInGame.ani_dealer.AnimationState.SetAnimation(0, "normal", true);
            });

            cardDraPref.gameObject.SetActive(true);
            SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.DISPATCH_CARD);

            cardTigPref.gameObject.SetActive(false);
            DOTween.Sequence().AppendInterval(0.3f).AppendCallback(() =>
            {
                cardTigPref.gameObject.SetActive(true);
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.DISPATCH_CARD);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                setAniVs();
            })
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                clock.gameObject.SetActive(true);

                int timeStart = getInt(data, "time") / 1000;
                timeStart -= 2;//ko tru di thoi gian dien anim start voi chia bai ak.
                lbTimeRemain.text = timeStart.ToString();
                DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
                {
                    timeStart--;
                    if (timeStart > 5)
                    {
                        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
                    }
                    else
                    {
                        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_HURRY);
                    }
                    lbTimeRemain.text = timeStart + "";
                }).SetLoops(timeStart).SetId("SoundClock");
            });
        })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                btnBets.SetActive(true);
                disablePot(true);
            });
        setStatusButtonsBet(!checkBeted, checkBeted);
    }

    public void setAniVs()
    {
        imageVS.SetActive(true);
        imageVS.transform.DOLocalMove(new Vector2(0, 53), 0.5f);
        imageVS.transform.DOScale(new Vector2(1.0f, 1.0f), 0.5f);
    }

    public void setAniVsNormal()
    {
        imageVS.SetActive(true);
        imageVS.transform.localPosition = new Vector2(0, 53);
        imageVS.transform.DOScale(new Vector2(1.0f, 1.0f), 0.5f);
    }

    public void setAniVsHide()
    {
        imageVS.SetActive(false);
        imageVS.transform.localPosition = new Vector2(0, 53);
        imageVS.transform.DOScale(new Vector2(1.5f, 1.5f), 0.5f);
    }

    public void handleFinishGame(JObject dataFn)
    {
        txtWaiting.gameObject.SetActive(false);
        stateGame = Globals.STATE_GAME.WAITING;
        finishDT = dataFn;
        cardDraPref.gameObject.transform.DOScale(new Vector2(0f, 0f), 0.5f);
        cardTigPref.gameObject.transform.DOScale(new Vector2(0f, 0f), 0.5f);

        clock.SetActive(false);
        btnBets.SetActive(false);
        disablePot(false);
        setStatusButtonsBet(!checkBeted, checkBeted);

        JObject data = JObject.Parse((string)finishDT["data"]);
        //JObject data = JObject.Parse("{\"DragonCardID\":13,\"TigerCardID\":5,\"typeWin\":1,\"dragonBig\":true,\"dragonSmail\":false,\"tigerBig\":false,\"tigerSmail\":true,\"dragon\":2,\"tiger\":5,\"tie\":1,\"dragonWinBig\":3,\"dragonWinSmail\":5,\"tigerWinBig\":2,\"tigerWinSmail\":6,\"listUser\":[{\"pid\":4399923,\"ag\":91633,\"agadd\":0,\"agLose\":-2000,\"vip\":3,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":4349730,\"ag\":0,\"agadd\":0,\"agLose\":-43821,\"vip\":1,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":4373480,\"ag\":75831,\"agadd\":0,\"agLose\":0,\"vip\":2,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":4397501,\"ag\":106759,\"agadd\":0,\"agLose\":0,\"vip\":3,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":5920471,\"ag\":102672,\"agadd\":0,\"agLose\":-1000,\"vip\":2,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":4387345,\"ag\":73327,\"agadd\":0,\"agLose\":0,\"vip\":3,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0},{\"pid\":4335324,\"ag\":72568,\"agadd\":0,\"agLose\":0,\"vip\":1,\"Dragon\":0,\"Tiger\":0,\"Tie\":0,\"DragonBig\":0,\"DragonSmail\":0,\"TigerBig\":0,\"TigerSmail\":0}],\"arrHistory\":[2,2,2,3,1,2,2,1]}");
        saveDT = data;

        int dragonCardId = getInt(data, "DragonCardID");
        int tigerCardId = getInt(data, "TigerCardID");

        listSaveHistory = getListInt(data, "arrHistory");
        if (popupHistory != null)
        {
            popupHistory.handleResultHisLayer1(listSaveHistory);
            popupHistory.handleResultHisLayer2(listSaveHistory);
        }

        DOTween.Sequence().AppendInterval(0.2f).AppendCallback(() =>
        {
            SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CARD_FLIP_1);
            cardDraPref.setTextureWithCode(dragonCardId, true);
            cardTigPref.setTextureWithCode(tigerCardId, true);
            cardDraPref.gameObject.transform.DOScale(new Vector2(0.4f, 0.4f), 0.5f);
            cardTigPref.gameObject.transform.DOScale(new Vector2(0.4f, 0.4f), 0.5f);
        });

        DOTween.Sequence().AppendInterval(0.7f).AppendCallback(() =>
        {
            handleWinPots(data);
        });
    }

    public void handleResultHisLayer3(JObject data)
    {

        //"dragon":2,"tiger":3,"tie":0,
        //    "dragonWinBig":1,"dragonWinSmail":3,"tigerWinBig":1,"tigerWinSmail":4,
        int cntDragon = getInt(data, "dragon");
        int cntTiger = getInt(data, "tiger");
        int cntTie = getInt(data, "tie");
        int cntDrBig = getInt(data, "dragonWinBig");
        int cntTgBig = getInt(data, "tigerWinBig");
        int cntDrSmall = getInt(data, "dragonWinSmail");
        int cntTgSmall = getInt(data, "tigerWinSmail");

        saveNumDr = cntDragon;
        saveNumTi = cntTiger;

        popupHistory.cntTie += cntTie;
        popupHistory.cntDr += cntDragon;
        popupHistory.cntTg += cntTiger;
        popupHistory.cntDrBig += cntDrBig;
        popupHistory.cntTgBig += cntTgBig;
        popupHistory.cntDrSmall += cntDrSmall;
        popupHistory.cntTgSmall += cntTgSmall;

        popupHistory.textCountTie.text = popupHistory.cntTie.ToString();
        popupHistory.textCountDragon.text = popupHistory.cntDr.ToString();
        popupHistory.textCountTiger.text = popupHistory.cntTg.ToString();
        popupHistory.textCountDragonBig.text = popupHistory.cntDrBig.ToString();
        popupHistory.textCountTigerBig.text = popupHistory.cntTgBig.ToString();
        popupHistory.textCountDragonSmall.text = popupHistory.cntDrSmall.ToString();
        popupHistory.textCountTigerSmall.text = popupHistory.cntTgSmall.ToString();
    }

    private void handleWinPots(JObject data)
    {
        int type = getInt(data, "typeWin");
        bool dragonBig = getBool(data, "dragonBig");
        bool dragonSmall = getBool(data, "dragonSmail");
        bool tigerBig = getBool(data, "tigerBig");
        bool tigerSmall = getBool(data, "tigerSmail");

        //dragon= 1
        //dragon_small= 5
        //d-big =4
        //tiger =2
        //t_small = 7
        //t_big =6
        //tie= 3

        listWinResult.Add(type);

        if (dragonBig == true)
        {
            listWinResult.Add(4);
        }

        if (tigerSmall == true)
        {
            listWinResult.Add(7);
        }

        if (dragonSmall == true)
        {
            listWinResult.Add(5);
        }

        if (tigerBig == true)
        {
            listWinResult.Add(6);
        }

        if (type == 1)
        {
            bkgAniStart.gameObject.SetActive(true);
            bkgAniStart.enabled = false;

            listAniStart[0].gameObject.SetActive(true);
            listAniStart[0].Initialize(true);
            listAniStart[0].AnimationState.SetAnimation(0, "win", false);
            listAniStart[0].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
            listAniStart[0].gameObject.transform.localPosition = new Vector2(-140, 207);

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                listAniStart[0].gameObject.SetActive(true);
                listAniStart[0].Initialize(true);
                listAniStart[0].AnimationState.SetAnimation(0, "normal", true);
                listAniStart[0].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
                listAniStart[0].gameObject.transform.localPosition = new Vector2(-140, 207);
            });

            listAniStart[1].gameObject.SetActive(true);
            listAniStart[1].Initialize(true);
            listAniStart[1].AnimationState.SetAnimation(0, "lose", false);
            listAniStart[1].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
            listAniStart[1].gameObject.transform.localPosition = new Vector2(360, 207);
            listAniStart[2].gameObject.SetActive(false);

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                listAniStart[1].AnimationState.TimeScale = 0;
            });

            cardDraPref.transform.Find("animTwinkle").gameObject.SetActive(true);
            cardDraPref.transform.Find("blink").gameObject.SetActive(true);

            cardTigPref.transform.Find("mask").gameObject.SetActive(true);
            cardTigPref.transform.Find("vetcao").gameObject.SetActive(true);
        }
        else if (type == 2)
        {
            bkgAniStart.gameObject.SetActive(true);
            bkgAniStart.enabled = false;

            listAniStart[0].gameObject.SetActive(true);
            listAniStart[0].Initialize(true);
            listAniStart[0].AnimationState.SetAnimation(0, "lose", false);
            listAniStart[0].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
            listAniStart[0].gameObject.transform.localPosition = new Vector2(-140, 207);

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                listAniStart[0].AnimationState.TimeScale = 0;
            });

            listAniStart[1].gameObject.SetActive(true);
            listAniStart[1].Initialize(true);
            listAniStart[1].AnimationState.SetAnimation(0, "win", false);
            listAniStart[1].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
            listAniStart[1].gameObject.transform.localPosition = new Vector2(360, 207);
            listAniStart[2].gameObject.SetActive(false);

            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                listAniStart[1].gameObject.SetActive(true);
                listAniStart[1].Initialize(true);
                listAniStart[1].AnimationState.SetAnimation(0, "normal", true);
                listAniStart[1].gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
                listAniStart[1].gameObject.transform.localPosition = new Vector2(360, 207);
            });

            cardTigPref.transform.Find("animTwinkle").gameObject.SetActive(true);
            cardTigPref.transform.Find("blink").gameObject.SetActive(true);

            cardDraPref.transform.Find("mask").gameObject.SetActive(true);
            cardDraPref.transform.Find("vetcao").gameObject.SetActive(true);
        }
        else
        {
            cardDraPref.transform.Find("animTwinkle").gameObject.SetActive(true);
            cardDraPref.transform.Find("blink").gameObject.SetActive(true);

            cardTigPref.transform.Find("animTwinkle").gameObject.SetActive(true);
            cardTigPref.transform.Find("blink").gameObject.SetActive(true);
        }

        listWinResult.ForEach(x =>
        {
            effectWinGate(x);
        });

        DOTween.Sequence()
            .AppendInterval(2.0f)
            .AppendCallback(() =>
            {
                getChipLose();
                handleChipLoseForPlayers(data);
                for (int i = 0; i < 7; i++)
                {
                    listPot[i].transform.Find("bet").GetComponentInChildren<TextMeshProUGUI>().text = "";
                    listPot[i].transform.Find("mybet").GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                if (type == 1)
                {
                    cardDraPref.transform.Find("animTwinkle").gameObject.SetActive(false);
                    cardDraPref.transform.Find("blink").gameObject.SetActive(false);
                }
                else if (type == 2)
                {
                    cardTigPref.transform.Find("animTwinkle").gameObject.SetActive(false);
                    cardTigPref.transform.Find("blink").gameObject.SetActive(false);
                }
                else
                {
                    cardDraPref.transform.Find("animTwinkle").gameObject.SetActive(false);
                    cardDraPref.transform.Find("blink").gameObject.SetActive(false);

                    cardTigPref.transform.Find("animTwinkle").gameObject.SetActive(false);
                    cardTigPref.transform.Find("blink").gameObject.SetActive(false);
                }
                payChipWin();

            })
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                imageVS.SetActive(false);
                handleChipWinForPlayers(data);
            })
            .AppendInterval(2.0f)
            .AppendCallback(() =>
            {
                cardDraPref.setTextureWithCode(0);
                cardTigPref.setTextureWithCode(0);
                cardDraPref.gameObject.transform.DOLocalMove(new Vector2(52, -60), 0.5f);
                cardTigPref.gameObject.transform.DOLocalMove(new Vector2(52, -60), 0.5f);
                cardDraPref.gameObject.transform.DOScale(new Vector2(0f, 0f), 0.5f);
                cardTigPref.gameObject.transform.DOScale(new Vector2(0f, 0f), 0.5f);
            })
            .AppendInterval(1.5f).AppendCallback(() =>
            {
                resetGame();
                handleResultHis(getListInt(data, "arrHistory"));
                //handleResultHisLayer3(data);
            });
    }

    private void handleChipWinForPlayers(JObject data)
    {
        List<JObject> inforUser = data["listUser"].ToObject<List<JObject>>();

        inforUser.ForEach((dataUser) =>
        {
            int idPl = getInt(dataUser, "pid");
            int agPl = getInt(dataUser, "ag");
            int agAdd = getInt(dataUser, "agadd");
            int agLose = getInt(dataUser, "agLose");

            Player player1 = getPlayerWithID(idPl);
            PlayerViewDragonTiger plView = (PlayerViewDragonTiger)player1.playerView;
            if (agAdd > 0 && player1.playerView.gameObject.activeSelf)
            {
                player1.playerView.effectFlyMoney(agAdd, 30);
            }

            if (idPl == Globals.User.userMain.Userid)
            {
                if (agAdd > 0)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.WIN);
                    aniWin.SetActive(true);
                    aniWin.transform.parent.SetAsLastSibling();
                    lbWinChip.text = "+" + Globals.Config.FormatMoney(agAdd, true).ToString();
                }
                else if (Math.Abs(agLose) > 0)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.LOSE);
                    aniLose.SetActive(true);
                    aniLose.transform.parent.SetAsLastSibling();
                    lbLoseChip.text = Globals.Config.FormatMoney(agLose, true).ToString();
                }
            }
            player1.ag = agPl;
            player1.setAg();
        });
    }

    private void handleChipLoseForPlayers(JObject data)
    {
        List<JObject> inforUser = data["listUser"].ToObject<List<JObject>>();

        inforUser.ForEach((dataUser) =>
        {
            int idPl = getInt(dataUser, "pid");
            int agPl = getInt(dataUser, "ag");
            int agAdd = getInt(dataUser, "agadd");
            int agLose = getInt(dataUser, "agLose");

            Player player1 = getPlayerWithID(idPl);
            PlayerViewDragonTiger plView = (PlayerViewDragonTiger)player1.playerView;
            if (agLose < 0 && player1.playerView.gameObject.activeSelf)
            {
                player1.playerView.effectFlyMoney(agLose, 30);
            }

            player1.ag = agPl;
            player1.setAg();
        });
    }

    public void handleBet(JObject dataBet)
    {

        JObject data = JObject.Parse((string)dataBet["data"]);

        string name = getString(data, "N");
        int betGate = getInt(data, "Num");
        long chipBet = getLong(data, "M");
        listBet[betGate - 1] += chipBet;

        if (name == thisPlayer.namePl)
        {
            listMyBet[betGate - 1] += chipBet;
        }
        setStatusButtonsBet(!checkBeted, checkBeted);

        Player plBet = getPlayer(name);
        if (plBet == thisPlayer)
        {
            stateGame = Globals.STATE_GAME.PLAYING;
        }

        PlayerViewDragonTiger plView = (PlayerViewDragonTiger)plBet.playerView;
        Vector2 posPl = plView.transform.localPosition;
        plBet.ag -= chipBet;
        plBet.setAg();

        chipBetColorInx = listValueChipBets.IndexOf(chipBet);
        ChipDragonTiger chip = getChip(chipBetColorInx);
        chip.playerName = name;
        chip.gateId = betGate;
        chip.chipValue = chipBet;
        chip.transform.localPosition = posPl;
        chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
        chipMoveTo(chip, betGate);
        listChipInTable.Add(chip);

        setDisplayBet();
        updateStagePot();
    }

    public void chipMoveTo(ChipDragonTiger chip, int betGate)
    {
        Vector2 posPot = Globals.Config.getPosInOtherNode(listPot[betGate - 1].transform.position, chipContainer);
        //RectTransform rectfPot = listPot[betGate - 1].GetComponent<RectTransform>();
        chip.transform
            .DOLocalMove(posPot, 0.3f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Vector2 randomPosition = new Vector2(
                    posPot.x + UnityEngine.Random.Range(-30, 30),
                    posPot.y + UnityEngine.Random.Range(-30, 30)
                );

                chip.transform.DOLocalJump(randomPosition, 20, 1, 0.2f);
            });
    }

    public override void handleSTable(string strData)
    {
        base.handleSTable(strData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(strData);
        saveDT = data;
        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 2, agTable * 5, agTable * 10, agTable * 20 };
        setAnimDTNormal();
        //===============CREATE CHIP BET======//
        JArray ArrP = getJArray(data, "ArrP");
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPl = (JObject)ArrP[i];
            string name = getString(dataPl, "N");
            Player plView = getPlayer(name);
            JArray Arr = getJArray(dataPl, "Arr");
            for (int j = 0; j < Arr.Count; j++)
            {
                JObject dataChip = (JObject)Arr[j];
                int betGate = getInt(dataChip, "N");
                int chipBet = getInt(dataChip, "M");
                listBet[betGate - 1] += chipBet;
                chipBetColorInx = listValueChipBets.IndexOf(chipBet);
                ChipDragonTiger chip = getChip(chipBetColorInx);
                chip.playerName = name;
                chip.gateId = betGate;
                chip.chipValue = chipBet;
                chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
                chipMoveTo(chip, betGate);
                listChipInTable.Add(chip);
            }
        }
        //===============END CHIP BET=================//
        thisPlayer.playerView.transform.localScale = Vector2.one;
        ////--------------- STABLE WHEN STARTED -----------//
        int idCardDr = getInt(data, "IdCardDr");
        int idCardTi = getInt(data, "IdCardTi");

        int timeRemain = (int)getInt(data, "T") / 1000;
        timeRemain -= 1;

        cardDraPref.gameObject.SetActive(idCardDr >= 0);
        cardTigPref.gameObject.SetActive(idCardTi >= 0);

        cardDraPref.setTextureWithCode(0, true);
        cardTigPref.setTextureWithCode(0, true);
        if (timeRemain > 0)
        {
            txtWaiting.gameObject.SetActive(false);
            clock.SetActive(true);
            setStatusButtonsBet(!checkBeted, checkBeted);
            setStatusButtonsBet(false, false);
            btnBets.SetActive(true);
            btnPots.SetActive(true);
            //btnPotsNormal.SetActive(false);
            setAniVsNormal();
            setDisplayBet();
            disablePot(true);
            lbTimeRemain.text = timeRemain.ToString();
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeRemain--;

                if (timeRemain > 5)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
                }
                else
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_HURRY);
                }
                lbTimeRemain.text = timeRemain + "";
            }).SetLoops(timeRemain).SetId("SoundClock");
        }
        else
        {
            txtWaiting.gameObject.SetActive(true);
            clock.gameObject.SetActive(false);
            btnBets.gameObject.SetActive(false);
            btnPots.SetActive(true);
            //btnPotsNormal.SetActive(false);
            setAniVsHide();
            disablePot(false);
        }
        listSaveHistory = getListInt(data, "arrHistory");
        handleResultHis(getListInt(data, "arrHistory"));
        updateStagePot();
        ////--------------- END -----------//
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        DOTween.Kill("SoundClock");
    }

    public override void handleRJTable(string strData)
    {
        base.handleRJTable(strData);
        JObject data = JObject.Parse(strData);

        txtWaiting.gameObject.SetActive(false);

        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 2, agTable * 5, agTable * 10, agTable * 20 };
        setAnimDTNormal();
        setAniVsNormal();
        checkBeted = true;
        int timeRemain = (int)getInt(data, "T") / 1000;
        timeRemain -= 1;

        if (timeRemain > 0)
        {
            stateGame = Globals.STATE_GAME.PLAYING;
            JArray ArrP = getJArray(data, "ArrP");
            for (int i = 0; i < ArrP.Count; i++)
            {
                JObject dataPl = (JObject)ArrP[i];
                string name = getString(dataPl, "N");
                int agPl = getInt(dataPl, "AG");
                Player plView = getPlayer(name);

                JArray Arr = getJArray(dataPl, "Arr");
                for (int j = 0; j < Arr.Count; j++)
                {
                    JObject dataChip = (JObject)Arr[j];
                    int betGate = getInt(dataChip, "N");
                    int chipBet = getInt(dataChip, "M");
                    listBet[betGate - 1] += chipBet;
                    agPl -= chipBet;
                    plView.ag = agPl;
                    plView.setAg();
                    chipBetColorInx = listValueChipBets.IndexOf(chipBet);
                    ChipDragonTiger chip = getChip(chipBetColorInx);
                    chip.playerName = name;
                    chip.gateId = betGate;
                    chip.chipValue = chipBet;
                    chip.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet, true);
                    chipMoveTo(chip, betGate);
                    listChipInTable.Add(chip);
                    if (plView == thisPlayer)
                    {
                        listMyBet[betGate - 1] += chipBet;
                        listPot[betGate - 1].transform.Find("mybet").GetComponent<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chipBet);
                    }
                }
            }

            int idCardDr = getInt(data, "IdCardDr");
            int idCardTi = getInt(data, "IdCardTi");
            cardDraPref.gameObject.SetActive(idCardDr >= 0);
            cardTigPref.gameObject.SetActive(idCardTi >= 0);

            cardDraPref.setTextureWithCode(0, true);
            cardTigPref.setTextureWithCode(0, true);

            clock.SetActive(true);
            setStatusButtonsBet(!checkBeted, checkBeted);
            btnBets.SetActive(true);
            btnPots.SetActive(true);
            //btnPotsNormal.SetActive(false);
            setDisplayBet();
            disablePot(true);
            lbTimeRemain.text = timeRemain.ToString();
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                timeRemain--;
                if (timeRemain > 5)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_TICK);
                }
                else
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.CLOCK_HURRY);
                }
                lbTimeRemain.text = timeRemain + "";
            }).SetLoops(timeRemain).SetId("SoundClock");
        }
        else
        {
            clock.gameObject.SetActive(false);
            btnBets.gameObject.SetActive(false);
            btnPots.SetActive(true);
            //btnPotsNormal.SetActive(false);
            disablePot(false);
        }
        listSaveHistory = getListInt(data, "arrHistory");
        handleResultHis(getListInt(data, "arrHistory"));
        updateStagePot();
        //--------------- END CREAT CHIP BETTED --------------//
    }

    private void handleResultHis(List<int> listHis)
    {
        listHis.Reverse();
        UIManager.instance.destroyAllChildren(parentMiniHis);
        for (int i = 0; i < listHis.Count; i++)
        {
            if (i < 8)
            {
                GameObject itemHIs = Instantiate(listDots[listHis[i] - 1], parentMiniHis);

                itemHIs.SetActive(true);
                roundDot.SetActive(true);
            }
        }
    }

    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        stateGame = Globals.STATE_GAME.WAITING;
        JObject data = JObject.Parse(objData);

        thisPlayer.playerView.setPosThanhBarThisPlayer();
        agTable = getInt(data, "M");
        listValueChipBets = new List<long> { agTable, agTable * 2, agTable * 5, agTable * 10, agTable * 20 };
    }

    public override void handleJTable(string objData)
    {
        base.handleJTable(objData);
    }

    public override void handleLTable(JObject objData)
    {
        base.handleLTable(objData);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.REMOVE);
    }

    public override PlayerView createPlayerView()
    {
        var plView = Instantiate(playerViewPrefab, PlayerContainer.transform);//.GetComponent<PlayerView>();
        plView.transform.SetSiblingIndex((int)Globals.ZODER_VIEW.PLAYER);
        plView.transform.localScale = new Vector2(0.8f, 0.8f);

        return plView.GetComponent<PlayerViewDragonTiger>();
    }

    public override void setGameInfo(int m, int id = 0, int maxBet = 0)
    {
        base.setGameInfo(m, id, maxBet);
        setInfoBet(m);
    }

    public void setInfoBet(int m)
    {
        listValueChipBets = new List<long> { m, m * 2, m * 5, m * 10, m * 20 };
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(listValueChipBets[i], true);
            //listChipBets[i].interactable = listValueChipBets[i] <= (int)thisPlayer.ag;
        }
        listChipBets[0].transform.localPosition = new Vector2(listChipBets[0].transform.localPosition.x, 16);
        betValue = listValueChipBets[0];
    }

    public void setDisplayBet()
    {
        //long betValid = 0;
        bool check = false;
        for (int i = 4; i >= 0; i--)
        {
            if (listValueChipBets[i] > thisPlayer.ag)
            {
                listChipBets[i].interactable = false;
                listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, 0);
                check = true;
            }
            else
            {
                // break
                listChipBets[i].interactable = true;
                if (stateGame == Globals.STATE_GAME.PLAYING && check && myChipBetColor >= i)
                {
                    listChipBets[i].transform.localPosition = new Vector2(listChipBets[i].transform.localPosition.x, 16);
                    betValue = listValueChipBets[i];
                    check = false;
                }
                //betValid = listValueChipBets[i];
            }
        }
    }

    //dragon= 1
    //dragon_small= 5
    //d-big =4
    //tiger =2
    //t_small = 7
    //t_big =6
    //tie= 3

    public void onClickChipBet(int chipBet)
    {
        SoundManager.instance.soundClick();
        betValue = listValueChipBets[chipBet];
        for (int i = 0; i < 5; i++)
        {
            listChipBets[i].transform.localPosition = new Vector3(listChipBets[i].transform.localPosition.x, i == chipBet ? 16 : 0);
        }
        chipBetColorInx = chipBet;
        myChipBetColor = chipBet;
    }

    public void onClickBet(int gateBet)
    {
        Debug.Log("onclick bet.....");
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.BET);
        SocketSend.sendBetDragonTiger(betValue, gateBet);
        checkBeted = true;
        //stateGame = Globals.STATE_GAME.PLAYING;
        listPot[gateBet - 1].GetComponent<Button>().interactable = false;

        DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    listPot[gateBet - 1].GetComponent<Button>().interactable = true;
                });
    }

    public void onClickDoubleBet()
    {
        SoundManager.instance.soundClick();
        int timeleft = int.Parse(lbTimeRemain.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 7; i++)
            {
                if (listMyBet[i] > 0)
                {
                    SocketSend.sendBetDragonTiger(listMyBet[i], i + 1);
                }
            }
        }
    }

    public void onClickReBet()
    {
        SoundManager.instance.soundClick();
        int timeleft = int.Parse(lbTimeRemain.text);
        if (timeleft > 1)
        {
            for (int i = 0; i < 7; i++)
            {
                if (listLastBet[i] > 0)
                {
                    checkBeted = true;
                    SocketSend.sendBetDragonTiger(listLastBet[i], i + 1);
                }
            }
        }
    }

    public void setStatusButtonsBet(bool btnrebet, bool btndouble)
    {
        if (!listLastBet.Any(element => element != 0))
        {
            btnrebet = false;
        }
        btnBets.transform.Find("btn_rebet").GetComponent<Button>().interactable = btnrebet;
        btnBets.transform.Find("btn_doublebet").GetComponent<Button>().interactable = btndouble;
    }

    private ChipDragonTiger getChip(int chipIndex)
    {
        ChipDragonTiger chip;
        if (chipPoolTG.Count > 0)
        {
            chip = chipPoolTG[0];
            chipPoolTG.Remove(chip);
            chip.transform.parent = chipContainer.transform;
            chip.gameObject.SetActive(true);
        }
        else
        {
            chip = Instantiate<ChipDragonTiger>(chipPref, chipContainer.transform);
        }
        chip.chipSprite = chipIndex;
        if (chipIndex == -1)
        {
            chipIndex = 5;
        }
        chip.GetComponent<Image>().sprite = chip.listSpriteChip[chipIndex];
        chip.transform.localScale = new Vector2(0.4f, 0.4f);
        return chip;
    }

    private void getChipLose()
    {

        for (int i = 0; i < listChipInTable.Count; i++)
        {
            ChipDragonTiger chip = listChipInTable[i];
            if (!listWinResult.Contains(chip.gateId))
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.GET_CHIP);
                chip.transform.DOLocalMove(new Vector2(0, -37), 0.5f);
                chip.transform.DOScale(new Vector2(0f, 0f), 0.5f);
                //listChipInTable.Remove(chip);
            }
        }
    }

    private void putChip(ChipDragonTiger chip)
    {
        chipPoolTG.Add(chip);
        chip.transform.SetParent(null);
        chip.gameObject.SetActive(true);
    }

    public void onClickShowPlayer()
    {
        //if (listPlayer == null || buttonBet != null)
        updatePositionPlayerView();
        listPlayer = Instantiate(prefab_popup_player, transform).GetComponent<NodePlayerDragonTiger>();
        listPlayer.transform.SetSiblingIndex((int)Globals.GAME_ZORDER.Z_MENU_VIEW);

    }

    private void payChipWin()
    {
        Vector2 posModel = new Vector2(0, 220);
        for (int i = 0; i < listChipInTable.Count; i++)
        {
            var chip = listChipInTable[i];
            if (listWinResult.Contains(chip.gateId))
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.THROW_CHIP);
                ChipDragonTiger chip1 = getChip(chip.chipSprite);
                chip1.transform.localPosition = posModel;
                Vector2 posChip = chip.transform.localPosition;
                posChip.x += 5f;
                posChip.y += 5f;
                chip1.transform.DOLocalMove(posChip, 0.5f);
                chip1.playerName = chip.playerName;
                chip1.gateId = chip.gateId;
                chip1.chipValue = chip.chipValue;
                chip1.transform.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.FormatMoney(chip.chipValue, true);
                listChipsPay.Add(chip1);
            }
        }
        listChipInTable.AddRange(listChipsPay);
        listChipsPay.Clear();
        DOTween.Sequence()
            .AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                for (int i = 0; i < listChipInTable.Count; i++)
                {
                    var chip = listChipInTable[i];
                    if (listWinResult.Contains(chip.gateId))
                    {
                        // get player bet win position
                        Player playerBet = getPlayer(chip.playerName);
                        PlayerViewDragonTiger plView = (PlayerViewDragonTiger)playerBet.playerView;
                        Vector2 posPlayer = plView.transform.localPosition;
                        chip.transform.DOLocalMove(posPlayer, 0.5f);
                    }
                }
            });
    }

    public void updateStagePot()
    {
        for (int i = 0; i < 7; i++)
        {
            listPot[i].transform.Find("bet").GetComponentInChildren<TextMeshProUGUI>().text = listBet[i] > 0 ? Globals.Config.FormatMoney(listBet[i], true) : "";
            listPot[i].transform.Find("mybet").GetComponentInChildren<TextMeshProUGUI>().text = listMyBet[i] > 0 ? Globals.Config.FormatMoney(listMyBet[i], true) : "";
        }
    }

    public void effectWinGate(int index)
    {
        int resultWin = index - 1;

        for (int i = 0; i < listPot.Count; i++)
        {
            if (i == resultWin)
            {
                Button objButton = listPot[i].GetComponent<Button>();
                Sprite spr = objButton.spriteState.pressedSprite;
                GameObject btnPress = objButton.transform.Find("press").gameObject;
                if (btnPress != null)
                {
                    Image btnImgPress = btnPress.GetComponent<Image>();
                    btnImgPress.sprite = spr;
                    btnPress.gameObject.SetActive(true);
                    btnImgPress.enabled = true;
                    Color normalColor = btnImgPress.color;
                    Color noOpacity = new Color(1, 1, 1, 0);
                    DOTween.Sequence()
                        .Append(btnImgPress.DOColor(noOpacity, 0.2f))
                        .Append(btnImgPress.DOColor(normalColor, 0.2f))
                        .SetLoops(5)
                        .OnComplete(() =>
                        {
                            btnPress.gameObject.SetActive(false);
                            btnImgPress.enabled = false;
                        });
                }
            }
        }
    }

    public void OnClickShowPopupHistory()
    {
        SoundManager.instance.soundClick();
        popupHistory = Instantiate(popupHistoryPrefab, transform).GetComponent<HistoryDragonTiger>();

        popupHistory.transform.SetAsLastSibling();
        popupHistory.handleResultHisLayer1(listSaveHistory);
        popupHistory.handleResultHisLayer2(listSaveHistory);
        handleResultHisLayer3(saveDT);
        popupHistory.UpdateFillRange(saveNumDr, saveNumTi);
    }

    public void disablePot(bool s)
    {
        for (int j = 0; j < 7; j++)
        {
            listPot[j].GetComponent<Button>().interactable = s;
        }
    }
}
