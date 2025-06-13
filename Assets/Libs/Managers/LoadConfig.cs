using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Globals;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
public class CertificateWhore : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}
public class LoadConfig : MonoBehaviour
{
    public static LoadConfig instance;
    // string url_start = "https://n.cfg.davaogames.com/info";
    string url_start = "https://n.cfg.ngwcasino.com/info";
    string config_info = "";


    private bool _IsConfigLoaded = false;
    void Awake()
    {
        Config.publisher = "kh999_tienlen_1.0";
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    config_info = @"{""gamenotification"":false,""allowPushOffline"":true,""is_reg"":false,""isShowLog"":false,""is_login_guest"":true,""is_login_fb"":true,""time_request"":5,""avatar_change"":2,""avatar_count"":10,""avatar_build"":""https://cdn.topbangkokclub.com/api/public/dl/VbfRjo1c/avatar/%avaNO%.png"",""avatar_fb"":""https://graph.facebook.com/v9.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%"",""name_fb"":""https://graph.facebook.com/%userID%/?fields=name&access_token=%token%"",""text"":[{""lang"":""EN"",""url"":""https://conf.topbangkokclub.com/textEnglish""},{""lang"":""THAI"",""url"":""https://conf.topbangkokclub.com/textThai""}],""url_help"":"""",""bundleID"":""71D97F59-4763-5A1E-8862-B29980CF2D4C"",""version"":""1.00"",""operatorID"":7000,""os"":""android_unity"",""publisher"":""dummy_co_1_10"",""disID"":1007,""fbprivateappid"":"""",""fanpageID"":"""",""groupID"":"""",""hotline"":"""",""listGame"":[{""id"":8015,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8100,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8013,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8010,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8802,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9008,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":9007,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8818,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9950,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9900,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2}],""u_chat_fb"":"""",""infoChip"":"""",""infoDT"":"""",""infoBNF"":""https://conf.topbangkokclub.com/infoBNF"",""url_rule_js_new"":"""",""delayNoti"":[{""time"":5,""title"":""Pusoy"",""text"":""⚡️ Chip Free ⚡️"",""ag"":100000},{""time"":600,""title"":""Pusoy"",""text"":""💰Chip Free 💰"",""ag"":0},{""time"":86400,""title"":""Pusoy"",""text"":""⏰ Chip Free ⏰"",""ag"":0}],""data0"":false,""infoUser"":"""",""umode"":4,""uop1"":""Quit"",""umsg"":""This version don't allow to play game"",""utar"":"""",""newest_versionUrl"":""""}";

        //}
        //else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    config_info = @"{""gamenotification"":false,""allowPushOffline"":true,""is_reg"":false,""isShowLog"":false,""is_login_guest"":true,""is_login_fb"":true,""time_request"":5,""avatar_change"":2,""avatar_count"":10,""avatar_build"":""https://cdn.topbangkokclub.com/api/public/dl/VbfRjo1c/avatar/%avaNO%.png"",""avatar_fb"":""https://graph.facebook.com/v9.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%"",""name_fb"":""https://graph.facebook.com/%userID%/?fields=name&access_token=%token%"",""text"":[{""lang"":""EN"",""url"":""https://conf.topbangkokclub.com/textEnglish""},{""lang"":""THAI"",""url"":""https://conf.topbangkokclub.com/textThai""}],""url_help"":"""",""bundleID"":""71D97F59-4763-5A1E-8862-B29980CF2D4C"",""version"":""1.00"",""operatorID"":7000,""os"":""android_unity"",""publisher"":""dummy_co_1_10"",""disID"":1007,""fbprivateappid"":"""",""fanpageID"":"""",""groupID"":"""",""hotline"":"""",""listGame"":[{""id"":8015,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8100,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8013,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8010,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8802,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9008,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":9007,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8818,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9950,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9900,""ip"":""35.240.208.204"",""ip_dm"":""app1.topbangkokclub.com"",""agSvipMin"":25000,""v_tb"":2}],""u_chat_fb"":"""",""infoChip"":"""",""infoDT"":"""",""infoBNF"":"""",""url_rule_js_new"":"""",""delayNoti"":[{""time"":5,""title"":""Pusoy"",""text"":""⚡️ Chip Free ⚡️"",""ag"":100000},{""time"":600,""title"":""Pusoy"",""text"":""💰Chip Free 💰"",""ag"":0},{""time"":86400,""title"":""Pusoy"",""text"":""⏰ Chip Free ⏰"",""ag"":0}],""data0"":false,""infoUser"":"""",""umode"":4,""uop1"":""Quit"",""umsg"":""This version don't allow to play game"",""utar"":"""",""newest_versionUrl"":""""}";

        //}
        //else
        //{
        // config_info = "{\"gamenotification\":false,\"is_reg\":false,\"isShowLog\":false,\"is_login_guest\":true,\"is_login_fb\":true,\"time_request\":5,\"avatar_change\":2,\"avatar_count\":10,\"avatar_build\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/avatar/%avaNO%.png?inline=true\",\"avatar_fb\":\"https://graph.facebook.com/v10.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%\",\"name_fb\":\"https://graph.facebook.com/%userID%/?fields=name&access_token=%token%\",\"contentChat\":\"https://cfg.jakartagames.net/contentChat\",\"bundleID\":\"diamond.domino.slots\",\"version\":\"1.00\",\"operatorID\":7000,\"os\":\"android_cocosjs\",\"publisher\":\"config_offline_android\",\"disID\":1005,\"fbprivateappid\":\"\",\"fanpageID\":\"\",\"groupID\":\"\",\"hotline\":\"\",\"listGame\":[{\"id\":8009,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8010,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8020,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8021,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8044,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8805,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":8818,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9007,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9008,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9500,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9501,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9900,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9950,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9011,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2}],\"u_chat_fb\":\"\",\"infoUser\":\"https://cfg.jakartagames.net/infoUser\",\"umode\":0,\"uop1\":\"OK\",\"umsg\":\"\",\"utar\":\"\",\"uop2\":\"Cancel\",\"newest_versionUrl\":\"https://play.google.com/store/apps/details?id=diamond.domino.slots\"}";
        //}
        // if (Application.platform == RuntimePlatform.Android)
        // {
        //     config_info = "{\"gamenotification\":false,\"is_reg\":false,\"isShowLog\":false,\"is_login_guest\":true,\"is_login_fb\":true,\"time_request\":5,\"avatar_change\":2,\"avatar_count\":10,\"avatar_build\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/avatar/%avaNO%.png?inline=true\",\"avatar_fb\":\"https://graph.facebook.com/v10.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%\",\"name_fb\":\"https://graph.facebook.com/%userID%/?fields=name&access_token=%token%\",\"contentChat\":\"https://cfg.jakartagames.net/contentChat\",\"bundleID\":\"indo.test\",\"version\":\"1.00\",\"operatorID\":7000,\"os\":\"android_cocosjs\",\"publisher\":\"config_offline_android\",\"disID\":1005,\"fbprivateappid\":\"\",\"fanpageID\":\"\",\"groupID\":\"\",\"hotline\":\"\",\"listGame\":[{\"id\":8009,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8010,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8020,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8021,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8044,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8805,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":8818,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9007,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9008,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9500,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9501,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9900,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9950,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9011,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2}],\"u_chat_fb\":\"\",\"infoUser\":\"https://cfg.jakartagames.net/infoUser\",\"umode\":0,\"uop1\":\"OK\",\"umsg\":\"\",\"utar\":\"\",\"uop2\":\"Cancel\",\"newest_versionUrl\":\"https://play.google.com/store/apps/details?id=indo.test\"}";
        // }
        // else if (Application.platform == RuntimePlatform.IPhonePlayer)
        // {
        //     config_info = @"{""gamenotification"":false,""allowPushOffline"":true,""is_reg"":false,""isShowLog"":true,""is_login_guest"":true,""is_login_fb"":true,""time_request"":5,""avatar_change"":2,""avatar_count"":10,""avatar_build"":""https://storage.googleapis.com/cdn.davaogames.com/img/avatar/%avaNO%.png"",""avatar_fb"":""https://graph.facebook.com/v10.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%"",""listGame"":[{""id"":8091,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8044,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8090,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8088,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":6688,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":9007,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8802,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8011,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8808,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8012,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9008,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":9500,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":10000,""v_tb"":2},{""id"":8803,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8010,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":1111,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2},{""id"":8818,""ip"":""34.87.57.36"",""ip_dm"":""app2.davaogames.com"",""agSvipMin"":25000,""v_tb"":2}],""bundleID"":""bitbet.global.tongits"",""version"":""1.05"",""operatorID"":7000,""os"":""ios_cocosjs"",""publisher"":""config_offline_ios"",""disID"":1007,""fbprivateappid"":"""",""fanpageID"":"""",""groupID"":"""",""hotline"":"""",""u_chat_fb"":"""",""infoUser"":""https://n.cfg.davaogames.com/infoUser"",""umode"":0,""uop1"":""OK"",""umsg"":"""",""utar"":"""",""uop2"":""Cancel"",""newest_versionUrl"":""https://play.google.com/store/apps/details?id=bitbet.global.tongits""}';this.config_PM='[{""type"":""iap"",""title"":""iap"",""title_img"":""https://storage.googleapis.com/cdn.davaogames.com/img/shop/IAPIOS.png"",""items"":[{""url"":""bitbet.global.tongits.1"",""txtPromo"":""1USD=392,727Chips"",""txtChip"":""388,800Chips"",""txtBuy"":""0.99USD"",""txtBonus"":""0%"",""cost"":1},{""url"":""bitbet.global.tongits.2"",""txtPromo"":""1USD=390,754Chips"",""txtChip"":""777,600Chips"",""txtBuy"":""1.99USD"",""txtBonus"":""0%"",""cost"":2},{""url"":""bitbet.global.tongits.5"",""txtPromo"":""1USD=389,579Chips"",""txtChip"":""1,944,000Chips"",""txtBuy"":""4.99USD"",""txtBonus"":""0%"",""cost"":5},{""url"":""bitbet.global.tongits.10"",""txtPromo"":""1USD=486,486Chips"",""txtChip"":""4,860,000Chips"",""txtBuy"":""9.99USD"",""txtBonus"":""25%"",""cost"":10},{""url"":""bitbet.global.tongits.20"",""txtPromo"":""1USD=486,243Chips"",""txtChip"":""9,720,000Chips"",""txtBuy"":""19.99USD"",""txtBonus"":""25%"",""cost"":20},{""url"":""bitbet.global.tongits.50"",""txtPromo"":""1USD=486,097Chips"",""txtChip"":""24,300,000Chips"",""txtBuy"":""49.99USD"",""txtBonus"":""25%"",""cost"":50}]}]";
        // }
        // else
        // {
        //     config_info = "{\"gamenotification\":false,\"is_reg\":false,\"isShowLog\":false,\"is_login_guest\":true,\"is_login_fb\":true,\"time_request\":5,\"avatar_change\":2,\"avatar_count\":10,\"avatar_build\":\"https://cdn.tongitsonline.com/api/public/dl/ierd34s/images/avatar/%avaNO%.png?inline=true\",\"avatar_fb\":\"https://graph.facebook.com/v10.0/%fbID%/picture?width=200&height=200&redirect=true&access_token=%token%\",\"name_fb\":\"https://graph.facebook.com/%userID%/?fields=name&access_token=%token%\",\"contentChat\":\"https://cfg.jakartagames.net/contentChat\",\"bundleID\":\"diamond.domino.slots\",\"version\":\"1.00\",\"operatorID\":7000,\"os\":\"android_cocosjs\",\"publisher\":\"config_offline_ios\",\"disID\":1005,\"fbprivateappid\":\"\",\"fanpageID\":\"\",\"groupID\":\"\",\"hotline\":\"\",\"listGame\":[{\"id\":8009,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8010,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8020,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8021,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8044,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":8805,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":8818,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9007,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9008,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9500,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":10000,\"v_tb\":2},{\"id\":9501,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9900,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9950,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2},{\"id\":9011,\"ip\":\"8.213.193.230\",\"ip_dm\":\"app1.jakartagames.net\",\"agSvipMin\":25000,\"v_tb\":2}],\"u_chat_fb\":\"\",\"infoUser\":\"https://cfg.jakartagames.net/infoUser\",\"umode\":0,\"uop1\":\"OK\",\"umsg\":\"\",\"utar\":\"\",\"uop2\":\"Cancel\",\"newest_versionUrl\":\"https://play.google.com/store/apps/details?id=diamond.domino.slots\"}";
        // }

        string storedConfig = PlayerPrefs.GetString("config_save", "");
        init();
        if (!storedConfig.Equals("")) handleConfigInfo(storedConfig);
        // else UIManager.instance.showWaiting();
        StartCoroutine(loadConfig());
        IEnumerator loadConfig()
        {
            do
            {
                getConfigInfo();
                yield return new WaitForSeconds(10f);
            }
            while (!_IsConfigLoaded);
        }
    }

