using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TriggerUltimate : MonoBehaviour
{
    [SerializeField] private bool isMultiTrigger;

    public UnityEvent onTrigger;
    private bool isTrigger;
    [TagSelector, SerializeField] private string colTag;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTrigger && other.CompareTag(colTag))
        {
            isTrigger = !isMultiTrigger;
            onTrigger?.Invoke();
        }
    }
}