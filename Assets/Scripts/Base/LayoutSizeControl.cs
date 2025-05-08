using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutSizeControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] RectTransform rectTf;
    [SerializeField] int spacing = 0;
    [SerializeField] int paddingTop;
    [SerializeField] int paddingBot;
    [SerializeField] int paddingLeft;
    [SerializeField] int paddingRight;
    [SerializeField] int childWidth = 0;
    [SerializeField] int childHeight = 0;
    [SerializeField] int maxRecheckInUpdate = 10;
    public float checkTurn = 0;
    float updateTimer = 0;
    float updateInterval = 0.25f;
   
    public enum TYPE
    {
        VERTICAL,
        HORIZONTAL,
    };
    public TYPE type = TYPE.VERTICAL;

    void Start()
    {
        //updateSizeContent();
        
    }
    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer < updateInterval||checkTurn==maxRecheckInUpdate) return; // we don't need to do the math every frame
        checkTurn++;
        updateTimer = 0;
        updateSizeContent();
    }
    // Update is called once per frame
    private void Awake()
    {

    }
    public void updateSizeContent()
    {
        float sizeContent = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (type == TYPE.VERTICAL)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    sizeContent += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                    sizeContent += spacing;
                }
            }
            else
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    sizeContent += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
                    sizeContent += spacing;
                    //Debug.Log("sizeChild=" + transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x);

                }
            }
        }
        if (type == TYPE.VERTICAL)
        {
            sizeContent += paddingTop;
            sizeContent += paddingBot;
            rectTf.sizeDelta = new Vector2(rectTf.sizeDelta.x, sizeContent);
        }
        else
        {
            sizeContent += paddingLeft;
            sizeContent += paddingRight;
            rectTf.sizeDelta = new Vector2(sizeContent, rectTf.sizeDelta.y);
        }
        
    }
    public void Reset()
    {
        checkTurn = updateTimer = checkTurn=0;
    }
}
