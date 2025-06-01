using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    public class Helicoptar : MonoBehaviour
    {
        public static Helicoptar cur;

        public new Transform transform;
        public new GameObject gameObject;
        [SerializeField]private Animator _animator;
        [SerializeField] private float outSideDistance = 5f;
        [SerializeField] private float flyHeight;
        [SerializeField] private float moveSpeed;
        private Tween moveTween;

        private Bus currentBus;
        private float t;
        private Coroutine _coroutine;
        private void Awake()
        {
            cur = this;
        }

        public void Reposition()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            t = Time.time;
            transform.DOKill();
            moveTween?.Kill();
            this.StopAllCoroutines();
            if (currentBus != null) currentBus.transform.SetParent(null, true);
            currentBus = null;
            gameObject.SetActive(false);
           
        }

        private IEnumerator IePickup(Action action)
        {
            yield return Yielders.Get(0.32f);
            action?.Invoke();
        }

        public void PickupBus(Bus bus, Action onDone = null)
        {
            gameObject.SetActive(true);

            if (bus.IsGrayBus) bus.TurnBackNormalFromGray();

            var targetPos = bus.transform.position;
            var busForward = bus.transform.forward;
            var aboveBusPoint = new Vector3(targetPos.x, flyHeight, targetPos.z);

            var startPoint = aboveBusPoint + busForward * -outSideDistance;

            var height = bus.Type switch
            {
                BusType.Car => Config.cur.carHeight,
                BusType.Van => Config.cur.vanHeight,
                BusType.Bus => Config.cur.busHeight,
            };

            var justAboveBusPoint = new Vector3(targetPos.x, height, targetPos.z);
            var lookPoint = justAboveBusPoint + busForward * 2.45f;
            var anchor = aboveBusPoint + busForward * -.72f + Vector3.up * .12f;

            transform.position = startPoint;
            var path = new Vector3[] { justAboveBusPoint, anchor, anchor };
            moveTween = transform.DOPath(path, moveSpeed, PathType.CubicBezier).SetSpeedBased(true).SetLookAt(lookPoint).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    currentBus = bus;
                    _animator.SetTrigger("grab");
                    bus.transform.SetParent(transform, true);
                    /*
                    bus.smokeEffectObject.SetActive(true);
                    */
                    var yOffset = transform.position.y - bus.transform.position.y;

                    _coroutine=Timer.Schedule(this,0.32f, () =>
                    {
                        if(Time.time-t<1f) return;
                        //handle remove bus
                        bus.IsStayOnBusZone = false;
                        bus.OnMoveOutOfParkZone();
                        if (bus.IsSlideBus) BusSlide.cur.IsBusDone = true;
                    });

                    var parkSlot = ParkingLot.cur.GetVipSlot();
                    var parkPoint = parkSlot.transform.position;
                    var parkRotate = parkSlot.transform.eulerAngles;
                    var dir = parkPoint - targetPos;
                    var justAboveParkPoint = new Vector3(parkPoint.x, parkSlot.transform.position.y+yOffset, parkPoint.z);

                    var firstAnchor = justAboveBusPoint + dir * .24f + Vector3.up * flyHeight;
                    var secondAnchor = justAboveParkPoint - dir * .42f + Vector3.up * flyHeight;

                    var parkPath = new Vector3[] { justAboveParkPoint, firstAnchor, secondAnchor };
                    moveTween = transform.DOPath(parkPath, moveSpeed, PathType.CubicBezier).SetSpeedBased(true).SetEase(Ease.InOutSine)
                        .OnStart(() =>
                        {
                            var time = moveTween.Duration();
                            transform.DORotate(parkRotate, time).SetEase(Ease.OutSine);
                        })
                        .OnComplete(() =>
                        {
                            onDone?.Invoke();
                            currentBus.transform.SetParent(null, true);
                            currentBus = null;
                            _animator.SetTrigger("drop");
                            moveTween = transform.DOMove(justAboveParkPoint + Vector3.up * 4.52f + transform.forward * 5.92f, moveSpeed).SetSpeedBased(true).SetEase(Ease.InSine)
                                .OnComplete(Reposition).SetDelay(0.2f);
                        }).SetDelay(0.4f);
                });
        }
    }
}