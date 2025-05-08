using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Globals;
public enum DIRECTION
{
    HORIZONTAL,
    VERTICAL
}

public class ListViewCtrl : MonoBehaviour
{
    [SerializeField]
    DIRECTION direction = DIRECTION.VERTICAL;

    [SerializeField]
    GameObject itemTemplate;

    [SerializeField]
    ScrollRect scrollView;

    [SerializeField]
    float itemHeight = 80f, itemWidth = 80f, spacing = 5f, bufferZone = 20f;
    [SerializeField]
    int totalCount = 0, initCount = 20;

    List<GameObject> lsItems = new List<GameObject>();
    List<GameObject> lsAllItems = new List<GameObject>();
    float updateTimer = 0;
    float updateInterval = 0.1f;
    float lastContentPosY = 0;
    float lastContentPosX = 0;

    List<GameObject> listItemRemove = new List<GameObject>();
    System.Action<GameObject, JObject> updateCallback = null;
    JArray listItemData = null;

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();

        if (direction == DIRECTION.VERTICAL)
        {
            bufferZone = (rect.sizeDelta.y / 2 + itemHeight);
            var _initCount = (int)(rect.sizeDelta.y / itemHeight);
            initCount = _initCount > initCount ? _initCount : initCount;
        }
        else
        {
            bufferZone = (rect.sizeDelta.x / 2 + itemWidth);
            var _initCount = (int)(rect.sizeDelta.x / itemWidth);
            initCount = _initCount > initCount ? _initCount : initCount;
        }

