using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Linq;
using Globals;

public class HistoryDragonTiger : BaseView
{
    [SerializeField] private Transform parentMiniHisLayer1;
    [SerializeField] private GameObject parentMiniHisLayer2;
    [SerializeField] private Transform layer2;

    [SerializeField] public TextMeshProUGUI percentLineYellow;
    [SerializeField] public TextMeshProUGUI percentLineBlue;
    [SerializeField] public TextMeshProUGUI textCountTie;
    [SerializeField] public TextMeshProUGUI textCountDragon;
    [SerializeField] public TextMeshProUGUI textCountTiger;
    [SerializeField] public TextMeshProUGUI textCountDragonBig;
    [SerializeField] public TextMeshProUGUI textCountTigerBig;
    [SerializeField] public TextMeshProUGUI textCountDragonSmall;
    [SerializeField] public TextMeshProUGUI textCountTigerSmall;
    [SerializeField] public Image lineDragon;
    [SerializeField] public Image lineTiger;

    [SerializeField] public List<GameObject> listDots = new List<GameObject>();

    [HideInInspector] public int cntTie = 0, cntDr = 0, cntTg = 0, cntDrBig = 0, cntTgBig = 0, cntDrSmall = 0, cntTgSmall = 0;

    private List<List<int>> resultHis = new List<List<int>>();

    private void Update()
    {
        transform.SetAsLastSibling();
    }

    public void handleResultHisLayer1(List<int> listHis)
    {
        //listHis.Reverse();
        //listHis = new List<int> { 1, 1, 3, 2, 1, 1, 1, 2, 3, 2, 1, 1, 1, 1, 3, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1 };

        int count = listHis.Count;
        int startIndex = (count <= 20) ? 0 : count - 20;

        UIManager.instance.destroyAllChildren(parentMiniHisLayer1);

        for (int i = startIndex; i < count; i++)
        {
            GameObject itemHIs = Instantiate(listDots[listHis[i] - 1], parentMiniHisLayer1);

            itemHIs.SetActive(true);
        }
    }

    public void handleResultHisLayer2(List<int> listHis)
    {
        //listHis = new List<int> { 1, 1, 3, 2, 1, 1, 1, 2, 3, 2, 1, 1, 1, 1, 3, 1, 2, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 2, 2, 2, 2, 1, 1, 1, 1, 2, 3, 1, 3, 3, 1, 1, 1 };
        List<int> currentList = new List<int> { listHis[0] };
        resultHis.Add(currentList);

        for (int i = 1; i < listHis.Count; i++)
        {
            if (listHis[i] == listHis[i - 1] && currentList.Count < 5)
            {
                currentList.Add(listHis[i]);
            }
            else
            {
                currentList = new List<int> { listHis[i] };
                resultHis.Add(currentList);
            }
        }
        //resultHis.Reverse();
        Vector2 currentPos = new Vector2(-418, 91);

        UIManager.instance.destroyAllChildren(layer2);
        for (int i = (resultHis.Count < 20 ? 0 : resultHis.Count - 20); i < resultHis.Count; i++)
        {
            GameObject itemDotCol = Instantiate(parentMiniHisLayer2, layer2);
            itemDotCol.transform.localPosition = new Vector2(currentPos.x, currentPos.y);
            currentPos.x = currentPos.x + 46f;
            itemDotCol.SetActive(true);

            for (int j = 0; j < resultHis[i].Count; j++)
            {
                GameObject itemHIs = Instantiate(listDots[resultHis[i][j] - 1], itemDotCol.transform);
                itemHIs.SetActive(true);

            }
        }
        resultHis.Clear();
    }

    public void UpdateFillRange(int numDragon, int numTiger)
    {
        if (numDragon == 0 && numTiger == 0)
        {
            return;
        }
        float fillRangeDragon = (float)numDragon / (numDragon + numTiger) * 100;
        float fillRangeTiger = (float)numTiger / (numDragon + numTiger) * 100;

        lineDragon.fillAmount = Mathf.Round(fillRangeDragon) / 100;
        lineTiger.fillAmount = Mathf.Round(fillRangeTiger) / 100;

        percentLineBlue.text = Mathf.Round(fillRangeDragon).ToString() + "%";
        percentLineYellow.text = Mathf.Round(fillRangeTiger).ToString() + "%";

    }
}
