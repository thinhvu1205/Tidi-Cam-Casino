using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System;
using Random = UnityEngine.Random;

public class ItemSpinController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<Image> listSprItem = new List<Image>();

    [HideInInspector]
    public List<SkeletonGraphic> listSpineItem = new List<SkeletonGraphic>();

    [SerializeField]
    public List<Sprite> listSpriteIcon = new List<Sprite>();
    [SerializeField]
    public List<SkeletonDataAsset> listSkeDataIcon = new List<SkeletonDataAsset>();
    [SerializeField]
    public List<Sprite> listSpriteIconBlur = new List<Sprite>();
    [SerializeField]
    protected CollumSpinController CollumSpinCtrl;

    public float SPEED = 0.075f;
    public float SPEED_BACKSPIN = 0.175f;
    public List<int> listIdIcon = new List<int>();
    public List<Vector2> listPosSprItem = new List<Vector2>();

    [SerializeField]
    float posResetY = 1190;
    [SerializeField]
    protected int typePosition = 0;

    protected string ICON_ANIMPATH = "GameView/SlotSpine/Noel/SpineIcon/%id/skeleton_SkeletonData";

    [HideInInspector]
    public List<int> arrID = new List<int>();

    protected virtual void Awake()
    {
        listPosSprItem.Clear();
        listPosSprItem.Add(new Vector2(0, 144));
        listPosSprItem.Add(new Vector2(0, 0));
        listPosSprItem.Add(new Vector2(0, -144));
        //listSprItem.ForEach(item =>
        //{
        //    listPosSprItem.Add(item.transform.localPosition);
        //    Globals.Logging.Log("listPosSprItem:" + item.transform.localPosition);
        //});
    }
    protected virtual void Start()
    {
        //typePosition = ;
        typePosition = (int)char.GetNumericValue(gameObject.name[gameObject.name.Length - 1]);

    }
    public virtual void setHolderPackage(int index)
    {

    }

    // Update is called once per frame

    public void moveDownLoop()
    {
        //Globals.Logging.Log("Move DownLoop");
        Sequence seq = DOTween.Sequence();
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 nextPos = rect.localPosition - new Vector3(0, rect.sizeDelta.y, 0);
        Vector3 localPos = rect.localPosition;

        System.Action acCheck = () =>
         {
             if (rect.localPosition.y < 0)
             {
                 rect.localPosition = new Vector3(localPos.x, posResetY, 0);
                 typePosition = 3;

             }
             if (!CollumSpinCtrl.isStop)
             {
                 moveDownLoop();
             }
             else
             {
                 stopSpin();
             }
         };
        float speedMove = SPEED * (CollumSpinCtrl.gameView.countScatter > 2 ? 1.75f : 1);
        //seq.Append(rect.DOLocalMoveY(nextPos.y, speedMove));
        seq.Append(rect.DOBlendableLocalMoveBy(new Vector2(0, -rect.sizeDelta.y), speedMove));
        seq.AppendCallback(() =>
        {
            typePosition--;
            acCheck();
        });
    }
    public virtual void startSpin()
    {
        Sequence seq = DOTween.Sequence();
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 backPos = rect.localPosition + new Vector3(0, 30, 0);
        Vector3 initPos = rect.localPosition;
        seq.Append(rect.DOLocalMoveY(backPos.y, SPEED_BACKSPIN).SetEase(Ease.OutSine));
        seq.Append(rect.DOLocalMoveY(initPos.y, SPEED));
        seq.AppendCallback(() =>
        {
            if (rect.localPosition.y < 150)
            {
                rect.localPosition = new Vector3(rect.localPosition.x, posResetY, 0);
            }
            if (Globals.Config.curGameId == (int)Globals.GAMEID.SLOT_JUICY_GARDEN)
            {
                hideAllTextPackage();
            }
            moveDownLoop();
        });
    }
    protected virtual void hideAllTextPackage()
    {

    }
    public virtual void stopSpin()
    {
        Sequence seq = DOTween.Sequence();
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 downPos = rect.localPosition + new Vector3(0, -rect.sizeDelta.y - 30, 0);
        Vector3 endPos = rect.localPosition + new Vector3(0, -rect.sizeDelta.y, 0);
        seq.AppendCallback(() =>
        {
            //if (rect.localPosition.y < 700 && rect.localPosition.y > 200)
            if (typePosition == 2)
            {
                setItemData(arrID);
                if (arrID.Contains(12))
                {
                    CollumSpinCtrl.gameView.countScatter++;
                }
                CollumSpinCtrl.itemResult = this;
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT.STOP_SPIN);
            }
            else
            {
                setRandomData();
            }

        });
        seq.Append(rect.DOLocalMoveY(downPos.y, SPEED).SetEase(Ease.OutCirc));
        seq.Append(rect.DOLocalMoveY(endPos.y, SPEED_BACKSPIN).SetEase(Ease.InCirc));
        seq.AppendCallback(() =>
        {

            if (typePosition == 1)
            {
                CollumSpinCtrl.onCollumStop();
                if (CollumSpinCtrl.isNearFreeSpin)
                {
                    CollumSpinCtrl.gameView.offAnimNearFreeSpin();
                    //CollumSpinCtrl.isNearFreeSpin = false;
                }
                if (CollumSpinCtrl.isLastCollum)
                {

                    CollumSpinCtrl.onCollumStopCompleted();
                }
            }
        });
    }
    public virtual void setDark(bool state, int index = 3)
    {

        Color colorState = state == true ? Color.gray : Color.white;
        if (index == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                listSprItem[i].color = colorState;
                if (i < listSpineItem.Count)
                {
                    listSpineItem[i].gameObject.SetActive(false);
                    CollumSpinCtrl.gameView.removeAnimIcon(listSpineItem[i].gameObject);

                }
                listSprItem[i].gameObject.SetActive(true);
            }
            listSpineItem.Clear();
        }
        else
        {
            listSprItem[index].color = colorState;
            setItemAnim(index, listIdIcon[index]);

        }
    }
    public virtual void setItemData(List<int> arrId)
    {
        listIdIcon = arrId;
        for (int i = 0; i < arrId.Count; i++)
        {
            listSprItem[i].sprite = listSpriteIcon[arrId[i]];
            listSprItem[i].SetNativeSize();
            switch (arrId[i])
            {
                case 11:
                case 12:
                    {
                        listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
                        break;
                    }
                default:
                    {
                        listSprItem[i].transform.localScale = Vector2.one;
                        break;
                    }
            }

        }
    }

    public virtual void setIconData(int index, int id)
    {
        listSprItem[index].sprite = listSpriteIcon[id];
        listSprItem[index].SetNativeSize();
        listSprItem[index].color = Color.white;
    }

    public virtual void setItemAnim(int index, int id, bool isWild = false)
    {


        SkeletonGraphic itemSpine;
        listSprItem[index].gameObject.SetActive(false);
        if (index >= listSpineItem.Count)
        {
            itemSpine = CollumSpinCtrl.gameView.getAnimIcon(transform).GetComponent<SkeletonGraphic>();
            listSpineItem.Add(itemSpine);
        }
        else
        {
            itemSpine = listSpineItem[index];
        }
        itemSpine.transform.parent = transform;
        //itemSpine = CollumSpinCtrl.gameView.getAnimIcon(transform).GetComponent<SkeletonGraphic>();

        switch (id)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                itemSpine.transform.localScale = new Vector2(0.8f, 0.8f);
                break;
            case 11:
                itemSpine.transform.localScale = new Vector2(0.52f, 0.6f);
                break;
            case 12:
                itemSpine.transform.localScale = new Vector2(0.52f, 0.6f);
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                itemSpine.transform.localScale = new Vector2(0.65f, 0.65f);
                break;
            default:
                itemSpine.transform.localScale = Vector2.one;
                break;
        }
        itemSpine.gameObject.transform.localPosition = listSprItem[index].gameObject.GetComponent<RectTransform>().localPosition;
        itemSpine.gameObject.SetActive(true);
        itemSpine.skeletonDataAsset = UIManager.instance.loadSkeletonData(ICON_ANIMPATH.Replace("%id", id.ToString()));
        itemSpine.Initialize(true);
        itemSpine.AnimationState.SetAnimation(0, "animation", true);

    }
    public virtual void setItemValuePackage(int index, long value)
    {

    }
    public virtual void setRandomData(bool isBlur = false)
    {
        for (int i = 0; i < 3; i++)
        {
            listSprItem[i].sprite = listSpriteIcon[Random.Range(0, 10)];
            listSprItem[i].SetNativeSize();
            listSprItem[i].transform.localScale = Vector2.one;
        }
    }
    public Vector2 getPositionItem(int index)
    {
        GameObject sprItem = listSprItem[index].gameObject;
        RectTransform sprItemRectComp = sprItem.GetComponent<RectTransform>();
        //return sprItemRectComp.position;
        return sprItem.transform.parent.TransformPoint(listPosSprItem[index]);

    }

    public void showScatterAnim()
    {

        int indexScatter = listIdIcon.IndexOf(12);
        setItemAnim(indexScatter, 12);
    }

}
