using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using UnityEngine;

public class MysteriousSlot : MonoBehaviour
{
    [SerializeField] private GameObject m_BgUnselectable, m_BgSelectable, m_UnselectedGold, m_SelectedGold, m_UnselectedBomb, m_SelectedBomb;
    [SerializeField] private SkeletonGraphic m_AnimBombSG;
    private Action _onClickCb;
    private int _id;

    #region Button
    public void DoClickOpen()
    {
        _onClickCb?.Invoke();
    }
    #endregion

    public MysteriousSlot TurnUnchosable(bool show) { m_BgUnselectable.SetActive(show); return this; }
    public MysteriousSlot TurnChosable(bool show) { m_BgSelectable.SetActive(show); return this; }
    public MysteriousSlot TurnUnchosenGold(bool show) { m_UnselectedGold.SetActive(show); return this; }
    public MysteriousSlot TurnChosenGold(bool show) { m_SelectedGold.SetActive(show); return this; }
    public MysteriousSlot TurnUnchosenBomb(bool show) { m_UnselectedBomb.SetActive(show); return this; }
    public MysteriousSlot TurnChosenBomb(bool show) { m_SelectedBomb.SetActive(show); return this; }
    public MysteriousSlot SetOnclickCB(Action cb) { _onClickCb = cb; return this; }
    public bool IsBomb() { return m_UnselectedBomb.activeSelf; }
    public int GetId() { return _id; }
    public MysteriousSlot SetId(int id) { _id = id; return this; }

    public void SetData(JObject jData)
    { //set state cac image trong nay theo data server tra ve san, mo ra chi dien, neu la o co bomb thi m_Bomb se enable
        m_BgUnselectable.SetActive(true);
    }
    private void Start()
    {
        // m_AnimBombSG.AnimationState.Complete += (trackentry) => { m_AnimBombSG.gameObject.SetActive(false); };
    }
}
