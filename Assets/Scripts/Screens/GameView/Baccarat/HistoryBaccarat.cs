using Globals;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HistoryBaccarat : BaseView
{
    [SerializeField]
    public TextMeshProUGUI lb_his_banker_detail;

    [SerializeField]
    public TextMeshProUGUI lb_his_player_detail;

    [SerializeField]
    public TextMeshProUGUI lb_his_tie_detail;

    [SerializeField]
    public TextMeshProUGUI lb_his_bankerPair;

    [SerializeField]
    public TextMeshProUGUI lb_his_playerPair;

    [SerializeField]
    private Transform layer1;

    [SerializeField]
    private GameObject parentMinHisLayer1;

    [SerializeField]
    private Transform layer2;

    [SerializeField]
    private GameObject parentMinHisLayer2;

    [SerializeField]
    public List<GameObject> listDots = new List<GameObject>();

    [SerializeField]
    public List<GameObject> listDots2 = new List<GameObject>();
    [SerializeField]
    public HistoryBaccarat_SmallRoad tableSmallRoad;
    public HistoryBaccarat_BigEye tableBigEyes;
    public HistoryBaccarat_CockRoad tableCockRoad;

    [HideInInspector] public int numWinB = 0, numWinP = 0, numWinT = 0, numWinBP = 0, numWinPP = 0;

    private List<List<int>> resultHis = new List<List<int>>();

    private List<List<int>> resultHis2 = new List<List<int>>();


    // Update is called once per frame
    void Update()
    {
        transform.SetAsLastSibling();
    }

    public void handleResultHisLayer1(List<int> listHis)
    {
        // listHis = new List<int> { 101, 2, 1, 2, 3, 2, 3, 2, 3, 2, 1, 2, 1, 1, 2, 1, 1, 2, 11, 102, 2, 2, 2, 2, 2, 2, 11, 2, 11, 2, 11, 1, 1, 2, 1, 13, 2, 1, 2, 2, 2, 1, 1, 101, 3, 11, 2, 1, 1, 102 };
        List<int> currentList = new List<int> { listHis[0] };
        resultHis.Add(currentList);

        for (int i = 1; i < listHis.Count; i++)
        {
            if (currentList.Count < 6)
            {
                currentList.Add(listHis[i]);
            }
            else
            {
                currentList = new List<int> { listHis[i] };
                resultHis.Add(currentList);
            }
        }

        Vector2 currentPos = new Vector2(-163, 63);

        UIManager.instance.destroyAllChildren(layer1);
        for (int i = (resultHis.Count < 14 ? 0 : resultHis.Count - 14); i < resultHis.Count; i++)
        {
            GameObject itemDotCol = Instantiate(parentMinHisLayer1, layer1);
            itemDotCol.transform.localPosition = new Vector2(currentPos.x, currentPos.y);
            currentPos.x = currentPos.x + 25f;
            itemDotCol.SetActive(true);


            for (int j = 0; j < resultHis[i].Count; j++)
            {
                if (resultHis[i][j] == 1)
                {

                    GameObject itemHis = Instantiate(listDots[0], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 2)
                {

                    GameObject itemHis = Instantiate(listDots[4], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 3)
                {

                    GameObject itemHis = Instantiate(listDots[8], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 102)
                {

                    GameObject itemHis = Instantiate(listDots[6], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 12)
                {

                    GameObject itemHis = Instantiate(listDots[5], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 112)
                {

                    GameObject itemHis = Instantiate(listDots[7], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 101)
                {

                    GameObject itemHis = Instantiate(listDots[2], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 11)
                {

                    GameObject itemHis = Instantiate(listDots[1], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis[i][j] == 111)
                {

                    GameObject itemHis = Instantiate(listDots[3], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else
                {
                    GameObject itemHis = Instantiate(listDots[8], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
            }
        }
        resultHis.Clear();
    }

    public void handleResultHisLayer2(List<int> listHis)
    {
        //listHis = new List<int> { 101, 2, 1, 2, 3, 2, 3, 2, 3, 2, 1, 2, 1, 1, 2, 1, 1, 2, 11, 102, 2, 2, 2, 2, 2, 2, 11, 2, 11, 2, 11, 1, 1, 2, 1, 13, 2, 1, 2, 2, 2, 1, 1, 101, 3, 11, 2, 1, 1, 102 };
        List<int> newArr = new List<int>();

        foreach (int element in listHis)
        {
            if (element != 3 && element != 13 && element != 103 && element != 113)
            {
                newArr.Add(element);
            }
        }

        List<int> currentList = new List<int> { newArr[0] };
        resultHis2.Add(currentList);

        for (int i = 1; i < newArr.Count; i++)
        {
            if ((newArr[i] % 10) == (newArr[i - 1] % 10) && currentList.Count < 6)
            {
                currentList.Add(newArr[i]);
            }
            else
            {
                currentList = new List<int> { newArr[i] };
                resultHis2.Add(currentList);
            }
        }

        Vector2 currentPos = new Vector2(-363, 61);

        List<Vector2> listPos = new List<Vector2> { new Vector2(-363, 61),
                                                    new Vector2(-340,61),
                                                    new Vector2(-317,61),
                                                    new Vector2(-294,61),
                                                    new Vector2(-271,61),
                                                    new Vector2(-245,61),
                                                    new Vector2(-223,61),
                                                    new Vector2(-200,61),
                                                    new Vector2(-176,61),
                                                    new Vector2(-152,61),
                                                    new Vector2(-128,61),
                                                    new Vector2(-105,61),
                                                    new Vector2(-82,61),
                                                    new Vector2(-59,61),
                                                    new Vector2(-35,61),
                                                    new Vector2(-12,61),
                                                    new Vector2(12,61),
                                                    new Vector2(37,61),
                                                    new Vector2(59,61),
                                                    new Vector2(84,61),
                                                    new Vector2(106,61),
                                                    new Vector2(129,61),
                                                    new Vector2(152,61),
                                                    new Vector2(178,61),
                                                    new Vector2(202,61),
                                                    new Vector2(223,61),
                                                    new Vector2(247,61),
                                                    new Vector2(272,61),
                                                    new Vector2(296,61),
                                                    new Vector2(318,61),
                                                    new Vector2(343,61),
                                                    new Vector2(367,61)};


        UIManager.instance.destroyAllChildren(layer2);
        for (int i = (resultHis2.Count < 32 ? 0 : resultHis2.Count - 32); i < resultHis2.Count; i++)
        {
            GameObject itemDotCol = Instantiate(parentMinHisLayer2, layer2);
            itemDotCol.transform.localPosition = listPos[i];
            itemDotCol.SetActive(true);

            for (int j = 0; j < resultHis2[i].Count; j++)
            {
                if (resultHis2[i][j] == 1)
                {

                    GameObject itemHis = Instantiate(listDots2[0], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 2)
                {

                    GameObject itemHis = Instantiate(listDots2[4], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 102)
                {

                    GameObject itemHis = Instantiate(listDots2[6], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 12)
                {

                    GameObject itemHis = Instantiate(listDots2[5], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 112)
                {

                    GameObject itemHis = Instantiate(listDots2[7], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 101)
                {
                    GameObject itemHis = Instantiate(listDots2[2], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 11)
                {

                    GameObject itemHis = Instantiate(listDots2[1], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
                else if (resultHis2[i][j] == 111)
                {

                    GameObject itemHis = Instantiate(listDots2[3], itemDotCol.transform);
                    itemHis.SetActive(true);

                }
            }
        }
        resultHis2.Clear();
    }
    public void handleResultBigEyes(List<int> listHis)
    {
        //listHis.Clear();
        //List<int> gate = new List<int> { 1, 2, 3, 102, 12, 101, 101, 11, 12, 12, 12, 101, 102, 111, 103, 103, 13, 113, 113, 1, 1, 1, 2, 2, 3, 1, 2, 3, 102, 12, 101, 101, 11, 12, 12, 12, 101, 102, 111, 103, 103, 13, 113, 113, 1, 1, 1, 2, 2, 3 };
        //for (int i = 0; i < 50; i++)
        //{
        //    listHis.Add(gate[Random.Range(0, gate.Count)]);
        //}
        //listHis = gate;
        for (int i = 0, l = listHis.Count; i < l; i++)
        {
            tableBigEyes.updateHistoryBigEye(listHis[i]);
            tableSmallRoad.updateHistorySmallRoad(listHis[i]);
            tableCockRoad.updateHistorySmallRoad(listHis[i]);
        }

    }
}

