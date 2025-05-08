using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BinhChosenGroupCards : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector] public List<Card> FakeChosenCs = new();
    [HideInInspector] public List<int> ChosenIds = new(), TargetIds = new();
    private Action<PointerEventData> _OnPointerUpCb, _OnBeginDragCb, _OnDragCb, _OnEndDragCb, _OnPointerDownCb;
    private bool _IsDragging;

    public void SetCallBacks(
        Action<PointerEventData> onPointerUpCb,
        Action<PointerEventData> onBeginDragCb,
        Action<PointerEventData> onDragCb,
        Action<PointerEventData> onEndDragCb,
        Action<PointerEventData> onPointerDownCb
    )
    {
        _OnPointerUpCb = onPointerUpCb;
        _OnBeginDragCb = onBeginDragCb;
        _OnDragCb = onDragCb;
        _OnEndDragCb = onEndDragCb;
        _OnPointerDownCb = onPointerDownCb;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_IsDragging) _OnPointerUpCb?.Invoke(eventData);
        _IsDragging = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        _OnPointerDownCb?.Invoke(eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _IsDragging = true;
        _OnBeginDragCb?.Invoke(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        _OnDragCb?.Invoke(eventData);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _OnEndDragCb?.Invoke(eventData);
    }
}
