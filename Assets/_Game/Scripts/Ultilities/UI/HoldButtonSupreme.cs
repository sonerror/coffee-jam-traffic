using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class HoldButtonSupreme : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool IsInteractable = true;
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public UnityEvent onHold;
    private bool isHold;

    private void Update()
    {
        if (!IsInteractable) return;
        if (isHold)
        {
            onHold?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        onPointerDown?.Invoke();
        isHold = true;

        Debug.Log("HOLD BUTTON DOWN");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        onPointerUp?.Invoke();
        isHold = false;

        Debug.Log("HOLD BUTTON UP");
    }
}