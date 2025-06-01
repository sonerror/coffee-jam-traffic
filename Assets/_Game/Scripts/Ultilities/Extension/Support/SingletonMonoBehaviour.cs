using System;
using UnityEngine;
public abstract class Singlemono : MonoBehaviour
{
    public abstract void OnCreateIns();
}
public class SingletonMonoBehaviour<T> : Singlemono where T : Singlemono
{
    public static T ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindExistingInstance() ?? CreateNewInstance();
                _instance.OnCreateIns();
            }
            return _instance;
        }
    }

    #region Singleton Implementation

    /// <summary>
    ///     Holds the unique instance for this class
    /// </summary>
    private static T _instance;

    /// <summary>
    ///     Finds an existing instance of this singleton in the scene.
    /// </summary>
    private static T FindExistingInstance()
    {
        T[] existingInstances = FindObjectsOfType<T>();

        // No instance found
        if (existingInstances == null || existingInstances.Length == 0) return null;

        return existingInstances[0];
    }

    /// <summary>
    ///     If no instance of the T MonoBehaviour exists, creates a new GameObject in the scene
    ///     and adds T to it.
    /// </summary>
    private static T CreateNewInstance()
    {
        var containerGO = new GameObject("__" + typeof(T).Name + " (Singleton)");
        return containerGO.AddComponent<T>();
    }

    #endregion
    public override void OnCreateIns()
    {

    }
}

#region old
// using UnityEngine;

// public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
// {
//     private static T inst;
//     public static T ins;
//     //{
//     //    get
//     //    {
//     //        if (inst == null)
//     //        {
//     //            inst = GameObject.FindObjectOfType<T>();

//     //            if (inst == null)
//     //            {
//     //                inst = new GameObject().AddComponent<T>();
//     //            }
//     //        }

//     //        return inst;
//     //    }
//     //}

//     public bool isDontDestroy;

//     public virtual void Awake()
//     {
//         if (ins == null)
//         {
//             ins = this as T;
//             if (isDontDestroy)
//             {
//                 DontDestroyOnLoad(this);
//             }
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
        
//     }



//     public static bool Exists()
//     {
//         return (ins != null);
//     }
// }
#endregion