using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class Lucky9ScoreResult : MonoBehaviour
{
    public Image bg_score;
    public TextMeshProUGUI lb_score;
    public TextMeshProUGUI lb_under;
    public List<Sprite> listImgBg = new List<Sprite>();
    private bool isShow = false;

    public void onShow(int score, int rate = 0)
    {
        isShow = true;
        if (rate > 1)
        {
            bg_score.sprite = listImgBg[1];
            lb_under.color = HexToColor("#7A0A06");
            lb_score.color = HexToColor("#7A0A06");
        }
        else
        {
            bg_score.sprite = listImgBg[0];
            lb_under.color = HexToColor("#182D48");
            lb_score.color = HexToColor("#182D48");
        }
        lb_score.text = score.ToString();
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        StopAllCoroutines();
        transform.DOScale(1, 1).SetEase(Ease.OutBack);
    }

    public void onHide()
    {
        if (!isShow) return;
        isShow = false;
        StopAllCoroutines();
        transform.DOScale(0, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return Color.black;
    }
}
