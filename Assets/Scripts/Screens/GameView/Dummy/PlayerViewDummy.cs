using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerViewDummy : PlayerView
{
    [SerializeField]
    TextMeshProUGUI txtScore, txtCardCount;
    [SerializeField]
    Transform potDummy, hitPot;

    public void updateKaengPoint(int score, bool isHide = false)
    {
        if (Globals.Config.curGameId != (int)Globals.GAMEID.DUMMY)
        {
            if (score < 0)
            {
                txtScore.transform.parent.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            if (isHide)
            {
                txtScore.transform.parent.gameObject.SetActive(false);
                return;
            }
        }

        txtScore.transform.parent.gameObject.SetActive(true);
        txtScore.text = score + "";
        var pos = txtScore.transform.parent.localPosition;
        if (transform.localPosition.x < 0)
        {
            pos.x = 50;
        }
        else
        {
            pos.x = -50;
        }
        txtScore.transform.parent.localPosition = pos;
    }
    public void updatePotDummy(int indexPot)
    {
        potDummy.gameObject.SetActive(true);
        for (var i = 0; i < potDummy.childCount; i++)
        {
            potDummy.GetChild(i).gameObject.SetActive(i < indexPot);
        }
    }
    public void enablePotDummy(bool isShow = false)
    {
        potDummy.gameObject.SetActive(isShow);
    }
    public void enableHitpot(bool isShow = false)
    {
        hitPot.gameObject.SetActive(isShow);
    }
    public void setHitpot(int num)
    {
        for (var i = 0; i < hitPot.childCount; i++)
        {
            //listCham.push(pots[i].getChildByName('cham_on'))
            hitPot.GetChild(i).GetChild(0).gameObject.SetActive(i < num);
        }
    }

    public int getCardCount()
    {
        return int.Parse(txtCardCount.text);
    }

    public void updateNumCard(int number, Vector3 pos, bool isSetPos = true)
    {
        if (number <= 0)
        {
            txtCardCount.gameObject.SetActive(false);
            return;
        }
        txtCardCount.gameObject.SetActive(true);
        txtCardCount.text = number + "";
        if (isSetPos)
        {
            txtCardCount.transform.localPosition = pos;//transform.InverseTransformPoint(posWorld);
            txtCardCount.transform.SetAsLastSibling();
        }
    }
}
