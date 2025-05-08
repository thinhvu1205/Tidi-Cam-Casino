using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InfoPlayerInGame : BaseView
{
    [SerializeField] TextMeshProUGUI txtName, txtID, txtChip;
    [SerializeField] Avatar avatar;

    [SerializeField] VipContainer vipContainer;
    [SerializeField] private GameObject m_ActionButtons;
    private bool blockSpamTabGame = false;
    Player player;
    async public void setInfo(Player _player, bool showActionButtons = true)
    {
        m_ActionButtons.SetActive(showActionButtons);
        player = _player;
        //pname = (string)jsonData["name"];
        //int avatarId = (int)jsonData["avatar"];
        //idPlayer = (string)jsonData["uid"];
        //string fbId = (string)jsonData["fbid"];
        //int chip = (int)jsonData["ag"];
        //int vip = (int)jsonData["vip"];
        //pname = player.namePl;
        //int avatarId = player.avatar_id;
        //idPlayer = player.id;
        //string fbId = player.fid;
        //int vip = player.vip;
        //int chip = player.ag;

        //{\"id\":199182,\"A\":true,\"N\":\"Anuwat Somchat\",\"Url\":\"fb.1136983050395482\",\"AG\":40849,\"AGC\":5000,\"LQ\":0,\"VIP\":4,\"isStart\":true,\"IK\":0,\"Arr\":[0,0,0],\"Av\":999,\"FId\":0,\"UserType\":11,\"TotalAG\":0,\"rate\":0,\"score\":0,\"displayName\":\"Anuwat Somchat\",\"keyObjectInGame\":0}
        //{\"name\":\"Anuwat Somchat\",\"namelq\":\"\",\"avatar\":999,\"online\":0,\"vip\":0,\"ag\":0,\"uid\":199182,\"idtable\":0,\"status\":\"...\",\"level\":0,\"fbid\":1136983050395482}
        Globals.Logging.Log("-=-=-= setinfo player");
        if (player.displayName != "" && player.displayName != null)
        {
            txtName.text = player.displayName;
        }
        else
        {
            txtName.text = player.namePl;
        }

        txtID.text = "ID: " + player.id;
        txtChip.text = Globals.Config.FormatNumber(player.ag);
        //avatar.loadAvatar(avatarId, name, fbId);
        avatar.loadAvatarAsync(player.avatar_id, txtName.text, player.fid);
        vipContainer.setVip(player.vip);
        avatar.setVip(player.vip);
    }

    public void onClickChatAction(int action)
    {
        if (!blockSpamTabGame)
        {
            if (player.id == Globals.User.userMain.Userid)
            {
                for (var i = 0; i < Mathf.Min(UIManager.instance.gameView.GetCountListPosView(), UIManager.instance.gameView.players.Count); i++)
                {
                    var pl = UIManager.instance.gameView.players[i];
                    if (pl.id != Globals.User.userMain.Userid)
                    {
                        SocketSend.sendChatEmo(Globals.User.userMain.displayName, pl.displayName == null ? pl.namePl : pl.displayName, 2, action.ToString(), false);
                    }
                    UIManager.instance.SendChatEmoToHiddenPlayers = true;
                }
            }
            else
            {
                SocketSend.sendChatEmo(Globals.User.userMain.displayName, player.displayName == null ? player.namePl : player.displayName, 2, action.ToString());
            }
            hide();
            blockSpamTabGame = true;
            DOTween.Kill("blockSpamIconNem");
            DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
            {
                blockSpamTabGame = false;
            }).SetId("blockSpamIconNem");
        }
        else
        {

        }

    }
}
