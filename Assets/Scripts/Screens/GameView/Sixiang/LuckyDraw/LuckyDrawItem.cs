using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using System;

public class LuckyDrawItem : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    SkeletonGraphic spine;
    [SerializeField]
    TextMeshProUGUI lbChipWin;
    public SiXiangLuckyDrawView.TYPE_ITEM typeItem = SiXiangLuckyDrawView.TYPE_ITEM.NORMAL;
    private List<string> listAnimName = new List<string> { "minor", "major", "mega", "grand" };
    void Start()
    {

    }
    public async Task setResult(JObject data)
    {
        //{"winAmount":0,"userAmount":2299779,"card":4,"multiplier":0.0,"index":10,"jackpot":0,"isFinished":false," ":0,"isSelectBonusGame":false}
        string animationQuay = "quay_";
        string animationNormal = "normal_";
        string typeAnim = "normal";
        long winAmount = 0;
        if (data.ContainsKey("cardAmount"))
        {
            winAmount = (long)data["cardAmount"];
        }
        else if (data.ContainsKey("winAmount"))
        {
            winAmount = (long)data["winAmount"];
        }
        typeItem = SiXiangLuckyDrawView.TYPE_ITEM.NORMAL;
        switch ((int)data["card"])
        {

            case (int)SiXiangLuckyDrawView.TYPE_ITEM.MINOR:
                {
                    typeAnim = "minor";
                    typeItem = SiXiangLuckyDrawView.TYPE_ITEM.MINOR;
                    break;
                }
            case (int)SiXiangLuckyDrawView.TYPE_ITEM.MAJOR:
                {
                    typeAnim = "major";
                    typeItem = SiXiangLuckyDrawView.TYPE_ITEM.MAJOR;
                    break;
                }
            case (int)SiXiangLuckyDrawView.TYPE_ITEM.MEGA:
                {
                    typeAnim = "mega";
                    typeItem = SiXiangLuckyDrawView.TYPE_ITEM.MEGA;
                    break;
                }
            case (int)SiXiangLuckyDrawView.TYPE_ITEM.GRAND:
                {
                    typeAnim = "grand";
                    typeItem = SiXiangLuckyDrawView.TYPE_ITEM.GRAND;

                    break;
                }
            default:
                typeAnim = "normal";
                animationQuay = "";
                animationNormal = "";
                typeItem = SiXiangLuckyDrawView.TYPE_ITEM.NORMAL;

                break;

        }
        spine.Initialize(true);
        spine.AnimationState.SetAnimation(0, animationQuay + typeAnim, false);
        float duration = spine.Skeleton.Data.FindAnimation(animationQuay + typeAnim).Duration;
        if (typeItem == (int)SiXiangLuckyDrawView.TYPE_ITEM.NORMAL)
        {
            Vector2 initPos = transform.localPosition;
            DOTween.Sequence()
                .SetId("showNormal")
                .Append(transform.DOScale(new Vector2(0.85f, 0.85f), 0.4f))
                .Join(transform.DOLocalMoveX(initPos.x - 20, 0.1f).OnComplete(() =>
                {
                    transform.DOLocalMoveX(initPos.x, 0.1f).OnComplete(() =>
                   {
                       transform.DOLocalMoveX(initPos.x + 20, 0.1f).OnComplete(() =>
                       {
                           transform.DOLocalMoveX(initPos.x, 0.1f);
                       });
                   });
                })).AppendCallback(() =>
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.LUCKYDRAW_ITEM_NORMAL);
                    DOTween.Sequence().Append(
                        transform.DOScale(new Vector2(1.1f, 1.1f), 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        lbChipWin.gameObject.SetActive(true);
                        lbChipWin.text = Globals.Config.FormatMoney(winAmount, true);
                    })).Append(transform.DOScale(Vector2.one, 0.1f));
                });
            Tween nodeShowTween = DOTween.TweensById("showNormal")[0];
            await nodeShowTween.AsyncWaitForCompletion();
        }
        else
        {
            float timeDelayAnim = spine.Skeleton.Data.FindAnimation(animationQuay + typeAnim).Duration;
            string animName = animationQuay + typeAnim;
            if (data.ContainsKey("isFinished") && (bool)data["isFinished"])
            {
                animName = "win_" + typeAnim;
            }
            DOTween.Sequence().AppendInterval(timeDelayAnim / 2).AppendCallback(() =>
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.LUCKYDRAW_ITEM_JACKPOT);
            });
            await Task.Delay(TimeSpan.FromSeconds(timeDelayAnim));
            spine.AnimationState.SetAnimation(0, (data.ContainsKey("isFinished") && (bool)data["isFinished"]) ? animName : animationNormal + typeAnim, true);
        }


    }

    public void showEffectWinJp()
    {
        Debug.Log("showEffectWinJp:");
        spine.AnimationState.GetCurrent(0).TimeScale = 0;
        spine.AnimationState.GetCurrent(0).Reset();
        spine.Initialize(true);
        spine.AnimationState.SetAnimation(0, "win_" + listAnimName[(int)typeItem - 1], true);
        spine.timeScale = 1;
    }

    // Update is called once per frame

}
