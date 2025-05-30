using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerViewTienlen : PlayerView
{
    [SerializeField] private SkeletonGraphic m_AniWin;
    [SerializeField] private SkeletonGraphic m_AniWin1;
    public void ShowAniWin()
    {
        m_AniWin.gameObject.SetActive(true);
        m_AniWin.transform.SetAsLastSibling();
        m_AniWin.Initialize(true);
        m_AniWin.AnimationState.SetAnimation(0, "win", false);
        // m_AniWin1.gameObject.SetActive(true);
        // m_AniWin1.transform.SetAsLastSibling();
        // m_AniWin1.Initialize(true);
        // m_AniWin1.AnimationState.SetAnimation(0, "win", false);
        Debug.Log("Có chạy vào show ani win");

        StartCoroutine(StopAniWinAfterDelay(3f));
    }

    private IEnumerator StopAniWinAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_AniWin.AnimationState.ClearTrack(0);
        m_AniWin.gameObject.SetActive(false);
        // m_AniWin1.AnimationState.ClearTrack(0);
        // m_AniWin1.gameObject.SetActive(false);
    }
    public string GetNamePlayer()
    {
        return txtName.text;
    }

}
