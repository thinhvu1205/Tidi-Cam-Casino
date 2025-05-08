using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VipContainer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> listSpriteStar;
    public List<Image> listSprVip;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setVip(int vip)
    {
        int countStar = (int)Mathf.Floor(vip / 2);
        for (int i = 0; i < listSprVip.Count; i++)
        {
            Image sprVip = listSprVip[i];
            if (vip == 0)
            {
                sprVip.sprite = listSpriteStar[0];
            }
            else
            {
                if (i < countStar)
                {
                    sprVip.sprite = listSpriteStar[2];
                }
                else
                {
                    if (vip % 2 == 0)
                    {
                        sprVip.sprite = listSpriteStar[0];
                    }
                    else
                    {
                        if (i == countStar)
                        {
                            sprVip.sprite = listSpriteStar[1];
                        }
                        else
                        {
                            sprVip.sprite = listSpriteStar[0];
                        }
                    }

                }


            }
            sprVip.SetNativeSize();
        }


    }
}
