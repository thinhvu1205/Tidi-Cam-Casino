using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TestScene : MonoBehaviour
{
    [SerializeField] private Image m_TestImg;
    [SerializeField] private int m_Test;
    [ContextMenu("Run Test")]
    public void Test()
    {
        // Debug.Log(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        List<string> listColors = new List<string>
        {
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
            "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
            "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE",
            "#920E48", "#F277E2", "#BC8B15", "#AC6456", "#E17512",
            "#E8C500", "#F0E915", "#FD93A1", "#C735D4", "#FF0C04",
            "#69C4C9", "#067048", "#25A0F0", "#6AF28E", "#003CC3",
            "#1DC42C", "#6C58B1", "#97B158", "#0F0098", "#6700BE"
        };
        ColorUtility.TryParseHtmlString(listColors[m_Test], out Color outputC);
        m_TestImg.color = outputC;
    }
    private void Start()
    {
        // Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        // foreach (Button btn in buttons)
        // {
        //     for (int i = 0; i < btn.onClick.GetPersistentEventCount(); i++)
        //     {
        //         Debug.Log("Listerner:" + btn.name + ", " + btn.gameObject.activeSelf + ", " + btn.onClick.GetPersistentMethodName(i));
        //     }
        // }
    }
}
