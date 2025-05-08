using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


//#if UNITY_EDITOR
//using UnityEditor;

//[CustomEditor(typeof(DropBoxCustom))]
//public class DropBoxEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DropBoxCustom mTest = (DropBoxCustom)target;


//        var listBtnDrop = mTest.listBtnDropbox;
//        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Button", listBtnDrop.Count));
//        while (newCount < listBtnDrop.Count)
//            listBtnDrop.RemoveAt(listBtnDrop.Count - 1);
//        while (newCount > listBtnDrop.Count)
//            listBtnDrop.Add(null);
//        for (int i = 0; i < listBtnDrop.Count; i++)
//        {
//            listBtnDrop[i] = (Button)EditorGUILayout.ObjectField("Button " + i, listBtnDrop[i], typeof(Button), false);
//        }

//        mTest.type = (DropBoxCustom.TYPE)EditorGUILayout.EnumPopup("Type", mTest.type);
//        switch (mTest.type)
//        {
//            case DropBoxCustom.TYPE.TEXT:
//                {
//                    mTest.textMeshTarget = (TextMeshProUGUI)EditorGUILayout.ObjectField("TextMeshProUGUI", mTest.textMeshTarget, typeof(TextMeshProUGUI), false);
//                    var list = mTest.listDataString;
//                    int strCount = Mathf.Max(0, EditorGUILayout.IntField("List Data String", list.Count));
//                    while (strCount < list.Count)
//                        list.RemoveAt(list.Count - 1);
//                    while (strCount > list.Count)
//                        list.Add("");
//                    for (int i = 0; i < list.Count; i++)
//                    {
//                        list[i] = EditorGUILayout.TextField("Element " + i, list[i]);
//                    }
//                    break;
//                }
//            case DropBoxCustom.TYPE.IMAGE:
//                {
//                    mTest.imageTarget = (Image)EditorGUILayout.ObjectField("Image", mTest.imageTarget, typeof(Image), false);

//                    var listString = mTest.listDataSprite;
//                    int imgCount = Mathf.Max(0, EditorGUILayout.IntField("List Data Sprites", listString.Count));
//                    while (imgCount < listString.Count)
//                        listString.RemoveAt(listString.Count - 1);
//                    while (imgCount > listString.Count)
//                        listString.Add(null);
//                    for (int i = 0; i < listString.Count; i++)
//                    {
//                        listString[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + i, listString[i], typeof(Sprite), false);
//                    }
//                    break;
//                }
//        }
//    }
//}
//#endif

public class DropBoxCustom : MonoBehaviour
{
    [SerializeField]
    public List<Button> listBtnDropbox = new List<Button>();

    //public TextMeshProUGUI textMeshTarget;
    //public List<string> listDataString = new List<string>();

    [SerializeField]
    public Image imageTarget;

    //[SerializeField]
    //List<Sprite> listDataSprite = new List<Sprite>();

    Action<int> actionCallback;

    void Start()
    {

    }
    public void setClickSelect()
    {
        for (var i = 0; i < listBtnDropbox.Count; i++)
        {
            var index = i;
            var btn = listBtnDropbox[i];
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                OnClickSelect(btn, index);
            });
        }
    }
    public void Show()
    {
        Debug.Log("DropBox Show");
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void setCallback(Action<int> _actionCallback)
    {
        actionCallback = _actionCallback;
    }

    void OnClickSelect(Button button, int indexSelect)
    {
        imageTarget.sprite = button.transform.GetChild(0).GetComponent<Image>().sprite;
        imageTarget.SetNativeSize();
        if (actionCallback != null)
        {
            actionCallback.Invoke(indexSelect);
        }
        Hide();
    }

    public void SetSlectWithIndex(int index)
    {
        if (index < 0 || index > listBtnDropbox.Count - 1) return;
        imageTarget.sprite = listBtnDropbox[index].transform.GetChild(0).GetComponent<Image>().sprite;
        imageTarget.SetNativeSize();
    }
}