    void init()
    {
        Config.deviceId = SystemInfo.deviceUniqueIdentifier;
        //Config.versionGame = Application.version;

    }

    //IEnumerator GetRequest(string uri, WWWForm wwwForm, System.Action<string> callback)
    //{
    //    //Thread trd = new Thread(new ThreadStart(()=> {
    //    Logging.Log("-=-=uri " + uri);
    //    using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, wwwForm))
    //        {
    //        // Request and wait for the desired page.
    //        yield return webRequest.SendWebRequest();

    //        Logging.Log("Received: " + webRequest.downloadHandler.text);
    //        //Logging.Log("Received code: " + webRequest.responseCode);

    //        if (!webRequest.isNetworkError)
    //        {
    //            callback.Invoke(webRequest.downloadHandler.text);
    //        }
    //        else {
    //            Logging.LogError(webRequest.error);
    //        }
    //        }
    //    //}));

    //    //trd.Start();
    //}

    async void ProgressHandle(string url, string json, Action<string> callback, Action callbackError = null)
    {
        // UIManager.instance.showWaiting();
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = new DownloadHandlerBuffer();
        //uwr.SetRequestHeader("Content-Type", "application/json");
        www.certificateHandler = new CertificateWhore();
        // begin request:
        www.SetRequestHeader("Access-Control-Allow-Origin", "*");
        www.SetRequestHeader("Content-type", "application/json; charset=UTF-8");
        if (Application.isMobilePlatform)
            www.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
        var asyncOp = www.SendWebRequest();


        //// await until it's done: 
        while (!asyncOp.isDone)
        {
            await Task.Yield();
            //await Task.Delay(200);//30 hertz
        }
        if (UIManager.instance != null) UIManager.instance.hideWatting();
        // read results:
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
        {
            Logging.Log("Error While Sending: " + www.error);
            if (callbackError != null)
            {
                callbackError.Invoke();
            }
            www.Dispose();
        }
        else
        {
            Logging.Log("Received: " + www.downloadHandler.text);
            callback.Invoke(www.downloadHandler.text);
            www.Dispose();
        }

        //StartCoroutine(GetRequest(url, json, callback));
    }

