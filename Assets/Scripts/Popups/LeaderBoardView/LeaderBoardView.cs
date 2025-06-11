using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System;
using Globals;

public class LeaderBoardView : BaseView
{
    public static LeaderBoardView instance;
    [SerializeField] GameObject itemTabGame, itemViewRank;
    [SerializeField] TextMeshProUGUI lbTimeRemain;
    [SerializeField] ScrollRect scrollView, scrTab;
    [SerializeField] ItemTopGame itemMe;

    List<TabView> listTab = new();
    JArray dataTop = new();
    TabView _CurrentTV = null;

    private long _timeRemain = -1;
    public long timeRemain   // property
    {
        get => _timeRemain;    // get method
        set
        {
            if (_timeRemain == -1)
            {
                _timeRemain = value;
            }
        }  // set method
    }

    protected override void Awake()
    {
        base.Awake();
        LeaderBoardView.instance = this;
    }
    protected override void Start()
    {
        base.Start();

        SocketIOManager.getInstance().emitSIOCCCNew(Config.formatStr("ClickShowTopGame_%s", CURRENT_VIEW.getCurrentSceneName()));
        CURRENT_VIEW.setCurView(CURRENT_VIEW.TOP_VIEW);
        //List<int> listGameID = new List<int>();

        //foreach (JObject data in Config.listRankGame)
        //{
        //    Logging.Log("Clmmm" + data["id"]);
        //    listGameID.Add((int)data["id"]);
        //}
        ////List<string> listGameName = new List<string>()
        ////{
        ////    "Roulette","Tongits","Hong Kong Poker","Hilo","Gounds Crab Fish","Bork Kdeng","Shan Koe Mee","13 Poker","Burmese Poker","Black Jack","Baccarat","Tongits 11","Lucky 9","Fruits Slot","Three Cards Poker","ShaBong","Sicbo","Dummy","Kaeng"
        ////};
        //int size = listGameID.Count;

        //for (int i = 0; i < size; i++)
        //{
        //    JObject dataGameDefined = new JObject();
        //    dataGameDefined["id"] = listGameID[i];
        //    listGameDefined.Add(dataGameDefined);
        //}
        genListTabGame();
    }
    int gameIDOpen = -1;
    public void openTabGameWithID(int gameID)
    {
        if (gameID <= 0) return;
        gameIDOpen = gameID;
    }

    public void onClickItemGame(TabView itemTab)
    {
        SoundManager.instance.soundClick();
        _CurrentTV = itemTab;
        SocketSend.getTopGamer((int)itemTab.data["id"], 0);
    }
    private void genListTabGame()
    {
        for (int i = 0; i < scrTab.content.childCount; i++) scrTab.content.GetChild(i).gameObject.SetActive(false);
        int indexSelect = 0;
        scrTab.gameObject.SetActive(true);
        List<int> listGameID = new();
        JArray listRankGame = new();
        foreach (JObject data in Config.listGame) listGameID.Add((int)data["id"]);
        foreach (JObject data in Config.listRankGame) if (listGameID.Contains((int)data["id"])) listRankGame.Add(data);
        Config.listRankGame = listRankGame;
        List<int> gameIds = new(){
                (int)GAMEID.PUSOY,(int)GAMEID.SLOTFRUIT,(int)GAMEID.SLOTINCA,(int)GAMEID.BACCARAT,
                (int)GAMEID.SICBO,(int)GAMEID.SLOTNOEL,(int)GAMEID.SLOTTARZAN,(int)GAMEID.SLOTJUICYGARDEN,
                (int)GAMEID.SLOTSIXIANG,
            };
        for (int i = 0; i < Config.listRankGame.Count; i++)
        {
            JObject itemData = (JObject)Config.listRankGame[i];
            int gameId = (int)itemData["id"];
            if (!gameIds.Contains(gameId)) continue;
            string name = getGameNameWithId(gameId);
            if (!name.Equals(""))
            {
                TabView itemTab;
                if (i < listTab.Count) itemTab = listTab[i];
                else
                {
                    itemTab = Instantiate(itemTabGame, scrTab.content).GetComponent<TabView>();

                    listTab.Add(itemTab);
                }
                itemTab.data = itemData;
                itemTab.gameObject.SetActive(true);
                itemTab.setInfo(name, onClickItemGame);
                if (gameId == gameIDOpen) indexSelect = i;
            }
        }
        if (indexSelect < listTab.Count) onClickItemGame(listTab[indexSelect]);
    }
    private string getGameNameWithId(int idGame)
    {
        return Config.getTextConfig(idGame.ToString());
    }

    public void reloadList(JArray listTopGamer)
    {
        for (var i = 0; i < listTab.Count; i++) listTab[i].SetActiveTab(_CurrentTV == listTab[i]);
        dataTop = listTopGamer;
        JToken dataMe = null;
        foreach (Transform tf in scrollView.content) tf.gameObject.SetActive(false);
        for (int i = 0; i < dataTop.Count; i++)
        {
            JToken dataItem = dataTop[i];
            Debug.Log(dataItem.ToString());
            if ((int)dataItem["Id"] == User.userMain.Userid && dataMe == null) dataMe = dataItem;
            if (i == dataTop.Count - 1) continue;
            GameObject itemView;
            if (i < scrollView.content.childCount) itemView = scrollView.content.GetChild(i).gameObject;
            else itemView = Instantiate(itemViewRank, scrollView.content);
            itemView.SetActive(true);
            itemView.GetComponent<ItemTopGame>().setInfo((JObject)dataItem);
        }
        scrollView.DOVerticalNormalizedPos(1.0f, 0.2f).SetEase(Ease.OutSine);
        if (dataMe != null)
        {
            itemMe.gameObject.SetActive(true);
            itemMe.setInfo((JObject)dataMe, true);
        }
    }

    public void setTimeRemain(long time)
    {
        timeRemain = time;
        Sequence s = DOTween.Sequence();
        s.AppendInterval(1.0f)
        .AppendCallback(() =>
        {
            countDownTime();
            if (timeRemain > 0)
            {
                setTimeRemain(timeRemain--);
            }
        });

    }
    private void countDownTime()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        long deltaTime = timeRemain - currentTime;
        string seconds = Math.Floor(((double)(deltaTime / 1000) % 60)) + "";
        string minutes = Math.Floor(((double)(deltaTime / 1000 / 60) % 60)) + "";
        string hours = Math.Floor(((double)(deltaTime / (1000 * 60 * 60)) % 24)) + "";
        string days = Math.Floor(((double)deltaTime / (1000 * 60 * 60 * 24))) + "";
        double dayNum = Math.Floor(((double)deltaTime / (1000 * 60 * 60 * 24)));
        if (hours.Length < 2) hours = "0" + hours;
        if (minutes.Length < 2) minutes = "0" + minutes;
        if (seconds.Length < 2) seconds = "0" + seconds;

        string time_ = days + (dayNum < 2 ? " " + Config.getTextConfig("txt_day") : " " + Config.getTextConfig("txt_day")) + ", " + hours + ":" + minutes + ":" + seconds;
        lbTimeRemain.text = time_;
    }
}
