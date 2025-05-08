using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Globals;

public class CollumSpinController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<ItemSpinController> listItem = new List<ItemSpinController>();
    [SerializeField]
    public ItemSpinController itemInit;
    public int collumIndex = 1;
    public bool isLastCollum = false;

    [SerializeField]
    public BaseSlotGameView gameView;
    public JObject SPEED_TYPE = new JObject();
    public bool isStop = false;
    public ItemSpinController itemResult;
    public bool isNearFreeSpin = false;


    protected virtual void Start()
    {
        SPEED_TYPE["NORMAL"] = 0.18f;
        SPEED_TYPE["AUTO"] = 0.12f;
    }
    public virtual void startSpin(BaseSlotGameView reference)
    {
        if (listItem.Count == 1)
        {
            listItem.Clear();
            ItemSpinController item2 = Instantiate(itemInit.gameObject, transform).GetComponent<ItemSpinController>();
            item2.transform.localPosition = new Vector2(itemInit.transform.localPosition.x, itemInit.transform.localPosition.y + itemInit.GetComponent<RectTransform>().sizeDelta.y);
            item2.setRandomData();

            ItemSpinController item3 = Instantiate(itemInit.gameObject, transform).GetComponent<ItemSpinController>();
            item3.transform.localPosition = new Vector2(itemInit.transform.localPosition.x, itemInit.transform.localPosition.y + 2 * itemInit.GetComponent<RectTransform>().sizeDelta.y);
            item3.setRandomData();

            listItem.Add(item3);
            listItem.Add(item2);
            listItem.Add(itemInit);
        }
        gameView = reference;
        isStop = false;
        for (int i = 0; i < listItem.Count; i++)
        {
            if (gameView.spintype == BaseSlotGameView.SPIN_TYPE.NORMAL)
            {
                listItem[i].SPEED = (float)SPEED_TYPE["NORMAL"];
                listItem[i].SPEED_BACKSPIN = (float)SPEED_TYPE["NORMAL"] - 0.05f;
            }
            else
            {
                listItem[i].SPEED = (float)SPEED_TYPE["AUTO"];
                listItem[i].SPEED_BACKSPIN = (float)SPEED_TYPE["AUTO"] - 0.05f;
            }
            listItem[i].startSpin();
        }
    }
    public void setStartView(List<int> arrId, BaseSlotGameView reference)
    {
        gameView = reference;
        //for (int i = 0; i < 3; i++)
        //{
        //    listItem[i].arrID = arrId;
        //    listItem[i].setItemData(arrId);
        //}
        listItem[0].setItemData(arrId);
        isNearFreeSpin = false;
    }
    public void setStartView(JArray arrId, BaseSlotGameView reference)
    {
        gameView = reference;
        List<int> listIdView = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            listIdView.Add((int)arrId[i]["id"]);
            ((SlotJuicyGardenItemSpin)listItem[0]).arrValuePackage.Add((int)arrId[i]["value"]);
        }
        listItem[0].setItemData(listIdView);
        for (int i = 0; i < 3; i++)
        {
            listItem[0].setItemValuePackage(i, (int)arrId[i]["value"]);
            //if ((int)arrId[i]["id"] > 12)
            //{
            //    listItem[0].setHolderPackage(i);
            //}
        }
    }
    public void setFinishView(List<int> arrId)
    {
        for (int i = 0; i < 3; i++)
        {
            listItem[i].arrID = arrId;
        }
    }
    public virtual void setFinishView(JArray arrId)
    {
        foreach (ItemSpinController item in listItem)
        {
            item.arrID.Clear();
            SlotJuicyGardenItemSpin itemSpinJuicy = (SlotJuicyGardenItemSpin)item;
            itemSpinJuicy.arrValuePackage.Clear();

        }
        List<int> listIdView = new List<int>();
        List<long> listIdValuePackage = new List<long>();
        for (int i = 0; i < 3; i++)
        {
            int idIcon = (int)arrId[i]["id"];
            if (idIcon == -1)
            {
                idIcon = Random.Range(0, 11);
            }
            listIdView.Add(idIcon);
            listIdValuePackage.Add((long)arrId[i]["value"]);
        }

        foreach (ItemSpinController item in listItem)
        {
            item.arrID = listIdView;
            SlotJuicyGardenItemSpin itemSpinJuicy = (SlotJuicyGardenItemSpin)item;
            itemSpinJuicy.arrValuePackage = listIdValuePackage;
        }
    }
    public void setDarkItem(bool state, int index = 3)
    {
        if (itemResult != null)
            itemResult.setDark(state, index);
    }
    public void setDarkAllCollum(bool state)
    {
        listItem.ForEach(item =>
        {
            item.setDark(state);
        });
    }
    public Vector2 getPositionItemAtIndex(int index)
    {
        return itemResult.getPositionItem(index);
    }
    public virtual void onCollumStop()
    {
        gameView.nextColStop();
    }
    public virtual void onCollumStopCompleted()
    {
        gameView.onStopSpin();
    }
    public void effectIconEnd()
    {
        gameView.showNextLineEffect();
    }


}
