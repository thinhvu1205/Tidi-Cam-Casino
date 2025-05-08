using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinhLogicManager : MonoBehaviour
{
    public bool checkThung(List<Card> listIn, int size = 5)
    {
        List<Card> list = new List<Card>(listIn);

        if (list.Count < size)
        {
            return false;
        }
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            count = 0;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[j].S == list[i].S)
                {
                    count++;
                }
            }
            if (count >= size - 1)
            {
                return true;
            }
        }
        return false;
    }

    public bool checkSanh(List<Card> listIn, int size = 5)
    {
        if (listIn.Count < size)
        {
            return false;
        }

        List<int> c = new List<int>();

        foreach (var card in listIn)
        {
            c.Add(card.N);
            if (card.N == 14)
            {
                c.Add(1);
            }
        }

        c.Sort();

        for (int i = 0; i < c.Count - 1; i++)
        {
            int count = 0;

            for (int j = i + 1; j < c.Count; j++)
            {
                if ((c[i] + count + 1) == c[j])
                {
                    count++;
                }
            }
            if (count >= size - 1)
            {
                return true;
            }
        }
        return false;
    }

    public bool checkThungPhaSanh(List<Card> listIn, out List<bool> isThungAndSanh, int size = 5)
    {
        List<Card> list = new List<Card>(listIn);
        isThungAndSanh = new() { checkThung(list, size), checkSanh(list, size) };
        if (isThungAndSanh[0] && isThungAndSanh[1])
            return true;

        return false;
    }

    public bool checkSamCo(List<Card> listIn)
    {
        List<Card> list = new List<Card>(listIn);
        list.Sort((x, y) => x.N - y.N);

        if (list.Count < 3)
        {
            return false;
        }

        for (int i = 0; i < list.Count - 2; i++)
        {
            if (list[i].N == list[i + 1].N && list[i + 1].N == list[i + 2].N)
            {
                return true;
            }
        }

        return false;
    }

    public bool checkTuQuy(List<Card> listIn, bool isSortChi = false)
    {
        List<Card> list = new List<Card>(listIn);
        if (!isSortChi) list.Sort((x, y) => x.N - y.N);
        else list.Reverse();
        if (list.Count < 4)
            return false;

        for (int i = 0; i < list.Count - 1; i++)
        {
            int count = 0;

            for (int j = i + 1; j < list.Count; j++)
                if (list[j].N == list[i].N)
                    count++;

            if (count == 3)
                return true;
        }

        return false;
    }

    public bool checkCulu(List<Card> listIn, out bool isSamCo, bool isSortChi = false)
    {
        List<Card> list = new List<Card>(listIn);
        isSamCo = checkSamCo(list);
        if (!isSamCo || list.Count < 5)
            return false;

        if (!isSortChi) list.Sort((x, y) => x.N - y.N);
        else list.Reverse();
        if (list.Count == 5)
        {
            for (int i = 0; i < list.Count - 4; i++)
            {
                if (list[i].N == list[i + 1].N)
                {
                    if (list[i + 1].N == list[i + 2].N && list[i + 3].N == list[i + 4].N) // 3-2
                        return true;
                    if (list[i + 2].N == list[i + 3].N && list[i + 3].N == list[i + 4].N) // 2-3
                        return true;
                }
            }
            return false;
        }

        List<Card> tmp = new List<Card>();
        for (int i = 0; i < list.Count - 2; i++)
        {
            tmp = new List<Card>(list);
            if (list[i].N == list[i + 1].N && list[i + 1].N == list[i + 2].N)
            {
                tmp.RemoveRange(i, 3);
                if (checkDoi(tmp))
                    return true;
            }
        }

        return false;
    }

    public bool checkDoi(List<Card> listIn, bool isSortChi = false)
    {
        List<Card> list = new List<Card>(listIn);

        if (!isSortChi) list.Sort((x, y) => x.N - y.N);
        else list.Reverse();
        if (list.Count < 2)
            return false;

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].N == list[i + 1].N)
                return true;
        }

        return false;
    }

    public bool checkThu(List<Card> listIn, bool isSortChi = false)
    {
        List<Card> list = new List<Card>(listIn);
        if (list.Count < 4)
            return false;
        if (!isSortChi) list.Sort((x, y) => x.N - y.N);
        else list.Reverse();

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].N == list[i + 1].N)
            {
                for (int j = i + 2; j < list.Count - 1; j++)
                {
                    if (list[j].N == list[j + 1].N)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool checkBinhGrandDragon(List<Card> listIn)
    {
        for (int i = 1; i < listIn.Count; i++)
        {
            if (listIn[i].S != listIn[i - 1].S)
            {
                return false;
            }
        }

        if (!checkBinhDragon(listIn))
        {
            return false;
        }

        return true;
    }

    public bool checkBinhDragon(List<Card> listIn)
    {
        List<Card> list = new List<Card>(listIn);

        list.Sort((x, y) => x.N - y.N);

        for (int i = 1; i < list.Count; i++)
        {
            if (list[i].N != list[i - 1].N + 1)
            {
                return false;
            }
        }

        return true;
    }

    public bool checkBinhSameColor(List<Card> listIn)
    {
        int black = 0;
        int red = 0;

        foreach (var card in listIn)
        {
            if (card.S <= 2)
            {
                black++;
            }
            else
            {
                red++;
            }
        }

        return black == 13 || red == 13;
    }

    public bool checkBinhSixPairs(List<Card> listIn)
    {
        List<Card> list = new List<Card>(listIn);

        list.Sort((x, y) => x.N - y.N);

        int index = 0;
        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i].N == list[i + 1].N)
            {
                index++;
                i = i + 1;
            }
        }

        return index == 6;
    }

    public bool checkBinhThreeStraights(List<Card> list1, List<Card> list2, List<Card> list3)
    {
        if (!checkSanh(list1, 3) || !checkSanh(list2, 5) || !checkSanh(list3, 5))
        {
            return false;
        }

        return true;
    }

    public bool checkBinhThreeFlushes(List<Card> list1, List<Card> list2, List<Card> list3)
    {
        if (!checkThung(list1, 3) || !checkThung(list2, 5) || !checkThung(list3, 5))
        {
            return false;
        }

        return true;
    }

}
