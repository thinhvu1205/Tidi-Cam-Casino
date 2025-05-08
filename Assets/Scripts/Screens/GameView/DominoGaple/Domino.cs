using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Domino : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public List<Sprite> listSpriteDomino = new List<Sprite>();
    [SerializeField] public List<Sprite> listSpriteBgDomino = new List<Sprite>();
    [SerializeField] public int cardID;
    System.Action<PointerEventData, Domino> OnBeginDragCallback, OnDragCallback, OnEndDragCallback;
    public bool isSamePoint()
    {
        int top = cardID / 7;
        int bottom = cardID % 7;
        return (top == bottom);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragCallback != null)
        {
            OnBeginDragCallback.Invoke(eventData, this);
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (OnDragCallback != null)
        {
            OnDragCallback.Invoke(eventData, this);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragCallback != null)
        {
            OnEndDragCallback.Invoke(eventData, this);
        }
    }
    public void setListenerDragDrop(System.Action<PointerEventData, Domino> _OnBeginDrag, System.Action<PointerEventData, Domino> _OnDrag, System.Action<PointerEventData, Domino> _OnEndDrag)
    {
        OnBeginDragCallback = _OnBeginDrag;
        OnDragCallback = _OnDrag;
        OnEndDragCallback = _OnEndDrag;
    }
}
