using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;


public class SiXiangRapidPayView : MonoBehaviour
{
    // Start is called before the first frame update

    public static SiXiangRapidPayView instance = null;
    [SerializeField]
    List<RapidPayRowController> listRows = new List<RapidPayRowController>();

    [SerializeField]
    public SkeletonGraphic spineLight;
    [SerializeField]
    public SkeletonGraphic spineBgRow, spineResult;
    [SerializeField]
    public TextMeshProUGUI lbBonusTotal, lbWinAmount;
    [SerializeField]
    public TextNumberControl lbWinResult;
    [SerializeField]
    public Button btnCollect;

    private RapidPayRowController currentRow;
    private int indexRow = 0, totalBonus = 1;
    public int winAmount = 0;
    private long userAmount = 0;
    private bool isFinished = false;
    private bool isSelectBonusGame = false;

    public int index = 0;
    [HideInInspector]
    private SiXiangView gameView;
    private List<Button> listItem = new List<Button>();
    public Task rapidTask;
    void Start()
    {
        SiXiangRapidPayView.instance = this;
        listRows.ForEach(row =>
        {
            listItem.AddRange(row.btnItemPick);
        });
    }

    // Update is called once per frame
    public Task Show(SiXiangView SiXiangView, bool isUltimate, List<JObject> initData = null)
    {
        Debug.Log("winAmount====" + Globals.Config.FormatNumber(winAmount));
        currentRow = listRows[0];
        currentRow.activeButton();
        gameView = SiXiangView;
        totalBonus = isUltimate ? 4 : 1;
        lbBonusTotal.text = "x" + totalBonus;
        lbWinAmount.text = Globals.Config.FormatNumber(isUltimate ? winAmount * 4 : winAmount);
        if (initData != null)
        {
            setInitView(initData);
        }

        rapidTask = new Task(() => { });
        return rapidTask;
    }
    private void setInitView(List<JObject> data)
    {
        for (int i = 0, l = data.Count; i < l; i++)
        {
            JObject dataRow = data[i];
            totalBonus *= (int)dataRow["multiplier"];
            listRows[i].setResult(dataRow);
            indexRow++;
        }
        currentRow = listRows[indexRow];
        currentRow.activeButton();
        spineBgRow.transform.DOLocalMoveY(spineBgRow.transform.localPosition.y + 123 * indexRow - indexRow * 3.5f, 0.3f).SetEase(Ease.InSine);
        lbBonusTotal.text = "x" + totalBonus;

    }
    public async void setResult(JObject data)
    {
        Button btnItem = await currentRow.setResult(data);
        int indexPick = listItem.IndexOf(btnItem) + 1;
        winAmount = (int)data["winAmount"];
        isSelectBonusGame = (bool)data["isSelectBonusGame"];
        if (data.ContainsKey("userAmount")) userAmount = (long)data["userAmount"];

        isFinished = (bool)data["isFinished"];
        int itemPicked = (int)data["item"];
        if (itemPicked != 1) //ko pick phai end = 1
        {
            spineLight.gameObject.SetActive(true);
            spineLight.Initialize(true);
            spineLight.AnimationState.SetAnimation(0, indexPick.ToString(), false);
            totalBonus *= (int)data["multiplier"];
            SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.RAPID_CHIP_FLY);
        }

        DOTween.Sequence()
            .AppendInterval(spineLight.Skeleton.Data.FindAnimation(indexPick.ToString()).Duration - 0.35f)
            .AppendCallback(() =>
            {
                lbBonusTotal.text = "x" + totalBonus;
            }).AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                spineLight.gameObject.SetActive(false);
                Globals.Config.tweenNumberToNumber(lbWinAmount, winAmount);
                if (!isFinished) nextRow();
                else showResult();
            });
    }
    private void nextRow()
    {
        spineBgRow.transform.DOLocalMoveY(spineBgRow.transform.localPosition.y + 123 - indexRow * 3.5f, 0.3f).SetEase(Ease.InSine);
        indexRow++;
        currentRow = listRows[indexRow];
        currentRow.activeButton();
    }
    private void showResult()
    {
        spineResult.skeletonDataAsset = UIManager.instance.loadSkeletonData("GameView/SiXiang/Spine/BigWinRapid/skeleton_SkeletonData");
        spineResult.Initialize(true);
        spineResult.AnimationState.SetAnimation(0, "eng", false);
        spineResult.transform.parent.gameObject.SetActive(true);
        //Globals.Config.tweenNumberToNumber(lbWinResult, winAmount);
        btnCollect.gameObject.SetActive(false);
        AudioSource soundMoney = SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_START);
        float timeRun = 2f;
        lbWinResult.setValue(winAmount, true, timeRun, "", () =>
        {
            btnCollect.gameObject.SetActive(true);
            soundMoney.Stop();
            SoundManager.instance.playEffectFromPath(Globals.SOUND_SLOT_BASE.COUNGTING_MONEY_END);

        });

        IEnumerator ShowButtonCollect()
        {
            btnCollect.gameObject.SetActive(false);
            yield return new WaitForSeconds(timeRun);
            btnCollect.gameObject.SetActive(true);
        }
    }
    public async void onClickCollect()
    {
        spineResult.transform.DOScale(new Vector2(0.8f, 0.8f), 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            spineResult.transform.parent.gameObject.SetActive(false);
        });


        //await gameView.showAnimCutScene();
        Destroy(gameObject);
        JObject dataEnd = new JObject();
        dataEnd["winAmount"] = winAmount;
        dataEnd["userAmount"] = userAmount;
        dataEnd["gameType"] = (int)SiXiangView.GAME_TYPE.RAPID_PAY;
        dataEnd["isSelectBonusGame"] = isSelectBonusGame;
        //dataEnd["userAmount"]=
        await gameView.endMinigame(dataEnd);
        rapidTask.Start();


    }
}
