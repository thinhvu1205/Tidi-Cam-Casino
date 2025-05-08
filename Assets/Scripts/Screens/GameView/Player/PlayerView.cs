using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;
using Globals;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI txtName, txtMoney;
    [SerializeField]
    public Avatar avatar;
    [SerializeField]
    Image timeCounDown;

    [SerializeField]
    GameObject objHost, objExit, dealerIcon, bkgThanhBar;
    [SerializeField]
    TextMeshProUGUI lbChipWinLose;

    [SerializeField]
    TMP_FontAsset fontWin, fontLose;

    float timeTurn = 0;
    public long agLose = 0, agWin = 0;
    private long agCurrent = 0;
    public bool isThisPlayer = false;
    public long chipJackpot = 0;

    [SerializeField]
    public SkeletonGraphic animResult;

    [SerializeField]
    public GameObject aniAllIn;

    [SerializeField]
    public GameObject nodeCard;

    [SerializeField]
    public GameObject lucky9Ani;

    [SerializeField]
    public GameObject hitpot;

    [SerializeField]
    public List<GameObject> pots;

    //[SerializeField]
    //public SkeletonDataAsset animWin;
    //[SerializeField]
    //public SkeletonDataAsset animLose;
    //[SerializeField]
    //public SkeletonDataAsset animDraw;

    [SerializeField]
    [Tooltip("0-lose, 1-draw, 2-win")]
    public List<SkeletonDataAsset> listAnimResult;
    [HideInInspector]
    public List<Card> cards = new List<Card>();
    private Sequence seqTextFly;

    GameObject itemVip;

    public void setName(string namePl)
    {
        txtName.text = namePl;
        Globals.Config.effectTextRunInMask(txtName);
    }
    public void setEffectAllIn(bool isAllIn = true)
    {
        aniAllIn.gameObject.SetActive(isAllIn);
    }

    public void setAvatar(int avaId, string fname, string Faid, int vip)
    {
        //Debug.Log("avaId=" + avaId);
        //Debug.Log("fname=" + fname);
        //Debug.Log("Faid=" + Faid);
        avatar.loadAvatarAsync(avaId, fname, Faid);
        avatar.setVip(vip);

    }

    bool isOnItemVip = true;
    public void updateItemVip(int idVip, int vip, int idPosTongits = -1)
    {
        if (vip >= 5 && idVip != 0)
        {
            if (itemVip == null && isOnItemVip)
            {
                isOnItemVip = false;
                itemVip = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/ItemVip"), transform);
            }
            var vecPos = itemVip.transform.localPosition;
            var vecPosThis = transform.localPosition;
            var size = gameObject.GetComponent<RectTransform>().sizeDelta;
            if (Config.curGameId == (int)GAMEID.TONGITS_OLD || Config.curGameId == (int)GAMEID.TONGITS_JOKER || Config.curGameId == (int)GAMEID.TONGITS)
            {
                switch (idPosTongits)
                {
                    case 0: vecPos.x = 55; break;
                    case 1: vecPos.x = -100; break;
                    case 2: vecPos.x = 100; break;
                }
            }
            else if (Config.curGameId == (int)GAMEID.LUCKY_89 || Config.curGameId == (int)GAMEID.GAOGEA)
            {
                vecPos.x = vecPosThis.x < 0 ? 100 : -100;
                vecPos.y = -60;
            }
            else if (Config.curGameId == (int)GAMEID.SICBO)
            {
                vecPos.x = vecPosThis.x < 0 ? -100 : 100;
                vecPos.y = 0;
            }
            else
            {
                vecPos.x = vecPosThis.x > 0 ? 100 : -100;
                vecPos.y = -60;
            }
            itemVip.SetActive(true);
            itemVip.transform.localPosition = vecPos;
            updateItemVipFromSV(idVip);
            itemVip.transform.SetAsLastSibling();
            Button btn = itemVip.GetComponent<Button>();
            btn.interactable = (vip > 5) && isThisPlayer;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { onClickSelectItemVip(vip); });
        }
        else
        {
            if (itemVip != null) itemVip.SetActive(false);
        }
    }


    public void OnDisable()
    {
        OnDestroy();
    }

    public void OnDestroy()
    {
        if (itemVip != null)
        {
            Destroy(itemVip.gameObject);
        }
        itemVip = null;
        isOnItemVip = true;

        if (BkgVip != null)
        {
            Destroy(BkgVip.gameObject);
        }
        BkgVip = null;
    }

    public void updateItemVipFromSV(int idItem)
    {
        Debug.Log("idItem  " + idItem);
        var itemIdVip = idItem / 10;
        if (itemIdVip > 10) itemIdVip = 10;
        Debug.Log("itemIdVip  " + itemIdVip);
        if (itemVip != null && itemIdVip >= 5)
        {
            GameObject animGO = itemVip.transform.Find("anim").gameObject;
            if (animGO != null)
            {
                SkeletonGraphic animSG = animGO.GetComponent<SkeletonGraphic>();
                if (animSG != null) animSG.AnimationState.SetAnimation(0, itemIdVip.ToString(), true);
            }
        }
    }

    Transform BkgVip;
    void onClickSelectItemVip(int vip)
    {
        if (itemVip != null && isThisPlayer)
        {
            if (BkgVip == null)
            {
                BkgVip = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/BkgItemVip"), UIManager.instance.gameView.transform).transform;
                //BkgVip.gameObject.SetActive(false);
                //var bkgItems = itemVip.transform.GetChild(0);

                for (var i = 0; i < BkgVip.childCount; i++)
                {
                    var index = i;
                    var item = BkgVip.GetChild(i);
                    if (index + 5 <= vip)
                    {
                        item.gameObject.SetActive(true);
                        item.GetComponent<Button>().onClick.RemoveAllListeners();
                        item.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            onClickItemVip(index + 5);
                        });
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                BkgVip.gameObject.SetActive(!BkgVip.gameObject.activeSelf);
            }


            if (BkgVip.gameObject.activeSelf)
            {
                //var sizeScreen = Screen.currentResolution;
                BkgVip.SetAsLastSibling();
                //BkgVip.localScale = itemVip.transform.localScale;
                var scale = itemVip.transform.localScale.x;
                BkgVip.localScale = Vector3.zero;

                DOTween.Sequence().AppendInterval(0.2f).AppendCallback(() =>
                {
                    var sizeBkg = BkgVip.GetComponent<RectTransform>().sizeDelta;
                    var vecPosThis = transform.localPosition;
                    var posBkg = BkgVip.localPosition;

                    if (itemVip.transform.localPosition.x < 0)
                    {
                        posBkg.x = vecPosThis.x - 100;
                    }
                    else
                    {
                        posBkg.x = vecPosThis.x + 100;
                    }
                    if (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
                    {
                        posBkg.y = vecPosThis.y - sizeBkg.y * scale - 30;
                    }
                    else
                        posBkg.y = vecPosThis.y - 30;
                    //posBkg.y = 50;
                    //posBkg.x = 0;

                    //        //if (itemVip.transform.position.x - sizeBkg.x * scale / 2 < -sizeScreen.width / 2)
                    //        //{
                    //        //    Debug.Log("-=-=-=-=-=-=-==-th 1");
                    //        //    posBkg.x = -sizeScreen.width / 2 + sizeBkg.x * scale / 2 - vecPos.x;
                    //        //}

                    //        //if (itemVip.transform.position.y + sizeBkg.y * scale > sizeScreen.height / 2)
                    //        //{
                    //        //    Debug.Log("-=-=-=-=-=-=-==-th 2");
                    //        //    posBkg.y = -sizeBkg.y - 60;
                    //        //}

                    //        if (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
                    //{
                    //    posBkg.y = -sizeBkg.y - 60;
                    //}
                    //        //if (vecPos.x - sizeBkg.x * scale / 2 < -sizeScreen.width / 2)
                    //        //{
                    //        //    Debug.Log("-=-=-=-=-=-=-==-th 1");
                    //        //    posBkg.x = -sizeScreen.width / 2 + sizeBkg.x * scale / 2 - vecPos.x;
                    //        //}

                    //        //if (vecPos.y + sizeBkg.y * scale > sizeScreen.height / 2)
                    //        //{
                    //        //    Debug.Log("-=-=-=-=-=-=-==-th 2");
                    //        //    posBkg.y = -sizeBkg.y - 60;
                    //        //}

                    BkgVip.localPosition = posBkg;
                    BkgVip.DOScale(itemVip.transform.localScale, .2f);
                });
            }
            //    var sizeScreen = Screen.currentResolution;

            //    Debug.Log("-=-=-=-=-=-=-==-sizeScreen  " + sizeScreen);
            //    Transform bkg = itemVip.transform.Find("Bkg");
            //    bkg.gameObject.SetActive(!bkg.gameObject.activeSelf);


            //    if (bkg.gameObject.activeSelf)
            //    {
            //        itemVip.transform.SetAsLastSibling();

            //        var posBkg = bkg.localPosition;
            //        posBkg.x = -2000;
            //        bkg.localPosition = posBkg;
            //        DOTween.Sequence().AppendInterval(0.2f).AppendCallback(() =>
            //        {
            //            var vecPos = itemVip.transform.localPosition;
            //            var sizeBkg = bkg.GetComponent<RectTransform>().sizeDelta;

            //            posBkg.y = 50;
            //            posBkg.x = 0;

            //            //if (itemVip.transform.position.x - sizeBkg.x * scale / 2 < -sizeScreen.width / 2)
            //            //{
            //            //    Debug.Log("-=-=-=-=-=-=-==-th 1");
            //            //    posBkg.x = -sizeScreen.width / 2 + sizeBkg.x * scale / 2 - vecPos.x;
            //            //}

            //            //if (itemVip.transform.position.y + sizeBkg.y * scale > sizeScreen.height / 2)
            //            //{
            //            //    Debug.Log("-=-=-=-=-=-=-==-th 2");
            //            //    posBkg.y = -sizeBkg.y - 60;
            //            //}

            //            if (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
            //            {
            //                posBkg.y = -sizeBkg.y - 60;
            //            }
            //            //if (vecPos.x - sizeBkg.x * scale / 2 < -sizeScreen.width / 2)
            //            //{
            //            //    Debug.Log("-=-=-=-=-=-=-==-th 1");
            //            //    posBkg.x = -sizeScreen.width / 2 + sizeBkg.x * scale / 2 - vecPos.x;
            //            //}

            //            //if (vecPos.y + sizeBkg.y * scale > sizeScreen.height / 2)
            //            //{
            //            //    Debug.Log("-=-=-=-=-=-=-==-th 2");
            //            //    posBkg.y = -sizeBkg.y - 60;
            //            //}

            //            bkg.localPosition = posBkg;
            //            bkg.transform.DOScale(Vector3.one, .2f);
            //        });
            //    }
        }
    }


    void onClickItemVip(int vip)
    {
        SocketSend.sendUpdateItemVip(vip);
        if (BkgVip != null)
        {
            BkgVip.gameObject.SetActive(false);
        }
    }

    public void setAg(long ag)
    {
        //txtMoney.text = Globals.Config.FormatMoney(ag);
        Globals.Config.tweenNumberTo(txtMoney, ag, agCurrent, 0.3f, false, false);
        agCurrent = ag;
    }

    public void setCallbackClick(System.Action callback)
    {
        var btnCom = avatar.gameObject.GetComponent<Button>();
        if (btnCom == null)
        {
            btnCom = avatar.gameObject.AddComponent<Button>();
        }
        btnCom.onClick.RemoveAllListeners();
        btnCom.onClick.AddListener(() =>
        {
            Globals.Logging.Log("Click avatar");
            callback();
        });
    }

    public void setPosThanhBarThisPlayer()
    {
        if (
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_JOKER ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11 ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_OLD

        )
        {
            bkgThanhBar.transform.localPosition = new Vector2(-120, 5);
            bkgThanhBar.GetComponent<Image>().enabled = false;
            txtMoney.fontSize = 23;
            txtName.fontSize = 26;
            txtName.gameObject.transform.parent.transform.localPosition = new Vector2(txtName.gameObject.transform.parent.transform.localPosition.x, 15);
            return;
        }
        else if (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
        {
            bkgThanhBar.transform.localPosition = new Vector2(113, -20);
        }
        else
        {
            bkgThanhBar.transform.localPosition = new Vector2(120, -12);
        }
    }

    public virtual void setTurn(bool isTurn, float _timeTurn = 0f, bool _isMe = false, float timeVibrate = 5f)
    {
        timeCounDown.gameObject.SetActive(isTurn);
        if (isTurn)
        {
            StartCoroutine(fillAmountToZero());
        }
        ///////////////////////////////////
        IEnumerator fillAmountToZero()
        {
            timeTurn = _timeTurn;
            timeCounDown.fillAmount = 1;
            avatar.transform.DOScale(1.1f * Vector2.one, .1f).OnComplete(() => { avatar.transform.DOScale(Vector2.one, .1f); });
            float elapsedTime = 0;
            while (timeCounDown.fillAmount > 0)
            {
                yield return new WaitForFixedUpdate();
                timeCounDown.fillAmount -= Time.fixedDeltaTime / timeTurn;
                elapsedTime += Time.fixedDeltaTime;
                if (!timeCounDown.gameObject.activeSelf) yield break;
                if (_isMe && (elapsedTime >= timeTurn - timeVibrate))
                {
                    Config.Vibration();
                    elapsedTime = -99;
                }
            }
        }
    }
    public bool getIsTurn()
    {
        return timeCounDown.gameObject.activeInHierarchy;
    }
    public Sprite getAvatarSprite()
    {
        return avatar.image.sprite;
    }
    public void setDark(bool isDark)
    {
        avatar.setDark(isDark);

    }
    public virtual void setEffectWin(string animName = "", bool isLoop = true)
    {
        //animResult.TrimRenderers();
        animResult.gameObject.SetActive(true);
        animResult.skeletonDataAsset = listAnimResult[2];
        animResult.Initialize(true);
        if (animName == "")
        {
            animResult.AnimationState.SetAnimation(0, "win", isLoop);
        }
        else
        {
            animResult.AnimationState.SetAnimation(0, animName, isLoop);
        }
        if (isLoop == false)
        {
            animResult.AnimationState.Complete += delegate
            {
                animResult.gameObject.SetActive(false);
            };
        }


    }
    public virtual void setEffectLose(bool isLoop = true)
    {
        animResult.TrimRenderers();
        animResult.gameObject.SetActive(true);
        animResult.skeletonDataAsset = listAnimResult[0];
        animResult.Initialize(true);
        animResult.AnimationState.SetAnimation(0, "lose", isLoop);

        if (isLoop == false)
        {
            animResult.AnimationState.Complete += delegate
            {
                animResult.gameObject.SetActive(false);
            };
        }
    }
    public void setEffectDraw(bool isLoop = true)
    {
        animResult.gameObject.SetActive(true);
        animResult.skeletonDataAsset = listAnimResult[1];
        animResult.Initialize(true);
        animResult.AnimationState.SetAnimation(0, "draw", true);

        if (isLoop == false)
        {
            animResult.AnimationState.Complete += delegate
            {
                animResult.gameObject.SetActive(false);
            };
        }
    }
    public void setReady(bool isReady)
    {
        avatar.setDark(!isReady);
    }

    public void setHost(bool isHost)
    {
        objHost.SetActive(isHost);
    }

    public void setExit(bool isExit)
    {
        objExit.SetActive(isExit);
    }

    public void effectFlyMoney(long mo, int fonzSize = 50)
    {
        if (mo == 0)
        {
            return;
        }
        lbChipWinLose.fontSize = fonzSize;
        if (mo < 0)
        {
            lbChipWinLose.font = fontLose;
            lbChipWinLose.text = Globals.Config.FormatMoney2(mo, true, true);
        }
        else
        {
            lbChipWinLose.font = fontWin;
            lbChipWinLose.text = "+" + Globals.Config.FormatMoney2(mo, true, true);
        }


        lbChipWinLose.transform.localPosition = Vector2.zero;
        int height = 100;
        if (Globals.Config.curGameId == (int)Globals.GAMEID.SICBO && transform.localPosition.y > 280)
        {
            height = 50;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.BANDAR_QQ || Globals.Config.curGameId == (int)Globals.GAMEID.PUSOY)
        {
            lbChipWinLose.transform.localPosition = new Vector2(0, -30);
            height = 50;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.RONGHO)
        {
            //lbChipWinLose.transform.localPosition = new Vector2(0, );
            height = 50;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.BLACKJACK)
        {
            //lbChipWinLose.transform.localPosition = new Vector2(0, );
            height = 60;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.BACCARAT)
        {
            height = 60;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.KARTU_QIU)
        {
            height = 60;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.DOMINO)
        {
            lbChipWinLose.transform.localPosition = new Vector2(0, -30);
            height = 50;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.LUCKY9)
        {
            height = 35;
        }
        lbChipWinLose.gameObject.SetActive(true);
        if (seqTextFly != null)
        {
            seqTextFly.Kill();
        }
        seqTextFly = DOTween.Sequence()
             .Append(lbChipWinLose.transform.DOLocalMove(new Vector2(0, height), 2.0f).SetEase(Ease.OutBack))
             .AppendInterval(1.0f)
             .AppendCallback(() =>
             {
                 lbChipWinLose.gameObject.SetActive(false);
             });
    }

    public void showDealer(bool isShow, bool isLeft = false, bool isUp = false)
    {
        dealerIcon.SetActive(isShow);
        if (!isShow) return;
        float posx = isLeft == true ? -60 : 60;
        float posy = isUp == true ? 25 : -25;
        if (Globals.Config.curGameId == (int)Globals.GAMEID.GAOGEA)
        {
            posy = -82;
            posx = isLeft == true ? -82 : 82;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.KARTU_QIU)
        {
            posx = isLeft == true ? -47 : 47;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.LUCKY9)
        {
            posx = isLeft == true ? -85 : 85;
            posy = isUp == true ? 50 : -50;
        }
        dealerIcon.transform.DOLocalMove(new Vector2(posx, posy), 0);
        dealerIcon.GetComponent<CanvasGroup>().alpha = 0;
        dealerIcon.transform.eulerAngles = new Vector3(0, 0, 90);
        dealerIcon.transform.localScale = new Vector2(3, 3);
        dealerIcon.transform.DOScale(Vector2.one, 0.6f).SetEase(Ease.OutCubic);
        dealerIcon.transform.DOLocalRotate(Vector3.zero, 0.6f).SetEase(Ease.OutCubic);
        dealerIcon.GetComponent<CanvasGroup>().DOFade(1, 0.6f);
    }

    public void effectFlyMoneyLucky9(long mo, int fonzSize = 50, int height = 30, float durationMove = 1.5f, float durationOnEnable = 0f)
    {
        if (mo == 0)
        {
            return;
        }

        lbChipWinLose.fontSize = fonzSize;
        if (mo < 0)
        {
            lbChipWinLose.font = fontLose;
            lbChipWinLose.text = Globals.Config.FormatMoney2(mo, true, true);
        }
        else
        {
            lbChipWinLose.font = fontWin;
            lbChipWinLose.text = "+" + Globals.Config.FormatMoney2(mo, true, true);
        }
        lbChipWinLose.transform.localPosition = Vector2.zero;
        lbChipWinLose.gameObject.SetActive(true);
        if (seqTextFly != null)
        {
            seqTextFly.Kill();
        }
        seqTextFly = DOTween.Sequence()
            .Append(lbChipWinLose.transform.DOLocalMove(new Vector2(0, height), durationMove).SetEase(Ease.OutBack))
            .AppendInterval(durationOnEnable)
            .AppendCallback(() =>
            {
                lbChipWinLose.gameObject.SetActive(false);
            });
    }

    public void enableHitpot(bool isShow = false)
    {
        hitpot.SetActive(isShow);
    }

    public void setHitpot(int num)
    {
        for (int i = 0; i < 4; i++)
        {
            pots[i].gameObject.SetActive(i < num);
        }
    }
}
