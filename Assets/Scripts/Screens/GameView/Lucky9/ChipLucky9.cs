using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChipLucky9 : MonoBehaviour
{
    public Lucky9View gameView; 

    public void onMove(Vector3 pos, float time)
    {
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 180f));

        transform.DOLocalMove(pos, time).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            transform.DOScale(new Vector3(2, 2, 2), 0.2f).OnComplete(() =>
            {
                gameView.putChip(this);
            });
        });
    }
}
