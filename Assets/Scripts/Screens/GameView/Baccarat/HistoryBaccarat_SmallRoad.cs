using Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HistoryBaccarat_SmallRoad : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] protected int HISTORY_LAST_SMALL_VALUE = -1, HISTORY_LAST_SMALL_ROW = 0, HISTORY_LAST_SMALL_COL = 0, HISTORY_STREAK_SMALL = 0, indexSmallRoad = 1, HISTORY_STREAK = 1, HISTORY_LAST_VALUE=-1, HISTORY_LAST_RESULT_ROW=0, HISTORY_LAST_RESULT_COL=0;
    [HideInInspector]  protected bool HISTORY_STATE_IS_VERTICAL_SMALL = false, isChuyenCot = false, HISTORY_STATE_IS_VERTICAL = false;
    [HideInInspector] int[][] historySmallRoad;
    [HideInInspector] protected List<int> dataBigEyeRoad = new List<int>();
    [SerializeField] protected SpriteAtlas BaccaratAtlas;
    [SerializeField] GameObject tb_SmallRoad;
    [SerializeField] GameObject hisSmallRoad;
    [HideInInspector] GameObject SMALL_CHILD;
    [HideInInspector] protected int[][] historyArray;
    [HideInInspector] List<GameObject> old_small_road = new List<GameObject>();
    protected virtual void Awake()
    {
      
        old_small_road.Add(hisSmallRoad);
        old_small_road.Add(hisSmallRoad);
        setup2dArray();
    }
    protected virtual void setup2dArray()
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
        historySmallRoad = new int[6][];
        for (int i = 0; i < historySmallRoad.Length; i++)
        {
            historySmallRoad[i] = new int[200];
        }
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < 200; k++)
            {
                historySmallRoad[j][k] = 0;
            }
        }
    }
    // Update is called once per frame
    private void getPositionSmallRoad(int result)
    {
        //result = parseInt(result);
        //First Time Running 
        if (HISTORY_LAST_SMALL_VALUE == -1)
        {
            HISTORY_LAST_SMALL_ROW = 0;
            HISTORY_LAST_SMALL_COL = 0;
            HISTORY_LAST_SMALL_VALUE = result;
            historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

        }
        else
        {
            // ket qua giong van truoc
            if (result == HISTORY_LAST_SMALL_VALUE)
            {
                if (HISTORY_STREAK_SMALL == 0)
                {
                    HISTORY_STATE_IS_VERTICAL_SMALL = true;
                }
                HISTORY_STREAK_SMALL++;
                if (HISTORY_STATE_IS_VERTICAL_SMALL)
                {
                    if (HISTORY_LAST_SMALL_ROW >= 5 || historySmallRoad[HISTORY_LAST_SMALL_ROW + 1][HISTORY_LAST_SMALL_COL] == 1)
                    {
                        HISTORY_STATE_IS_VERTICAL_SMALL = false;
                        HISTORY_LAST_SMALL_COL++;
                        historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

                    }
                    else
                    {
                        if (historySmallRoad[HISTORY_LAST_SMALL_ROW + 1][HISTORY_LAST_SMALL_COL] == 1)
                        {
                            HISTORY_STATE_IS_VERTICAL_SMALL = false;
                            HISTORY_LAST_SMALL_COL++;
                            historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

                        }
                        else
                        {
                            HISTORY_LAST_SMALL_ROW++;
                            historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

                        }
                    }
                }
                else
                {
                    HISTORY_LAST_SMALL_COL += 1;
                    historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

                }
                HISTORY_LAST_SMALL_VALUE = result;
            }
            // ket qua ko giong van truoc
            else
            {
                //mac dinh quay lai thang dau tien trong tren hang 1
                for (int j = 0; j < historySmallRoad[0].Length; j++)
                {
                    if (historySmallRoad[0][j] == 0)
                    {
                        HISTORY_LAST_SMALL_COL = j;
                        break;
                    }
                }
                HISTORY_STREAK_SMALL = 0;
                HISTORY_LAST_SMALL_ROW = 0;
                HISTORY_LAST_SMALL_VALUE = result;
                historySmallRoad[HISTORY_LAST_SMALL_ROW][HISTORY_LAST_SMALL_COL] = 1;

            }
        }
        setPositionItemSmallRoad(result);
    }
    private void setPositionItemSmallRoad(int state)
    {
        GameObject itemHis = new GameObject();
        Image sprItemHis = itemHis.AddComponent<Image>();
     
        if (state != 3)
        {
            sprItemHis.sprite = getSprBigRoad(state, "SmallRoad");
        }
        sprItemHis.SetNativeSize();
      var numChild = tb_SmallRoad.transform.childCount;
        itemHis.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        if (HISTORY_LAST_SMALL_COL >= 30 * indexSmallRoad)
        {
            if (HISTORY_LAST_SMALL_COL == 30 * numChild)
            {
                var bkgTable = new GameObject();
                bkgTable.GetComponent<RectTransform>().sizeDelta = new Vector2(355, 77);
                bkgTable.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                Image sprBkgTable = bkgTable.AddComponent<Image>();
                sprBkgTable.sprite = getSprBigRoad(0, "cockroad");
                sprBkgTable.SetNativeSize();
                bkgTable.transform.parent = tb_SmallRoad.transform;
                old_small_road.Add(bkgTable);
                if (HISTORY_LAST_SMALL_COL == 30)
                {
                    indexSmallRoad = 1;
                }
                else
                {
                    indexSmallRoad++;
                }
            }
            SMALL_CHILD = old_small_road[old_small_road.Count - 1];
            itemHis.transform.SetParent(SMALL_CHILD.transform);
        }
        else
        {
            SMALL_CHILD = old_small_road[old_small_road.Count - 2];;
            itemHis.transform.SetParent(SMALL_CHILD.transform);
            if (HISTORY_LAST_SMALL_COL >= 30)
            {
                Vector2 vectorPos = convertArrayIndexToPosition(HISTORY_LAST_SMALL_COL - 30 * (indexSmallRoad - 1), HISTORY_LAST_SMALL_ROW, "SmallRoad");
                itemHis.transform.localPosition= vectorPos;
                return;
            }
        }
        Vector2 vector = convertArrayIndexToPosition(HISTORY_LAST_SMALL_COL, HISTORY_LAST_SMALL_ROW, "SmallRoad");
        itemHis.transform.localPosition= new Vector2(vector.x,vector.y-2);
        itemHis.transform.localScale = new Vector2(0.5f, 0.5f);

    }
    protected Vector2 convertArrayIndexToPosition(int row, int col, string typeTable)
    {
        int indexCol = 0;
        float indexRow = 0;
        if (HISTORY_LAST_SMALL_COL >= 30 * (indexSmallRoad)) row -= 30 * (indexSmallRoad);
        if (row % 2 != 0) indexRow = 0.3f;
        if (row == 0) indexRow = 2;
        if (col == 0) indexCol = 2;
        return new Vector2((row * 11.8f + indexRow), (col * -13 - indexCol));
    }
    protected Sprite getSprBigRoad(int typeWin, string table)
    {
        Sprite spr;
        string spritNameFrefix = "";
        string sprName = "";
        switch (table)
        {
            case "BigRoad":
                spritNameFrefix = "BigRoad";
                break;
            case "BreadRoad":
                spritNameFrefix = "BreadRoad";
                break;
            case "CockRoad":
                spritNameFrefix = "CockRoad";
                break;
            case "SmallRoad":
                spritNameFrefix = "SmallRoad";
                break;
            case "BigEye":
                spritNameFrefix = "BigEye";
                break;
            case "cockroad":
                spritNameFrefix = "cockroad";
                break;
        }
        switch (typeWin)
        {
            case TYPEWIN_BACCARAT.PLAYER:
                sprName = "_P";
                break;
            case TYPEWIN_BACCARAT.PLAYER_P:
                sprName = "_P_P";
                break;
            case TYPEWIN_BACCARAT.PLAYER_B:
                sprName = "_P_B";
                break;
            case TYPEWIN_BACCARAT.PLAYER_PB:
                sprName = "_P_PB";
                break;
            case TYPEWIN_BACCARAT.BANKER:
                sprName = "_B";
                break;
            case TYPEWIN_BACCARAT.BANKER_P:
                sprName = "_B_P";
                break;
            case TYPEWIN_BACCARAT.BANKER_B:
                sprName = "_B_B";
                break;
            case TYPEWIN_BACCARAT.BANKER_PB:
                sprName = "_B_PB";
                break;
            case TYPEWIN_BACCARAT.TIE:
            case TYPEWIN_BACCARAT.TIE_P:
            case TYPEWIN_BACCARAT.TIE_B:
            case TYPEWIN_BACCARAT.TIE_PB:
                sprName = "_T";
                break;
        }
        spr = BaccaratAtlas.GetSprite(spritNameFrefix + sprName);
        return spr;
    }
    public void updateHistorySmallRoad(int typeWin)
    {
        GameObject itemHis = new GameObject();
        Image sprItemHis = itemHis.AddComponent<Image>();
        sprItemHis.sprite = getSprBigRoad(typeWin, "BigRoad");

        int resultWin = 0;
        switch (typeWin)
        {
            case TYPEWIN_BACCARAT.PLAYER:
                resultWin = 2;
                break;
            case TYPEWIN_BACCARAT.PLAYER_P:
                resultWin = 2;
                break;
            case TYPEWIN_BACCARAT.PLAYER_B:
                resultWin = 2;
                break;
            case TYPEWIN_BACCARAT.PLAYER_PB:
                resultWin = 2;
                break;
            case TYPEWIN_BACCARAT.BANKER:
                resultWin = 1;
                break;
            case TYPEWIN_BACCARAT.BANKER_P:
                resultWin = 1;
                break;
            case TYPEWIN_BACCARAT.BANKER_B:
                resultWin = 1;
                break;
            case TYPEWIN_BACCARAT.BANKER_PB:
                resultWin = 1;
                break;
            case TYPEWIN_BACCARAT.TIE:
            case TYPEWIN_BACCARAT.TIE_P:
            case TYPEWIN_BACCARAT.TIE_B:
            case TYPEWIN_BACCARAT.TIE_PB:
                resultWin = 3;
                break;
        }
        getPositionToInsertSprite(resultWin, typeWin);
    }
    private void dataIndexSmallRoad()
    {
        int itemIndex = 0;
        if (!isChuyenCot && dataBigEyeRoad.Count > 1 && HISTORY_STREAK > 1)
        {
            if (HISTORY_STREAK == dataBigEyeRoad[dataBigEyeRoad.Count - 2] + 1)
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
            if (dataBigEyeRoad.Count > 2)
            {
                if (dataBigEyeRoad[dataBigEyeRoad.Count - 1] != dataBigEyeRoad[dataBigEyeRoad.Count - 3])
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
        getPositionSmallRoad(itemIndex);

    }
    protected virtual void getPositionToInsertSprite(int result, int typeWin)
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
        dataIndexSmallRoad();
        //dataIndexCockRoad();
    }
}
