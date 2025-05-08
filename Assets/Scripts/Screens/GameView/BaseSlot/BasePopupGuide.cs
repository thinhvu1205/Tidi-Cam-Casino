using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BasePopupGuide : BaseView
{
    // Start is called before the first frame update
    [SerializeField]
    ScrollRect scrView;

    // Update is called once per frame
    int currentPage = 1;
    void Update()
    {
        
    }
    public void onClickPrevious()
    {
        RectTransform rectContent = scrView.content.GetComponent<RectTransform>();
        if (currentPage == 1)
        {
            scrView.normalizedPosition = new Vector2(1,0);
            currentPage = scrView.content.childCount;
        }
        else
        {
            currentPage--;
            //rect.DOLocalMoveY(backPos.y, SPEED_BACKSPIN).SetEase(Ease.InSine));
            float previosPos = scrView.content.localPosition.x + scrView.content.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
            rectContent.DOLocalMoveX(previosPos, 0.3f);
            //scrView.horizontalNormalizedPosition = scrView.horizontalNormalizedPosition - 0.3f;

        }
        Globals.Logging.Log(" scrView.normalizedPosition:" + scrView.normalizedPosition);

    }
    public void onClickNext()
    {
        RectTransform rectContent = scrView.content.GetComponent<RectTransform>();
        if (currentPage == scrView.content.childCount)
        {
            scrView.normalizedPosition = new Vector2(0.0f, 0.0f);
            //scrView.horizontalNormalizedPosition = 0;
            currentPage = 1;
        }
        else
        {
            currentPage++;
            float nextPos = scrView.content.localPosition.x - scrView.content.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
            rectContent.DOLocalMoveX(nextPos, 0.3f);
        }
        Globals.Logging.Log(" scrView.normalizedPosition:" + scrView.normalizedPosition);
    }
}
