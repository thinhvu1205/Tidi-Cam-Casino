using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DummyItemTag : MonoBehaviour
{
    //    listBg: [cc.SpriteFrame],
    //        listBgThai: [cc.SpriteFrame],
    //        bg: cc.Sprite,
    //        value: cc.Label,
    [SerializeField]
    List<Sprite> listBg, listBgThai;
    [SerializeField]
    Image bg;
    [SerializeField]
    TextMeshProUGUI value;
    public void onShow(Player player, int type, string _value)
    {
        var pos = Vector2.zero;
        switch (player._indexDynamic)
        {
            case 0:
                pos = new Vector2(-95, -100);
                break;
            case 1:
                pos = new Vector2(-95, 150);
                break;
            case 2:
                pos = new Vector2(95, 150);
                break;
            case 3:
                pos = new Vector2(95, -100);
                break;
        }

        bg.transform.localScale = Vector3.zero;
        bg.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack);
        var posText = value.transform.localPosition;
        posText.y = 20;
        value.transform.localPosition = posText;

        //var str = value.toString();
        //value.node.color = str.charAt(0) == '-' ? cc.hexToColor('#FF0000') : cc.hexToColor('#1FFF00')

        value.text = _value + "";
        Color tempColor = value.color;

        tempColor = value.text[0] == '-' ? Color.red : Color.green;
        tempColor.a = 0f;
        value.color = tempColor;

        DOTween.Sequence().AppendInterval(0.4f).AppendCallback(() =>
        {
            value.DOFade(1, 0.4f);
            value.transform.DOLocalMove(new Vector3(0, -20), 0.4f).SetEase(Ease.OutBounce);
        });
        //var langLocal = cc.sys.localStorage.getItem("language_client");
        bg.sprite = Globals.Config.language == "EN" ? this.listBg[type] : listBgThai[type];//langLocal == LANGUAGE_TEXT_CONFIG.LANG_EN ? this.listBgThai[type] : this.listBg[type];
        transform.localPosition = pos;

        DOTween.Sequence().AppendInterval(2).AppendCallback(() =>
        {
            onHide(player);
        });
    }

    public void onHide(Player player)
    {
        var pos = player.playerView.transform.localPosition;
        transform.DOScale(0.1f, 0.5f);
        transform.DOLocalMove(pos, 0.5f);
        DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }
}
