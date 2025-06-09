using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;
using System.Globalization;
using System.Threading;
using Spine.Unity;
using System.Runtime.InteropServices;
using System.Linq;
//using Facebook.Unity;


namespace Globals
{

    public class CURRENT_VIEW
    {
        public const string LOGIN_VIEW = "LOGIN_VIEW",
    LOBBY = "LOBBY",
    PAYMENT = "PAYMENT",
    MAIL = "MAIL",
    PERSONAL = "PERSONAL",
    CHAT_FRIEND = "CHAT_FRIEND",
    RULE_VIEW = "RULE_VIEW",
    GAMELIST_VIEW = "GAMELIST_VIEW",
    FEEDBACK_VIEW = "FEEDBACK_VIEW",
    NEWS_VIEW = "NEWS_VIEW",
    SETTING_VIEW = "SETTING_VIEW",
    JACKPOT_VIEW = "JACKPOT_VIEW",
    GUIDE_INGAME = "GUIDE_INGAME",
    COUNTDOWN = "COUNTDOWN",
    REGISTER_VIEW = "REGISTER_VIEW",
    RANK_VIEW = "RANK_VIEW",
    INVITE_FRIEND_VIEW = "INVITE_FRIEND_VIEW",
    INVITE_PLAYERVIEW = "INVITE_PLAYERVIEW",
    DT_VIEW = "DT_VIEW",
    KET_VIEW = "KET_VIEW",
    CHATWORLD = "CHATWORLD",
    TOP_VIEW = "TOP_VIEW",
    FRIEND_VIEW = "FRIEND_VIEW",
    INFO_FRIEND_VIEW = "INFO_FRIEND_VIEW",
    CREATE_TABLE_GAME = "CREATE_TABLE_GAME",
    GIFT_CODE_VIEW = "GIFT_CODE_VIEW",
    MISSION_VIEW = "MISSION_VIEW",
    GAME_VIEW = "GAME_VIEW",
    SPECIAL_OFFER = "SPECIAL_OFFER",
    SEND_GIFT_VIEW = "SEND_GIFT_VIEW",
    MAIL_CHIP_VIEW = "MAIL_CHIP_VIEW",
    FREECHIP_VIEW = "FREECHIP_VIEW",
    PROFILE_VIEW = "PROFILE_VIEW",
    TOPRICH_VIEW = "TOPRICH_VIEW",
    CHECKPASS_VIEW = "CHECKPASS_VIEW",
    INFO_PLAYER_VIEW = "INFO_PLAYER_VIEW",
    GROUP_OPTION_INGAME = "GROUP_OPTION_INGAME",
    LIST_PLAYER_VIEW = "LIST_PLAYER_VIEW",
    LOTO = "LOTO";


        public static string currentView = "";

        public static void setCurView(string curView)
        {
            Debug.Log("setCurView  " + curView);
            currentView = curView;
            if (Config.isLoginSuccess)
            {
                SocketIOManager.getInstance().emitUpdateInfo();
            }
        }

        public static string getCurrentSceneName()
        {

            var sceneName = "";
            if (currentView == Config.curGameId.ToString())
            {
                sceneName = "GAMEVIEW_" + Config.curGameId;
            }
            else
            {
                switch (currentView)
                {
                    case LOGIN_VIEW:
                        sceneName = "LOGINVIEW";
                        break;

                    case LOBBY:
                        sceneName = "LOBBYVIEW";
                        break;

                    case PAYMENT:
                        sceneName = Config.formatStr("{0}{1}", "PA", "YMENTVIEW");
                        break;

                    case MAIL:
                        sceneName = "MAILVIEW";
                        break;

                    case PERSONAL:
                        sceneName = "PERSONALVIEW";
                        break;

                    case CHAT_FRIEND:
                        sceneName = "CHATFRIENDVIEW";
                        break;

                    case RULE_VIEW:
                        sceneName = "RULEVIEW";
                        break;

                    case GAMELIST_VIEW:
                        sceneName = "GAMELISTVIEW";
                        break;

                    case FEEDBACK_VIEW:
                        sceneName = "FEEDBACKVIEW";
                        break;

                    case NEWS_VIEW:
                        sceneName = "NEWSVIEW";
                        break;

                    case SETTING_VIEW:
                        sceneName = "SETTINGVIEW";
                        break;

                    case JACKPOT_VIEW:
                        sceneName = "JACKPOTVIEW";
                        break;

                    case GUIDE_INGAME:
                        sceneName = "GUIDEINGAMEVIEW";
                        break;

                    case COUNTDOWN:
                        sceneName = "COUNTDOWNVIEW";
                        break;

                    case REGISTER_VIEW:
                        sceneName = "REGISTERVIEW";
                        break;

                    case RANK_VIEW:
                        sceneName = "RANKVIEW";
                        break;
                    case DT_VIEW:
                        sceneName = "DTVIEW";
                        break;

                    case KET_VIEW:
                        sceneName = "KETVIEW";
                        break;

                    case CHATWORLD:
                        sceneName = "CHATWORLDVIEW";
                        break;

                    case TOP_VIEW:
                        sceneName = "TOPVIEW";
                        break;

                    case FRIEND_VIEW:
                        sceneName = "FRIENDVIEW";
                        break;

                    case CREATE_TABLE_GAME:
                        sceneName = "CREATETABLEVIEW";
                        break;

                    case GIFT_CODE_VIEW:
                        sceneName = "GIFTCODEVIEW";
                        break;

                    case MISSION_VIEW:
                        sceneName = "MISSIONVIEW";
                        break;

                    case TOPRICH_VIEW:
                        sceneName = "TOPRICHVIEW";
                        break;

                    case SEND_GIFT_VIEW:
                        sceneName = "SENDGIFTVIEW";
                        break;

                    case MAIL_CHIP_VIEW:
                        sceneName = "MAILVIEW";
                        break;

                    case FREECHIP_VIEW:
                        sceneName = "FREECHIPVIEW";
                        break;

                    case PROFILE_VIEW:
                        sceneName = "PROFILEVIEW";
                        break;
                    case INFO_FRIEND_VIEW:
                        sceneName = "INFOFRIENDVIEW";
                        break;
                    case LOTO:
                        sceneName = "LOTO";
                        break;
                    default:
                        break;
                }
            }
            return sceneName;
        }
    }
    public class ACTION_SLOT_SIXIANG
    {
        public const string getInfo = "getInfo";
        public const string normalSpin = "normalSpin";
        public const string scatterSpin = "scatterSpin";
        public const string dragonPearlSpin = "dragonPearlSpin";
        public const string luckyDraw = "luckyDraw";
        public const string goldPick = "goldPick";
        public const string rapidPay = "rapidPay";
        public const string selectBonusGame = "selectBonusGame";
        public const string getBonusGames = "getBonusGames";
        public const string buyBonusGame = "buyBonusGame";
        public const string exitGame = "exit";
    }
    public class TYPEWIN_BACCARAT
    {
        public const int BANKER = 1;
        public const int PLAYER = 2;
        public const int TIE = 3;
        public const int PLAYER_P = 102;
        public const int PLAYER_B = 12;
        public const int PLAYER_PB = 112;
        public const int BANKER_P = 101;
        public const int BANKER_B = 11;
        public const int BANKER_PB = 111;
        public const int TIE_P = 103;
        public const int TIE_B = 13;
        public const int TIE_PB = 113;
    }
    public enum GAMEID
    {
        GAOGEA = 8100,//show
        KEANG = 8013,
        DUMMY = 8015,
        SLOT_20_LINE = 1009, //doi cho 50.tam thoi;
        SLOT_20_LINE_JP = 1010,
        TIENLEN = 9009,
        SLOT100LINE = 1008,
        ROULETTE = 1111,
        SLOT_INCA = 9008, // doi cho 20.tam thoi
        TONGITS_OLD = 8091,
        TONGITS = 8090,
        TONGITS_JOKER = 8088,
        XOCDIA = 8813,
        SHOW = 8808,
        BAUCUA = 8803,
        LUCKY_89 = 8802,
        SHANKOEMEE = 1001,
        PUSOY = 8044,
        BURMESE_POKER = 8819,
        BLACKJACK = 9501,
        BACCARAT = 9500,
        TONGITS11 = 8089,
        LUCKY9 = 6688,
        SLOT20FRUIT = 9007,
        THREE_CARD_POKER = 8012,
        SABONG = 8011,
        SICBO = 8010,
        SLOTNOEL = 8818,
        SLOT_JUICY_GARDEN = 9900,
        SLOTTARZAN = 9950,
        DOMINO = 8020,
        BANDAR_QQ = 8021,
        SLOT_SIXIANG = 9011,
        RONGHO = 8009,
        KARTU_QIU = 8805,
        MINE_FINDING = 8804
    }
    public enum DOTWEEN_TAG
    {
        PROFILE_COUNTTIME = 1000,
    }
    public enum EFFECT_POPUP
    {
        NONE,
        SCALE,
        MOVE_LEFT,
        MOVE_RIGHT,
        MOVE_UP,
        MOVE_DOWN
    }
    public enum ConnectionStatus
    {
        NONE,
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
    }
    public enum STATE_GAME
    {
        WAITING = 0,
        PLAYING = 1,
        VIEWING = 2
    }

