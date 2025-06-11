using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Globals;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using Facebook.Unity;

public class GameView : BaseView
{
    public List<Player> players = new List<Player>();
    [SerializeField]
    protected List<Vector2> listPosView = new List<Vector2>();

    public Player thisPlayer;

    [SerializeField]
    protected PlayerView playerViewPrefab;
    [SerializeField]
    TextMeshProUGUI lbInfo;

    [SerializeField]
    GameObject invitePrefab;
    [SerializeField]
    Transform inviteContainer;
    [SerializeField]
    public Transform playerContainer;
    [SerializeField]
    TextMeshProUGUI txtGameName;
    [SerializeField] private Transform m_HiddenPlayersTf;


    public int agTable;
    public int maxbet = 0;

    protected List<Card> cardPool = new List<Card>();
    protected List<JObject> listDelayEvt = new List<JObject>();
    protected List<Card> cardsOnTable = new List<Card>();
    protected List<ChipBet> chipPool = new List<ChipBet>();
    protected List<GameObject> listBtnInvite = new List<GameObject>();
    public List<string> delayEvents = new List<string>();
    public STATE_GAME stateGame = STATE_GAME.WAITING;
    [HideInInspector]
    public JObject dataLeave;
    [HideInInspector]
    public string soundBg = SOUND_GAME.IN_GAME_COMMON;
    public int GetCountListPosView() { return listPosView.Count; }
    protected override void Awake()
    {
        base.Awake();
        Config.isBackGame = false;
        HandleGame.listDelayEvt.Clear();
    }
    protected override void Start()
    {

        base.Start();
        soundBg = SOUND_GAME.IN_GAME_COMMON;
        if (Config.curGameId == (int)GAMEID.SLOTNOEL) soundBg = SOUND_SLOT.BG_NOEL;
        else if (Config.curGameId == (int)GAMEID.SLOTTARZAN) soundBg = SOUND_SLOT.BG_TARZAN;
        else if (Config.curGameId == (int)GAMEID.SLOTJUICYGARDEN) soundBg = SOUND_SLOT.BG_JUICY_GARDEN;
        else if (Config.curGameId == (int)GAMEID.SLOTSIXIANG) soundBg = SOUND_SLOT_BASE.BG_GAME;
        SoundManager.instance.playMusicInGame(soundBg);
        for (var i = 0; i < listPosView.Count; i++)
        {
            if (i == 0) continue;
            GameObject btnInvite;
            btnInvite = Instantiate(invitePrefab, inviteContainer == null ? transform : inviteContainer);
            btnInvite.transform.localScale = Vector3.one;
            btnInvite.transform.localPosition = listPosView[i];
            btnInvite.GetComponent<Button>().onClick.AddListener(() => { onClickInvite(); });
            listBtnInvite.Add(btnInvite);
        }
        if (txtGameName != null) txtGameName.text = Config.getTextConfig(Config.curGameId.ToString());
    }

    // Update is called once per frame

    public virtual void onClickRule()
    {

    }
    public override void OnDestroy()
    {
        Logging.Log("-=-=OnDestroy ");
        Config.lastGameIDSave = Config.curGameId;
        UIManager.instance.lobbyView.setQuickPlayGame(Config.lastGameIDSave);
        User.userMain.lastGameID = 0;
        foreach (var c in cardPool)
        {
            Destroy(c.gameObject);
        }
        cardPool.Clear();
        //foreach (var c in chipPool)
        //{
        //    Destroy(c.gameObject);
        //}
        chipPool.ForEach(chip =>
        {
            Destroy(chip.gameObject);
        });
        if (WebViewControl.instance != null)
        {
            WebViewControl.instance.closeWebView();
        }
        UIManager.instance.destroyAllPopup();
        chipPool.Clear();
        HandleGame.listDelayEvt.Clear();
        SoundManager.instance.playMusic();
        SoundManager.instance.stopAllCurrentEffect();
        SocketSend.sendUAG();
        SocketSend.getInfoSafe();
        //if (!Globals.Config.listGamePlaynow.Contains(Globals.Config.curGameId)) //game ko phai playnow thi back ra tableview moi get farminfo con game playnow mac dinh lobbyview da send roi
        //{
        //}
        base.OnDestroy();
    }

