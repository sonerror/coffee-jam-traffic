using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DuoAnimContinuos : MonoBehaviour
{
    [SpineAnimation] public string firstAnim;
    [SpineAnimation] public string secondAnim;

    public SkeletonGraphic anim;

    private void OnEnable()
    {
        anim.Clear();
        PlayAnims();
    }

    private void PlayAnims()
    {
        // Debug.Log("START ANIM", gameObject);
        anim.AnimationState.AddAnimation(0, firstAnim, false, 0);
        anim.AnimationState.AddAnimation(0, secondAnim, false, 0).Complete += (entry) => { PlayAnims(); };
    }

    private void OnDisable()
    {
        anim.AnimationState.ClearTracks();
    }
}