using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Stuff/Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    public UnityAction<float> onEventRaised;

    public void RaiseEvent(float value)
    {
        onEventRaised?.Invoke(value);
    }
}