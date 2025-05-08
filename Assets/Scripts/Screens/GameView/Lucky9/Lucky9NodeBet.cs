using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Lucky9NodeBet : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> lb_bet;
    [SerializeField] List<Button> listBtn;
    [SerializeField] bool isShow = false;

    private Lucky9View gameView;

    public long totalBet = 0;
    public long lastBet = 0;


    public void setParentNodeBet(Lucky9View lucky9)
    {
        gameView = lucky9;
    }

    public void onHide()
    {
        if (isShow == false) return;
        isShow = false;
        StopAllCoroutines();
        foreach (var btn in listBtn)
        {
            btn.interactable = false;
        }
        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        Vector3 targetPos = new Vector3(0, -100, 0);
        float elapsedTime = 0.5f;
        Vector3 startingPos = transform.localPosition;
        while (elapsedTime > 0)
        {
            transform.localPosition = Vector3.Lerp(startingPos, targetPos, (0.5f - elapsedTime) / 0.5f);
            elapsedTime -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
        gameObject.SetActive(false);
    }

    public void onShow(float time)
    {
        isShow = true;
        //this.transform.localPosition = new Vector3(0, -100, 0);
        SetInfo();
        gameObject.SetActive(true);
        StopAllCoroutines();
        foreach (var btn in listBtn)
        {
            btn.interactable = true;
        }
        if (lastBet <= 0) listBtn[0].interactable = false;
        if (lastBet <= 0 && totalBet <= 0) listBtn[1].interactable = false;

        StartCoroutine(ShowRoutine(time));
    }

    private IEnumerator ShowRoutine(float time)
    {
        Vector3 targetPos = new Vector3(0, 0, 0);
        float elapsedTime = 0.5f;
        Vector3 startingPos = transform.localPosition;
        while (elapsedTime > 0)
        {
            transform.localPosition = Vector3.Lerp(startingPos, targetPos, (0.5f - elapsedTime) / 0.5f);
            elapsedTime -= Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
        yield return new WaitForSeconds(time);
        onHide();
    }

    public void SetInfo()
    {
        lb_bet[0].text = Globals.Config.FormatMoney(gameView.agTable);
        lb_bet[1].text = Globals.Config.FormatMoney(gameView.agTable * 5);
    }

    public void OnBtnBetClick(int data)
    {
        long curMoney = gameView.thisPlayer.ag;
        long moneyBet = data * gameView.agTable;
        if (curMoney < moneyBet) moneyBet = curMoney;
        if (moneyBet <= 0) return;
        SocketSend.sendRaise(moneyBet);

    }

    public void onSpecialBetClick(int data)
    {
        long curMoney = gameView.thisPlayer.ag;
        long moneyBet = 0;
        if (data == 0)
            moneyBet = lastBet;
        else
        {
            long maxbet = gameView.getMaxBetTable();
            if (data == 1)
            {
                moneyBet = totalBet == 0 ? (lastBet * 2) : totalBet;
                if (moneyBet > maxbet)
                {
                    string str = Globals.Config.getTextConfig("txt_lucky9_out_maxbet").Replace("%d", maxbet.ToString());
                    UIManager.instance.showToast(str);
                    return;
                }
            }
            else
            {
                moneyBet = maxbet - totalBet;
            }
        }
        if (moneyBet <= 0) return;
        if (curMoney < moneyBet) moneyBet = curMoney;

        lastBet = totalBet;

        SocketSend.sendRaise(moneyBet);

        onHide();
    }

    public void checkButtons()
    {
        if (lastBet <= 0 && totalBet <= 0)
        {
            listBtn[1].interactable = false;
        }
        else
        {
            listBtn[1].interactable = true;
        }
        if (gameView.thisPlayer.ag == 0)
        {
            foreach (var btn in listBtn)
            {
                btn.interactable = false;
            }
        }
        if (totalBet > 0)
        {
            listBtn[0].interactable = false;
        }
    }

    public void resetTotalBet()
    {
        totalBet = 0;
    }
}
