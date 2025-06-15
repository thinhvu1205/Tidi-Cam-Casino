using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultHistory : MonoBehaviour
{
    public Sprite[] images;
    public Image imageResult;
    public TextMeshProUGUI textResult;
    public SkeletonGraphic animVongSang;
    public int id;
    public void Init(int result, int num, bool isActiveAnim)
    {
        imageResult.sprite = images[num];
        imageResult.SetNativeSize();
        textResult.text = $"{result}";
        if (isActiveAnim)
        {
            animVongSang.AnimationState.SetAnimation(0, "", true);
        }
    }
}
