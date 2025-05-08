using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;


public class LineController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    UILineRenderer UIline;
    [HideInInspector]
    List<Vector2> listPos = new List<Vector2>();
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void drawLine(List<Vector2> listPos, Color colorLine)
    {
        UIline.color = colorLine;
        UIline.Points = listPos.ToArray();
    }

    public void drawRect(Vector2 startPos, Vector2 sizeRect, Color colorRect)
    {
        Vector3 A = new Vector3(startPos.x - sizeRect.x / 2, startPos.y - sizeRect.y / 2, -1);
        Vector3 B = new Vector3(A.x, A.y + sizeRect.y, -1);
        Vector3 C = new Vector3(B.x + sizeRect.x, B.y, -1);
        Vector3 D = new Vector3(C.x, A.y, -1);
        UIline.color = colorRect;
        UIline.Points = new Vector2[] { A, B, C, D,A };

    }
}
