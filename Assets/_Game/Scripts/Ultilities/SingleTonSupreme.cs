using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonsMono<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T ins;
    [SerializeField] private bool isDontDesTroyOnLoad;

    protected virtual void Awake()
    {
        if (ins == null)
        {
            ins = this as T;

            if (isDontDesTroyOnLoad) DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        #if UNITY_EDITOR
        Debug.Log("SINGLETONS  " + gameObject.name);
        #endif
    }
}

public class SingleTonNonmono<T> where T : new()
{
    public static T ins;

    public static T Instance
    {
        get
        {
            if (ins == null)
            {
                ins = new T();
            }

            return ins;
        }
    }
}
