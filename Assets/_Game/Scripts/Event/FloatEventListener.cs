using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class FloatEvent : UnityEvent<float>
{
}

public class FloatEventListener : MonoBehaviour
{
    [SerializeField] private FloatEventChannelSO channel;

    public FloatEvent OnEventRaised;

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

    private void Respond(float value)
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value);
    }
}