using UnityEngine;

public class LoginRequestPacket
{
    [SerializeField]
    int classId = Globals.CMD.LOGIN_REQUEST;
    [SerializeField]
    string user;
    [SerializeField]
    string password;
    [SerializeField]
    int operatorid;
    [SerializeField]
    byte[] credentials;

    public LoginRequestPacket(string user, string password, int operatorid, byte[] credentials)
    {
        this.user = user;
        this.password = password;
        this.operatorid = operatorid;
        this.credentials = credentials;
    }
}