    public enum ZODER_VIEW
    {
        NON = 0,
        PLAYER = 10,
        CARD = 101,
        EFFECT = 102
    }
    public enum GAME_ZORDER
    {
        Z_PLAYERVIEW = 35,
        Z_CARD = 50,
        Z_BET = 100,
        Z_CHAT = 150,
        Z_EMO = 200,
        Z_BUTTON = 250,
        Z_TEXT_FLY = 270,
        Z_MENU_VIEW = 300
    }

    public enum LOGIN_TYPE
    {
        NONE = -1,
        NORMAL = 0,
        PLAYNOW = 1,
        FACEBOOK = 2,
        FACEBOOK_INSTANT = 3,
        APPLE_ID = 4,
        REG_ACC = 5
    }
    public class SOUND_GAME
    {
        public const string CARD_FLIP_1 = "Sounds/Common/card_flip_1";
        public const string CARD_FLIP_2 = "Sounds/Common/cardPlipBlackJack";
        public const string DISPATCH_CARD = "Sounds/Common/chiabai";
        public const string WIN = "Sounds/Common/win";
        public const string LOSE = "Sounds/Common/lose";
        public const string CLOCK_TICK = "Sounds/Common/clockTick";
        public const string FOLD = "Sounds/Common/fold";
        public const string TIP = "Sounds/Common/tip";
        public const string ALL_IN = "Sounds/Common/allin";
        public const string ALERT = "Sounds/Common/alert";
        public const string DROP = "Sounds/Common/boluot";
        public const string BET = "Sounds/Common/bet";
        public const string REMOVE = "Sounds/Common/Remove";
        public const string THROW_CHIP = "Sounds/Common/nemxu";
        public const string START_GAME = "Sounds/Common/start_game";
        public const string GET_CHIP = "Sounds/Common/getChip";
        public const string IN_GAME_COMMON = "Sounds/Common/casbg_audio";
        public const string EXPLODE = "Sounds/Common/explode_small";
        public const string TICKTOK = "Sounds/Common/tickTok";
        public const string REWARD = "Sounds/Common/reward";
        public const string CLOCK_HURRY = "Sounds/Common/clockhurry";
        public const string CLICK = "Sounds/Common/buttonClick";

    }
    public class SOUND_CHAT
    {
        public const string BEER = "Sounds/ChatInGame/beer";
        public const string BOOM = "Sounds/ChatInGame/bom";
        public const string EGG = "Sounds/ChatInGame/egg";
        public const string KISS = "Sounds/ChatInGame/kiss";
        public const string ROSE = "Sounds/ChatInGame/rose";
        public const string TOMATO = "Sounds/ChatInGame/tomato";
        public const string WATER = "Sounds/ChatInGame/water";

    }
    public class SOUND_HILO
    {
        public const string DICE_OPEN = "Sounds/HiLo/audio_diceopen_01";
        public const string DICE_SHAKE = "Sounds/HiLo/audio_diceshake_01";
        public const string CHIP_LOSER = "Sounds/HiLo/chip_loser";
        public const string CHIP_WINNER = "Sounds/HiLo/chip_winner";
        public const string LOSE = "Sounds/HiLo/lose_baucua";
        public const string WIN = "Sounds/HiLo/win_baucua";
        public const string START_GAME = "Sounds/HiLo/start_game";
    }
    public class SOUND_SLOT
    {
        public const string BIG_WIN = "Sounds/Slot/snd_big_win";
        public const string MEGA_WIN = "Sounds/Slot/snd_mega_win";
        public const string WHEEL = "Sounds/Slot/snd_wheel";
        public const string SHOW_LINE = "Sounds/Slot/snd_show_line";
        public const string STOP_SPIN = "Sounds/Slot/snd_stop";
        public const string FREESPIN = "Sounds/Slot/snd_free_spin";
        public const string WILD_SCATTER = "Sounds/Slot/snd_wild_scatter";
        public const string BG_NOEL = "Sounds/Slot/bg_noel";
        public const string BG_TARZAN = "Sounds/SlotTarzan/TarzanBg";
        public const string BG_JUICY_GARDEN = "Sounds/JuicyGarden/juicyBg";

    }
    public class SOUND_SLOT_BASE
    {
        public const string BG_GAME = "GameView/SiXiang/Sound/background";
        public const string SPIN = "GameView/SiXiang/Sound/quay";
        public const string COLLUM_STOP = "GameView/SiXiang/Sound/item_Stop";
        public const string LINE_WIN = "GameView/SiXiang/Sound/line-win";
        public const string CLICK = "GameView/SiXiang/Sound/buttonClick";
        public const string CHIP_REWARD = "GameView/SiXiang/Sound/reward";
        public const string SPIN_REEL = "GameView/SiXiang/Sound/run-item";
        public const string BIGWIN_START = "GameView/SiXiang/Sound/bgm_bigwin_main";
        public const string BIGWIN_END = "GameView/SiXiang/Sound/bgm_bigwin_end";
        public const string WILD_EXPAND = "GameView/SiXiang/Sound/wildExpand";
        public const string WILD_SYMBOL = "GameView/SiXiang/Sound/wildSymbol";
        public const string SCATTER_SYMBOL = "GameView/SiXiang/Sound/symScatter";
        public const string SCATTER_WIN = "GameView/SiXiang/Sound/scatterWinHighlight";
        public const string NEAR_FREESPIN_START = "GameView/SiXiang/Sound/nearFsStart";
        public const string NEAR_FREESPIN_END = "GameView/SiXiang/Sound/nearFsEnd";
        public const string CUT_SCENE = "GameView/SiXiang/Sound/cut_scene";
        public const string SHOW_ANIMAL = "GameView/SiXiang/Sound/showAnimal";
        public const string CLICK_ITEM_WIN = "GameView/SiXiang/Sound/click_trung_minigame";
        public const string CLICK_ITEM_MISS = "GameView/SiXiang/Sound/click_truot_minigame";
        public const string COUNGTING_MONEY_START = "GameView/SiXiang/Sound/prizeInfobarTotalwinMedMain";
        public const string COUNGTING_MONEY_END = "GameView/SiXiang/Sound/prizeInfobarTotalwinMedEnd";
        public const string PEARL_ITEM = "GameView/SiXiang/Sound/pearlItem";
        public const string PEARL_RUNITEM = "GameView/SiXiang/Sound/Pearl_RunItem";
        public const string PEARL_BG = "GameView/SiXiang/Sound/bgDragonPealSpin";
        public const string PEARL_Dragon = "GameView/SiXiang/Sound/Pearl_Dragon";
        public const string PEARL_Tiger = "GameView/SiXiang/Sound/pearl_tiger";
        public const string PEARL_Turtle = "GameView/SiXiang/Sound/Pearl-Turtle";
        public const string PEARL_Phoenix = "GameView/SiXiang/Sound/Pearl_Phoenix";
        public const string PEARL_Item_Normal = "GameView/SiXiang/Sound/Pear_ItemNormal";
        public const string RAPID_CHIP_FLY = "GameView/SiXiang/Sound/coin_flying";
        public const string RAPID_ITEM_WIN = "GameView/SiXiang/Sound/rapid_itemWin";
        public const string LUCKYDRAW_ITEM_NORMAL = "GameView/SiXiang/Sound/luckyDrawItemNormal";
        public const string LUCKYDRAW_ITEM_JACKPOT = "GameView/SiXiang/Sound/LuckyDrawItemJackpot";
        public const string LUCKYDRAW_WIN_JACKPOT = "GameView/SiXiang/Sound/LuckyDraw_Win_Jackpot";
        public const string WIN_JACKPOT_START = "GameView/SiXiang/Sound/bgm_jackpot_start";
        public const string WIN_JACKPOT_END = "GameView/SiXiang/Sound/bgm_jackpot_end";
        public const string SCATTER_SPIN = "GameView/SiXiang/Sound/Scatter_spin";


    }
    public class SOUND_DUMMY
    {
        public const string BURNED = "Sounds/DUMMY/effect_burned";
        public const string DUMMY = "Sounds/DUMMY/effect_dummy";
        public const string FINISH = "Sounds/DUMMY/effect_finish";
        public const string FULL_DUMMY = "Sounds/DUMMY/effect_fulldummy";
        public const string KEEP_HEAD = "Sounds/DUMMY/effect_keephead";
        public const string KNOCK_COLOR = "Sounds/DUMMY/effect_knockcolor";
        public const string KNOCK_DARK = "Sounds/DUMMY/effect_knockdark";
        public const string KNOCK_DARK_COLOR = "Sounds/DUMMY/effect_knockdarkcolor";
        public const string KNOCK_OUT = "Sounds/DUMMY/effect_knockout";
        public const string CHIABAI = "Sounds/DUMMY/effect_lc";
        public const string MELD = "Sounds/DUMMY/effect_meld";
        public const string SHOW = "Sounds/DUMMY/effect_show";

    }
    public class SOUND_DOMINO
    {
        public const string CHIP_WIN = "Sounds/Domino/chips_winner";
        public const string REWARD = "Sounds/Domino/reward";
        public const string FOLD = "Sounds/Domino/burned";
        public const string SINGLE = "Sounds/Domino/fold";
        public const string SHOW_RESULTS = "Sounds/Domino/PopUpOpen";
        public const string CHIA_DOMINO = "Sounds/Domino/chiaDomino";
        public const string DC_DOMINO = "Sounds/Domino/dominoAudio";
    }

