using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using Globals;

public class SlotJuicyGardenItemSpin : ItemSpinController
{
    // Start is called before the first frame update
    [SerializeField]
    private List<TextMeshProUGUI> listLbValuePackage = new List<TextMeshProUGUI>();
    [SerializeField]
    public GameObject lbValuePackageTemplate;
    public List<long> arrValuePackage = new List<long>();
    private SlotJuicyGardenView gardenView;
    protected override void Start()
    {
        base.Start();
        ICON_ANIMPATH = "GameView/SlotSpine/JuicyGarden/SpineIcon/%id/skeleton_SkeletonData";
    }
    protected override void Awake()
    {
        base.Awake();
        gardenView = (SlotJuicyGardenView)CollumSpinCtrl.gameView;
    }
    public override void setItemAnim(int index, int id, bool isWild = false)
    {
        if (id < 13)
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
            itemSpine.transform.localScale = Vector2.one;
            //itemSpine = CollumSpinCtrl.gameView.getAnimIcon(transform).GetComponent<SkeletonGraphic>();
            Vector2 position = listSprItem[index].gameObject.GetComponent<RectTransform>().localPosition;
            switch (id)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    itemSpine.transform.localScale = new Vector2(1.2f, 1.2f);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        itemSpine.transform.localScale = new Vector2(0.9f, 0.9f);
                        break;
                    }
                case 8:
                case 9:
                case 10:
                    {
                        itemSpine.transform.localScale = new Vector2(0.75f, 0.75f);
                        position = new Vector2(position.x, position.y - 20);
                        break;
                    }
                case 12:
                case 11:
                    itemSpine.transform.localScale = new Vector2(0.65f, 0.65f);
                    break;
                default:
                    itemSpine.transform.localScale = Vector2.one;
                    break;
            }
            itemSpine.gameObject.transform.localPosition = position;
            itemSpine.gameObject.SetActive(true);
            itemSpine.skeletonDataAsset = UIManager.instance.loadSkeletonData(ICON_ANIMPATH.Replace("%id", id.ToString()));
            itemSpine.Initialize(true);
            itemSpine.AnimationState.SetAnimation(0, "animation", true);
        }
        else
        {
            setIconData(index, id);

        }
    }
    public override void setItemValuePackage(int index, long value)
    {
        //value = 123456;
        if (listIdIcon[index] >= 13)
        {
            TextMeshProUGUI lbValuePackage;
            if (index >= listLbValuePackage.Count)
            {
                lbValuePackage = Instantiate(lbValuePackageTemplate, listSprItem[index].transform).GetComponent<TextMeshProUGUI>();

                listLbValuePackage.Add(lbValuePackage);
            }
            else
            {
                lbValuePackage = listLbValuePackage[index];
                if (lbValuePackage.transform.parent != listSprItem[index].transform)
                {
                    lbValuePackage.transform.parent = listSprItem[index].transform;
                }
            }
            lbValuePackage.transform.localPosition = Vector2.zero;// listSprItem[index].transform.localPosition;
            lbValuePackage.transform.localScale = Vector2.one / lbValuePackage.transform.parent.localScale;
            lbValuePackage.text = Globals.Config.FormatMoney3(value, 1000);
            if (listIdIcon[index] > 13)
            {
                lbValuePackage.gameObject.SetActive(false);
            }
            else
            {
                lbValuePackage.gameObject.SetActive(true);
                lbValuePackage.color = Color.white;
            }
        }

    }
    public override void setHolderPackage(int index)
    {
        gardenView.createHoldPackage(convertIndex(index), listSprItem[index].gameObject);

    }
    public override void setRandomData(bool isBlur = false)
    {
        base.setRandomData();
        hideAllTextPackage();
        if (gardenView.isBonusGame && gardenView.freespinLeft > 0)
        {
            setDark(true);
        }
    }
    private int convertIndex(int index)
    {

        int indexNew = index * 5 + (CollumSpinCtrl.collumIndex);
        return indexNew;
    }
    public override void setItemData(List<int> arrId)
    {

        listIdIcon = arrId;
        for (int i = 0; i < arrId.Count; i++)
        {
            listSprItem[i].sprite = listSpriteIcon[arrId[i]];
            listSprItem[i].SetNativeSize();
            listSprItem[i].transform.localPosition = listPosSprItem[i];
            listSprItem[i].transform.localScale = Vector2.one;
            switch (arrId[i])
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    {
                        listSprItem[i].transform.localPosition = listPosSprItem[i] - new Vector2(0, 7);
                        listSprItem[i].transform.localScale = new Vector2(1.2f, 1.2f);
                        break;
                    }
                case 4:
                    {
                        listSprItem[i].transform.localPosition = listPosSprItem[i] + new Vector2(0, 4);
                        break;
                    }
                case 11:
                case 12:
                    {
                        listSprItem[i].transform.localScale = new Vector2(0.8f, 0.75f);
                        break;
                    }
                case 13:
                    listSprItem[i].transform.localScale = new Vector2(0.9f, 0.9f);
                    break;
                case 14:
                case 15:
                case 16:
                    {

                        listSprItem[i].transform.localPosition = new Vector2(listPosSprItem[i].x, listPosSprItem[i].y + 15.5f);
                        listSprItem[i].transform.localScale = new Vector2(0.9f, 0.9f);
                        break;
                    }
                default:
                    {
                        listSprItem[i].transform.localScale = Vector2.one;
                        break;
                    }
            }
            if (gardenView.isBonusGame && gardenView.allowSelectBonus == false)
            {
                listSprItem[i].color = arrId[i] > 12 ? Color.white : Color.gray;
            }

        }
    }
    protected override void hideAllTextPackage()
    {
        listLbValuePackage.ForEach(lb =>
        {
            lb.gameObject.SetActive(false);
        });
    }
    public void setLight()
    {
        for (int i = 0; i < 3; i++)
        {
            listSprItem[i].color = Color.white;
            if (listSprItem[i].transform.childCount > 0)
            {
                listSprItem[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }
    public override void setDark(bool state, int index = 3)
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
            for (int i = 0; i < listLbValuePackage.Count; i++)
            {
                listLbValuePackage[i].color = colorState;
            }
            listSpineItem.Clear();
        }
        else
        {
            listSprItem[index].color = colorState;
            setItemAnim(index, listIdIcon[index]);
            if (index < listLbValuePackage.Count)
            {
                listLbValuePackage[index].color = colorState;
            }


        }
    }
    public override void stopSpin()
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
                CollumSpinCtrl.itemResult = this;
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT.STOP_SPIN);
                int index = 0;
                foreach (int idValue in arrValuePackage)
                {
                    setItemValuePackage(index, idValue);
                    index++;
                }
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
                if (CollumSpinCtrl.isLastCollum)
                {
                    CollumSpinCtrl.onCollumStopCompleted();
                }
            }

        });

    }
}
