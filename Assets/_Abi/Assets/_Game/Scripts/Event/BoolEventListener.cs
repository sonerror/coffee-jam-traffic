using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class BoolEvent : UnityEvent<bool>
{
}

public class BoolEventListener : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO channel;

    public BoolEvent OnEventRaised;

    private void OnEnable()
    {
        if (channel != null)
            channel.onEventRaised += Respond;
    }

    private void OnDisable()
    {
        if (channel != null)
            channel.onEventRaised -= Respond;
    }

    private void Respond(bool value)
    {
        OnEventRaised?.Invoke(value);
    }
}