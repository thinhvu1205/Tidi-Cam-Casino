using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Spine.Unity;
using Globals;

public class DailyBonusView : BaseView
{
    public static DailyBonusView instance;
    // Start is called before the first frame update

    [SerializeField]
    public List<GameObject> listNodeStreak = new List<GameObject>();

    //[SerializeField]
    //public List<Sprite> listBgStreak = new List<Sprite>();

    [SerializeField]
    public List<DailyBonusCollumSpin> listCollum = new List<DailyBonusCollumSpin>();

    [SerializeField]
    public SkeletonGraphic nodeStreakBonus;

    public Text lbBasic, lbVipBonus, lbTotalReward;

    [SerializeField]
    Button btnSpin;

    [HideInInspector]
    List<List<int>> listDefine;
    List<List<int>> listbar;
    List<List<List<int>>> listResult;

    private int currentIndexStop = -1;
    private bool is_sendpromotion = false;

    private bool isCollect = false;

    [SerializeField]
    Image coinEffectPrefab;

    [SerializeField]
    Transform tf_from, tf_to;
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    protected override void Start()
    {
        base.Start();

        listDefine = new List<List<int>>() {
           new List<int>() {10, 10, 10},
            new List<int>() {0, 0, 0 },
             new List<int>() {9, 9, 9 },
             new List<int>() {4, 4, 4},
             new List<int>() {5, 5, 5},
             new List<int>() {7, 7, 7},
             new List<int>() {3, 3, 3},
             new List<int>() {3, 4, 3},
             new List<int>() {3, 5, 3},
             new List<int>() {3, 7, 3},
             new List<int>() {3, 6, 3},
             new List<int>() {2, 2, 2},
             new List<int>() {2, 4, 2 },
             new List<int>() {2, 5, 2},
             new List<int>() {2, 7, 2},
             new List<int>() {2, 6, 2},
             new List<int>() {1, 1, 1},
             new List<int>() {1, 4, 1},
             new List<int>() {1, 5, 1},
             new List<int>() {1, 7, 1},
             new List<int>() {1, 6, 1},
             new List<int>() {1, 7, 1},
             new List<int>() {1, -1, -1},
             new List<int>() {8, 8, 8},
             new List<int>() {8, 8, -1},
             new List<int>() {8, -1, -1},
        };
        listbar = new List<List<int>>();
        for (int i = 1; i <= 3; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                for (int k = 1; k <= 3; k++)
                {
                    if (i == j && j == k)
                    {
                        continue;
                    }
                    List<int> item = new List<int>();
                    item.Add(i);
                    item.Add(j);
                    item.Add(k);
                    listbar.Add(item);
                }
            }
        }
        listDefine[22] = listbar[getRanNum(0, 24)];
        listResult = new List<List<List<int>>>();
        for (int i = 0; i < listDefine.Count; i++)
        {
            List<List<int>> randResutl = new List<List<int>>();
            for (int j = 0; j < 3; j++)
            {
                List<int> ranItem = new List<int>();
                ranItem.Add(getRanNum(0, 11));
                ranItem.Add(-1);
                ranItem.Add(getRanNum(0, 11));
                if (listDefine[i][j] == -1) //random id
                    listDefine[i][j] = getRanNum(0, 11, listDefine[i][0]);
                ranItem[1] = listDefine[i][j];
                randResutl.Add(ranItem);
            }
            listResult.Add(randResutl);
        }


