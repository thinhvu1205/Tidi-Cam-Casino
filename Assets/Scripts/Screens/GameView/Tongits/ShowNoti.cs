using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using DG.Tweening;

public class ShowNoti : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lbl_content;
    [SerializeField] GameObject Bg_background;

    public async void showContent(string content)
    {
        transform.DOScale(1.2f, 0.6f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            transform.DOScale(1, 0.3f).SetEase(Ease.OutCubic);
        });
        lbl_content.text = content;
        await Task.Delay(2000);
        Destroy(gameObject);
    }
}
