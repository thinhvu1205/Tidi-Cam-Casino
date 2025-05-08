using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;

public class AlertMessage : MonoBehaviour
{
    public static AlertMessage instance;
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI lbAlert;

    //[SerializeField] RectTransform CenterNode;
    [SerializeField]
    Transform PopupContainer;

    [SerializeField]
    Transform PopupTabContainer;

    [SerializeField]
    RectTransform rectTf;
    [SerializeField]
    RectTransform rectTfParent;

    [SerializeField]
    Image bgAlert;

    private List<JObject> listData = new List<JObject>();
    private bool isRunning = false;
    private Vector2 posInView;
    private Vector2 sizeBg;
    private Rect parentRect;
    void Update()
    {
        //checkPosition();
    }
    void Awake()
    {
        instance = this;
        rectTfParent = transform.parent.GetComponent<RectTransform>();
        parentRect = rectTfParent.rect;
        lbAlert.transform.localPosition = new Vector2(parentRect.width / 2, 17);
    }

    // Update is called once per frame
    public void addAlertMessage(JObject data)
    {

        listData.Add(data);
        if (!isRunning)
        {
            showAlertMessage();
        }
    }
    //Guid uid_action;
    public void showAlertMessage()
    {

        if (listData.Count > 0 && !UIManager.instance.isLoginShow())
        {
            if (UIManager.instance.gameView == null)
            {
                if (!gameObject.activeSelf)
                {
                    UIManager.instance.showAlert(true);
                }
            }
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            isRunning = true;
            JObject data = listData[0];
            listData.RemoveAt(0);
            Globals.Config.list_Alert.Remove(data);
            lbAlert.text = (string)data["data"];
            Vector2 posEnd = Vector2.zero;
            if (transform.localEulerAngles.z == 0)
            {
                lbAlert.transform.localPosition = new Vector2(parentRect.width / 2, 17);
                posEnd = new Vector2(-parentRect.width / 2 - lbAlert.preferredWidth, 17);
            }
            else
            {
                lbAlert.transform.localPosition = new Vector2(parentRect.height / 2 + 17, 17);
                posEnd = new Vector2(-parentRect.height / 2 - lbAlert.preferredWidth, 17);
            }
            lbAlert.transform.DOLocalMoveX(posEnd.x, 12.5f).OnComplete(() =>
            {
                isRunning = false;
                DOTween.Sequence().AppendInterval(0.5f).AppendCallback(() =>
                {
                    showAlertMessage();
                });

            });
        }
        else
        {
            DOTween.Kill(lbAlert.transform);
            isRunning = false;
            UIManager.instance.showAlert(false);
            gameObject.SetActive(false);
        }
        //checkPosition();
    }
    private void checkPosition()
    {
        if (UIManager.instance.isLoginShow())
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            if (listData.Count > 0)
            {
                gameObject.SetActive(true);
            }
        }

        //if (PopupContainer.childCount > 0 || PopupTabContainer.childCount > 0)
        //{
        posInView = new Vector2(0, rectTfParent.rect.height / 2 - 15);
        sizeBg = new Vector2(parentRect.width, 60);
        lbAlert.fontSize = 25;
        transform.localEulerAngles = Vector3.zero;
        //}
        //else
        //{
        //    if (UIManager.instance.gameView != null)
        //    {
        //        lbAlert.fontSize = 25;
        //        //bgAlert.enabled = false;
        //        if (Globals.Config.curGameId == (int)Globals.GAMEID.DUMMY || Globals.Config.curGameId == (int)Globals.GAMEID.KEANG || Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
        //        {
        //            posInView = new Vector2(rectTfParent.rect.width / 2 - 20, 0);
        //            transform.localEulerAngles = new Vector3(0, 0, 270);
        //            sizeBg = new Vector2(parentRect.height, 60);
        //        }
        //        else
        //        {
        //            posInView = new Vector2(0, rectTfParent.rect.height / 2 - 20);
        //            transform.localEulerAngles = Vector3.zero;
        //            sizeBg = new Vector2(parentRect.width, 60);
        //        }
        //    }
        //    else
        //    {
        //        posInView = new Vector2(0, rectTfParent.rect.height / 2 - 150);
        //        transform.localEulerAngles = Vector3.zero;
        //        sizeBg = new Vector2(parentRect.width, 60);
        //        lbAlert.fontSize = 35;
        //    }
        //    //Debug.Log("position in view=" + posInView.y);
        //}
        //if (TableView.instance != null)
        //{
        //    if (Globals.Config.curGameId == (int)Globals.GAMEID.KEANG || Globals.Config.curGameId == (int)Globals.GAMEID.DUMMY || Globals.Config.curGameId == (int)Globals.GAMEID.SICBO)
        //    {
        //        posInView = new Vector2(rectTfParent.rect.width / 2 - 15, 0);
        //        transform.localEulerAngles = new Vector3(0, 0, 270);
        //        sizeBg = new Vector2(parentRect.height, 60);
        //    }
        //    else
        //    {
        //        posInView = new Vector2(0, rectTfParent.rect.height / 2 - 20);
        //        transform.localEulerAngles = Vector3.zero;
        //        sizeBg = new Vector2(parentRect.width, 60);
        //    }
        //    if (ShopView.instance != null)
        //    {
        //        posInView = new Vector2(0, rectTfParent.rect.height / 2 - 20);
        //        transform.localEulerAngles = Vector3.zero;
        //        sizeBg = new Vector2(parentRect.width, 60);
        //    }

        //}
        transform.localPosition = posInView;
        rectTf.sizeDelta = sizeBg;
    }

}
