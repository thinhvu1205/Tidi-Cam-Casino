using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;
using DG.Tweening;
using Globals;

public class ButtonVipFarm : MonoBehaviour
{
    [SerializeField] private List<Sprite> listFG = new List<Sprite>();
    [SerializeField] private TextMeshProUGUI txtPercentVipFarm;
    [SerializeField] private Image imgFG;
    [SerializeField] private SkeletonGraphic animBar, animScore;
    private float _farmPercent;

    #region Button
    public void OnClickVipFarm()
    {
        SoundManager.instance.soundClick();
        UIManager.instance.openVipFarm();
    }
    #endregion
    public void SetData(float farmPercent)
    {
        _farmPercent = farmPercent;
        txtPercentVipFarm.text = farmPercent + "%";

        animBar.AnimationState.SetAnimation(0, "bar_run", true);
        animScore.AnimationState.SetAnimation(0, farmPercent >= 100f ? "box_fullscore" : "box_normal", true);
        imgFG.sprite = listFG[farmPercent >= 100f ? 1 : 0];
        //imgFG.fillAmount = farmPercent;
        imgFG.DOFillAmount(farmPercent / 100, 0.3f);

        var pos = animBar.transform.localPosition;
        var sizeW = imgFG.rectTransform.rect.width;
        pos.x = farmPercent / 100 * sizeW - sizeW / 2;
        animBar.transform.localPosition = pos;
    }
    public float GetFarmPercent()
    {
        return _farmPercent;
    }
    private void OnDestroy()
    {
        UIManager.instance.UpdateVipFarmsList(this, false);
    }
    private void OnEnable()
    {
        if (UIManager.instance != null) SocketSend.getFarmInfo();
    }
    private void Start()
    {
        if (User.userMain != null) gameObject.SetActive(User.userMain.VIP > 1);
        if (UIManager.instance != null) UIManager.instance.SetDataVipFarmList();
        if (gameObject.activeSelf) StartCoroutine(ContinuouslyGetVipFarm());
    }
    private IEnumerator ContinuouslyGetVipFarm()
    {
        while (UIManager.instance.gameView != null && gameObject.activeSelf)
        {
            yield return new WaitForSecondsRealtime(20f);
            SocketSend.getFarmInfo();
        }
    }
    private void Awake()
    {
        if (UIManager.instance != null) UIManager.instance.UpdateVipFarmsList(this, true);
    }
}
