using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DuoIdle : MonoBehaviour
{
    [SpineAnimation] public string firstAnim;
    [SpineAnimation] public string secondAnim;

    public SkeletonGraphic anim;

    [SerializeField] private float offset;

    private void OnEnable()
    {
        anim.Clear();
        PlayAnims();
    }

    private void PlayAnims()
    {
        anim.AnimationState.AddAnimation(0, firstAnim, false, 0).TrackTime = offset;
        anim.AnimationState.AddAnimation(0, secondAnim, false, 0);
        anim.AnimationState.AddAnimation(0, secondAnim, false, 0);
        anim.AnimationState.AddAnimation(0, secondAnim, false, 0).Complete += (entry) => { PlayAnims(); };
    }

    private void OnDisable()
    {
        anim.AnimationState.ClearTracks();
    }
}