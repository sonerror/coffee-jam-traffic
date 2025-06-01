using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TapButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent tapEvent;
    // public static PointerEventData CurPointerEventData;

    public void OnPointerDown(PointerEventData eventData)
    {
        // CurPointerEventData = eventData;
        tapEvent?.Invoke();
    }
}