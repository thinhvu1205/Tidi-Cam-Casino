using Newtonsoft.Json.Linq;
using UnityEngine;

public class JoinRequestPacket
{
    [SerializeField]
    public int classId = Globals.CMD.JOIN_REQUEST;
    [SerializeField]
    public int tableid;
    [SerializeField]
    public int seat;

    //[SerializeField]
    //public string str_params;

    //this.tableid = { }; // int;
    //this.seat = { }; // int;
    //this.params = [];
}
