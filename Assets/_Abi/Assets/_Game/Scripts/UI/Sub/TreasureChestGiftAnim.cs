using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class TreasureChestGiftAnim : MonoBehaviour
{
    public new GameObject gameObject;

    public SkeletonGraphic anim;

    [SerializeField, SpineAnimation] private string idle1;
    [SerializeField, SpineAnimation] private string idle2;
    [SerializeField, SpineAnimation] private string open;

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    public void PlayAnim(bool isIdle, bool isSpace = false)
    {
        if (isIdle)
        {
            if(isSpace == false)
            {
                Idle();
            }
            else
            {
                IdleSpace();
            }
        }
        else
        {
            // anim.AnimationState.ClearTrack(0);
            anim.AnimationState.SetAnimation(0, open, false).Complete += entry => gameObject.SetActive(false);
        }

        return;

        void Idle()
        {
            anim.AnimationState.SetAnimation(0, idle1, false);
            anim.AnimationState.AddAnimation(0, idle1, false, 0);
            anim.AnimationState.AddAnimation(0, idle1, false, 0);
            anim.AnimationState.AddAnimation(0, idle1, false, 0);
            anim.AnimationState.AddAnimation(0, idle2, false, 0)
                .Complete += (entry) => Idle();
        }
        void IdleSpace()
        {
            anim.AnimationState.SetAnimation(0, idle1, false);
            anim.AnimationState.AddAnimation(0, idle2, true, 0);
        }
    }
}