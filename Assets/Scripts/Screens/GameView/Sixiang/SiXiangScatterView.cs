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
using Globals;

public class SiXiangScatterView : MonoBehaviour
{
    // Start is called before the first frame update
    public static SiXiangView instance;
    [SerializeField]
    GameObject nodeReel, spriteBg;


    [SerializeField]
    GameObject nodeSpin;

    [SerializeField]
    TextNumberControl lbChipWins;
    [SerializeField]
    List<TextMeshProUGUI> lbGoldValue = new List<TextMeshProUGUI>();

    [SerializeField]
    Button btnCollect;

    [SerializeField]
    Button btnSpin;

    [SerializeField]
    SkeletonGraphic animBgWin, bgLight, animBG;

    [SerializeField]
    SkeletonGraphic animBtnSpin;

    [SerializeField]
    SkeletonGraphic animResultSpin;

    [HideInInspector]
    private bool isPrepareStop = false;
    public Task scatterTask;
    [HideInInspector]
    private int typeResult = 5;
    private long winAmount = 0, userAmount = 0;
    private bool isWaitForAutoSpin = true;
    [HideInInspector]
    private enum RESULT_SPIN
    {

        COIN_1 = 0,
        GOLD_PICK = 7,
        COIN_2 = 6,
        RAPID_PAY = 5,
        COIN_4 = 4,
        LUCKY_DRAW = 3,
        COIN_5 = 2,
        DRAGON_PEARL = 1,
    }
    [HideInInspector]
    SiXiangView gameView;
    List<int> rateGold = new List<int> { 3, 6, 10, 15 };
    private void Awake()
    {
        //SiXiangView.instance = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        isWaitForAutoSpin = true;
        btnSpin.interactable = true;
        bgLight.gameObject.SetActive(true);
        bgLight.Initialize(true);
        bgLight.AnimationState.SetAnimation(0, "light run", true);
        animBtnSpin.Initialize(true);
        animBtnSpin.AnimationState.SetAnimation(0, "spin_anim", true);
        DOTween.Sequence().AppendInterval(10).AppendCallback(() =>
        {
            if (isWaitForAutoSpin)
            {
                onClickSpin("");
            }
        });
    }

