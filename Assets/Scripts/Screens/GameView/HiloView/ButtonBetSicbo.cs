using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonBetSicbo : MonoBehaviour
{
    [SerializeField] public Button btn_Rebet, btn_Double;

    [SerializeField] List<Button> listBtnBetChip;

    [SerializeField] public GameObject ske_rebet, ske_double;

    [SerializeField] public TextMeshProUGUI label_totalBet;

    [SerializeField] public TextMeshProUGUI label_clear;

    [SerializeField] TMP_FontAsset fontChip;

    HiloView sicboGameView;
    public List<int> listValue = new List<int>();

    private void Awake()
    {
        sicboGameView = (HiloView)UIManager.instance.gameView;
        setSprChipBet();
    }
    private void OnEnable()
    {
        StartCoroutine(ShowButtons());
        ///////////////////////////////////////////////////////////////
        IEnumerator ShowButtons()
        {
            int valueBet = 0;
            for (int i = 0; i < sicboGameView.boxBet.dataBet.Count; i++)
            {
                JObject data = (JObject)sicboGameView.boxBet.dataBet[i];
                valueBet += (int)data["value"];
            }
            bool enableRebet = sicboGameView.matchCount != 0 && !(sicboGameView.thisPlayer.ag < valueBet || sicboGameView.boxBet.dataBet.Count <= 0);
            btn_Rebet.interactable = enableRebet;
            // ske_rebet.gameObject.SetActive(enableRebet);
            btn_Double.interactable = false;
            //ske_double.gameObject.SetActive(false);
            CanvasGroup canvasGr = GetComponent<CanvasGroup>();
            canvasGr.alpha = 0;
            for (int i = 0; i < listBtnBetChip.Count; i++) listBtnBetChip[i].gameObject.SetActive(false);
            float fadingTime = .05f;
            canvasGr.DOComplete();
            canvasGr.DOFade(1, fadingTime);
            yield return new WaitForSecondsRealtime(fadingTime);
            foreach (Button betBtn in listBtnBetChip)
            {
                yield return new WaitForSecondsRealtime(.1f);
                betBtn.gameObject.SetActive(true);
                betBtn.interactable = true;
                betBtn.transform.DOComplete();
                betBtn.transform.DOScale(new Vector2(1.2f, 1.2f), .05f).OnComplete(() => { betBtn.transform.DOScale(Vector2.one, .05f); });
            }
            yield return new WaitForSecondsRealtime(.6f);
            onClickChip(sicboGameView.chipDealLastMatch.ToString());
            sicboGameView.gateBet.setStateButtonBet(false);
        }
    }
    public void onClickDouble()
    {
        Debug.Log("onclick button double!!!!");
        sicboGameView.onClickDouble();
        btn_Double.interactable = false;
        //ske_double.gameObject.SetActive(false);
    }

    public void onClickRebet()
    {
        Debug.Log("onclick button rebet!!!!");
        sicboGameView.onClickRebet();
        btn_Rebet.interactable = false;
        //ske_rebet.gameObject.SetActive(false);
    }
    public void onClickDeal()
    {
        sicboGameView.onClickDeal();

    }
    public void onClickClear()
    {
        sicboGameView.onClickClear();
    }
    public void onClickChip(string dataChip)
    {

        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            listBtnBetChip[i].transform.Find("border").gameObject.SetActive(false);
        }
        SoundManager.instance.soundClick();
        int data = int.Parse(dataChip);
        sicboGameView.setValueBtnBet(data);
        setStateButtonChip();
    }

    public void activeDouble()
    {
        bool isDisable = sicboGameView.thisPlayer.ag < sicboGameView.totalBet;
        btn_Double.interactable = !isDisable;
        //ske_double.gameObject.SetActive(!isDisable);
    }

    public void setStateButtonChip()
    {

        var agClickBet = sicboGameView.thisPlayer.ag - sicboGameView.boxBet.totalBoxBet;

        if (agClickBet < sicboGameView.curChipBet) sicboGameView.curChipBet = agClickBet;

        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            if (agClickBet < listValue[i])
            {
                listBtnBetChip[i].interactable = false;
            }
            else
            {
                listBtnBetChip[i].interactable = true;
            }
        }
        var index = sicboGameView.chipDealLastMatch - 1;
        if (index < 1)
            index = 0;
        listBtnBetChip[index].transform.Find("border").gameObject.SetActive(true);
    }
    public void setStateButtonOnBet()
    {

        for (int i = 0; i < listBtnBetChip.Count; i++)
        {
            if (sicboGameView.thisPlayer.ag < listValue[i])
            {
                listBtnBetChip[i].interactable = false;
            }
            else
            {
                listBtnBetChip[i].interactable = true;
            }
        }
        int index = sicboGameView.chipDealLastMatch - 1;
        if (index < 1)
            index = 0;
        listBtnBetChip[index].transform.Find("border").gameObject.SetActive(true);
    }
    private void setSprChipBet()
    {
        listValue = sicboGameView.listValue;
        for (int i = 0; i < listValue.Count; i++)
        {
            TextMeshProUGUI nodeText = listBtnBetChip[i].transform.GetComponentInChildren<TextMeshProUGUI>();
            nodeText.font = fontChip;
            nodeText.text = Globals.Config.FormatMoney2(listValue[i], true);
            nodeText.transform.localScale = new Vector2(1, 1);
            nodeText.color = Color.black;
        }
    }
}
