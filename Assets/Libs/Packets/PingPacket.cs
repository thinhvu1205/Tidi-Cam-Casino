using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPacket
{
    [SerializeField]
    public int classId = Globals.CMD.PING;
}
