using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class SafeArena : MonoBehaviour
{
    public static SafeArena instance = null;
    RectTransform _ThisRT;
    bool isChange = true;
    // Start is called before the first frame update
    void Start()
    {
        SafeArena.instance = this;
        _ThisRT = GetComponent<RectTransform>();
        changeOrient();
    }
    public void changeOrientation()
    {

    }
    private void Update()
    {
        // return; // test portrait orientation, remove on build
        changeOrient();
        enabled = false;
    }
    public void changeOrient()
    {
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            isChange = false;
            Rect safeAreaRect = Screen.safeArea;

            var minAnchor = safeAreaRect.position;
            var maxAnchor = minAnchor + safeAreaRect.size;
            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            _ThisRT.anchorMin = minAnchor;
            _ThisRT.anchorMax = maxAnchor;
        }
        else if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            isChange = true;
            _ThisRT.anchorMin = Vector2.zero;
            _ThisRT.anchorMax = Vector2.one;
        }
    }
}
