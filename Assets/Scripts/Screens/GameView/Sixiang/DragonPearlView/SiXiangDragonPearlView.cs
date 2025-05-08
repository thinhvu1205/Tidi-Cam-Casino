using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Facebook.Unity;
using Globals;

public class SiXiangDragonPearlView : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Sprite> listBgItem = new List<Sprite>();
    [SerializeField] GameObject itemContainer;
    [SerializeField] public GameObject itemInitGold;
    List<List<DragonPearlItem>> listItem = new List<List<DragonPearlItem>>();
    [HideInInspector] List<GameObject> listItemGold = new List<GameObject>();
    private List<JObject> dataPearl = new List<JObject>();
    public bool isFinish = false;
    private bool isGrandJackpot;
    private int winAmount = 0;
    private long userAmount = 0;
    public bool isDPSpin = false;
    public bool isBonusGame = false;
    private bool isAutoPlay = true;
    private bool isSelectBonusGame = false;
    private string PATH_ANIM_WINRESULT = "GameView/SiXiang/Spine/BigWinGoldPick/skeleton_SkeletonData";
    private void Awake()
    {
        for (int i = 1; i < 15; i++)
        {
            DragonPearlItem item = Instantiate(itemContainer.transform.GetChild(0), itemContainer.transform).GetComponent<DragonPearlItem>();
            item.setBgItem(listBgItem[i]);

        }
        for (int i = 0; i < 5; i++)
        {
            List<DragonPearlItem> list = new List<DragonPearlItem>();
            list.Add(itemContainer.transform.GetChild(i).GetComponent<DragonPearlItem>());
            list.Add(itemContainer.transform.GetChild(i + 5).GetComponent<DragonPearlItem>());
            list.Add(itemContainer.transform.GetChild(i + 10).GetComponent<DragonPearlItem>());
            listItem.Add(list);
        }

    }
    private void OnDisable()
    {
        resetView();
    }
    private void OnEnable()
    {
        isFinish = false;
        DOTween.Sequence().AppendInterval(10).AppendCallback(() =>
        {
            if (isAutoPlay)
            {
                SiXiangView.Instance.onClickSpinDP();
            }
        });
    }
    public async Task setInfo(JObject data, bool isInit6Gold, bool isDPSpinn = false)
    {
        SiXiangView.Instance.gameState = SiXiangView.GAME_STATE.SHOWING_RESULT;
        if (data.ContainsKey("userAmount"))
        {
            userAmount = (long)data["userAmount"];
        }
        isDPSpin = isDPSpinn;
        List<JObject> pearls = new List<JObject>();
        List<Task> tasksDP = new List<Task>();
        if (data.ContainsKey("dragonPearls"))
        {
            pearls = data["dragonPearls"].ToObject<List<JObject>>();
        }
        else
        {
            pearls = data["pearls"].ToObject<List<JObject>>();
        }
        dataPearl = pearls;
        if (data.ContainsKey("dragonPearlWinPot"))
        {
            winAmount = (int)data["dragonPearlWinPot"];
        }
        else
        {
            winAmount = (int)data["winAmount"];
        }
        List<Task> tasksSetInfo = new List<Task>();
        if (!isDPSpin)
        {
            resetView();
        }
        else
        {
            isAutoPlay = false;
            isSelectBonusGame = (bool)data["isSelectBonusGame"];
        }
        if (isInit6Gold)
        {
            await startView6Gold(pearls);
        }

        pearls.ForEach(dataPearl =>
        {
            int row = (int)dataPearl["row"];
            int col = (int)dataPearl["col"];
            if ((bool)dataPearl["isDoubled"] == false && isDPSpin == true)
            {
                DragonPearlItem item = listItem[col][row];
                tasksSetInfo.Add(item.setInfo(dataPearl, this));
            }
            else if (isDPSpin == false)
            {
                DragonPearlItem item = listItem[col][row];
                tasksSetInfo.Add(item.setInfo(dataPearl, this));
            }
        });

        if (pearls.Count > 0)
        {
            await Task.WhenAll(tasksSetInfo.ToArray());
            SiXiangView.Instance.updateWinAmount(winAmount);
            SiXiangView.Instance.infoBar.setStateWin("totalWin");
            if (isInit6Gold && SiXiangView.Instance.spintype == SiXiangView.SPIN_TYPE.AUTO)
            {
                SiXiangView.Instance.onClickSpinDP();
            }
        }
        else
        {
            await Task.Delay(TimeSpan.FromSeconds(0.25f));
        }

        if (data.ContainsKey("isFinished"))
        {
            isFinish = (bool)data["isFinished"];
            if ((bool)data["isFinished"] == true)
            {
                isGrandJackpot = (bool)data["isGrandJackpot"];
                await Task.Delay(TimeSpan.FromSeconds(1.5f));
                await showResult();
            }
        }
        else
        {
            SiXiangView.Instance.gameState = SiXiangView.GAME_STATE.PREPARE;
        }
    }
    private async Task showResult()
    {
        JObject dataEnd = new JObject();
        dataEnd["winAmount"] = winAmount;
        dataEnd["gameType"] = SiXiangView.Instance.gameType;
        dataEnd["isGrandJackpot"] = isGrandJackpot;
        dataEnd["userAmount"] = userAmount;
        dataEnd["isSelectBonusGame"] = isSelectBonusGame;
        await SiXiangView.Instance.endMinigame(dataEnd);
        gameObject.SetActive(false);
    }
    public async Task startView6Gold(List<JObject> pearls)
    {

        Debug.Log("startView6Gold");

        resetView();
        listItemGold.Clear();
        for (int i = 0; i < pearls.Count; i++)
        {
            JObject data = pearls[i];
            GameObject itemGold = Instantiate(itemInitGold, transform);

            itemGold.SetActive(true);
            Vector2 posSymbol = transform.InverseTransformPoint(SiXiangView.Instance.getPosSymbol((int)data["col"], (int)data["row"] + 1));
            itemGold.transform.DOLocalMove(posSymbol, 1.0f).SetEase(Ease.OutSine);
            itemGold.transform.DOScale(new Vector2(1.0f, 1.0f), 1.0f).SetEase(Ease.OutSine).SetId("itemGold_" + i);
            listItemGold.Add(itemGold);
            SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.PEARL_RUNITEM);
            await Task.Delay(TimeSpan.FromSeconds(i != pearls.Count - 1 ? 0.1f : 0.9f));
        }
        listItemGold.ForEach(item =>
        {
            Destroy(item);
        });
    }
    private void resetView()
    {
        listItem.ForEach(col =>
        {
            col.ForEach(item =>
            {
                item.hideItem();
            });
        });
        isGrandJackpot = false;
        isBonusGame = false;
        isAutoPlay = true;
    }
    // Update is called once per frame
    public Vector2 getPosSymbolChuTuoc()
    {
        Vector2 posChuTuoc = Vector2.zero;
        dataPearl.ForEach(data =>
        {
            if ((int)data["item"] == 1 && (int)data["luckyMoney"] == 3)
            {
                posChuTuoc = listItem[(int)data["col"]][(int)data["row"]].transform.position;
            }
        });
        return posChuTuoc;
    }
    public Vector2 getPosItem(int col, int row)
    {
        return listItem[col][row].transform.position;
    }
    public async Task setDoubleItem()
    {
        bool isWait = false;
        dataPearl.ForEach(dataPearl =>
        {
            if ((bool)dataPearl["isDoubled"] == true && isDPSpin == true)
            {
                int row = (int)dataPearl["row"];
                int col = (int)dataPearl["col"];
                DragonPearlItem item = listItem[col][row];
                item.setInfo(dataPearl, this);
                isWait = true;
            }
        });
        await Task.Delay(TimeSpan.FromSeconds(isWait ? 2.0f : 0));
    }
}
