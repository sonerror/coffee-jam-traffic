using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InAppBox : MonoBehaviour
{
    public new GameObject gameObject;
    [SerializeField] private float boxHeight;
    public float BoxHeight => boxHeight;

    public bool IsActive { get; private set; }

    public void Enable(bool isOn, bool isHideOnInit = true)
    {
        IsActive = isOn;

        gameObject.SetActive(isOn);
    }

#if UNITY_EDITOR
    public void SetHeight()
    {
        boxHeight = GetComponent<RectTransform>().sizeDelta.y;
    }
#endif
}