using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using Globals;

public class SiXiangLuckyDrawView : MonoBehaviour
{
    public static SiXiangLuckyDrawView instance = null;
    // Start is called before the first frame update
    [SerializeField]
    public GameObject itemCollum, collumContainer;

    [SerializeField]
    public SkeletonGraphic spineResult;

    [SerializeField]
    public Button btnCollect;

    [SerializeField]
    public TextNumberControl lbTotalWin;

    [HideInInspector]
    private Task luckyDrawTask;
    private SiXiangView gameView;

    private LuckyDrawItem currenItem;
    private List<LuckyDrawItem> listItem = new List<LuckyDrawItem>();
    private long winAmount = 0;
    private long userAmount = 0;
    private bool isFinished = false;
    private bool isInitView = false;
    private int jackpotType = 0;
    private bool isAutoPlay = true;
    private bool isSelectBonusGame = false;
    private bool canClick = true;
    private List<LuckyDrawItem> listItemRemain = new List<LuckyDrawItem>();

    void Awake()
    {
        SiXiangLuckyDrawView.instance = this;
        for (int i = 1; i < 5; i++)
        {
            GameObject itemCol = Instantiate(itemCollum, collumContainer.transform);

        }
        for (int i = 0; i < 5; i++)
        {
            Transform col = collumContainer.transform.GetChild(i);
            for (int j = 0; j < col.transform.childCount; j++)
            {
                listItem.Add(col.GetChild(j).GetComponent<LuckyDrawItem>());
            }
        }
        listItemRemain.AddRange(listItem);
    }
    [HideInInspector]
    public enum TYPE_ITEM
    {
        MINOR = 1,
        MAJOR = 2,
        MEGA = 3,
        GRAND = 4,
        NORMAL = 0
    }

