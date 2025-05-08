using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotNoelItemSpin : ItemSpinController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    public override void setItemData(List<int> arrId)
    {
        listIdIcon = arrId;
        for (int i = 0; i < arrId.Count; i++)
        {

            listSprItem[i].sprite = listSpriteIcon[arrId[i]];
            listSprItem[i].SetNativeSize();
            listSprItem[i].transform.localScale = new Vector2(0.85f, 0.85f);
            RectTransform rt = listSprItem[i].GetComponent<RectTransform>();
            if (rt.sizeDelta.y > 134)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x / 1.1f, rt.sizeDelta.y / 1.1f);
            }
            if (arrId[i] == 4 || arrId[i] == 10)
            {
                listSprItem[i].transform.localScale = Vector2.one;
            }

        }
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
            listSprItem[i].sprite = listSpriteIcon[Random.Range(0, listSpriteIcon.Count)];
            listSprItem[i].SetNativeSize();
            listSprItem[i].transform.localScale = new Vector2(0.85f, 0.85f);
            //}
            RectTransform rt = listSprItem[i].GetComponent<RectTransform>();
            if (rt.sizeDelta.x > 134)
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x / 1.1f, rt.sizeDelta.y / 1.1f);
            }
        }
    }
    public override void setItemAnim(int index, int id, bool isWild = false)
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
                itemSpine.transform.localScale = new Vector2(0.6f, 0.63f);
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

}