    public void onClickInvite()
    {
        SoundManager.instance.soundClick();
        var subView = Instantiate(UIManager.instance.loadPrefabPopup("PopupInvite"), transform).GetComponent<InviteView>();
        subView.setAgTable(agTable);
        subView.transform.localScale = Vector3.one;
        subView.transform.SetAsLastSibling();

    }

    public virtual void onClickBack()
    {
        SoundManager.instance.soundClick();
        var subView = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/GroupMenu"), transform);
        subView.transform.localScale = Vector3.one;
    }
    public void onClickChat(string isChatText)
    {
        SoundManager.instance.soundClick();
        if (stateGame == STATE_GAME.VIEWING) return;
        var subView = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/ChatInGame"), transform).GetComponent<ChatIngameView>();
        subView.transform.localScale = Vector3.one;
        subView.transform.SetAsLastSibling();

        subView.onClickTab(isChatText);
    }


    public void onClickInfoPlayer(Player player, bool showActionButtons = true)
    {
        SoundManager.instance.soundClick();

        if (stateGame == STATE_GAME.VIEWING && Config.curGameId != (int)GAMEID.SICBO) return;
        var subView = Instantiate(UIManager.instance.loadPrefab("GameView/Objects/InfoPlayerInGame"), transform).GetComponent<InfoPlayerInGame>();
        subView.transform.localScale = Vector3.one;
        subView.transform.SetAsLastSibling();


        subView.setInfo(player, showActionButtons);
    }

    public void onClickShareScreen()
    {
        SoundManager.instance.soundClick();
        //StartCoroutine(Config.ShareImageShot());
    }
    public void checkAutoExit()
    {
        Globals.Logging.Log("Check Auto Exit:" + Config.isBackGame);
        if (Config.isBackGame)
        {
            SocketSend.sendExitGame();
        }
    }
    /*Handle Game*/
    public void handleChatTable(JObject data)
    {
        //cc.NGWlog('-=-=--=-=->1 chattable', data);
        var datName = (string)data["Name"];
        var datNName = (string)data["NName"];
        var datMSG = ((string)data["Data"]).Trim();
        var datType = (string)data["T"];
        //// client sua chattable
        if (datMSG.Trim().Equals("") && datType.Equals(""))
        {
            return;
        }
        var player = getPlayer(datName);
        if (player != null)
        {
            if (datType.Contains("*f") && !datNName.Equals(""))
            {
                var npl = getPlayer(datNName);
                if (npl != null)
                {
                    if (player.playerView == null && m_HiddenPlayersTf == null) return;
                    var idAnimation = int.Parse(string.Join("", datType.ToCharArray().Where(Char.IsDigit)));
                    var iteChat = Instantiate(BundleHandler.LoadGameObject("GameView/Objects/ItemChatAction") as GameObject, transform).GetComponent<ItemChatAction>();
                    if (player.playerView != null) iteChat.transform.position = player.playerView.transform.position;
                    else iteChat.transform.position = m_HiddenPlayersTf.position;
                    if (npl.playerView != null)
                    {
                        var posTo = npl.playerView.transform.position;
                        StartCoroutine(iteChat.setData(idAnimation, posTo));
                    }
                    else Destroy(iteChat.gameObject);
                    if (UIManager.instance.SendChatEmoToHiddenPlayers && m_HiddenPlayersTf != null)
                    {
                        UIManager.instance.SendChatEmoToHiddenPlayers = false;
                        iteChat = Instantiate(BundleHandler.LoadGameObject("GameView/Objects/ItemChatAction") as GameObject, transform).GetComponent<ItemChatAction>();
                        iteChat.transform.position = player.playerView.transform.position;
                        StartCoroutine(iteChat.setData(idAnimation, m_HiddenPlayersTf.position));
                    }
                }
            }
            else
            {
                var iteChat = Instantiate(BundleHandler.LoadGameObject("GameView/Objects/ItemChat") as GameObject, transform).GetComponent<ItemChatInGame>();
                iteChat.setMsg(datMSG, datType, player.playerView);
            }
        }

    }

