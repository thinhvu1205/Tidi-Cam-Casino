using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BetButtonRoulette : MonoBehaviour
{
    [SerializeField] public Button button;
    public Image imageBorder;
    public TextMeshProUGUI textbet;
    public int id;
    
    void Start(){
        button.onClick.AddListener(ClickButtonSelecBet);
        RouLetteView.instance.SelectButtonBet(0);  
    }
    private void ClickButtonSelecBet(){
        RouLetteView.instance.SelectButtonBet(id);   
    }
}
