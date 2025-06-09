using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System;
using Spine;
using Random = UnityEngine.Random;

public class SymbolController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public SkeletonGraphic spine;
    [SerializeField]
    public SkeletonGraphic bgWin;
    [SerializeField]
    public Image sprite;
    [SerializeField]
    protected CollumController collumCtrl;

    [SerializeField]
    List<Sprite> spriteNormal = new List<Sprite>();
    [SerializeField]
    List<Sprite> spriteBlur = new List<Sprite>();


    public int indexSymbol;
    public int indexStop = -1;
    public float Speed = 0.1f;
    public int id = 0;
    protected int SCATTER_ID = 10;

    // Update is called once per frame
    void Update()
    {
        if (collumCtrl.isNeerSpin)
        {
            Speed = collumCtrl.speedNFS;
        }
    }
    public void spin()
    {
        float posNew = transform.localPosition.y - collumCtrl.stepMove;
        indexSymbol++;
        transform.DOLocalMoveY(posNew, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
         {
             loopMove();
         });
    }
    public void loopMove()
    {
        if (indexSymbol > 3)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, collumCtrl.positionReset);
            indexSymbol = 0;
            setSprite(Random.Range(0, 8), true);
        }
        indexSymbol++;
        //DOTween.Sequence().Append(transform.DOLocalMove(new Vector2(transform.localPosition.x, transform.localPosition.y - collumCtrl.stepMove), Speed)).AppendCallback(() =>
        DOTween.Sequence().Append(transform.DOBlendableLocalMoveBy(new Vector2(0, -collumCtrl.stepMove), Speed)).AppendCallback(() =>
        {
            if (!collumCtrl.isStop)
            {
                loopMove();
            }
            else
            {
                stopMove();
            }
        });
    }
    private void stopMove()
    {
        if (indexSymbol > 3)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, collumCtrl.positionReset);
            indexSymbol = 0;
            if (collumCtrl.listIDResult.Count > 0)
            {
                indexStop = collumCtrl.listIDResult.Count;
                int idResult = collumCtrl.listIDResult.Last();
                collumCtrl.listIDResult.RemoveAt(collumCtrl.listIDResult.Count - 1);
                setSprite(idResult);
                stopMove();
            }
            else
            {// doan nay la nhung thang indexsymbol=0
                transform.localPosition = new Vector2(transform.localPosition.x, collumCtrl.positionReset + collumCtrl.stepMove);
                //DOTween.Sequence().Append(transform.DOLocalMove(new Vector2(transform.localPosition.x, transform.localPosition.y - collumCtrl.stepMove), Speed).SetEase(Ease.OutBack));
                DOTween.Sequence().Append(transform.DOBlendableLocalMoveBy(new Vector2(0, -collumCtrl.stepMove), Speed).SetEase(Ease.OutBack));
                setSprite(Random.Range(0, 9));
            }
        }
        else
        {

            Ease easing = (indexSymbol + 1 == indexStop) ? Ease.OutBack : Ease.Linear;
            float speedMove = (indexSymbol + 1 == indexStop) ? Speed * 3 : Speed;
            indexSymbol++;
            if (indexSymbol == indexStop)
            {
                if (id == SCATTER_ID)
                {
                    SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.SCATTER_SYMBOL);
                }
            }
            //DOTween.Sequence().Append(transform.DOLocalMove(new Vector2(transform.localPosition.x, transform.localPosition.y - collumCtrl.stepMove), speedMove).SetEase(easing)).AppendCallback(() =>
            DOTween.Sequence().Append(transform.DOBlendableLocalMoveBy(new Vector2(0, -collumCtrl.stepMove), speedMove).SetEase(easing)).AppendCallback(() =>
            {
                if (indexSymbol != indexStop)
                {
                    stopMove();
                }
                else if (indexSymbol == 3)
                {
                    collumCtrl.stopCollumCompleted();
                }
            });
            if (indexSymbol == 3)
            {
                DOTween.Sequence().AppendInterval(0.85f * speedMove).AppendCallback(() =>
                {
                    collumCtrl.prepareStop();
                });
            }
        }
    }
    public virtual void setSprite(int idSprite, bool isBlur = false)
    {
        id = idSprite;
        spine.gameObject.SetActive(false);
        if (!isBlur)
        {
            sprite.sprite = spriteNormal[id];
        }
        else
        {
            sprite.sprite = spriteBlur[id];
        }
        if (id == 11 || id == 13)
        {
            sprite.color = Color.white;
        }
        sprite.SetNativeSize();
    }
    public void setSpine(int idSpine, float timeScale = 1.0f)
    {
        Action<SkeletonDataAsset> cb = (skeData) =>
        {
            spine.gameObject.SetActive(true);
            spine.skeletonDataAsset = skeData;
            spine.Initialize(true);
            spine.AnimationState.SetAnimation(0, "animation", false);
            spine.timeScale = timeScale;
            spine.startingLoop = false;
            if (idSpine == 9 || idSpine == 10)
            {
                spine.transform.localScale = new Vector2(0.9f, 0.9f);
            }
            else
            {
                spine.transform.localScale = Vector2.one;
            }

        };
        UIManager.instance.loadSkeletonDataAsync(getSpinePath(idSpine), cb);
    }
    public async void showScatterSpine()
    {
        setSpine(10);
        sprite.gameObject.SetActive(false);
        await Task.Delay(1000);
        spine.gameObject.SetActive(false);
        sprite.gameObject.SetActive(true);
    }
    private string getSpinePath(int idSpine)
    {
        string path = "GameView/SiXiang/Spine/Icon/%s/skeleton_SkeletonData";
        List<string> listFolderName = new List<string> { "10", "J", "Q", "K", "A", "Dragon", "Tiger", "Turtle", "Phoenix", "Wild", "Scatter" };
        path = Globals.Config.formatStr(path, listFolderName[idSpine]);
        return path;
    }
    public void Reset()
    {
        if (indexSymbol == 0)
        {
            setSprite(Random.Range(0, 9));
        }
        sprite.color = Color.white;
        indexStop = -1;
        bgWin.gameObject.SetActive(false);
        spine.gameObject.SetActive(false);

    }
    public void setBgWin(bool state)
    {
        bgWin.gameObject.SetActive(state);
        if (state)
        {
            bgWin.Initialize(true);
            bgWin.AnimationState.SetAnimation(0, "animation", true);
        }


    }


}
