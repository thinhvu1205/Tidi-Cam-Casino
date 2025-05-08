using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;



#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CCFS))]
public class CCFSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CCFS mTest = (CCFS)target;
        mTest.type = (CCFS.TYPE)EditorGUILayout.EnumPopup("Type", mTest.type);
        switch (mTest.type)
        {
            case CCFS.TYPE.TEXT:
                {
                    mTest.key = EditorGUILayout.TextField("Key", mTest.key);
                    mTest.isUpper = EditorGUILayout.Toggle("isUpper", mTest.isUpper);
                    break;
                }
            case CCFS.TYPE.IMAGE:
                {
                    var list = mTest.sprites;
                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("Sprites", list.Count));
                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add(null);
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = (Sprite)EditorGUILayout.ObjectField("Element " + i, list[i], typeof(Sprite), false);
                    }
                    break;
                }
            case CCFS.TYPE.SKELETON:
                {
                    var list = mTest.animName;
                    int newCount = Mathf.Max(0, EditorGUILayout.IntField("Skeleton Animations", list.Count));
                    while (newCount < list.Count)
                        list.RemoveAt(list.Count - 1);
                    while (newCount > list.Count)
                        list.Add("");
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i] = EditorGUILayout.TextField("Element " + i, list[i]);
                    }
                    break;
                }
        }
    }
}
#endif

public class CCFS : MonoBehaviour
{
    public enum TYPE
    {
        TEXT,
        IMAGE,
        SKELETON
    };
    public TYPE type = TYPE.TEXT;
    public string key = "";

    [Tooltip("0-Eng, 1-Thai")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("0-Eng, 1-Thai")]
    public List<string> animName = new List<string>();

    public bool isUpper = false;

    public void RefreshUI()
    {
        switch (type)
        {
            case TYPE.TEXT:
                {
                    TextMeshProUGUI textMeshProUGUI = GetComponent<TextMeshProUGUI>();
                    if (textMeshProUGUI != null)
                    {
                        if (isUpper)
                        {
                            textMeshProUGUI.text = Globals.Config.getTextConfig(key).ToUpper();
                        }
                        else
                            textMeshProUGUI.text = Globals.Config.getTextConfig(key);
                    }
                    else
                    {
                        Text textCom = GetComponent<Text>();
                        if (textCom != null)
                        {
                            if (isUpper)
                            {
                                textCom.text = Globals.Config.getTextConfig(key).ToUpper();
                                Debug.Log(" textCom.text=" + textCom.text);
                            }
                            else
                                textCom.text = Globals.Config.getTextConfig(key);
                        }
                    }
                    return;
                }
            case TYPE.IMAGE:
                {
                    Image imge = GetComponent<Image>();
                    if (imge == null) return;
                    imge.sprite = Globals.Config.language == "EN" ? sprites[0] : sprites[1];
                    imge.SetNativeSize();
                    return;
                }
            case TYPE.SKELETON:
                {
                    SkeletonGraphic anim = GetComponent<SkeletonGraphic>();
                    if (anim == null) return;
                    anim.AnimationState.SetAnimation(0, Globals.Config.language == "EN" ? animName[0] : animName[1], false);
                    return;
                }
        }
    }
    private void OnEnable()
    {
        StartCoroutine(delay1FrameThenRefresh());
        IEnumerator delay1FrameThenRefresh()
        {   // chậm 1 frame tránh bug missing lúc đầu mở game
            yield return null;
            RefreshUI();
        }
    }
}
