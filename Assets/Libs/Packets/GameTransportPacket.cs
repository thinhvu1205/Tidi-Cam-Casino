using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTransportPacket
{
    [SerializeField]
    public int classId = Globals.CMD.GAME_TRANSPORT;
    [SerializeField]
    public int tableid;
    [SerializeField]
    public int pid;
    [SerializeField]
    public string gamedata;
    [SerializeField]
    public int test;

    public string str_gamedata;
}