    public void Show(SiXiangView SiXiangView)
    {
        gameView = SiXiangView;
        for (int i = 0; i < 4; i++)
        {
            lbGoldValue[i].text = Globals.Config.FormatMoney2(rateGold[i] * SiXiangView.getBetValue(), true);
        }
    }
    public void onClickSpin(string minigameType)
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        btnSpin.interactable = false;
        animBtnSpin.Initialize(true);
        animBtnSpin.AnimationState.SetAnimation(0, "spin normal", true);
        animBG.Initialize(true);
        animBG.AnimationState.SetAnimation(0, "spin", true);
        SocketSend.sendPackageMiniGame(Globals.ACTION_SLOT_SIXIANG.scatterSpin, minigameType);
        isWaitForAutoSpin = false;
    }
    public Task startSpin()
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SCATTER_SPIN);
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SPIN_REEL);
        float startAngle = 0;
        int deltaAngle = typeResult * 45;
        int totalAngle = 4320 + deltaAngle;
        DOTween.To(() => startAngle, x => startAngle = x, totalAngle, 5.0f).OnUpdate(() =>
        {
            nodeReel.transform.localEulerAngles = new Vector3(0, 0, startAngle);
            if (startAngle > 3000 && isPrepareStop == false)
            {
                prepareStop();
            }
        }).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            animBgWin.Initialize(true);
            animBgWin.AnimationState.SetAnimation(0, "khung eat", true);
            animBgWin.gameObject.SetActive(true);
            bgLight.gameObject.SetActive(false);
            animBG.AnimationState.SetAnimation(0, "normal", true);
            SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SCATTER_SYMBOL);
            preShowResult();
        });
        scatterTask = new Task(() => { });
        return scatterTask;
    }
    public async Task handleScatterSpin(JObject data)
    {
        int reward = (int)data["reward"];
        userAmount = (long)data["userAmount"];
        winAmount = (int)data["winAmount"];
        switch (reward)
        {
            case 0:
                typeResult = 1;
                break; // dragon pearl
            case 1:
                typeResult = 3;
                break; // lucky draw
            case 2:
                typeResult = 7;
                break; // gold pick - (1 + 8)
            case 3:
                typeResult = 5;
                break; // rapid pay - (3 + 8)
            case 4:
                typeResult = 4; // x3
                break;
            case 5:
                typeResult = 6; // x6 - (2 + 8)
                break;
            case 6:
                typeResult = 0; // x10 - (0 + 8)
                break;
            case 7:
                typeResult = 2; // x15
                break;
        }
        await startSpin();
    }
    private void prepareStop()
    {
        isPrepareStop = true;
        nodeSpin.transform.DOLocalMoveY(-331, 1.0f).SetEase(Ease.InSine);
        nodeSpin.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 1.0f).SetEase(Ease.InSine);

    }
    private async void preShowResult()
    {
        nodeSpin.transform.DOLocalMoveY(-39, 1.0f).SetEase(Ease.OutSine);
        nodeSpin.transform.DOScale(new Vector3(1.0f, 1.0f, 1), 1.0f).SetEase(Ease.OutSine).SetId("nodeSpin");
        Tween nodeSpinTween = DOTween.TweensById("nodeSpin")[0];
        await nodeSpinTween.AsyncWaitForCompletion();
        await Task.Delay(1000);

        await showResultAnim();
    }
    private async Task showResultAnim()
    {
        btnCollect.gameObject.SetActive(false);
        string pathSkeData = "";
        string animName = "";
        if (typeResult % 2 != 0)
        {
            SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.SHOW_ANIMAL);
            switch (typeResult)
            {
                case (int)RESULT_SPIN.DRAGON_PEARL:
                    {
                        pathSkeData = "GameView/SiXiang/Spine/Animal/Dragon/skeleton_SkeletonData";
                        animName = "animation";
                        break;
                    }
                case (int)RESULT_SPIN.GOLD_PICK:
                    {
                        pathSkeData = "GameView/SiXiang/Spine/Animal/Tiger/skeleton_SkeletonData";
                        animName = "3";
                        break;
                    }
                case (int)RESULT_SPIN.RAPID_PAY:
                    {
                        animName = "animation";
                        pathSkeData = "GameView/SiXiang/Spine/Animal/Phoenix/skeleton_SkeletonData";
                        break;
                    }
                case (int)RESULT_SPIN.LUCKY_DRAW:
                    {
                        animName = "animation";
                        pathSkeData = "GameView/SiXiang/Spine/Animal/Turle/skeleton_SkeletonData";
                        break;
                    }
            }
            lbChipWins.gameObject.SetActive(false);

        }
        else
        {
            animName = "eng";
            pathSkeData = "GameView/SiXiang/Spine/WinResult/skeleton_SkeletonData";
            lbChipWins.gameObject.SetActive(true);
            //Globals.Config.tweenNumberToNumber(lbChipWins, winAmount, 0, 2.0f);
            AudioSource soundMoney = SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_START);
            lbChipWins.setValue(winAmount, true, 2.0f, "", () =>
            {
                soundMoney.Stop();
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_END);
            });

        }
        animResultSpin.skeletonDataAsset = UIManager.instance.loadSkeletonData(pathSkeData);
        animResultSpin.Initialize(true);
        animResultSpin.AnimationState.SetAnimation(0, animName, false);
        animResultSpin.transform.parent.gameObject.SetActive(true);
        await Task.Delay((int)animResultSpin.Skeleton.Data.FindAnimation(animName).Duration * 1000);
        if (typeResult % 2 != 0)
        {
            endView();
        }
        else
        {
            btnCollect.gameObject.SetActive(true);
            if (gameView.spintype == BaseSlotView.SPIN_TYPE.AUTO)
            {
                DOTween.Sequence()
                    .AppendInterval(5.0f)
                    .AppendCallback(() =>
                    {
                        endView();
                    })
                    .SetId("autoEnd");
            }
        }
    }
    public void onClickCollect()
    {
        DOTween.Kill("autoEnd");
        endView();
    }
    private async void endView()
    {
        animResultSpin.transform.parent.gameObject.SetActive(false);
        await gameView.showAnimCutScene();

        scatterTask.Start();
        Destroy(gameObject);
        nodeReel.transform.localEulerAngles = Vector3.zero;
        if (typeResult == (int)RESULT_SPIN.COIN_1 || typeResult == (int)RESULT_SPIN.COIN_2 || typeResult == (int)RESULT_SPIN.COIN_4 || typeResult == (int)RESULT_SPIN.COIN_5)
        {
            JObject dataEnd = new JObject();
            dataEnd["winAmount"] = winAmount;
            dataEnd["gameType"] = (int)SiXiangView.GAME_TYPE.SCATTER;
            dataEnd["userAmount"] = userAmount;
            dataEnd["isSelectBonusGame"] = false;
            await SiXiangView.Instance.endMinigame(dataEnd);
        }


    }

}