    public class SOUND_BINH
    {
        public const string WIN_BANKER = "Sounds/Binh/win_banker";
        public const string COMPARE_WIN = "Sounds/Binh/compare_win";
        public const string COMPARE_LOSE = "Sounds/Binh/compare_lose";
    }

    public class SOUND_TONGITS
    {
        public const string TgFightMusic = "Sounds/Tongits/fight";
        public const string TgFoldMusic = "Sounds/Tongits/fold";
        public const string TgChallengetMusic = "Sounds/Tongits/challenge";
        public const string TgBurnedMusic = "Sounds/Tongits/burned";
        public const string TgTongitsMusic = "Sounds/Tongits/tongits";
        public const string TgEatcardMusic = "Sounds/Tongits/eat_card";
    }

    public class User
    {
        public User() { }
        //static User instance = null;
        public static User userMain = null;
        static public string AccessToken = "";
        static public string FacebookID;
        //{
        //    if (instance == null)
        //        instance = new User();
        //    return instance;
        //}

        //public int id;

        //public string UserName = "";
        public string Password = "";

        public int Userid;
        public string Username;
        public string Tinyurl;
        public long AG;
        public long LQ;
        public int VIP;
        public int MVip;
        public int markLevel;
        public int PD;
        public int OD;
        public int Avatar;
        public int NM;
        public long nmAg = 0;
        public string ListDP;
        public int NewAccFBInDevice;
        public long agSafe;
        //public int gameid;
        public int NumFriendMail;
        public int gameNo;
        public int Diamond;
        public int vippoint;
        public int vippointMax;
        public string FacebookName;
        public string displayName;
        public float LQ0;
        public float CO;
        public float CO0;
        public float LQSMS;
        public float LQIAP;
        public float LQOther;
        public float BLQ1;
        public float BLQ3;
        public float BLQ5;
        public float BLQ7;
        public float AVG7;
        public float Group;
        public long CreateTime;
        public int keyObjectInGame;
        public string idChatContents;
        public string UsernameLQ;
        public bool isShowMailAg = true;
        public int LoginType = 0; //0-playnow,1-Login Id,2-Login Fb
        public int uidInvite = 0;
        public bool canInputInvite = false;
        public int timeInputInvite = 0;
        public long timeInputInviteRemain = 0;

        public int lastGameID = 0;
        public int mailUnRead = 0;
        public int messageUnRead = 0;

        public List<JObject> listMailAdmin = new List<JObject>();

    }
    public class Config
    {
        public const string Bundle_URL = "";
        public const int CODE_JOKER_BLACK = 60;
        public const int CODE_JOKER_RED = 61;

        public static bool isSvTest = false;
        public const int OPERATOR = 7000;

        public static string user_name = "";
        public static string user_pass = "";
        public static string user_name_temp = "";
        public static string user_pass_temp = "";
        public static string username_normal = "";
        public static string password_normal = "";
        public static string chat_support_link = "https://m.me/100087270858966";
        //public static string IP = "app1.topbangkokclub.com";
        public const string PORT = "443";
        public static bool isSound = false;
        public static bool isMusic = false;
        public static bool isVibration = false;
        public static bool invitePlayGame = true;
        public static bool isErrorNet = false;
        public static string language = "THAI";
        public static bool isReconnect = false;
        public static LOGIN_TYPE typeLogin = LOGIN_TYPE.NORMAL;

        public static string curServerIp = "app-001.ngwcasino.com";
        public static int curGameId = 0;
        public static bool isBackGame = false;

        public static bool isChangeTable = false;

        public static int tableId = 0;
        public static int tableMark = 0;

        public static int agRename = 0;
        public static int agContactAd = 500;
        public static JArray listRuleJackPot = new JArray();
        public static JArray listVipBonusJackPot = new JArray();
        public static JObject dataRegister = new JObject();
        public static List<JObject> list_Alert = new List<JObject>();
        public static List<JObject> list_AlertShort = new List<JObject>();


        public static JObject dataVipFarm;

        public static string deviceId = "";
        public static string versionGame = Application.version;
        public static string publisher = "diamond_domino_slots_" + versionGame.Replace('.', '_');
        public static string package_name = Application.identifier;
        public static string versionDevice = getVersionDevice();
        public static string versionNameOS = SystemInfo.operatingSystem;
        public static string model = SystemInfo.deviceName;
        public static string brand = SystemInfo.deviceModel;

        public static bool gamenotification = true;
        public static bool allowPushOffline = true;
        public static bool is_reg = false;
        public static bool isShowLog = true;
        public static bool is_login_guest = true;
        public static bool is_login_fb = true;
        public static int time_request = 5;
        public static int avatar_change = 2;
        public static int avatar_count = 10;
        public static string avatar_build = "";
        public static string url_privacy_policy = "";
        public static string avatar_fb = "";
        public static string name_fb = "";
        public static JArray listTextConfig = new JArray();//arr
        public static int disID = -1;
        public static string fbprivateappid = "";
        public static string fanpageID = "";
        public static string groupID = "";
        public static string hotline = "";
        public static long TimeOpenApp = 0;
        public static bool isLoginSuccess = false;
        public static int lastGameIDSave = 0;
        public static string u_SIO = "";

        [Tooltip("id - gameid, ip - ip sv, ip_dm - ip domain sv, agSvipMin - ag min, v_tb")]
        public static JArray listGame = new JArray();//array
        public static JArray listRankGame = new JArray();//array
        public static string u_chat_fb = "";
        public static string infoChip = "";
        public static string infoDT = "";
        public static string infoUser = "";
        public static string infoBNF = "";
        public static string url_rule = "";
        public static string url_help = "";
        public static string url_rule_refGuide = "";
        public static JArray delayNoti = new JArray();//array
        public static JArray mail20 = new JArray();

        public static bool data0 = false;
        public static bool isSendingSelectGame = false;


        public static string newest_versionUrl = "";
        public static string ApkFullUrl = "";
        public static int ketPhe = 10;
        public static bool is_dt = false;
        public static bool ketT = false;
        public static bool ket = false;
        public static bool ismaqt = false;
        public static bool is_bl_salert = false;
        public static bool is_bl_fb = false;
        public static bool is_xs = false;
        public static bool show_new_alert = false;
        public static bool is_First_CheckVIPFarms = true;
        public static bool isPlayNowFromLobby = false;
        public static bool enableLottery = false;
        public static bool IsBuildStore = true;

        public static JArray arrOnlistTrue = new JArray();
        public static JArray arrBannerLobby = new JArray();

        public static List<int> listGamePlaynow = new List<int>() { (int)GAMEID.RONGHO, (int)GAMEID.BANDAR_QQ, (int)GAMEID.ROULETTE, (int)GAMEID.SLOT_SIXIANG, (int)GAMEID.SLOT20FRUIT, (int)GAMEID.SLOT_INCA, (int)GAMEID.SLOTNOEL, (int)GAMEID.SLOT_JUICY_GARDEN, (int)GAMEID.SLOTTARZAN, (int)GAMEID.XOCDIA, (int)GAMEID.BAUCUA, (int)GAMEID.MINE_FINDING };
        public static List<int> listGameSlot = new List<int>() { (int)GAMEID.SLOT_SIXIANG, (int)GAMEID.SLOT_INCA, (int)GAMEID.SLOT20FRUIT, (int)GAMEID.SLOT_JUICY_GARDEN, (int)GAMEID.SLOTTARZAN, (int)GAMEID.SLOTNOEL };


