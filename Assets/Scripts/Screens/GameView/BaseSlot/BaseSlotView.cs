using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using TMPro;
using Spine.Unity;
using Newtonsoft.Json.Linq;
using System.Threading;
using OneSignalSDK;
using static SiXiangView;
using Unity.VisualScripting;
using Socket.Quobject.EngineIoClientDotNet.Modules;
using Globals;

public class BaseSlotView : GameView
{
    // Start is called before the first frame update
    public static BaseSlotView Instance = null;

    [SerializeField]
    protected List<CollumController> listCollum;

    [SerializeField] protected SkeletonGraphic spineSpecialWin;
    [SerializeField] protected SkeletonGraphic spineJackpotWin;
    [SerializeField] protected SkeletonGraphic spineBgMoney;
    [SerializeField] protected SkeletonGraphic spineBtnSpin;
    [HideInInspector] protected Spine.TrackEntry spineSpecialWinTrack;
    [HideInInspector] protected Spine.TrackEntry spineJPWinTrack;

    [SerializeField]
    protected GameObject collumContainer;
    [SerializeField]
    protected List<Sprite> sprStateBets = new List<Sprite>();
    [SerializeField]
    protected Image imgStateBet;
    [SerializeField]
    protected GameObject nodeAutoSpin;
    [SerializeField]
    protected GameObject effectContainer;

    [SerializeField]
    GameObject rulePr;

    [SerializeField]
    protected TextMeshProUGUI lbAutoRemain, lbInfoSession;
    [SerializeField]
    protected TextNumberControl lbSpecicalWin;
    [SerializeField]
    protected TextNumberControl lbJPWin;
    [SerializeField]
    protected TextMeshProUGUI lbBetLevel;
    [SerializeField]
    protected TextNumberControl lbUserAmount;
    [SerializeField]
    public TextNumberControl lbChipWins;
    [SerializeField]
    protected List<TextNumberControl> listLbJackpot = new List<TextNumberControl>();

    [SerializeField]
    public InfoBarController infoBar;
    protected bool _CanSpinDP = true;
    //[HideInInspector] protected string soundLineWin = "";



    [HideInInspector]
    public enum SPIN_TYPE
    {
        NORMAL,
        AUTO
    }
    [HideInInspector]
    public enum SPEED_MOVE
    {
        NORMAL,
        AUTO
    }
    [HideInInspector]
    public enum GAME_STATE
    {
        PREPARE,
        SPINNING,
        SHOWING_RESULT,
        JOIN_GAME
    }
    [HideInInspector] public SPIN_TYPE spintype = SPIN_TYPE.NORMAL;
    [HideInInspector] public GAME_STATE gameState = GAME_STATE.JOIN_GAME;
    [SerializeField] public SkeletonGraphic animBtnSpin;
    [HideInInspector]
    protected bool isHoldSpin = false;

    protected WIN_TYPE winType = WIN_TYPE.NORMAL;
    public WIN_JACKPOT_TYPE winTypeJackpot = WIN_JACKPOT_TYPE.NONE;

    protected int autoSpinRemain = 0;
    protected int currentBetLevel = 0; //index trong list betlevel
    public long agPlayer = 0;
    public long winAmount = 0;
    public long normalWinAmount = 0;
    public long oldWinAmount = 0;
    protected float timeHoldSpin = 0;
    public int gameType = 0;
    protected int freeSpinleft = 0;

    private bool isChangeBet = false;
    protected bool isGrandJackpot = false;
    Vector2 RECT_SIZE = new Vector2(135f, 135f);

    protected List<int> betLevels = new List<int>();
    protected List<int> jackpotLevel = new List<int>();
    protected List<int> validBetLevels = new List<int>();
    protected List<List<int>> spinReelView = new List<List<int>>();
    protected List<List<int>> payTalbe = new List<List<int>>();

    protected JObject spinData = new JObject();
    protected List<JObject> spinPayLines = new List<JObject>();
    protected JObject GET_INFO_DATA = new JObject();
    internal static UnityMainThread wkr;

    List<Image> lsCoinPool = new List<Image>();
    [SerializeField] Image coinEffectPrefab;
    protected List<GameObject> listLineStraight = new List<GameObject>();
    [SerializeField]
    protected GameObject linePrf;
    [SerializeField]
    protected GameObject lineContainer;
    public CancellationTokenSource cts_ShowEffect;
    private List<CancellationTokenSource> cancelTokList = new List<CancellationTokenSource>();
    protected Task spineSpecialWinTask = null;
    protected Task spineJPWinTask = null;

    protected string PATH_ANIM_SPECICAL_WIN = "";

    protected List<List<int>> payLines = new List<List<int>> {new List<int>{1, 1, 1, 1, 1}, new List<int>{0, 0, 0, 0, 0}, new List<int>{2, 2, 2, 2, 2}, new List<int>{0, 1, 2, 1, 0}, new List<int>{2, 1, 0, 1, 2},
        new List<int>{1, 0, 0, 0, 1}, new List<int>{1, 2, 2, 2, 1}, new List<int>{0, 0, 1, 2, 2}, new List<int>{2, 2, 1, 0, 0}, new List<int>{1, 2, 1, 0, 1},
        new List<int>{1, 0, 1, 2, 1}, new List<int>{0, 1, 1, 1, 0}, new List<int>{2, 1, 1, 1, 2}, new List<int>{0, 1, 0, 1, 0}, new List<int>{2, 1, 2, 1, 2},
        new List<int>{1, 1, 0, 1, 1}, new List<int>{1, 1, 2, 1, 1}, new List<int>{0, 0, 2, 0, 0}, new List<int>{2, 2, 0, 2, 2}, new List<int>{0, 2, 2, 2, 0},
        new List<int>{2, 0, 0, 0, 2}, new List<int>{1, 2, 0, 2, 1}, new List<int>{1, 0, 2, 0, 1}, new List<int>{0, 2, 0, 2, 0}, new List<int>{2, 0, 2, 0, 2}};

