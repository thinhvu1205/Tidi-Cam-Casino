using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Globals;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    Queue<Action> jobsResend = new Queue<Action>();
    [HideInInspector] public ConnectionStatus connectionStatus = ConnectionStatus.NONE;
    WebSocket ws = null;
    Action _OnConnectCb;
    static WebSocketManager instance = null;
    public bool UserLogout;

    public WebSocketManager()
    {
    }

    public static WebSocketManager getInstance()
    {
        return instance;
    }

    public void Connect(Action callback)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (Config.isErrorNet) return;
            Config.isErrorNet = true;
            UIManager.instance.showMessageBox(Config.getTextConfig("err_network"));
            UIManager.instance.hideWatting();
            return;
        }
        UserLogout = true;
        _OnConnectCb = callback;
        Config.isErrorNet = false;
        stop();
        jobsResend.Clear();
        Config.curServerIp = "app-002.ngwcasino.com";
        Debug.Log(" Config.curServerI=" + Config.curServerIp);
        Debug.Log(" Config.PORT=" + Config.PORT);

        // Tạo WebSocket với cấu hình SSL
        ws = new WebSocket("wss://" + Config.curServerIp);
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ws.SslConfiguration.CheckCertificateRevocation = false;
        ws.Log.Level = WebSocketSharp.LogLevel.Trace; // Enable detailed logging

        Logging.Log("IP CONNECT:" + Config.curServerIp);
        connectionStatus = ConnectionStatus.CONNECTING;

        // Add certificate validation callback
        ws.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
        {
            if (errors != System.Net.Security.SslPolicyErrors.None)
            {
                Logging.Log($"SSL Certificate Error: {errors}");
                // Return true to accept certificate despite errors
                return true;
            }
            return true;
        };

        ws.ConnectAsync();

        ws.EmitOnPing = true;
        ws.WaitTime = TimeSpan.FromSeconds(30); // Tăng timeout lên 30 giây

        ws.OnError += (sender, e) =>
        {
            var wsError = e as WebSocketSharp.ErrorEventArgs;
            if (wsError != null && wsError.Exception != null)
            {
                Logging.Log($"WebSocket Error: {wsError.Message}\nException: {wsError.Exception}\nStackTrace: {wsError.Exception.StackTrace}");
            }
            _HandleOnErrorWebSocket();
        };
        ws.OnClose += (sender, e) =>
        {
            var wsClose = e as WebSocketSharp.CloseEventArgs;
            if (wsClose != null)
            {
                Logging.Log($"WebSocket Closed: Code={wsClose.Code}, Reason={wsClose.Reason}, WasClean={wsClose.WasClean}");
            }
            _HandleOnCloseWebSocket();
        };
        ws.OnOpen += (sender, e) => _HandleOnOpenWebSocket();
        ws.OnMessage += (sender, e) => _HandleOnMessageWebSocket(e.Data);
    }
    private void _HandleOnErrorWebSocket()
    {
        if (connectionStatus == ConnectionStatus.DISCONNECTED) return;
        connectionStatus = ConnectionStatus.DISCONNECTED;
        Logging.Log($"OnError: Status={connectionStatus}, IsAlive={ws?.IsAlive}, ReadyState={ws?.ReadyState}");
        UnityMainThread.instance.AddJob(() =>
        {
            UIManager.instance.showLoginScreen(false);
        });
    }
    private void _HandleOnCloseWebSocket()
    {
        if (connectionStatus == ConnectionStatus.DISCONNECTED) return;
        connectionStatus = ConnectionStatus.DISCONNECTED;
        Logging.Log($"OnClose: Status={connectionStatus}, IsAlive={ws?.IsAlive}, ReadyState={ws?.ReadyState}, UserLogout={UserLogout}");
        UnityMainThread.instance.AddJob(() =>
        {
            UIManager.instance.showLoginScreen(false);
            if (!UserLogout)
            {
                UIManager.instance.showDialog("You have logged in from another place!", "Confirm");
            }
        });
    }
    private void _HandleOnOpenWebSocket()
    {
        connectionStatus = ConnectionStatus.CONNECTED;

        _OnConnectCb?.Invoke();
        Logging.Log("OnOpen ");
        while (jobsResend.Count > 0)
            jobsResend.Dequeue().Invoke();
    }
    private void _HandleOnMessageWebSocket(string data)
    {
        UnityMainThread.instance.AddJob(() =>
            {
                UIManager.instance.hideWatting();
                JObject objData = JObject.Parse(data);
                int cmdId = (int)objData["classId"];
                switch (cmdId)
                {
                    case CMD.LOGIN_RESPONSE:
                        HandleData.handleLoginResponse(data);
                        break;
                    case CMD.SERVICE_TRANSPORT:
                        HandleData.handleServiceTransportPacket(data);
                        break;
                    case CMD.GAME_TRANSPORT:
                        HandleData.handleGameTransportPacket(data);
                        break;
                    case CMD.FORCE_LOGOUT:
                        HandleData.handleForcedLogoutPacket(data);
                        break;
                    case CMD.JOIN_RESPONSE:
                        HandleData.handleJoinResponsePacket(data);
                        break;
                    case CMD.LEAVE_RESPONSE:
                        HandleData.handleLeaveResponsePacket(data);
                        break;
                    case CMD.PING:
                        Logging.Log("PING PONG!!!!");
                        break;
                    default:
                        {
                            break;
                        }
                }
            });
    }

    public void runConnect()
    {


    }

    public void stop(bool isClearTask = true)
    {
        if (ws != null) ws.Close();
        if (isClearTask) jobsResend.Clear();
    }

    public bool IsAlive()
    {
        return ws != null && ws.IsAlive;
    }

    public void SendData(string dataSend)
    {
        if (connectionStatus == ConnectionStatus.CONNECTED && ws.ReadyState == WebSocketState.Open)
        {
            ws.SendAsync(dataSend, (msg) => { });
        }
        else
        {
            jobsResend.Enqueue(() =>
            {
                ws.SendAsync(dataSend, (msg) => { });
            });
        }

    }

    /**
     * Send a ServiceTransportPacket
     * @param {Number} pid player id
     * @param {Number} gameId gamed id
     * @param {Number} classId class id
     * @param {String} serviceContract name of service contract
     * @param {Array} byteArray game data
     */
    public void sendService(string strData, bool ping = true)
    {

        //if (NetworkManager.getInstance().statusConnect != FIREBASE.ConnectionStatus.CONNECTED) return;
        ServiceTransportPacket serviceTransport = new ServiceTransportPacket();
        serviceTransport.service = "com.athena.services.api.ServiceContract";
        serviceTransport.servicedata = Config.getByte(strData);// utf8.toByteArray(data);
        Debug.Log(User.userMain.Userid + "xem ở chỗ này cái id của người là gì");
        serviceTransport.pid = User.userMain.Userid;
        serviceTransport.seq = 1;
        serviceTransport.idtype = 1;
        //connector.sendProtocolObject(serviceTransport);
        SendData(JsonUtility.ToJson(serviceTransport));

        var objData = new JObject();
        var dataParse = JObject.Parse(strData);
        if (dataParse.ContainsKey("evt"))
        {
            objData["evt"] = dataParse["evt"];
        }
        else if (dataParse.ContainsKey("idevt"))
        {
            objData["idevt"] = dataParse["idevt"];
        }
        objData["data"] = strData;
        SocketIOManager.getInstance().emitSIOWithValue(objData, "ServiceTransportPacket", true);
    }
    /**
     * Send a Styx protocol object to a table. This protocol
     * object send to the game using a GameTransportPacket.
     *
     * @param {Number} pid player id
     * @param {Number} tableid table id
     * @param {Object} protocolObject Styx protocol object
     */
    public void sendDataGame(string strData)
    {
        //Logging.Log("sendDataGame:" + strData);
        GameTransportPacket gameTransportPacket = new GameTransportPacket();
        gameTransportPacket.pid = User.userMain.Userid;
        gameTransportPacket.tableid = Config.tableId;
        gameTransportPacket.gamedata = Config.Base64Encode(strData);
        SendData(JsonUtility.ToJson(gameTransportPacket));


        var objData = new JObject();
        var dataParse = JObject.Parse(strData);
        if (dataParse.ContainsKey("evt"))
        {
            objData["evt"] = dataParse["evt"];
        }
        else if (dataParse.ContainsKey("idevt"))
        {
            objData["idevt"] = dataParse["idevt"];
        }
        objData["data"] = strData;
        SocketIOManager.getInstance().emitSIOWithValue(objData, "GameTransportPacket", true);
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}