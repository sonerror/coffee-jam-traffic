using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraCon : SingleTons<CameraCon>
{
    public Camera cam;

    public new Transform transform;
    public new GameObject gameObject;

    private float cur;

    private Tween camMoveTween;
    private Tween camRotTween;

    [SerializeField] private float rootOrthoSize = 10.5f;
    [SerializeField] private AnimationCurve _curveAdjustOthorSize;
    private void Start()
    {
        // rootOrthoSize = cam.orthographicSize;
        // offset = transform.position - PlayerCon.ins.transform.position;
        CamOffsetToRatioAdaption();
        AdjustCamOrthoSizeToRatioAdaption();
        // transform.position = PlayerCon.ins.transform.position + offset;
        // transform.eulerAngles = eulerAngle;

        // rootOrthoSize = cam.orthographicSize;
    }

    public void SwitchPerspective()
    {
        cam.orthographic = false;
        cam.farClipPlane = 30;
        cam.fieldOfView = 60;
    }

    public void SwitchOrthographic()
    {
        cam.orthographic = false;
        cam.farClipPlane = 150;
        cam.fieldOfView = 15;
        float tarRatio = (float)Screen.height / Screen.width;
        cam.fieldOfView=_curveAdjustOthorSize.Evaluate(tarRatio);
    }

    public void MoveCam(Transform target, float duration = 1f, Ease moveEase = Ease.Linear, Ease rotEase = Ease.Linear)
    {
        camMoveTween?.Kill();
        camRotTween?.Kill();
        camMoveTween = transform.DOMove(target.position, duration).SetEase(moveEase);
        camRotTween = transform.DORotate(target.eulerAngles, duration, RotateMode.Fast).SetEase(rotEase);
    }

    public void MoveCam(Vector3 target, Vector3 eulerAngles, float duration = 1f, Ease moveEase = Ease.Linear,
        Ease rotEase = Ease.Linear)
    {
        camMoveTween?.Kill();
        camRotTween?.Kill();
        camMoveTween = transform.DOMove(target, duration).SetEase(moveEase);
        camRotTween = transform.DORotate(eulerAngles, duration, RotateMode.Fast).SetEase(rotEase);
    }

    public void MoveCamLocal(Transform target, float duration = 1f, Ease moveEase = Ease.Linear, Ease rotEase = Ease.Linear)
    {
        camMoveTween?.Kill();
        camRotTween?.Kill();
        camMoveTween = transform.DOLocalMove(target.localPosition, duration).SetEase(moveEase);
        camRotTween = transform.DOLocalRotate(target.localEulerAngles, duration, RotateMode.Fast).SetEase(rotEase);
    }

    public void MoveCamFollow(Transform target, Transform restPoint, float duration = 1f, Ease moveEase = Ease.Linear,
        Ease rotEase = Ease.Linear)
    {
        var curRot = transform.rotation;
        float timer = 0;
        var pos = restPoint.position;
        var rot = restPoint.rotation;
        camMoveTween?.Kill();
        camMoveTween = transform.DOMove(pos, duration).SetEase(moveEase)
            .OnUpdate(() =>
            {
                curRot = Quaternion.Lerp(curRot, Quaternion.LookRotation(target.position - transform.position),
                    Time.deltaTime * 16.32f);
                var per = timer / duration;
                per = per * per * per * per;
                transform.rotation = Quaternion.Lerp(curRot, rot, per);
                timer += Time.deltaTime;
            });
    }

    private Tween curShakeTween;
    private Tween curReturnTween;
    private int curMaxStrength = 0;

    public void ShakeCam(int strength = 1)
    {
        AudioManager.ins.MakeVibrate(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        if (curMaxStrength <= strength) curShakeTween?.Kill();
        else return;
        curMaxStrength = strength;
        curReturnTween?.Kill();
        curShakeTween = transform.DOShakeRotation(.08f * strength, strength * .06f,
                Mathf.RoundToInt(10f * strength / 24 + 8.75f), 90)
            .OnComplete(() =>
            {
                curReturnTween = transform.DOLocalMove(Vector3Pot.zero, .24f);
                curMaxStrength = 0;
            });
    }

    public void ContinuesShakeCam(int strength = 1)
    {
        curShakeTween?.Kill();
        curMaxStrength = strength;
        curReturnTween?.Kill();
        curShakeTween = transform.DOShakeRotation(.042f * strength, strength * .062f,
                Mathf.RoundToInt(10f * strength / 24 + 28.75f), 90)
            .SetLoops(-1)
            .OnKill(() => { curMaxStrength = 0; });
    }

    public void SetRestPoint(Transform restPoint)
    {
        transform.SetPositionAndRotation(restPoint.position, restPoint.rotation);
    }

    public void CamOffsetToRatioAdaption()
    {
        float tarRatio = (float)Screen.height / Screen.width;
        float rootRatio = 1920f / 1080;
        if (tarRatio > rootRatio)
        {
            // offset *= tarRatio / rootRatio;
        }
    }

    public void AdjustCamOrthoSizeToRatioAdaption()
    {
        float tarRatio = (float)Screen.height / Screen.width;
        /*
        float rootRatio = 1920f / 1080;
        if (tarRatio > rootRatio)
        {
            rootOrthoSize *= tarRatio / rootRatio;
        }
        */

        cam.orthographicSize = _curveAdjustOthorSize.Evaluate(tarRatio);
    }
}