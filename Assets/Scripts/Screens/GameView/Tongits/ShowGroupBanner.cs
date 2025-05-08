using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGroupBanner : MonoBehaviour
{
    public Image true_banner;
    public Image false_banner;
    public Image joker_banner;
    public GameObject specified;
    public float Stret, PosX;
    [HideInInspector] public bool IsValidWithoutJoker, IsJokerGroup, IsSpecified;
    private List<Card> CardCs = new();
    void Awake()
    {
        HideAllBanners();
    }
    public void RefreshUI(bool hasSecretMelds, bool hasOtherValidGroup, bool hasDroppedGroup, bool isLastGroup)
    {
        if (GetListCardCs().Count < 3)
        {
            HideAllBanners();
            return;
        }
        if (IsSpecified)
        {
            specified.transform.localPosition = new Vector2(PosX, -345);
            SetTrueGroup();
            specified.SetActive(true);
        }
        else
        {
            specified.SetActive(false);
            if (IsValidWithoutJoker) SetTrueGroup();
            else
            {
                if (IsJokerGroup)
                {
                    if (hasSecretMelds || hasDroppedGroup)
                    {
                        SetTrueGroup();
                    }
                    else
                    {
                        if (hasOtherValidGroup) SetJokerGroup();
                        else SetFalseGroup();
                    }
                }
                else
                {
                    if (isLastGroup) HideAllBanners();
                    else SetFalseGroup();
                }
            }
        }
    }
    public void SetInfo(List<Card> cardCs, float stret, float posX, bool isValidWithoutjoker, bool isJokerGroup, bool isSpecified)
    {
        CardCs = new();
        CardCs.AddRange(cardCs);
        Stret = stret;
        PosX = posX;
        IsValidWithoutJoker = isValidWithoutjoker;
        IsJokerGroup = isJokerGroup;
        IsSpecified = isSpecified;
    }
    public List<Card> GetListCardCs() { return CardCs; }
    void HideAllBanners()
    {
        true_banner.gameObject.SetActive(false);
        false_banner.gameObject.SetActive(false);
        joker_banner.gameObject.SetActive(false);
    }
    void SetTrueGroup()
    {
        true_banner.gameObject.SetActive(true);
        false_banner.gameObject.SetActive(false);
        joker_banner.gameObject.SetActive(false);
        true_banner.rectTransform.sizeDelta = new Vector2(Stret, true_banner.rectTransform.sizeDelta.y);
        true_banner.transform.localPosition = new Vector2(PosX, -340);
    }

    void SetFalseGroup()
    {
        Debug.Log(" BANNER === false");
        false_banner.gameObject.SetActive(true);
        true_banner.gameObject.SetActive(false);
        joker_banner.gameObject.SetActive(false);
        false_banner.rectTransform.sizeDelta = new Vector2(Stret, false_banner.rectTransform.sizeDelta.y);
        false_banner.transform.localPosition = new Vector2(PosX, -340);
    }

    void SetJokerGroup()
    {
        Debug.Log(" BANNER === joker");
        false_banner.gameObject.SetActive(false);
        true_banner.gameObject.SetActive(false);
        joker_banner.gameObject.SetActive(true);
        joker_banner.rectTransform.sizeDelta = new Vector2(Stret, joker_banner.rectTransform.sizeDelta.y);
        joker_banner.transform.localPosition = new Vector2(PosX, -340);
    }

    public void MoveBanner()
    {
        false_banner.transform.localPosition = new Vector2(PosX, -340);
        true_banner.transform.localPosition = new Vector2(PosX, -340);
    }

    internal void SetInfo(object bannerInfo)
    {
        throw new NotImplementedException();
    }
}
