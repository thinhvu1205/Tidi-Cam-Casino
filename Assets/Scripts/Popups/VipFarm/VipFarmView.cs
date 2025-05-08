using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using Globals;

public class VipFarmView : BaseView
{
    public static VipFarmView instance = null;
    [SerializeField] TextMeshProUGUI txtMoneyReceive, txtFarmPercent, txtMoneyReward;
    [SerializeField] LevelInfo lvNext, lvCurrent;
    [SerializeField] GameObject gameObjectArrow, btnReceive, lightTim, lightVang;
    [SerializeField] List<SkeletonGraphic> animTrees = new();
    [SerializeField] List<SkeletonDataAsset> listAnimTree = new();
    [SerializeField] SkeletonGraphic animReward;
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        txtMoneyReward.transform.parent.gameObject.SetActive(false);
        SetData();
        // SocketSend.getFarmInfo();
        //JObject data = new JObject();
        //data["farmLevel"] = 9;
        //data["farmPercent"] = 1f;
        //data["currentReward"] = 9000000;
        //data["nextReward"] = 10000000;
        //HandleInfo(data);
    }

    private void SetData()
    {
        //{ "farmLevel":10,"farmPercent":0.08,"currentReward":10000000,"nextReward":10000000,"evt":"farmInfo"}
        var farmLevel = (int)Config.dataVipFarm["farmLevel"];
        var farmPercent = (float)Config.dataVipFarm["farmPercent"];
        var currentReward = (int)Config.dataVipFarm["currentReward"];
        var nextReward = (int)Config.dataVipFarm["nextReward"];

        if (farmLevel >= 10)
        {
            lvCurrent.gameObject.SetActive(false);
            gameObjectArrow.SetActive(false);
            lvNext.SetInfo(farmLevel, currentReward, true);
        }
        else
        {
            lvCurrent.gameObject.SetActive(true);
            gameObjectArrow.SetActive(true);
            lvCurrent.SetInfo(farmLevel, currentReward, false);
            lvNext.SetInfo(farmLevel + 1, nextReward, true);
        }

        lightTim.SetActive(farmPercent >= 100);
        lightVang.SetActive(farmPercent >= 100);
        btnReceive.gameObject.SetActive(UIManager.instance.lobbyView.gameObject.activeSelf && farmPercent >= 100);

        txtFarmPercent.text = farmPercent + "%";
        txtMoneyReceive.text = Globals.Config.FormatNumber(currentReward);
        var indexPer = 1;
        if (farmPercent > 25f && farmPercent <= 50f)
        {
            indexPer = 2;
        }
        else if (farmPercent > 50 && farmPercent <= 75f)
        {
            indexPer = 3;
        }

        else if (farmPercent >= 100f)
        {
            indexPer = 4;
        }
        animTrees.ForEach(tree =>
        {
            tree.skeletonDataAsset = listAnimTree[farmLevel - 2];
            tree.Initialize(true);
            tree.AnimationState.SetAnimation(0, "V" + farmLevel + "_" + indexPer, true);
        });
    }
    public void HandleReward(JObject data)
    {
        SocketSend.sendUAG();
        //{ "evt":"farmReward","value":0,"msg":"get reward faild"}
        var value = (int)data["value"];
        if (value > 0)
        {
            txtMoneyReward.transform.parent.gameObject.SetActive(true);
            txtMoneyReward.text = Globals.Config.FormatNumber(value);

            animReward.AnimationState.SetAnimation(0, "animation", false);
            DOTween.Sequence().AppendInterval(3.75f).AppendCallback(() =>
            {
                txtMoneyReward.transform.parent.gameObject.SetActive(false);
            }).AppendInterval(1.0f).AppendCallback(() =>
            {
                hide();
            });
        }
        else
        {
            UIManager.instance.showMessageBox((string)data["msg"]);
        }
    }


    public void OnClickReceive()
    {
        SoundManager.instance.soundClick();
        SocketSend.getFarmReward();
        //JObject data = new JObject();
        //data["value"] = 10000000;
        //data["msg"] = "get reward successfully!";
        //HandleReward(data);
    }
}
