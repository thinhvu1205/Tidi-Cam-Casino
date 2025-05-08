using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxBetShow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Image spIcStatus;

    [SerializeField]
    List<Sprite> listSprite;

    [SerializeField]
    TextMeshProUGUI lbChip;

    public int chip = 0, theFirst = 0;
    public string status = "";



    public void setInfo(string statuss, int indexDynamic, int chipBet = 0)
    {
        chip += chipBet;
        transform.localScale = indexDynamic <= 4 ? Vector2.one : Vector2.one * -1;
        lbChip.transform.localScale = indexDynamic <= 4 ? Vector2.one : Vector2.one * -1;
        spIcStatus.transform.localScale = indexDynamic <= 4 ? Vector2.one : Vector2.one * -1;
        status = statuss;
        if (chip == 0)
        {
            GetComponent<Image>().enabled = false;
            lbChip.text = "";
        }
        else
        {
            if (!GetComponent<Image>().enabled)
            {
                GetComponent<Image>().enabled = true;
            }
            lbChip.text = Globals.Config.FormatMoney2(chip,true);
        }

        switch (status)
        {
            case "Allin":
                spIcStatus.sprite = listSprite[0];
                //require("SoundManager1").instance.dynamicallyPlayMusic(ResDefine.allinAudio);
                break;
            case "Raise":
                spIcStatus.sprite = listSprite[1];
                //require("SoundManager1").instance.dynamicallyPlayMusic(ResDefine.sound_bet);
                break;
            case "Call":
                //require("SoundManager1").instance.dynamicallyPlayMusic(ResDefine.sound_bet);
                if (chip == 0)
                {
                    spIcStatus.sprite = listSprite[3];
                }
                else
                {
                    spIcStatus.sprite = listSprite[2];
                }
                break;
            case "Check":
                GetComponent<Image>().enabled = false;
                spIcStatus.sprite = listSprite[3];
                break;
            case "Fold":
                GetComponent<Image>().enabled = false;
                lbChip.text = "";
                spIcStatus.sprite = listSprite[4];
                break;
            default:
                spIcStatus.sprite = null;
                break;
        }
    }

    // Update is called once per frame
    private void offSpriteAll()
    {
        GetComponent<Image>().enabled = false;
        lbChip.text = "";
    }
}
