using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HomePointLight : MonoBehaviour
{
    public Transform[] lightTrans;
    public string[] keys;

    private void Start()
    {
        SetLight();
    }

    [ContextMenu("SET LIGHT")]
    public void SetLight()
    {
        for (int i = 0; i < lightTrans.Length; i++)
        {
            Shader.SetGlobalVector(keys[i], lightTrans[i].position);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetLight();
        }
    }
#endif
}