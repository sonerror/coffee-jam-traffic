using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class HoldButtonComp : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool IsInteractable = true;
    public UnityEvent unityEvent;
    [SerializeField] private float holdTime = .92f;
    private float timer;
    [SerializeField] private float mulRect = 1.12f;
    public Image filler;
    public RectTransform buttonRect;
    private Tween curZoomTween;

    private bool isHold;
    private bool isUpdate = true;

    private void Update()
    {
        if (!IsInteractable) return;
        if (!isUpdate) return;
        if (isHold)
        {
            timer = Mathf.Clamp(timer + Time.unscaledDeltaTime, 0, holdTime);
            filler.fillAmount = timer / holdTime;
        }
        else
        {
            timer = Mathf.Clamp(timer - Time.unscaledDeltaTime, 0, holdTime);
            filler.fillAmount = timer / holdTime;
        }

        if (timer > holdTime - .01f)
        {
            isUpdate = false;
            Timer.ScheduleSupreme(.18f, () => unityEvent.Invoke());
        }
    }

    private void OnDestroy()
    {
        unityEvent.RemoveAllListeners();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        curZoomTween?.Complete();
        curZoomTween = buttonRect.DOScale(buttonRect.localScale * mulRect, .18f).SetEase(Ease.OutSine).SetUpdate(true);

        isHold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable) return;
        curZoomTween?.Complete();
        curZoomTween = buttonRect.DOScale(buttonRect.localScale / mulRect, .18f).SetEase(Ease.OutSine).SetUpdate(true);

        isHold = false;
    }
}
