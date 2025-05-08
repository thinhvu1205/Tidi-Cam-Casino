using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Globals;
using UnityEngine.U2D;
using System;

public class HistoryBaccarat_BigEye : MonoBehaviour
{
    [SerializeField] GameObject tb_BigEye;
    [SerializeField] GameObject hisBigEye;
    [SerializeField] SpriteAtlas BaccaratAtlas;
    // Start is called before the first frame update
    List<GameObject> old_Big_Eye = new List<GameObject>();
    List<int> dataBigEyeRoad = new List<int>();
    [HideInInspector]
    private int HISTORY_LAST_BIGEYE_COL = 0, HISTORY_LAST_BIGEYE_VALUE = -1, HISTORY_LAST_BIGEYE_ROW = 0, HISTORY_STREAK_BIGEYE = 0, HISTORY_LAST_VALUE = -1, HISTORY_LAST_RESULT_ROW = 0, HISTORY_LAST_RESULT_COL = 0;
    [HideInInspector]
    private int indexBigEye = 1, HISTORY_STREAK = 1;
    [HideInInspector]
    private bool isChuyenCot = false, HISTORY_STATE_IS_VERTICAL_BIGEYE = false, HISTORY_STATE_IS_VERTICAL = false;
    [HideInInspector] int[][] historyBigEyeArr;
    [HideInInspector] int[][] historyArray;
    [HideInInspector] GameObject BIG_EYE_CHILD;

