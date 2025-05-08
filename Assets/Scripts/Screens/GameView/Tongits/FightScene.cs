using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System;
using Globals;


public class FightScene : MonoBehaviour
{
    [SerializeField] GameObject dark_bg;
    [SerializeField] SkeletonGraphic fight_ani;
    //[SerializeField] SkeletonGraphic solo_ani;
    [SerializeField] GameObject fight_buttons;
    [SerializeField] GameObject challenger_left;
    [SerializeField] Avatar left_ava;
    [SerializeField] SkeletonGraphic left_anim;
    [SerializeField] TextMeshProUGUI left_name;
    [SerializeField] GameObject challenger_right;
    [SerializeField] Avatar right_ava;
    [SerializeField] SkeletonGraphic right_anim;
    [SerializeField] TextMeshProUGUI right_name;
    [SerializeField] GameObject challenger;
    [SerializeField] Avatar challenger_ava;
    [SerializeField] TextMeshProUGUI challenger_name;
    [SerializeField] TextMeshProUGUI your_point;
    [SerializeField] Image imgTime;
    //[SerializeField] SkeletonGraphic aniPhao;
    //[SerializeField] Image imgTime;
    [SerializeField] Sprite[] bkg_left_type;
    [SerializeField] Sprite[] bgk_right_type;
    [SerializeField] Sprite[] fight_type;
    [SerializeField] Sprite[] fight_type_phi;
    [SerializeField] SkeletonDataAsset[] listAni;

    int currentFighterDIndex = -1;
    float time_turn_cur = 0f;
    int time_turn = 0;
    bool isLeft = false;
    bool isRight = false;
    bool isVibrate;
    Player challengerSolo = null;
    string languaSave;
    TongitsView tongits;

    void Start()
    {
        languaSave = Globals.Config.language;
        tongits = gameObject.GetComponentInParent<TongitsView>();
    }
    private void OnEnable()
    {
        isVibrate = true;
    }

    void Update()
    {
        time_turn_cur += Time.deltaTime;
        if (time_turn_cur <= time_turn)
        {
            float timeLeft = time_turn - time_turn_cur;
            imgTime.fillAmount = timeLeft / time_turn;
            if (isVibrate && timeLeft <= 3f && imgTime.gameObject.activeSelf)
            {
                isVibrate = false;
                Config.Vibration();
            }
            // angleNow tinh theo radian, 1rad = 180/?
            float angleNow = -time_turn_cur * ((360 / time_turn / 180) * Mathf.PI);
            float x = 65 * Mathf.Cos(angleNow);
            float y = 65 * Mathf.Sin(angleNow);
            //aniPhao.transform.localPosition = new Vector2(x, y);
        }
        else
        {
            imgTime.gameObject.SetActive(false);
            fight_buttons.SetActive(false);
        }
    }

    public void startFight(Player player, int point)
    {
        challengerSolo = player;
        time_turn = 12;
        challenger_left.transform.position = Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11 ? new Vector2(-850f, -150f) : new Vector2(-850f, -200f);
        challenger_left.SetActive(false);
        challenger_right.transform.position = Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11 ? new Vector2(850f, -150f) : new Vector2(850f, -200f);
        challenger_right.SetActive(false);

        string aniStr = "eng";
        if (languaSave != "EN")
        {
            aniStr = "phi";
        }

        fight_ani.gameObject.SetActive(true);
        fight_ani.startingAnimation = aniStr;
        fight_ani.AnimationState.SetAnimation(0, aniStr, true);
        fight_ani.Initialize(true);


        //dark_bg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        //dark_bg.transform.GetComponent<Renderer>().material.DOFade(210f, 0.4f);
        challenger_ava.loadAvatar(player.avatar_id, player.displayName, player.fid);
        challenger_ava.setVip(player.vip);
        string name = player.displayName;
        if (name.Length > 15)
        {
            name = name.Substring(0, 15) + "...";
        }
        challenger_name.text = name;
        //GameManager.instance.checkFontText(challenger_name);
        currentFighterDIndex = player._indexDynamic;
        Debug.Log("currentFighterDIndex = " + currentFighterDIndex);
        if (currentFighterDIndex == 0 || point == -1)
        {
            fight_buttons.SetActive(false);
            your_point.gameObject.SetActive(false);
        }
        else
        {
            your_point.text = "Your score: " + point;
        }
    }

