using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using DG.Tweening;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

    [HideInInspector]
    public int code = 0, N = 0, S = 0;
    bool isJoker = false, isSelect = false, isTouch = true;
    public string nameCard = "";
    [SerializeField]
    Image imgBackground, imgValue, imgSuitSmall, imgSuitLarge;
    public bool isAllowTouch = true;
    [SerializeField]
    GameObject target, border, bkgMask;
    [SerializeField]
    public SkeletonGraphic animFire;
    [SerializeField]
    public SkeletonGraphic animSmoke;
    [SerializeField]
    public SkeletonDataAsset animSmokeData;

    [SerializeField]
    public SkeletonGraphic anim_laplanh;
    [SerializeField]
    public SkeletonGraphic anim_border;
    //public GameView GameController;
    private Button buttonClick;

    //BINH
    [SerializeField] Sprite spriteCardUp;
    [SerializeField] GameObject ic_starBinh;
    [SerializeField] Card card;
    //BINH

    // TONGITS
    [SerializeField] GameObject card_bg_yellow;
    [SerializeField] GameObject specialTg;
    [SerializeField] GameObject card_border_blue;
    // TONGITS

    // LUCKY9
    [SerializeField] GameObject imgShan;
    // LUCKY9



    [SerializeField]
    GameObject bkgDummySpecial;
    // Start is called before the first frame update
    [SerializeField] private Sprite spriteBackCard;
    private RectTransform _ThisRT;
    void Start()
    {
        if (Globals.Config.curGameId == (int)Globals.GAMEID.DUMMY || Globals.Config.curGameId == (int)Globals.GAMEID.KEANG)
        {
            GetComponent<Image>().raycastTarget = true;
        }
        _ThisRT = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        //reset ui
        if (animFire != null)
        {
            animFire.gameObject.SetActive(false);
        }

    }
    // Update is called once per frame
    //void Update()
    //{

    //}
    public void resetDefaul()
    {
        setTextureWithCode(0);
        setColor(-1);
        setOpacity(255);
        setDark(false);
        showBorder(false);
        showSmoke(false);
        showTarget(false);
    }

    public void addButtonClick(System.Action callback)
    {
        if (buttonClick == null)
        {
            buttonClick = gameObject.AddComponent<Button>();
        }
        buttonClick.onClick.RemoveAllListeners();
        buttonClick.onClick.AddListener(() =>
        {
            callback.Invoke();
        });
    }

    public void addButtonClick(System.Action<Card> callback)
    {
        if (buttonClick == null)
        {
            buttonClick = gameObject.AddComponent<Button>();
        }
        buttonClick.onClick.RemoveAllListeners();
        buttonClick.onClick.AddListener(() =>
        {
            callback.Invoke(this);
        });
    }
    public void SetPivot(float x, float y)
    {
        if (_ThisRT == null) _ThisRT = GetComponent<RectTransform>();
        _ThisRT.pivot = new Vector2(x, y);
    }
    public void setDark(bool isDark, Sprite _spriteFrame = null)
    {
        bkgMask.SetActive(isDark);
        showSpecialTg(false);
        //RectTransform bkgMaskRect = bkgMask.GetComponent<RectTransform>();
        //if (imgSuitSmall.gameObject.activeSelf == true)
        //{
        //    bkgMaskRect.sizeDelta = new Vector2(127, bkgMaskRect.sizeDelta.y);
        //}
        //else
        //{
        //    bkgMaskRect.sizeDelta = new Vector2(147, bkgMaskRect.sizeDelta.y);
        //}
        // this.showTarget(false);
    }

    public void SetTextureBackCard()
    {
        imgValue.gameObject.SetActive(false);
        imgSuitSmall.gameObject.SetActive(false);
        imgSuitLarge.gameObject.SetActive(false);
        imgBackground.sprite = spriteBackCard;
    }
    public void setTextureWithCode(int cod, bool isSmall = false, bool isShan = false, bool isShowCorner = false)
    {
        try
        {
            code = cod;
            if (code == Globals.Config.CODE_JOKER_RED || code == Globals.Config.CODE_JOKER_BLACK)
            {
                decodeCard(code);
                imgValue.gameObject.SetActive(true);
                imgSuitSmall.gameObject.SetActive(false);
                imgSuitLarge.gameObject.SetActive(true);
                setJoker(true);

                imgBackground.sprite = UIManager.instance.cardAtlas.GetSprite("bkg_white");
                imgValue.sprite = UIManager.instance.cardAtlas.GetSprite("card_joker_text");
                if (code == Globals.Config.CODE_JOKER_RED)
                {
                    imgSuitLarge.sprite = UIManager.instance.cardAtlas.GetSprite("card_joker_red");
                    imgValue.color = Color.red;
                }
                else
                {
                    imgSuitLarge.sprite = UIManager.instance.cardAtlas.GetSprite("card_joker_black");
                    imgValue.color = Color.black;
                }
                imgValue.SetNativeSize();
                //imgSuitSmall.SetNativeSize();
                imgSuitLarge.SetNativeSize();
            }
            else if (code <= 0 || code > 52)
            {
                imgValue.gameObject.SetActive(false);
                imgSuitSmall.gameObject.SetActive(false);
                imgSuitLarge.gameObject.SetActive(false);
                //bg_cardSmall.active = false;
                //imgBackground.spriteFrame = UIManager.instance.listFrameCard.getSpriteFrame(ResDefine.card_back);

                //imgBackground.sprite = UIManager.instance.cardAtlas.GetSprite("card_back");
                // BINH comment
                imgBackground.sprite = spriteCardUp;
            }
            else
            {
                imgValue.gameObject.SetActive(true);
                imgSuitSmall.gameObject.SetActive(true);
                imgSuitLarge.gameObject.SetActive(true);

                decodeCard(code);

                imgBackground.sprite = UIManager.instance.cardAtlas.GetSprite("bkg_white");
                var strSuit = getSuitInVN();

                imgValue.sprite = UIManager.instance.cardAtlas.GetSprite(string.Format("card_{0}", getValue()));
                imgSuitSmall.sprite = UIManager.instance.cardAtlas.GetSprite(string.Format("card_{0}_small", strSuit));
                if (N >= 11 && N <= 13)
                {
                    imgSuitLarge.sprite = UIManager.instance.cardAtlas.GetSprite(string.Format("card_{0}_{1}", getValue(), strSuit));
                }
                else
                {
                    imgSuitLarge.sprite = UIManager.instance.cardAtlas.GetSprite(string.Format("card_{0}", strSuit));
                }

                if (strSuit == "tep" || strSuit == "bich")
                {
                    imgValue.color = Color.black;
                }
                else
                {
                    imgValue.color = Color.red;
                }

                imgValue.SetNativeSize();
                imgSuitSmall.SetNativeSize();
                imgSuitLarge.SetNativeSize();
                if (N == 10)
                {
                    imgValue.GetComponent<RectTransform>().sizeDelta = new Vector2(37, imgValue.GetComponent<RectTransform>().sizeDelta.y);
                }
                if (Globals.Config.curGameId == (int)Globals.GAMEID.DUMMY && (code == 11 || code == 14))
                {
                    bkgDummySpecial.SetActive(true);
                }
                else if (bkgDummySpecial != null)
                {
                    bkgDummySpecial.SetActive(false);
                }
            }
            var gameId = Globals.Config.curGameId;
            if (gameId == (int)Globals.GAMEID.TONGITS_OLD || gameId == (int)Globals.GAMEID.TONGITS || gameId == (int)Globals.GAMEID.TONGITS11 || gameId == (int)Globals.GAMEID.TONGITS_JOKER)
            {
                if (N == 14 || N == 13 || N == 1)
                {
                    if (code != 0)
                    {
                        showSpecialTg(true);
                    }
                    else
                    {
                        showSpecialTg(false);
                    }
                }
                else
                {
                    showSpecialTg(false);
                }
                if (card_bg_yellow.activeSelf == true || card_border_blue.activeSelf == true || bkgMask.activeSelf == true)
                {
                    showSpecialTg(false);
                }
            }
            if (isShan == true)
            {
                showShanCard();
                showShanCorner(isShowCorner);
            }
            else
            {
                if (imgShan != null) imgShan.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Globals.Logging.LogException(e);
        }

    }

    public void showShanCard()
    {
        if (imgShan == null) return;
        string suitName = getSuitInVN();
        imgShan.SetActive(true);
        CardShan cardShanComponent = imgShan.GetComponent<CardShan>();
        if (cardShanComponent != null)
        {
            cardShanComponent.SetInfo(S, N, suitName, imgSuitSmall.sprite, imgValue.sprite);
        }
    }

    public void showShanCorner(bool isShow, float time = 0.4f)
    {
        if (imgShan != null) imgShan.GetComponent<CardShan>().showCorner(isShow, time);
    }


    public void setFire(int level)
    {
        string animName = "fire" + level;
        animFire.gameObject.SetActive(true);
        animFire.Initialize(true);
        animFire.AnimationState.SetAnimation(0, animName, false);
    }
    public void setSmoke()
    {
        animSmoke.gameObject.SetActive(true);
        animSmoke.skeletonDataAsset = animSmokeData;
        animSmoke.Initialize(true);
        animSmoke.AnimationState.Complete += delegate
        {
            animSmoke.gameObject.SetActive(false);
        };
        animSmoke.AnimationState.SetAnimation(0, "animation", false);
    }
    public void setJoker(bool _isJoker)
    {
        isJoker = _isJoker;
    }


    void showSmall(bool isSmall)
    {
        if (isSmall == true)
        {
            //bg_cardSmall.active = true;
            //imgBackground.gameObject.SetActive(false;
        }
        else
        {
            //bg_cardSmall.active = false;
            //imgBackground.gameObject.SetActive(true;
        }
    }

    public int encodeCard()
    {
        // // mỗi game có 1 điều encode # nhau
        if (N == Globals.Config.CODE_JOKER_RED || N == Globals.Config.CODE_JOKER_BLACK)
            return N;
        if (Globals.Config.curGameId == (int)Globals.GAMEID.KARTU_QIU)
            return 13 * (S - 1) + N;
        return 13 * (S - 1) + N - 1;
    }
    public void decodeCard(int cod)
    {
        code = cod;
        Globals.Config.decodeCard(cod, ref N, ref S);
        nameCard = N + getSuitInVN();
        if (code == Globals.Config.CODE_JOKER_RED || code == Globals.Config.CODE_JOKER_BLACK)
        {
            S = code;
            N = code;
            return;
        }
        // // mỗi game có 1 điều decode # nhau
        if (Globals.Config.curGameId != (int)Globals.GAMEID.LUCKY_89)
        {
            S = ((cod - 1) / 13) + 1; //>=1 <=4
            N = ((cod - 1) % 13) + 2; // >=2 , <=14
        }

        if (Globals.Config.curGameId == (int)Globals.GAMEID.KARTU_QIU
            || Globals.Config.curGameId == (int)Globals.GAMEID.KEANG)
        {
            N = ((cod - 1) % 13) + 1;
        }
        if (Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11 ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_OLD ||
            Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_JOKER)
        {
            if (N == 14) N = 1;
        }
        //nameCard = N + getSuitInVN();
    }

    int getValue()
    {
        if (N == Globals.Config.CODE_JOKER_RED || N == Globals.Config.CODE_JOKER_BLACK)
            return N;
        if (N > 13)
        {
            return (N - 13);
        }

        return N;
    }

    public string getSuitInVN()
    {
        //if (Globals.Config.curGameId == Globals.GAMEID.BOOGYI || Globals.Config.curGameId === Globals.GAMEID.BOHN)
        //{
        //	switch (S)
        //	{
        //		case 1:
        //			return "tep";
        //		case 2:
        //			return "ro";
        //		case 3:
        //			return "co";
        //		case 4:
        //			return "bich";
        //		default:
        //			return "joker";
        //	}
        //}
        //else
        //{
        switch (S)
        {
            case 1:
                return "bich";
            case 2:
                return "tep";
            case 3:
                return "ro";
            case 4:
                return "co";
            default:
                return "joker";
        }
        //}
    }

    public void showBorder(bool isShow)
    {
        border.SetActive(isShow);
    }


    public void showTarget(bool isShow)
    {
        target.SetActive(isShow);
    }

    public void showSmoke(bool isShow)
    {
        if (isShow)
        {
            animSmoke.gameObject.SetActive(true);
            animSmoke.skeletonDataAsset = animSmokeData;
            animSmoke.Initialize(true);
            animSmoke.AnimationState.SetAnimation(0, "animation", true);
            animSmoke.AnimationState.Complete += delegate
            {
                animSmoke.gameObject.SetActive(false);
            };
        }

        else
        {
            animSmoke.gameObject.SetActive(false);
        }

    }

    public void setEffect_Twinkle(bool isShow, float delay)
    {
        if (isShow)
        {
            anim_laplanh.gameObject.SetActive(true);
            anim_laplanh.AnimationState.SetAnimation(0, "animation", true);
            DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
            {
                anim_laplanh.gameObject.SetActive(false);
            });
            //this.node.runAction(cc.sequence(cc.delayTime(delay), cc.callFunc(() =>
            //{
            //	this.anim_laplanh.node.active = false;
            //})));
        }
        else
        {
            anim_laplanh.gameObject.SetActive(false);
        }
    }

    public void setOpacity(int opacity)
    {
        var color = imgBackground.color;
        color.a = 1.0f * opacity / 255f;
        imgBackground.color = color;
    }

    public int getOpacity()
    {
        return (int)(255 * imgBackground.color.a);
    }

    public void setColor(int type)
    {
        var color = Color.white;
        switch (type)
        {
            case 1:
                color = new Color(251f / 255f, 255f / 255f, 146f / 255f);

                break;
            case 2:
                color = new Color(134f / 255f, 221f / 255f, 255f / 255f);
                break;
        }
        imgBackground.color = color;
    }

    System.Action<PointerEventData, Card> OnBeginDragCallback, OnDragCallback, OnEndDragCallback, OnPointerClickCallback;
    public void setListenerDragDrop(System.Action<PointerEventData, Card> _OnBeginDrag, System.Action<PointerEventData, Card> _OnDrag, System.Action<PointerEventData, Card> _OnEndDrag, System.Action<PointerEventData, Card> _OnPointClick = null)
    {
        OnBeginDragCallback = _OnBeginDrag;
        OnDragCallback = _OnDrag;
        OnEndDragCallback = _OnEndDrag;
        OnPointerClickCallback = _OnPointClick;
    }

    public void removeAllListenerDragDrop()
    {
        OnBeginDragCallback = null;
        OnDragCallback = null;
        OnEndDragCallback = null;
        OnPointerClickCallback = null;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragCallback != null)
        {
            OnBeginDragCallback.Invoke(eventData, this);
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (OnDragCallback != null)
        {
            OnDragCallback.Invoke(eventData, this);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragCallback != null)
        {
            OnEndDragCallback.Invoke(eventData, this);
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (OnPointerClickCallback != null)
        {
            OnBeginDragCallback.Invoke(eventData, this);
        }
    }

    public string LogInfo()
    {
        return (N + getSuitInVN() + " ");
    }

    public string LogInfo2()
    {
        return ("{ N: " + N + ", S: " + S + "},");
    }

    // BINH 
    public void showStarBinh(bool isShow, int index = 0)
    {
        ic_starBinh.gameObject.SetActive(false);
        ic_starBinh.transform.DOKill();

        if (isShow)
        {
            ic_starBinh.gameObject.SetActive(true);
            ic_starBinh.GetComponent<Image>().color = index == 0 ? Color.white : Color.green;
            ic_starBinh.transform.localScale = new Vector3(10f, 10f, 10f);
            ic_starBinh.transform.rotation = Quaternion.Euler(0f, 0f, -180f);
            ic_starBinh.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic);
            ic_starBinh.transform.DORotate(Vector3.zero, 0.5f).SetEase(Ease.OutCubic);
        }
    }
    // BINH

    // TONGITS
    public void turnHighlightYellow(bool isOn)
    {
        card_bg_yellow.SetActive(isOn);
        showTarget(false);
        showSpecialTg(false);
    }

    public void showSpecialTg(bool isShow)
    {
        specialTg.SetActive(isShow);
    }

    public void turnBorderBlue(bool isShow)
    {
        card_border_blue.SetActive(isShow);
        showSpecialTg(false);
    }

}
