using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;
using TMPro;
public class DealerInGameView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public SkeletonGraphic ani_dealer;
    public Button btn_tip;
    public TextMeshProUGUI lb_thank;
    public TextMeshProUGUI lb_chip;
    public TMP_FontAsset fontPlus;
    public TMP_FontAsset fontSubtract;
    public GameObject background;
    public GameObject icDealer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void init()
    {
        background.SetActive(false);
    }
    public void show(string content,int chip)
    {
        //content = content.Substring(12, content.Length) + "...";
        var numRand =Random.Range(1, 7);
        var key = "tip_thanks_" + numRand;
        var str = content + ", " +Globals.Config.getTextConfig(key);
        background.SetActive(true);
        background.transform.localScale = Vector2.zero;
        lb_chip.text = Globals.Config.FormatNumber(chip);
        lb_thank.text = str;
        DOTween.Sequence()
            .Append(background.transform.DOScale(Vector2.one, 0.3f)).SetEase(Ease.OutBack)
            .AppendInterval(3.5f)
            .AppendCallback(() =>
            {
                background.SetActive(false);
            });
    }
    public void dispatchCard()
    {
        ani_dealer.Initialize(true);
        ani_dealer.timeScale = 0.8f;
        ani_dealer.AnimationState.SetAnimation(0, "chiabai", true);
        DOTween.Sequence().AppendInterval(1.3f).AppendCallback(() =>
        {
            ani_dealer.timeScale = 1.0f;
            ani_dealer.AnimationState.SetAnimation(0, "normal", true);
        });
    }
    public void onClickTip()
    {
        
        if (UIManager.instance.gameView.thisPlayer.ag> UIManager.instance.gameView.agTable)
        {
            SocketSend.sendTip();
        }
        
    }
}
