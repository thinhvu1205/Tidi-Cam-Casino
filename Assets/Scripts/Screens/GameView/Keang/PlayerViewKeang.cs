using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerViewKeang : PlayerView
{
    // Start is called before the first frame update
    [HideInInspector]
    public List<ChipBet> listChip = new List<ChipBet>();
    public List<Card> cardOnTables = new List<Card>();
    [SerializeField]
    public TextMeshProUGUI lbScore;
    [SerializeField]
    public List<Image> listPotCount = new List<Image>();
    [SerializeField]
    public List<Sprite> sprPot = new List<Sprite>();
    [SerializeField]
    public GameObject nodeCard;
    public int potCount = 0;
    void Start()
    {

    }
    private void OnEnable()
    {
        setPotCount(0);
    }
    // Update is called once per frame
    public void showScore(bool state, int score = 0, bool isThisPlayer = false)
    {
        lbScore.text = score.ToString();
        lbScore.transform.parent.gameObject.SetActive(state);
        lbScore.transform.parent.localPosition = isThisPlayer ? new Vector2(616, -80) : new Vector2(37, 41.2f);
        lbScore.transform.parent.localScale = isThisPlayer ? new Vector2(0.8f, 0.8f) : new Vector2(0.5f, 0.5f);
    }
    public void setPotCount(int count)
    {
        potCount = count;
        int index = 0;
        foreach (Image dot in listPotCount)
        {
            index++;
            if (index <= count)
            {
                dot.sprite = sprPot[1];
            }
            else
            {
                dot.sprite = sprPot[0];
            }
        }
    }
    void Update()
    {

    }
}
