using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;

public class PlayerViewBorkKDeng : PlayerView
{

    [SerializeField]
    public GameObject cardContainer;
    [SerializeField]
    public GameObject bgScore;
    [SerializeField]
    public SkeletonGraphic animQiu;
    [SerializeField]
    public Image bg_bonus;
    [SerializeField]
    public TextMeshProUGUI lbScore;

    [SerializeField]
    public Sprite bgScore8;

    [SerializeField]
    public Sprite bgScore9;

    [SerializeField]
    public Sprite bgScoreNormal;

    [SerializeField]
    public List<Sprite> listImgBouns = new List<Sprite>();

    [HideInInspector]
    public List<ChipBet> listChip = new List<ChipBet>();
    public bool isShowCard = false;
    public int rate = 1;
    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        cardContainer = transform.Find("CardContainer").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void resetUI()
    {
        listChip.Clear();
        cards.Clear();
        isShowCard = false;
        animResult.gameObject.SetActive(false);
        rate = 1;
    }
    public void showScoreCard()
    {
        //    int cardScore = 0;
        int numCard = cards.Count;
        //score = 9;
        if (rate > 1)
        {
            bg_bonus.gameObject.SetActive(true);
            //Globals.Logging.Log("rate==" + rate + "--name=" + txtName.text);
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
        RectTransform rtBgScore = bgScore.transform.GetComponent<RectTransform>();
        if (numCard == 2)
        {
            if (score == 9)
            {
                bgScore.gameObject.SetActive(true);
                lbScore.gameObject.SetActive(false);
                //bgScore.GetComponent<Image>().sprite = bgScore9;
                //bgScore.GetComponent<Image>().SetNativeSize();
                bgScore.GetComponent<Image>().enabled = false;
                animQiu.gameObject.SetActive(true);
                lbScore.gameObject.SetActive(false);
                animQiu.AnimationState.SetAnimation(0, "animation", false);
                animQiu.Initialize(true);
            }
            else
            {
                bgScore.GetComponent<Image>().enabled = true;
                animQiu.gameObject.SetActive(false);
                bgScore.gameObject.SetActive(true);
                lbScore.gameObject.SetActive(true);
                bgScore.GetComponent<Image>().sprite = bgScoreNormal;
                lbScore.text = score + " " + Globals.Config.getTextConfig("diem");
              
                rtBgScore.sizeDelta = new Vector2(147, 64);
            }
        }
        else if (numCard == 3)
        {
            animQiu.gameObject.SetActive(false);
            bgScore.gameObject.SetActive(true);
            bgScore.GetComponent<Image>().enabled = true;
            bgScore.GetComponent<Image>().sprite = bgScoreNormal;
            //bg_bonus.gameObject.SetActive(false);
            lbScore.gameObject.SetActive(true);
            // 11: "Face Cards" 12: "Straight" 13: "Straight flush" 14: "Three of a kind" 
            if (score < 11)
            {
                lbScore.text = score + " " + Globals.Config.getTextConfig("diem");
                rtBgScore.sizeDelta = new Vector2(147, 64);
            }
            else
            {
                if (score == 11) lbScore.text = Globals.Config.getTextConfig("txt_pok_3daunguoi");
                if (score == 12) lbScore.text = Globals.Config.getTextConfig("txt_pok_sanh");
                if (score == 13) lbScore.text = Globals.Config.getTextConfig("txt_pok_tpsanh");
                if (score == 14) lbScore.text = Globals.Config.getTextConfig("txt_pok_xam");
                rtBgScore.sizeDelta = new Vector2(210, 64);
            }

        }
        float posX = 0;
        float firstPosX = cards[cards.Count - 1].transform.localPosition.x;
        float lastPosX = cards[0].transform.localPosition.x;
        posX = (lastPosX - firstPosX) / 2;
        bgScore.transform.localPosition = new Vector2(firstPosX + posX, cards[1].transform.position.y - 35);
        bgScore.transform.SetSiblingIndex(cardContainer.transform.childCount - 1);


    }


}
