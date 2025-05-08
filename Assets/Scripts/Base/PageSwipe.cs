using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwipe : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.0f;
    public int totalPages = 1;
    private int currentPage = 1;
    private float pageSize = 630;

    // Start is called before the first frame update
    void Start()
    {
        panelLocation = transform.localPosition;
        pageSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
        Debug.Log("Page Size=" + pageSize);
    }
    public void OnDrag(PointerEventData data)
    {
        Globals.Logging.Log("data------:" + data.position);
        float difference = data.pressPosition.x - data.position.x;
        transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
        Globals.Logging.Log("localPosition------:" + transform.localPosition);
    }
    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / 630;
        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages)
            {
                currentPage++;
                newLocation += new Vector3(-630, 0, 0);
            }
            else if (percentage < 0 && currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(630, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.localPosition, panelLocation, easing));
        }
    }
    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }

    public void ScrollPageWithIndex(int index) {
        if (currentPage < 1 || currentPage > totalPages) return;
        var posx = -(currentPage - 1) * pageSize;
        StartCoroutine(SmoothMove(transform.localPosition, new Vector3(posx, transform.localPosition.y, 0), easing));
    }
    public void nextPage()
    {
        currentPage++;
        if (currentPage >= totalPages) currentPage = totalPages;
        ScrollPageWithIndex(currentPage);
    }
    public void previousPage()
    {
        currentPage--;
        if (currentPage <= 1) currentPage = 1; 
        ScrollPageWithIndex(currentPage);
    }
    //public void previousPage()
    //{

    //}
}