        initialize();

    }

    public void OnDestroy()
    {
        foreach (var it in lsAllItems)
        {
            Destroy(it);
        }
    }

    public void setDataList(System.Action<GameObject, JObject> _updateCallback, JArray _listItemData)
    {

        updateCallback = _updateCallback;
        listItemData = _listItemData;
        totalCount = listItemData.Count;
        Globals.Logging.Log("-=-=totalCount " + totalCount);
        Globals.Logging.Log("-=-=lsItems " + lsItems.Count);
        if (listItemData.Count == 0)
        {
            for (var i = 0; i < scrollView.content.childCount; i++)
            {
                scrollView.content.GetChild(i).gameObject.SetActive(false);
                scrollView.content.GetChild(i).SetParent(null);
                i--;
            }
        }

        if (direction == DIRECTION.VERTICAL)
        {
            scrollView.content.sizeDelta = new Vector2(scrollView.content.sizeDelta.x, totalCount * (itemHeight + spacing) + spacing); // get total content height
        }
        else
        {
            scrollView.content.sizeDelta = new Vector2(totalCount * (itemWidth + spacing) + spacing, scrollView.content.sizeDelta.x); // get total content width
        }

        if (totalCount == 0 || totalCount <= lsItems.Count)
        {
            Globals.Logging.Log("-=-= vào đây rồi mà  " + lsItems.Count);
            for (var i = totalCount; i < lsItems.Count; i++)
            {
                listItemRemove.Add(lsItems[i]);
                lsItems[i].SetActive(false);
                lsItems[i].transform.SetParent(null);
                lsItems.RemoveAt(i);
                i--;
            }
        }
        else
        {
            var indexRun = lsItems.Count;
            for (var i = 0; i < listItemRemove.Count && indexRun < totalCount; i++)
            {
                lsItems.Add(listItemRemove[i]);
                listItemRemove.RemoveAt(i);
                i--;
                indexRun++;
            }
        }

        Globals.Logging.Log("-=-= vào đây rồi mà2  " + lsItems.Count);
        for (var i = 0; i < lsItems.Count; i++)
        {
            lsItems[i].transform.SetParent(scrollView.content);
            lsItems[i].transform.localScale = Vector3.one;
            if (direction == DIRECTION.VERTICAL)
                lsItems[i].transform.localPosition = new Vector2(0, -lsItems[i].GetComponent<RectTransform>().sizeDelta.y * (0.5f + i) - spacing * (i + 1));
            else
            {
                lsItems[i].transform.localPosition = new Vector2(lsItems[i].GetComponent<RectTransform>().sizeDelta.x * (0.5f + i) + spacing * (i + 1), 0);
            }
            lsItems[i].name = i + "";
            lsItems[i].gameObject.SetActive(true);
            if (updateCallback != null)
            {
                updateCallback.Invoke(lsItems[i], (JObject)listItemData[i]);
            }
        }

    }

    void initialize()
    {
        if (direction == DIRECTION.VERTICAL)
        {
            scrollView.content.sizeDelta = new Vector2(scrollView.content.sizeDelta.x, totalCount * (itemHeight + spacing) + spacing); // get total content height
        }
        else
        {
            scrollView.content.sizeDelta = new Vector2(totalCount * (itemWidth + spacing) + spacing, scrollView.content.sizeDelta.y); // get total content height
        }

        for (var i = 0; i < initCount; ++i)
        { // spawn items, we only need to do this once
            GameObject item = Instantiate(itemTemplate, scrollView.content);

            if (direction == DIRECTION.VERTICAL)
            {
                item.transform.localPosition = new Vector2(0, -item.GetComponent<RectTransform>().sizeDelta.y * (0.5f + i) - spacing * (i + 1));
            }
            else
            {
                item.transform.localPosition = new Vector2(item.GetComponent<RectTransform>().sizeDelta.x * (0.5f + i) + spacing * (i + 1), 0);
            }
            item.transform.localScale = Vector3.one;
            item.name = i + "";
            lsItems.Add(item);
            item.SetActive(false);
            item.transform.SetParent(null);

            lsAllItems.Add(item);
        }
    }

    Vector2 getPositionInView(GameObject item)
    {
        var worldPos = item.transform.parent.TransformPoint(item.transform.localPosition);
        var viewPos = scrollView.transform.InverseTransformPoint(worldPos);
        return viewPos;
    }

    void Update()
    {
        if (listItemData == null || listItemData.Count <= initCount) return;
        updateTimer += Time.deltaTime;
        if (updateTimer < updateInterval) return; // we don't need to do the math every frame
        updateTimer = 0;
        var items = lsItems;
        var buffer = bufferZone;
        bool isDown;
        float offset;
        if (direction == DIRECTION.VERTICAL)
        {
            isDown = scrollView.content.localPosition.y < lastContentPosY; // scrolling direction
            offset = (itemHeight + spacing) * items.Count;
        }
        else
        {
            isDown = scrollView.content.localPosition.x > lastContentPosX; // scrolling direction
            offset = (itemWidth + spacing) * items.Count;
        }
        for (var i = 0; i < items.Count; ++i)
        {
            var viewPos = getPositionInView(items[i]);
            var pos = items[i].transform.localPosition;
            if (isDown)
            {
                if (direction == DIRECTION.VERTICAL)
                {
                    //Globals.Logging.Log("viewPos.y==" + listItemData.Count);
                    if (viewPos.y < -buffer && pos.y + offset < 0)
                    {

                        items[i].transform.localPosition = new Vector2(0, pos.y + offset);
                        var itemId = int.Parse(items[i].name) - items.Count;
                        items[i].name = itemId + "";
                        //items[i].transform.SetSiblingIndex(itemId);
                        if (updateCallback != null)
                            updateCallback(items[i], (JObject)listItemData[itemId]);
                    }
                }
                else
                {
                    if (viewPos.x > buffer && pos.x - offset > 0)
                    {
                        items[i].transform.localPosition = new Vector2(pos.x - offset, 0);
                        var itemId = int.Parse(items[i].name) - items.Count; // update item id
                        items[i].name = itemId + "";
                        //items[i].transform.SetSiblingIndex(itemId);
                        if (updateCallback != null)
                            updateCallback(items[i], (JObject)listItemData[itemId]);
                    }
                }
            }
            else
            {
                // if away from buffer zone and not reaching bottom of content
                if (direction == DIRECTION.VERTICAL)
                {
                    if (viewPos.y > buffer && pos.y - offset > -scrollView.content.sizeDelta.y)
                    {
                        items[i].transform.localPosition = new Vector2(0, pos.y - offset);
                        var itemId = int.Parse(items[i].name) + items.Count;
                        items[i].name = itemId + "";

                        //items[i].transform.SetSiblingIndex(itemId);
                        if (updateCallback != null)
                            updateCallback(items[i], (JObject)listItemData[itemId]);
                    }
                }
                else
                {
                    if (viewPos.x < -buffer && pos.x + offset < scrollView.content.sizeDelta.x)
                    {
                        items[i].transform.localPosition = new Vector2(pos.x + offset, 0);
                        var itemId = int.Parse(items[i].name) + items.Count;
                        items[i].name = itemId + "";

                        items[i].transform.SetSiblingIndex(itemId);
                        if (updateCallback != null)
                            updateCallback(items[i], (JObject)listItemData[itemId]);
                    }
                }

            }
        }
        // update lastContentPosY
        lastContentPosX = scrollView.content.localPosition.x;
        lastContentPosY = scrollView.content.localPosition.y;
    }
    public void ScrollToTop()
    {
        //scrollView.verticalNormalizedPosition = 1;
        scrollView.DOVerticalNormalizedPos(1.0f, 0.2f).SetEase(Ease.OutSine);
    }
    public void ScrollToBottom()
    {
        //scrollView.verticalNormalizedPosition = 0;
        scrollView.DOVerticalNormalizedPos(0, 0.2f).SetEase(Ease.OutSine);
    }

    public void ScrollToLeft()
    {
        scrollView.DOHorizontalNormalizedPos(0, 0.2f).SetEase(Ease.OutSine);
        //scrollView.horizontalNormalizedPosition = 0;
    }
    public void ScrollToRight()
    {
        scrollView.DOHorizontalNormalizedPos(1.0f, 0.2f).SetEase(Ease.OutSine);
        //scrollView.horizontalNormalizedPosition = 1;
    }
}
