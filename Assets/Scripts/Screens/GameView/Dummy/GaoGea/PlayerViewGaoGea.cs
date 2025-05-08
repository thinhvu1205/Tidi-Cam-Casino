using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerViewGaoGea : PlayerView
{
    // Start is called before the first frame update
    [SerializeField]
    public GameObject cardContainer;
    [HideInInspector]
    public BoxBetShow boxbet;
    [HideInInspector]
    public ShowResultScore resultScore;
    public bool isShowCardFold = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
        if (boxbet != null)
        {
            Destroy(boxbet.gameObject);
            boxbet = null;
        }
    }
    public override void setEffectWin(string animName = "", bool isLoop = true)
    {
        base.setEffectWin(animName, isLoop);
        animResult.transform.SetAsLastSibling();
    }
    public override void setEffectLose(bool isLoop = true)
    {
        base.setEffectLose(isLoop);
        animResult.transform.SetAsLastSibling();
    }
}
