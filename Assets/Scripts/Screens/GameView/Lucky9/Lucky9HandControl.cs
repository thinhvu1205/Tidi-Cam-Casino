using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using System.Linq;

public class Lucky9HandControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] GameObject handBase;
    [SerializeField] GameObject thumbBase;
    [SerializeField] GameObject thumbTip;
    [SerializeField] GameObject mask;
    [SerializeField] List<GameObject> listNodeCard;
    [SerializeField] Image timeCountDown;
    [SerializeField] GameObject timeRemain;
    [SerializeField] Player thisPlayer;
    Action<PointerEventData, GameObject> OnBeginDragCallback, OnDragCallback, OnEndDragCallback;

    public Lucky9View gameView;
    bool isShow;
    int timeLeft;

    private float BASEX = -305f;
    private float BASEY = 588f;

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragCallback != null)
        {
            OnBeginDragCallback.Invoke(eventData, mask);
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (OnDragCallback != null)
        {
            OnDragCallback.Invoke(eventData, mask);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragCallback != null)
        {
            OnEndDragCallback.Invoke(eventData, mask);
        }
    }
    public void setListenerDragDrop(System.Action<PointerEventData, GameObject> _OnBeginDrag, System.Action<PointerEventData, GameObject> _OnDrag, System.Action<PointerEventData, GameObject> _OnEndDrag)
    {
        OnBeginDragCallback = _OnBeginDrag;
        OnDragCallback = _OnDrag;
        OnEndDragCallback = _OnEndDrag;
    }

    public void removeAllListenerDragDrop()
    {
        OnBeginDragCallback = null;
        OnDragCallback = null;
        OnEndDragCallback = null;
    }

    public void onShow(List<int> arrC, int time)
    {
        isShow = true;
        if (arrC.Count == 0) return;
        handBase.transform.localRotation = Quaternion.Euler(0, 0, -70);
        StartCoroutine(FadeTo(mask, 220, 0.5f));

        for (int i = 0; i < arrC.Count; i++)
        {
            GameObject nodeCardParent = i == 0 ? listNodeCard[0] : listNodeCard[1];
            Card cardTemp;
            if (nodeCardParent.transform.childCount == 0)
            {
                cardTemp = gameView.spawnCard();
                cardTemp.transform.SetParent(nodeCardParent.transform);
            }
            else
            {
                cardTemp = nodeCardParent.transform.GetChild(0).GetComponent<Card>();
            }
            bool isCornner = i != 0;
            cardTemp.setTextureWithCode(arrC[i], false, true, isCornner);
            cardTemp.transform.localScale = new Vector3(1.9f, 2, 1);
            cardTemp.transform.localPosition = new Vector3(0, 0, 0);
            cardTemp.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        thumbTip.transform.localRotation = Quaternion.Euler(0, 0, 0);
        listNodeCard[0].transform.localPosition = new Vector2(BASEX, BASEY);
        listNodeCard[0].transform.localRotation = Quaternion.Euler(0, 0, 0);

        Quaternion newRotation = Quaternion.Euler(0, 0, 0);
        handBase.transform.DORotate(newRotation.eulerAngles, 0.5f).OnComplete(() =>
        {
            setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
        });
        timeLeft = time;
        countDown(time);

    }

    IEnumerator FadeTo(GameObject obj, float targetAlpha, float duration)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha / 255f, t);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha / 255f;
    }

    public void countDown(int time)
    {
        timeCountDown.gameObject.SetActive(true);
        timeCountDown.DOKill();
        timeRemain.transform.GetComponentInChildren<TextMeshProUGUI>().text = time + "";
        timeCountDown.gameObject.SetActive(true);
        timeCountDown.fillAmount = 1;
        timeCountDown.DOFillAmount(0, time);

        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            time--;
            timeRemain.transform.GetComponentInChildren<TextMeshProUGUI>().text = time + "";
        }).SetLoops(time).OnComplete(() =>
        {
            timeCountDown.gameObject.SetActive(false);
            if (gameObject.activeSelf)
                gameView.onHideFoldCard(timeLeft);
            gameObject.SetActive(false);
        });
    }

    void OnBeginDrag(PointerEventData eventData, GameObject handControl)
    {
        Debug.Log("OnBeginDrag");
        thumbTip.GetComponent<MonoBehaviour>().StopAllCoroutines();
    }

    void OnDrag(PointerEventData eventData, GameObject handControl)
    {
        Vector2 delta = eventData.delta;
        Debug.Log("OnDrag = " + delta);

        float minRotation = -20.0f;
        float maxRotation = 30.0f;

        float deltaX = delta.x / 10;
        float betaX = listNodeCard[0].transform.localPosition.x + delta.x / -5;
        float betaY = listNodeCard[0].transform.localPosition.y + delta.y / -10;

        if (betaX < BASEX - 70) betaX = BASEX - 70;
        if (betaX > BASEX + 40) betaX = BASEX + 40;
        if (betaY < BASEY - 35) betaY = BASEY - 35;
        if (betaY > BASEY + 20) betaY = BASEY + 20;


        Vector3 currentRotation = thumbTip.transform.eulerAngles;

        float currentZRotation = currentRotation.z;
        if (currentZRotation > 180)
        {
            currentZRotation -= 360;
        }

        float newZRotation = Mathf.Clamp(currentZRotation - deltaX, minRotation, maxRotation);

        thumbTip.transform.eulerAngles = new Vector3(0, 0, newZRotation);
        listNodeCard[0].transform.eulerAngles = new Vector3(0, 0, -newZRotation/3);
        listNodeCard[0].transform.localPosition = new Vector2(betaX, betaY);
    }

    void OnEndDrag(PointerEventData eventData, GameObject handControl)
    {
        Debug.Log("OnEndDrag");
        onRevealCard();
    }

    async void onRevealCard()
    {
        var rot = thumbTip.transform.eulerAngles.z;
        if (rot > 15 && rot < 40 || rot < 353 && rot > 335)
        {
            var cardTemp = listNodeCard[0].transform.GetChild(0).GetComponent<Card>();
            cardTemp.showShanCorner(true, 0.6f);
            removeAllListenerDragDrop();
            thumbTip.transform.DORotate(new Vector3(0, 0, -15), 0.6f).SetEase(Ease.InOutCubic);
            listNodeCard[0].transform.DOLocalMove(new Vector2(BASEX - 30, BASEY - 15), 0.6f).SetEase(Ease.InOutCubic);
            listNodeCard[0].transform.DORotate(new Vector3(0, 0, 5), 0.6f).SetEase(Ease.InOutCubic);
            await Task.Delay(1000);
            onHide();
        }
        else
        {
            thumbTip.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.InOutCubic);
            listNodeCard[0].transform.DOLocalMove(new Vector2(BASEX, BASEY), 0.2f).SetEase(Ease.InOutCubic);
            listNodeCard[0].transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.InOutCubic);
        }
    }

    public async void onHide()
    {
        if (isShow == false) return;
        isShow = false;

        removeAllListenerDragDrop();

        mask.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        handBase.transform.DORotate(new Vector3(0, 0, -70), 0.5f).SetEase(Ease.InBack);
        timeCountDown.gameObject.SetActive(false);

        gameObject.transform.DOKill();
        await Task.Delay(500);
        gameObject.SetActive(false);
        gameView.onHideFoldCard(timeLeft);
    }
}