    //IEnumerator GetRequest(string url, string json, Action<string> callback, Action callbackError = null)
    //{

    //    //Logging.Log("===> datapost ===>> : " + json);
    //    UIManager.instance.showWatting();
    //    var uwr = new UnityWebRequest(url, "POST");
    //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    //uwr.SetRequestHeader("Content-Type", "application/json");
    //    uwr.certificateHandler = new CertificateWhore();
    //    //    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    //    //(sender, certificate, chain, sslPolicyErrors) => true;

    //    uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
    //    uwr.SetRequestHeader("Content-type", "application/json; charset=UTF-8");
    //    if (Application.isMobilePlatform)
    //        uwr.SetRequestHeader("X-Requested-With", "XMLHttpRequest");

    //    //Send the request then wait here until it returns
    //    yield return uwr.SendWebRequest();

    //    UIManager.instance.hideWatting();
    //    if (uwr.result == UnityWebRequest.Result.ConnectionError)
    //    {
    //        Logging.Log("Error While Sending: " + uwr.error);
    //        if (callbackError != null)
    //        {
    //            callbackError.Invoke();
    //        }
    //    }
    //    else
    //    {
    //        Logging.Log("Received2: " + uwr.downloadHandler.text);
    //        callback.Invoke(uwr.downloadHandler.text);
    //    }
    //}

