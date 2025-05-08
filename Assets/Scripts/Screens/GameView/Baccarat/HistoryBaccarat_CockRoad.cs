using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryBaccarat_CockRoad : HistoryBaccarat_SmallRoad
{
    // Start is called before the first frame update
    [HideInInspector] int HISTORY_LAST_COCK_VALUE = 0, HISTORY_LAST_COCK_ROW = 0, HISTORY_LAST_COCK_COL = 0, HISTORY_STREAK_COCK = 0, indexCockRoad = 1;
    [HideInInspector] bool HISTORY_STATE_IS_VERTICAL_COCK = false;
    [HideInInspector] int[][]historyCockRoad;
    [SerializeField] GameObject tb_CockRoad;
    [HideInInspector] GameObject COCK_CHILD;
    [SerializeField] GameObject hisCockRoad;
    [HideInInspector] List<GameObject> old_cock_road = new List<GameObject>();
    private List<int> listTemp = new List<int>();
    protected override void Awake()
    {
        old_cock_road.Add(hisCockRoad);
        old_cock_road.Add(hisCockRoad);
        setup2dArray();
    }
    // Update is called once per frame
    protected override void setup2dArray()
    {
        historyArray = new int[6][];
        for (int i = 0; i < historyArray.Length; i++)
        {
            historyArray[i] = new int[200];
        }
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < 200; k++)
            {
                historyArray[j][k] = 0;

            }
        }
        historyCockRoad = new int[6][];
        for (int i = 0; i < historyCockRoad.Length; i++)
        {
            historyCockRoad[i] = new int[200];
        }
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < 200; k++)
            {
                historyCockRoad[j][k] = 0;
            }
        }
    }
    private void dataIndexCockRoad()
    {
        int itemIndex = 0;
        if (!isChuyenCot && dataBigEyeRoad.Count > 2 && HISTORY_STREAK > 1)
        {
            if (HISTORY_STREAK == dataBigEyeRoad[dataBigEyeRoad.Count - 3] + 1)
            {
                itemIndex = 1;
            }
            else
            {
                itemIndex = 2;
            }
        }
        else
        {
            if (dataBigEyeRoad.Count > 3)
            {
                if (dataBigEyeRoad[dataBigEyeRoad.Count - 1] != dataBigEyeRoad[dataBigEyeRoad.Count - 4])
                {
                    itemIndex = 1;
                }
                else
                {
                    itemIndex = 2;
                }
            }
            else
            {
                return;
            }
        }
        getPositionCockRoad(itemIndex);

    }
    private void getPositionCockRoad(int result)
    {
        //result = parseInt(result);
        //First Time Running 
        if (HISTORY_LAST_COCK_VALUE == 0)
        {
            HISTORY_LAST_COCK_ROW = 0;
            HISTORY_LAST_COCK_COL = 0;
            HISTORY_LAST_COCK_VALUE = result;
            historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

        }
        else
        {
            if (result == HISTORY_LAST_COCK_VALUE)
            {
                if (HISTORY_STREAK_COCK == 0)
                {
                    HISTORY_STATE_IS_VERTICAL_COCK = true;
                }
                HISTORY_STREAK_COCK++;
                if (HISTORY_STATE_IS_VERTICAL_COCK)
                {
                    if (HISTORY_LAST_COCK_ROW >= 5 || historyCockRoad[HISTORY_LAST_COCK_ROW + 1][HISTORY_LAST_COCK_COL] == 1)
                    {
                        HISTORY_STATE_IS_VERTICAL_COCK = false;
                        HISTORY_LAST_COCK_COL++;
                        historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

                    }
                    else
                    {
                        if (historyCockRoad[HISTORY_LAST_COCK_ROW + 1][HISTORY_LAST_COCK_COL] == 1)
                        {
                            HISTORY_STATE_IS_VERTICAL_COCK = false;
                            HISTORY_LAST_COCK_COL++;
                            historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

                        }
                        else
                        {
                            HISTORY_LAST_COCK_ROW++;
                            historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

                        }
                    }
                }
                else
                {
                    HISTORY_LAST_COCK_COL += 1;
                    historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

                }
                HISTORY_LAST_COCK_VALUE = result;
            }
            else
            {
                for (int j = 0; j < historyCockRoad[0].Length; j++)
                {
                    if (historyCockRoad[0][j] == 0)
                    {
                        HISTORY_LAST_COCK_COL = j;
                        break;
                    }
                }
                HISTORY_STREAK_COCK = 0;
                HISTORY_LAST_COCK_ROW = 0;
                HISTORY_LAST_COCK_VALUE = result;
                historyCockRoad[HISTORY_LAST_COCK_ROW][HISTORY_LAST_COCK_COL] = 1;

            }
        }
        setPositionItemCockRoad(result);
    }
    private void setPositionItemCockRoad(int state)
    {
        GameObject itemHis = new GameObject();
        Image sprItemHis = itemHis.AddComponent<Image>();

        if (state != 3)
        {
            sprItemHis.sprite = getSprBigRoad(state, "CockRoad");
        }
        sprItemHis.SetNativeSize();
        var numChild = tb_CockRoad.transform.childCount;
        itemHis.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        if (HISTORY_LAST_COCK_COL >= 30 * indexCockRoad)
        {
            if (HISTORY_LAST_COCK_COL == 30 * numChild)
            {
                var bkgTable = new GameObject();
                RectTransform bkgRect = bkgTable.AddComponent<RectTransform>();
                bkgRect.sizeDelta = new Vector2(355, 77);
                bkgRect.pivot = new Vector2(0, 1);
                Image sprBkgTable = bkgTable.AddComponent<Image>();
                sprBkgTable.sprite = getSprBigRoad(0, "cockroad");
                sprBkgTable.SetNativeSize();
                tb_CockRoad.transform.SetParent(bkgTable.transform);
                sprBkgTable.transform.localScale = Vector2.one;
                old_cock_road.Add(bkgTable);
                if (HISTORY_LAST_COCK_COL == 30)
                {
                    indexCockRoad = 1;
                }
                else
                {
                    indexCockRoad++;
                }
            }
            COCK_CHILD = old_cock_road[old_cock_road.Count - 1];
            itemHis.transform.SetParent(COCK_CHILD.transform);
        }
        else
        {
            COCK_CHILD = old_cock_road[old_cock_road.Count - 2];
            itemHis.transform.parent = COCK_CHILD.transform;
            if (HISTORY_LAST_COCK_COL >= 30)
            {
                Vector2 vectorPos = convertArrayIndexToPosition(HISTORY_LAST_COCK_COL - 30 * (indexCockRoad - 1), HISTORY_LAST_COCK_ROW, "CockRoad");
                itemHis.transform.localPosition = new Vector2(vectorPos.x, vectorPos.y - 2);
                return;
            }
        }
        Vector2 vector = convertArrayIndexToPosition(HISTORY_LAST_COCK_COL, HISTORY_LAST_COCK_ROW, "CockRoad");
        itemHis.transform.localPosition = new Vector2(vector.x, vector.y - 2);
        itemHis.transform.localScale = new Vector2(0.5f, 0.5f);

    }
    protected override void getPositionToInsertSprite(int result, int typeWin)
    {
        if (result == 3 && HISTORY_LAST_VALUE == -1)
        {
            HISTORY_LAST_VALUE = -1;
            return;
        }
        //First Time Running 
        if (HISTORY_LAST_VALUE == -1)
        {
            HISTORY_LAST_RESULT_ROW = 0;
            HISTORY_LAST_RESULT_COL = 0;
            HISTORY_LAST_VALUE = result;
            historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

        }
        else
        {
            //Neu ket qua hoa
            if (result == 3)
            {
                Vector2 vector = convertArrayIndexToPosition(HISTORY_LAST_RESULT_COL, HISTORY_LAST_RESULT_ROW, "");
                //setPositionSpriteHistory(result, typeWin); set sau
                return;
            }
            //Neu ket qua giong van truoc
            if (result == HISTORY_LAST_VALUE)
            {
                isChuyenCot = false;
                if (HISTORY_STREAK == 1)
                {
                    HISTORY_STATE_IS_VERTICAL = true;
                }
                HISTORY_STREAK++;
                if (HISTORY_STATE_IS_VERTICAL)
                {
                    if (HISTORY_LAST_RESULT_ROW >= 5 || historyArray[HISTORY_LAST_RESULT_ROW + 1][HISTORY_LAST_RESULT_COL] == 1)
                    {
                        HISTORY_STATE_IS_VERTICAL = false;
                        HISTORY_LAST_RESULT_COL++;
                        historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

                    }
                    else
                    {
                        if (historyArray[HISTORY_LAST_RESULT_ROW + 1][HISTORY_LAST_RESULT_COL] == 1)
                        {
                            HISTORY_STATE_IS_VERTICAL = false;
                            HISTORY_LAST_RESULT_COL++;
                            historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

                        }
                        else
                        {
                            HISTORY_LAST_RESULT_ROW++;
                            historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

                        }
                    }
                }
                else
                {
                    HISTORY_LAST_RESULT_COL += 1;
                    historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

                }
                HISTORY_LAST_VALUE = result;
            }
            //Ket qua khong giong van truoc(doi cot)
            else
            {
                //Mac dinh quay lai thang dau tien trong tren hang 1
                for (int j = 0; j < historyArray[0].Length; j++)
                {
                    if (historyArray[0][j] == 0)
                    {
                        HISTORY_LAST_RESULT_COL = j;
                        break;
                    }
                }
                dataBigEyeRoad.Add(HISTORY_STREAK);
                isChuyenCot = true;
                HISTORY_STREAK = 1;
                HISTORY_LAST_RESULT_ROW = 0;
                HISTORY_LAST_VALUE = result;
                historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

            }
        }
        dataIndexCockRoad();
    }

}
