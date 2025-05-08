using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class LevelInfo : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI txtLevel, txtMoney;

    [SerializeField]
    Image imgIcon;
    [SerializeField]
    List<Sprite> listIcon = new List<Sprite>();

    [SerializeField]
    GameObject objActive;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInfo(int level, int money, bool isActive)
    {
        txtLevel.text = "Lv." + level;
        txtMoney.text = Globals.Config.FormatMoney(money);
        imgIcon.sprite = listIcon[level - 2];
        objActive.SetActive(isActive);
    }
}