        public static bool isShowTableWithGameId(int gameID)
        {
            switch (gameID)
            {
                case (int)GAMEID.SLOTNOEL:
                case (int)GAMEID.SLOT20FRUIT:
                case (int)GAMEID.SLOT_INCA:
                case (int)GAMEID.SLOT_JUICY_GARDEN:
                case (int)GAMEID.SLOTTARZAN:
                case (int)GAMEID.BANDAR_QQ:
                case (int)GAMEID.RONGHO:
                case (int)GAMEID.SLOT_SIXIANG:
                case (int)GAMEID.MINE_FINDING:
                    {
                        return false;
                    }
            }
            return true;
        }
        public static string getVersionDevice()
        {
#if UNITY_ANDROID
            var clazz = AndroidJNI.FindClass("android/os/Build$VERSION");
            var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
            var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
            return sdkLevel.ToString();
#elif UNITY_IOS
                return UnityEngine.iOS.Device.systemVersion;
#else

            return SystemInfo.operatingSystem;
#endif
        }


        public static void getDataUser()
        {
            user_name = PlayerPrefs.GetString("user_name", "");
            user_pass = PlayerPrefs.GetString("user_pass", "");
            typeLogin = (LOGIN_TYPE)PlayerPrefs.GetInt("type_login", (int)LOGIN_TYPE.NONE);

            username_normal = PlayerPrefs.GetString("username_normal", "");
            password_normal = PlayerPrefs.GetString("userpass_normal", "");
        }

        public static void setDataUser()
        {
            PlayerPrefs.SetString("user_name", user_name);
            PlayerPrefs.SetString("user_pass", user_pass);
            if (typeLogin == LOGIN_TYPE.NORMAL)
            {
                PlayerPrefs.SetString("username_normal", user_name);
                PlayerPrefs.SetString("userpass_normal", user_pass);
            }
            PlayerPrefs.Save();
        }

        public static void decodeCard(int code, ref int N, ref int S)
        {
            if (code == CODE_JOKER_RED || code == CODE_JOKER_BLACK)
            {
                S = code;
                N = code;
                return;
            }
            // // mỗi game có 1 điều decode # nhau
            S = ((code - 1) / 13) + 1; //>=1 <=4
            N = ((code - 1) % 13) + 2; // >=2 , <=14

            if (curGameId == (int)GAMEID.LUCKY_89
                || curGameId == (int)GAMEID.KEANG
                || curGameId == (int)GAMEID.RONGHO)
            {
                N = ((code - 1) % 13) + 1;
            }

            if (curGameId == (int)GAMEID.TONGITS_JOKER ||
                curGameId == (int)GAMEID.TONGITS11)
            {
                if (N == 14) N = 1;
            }
            //nameCard = N + getSuitInVN();
        }

        public static byte[] getByte(string str)
        {
            var result = Regex.Replace(str, @"[^\x00-\x7F]", c =>
      string.Format(@"\u{0:x4}", (int)c.Value[0]));
            return Encoding.UTF8.GetBytes(result);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = getByte(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
            //return (new UTF8Encoding(false)).GetString(Convert.FromBase64String(base64EncodedData));
        }

        public static bool checkContainBoundingBox(GameObject gameObject, PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var c in results)
            {
                Globals.Logging.Log("RaycastResult " + c.gameObject.name);
                if (gameObject == c.gameObject)
                {
                    return true;
                }
            }

            return false;
        }
        public static void saveLoginAccount()
        {
            PlayerPrefs.SetString("username", user_name);
            PlayerPrefs.SetString("userpass", user_pass);
            username_normal = user_name;
            password_normal = user_pass;
        }

        public static string FormatMoney(int money, bool isK = false)
        {
            double absoluteValue = Mathf.Abs(money);
            string input = absoluteValue.ToString(), floatPart = "", format = "";
            int idNumberNextToDotFromTail = 0, integerValue = 0;
            int aBillion = 1000000000, aMillion = 1000000, aThousand = 1000;
            if (absoluteValue >= aBillion)
            {
                format = "B";
                integerValue = (int)(absoluteValue / aBillion);
                idNumberNextToDotFromTail = absoluteValue.ToString().Length - 9;
            }
            else if (absoluteValue >= aMillion)
            {
                format = "M";
                integerValue = (int)(absoluteValue / aMillion);
                idNumberNextToDotFromTail = absoluteValue.ToString().Length - 6;
            }
            else
            {
                if (isK)
                {
                    if (absoluteValue >= aThousand)
                    {
                        format = "K";
                        integerValue = (int)(absoluteValue / aThousand);
                        idNumberNextToDotFromTail = absoluteValue.ToString().Length - 3;
                    }
                    else return FormatNumber(money);
                }
                else return FormatNumber(money);
            }
            bool foundNotZero = false;
            for (int i = idNumberNextToDotFromTail + 2; i >= idNumberNextToDotFromTail; i--)
            {
                if (input[i] == '0' && !foundNotZero) continue;
                floatPart = input[i] + floatPart;
                foundNotZero = true;
            }
            if (floatPart.Length > 0) floatPart = "." + floatPart;
            return (money < 0 ? "-" : "") + FormatNumber(integerValue) + floatPart + format;
        }
        public static string FormatMoney(long money, bool isK = false)
        {
            double absoluteValue = Mathf.Abs(money);
            string input = absoluteValue.ToString(), floatPart = "", format = "";
            int idNumberNextToDotFromTail = 0, integerValue = 0;
            int aBillion = 1000000000, aMillion = 1000000, aThousand = 1000;
            if (absoluteValue >= aBillion)
            {
                format = "B";
                integerValue = (int)(absoluteValue / aBillion);
                idNumberNextToDotFromTail = absoluteValue.ToString().Length - 9;
            }
            else if (absoluteValue >= aMillion)
            {
                format = "M";
                integerValue = (int)(absoluteValue / aMillion);
                idNumberNextToDotFromTail = absoluteValue.ToString().Length - 6;
            }
            else
            {
                if (isK)
                {
                    if (absoluteValue >= aThousand)
                    {
                        format = "K";
                        integerValue = (int)(absoluteValue / aThousand);
                        idNumberNextToDotFromTail = absoluteValue.ToString().Length - 3;
                    }
                    else return FormatNumber(money);
                }
                else return FormatNumber(money);
            }
            bool foundNotZero = false;
            for (int i = idNumberNextToDotFromTail + 2; i >= idNumberNextToDotFromTail; i--)
            {
                if (input[i] == '0' && !foundNotZero) continue;
                floatPart = input[i] + floatPart;
                foundNotZero = true;
            }
            if (floatPart.Length > 0) floatPart = "." + floatPart;
            return (money < 0 ? "-" : "") + FormatNumber(integerValue) + floatPart + format;
        }

