using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Bus
{
    [SelectionBase]
    public class Bus : MonoBehaviour, IPointerDownHandler
    {
        public static Dictionary<Collider, Bus> busColliderDict = new Dictionary<Collider, Bus>();
        
        public new GameObject gameObject;
        public new Transform transform;
        public Animator BoxAnimator;
        public Vector3 RootScale { get; set; }
        public static Bus LastMovedBus { get; private set; } // only use for Count Down Note

        public GameObject[] hideSeatModelObject;

        public Transform MinionHolder;
        /*
        public GameObject[] showSeatModelObject;
        */
        public Transform PickUpPoint;
        
        public Transform[] headPointTrans;

        public Transform upperModelTrans;
        public BoxCollider boxCollider;

        public BusType Type => busType;
        [SerializeField] private BusType busType;
        public JunkColor color;
        public bool IsPushed { get; set; }
        public bool IsInActiveList { get; set; }
        public bool IsTouchable;
        public bool IsOutOfTunnel { get; set; }
        public bool IsGrayBus { get; set; }
        public bool IsSlideBus { get; set; }

        public ParkSlot CurrentParkSlot { get; set; }

        public Quaternion RootUpperRotation { get; set; }
        public Vector3 RootPosition { get; set; }
        public Renderer[] mainColorRend;

        public bool IsStayOnBusZone { get; set; }
        public bool IsBusTaken { get; set; }
        public bool IsBusCarPartTaken { get; set; }
        public bool IsBusVanPartTaken { get; set; }

        public List<Bus> nextNodeList;
        public List<Bus> backTrackNodeList;
        public List<Bus> directBackNodeList;
        public int Weight { get; set; }

        public int Layer
        {
            get
            {
                var yPosition = transform.position.y;
                if (yPosition < 0.4f)
                {
                    return  1;
                }
                else
                {
                    if (yPosition < 0.8f)
                    {
                        return 2;
                    }
                    else
                    {
                        if (yPosition < 1.5f)
                        {
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                }
            }
        }
        private Tween shakeTween;
        private Tween moveTween;
        private Tween popTween;

        public GameObject smokeEffectObject;

        //minions
        public int CurrentMinionNum { get; private set; }
        [SerializeField] private int maxMinions;
        public int MaxMinion => maxMinions;

        private List<Minion> currentMinions = new List<Minion>();

        public Transform[] seatTransList;
        public Transform[] sideStepTransList;

        public Vector3[] bakedSeatPosList;
        /*
        public Vector3[] bakedSideStepPosList;
        */

        //objective
        public Material CurrentGrayMat { get; set; }
        private Tween grayTween;
        private Tween markTween;

        public GameObject arrowObject;
        public GameObject questionObject;
        public Transform markTrans;
        [SerializeField] private Vector3 markRootScale;

        [SerializeField] private float onParkScaleFactor = 1f;
        private Vector3 minionLocalPosition;

        public bool IsCovering
        {
            get
            {
                return nextNodeList.Any(v =>
                {
                    return (v.transform.position.y - transform.position.y) > BUSLAYER_DISTANCE_THRESHOLD && v.IsStayOnBusZone;
                });
            }
        }

        private const float BUSLAYER_DISTANCE_THRESHOLD = 0.4f;
        
        public bool IsCoveredBus = false;
        
        private void Awake()
        {
            RootUpperRotation = upperModelTrans.localRotation;
            minionLocalPosition = MinionHolder.localPosition;
            if (boxCollider != null) busColliderDict.Add(boxCollider, this);

            if (markTrans != null) markRootScale = markTrans.localScale;
        }

        public static Bus GetBusFromCol(Collider col)
        {
            return busColliderDict.GetValueOrDefault(col);
        }

#if UNITY_EDITOR
        // private void OnDestroy()
        // {
        //     IsBusTaken = true;
        //     BusStation.cur.RemoveFromActiveBuses(this);
        //     DecreaseWeight();
        //     BusStation.cur.DrawBranch();
        // }

        [ContextMenu("BAKE POS")]
        private void BakePos()
        {
            var seatList = new List<Vector3>();
            var sideList = new List<Vector3>();

            for (int i = 0; i < seatTransList.Length; i++)
            {
                seatList.Add(seatTransList[i].localPosition);
            }

            for (int i = 0; i < sideStepTransList.Length; i++)
            {
                sideList.Add(sideStepTransList[i].localPosition);
            }

            bakedSeatPosList = seatList.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            /*
            bakedSideStepPosList = sideList.ToArray();
        */
        }
#endif

        public virtual void OnInit()
        {
            BoxAnimator.ResetAllAnimatorTriggers();
            transform.localScale = RootScale * JunkPile.ins.GetScaleFactor();
            upperModelTrans.localRotation = RootUpperRotation;
            RootPosition = transform.position;
            IsTouchable = true;
            
            currentMinions.Clear();
            CurrentMinionNum = 0;

            IsStayOnBusZone = true;
            IsBusTaken = false;
            IsBusCarPartTaken = false;
            IsBusVanPartTaken = false;
            IsOutOfTunnel = false;
            IsGrayBus = false;
            IsSlideBus = false;

            CurrentGrayMat = null;

            directBackNodeList.Clear();
            backTrackNodeList.Clear();
            nextNodeList.Clear();

            SetBusOpenOrHide(true);

            grayTween?.Kill();
            markTween?.Kill();

            smokeEffectObject.SetActive(false);

            arrowObject.SetActive(true);
            questionObject.SetActive(false);
            markTrans.localScale = markRootScale;
            MinionHolder.localPosition = minionLocalPosition;

            ForceUnCoverd();
        }


        public void Setup()
        {
            /*
            RootScale = new Vector3(7.17600012f, 7.17600012f, 7.17600012f);
            */
            IsCoveredBus = IsCovering;
            transform.localScale = RootScale * JunkPile.ins.GetScaleFactor();
            arrowObject.GetComponentInChildren<SpriteRenderer>().color = JunkPile.ins.NormalArrowColor;
        }
        
        public virtual void OnPostInit()
        {
        }

        public virtual void OnPush()
        {
            moveTween?.Kill();
            popTween?.Kill();
            transform.DOKill();
        }

        private void HandleNextNodeList()
        {
            var origins = new List<Bus>(nextNodeList);
            foreach (var bus in origins)
            {
                var nn = bus;
                if (nn == null || !nn.IsStayOnBusZone ||nn.gameObject.activeSelf==false)
                {
                    nextNodeList.Remove(bus);
                }
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsTouchable || !TransportCenter.cur.IsGamePlaying) return;
            if (IsSlideBus && !BusSlide.cur.WantToMove(this)) return;
            HandleNextNodeList();
            if(IsCovering&&IsCoveredBus) return;
            IsTouchable = false;

            AudioManager.ins.PlaySound(SoundType.BusClicked);
            AudioManager.ins.MakeVibrate();

            if (BusStation.cur.IsVipBus)
            {
                BusStation.cur.CurrentVipBus = this;
                return;
            }

            Move();
        }

        public void SetBusOpenOrHide(bool isHideSeat)
        {
            /*hideSeatModelObject[0].SetActive(isHideSeat);
            hideSeatModelObject[1].SetActive(isHideSeat);
            showSeatModelObject[0].SetActive(!isHideSeat);
            showSeatModelObject[1].SetActive(!isHideSeat);*/
            hideSeatModelObject[0].SetActive(isHideSeat);
            /*
            hideSeatModelObject[1].SetActive(isHideSeat);
            */
            if (isHideSeat == false)
            {
                BoxAnimator.SetTrigger("open");
            }
        }

        public virtual void GetHit(Transform hitterTrans, Vector3 surfacePoint)
        {
            //shaking
            var isHitOnLeft = Vector3.Dot(hitterTrans.position - transform.position, transform.right) < 0;
            shakeTween?.Complete();
            shakeTween = upperModelTrans.DOPunchRotation(new Vector3(0, 0, isHitOnLeft ? -6.8f : 6.8f), .28f).SetEase(Ease.OutSine);

            AudioManager.ins.PlaySound(SoundType.BusCollide);
            ProjectileManager.ins.Pop(ProjectileType.HitSpark, surfacePoint, hitterTrans.rotation);
        }

        public void Jiggle()
        {
            shakeTween?.Complete();
            shakeTween = upperModelTrans.DOPunchRotation(new Vector3(0, 0, Random.Range(0, 2) == 0 ? -6.8f : 6.8f), .28f).SetEase(Ease.OutSine);
        }

        public void OnMoveOutOfParkZone()
        {
            if (!IsBusTaken)
            {
                BusStation.cur.AwaitForFetchDataBus.Add(GetColorDataWithBackTrack());
                IsBusTaken = true;
            }
            else
            {
                DecreaseWeight();
            }

            BusStation.cur.RemoveFromActiveBuses(this);
            BusStation.cur.RemoveFromGroundBuses(this);
            BusStation.cur.MovedBus++;

            LastMovedBus = this;

            VoidEventChannelSO.TriggerEvent(VoidEventType.OnBusMoveOutParkZone);

            if (ParkingLot.cur.IsOneSlotLeft()) CanvasGamePlay.PopOutOfSlotNoffStatic(CanvasGamePlay.ONE_SPACE_LEFT);

            BusStation.cur.CheckGrayBus();
            BusStation.cur.CheckCoveredBuses();
        }

        public bool Move() //return if moved
        {
            //check node
            var nextNode = GetNextNode();
            if (nextNode == null)
            {
                if (ParkingLot.cur.IsAllSlotTaken(out var parkSlot))
                {
                    IsTouchable = true;

                    CanvasGamePlay.PopOutOfSlotNoffStatic(CanvasGamePlay.OUT_OF_SLOT);
                    if (IsSlideBus) BusSlide.cur.IsBusDone = true;
                    return false;
                }

                //free to go
                IsStayOnBusZone = false;
                ParkingLot.cur.ReCalMinAndMaxPoint(this);
                var path = ParkingLot.cur.GeneratePath(parkSlot, this);
                parkSlot.PreParkTheBus(this);

                // Debug.Log("BUS MOVE " + pathLength, gameObject);
                /*
                smokeEffectObject.SetActive(true);
                */

                moveTween?.Kill();
                /*transform.DORotate(parkSlot.transform.eulerAngles, 0.75f);
                moveTween = transform.DOJump(path.Last(), 5, 1, 0.75f)
                    .OnComplete(() =>
                    {
                        AudioManager.ins.MakeVibrate();
                        parkSlot.IsBusArrive = true;
                        SetOnParkScale();
                        SetBusOpenOrHide(false);
                        TransportCenter.cur.StartMinionTransport(this);
                    });*/
                var jumpMinDuration = 0.55f;
                var jumpoMaxDuration = 0.8f;
                var jumpPower = 3.85f;
                if (path.Length > 1)
                {
                    var p = path[0];
                    if (Mathf.Abs(p.x) < -5 && p.z > -15.5 && p.z < -8) 
                    {
                        p += transform.forward.normalized*0.33f;
                    }
                    transform.DOMove(p, Config.cur.busMoveSpeed*1.25f).SetSpeedBased(true).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            var distance = Vector3.Distance(parkSlot.transform.position, transform.position);
                            var realJumpDuration = Mathf.Lerp(jumpMinDuration, jumpoMaxDuration, Mathf.Clamp((distance - 6) / 6,0f,1f));
                            transform.DORotate(parkSlot.transform.eulerAngles, realJumpDuration);
                            SetBusOpenOrHide(false);
                            moveTween = transform.DOJump(path.Last(), jumpPower, 1, realJumpDuration)
                                .SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    AudioManager.ins.MakeVibrate();
                                    parkSlot.IsBusArrive = true;
                                    SetOnParkScale();
                                    TransportCenter.cur.StartMinionTransport(this);
                                });
                        });
                }
                else
                {
                    var distance = Vector3.Distance(parkSlot.transform.position, transform.position);
                    var realJumpDuration = Mathf.Lerp(jumpMinDuration, jumpoMaxDuration, Mathf.Clamp((distance - 6) / 6,0f,1f));
                    transform.DORotate(parkSlot.transform.eulerAngles, realJumpDuration);
                    SetBusOpenOrHide(false);
                    moveTween = transform.DOJump(path.Last(), jumpPower, 1, realJumpDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        AudioManager.ins.MakeVibrate();
                        parkSlot.IsBusArrive = true;
                        SetOnParkScale();
                        transform.DOPunchScale(0.2f * transform.localScale, 0.15f);
                        /*
                        SetBusOpenOrHide(false);
                        */
                        TransportCenter.cur.StartMinionTransport(this);
                    });
                }
                /*moveTween = transform.DOPath(path, Config.cur.busMoveSpeed).SetLookAt(0.1f * (Config.cur.averageBusParkPathLength / pathLength), true).SetSpeedBased(true).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // Debug.Log("MOVE BUS COMPLETE");
                        AudioManager.ins.MakeVibrate();
                        parkSlot.IsBusArrive = true;
                        SetOnParkScale();
                        SetBusOpenOrHide(false);
                        TransportCenter.cur.StartMinionTransport(this);
                    });*/

                OnMoveOutOfParkZone();
                if (IsSlideBus) BusSlide.cur.IsBusDone = true;
                return true;
            }
            else
            {
                //hit another car
                moveTween?.Complete();
                var (point, surfacePoint) = GetHitPointAndPos(nextNode);
                if (Vector3.Distance(transform.position, point) < Config.cur.closeDistanceBetweenBuses)
                {
                    //bounce back instant
                    IsTouchable = true;
                    moveTween = transform.DOPunchPosition(-transform.forward * Config.cur.bounceBackDistance, .24f, 1).SetEase(Ease.OutSine)
                        .OnComplete(() =>
                        {
                            if (IsSlideBus) BusSlide.cur.IsBusDone = true;
                        });

                    nextNode.GetHit(transform, surfacePoint);
                }
                else
                {
                    //move and bounce back
                    moveTween = transform.DOMove(point, Config.cur.busMoveSpeed).SetSpeedBased(true).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            nextNode.GetHit(transform, surfacePoint);
                            moveTween = transform.DOMove(RootPosition, Config.cur.busMoveSpeed * 1.18f).SetSpeedBased(true).SetEase(Ease.OutSine)
                                .OnComplete(() =>
                                {
                                    IsTouchable = true;
                                    if (IsSlideBus) BusSlide.cur.IsBusDone = true;
                                });
                        });
                }
            }

            return false;
        }

        public void SetOnParkScale()
        {
            transform.localScale = RootScale * onParkScaleFactor * JunkPile.ins.GetScaleFactor();
        }

        private Bus GetNextNode()
        {
            while (nextNodeList.Count > 0)
            {
                var nn = nextNodeList.Peek();
                if (nn != null && nn.IsStayOnBusZone && !nn.IsOutOfTunnel) return nn;
                nextNodeList.Pop();
            }
            return null;
        }

        public bool IsClearAllNextNode()
        {
            return GetNextNode() == null;
        }

        public int GetBranchDepth()
        {
            int depth = 0;
            for (int i = 0; i < nextNodeList.Count; i++)
            {
                depth = Mathf.Max(depth, nextNodeList[i].GetBranchDepth());
            }

            return depth + 1;
        }

        public int GetBranchWeight(List<Bus> countedBusList)
        {
            int block = 1;
            for (int i = 0; i < nextNodeList.Count; i++)
            {
                var node = nextNodeList[i];
                if (countedBusList.Contains(node)) continue;
                countedBusList.Add(node);
                block += node.GetBranchWeight(countedBusList);
            }

            return block;
        }

        public static bool IsOnTheSameLayer(Transform bus1, Transform bus2)
        {
            return Mathf.Abs(bus1.position.y-bus2.position.y) < BUSLAYER_DISTANCE_THRESHOLD;
        }
        public void DecreaseWeight()
        {
            for (int i = 0; i < backTrackNodeList.Count; i++)
            {
                var bus = backTrackNodeList[i];
                if (!bus.IsOutOfTunnel) bus.Weight--;
            }
        }

        public void BackTrackAllNode()
        {
            backTrackNodeList = new List<Bus>();
            for (int i = 0; i < directBackNodeList.Count; i++)
            {
                directBackNodeList[i].BackTrackAllNode(backTrackNodeList);
            }
        }

        private void BackTrackAllNode(List<Bus> backList)
        {
            if (backList.Contains(this)) return;
            backList.Add(this);

            for (int i = 0; i < directBackNodeList.Count; i++)
            {
                directBackNodeList[i].BackTrackAllNode(backList);
            }
        }

        private (Vector3, Vector3) GetHitPointAndPos(Bus targetBus)
        {
            var targetCol = targetBus.boxCollider;

            var headPos = headPointTrans[0].position;
            var subHeadPos = headPointTrans[1].position;
            var refPoint = targetCol.ClosestPoint(headPos);
            var subRefPoint = targetCol.ClosestPoint(subHeadPos);

            var selectedRefPoint = Vector3.Distance(headPos, refPoint) < Vector3.Distance(subHeadPos, subRefPoint) ? refPoint : subRefPoint;
            var plane = new Plane(transform.forward, headPos);
            var contactPoint = plane.ClosestPointOnPlane(selectedRefPoint);
            var offset = RootPosition - contactPoint;

            return (selectedRefPoint + offset, (refPoint + subRefPoint) / 2);
        }

        public bool IsEmptySeat()
        {
            return CurrentMinionNum < maxMinions;
        }

        private void CheckReadyForGoing()
        {
            if (IsEmptySeat()) return;
            //queueing for exit parking slot
            TransportCenter.cur.RequireLeavePermit(this);
        }


        public void ForceLeaveTheCity()
        {
            for (int i = 0; i < currentMinions.Count; i++) JunkPile.ins.Push(currentMinions[i]);
            currentMinions.Clear();
            this.transform.SetParent(null);
            JunkPile.ins.Push(this);
            if (color == JunkColor.Red) TreasureDataFragment.cur.RecordObject(GetSeatNum(), TreasureDataFragment.TreasureType.Minion);
            if (color == JunkColor.Blue) TreasureDataFragment.cur.RecordObject(1, TreasureDataFragment.TreasureType.Bus);
            int GetSeatNum()
            {
                return Type switch
                {
                    BusType.Car => 4,
                    BusType.Van => 6,
                    BusType.Bus => 10,
                };
            }
        }
        
        public void LeaveTheCity()
        {
            BoxAnimator.SetTrigger("close");
            MinionHolder.DOLocalMove(minionLocalPosition + Vector3.down * 0.078f, 0.6f);
            var shipper=JunkPile.ins.GetShipper();
            CurrentParkSlot.ReleaseBus();
            shipper.Carry(this, () =>
            {

                ForceLeaveTheCity();
                /*shipper.AttachBus(this, () =>
                {
                    shipper.LeaveTheCity(this, () =>
                    {
                        JunkPile.ins.Push(this);
                    });

                    /*
                    var (path, pathLength) = ParkingLot.cur.GenerateExitPath(CurrentParkSlot);
                    #1#
                    CurrentParkSlot.ReleaseBus();
                    for (int i = 0; i < currentMinions.Count; i++) JunkPile.ins.Push(currentMinions[i]);
                    currentMinions.Clear();
                    /*
                    JunkPile.ins.Push(this);
                    #1#
                    /*moveTween = transform.DOPath(path, Config.cur.busMoveSpeed *0.7f).SetLookAt(0.18f * (Config.cur.averageBusLeavePathLength / pathLength), Vector3.back, Vector3.up).SetSpeedBased(true).SetEase(Ease.Linear)
                       .OnComplete(() =>
                       {
                           transform.LookAtSupreme(ParkingLot.cur.BusExitPoint);
                           moveTween = transform.DOMove(ParkingLot.cur.BusExitPoint, Config.cur.busMoveSpeed*0.7f).SetSpeedBased(true).SetEase(Ease.Linear)
                               .OnComplete(() =>
                               {

                               });
                       }).SetDelay(0.5f);
                       #1#




                });*/
               
            });
            
            
            return;
            foreach (var minion in currentMinions)
            {
                minion.SetAnim(Minion.ANIM_SEAT);
            }

            Timer.Schedule(this, 0.7f, () =>
            {
                
            });
            
        }

        public void MinionOnBoard(Minion minion, Action onSitDown = null)
        {
            if (CurrentMinionNum >= maxMinions) return;
            currentMinions.Add(minion);
            CurrentMinionNum++;

            var index = CurrentMinionNum - 1;
            var isLastMinionOnBoard = !IsEmptySeat();

            minion.SetAnim(Minion.ANIM_RUN);
            minion.InitOnboardRotation();
    
            /*
            var path = ParkingLot.cur.GetMinionPath(this, minion.LineFlag);
            */
            var seatPoint = transform.TransformPoint(bakedSeatPosList[index]);
            minion.transform.SetParent(MinionHolder);
            var localPoint = MinionHolder.InverseTransformPoint(seatPoint);
            minion.transform.DOScale(minion.OnBusScale, 0.45f).SetEase(Ease.InQuad);
            minion.CurrentMoveToBusTween = minion.transform
                .DOLocalJump( localPoint, 0.18f,1,0.45f/*Config.cur.minionMoveSpeed*/)/*.SetSpeedBased(true)*//*.SetLookAt(.05f).SetEase(Ease.Linear)*/
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    /*
                    transform.localScale = RootScale * onParkScaleFactor;
                    */
                    /*
                    minion.SetAnim(Minion.ANIM_SEAT);
                    */
                    minion.transform.localScale = minion.OnBusScale;
          
                    minion.transform.localRotation=Quaternion.identity;
                    
                    minion.IsOnBoard = true;
                    
                    AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                    AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                    PopOnMinionAbroad(); // already set to root scale
                    DOTween.Complete(transform);
                    transform.DOPunchScale(0.05f*transform.localScale,0.1f);
                    /*
                    if (isLastMinionOnBoard) CheckReadyForGoing();
                    */
                    onSitDown?.Invoke();
                });
            Timer.Schedule(this, 0.45f, () =>
            {
                if (isLastMinionOnBoard) CheckReadyForGoing();
                onSitDown?.Invoke();
            });
        }

        private void PopOnMinionAbroad()
        {
            /*popTween?.Complete();
            popTween = transform.DOPunchScale(RootScale * .12f * onParkScaleFactor, .18f, 1, 0);*/
        }

        public BusColorData GetColorData()
        {
            if (Type == BusType.Bus)
            {
                var busMinionNum = (IsBusCarPartTaken ? 0 : 4) + (IsBusVanPartTaken ? 0 : 6);
                return new BusColorData() { color = color, num = busMinionNum };
            }
            else
            {
                return new BusColorData() { color = color, num = maxMinions };
            }
        }

        public BusColorDataWithBackTrack GetColorDataWithBackTrack()
        {
            if (Type == BusType.Bus)
            {
                var busMinionNum = (IsBusCarPartTaken ? 0 : 4) + (IsBusVanPartTaken ? 0 : 6);
                return new BusColorDataWithBackTrack() { color = color, num = busMinionNum, backTrackList = new List<Bus>(backTrackNodeList) };
            }
            else
            {
                return new BusColorDataWithBackTrack() { color = color, num = maxMinions, backTrackList = new List<Bus>(backTrackNodeList) };
            }
        }

        public BusColorData GetRequireColorData()
        {
            return new BusColorData() { color = color, num = maxMinions - CurrentMinionNum };
        }

        public int GetTrueMinionNum()
        {
            if (Type == BusType.Bus)
            {
                var busMinionNum = (IsBusCarPartTaken ? 0 : 4) + (IsBusVanPartTaken ? 0 : 6);
                // Debug.Log("TRUE MINION NUM " + busMinionNum + "  " + IsBusCarPartTaken + "   " + IsBusVanPartTaken);
                return busMinionNum;
            }

            return maxMinions;
        }

        public void TurnToCovered()
        {
            if(IsGrayBus) return;
            JunkPile.ins.ChangeColor(this,color,true);
            arrowObject.GetComponentInChildren<SpriteRenderer>().DOKill();
            arrowObject.GetComponentInChildren<SpriteRenderer>().color = JunkPile.ins.CoverArrowColor;
        }

        public void ForceUnCoverd()
        {
            arrowObject.GetComponentInChildren<SpriteRenderer>().DOKill();
            arrowObject.GetComponentInChildren<SpriteRenderer>().color = JunkPile.ins.NormalArrowColor;
        }
        public void TurnToUnCovered()
        {
            if(IsGrayBus) return;
            IsCoveredBus = false;
            /*
            transform.DOPunchScale(transform.localScale*0.05f, 0.15f);
            */
            var oldMat = JunkPile.ins.GetBusCoverColorByJunkColor(color);
            var target = JunkPile.ins.GetBusColorByJunkColor(color);
            var newMat = new Material(oldMat);
            JunkPile.ins.ChangeBusMat(this,newMat);
            var duration = 0.5f;
            newMat.DOColor(target.color, duration).OnComplete(() =>
            {
                if (IsCoveredBus == false)
                {
                    JunkPile.ins.ChangeBusMat(this,target);
                }
            });
            newMat.DOFloat(target.GetFloat("_Metallic"), "_Metallic", duration);
            newMat.DOFloat(target.GetFloat("_Smoothness"), "_Smoothness", duration);
            arrowObject.GetComponentInChildren<SpriteRenderer>().DOColor(JunkPile.ins.NormalArrowColor, duration);
            Debug.Log("to Uncovered");
        }
        
        public void TurnToGray()
        {
            IsGrayBus = true;
            BusStation.cur.groundBus.Remove(this);

            JunkPile.ins.ChangeToGray(this, color);

            arrowObject.SetActive(false);
            questionObject.SetActive(true);
        }

        public void TurnBackNormalFromGray()
        {
            IsGrayBus = false;

            if (!BusStation.cur.groundBus.Contains(this)) BusStation.cur.groundBus.Add(this);

            grayTween = DOVirtual.Float(0, 1, .64f, (fuk) => CurrentGrayMat.SetFloat(Variables.GRAY_PER, fuk))
                .OnComplete(() =>
                {
                    JunkPile.ins.ChangeColor(this, color);
                    CurrentGrayMat = null;
                });

            markTween = markTrans.DOScale(Vector3.zero, .32f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    arrowObject.SetActive(true);
                    questionObject.SetActive(false);
                    markTween = markTrans.DOScale(markRootScale, .32f).SetEase(Ease.OutSine);
                });
        }
    }
}