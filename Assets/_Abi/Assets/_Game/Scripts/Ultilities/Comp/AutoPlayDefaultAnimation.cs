using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayDefaultAnimation : MonoBehaviour
{
    public Animation anim;

    private void OnEnable()
    {
        anim.Play();
    }
}