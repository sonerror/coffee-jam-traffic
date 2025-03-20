using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    [SelectionBase]
    public class Minion : MonoBehaviour
    {
        public new GameObject gameObject;
        public new Transform transform;

        public Vector3 RootScale { get; private set; }
        [SerializeField] private Vector3 onBusScale = Vector3.one;
        public Vector3 OnBusScale => onBusScale;
        [SerializeField] private float initScaleMultipler = 1.072f;

        public JunkColor Color { get; set; }
        public bool IsPushed { get; set; }
        public bool IsInActiveList { get; set; }
        public bool IsOnBoard { get; set; }
        public int LineFlag { get; set; } //use only in multi line

        public Renderer rend2;
        public int QueueIndex { get; private set; }
        private Tween moveQueueTween;

        public AnimMeshPlayer anim;
        public static int ANIM_IDLE = Animator.StringToHash("IDLE");
        public static int ANIM_RUN = Animator.StringToHash("RUN");
        public static int ANIM_SEAT = Animator.StringToHash("SEAT");

        public static Quaternion onWaitLineQuaternion = Quaternion.Euler(12, 0, 0);

        private Vector3 onBoardLastPos;
        private Vector3 onBoardCurPos;

        private int currentAnim;

        public Tween CurrentMoveToBusTween { get; set; }

        public int ambulanceTriggerIndex = 18;
        public int fireTruckTriggerIndex = 18;

        public GameObject shadowObject;
        public Transform shadowTrans;
        private Coroutine _setAnimCour;
        private void Awake()
        {
            RootScale = transform.localScale * initScaleMultipler;
        }

        public void SetRender(Material mat)
        {
            var mats = rend2.sharedMaterials;
            mats[0] = mat;
            mats[2] = mat;
            rend2.sharedMaterials = mats;
        
        }
        public void OnInit()
        {
            SetAnim(ANIM_IDLE, true);
            IsOnBoard = false;
            transform.SetParent(null);
            transform.localScale = RootScale;
            rend2.gameObject.SetActive(true);
        }


        private void SetAnim(string paramName)
        {
            if (_setAnimCour != null)
            {
                this.StopCoroutine(_setAnimCour);
            }
            _setAnimCour = this.StartCoroutine(IeAutoDisableAnimator());
        }

        private IEnumerator IeAutoDisableAnimator()
        {
            yield return Yielders.Get(1f);
        }
        
        public void EnterQueue(int index)
        {
            QueueIndex = index;
        }

        public void MoveNextInQueue()
        {
            QueueIndex--;
        }

        public void AddQueueIndex(int num)
        {
            QueueIndex += num;
        }

        public void MoveQueueSegment(float per)
        {
            var (curPos, prevPos) = ParkingLot.cur.GetStandPointPair(QueueIndex, LineFlag);

            var pos = Vector3.Lerp(prevPos, curPos, per);
            var rot = Vector3.Distance(curPos, prevPos) > float.Epsilon ? onWaitLineQuaternion * Quaternion.LookRotation(curPos - prevPos) : transform.rotation;

            transform.SetPositionAndRotation(pos, rot);

            var isMoving = false;

            if (ParkingLot.cur.isSingleLine)
            {
                isMoving = ParkingLot.cur.IsMinionQueueMoving;
            }
            else
            {
                isMoving = LineFlag switch
                {
                    0 => ParkingLot.cur.IsFirstMinionQueueMoving,
                    1 => ParkingLot.cur.IsSecondMinionQueueMoving,
                    2 => ParkingLot.cur.IsThirdMinionQueueMoving,
                };
            }

            SetAnim(isMoving ? ANIM_RUN : ANIM_IDLE);

            shadowTrans.rotation = Quaternion.identity;
        }

        public void InitOnboardRotation()
        {
            onBoardLastPos = onBoardCurPos = transform.position;
        }

        public void OnboardRotate()
        {
            onBoardLastPos = onBoardCurPos;
            onBoardCurPos = transform.position;

            var rot = Vector3.Distance(onBoardCurPos, onBoardLastPos) > float.Epsilon ? onWaitLineQuaternion * Quaternion.LookRotation(onBoardCurPos - onBoardLastPos) : transform.rotation;
            transform.rotation = rot;
        }

        public void SetAnim(int targetAnim, bool isForceAnim = false)
        {
            if (targetAnim != currentAnim || isForceAnim)
            {
                currentAnim = targetAnim;
                anim.PlayAnim(currentAnim);
                shadowObject.SetActive(targetAnim != ANIM_SEAT);
                if (targetAnim == ANIM_SEAT)
                {
                    SetAnim("complete");
                }
            }
        }

        public void PopOnColorChange()
        {
            transform.DOPunchScale(RootScale * .12f, .24f);
        }

        public void OnBoardHandle()
        {
            if (Color == JunkColor.VipProMax)
            {
                var pos = (transform.position + Vector3.up * .24f).ToCanvasPosFromWorldPos(CanvasFloatingStuff.cur.canvasRect, true);
                CanvasFloatingStuff.cur.PopVipMoney(pos);

                AudioManager.ins.PlaySound(SoundType.SpinTick);

                ResourcesDataFragment.cur.AddGold(Config.cur.goldPerVip, "TIP_FROM_VIP");
            }
        }
    }
}