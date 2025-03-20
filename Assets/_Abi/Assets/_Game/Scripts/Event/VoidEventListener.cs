using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class VoidEventListener : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO channel;

    public UnityEvent OnEventRaised;

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

    private void Respond()
    {
        if (OnEventRaised != null)
            OnEventRaised.Invoke();
    }
}