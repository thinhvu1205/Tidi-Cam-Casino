using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ChipBetRouLette : MonoBehaviour
{
    [SerializeField] private List<Sprite> listSpriteChipBetRouLette;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textBet;
    public bool isDealed = false;

    public void Init(int id, long value)
    {
        image.sprite = listSpriteChipBetRouLette[id];
        image.SetNativeSize();
        image.transform.localPosition = new Vector3(0, 32, 0);
        image.transform.DOLocalMove(Vector3.zero, 0.25f);
        image.transform.DOScale(Vector3.one * 0.5f, 0.25f);
        textBet.text = FormatNumber(value);
    }

    private string FormatNumber(long value)
    {
        if (value >= 1000000)
            return (value / 1000000f).ToString("0.#") + "M";
        if (value >= 1000)
            return (value / 1000f).ToString("0.#") + "K";
        return value.ToString();
    }

}
