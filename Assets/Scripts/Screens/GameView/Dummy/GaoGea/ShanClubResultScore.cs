using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class ShanClubResultScore : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public Image bg_score;

    [SerializeField]
    public Image bg_bonus;

    [SerializeField]
    TextMeshProUGUI lb_score;

    [SerializeField]
    Image bg_Shan;

    [SerializeField]
    List<SkeletonDataAsset> listAniBork;

    [SerializeField]
    List<Sprite> listImgBouns;
    [SerializeField]
    List<Sprite> listImgShan;

    void Start()
    {

    }

    // Update is called once per frame
    public void setResult(int score, int rate, int numCard)
    {
        if (rate > 1)
        {
            bg_bonus.gameObject.SetActive(true);
            if (rate == 2)
            {
                bg_bonus.sprite = listImgBouns[0];
            }
            else if (rate == 3)
            {
                bg_bonus.sprite = listImgBouns[1];
            }
            else
            {
                bg_bonus.sprite = listImgBouns[2];
            }
        }
        else
        {
            bg_bonus.gameObject.SetActive(false);
        }
        if (numCard == 2)
        {
            if (score == 8)
            {
                bg_Shan.gameObject.SetActive(true);
                bg_Shan.sprite = listImgShan[0];
            }
            else if (score == 9)
            {
                bg_Shan.gameObject.SetActive(true);
                bg_Shan.sprite = listImgShan[1];
            }
            else
            {
                bg_score.gameObject.SetActive(true);
                //lb_score.string = score + " " + require('GameManager').getInstance().getTextConfig('diem');
                lb_score.text = score + " Point";
            }
        }
        else if (numCard == 3)
        {
            bg_score.gameObject.SetActive(true);
            bg_Shan.gameObject.SetActive(false);
            lb_score.text = score + " " + "Point";
            //if (score == 11) lb_score.text = require('GameManager').getInstance().getTextConfig('txt_pok_sanh');
            //if (score == 12) lb_score.text = require('GameManager').getInstance().getTextConfig('txt_pok_tpsanh');
            //if (score == 13) lb_score.text = require('GameManager').getInstance().getTextConfig('txt_pok_3daunguoi');
            //if (score == 14) lb_score.text = require('GameManager').getInstance().getTextConfig('txt_pok_xam');
        }

    }
    public void unuse()
    {
        bg_score.gameObject.SetActive(false);
        bg_Shan.gameObject.SetActive(false);
        bg_bonus.gameObject.SetActive(false);
    }
}