    public virtual void handleCCTable(JObject data)
    {
        stateGame = STATE_GAME.WAITING;
        var namePl = (string)data["Name"];
        var player = getPlayer(namePl);
        if (player == null)
            return;

        players.ForEach(pl => pl.setHost(pl == player));
    }

    public virtual void handleLTable(JObject data)
    {
        var namePl = (string)data["Name"];
        var player = getPlayer(namePl);
        if (player == null) return;
        //var msg = "";
        //      if (data.ContainsKey("message"))
        //      {
        //          msg = (string)data["message"];
        //      }
        //if (name != User.userMain.UserName)
        if (player != thisPlayer)
        {
            //cc.NGWlog('nhảy vào đay rồi== ham lbtable')
            // require('SoundManager').playSound(ResDefine.sound_remove);
            //if (typeof player.displayName !== 'undefined')
            //{
            //    addChatLeave(player.displayName);
            //}
            removePlayer(namePl);
        }
        else
        {
            //DestroyImmediate(gameObject);
            //if (TableView.instance)
            //{
            //    UIManager.instance.openTableView();
            //}
            //else
            //{
            //    UIManager.instance.showLobbyScreen(false);
            //}
        }
    }
    public virtual void handleJTable(string strData)
    {
        var listPlayer = JObject.Parse(strData);
        var player = new Player();
        players.Add(player);
        player.playerView = createPlayerView();
        //player.playerView.transform.localScale = Vector3.one;
        readDataPlayer(player, listPlayer);
        player.updatePlayerView();
        //addChatJoin(player.displayName);
        updatePositionPlayerView();
    }
    public virtual void handleVTable(string strData)
    {
        stateGame = STATE_GAME.VIEWING;
        var data = JObject.Parse(strData);
        if (data.ContainsKey("maxBet"))
            setGameInfo((int)data["M"], (int)data["Id"], (int)data["maxBet"]);
        else
        {
            setGameInfo((int)data["M"], (int)data["Id"]);
        }
        var listPlayer = (JArray)data["ArrP"];
        players.Clear();

        bool isHasThis = false;
        for (var i = 0; i < listPlayer.Count; i++)
        {
            var pl = new Player();
            players.Add(pl);

            pl.playerView = createPlayerView();
            //pl.playerView.transform.localScale = Vector3.one;
            readDataPlayer(pl, (JObject)listPlayer[i]);
            if (i == 0)
                pl.setHost(true);
            if (pl.id == User.userMain.Userid)
            {
                isHasThis = true;
                thisPlayer = pl;
            }
            pl.updatePlayerView();
        }
        //Init thisPlayer
        if (!isHasThis)
        {
            var player = new Player();
            player.playerView = createPlayerView();
            //player.playerView.transform.localScale = Vector3.one;
            player.id = User.userMain.Userid;
            if (User.userMain.Tinyurl.IndexOf("fb.") != -1)
            {
                player.fid = User.userMain.Tinyurl.Substring(3);
            }
            player.namePl = User.userMain.Username;
            player.displayName = User.userMain.displayName;
            player.ag = User.userMain.AG;
            player.vip = User.userMain.VIP;
            player.avatar_id = User.userMain.Avatar;
            player.is_ready = true;
            //player.ip = '0.0.0.0';
            //player.keyObjectInGame = User.userMain.keyObjectInGame;
            player.updatePlayerView();
            thisPlayer = player;
            players.Add(player);
        }
        updatePositionPlayerView();
        thisPlayer.playerView.setDark(true);
    }

