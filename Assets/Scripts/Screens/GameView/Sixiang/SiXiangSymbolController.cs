using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Spine.Unity;

public class SiXiangSymbolController : SymbolController
{
    // Start is called before the first frame update


    public async Task showEffectSpeadWild(Vector2 initPos)
    {
        spine.transform.SetParent(collumCtrl.transform);
        spine.transform.localPosition = initPos;
        float timeScale = collumCtrl.slotView.spintype == BaseSlotView.SPIN_TYPE.NORMAL ? 1.0f : 1.5f;
        setSpine(9, timeScale);
        Sequence seq = DOTween.Sequence();
        float timeEff = collumCtrl.slotView.spintype == BaseSlotView.SPIN_TYPE.NORMAL ? 1.0f : 0.7f;
        seq.Append(spine.transform.DOLocalMoveY(transform.localPosition.y, timeEff).OnComplete(() =>
        {
            spine.transform.SetParent(transform);
            spine.transform.localPosition = Vector2.zero;
        })).AppendInterval(timeEff).AppendCallback(() =>
        {
            spine.gameObject.SetActive(false);
            spine.timeScale = 1.0f;
            setSprite(9);
        }).SetId("seqWildSpead");
        Tween tw = DOTween.TweensById("seqWildSpead")[0];
        await tw.AsyncWaitForCompletion();
    }
    public void showWild(float timeScale = 1.0f)
    {
        setSpine(9, timeScale);
    }
    public override void setSprite(int idSprite, bool isBlur = false)
    {
        base.setSprite(idSprite, isBlur);
        if (idSprite > 4 && idSprite < 9)
        {
            sprite.transform.localScale = new Vector2(0.9f, 0.9f);
        }
        else if (idSprite == 9 || idSprite == 10)
        {
            sprite.transform.localScale = new Vector2(0.9f, 0.9f);
        }
        else
        {
            sprite.transform.localScale = Vector2.one;
        }
        if (SiXiangView.Instance != null && SiXiangView.Instance.gameType == (int)SiXiangView.GAME_TYPE.DRAGON_PEARL)
        {
            sprite.transform.localScale = sprite.transform.localScale * new Vector2(0.97f, 0.97f);
        }
    }
}
