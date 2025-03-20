using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    public List<TKey> KeyList = new List<TKey>();
    public List<TValue> ValueList = new List<TValue>();

    public void OnBeforeSerialize()
    {
        KeyList.Clear();
        ValueList.Clear();
        foreach (var (key, value) in this)
        {
            KeyList.Add(key);
            ValueList.Add(value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (KeyList.Count != ValueList.Count)
        {
            // Debug.LogError("Dictionary data serialize error");
        }

        for (var i = 0; i < KeyList.Count; i++)
        {
            this.Add(KeyList[i], ValueList[i]);
        }
    }
}