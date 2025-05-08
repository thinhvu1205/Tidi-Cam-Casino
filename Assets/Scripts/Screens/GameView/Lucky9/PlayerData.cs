using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData: MonoBehaviour
{
    public int pointWin;
    public Player player;

    public PlayerData(int pointWin, Player player)
    {
        this.pointWin = pointWin;
        this.player = player;
    }
}
