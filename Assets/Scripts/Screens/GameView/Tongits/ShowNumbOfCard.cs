using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ShowNumbOfCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI card_stack;
    [SerializeField] TextMeshProUGUI player_1;
    [SerializeField] TextMeshProUGUI player_2;

    int currentpl1, currentpl2, currentStack;

    private void OnEnable()
    {
        player_1.gameObject.SetActive(false);
        player_2.gameObject.SetActive(false);
        currentpl1 = 0;
        currentpl2 = 0;
        currentStack = 0;
    }

    public void setInfo(int stack, List<int> numbOfCardPlayer)
    {
        //if (GameManager.getInstance().curGameId == GAME_ID.TONGITS11)
        //{
        //    player_1.node.position = new Vector2(-50, 220);
        //}

        card_stack.text = stack.ToString();
        card_stack.gameObject.SetActive(stack != 0);
        currentStack = stack;

        for (int i = 0; i < numbOfCardPlayer.Count; i++)
        {
            if (i == 0)
            {
                player_1.text = numbOfCardPlayer[i].ToString();
                player_1.gameObject.SetActive(true);
                currentpl1 = numbOfCardPlayer[i];
            }
            else
            {
                player_2.text = numbOfCardPlayer[i].ToString();
                player_2.gameObject.SetActive(true);
                currentpl2 = numbOfCardPlayer[i];
            }
        }
    }

    public void updateCardStack(int numb, int type)
    {
        if (type == 0)
        {
            currentStack -= numb;
        }
        else
        {
            currentStack += numb;
        }

        card_stack.text = currentStack.ToString();
        card_stack.gameObject.SetActive(currentStack != 0);
    }

    public void updatePlayer(int player, int numb, int type)
    {
        if (type == 0)
        {
            if (player == 1)
            {
                currentpl1 -= numb;
                player_1.text = currentpl1.ToString();
            }
            else
            {
                currentpl2 -= numb;
                player_2.text = currentpl2.ToString();
            }
        }
        else
        {
            if (player == 1)
            {
                currentpl1 += numb;
                player_1.text = currentpl1.ToString();
            }
            else
            {
                currentpl2 += numb;
                player_2.text = currentpl2.ToString();
            }
        }

        player_1.gameObject.SetActive(currentpl1 != 0);
        player_2.gameObject.SetActive(currentpl2 != 0);
    }
}
