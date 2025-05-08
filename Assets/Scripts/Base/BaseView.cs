using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Globals;

public class BaseView : MonoBehaviour
{
    [SerializeField]
    protected Globals.EFFECT_POPUP effect_popup = Globals.EFFECT_POPUP.SCALE;
    protected Globals.EFFECT_POPUP effect_popup_reverse = Globals.EFFECT_POPUP.SCALE;
    [SerializeField]
    public Image background = null;

    [SerializeField]
    [Range(0, 5)]
    protected float time_run = 0.3f;

    [SerializeField]
    bool isPlayOnLoad = false;

    [SerializeField]
    bool isEasing = true;

    [SerializeField]
    protected bool isCloseReverse = true;

    float originX = 0;
    float originY = 0;


    //[SerializeField]
    //bool isPopupOnTab = false;
    [SerializeField]
    bool isAlawayTop = false;

    [SerializeField]
    bool isSetParentNull = false;
    [SerializeField]
    bool isPlaySoundOpen = true;

    protected virtual void Awake()
    {
        if (background != null)
        {
            originX = background.transform.localPosition.x;
            originY = background.transform.localPosition.y;

        }
    }
    protected virtual void Update()
    {
        if (isAlawayTop)
            transform.SetAsLastSibling();
    }
    protected virtual void OnEnable()
    {
        if (isPlayOnLoad)
        {
            show();
        }
    }
    // // Start is called before the first frame update
    protected virtual void Start()
    {
        setStretch();
    }

    public virtual void OnDestroy()
    {
        //    if (isPopupOnTab)
        //    {
        //        SocketIOManager.getInstance().emitSIOCCCNew(Globals.Config.formatStr("ClickClose_%s", Globals.CURRENT_VIEW.getCurrentSceneName()));
        //        //Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.GAMELIST_VIEW);
        //    }
    }
    public void setOriginPosition(float _originX, float _originY)
    {
        originX = _originX;
        originY = _originY;
    }
    void setStretch()
    {
        var rect = GetComponent<RectTransform>();
        var anchorMin = rect.anchorMin;
        var anchorMax = rect.anchorMax;

        if (anchorMin.Equals(Vector2.zero) && anchorMax.Equals(Vector2.one))
        {
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
    }
    public void show(System.Action callback = null)
    {
        if (isPlaySoundOpen)
        {

            SoundManager.instance.soundClick();
        }

        gameObject.SetActive(true);
        if (background != null)
        {
            background.gameObject.SetActive(true);
        }

        if (background)
        {
            background.DOKill();
            Ease easssing = Ease.InSine;
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() =>
            {
                //    Globals.Logging.Log("ahihi");
                setStretch();

            });
            System.Action fade = () =>
            {
                var canvasGroup = background.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = background.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0.75f;

                canvasGroup.DOKill();
                canvasGroup.DOFade(1, time_run).SetEase(easssing);
            };
            switch (effect_popup)
            {
                case Globals.EFFECT_POPUP.NONE:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.NONE;
                        setStretch();
                        if (callback != null)
                            callback();
                        break;
                    }
                case Globals.EFFECT_POPUP.SCALE:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.SCALE;
                        background.rectTransform.localScale = new Vector3(0.8f, 0.8f, 0);
                        fade();
                        s.Append(background.rectTransform.DOScale(1, time_run).SetEase(Ease.OutBack).SetAutoKill(true));

                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_LEFT:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.MOVE_RIGHT;
                        fade();
                        background.transform.localPosition = new Vector3(-Screen.width, originY);

                        s.Append(background.transform.DOLocalMoveX(originX, time_run).SetEase(easssing).SetAutoKill(true));
                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_RIGHT:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.MOVE_LEFT;
                        fade();
                        background.rectTransform.localPosition = new Vector3(Screen.width, originY);
                        s.Append(background.rectTransform.DOLocalMoveX(originX, time_run).SetEase(easssing).SetAutoKill(true));
                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_UP:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.MOVE_DOWN;
                        fade();
                        background.rectTransform.localPosition = new Vector3(originX, -Screen.height);
                        s.Append(background.rectTransform.DOLocalMoveY(originX, time_run).SetEase(easssing).SetAutoKill(true));
                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_DOWN:
                    {
                        effect_popup_reverse = Globals.EFFECT_POPUP.MOVE_UP;
                        fade();
                        background.rectTransform.localPosition = new Vector3(originX, Screen.height);
                        s.Append(background.rectTransform.DOLocalMoveY(originX, time_run).SetEase(easssing).SetAutoKill(true));
                        break;
                    }
            }

            s.AppendCallback(() =>
            {
                //setStretch();
                if (callback != null)
                {
                    callback();
                }
            });
        }
        else
        {
            setStretch();
            if (callback != null)
                callback();
        }
    }

    public void hide(bool isDestroy = true, System.Action callback = null)
    {
        System.Action endFun = () =>
         {
             if (isDestroy)
             {
                 Destroy(gameObject);
             }
             else
             {
                 gameObject.SetActive(false);
                 if (isSetParentNull)
                 {
                     transform.SetParent(null);
                 }
             }
         };
        if (background)
        {
            background.DOKill();

            Ease easssing = Ease.OutSine;
            Sequence s = DOTween.Sequence();

            System.Action fade = () =>
            {
                var canvasGroup = background.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.DOKill();
                    canvasGroup.DOFade(0, time_run).SetEase(easssing);
                }
            };
            Globals.EFFECT_POPUP effPopup = isCloseReverse ? effect_popup_reverse : effect_popup;
            switch (effPopup)
            {
                case Globals.EFFECT_POPUP.NONE:
                    {
                        endFun();
                        break;
                    }
                case Globals.EFFECT_POPUP.SCALE:
                    {
                        fade();
                        s.Append(background.rectTransform.DOScale(0, time_run).SetEase(Ease.InBack).SetAutoKill(true));
                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_LEFT:
                    {
                        fade();
                        s.Append(background.rectTransform.DOLocalMoveX(-Screen.width, time_run).SetEase(easssing).SetAutoKill(true));

                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_RIGHT:
                    {

                        fade();
                        s.Append(background.rectTransform.DOLocalMoveX(Screen.width, time_run).SetEase(easssing).SetAutoKill(true));

                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_UP:
                    {
                        fade();
                        s.Append(background.rectTransform.DOLocalMoveY(Screen.height, time_run).SetEase(easssing).SetAutoKill(true));

                        break;
                    }
                case Globals.EFFECT_POPUP.MOVE_DOWN:
                    {
                        fade();
                        s.Append(background.rectTransform.DOLocalMoveY(-Screen.height, time_run).SetEase(easssing).SetAutoKill(true));

                        break;
                    }
            }
            s.AppendCallback(() =>
            {
                if (callback != null)
                {
                    callback();
                }
                endFun();
            });
        }
        else
        {
            endFun();
        }
    }

    public bool getIsShow()
    {
        return gameObject.activeSelf;
    }

    public virtual void onClickClose(bool isDestroy = true)
    {
        if (isPlaySoundOpen)
        {
            SoundManager.instance.soundClick();
        }
        hide(isDestroy);
    }
}
