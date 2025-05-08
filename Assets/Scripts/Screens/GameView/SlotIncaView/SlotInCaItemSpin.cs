using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInCaItemSpin : ItemSpinController
{
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        ICON_ANIMPATH = "GameView/SlotSpine/InCa/SpineIcon/%id/skeleton_SkeletonData";
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
            switch (arrId[i])
            {
                case 0:
                case 2:
                 
                    break;
                case 11:
                case 12:
                    {
                        //listSprItem[i].transform.localScale = new Vector2(0.8f, 0.8f);
                        break;
                    }
                default:
                    {
                        //listSprItem[i].transform.localScale = Vector2.one;
                        break;
                    }
            }

        }
    }
    // Update is called once per frame
    public override void setItemAnim(int index, int id, bool isWild = false)
    {
        SkeletonGraphic itemSpine;
        string idPath = "";
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
        itemSpine.transform.localScale = Vector2.one;
        switch (id)
        {
            case 0:
                annimName = "J";
                idPath = "AJQK";
                break;
            case 1:
                annimName = "Q";
                idPath = "AJQK";
                break;
            case 2:
                annimName = "K";
                idPath = "AJQK";
                break;
            case 3:
                annimName = "A";
                idPath = "AJQK";
                //itemSpine.transform.localScale = new Vector2(0.65f, 0.65f);
                break;
            case 4:
                annimName = "bich";
                idPath = "bichcorotep";
                break;
            case 5:
                annimName = "co";
                idPath = "bichcorotep";
                break;
            case 6:
                annimName = "zo";
                idPath = "bichcorotep";
                break;
            case 7:
                annimName = "tep";
                idPath = "bichcorotep";
                break;
            case 8:
                idPath = "binh";
                break;
            case 9:
                idPath = "bird";
                break;
            case 10:
                idPath = "sun";
                break;
            case 11:
                idPath = "wild";
                itemSpine.transform.localScale = new Vector2(0.8f, 0.8f);
                break;
            case 12:
                idPath = "scatter";
                itemSpine.transform.localScale = new Vector2(0.8f, 0.8f);
                break;
        }
        itemSpine.gameObject.GetComponent<RectTransform>().localPosition = listSprItem[index].gameObject.GetComponent<RectTransform>().localPosition;
        itemSpine.gameObject.SetActive(true);
        itemSpine.skeletonDataAsset = UIManager.instance.loadSkeletonData(ICON_ANIMPATH.Replace("%id", idPath));
        itemSpine.Initialize(true);
        itemSpine.AnimationState.SetAnimation(0, annimName, true);
        //itemSpine.AnimationState.Complete += delegate {
        //    itemSpine.gameObject.SetActive(false);
        //};
    }
}
