using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShowResultScore : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Image bg_score;

    [SerializeField]
    Image bg_bork_img;

    [SerializeField]
    Image bg_bonus;

    [SerializeField]
    Image txt_type_card;

    [SerializeField]
    TextMeshProUGUI lb_score;

    [SerializeField]
    public SkeletonGraphic aniType;

    [SerializeField]
    List<SkeletonDataAsset> listAniBork;

    [SerializeField]
    List<SkeletonGraphic> listAniWinLost;

    [SerializeField]
    List<Sprite> listImgBork;

    [SerializeField]
    List<Sprite> listTypeCard;

    [SerializeField]
    List<Sprite> listImgBouns;

    [SerializeField]
    List<Sprite> listImgWinlose;


    private void Awake()
    {

    }
    private void OnEnable()
    {
        aniType.gameObject.SetActive(false);
    }

    void Start()
    {
        //bg_score.gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void changeSize()
    {
        float sizeWidth = lb_score.preferredWidth;
        bg_score.rectTransform.sizeDelta = new Vector2(sizeWidth + 70, 35);
    }
    public void showEffectWinLose(int isWin, bool isLoop = true)
    {
        GameObject actionNode;
        TweenCallback funcEnd;
        if (isWin == 0)
        {
            aniType.gameObject.SetActive(true);
            funcEnd = () =>
           {
               aniType.gameObject.SetActive(false);
           };
            actionNode = aniType.gameObject;
        }
        else if (isWin == 1)
        {
            listAniWinLost[0].gameObject.SetActive(true);

            funcEnd = () =>
            {
                listAniWinLost[0].gameObject.SetActive(false);
            };
            actionNode = listAniWinLost[0].gameObject;
        }
        else
        {
            listAniWinLost[1].gameObject.SetActive(true);
            if (Globals.Config.curGameId == (int)Globals.GAMEID.KEANG)
                listAniWinLost[1].transform.DOLocalMoveY(25, 0);
            funcEnd = () =>
            {
                listAniWinLost[1].gameObject.SetActive(false);
            };
            actionNode = listAniWinLost[1].gameObject;
        }

        //actionNode.stopAllActions();
        int time = 3;
        if (Globals.Config.curGameId == (int)Globals.GAMEID.KEANG) time = 2;
        DOTween.Sequence()
            .AppendInterval(time)
            .AppendCallback(funcEnd);
    }
    public void setResult(int score, int rate)
    {
        try {

            if (score < 0)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            if (aniType != null && rate < 5)
            {
                aniType.gameObject.SetActive(true);
                txt_type_card.sprite = listTypeCard[rate+5];// listTypeCard[cc.sys.localStorage.getItem("language_client") == LANGUAGE_TEXT_CONFIG.LANG_EN ? rate : rate + 5];
                bg_score.gameObject.SetActive(false);
                txt_type_card.SetNativeSize();
            }
            else
            {
                bg_score.gameObject.SetActive(true);
            }
            if (lb_score != null) lb_score.text = score + " " + Globals.Config.getTextConfig("diem");// require('GameManager').getInstance().getTextConfig('diem');
            changeSize();
        }
        catch(System.SystemException e)
        {
            Globals.Logging.Log("errr--set result:" + e);
        }
    }
    private void resultLose(bool isLose)
    {
        if (isLose)
        {
            bg_score.sprite = listImgWinlose[1];
        }
    }

    public void unuse()
    {

        bg_bork_img.gameObject.SetActive(false);
        bg_bonus.gameObject.SetActive(false);
        bg_score.sprite = listImgWinlose[0];
    }
}
