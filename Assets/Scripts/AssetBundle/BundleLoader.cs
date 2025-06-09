using System;
using System.IO;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent, ExecuteInEditMode]
public class BundleLoader : MonoBehaviour
{
    public enum TYPE_ASSET { NONE, IMAGE, SKELETON_GRAPHIC };
    [HideInInspector] public TYPE_ASSET Type = TYPE_ASSET.IMAGE;
    [HideInInspector] public Image ThisImg;
    [HideInInspector] public SkeletonGraphic ThisSG;
    [HideInInspector] public string BundleLabel, AssetName, AnimName;
    [HideInInspector] public bool SetNativeSize;
    [SerializeField] private UnityEvent m_OnEnableUE;

    public void RefreshUI()
    {
        switch (Type)
        {
            case TYPE_ASSET.IMAGE:
                {
                    if (ThisImg == null) ThisImg = GetComponent<Image>();
                    if (ThisImg == null || !ThisImg.enabled) return;
                    ThisImg.sprite = BundleHandler.LoadSprite(AssetName);
                    if (SetNativeSize) ThisImg.SetNativeSize();
                    break;
                }
            case TYPE_ASSET.SKELETON_GRAPHIC:
                {
                    if (ThisSG == null) ThisSG = GetComponent<SkeletonGraphic>();
                    if (ThisSG == null || !ThisSG.enabled) return;
                    BundleHandler.SetDataForASkeletonGraphic(ThisSG, AssetName, AnimName, ThisSG.startingLoop);
                    break;
                }
        }
    }
    public void RemoveOnEnableCbListeners() => m_OnEnableUE.RemoveAllListeners();
    public void AddOnEnableCb(UnityAction eventUE) => m_OnEnableUE.AddListener(eventUE);
    private void OnDisable()
    {
        BundleHandler.MAIN.RemoveLoader(this);
    }
    void Start()
    {
        RefreshUI();
    }
    private void OnEnable()
    {

        m_OnEnableUE?.Invoke();
    }
    private void Awake()
    {
        BundleHandler.MAIN.AddLoader(this);
    }
}
//-------------------------------------------------- |   ) )=3 --------------------------------------------------
#if UNITY_EDITOR 
[CustomEditor(typeof(BundleLoader))]
public class LoaderEditor : Editor
{
    private string[] _AnimNames;
    private SkeletonData _LastSD;

