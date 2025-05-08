using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Globals;

public class SlotFruitItemSpin : ItemSpinController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ICON_ANIMPATH = "GameView/SlotSpine/Fruit/SpineIcon/%id/skeleton_SkeletonData";
        //CollumSpinCtrl = (SlotTarzanCollumController)CollumSpinCtrl;
    }
    public override void setItemData(List<int> arrId)
    {
        listIdIcon = arrId;
        for (int i = 0; i < arrId.Count; i++)
        {
            listSprItem[i].sprite = listSpriteIcon[arrId[i]];
            listSprItem[i].SetNativeSize();
            listSprItem[i].transform.localScale = new Vector2(0.9f, 0.9f);
            //switch (arrId[i])
            //{
            //    case 8:
            //    case 9:
            //    case 10:
            //        listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
            //        break;
            //    case 11:
            //    case 12:
            //        {
            //            //listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
            //            break;
            //        }
            //    default:
            //        {
            //            //listSprItem[i].transform.localScale = Vector2.one;
            //            break;
            //        }
            //}

        }
    }
    public override void setItemAnim(int index, int id, bool isWild = false)
    {
        SkeletonGraphic itemSpine;
        string idPath = id.ToString();
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
        string annimName = "animation";
        itemSpine.transform.SetParent(transform);
        itemSpine.transform.localScale = new Vector2(0.9f, 0.9f);
        Vector2 posSpine = listSprItem[index].gameObject.GetComponent<RectTransform>().localPosition;
        itemSpine.gameObject.SetActive(true);
        switch (id)
        {
            case 8:
            case 9:
            case 10:
                itemSpine.transform.localScale = new Vector2(0.8f, 0.8f);
                posSpine = new Vector2(posSpine.x, posSpine.y - 10);
                break;
            case 11:
                itemSpine.transform.localScale = new Vector2(0.8f, 0.8f);
                break;
            case 12:
                itemSpine.transform.localScale = new Vector2(0.75f, 0.75f);
                break;
            default:
                itemSpine.transform.localScale = new Vector2(1, 1);
                break;
        }
        itemSpine.gameObject.GetComponent<RectTransform>().localPosition = posSpine;
        itemSpine.skeletonDataAsset = UIManager.instance.loadSkeletonData(ICON_ANIMPATH.Replace("%id", idPath));
        itemSpine.Initialize(true);
        itemSpine.AnimationState.SetAnimation(0, annimName, true);
        //itemSpine.AnimationState.Complete += delegate {
        //    itemSpine.gameObject.SetActive(false);
        //};
    }

}
