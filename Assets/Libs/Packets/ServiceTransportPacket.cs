using UnityEngine;

//public class my_type {
//    public byte[] byteArr;
//    public string str;
//}

public class ServiceTransportPacket
{
    [SerializeField]
    public int classId = Globals.CMD.SERVICE_TRANSPORT;
    [SerializeField]
    public int pid;
    [SerializeField]
    public int seq;
    [SerializeField]
    public string service;
    [SerializeField]
    public int idtype;
    [SerializeField]
    public byte[] servicedata;

    public string str_servicedata;
}
