using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [Serializable]
// public class IntEvent : UnityEvent<int>
// {
// }

public class IntEventListener : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO channel;

    public UnityEvent<int> OnEventRaised;

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

    private void Respond(int value)
    {
        // Debug.Log("INT EVENT " + value);
        if (OnEventRaised != null)
            OnEventRaised.Invoke(value);
    }
}