    public virtual void handleCTable(string strData)
    {
        Debug.Log("handleCCTable gameView");
        //// { "evt": "ctable", "data": "{\"N\":\"Poker[1732]\",\"M\":100,\"ArrP\":[{\"id\":100425492,\"N\":\"sondt123789\",\"Url\":\"sondt123789\",\"AG\":101480,\"LQ\":0,\"VIP\":1,\"isStart\":true,\"IK\":0,\"sIP\":\"116.96.80.111\",\"G\":3,\"Av\":5,\"FId\":0,\"GId\":0,\"UserType\":1,\"TotalAG\":0,\"timeToStart\":0}],\"Id\":1732,\"V\":0,\"S\":5,\"issd\":true}" }
        Globals.Logging.Log("Hanlde Ctable");
        var data = JObject.Parse(strData);
        stateGame = STATE_GAME.WAITING;
        if (data.ContainsKey("maxBet"))
            setGameInfo((int)data["M"], (int)data["Id"], (int)data["maxBet"]);
        else
        {
            setGameInfo((int)data["M"], (int)data["Id"]);
        }

        var listPlayer = (JObject)((JArray)data["ArrP"])[0];
        thisPlayer = new Player();
        players.Add(thisPlayer);
        thisPlayer.playerView = createPlayerView();
        //thisPlayer.playerView.transform.parent = transform;
        //thisPlayer.playerView.transform.localScale = Vector3.one;
        //node.addChild(thisPlayer._playerView.node, GAME_ZORDER.Z_PLAYERVIEW);
        readDataPlayer(thisPlayer, listPlayer);
        thisPlayer.setHost(true);
        //addChatJoin(thisPlayer.displayName);
        thisPlayer.updatePlayerView();
        updatePositionPlayerView();
    }

    public virtual void handleRTable(JObject data)
    {
        var name = (string)data["Name"];
        var player = getPlayer(name);
        if (player == null)
            return;

        player.setReady(true);
    }

    public virtual void handleRJTable(string strData)
    {
        //{\"M\":1000,\"ArrP\":[{\"id\":8240,\"N\":\"hienndm\",\"AG\":1900674,\"VIP\":3,\"Arr\":[32,8,41,11,36,40,17,26,18,14,42],\"Av\":4,\"FId\":0,\"UserType\":0,\"displayName\":\"hienndm\",\"numbCardHand\":11,\"lstMeld\":[],\"point\":-150,\"numberMatchWin\":0},{\"id\":1475,\"N\":\"christine_21\",\"AG\":1389405,\"VIP\":4,\"Arr\":[0,0,0,0,0,0,0],\"Av\":1,\"FId\":0,\"UserType\":0,\"displayName\":\"christine_21\",\"numbCardHand\":7,\"lstMeld\":[{\"idMeld\":0,\"arrMeld\":[13,52,39]},{\"idMeld\":1,\"arrMeld\":[23,24,25]},{\"idMeld\":2,\"arrMeld\":[47,48,45,46,44,49]},{\"idMeld\":3,\"arrMeld\":[5,6,7]},{\"idMeld\":4,\"arrMeld\":[2,15,28]},{\"idMeld\":5,\"arrMeld\":[12,51,38]},{\"idMeld\":6,\"arrMeld\":[9,35,22]}],\"point\":255,\"numberMatchWin\":0}],\"Id\":13,\"S\":4,\"CN\":1475,\"CT\":11,\"listCardShow\":[3,16,1],\"deckSize\":7,\"gameStatus\":\"STARTED\",\"topCard\":39,\"isDraw\":true,\"canShow\":false,\"pot\":40000,\"round\":1,\"lstWinning\":[]}
        stateGame = STATE_GAME.PLAYING;

        var data = JObject.Parse(strData);
        if (data.ContainsKey("maxBet"))
            setGameInfo((int)data["M"], (int)data["Id"], (int)data["maxBet"]);
        else
        {
            setGameInfo((int)data["M"], (int)data["Id"]);
        }
        var listPlayer = (JArray)data["ArrP"];
        //List<Player> lTemp = new List<Player>();
        players.Clear();

        for (var i = 0; i < listPlayer.Count; i++)
        {
            var player = new Player();
            players.Add(player);
            player.playerView = createPlayerView();
            //player.playerView.transform.localScale = Vector3.one;
            readDataPlayer(player, (JObject)listPlayer[i]);
            if (i == 0)
            {
                player.setHost(true);
            }
            if (player.id == User.userMain.Userid)
            {
                thisPlayer = player;
            }
            player.updatePlayerView();
            player.is_ready = true;
        }
        //players = lTemp;
        updatePositionPlayerView();
        //if (thisPlayer != null)
        //	addChatJoin(thisPlayer.displayName);

        //if (cc.sys.localStorage.getItem("isBack") == 'true')
        //{
        //	thisPlayer._playerView.icBack.node.active = true;
        //}
        //else
        //{
        //	thisPlayer._playerView.icBack.node.active = false;
        //}
    }