        public static string FormatMoney2(int mo, bool isK = false)
        {
            if (mo > 999999999 || mo < -999999999)
            {
                return mo.ToString("0,,,.###B", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (mo > 999999 || mo < -999999)
            {
                return mo.ToString("0,,.##M", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (mo > 999 || mo < -999)
            {
                if (isK)
                {
                    //return mo.ToString("#,##0,K", CultureInfo.InvariantCulture);
                    return mo.ToString("0,.##K", CultureInfo.InvariantCulture).Replace(".", ",");
                }
                return FormatNumber(mo);
            }
            else
            {
                return mo.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
        }
        public static string FormatMoney2(long mo, bool isK = false, bool isBiggerThan100K = false)
        {
            if (mo > 999999999 || mo < -999999999)
            {
                return mo.ToString("#,,,.###B", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (mo > 999999 || mo < -999999)
            {
                return mo.ToString("#,,.##M", CultureInfo.InvariantCulture).Replace(".", ",");
            }

            else if (mo > 999 || mo < -999)
            {
                if (isK)
                {
                    if (isBiggerThan100K && (mo >= 100000 || mo <= -100000))
                    {
                        return mo.ToString("0,.##K", CultureInfo.InvariantCulture).Replace(".", ",");

                    }
                    else
                    {
                        return FormatNumber(mo);
                    }
                }
                return FormatNumber(mo);
            }
            else
            {
                return mo.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            }
        }
        public static string FormatMoney3(long mo, long valueMinFormatK = 1000)
        {
            if (mo > 999999999 || mo < -999999999)
            {
                return mo.ToString("0,,,.###B", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else if (mo > 999999 || mo < -999999)
            {
                return mo.ToString("0,,.##M", CultureInfo.InvariantCulture).Replace(".", ",");
            }

            else if (mo >= valueMinFormatK || mo <= -valueMinFormatK)
            {
                return mo.ToString("0,.##K", CultureInfo.InvariantCulture).Replace(".", ",");
            }
            else
            {
                return FormatNumber(mo);
            }
        }
        public static void tweenNumberFromK(TMPro.TextMeshProUGUI lbText, int toNumber, int startNumber = 0, float timeRun = 0.3f, bool isFormatK = false)
        {
            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney2(startNumber, isFormatK));
        }

        public static void tweenNumberTo(TMPro.TextMeshProUGUI lbText, int toNumber, int startNumber = 0, float timeRun = 0.3f, bool isFormatK = false)
        {
            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney2(startNumber, isFormatK, true));
        }
        public static void tweenNumberTo(TMPro.TextMeshProUGUI lbText, long toNumber, long startNumber = 0, float timeRun = 0.3f, bool isFormatK = false, bool is2Digit = true)
        {
            if (!is2Digit)
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney(startNumber, isFormatK));
            else
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney2(startNumber, isFormatK, true));
        }
        public static void tweenNumberToMoney(TMPro.TextMeshProUGUI lbText, int toNumber, int startNumber = 0, float timeRun = 0.3f, long valueMinFormatK = 10000)
        {
            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney3(startNumber, valueMinFormatK));
        }
        public static void tweenNumberToMoney(TMPro.TextMeshProUGUI lbText, long toNumber, long startNumber = 0, float timeRun = 0.3f, long valueMinFormatK = 10000, bool isLowerCase = false)
        {
            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(()
                =>
            {
                if (isLowerCase)
                {
                    lbText.text = FormatMoney3(startNumber, valueMinFormatK).ToLower();
                }
                else
                {
                    lbText.text = FormatMoney3(startNumber, valueMinFormatK);
                }
            }
                );
        }
        public static void tweenNumberToNumber(TMPro.TextMeshProUGUI lbText, int toNumber, int startNumber = 0, float timeRun = 0.3f, bool isLowerCase = false)
        {

            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).SetEase(Ease.InSine).OnUpdate(() => { if (isLowerCase) lbText.text = FormatNumber(startNumber).ToLower(); else lbText.text = FormatNumber(startNumber); });
            Vector2 normalScale = lbText.transform.localScale;
            Vector2 biggerScale = new Vector2(normalScale.x + 0.2f, normalScale.y + 0.2f);
            DOTween.Kill(lbText.transform);
            DOTween.Sequence()
            .Append(lbText.transform.DOScale(biggerScale, timeRun * 0.45f))
            .AppendInterval(timeRun * 0.45f)
            .Append(lbText.transform.DOScale(normalScale, timeRun * 0.1f));
        }
        private static Guid lbTweenGuid = Guid.NewGuid();
        public static void tweenNumberToNumber(TMPro.TextMeshProUGUI lbText, long toNumber, long startNumber = 0, float timeRun = 0.5f, bool isLowerCase = false)
        {
            DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => { if (isLowerCase) lbText.text = FormatNumber(startNumber).ToLower(); else lbText.text = FormatNumber(startNumber); }).OnComplete(() =>
            {
            });
            Vector2 normalScale = lbText.transform.localScale;
            Vector2 biggerScale = new Vector2(normalScale.x + 0.2f, normalScale.y + 0.2f);
            DOTween.Kill(lbText.transform);
            DOTween.Sequence()
            .Append(lbText.transform.DOScale(biggerScale, timeRun * 0.45f))
            .AppendInterval(timeRun * 0.45f)
            .Append(lbText.transform.DOScale(normalScale, timeRun * 0.1f));
        }
        public static void tweenNumberTo(TMPro.TextMeshProUGUI lbText, long toNumber, long startNumber = 0, float timeRun = 0.3f)
        {
            if (toNumber < 10000)
            {
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatNumber(startNumber));
            }
            else
            {
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney2(startNumber));
            }

        }
        //public static void tweenNumberToMoney(TMPro.TextMeshProUGUI lbText, long toNumber, long startNumber = 0, float timeRun = 0.3f)
        //{
        //    if (toNumber < 10000)
        //    {

        //    }
        //}
        public static void tweenNumberTo(Text lbText, int toNumber, int startNumber = 0, float timeRun = 0.3f)
        {

            if (toNumber < 999999)
            {
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatNumber(startNumber));
            }
            else
            {
                DOTween.To(() => startNumber, x => startNumber = x, toNumber, timeRun).OnUpdate(() => lbText.text = FormatMoney2(startNumber));
            }
        }
        public static string FormatNumber(int number)
        {
            //return o.toString().replace(/\B(?= (\d{ 3})+(? !\d))/ g, ",")
            return String.Format("{0:n0}", number);
        }
        public static string FormatNumber(long number)
        {
            //return o.toString().replace(/\B(?= (\d{ 3})+(? !\d))/ g, ",")
            return String.Format("{0:n0}", number);
        }
        public static string FormatNumber(float number)
        {
            //return o.toString().replace(/\B(?= (\d{ 3})+(? !\d))/ g, ",")
            return String.Format("{0:n0}", number);
        }

        public static int splitToInt(string number)
        {
            while (number.Contains("."))
            {
                number = number.Replace(".", "");
            }
            while (number.Contains(","))
            {
                number = number.Replace(",", "");
            }
            return int.Parse(number);
        }

        public static long splitToLong(string number)
        {
            while (number.Contains("."))
            {
                number = number.Replace(".", "");
            }
            while (number.Contains(","))
            {
                number = number.Replace(",", "");
            }
            return long.Parse(number);
        }

        public static Image createSprite(Sprite _spriteFrame = null, Transform parent = null)
        {
            GameObject imgObject = new GameObject("Image");

            RectTransform trans = imgObject.AddComponent<RectTransform>();
            if (parent != null)
            {
                imgObject.transform.SetParent(parent);
            }
            Image image = imgObject.AddComponent<Image>();
            if (_spriteFrame != null)
                image.sprite = _spriteFrame;
            //image.SetNativeSize();

            image.rectTransform.localScale = Vector3.one;


            return image;
        }
        public static Vector2 getPosInOtherNode(Vector2 nodePosition, GameObject otherNode)
        {
            return otherNode.transform.InverseTransformPoint(nodePosition);
        }

        //Vector3 linePoint1 A, Vector3 lineVec1 A1, Vector3 linePoint2 B, Vector3 lineVec2B1
        public static Vector2 LineLineIntersection(Vector2 A, Vector2 A1, Vector2 B, Vector2 B1)
        {
            Vector2 intersectionPoint = new Vector2(0, 0);
            Vector3 lineVec3 = B - A;
            Vector3 crossVec1and2 = Vector3.Cross(A1, B1);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, B1);
            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
            //is coplanar, and not parallel

