using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used for Events that have no arguments (Example: Exit game event)
/// </summary>
[CreateAssetMenu(menuName = "Stuff/Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> onEventRaised;

    public void RaiseEvent(int value)
    {
        onEventRaised?.Invoke(value);
    }
}