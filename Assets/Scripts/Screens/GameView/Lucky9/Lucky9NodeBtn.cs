using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

public class Lucky9NodeBtn : MonoBehaviour
{
    [SerializeField] Button btn_hit;
    [SerializeField] Button btn_no;
    [SerializeField] bool isShow = false;

    public async void onShow(float timeExist = 0)
    {
        Debug.Log($"!==> show button with time exist = {timeExist}");
        isShow = true;
        gameObject.SetActive(true);
        float height = transform.parent.Find("test").localPosition.y;
        transform.localPosition = new Vector3(0, height - 50, 0);
        transform.DOKill();
        transform.DOLocalMoveY(height + 60, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            btn_hit.interactable = true;
            btn_no.interactable = true;
        });

        if (timeExist > 0)
        {
            StartCoroutine(waitOnHide(timeExist));
        }
    }

    private IEnumerator waitOnHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isShow && gameObject != null)
        {
            onHide();
        }
    }

    public void onHide()
    {
        Debug.Log("!==> on hide button");
        if (!isShow) return;
        isShow = false;
        btn_hit.interactable = false;
        btn_no.interactable = false;
        if (!gameObject.activeSelf) return;
        transform.DOKill();
        float height = transform.parent.Find("test").localPosition.y;
        transform.DOLocalMoveY(height - 50, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void OnClickBtn(int type)
    {
        SocketSend.sendReiveCard(type);
        onHide();
    }
}
