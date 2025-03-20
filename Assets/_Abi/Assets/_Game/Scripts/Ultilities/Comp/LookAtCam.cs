using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    public new Transform transform;

    [SerializeField] private Vector3 targetEuler = new Vector3(0, 180, 0);

    private void Update()
    {
        transform.eulerAngles = targetEuler;
    }
}