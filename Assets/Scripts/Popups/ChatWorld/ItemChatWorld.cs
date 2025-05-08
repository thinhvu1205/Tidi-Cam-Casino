using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Threading;
using DG.Tweening;

public class ItemChatWorld : MonoBehaviour
{
    [SerializeField]
    GameObject messLeft;

    [SerializeField]
    GameObject messRight;

    [SerializeField]
    Image bgMessLeft;

    [SerializeField]
    Image bgMessRight;

    [SerializeField]
    TextMeshProUGUI lbMessLeft, lbMessRight, lbNameLeft, lbNameRight;

    [SerializeField]
    Avatar avatarLeft;

    [SerializeField]
    Avatar avatarRight;
    [SerializeField]
    RectTransform lbRightRt, lbLeftRt, itemRt;

    public JObject dataChat;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setInfoMess(JObject data)
    {
        dataChat = data;

        if ((int)data["ID"] == Globals.User.userMain.Userid)
        {
            setInfoMessRight(data);

        }
        else
        {
            setInfoMessLeft(data);
        }
    }
    void setInfoMessLeft(JObject data)
    {
        messLeft.SetActive(true);
        messRight.SetActive(false);
        int avatar = (int)data["Avatar"];
        Globals.Logging.Log("setInfoMessLeft avatar " + avatar);
        if (avatar == 0 && (long)data["FaceID"] == 0)
        {
            avatar = 1;
        }
        lbNameLeft.text = $"<color=yellow>[V{data["Vip"]}]</color>{(string)data["Name"]}";
        lbMessLeft.text = (string)data["Data"];
        avatarLeft.image.sprite = ChatPrivateView.instance._spriteAvatarSelect;

        float sizeLbWitdh = lbMessLeft.preferredWidth;
        float sizeLbHeight = lbMessLeft.preferredHeight;


        DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
        {
            bgMessLeft.rectTransform.sizeDelta = new Vector2(bgMessLeft.rectTransform.rect.width, lbMessLeft.preferredHeight);
            lbLeftRt.sizeDelta = new Vector2(lbMessLeft.preferredWidth, lbLeftRt.rect.height);
            if (lbMessLeft.preferredWidth > 350)
            {
                lbLeftRt.sizeDelta = new Vector2(350, lbLeftRt.rect.height);
            }

        })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                itemRt.sizeDelta = new Vector2(itemRt.sizeDelta.x, bgMessLeft.rectTransform.sizeDelta.y + 40);
            });
    }
    void setInfoMessRight(JObject data)
    {
        messLeft.SetActive(false);
        messRight.SetActive(true);
        int avatar = Globals.User.userMain.Avatar;
        lbNameRight.text = $"<color=yellow>[V{data["Vip"]}]</color>{(string)data["Name"]}";
        lbMessRight.text = (string)data["Data"];
        Debug.Log("setInfoMessRight:" + (int)data["Avatar"]);
        Debug.Log("-=-=avatar " + avatar);
        Debug.Log("-=-=FaceID " + Globals.User.FacebookID);
        avatarRight.loadAvatar(avatar, Globals.User.userMain.Username, Globals.User.FacebookID);

        float sizeLbWitdh = lbMessRight.preferredWidth;
        float sizeLbHeight = lbMessRight.preferredHeight;

        DOTween.Sequence().AppendInterval(0.1f).AppendCallback(() =>
        {
            lbRightRt.sizeDelta = new Vector2(lbMessRight.preferredWidth, lbRightRt.rect.height);
            Debug.Log("lbMessRight.preferredWidth:" + lbMessRight.preferredWidth);
            if (lbMessRight.preferredWidth > 350)
            {
                lbRightRt.sizeDelta = new Vector2(350, lbRightRt.rect.height);
            }
            else
            {
                //lbMessRight.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                itemRt.sizeDelta = new Vector2(itemRt.sizeDelta.x, bgMessRight.rectTransform.sizeDelta.y + 40);
            });

    }

}
