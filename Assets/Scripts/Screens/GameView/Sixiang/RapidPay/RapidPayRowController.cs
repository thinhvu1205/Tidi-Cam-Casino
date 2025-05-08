using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Spine.Unity;
using System.Threading.Tasks;


public class RapidPayRowController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<Button> btnItemPick = new List<Button>();

    [SerializeField]
    private int indexRow = 0;
    [HideInInspector]
    private List<int> listResult = new List<int>();

    private Button currentItemPick;
    private void Awake()
    {
        btnItemPick.ForEach((btn) =>
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                onClickItem(btn);
            });
            btn.interactable = false;
        });
    }

    public void activeButton()
    {

        btnItemPick.ForEach((btn) =>
        {
            btn.interactable = true;
        });
    }

    // Update is called once per frame

    public void onClickItem(Button btn)
    {
        currentItemPick = btn;
        btnItemPick.ForEach((btn) =>
        {
            btn.interactable = false;
        });
        SocketSend.sendPackageRapidPay(Globals.ACTION_SLOT_SIXIANG.rapidPay, btnItemPick.IndexOf(btn).ToString());
    }

    public async Task<Button> setResult(JObject data)
    {
        listResult = data["items"].ToObject<List<int>>();
        int result = (int)data["item"];
        int indexItem = (int)data["index"];
        currentItemPick = btnItemPick[indexItem];
        btnItemPick.ForEach((btn) =>
        {
            btn.interactable = false;
        });
        if (currentItemPick != null)
        {
            SkeletonGraphic spineItemCurrent = currentItemPick.GetComponentInChildren<SkeletonGraphic>();
            spineItemCurrent.Initialize(true);
            spineItemCurrent.AnimationState.SetAnimation(0, getAnimName(result), false);
            if (getAnimName(result).Equals("end"))
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.CLICK_ITEM_MISS);
            else
                SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.RAPID_ITEM_WIN);
            await Task.Delay((int)spineItemCurrent.Skeleton.Data.FindAnimation(getAnimName(result)).Duration * 1000);
        }
        for (int i = 0; i < btnItemPick.Count; i++)
        {
            if (btnItemPick[i] != currentItemPick)
            {
                SkeletonGraphic spineItem = btnItemPick[i].GetComponentInChildren<SkeletonGraphic>();
                spineItem.color = Color.gray;
                spineItem.Initialize(true);
                spineItem.AnimationState.SetAnimation(0, getAnimName(listResult[i]), false);
            }
        }
        return currentItemPick;
    }
    private void setEffectResult()
    {

    }
    private string getAnimName(int item)
    {
        string animation = "normal";
        switch (item)
        {
            case 1: animation = "end"; break;
            case 2: animation = "2x"; break;
            case 3: animation = "3x"; break;
            case 4: animation = "4x"; break;
        }
        return animation;
    }
}
