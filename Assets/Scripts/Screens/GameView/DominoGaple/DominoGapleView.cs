using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine.EventSystems;
using Spine.Unity;
using System;
using System.Threading.Tasks;
using Globals;

public class DominoGapleView : GameView
{
    [SerializeField] GameObject startTime;
    [SerializeField] SkeletonGraphic animStart;
    [SerializeField] SkeletonGraphic animResult;
    [SerializeField] GameObject DominoContainer;
    [SerializeField] Domino dominoPref;
    [SerializeField] dominoDiceRemain diceRemainBox;
    [SerializeField] TextMeshProUGUI textBet;
    [SerializeField] GameObject diceInfo;
    [SerializeField] List<Domino> listDominoDice = new List<Domino>();
    [SerializeField] List<Sprite> listLewal = new List<Sprite>();
    [SerializeField] GameObject NoticeLastDomino;



    List<int> listDominoDrop = new List<int>();
    private List<Domino> dominoPool = new List<Domino>();
    protected List<Domino> listDominoInTable = new List<Domino>();
    protected List<Domino> listDominoPlayed = new List<Domino>();
    const int DOMINOS_PER_PLAYER = 7;
    int tail = -1, head = -1;
    int indexTail = -1, indexHead = -1;
    double tailLoc = 0, headLoc = 0;
    int idDiscard = -1;
    bool checkTail = false, checkHead = false, isMyTurn = false, isHoldCard = false;
    Domino dominoHolding;
    List<double> dominoInfoHead = new List<double> { 0, 0, 0 };
    List<double> dominoInfoTail = new List<double> { 0, 0, 0 };
    Domino dominoHead, dominoTail;
    public static int resetGame = 1;
    bool isAutoPlayLastOne = true, checkLast = false;

    public void cleanTable()
    {
        List<double> dominoInfoHead = new List<double> { 0, 0, 0 };
        List<double> dominoInfoTail = new List<double> { 0, 0, 0 };
        NoticeLastDomino.SetActive(false);
        tail = -1;
        head = -1;
        tailLoc = 0;
        headLoc = 0;
        checkTail = false;
        checkHead = false;
        Quaternion newRotation = Quaternion.Euler(0, 0, 0);
        listDominoInTable.ForEach(domino =>
        {
            domino.transform.Find("up_duoi").gameObject.SetActive(true);
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = new Vector2(1, 1);
            domino.GetComponent<CanvasGroup>().alpha = 1;
            //setDominoForm(domino, 0);
            putDomino(domino);
        });
        listDominoInTable.Clear();
        players.ForEach(pl =>
        {
            PlayerViewDomino playerDomino = (PlayerViewDomino)pl.playerView;
            playerDomino.listMyDomino.Clear();
            playerDomino.resetFoldDice();
        });
        animResult.gameObject.SetActive(false);
        dominoHead = getDomino();
        dominoTail = getDomino();
        dominoHead.gameObject.SetActive(false);
        dominoTail.gameObject.SetActive(false);
        dominoHead.GetComponent<CanvasGroup>().alpha = 0.5f;
        dominoTail.GetComponent<CanvasGroup>().alpha = 0.5f;
        dominoHead.transform.localScale = new Vector2(0.6f, 0.6f);
        dominoTail.transform.localScale = new Vector2(0.6f, 0.6f);
        dominoHead.transform.Find("up_duoi").gameObject.SetActive(false);
        dominoTail.transform.Find("up_duoi").gameObject.SetActive(false);
        diceInfo.SetActive(false);
        diceRemainBox.updateDiceRemain();
        listDominoDrop.Clear();
        resetGame = 1;
        stateGame = Globals.STATE_GAME.WAITING;
        checkLast = false;
    }

    public void handleStartGame(JObject data)
    {
        //  { "evt":"notiStart","actionTime":5}
        cleanTable();
        SoundManager.instance.playEffectFromPath(Globals.SOUND_GAME.START_GAME);
        int timeStart = (int)data["actionTime"];
        startTime.SetActive(true);
        startTime.transform.GetComponentInChildren<TextMeshProUGUI>().text = timeStart.ToString();
        DOTween.Sequence().AppendInterval(1.0f).AppendCallback(() =>
        {
            timeStart--;
            checkAutoExit();
            startTime.transform.GetComponentInChildren<TextMeshProUGUI>().text = timeStart + "";
        }).SetLoops(timeStart).OnComplete(() =>
        {
            startTime.SetActive(false);
        });
    }

