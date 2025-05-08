using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Globals;

public class SiXiangBuyPealsPopup : BaseView
{
    // Start is called before the first frame update
    [SerializeField]
    Image imgGem;
    [SerializeField]
    Button btnConfirm;
    [SerializeField]
    TextMeshProUGUI lbInfo;
    [SerializeField]
    List<Sprite> listSprGem = new List<Sprite>();
    public int betCurrent = 0;
    public int typeGameBonus = 0;
    public int price = 0;
    private long playerBalance = 0;
    public void onClickConfirm()
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        if (playerBalance >= price)
        {
            SocketSend.sendBuyBonusGame(Globals.ACTION_SLOT_SIXIANG.buyBonusGame, typeGameBonus, betCurrent);
            SiXiangView.Instance.agPlayer -= price;
            SiXiangView.Instance.setAGPlayer();
            onClickClose(true);
        }
        else
        {
            string textShow = Globals.Config.getTextConfig("txt_not_enough_money_gl");
            UIManager.instance.showDialog(textShow, Globals.Config.getTextConfig("shop"), () =>
             {
                 UIManager.instance.openShop();
             }, Globals.Config.getTextConfig("label_cancel"));
            onClickClose(true);
        }
    }
    public void setInfo(int indexGem, int pricePearl, int bet, long agPlayer)
    {
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
        playerBalance = agPlayer;
        int indexSprite = 0;
        price = pricePearl;
        switch (indexGem)
        {
            case 2:
                indexSprite = 0; break;
            case 4:
                indexSprite = 1; break;
            case 5:
                indexSprite = 2; break;
            case 3:
                indexSprite = 3; break;
        }
        //btnConfirm.interactable = agPlayer >= price;
        //btnConfirm.GetComponent<Image>().color = agPlayer >= price ? Color.white : Color.gray;
        typeGameBonus = indexGem;
        betCurrent = bet;
        imgGem.sprite = listSprGem[indexSprite];
        Debug.Log("price=" + price);
        lbInfo.text = "Pay " + Config.FormatNumber(price) + " chips to receive this gem!";//Globals.Config.formatStr(Globals.Config.getTextConfig("text_sixiang_buy_gem"), Globals.Config.FormatNumber(price));
    }
    public void onClickClosePopup(bool isDestroy = true)
    {
        onClickClose(isDestroy);
        SoundManager.instance.playEffectFromPath(SOUND_SLOT_BASE.CLICK);
    }
}
