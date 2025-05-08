using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class ItemFriend : MonoBehaviour
{
    public JObject data = new JObject();
    [HideInInspector]
    public int idFriend = 0;
    [HideInInspector]
    public bool isSelect = false;
    [SerializeField]
    public GameObject ic_nofity;

    [SerializeField]
    public TextMeshProUGUI lbNotiNum;

    [SerializeField]
    public Image bkg;

    [SerializeField]
    public TextMeshProUGUI lbName;

    [SerializeField]
    public Avatar avtCtrl;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
   public void setInfo()
    {
        if ((int)data["count"] > 0)
        {
            ic_nofity.gameObject.SetActive(true);
            lbNotiNum.text = ((int)data["count"]).ToString() ;
        }
        else
        {
            ic_nofity.gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        resetData();
    }
    private void resetData()
    {
        idFriend = 0;
        isSelect = false;
        data = new JObject();
        lbName.color = Color.gray;
        avtCtrl.setDark(true);
        bkg.enabled = false;
    }
    public void setSelect(bool _isSelect)
    {
        gameObject.SetActive(true);
        Globals.Logging.Log("setSelect:"+_isSelect);
        isSelect = _isSelect;
        lbName.color = isSelect ? Color.white : Color.gray;
        avtCtrl.setDark(!isSelect);
        bkg.enabled = isSelect;
    }

    //public void setVisibleBkg(bool isVisible)
    //{
    //    bkg.enabled = isVisible;
    //}
}