    public virtual void handleSTable(string strData)
    {
        var data = JObject.Parse(strData);
        setGameInfo((int)data["M"], (int)data["Id"], data.ContainsKey("maxBet") ? (int)data["maxBet"] : 0);
        for (int i = 0; i < players.Count; i++)
            if (players[i].playerView != null)
                Destroy(players[i].playerView.gameObject);

        stateGame = STATE_GAME.WAITING;
        JArray listPlayer = (JArray)data["ArrP"];
        players.Clear();
        for (var i = 0; i < listPlayer.Count; i++)
        {
            var player = new Player();
            players.Add(player);
            player.playerView = createPlayerView();
            readDataPlayer(player, (JObject)listPlayer[i]);

            if (i == 0) player.setHost(true);

            if (player.id == User.userMain.Userid || player.id == 8240)
            { //che do test
                thisPlayer = player;
                player.playerView.transform.localScale = Vector2.one;
            }
            player.updatePlayerView();
            player.is_ready = true;
            player.playerView.setDark(false);
        }
        //if (thisPlayer != null)
        //	addChatJoin(thisPlayer.displayName);

        updatePositionPlayerView();
    }


    public virtual void handleFinishGame()
    {
        stateGame = STATE_GAME.WAITING;

        ////  Config.vectorDelay[0].timeDelay = 0;
        clearAllCard();
    }
    public virtual void handleSpin(JObject data)
    {

    }
    public virtual void HandlerTip(JObject data)
    {

    }
    public virtual void HandlerUpdateUserChips(JObject data)
    {

    }

    void setTimeOut(System.Action callback, float time)
    {
        if (gameObject.activeSelf)
        {
            //coroutine = ExampleCoroutine(callback, time);
            StartCoroutine(ExampleCoroutine(callback, time));
        }

    }
    IEnumerator ExampleCoroutine(System.Action callback, float time)
    {
        //Print the time of when the function is first called.

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);

