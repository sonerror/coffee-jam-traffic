using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTons<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T ins;
    [SerializeField] private bool isDontDesTroyOnLoad;
    protected bool isAwakable;
    [SerializeField] private bool isDestroyOldInstance;

    protected virtual void Awake()
    {
        if (ins == null)
        {
#if UNITY_EDITOR
            Debug.Log("SINGLETONS  " + gameObject.name);
#endif
            ins = this as T;

            if (isDontDesTroyOnLoad) DontDestroyOnLoad(this);
            isAwakable = true;
        }
        else if (isDestroyOldInstance)
        {
            DestroyImmediate(ins.gameObject);
            ins = this as T;
            isAwakable = true;
        }
        else
        {
            DestroyImmediate(gameObject);
            isAwakable = false;
        }
    }
}