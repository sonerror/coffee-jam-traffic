using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    public class ParkingLot : MonoBehaviour
    {
        public static ParkingLot cur;
        public Transform ShipperStartPoint;
        public Transform RoadPoint;
        public Transform playCamRestPoint;
        public Transform playCamTabletPoint;
        public Transform onRoadPoint;
        private bool isTablet;

        [SerializeField] private Vector3 minPoint;
        [SerializeField] private Vector3 maxPoint;
        [SerializeField] private Vector3 minionRunAwayLinePoint;
        [SerializeField] private Vector3 endOfMinionLinePoint;

        private Plane upPlane;
        private Plane downPlane;
        private Plane leftPlane;
        private Plane rightPlane;

        public Plane MinionPlane { get; private set; }
        public Vector3 MinionExitPoint { get; private set; } // not set yet
        public Vector3 BusExitPoint { get; private set; }

        public ParkSlot[] parkSlots;

        //minion stand
        public bool isSingleLine;
        public bool isThirdLineOpen;
        public bool isBarrier;
        public bool isContinueBarrier;

        public Vector3[] minionsStandPos;
        private int minionStandPosMaxIndex;

        public Vector3[] firstStandPosLine; //from right to left
        public Vector3[] secondStandPosLine;
        public Vector3[] thirdStandPosLine;
        [SerializeField] private int maxMinionLineIndex;

        [SerializeField] private Vector3 firstExitPos;
        [SerializeField] private Vector3 secondExitPos;
        [SerializeField] private Vector3 thirdExitPos;

        public bool IsMinionQueueMoving { get; private set; }
        public bool IsFirstMinionQueueMoving { get; private set; }
        public bool IsSecondMinionQueueMoving { get; private set; }
        public bool IsThirdMinionQueueMoving { get; private set; }
        public Queue<Minion> MinionsQueue { get; } = new Queue<Minion>();
        public Queue<Minion> FirstMinionQueue { get; } = new Queue<Minion>();
        public Queue<Minion> SecondMinionQueue { get; } = new Queue<Minion>();
        public Queue<Minion> ThirdMinionQueue { get; } = new Queue<Minion>();
        private const float MINION_GROUND_OFFSET = .046f;

        public Queue<JunkColor> AwaitMinions { get; set; } = new Queue<JunkColor>();

        public TMP_Text minionCountText;
        public TMP_Text minionMultiCountText;

        public int MinionsLeft { get; private set; }
        private int maxMinions;

        public GameObject lightObject;

        public GameObject singleLineObject;
        public GameObject multiLineObject;

        public GameObject textSignHighObject;
        public GameObject textSignLowObject;

        public Transform textSignHighPoint;
        public Transform textSignLowPoint;

        public Transform textSignTrans;

        public GameObject stopBoardObject;

        public GameObject[] barrierObjects;
        public GameObject[] barrierLightObjects;

        public Transform babyTutRefPoint;
        private Vector3 originMaxPoint;
        private void Awake()
        {
            cur = this;
            originMaxPoint = maxPoint;
            isTablet = AdsManager.Ins.isCheckTablet && (float)Screen.height / Screen.width < 1921f / 1080;
        }

        [ContextMenu("FORCE START")]
        private void Start()
        {
            upPlane = new Plane(Vector3.forward, maxPoint);
            downPlane = new Plane(Vector3.forward, minPoint);
            leftPlane = new Plane(Vector3.right, minPoint);
            rightPlane = new Plane(Vector3.right, maxPoint);
            MinionPlane = new Plane(Vector3.forward, minionRunAwayLinePoint);
            MinionExitPoint = new Vector3(endOfMinionLinePoint.x, MINION_GROUND_OFFSET, minionRunAwayLinePoint.z);

            BusExitPoint = new Vector3(maxPoint.x + 5.2f, 0, maxPoint.z);

            minionStandPosMaxIndex = minionsStandPos.Length - 1;
        
            if (AdsManager.Ins.IsTabletOrSame())
            {
                textSignHighObject.SetActive(false);
                textSignLowObject.SetActive(true);
                /*
                textSignTrans.position = textSignLowPoint.position;
            */
            }
            else
            {
                textSignHighObject.SetActive(true);
                textSignLowObject.SetActive(false);
                /*
                textSignTrans.position = textSignHighPoint.position;
            */
            }
        }


       public void ReCalMinAndMaxPoint(Bus bus=null)
        {
            if (DataManager.ins == null) return;
            var groundedBuses = BusStation.cur.groundBus;
            //2.5 bottom,left right
            var left = 1000f;
            var right = -1000f;
            var top = -1000f;
            var bottom = +1000f;

            foreach (var groundbus in groundedBuses)
            {
                if(!Bus.IsOnTheSameLayer(groundbus.transform,bus.transform)) continue;;
                if (groundbus != null)
                {
                    if (groundbus.IsStayOnBusZone && !groundbus.IsSlideBus)
                    {
                        var busPosition = groundbus.transform.position;
                        var added = 0f;
                        switch (groundbus.Type)
                        {
                            case BusType.Bus:
                                added = 1.4f;
                                break;
                            default:
                                added = 1.15f;
                                break;
                        }
                        if (busPosition.x-added < left)
                        {
                            left = busPosition.x-added;
                        }
                        if (busPosition.x+added > right)
                        {
                            right = busPosition.x + added;
                        }
                        if (busPosition.z > top)
                        {
                            top = busPosition.z;
                        }
                        if (busPosition.z-added < bottom)
                        {
                            bottom = busPosition.z-added;
                        }
                    }
                }
            }

            var bustunnels = FindObjectsByType<BusTunnel>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var busTunnel in bustunnels)
            {
                if(!Bus.IsOnTheSameLayer(busTunnel.transform,bus.transform)) continue;;
                var added = 1.5f;
                var busPosition = busTunnel.transform.position;
                if (busPosition.x-added < left)
                {
                    left = busPosition.x-added;
                }
                if (busPosition.x+added > right)
                {
                    right = busPosition.x + added;
                }
                if (busPosition.z > top)
                {
                    top = busPosition.z;
                }
                if (busPosition.z-added < bottom)
                {
                    bottom = busPosition.z-added;
                }
            }
            

            var offset = 1.4f;
            offset = 0f;
            minPoint.x = left - offset;
            minPoint.z = bottom - offset;
            maxPoint.x = right + offset;
            maxPoint.z = top + offset;
            if (bus.IsSlideBus)
            {
                maxPoint = originMaxPoint;
            }
            /*var th = 5.4f;
            minPoint.x = Mathf.Clamp(minPoint.x, -th, th);
            maxPoint.x = Mathf.Clamp(maxPoint.x, -th, th);*/

            /*
            maxPoint.z = top + offset;
        */
            
            UpdatePlanes();
        }

        private void UpdatePlanes()
        {
            upPlane = new Plane(Vector3.forward, maxPoint+Vector3.left*20);// for slide 
            downPlane = new Plane(Vector3.forward, minPoint);
            leftPlane = new Plane(Vector3.right, minPoint);
            rightPlane = new Plane(Vector3.right, maxPoint);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(minPoint, Vector3.forward * 20);
            Gizmos.DrawRay(minPoint, Vector3.right * 20);
            Gizmos.DrawRay(maxPoint, Vector3.back * 20);
            Gizmos.DrawRay(maxPoint, Vector3.left * 20);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(minionRunAwayLinePoint, Vector3.left * 20);

            // Gizmos.color = Color.red;
            // Gizmos.DrawRay(endOfMinionLinePoint, Vector3.up * 10);
        }
