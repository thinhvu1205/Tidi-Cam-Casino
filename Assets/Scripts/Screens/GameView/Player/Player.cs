using Globals;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public string namePl, displayName;
    public int typeCard;
    public PlayerView playerView;
    public int _indexDynamic, vip = 0, avatar_id = 0, lastPlay = 0, agBet = 0;
    public long ag = 0;
    public string fid = "";

    public string avatar_url = "";
    public bool is_host = false, is_ready = false, is_turn = false, is_dealer = false, isFold = false, isSpecial = false;

    public bool isSpecialQb2T = false;

    public int score = 0;
    public int idVip = 0;

    // BINH
    public bool mauBinhSoBai = false;
    public bool mauBinh_BL = false;
    public bool isSapLang = false;
    public int timeSwapCard = 0;
    public int mauBinh_M = 0;
    public int mauBinh_MB = 0;
    public int totalPoint = 0;
    public int scoreChi1 = 0;
    public int scoreChi2 = 0;
    public int scoreChi3 = 0;
    public int bonusChi1 = 0;
    public int bonusChi2 = 0;
    public int bonusChi3 = 0;
    public JArray arrBinhWin;
    public List<int> jcards = new List<int>();
    public List<Card> vectorChi1 = new List<Card>();
    public List<Card> vectorChi2 = new List<Card>();
    public List<Card> vectorChi3 = new List<Card>();
    // BINH

    // LUCKY9
    public int point = 0;
    public int rate = 0;
    // LUCKY9


    public List<Card> vectorCard = new List<Card>();
    public List<Card> vectorCardD = new List<Card>();
    public List<List<Card>> vectorCardD2 = new List<List<Card>>();
    public List<int> arrCodeCard = new List<int>();
    public Dictionary<int, List<Transform>> sabongBetChips = new();
    public void updatePlayerView()
    {
        setName();
        setAvatar();
        setAg();
        playerView.isThisPlayer = Globals.User.userMain.Userid == id;
        playerView.setCallbackClick(() => { UIManager.instance.gameView.onClickInfoPlayer(this); });
    }

    public void clearAllCard()
    {
    }

    public void setHost(bool _isHost)
    {
        this.is_host = _isHost;
    }


    public void setReady(bool isReady)
    {
    }

    public void setTurn(bool isTurn, float timeTurn = 20, float timeVibrate = 5f)
    {
        is_turn = isTurn;
        playerView.setTurn(isTurn, timeTurn, id == Globals.User.userMain.Userid, timeVibrate);
    }


    public void setName()
    {
        playerView.setName((displayName != null ? displayName : namePl));
    }

    public void setAvatar()
    {
        playerView.setAvatar(avatar_id, namePl, fid, vip);
    }

    public void updateItemVipFromSV(int id)
    {
        Debug.Log("-=-=updateItemVipFromSV " + id);
        idVip = id;
        playerView.updateItemVipFromSV(id);
    }

    public void updateItemVip(int vip, int idPosTongits = -1)
    {
        playerView.updateItemVip(idVip, vip, idPosTongits);
    }

    public void setDealer(bool state, bool isLeft = false, bool isUp = false)
    {
        playerView.showDealer(state, isLeft, isUp);
    }
    public void setAg()
    {
        playerView.setAg(ag);
    }
    public void updateMoney()
    {
        if (playerView != null)
            playerView.setAg(ag);
    }
    public void setDark(bool state)
    {
        if (playerView != null)
            playerView.setDark(state);
    }

    public bool getIsTurn()
    {
        //return playerView.getIsTurn();

        return is_turn;
    }
}