    JObject createBodyJsonNormal()
    {
        var osName = "android";
        if (Application.platform == RuntimePlatform.Android)
            osName = "android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            osName = "ios";

        //form.AddField("os", osName);
        //form.AddField("mcc", "[0,0]");

        JObject wWForm = new JObject();
        wWForm["version"] = Config.versionGame + "";
        wWForm["operatorID"] = Config.OPERATOR + "";
        wWForm["bundleID"] = "kh999.tienlen.ai.com";
        wWForm["publisher"] = Config.publisher;
        wWForm["os"] = osName;
        wWForm["mcc"] = "[0,0]";
        if (User.userMain != null)
        {
            wWForm["vip"] = User.userMain.VIP + "";
        }
        return wWForm;
    }

    JObject createBodyJson()
    {
        var wWForm = createBodyJsonNormal();
        if (User.userMain != null)
        {
            wWForm["id"] = User.userMain.Userid + "";
            wWForm["ag"] = User.userMain.AG + "";
            wWForm["lq"] = User.userMain.LQ + "";
            wWForm["vip"] = User.userMain.VIP + "";
            wWForm["group"] = (int)User.userMain.Group + "";
        }
        return wWForm;
    }


    public void getConfigInfo()
    {
        //loadInfo();
        var wWForm = createBodyJsonNormal();
        Debug.Log("-=-=getConfigInfo   " + wWForm.ToString());
        _IsConfigLoaded = false;
        ProgressHandle(url_start, wWForm.ToString(), handleConfigInfo);
    }


