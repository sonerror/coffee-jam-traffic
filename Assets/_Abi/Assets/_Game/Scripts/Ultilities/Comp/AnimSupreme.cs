using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSupreme : MonoBehaviour
{
    public Animator anim;
    private int curAnim;
    private bool isAnimChangable = true;
    //anim
    // public static int IDLE = Animator.StringToHash("Idle");

    public void SetChangable(bool isChangable)
    {
        isAnimChangable = isChangable;
    }

    public void ChangeAnim(int targetAnim)
    {
        if (curAnim != targetAnim && isAnimChangable)
        {
            anim.ResetTrigger(curAnim);
            curAnim = targetAnim;
            anim.SetTrigger(curAnim);
        }
    }

    public void ForceChangeAnim(int targetAnim)
    {
        anim.ResetTrigger(curAnim);
        curAnim = targetAnim;
        anim.SetTrigger(curAnim);
    }

}
