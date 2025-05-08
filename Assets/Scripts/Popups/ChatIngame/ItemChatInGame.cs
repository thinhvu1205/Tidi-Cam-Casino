using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using System.Linq;
using System;
using DG.Tweening;

public class ItemChatInGame : MonoBehaviour
{
    [SerializeField]
    Image bkg;

    [SerializeField]
    TextMeshProUGUI txtMsg;

    [SerializeField]
    SkeletonGraphic anim;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setMsg(string msg, string type, PlayerView playerView)
    {
        var pos = playerView.transform.localPosition;
        var sizePl = playerView.GetComponent<RectTransform>().sizeDelta.x;
        var scale = playerView.transform.localScale;

        if (!msg.Equals(""))
        {
            bkg.gameObject.SetActive(true);
            txtMsg.gameObject.SetActive(true);
            anim.gameObject.SetActive(false);

            txtMsg.text = msg;
            var sizeBg = bkg.rectTransform.sizeDelta;

            if (msg.Length > 35)
            {
                sizeBg.x = 300;
            }
            else
            {
                sizeBg.x = 200;
            }
            bkg.rectTransform.sizeDelta = sizeBg;
            bkg.transform.localScale = new Vector3(pos.x < 0 ? 1 : -1, 1, 1);
            txtMsg.transform.localScale = new Vector3(pos.x < 0 ? 1 : -1, 1, 1);
            pos.x += sizePl * .5f * scale.x + sizeBg.x * .5f + 10 + (pos.x < 0 ? 0 : -300);
            pos.y += 50;
        }
        else if (type != "")
        {
            SkeletonDataAsset skeEmo = null;
            if (type.Contains("*e"))
            {
                var idAnimation = string.Join("", type.ToCharArray().Where(Char.IsDigit));
                //Globals.Logging.Log("-=idAnimation   " + idAnimation);
                skeEmo = Resources.Load<SkeletonDataAsset>("GameView/Emo/emoticon/e" + idAnimation + "/skeleton_SkeletonData");
            }

            if (skeEmo != null)
            {
                bkg.gameObject.SetActive(false);
                txtMsg.gameObject.SetActive(false);
                anim.gameObject.SetActive(true);
                anim.skeletonDataAsset = skeEmo;
                anim.startingAnimation = "animation";
                anim.startingLoop = true;
                anim.Initialize(true);
            }
        }

        transform.localPosition = pos;
        Globals.Logging.Log("Pos chat ==" + pos.x + "," + pos.y);
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        DOTween.Sequence().Append(canvasGroup.DOFade(1, 0.5f)).AppendInterval(2).AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }
}
