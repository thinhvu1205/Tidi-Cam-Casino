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
using Socket.Quobject.EngineIoClientDotNet.Modules;
using Globals;
using UnityEngine.Events;

public class SiXiangLuckyGoldView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public GameObject itemGold;

    [SerializeField]
    public Transform itemContainer;
    [SerializeField]
    public TextMeshProUGUI lbRemainPick;
    [SerializeField]
    public TextNumberControl lbTotalWin;
    [SerializeField]
    public Button btnCollect;
    [SerializeField]
    public SkeletonGraphic animResult;
    [HideInInspector]
    private bool isRaining = true;
    private TextNumberControl lbWinAmount;
    private List<GameObject> itemPool = new List<GameObject>();
    private RectTransform itemConTainerRect;
    private SiXiangView gameView;
    private Task luckyGoldTask;
    private GameObject currentItemClick;
    public int remainPick = 20;
    private bool isFinished = false;
    private long totalWinAmount = 0;
    private long userAmount = 0;
    private bool canClick = true;
    private List<GameObject> listItem = new List<GameObject>();
    public bool isAutoPlay = true;
    private bool isSelectBonusGame = false;
    [SerializeField] List<Material> materialText = new List<Material>(); //0 green,1 gold

    void Start()
    {
        itemConTainerRect = itemContainer.GetComponent<RectTransform>();

    }
    private void OnEnable()
    {
        canClick = true;
        initRainItem();
        DOTween.Sequence().AppendInterval(7.0f).AppendCallback(() =>
        {
            onAutoPlay();
        }).SetId("autoPlay");

    }
    private void onAutoPlay()
    {
        Debug.Log("onAutoPlay");
        GameObject itemAuto = listItem.Find((item) =>
            {
                return (item.transform.localPosition.y > 0 && item.transform.localPosition.y < 250 && item.gameObject.activeSelf && (item.transform.localPosition.x > -350 && item.transform.localPosition.x < 350));
            });
        if (itemAuto != null)
        {
            onClickItemGold(itemAuto);
        }
        else
        {
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                onAutoPlay();
            }).SetId("autoPlay");
        }
    }
    public Task Show(SiXiangView SiXiangView)
    {
        GameObject bottom = Instantiate(SiXiangView.Instance.transform.Find("Bottom").gameObject, transform);

        lbWinAmount = bottom.transform.Find("lbTotalWin").GetComponent<TextNumberControl>();
        lbWinAmount.Text = Globals.Config.FormatNumber(SiXiangView.Instance.winAmount);
        Destroy(bottom.transform.Find("infoBar").gameObject);
        bottom.transform.SetSiblingIndex(transform.Find("EffectContainer").GetSiblingIndex() - 1);
        lbRemainPick.text = remainPick + " Remaining Picks";
        gameView = SiXiangView;
        luckyGoldTask = new Task(() => { });
        SiXiangView.gameState = BaseSlotView.GAME_STATE.SHOWING_RESULT;
        return luckyGoldTask;
    }
    private void initRainItem()
    {
        DOTween.Sequence()
            .AppendCallback(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject item = createItemGold();
                item.transform.localPosition = new Vector2(Random.Range(-600, 600), Random.Range(400, 500));
                item.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                moveItem(item, i);
            }
        })
            .AppendInterval(3.5f).SetLoops(-1).SetId("initRainItem");

    }
    private async void moveItem(GameObject item, int index)
    {
        LuckyGoldItem itemComp = item.GetComponent<LuckyGoldItem>();
        await Task.Delay(TimeSpan.FromSeconds(index * 0.2f));
        var time = Random.Range(5, 7);
        item.transform.DOBlendableLocalMoveBy(new Vector3(0, -700), time, true).OnUpdate(() =>
        {
            if (item.transform.localPosition.y < -300)
            {
                removeItem(item);
            }
        }).SetLoops(-1, LoopType.Incremental);
        item.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, Random.Range(1, 3) < 2 ? 360 : -360), time, RotateMode.FastBeyond360).OnUpdate(() =>
        {
            itemComp.lbMoney.transform.localEulerAngles = new Vector3(0, 0, -item.transform.localEulerAngles.z);
        }).SetLoops(-1, LoopType.Incremental);
    }
    int index = 0;
    public void onClickItemGold(GameObject item)
    {
        if (canClick)
        {
            Debug.Log("OnClickItemGold");

            currentItemClick = item;
            SocketSend.sendGoldPickSlotSixiang(Globals.ACTION_SLOT_SIXIANG.goldPick);
            DOTween.Kill("autoPlay");
            canClick = false;

            DOTween.Sequence()
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    canClick = true;
                })
                .AppendInterval(3.0f).AppendCallback(() =>
                {
                    onAutoPlay();
                }).SetId("autoPlay");
        }
        else
        {
            Debug.Log("Chua Dc Click");
        }

    }
    public void setResult(JObject data)
    {
        if (currentItemClick != null)
        {

            LuckyGoldItem currentItemComp = currentItemClick.GetComponent<LuckyGoldItem>();
            SkeletonGraphic spineItem = currentItemComp.spine;
            currentItemComp.bgImage.enabled = false;
            currentItemComp.btnItem.interactable = false;
            long coinAmount = (long)data["coinAmount"];
            userAmount = (long)data["userAmount"];
            isFinished = (bool)data["isFinished"];
            totalWinAmount = (int)data["winAmount"];
            isSelectBonusGame = (bool)data["isSelectBonusGame"];

            if (coinAmount != 0)
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.CLICK_ITEM_WIN);
                TextMeshProUGUI lbChipWin = currentItemComp.lbMoney;
                lbChipWin.gameObject.SetActive(true);
                switch ((int)data["jackpot"])
                {
                    case 0:
                        lbChipWin.text = Globals.Config.FormatMoney(coinAmount, true);
                        break;
                    case 1:
                        lbChipWin.text = "MINOR";
                        break;
                    case 2:
                        lbChipWin.text = "MAJOR";
                        break;
                    case 3:
                        lbChipWin.text = "MEGA";
                        break;
                    case 4:
                        lbChipWin.text = "GRAND";
                        break;
                }
                if ((int)data["jackpot"] == 0)
                {
                    lbChipWin.fontMaterial = materialText[0];
                }
                else
                {
                    lbChipWin.fontMaterial = materialText[1];
                }
                //Globals.Config.tweenNumberToNumber(SiXiangView.instance.lbChipWins, (int)data["winAmount"], totalWinAmount);
                SiXiangView.Instance.lbChipWins.setValue(totalWinAmount, true);
                lbWinAmount.setValue(totalWinAmount, true);
                lbChipWin.transform.localScale = new Vector2(0, 0);
                lbChipWin.transform.DOScale(new Vector2(1, 1), 0.2f).SetEase(Ease.OutBack);
                spineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/LuckyGoldItem/skeleton_SkeletonData");
                spineItem.Initialize(true);
                spineItem.AnimationState.SetAnimation(0, "animation", false);
            }
            else
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.CLICK_ITEM_MISS);
                spineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/LuckyGoldTryAgain/skeleton_SkeletonData");
                spineItem.Initialize(true);
                spineItem.AnimationState.SetAnimation(0, "eng", false);
            }
            spineItem.gameObject.SetActive(true);
            remainPick = (int)data["numberOfPick"];
            lbRemainPick.text = remainPick + " Remaining Picks";
            currentItemComp.bgImage.raycastTarget = false;
            currentItemComp.itemImage.gameObject.SetActive(false);
            currentItemClick.transform.SetAsLastSibling();
            if (isFinished)
            {
                //Globals.Config.tweenNumberToNumber(SiXiangView.instance.lbChipWins, (int)data["winAmount"], totalWinAmount);
                totalWinAmount = (long)data["winAmount"];
                DOTween.Sequence()
                    .AppendInterval(2.0f)
                    .AppendCallback(() =>
                    {
                        showAnimResult();
                    });
                DOTween.Kill("initRainItem");
                DOTween.Kill("autoPlay");
            }
            currentItemClick = null;
        }
    }
    private async void showAnimResult()
    {
        animResult.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/BigWinGoldPick/skeleton_SkeletonData");
        animResult.Initialize(true);
        animResult.transform.parent.gameObject.SetActive(true);
        animResult.AnimationState.SetAnimation(0, "eng", false);
        //Globals.Config.tweenNumberToNumber(lbTotalWin, totalWinAmount, 0, animResult.Skeleton.Data.FindAnimation("cam").Duration * 0.85f);
        btnCollect.gameObject.SetActive(false);
        AudioSource soundMoney = SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_START);
        lbTotalWin.setValue(totalWinAmount, true, 2.0f, "", () =>
        {
            soundMoney.Stop();
            SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_END);
        });
        await Task.Delay(2000);
        btnCollect.gameObject.SetActive(true);
        if (gameView.spintype == BaseSlotView.SPIN_TYPE.AUTO)
        {
            DOTween.Sequence()
                .AppendInterval(3.0f)
                .AppendCallback(() =>
                {
                    if (gameObject.activeSelf)
                    {
                        onClickCollect();
                    }
                }).SetId("autoEnd");
        }
    }
    private GameObject createItemGold()
    {
        GameObject item;
        if (itemPool.Count == 0)
        {
            //itemPool.Add();
            item = Instantiate(itemGold, itemContainer);

        }
        else
        {
            item = itemPool[0];
            itemPool.RemoveAt(0);
        }
        LuckyGoldItem itemComp = item.GetComponent<LuckyGoldItem>();
        item.SetActive(true);
        itemComp.bgImage.enabled = true;
        itemComp.spine.gameObject.SetActive(false);
        itemComp.btnItem.interactable = true;
        itemComp.lbMoney.gameObject.SetActive(false);
        itemComp.bgImage.raycastTarget = true;
        itemComp.itemImage.gameObject.SetActive(true);
        if (!listItem.Contains(item))
        {
            listItem.Add(item);
        }
        item.transform.SetAsLastSibling();
        return item;
    }
    private void removeItem(GameObject item)
    {
        item.SetActive(false);
        DOTween.Kill(item.transform);
        itemPool.Add(item);
    }
    public async void onClickCollect()
    {
        DOTween.Kill("autoPlay");
        DOTween.Kill("autoEnd");
        SiXiangView.Instance.winAmount = totalWinAmount;
        SiXiangView.Instance.setWinType(SiXiangView.Instance.winAmount);
        animResult.transform.parent.gameObject.SetActive(false);
        btnCollect.gameObject.SetActive(false);
        //await gameView.showAnimCutScene();
        JObject dataEnd = new JObject();
        dataEnd["winAmount"] = totalWinAmount;
        dataEnd["userAmount"] = userAmount;
        dataEnd["gameType"] = (int)SiXiangView.GAME_TYPE.LUCKY_GOLD;
        dataEnd["isSelectBonusGame"] = isSelectBonusGame;

        gameView.setStateNodeGameForLuckyGold(true);
        luckyGoldTask.Start();
        totalWinAmount = 0;
        remainPick = 20;
        listItem.ForEach(item =>
        {
            removeItem(item);
        });
        listItem.Clear();
        isFinished = false;
        index = 0;
        DOTween.Kill("initRainItem");
        gameObject.SetActive(false);
        await gameView.endMinigame(dataEnd);
    }

}
