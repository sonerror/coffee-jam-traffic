using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopChatBox : MonoBehaviour
{
    public GameObject popObject;
    public RectTransform popRect;

    private Tween popTween;

    private void OnEnable()
    {
        popTween?.Kill();
        popObject.SetActive(false);
    }


    public void OnClickChest()
    {
        PopPop();
    }

    private void PopPop()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);

        popTween?.Kill();
        popObject.SetActive(true);
        popRect.localScale = Vector3.zero;
        popTween = popRect.DOScale(Vector3.one, .24f).SetEase(Ease.OutSine);

        Timer.ScheduleCondition(() => Input.GetMouseButtonDown(0), () => // no need to check if is clicked agian due to PopPop execute in button event --> trigger on mouse UP , after this mouse down
        {
            popTween.Kill();
            popTween = popRect.DOScale(Vector3.zero, .24f).SetEase(Ease.OutSine).OnComplete(() => popObject.SetActive(false));
        });
    }
}