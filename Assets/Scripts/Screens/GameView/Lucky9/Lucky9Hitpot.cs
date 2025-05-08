using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lucky9Hitpot : MonoBehaviour
{
    public Avatar avatar;
    public TextMeshProUGUI chip;
    public Image progress;
    public GameObject iconChip;
    public List<GameObject> ArrP;
    private int _PotChip;

    private float[] fillProg = { 0f, 0.25f, 0.5f, 0.75f, 1f };
    private Coroutine showPlayerCoroutine;

    public Vector3 getWorldPositionIconChip()
    {
        return iconChip.transform.parent.TransformPoint(iconChip.transform.localPosition);
    }

    public void updateMoney(int chipPot)
    {
        Globals.Config.EffRunNumber(chip, _PotChip, chipPot, 1);
        _PotChip = chipPot;
    }

    public void setInfo(List<Dictionary<string, object>> players)
    {
        int pot = 0;
        foreach (var player in players)
        {
            pot = (int)player["Pot"];
        }
        if (pot > 4) pot = 4;
        progress.fillAmount = fillProg[pot];

        if (showPlayerCoroutine != null)
        {
            StopCoroutine(showPlayerCoroutine);
        }

        if (players.Count == 0) return;

        showPlayerCoroutine = StartCoroutine(ShowPlayerRoutine(players));
    }

    private IEnumerator ShowPlayerRoutine(List<Dictionary<string, object>> players)
    {
        int currentIndex = -1;
        while (true)
        {
            if (transform == null || gameObject == null) break;
            currentIndex++;
            if (currentIndex >= players.Count) currentIndex = 0;
            Dictionary<string, object> currentPlayer = players[currentIndex];
            avatar.loadAvatar((int)currentPlayer["AvatarId"], (string)currentPlayer["PlayerName"], (string)currentPlayer["Fid"]);
            yield return new WaitForSeconds(2f);
        }
    }
}
