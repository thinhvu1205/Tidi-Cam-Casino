using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;

public class PlayerViewDomino : PlayerView
{
    [SerializeField] public TextMeshProUGUI lbDominoRemain;
    [SerializeField] public GameObject domino;
    [SerializeField] public GameObject lewal;
    [SerializeField] public List<GameObject> listFolds = new List<GameObject>();
    [HideInInspector] public List<Domino> listMyDomino = new List<Domino>();
    [SerializeField] TextMeshProUGUI time;

    int turnTime = 10;
    public int findCardID(int id)
    {
        for (int i = 0; i < listMyDomino.Count; i++)
        {
            if (listMyDomino[i].cardID == id)
            {
                return i;
            }
        }
        return -1;
    }

    public void resetFoldDice()
    {
        for (int i = 0; i < 7; i++)
        {
            listFolds[i].SetActive(false);
        }
    }

    public void setFoldDice(List<int> listFold)
    {
        listFold.ForEach(f =>
        {
            listFolds[f].SetActive(true);
        });
    }

    Sequence sequence;
    public override async void setTurn(bool isTurn, float _timeTurn = 0f, bool _isMe = false, float timeVibrate = 5f)
    {
        if (sequence != null)
        {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        turnTime = (int)_timeTurn;
        base.setTurn(isTurn, _timeTurn, _isMe);
        time.text = turnTime.ToString();
        sequence.AppendInterval(1.0f).AppendCallback(() =>
        {
            turnTime--;
            time.text = turnTime + "";
        }).SetLoops(turnTime);
        if (isTurn)
        {
            time.gameObject.SetActive(true);
            sequence.Play();
        }
        else
        {
            time.gameObject.SetActive(false);
            sequence.Pause();
        }
    }
}