        //After we have waited 5 seconds print the time again.
        callback.Invoke();
    }

    //public void forceLeave()
    //{
    //    UIManager.instance.gameView = null;
    //    Destroy(gameObject);
    //    if (TableView.instance && Config.curGameId != (int)GAMEID.SLOTNOEL && TableView.instance && Config.curGameId != (int)GAMEID.SLOTTARZAN)
    //    {
    //        UIManager.instance.openTableView();
    //    }
    //    else
    //    {
    //        UIManager.instance.showLobbyScreen(false);
    //    }
    //}

    public void destroyThis()
    {
        DestroyImmediate(gameObject);
    }

    public virtual void onLeave()
    {
        //// return;
        Globals.Logging.Log("-------------Trang thaionLeave :   " + stateGame);
        if (stateGame != STATE_GAME.PLAYING)
        {
            Globals.Logging.Log("Trang thai oke, destroy leave ban ne!");
            User.userMain.lastGameID = 0;
            if (dataLeave != null)
            {
                Debug.Log("Data Leave=" + dataLeave.ToString());
                SocketIOManager.getInstance().emitSIOWithValue(dataLeave, "LeavePacket", false);
            }
            cleanGame();
            if (thisPlayer != null)
                User.userMain.AG = thisPlayer.ag;

            UIManager.instance.gameView = null;
            DestroyImmediate(gameObject);
            //if (TableView.instance && Config.curGameId != (int)GAMEID.SLOTNOEL && TableView.instance && Config.curGameId != (int)GAMEID.SLOTTARZAN)
            if (Config.isShowTableWithGameId(Config.curGameId) && User.userMain.VIP >= 1)
            {
                UIManager.instance.openTableView();
            }
            else
            {
                UIManager.instance.showLobbyScreen(false);
            }
            UIManager.instance.updateChipUser();

        }
        else
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    if (transform)
                        onLeave();
                });
        }
    }
    public virtual void onCardClick(Card card)
    {

        Logging.Log("Click Card:" + card.code);
    }
    public void congTienAm(string name, int money)
    {
        var pl = getPlayer(name);
        //cc.NGWlog("cong tien AM trong gameVIew");
        if (pl != null)
        {
            pl.ag += money;
            pl.setAg();
            pl.playerView.effectFlyMoney(money);
            if (pl == thisPlayer)
            {
                User.userMain.AG += money;
                var msg = Config.getTextConfig("nhan_ag_tu_server").Replace("%s", Config.FormatNumber(money)); ;
                UIManager.instance.showToast(msg, transform);
                //require('SMLSocketIO').getInstance().emitUpdateInfo();
            }
        }
    }
    public JArray getArrIdsCard(List<Card> cards)
    {
        JArray arrIds = new JArray();
        foreach (Card card in cards)
        {
            arrIds.Add(card.code);
        }
        return arrIds;
    }
    public void handleAutoExit(JObject data)
    {
        if (data.ContainsKey("reg") && data["reg"].GetType() == typeof(bool))
        {
            string str;
            if ((bool)data["reg"])
            {
                str = Config.getTextConfig("wait_game_end_to_leave");
            }
            else
            {
                str = Config.getTextConfig("minidice_unsign_leave_table");
            }
            ;

            thisPlayer.playerView.setExit((bool)data["reg"]);
            UIManager.instance.showToast(str, transform);
        }
        else if (data.ContainsKey("data"))
        {
            thisPlayer.playerView.setExit(true);
            UIManager.instance.showToast((string)data["data"], transform);
        }
    }

    /*End Handle Game*/
    void clearAllCard()
    {
        for (var i = 0; i < players.Count; i++)
        {
            players[i].clearAllCard();
        }
    }

    public virtual void cleanGame() { }
    public virtual void setGameInfo(int m, int id = 0, int maxBett = 0)
    {
        Config.tableMark = m;
        agTable = m;
        maxbet = maxBett;
        //Debug.Log("maxbet === " + maxBett);
        if (Config.curGameId == (int)GAMEID.BLACKJACK)
        {
            lbInfo.text = string.Format("{0} {1}\n{2}: {3}\n{4}: {5}", Config.getTextConfig("txt_id"), id, Config.getTextConfig("txt_bet"), Config.FormatNumber(m), Config.getTextConfig("shan2_maxbet"), Config.FormatNumber(maxbet));
        }
        else
        {
            lbInfo.text = string.Format("{0} {1}\n{2}: {3}", Config.getTextConfig("txt_id"), id, Config.getTextConfig("txt_bet"), Config.FormatMoney(m));
        }

        // Config.table_mark = m;
        // Config.tableId = id;
    }

    public void readDataPlayer(Player _player, JObject data)
    {
        //{\"M\":1000,\"ArrP\":[{\"id\":8240,\"N\":\"hienndm\",\"AG\":1920674,\"VIP\":3,\"Av\":4,\"FId\":0,\"UserType\":0,\"TotalAG\":0,\"timeToStart\":0,\"displayName\":\"hienndm\",\"numberMatchWin\":0}],\"Id\":13,\"AG\":1000000,\"S\":4,\"pot\":0,\"lstWinning\":[],\"round\":1}
        _player.id = (int)data["id"];
        if (data.ContainsKey("Url"))
        {
            //fb.1136983050395482
            var urlFID = (string)data["Url"];
            if (urlFID.Length > 0)
                _player.fid = urlFID.Substring(3);
            else
            {
                _player.fid = (string)data["FId"];
            }
        }
        else
            _player.fid = (string)data["FId"];

        _player.namePl = (string)data["N"];
        _player.ag = (long)data["AG"];
        _player.vip = (int)data["VIP"];
        if (data.ContainsKey("keyObjectInGame"))
            _player.idVip = (int)data["keyObjectInGame"];
        _player.avatar_id = (int)data["Av"];
        _player.is_ready = data.ContainsKey("isStart") ? (bool)data["isStart"] : true;
        //_player.ip = data.sIP;
        _player.displayName = (string)data["displayName"];
        //cc.NGWlog('====> read data player?', data.keyObjectInGame)
        //if (data.keyObjectInGame != null)
        //	_player.keyObjectInGame = data.keyObjectInGame;
    }

    public void updateItemVip(JObject jsonData)
    {
        if (jsonData.ContainsKey("uid"))
        {
            var pl = getPlayerWithID((int)jsonData["uid"]);
            if (pl != null)
            {
                pl.updateItemVipFromSV((int)jsonData["key"]);
            }
        }
        else
        {
            if (thisPlayer != null)
            {
                thisPlayer.updateItemVipFromSV((int)jsonData["key"]);
            }
        }
    }

    public void removePlayer(string nameP, bool isInGame = false)
    {
        var player = getPlayer(nameP);
        if (player != null)
        {
            players.RemoveAt(players.IndexOf(player));
            player.clearAllCard();
            Destroy(player.playerView.gameObject);
            player = null;

            if (!isInGame)
            {
                updatePositionPlayerView();
            }
        }
    }

    protected Player getPlayer(string namePlayer, bool isGetWithDisplayName = false) // mot so game n lai tra data name theo display name a son ak;
    {
        Player playerFind = null;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null) continue;
            PlayerView playerV = players[i].playerView;
            string displayName = players[i].displayName != null ? players[i].displayName : "";
            //Globals.Logging.Log("Search Player:-->namePl" + players[i].namePl + "--->DisplayName:" + players[i].displayName+"---->lbName="+ playerV.txtName.text);
            if (players[i].namePl.Equals(namePlayer) || displayName.Equals(namePlayer) || playerV != null && playerV.txtName.text.Equals(namePlayer))
            {
                playerFind = players[i];
            }
        }
        return playerFind;
        //}


    }
    protected Player getPlayerWithID(int id)
    {
        return players.FirstOrDefault(pl => pl.id == id);
    }

    protected int getIndexWithName(string namePlayer)
    {
        return players.FindIndex(pl => pl.namePl == namePlayer);
    }
    protected int getIndexOfPlayer(Player player)
    {
        return players.IndexOf(player);

    }
    protected int getIndexOf(Player player)
    {
        var index = getIndexOfPlayer(player); //vi tri hien tai trong players
        var thisPlayerIndex = players.Count;
        if (thisPlayer != null)
        {
            thisPlayerIndex = getIndexOfPlayer(thisPlayer);
        }
        return ((index + players.Count - thisPlayerIndex) % players.Count);
    }

    protected virtual void updatePositionPlayerView()
    {
        for (var i = 0; i < players.Count; i++)
        {
            var idPos = getDynamicIndex(getIndexOf(players[i]));
            players[i].playerView.transform.localPosition = listPosView[idPos];
            players[i]._indexDynamic = idPos;
            players[i].updateItemVip(players[i].vip, idPos);
        }
    }

    protected virtual int getDynamicIndex(int index)
    {
        if (index == 0) return 0;
        var _index = index;

        if (players.Count == 2)
        {
            _index++;
            return _index;
        }

        return _index;
    }

    public virtual PlayerView createPlayerView()
    {
        var plView = Instantiate(playerViewPrefab, (playerContainer != null ? playerContainer : transform));//.GetComponent<PlayerView>();
        plView.transform.SetSiblingIndex((int)ZODER_VIEW.PLAYER);
        plView.transform.localScale = Vector2.one;

        switch (Config.curGameId)
        {
            case (int)GAMEID.DRAGONTIGER:
                {
                    plView.transform.localScale = new Vector2(0.8f, 0.8f);
                    return plView.GetComponent<PlayerViewDragonTiger>();
                }
        }

        return plView.GetComponent<PlayerView>();
    }

    protected void removerCard(Card card)
    {
        if (card != null)
            Destroy(card.gameObject);
        //card.transform.SetParent(null);
        //card.gameObject.SetActive(false);
        //card.setDark(false);
        //cardPool.Add(card);
        //cardsOnTable.RemoveAt(cardsOnTable.IndexOf(card));
    }

    protected Card getCard(Transform parent = null, float scale = 1)
    {
        Card card;
        //if (cardPool.Count < 1)
        //{
        if (parent != null)
            card = Instantiate(UIManager.instance.loadPrefab("GameView/Card"), parent).GetComponent<Card>();
        else
            card = Instantiate(UIManager.instance.loadPrefab("GameView/Card")).GetComponent<Card>();
        //}
        //else
        //{
        //    card = cardPool[0].GetComponent<Card>();
        //    cardPool.RemoveAt(0);
        //    if (parent != null)
        //    {
        //        card.transform.SetParent(parent);
        //    }
        //}
        ////card.GameController = this;
        //card.transform.DOKill();

        card.resetDefaul();
        card.gameObject.SetActive(true);
        card.transform.localScale = new Vector3(scale, scale, scale);
        //cardsOnTable.Add(card);
        return card;
    }


    protected ChipBet getChipBet(Transform parent = null)
    {
        ChipBet chip;
        if (chipPool.Count < 1)
        {
            if (parent != null)
                chip = Instantiate(UIManager.instance.loadPrefab("GameView/ChipBet"), parent).GetComponent<ChipBet>();
            else
                chip = Instantiate(UIManager.instance.loadPrefab("GameView/ChipBet"), transform).GetComponent<ChipBet>();

        }
        else
        {
            chip = chipPool[0].GetComponent<ChipBet>();
            chipPool.RemoveAt(0);
            if (parent != null)
            {
                chip.transform.SetParent(parent);
            }
        }

        chip.transform.DOKill();
        chip.gameObject.SetActive(true);
        chip.transform.localScale = Vector2.one;
        return chip;
    }
    protected void removerChip(ChipBet chip)
    {
        //this.cardPool.put(card);
        chip.transform.SetParent(null);
        chip.gameObject.SetActive(false);
        chipPool.Add(chip);
    }

    public void updateVip()
    {
        if (thisPlayer != null)
        {
            thisPlayer.vip = User.userMain.VIP;
            thisPlayer.ag = User.userMain.AG;

            thisPlayer.updateMoney();
        }
    }
    public void playSound(string path)
    {
        SoundManager.instance.playEffectFromPath(path);
    }
    public string getString(JObject data, string propertyName)
    {
        return ObjectParse.getString(data, propertyName);
    }
    public int getInt(JObject data, string propertyName)
    {
        return ObjectParse.getInt(data, propertyName);
    }
    public long getLong(JObject data, string propertyName)
    {
        return ObjectParse.getLong(data, propertyName);
    }
    public float getFloat(JObject data, string propertyName)
    {
        return ObjectParse.getFloat(data, propertyName);
    }
    public bool getBool(JObject data, string propertyName)
    {
        return ObjectParse.getBool(data, propertyName);
    }
    public List<int> getListInt(JObject data, string propertyName)
    {
        return ObjectParse.getListInt(data, propertyName);
    }
    public JArray getJArray(JObject data, string propertyName)
    {
        return ObjectParse.getJArray(data, propertyName);
    }
    public List<JObject> getListJObject(JObject data, string propertyName)
    {
        return ObjectParse.getListJObject(data, propertyName);
    }

}
