using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class CountDownTime : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    TextMeshProUGUI lbTime;

    [SerializeField]
    TextMeshProUGUI lbInfo;
    [SerializeField]
    List<TMP_FontAsset> fontType;
    public bool isPlain = false;
    Sequence Intervall;
    void Start()
    {

    }

    // Update is called once per frame
    public void setInfo(int time, int type)
    {
        lbTime.font = fontType[0];
        if (type == 1)
        {
            lbTime.font = fontType[1];
            lbTime.transform.localScale = new Vector2(0.5f, 0.5f);
            lbTime.transform.localPosition = new Vector2(lbTime.transform.localPosition.x, 0);
            lbInfo.gameObject.SetActive(false);
            GetComponent<Image>().enabled = false;
            isPlain = true;
            lbTime.fontSize = 100;
            //lbTime.preferredHeight = 32.0f;
            lbInfo.text = Globals.Config.getTextConfig("shan2_bettime");// require('GameManager').getInstance().getTextConfig('shan2_starttime');
        }
        else if (type == 2)
        {
            lbTime.fontSize = 60;
            lbInfo.text = Globals.Config.getTextConfig("shan2_bettime");
        }
        else
        {
            lbInfo.text = Globals.Config.getTextConfig("shan2_bettime");// require('GameManager').getInstance().getTextConfig('shan2_bettime');
        }
        if (time > -1)
        {
            if (isPlain)
            {
                showPlainNumberEffect(time);
            }
            else
            {
                lbTime.text = time.ToString();
            }
        }
        Sequence timeVal = DOTween.Sequence();
        timeVal.AppendInterval(1.0f).AppendCallback(() =>
        {
            time--;

            if (time <= 0)
            {
                //node.stopAllActions();
                Destroy(gameObject);
                return;
            }

            if (Globals.Config.curGameId == (int)Globals.GAMEID.BACCARAT && time < 1)
            {
                transform.DOLocalMove(new Vector2(0, 1000), 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
            if (time > -1)
            {

                if (transform == null)
                {
                    timeVal.Kill(true);
                }
                else
                {
                    if (isPlain)
                    {
                        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.TICKTOK);
                        showPlainNumberEffect(time);
                    }
                    else
                    {
                        lbTime.text = time.ToString();
                    }
                }

            }
        }).SetLoops(time);

    }
    private void showPlainNumberEffect(int time)
    {
        lbTime.text = time.ToString();
        //lbTime.node.stopAllActions();
        lbTime.transform.localScale = new Vector2(0.5f, 0.5f);
        lbTime.transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.OutBack);
    }
}