    public override void OnInspectorGUI()
    {
        BundleLoader thisBL = (BundleLoader)target;
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Asset Name", thisBL.AssetName);
            if (thisBL.Type == BundleLoader.TYPE_ASSET.SKELETON_GRAPHIC) EditorGUILayout.LabelField("Anim Name", thisBL.AnimName);
            return; // test in editor play mode will cause error, only work with this in editor idle mode
        }
        base.OnInspectorGUI();
        if (thisBL.GetComponent<Image>() != null) thisBL.Type = BundleLoader.TYPE_ASSET.IMAGE;
        else if (thisBL.GetComponent<SkeletonGraphic>() != null) thisBL.Type = BundleLoader.TYPE_ASSET.SKELETON_GRAPHIC;
        else thisBL.Type = BundleLoader.TYPE_ASSET.NONE;
        EditorGUILayout.LabelField("Type: " + thisBL.Type.ToString(), EditorStyles.boldLabel);
        switch (thisBL.Type)
        {
            case BundleLoader.TYPE_ASSET.NONE:
                {
                    EditorGUILayout.HelpBox("YOU MUST ADD A COMPONENT FIRST!", MessageType.Warning);
                    thisBL.AddOnEnableCb(null);
                    break;
                }
            case BundleLoader.TYPE_ASSET.IMAGE:
                {
                    if (thisBL.ThisImg == null) thisBL.ThisImg = thisBL.GetComponent<Image>();
                    if (thisBL.ThisImg == null || !thisBL.ThisImg.enabled)
                    {
                        EditorGUILayout.HelpBox("You must have an active Image!", MessageType.Warning);
                        thisBL.AddOnEnableCb(null);
                        return;
                    }
                    if (thisBL.ThisImg.sprite == null)
                    {
                        EditorGUILayout.HelpBox("No Image asset found!", MessageType.Warning);
                        EditorGUILayout.LabelField("Label", thisBL.BundleLabel);
                        if (thisBL.BundleLabel.Equals(""))
                            EditorGUILayout.HelpBox("No label, this asset is not in any bundle!", MessageType.Warning);
                        EditorGUILayout.LabelField("Asset Name", thisBL.AssetName);
                        EditorGUILayout.LabelField("Set Native Size", thisBL.SetNativeSize ? "True" : "False");
                        thisBL.AddOnEnableCb(null);
                        return;
                    }
                    thisBL.AssetName = AssetDatabase.GetAssetPath(thisBL.ThisImg.sprite);
                    string path = thisBL.AssetName;
                    do
                    {
                        thisBL.BundleLabel = AssetImporter.GetAtPath(path).assetBundleName;
                        if (thisBL.BundleLabel.Equals("")) path = Path.GetDirectoryName(path);
                        else path = "";
                    } while (!path.Equals(""));
                    EditorGUILayout.TextField("Bundle Label", thisBL.BundleLabel);
                    if (thisBL.BundleLabel.Equals(""))
                        EditorGUILayout.HelpBox("No label, this asset is not in any bundle!", MessageType.Warning);
                    EditorGUILayout.TextField("Asset Name", thisBL.AssetName);
                    thisBL.SetNativeSize = EditorGUILayout.Toggle("Set Native Size", thisBL.SetNativeSize);
                    break;
                }
            case BundleLoader.TYPE_ASSET.SKELETON_GRAPHIC:
                {
                    if (thisBL.ThisSG == null) thisBL.ThisSG = thisBL.GetComponent<SkeletonGraphic>();
                    if (thisBL.ThisSG == null || !thisBL.ThisSG.enabled)
                    {
                        EditorGUILayout.HelpBox("You must have an active SkeletonGraphic!", MessageType.Warning);
                        thisBL.AddOnEnableCb(null);
                        return;
                    }
                    SkeletonData thisSD = thisBL.ThisSG.SkeletonData;
                    if (thisSD == null)
                    {
                        EditorGUILayout.HelpBox("No SkeletonData asset found!", MessageType.Warning);
                        EditorGUILayout.LabelField("Label", thisBL.BundleLabel);
                        if (thisBL.BundleLabel.Equals(""))
                            EditorGUILayout.HelpBox("No label, this asset is not in any bundle!", MessageType.Warning);
                        EditorGUILayout.LabelField("Asset Name", thisBL.AssetName);
                        EditorGUILayout.LabelField("Anim Name", thisBL.AnimName);
                        thisBL.AddOnEnableCb(null);
                        return;
                    }
                    thisBL.AssetName = AssetDatabase.GetAssetPath(thisBL.ThisSG.skeletonDataAsset);
                    string path = Path.GetDirectoryName(thisBL.AssetName);
                    do
                    {
                        thisBL.BundleLabel = AssetImporter.GetAtPath(path).assetBundleName;
                        if (thisBL.BundleLabel.Equals("")) path = Path.GetDirectoryName(path);
                        else path = "";
                    }
                    while (!path.Equals(""));
                    EditorGUILayout.TextField("Bundle Label", thisBL.BundleLabel);
                    if (thisBL.BundleLabel.Equals(""))
                        EditorGUILayout.HelpBox("No label, this asset is not in any bundle!", MessageType.Warning);
                    EditorGUILayout.TextField("Asset Name", thisBL.AssetName);
                    if (_LastSD != thisSD)
                    {
                        _LastSD = thisSD;
                        _AnimNames = new string[thisSD.Animations.Count];
                        int id = 0;
                        ExposedList<Spine.Animation> thisAs = thisSD.Animations;
                        foreach (Spine.Animation anim in thisAs) _AnimNames[id++] = anim.Name;
                    }
                    thisBL.AnimName = _AnimNames[EditorGUILayout.Popup("Animation", Mathf.Max(0, Array.IndexOf(_AnimNames, thisBL.AnimName)), _AnimNames)];
                    break;
                }
        }
    }
}
#endif
