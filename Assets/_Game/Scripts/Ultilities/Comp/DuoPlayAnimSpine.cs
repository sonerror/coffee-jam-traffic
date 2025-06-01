using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DuoPlayAnimSpine : MonoBehaviour
{
    [SpineAnimation] public string firstAnim;
    [SpineAnimation] public string secondAnim;

    public SkeletonGraphic anim;

    [SerializeField] private float delay;

    private void OnEnable()
    {
        anim.Clear();
        anim.AnimationState.ClearTracks();
        anim.AnimationState.SetAnimation(0, firstAnim, true).TrackTime = delay;
        anim.AnimationState.SetAnimation(1, secondAnim, true).TrackTime = delay;
    }

    private void OnDisable()
    {
        anim.AnimationState.ClearTracks();
    }
}