#endif

        public void SetupMap()
        {
            singleLineObject.SetActive(isSingleLine);
            multiLineObject.SetActive(!isSingleLine);

            stopBoardObject.SetActive(!isThirdLineOpen);

            SetupLight(isBarrier);
            SetLight(0);
            if (false)
            {
                ReCalMinAndMaxPoint();
            }
        }

        public Transform GetCamRestPoint()
        {
            return isTablet ? playCamTabletPoint : playCamRestPoint;
        }

        public void UnlockPossibleAdsParkSlot()
        {
            for (int i = 1; i < parkSlots.Length; i++)
            {
                var slot = parkSlots[i];
                if (slot.IsAdsSlot)
                {
                    slot.UnlockSlot();
                    return;
                }
            }
        }

        public bool IsAnyAdsSlotLeft()
        {
            for (int i = 1; i < parkSlots.Length; i++)
            {
                var slot = parkSlots[i];
                if (slot.IsAdsSlot) return true;
            }

            return false;
        }

        public bool IsOneSlotLeft()
        {
            int num = 0;
            for (int i = 1; i < parkSlots.Length; i++)
            {
                var slot = parkSlots[i];
                if (!slot.IsAdsSlot && !slot.IsSlotTaken)
                {
                    num++;
                }
            }

            return num == 1;
        }

        public void UnlockSlotAfterBuyingVipPack()
        {
            var preNum = 0;
            if (!parkSlots[5].IsAdsSlot && !parkSlots[5].IsUnlockByVipPass) preNum++;
            if (!parkSlots[6].IsAdsSlot && !parkSlots[6].IsUnlockByVipPass) preNum++;
            if (!parkSlots[7].IsAdsSlot && !parkSlots[7].IsUnlockByVipPass) preNum++;

            parkSlots[5].SetupSlot(true);
            parkSlots[6].SetupSlot(true);
            parkSlots[7].SetupSlot(true);

            CheckVipPackSlot();

            if (parkSlots[5].IsAdsSlot && preNum > 0)
            {
                parkSlots[5].UnlockSlot();
                preNum--;
            }

            if (parkSlots[6].IsAdsSlot && preNum > 0)
            {
                parkSlots[6].UnlockSlot();
                preNum--;
            }

            if (parkSlots[7].IsAdsSlot && preNum > 0)
            {
                parkSlots[7].UnlockSlot();
            }
        }

        public void CheckVipPackSlot()
        {
            var unlockNum = 0;
            if (VipPassDataFragment.cur.gameData.isVip_3_Active) unlockNum++;
            if (VipPassDataFragment.cur.gameData.isVip_7_Active) unlockNum++;
            if (VipPassDataFragment.cur.gameData.isVip_15_Active) unlockNum++;

            if (unlockNum >= 1) parkSlots[5].UnlockSlot(true);
            if (unlockNum >= 2) parkSlots[6].UnlockSlot(true);
            if (unlockNum >= 3) parkSlots[7].UnlockSlot(true);
        }

        public void FetchMinionsNUm(int num)
        {
            MinionsLeft = maxMinions = num;
            UpdateMinionLeftText();
        }

        public float GetCompletePercent()
        {
            return 1 - (float)MinionsLeft / maxMinions;
        }

        public void ResetAll()
        {
            for (int i = 0; i < parkSlots.Length; i++)
            {
                var ps = parkSlots[i];
                ps.ReleaseBus();
            }

            parkSlots[1].SetupSlot(false);
            parkSlots[2].SetupSlot(false);
            parkSlots[3].SetupSlot(false);
            parkSlots[4].SetupSlot(false);
            parkSlots[5].SetupSlot(true);
            parkSlots[6].SetupSlot(true);
            parkSlots[7].SetupSlot(true);
            foreach (var parkslot in parkSlots)
            {
                parkslot.transform.localScale = Vector3.one * (JunkPile.ins.GetScaleFactor()>1.12?1.12f:JunkPile.ins.GetScaleFactor());
            }
            CheckVipPackSlot();

            IsMinionQueueMoving = false;
            IsFirstMinionQueueMoving = false;
            IsSecondMinionQueueMoving = false;
            IsThirdMinionQueueMoving = false;
            MinionsQueue.Clear();
            FirstMinionQueue.Clear();
            SecondMinionQueue.Clear();
            ThirdMinionQueue.Clear();
            AwaitMinions.Clear();
        }

        public bool IsAllSlotTaken(out ParkSlot parkSlot)
        {
            parkSlot = parkSlots[0];
            for (int i = 0; i < parkSlots.Length; i++)
            {
                var ps = parkSlots[i];
                if (!ps.IsSlotTaken && !ps.IsAdsSlot)
                {
                    parkSlot = ps;
                    return false;
                }
            }

            return true;
        }

        public ParkSlot GetVipSlot()
        {
            return parkSlots[0];
        }

        public Vector3[] GeneratePath(ParkSlot parkSlot, Bus bus)
        {
            var (enterPoint, restPoint) = parkSlot.GetEnterAndRestPoint(upPlane);

            if (bus.IsSlideBus)
            {
                return new[] { restPoint };
            }
            else
            {
                var (startPoint, direction) = GetExitPoint(bus.transform.position, bus.transform.forward);
                var wayPoints = new List<Vector3> { startPoint };
                switch (direction)
                {
                    case ExitDirection.Up:
                        break;
                    case ExitDirection.Down:
                        wayPoints.Add(new Vector3(maxPoint.x, 0, minPoint.z));
                        wayPoints.Add(new Vector3(maxPoint.x, 0, maxPoint.z));
                        break;
                    case ExitDirection.Left:
                        wayPoints.Add(new Vector3(minPoint.x, 0, maxPoint.z));
                        break;
                    case ExitDirection.Right:
                        wayPoints.Add(new Vector3(maxPoint.x, 0, maxPoint.z));
                        break;
                }

                /*
                wayPoints.Add(enterPoint);
                */
                wayPoints.Add(restPoint);
#if UNITY_EDITOR
                for (int i = 0; i < wayPoints.Count; i++)
                {
                    Debug.DrawRay(wayPoints[i], Vector3.up * .5f, Color.cyan, 10);
                }
#endif
                return wayPoints.ToArray();

            }
           

            /*float pathLength = Vector3.Distance(startPoint, bus.transform.position);
            for (int i = 1; i < wayPoints.Count; i++) pathLength += Vector3.Distance(wayPoints[i], wayPoints[i - 1]);*/




            (Vector3, ExitDirection) GetExitPoint(Vector3 point, Vector3 direction)
            {
                var isUp = Vector3.Dot(direction, Vector3.forward) > 0;
                var isRight = Vector3.Dot(direction, Vector3.right) > 0;

                var ray = new Ray(point, direction);
                Debug.DrawRay(ray.origin, ray.direction * 5, Color.yellow, 5f);
                if ((isUp ? upPlane : downPlane).Raycast(ray, out var time))
                {
                    var intersectPoint = ray.GetPoint(time);
                    Debug.DrawRay(intersectPoint, Vector3.up * .5f, Color.red, 5);
                    if (intersectPoint.x < maxPoint.x && intersectPoint.x > minPoint.x) return (intersectPoint, isUp ? ExitDirection.Up : ExitDirection.Down);
                }

                if ((isRight ? rightPlane : leftPlane).Raycast(ray, out var subTime))
                {
                    var intersectPoint = ray.GetPoint(subTime);
                    Debug.DrawRay(intersectPoint, Vector3.up * .5f, Color.blue, 5);
                    if (intersectPoint.z < maxPoint.z && intersectPoint.z > minPoint.z) return (intersectPoint, isRight ? ExitDirection.Right : ExitDirection.Left);
                }

                return (point + direction * 2f, isRight ? ExitDirection.Right : ExitDirection.Left);
            }
        }

        private enum ExitDirection
        {
            Up,
            Down,
            Left,
            Right,
        }

        public (Vector3[], float) GenerateExitPath(ParkSlot parkSlot)
        {
            var (enterPoint, restPoint) = parkSlot.GetEnterAndRestPoint(upPlane);

            var path = new Vector3[]
            {
                enterPoint,
                enterPoint + Vector3.left * .12f,
            };

            var pathLength = Vector3.Distance(restPoint, enterPoint) + Vector3.Distance(path[0], path[1]);
            return (path, pathLength);
        }

        public (Vector3, Vector3 ) GetStandPointPair(int index, int flag)
        {
            if (isSingleLine)
            {
                index = Mathf.Max(index, 0);
                var curPos = minionsStandPos[Mathf.Min(minionStandPosMaxIndex, index)];
                var prevPos = minionsStandPos[Mathf.Min(minionStandPosMaxIndex, index + 1)];

                return (curPos, prevPos);
            }
            else
            {
                index = Mathf.Max(index, 0);
                var targetStandPos = flag switch
                {
                    0 => firstStandPosLine,
                    1 => secondStandPosLine,
                    _ => thirdStandPosLine,
                };

                var curPos = targetStandPos[Mathf.Min(maxMinionLineIndex, index)];
                var prevPos = targetStandPos[Mathf.Min(maxMinionLineIndex, index + 1)];

                return (curPos, prevPos);
            }
        }

        //minion
        public Vector3[] GetMinionPath(Bus bus, int flag = 0)
        {
            /*
            var index = Mathf.FloorToInt((bus.CurrentMinionNum - 1) / 2f);
            */
            /*
            var sidePoint = bus.transform.TransformPoint(bus.bakedSideStepPosList[index]) + Vector3.up * MINION_GROUND_OFFSET;
            */
            var exitPoint = isSingleLine
                ? MinionExitPoint
                : flag switch
                {
                    0 => firstExitPos,
                    1 => secondExitPos,
                    2 => thirdExitPos,
                };

            /*
            var ray = new Ray(sidePoint, Vector3.forward);
            */
            /*
            var intersectPoint = MinionPlane.Raycast(ray, out var time) ? ray.GetPoint(time) : sidePoint;
            */

            var path = new Vector3[] { exitPoint/*, intersectPoint, sidePoint */};
            return path;
        }

        public int GetStandPosNum()
        {
            if (isSingleLine) return minionStandPosMaxIndex + 1;
            else return isThirdLineOpen ? (maxMinionLineIndex + 1) * 3 : (maxMinionLineIndex + 1) * 2;
        }

        public int GetMultiLineStandPosNum()
        {
            return maxMinionLineIndex + 1;
        }

        public bool IsAnyLineHasSlot()
        {
            return IsFirstLineHasSlot() || IsSecondLineHasSlot() || IsThirdLineHasSlot();
        }

        public bool IsAllLineEmpty()
        {
            return FirstMinionQueue.Count == 0 && SecondMinionQueue.Count == 0 && ThirdMinionQueue.Count == 0;
        }

        public bool IsFirstLineHasSlot()
        {
            return FirstMinionQueue.Count <= maxMinionLineIndex;
        }

        public bool IsSecondLineHasSlot()
        {
            return SecondMinionQueue.Count <= maxMinionLineIndex;
        }

        public bool IsThirdLineHasSlot()
        {
            return isThirdLineOpen && ThirdMinionQueue.Count <= maxMinionLineIndex;
        }

        public void AddMinionToQueue(List<JunkColor> colors)
        {
            if (isSingleLine)
            {
                for (int i = 0; i < colors.Count; i++)
                {
                    if (MinionsQueue.Count < minionStandPosMaxIndex + 1) AddMinionToQueue(JunkPile.ins.GetMinion(colors[i]));
                    else AwaitMinions.Enqueue(colors[i]);
                }
            }
            else
            {
                for (int i = 0; i < colors.Count; i++)
                {
                    if (IsAnyLineHasSlot()) AddMinionToQueue(JunkPile.ins.GetMinion(colors[i]));
                    else AwaitMinions.Enqueue(colors[i]);
                }
            }
        }

        public void EnqueueAwaitingMinions()
        {
            if (AwaitMinions.Count == 0) return;
            AddMinionToQueue(JunkPile.ins.GetMinion(AwaitMinions.Dequeue()));
        }

        private float _initAddMinionToQueueLerp = 1f;

        public void SetMinionToQueueLerp(float l)
        {
            _initAddMinionToQueueLerp = l;
        }
        public void AddMinionToQueue(Minion minion)
        {
            float lerp = _initAddMinionToQueueLerp;
            if (isSingleLine)
            {
                minion.EnterQueue(MinionsQueue.Count);
                MinionsQueue.Enqueue(minion);
                minion.MoveQueueSegment(lerp);
            }
            else
            {
                if (IsFirstLineHasSlot())
                {
                    minion.EnterQueue(FirstMinionQueue.Count);
                    minion.LineFlag = 0;
                    FirstMinionQueue.Enqueue(minion);
                    minion.MoveQueueSegment(lerp);
                    return;
                }

                if (IsSecondLineHasSlot())
                {
                    minion.EnterQueue(SecondMinionQueue.Count);
                    minion.LineFlag = 1;
                    SecondMinionQueue.Enqueue(minion);

                    minion.MoveQueueSegment(lerp);
                    return;
                }

                if (isThirdLineOpen && IsThirdLineHasSlot())
                {
                    minion.EnterQueue(ThirdMinionQueue.Count);
                    minion.LineFlag = 2;
                    ThirdMinionQueue.Enqueue(minion);

                    minion.MoveQueueSegment(lerp);
                }
            }
        }

        public void JustMoveMinion(float speedMul = 1, int flag = -1)
        {
            if (isSingleLine)
            {
                if (IsMinionQueueMoving || MinionsQueue.Count == 0) return;
                IsMinionQueueMoving = true;
                JustUpdateQueueIndex();
                DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime / speedMul, (fuk) => MoveQueue(fuk)).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        MoveQueue(1);
                        IsMinionQueueMoving = false;
                    });
            }
            else
            {
                if (flag == 0)
                {
                    if (IsFirstMinionQueueMoving || FirstMinionQueue.Count == 0) return;
                    IsFirstMinionQueueMoving = true;
                    JustUpdateLineQueueIndex(0);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime / speedMul, (fuk) => MoveQueue(fuk, 0)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveQueue(1, 0);
                            IsFirstMinionQueueMoving = false;
                        });
                }
                else if (flag == 1)
                {
                    if (IsSecondMinionQueueMoving || SecondMinionQueue.Count == 0) return;
                    IsSecondMinionQueueMoving = true;
                    JustUpdateLineQueueIndex(1);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime / speedMul, (fuk) => MoveQueue(fuk, 1)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveQueue(1, 1);
                            IsSecondMinionQueueMoving = false;
                        });
                }
                else if (flag == 2)
                {
                    if (IsThirdMinionQueueMoving || ThirdMinionQueue.Count == 0) return;
                    IsThirdMinionQueueMoving = true;
                    JustUpdateLineQueueIndex(2);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime / speedMul, (fuk) => MoveQueue(fuk, 2)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            MoveQueue(1, 2);
                            IsThirdMinionQueueMoving = false;
                        });
                }
            }
        }

        private void JustUpdateQueueIndex()
        {
            if (MinionsQueue.Count == 0) return;
            foreach (var m in MinionsQueue)
            {
                m.MoveNextInQueue();
            }
        }

        private void JustUpdateLineQueueIndex(int flag)
        {
            switch (flag)
            {
                case 0 when FirstMinionQueue.Count > 0:
                {
                    foreach (var m in FirstMinionQueue) m.MoveNextInQueue();
                    break;
                }
                case 1 when SecondMinionQueue.Count > 0:
                {
                    foreach (var m in SecondMinionQueue) m.MoveNextInQueue();
                    break;
                }
                case 2 when ThirdMinionQueue.Count > 0:
                {
                    foreach (var m in ThirdMinionQueue) m.MoveNextInQueue();
                    break;
                }
            }
        }

        public void ReleaseMinionPostHandle(int flag = -1)
        {
            if (isSingleLine)
            {
                if (IsMinionQueueMoving || MinionsQueue.Count == 0) return;
                IsMinionQueueMoving = true;
                UpdateQueueIndex();
                DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime, (fuk) => MoveQueue(fuk)).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        MoveQueue(1);
                        IsMinionQueueMoving = false;
                    });
            }
            else
            {
                if (flag == 0)
                {
                    if (IsFirstMinionQueueMoving || FirstMinionQueue.Count == 0) return;
                    IsFirstMinionQueueMoving = true;
                    UpdateQueueIndex(0);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime, (fuk) => MoveQueue(fuk, 0)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            IsFirstMinionQueueMoving = false;
                            MoveQueue(1, 0);
                        });
                }
                else if (flag == 1)
                {
                    if (IsSecondMinionQueueMoving || SecondMinionQueue.Count == 0) return;
                    IsSecondMinionQueueMoving = true;
                    UpdateQueueIndex(1);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime, (fuk) => MoveQueue(fuk, 1)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            IsSecondMinionQueueMoving = false;
                            MoveQueue(1, 1);
                        });
                }
                else if (flag == 2)
                {
                    if (IsThirdMinionQueueMoving || ThirdMinionQueue.Count == 0) return;
                    IsThirdMinionQueueMoving = true;
                    UpdateQueueIndex(2);
                    DOVirtual.Float(0, 1, Config.cur.minionQueueSegmentMoveTime, (fuk) => MoveQueue(fuk, 2)).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            IsThirdMinionQueueMoving = false;
                            MoveQueue(1, 2);
                        });
                }
            }
        }

        public void UpdateQueueIndex(int flag = -1)
        {
            if (isSingleLine)
            {
                JustUpdateQueueIndex();
            }
            else
            {
                JustUpdateLineQueueIndex(flag);
            }

            EnqueueAwaitingMinions();
        }

        public void MoveQueue(float per, int flag = -1)
        {
            if (isSingleLine)
            {
                if (MinionsQueue.Count == 0) return;
                foreach (var m in MinionsQueue)
                {
                    m.MoveQueueSegment(per);
                }
            }
            else
            {
                if (flag == 0 && FirstMinionQueue.Count > 0)
                {
                    foreach (var m in FirstMinionQueue) m.MoveQueueSegment(per);
                }
                else if (flag == 1 && SecondMinionQueue.Count > 0)
                {
                    foreach (var m in SecondMinionQueue) m.MoveQueueSegment(per);
                }
                else if (flag == 2 && ThirdMinionQueue.Count > 0)
                {
                    foreach (var m in ThirdMinionQueue) m.MoveQueueSegment(per);
                }
            }
        }

        public Minion GetCurrentMinion(int flag = -1)
        {
            if (isSingleLine)
            {
                return MinionsQueue.Count == 0 ? null : MinionsQueue.Peek();
            }
            else
            {
                return flag switch
                {
                    0 => FirstMinionQueue.Count == 0 ? null : FirstMinionQueue.Peek(),
                    1 => SecondMinionQueue.Count == 0 ? null : SecondMinionQueue.Peek(),
                    2 => ThirdMinionQueue.Count == 0 ? null : ThirdMinionQueue.Peek(),
                };
            }
        }

        public Minion PopMinion(int flag = -1)
        {
            if (isSingleLine)
            {
                if (MinionsQueue.Count == 0) return null;
                var minion = MinionsQueue.Dequeue();
                MinionsLeft--;
                UpdateMinionLeftText();

                return minion;
            }
            else
            {
                if (flag == 0 && FirstMinionQueue.Count > 0)
                {
                    var minion = FirstMinionQueue.Dequeue();
                    MinionsLeft--;
                    UpdateMinionLeftText();

                    return minion;
                }

                if (flag == 1 && SecondMinionQueue.Count > 0)
                {
                    var minion = SecondMinionQueue.Dequeue();
                    MinionsLeft--;
                    UpdateMinionLeftText();

                    return minion;
                }

                if (flag == 2 && ThirdMinionQueue.Count > 0)
                {
                    var minion = ThirdMinionQueue.Dequeue();
                    MinionsLeft--;
                    UpdateMinionLeftText();

                    return minion;
                }
            }

            return null;
        }

        public void UpdateMinionLeftText()
        {
            if (isSingleLine)
            {
                minionCountText.text = MinionsLeft.ToString();
            }
            else
            {
                minionMultiCountText.text = MinionsLeft.ToString();
            }
        }

        public void SetupLight(bool isOn)
        {
            barrierObjects[0].SetActive(isOn);
            barrierObjects[1].SetActive(isOn);
            barrierObjects[2].SetActive(isOn && isThirdLineOpen);
        }

        public void SetLight(int index)
        {
            var length = isThirdLineOpen ? 3 : 2;
            var trueIndex = index % (length);
            for (int i = 0; i < length; i++)
            {
                var isOn = i == trueIndex;
                barrierLightObjects[i * 2].SetActive(isOn);
                barrierLightObjects[i * 2 + 1].SetActive(!isOn);
            }
        }