            if (Mathf.Abs(planarFactor) < 0.0001f
                    && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                        / crossVec1and2.sqrMagnitude;
                intersectionPoint = A + (A1 * s);
                return intersectionPoint;
            }
            else
            {
                intersectionPoint = Vector2.zero;
                return intersectionPoint;
            }

        }
        public static bool pLineIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 retP)
        {
            if ((A.x == B.x && A.y == B.y) || (C.x == D.x && C.y == D.y))
            {
                return false;
            }
            float BAx = B.x - A.x;
            float BAy = B.y - A.y;
            float DCx = D.x - C.x;
            float DCy = D.y - C.y;
            float ACx = A.x - C.x;
            float ACy = A.y - C.y;

            float denom = DCy * BAx - DCx * BAy;

            retP.x = DCx * ACy - DCy * ACx;
            retP.y = BAx * ACy - BAy * ACx;

            if (denom == 0)
            {
                if (retP.x == 0 || retP.y == 0)
                {
                    // Lines incident
                    return true;
                }
                // Lines parallel and not incident
                return false;
            }

            retP.x = retP.x / denom;
            retP.y = retP.y / denom;

            return true;
        }
        public static Vector2 pIntersectPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            Vector2 retP = Vector2.zero;
            if (pLineIntersect(A, B, C, D, retP))
            {
                Vector2 P = Vector2.zero;
                P.x = A.x + retP.x * (B.x - A.x);
                P.y = A.y + retP.x * (B.y - A.y);
                return P;
            }

            return Vector2.zero;
        }

        public static Button createNodeButton(Sprite _spriteFrame = null, string _title = "")
        {
            if (_spriteFrame == null) return null;
            var nodeButton = Config.createSprite(_spriteFrame);
            var btnCom = nodeButton.gameObject.AddComponent<Button>();
            btnCom.interactable = true;
            if (_title != "")
            {
                var lbCom = Config.createLabel(_title, 25);
                lbCom.rectTransform.SetParent(nodeButton.rectTransform);
            }

            return btnCom;
        }

        //public static Text createLabel(string _string, int _fontSize, Transform parent = null)
        //{
        //    var nodeLb = new GameObject("Label");
        //    RectTransform trans = nodeLb.AddComponent<RectTransform>();

        //    var lbCom = nodeLb.AddComponent<Text>();
        //    lbCom.text = _string;
        //    lbCom.font = UIManager.instance.fontDefault;
        //    lbCom.fontSize = _fontSize;
        //    lbCom.alignment = TextAnchor.MiddleCenter;

        //    return lbCom;
        //}

        public static TextMeshProUGUI createLabel(string _string, int _fontSize, Transform parent = null)
        {
            var nodeLb = new GameObject("Label");
            RectTransform trans = nodeLb.AddComponent<RectTransform>();

            var lbCom = nodeLb.AddComponent<TextMeshProUGUI>();
            lbCom.text = _string;
            lbCom.font = UIManager.instance.fontDefault;
            lbCom.fontSize = _fontSize;
            lbCom.alignment = TextAlignmentOptions.Center;


            return lbCom;
        }
        public static string url_log = "http://192.168.1.132:3000";

        public static IEnumerator sendLog(string str, bool isDel)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", User.userMain.Userid);
            form.AddField("name", User.userMain.Username);
            if (!isDel)
                form.AddField("data", str);

            using (UnityWebRequest www = UnityWebRequest.Post(url_log + (isDel ? "clearlog" : "/savelog"), form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                }
            }
        }

        //public static async Task<Texture2D> GetRemoteTexture(string url)
        //{
        //    using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        //    {
        //        // begin request:
        //        var asyncOp = www.SendWebRequest();
        //        //// await until it's done: 
        //        while (!asyncOp.isDone)
        //        {
        //            //await Task.Yield();
        //            await Task.Delay(200);//30 hertz
        //        }

        //        // read results:
        //        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
        //        {
        //            // nothing to return on error:
        //            return null;
        //        }
        //        else
        //        {
        //            // return valid results:
        //            return DownloadHandlerTexture.GetContent(www);
        //        }
        //    }
        //}


        public class DataLoadImage
        {
            public Image sprite;
            //public string url;
            public UnityWebRequest www;
            public Action<Texture2D> callback;
            public Action callback2;

            public bool isDone;
            public string nameSave;
        }

        static string GetNameImageFromURL(string url)
        {
            Uri myUri = new Uri(url);
            string nameIMG = myUri.AbsolutePath.Replace('/', '_');
            return nameIMG;
        }

        public static void loadImgFromUrlAsync(string url = "", Action<Texture2D> callback = null, bool isLoadBanner = false)
        {
            if (isLoadBanner)
            {
                string nameIMG = GetNameImageFromURL(url);
                if (ImageManager.Instance.ImageExists(nameIMG))
                {
                    if (callback != null)
                    {
                        callback.Invoke(ImageManager.Instance.LoadTexture2D(nameIMG));
                    }
                }
                else
                {

                    DataLoadImage dataLoadImage = new DataLoadImage();
                    dataLoadImage.nameSave = nameIMG;
                    dataLoadImage.www = UnityWebRequestTexture.GetTexture(url);
                    Debug.Log("start Load Image");
                    dataLoadImage.www.SendWebRequest();
                    dataLoadImage.callback = callback;
                    UIManager.instance.addJobLoadImage(dataLoadImage);

                }
            }
            else
            {
                DataLoadImage dataLoadImage = new DataLoadImage();
                dataLoadImage.nameSave = "";
                dataLoadImage.www = UnityWebRequestTexture.GetTexture(url);
                dataLoadImage.www.SendWebRequest();
                dataLoadImage.callback = callback;
                UIManager.instance.addJobLoadImage(dataLoadImage);
            }
            //Texture2D texture = await GetRemoteTexture(url);
            //if (callback != null)
            //{
            //    callback.Invoke(texture);
            //}
        }

        public static async void loadImgFromUrlAsync(Image _sprite = null, string url = "", Action callback = null, bool isLoadBanner = false)
        {
            if (_sprite == null) return;
            if (isLoadBanner)
            {
                string nameIMG = GetNameImageFromURL(url);
                if (ImageManager.Instance.ImageExists(nameIMG))
                {
                    _sprite.sprite = ImageManager.Instance.LoadSprite(nameIMG);
                    if (callback != null)
                    {
                        callback.Invoke();
                    }
                }
                else
                {
                    _sprite.sprite = await GetRemoteSprite(url);
                    if (callback != null)
                    {
                        callback.Invoke();
                    }
                    //DataLoadImage dataLoadImage = new DataLoadImage();
                    //dataLoadImage.nameSave = nameIMG;
                    //dataLoadImage.www = UnityWebRequestTexture.GetTexture(url);
                    //dataLoadImage.www.SendWebRequest();
                    //dataLoadImage.sprite = _sprite;
                    //dataLoadImage.callback2 = callback;
                    //UIManager.instance.addJobLoadImage(dataLoadImage);

                }
            }
            else
            {
                //DataLoadImage dataLoadImage = new DataLoadImage();
                //dataLoadImage.nameSave = "";
                //dataLoadImage.www = UnityWebRequestTexture.GetTexture(url);
                //dataLoadImage.www.SendWebRequest();
                //dataLoadImage.sprite = _sprite;
                //dataLoadImage.callback2 = callback;
                //UIManager.instance.addJobLoadImage(dataLoadImage);
                _sprite.sprite = await GetRemoteSprite(url);
                if (callback != null)
                {
                    callback.Invoke();
                }
            }

        }
        public static async Task loadImgFromUrlAsync2(Image _sprite = null, string url = "", Action callback = null)
        {
            if (_sprite == null) return;
            Texture2D texture = await GetRemoteTexture(url);
            //}
            if (texture != null)
            {
                if (_sprite.IsDestroyed()) return;
                _sprite.sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            else
            {
                _sprite.sprite = UIManager.instance.getRandomAvatar();
            }
            if (callback != null)
            {
                callback.Invoke();
            }
        }
        public static async Awaitable<Sprite> GetRemoteSprite(string url, bool isLoadBanner = false)
        {
            if (isLoadBanner)
            {
                string nameIMG = GetNameImageFromURL(url);
                if (ImageManager.Instance.ImageExists(nameIMG)) return ImageManager.Instance.LoadSprite(nameIMG);
                else return await GetRemoteSprite(url, false);
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    // begin request:
                    var asyncOp = www.SendWebRequest();
                    // await until it's done: 
                    while (asyncOp.isDone == false) await Awaitable.WaitForSecondsAsync(.05f);
                    // read results:
                    if (www.result != UnityWebRequest.Result.Success)// for Unity >= 2020.1
                    {
                        Debug.Log("Error load Image:" + $"{www.error}, URL:{www.url}");
                        return null;
                    }
                    else
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(www);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
                        return sprite;
                    }
                }
            }
        }
        public static async Awaitable<Texture2D> GetRemoteTexture(string url, bool isBanner = false)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                // begin request:
                var asyncOp = www.SendWebRequest();
                // await until it's done: 
                while (asyncOp.isDone == false) await Awaitable.WaitForSecondsAsync(.05f);
                // read results:
                if (www.result != UnityWebRequest.Result.Success)// for Unity >= 2020.1
                {
                    Debug.Log($"{www.error}, URL:{www.url}");
                    return null;
                }
                else
                {
                    return DownloadHandlerTexture.GetContent(www);
                }
            }
        }
        [Tooltip("Cho text vào 1 thằng cha có RectMask")]
        public static void effectTextRunInMask(Text txtName, bool isFixLeft = false) //dieu kien la thang text phai co cha la RectMask
        {
            RectTransform txtRt = txtName.transform.GetComponent<RectTransform>();
            float textSize = txtName.preferredWidth;
            Transform maskText = txtName.transform.parent;
            RectTransform maskRt = maskText.GetComponent<RectTransform>();

            if (textSize > maskRt.sizeDelta.x)
            {
                float deltaX = textSize - maskRt.sizeDelta.x;
                float minPos = -deltaX / 2;
                float maxPos = deltaX / 2;
                if (txtName.alignment == TextAnchor.MiddleLeft)
                {
                    minPos = -textSize / 2;
                    maxPos = 0;
                }

                Sequence seq = DOTween.Sequence()
                     .Append(txtRt.DOLocalMoveX(maxPos, 0f))
                     .AppendInterval(1.0f)
                     .Append(txtRt.DOLocalMoveX(minPos, 1.0f))
                     .AppendInterval(1.0f)
                     .Append(txtRt.DOLocalMoveX(maxPos, 1.0f))
                     .AppendInterval(1.0f);
                seq.SetLoops(-1);
            }
            else
            {
                txtRt.localPosition = Vector3.zero;
                if (isFixLeft)
                {
                    txtRt.localPosition = new Vector2(-textSize / 2, 0);
                }
            }
        }

        [Tooltip("Cho text vào 1 thằng cha có RectMask")]
        public static void effectTextRunInMask(TextMeshProUGUI txtName, bool isFixLeft = false) //dieu kien la thang text phai co cha la RectMask
        {
            RectTransform txtRt = txtName.transform.GetComponent<RectTransform>();
            float textSize = txtName.preferredWidth;
            Transform maskText = txtName.transform.parent;
            RectTransform maskRt = maskText.GetComponent<RectTransform>();
            DOTween.Kill(txtName);
            if (textSize > maskRt.sizeDelta.x)
            {
                float deltaX = textSize - maskRt.sizeDelta.x;
                float minPos = -deltaX / 2;
                float maxPos = deltaX / 2;
                if (txtName.alignment == TextAlignmentOptions.MidlineLeft)
                {
                    minPos = -textSize / 2;
                    maxPos = 0;
                }
                Sequence seqMaskName = DOTween.Sequence();
                seqMaskName.Append(txtRt.DOLocalMoveX(maxPos, 0f))
                     .AppendInterval(1.0f)
                     .Append(txtRt.DOLocalMoveX(minPos, 1.0f))
                     .AppendInterval(1.0f)
                     .Append(txtRt.DOLocalMoveX(maxPos, 1.0f))
                     .AppendInterval(1.0f).SetLoops(-1);
                seqMaskName.SetTarget(txtName);

            }
            else
            {

                DOTween.Kill(txtName);
                txtRt.localPosition = Vector3.zero;
                if (isFixLeft)
                {
                    float deltaX = textSize - maskRt.sizeDelta.x;
                    txtRt.localPosition = new Vector2(deltaX / 2, 0);
                }
            }
        }

        public static async void EffRunNumber(TextMeshProUGUI lb, int curValue, int lastValue, float timeRun)
        {
            lb.StopAllCoroutines();
            // if (curValue == lastValue) return;

            lb.text = curValue.ToString();
            int deltaNum = lastValue - curValue;
            float numPerTime = deltaNum / (timeRun * 20f);

            await RunNumberAsync(lb, curValue, lastValue, numPerTime, timeRun);
        }

        private static async Task RunNumberAsync(TextMeshProUGUI lb, float curValue, int lastValue, float numPerTime, float timeRun)
        {
            for (int i = 0; i < 20 * timeRun; i++)
            {
                curValue += numPerTime;
                if (numPerTime > 0)
                {
                    if (curValue >= lastValue)
                    {
                        curValue = lastValue;
                        break;
                    }
                }
                else
                {
                    if (curValue <= lastValue)
                    {
                        curValue = lastValue;
                        break;
                    }
                }
                lb.text = FormatNumber(Mathf.RoundToInt(curValue));
                await Task.Delay((int)(timeRun / 20f * 1000));  // Convert seconds to milliseconds
            }
            lb.text = FormatNumber(lastValue);
        }

        public static void Vibration()
        {
            if (isVibration)
                Handheld.Vibrate();
        }


        public static void getConfigSetting()
        {
            // isSound = PlayerPrefs.GetInt("sound", 1) == 1;
            // isMusic = PlayerPrefs.GetInt("music", 1) == 1;
            // isVibration = PlayerPrefs.GetInt("vibration", 1) == 1;
        }

        public static void updateConfigSetting()
        {
            PlayerPrefs.SetInt("sound", isSound ? 1 : 0);
            PlayerPrefs.SetInt("music", isMusic ? 1 : 0);
            PlayerPrefs.SetInt("vibration", isVibration ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static string Utf16ToUtf8(string utf16String)
        {
            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Return UTF8 bytes as ANSI string
            return Encoding.Default.GetString(utf8Bytes);
        }

        static JObject jsonConfig = null;
        static JObject jsonConfigOff = null;

        public static void loadTextConfig(string lang = "THAI")
        {
            jsonConfig = null;
            language = PlayerPrefs.GetString("language_client", "");
            if (language == "")
            {
                PlayerPrefs.SetString("language_client", language);
                PlayerPrefs.Save();
            }
            language = lang;

            for (var i = 0; i < listTextConfig.Count; i++)
            {
                JObject itemLanguage = (JObject)listTextConfig[i];
                if (((string)itemLanguage["lang"]).ToUpper().Equals(language.ToUpper()))
                {
                    string conent = PlayerPrefs.GetString("config_text_" + language.ToUpper(), "");
                    if (conent != "")
                    {
                        jsonConfig = (JObject)JObject.Parse(conent)["ConfigClient"];
                    }
                    break;
                }
            }

            TextAsset jsonData = (TextAsset)Resources.Load("JsonText/text_" + language.ToLower());

            jsonConfigOff = (JObject)JObject.Parse(jsonData.text)["ConfigClient"];
        }

        public static string getTextConfig(string key)
        {
            if (jsonConfig != null && jsonConfig.ContainsKey(key))
            {
                return (string)jsonConfig[key];
            }
            if (jsonConfigOff != null && jsonConfigOff.ContainsKey(key))
            {
                return (string)jsonConfigOff[key];
            }
            return "Missing " + key;
        }
        public static Sprite LoadGameNameByGameId(int id)
        {
            return Resources.Load<Sprite>("Sprite Assets/Game Names/game_" + id);
        }
        public static Sprite LoadLuckyBallById(int id)
        {
            return Resources.Load<Sprite>("Sprite Assets/Lucky Number/ball_" + id);
        }

        public static string convertTimeToString(int seconds)
        {
            int h = (seconds / 3600) % 24;
            int p = (seconds / 60) % 60;
            int s = seconds % 60;// - h * 3600 - p * 60;
            return (h < 10 ? "0" : "") + h + ":" + (p < 10 ? "0" : "") + p + ":" + (s < 10 ? "0" : "") + s;
        }
        public static string convertSeccondToDDHHMMSS(int timeRemain)
        {
            long deltaTime = timeRemain * 1000;
            string seconds = Math.Floor(((double)(deltaTime / 1000) % 60)) + "";
            string minutes = Math.Floor(((double)(deltaTime / 1000 / 60) % 60)) + "";
            string hours = Math.Floor(((double)(deltaTime / (1000 * 60 * 60)) % 24)) + "";
            string days = Math.Floor(((double)deltaTime / (1000 * 60 * 60 * 24))) + "";
            double dayNum = Math.Floor(((double)deltaTime / (1000 * 60 * 60 * 24)));
            if (hours.Length < 2) hours = "0" + hours;
            if (minutes.Length < 2) minutes = "0" + minutes;
            if (seconds.Length < 2) seconds = "0" + seconds;

            return days + (dayNum < 2 ? " " + Config.getTextConfig("txt_day") : " " + Config.getTextConfig("txt_day")) + ", " + hours + ":" + minutes + ":" + seconds;
        }

        public static string formatStr(string format, object arg0)
        {
            int index = 0;
            while (format.Contains("%s"))
            {
                var ibdex = format.IndexOf("%s");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }

            while (format.Contains("%d"))
            {
                var ibdex = format.IndexOf("%d");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }
            return string.Format(format, arg0);
        }

        public static string formatStr(string format, object arg0, object arg1)
        {
            int index = 0;
            while (format.Contains("%s"))
            {
                var ibdex = format.IndexOf("%s");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }

            while (format.Contains("%d"))
            {
                var ibdex = format.IndexOf("%d");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }
            return string.Format(format, arg0, arg1);
        }

        public static string formatStr(string format, object arg0, object arg1, object arg2)
        {
            int index = 0;
            while (format.Contains("%s"))
            {
                var ibdex = format.IndexOf("%s");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }

            while (format.Contains("%d"))
            {
                var ibdex = format.IndexOf("%d");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }
            return string.Format(format, arg0, arg1, arg2);
        }

        public static string formatStr(string format, params object[] args)
        {
            int index = 0;
            while (format.Contains("%s"))
            {
                var ibdex = format.IndexOf("%s");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }

            while (format.Contains("%d"))
            {
                var ibdex = format.IndexOf("%d");
                format = format.Remove(ibdex, 2).Insert(ibdex, "{" + index + "}");
                index++;
            }
            return string.Format(format, args);
        }

        public static float convertStringToNumber(string str)
        {

            var strNum = "";
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '.')
                {
                    strNum += str[i];
                }
                else
                {
                    if (str[i] >= '0' && str[i] <= '9')
                    {
                        strNum += str[i];
                    }
                }
            }

            return float.Parse(strNum, System.Globalization.CultureInfo.InvariantCulture);
        }
        //public static IEnumerator ShareImageShot()
        //{
        //var Screenshot_Name = DateTimeOffset.Now.ToUnixTimeMilliseconds() + ".png";

        //yield return new WaitForEndOfFrame();
        //Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);

        //screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);

        //screenTexture.Apply();

        //byte[] dataToSave = screenTexture.EncodeToPNG();

        //string destination = Path.Combine(Application.persistentDataPath, Screenshot_Name);

        //File.WriteAllBytes(destination, dataToSave);

        //var wwwForm = new WWWForm();
        //wwwForm.AddBinaryData("image", dataToSave, Screenshot_Name);

        //FB.API("me/photos", HttpMethod.POST, (IGraphResult result)=> {
        //    if (result.Error != null)
        //    {
        //        UIManager.instance.showToast(getTextConfig("txt_share_error"));
        //    }
        //    else
        //    {
        //        UIManager.instance.showToast(getTextConfig("txt_share_success"));
        //    }
        //}, wwwForm);
        //}

        //public static Type getData(Type t, JObject jData, string key)
        //{
        //    return (Type)jData[key];
        //}

        //void ahaha()
        //{
        //    ReadJson<bool>.getData(null, "");
        //}
    }
    //public class ReadJson<T>
    //{
    //    public static T getData(JObject jData, string key)
    //    {
    //        if(typeof(T) is bool)
    //        {
    //            return jData[key];
    //        }
    //       //return (T)(jData.GetType().get.GetProperty(key).ToString());
    //        //return (T)jData;
    //    }
    //}
    public class CMD
    {
        public const string OK = "OK";
        public const string FAILED = "FAILED";
        public const string DENIED = "DENIED";
        public const string ENUM_ERROR = "ENUM_ERROR";

        public const string W_DEFAULT = "w_default";
        public const string W_REPLACE = "w_replace";
        public const string U_DEFAULT = "u_default";
        public const string U_REPLACE = "u_replace";
        public const string IAP = "iap";

        public const int LOGIN_REQUEST = 10;
        public const int LOGIN_RESPONSE = 11;
        public const int FORCE_LOGOUT = 14;
        public const int JOIN_REQUEST = 30;
        public const int JOIN_RESPONSE = 31;
        public const int LEAVE_REQUEST = 36;
        public const int LEAVE_RESPONSE = 37;
        public const int GAME_TRANSPORT = 100;
        public const int SERVICE_TRANSPORT = 101;
        public const int PING = 7;
    }
    public class Promotion
    {
        public static int notEnoughMoney = 0; // hết tiền---
        public static int adminMoney = 0; // admin---
        public static int upVip = 0; // upVip---
        public static int online = 0; // online
        public static int video = 0; // video---
        public static int giftCode = 0; //giftcode---
        public static int time = 0; //time còn lại của online
        public static int videoCurrent = 0; // video current
        public static int videoMax = 0; // video max
        public static int onlineCurrent = 10; // online current
        public static int onlineMax = 0; // online max
        public static int agViewVideo = 0; // ag xem video
        public static int agOnline = 0; // ag online
        public static int agInviteFriend = 0; // ag invite fr

        public static int inviteMark = 0;
        public static int inviteNum = 0;

        public static int numberP = 0;
        public static List<int> timeWaiting = new List<int>();
        public static List<int> chipBonus = new List<int>();
        public static JObject DailyPromotionSpecial = null;
        public static JObject OnlinePolicy = null;

        public static int countMailAg = 0;

        public static void setPromotionInfo(JObject jsonData)
        {
            notEnoughMoney = (int)jsonData["P"];
            adminMoney = (int)jsonData["A"];
            upVip = (int)jsonData["UV"];
            online = (int)jsonData["O"];
            video = (int)jsonData["V"];
            giftCode = (int)jsonData["C"];
            time = (int)jsonData["T"];
            videoCurrent = (int)jsonData["VC"];
            videoMax = (int)jsonData["VM"];
            onlineCurrent = (int)jsonData["OC"];
            onlineMax = (int)jsonData["OM"];
            agViewVideo = (int)jsonData["NV"];
            agOnline = (int)jsonData["NO"];
            agInviteFriend = (int)jsonData["NIV"];
            inviteMark = (int)jsonData["InviteMark"];
            inviteNum = (int)jsonData["InviteNum"];

            if (jsonData.ContainsKey("OnlinePolicy"))
            {
                JObject OnlinePolicy = JObject.Parse((string)jsonData["OnlinePolicy"]);
                numberP = (int)OnlinePolicy["numberP"];

                JArray listTimeWaitting = (JArray)OnlinePolicy["timeWaiting"];
                timeWaiting = listTimeWaitting.ToObject<List<int>>();

                JArray listChipBonus = (JArray)OnlinePolicy["chipBonus"];
                chipBonus = listChipBonus.ToObject<List<int>>();
            }
            if (jsonData.ContainsKey("DailyPromotionSpecial"))
            {

                DailyPromotionSpecial = JObject.Parse((string)jsonData["DailyPromotionSpecial"]);
                Globals.Logging.Log(DailyPromotionSpecial);

                if (DailyBonusView.instance != null && DailyBonusView.instance.gameObject.activeSelf == true)
                {
                    DailyBonusView.instance.reloadHistory();
                    if (time == 0)
                    {
                        DailyBonusView.instance.setInfo();
                    }
                }
            }

            countMailAg = 0;
            if (adminMoney > 0)
            {
                countMailAg++;
            }
            if (upVip > 0)
            {
                countMailAg++;
            }
            if (notEnoughMoney > 0)
            {
                countMailAg++;
            }
            if (giftCode > 0)
            {
                countMailAg++;
            }
        }
    }

    public class COMMON_DATA
    {
        public static JArray ListChatWorld = new JArray();
    }

    public class NativeCall
    {
        static AndroidJavaClass unityPlayerClass;
        static NativeCall()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                var windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager");
                var displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay");

            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {

            }
        }
        //static DisplayMetricsAndroid()
        //{
        //    // Early out if we're not on an Android device
        //    if (Application.platform != RuntimePlatform.Android)
        //    {
        //        return;
        //    }

        //    // The following is equivalent to this Java code:
        //    //
        //    // metricsInstance = new DisplayMetrics();
        //    // UnityPlayer.currentActivity.getWindowManager().getDefaultDisplay().getMetrics(metricsInstance);
        //    //
        //    // ... which is pretty much equivalent to the code on this page:
        //    // http://developer.android.com/reference/android/util/DisplayMetrics.html

        //    using (
        //      AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"),
        //      metricsClass = new AndroidJavaClass("android.util.DisplayMetrics")
        //    )
        //    {
        //        using (
        //         AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
        //         activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
        //         windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
        //         displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay")
        //        )
        //        {
        //            displayInstance.Call("getMetrics", metricsInstance);
        //            Density = metricsInstance.Get<float>("density");
        //            DensityDPI = metricsInstance.Get<int>("densityDpi");
        //            HeightPixels = metricsInstance.Get<int>("heightPixels");
        //            WidthPixels = metricsInstance.Get<int>("widthPixels");
        //            ScaledDensity = metricsInstance.Get<float>("scaledDensity");
        //            XDPI = metricsInstance.Get<float>("xdpi");
        //            YDPI = metricsInstance.Get<float>("ydpi");
        //        }
        //    }
        //}
    }

    public class AssetBundleName
    {
        public const string POPUPS = "popups";
    }

    public class Logging
    {
        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        static public void Log(object message)
        {
            Debug.Log(message);
        }
        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        static public void LogError(object message)
        {
            Debug.LogError(message);
        }
        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        static public void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
        //[System.Diagnostics.Conditional("ENABLE_LOG")]
        static public void LogException(Exception message)
        {
            Debug.LogException(message);
        }

    }
}