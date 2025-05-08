using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

public class SixiangChooseGameBonus : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    SkeletonGraphic spinebg;

    [SerializeField]
    Transform container;

    private bool isSelected = false;

    // Update is called once per frame

    protected void OnEnable()
    {
        container.localScale = Vector2.one;
        spinebg.Initialize(true);
        spinebg.AnimationState.SetAnimation(0, "eng", false);

    }
    public void onClickSelectGame(int index)
    {
        SocketSend.sendSelectMiniGame(Globals.ACTION_SLOT_SIXIANG.selectBonusGame, index.ToString());
    }
    public void onClose()
    {
        container.DOScale(new Vector2(0.8f, 0.8f), 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
