using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Spine.Unity;
using System.Runtime.ConstrainedExecution;

using TMPro;

public class BorkSlideCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    [SerializeField] GameObject thumbTip;
    [SerializeField] Card Card1;
    [SerializeField] Card Card2;
    [SerializeField] Card Card3;
    [SerializeField] List<Card> listCard = new List<Card>();
    [SerializeField] public Button BtnDraw;
    [SerializeField] public Button BtnCancel;
    [SerializeField] public Button BtnShowCard;
    [SerializeField] Transform thumbbase;
    [SerializeField] TextMeshProUGUI lbTimer;
    [SerializeField] SkeletonGraphic animBanTay;
    private Vector2 beginPos = Vector2.zero;
    private float initRotCard1 = 0, initRotCard2 = 0, initRotCard3 = 0, initRotThumbTip = 0, initRotThumbTip2 = 0, initRotCardParent12 = 0;
    private Vector2 initPosCard1 = Vector2.zero, initPosCard2 = Vector2.zero, initPosCard3 = Vector2.zero, initPosThumb = Vector2.zero;
    private int totalCard = 2;
    private Action cbShowCard = null;
    private bool isSlide = true;
    void Start()
    {

    }
    private void Awake()
    {
        initRotCard1 = Card1.transform.localEulerAngles.z;
        initRotCard2 = Card2.transform.localEulerAngles.z;
        initRotCard3 = Card3.transform.localEulerAngles.z;
        initRotCardParent12 = Card1.transform.parent.transform.localEulerAngles.z;
        initPosCard1 = Card1.transform.localPosition;
        initPosCard2 = Card2.transform.localPosition;
        initPosCard3 = Card3.transform.localPosition;
        initRotThumbTip = thumbTip.transform.localEulerAngles.z;
        initPosThumb = thumbTip.transform.localPosition;
    }

    // Update is called once per frame

    public void OnDrag(PointerEventData eventData)
    {

        //Debug.Log("isSlide=" + isSlide);
        if (isSlide)
        {
            float delta = eventData.position.x - beginPos.x;
            float DeltaRotate = -(delta / 10);
            if (totalCard == 2)
            {

                if (Mathf.Abs(DeltaRotate) <= 12) //goc quay tay (0-24,goc mac dinh 12 +>Delta goc=12);
                {

                    float eulerAngle = initRotCard2 + DeltaRotate;
                    float eulerAngleThumb = initRotThumbTip + DeltaRotate;
                    Card2.transform.eulerAngles = new Vector3(0, 0, eulerAngle);
                    Card1.transform.eulerAngles = new Vector3(0, 0, -eulerAngle);
                    Card2.transform.localPosition = new Vector2(initPosCard2.x - (DeltaRotate * 2), initPosCard2.y);
                    Card1.transform.localPosition = new Vector2(initPosCard1.x + (DeltaRotate), initPosCard1.y);
                    thumbTip.transform.eulerAngles = new Vector3(0, 0, eulerAngleThumb);
                }
                else
                {
                    if (DeltaRotate < 0)
                    {
                        OnEndDrag(eventData);
                        isSlide = false;
                    }

                }
            }
            else
            {
                float DeltaRotate2 = (delta / 10);
                if (Mathf.Abs(DeltaRotate2) <= 10)
                {
                    float eulerAngleThumb2 = initRotThumbTip2 + DeltaRotate;
                    float eulerAngleCardParent = DeltaRotate2;
                    float eulerAngle3 = initRotCard3 + DeltaRotate2;
                    Card3.transform.localPosition = new Vector2(initPosCard3.x + (DeltaRotate2), initPosCard3.y);
                    Card3.transform.eulerAngles = new Vector3(0, 0, eulerAngle3);
                    thumbTip.transform.eulerAngles = new Vector3(0, 0, eulerAngleThumb2);
                    Card1.transform.parent.transform.localEulerAngles = new Vector3(0, 0, -eulerAngleCardParent);
                }
                else
                {
                    if (DeltaRotate2 > 0)
                    {
                        OnEndDrag(eventData);
                        isSlide = false;
                    }

                }

            }

        }
        else
        {
            Debug.Log("------> Chua dc Slide<----");
            eventData.pointerDrag = null;
        }

    }
    public void show(List<int> arrCardID, Action cb, int timer)
    {
        if (GroupMenuView.instance != null && GroupMenuView.instance.gameObject.activeSelf)
        {
            GroupMenuView.instance.hide();
        }
        if (ChatIngameView.instance != null && ChatIngameView.instance.gameObject.activeSelf)
        {
            ChatIngameView.instance.hide();
        }
        animBanTay.gameObject.SetActive(true);
        isSlide = false;
        thumbbase.transform.localEulerAngles = new Vector3(0, 0, 180);
        thumbbase.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack);
        if (arrCardID.Count == 2)
        {
            Card1.setTextureWithCode(arrCardID[0]);
            Card2.setTextureWithCode(arrCardID[1]);
            BtnCancel.gameObject.SetActive(false);
            BtnDraw.gameObject.SetActive(false);
            BtnShowCard.gameObject.SetActive(true);
        }
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f).OnComplete(() =>
        {
            isSlide = true;
        });
        cbShowCard = cb;
        setTimer(timer);
    }
    private void setTimer(int timer)
    {
        lbTimer.text = timer.ToString();
        DOTween.Sequence().AppendInterval(1.0f)
            .AppendCallback(() =>
            {
                timer--;
                lbTimer.text = timer.ToString();
            }).SetLoops(timer);
    }
    public void takeCard(List<int> arrCardID, Action cb)
    {
        isSlide = false;
        totalCard = 3;
        DOTween.Sequence().AppendInterval(0.2f).AppendCallback(() =>
        {

            Card3.setTextureWithCode(0);
            Card3.gameObject.SetActive(true);
            Card3.transform.localPosition = new Vector2(initPosCard3.x, initPosCard3.y + Screen.height);
            Card3.transform.DOLocalMove(initPosCard3, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
            {
                Card3.setTextureWithCode(arrCardID[0]);
                isSlide = true;
            });
        });

        initRotThumbTip2 = thumbTip.transform.localEulerAngles.z + 6;
        thumbTip.transform.DOLocalRotate(new Vector3(0, 0, initRotThumbTip2), 0.3f);
        cbShowCard = cb;
        BtnShowCard.gameObject.SetActive(true);
    }
    public void hide()
    {
        isSlide = false;
        totalCard = 2;
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0, 0.3f);
        thumbbase.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Card3.gameObject.SetActive(false);
            Card1.transform.localEulerAngles = new Vector3(0, 0, initRotCard1);
            Card2.transform.localEulerAngles = new Vector3(0, 0, initRotCard2);
            Card3.transform.localEulerAngles = new Vector3(0, 0, initRotCard3);
            Card1.transform.localPosition = initPosCard1;
            Card2.transform.localPosition = initPosCard2;
            Card3.transform.localPosition = initPosCard3;
            thumbTip.transform.localEulerAngles = new Vector3(0, 0, initRotThumbTip);
            thumbTip.transform.localPosition = initPosThumb;
            Card1.transform.parent.transform.localEulerAngles = new Vector3(0, 0, initRotCardParent12);
        });
        if (cbShowCard != null)
        {
            cbShowCard.Invoke();
        }
        SocketSend.sendPlayingCardDone();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        beginPos = eventData.position;
        animBanTay.gameObject.SetActive(false);
    }
    public void onClickShowCard()
    {
        isSlide = false;
        if (totalCard == 2)
        {
            Card1.transform.DOLocalRotate(new Vector3(0, 0, initRotCard1 + 12), 0.3f);
            Card2.transform.DOLocalRotate(new Vector3(0, 0, initRotCard2 - 12), 0.3f);
            Card1.transform.DOLocalMoveX(initPosCard1.x - 12, 0.3f);
            Card2.transform.DOLocalMoveX(initPosCard2.x + 12, 0.3f);
            thumbTip.transform.DOLocalRotate(new Vector3(0, 0, initRotThumbTip - 12), 0.3f);
            BtnShowCard.gameObject.SetActive(false);
            BtnDraw.gameObject.SetActive(true);
            BtnCancel.gameObject.SetActive(true);
            if (cbShowCard != null)
            {
                cbShowCard.Invoke();
                cbShowCard = null;
            }
        }
        else
        {
            thumbTip.transform.localEulerAngles = new Vector3(0, 0, initRotThumbTip + 10);
            Card3.transform.DOLocalRotate(new Vector3(0, 0, initRotCard3 + 10), 0.3f);
            Card3.transform.DOLocalMoveX(initPosCard3.x - 10, 0.3f);
            Card1.transform.parent.transform.DOLocalRotate(new Vector3(0, 0, initRotCardParent12 - 10), 0.3f);
            thumbTip.transform.DOLocalRotate(new Vector3(0, 0, initRotThumbTip - 10), 0.3f);
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                hide();

            });
            BtnShowCard.gameObject.SetActive(false);
            SocketSend.sendActionTurn(0);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("keo xong.....");
        if (isSlide)
        {
            float delta = eventData.position.x - beginPos.x;
            float DeltaRotate = -(delta / 10);
            if (totalCard == 2)
            {
                if (Mathf.Abs(DeltaRotate) <= 9)
                {
                    Card1.transform.DOLocalRotate(new Vector3(0, 0, initRotCard1), 0.3f);
                    Card2.transform.DOLocalRotate(new Vector3(0, 0, initRotCard2), 0.3f);
                    Card1.transform.DOLocalMove(initPosCard1, 0.3f);
                    Card2.transform.DOLocalMove(initPosCard2, 0.3f);
                    thumbTip.transform.DOLocalRotate(new Vector3(0, 0, initRotThumbTip), 0.3f);
                }
                else
                {
                    BtnCancel.gameObject.SetActive(true);
                    BtnDraw.gameObject.SetActive(true);
                    BtnShowCard.gameObject.SetActive(false);
                    isSlide = false;
                    onClickShowCard();
                    if (cbShowCard != null)
                    {
                        Debug.Log("SHOw Card 2");
                        cbShowCard.Invoke();
                        cbShowCard = null;
                    }
                }
            }
            else
            {
                if (Mathf.Abs(DeltaRotate) <= 9)
                {
                    Card3.transform.DOLocalRotate(new Vector3(0, 0, initRotCard3), 0.3f);
                    Card3.transform.DOLocalMove(initPosCard3, 0.3f);
                    thumbTip.transform.DOLocalRotate(new Vector3(0, 0, initRotThumbTip2), 0.3f);
                    Card1.transform.parent.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    if (cbShowCard != null)
                    {
                        cbShowCard.Invoke();
                        cbShowCard = null;
                    }
                    eventData.pointerDrag = null;
                    DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
                    {
                        hide();
                    });
                }

            }
        }


    }
}
