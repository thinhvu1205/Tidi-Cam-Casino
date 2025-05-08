using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
public class KeangButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Button btnKeang;
    [SerializeField]
    Button btnDiscard;
    [SerializeField]
    Button btnMatch;
    [SerializeField]
    Button btnDraw;
    [SerializeField]
    List<Button> listBtn;
    KeangView KeangController;
    void Start()
    {
        
    }

    public void setController(KeangView controller)
    {
        KeangController = controller;
    }
    // Update is called once per frame
    public void onClickAction(string type)
    {
        switch (type)
        {
            case "DISCARD":
                {
                    onClickDrop();
                    break;
                }
            case "DRAW":
                {
                    onClickDraw();
                    break;
                }
            case "MATCH":
                {
                    onClickMatch();
                    break;
                }
            case "KEANG":
                {
                    onClickKaeng();
                    break;
                }
        }
    }
     public void  onClickKaeng()
    {
        SocketSend.sendKaeng();
    }
    public void onClickDraw ()
    {
        SocketSend.sendKaengBc();
    }
    public void onClickDrop()
    {
        List<Card> selectedCard = KeangController.selectedCards;
        JArray arrayId = KeangController.getArrIdsCard(selectedCard);
        SocketSend.sendKaengDc(arrayId);
    }
    public void onClickMatch()
    {
        List<Card> selectedCard = KeangController.selectedCards;
        JArray arrayId = KeangController.getArrIdsCard(selectedCard);
        SocketSend.sendKaengChop(arrayId);
    }
    public void showButton( List<int> array)
    {
        // show button by index, otherwise hide it

        foreach(Button button in listBtn)
        {
            button.gameObject.SetActive(false);
        }
        foreach(int element in array)
        {
            for (int i = 0; i < 4; i++)
            {
                if (element == i)
                {
                    listBtn[i].gameObject.SetActive(true);
                }
            }
        }
    }
    public void allowPush(int index,bool isAllow)
    {
        listBtn[index].interactable = isAllow;
    }
}