    public void getInfoUser(string _data0)
    {
        var wWForm = createBodyJson();
        if (Config.data0) wWForm["data0"] = _data0;
        Debug.Log("-=-=getInfoUser   " + Config.infoUser + " / " + wWForm.ToString());
        //StartCoroutine(GetRequest(Config.infoUser, wWForm.ToString(), handleUserInfo));
        ProgressHandle(Config.infoUser, wWForm.ToString(), handleUserInfo);
    }

    public void getInfoShop(Action<string> callback, Action callbackError = null)
    {
        var wWForm = createBodyJson();
        Logging.Log(wWForm);
        Logging.Log(Config.infoChip);

        Debug.Log("-=-=Config.infoChip===" + Config.infoChip);
        //StartCoroutine(GetRequest(Config.infoChip, wWForm.ToString(), callback, callbackError));
        ProgressHandle(Config.infoChip, wWForm.ToString(), callback, callbackError);
    }

    public void getInfoEX(Action<string> callback)
    {
        var wWForm = createBodyJson();
        //StartCoroutine(GetRequest(Config.infoDT, wWForm.ToString(), callback));
        ProgressHandle(Config.infoDT, wWForm.ToString(), callback);
    }

    public void getInfoBenefit(Action<string> callback)
    {
        var wWForm = createBodyJson();
        //StartCoroutine(GetRequest(Config.infoBNF, wWForm.ToString(), callback));
        ProgressHandle(Config.infoBNF, wWForm.ToString(), callback);
    }
    public void getTextConfig(string _url, string _language, bool isInit)
    {
        var wWForm = createBodyJsonNormal();
        //StartCoroutine(GetRequest(_url, wWForm.ToString(), (string strData) =>
        //{
        //    //Logging.Log("___ language  " + _language);
        //    //Logging.Log(_url + ": " + strData);
        //    JObject jConfig = null;
        //    try
        //    {
        //        jConfig = JObject.Parse(strData);
        //    }
        //    catch (Exception e)
        //    {
        //        Logging.LogException(e);
        //    }

        //    if (jConfig == null) return;
        //    var key = "config_text_" + _language.ToUpper();
        //    PlayerPrefs.SetString(key, strData);
        //    if (isInit)
        //        Config.loadTextConfig();
        //}));

        ProgressHandle(_url, wWForm.ToString(), (string strData) =>
        {
            //Logging.Log("___ language  " + _language);
            //Logging.Log(_url + ": " + strData);
            JObject jConfig = null;
            try
            {
                jConfig = JObject.Parse(strData);
            }
            catch (Exception e)
            {
                Logging.LogException(e);
            }

            if (jConfig == null) return;
            var key = "config_text_" + _language.ToUpper();
            PlayerPrefs.SetString(key, strData);
            if (isInit)
                Config.loadTextConfig();
        });
    }

