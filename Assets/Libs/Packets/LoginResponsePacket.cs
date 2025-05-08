using UnityEngine;

public class LoginResponsePacket
{
    public int classId = Globals.CMD.LOGIN_RESPONSE;
    public string screenname;
    public int pid;
    public string status;
    public int code;
    public string message;
    public string credentials;
}