    private void Awake()
    {
      
        old_Big_Eye.Add(hisBigEye);
        old_Big_Eye.Add(hisBigEye);
        setup2dArray();
    }
    private void setup2dArray()
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
        // vt Pos BigEyes
        historyBigEyeArr = new int[6][];
        for (int i = 0; i < historyBigEyeArr.Length; i++)
        {
            historyBigEyeArr[i] = new int[200];
        }
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < 200; k++)
            {
                historyBigEyeArr[j][k] = 0;

            }
        }
        Debug.Log("historyArray size=" + historyArray.Length);
        //vt smallroad
        //historySmallRoad = new Array(6);
        //for (let i = 0; i < historySmallRoad.length; i++)
        //{
        //    historySmallRoad[i] = new Array(200);
        //}
        //for (let j = 0; j < 6; j++)
        //{
        //    for (let k = 0; k < 200; k++)
        //    {
        //        historySmallRoad[j][k] = (0)

        //    }
        //}
        ////vt cock road
        //historyCockRoad = new Array(6);
        //for (let i = 0; i < historyCockRoad.length; i++)
        //{
        //    historyCockRoad[i] = new Array(200);
        //}
        //for (let j = 0; j < 6; j++)
        //{
        //    for (let k = 0; k < 200; k++)
        //    {
        //        historyCockRoad[j][k] = (0)

        //    }
        //}
    }

    // Update is called once per frame
    private Vector2 convertArrayIndexToPosition(int row, int col, string typeTable)
    {
        int indexCol = 0;
        float indexRow = 0;
        if (HISTORY_LAST_BIGEYE_COL >= 64 * (indexBigEye)) row -= 64 * (indexBigEye);
        if (row % 2 != 0) indexRow = 0.3f;
        if (row == 0) indexRow = 2;
        if (col == 0) indexCol = 2;
        return new Vector2((row * 11.8f + indexRow), (col * -13 - indexCol));
    }
    private void dataBigEye()
    { //type win xanh do cua bigRoad
        int itemIndex = 0;
        if (!isChuyenCot && dataBigEyeRoad.Count > 0 && HISTORY_STREAK > 1)
        {
            if (HISTORY_STREAK == dataBigEyeRoad[dataBigEyeRoad.Count - 1] + 1)
            {
                itemIndex = 2;
            }
            else
            {
                itemIndex = 1;
            }
        }
        else
        {
            if (dataBigEyeRoad.Count > 1)
            {
                if (dataBigEyeRoad[dataBigEyeRoad.Count - 1] != dataBigEyeRoad[dataBigEyeRoad.Count - 2])
                {
                    itemIndex = 2;
                }
                else
                {
                    itemIndex = 1;
                }
            }
            else
            {
                return;
            }
        }
        getPositionBigEyeRoad(itemIndex);

    }
    private void getPositionBigEyeRoad(int result)
    {
        //First Time Running 
        if (HISTORY_LAST_BIGEYE_VALUE == -1)
        {
            HISTORY_LAST_BIGEYE_ROW = 0;
            HISTORY_LAST_BIGEYE_COL = 0;
            HISTORY_LAST_BIGEYE_VALUE = result;
            historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

        }
        else
        {
            //Ket qua giong van truoc
            if (result == HISTORY_LAST_BIGEYE_VALUE)
            {
                if (HISTORY_STREAK_BIGEYE == 0)
                {
                    HISTORY_STATE_IS_VERTICAL_BIGEYE = true;
                }
                HISTORY_STREAK_BIGEYE++;
                if (HISTORY_STATE_IS_VERTICAL_BIGEYE)
                {
                    if (HISTORY_LAST_BIGEYE_ROW >= 5 || historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW + 1][HISTORY_LAST_BIGEYE_COL] == 1)
                    {
                        HISTORY_STATE_IS_VERTICAL_BIGEYE = false;
                        HISTORY_LAST_BIGEYE_COL++;
                        historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

                    }
                    else
                    {
                        if (historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW + 1][HISTORY_LAST_BIGEYE_COL] == 1)
                        {
                            HISTORY_STATE_IS_VERTICAL_BIGEYE = false;
                            HISTORY_LAST_BIGEYE_COL++;
                            historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

                        }
                        else
                        {
                            HISTORY_LAST_BIGEYE_ROW++;
                            historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

                        }
                    }
                }
                else
                {
                    HISTORY_LAST_BIGEYE_COL += 1;
                    historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

                }
                HISTORY_LAST_BIGEYE_VALUE = result;
            }
            //Ket qua khong giong van truoc
            else
            {
                // mac dinh quay lai thang dau tien trong tren hang 1
                for (int j = 0; j < historyBigEyeArr[0].Length; j++)
                {
                    if (historyBigEyeArr[0][j] == 0)
                    {
                        HISTORY_LAST_BIGEYE_COL = j;
                        break;
                    }
                }
                HISTORY_STREAK_BIGEYE = 0;
                HISTORY_LAST_BIGEYE_ROW = 0;
                HISTORY_LAST_BIGEYE_VALUE = result;
                historyBigEyeArr[HISTORY_LAST_BIGEYE_ROW][HISTORY_LAST_BIGEYE_COL] = 1;

            }
        }
        setPositionItemBigEye(result);
    }
    private void setPositionItemBigEye(int state)
    {
        GameObject itemHis = new GameObject();
        Image sprItemHis = itemHis.AddComponent<Image>();
        itemHis.transform.localScale = new Vector2(0.5f, 0.5f);
        if (state != 3)
        {
            sprItemHis.sprite = getSprBigRoad(state, "BreadRoad");
        }
        sprItemHis.SetNativeSize();
        RectTransform rect = sprItemHis.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
        var numChild = tb_BigEye.transform.childCount;
        itemHis.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
        if (HISTORY_LAST_BIGEYE_COL >= 64 * indexBigEye)
        {
            if (HISTORY_LAST_BIGEYE_COL == 64 * numChild)
            {
                var bkgTable = new GameObject();
                bkgTable.GetComponent<RectTransform>().sizeDelta = new Vector2(754, 77);
                bkgTable.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                Image sprBkgTable = bkgTable.AddComponent<Image>();
                sprBkgTable.sprite = getSprBigRoad(0, "BigEye");
                sprBkgTable.SetNativeSize();
                bkgTable.transform.SetParent(tb_BigEye.transform);
                sprBkgTable.transform.localScale = Vector2.one;
                old_Big_Eye.Add(bkgTable);
                if (HISTORY_LAST_BIGEYE_COL == 64)
                {
                    indexBigEye = 1;
                }
                else
                {
                    indexBigEye++;
                }
            }
            BIG_EYE_CHILD = old_Big_Eye[old_Big_Eye.Count - 1];
            itemHis.transform.SetParent(BIG_EYE_CHILD.transform);
        }
        else
        {
            BIG_EYE_CHILD = old_Big_Eye[old_Big_Eye.Count - 2];
            itemHis.transform.SetParent(BIG_EYE_CHILD.transform);
            if (HISTORY_LAST_BIGEYE_COL >= 64)
            {
                Vector2 vectorPos = convertArrayIndexToPosition(HISTORY_LAST_BIGEYE_COL - 64 * (indexBigEye - 1), HISTORY_LAST_BIGEYE_ROW, "BigEye");
                itemHis.transform.localPosition = vectorPos;
                return;
            }
        }
        Vector2 vector = convertArrayIndexToPosition(HISTORY_LAST_BIGEYE_COL, HISTORY_LAST_BIGEYE_ROW, "BigEye");
        itemHis.transform.localPosition = new Vector2(vector.x,vector.y-2);
        itemHis.transform.localScale = Vector2.one;

    }
    private Sprite getSprBigRoad(int typeWin, string table)
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
  
    public void getPositionToInsertSprite(int result, int typeWin)
    {
        //result = parseInt(result);
        // if item fist = tie
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
            Debug.Log("historyArray size=" + historyArray.Length);
            historyArray[HISTORY_LAST_RESULT_ROW][HISTORY_LAST_RESULT_COL] = 1;

        }
        else
        {
            //Neu ket qua hoa
            if (result == 3)
            {
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
        //setPositionSpriteHistory(result, typeWin);
        dataBigEye();
        //dataIndexSmallRoad();
        //dataIndexCockRoad();
    }
    public void updateHistoryBigEye(int typeWin)
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

}