    void handleConfigInfo(string strData)
    {
        PlayerPrefs.SetString("config_save", strData);
        Logging.Log("-=-=handleConfigInfo: " + strData);
        JObject jConfig = null;
        try
        {
            jConfig = JObject.Parse(strData);
        }
        catch (Exception e)
        {
            Logging.LogException(e);
        }

        if (jConfig == null) return;
        //Logging.Log("-=-=-=-=-=-=-=-=-= 1");
        //Logging.Log(jConfig);

        if (jConfig.ContainsKey("gamenotification"))
            Config.gamenotification = (bool)jConfig["gamenotification"];
        if (jConfig.ContainsKey("allowPushOffline"))
            Config.allowPushOffline = (bool)jConfig["allowPushOffline"];
        if (jConfig.ContainsKey("is_reg"))
            Config.is_reg = (bool)jConfig["is_reg"];
        if (jConfig.ContainsKey("isShowLog"))
            Config.isShowLog = (bool)jConfig["isShowLog"];
        if (jConfig.ContainsKey("is_login_guest"))
            Config.is_login_guest = (bool)jConfig["is_login_guest"];
        if (jConfig.ContainsKey("is_login_fb"))
            Config.is_login_fb = (bool)jConfig["is_login_fb"];
        if (jConfig.ContainsKey("time_request"))
            Config.time_request = (int)jConfig["time_request"];
        if (jConfig.ContainsKey("avatar_change"))
            Config.avatar_change = (int)jConfig["avatar_change"];
        if (jConfig.ContainsKey("avatar_count"))
            Config.avatar_count = (int)jConfig["avatar_count"];
        if (jConfig.ContainsKey("avatar_build"))
            Config.avatar_build = (string)jConfig["avatar_build"];
        if (jConfig.ContainsKey("url_privacy_policy"))
            Config.url_privacy_policy = (string)jConfig["url_privacy_policy"];
        if (jConfig.ContainsKey("lotteryEnable"))
            Config.enableLottery = (bool)jConfig["lotteryEnable"];
        if (jConfig.ContainsKey("u_SIO"))
        {
            Config.u_SIO = (string)jConfig["u_SIO"];
            Logging.LogWarning("-=-=-u_SIO  " + Config.u_SIO);
            SocketIOManager.getInstance().initSml();
            SocketIOManager.getInstance().startSIO();
        }
        else
        {
            Config.u_SIO = "";
        }

        if (jConfig.ContainsKey("avatar_fb"))
            Config.avatar_fb = (string)jConfig["avatar_fb"];
        if (jConfig.ContainsKey("name_fb"))
            Config.name_fb = (string)jConfig["name_fb"];
        if (jConfig.ContainsKey("text"))
        {
            Config.listTextConfig = jConfig["text"] as JArray;//arr
            for (var i = 0; i < Config.listTextConfig.Count; i++)
            {
                JObject itemLanguage = (JObject)Config.listTextConfig[i];
                getTextConfig((string)itemLanguage["url"], (string)itemLanguage["lang"], i >= Config.listTextConfig.Count - 1);
            }
        }
        if (jConfig.ContainsKey("disID"))
            Config.disID = (int)jConfig["disID"];

        Logging.Log("-=-=disID   " + Config.disID);
        if (jConfig.ContainsKey("fbprivateappid"))
            Config.fbprivateappid = (string)jConfig["fbprivateappid"];
        if (jConfig.ContainsKey("fanpageID"))
            Config.fanpageID = (string)jConfig["fanpageID"];
        else
            Config.fanpageID = "";
        if (jConfig.ContainsKey("groupID"))
            Config.groupID = (string)jConfig["groupID"];
        else
            Config.groupID = "";
        if (jConfig.ContainsKey("hotline"))
            Config.hotline = (string)jConfig["hotline"];
        else
            Config.hotline = "";

        if (jConfig.ContainsKey("listGame"))
        {
            List<int> sortedListId = new() {
                (int)GAMEID.SLOTSIXIANG, (int)GAMEID.BORKDENG, (int)GAMEID.TIENLEN, (int)GAMEID.SLOTFRUIT, (int)GAMEID.PUSOY,
                (int)GAMEID.SLOTINCA, (int)GAMEID.OSPHE, (int)GAMEID.BACCARAT, (int)GAMEID.SICBO, (int)GAMEID.ROULETTE,
                (int)GAMEID.SLOTTARZAN, (int)GAMEID.SLOTJUICYGARDEN,
            };
            Config.listGame = new();
            JArray tempListGameJA = jConfig["listGame"] as JArray, sortedListGameJA = new();
            for (int i = 0; i < tempListGameJA.Count; i++)
            {
                if ((int)tempListGameJA[i]["id"] == (int)GAMEID.OSPHE || (int)tempListGameJA[i]["id"] == (int)GAMEID.BORKDENG)
                {
                   tempListGameJA.RemoveAt(i);
                    i--;
                }
            }
            foreach (int id in sortedListId)
            {
                foreach (JToken item in tempListGameJA)
                {
                    if ((int)item["id"] == id)
                    {
                        sortedListGameJA.Add(item);
                        tempListGameJA.Remove(item);
                        break;
                    }
                }
            }
            sortedListGameJA.AddRange(tempListGameJA);
            Config.listGame.AddRange(sortedListGameJA);
            Config.curServerIp = (string)Config.listGame[0]["ip_dm"];
            PlayerPrefs.SetString("curServerIp", Config.curServerIp);
        }
        Debug.Log("=-=-=-=-=-=-=-=-=- list agam");
        Debug.Log(Config.listGame);
        if (jConfig.ContainsKey("listTop"))
        {
            Config.listRankGame = jConfig["listTop"] as JArray;//array
        }
        else Config.listRankGame.Clear();
        if (jConfig.ContainsKey("u_chat_fb"))
            Config.u_chat_fb = (string)jConfig["u_chat_fb"];
        else Config.u_chat_fb = "";
        if (jConfig.ContainsKey("infoChip"))
        {
            Config.infoChip = (string)jConfig["infoChip"];
        }
        else
        {
            Config.infoChip = "";
        }
        if (jConfig.ContainsKey("infoDT"))
            Config.infoDT = (string)jConfig["infoDT"];
        else Config.infoDT = "";
        if (jConfig.ContainsKey("infoBNF"))
        {
            Config.infoBNF = (string)jConfig["infoBNF"];
            getInfoBenefit((res) =>
            {
                if (res == "") return;
                var objData = JObject.Parse(res);
                if (objData.ContainsKey("jackpot"))
                {
                    Config.listRuleJackPot.Clear();

                    var data = (JArray)objData["jackpot"];

                    for (var i = 0; i < data.Count; i++)
                    {
                        JObject item = new JObject();
                        item["gameid"] = data[i]["gameid"];
                        JArray arrMark = new JArray();
                        JArray arrChip = new JArray();
                        JArray mark = (JArray)data[i]["mark"];
                        JArray chip = (JArray)data[i]["chip"];

                        for (var id = 0; id < mark.Count; id++)
                        {
                            arrMark.Add(mark[id]);
                            arrChip.Add(chip[id]);
                        }
                        item["listMark"] = arrMark;
                        item["listChip"] = arrChip;
                        Config.listRuleJackPot.Add(item);
                        Config.listVipBonusJackPot.Add(data[i]["bonus_vip"]);
                    }
                }

                if (objData.ContainsKey("agContactAd"))
                    Config.agContactAd = (int)objData["agContactAd"];
                if (objData.ContainsKey("agRename"))
                    Config.agRename = (int)objData["agRename"];

            });
        }
        if (jConfig.ContainsKey("url_rule_js_new"))
            Config.url_rule = (string)jConfig["url_rule_js_new"];
        else
            Config.url_rule = "";
        if (jConfig.ContainsKey("url_help"))
            Config.url_help = (string)jConfig["url_help"];
        else
            Config.url_help = "";
        if (jConfig.ContainsKey("url_rule_refGuide"))
            Config.url_rule_refGuide = (string)jConfig["url_rule_refGuide"];
        if (jConfig.ContainsKey("delayNoti"))
            Config.delayNoti = jConfig["delayNoti"] as JArray;//array
        Config.data0 = jConfig.ContainsKey("") ? (bool)jConfig["data0"] : false;
        if (jConfig.ContainsKey("infoUser"))
            Config.infoUser = (string)jConfig["infoUser"];
        else
            Config.infoUser = "";

        if (jConfig.ContainsKey("newest_versionUrl"))
            Config.newest_versionUrl = (string)jConfig["newest_versionUrl"];
        if (UIManager.instance != null)
        {
            var umode = jConfig.ContainsKey("umode") ? (int)jConfig["umode"] : 0;
            var uop1 = jConfig.ContainsKey("uop1") ? (string)jConfig["uop1"] : "";
            var uop2 = jConfig.ContainsKey("uop2") ? (string)jConfig["uop2"] : "";
            var umsg = jConfig.ContainsKey("umsg") ? (string)jConfig["umsg"] : "";
            var utar = jConfig.ContainsKey("utar") ? (string)jConfig["utar"] : "";
            updateConfigUmode(umode, uop1, uop2, utar, umsg);
            UIManager.instance.refreshUIFromConfig();
        }
        if (jConfig.ContainsKey("url_cdn")) Config.Bundle_URL = (string)jConfig["url_cdn"];
        Debug.Log((string)jConfig["url_cdn"] + "check xem là cái url_cdn");
        PlayerPrefs.Save();
        _IsConfigLoaded = true;
    }