    // Update is called once per frame
    public Task Show(SiXiangView SiXiangView)
    {

        gameView = SiXiangView;
        gameView.gameState = SiXiangView.GAME_STATE.SHOWING_RESULT;
        gameView.lbChipWins.ResetValue();
        gameView.infoBar.setStateWin("totalWin");
        luckyDrawTask = new Task(() => { });
        DOTween.Sequence(luckyDrawTask);
        DOTween.Sequence(transform)
            .AppendInterval(10.0f)
            .AppendCallback(() =>
            {
                onPlayAuto();
            }).SetId("autoPlay");
        return luckyDrawTask;
    }
    public void setInitView(JObject dataInit, SiXiangView SiXiangView)
    {
        gameView = SiXiangView;
        isInitView = true;
        List<JObject> initView = dataInit["cards"].ToObject<List<JObject>>();
        for (int i = 0; i < initView.Count; i++)
        {
            setInfoItem(initView[i]);
        }
        isInitView = false;
        winAmount = (long)dataInit["winAmount"];
        userAmount = (long)dataInit["userAmount"];
        gameView.infoBar.setStateWin("totalWin");
        gameView.lbChipWins.ResetValue();
        gameView.lbChipWins.setValue(winAmount, true);
    }
    public void onClickItem(LuckyDrawItem item)
    {
        if (canClick)
        {
            SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.CLICK);
            Debug.Log("onClickItem:" + item.typeItem);
            SocketSend.sendPackageRapidPay(Globals.ACTION_SLOT_SIXIANG.luckyDraw, listItem.IndexOf(item).ToString());
            listItemRemain.Remove(item);
            isAutoPlay = false;
            canClick = false;
            DOTween.Sequence().AppendInterval(0.75f).AppendCallback(() =>
            {
                canClick = true;
            });
        }

    }
    private void onPlayAuto()
    {
        Debug.Log("onPlayAuto:" + isAutoPlay);
        if (isAutoPlay)
        {
            int randomIndex = Random.Range(0, listItemRemain.Count);
            onClickItem(listItemRemain[randomIndex]);
        }

    }
    public async void setInfoItem(JObject data)
    {
        if (data.ContainsKey("userAmount"))
        {
            userAmount = (long)data["userAmount"];
        }
        if (isInitView == false)
        {
            winAmount = (long)data["winAmount"];
            gameView.lbChipWins.setValue(winAmount, true);
        }
        if (data.ContainsKey("isSelectBonusGame"))
        {
            isSelectBonusGame = (bool)data["isSelectBonusGame"];
        }
        await listItem[(int)data["index"]].setResult(data);
        if (data.ContainsKey("isFinished"))
        {
            isFinished = (bool)data["isFinished"];
        }
        jackpotType = (int)data["jackpot"];
        DOTween.Kill("autoPlay");
        if (isFinished)
        {
            //DOTween.Sequence().AppendInterval(1.0f).AppendCallback(async () =>
            //{
            //    if (jackpotType != 0)
            //    {
            //        await showEffectItemJP();
            //    }
            //    showAnimResult();
            //});
            if (jackpotType != 0)
            {
                await showEffectItemJP();
            }
            showAnimResult();
        }
        else
        {
            DOTween.Sequence(transform)
                .AppendInterval(4.0f)
                .AppendCallback(() =>
                {
                    isAutoPlay = true;
                    onPlayAuto();
                }).SetId("autoPlay");
        }

    }

    public async void onClickCollect()
    {
        Debug.Log("onClickCollect");
        DOTween.Kill("autoEnd");
        spineResult.transform.DOScale(new Vector2(0.8f, 0.8f), 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            spineResult.transform.parent.gameObject.SetActive(false);
        });
        JObject dataEnd = new JObject();
        dataEnd["winAmount"] = winAmount;
        dataEnd["userAmount"] = userAmount;
        dataEnd["gameType"] = (int)SiXiangView.GAME_TYPE.LUCKY_DRAW;
        dataEnd["isSelectBonusGame"] = isSelectBonusGame;
        //await gameView.showAnimCutScene();
        luckyDrawTask.Start();
        gameView.setStateNodeGameForLuckyDraw(true);
        btnCollect.gameObject.SetActive(false);
        Destroy(gameObject);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.CLICK);
        await gameView.endMinigame(dataEnd);
    }
    public async void showAnimResult()
    {
        Task loadAsynTask = new Task(() => { });
        long chipWin = 0;
        string soundPathStart = "";
        string soundPathEnd = "";
        if (jackpotType != 0)
        {
            spineResult.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/LuckyDraw/BigWin/skeleton_SkeletonData");
            chipWin = gameView.getJackpotValue(SiXiangView.WIN_JACKPOT_TYPE.NONE, jackpotType);
            soundPathStart = Globals.SOUND_SLOT_BASE.WIN_JACKPOT_START;
            soundPathEnd = Globals.SOUND_SLOT_BASE.WIN_JACKPOT_END;
        }
        else
        {
            chipWin = winAmount;
            spineResult.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/BigWinGoldPick/skeleton_SkeletonData");
            soundPathStart = Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_START;
            soundPathEnd = Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_END;
        }
        await Task.Delay(1000);
        spineResult.transform.parent.gameObject.SetActive(true);
        spineResult.Initialize(true);
        spineResult.AnimationState.SetAnimation(0, getAnimResultName(), false);

        float duration = spineResult.Skeleton.Data.FindAnimation(getAnimResultName()).Duration;
        AudioSource soundCount = SoundManager.instance.playEffectFromPath(soundPathStart);
        lbTotalWin.setValue(chipWin, true, duration * 0.85f, "", () =>
        {
            soundCount.Stop();
            SoundManager.instance.playEffectFromPath(soundPathEnd);
        });
        await Task.Delay(TimeSpan.FromSeconds(duration));
        btnCollect.gameObject.SetActive(true);
        if (gameView.spintype == SiXiangView.SPIN_TYPE.AUTO)
        {
            DOTween.Sequence()
                .SetId("autoEnd")
                .AppendInterval(3.0f)
                .AppendCallback(() =>
                {
                    onClickCollect();
                });
        }

    }
    private void OnDestroy()
    {
        DOTween.Kill("autoEnd");
    }
    private async Task showEffectItemJP()
    {
        SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.LUCKYDRAW_WIN_JACKPOT);
        listItem.ForEach(item =>
        {
            if ((int)item.typeItem == jackpotType)
            {

                item.showEffectWinJp();
            }
        });
        await Task.Delay(2000);
    }
    private string getAnimResultName()
    {
        string animName = "";
        switch (jackpotType)
        {
            case (int)TYPE_ITEM.MINOR:
                {
                    animName = "minor";
                    break;
                }
            case (int)TYPE_ITEM.MAJOR:
                {
                    animName = "major";
                    break;
                }
            case (int)TYPE_ITEM.MEGA:
                {
                    animName = "mega";
                    break;
                }
            case (int)TYPE_ITEM.GRAND:
                {
                    animName = "grand";
                    break;
                }
            default:
                animName = "eng";
                break;

        }
        return animName;
    }
}
