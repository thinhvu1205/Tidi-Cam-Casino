using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;


public class dominoDiceRemain : MonoBehaviour
{
    [SerializeField] public List<GameObject> listText = new List<GameObject>();
    
    public void updateDiceRemain(JObject diceRemain = null)
    {
        if (diceRemain == null)
        {
            listText[0].GetComponent<TextMeshProUGUI>().text = "7";
            listText[1].GetComponent<TextMeshProUGUI>().text = "7";
            listText[2].GetComponent<TextMeshProUGUI>().text = "7";
            listText[3].GetComponent<TextMeshProUGUI>().text = "7";
            listText[4].GetComponent<TextMeshProUGUI>().text = "7";
            listText[5].GetComponent<TextMeshProUGUI>().text = "7";
            listText[6].GetComponent<TextMeshProUGUI>().text = "7";
        }
        else
        {
            int num;
            num = (int)diceRemain["0"];
            listText[0].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["1"];
            listText[1].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["2"];
            listText[2].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["3"];
            listText[3].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["4"];
            listText[4].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["5"];
            listText[5].GetComponent<TextMeshProUGUI>().text = num + "";
            num = (int)diceRemain["6"];
            listText[6].GetComponent<TextMeshProUGUI>().text = num + "";
        }
    }
}