    protected List<string> listColor = new List<string> {
        "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
    "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
    "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
    "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
    "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
    "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
    "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
    "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
    "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
    "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
    "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
    "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE"};
    protected enum WIN_TYPE
    {
        NORMAL = 0,
        NICE_WIN = 1,
        BIGWIN = 2,
        HUGEWIN = 3,
        MEGAWIN = 4,

    }
    public enum WIN_JACKPOT_TYPE
    {
        NONE = 0,
        JACKPOT_MINOR = 1,
        JACKPOT_MAJOR = 2,
        JACKPOT_MEGA = 3,
        JACKPOT_GRAND = 4,
    }
    public override void OnDestroy()
    {
        base.OnDestroy();

        if (cts_ShowEffect != null)
        {
            cts_ShowEffect.Cancel();
        }
        cancelTokList.ForEach((token) =>
        {
            Debug.Log("Cancel Token");
            token?.Cancel();
            token.Dispose();
        });
    }
    protected override void Start()
    {
        base.Start();
        infoBar.prepareSpin();

    }
    protected override void Awake()
    {
        base.Awake();
        initCollum();
        setRandomView();
    }
    protected override void Update()
    {
        base.Update();
        if ((gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME) && validBetLevels.Count == 0)
        {
            //infoBar.setInfoText(Globals.Config.getTextConfig("not_enought_gold"));
        }
        else
        {
            if (isHoldSpin)
            {
                timeHoldSpin += Time.deltaTime;
                //Debug.Log("timeHoldSpin:" + timeHoldSpin);
                if (timeHoldSpin > 1.3 && spintype == SPIN_TYPE.NORMAL && gameState != GAME_STATE.SPINNING)
                {
                    onShowNodeAuto();
                    isHoldSpin = false;
                }
            }
        }

    }
    public void setAGPlayer()
    {
        lbUserAmount.setValue(agPlayer, true);
    }
    protected async void showEffectChip()
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CHIP_REWARD);
        Transform transFrom = lbChipWins.transform;
        Transform transTo = lbUserAmount.transform;
        coinFly(transFrom, transTo);
        await Task.Delay(TimeSpan.FromSeconds(1.0f));
        //Globals.Config.tweenNumberToNumber(lbUserAmount, agPlayer, agPlayer - winAmount, 0.2f);
        lbUserAmount.setValue(agPlayer, true);
    }
    protected void coinFly(Transform transFrom, Transform transTo, int count = 10, float timeInterval = 0.1f)
    {
        var parentCoin = transform.Find("CoinLayer");
        for (var i = 0; i < 6; i++)
        {
            DOTween.Sequence().AppendInterval(i * 0.1f).AppendCallback(() =>
            {
                Image obj;
                if (lsCoinPool.Count > 0)
                {
                    obj = lsCoinPool[0];
                    lsCoinPool.RemoveAt(0);
                }
                else
                {
                    obj = Instantiate(coinEffectPrefab, parentCoin).GetComponent<Image>();

                }
                obj.gameObject.SetActive(true);
                obj.GetComponent<Animator>().Play("idle");
                effectCoinFly(obj, transFrom, transTo);
            });

        }

    }
    void effectCoinFly(Image coinEffect, Transform transFrom, Transform transTo)

    {
        DOTween.Kill(coinEffect.transform);
        coinEffect.transform.position = transFrom.position;
        coinEffect.transform.DOJump(transTo.position, 1, 1, 2).SetEase(Ease.InOutCubic);
        var cc = coinEffect.color;
        cc.a = .2f;
        coinEffect.color = cc;
        coinEffect.DOFade(1, .75f);

        DOTween.Sequence()
            .AppendInterval(.5f)
            .Append(coinEffect.transform.DOScale(2, 0.25f))
            .AppendInterval(0.15f)
            .Append(coinEffect.transform.DOScale(0.5f, 0.25f))//0.85
            .AppendInterval(0.6f)
            .Append(coinEffect.DOFade(0, .25f)).AppendCallback(() =>
            {
                coinEffect.gameObject.SetActive(false);
                lsCoinPool.Add(coinEffect);
                //Destroy(coinEffect.gameObject);
            });
    }
    public virtual void setStateBtnSpin()
    {

    }
    protected virtual void resetSlotView()
    {
        setStateSpin(GAME_STATE.PREPARE);
        infoBar.prepareSpin();
        listCollum.ForEach((col) =>
        {
            col.Reset();
        });
        setBetLevel(validBetLevels[currentBetLevel]);

    }
    public virtual void handleGetInfo(JObject data)
    {
        //{\"minor\":10,\"major\":20,\"mega\":30,\"grand\":150,\"payTable\":[[0.0,0.0,0.5,2.5,5.0],[0.0,0.0,0.5,2.5,5.0],[0.0,0.0,0.5,2.5,5.0],[0.0,0.0,0.5,2.5,5.0],[0.0,0.0,0.5,2.5,5.0],[0.0,0.0,2.0,10.0,20.0],[0.0,0.0,1.5,7.5,15.0],[0.0,0.0,1.2,6.0,12.0],[0.0,0.0,1.0,5.0,10.0],[0.0,0.0,0.0,0.0,0.0],[0.0,0.0,0.0,0.0,0.0]],\"betLevels\":[1,5,50,500,1000,2500,5000,10000,25000,50000,100000,200000,500000,1000000,5000000,10000000],\"bet\":2500,\"userAmount\":91338,\"gameType\":0,\"isBonusGame\":false,\"bonusGameMultiplier\":4.0,\"winAmount\":0,\"pearls\":[],\"numberOfDragonPearlSpins\":0,\"numberOfPick\":0,\"cards\":[],\"rewards\":[]}
        GET_INFO_DATA = data;
        betLevels = data["betLevels"].ToObject<List<int>>();
        agPlayer = getLong(data, "userAmount");
        lbUserAmount.Text = Globals.Config.FormatNumber(agPlayer);
        jackpotLevel = new List<int> { getInt(data, "minor"), getInt(data, "major"), getInt(data, "mega"), getInt(data, "grand") };
        payTalbe = data["payTable"].ToObject<List<List<int>>>();
        currentBetLevel = betLevels.IndexOf(getInt(data, "bet"));
        setBetLevel(getInt(data, "bet"));

    }
    protected void onClickChangeBetLevel(string type)
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        if (gameType == (int)GAME_TYPE.NORMAL)
        {
            if (isChangeBet || gameType == 2 || gameState == GAME_STATE.SPINNING)
            {
                return;
            }
            isChangeBet = true;
            if (type == "plus")
            {
                currentBetLevel = (currentBetLevel < validBetLevels.Count - 1) ? currentBetLevel + 1 : 0;
            }
            else if (type == "minus")
            {
                currentBetLevel = (currentBetLevel > 0) ? currentBetLevel - 1 : validBetLevels.Count - 1;
            }
            else //maxbet
            {
                if (currentBetLevel == validBetLevels.Count - 1 && canClickSpin())
                {
                    onSpinTriggerUp();
                    DOTween.Sequence().AppendInterval(0.25f).AppendCallback(() =>
                    {
                        isChangeBet = false;
                    });
                    return;
                }
                else
                {
                    currentBetLevel = validBetLevels.Count - 1;
                }

            }
            lbBetLevel.text = Globals.Config.FormatNumber(validBetLevels[currentBetLevel]);
            if (currentBetLevel == validBetLevels.Count - 1)
            {
                imgStateBet.sprite = sprStateBets[1];
            }
            else
            {
                imgStateBet.sprite = sprStateBets[0];
            }
            imgStateBet.SetNativeSize();
            setJackpotInfo();
            DOTween.Sequence().AppendInterval(0.25f).AppendCallback(() =>
            {
                isChangeBet = false;
            });
            SocketSend.sendPackageSlotSixiang(Globals.ACTION_SLOT_SIXIANG.getBonusGames, validBetLevels[currentBetLevel]);
        }

    }
    private bool canClickSpin()
    {
        if (gameState == GAME_STATE.SPINNING)
        {
            return false;
        }
        return true;
    }
    public virtual void handleBonusInfo(JObject data)
    {

    }
    // Update is called once per frame
    protected void initCollum()
    {
        for (int i = 1; i < 5; i++)
        {
            CollumController col = Instantiate(listCollum[0], collumContainer.transform).GetComponent<CollumController>();

            col.transform.name = "ItemCollum" + (i + 1);
            col.collumIndex = i;
            listCollum.Add(col);
        }
    }
    protected void setRandomView()
    {
        listCollum.ForEach((col) =>
        {
            col.setRandomView();
        });
    }
    public void startSpin()
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SPIN_REEL);
        onHideNodeAuto();
        listCollum.ForEach((col) =>
        {
            col.spinSymbol();
        });
    }
    public void onClickSelectAuto(int number)
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        if (!DOTween.IsTweening(nodeAutoSpin.transform))
        {
            if (cts_ShowEffect != null)
            {
                cts_ShowEffect.Cancel();
            }
            autoSpinRemain = number;
            onHideNodeAuto();
            lbAutoRemain.gameObject.SetActive(true);
            spintype = SPIN_TYPE.AUTO;
            if (gameState == GAME_STATE.SHOWING_RESULT)
            {
                cts_ShowEffect.Cancel(true);
            }
            else
            {
                onSpinTriggerUp();
            }
        }


    }

    protected void setAutoSpinRemain()
    {
        lbAutoRemain.gameObject.SetActive(autoSpinRemain > 0);
        if (autoSpinRemain > 100)
        {
            lbAutoRemain.text = System.Uri.UnescapeDataString("\u221E");
            lbAutoRemain.fontSize = 60;
        }
        else
        {
            lbAutoRemain.fontSize = 40;
            lbAutoRemain.text = autoSpinRemain.ToString();
        }
    }

    protected async Task checkScatterItem()
    {
        int scatterInCol = 0;

        spinReelView.ForEach(colView =>
        {
            if (colView.Contains(10))
            {
                scatterInCol++;
            }
        });
        if (scatterInCol > 2)
        {
            listCollum.ForEach(col =>
            {
                col.showScatterSymbol();
            });
            SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SCATTER_WIN);
            await Task.Delay(TimeSpan.FromSeconds(1.5f));
        }
    }

    public virtual async void allCollumStopCompleted()
    {
        cts_ShowEffect = new CancellationTokenSource();
    }
    public CancellationTokenSource getCancelToken()
    {
        CancellationTokenSource cancelTok = new CancellationTokenSource();
        cancelTokList.Add(cancelTok);
        return cancelTok;
    }
    public void onShowNodeAuto()
    {
        if (nodeAutoSpin.activeSelf == false && gameType == (int)GAME_TYPE.NORMAL)
        {
            nodeAutoSpin.gameObject.SetActive(true);
            nodeAutoSpin.transform.DOLocalMoveY(150, 0.2f).SetEase(Ease.OutSine);
        }
    }
    public void onHideNodeAuto()
    {
        nodeAutoSpin.transform.DOLocalMoveY(-153, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            nodeAutoSpin.SetActive(false);
        });
    }

    public async void handleStopSpin()
    {
        foreach (CollumController col in listCollum)
        {
            col.Stop(spinReelView[listCollum.IndexOf(col)]);
            await Task.Delay(TimeSpan.FromSeconds(spintype == SPIN_TYPE.NORMAL ? 0.4f : 0.2f));
        }
        _CanSpinDP = true;
    }

    public virtual void handleNormalSpin(JObject data)
    {
        //{\"winAmount\":-50,\"userAmount\":80167,\"isGrandJackpot\":false,\"isBonusMiniGame\":false,\"payLines\":[],\"reels\":[[4,3,8],[2,3,2],[0,10,0],[5,8,1],[4,2,9]],\"isSelectBonusGame\":false}
        //data = JObject.Parse(SiXiangFakeData.Instance.GrandJackpotNormalSpin);
        spinData = data;
        spinReelView = data["reels"].ToObject<List<List<int>>>();
        spinPayLines = data["payLines"].ToObject<List<JObject>>();
        agPlayer = getLong(data, "userAmount");
        setNearSpinEffect();

        handleStopSpin();
    }

    public void setWinType(long winAmount)
    {

        winType = WIN_TYPE.NORMAL;
        if (isGrandJackpot)
        {
            winAmount = winAmount + validBetLevels[currentBetLevel] * jackpotLevel[3];
        }
        Debug.Log("winAmount=" + winAmount);
        int betAmount = validBetLevels[currentBetLevel];
        if (winAmount > 50 * betAmount)
        {
            winType = WIN_TYPE.MEGAWIN;
        }
        else if (winAmount > 25 * betAmount)
        {
            winType = WIN_TYPE.HUGEWIN;
        }
        else if (winAmount > 10 * betAmount)
        {
            winType = WIN_TYPE.BIGWIN;
        }
        else if (winAmount > 5 * betAmount)
        {
            winType = WIN_TYPE.NICE_WIN;
        }
    }
    protected void setNearSpinEffect()
    {
        int scatterInCol = 0;
        //List<int> indexStartColNear = new List<int>();

        spinReelView.ForEach(colView =>
        {
            if (scatterInCol >= 2)
            {
                if (listCollum[spinReelView.IndexOf(colView)].collumIndex == 4)
                    listCollum[spinReelView.IndexOf(colView)].isNeerSpin = true;
            }
            if (colView.Contains(10))
            {
                scatterInCol++;
            }
        });

    }
    protected async Task showWinLine()
    {
        if (spinPayLines.Count > 0)
        {
            hideAllSymbol();
            List<Vector2> listPos = new List<Vector2>();
            for (int i = 0, l = spinPayLines.Count; i < l; i++)
            {
                JObject dataLine = spinPayLines[i];
                List<int> payLineWin = payLines[getInt(dataLine, "lineNumber")];
                Color colorLine = new Color();
                UnityEngine.ColorUtility.TryParseHtmlString(listColor[getInt(dataLine, "lineNumber")], out colorLine);
                listPos.Clear();
                for (int j = 0; j < 5; j++)
                {
                    listPos.Add(lineContainer.transform.InverseTransformPoint(listCollum[j].getPosSymbol(payLineWin[j] + 1)));
                }
                drawLines(listPos, colorLine);
                await Task.Delay(100);
            }
            await Task.Delay(500);
            listLineStraight.ForEach(line => Destroy(line));
            listLineStraight.Clear();
            await showLineOneByOne();
        }
    }
    public Vector2 getPosSymbol(int col, int row)
    {
        return listCollum[col].getPosSymbol(row);
    }
    protected async Task showLineOneByOne()
    {
        if (autoSpinRemain == 0 || (spintype == SPIN_TYPE.AUTO && spinPayLines.Count == 1))
        {
            await Task.Run(async () =>
            {
                UnityMainThread.instance.AddJob(() =>
                {
                    setStateSpin(GAME_STATE.SHOWING_RESULT);

                    if (spinPayLines.Count > 0)
                    {
                        lbChipWins.ResetValue();
                        lbChipWins.setValue(normalWinAmount, true);
                        infoBar.setStateWin("win");
                    }
                });
                List<Vector2> listPos = new List<Vector2>();
                int index = 0;
                for (int i = 0, l = spinPayLines.Count; i < l; i++)
                {
                    List<Task> drawLinesTask = new List<Task>();

                    UnityMainThread.instance.AddJob(() =>
                    {
                        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.LINE_WIN);
                        hideAllSymbol();
                        listPos.Clear();
                        JObject dataLine = spinPayLines[index];
                        List<int> payLineWin = payLines[getInt(dataLine, "lineNumber")];
                        int numberOfSymbols = getInt(dataLine, "numberOfSymbols");
                        Color colorLine = new Color();
                        UnityEngine.ColorUtility.TryParseHtmlString(listColor[getInt(dataLine, "lineNumber")], out colorLine);
                        infoBar.setInfoWinLine(getInt(dataLine, "symbol"), numberOfSymbols, getInt(dataLine, "winAmount"));
                        for (int j = 0; j < numberOfSymbols; j++)
                        {
                            //listPos.Add(lineContainer.transform.InverseTransformPoint(listCollum[j].getPosSymbol(payLineWin[j] + 1)));
                            drawLinesTask.Add(listCollum[j].activeSymbol(payLineWin[j] + 1));

                        }
                        index++;

                    });
                    await Task.Delay(TimeSpan.FromSeconds(0.1f), cts_ShowEffect.Token);
                    await Task.WhenAll(drawLinesTask.ToArray());
                }
                UnityMainThread.instance.AddJob(() =>
                {
                    activeAllSymbol();
                });
            }, cts_ShowEffect.Token);
        }

    }
    protected Vector2 getCrossPoint(Vector2 vec1, Vector2 vec2)
    {

        int delta = vec1.y > vec2.y ? -1 : 1;
        Vector2 posA = new Vector2(vec1.x - RECT_SIZE.x / 2, vec1.y + (RECT_SIZE.y / 2) * delta);
        Vector2 posB = new Vector2(posA.x + RECT_SIZE.x, posA.y);
        if (Mathf.Abs(vec1.y - vec2.y) < 1)
        {
            posA = new Vector2(vec1.x + RECT_SIZE.x / 2, vec1.y + RECT_SIZE.y / 2);
            posB = new Vector2(vec1.x + RECT_SIZE.x / 2, vec1.y - RECT_SIZE.y / 2);
        }
        Vector2 getCrossPoint = new Vector2(posA.x + RECT_SIZE.x / 2, posA.y);
        return getCrossPoint;
    }
    protected void drawLines2(List<Vector2> listPos, Color colorLine)
    {
        GameObject lineStraight = Instantiate(linePrf, lineContainer.transform);

        lineStraight.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        //lineStraight.SetActive(false);
        listLineStraight.Add(lineStraight);
        LineController lineComp = lineStraight.GetComponent<LineController>();
        lineComp.drawLine(listPos, colorLine);
    }
    protected void drawRect(List<Vector2> listPosRect, Color colorLine, int numberItem)
    {

        List<Vector2> listPosRemain = new List<Vector2>();
        for (int i = 0, l = listPosRect.Count; i < l; i++)
        {
            Vector2 startPos = listPosRect[i];
            if (i < l - 1)
            {
                Vector2 nextPos = listPosRect[i + 1];

                if (Mathf.Abs(nextPos.y - startPos.y) > 200)// check 2 o vuong cach nhau 1 moi ve line noi tiep;
                {

                    if (i < numberItem - 1)
                    {
                        List<Vector2> listPos = new List<Vector2>();
                        Vector2 firstIntersectPos = getCrossPoint(listPosRect[i], listPosRect[i + 1]);
                        Vector2 nextIntersectPos = getCrossPoint(listPosRect[i + 1], listPosRect[i]);
                        listPos.Add(firstIntersectPos);
                        listPos.Add(nextIntersectPos);
                        drawLines2(listPos, colorLine);
                    }
                }

            }
            if (i >= numberItem && listPosRemain.Count == 0) // t?nh position ?i?m b?t ??u v? line t? ? cu?i c?ng ??n m?p ph?i
            {
                Vector2 previousPos = listPosRect[i - 1];
                Vector2 startPosLineRemain = new Vector2();
                if (previousPos.y == startPos.y)
                {
                    startPosLineRemain = new Vector2(listPosRect[i - 1].x + RECT_SIZE.x / 2, listPosRect[i - 1].y);
                }
                else
                {
                    bool isTwoItemSpace = (Mathf.Abs(startPos.y - previousPos.y) > 200);
                    if (previousPos.y < startPos.y)
                    {
                        startPosLineRemain = isTwoItemSpace ? new Vector2(previousPos.x, previousPos.y + RECT_SIZE.y / 2) : new Vector2(previousPos.x + RECT_SIZE.x / 2, previousPos.y + RECT_SIZE.y / 2);
                    }
                    else
                    {
                        startPosLineRemain = isTwoItemSpace ? new Vector2(previousPos.x, previousPos.y - RECT_SIZE.y / 2) : new Vector2(previousPos.x + RECT_SIZE.x / 2, previousPos.y - RECT_SIZE.y / 2);
                    }

                }
                listPosRemain.Add(startPosLineRemain);
            }
            if (i >= numberItem)
            {
                listPosRemain.Add(listPosRect[i]);
            }
        }
        listPosRemain.Add(new Vector2(listPosRect[listPosRect.Count - 1].x + RECT_SIZE.x / 2, listPosRect[listPosRect.Count - 1].y)); //add them 1 doan cuoi cung ve tu tam icon cuoi ra mep phai
        drawLines2(listPosRemain, colorLine);
    }
    public virtual void onClickSpin()
    {
        Globals.Config.Vibration();
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SPIN);
        if (cts_ShowEffect != null)
            cts_ShowEffect.Cancel();
        if (gameType == 0)
        {
            agPlayer -= validBetLevels[currentBetLevel];
            lbUserAmount.setValue(agPlayer, true, 0.5f, "AGplayer: ");
            infoBar.setStateWin("lastWin");
            infoBar.setSpining();
            startSpin();
        }
        //showSpineSpecialWin(WIN_TYPE.HUGEWIN, 123456);
    }

    public void onSpinTriggerDown()
    {

        timeHoldSpin = 0;
        isHoldSpin = true;

    }
    public virtual void onSpinTriggerUp()
    {
        Debug.Log("onSpinTriggerUp:gameState=" + gameState);
        if ((gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME) && agPlayer < validBetLevels[currentBetLevel])
        {
            infoBar.setInfoText(Globals.Config.getTextConfig("not_enought_gold"));
            string textShow = Globals.Config.getTextConfig("txt_not_enough_money_gl");
            string textBtn2 = Globals.Config.getTextConfig("shop");
            string textBtn3 = Globals.Config.getTextConfig("label_cancel");
            UIManager.instance.showDialog(textShow, textBtn2, () =>
            {
                UIManager.instance.openShop();
            }, textBtn3);
        }
        else
        {
            if (timeHoldSpin < 1.3f) //click bt
            {
                if (gameState != GAME_STATE.SPINNING) //dang dung.chua quay
                {
                    if (gameState == GAME_STATE.SHOWING_RESULT)
                    {
                        if (cts_ShowEffect != null)
                            cts_ShowEffect.Cancel();
                    }
                    else
                    {
                        if (gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME)
                        {
                            onClickSpin();
                        }
                    }
                }
                else
                {
                    spintype = SPIN_TYPE.NORMAL;
                    Debug.Log("SpinType=" + spintype + "--gameType=" + gameType);
                    if (gameType == (int)GAME_TYPE.NORMAL || gameType == (int)GAME_TYPE.SCATTER)
                    {
                        autoSpinRemain = 0;
                    }
                    lbAutoRemain.gameObject.SetActive(false);
                    setStateSpin(gameState);
                    setStateBtnSpin();
                }
                timeHoldSpin = 0;
                isHoldSpin = false;
            }
            else //show node auto
            {
                isHoldSpin = false;
                timeHoldSpin = 0;
                if (gameState == GAME_STATE.PREPARE || gameState == GAME_STATE.JOIN_GAME)
                {
                    onShowNodeAuto();
                }
            }
            Debug.Log("timeHoldSpin=" + timeHoldSpin);
        }
        isHoldSpin = false;

    }
    protected void setBetLevel(int betInit = 0)
    {
        //validBetLevels.Clear();
        //for (int i = 0, l = betLevels.Count; i < l; i++)
        //{
        //    if (betLevels[i] <= agPlayer)
        //    {
        //        validBetLevels.Add(betLevels[i]);
        //    }
        //}
        validBetLevels = betLevels;
        if (currentBetLevel < 0)
        {
            lbBetLevel.text = Globals.Config.FormatNumber(betInit);
        }
        else
        {
            lbBetLevel.text = Globals.Config.FormatNumber(validBetLevels[currentBetLevel]);
        }


        if (currentBetLevel == validBetLevels.Count - 1)
        {
            imgStateBet.sprite = sprStateBets[1];
        }
        else
        {
            imgStateBet.sprite = sprStateBets[0];
        }
        imgStateBet.SetNativeSize();
        setJackpotInfo();

        //if (betInit != 0)
        //{
        //    if (validBetLevels.Count > 0)
        //    {
        //        for (int i = validBetLevels.Count - 1; i > 0; i--)
        //        {
        //            if (betInit == validBetLevels[i])
        //            {
        //                currentBetLevel = i;
        //                break;
        //            }
        //            else if (betInit > validBetLevels[i])
        //            {
        //                while (betLevels[currentBetLevel] != validBetLevels[i])
        //                {
        //                    currentBetLevel--;
        //                }
        //                break;
        //            }
        //            else
        //            {
        //                //Debug.Log("Chay vao day");
        //            }
        //        }
        //lbBetLevel.text = Globals.Config.FormatNumber(validBetLevels[currentBetLevel]);
        //setJackpotInfo();
        //    }
        //    else
        //    {
        //        currentBetLevel = -1;
        //        lbBetLevel.text = "0";
        //        setJackpotInfo();
        //    }
        //}


    }
    public int getBetValue()
    {
        return validBetLevels[currentBetLevel];
    }
    protected void setJackpotInfo()
    {
        if (currentBetLevel >= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                listLbJackpot[i].setValue(validBetLevels[currentBetLevel] * jackpotLevel[i], true, 0.2f);
            }
        }
        else
        {

            for (int i = 0; i < 4; i++)
            {
                listLbJackpot[i].setValue(0);
            }
        }

    }
    public void hideAllSymbol(int ignoreId = -1)
    {
        listCollum.ForEach((col) =>
        {
            col.hideAllSymbol(ignoreId);
        });
    }
    public void activeAllSymbol()
    {
        listCollum.ForEach((col) =>
        {
            col.activeAllSymbol();
        });
    }
    protected void drawLines(List<Vector2> listPos, Color colorLine)
    {
        Vector2 startPos = new Vector2(listPos[0].x - 80, listPos[0].y);
        Vector2 lastPos = new Vector2(listPos[listPos.Count - 1].x + 80, listPos[listPos.Count - 1].y);
        listPos.Insert(0, startPos);
        listPos.Add(lastPos);
        GameObject lineStraight = Instantiate(linePrf, lineContainer.transform);

        RectTransform rt = lineStraight.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0);
        lineStraight.SetActive(true);
        listLineStraight.Add(lineStraight);
        LineController lineComp = lineStraight.GetComponent<LineController>();
        lineComp.drawLine(listPos, colorLine);
    }
    protected void setStateSpin(GAME_STATE state)
    {
        gameState = state;
        switch (gameState)
        {
            case GAME_STATE.SPINNING:
                {
                    animBtnSpin.color = spintype == SPIN_TYPE.NORMAL ? Color.gray : Color.white;
                    break;
                }
            case GAME_STATE.SHOWING_RESULT:
                {
                    animBtnSpin.color = Color.white;
                    break;
                }
            case GAME_STATE.JOIN_GAME:
                {
                    animBtnSpin.color = Color.white;
                    break;
                }
            case GAME_STATE.PREPARE:
                {
                    animBtnSpin.color = Color.white;
                    break;
                }
        }
    }
    public async Task showResultMoneyAnim(string path, string animName, long chipWin, Vector2 posLbMoney)
    {

        spineJPWinTask = new Task(() => { spineJPWinTask = null; });
        Action<SkeletonDataAsset> cb = async (skeData) =>
        {
            spineJackpotWin.skeletonDataAsset = skeData;// UIManager.instance.loadSkeletonData(path);
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            spineBgMoney.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_SPECICAL_WIN);
            spineBgMoney.Initialize(true);
            spineBgMoney.AnimationState.SetAnimation(0, "money", true);
            spineBgMoney.gameObject.SetActive(true);
            effectContainer.SetActive(true);
            spineJackpotWin.transform.parent.gameObject.SetActive(true);
            spineJackpotWin.gameObject.SetActive(true);
            spineJackpotWin.Initialize(true);
            spineJPWinTrack = spineJackpotWin.AnimationState.SetAnimation(0, animName, false);
            spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(true);
            lbJPWin.gameObject.SetActive(true);
            lbJPWin.transform.localPosition = posLbMoney;
            lbJPWin.ResetValue();
            AudioSource soundMoney = SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_START);
            lbJPWin.setValue(chipWin, true, 2.0f, "", () =>
            {
                soundMoney?.Stop();
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_END);
            });
            await Task.Delay(TimeSpan.FromSeconds(spineJackpotWin.Skeleton.Data.FindAnimation(animName).Duration + 1.0f));
            Debug.Log("spineJPWinTask=" + spineJPWinTask);
            spineJPWinTask.Start();
            spineJackpotWin.transform.parent.gameObject.SetActive(false);
            spineJackpotWin.gameObject.SetActive(false);
            effectContainer.SetActive(false);
            spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(true);
        };

        UnityMainThread.instance.AddJob(() =>
        {
            StartCoroutine(UIManager.instance.loadSkeletonDataAsync(path, cb));
        });
        await spineJPWinTask;
    }

    protected Task showSpineSpecialWin(WIN_TYPE wintype, long chipWin = 0)
    {
        spineSpecialWinTask = new Task(() =>
        {
            spineSpecialWinTask = null;
            Debug.Log("showSpineSpecialWin start:" + winType);
        });

        if (wintype != WIN_TYPE.NORMAL)
        {
            Action<SkeletonDataAsset> cb = async (skeData) =>
            {
                string animName = "";
                spineSpecialWin.skeletonDataAsset = skeData;// UIManager.instance.loadSkeletonData(PATH_ANIM_SPECICAL_WIN);
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
                float timeRun = 1.0f;
                switch (wintype)
                {
                    case WIN_TYPE.NICE_WIN:
                        {
                            animName = "nicewin";
                            break;
                        }
                    case WIN_TYPE.BIGWIN:
                        {
                            animName = "bigwin";
                            timeRun = 2.5f;
                            break;
                        }
                    case WIN_TYPE.HUGEWIN:
                        {
                            animName = "hugewin";
                            timeRun = 3.5f;
                            break;
                        }
                    case WIN_TYPE.MEGAWIN:
                        {
                            animName = "megawin";
                            timeRun = 5.5f;
                            break;
                        }
                }
                effectContainer.SetActive(true);
                spineSpecialWin.gameObject.SetActive(true);
                spineSpecialWin.Initialize(true);
                spineBgMoney.skeletonDataAsset = spineSpecialWin.skeletonDataAsset;
                spineBgMoney.Initialize(true);
                spineBgMoney.AnimationState.SetAnimation(0, "money", true);
                spineBgMoney.gameObject.SetActive(true);
                spineSpecialWinTrack = spineSpecialWin.AnimationState.SetAnimation(0, animName, false);
                spineSpecialWin.transform.Find("btnConfirm").gameObject.SetActive(true);
                spineSpecialWin.transform.Find("btnConfirm").localPosition = new Vector2(0, -299);
                spineSpecialWin.transform.parent.gameObject.SetActive(true);
                lbSpecicalWin.gameObject.SetActive((chipWin != 0));
                lbSpecicalWin.transform.localPosition = new Vector2(0, -165);
                lbSpecicalWin.ResetValue();
                AudioSource soundBig = SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.BIGWIN_START);
                lbSpecicalWin.setValue(chipWin, true, timeRun, "", () =>
                {
                    if (soundBig != null && soundBig.isPlaying)
                    {
                        soundBig.Stop();
                    }
                    SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.BIGWIN_END);
                });

                spineSpecialWin.AnimationState.Complete += delegate
                {
                    hideSpineSpecical();
                };
            };
            UnityMainThread.instance.AddJob(() =>
             {
                 StartCoroutine(UIManager.instance.loadSkeletonDataAsync(PATH_ANIM_SPECICAL_WIN, cb));
             });
        }
        else
        {
            DOTween.Sequence().AppendInterval(0.2f).AppendCallback(() =>
            {
                spineSpecialWinTask.Start();
            });
        }
        return spineSpecialWinTask;

    }
    public async Task showSpineJackpotWin(long chipWin)
    {

        Action<SkeletonDataAsset> cb = async (skeData) =>
        {
            string animName = "";
            spineJackpotWin.skeletonDataAsset = skeData;// UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/LuckyDraw/BigWin/skeleton_SkeletonData");
            spineBgMoney.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_SPECICAL_WIN);
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            switch (winTypeJackpot)
            {
                case WIN_JACKPOT_TYPE.JACKPOT_MINOR:
                    {
                        animName = "minor";
                        break;
                    }
                case WIN_JACKPOT_TYPE.JACKPOT_MAJOR:
                    {
                        animName = "major";
                        break;
                    }
                case WIN_JACKPOT_TYPE.JACKPOT_MEGA:
                    {
                        animName = "mega";
                        break;
                    }
                case WIN_JACKPOT_TYPE.JACKPOT_GRAND:
                    {
                        animName = "grand";
                        break;
                    }
            }
            effectContainer.SetActive(true);
            spineBgMoney.Initialize(true);
            spineBgMoney.AnimationState.SetAnimation(0, "money", true);
            spineBgMoney.gameObject.SetActive(true);
            spineJackpotWin.gameObject.SetActive(true);
            spineJackpotWin.Initialize(true);
            spineJPWinTrack = spineJackpotWin.AnimationState.SetAnimation(0, animName, false);
            lbJPWin.gameObject.SetActive(true);
            lbJPWin.transform.localPosition = new Vector2(0, -70);
            spineJackpotWin.transform.parent.gameObject.SetActive(true);
            spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(false);
            lbJPWin.ResetValue();
            //lbSpecicalWin.setValue(chipWin, true, (spineSpecialWin.Skeleton.Data.FindAnimation(animName).Duration - 1.0f));
            AudioSource soundMoney = SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.WIN_JACKPOT_START);
            lbJPWin.setValue(chipWin, true, 4.0f, "", () =>
            {
                soundMoney.Stop();
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.WIN_JACKPOT_END);
            });
            spineJackpotWin.AnimationState.Complete += delegate
            {
                spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(true);
            };
            if (spintype == SPIN_TYPE.AUTO)
            {
                await Task.Delay(TimeSpan.FromSeconds(5.0f + spineJackpotWin.Skeleton.Data.FindAnimation(animName).Duration));
                if (spineJackpotWin.gameObject.activeSelf)
                {
                    hideSpineJackpot();
                }
            }
        };
        UnityMainThread.instance.AddJob(() =>
           {
               StartCoroutine(UIManager.instance.loadSkeletonDataAsync("GameView/SiXiang/Spine/LuckyDraw/BigWin/skeleton_SkeletonData", cb));
           });
        spineJPWinTask = new Task(() =>
        {
            Debug.Log("showSpineJackpotWin start");
            spineJPWinTask = null;
        });
        await spineJPWinTask;
        //spineSpecialWin.transform.parent.gameObject.SetActive(false);
        //spineSpecialWin.gameObject.SetActive(false);
        //effectContainer.SetActive(false);

    }
    public void hideSpineSpecical()
    {

        spineSpecialWinTask.Start();
        spineSpecialWin.transform.parent.gameObject.SetActive(false);
        spineSpecialWin.gameObject.SetActive(false);
        effectContainer.SetActive(false);
        Debug.Log("hideSpineSpecical");
    }
    public void hideSpineJackpot()
    {
        spineJPWinTask.Start();
        spineJackpotWin.transform.parent.gameObject.SetActive(false);
        spineJackpotWin.gameObject.SetActive(false);
        effectContainer.SetActive(false);
        Debug.Log("hideSpineJackpot");
    }
    public void onClickSkipSpecialSpine()
    {
        if (spineSpecialWinTrack.IsComplete)
        {
            hideSpineSpecical();
        }
        else
        {
            float duration = spineSpecialWin.Skeleton.Data.FindAnimation(spineSpecialWinTrack.Animation.Name).Duration;
            //if(spineSpecialWin.Skeleton.Data.FindAnimation(spineSpecialWinTrack.Animation.Name))
            float currentTime = spineSpecialWin.AnimationState.GetCurrent(0).AnimationTime;
            if (currentTime > duration * 0.6f)
            {

                lbSpecicalWin.setLastValue();
                spineSpecialWinTrack.TrackTime = duration;
                spineSpecialWin.transform.Find("btnConfirm").gameObject.SetActive(false);
            }
            else
            {
                spineSpecialWinTrack.TrackTime = duration * 0.7f;
                lbSpecicalWin.setLastValue();
                spineSpecialWin.transform.Find("btnConfirm").gameObject.SetActive(false);
            }

        }
    }
    public void onClickSkipSpineJackPot()
    {
        if (spineJPWinTrack.IsComplete)
        {
            hideSpineJackpot();
        }
        else
        {
            float duration = spineJackpotWin.Skeleton.Data.FindAnimation(spineJPWinTrack.Animation.Name).Duration;
            float currentTime = spineJackpotWin.AnimationState.GetCurrent(0).AnimationTime;
            if (currentTime > duration * 0.6f)
            {

                lbJPWin.setLastValue();
                spineJPWinTrack.TrackTime = duration;
                spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(false);
            }
            else
            {
                spineJPWinTrack.TrackTime = duration * 0.7f;
                lbJPWin.setLastValue();
                spineJackpotWin.transform.Find("btnConfirm").gameObject.SetActive(false);
            }
        }
    }
    public void updateWinAmount(long amount)
    {
        UnityMainThread.instance.AddJob(() =>
        {
            if (oldWinAmount != amount)
            {
                //Globals.Config.tweenNumberToNumber(lbChipWins, amount, oldWinAmount);
                lbChipWins.setValue(amount, true);
                winAmount = amount;
            }
            else
            {
                lbChipWins.Text = Globals.Config.FormatNumber(winAmount);
            }

        });

    }
    public override void onClickBack()
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        var subView = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/GroupMenu"), transform);

        subView.transform.localScale = Vector3.one;

    }
    public override void onClickRule()
    {
        GameObject ruleView = Instantiate(rulePr, transform);

        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
    }
}