    public async void handleLc(JObject data)
    {
        stateGame = Globals.STATE_GAME.PLAYING;
        listDominoDrop.Clear();
        animStart.gameObject.SetActive(true);
        animStart.AnimationState.SetAnimation(0, "animation", false);
        animStart.Initialize(true);
        animStart.AnimationState.Complete += delegate
        {
            animStart.gameObject.SetActive(false);
        };
        int playerCount = players.Count;
        Vector2 posDefault = new Vector2(-Screen.width / 2, Screen.height / 2);
        SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIA_DOMINO);
        for (int i = 0; i < playerCount * DOMINOS_PER_PLAYER; i++)
        {
            Domino domino = getDomino();
            domino.transform.localPosition = posDefault;
            domino.transform.localScale = new Vector2(1, 1);
            domino.GetComponent<CanvasGroup>().alpha = 1;
            float pos = ((int)Math.Floor(2.0f * i / playerCount) - DOMINOS_PER_PLAYER + 0.5f) * 64 * 0.82f;
            Vector2 posDomino = new Vector2(pos, 0);
            domino.transform.DOLocalMove(posDomino, 0.5f).SetDelay(0.02f * i);
            listDominoInTable.Add(domino);
            domino.transform.SetSiblingIndex(1);
        }
        await Task.Delay(1000);
        int id = (int)data["nextTurn"];
        Player playerStart = getPlayerWithID(id);
        for (int i = 0; i < playerCount; i++)
        {
            players[i].playerView.showDealer(players[i] == playerStart, players[i]._indexDynamic > 1, true);
            PlayerViewDomino playerDomino = (PlayerViewDomino)players[i].playerView;
            if (players[i].id != thisPlayer.id)
            {
                if (playerDomino.transform.position.x < 0)
                {
                    playerDomino.transform.Find("Card").position = playerDomino.transform.Find("Card_1").position;
                    playerDomino.transform.Find("lewal").position = playerDomino.transform.Find("lewal_1").position;
                    playerDomino.lewal.GetComponent<Image>().sprite = listLewal[1];
                }
                playerDomino.transform.Find("Card").gameObject.SetActive(true);
            }
            else
            {
                playerDomino.transform.Find("lewal").position = playerDomino.transform.Find("lewal_1").position;
                playerDomino.lewal.GetComponent<Image>().sprite = listLewal[1];
            }
        };
        playerStart.setHost(true);
        int playerStartIndex = playerStart._indexDynamic;
        List<int> myDomino = data["arrCard"].ToObject<List<int>>();
        int count = 0;
        SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIA_DOMINO);
        for (int i = 0; i < playerCount * DOMINOS_PER_PLAYER; i++)
        {
            int number = 6 - i / playerCount;
            if (i % playerCount == 0) //thisPlayer
            {
                float pos = 195 - 65 * i / playerCount;
                Vector2 posDomino = new Vector2(pos, -255);
                listDominoInTable[i].transform.DOLocalMove(posDomino, 0.5f).SetDelay(0.02f * (playerCount * DOMINOS_PER_PLAYER - i));
                listDominoInTable[i].transform.DOScale(1.1f, 1.1f).SetDelay(0.02f * (playerCount * DOMINOS_PER_PLAYER - i));
                setDomino(listDominoInTable[i], myDomino[number]);
                listDominoInTable[i].setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
                listDominoInTable[i].cardID = myDomino[number];
                PlayerViewDomino myPlayer = (PlayerViewDomino)thisPlayer.playerView;
                myPlayer.listMyDomino.Add(listDominoInTable[i]);
                listDominoDrop.Add(myDomino[number]);
            }
            else //other
            {
                if (players[count] == thisPlayer)
                {
                    count = (count + 1) % playerCount;
                }
                PlayerViewDomino playerDomino = (PlayerViewDomino)players[count].playerView;
                Vector2 pos = Globals.Config.getPosInOtherNode(playerDomino.domino.transform.position, DominoContainer);
                listDominoInTable[i].transform.DOLocalMove(pos, 0.5f).SetDelay(0.02f * (playerCount * DOMINOS_PER_PLAYER - i));
                listDominoInTable[i].transform.DOScale(0.6f, 0.6f).SetDelay(0.02f * (playerCount * DOMINOS_PER_PLAYER - i));
                playerDomino.lbDominoRemain.text = int.Parse(playerDomino.lbDominoRemain.text) + 1 + "";
                playerDomino.listMyDomino.Add(listDominoInTable[i]);
                count = (count + 1) % playerCount;
            }
        }
        await Task.Delay(1000);
        for (int i = playerCount * DOMINOS_PER_PLAYER - 1; i >= 0; i--)
        {
            if (i % playerCount == 0)
            {
                await Task.Delay(12 * i);
                listDominoInTable[i].transform.Find("up_duoi").gameObject.SetActive(false);
            }
        }
        setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        JObject diceRemainData = (JObject)data["mapNode"];
        diceRemainBox.updateDiceRemain(diceRemainData);
        diceRemainBox.gameObject.SetActive(true);
    }

    public async void handleDc(JObject data)
    {
        //{ "evt":"disCard","turnTime":10,"pid":4315596,"nextTurn":5946088,"idCard":9,"side":"head","mapNode":{ "0":3,"1":4,"2":4,"3":4,"4":5,"5":3,"6":7} }
        int pid = (int)data["pid"];
        int idCard = (int)data["idCard"];
        idDiscard = idCard;
        string side = (string)data["side"];
        Player player = getPlayerWithID(pid);
        PlayerViewDomino playerDomino = (PlayerViewDomino)player.playerView;
        bool checkLastCard = false;
        if (playerDomino.listMyDomino.Count == 1)
        {
            checkLastCard = true;
        }
        if (pid == thisPlayer.id)
        {
            int num = playerDomino.findCardID(idCard);
            Vector2 pos;
            Vector3 rot;
            if (checkLastCard)
            {
                for (int i = 0; i < listDominoPlayed.Count; i++)
                {
                    listDominoPlayed[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                if (idCard / 7 == tail || idCard % 7 == tail)
                {
                    listDominoPlayed[indexTail].GetComponent<CanvasGroup>().alpha = 1f;
                }
                if (idCard / 7 == head || idCard % 7 == head)
                {
                    listDominoPlayed[indexHead].GetComponent<CanvasGroup>().alpha = 1f;
                }
            }
            if (side == "head")
            {
                dominoInfoHead = getPositionDomino(playerDomino.listMyDomino[num], side, dominoInfoHead);
                setDominoForm(playerDomino.listMyDomino[num], dominoInfoHead[2]);
                pos = new Vector2((float)dominoInfoHead[0], (float)dominoInfoHead[1]);
                rot = new Vector3(0, 0, (float)dominoInfoHead[2]);
            }
            else
            {
                dominoInfoTail = getPositionDomino(playerDomino.listMyDomino[num], side, dominoInfoTail);
                setDominoForm(playerDomino.listMyDomino[num], dominoInfoTail[2]);
                pos = new Vector2((float)dominoInfoTail[0], (float)dominoInfoTail[1]);
                rot = new Vector3(0, 0, (float)dominoInfoTail[2]);
            }
            playerDomino.listMyDomino[num].transform.DOLocalMove(pos, 0.5f).SetDelay(0.05f).OnComplete(() =>
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.DC_DOMINO);
            });
            playerDomino.listMyDomino[num].transform.DOScale(0.6f, 0.6f).SetDelay(0.05f);
            playerDomino.listMyDomino[num].transform.DOLocalRotate(rot, 0.5f).SetDelay(0.05f);
            playerDomino.listMyDomino[num].transform.SetSiblingIndex(0);
            listDominoPlayed.Add(playerDomino.listMyDomino[num]);
            playerDomino.listMyDomino.RemoveAt(num);
            for (int i = 0; i < playerDomino.listMyDomino.Count; i++)
            {
                float pos1 = -195 + 65 * (playerDomino.listMyDomino.Count - i - 1);
                Vector2 posDomino = new Vector2(pos1, -255);
                if (isHoldCard)
                {
                    if (playerDomino.listMyDomino[i].cardID == dominoHolding.cardID)
                    {
                        localDominoPos = posDomino;
                    }
                }
                playerDomino.listMyDomino[i].transform.DOLocalMove(posDomino, 0.4f);
            }
            if (playerDomino.listMyDomino.Count == 1)
            {
                NoticeLastDomino.SetActive(true);
                checkLast = true;
            }
        }
        else
        {
            Domino domino = playerDomino.listMyDomino[playerDomino.listMyDomino.Count - 1];
            setDomino(domino, idCard);
            domino.transform.Find("up_duoi").gameObject.SetActive(false);
            Vector2 pos;
            Vector3 rot;
            if (checkLastCard)
            {
                for (int i = 0; i < listDominoPlayed.Count; i++)
                {
                    listDominoPlayed[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                if (idCard / 7 == tail || idCard % 7 == tail)
                {
                    listDominoPlayed[indexTail].GetComponent<CanvasGroup>().alpha = 1f;
                }
                if (idCard / 7 == head || idCard % 7 == head)
                {
                    listDominoPlayed[indexHead].GetComponent<CanvasGroup>().alpha = 1f;
                }
            }
            if (side == "head")
            {
                dominoInfoHead = getPositionDomino(domino, side, dominoInfoHead);
                setDominoForm(domino, dominoInfoHead[2]);
                pos = new Vector2((float)dominoInfoHead[0], (float)dominoInfoHead[1]);
                rot = new Vector3(0, 0, (float)dominoInfoHead[2]);
            }
            else
            {
                dominoInfoTail = getPositionDomino(domino, side, dominoInfoTail);
                setDominoForm(domino, dominoInfoTail[2]);
                pos = new Vector2((float)dominoInfoTail[0], (float)dominoInfoTail[1]);
                rot = new Vector3(0, 0, (float)dominoInfoTail[2]);
            }
            playerDomino.listMyDomino.RemoveAt(playerDomino.listMyDomino.Count - 1);
            playerDomino.lbDominoRemain.text = int.Parse(playerDomino.lbDominoRemain.text) - 1 + "";
            if (playerDomino.listMyDomino.Count == 0)
            {
                playerDomino.transform.Find("Card").gameObject.SetActive(false);
            }
            domino.transform.DOLocalMove(pos, 0.5f).SetDelay(0.05f).OnComplete(() =>
            {
                SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.DC_DOMINO);
            });
            domino.transform.DOLocalRotate(rot, 0.5f).SetDelay(0.05f);
            domino.transform.SetSiblingIndex(0);
            listDominoPlayed.Add(domino);
        }
        if (stateGame != Globals.STATE_GAME.VIEWING)
        {
            JObject diceRemainData = (JObject)data["mapNode"];
            diceRemainBox.updateDiceRemain(diceRemainData);
        }
        if (playerDomino.listMyDomino.Count > 0)
        {
            setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        }
    }

    public override void handleSTable(string objData)
    {
        base.handleSTable(objData);
        JObject data = JObject.Parse(objData);
        JArray ArrP = getJArray(data, "ArrP");
        listDominoInTable.Clear();
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            if (getInt(dataPlayer, "id") == thisPlayer.id)
            {
                isAutoPlayLastOne = (bool)dataPlayer["isAutoPlayLastOne"];
                NoticeLastDomino.GetComponent<Toggle>().isOn = (bool)dataPlayer["isAutoPlayLastOne"];
            }
        }
    }

    public void changeOderDisCard()
    {
        isAutoPlayLastOne = NoticeLastDomino.GetComponent<Toggle>().isOn;
        if (checkLast)
        {
            SocketSend.autoDisLastOne(isAutoPlayLastOne);
        }
    }

    public void setStateAutoDisCard(JObject data)
    {
        NoticeLastDomino.GetComponent<Toggle>().isOn = (bool)data["status"];
    }

    public override void handleJTable(string objData)
    {
        base.handleJTable(objData);
    }

    public override void handleVTable(string objData)
    {
        base.handleVTable(objData);
        diceRemainBox.gameObject.SetActive(false);
        JObject data = JObject.Parse(objData);
        JArray ArrP = getJArray(data, "ArrP");
        listDominoInTable.Clear();
        int id = (int)data["startPlayer"];
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            Player player = getPlayerWithID(getInt(dataPlayer, "id"));
            player.playerView.showDealer(getInt(dataPlayer, "id") == id, players[i]._indexDynamic > 1, true);
            PlayerViewDomino playerDomino = (PlayerViewDomino)player.playerView;
            List<int> Arr = getListInt(dataPlayer, "Arr");
            List<int> folds = dataPlayer["folds"].ToObject<List<int>>();
            playerDomino.resetFoldDice();
            playerDomino.setFoldDice(folds);
            if (playerDomino.transform.position.x < 0)
            {
                playerDomino.transform.Find("Card").position = playerDomino.transform.Find("Card_1").position;
                playerDomino.transform.Find("lewal").position = playerDomino.transform.Find("lewal_1").position;
                playerDomino.lewal.GetComponent<Image>().sprite = listLewal[1];
            }
            Vector2 pos = Globals.Config.getPosInOtherNode(playerDomino.domino.transform.position, DominoContainer);
            for (int j = 0; j < Arr.Count; j++)
            {
                Domino domino = getDomino();
                domino.transform.localPosition = pos;
                domino.transform.localScale = new Vector2(0.6f, 0.6f);
                playerDomino.listMyDomino.Add(domino);
                listDominoInTable.Add(domino);
            }
            playerDomino.transform.Find("Card").gameObject.SetActive(true);
            playerDomino.lbDominoRemain.text = Arr.Count + "";
        }
        List<int> deck = getListInt(data, "deck");
        int index = 0;
        if (deck.Count > 0)
        {
            index = deck.IndexOf((int)data["idCardRoot"]);
        }
        Vector2 posD;
        Vector3 rot;
        Vector2 scaleD = new Vector2(0.6f, 0.6f);
        Quaternion newRotation;
        for (int i = index; i < deck.Count; i++)
        {
            Domino domino = getDomino();
            setDomino(domino, deck[i]);
            if (i == index)
            {
                dominoInfoHead = getPositionDomino(domino, "first", dominoInfoHead);
                setDominoForm(domino, dominoInfoHead[2]);
            }
            else
            {
                dominoInfoHead = getPositionDomino(domino, "head", dominoInfoHead);
                setDominoForm(domino, dominoInfoHead[2]);
            }
            posD = new Vector2((float)dominoInfoHead[0], (float)dominoInfoHead[1]);
            newRotation = Quaternion.Euler(0, 0, (float)dominoInfoHead[2]);
            domino.transform.Find("up_duoi").gameObject.SetActive(false);
            domino.transform.localPosition = posD;
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = scaleD;
            listDominoInTable.Add(domino);
            listDominoPlayed.Add(domino);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            Domino domino = getDomino();
            setDomino(domino, deck[i]);
            dominoInfoTail = getPositionDomino(domino, "tail", dominoInfoTail);
            setDominoForm(domino, dominoInfoTail[2]);
            posD = new Vector2((float)dominoInfoTail[0], (float)dominoInfoTail[1]);
            newRotation = Quaternion.Euler(0, 0, (float)dominoInfoTail[2]);
            domino.transform.Find("up_duoi").gameObject.SetActive(false);
            domino.transform.localPosition = posD;
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = scaleD;
            listDominoInTable.Add(domino);
            listDominoPlayed.Add(domino);
        }
        setPlayerTurn((int)data["currentTurn"], (int)data["CT"]);
    }

    public override void handleRJTable(string objData)
    {
        base.handleRJTable(objData);
        cleanTable();
        stateGame = Globals.STATE_GAME.PLAYING;
        JObject data = JObject.Parse(objData);
        JArray ArrP = getJArray(data, "ArrP");
        listDominoInTable.Clear();
        int idStart = (int)data["startPlayer"];
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            int id = getInt(dataPlayer, "id");
            Player player = getPlayerWithID(id);
            player.playerView.showDealer(getInt(dataPlayer, "id") == idStart, players[i]._indexDynamic > 1, true);
            PlayerViewDomino playerDomino = (PlayerViewDomino)player.playerView;
            List<int> Arr = getListInt(dataPlayer, "Arr");
            List<int> folds = dataPlayer["folds"].ToObject<List<int>>();
            playerDomino.resetFoldDice();
            playerDomino.setFoldDice(folds);
            if (id != thisPlayer.id) // other player
            {
                if (playerDomino.transform.position.x < 0)
                {
                    playerDomino.transform.Find("Card").position = playerDomino.transform.Find("Card_1").position;
                    playerDomino.transform.Find("lewal").position = playerDomino.transform.Find("lewal_1").position;
                    playerDomino.lewal.GetComponent<Image>().sprite = listLewal[1];
                }
                Vector2 pos = Globals.Config.getPosInOtherNode(playerDomino.domino.transform.position, DominoContainer);
                for (int j = 0; j < Arr.Count; j++)
                {
                    Domino domino = getDomino();
                    domino.transform.localPosition = pos;
                    domino.transform.localScale = new Vector2(0.6f, 0.6f);
                    playerDomino.listMyDomino.Add(domino);
                    listDominoInTable.Add(domino);
                }
                playerDomino.transform.Find("Card").gameObject.SetActive(true);
                playerDomino.lbDominoRemain.text = Arr.Count + "";
            }
            else // this player
            {
                isAutoPlayLastOne = (bool)dataPlayer["isAutoPlayLastOne"];
                NoticeLastDomino.GetComponent<Toggle>().isOn = (bool)dataPlayer["isAutoPlayLastOne"];
                for (int j = Arr.Count - 1; j >= 0; j--)
                {
                    float pos = -195 + 65 * (j);
                    Vector2 posDomino = new Vector2(pos, -255);
                    Domino domino = getDomino();
                    setDomino(domino, Arr[j]);
                    domino.transform.localScale = new Vector2(1.1f, 1.1f);
                    domino.transform.localPosition = posDomino;
                    listDominoInTable.Add(domino);
                    playerDomino.listMyDomino.Add(domino);
                    domino.transform.Find("up_duoi").gameObject.SetActive(false);
                    domino.setListenerDragDrop(OnBeginDrag, OnDrag, OnEndDrag);
                    domino.transform.SetSiblingIndex(1);
                    listDominoDrop.Add(Arr[j]);
                }
                playerDomino.transform.Find("lewal").position = playerDomino.transform.Find("lewal_1").position;
                playerDomino.lewal.GetComponent<Image>().sprite = listLewal[1];
                JObject diceRemainData = (JObject)dataPlayer["mapNode"];
                diceRemainBox.updateDiceRemain(diceRemainData);
            }
        }
        List<int> deck = getListInt(data, "deck");
        int index = 0;
        if (deck.Count > 0)
        {
            index = deck.IndexOf((int)data["idCardRoot"]);
        }
        Vector2 posD;
        Vector3 rot;
        Vector2 scaleD = new Vector2(0.6f, 0.6f);
        Quaternion newRotation;
        for (int i = index; i < deck.Count; i++)
        {
            Domino domino = getDomino();
            setDomino(domino, deck[i]);
            listDominoDrop.Add(deck[i]);
            if (i == index)
            {
                dominoInfoHead = getPositionDomino(domino, "first", dominoInfoHead);
                setDominoForm(domino, dominoInfoHead[2]);
            }
            else
            {
                dominoInfoHead = getPositionDomino(domino, "head", dominoInfoHead);
                setDominoForm(domino, dominoInfoHead[2]);
            }
            posD = new Vector2((float)dominoInfoHead[0], (float)dominoInfoHead[1]);
            newRotation = Quaternion.Euler(0, 0, (float)dominoInfoHead[2]);
            domino.transform.Find("up_duoi").gameObject.SetActive(false);
            domino.transform.localPosition = posD;
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = scaleD;
            domino.transform.SetSiblingIndex(0);
            listDominoInTable.Add(domino);
            listDominoPlayed.Add(domino);
        }
        for (int i = index - 1; i >= 0; i--)
        {
            Domino domino = getDomino();
            setDomino(domino, deck[i]);
            listDominoDrop.Add(deck[i]);
            dominoInfoTail = getPositionDomino(domino, "tail", dominoInfoTail);
            setDominoForm(domino, dominoInfoTail[2]);
            posD = new Vector2((float)dominoInfoTail[0], (float)dominoInfoTail[1]);
            newRotation = Quaternion.Euler(0, 0, (float)dominoInfoTail[2]);
            domino.transform.Find("up_duoi").gameObject.SetActive(false);
            domino.transform.localPosition = posD;
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = scaleD;
            domino.transform.SetSiblingIndex(0);
            listDominoInTable.Add(domino);
            listDominoPlayed.Add(domino);
        }
        setPlayerTurn((int)data["currentTurn"], (int)data["CT"]);
        diceRemainBox.gameObject.SetActive(true);
    }

    public override void handleCTable(string objData)
    {
        base.handleCTable(objData);
        diceRemainBox.gameObject.SetActive(false);
    }

    public async void handleFinishGame(JObject data)
    {
        setPlayerTurn(-1, 1);
        NoticeLastDomino.SetActive(false);
        checkLast = false;
        string str = "single";
        int num = (int)data["winFactor"];
        switch (num)
        {
            case 2:
                {
                    str = "double";
                    break;
                }
            case 3:
                {
                    str = "triple";
                    break;
                }
            case 4:
                {
                    str = "quartet";
                    break;
                }
            case 5:
                {
                    str = "quintet";
                    break;
                }
        }
        if (str == "single")
        {
            SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.SINGLE);
        }
        else
        {
            SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.SHOW_RESULTS);
        }
        showAnimationResult(str);
        await Task.Delay(700);
        Quaternion newRotation = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < listDominoPlayed.Count; i++)
        {
            listDominoPlayed[i].transform.Find("up_duoi").gameObject.SetActive(true);
            listDominoPlayed[i].transform.localRotation = newRotation;
            listDominoPlayed[i].transform.localScale = new Vector2(1, 1);
            listDominoInTable.Remove(listDominoPlayed[i]);
            putDomino(listDominoPlayed[i]);
        }
        listDominoPlayed.Clear();
        JArray ArrP = getJArray(data, "players");
        for (int i = 0; i < ArrP.Count; i++)
        {
            JObject dataPlayer = (JObject)ArrP[i];
            Player player = getPlayerWithID(getInt(dataPlayer, "userId"));
            PlayerViewDomino playerDomino = (PlayerViewDomino)player.playerView;
            if (player.id == thisPlayer.id) // this player
            {
                for (int j = 0; j < playerDomino.listMyDomino.Count; j++)
                {
                    playerDomino.listMyDomino[j].GetComponent<CanvasGroup>().alpha = 0.5f;
                }
            }
            else // other players
            {
                playerDomino.lbDominoRemain.text = 0 + "";
                playerDomino.transform.Find("Card").gameObject.SetActive(false);
                List<int> arrCard = getListInt(dataPlayer, "arrCard");
                for (int k = 0; k < arrCard.Count; k++)
                {
                    setDomino(playerDomino.listMyDomino[k], arrCard[k]);
                    Vector2 posD = Globals.Config.getPosInOtherNode(playerDomino.transform.Find("Card").position, DominoContainer);
                    if (playerDomino.transform.Find("Card").localPosition.x > 0)
                    {
                        posD = posD + new Vector2(10 + k * 37, 0);
                    }
                    else
                    {
                        posD = posD - new Vector2(10 + k * 37, 0);
                    }
                    playerDomino.listMyDomino[k].transform.DOLocalMove(posD, 0.3f).SetEase(Ease.InOutCubic);
                    playerDomino.listMyDomino[k].GetComponent<CanvasGroup>().alpha = 0.5f;
                    await Task.Delay(12 * k);
                    playerDomino.listMyDomino[k].transform.Find("up_duoi").gameObject.SetActive(false);
                }
            }
            long ag = getLong(dataPlayer, "ag");
            long bet = getLong(dataPlayer, "M");
            player.playerView.effectFlyMoney(bet, 40);
            if (bet > 0)
            {
                player.playerView.setEffectWin("", false);
            }
            if (bet < 0)
            {
                player.playerView.setEffectLose(false);
            }
            SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.CHIP_WIN);
            player.ag = ag;
            player.setAg();
        }
        await Task.Delay(2000);
        listDominoInTable.ForEach(domino =>
        {
            domino.transform.Find("up_duoi").gameObject.SetActive(true);
            domino.transform.localRotation = newRotation;
            domino.transform.localScale = new Vector2(1, 1);
            putDomino(domino);
        });
        listDominoInTable.Clear();
        diceInfo.SetActive(false);
        resetGame = 0;
        diceRemainBox.gameObject.SetActive(false);
        checkAutoExit();
    }

    public async void handleFold(JObject data)
    {
        Player playerFold = getPlayerWithID(getInt(data, "uidFold"));
        Player playerWin = getPlayerWithID(getInt(data, "uidWin"));
        long agFold = getLong(data, "agFold");
        List<int> folds = data["folds"].ToObject<List<int>>();
        playerFold.playerView.effectFlyMoney(-agFold, 40);
        playerFold.ag = (long)data["agUserFold"];
        playerFold.setAg();
        SoundManager.instance.playEffectFromPath(Globals.SOUND_DOMINO.FOLD);
        PlayerViewDomino playerDomino = (PlayerViewDomino)playerFold.playerView;
        playerDomino.lewal.SetActive(true);
        playerDomino.setFoldDice(folds);
        playerWin.playerView.effectFlyMoney(agFold, 40);
        playerWin.ag = (long)data["agUserWin"];
        playerWin.setAg();
        setPlayerTurn((int)data["nextTurn"], (int)data["turnTime"]);
        await Task.Delay(1000);
        playerDomino.lewal.SetActive(false);
    }

    public List<double> getPositionDomino(Domino domino, string side, List<double> preData, bool isReal = true)
    {
        double rotation = 0;
        double x = 0, y = 0;
        const double doc = 51;
        const double ngang = 70;
        const double maxTail = -292;
        const double maxHead = 292;
        int tail_ = tail, head_ = head;
        double tailLoc_ = tailLoc, headLoc_ = headLoc;
        switch (side)
        {
            case "first":
                {
                    if (isReal)
                    {
                        indexTail = listDominoPlayed.Count;
                        indexHead = listDominoPlayed.Count;
                    }
                    if (!domino.isSamePoint())
                    {
                        rotation = -90;
                    }
                    head = domino.cardID / 7;
                    tail = domino.cardID % 7;
                    dominoInfoHead[2] = rotation;
                    dominoInfoTail[2] = rotation;
                    break;
                }
            case "tail":
                {
                    if (isReal)
                    {
                        indexTail = listDominoPlayed.Count;
                    }
                    if (tailLoc < maxTail && preData[2] != 0)
                    {
                        checkTail = true;
                    }
                    if (checkTail)
                    {
                        if (preData[1] == 0)
                        {
                            if (domino.cardID / 7 == tail)
                            {
                                rotation = 180;
                                tail = domino.cardID % 7;
                            }
                            else
                            {
                                tail = domino.cardID / 7;
                            }
                            x = tailLoc - 18;
                            y = doc + 3;
                        }
                        else if (preData[1] == doc + 3)
                        {
                            if (domino.cardID / 7 == tail)
                            {
                                rotation = 90;
                                tail = domino.cardID % 7;
                            }
                            else
                            {
                                rotation = -90;
                                tail = domino.cardID / 7;
                            }
                            y = 2 * doc + 2;
                            x = tailLoc + 18;
                        }
                        else
                        {
                            if (domino.cardID / 7 == tail)
                            {
                                rotation = 90;
                                tail = domino.cardID % 7;
                            }
                            else
                            {
                                rotation = -90;
                                tail = domino.cardID / 7;
                            }
                            if (preData[2] == 0) // domino truoc doc
                            {
                                x = tailLoc + doc;
                            }
                            else // domino truoc ngang
                            {
                                if (domino.isSamePoint())
                                {
                                    x = tailLoc + doc;
                                    rotation = 0;
                                }
                                else
                                {
                                    x = tailLoc + ngang;
                                }
                            }
                            y = 2 * doc + 2;
                        }
                    }
                    else
                    {
                        if (domino.cardID / 7 == tail)
                        {
                            rotation = -90;
                            tail = domino.cardID % 7;
                        }
                        else
                        {
                            rotation = 90;
                            tail = domino.cardID / 7;
                        }
                        if (preData[2] == 0) // domino truoc doc
                        {
                            x = tailLoc - doc;
                        }
                        else // domino truoc ngang
                        {
                            if (domino.isSamePoint())
                            {
                                x = tailLoc - doc;
                                rotation = 0;
                            }
                            else
                            {
                                x = tailLoc - ngang;
                            }
                        }
                    }
                    tailLoc = x;
                    break;
                }
            case "head":
                {
                    if (isReal)
                    {
                        indexHead = listDominoPlayed.Count;
                    }
                    if (headLoc > maxHead && preData[2] != 0)
                    {
                        checkHead = true;
                    }
                    if (checkHead)
                    {
                        if (preData[1] == 0)
                        {
                            if (domino.cardID % 7 == head)
                            {
                                rotation = 180;
                                head = domino.cardID / 7;
                            }
                            else
                            {
                                head = domino.cardID % 7;
                            }
                            x = headLoc + 18;
                            y = -doc + 2;
                        }
                        else if (preData[1] == -doc + 2)
                        {
                            if (domino.cardID / 7 == head)
                            {
                                rotation = -90;
                                head = domino.cardID % 7;
                            }
                            else
                            {
                                rotation = 90;
                                head = domino.cardID / 7;
                            }
                            y = -2 * doc - 2;
                            x = headLoc - 18;
                        }
                        else
                        {
                            if (domino.cardID / 7 == head)
                            {
                                rotation = -90;
                                head = domino.cardID % 7;
                            }
                            else
                            {
                                rotation = 90;
                                head = domino.cardID / 7;
                            }
                            if (preData[2] == 0) // domino truoc doc
                            {
                                x = headLoc - doc;
                            }
                            else // domino truoc ngang
                            {
                                if (domino.isSamePoint())
                                {
                                    x = headLoc - doc;
                                    rotation = 0;
                                }
                                else
                                {
                                    x = headLoc - ngang;
                                }
                            }
                            y = -2 * doc - 2;
                        }
                    }
                    else
                    {
                        if (domino.cardID / 7 == head)
                        {
                            rotation = 90;
                            head = domino.cardID % 7;
                        }
                        else
                        {
                            rotation = -90;
                            head = domino.cardID / 7;
                        }
                        if (preData[2] == 0) // domino truoc doc
                        {
                            x = headLoc + doc;
                        }
                        else // domino truoc ngang
                        {
                            if (domino.isSamePoint())
                            {
                                x = headLoc + doc;
                                rotation = 0;
                            }
                            else
                            {
                                x = headLoc + ngang;
                            }
                        }
                    }
                    headLoc = x;
                    break;
                }
        }
        //setDominoForm(domino, rotation);
        List<double> data = new List<double>();
        data.Add(x);
        data.Add(y);
        data.Add(rotation);
        if (!isReal)
        {
            tail = tail_;
            head = head_;
            tailLoc = tailLoc_;
            headLoc = headLoc_;
        }
        return data;
    }

    public void showAnimationResult(string str)
    {
        animResult.startingAnimation = str;
        animResult.gameObject.SetActive(true);
        animResult.Initialize(true);
        animResult.AnimationState.SetAnimation(0, str, false);
        animResult.AnimationState.Complete += delegate
        {
            animResult.gameObject.SetActive(false);
        };
    }

    public void setPlayerTurn(int playerID, int time)
    {
        if (isMyTurn)
        {
            dominoHead.gameObject.SetActive(false);
            dominoTail.gameObject.SetActive(false);
            if (isHoldCard)
            {
                if (idDiscard != dominoHolding.cardID)
                {
                    setDominoForm(dominoHolding, 0);
                    dominoHolding.transform.DOLocalMove(localDominoPos, 0.5f).SetDelay(0.05f);
                }
            }
        }
        isMyTurn = (playerID == thisPlayer.id) ? true : false;
        checkMyDomino();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].setTurn(false);
        }
        Player player = getPlayerWithID(playerID);
        if (player != null)
        {
            player.setTurn(true, time);
        }
        isHoldCard = false;
    }

    public void onPointerDownDice(int diceNumber)
    {
        if (stateGame == Globals.STATE_GAME.PLAYING)
        {
            for (int i = 0; i < 7; i++)
            {
                setDomino(listDominoDice[i], i * 7 + diceNumber);
                if (i <= diceNumber)
                {
                    if (listDominoDrop.Contains(i * 7 + diceNumber))
                    {
                        listDominoDice[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                    }
                    else
                    {
                        listDominoDice[i].GetComponent<CanvasGroup>().alpha = 1f;
                    }
                }
                else
                {
                    if (listDominoDrop.Contains(diceNumber * 7 + i))
                    {
                        listDominoDice[i].GetComponent<CanvasGroup>().alpha = 0.5f;
                    }
                    else
                    {
                        listDominoDice[i].GetComponent<CanvasGroup>().alpha = 1f;
                    }
                }
            }
            diceInfo.SetActive(true);
        }
    }

    public void onPointerUpDice(int diceNumber)
    {
        diceInfo.SetActive(false);
    }


    public void checkMyDomino()
    {
        PlayerViewDomino playerDomino = (PlayerViewDomino)thisPlayer.playerView;
        for (int i = 0; i < playerDomino.listMyDomino.Count; i++)
        {
            int cardID = playerDomino.listMyDomino[i].cardID;
            playerDomino.listMyDomino[i].GetComponent<CanvasGroup>().alpha = 1;
            if (isMyTurn && !checkDomino(playerDomino.listMyDomino[i]))
            {
                playerDomino.listMyDomino[i].GetComponent<CanvasGroup>().alpha = 0.5f;
            }
        }
    }

    public bool checkDomino(Domino domino)
    {
        int cardID = domino.cardID;
        if (head == -1 || cardID / 7 == tail || cardID % 7 == tail || cardID / 7 == head || cardID % 7 == head)
        {
            return true;
        }
        return false;
    }

    public override void setGameInfo(int m, int id = 0, int maxBet = 0)
    {
        base.setGameInfo(m, id, maxBet);
        textBet.text = "Bertaruh : " + Globals.Config.FormatMoney(m, true);
    }

    private Domino getDomino()
    {
        Domino domino;
        if (dominoPool.Count > 0)
        {
            domino = dominoPool[0];
            dominoPool.Remove(domino);
            domino.transform.parent = DominoContainer.transform;
        }
        else
        {
            domino = Instantiate(dominoPref, DominoContainer.transform);

        }
        domino.gameObject.SetActive(true);
        setDominoForm(domino, 0);
        return domino;
    }

    private void setDomino(Domino domino, int id)
    {
        domino.cardID = id;
        domino.transform.Find("top").GetComponent<Image>().sprite = domino.listSpriteDomino[id / 7];
        domino.transform.Find("bottom").GetComponent<Image>().sprite = domino.listSpriteDomino[id % 7];
        domino.transform.Find("top").GetComponent<Image>().SetNativeSize();
        domino.transform.Find("bottom").GetComponent<Image>().SetNativeSize();
    }

    public void setDominoForm(Domino domino, double rotation)
    {
        if (rotation == 90)
        {
            Vector2 posTop = new Vector2(10f, 30f);
            Vector2 posBottom = new Vector2(10f, -30f);
            domino.GetComponent<Image>().sprite = domino.listSpriteBgDomino[1];
            domino.transform.Find("top").localPosition = posTop;
            domino.transform.Find("bottom").localPosition = posBottom;
        }
        else if (rotation == -90)
        {
            Vector2 posTop = new Vector2(-10f, 26f);
            Vector2 posBottom = new Vector2(-10f, -26f);
            domino.GetComponent<Image>().sprite = domino.listSpriteBgDomino[2];
            domino.transform.Find("top").localPosition = posTop;
            domino.transform.Find("bottom").localPosition = posBottom;
        }
        else
        {
            Vector2 posTop = new Vector2(0f, 35f);
            Vector2 posBottom = new Vector2(0f, -18.7f);
            domino.GetComponent<Image>().sprite = domino.listSpriteBgDomino[0];
            domino.transform.Find("top").localPosition = posTop;
            domino.transform.Find("bottom").localPosition = posBottom;
        }
    }

    private void putDomino(Domino domino)
    {
        dominoPool.Add(domino);
        domino.transform.SetParent(null);
        domino.gameObject.SetActive(false);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (dominoPool != null)
        {
            dominoPool.ForEach(it =>
            {
                Destroy(it.gameObject);
            });
            dominoPool.Clear();
        }
    }

    Vector2 localDominoPos;
    void OnBeginDrag(PointerEventData eventData, Domino card)
    {
        isHoldCard = true;
        dominoHolding = card;
        localDominoPos = card.transform.localPosition;
        int cardID = card.cardID;
        Vector2 pos;
        Quaternion newRotation;
        List<double> dominoInfo = new List<double> { 0, 0, 0 };
        if (isMyTurn && card.GetComponent<CanvasGroup>().alpha == 1)
        {
            if (head == -1)
            {
                string side = "first";
                setDomino(dominoHead, cardID);
                dominoInfo = getPositionDomino(card, side, dominoInfoHead, false);
                setDominoForm(dominoHead, dominoInfo[2]);
                pos = new Vector2((float)dominoInfo[0], (float)dominoInfo[1]);
                newRotation = Quaternion.Euler(0, 0, (float)dominoInfo[2]);
                dominoHead.transform.localPosition = pos;
                dominoHead.transform.localRotation = newRotation;
                dominoHead.gameObject.SetActive(true);
            }
            if (cardID / 7 == tail || cardID % 7 == tail)
            {
                string side = "tail";
                setDomino(dominoTail, cardID);
                dominoInfo = getPositionDomino(card, side, dominoInfoTail, false);
                setDominoForm(dominoTail, dominoInfo[2]);
                pos = new Vector2((float)dominoInfo[0], (float)dominoInfo[1]);
                newRotation = Quaternion.Euler(0, 0, (float)dominoInfo[2]);
                dominoTail.transform.localPosition = pos;
                dominoTail.transform.localRotation = newRotation;
                dominoTail.gameObject.SetActive(true);
            }
            if (cardID / 7 == head || cardID % 7 == head)
            {
                string side = "head";
                setDomino(dominoHead, cardID);
                dominoInfo = getPositionDomino(card, side, dominoInfoHead, false);
                setDominoForm(dominoHead, dominoInfo[2]);
                pos = new Vector2((float)dominoInfo[0], (float)dominoInfo[1]);
                newRotation = Quaternion.Euler(0, 0, (float)dominoInfo[2]);
                dominoHead.transform.localPosition = pos;
                dominoHead.transform.localRotation = newRotation;
                dominoHead.gameObject.SetActive(true);
            }
        }
    }

    void OnDrag(PointerEventData eventData, Domino card)
    {
        Vector2 posTouch = card.transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);// - new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
        //Vector2 posTouch1 = Globals.Config.getPosInOtherNode(eventData.position, DominoContainer);
        //Vector2 posTouch2 = Globals.Config.getPosInOtherNode(posTouch, DominoContainer);
        Debug.Log("posTouch = " + posTouch);
        //Debug.Log("posTouch1 = " + posTouch1);
        //Debug.Log("posTouch2 = " + posTouch2);
        //card.transform.localPosition = posTouch;


        if (isMyTurn && card.GetComponent<CanvasGroup>().alpha == 1 && card.transform.localScale.x == 1.1f)
        {
            if (posTouch.x > Screen.currentResolution.width / 2 - 40)
                posTouch.x = Screen.currentResolution.width / 2 - 40;
            if (posTouch.x < -Screen.currentResolution.width / 2 + 40)
                posTouch.x = -Screen.currentResolution.width / 2 + 40;
            if (posTouch.y > Screen.currentResolution.height / 2 - 78)
                posTouch.y = Screen.currentResolution.height / 2 - 78;
            if (posTouch.y < -Screen.currentResolution.height / 2 + 65)
                posTouch.y = -Screen.currentResolution.height / 2 + 65;
            card.transform.localPosition = posTouch;
        }
    }

    void OnEndDrag(PointerEventData eventData, Domino card)
    {
        string side = "none";
        int cardID = card.cardID;
        bool checkTail1 = false, checkHead1 = false;
        if (isMyTurn && card.GetComponent<CanvasGroup>().alpha == 1)
        {
            if (head == -1)
            {
                side = "first";
            }
            if (cardID / 7 == tail || cardID % 7 == tail)
            {
                checkTail1 = true;
                side = "tail";
            }
            if (cardID / 7 == head || cardID % 7 == head)
            {
                checkHead1 = true;
                side = "head";
            }
        }
        if (checkTail1 == checkHead1 && side == "head" && isMyTurn)
        {
            var posTouch = card.transform.parent.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition); //eventData.position - new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
            if (tailLoc == headLoc)
            {
                if (posTouch.x < 0)
                    side = "tail";
            }
            else if (Math.Abs(posTouch.x - tailLoc) <= Math.Abs(posTouch.x - headLoc))
            {
                side = "tail";
            }
        }
        if (side != "none")
        {
            SocketSend.sendDominoCard(cardID, side);
            dominoHead.gameObject.SetActive(false);
            dominoTail.gameObject.SetActive(false);
        }
    }

}
