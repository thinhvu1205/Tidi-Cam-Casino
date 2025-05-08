using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortCard : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] List<GameObject> cards = new List<GameObject>();
    void Start()
    {
        sortCardPlayer();
    }
    public void sortCardPlayer()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            //if (cards.Count % 2 != 0)
            //{
            //    cards[i].transform.localPosition = new Vector2((i - cards.Count) * 73.5f, cards[i].transform.localPosition.y);
            //}
            //else
            //{
            //    cards[i].transform.localPosition = new Vector2(i - (1+0.5f) * 73.5f, cards[i].transform.localPosition.y);
            //}
            //(i -(int)(cards.Count * .5) + (cards.Count % 2 == 0 ? 0.5 : 0))
            cards[i].transform.localPosition = new Vector2((i - ((int)(cards.Count / 2) + (cards.Count % 2 == 0 ? -0.5f : 0))) * 25f, cards[i].transform.localPosition.y);
            //Debug.Log("DKMMMM-------x===" + cards[i].transform.localPosition.x);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