#if UNITY_EDITOR
        //bake minion pos
        [SerializeField] private Transform[] minionStandPoints; // from end of line

        [ContextMenu("BAKE MINION POS")]
        private void BakePos()
        {
            var list = new List<Vector3>();
            for (int i = 0; i < minionStandPoints.Length; i++)
            {
                list.Add(minionStandPoints[i].position);
            }

            minionsStandPos = list.ToArray();
        }

        [SerializeField] private Transform[] firstStandPoints;
        [SerializeField] private Transform[] secondStandPoints;
        [SerializeField] private Transform[] thirdStandPoints;

        [ContextMenu("BAKE MULTI LINE")]
        private void BakeMultiLines()
        {
            var firstList = new List<Vector3>();
            var secondList = new List<Vector3>();
            var thirdList = new List<Vector3>();

            for (int i = 0; i < firstStandPoints.Length; i++) firstList.Add(firstStandPoints[i].position);
            for (int i = 0; i < secondStandPoints.Length; i++) secondList.Add(secondStandPoints[i].position);
            for (int i = 0; i < thirdStandPoints.Length; i++) thirdList.Add(thirdStandPoints[i].position);

            firstStandPosLine = firstList.ToArray();
            secondStandPosLine = secondList.ToArray();
            thirdStandPosLine = thirdList.ToArray();

            maxMinionLineIndex = firstStandPoints.Length - 1;
        }
#endif
    }
}