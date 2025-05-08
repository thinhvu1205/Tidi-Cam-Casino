using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ItemChatAction : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] List<Sprite> lsAction = new List<Sprite>();
    [SerializeField] SkeletonGraphic skeletonGraphic;
    [SerializeField] List<SkeletonDataAsset> lsAnimData = new List<SkeletonDataAsset>();
    public IEnumerator setData(int idAnimation, Vector3 targetV3)
    {
        Globals.Logging.Log("-=-=idAnimation " + idAnimation);
        if (lsAction[idAnimation] != null)
        {
            img.gameObject.SetActive(true);
            img.sprite = lsAction[idAnimation];
            img.SetNativeSize();
        }
        yield return new WaitForSeconds(.1f);
        transform.DOMove(targetV3, 1).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            img.gameObject.SetActive(false);
            skeletonGraphic.gameObject.SetActive(true);
            skeletonGraphic.skeletonDataAsset = lsAnimData[idAnimation];
            skeletonGraphic.Initialize(true);
            skeletonGraphic.startingAnimation = "animation";
            skeletonGraphic.startingLoop = false;
            skeletonGraphic.AnimationState.Complete += delegate
            {
                Destroy(gameObject);
            };
            string sound = "";
            switch (idAnimation)
            {
                case 0:
                    sound = Globals.SOUND_CHAT.BOOM;
                    break;
                case 1:
                    sound = Globals.SOUND_CHAT.KISS;
                    break;
                case 2:
                    sound = Globals.SOUND_CHAT.ROSE;
                    break;
                case 3:
                    sound = Globals.SOUND_CHAT.BEER;
                    break;
                case 4:
                    sound = Globals.SOUND_CHAT.TOMATO;
                    break;
                case 5:
                    sound = Globals.SOUND_CHAT.WATER;
                    break;
            }
            SoundManager.instance.playEffectFromPath(sound);
        });
    }
}