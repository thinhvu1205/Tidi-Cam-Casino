using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class AlertShort : MonoBehaviour
{
    // Start is called before the first frame update
    public static AlertShort Instance = null;
    public List<GameObject> listData = new List<GameObject>();
    [SerializeField] TextMeshProUGUI lbNickName, lbContent;
    [SerializeField] Avatar avatar;
    [SerializeField] GameObject alertShortView, skeBg;
    [SerializeField] RectTransform rectTf;
    private float alvWidth = 320f;
    private bool isTweening = false;
    private enum POSITION
    {
        CLOCK_1H30, CLOCK_2H00, CLOCK_2H30, CLOCK_3H30, CLOCK_3H45, CLOCK_4H00,
        CLOCK_4H15, CLOCK_4H30, CLOCK_8H00, CLOCK_8H30, CLOCK_11H00
    };
    private POSITION posType = POSITION.CLOCK_4H30;

    void Start()
    {
        Instance = this;
    }
    private Vector2 getPositionByType()
    {
        Vector2 posAlert = Vector2.zero;
        posType = getTypePos();
        float offsetX = rectTf.rect.width / 2 + alvWidth / 2;
        switch (posType)
        {
            case POSITION.CLOCK_1H30: posAlert = new(offsetX, 300); break;
            case POSITION.CLOCK_2H00: posAlert = new(offsetX, 220); break;
            case POSITION.CLOCK_2H30: posAlert = new(offsetX, 200); break;
            case POSITION.CLOCK_3H30: posAlert = new(offsetX, -50); break;
            case POSITION.CLOCK_3H45: posAlert = new(offsetX, -200); break;
            case POSITION.CLOCK_4H00: posAlert = new(offsetX, -220); break;
            case POSITION.CLOCK_4H15: posAlert = new(offsetX, -250); break;
            case POSITION.CLOCK_4H30: posAlert = new(offsetX, -300); break;
            case POSITION.CLOCK_8H00: posAlert = new(-offsetX, -220); break;
            case POSITION.CLOCK_8H30: posAlert = new(-offsetX, -200); break;
            case POSITION.CLOCK_11H00: posAlert = new(-offsetX, 225); break;
        }
        return posAlert;
    }
    private POSITION getTypePos()
    {
        POSITION posType = POSITION.CLOCK_1H30;
        if (UIManager.instance.gameView != null && UIManager.instance.gameView.gameObject.activeSelf)
        {
            switch (Globals.Config.curGameId)
            {
                case (int)Globals.GAMEID.SLOT_SIXIANG:
                case (int)Globals.GAMEID.LUCKY9:
                    {
                        posType = POSITION.CLOCK_8H30;
                        break;
                    }
                case (int)Globals.GAMEID.SICBO:
                    {
                        posType = POSITION.CLOCK_4H00;
                        break;
                    }
                case (int)Globals.GAMEID.BACCARAT:
                    {
                        posType = POSITION.CLOCK_8H00;
                        break;
                    }
                case (int)Globals.GAMEID.BANDAR_QQ:
                case (int)Globals.GAMEID.RONGHO:
                    {
                        posType = POSITION.CLOCK_4H30;
                        break;
                    }
                case (int)Globals.GAMEID.KARTU_QIU:
                case (int)Globals.GAMEID.SLOTNOEL:
                case (int)Globals.GAMEID.SLOT_JUICY_GARDEN:
                case (int)Globals.GAMEID.SLOTTARZAN:
                case (int)Globals.GAMEID.SLOT20FRUIT:
                    {
                        posType = POSITION.CLOCK_2H00;
                        break;
                    }
                case (int)Globals.GAMEID.PUSOY:
                case (int)Globals.GAMEID.LUCKY_89:
                    {
                        posType = POSITION.CLOCK_4H15;
                        break;
                    }
                case (int)Globals.GAMEID.TONGITS:
                case (int)Globals.GAMEID.TONGITS_OLD:
                case (int)Globals.GAMEID.TONGITS_JOKER:
                    {
                        posType = POSITION.CLOCK_3H30;
                        break;
                    }
                case (int)Globals.GAMEID.SABONG:
                    {
                        posType = POSITION.CLOCK_3H45;
                        break;
                    }
                case (int)Globals.GAMEID.MINE_FINDING:
                    {
                        posType = POSITION.CLOCK_11H00;
                        break;
                    }
            }
        }
        else if (TableView.instance != null && TableView.instance.gameObject.activeSelf)
        {
            posType = POSITION.CLOCK_2H30;
        }
        return posType;
    }

    // Update is called once per frame

    void Update()
    {
        checkPosition();

    }
    public void updateChangeOrient()
    {
        alertShortView.SetActive(false);
        DOTween.Kill(alertShortView.transform);
        alertShortView.transform.localPosition = new Vector2(rectTf.rect.width / 2 + alvWidth, 0);
    }
    private void checkPosition()
    {
        if (alertShortView.gameObject.activeSelf)
        {
            if (UIManager.instance.isLoginShow())
            {
                alertShortView.SetActive(false);
                DOTween.Kill(alertShortView.transform);
                isTweening = false;
                return;
            }
            POSITION typeCurrentView = getTypePos();
            if (posType != typeCurrentView)
            {
                alertShortView.SetActive(false);
                DOTween.Kill(alertShortView.transform);
                isTweening = false;
                posType = typeCurrentView;
            }
        }
    }
    public async Task showShortMessage()
    {
        //vip0,vip1 show all(sảnh game,sảnh bàn,ingame)
        //>=vip2 show sảnh bàn,in game

        bool isShowAlert = true;
        if (Globals.User.userMain.VIP > 1 && UIManager.instance.lobbyView.getIsShow())
        {
            isShowAlert = false;
        }
        if (!isShowAlert || isTweening)
        {
            return;
        }
        if (Globals.Config.list_AlertShort.Count > 0)
        {
            JObject data = Globals.Config.list_AlertShort[0];
            Globals.Config.list_AlertShort.RemoveAt(0);
            lbNickName.text = (string)data["title"];
            lbContent.text = (string)data["content"];
            string urlAvt = (string)data["urlAvatar"];
            if (urlAvt.Contains("fb."))
            {
                await avatar.loadAvatarAsync2(0, urlAvt);
            }
            else
            {
                avatar.setSpriteWithID(int.Parse(urlAvt));
            }
            avatar.setVip((int)data["vip"]);
            alertShortView.gameObject.SetActive(true);
            Vector2 posStart = getPositionByType();

            alertShortView.transform.localPosition = posStart;
            Vector2 posTo = new Vector2(posStart.x > 0 ? posStart.x - alvWidth : posStart.x + alvWidth, posStart.y);
            skeBg.transform.localScale = posStart.x < 0 ? new Vector2(-1, 1) : new Vector2(1, 1);
            isTweening = true;
            DOTween.Sequence()
                  .Append(alertShortView.transform.DOLocalMove(posTo, 0.5f).SetEase(Ease.OutSine))
                  .AppendInterval(5.5f)
                  .Append(alertShortView.transform.DOLocalMove(posStart, 0.5f)
                  .SetEase(Ease.InSine))
                  .AppendCallback(() =>
                  {
                      alertShortView.gameObject.SetActive(false);
                      isTweening = false;
                      checkShowAlertShort();
                  }).SetTarget(alertShortView.transform);
        }
    }
    public async Task checkShowAlertShort()
    {
        if (Globals.Config.list_AlertShort.Count > 0)
        {
            await showShortMessage();
        }
    }


}
