using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrayScaleImage : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image img;
    [SerializeField] Material grayMat;
    public void setGray(bool state)
    {
        img.material= state? grayMat : null;
    }
}
