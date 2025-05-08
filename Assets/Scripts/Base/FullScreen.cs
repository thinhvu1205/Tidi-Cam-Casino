using UnityEngine;

public class FullScreen : MonoBehaviour
{
    [SerializeField]
    bool isPortrait;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 sizeFullParent = transform.parent.GetComponent<RectTransform>().rect.size;
        Vector2 sizeFull;
        //if (isPortrait)
        //{
        //    Debug.Log("game Doc");
        //    sizeFull = new Vector2(sizeFullParent.x, sizeFullParent.y);
        //}
        //else
        //{
        //    Debug.Log("game Ngang");
        //    sizeFull = new Vector2(sizeFullParent.y, sizeFullParent.x);
        //}
        sizeFull = new Vector2(sizeFullParent.x, sizeFullParent.y);
        transform.GetComponent<RectTransform>().sizeDelta = sizeFull;
    }
}
