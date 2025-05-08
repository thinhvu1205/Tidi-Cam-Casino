using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lucky9RateItem : MonoBehaviour
{
    public List<Sprite> listFace;

    public void setRate(int rate)
    {
        if (rate - 2 >= 0 && rate - 2 < listFace.Count)
        {
            transform.GetComponent<Image>().sprite = listFace[rate - 2];
        }
        else
        {
            Debug.LogError("Rate out of bounds of listFace array");
        }
    }
}
