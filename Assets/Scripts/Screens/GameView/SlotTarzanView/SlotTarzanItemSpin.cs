using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;

public class SlotTarzanItemSpin : ItemSpinController
{
    // Start is called before the first frame update
    [SerializeField]
    public List<Sprite> listSprAnimalWild = new List<Sprite>();
    protected override void Start()
    {
        base.Start();
        ICON_ANIMPATH = "GameView/SlotSpine/Tarzan/SpineIcon/%id/skeleton_SkeletonData";
        //CollumSpinCtrl = (SlotTarzanCollumController)CollumSpinCtrl;
    }

    public override void setItemData(List<int> arrId)
    {
        listIdIcon = arrId;
        for (int i = 0; i < arrId.Count; i++)
        {

            listSprItem[i].sprite = listSpriteIcon[arrId[i]];
            listSprItem[i].SetNativeSize();
            //listSprItem[i].transform.localScale = arrId[i] == 12 ? new Vector2(0.45f, 0.45f) : new Vector2(0.9f, 0.9f);
            if (arrId[i] < 4)
            {
                listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
            }
            else if (arrId[i] == 12)
            {
                listSprItem[i].transform.localScale = new Vector2(0.45f, 0.45f);
            }
            else
            {
                listSprItem[i].transform.localScale = new Vector2(0.9f, 0.9f);
            }

        }
    }
    public void transformToWild()
    {
        List<int> listIdAnimal = new List<int> { 4, 5, 6, 7 };
        for (int i = 0; i < listIdIcon.Count; i++)
        {
            if (listIdAnimal.IndexOf(listIdIcon[i]) != -1)
            {
                setItemAnim(i, listIdIcon[i], true);
                listIdIcon[i] += 11;
            }
        }
        setItemData(listIdIcon);

    }
    public override void setRandomData(bool isBlur = false)
    {
        for (int i = 0; i < 3; i++)
        {
            //if (isBlur)
            //{
            //    listSprItem[i].sprite = listSpriteIconBlur[Random.Range(0, listSpriteIconBlur.Count)];
            //    listSprItem[i].SetNativeSize();
            //    listSprItem[i].transform.localScale = new Vector2(1.1f, 1.6f);
            //}
            //else
            //{
            int idRan = Random.Range(0, 12);
            listSprItem[i].sprite = listSpriteIcon[idRan];
            listSprItem[i].SetNativeSize();
            //listSprItem[i].transform.localScale = idRan == 12 ? new Vector2(0.45f, 0.45f) : new Vector2(0.9f, 0.9f);
            if (idRan < 4)
            {
                listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
            }
            else if (idRan == 12)
            {
                listSprItem[i].transform.localScale = new Vector2(0.45f, 0.45f);
            }
            else
            {
                listSprItem[i].transform.localScale = new Vector2(0.9f, 0.9f);
            }
        }

    }


    public override void setItemAnim(int index, int id, bool isWild = false)
    {
        SkeletonGraphic itemSpine;
        listSprItem[index].gameObject.SetActive(false);
        if (index >= listSpineItem.Count)
        {
            //itemSpine = Instantiate(animIconPr, transform).GetComponent<SkeletonGraphic>();
            itemSpine = CollumSpinCtrl.gameView.getAnimIcon(transform).GetComponent<SkeletonGraphic>();
            listSpineItem.Add(itemSpine);
        }
        else
        {
            itemSpine = listSpineItem[index];
        }
        itemSpine.transform.SetParent(transform);
        switch (id)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                itemSpine.transform.localScale = new Vector2(0.65f, 0.65f);
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
            case 13:
                itemSpine.transform.localScale = new Vector2(0.4f, 0.4f);
                break;
            case 15:
            case 16:
            case 17:
            case 18:
                itemSpine.transform.localScale = new Vector2(0.45f, 0.45f);
                break;
            case 12:
                itemSpine.transform.localScale = new Vector2(0.45f, 0.45f);
                break;
            default:
                itemSpine.transform.localScale = Vector2.one;
                break;
        }
        itemSpine.gameObject.GetComponent<RectTransform>().localPosition = listSprItem[index].gameObject.GetComponent<RectTransform>().localPosition;
        itemSpine.gameObject.SetActive(true);
        if (id > 14)
        {
            id -= 11;
            isWild = true;
        }
        itemSpine.skeletonDataAsset = UIManager.instance.loadSkeletonData(ICON_ANIMPATH.Replace("%id", id.ToString()));
        itemSpine.Initialize(true);
        if (isWild)
        {
            itemSpine.AnimationState.SetAnimation(0, "wild", true);
        }
        else
        {
            itemSpine.AnimationState.SetAnimation(0, "animation", true);
        }

        //itemSpine.AnimationState.Complete += delegate {
        //    itemSpine.gameObject.SetActive(false);
        //};
    }
    public void showAnimWild(int index, System.Action cb)
    {
        setItemAnim(index, 13);
        DOTween.Sequence()
            .AppendInterval(1.5f)
            .AppendCallback(() =>
            {
                cb();
                setDark(false);
            });
    }



}