    void handleUserInfo(string strData)
    {
        // -=-= handleUserInfo { "bundleID":"7E26B7BB-77C6-5938-AF2B-401DFB79724A","version":"1.00","operatorID":7000,"os":"android_unity","publisher":"dummy_co_1_10","disID":1006,"ketPhe":5,"is_dt":true,"ketT":true,"ket":true,"ismaqt":true,"is_bl_salert":true,"is_bl_fb":true,"is_xs":false}
        Logging.Log("-=-=handleUserInfo " + strData);
        JObject jConfig = null;
        try
        {
            jConfig = JObject.Parse(strData);
        }
        catch (Exception e)
        {
            Logging.LogException(e);
        }

        if (jConfig == null) return;
        Logging.Log("-------------------->Config Game<------------------>\n" + jConfig);

        if (jConfig.ContainsKey("disID"))
            Config.disID = (int)jConfig["disID"];

        Config.ketPhe = jConfig.ContainsKey("ketPhe") ? (int)jConfig["ketPhe"] : 10;
        Config.is_dt = jConfig.ContainsKey("is_dt") ? (bool)jConfig["is_dt"] : false;
        Config.ketT = jConfig.ContainsKey("ketT") ? (bool)jConfig["ketT"] : false;
        Config.ket = jConfig.ContainsKey("ket") ? (bool)jConfig["ket"] : false;
        Config.ket = false; // bản này không dùng chức năng két
        Config.ismaqt = jConfig.ContainsKey("ismaqt") ? (bool)jConfig["ismaqt"] : false;
        Config.is_bl_salert = jConfig.ContainsKey("is_bl_salert") ? (bool)jConfig["is_bl_salert"] : false;
        Config.is_bl_fb = jConfig.ContainsKey("is_bl_fb") ? (bool)jConfig["is_bl_fb"] : false;
        Config.is_xs = jConfig.ContainsKey("is_xs") ? (bool)jConfig["is_xs"] : false;
        Config.show_new_alert = jConfig.ContainsKey("show_new_alert") ? (bool)jConfig["show_new_alert"] : false;
        if (jConfig.ContainsKey("linkci"))
        {
            Config.ApkFullUrl = (string)jConfig["linkci"];
            if (Config.typeLogin == LOGIN_TYPE.NORMAL)
            {
                Config.ApkFullUrl = Config.ApkFullUrl.Replace("%username%", Config.user_name);
                Config.ApkFullUrl = Config.ApkFullUrl.Replace("%password%", Config.user_pass);
            }
            else if (Config.typeLogin == LOGIN_TYPE.PLAYNOW)
            {
                Config.ApkFullUrl = Config.ApkFullUrl.Replace("%username%", UIManager.instance.loginView.accPlayNow);
                Config.ApkFullUrl = Config.ApkFullUrl.Replace("%password%", UIManager.instance.loginView.passPlayNow);
            }
        }
        else Config.ApkFullUrl = "";
        UIManager.instance.refreshUIFromConfig();
    }

    void updateConfigUmode(int umode, string uop1, string uop2, string utar, string umsg)
    {
        //// let umode = 0; /*FIXED CHANGE WHEN RELEASE*/
        umode = 0;//dev de test
        switch (umode)
        {
            case 0: // mode == 0, vao thang ko can hoi
                    //cc.NGWlog('umode0: show login');
                break;
            case 1: // mode == 1, hoi update, 2 lua chon
                UIManager.instance.showDialog(umsg, uop1, () =>
                {
                    Application.OpenURL(utar);
                    Application.Quit();
                }, uop2);
                break;
            case 2: // mode == 2, hoi update, khong lua chon
                UIManager.instance.showDialog(umsg, uop1, () =>
                {
                    Application.OpenURL(utar);
                    Application.Quit();
                });
                break;
            case 3: // mode == 3, thong bao, 1 lua chon OK va vao game
                UIManager.instance.showMessageBox(umsg);
                break;
            case 4:// mode == 4, thong bao, 1 lua chon OK va finish
                UIManager.instance.showMessageBox(umsg, () =>
                {
                    Application.Quit();
                });
                break;
        }
    }
}
