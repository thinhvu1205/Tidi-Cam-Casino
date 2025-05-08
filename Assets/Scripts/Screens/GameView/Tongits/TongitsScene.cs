using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;

public class TongitsScene : MonoBehaviour
{
    [SerializeField] GameObject dark_bg;
    [SerializeField] SkeletonGraphic tongits_ani;
    [SerializeField] GameObject challenger;
    [SerializeField] Avatar challenger_ava;
    [SerializeField] TextMeshProUGUI challenger_name;

    public int currentFighterDIndex = -1;

    void Start()
    {
        //currentFighterDIndex = -1;
    }

    public void startScene(Player player)
    {
        tongits_ani.gameObject.SetActive(true);
        tongits_ani.AnimationState.SetAnimation(0, "tongits", true);
        tongits_ani.Initialize(true);
        tongits_ani.AnimationState.Complete += delegate
        {
            tongits_ani.gameObject.SetActive(false);
        };
        challenger_ava.loadAvatar(player.avatar_id, player.displayName, player.fid);
        challenger_ava.setVip(player.vip);
        string name = player.displayName;
        if (name.Length > 15)
        {
            name = name.Substring(0, 15) + "...";
        }
        challenger_name.text = name;
        currentFighterDIndex = player._indexDynamic;
        Destroy(gameObject, 4f);
    }
}
