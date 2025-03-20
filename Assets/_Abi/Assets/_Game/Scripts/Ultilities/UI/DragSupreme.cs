using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class DragSupreme : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform parentRect;
    public new GameObject gameObject;
    public bool IsDragable;
    public RectTransform rect;
    public bool isBackPos = true;
    public UnityEvent dragEvent;
    public UnityEvent onClickEvent;

    private Vector2 offset;
    private Vector2 rootPos;
    private Vector2 dragPos;
    private const float backMinDist = 420f;
    private const float backMaxDist = 720f;
    private Tween curBackPosTween;

    private void Start()
    {
        rootPos = rect.anchoredPosition;
        IsDragable = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsDragable) return;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out var tmp))
        {
            offset = rect.anchoredPosition - tmp;
        }

        onClickEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsDragable) return;
        if (isBackPos) BackPos();
    }

    private void BackPos()
    {
        curBackPosTween?.Kill();
        curBackPosTween = rect.DOAnchorPos(rootPos, Mathf.Clamp(Vector2.Distance(rect.anchoredPosition, rootPos), backMinDist, backMaxDist) / backMaxDist * .42f).SetEase(Ease.OutSine);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragable) return;
        dragEvent?.Invoke();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out dragPos))
        {
            rect.anchoredPosition = dragPos + offset;
        }
    }
}