        btnSpin.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.getTextConfig("txt_SPIN");
    }
    private int getRanNum(int min_value, int max_value, int numExclude = -1)
    {
        int ranNum = 0;
        int random_number = Random.Range(min_value, max_value);
        ranNum = random_number;
        if (numExclude != -1)
        {
            if (ranNum == numExclude)
                return getRanNum(min_value, max_value, numExclude);
        }
        return ranNum;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void reloadHistory()
    {

    }
    public void onClickSpin()
    {
        //      {
        //          "evt": "promotion_online_2",
        //"result": "{\"result\":26,\"vip\":2,\"turn\":1,\"baseAg\":1000,\"vipBonus\":20,\"turnBonus\":10,\"AG\":1300}"
        //}
        SoundManager.instance.soundClick();
        btnSpin.interactable = false;
        btnSpin.transform.Find("anim").gameObject.SetActive(false);
        if (isCollect)
        {
            SocketSend.sendPromotion();
            SocketSend.sendUAG();
            coinFly(tf_from, tf_to);
        }
        else if (!is_sendpromotion)
        {
            for (int i = 0; i < listCollum.Count; i++)
            {
                listCollum[i].startSpin();
            }
            SocketSend.sendGetDataSpin();
        }
    }
    public void nextColStop()
    {
        if (currentIndexStop < 2)
        {
            currentIndexStop++;
            listCollum[currentIndexStop].isStop = true;
        }
    }
    public void setInfo()
    {
        lbBasic.gameObject.SetActive(false);
        lbVipBonus.gameObject.SetActive(false);
        lbTotalReward.gameObject.SetActive(false);
        int onlineCurrent = Globals.Promotion.onlineCurrent;
        for (int i = 0; i < listNodeStreak.Count; i++)
        {
            GameObject itemStreak = listNodeStreak[i];

            //Image bgStreak = itemStreak.transform.Find("bgStreak").GetComponent<Image>();
            //if (i == onlineCurrent)
            //{
            //    bgStreak.sprite = listBgStreak[1];
            //}
            //else
            //{
            //    bgStreak.sprite = listBgStreak[0];
            //}

            SkeletonGraphic animStreak = itemStreak.transform.Find("anim").GetComponent<SkeletonGraphic>();
            animStreak.AnimationState.SetAnimation(0, i == onlineCurrent ? "on" : "off", true);
        }

        btnSpin.interactable = Globals.Promotion.time <= 0;
        btnSpin.transform.Find("anim").gameObject.SetActive(Globals.Promotion.time <= 0);

        if (Globals.Promotion.onlineCurrent == 5)
        {
            nodeStreakBonus.AnimationState.SetAnimation(0, Globals.Config.language == "EN" ? "bonuson" : "bonuson_thai", true);
        }
        else
        {
            nodeStreakBonus.AnimationState.SetAnimation(0, Globals.Config.language == "EN" ? "bonusoff" : "bonusoff_thai", true);
        }

    }
    public void handleResult(JObject data)
    {
        //{\"result\":26,\"vip\":3,\"turn\":1,\"baseAg\":1000,\"vipBonus\":30,\"turnBonus\":10,\"AG\":1400}"
        Globals.Logging.Log("handle Result");
        int arrID = (int)data["result"];
        lbBasic.text = Globals.Config.FormatMoney((int)data["baseAg"]);
        lbVipBonus.text = Globals.Config.FormatMoney((int)data["vipBonus"]);
        lbTotalReward.text = Globals.Config.FormatMoney((int)data["AG"]);
        List<List<int>> arrResult = listResult[arrID - 1];
        for (int i = 0; i < arrResult.Count; i++)
        {
            listCollum[i].setFinishView(arrResult[i]);
        }
        currentIndexStop = 0;
        listCollum[0].isStop = true;
    }
    public void effResult()
    {
        Sequence s = DOTween.Sequence();
        float duration = 0.75f;

        lbBasic.transform.localScale = new Vector2(5, 5);
        lbVipBonus.transform.localScale = new Vector2(5, 5);
        lbTotalReward.transform.localScale = new Vector2(5, 5);
        Color tempColor = lbBasic.color;
        tempColor.a = 0f;
        lbBasic.color = tempColor;
        lbVipBonus.color = tempColor;
        lbTotalReward.color = tempColor;
        lbBasic.rectTransform.localRotation = Quaternion.Euler(0, 0, -360);
        lbVipBonus.rectTransform.localRotation = Quaternion.Euler(0, 0, -360);
        lbTotalReward.rectTransform.localRotation = Quaternion.Euler(0, 0, -360);

        System.Action<Text> lbActEff = (Text lbGameObj) =>
        {
            lbGameObj.DOFade(1f, duration).SetAutoKill(true);
            lbGameObj.transform.DOScale(1, duration).SetEase(Ease.InSine);
            lbGameObj.transform.DORotate(new Vector3(0, 0, 360), duration).SetRelative();
        };
        s.AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                lbBasic.gameObject.SetActive(true);
                lbActEff(lbBasic);
            });
        s.AppendInterval(duration)
          .AppendCallback(() =>
          {
              lbVipBonus.gameObject.SetActive(true);
              lbActEff(lbVipBonus);
          });
        s.AppendInterval(duration + 0.25f)
         .AppendCallback(() =>
         {
             btnSpin.interactable = true;
             btnSpin.transform.Find("anim").gameObject.SetActive(true);
             isCollect = true;
             lbTotalReward.gameObject.SetActive(true);
             lbActEff(lbTotalReward);
             btnSpin.GetComponentInChildren<TextMeshProUGUI>().text = Globals.Config.getTextConfig("receive");
         });
    }
    public void spinFinish()
    {
        effResult();
    }

    void coinFly(Transform transFrom, Transform transTo, int count = 10, float timeInterval = 0.1f)
    {
        for (var i = 0; i < 10; i++)
        {
            DOTween.Sequence().AppendInterval(i * 0.1f).AppendCallback(() =>
            {
                var obj = Instantiate(coinEffectPrefab, transform).GetComponent<Image>();
                obj.gameObject.SetActive(true);
                effectCoinFly(obj, transFrom, transTo);

            });

        }
    }

    void effectCoinFly(Image coinEffect, Transform transFrom, Transform transTo)
    {
        coinEffect.transform.position = transFrom.position;
        coinEffect.transform.DOJump(transTo.position, 1, 1, 2).SetEase(Ease.InOutCubic);
        var cc = coinEffect.color;
        cc.a = .2f;
        coinEffect.color = cc;
        coinEffect.DOFade(1, .75f);

        DOTween.Sequence()
            .AppendInterval(.5f)
            .Append(coinEffect.transform.DOScale(2, 0.25f))
            .AppendInterval(0.15f)
            .Append(coinEffect.transform.DOScale(1, 0.25f))//0.85
            .AppendInterval(0.6f)
            .Append(coinEffect.DOFade(0, .25f)).AppendCallback(() =>
            {
                //coinEffect.gameObject.SetActive(false);
                Destroy(coinEffect.gameObject);
            });
    }
}
