using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System.Threading.Tasks;
using System;
using DG.Tweening;
using System.Threading;
using Globals;

public class DragonPearlItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lbChipWin;
    [SerializeField]
    TextMeshProUGUI lbChipFSP;

    [SerializeField] Image BgItem;
    [SerializeField] SkeletonGraphic SpineItem;
    [SerializeField] List<Material> materialText = new List<Material>();
    [HideInInspector] SiXiangDragonPearlView dragonPearlView;
    public Task setInfoTask;
    public Task setInfoTask2;
    public CancellationTokenSource cts_ShowEffectItem;
    private bool isCancelEffect = false;

    private string PATH_ANIM_GOLD = "GameView/SiXiang/Spine/DragonPearl/ItemGold/skeleton_SkeletonData";
    private string PATH_ANIM_LIXI = "GameView/SiXiang/Spine/DragonPearl/Lixi/skeleton_SkeletonData";
    private string PATH_ANIM_THANHLONG = "GameView/SiXiang/Spine/DragonPearl/ThanhLong/skeleton_SkeletonData";
    private string PATH_ANIM_BACHHO = "GameView/SiXiang/Spine/DragonPearl/BachHo/skeleton_SkeletonData";
    private string PATH_ANIM_CHUTUOC = "GameView/SiXiang/Spine/DragonPearl/ChuTuoc/skeleton_SkeletonData";
    private string PATH_ANIM_HUYENVU = "GameView/SiXiang/Spine/DragonPearl/HuyenVu/skeleton_SkeletonData";
    void Start()
    {

    }

    public Task setInfo(JObject data, SiXiangDragonPearlView dpView)
    {
        cts_ShowEffectItem = SiXiangView.Instance.getCancelToken();
        dragonPearlView = dpView;
        Task setInfoItemTask = new Task(() =>
        {

        });
        UnityMainThread.instance.AddJob(async () =>
        {

            if ((int)data["item"] != 1)
            {

                Action<SkeletonDataAsset> cb = async (skeData) =>
                {
                    try
                    {
                        if ((bool)data["isDoubled"] == true) //x2
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1.0f), cts_ShowEffectItem.Token);
                        }
                        if ((bool)data["isBonusSpin"] == true)//add item
                        {
                            await Task.Delay(TimeSpan.FromSeconds(2.0f), cts_ShowEffectItem.Token);
                            Vector2 posChuTuoc = dragonPearlView.getPosSymbolChuTuoc();
                            Vector2 posItem = dragonPearlView.getPosItem((int)data["col"], (int)data["row"]);
                            GameObject itemGold = Instantiate(dragonPearlView.itemInitGold, dragonPearlView.transform);

                            itemGold.transform.localPosition = dragonPearlView.transform.InverseTransformPoint(posChuTuoc);
                            itemGold.SetActive(true);
                            itemGold.transform.localScale = Vector2.one;
                            itemGold.transform.DOLocalMove(dragonPearlView.transform.InverseTransformPoint(posItem), 1.0f).SetEase(Ease.OutSine).OnComplete(() =>
                            {
                                Destroy(itemGold);
                            });
                            await Task.Delay(TimeSpan.FromSeconds(1.0f), cts_ShowEffectItem.Token);
                        }
                        BgItem.enabled = true;
                        SpineItem.gameObject.SetActive(true);
                        SpineItem.skeletonDataAsset = skeData;
                        SpineItem.Initialize(true);
                        SpineItem.AnimationState.SetAnimation(0, "start", false);
                        SpineItem.transform.localPosition = Vector2.zero;
                        SpineItem.transform.localScale = new Vector2(1, 1);
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("start").Duration), cts_ShowEffectItem.Token);
                        SpineItem.AnimationState.SetAnimation(0, "rung", false);
                        SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.PEARL_Item_Normal);
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("rung").Duration / 2), cts_ShowEffectItem.Token);
                        lbChipWin.fontMaterial = materialText[0];
                        lbChipWin.gameObject.SetActive(true);
                        int itemWinAmount = (int)data["winAmount"];
                        lbChipWin.text = Globals.Config.FormatMoney(itemWinAmount, true);
                        lbChipWin.transform.localScale = Vector2.zero;
                        lbChipWin.transform.DOScale(Vector2.one, 0.2f).SetEase(Ease.OutBack);
                        SpineItem.AnimationState.SetAnimation(0, "normal", true);

                        setInfoItemTask.Start();
                    }
                    catch (SystemException errr)
                    {
                        Debug.Log("errr:" + errr);
                    }

                };
                UnityMainThread.instance.AddJob(() =>
                {
                    UIManager.instance.loadSkeletonDataAsync(PATH_ANIM_GOLD, cb);
                });

            }
            else
            {
                try
                {
                    //SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.PEARL_ITEM);
                    string soundSymbol = Globals.SOUND_SLOT_BASE.PEARL_ITEM;
                    BgItem.enabled = true;
                    string pathEye = "";
                    switch ((int)data["luckyMoney"])
                    {
                        case 1:
                            pathEye = PATH_ANIM_HUYENVU;
                            soundSymbol = Globals.SOUND_SLOT_BASE.PEARL_Phoenix;
                            break; // + spin: numberOfBonusSpins
                        case 2:
                            pathEye = PATH_ANIM_BACHHO;
                            soundSymbol = Globals.SOUND_SLOT_BASE.PEARL_Tiger;
                            break; // x2 gia tri tat ca cac o
                        case 3:
                            pathEye = PATH_ANIM_CHUTUOC;
                            soundSymbol = Globals.SOUND_SLOT_BASE.PEARL_Turtle;
                            break; // roi 3 ngoc bat ki
                        case 4:
                            pathEye = PATH_ANIM_THANHLONG;
                            soundSymbol = Globals.SOUND_SLOT_BASE.PEARL_Dragon;
                            break; // roi 1 ngoc jackpot
                    }

                    SpineItem.gameObject.SetActive(true);
                    SpineItem.transform.localPosition = Vector2.zero;
                    SpineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_LIXI);
                    await Task.Delay(TimeSpan.FromSeconds(0.1f), cts_ShowEffectItem.Token);
                    SpineItem.Initialize(true);
                    SpineItem.AnimationState.SetAnimation(0, "animation", false);
                    await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("animation").Duration), cts_ShowEffectItem.Token);
                    SoundManager.instance.playEffectFromPath(soundSymbol);
                    SpineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData(pathEye);
                    await Task.Delay(TimeSpan.FromSeconds(0.1f), cts_ShowEffectItem.Token);
                    SpineItem.transform.localScale = new Vector2(0.9f, 0.9f);
                    Vector2 posSymbol = SpineItem.transform.parent.InverseTransformPoint(SiXiangView.Instance.getPosSymbol((int)data["col"], (int)data["row"] + 1));
                    SpineItem.transform.localPosition = new Vector2(posSymbol.x + 2, posSymbol.y);
                    //SpineItem.transform.localPosition = transform.parent.InverseTransformPoint(new Vector2(0.2f,-1.31f));
                    SpineItem.Initialize(true);
                    SpineItem.AnimationState.SetAnimation(0, "animation", false);
                    lbChipWin.gameObject.SetActive(false);
                    if ((int)data["luckyMoney"] == 4)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("animation").Duration), cts_ShowEffectItem.Token);
                        SpineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_GOLD);
                        await Task.Delay(TimeSpan.FromSeconds(0.1f), cts_ShowEffectItem.Token);
                        SpineItem.Initialize(true);
                        SpineItem.transform.localScale = Vector2.one;
                        SpineItem.AnimationState.SetAnimation(0, "rung_" + getAnimNameType((int)data["jackpot"]), false);
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("rung_" + getAnimNameType((int)data["jackpot"])).Duration), cts_ShowEffectItem.Token);
                        lbChipWin.gameObject.SetActive(true);
                        Debug.Log("Chay vao day");
                        lbChipWin.fontMaterial = materialText[1];
                        switch ((int)data["jackpot"])
                        {
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
                        SpineItem.AnimationState.SetAnimation(0, "normal_" + getAnimNameType((int)data["jackpot"]), true);
                    }
                    else if ((int)data["luckyMoney"] == 2)
                    {
                        //SpineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_BACHHO);
                        //await Task.Delay(TimeSpan.FromSeconds(0.1f));
                        //SpineItem.Initialize(true);
                        //SpineItem.AnimationState.SetAnimation(0, "animation", false);
                        await dragonPearlView.setDoubleItem();
                    }
                    else if ((int)data["luckyMoney"] == 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("animation").Duration * 1.0f), cts_ShowEffectItem.Token);
                        if (dragonPearlView.isDPSpin)
                        {
                            lbChipFSP.gameObject.SetActive(true);
                            lbChipFSP.alpha = 1.0f;
                            lbChipFSP.transform.localPosition = lbChipFSP.transform.parent.InverseTransformPoint(transform.position);
                            lbChipFSP.fontMaterial = materialText[1];
                            lbChipFSP.text = "+3 freespin";
                            Vector2 posJump = lbChipFSP.transform.parent.InverseTransformPoint(SiXiangView.Instance.infoBar.transform.position);
                            lbChipFSP.transform.DOLocalJump(posJump, 150, 1, 0.5f)
                                .OnComplete(() =>
                                {
                                    lbChipFSP.transform.localPosition = Vector2.zero;
                                    lbChipFSP.gameObject.SetActive(false);
                                    SiXiangView.Instance.updateFreeSpinLeft();
                                });
                            lbChipFSP.DOFade(0, 0.5f).SetEase(Ease.InSine).SetId("fadeEffect");
                            Tween nodeSpinTween = DOTween.TweensById("fadeEffect")[0];
                            await nodeSpinTween.AsyncWaitForCompletion();
                        }
                    }
                    else if ((int)data["luckyMoney"] == 3)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(SpineItem.Skeleton.Data.FindAnimation("animation").Duration), cts_ShowEffectItem.Token);
                        SpineItem.skeletonDataAsset = UIManager.instance.loadSkeletonData(PATH_ANIM_CHUTUOC);
                        await Task.Delay(TimeSpan.FromSeconds(0.1f), cts_ShowEffectItem.Token);
                        SpineItem.Initialize(true);
                        SpineItem.AnimationState.SetAnimation(0, "animation", false);

                    }
                    //setInfoTask.Start();
                    setInfoItemTask.Start();
                }
                catch (SystemException e)
                {
                    Debug.Log("Errr:" + e);
                }
            }


        });
        return setInfoItemTask;
    }
    private string getAnimNameType(int type)
    {
        string name = "";
        switch (type)
        {
            case 1:
                name = "xanh";
                break;
            case 2:
                name = "bien";
                break;
            case 3:
                name = "tim";
                break;
            case 4:
                name = "do";
                break;
        }
        return name;
    }
    public void hideItem()
    {
        BgItem.enabled = false;
        SpineItem.gameObject.SetActive(false);
        lbChipWin.gameObject.SetActive(false);
        lbChipFSP.gameObject.SetActive(false);
    }
    public void setBgItem(Sprite spr)
    {
        BgItem.sprite = spr;
    }

}
