using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Globals;

public class CardShan : MonoBehaviour
{
    public List<Image> upperNode;
    public List<Image> lowerNode;
    public GameObject itemSuit;
    public Image bigSuit;
    public GameObject suiteParent;

    public void SetInfo(int suit, int value, string suitName, Sprite frameSuit, Sprite frameValue)
    {
        if (value > 13) value -= 13;
        var objNode = SetElementPos(value);
        itemSuit.GetComponent<Image>().sprite = frameSuit;
        upperNode[0].sprite = frameValue;
        upperNode[1].sprite = frameSuit;
        lowerNode[0].sprite = frameValue;
        lowerNode[1].sprite = frameSuit;
        Color color = (suit == 3 || suit == 4) ? Color.red : Color.black;
        upperNode[0].color = color;
        lowerNode[0].color = color;
        foreach (Transform child in suiteParent.transform)
        {
            Destroy(child.gameObject);
        }
        if (value <= 10)
        {
            bigSuit.gameObject.SetActive(false);
            suiteParent.SetActive(true);
            for (int i = 0; i < value; i++)
            {
                var tiny = Instantiate(itemSuit);

                tiny.SetActive(true);
                tiny.transform.SetParent(suiteParent.transform, false);
                tiny.transform.localPosition = objNode.pos[i];
                tiny.transform.localRotation = Quaternion.Euler(0, 0, objNode.rotate[i]);
            }
        }
        else
        {
            bigSuit.gameObject.SetActive(true);
            //bigSuit.sprite = frameSheet.GetSprite(value + suitName);
            UIManager.instance.cardAtlas.GetSprite(string.Format("card_{0}_{1}", value, suitName));
            suiteParent.SetActive(false);
        }
    }

    public void showCorner(bool isShow = false, float time = 0.6f)
    {
        Debug.Log($"!>>>>> show corner card Shan {isShow}");
        var upperNodeParent = upperNode[0].gameObject;
        var lowerNodeParent = lowerNode[0].gameObject;
        upperNodeParent.transform.parent.gameObject.SetActive(isShow);
        lowerNodeParent.transform.parent.gameObject.SetActive(isShow);
        upperNodeParent.GetComponent<CanvasGroup>().alpha = 1;
        lowerNodeParent.GetComponent<CanvasGroup>().alpha = 1;
        if (isShow)
        {
            upperNodeParent.GetComponent<CanvasGroup>().alpha = 0;
            lowerNodeParent.GetComponent<CanvasGroup>().alpha = 0;
            upperNodeParent.GetComponent<CanvasGroup>().DOFade(1, time).SetEase(Ease.OutCubic);
            lowerNodeParent.GetComponent<CanvasGroup>().DOFade(1, time).SetEase(Ease.OutCubic);
        }
    }

    public (Vector2[] pos, float[] rotate) SetElementPos(int value)
    {
        var result = (pos: new Vector2[0], rotate: new float[0]);
        switch (value)
        {
            case 1:
                result.pos = new[] { new Vector2(0, 0) };
                result.rotate = new[] { 0f };
                break;
            case 2:
                result.pos = new[] { new Vector2(0, 70), new Vector2(0, -70) };
                result.rotate = new[] { 0f, 180f };
                break;
            case 3:
                result.pos = new[] { new Vector2(0, 70), new Vector2(0, 0), new Vector2(0, -70) };
                result.rotate = new[] { 0f, 180f, 180f };
                break;
            case 4:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 180f, 180f };
                break;
            case 5:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(0, 0), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 180f, 180f };
                break;
            case 6:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(-30, 0), new Vector2(30, 0), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 0f, 180f, 180f };
                break;
            case 7:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(0, 35), new Vector2(-30, 0), new Vector2(30, 0), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 0f, 0f, 180f, 180f };
                break;
            case 8:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(0, 35), new Vector2(-30, 0), new Vector2(30, 0), new Vector2(0, -35), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 0f, 0f, 180f, 180f, 180f };
                break;
            case 9:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(-30, 35), new Vector2(30, 35), new Vector2(0, 0), new Vector2(-30, -35), new Vector2(30, -35), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 0f, 0f, 180f, 180f, 180f, 180f };
                break;
            case 10:
                result.pos = new[] { new Vector2(-30, 70), new Vector2(30, 70), new Vector2(0, 45), new Vector2(-30, 20), new Vector2(30, 20), new Vector2(-30, -20), new Vector2(30, -20), new Vector2(0, -45), new Vector2(-30, -70), new Vector2(30, -70) };
                result.rotate = new[] { 0f, 0f, 0f, 0f, 0f, 180f, 180f, 180f, 180f, 180f };
                break;
        }
        return result;
    }
}