    public void playerFight(Player player, int type)
    {
        if (Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS11)
        {
            if (challengerSolo != null)
            {
                setLeftChallenger(challengerSolo, 3);
                fight_ani.gameObject.SetActive(false);
                challenger_left.SetActive(false);
                //solo_ani.SetActive(true);
                //solo_ani.GetComponent<SkeletonAnimation>().AnimationName = "animation";
            }
            imgTime.gameObject.SetActive(false);
            time_turn_cur = time_turn;
            setRightChallenger(player, type);
        }
        else
        {
            Debug.Log("currentFighterDIndex = " + currentFighterDIndex);
            if (currentFighterDIndex == 0)
            {
                if (player._indexDynamic == 1)
                {
                    setRightChallenger(player, type);
                }
                else
                {
                    setLeftChallenger(player, type);
                }
            }
            else
            {
                if (player._indexDynamic == 0)
                {
                    setLeftChallenger(player, type);
                    fight_buttons.SetActive(false);
                    imgTime.gameObject.SetActive(false);
                }
                else
                {
                    setRightChallenger(player, type);
                }
            }
        }

        StartCoroutine(DestroyNodeAfterDelay());
    }

    private IEnumerator DestroyNodeAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (this != null)
        {
            Debug.Log(TongitsView.CountPlayers);
            if (TongitsView.CountPlayers == 3)
            {
                if (isLeft && isRight)
                {
                    Debug.Log("DestroyNodeAfterDelay");
                    Destroy(gameObject);
                }
            }
            else
            {
                if (isLeft || isRight)
                {
                    Debug.Log("DestroyNodeAfterDelay");
                    Destroy(gameObject);
                }
            }
        }
    }

    public void setLeftChallenger(Player player, int type)
    {
        Debug.Log("Fight Scene === setLeftChallenger");

        isLeft = true;
        challenger_left.SetActive(true);

        // Load avatar texture
        left_ava.loadAvatar(player.avatar_id, player.displayName, player.fid);
        left_ava.setVip(player.vip);
        // Truncate name if it's too long
        string name = player.displayName;
        if (name.Length > 15)
        {
            name = name.Substring(0, 15) + "...";
        }

        // Set name and adjust font size if necessary
        left_name.text = name;
        //GameManager.instance.checkFontText(left_name);

        // Move to position
        Vector3 pos = getChallengerPosition(0);
        challenger_left.transform.DOLocalMove(pos, 0.2f).SetEase(Ease.InOutCubic);

        // Set background sprite
        Image bg = challenger_left.transform.Find("bg_challenger").GetComponent<Image>();
        bg.sprite = bkg_left_type[type];

        // Set animation
        left_anim.skeletonDataAsset = listAni[type];
        string aniStr = (languaSave != "EN") ? "phi" : "eng";
        left_anim.startingAnimation = aniStr;
        left_anim.AnimationState.SetAnimation(0, aniStr, false);
        left_anim.Initialize(true);
    }

    public void setRightChallenger(Player player, int type)
    {
        Debug.Log("Fight Scene === setRightChallenger");
        isRight = true;
        challenger_right.SetActive(true);

        // Load avatar texture
        right_ava.loadAvatar(player.avatar_id, player.displayName, player.fid);
        right_ava.setVip(player.vip);
        // Truncate name if it's too long
        string name = player.displayName;
        if (name.Length > 112)
        {
            name = name.Substring(0, 9) + "...";
        }

        // Set name and adjust font size if necessary
        right_name.text = name;
        //GameManager.instance.checkFontText(right_name);

        // Move to position
        Vector3 pos = getChallengerPosition(1);
        challenger_right.transform.DOLocalMove(pos, 0.2f).SetEase(Ease.InOutCubic);

        // Set background sprite
        Image bg = challenger_right.transform.Find("bg_challenger").GetComponent<Image>();
        bg.sprite = bgk_right_type[type];

        // Set animation
        right_anim.skeletonDataAsset = listAni[type];
        string aniStr = (languaSave != "EN") ? "phi" : "eng";
        right_anim.startingAnimation = aniStr;
        right_anim.AnimationState.SetAnimation(0, aniStr, false);
        right_anim.Initialize(true);
    }

    public Vector2 getChallengerPosition(int type)
    {
        if (Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_OLD || Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS || Globals.Config.curGameId == (int)Globals.GAMEID.TONGITS_JOKER)
        {
            return (type == 0) ? new Vector2(-325, -200) : new Vector2(325, -200);
        }
        else
        {
            return (type == 0) ? new Vector2(-310, 50) : new Vector2(310, -50);
        }
    }

    public void OnChallengeClick()
    {
        fight_buttons.SetActive(false);
        SocketSend.sendTgAcceptFight();
    }

    public void OnFoldClick()
    {
        fight_buttons.SetActive(false);
        List<List<int>> data = tongits.getDeclareData();
        SocketSend.sendTgUpdateCard(data);
        SocketSend.sendTgFold();
    }
}
