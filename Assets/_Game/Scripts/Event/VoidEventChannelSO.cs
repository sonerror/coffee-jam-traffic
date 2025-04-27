using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used for Events that have no arguments (Example: Exit game event)
/// </summary>
[CreateAssetMenu(menuName = "Stuff/Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    public static Dictionary<VoidEventType, VoidEventChannelSO> dictionary = new Dictionary<VoidEventType, VoidEventChannelSO>();

    public UnityAction onEventRaised;

    [SerializeField] private VoidEventType eventType;

#if UNITY_EDITOR
    private void OnEnable()
    {
        AddToDict();
    }
#else
    private void Awake()
    {
        AddToDict();
    }
#endif

    private void AddToDict()
    {
        if (dictionary.ContainsKey(eventType)) return;
        // Debug.Log("ADDD TO EVENT " + eventType);
        dictionary.Add(eventType, this);
    }

    public void RaiseEvent()
    {
        onEventRaised?.Invoke();
    }

    public static void TriggerEvent(VoidEventType eventType)
    {
        // Debug.Log("EVENT CALL");
        if (dictionary.ContainsKey(eventType))
        {
            dictionary[eventType].RaiseEvent();
            // Debug.Log("EVENT CALL DONE " + dictionary[eventType].name);
        }
    }
}

public enum VoidEventType
{
    None,
    OnBusMoveOutParkZone,
    OnRevive,
}