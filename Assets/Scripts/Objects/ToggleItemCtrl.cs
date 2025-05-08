using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class ToggleItemCtrl : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    public TextMeshProUGUI lbTextOff;
    [SerializeField]
    public TextMeshProUGUI lbTextOn;

    [SerializeField]
    public Image background;

    [SerializeField]
    public Image checkmark;

    [SerializeField]
    public Toggle toggle;

    [HideInInspector]
    public JObject data = new JObject();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void onToggleCheck(Toggle toggle)
    {
        bool isCheck = toggle.isOn;
        lbTextOff.gameObject.SetActive(!isCheck);
        lbTextOn.gameObject.SetActive(isCheck);
    }

}
