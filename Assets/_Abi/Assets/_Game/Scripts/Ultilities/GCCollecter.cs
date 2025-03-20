using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCCollecter : MonoBehaviour
{
    private float timer;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (!(timer < Time.time)) return;
        // Debug.Log("GARBAGE COLLECTED");
        System.GC.Collect();
        timer = Time.time + 30f;
    }
}
