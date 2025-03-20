using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class TransportCenter : MonoBehaviour
    {
        public static TransportCenter cur;

        public bool IsMinionTransporting { get; private set; }
        public bool IsBusLeaving { get; private set; }
        public bool IsGamePlaying { get; set; }
        public List<Bus> BusOnDutyQueue { get; set; } = new List<Bus>();
        public Queue<Bus> BusLeaveQueue { get; set; } = new Queue<Bus>();

        private readonly WaitUntil waitUntilMinionStopMoving = new WaitUntil(() => !ParkingLot.cur.IsMinionQueueMoving);
        private readonly WaitUntil waitUntilFirstMinionStopMoving = new WaitUntil(() => !ParkingLot.cur.IsFirstMinionQueueMoving);
        private readonly WaitUntil waitUntilSecondMinionStopMoving = new WaitUntil(() => !ParkingLot.cur.IsSecondMinionQueueMoving);
        private readonly WaitUntil waitUntilThirdMinionStopMoving = new WaitUntil(() => !ParkingLot.cur.IsThirdMinionQueueMoving);

        private Coroutine minionTransportCor;
        private int barrierIndex;

        private bool isTriggerOnTransporting;
        [SerializeField] private Material _covMaterial;
        [SerializeField]private float _length = -1000;
        private void Awake()
        {
            cur = this;
        }

        public bool IsTimeForBooster()
        {
            if (!IsGamePlaying) return false;
            if (BusStation.cur.CurrentVipBus != null) return false;
            var parkList = ParkingLot.cur.parkSlots;
            for (int i = 0; i < parkList.Length; i++)
            {
                if (parkList[i].IsSlotTaken && !parkList[i].IsBusArrive) return false;
            }

            return !IsMinionTransporting;
        }

        public bool IsNoCarOnDuty()
        {
            return BusOnDutyQueue.Count == 0;
        }

        private IEnumerator IeUpdateMat()
        {
            while (true)
            {
                if (IsMinionTransporting)
                {
                    _covMaterial.SetFloat("_Length",_length);
                    _length += Time.deltaTime;
                }
                yield return null;
            }
        }
        public void MinionEnter() //call after added minion
        {
            if (ParkingLot.cur.isSingleLine)
            {
                var queue = ParkingLot.cur.MinionsQueue;
                if (queue.Count == 0) return;

                IsGamePlaying = false;

                var addNum = ParkingLot.cur.GetStandPosNum() + 1;
                foreach (var minion in queue)
                {
                    minion.AddQueueIndex(addNum);
                }

                ParkingLot.cur.MoveQueue(0);

                StartCoroutine(ie_Wait());
            }
            else
            {
                var queue_1 = ParkingLot.cur.FirstMinionQueue;
                var queue_2 = ParkingLot.cur.SecondMinionQueue;
                var queue_3 = ParkingLot.cur.ThirdMinionQueue;

                if (queue_1.Count == 0 && queue_2.Count == 0 && queue_3.Count == 0) return;

                IsGamePlaying = false;

                var addNum = ParkingLot.cur.GetMultiLineStandPosNum() + 1;
                foreach (var minion in queue_1)
                {
                    minion.AddQueueIndex(addNum);
                }

                foreach (var minion in queue_2)
                {
                    minion.AddQueueIndex(addNum);
                }

                foreach (var minion in queue_3)
                {
                    minion.AddQueueIndex(addNum);
                }

                ParkingLot.cur.MoveQueue(0, 0);
                ParkingLot.cur.MoveQueue(0, 1);
                ParkingLot.cur.MoveQueue(0, 2);

                StartCoroutine(ie_MultiWait());
            }

            return;

            IEnumerator ie_Wait()
            {
                while (true)
                {
                    ParkingLot.cur.JustMoveMinion(1.42f);

                    yield return waitUntilMinionStopMoving;
                    var minion = ParkingLot.cur.GetCurrentMinion();
                    if (minion.QueueIndex == 0) break;
                }

                ParkingLot.cur.MoveQueue(1);
                IsGamePlaying = true;
            }

            IEnumerator ie_MultiWait()
            {
                while (true)
                {
                    ParkingLot.cur.JustMoveMinion(1.42f, 0);
                    ParkingLot.cur.JustMoveMinion(1.42f, 1);
                    ParkingLot.cur.JustMoveMinion(1.42f, 2);

                    yield return waitUntilFirstMinionStopMoving;
                    var minion = ParkingLot.cur.GetCurrentMinion(0);
                    if (minion.QueueIndex == 0) break;
                }

                yield return null;
                yield return null;
                yield return null;

                ParkingLot.cur.MoveQueue(1, 0);
                ParkingLot.cur.MoveQueue(1, 1);
                ParkingLot.cur.MoveQueue(1, 2);
                IsGamePlaying = true;
            }
        }

        public void StartMinionTransport(Bus bus = null)
        {
            if (bus != null) BusOnDutyQueue.Enqueue(bus);

            if (IsMinionTransporting)
            {
                isTriggerOnTransporting = true;
                return;
            }

            IsMinionTransporting = true;
            if (ParkingLot.cur.isSingleLine)
            {
                minionTransportCor = StartCoroutine(ie_Transport());
            }
            else
            {
                if (ParkingLot.cur.isBarrier)
                {
                    minionTransportCor = StartCoroutine(ie_MultiTransportWithBarrierContinue());
                }
                else
                {
                    minionTransportCor = StartCoroutine(ie_MultiTransport());
                }
            }

            return;

            IEnumerator ie_Transport()
            {
                var parkingLot = ParkingLot.cur; // xe vừa vào slot
                while (true)
                {
                    if (parkingLot.MinionsQueue.Count == 0) // ko có xe thì ko di chuyển người
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    var minion = parkingLot.GetCurrentMinion(); // lấy minion đầu hàng
                    if (!GetFittingBus(minion, out var fittingBus)) // nếu ko có xe phù hơpj thì ngừng di chuyển
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    parkingLot.PopMinion(); // lấy minion đầu tiên ra khỏi hàng

                    fittingBus.MinionOnBoard(minion); // di chuyển minion đến xe
                    parkingLot.ReleaseMinionPostHandle(); // di chuyển các minion còn lại đến vị trí mới

                    yield return waitUntilMinionStopMoving;

                    Callout(); // fill các minion mới vào chỗ trống
                }
                parkingLot.MoveQueue(1);
                CheckEndGameStatus();
                IsMinionTransporting = false;
            }

            IEnumerator ie_MultiTransport()
            {
                var parkingLot = ParkingLot.cur;
                while (true)
                {
                    if (parkingLot.IsAllLineEmpty())
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    int unfittingNum = 0;

                    //  0
                    var firstMinion = parkingLot.GetCurrentMinion(0);
                    yield return waitUntilFirstMinionStopMoving;
                    if (parkingLot.FirstMinionQueue.Count > 0 && GetFittingBus(firstMinion, out var firstFittingBus))
                    {
                        parkingLot.PopMinion(0);

                        firstFittingBus.MinionOnBoard(firstMinion);
                        parkingLot.ReleaseMinionPostHandle(0);
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    //  1
                    var secondMinion = parkingLot.GetCurrentMinion(1);
                    yield return waitUntilSecondMinionStopMoving;
                    if (parkingLot.SecondMinionQueue.Count > 0 && GetFittingBus(secondMinion, out var secondFittingBus))
                    {
                        parkingLot.PopMinion(1);

                        secondFittingBus.MinionOnBoard(secondMinion);
                        parkingLot.ReleaseMinionPostHandle(1);
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    //  2
                    var thirdMinion = parkingLot.GetCurrentMinion(2);
                    yield return waitUntilThirdMinionStopMoving;
                    if (parkingLot.ThirdMinionQueue.Count > 0 && GetFittingBus(thirdMinion, out var thirdFittingBus))
                    {
                        parkingLot.PopMinion(2);

                        thirdFittingBus.MinionOnBoard(thirdMinion);
                        parkingLot.ReleaseMinionPostHandle(2);
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    // when waiting for last one to check if fitting or not, a bus might enter park and have a suitable minion on line 1 but when last line is done moving and cant find fitting bus, the transporting is stop without check for the new bus which have suitable minion on first line
                    if (isTriggerOnTransporting)
                    {
                        isTriggerOnTransporting = false;
                        unfittingNum = 0;
                    }

                    if (unfittingNum >= 3)
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    Callout();
                }

                parkingLot.MoveQueue(1, 0);
                parkingLot.MoveQueue(1, 1);
                parkingLot.MoveQueue(1, 2);
                CheckEndGameStatus();
                IsMinionTransporting = false;
            }

            IEnumerator ie_MultiTransportWithBarrier()
            {
                var parkingLot = ParkingLot.cur;
                while (true)
                {
                    if (parkingLot.IsAllLineEmpty())
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    int unfittingNum = 0;

                    //  0
                    var firstMinion = parkingLot.GetCurrentMinion(0);
                    Debug.Log("CHECK 0");
                    if (GetTrueIndex() == 0 && parkingLot.FirstMinionQueue.Count > 0 && GetFittingBus(firstMinion, out var firstFittingBus))
                    {
                        Debug.Log("RUN 0");
                        parkingLot.PopMinion(0);

                        firstFittingBus.MinionOnBoard(firstMinion);
                        parkingLot.ReleaseMinionPostHandle(0);
                        yield return waitUntilFirstMinionStopMoving;
                        Debug.Log("STOP 0");
                        IncreaseBarrier();
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    //  1
                    var secondMinion = parkingLot.GetCurrentMinion(1);
                    Debug.Log("CHECK 1");
                    if (GetTrueIndex() == 1 && parkingLot.SecondMinionQueue.Count > 0 && GetFittingBus(secondMinion, out var secondFittingBus))
                    {
                        Debug.Log("RUN 1");
                        parkingLot.PopMinion(1);

                        secondFittingBus.MinionOnBoard(secondMinion);
                        parkingLot.ReleaseMinionPostHandle(1);
                        yield return waitUntilSecondMinionStopMoving;
                        Debug.Log("STOP 1");
                        IncreaseBarrier();
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    //  2
                    var thirdMinion = parkingLot.GetCurrentMinion(2);
                    Debug.Log("CHECK 2");
                    if (GetTrueIndex() == 2 && parkingLot.ThirdMinionQueue.Count > 0 && GetFittingBus(thirdMinion, out var thirdFittingBus))
                    {
                        Debug.Log("RUN 2");
                        parkingLot.PopMinion(2);

                        thirdFittingBus.MinionOnBoard(thirdMinion);
                        parkingLot.ReleaseMinionPostHandle(2);
                        yield return waitUntilThirdMinionStopMoving;
                        Debug.Log("STOP 2");
                        IncreaseBarrier();
                    }
                    else
                    {
                        unfittingNum++;
                    }

                    // may be not necessary
                    // if (isTriggerOnTransporting)
                    // {
                    //     isTriggerOnTransporting = false;
                    //     unfittingNum = 0;
                    // }

                    if (unfittingNum >= 3)
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    Callout();
                }

                parkingLot.MoveQueue(1, 0);
                parkingLot.MoveQueue(1, 1);
                parkingLot.MoveQueue(1, 2);
                CheckEndGameStatus();
                IsMinionTransporting = false;
            }

            IEnumerator ie_MultiTransportWithBarrierContinue()
            {
                var parkingLot = ParkingLot.cur;
                bool isMoved = false;
                while (true)
                {
                    if (parkingLot.IsAllLineEmpty())
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    int unfittingNum = 0;

                    //  0
                    var firstMinion = parkingLot.GetCurrentMinion(0);
                    Debug.Log("CHECK 0");
                    if (GetTrueIndex() == 0 && parkingLot.FirstMinionQueue.Count > 0 && GetFittingBus(firstMinion, out var firstFittingBus))
                    {
                        Debug.Log("RUN 0");
                        isMoved = true;
                        parkingLot.PopMinion(0);

                        firstFittingBus.MinionOnBoard(firstMinion);
                        parkingLot.ReleaseMinionPostHandle(0);
                        yield return waitUntilFirstMinionStopMoving;
                        Debug.Log("STOP 0");
                    }
                    else
                    {
                        if ((GetTrueIndex() == 0 && isMoved))
                        {
                            Debug.Log("MOVE 0");
                            isMoved = false;
                            IncreaseBarrier();
                        }
                        else
                        {
                            Debug.Log("NOT MOVE 0");
                            if (GetTrueIndex() == 0 && parkingLot.FirstMinionQueue.Count == 0)
                            {
                                Debug.Log("EMPTY 0");
                                IncreaseBarrier();
                            }

                            unfittingNum++;
                        }
                    }

                    //  1
                    var secondMinion = parkingLot.GetCurrentMinion(1);
                    Debug.Log("CHECK 1");
                    if (GetTrueIndex() == 1 && parkingLot.SecondMinionQueue.Count > 0 && GetFittingBus(secondMinion, out var secondFittingBus))
                    {
                        Debug.Log("RUN 1");
                        isMoved = true;
                        parkingLot.PopMinion(1);

                        secondFittingBus.MinionOnBoard(secondMinion);
                        parkingLot.ReleaseMinionPostHandle(1);
                        yield return waitUntilSecondMinionStopMoving;
                        Debug.Log("STOP 1");
                    }
                    else
                    {
                        if ((GetTrueIndex() == 1 && isMoved))
                        {
                            Debug.Log("MOVE 1");
                            isMoved = false;
                            IncreaseBarrier();
                        }
                        else
                        {
                            Debug.Log("NOT MOVE 1");
                            if (GetTrueIndex() == 1 && parkingLot.SecondMinionQueue.Count == 0)
                            {
                                Debug.Log("EMPTY 1");
                                IncreaseBarrier();
                            }

                            unfittingNum++;
                        }
                    }

                    //  2
                    var thirdMinion = parkingLot.GetCurrentMinion(2);
                    Debug.Log("CHECK 2");
                    if (GetTrueIndex() == 2 && parkingLot.ThirdMinionQueue.Count > 0 && GetFittingBus(thirdMinion, out var thirdFittingBus))
                    {
                        Debug.Log("RUN 2");
                        isMoved = true;
                        parkingLot.PopMinion(2);

                        thirdFittingBus.MinionOnBoard(thirdMinion);
                        parkingLot.ReleaseMinionPostHandle(2);
                        yield return waitUntilThirdMinionStopMoving;
                        Debug.Log("STOP 2");
                    }
                    else
                    {
                        if ((GetTrueIndex() == 2 && isMoved))
                        {
                            Debug.Log("MOVE 2");
                            isMoved = false;
                            IncreaseBarrier();
                        }
                        else
                        {
                            Debug.Log("NOT MOVE 2");
                            if (GetTrueIndex() == 2 && parkingLot.ThirdMinionQueue.Count == 0)
                            {
                                Debug.Log("EMPTY 2");
                                IncreaseBarrier();
                            }

                            unfittingNum++;
                        }
                    }

                    // may be not necessary
                    if (isTriggerOnTransporting)
                    {
                        isTriggerOnTransporting = false;
                        unfittingNum = 0;
                    }

                    if (unfittingNum >= 3)
                    {
                        IsMinionTransporting = false;
                        break;
                    }

                    Callout();
                }

                parkingLot.MoveQueue(1, 0);
                parkingLot.MoveQueue(1, 1);
                parkingLot.MoveQueue(1, 2);
                CheckEndGameStatus();
                IsMinionTransporting = false;
            }
        }

        private void IncreaseBarrier()
        {
            barrierIndex++;
            // Debug.Log("INCREASE " + barrierIndex);
            ParkingLot.cur.SetLight(barrierIndex);
        }

        public int GetTrueIndex()
        {
            var length = ParkingLot.cur.isThirdLineOpen ? 3 : 2;
            // Debug.Log("GET INDEX " + barrierIndex % length);
            return barrierIndex % length;
        }

        public void PauseMinionTransport()
        {
        }

        public void StopMinionTransport()
        {
            if (minionTransportCor != null) StopCoroutine(minionTransportCor);
            IsMinionTransporting = false;
            BusOnDutyQueue.Clear();
        }

        public bool GetFittingBus(Minion minion, out Bus bus)
        {
            bus = null;
            if (BusOnDutyQueue.Count == 0) return false;
            foreach (var b in BusOnDutyQueue)
            {
                if (b.color == minion.Color && b.IsEmptySeat())
                {
                    bus = b;
                    return true;
                }
            }

            return false;
        }

        public void RequireLeavePermit(Bus bus)
        {
            BusLeaveQueue.Enqueue(bus);
            BusOnDutyQueue.Remove(bus);
            if (IsBusLeaving) return;
            IsBusLeaving = true;
            StartCoroutine(ie_Leave());
            return;

            IEnumerator ie_Leave()
            {
                while (BusLeaveQueue.Count > 0)
                {
                    yield return Yielders.Get(Config.cur.delayBetweenBusLeave);
                    var b = BusLeaveQueue.Dequeue();
                    b.LeaveTheCity();
                }

                IsBusLeaving = false;
                yield break;
            }
        }

        public void StopBusLeavingProcess()
        {
            BusLeaveQueue.Clear();
            IsBusLeaving = false;
        }

        public void InitializeMap()
        {
            _length = -1000;
            _covMaterial.SetFloat("_Length",_length);
            ResetTriggerSpecialCar();

            StopMinionTransport();
            StopBusLeavingProcess();

            StopAllCoroutines();
            BusStation.cur.StopAllCoroutines();
            StartCoroutine(IeUpdateMat());

            IsGamePlaying = true;

            BusStation.cur.AwaitForFetchDataBus.Clear();
            BusStation.cur.PenaltyColorData.Clear();
            BusStation.cur.AwaitBusDatas.Clear();
            BusStation.cur.IsVipBus = false;
            BusStation.cur.CurrentVipBus = null;
            BusStation.cur.MovedBus = 0;

            Callout();
            Callout(); // the two after is for multi line fill to make sure all the line is full
            Callout();
            ParkingLot.cur.SetMinionToQueueLerp(1f);
            /*
            ParkingLot.cur.MoveQueue(1);
            */
            ParkingLot.cur.SetupMap();

            barrierIndex = 0;
            isTriggerOnTransporting = false;

            /*
            MinionEnter();
            */
            
        }

        public void NukeMap()
        {
            IsGamePlaying = false;

            StopMinionTransport();
            StopBusLeavingProcess();

            StopAllCoroutines();
        }

        public void Callout()
        {
            if (ParkingLot.cur.AwaitMinions.Count > 0) return;
            if (BusStation.cur.CallOfDuty(out var menAtWar))
            {
                ParkingLot.cur.AddMinionToQueue(menAtWar);
            }
        }

        public void CheckEndGameStatus()
        {
            if (!IsGamePlaying) return;
            if (ParkingLot.cur.isSingleLine)
            {
                if (ParkingLot.cur.MinionsQueue.Count == 0)
                {
                    IsGamePlaying = false;
                    WinHandle();
                    return;
                }
            }
            else
            {
                if (ParkingLot.cur.IsAllLineEmpty())
                {
                    IsGamePlaying = false;
                    WinHandle();
                    return;
                }
            }

            // Debug.Log("CHECK LOSE");

            if (ParkingLot.cur.isSingleLine)
            {
                var targetColor = ParkingLot.cur.MinionsQueue.Peek().Color;
                var parkSlots = ParkingLot.cur.parkSlots;
                for (int i = 0; i < parkSlots.Length; i++)
                {
                    var slot = parkSlots[i];
                    // Debug.Log("CHECK " + i + "  " + (!slot.IsSlotTaken));
                    // if (slot.AssignedBus != null) Debug.Log("CHECK " + i + "   " + (slot.AssignedBus.color == targetColor) + "   " + (slot.AssignedBus.IsEmptySeat()));
                    if (slot.IsAdsSlot) continue;
                    if (slot.IsSlotTaken && !slot.IsBusArrive) return;
                    if (slot.IsVipSlot && slot.AssignedBus == null) continue;
                    if (!slot.IsSlotTaken || (slot.AssignedBus.color == targetColor && slot.AssignedBus.IsEmptySeat())) return;
                    if (slot.IsSlotTaken && !slot.AssignedBus.IsEmptySeat()) return;
                }

                IsGamePlaying = false;
                // Debug.Log("LOSE");
                LoseHandle();
            }
            else if (ParkingLot.cur.isBarrier)
            {
                for (int j = 0; j < 3; j++)
                {
                    var queue = GetQueue(j);
                    if (queue.Count == 0 || GetTrueIndex() != j) continue;
                    var targetColor = queue.Peek().Color;
                    var parkSlots = ParkingLot.cur.parkSlots;
                    for (int i = 0; i < parkSlots.Length; i++)
                    {
                        var slot = parkSlots[i];
                        // Debug.Log("CHECK " + i + "  " + (!slot.IsSlotTaken));
                        // if (slot.AssignedBus != null) Debug.Log("CHECK " + i + "   " + (slot.AssignedBus.color == targetColor) + "   " + (slot.AssignedBus.IsEmptySeat()));
                        if (slot.IsAdsSlot) continue;
                        if (slot.IsSlotTaken && !slot.IsBusArrive) return;
                        if (slot.IsVipSlot && slot.AssignedBus == null) continue;
                        if (!slot.IsSlotTaken || (slot.AssignedBus.color == targetColor && slot.AssignedBus.IsEmptySeat())) return;
                        if (slot.IsSlotTaken && !slot.AssignedBus.IsEmptySeat()) return;
                    }
                }

                IsGamePlaying = false;
                // Debug.Log("LOSE");
                LoseHandle();
            }
            else
            {
                for (int j = 0; j < 3; j++)
                {
                    var queue = GetQueue(j);
                    if (queue.Count == 0) continue;
                    var targetColor = queue.Peek().Color;
                    var parkSlots = ParkingLot.cur.parkSlots;
                    for (int i = 0; i < parkSlots.Length; i++)
                    {
                        var slot = parkSlots[i];
                        // Debug.Log("CHECK " + i + "  " + (!slot.IsSlotTaken));
                        // if (slot.AssignedBus != null) Debug.Log("CHECK " + i + "   " + (slot.AssignedBus.color == targetColor) + "   " + (slot.AssignedBus.IsEmptySeat()));
                        if (slot.IsAdsSlot) continue;
                        if (slot.IsSlotTaken && !slot.IsBusArrive) return;
                        if (slot.IsVipSlot && slot.AssignedBus == null) continue;
                        if (!slot.IsSlotTaken || (slot.AssignedBus.color == targetColor && slot.AssignedBus.IsEmptySeat())) return;
                        if (slot.IsSlotTaken && !slot.AssignedBus.IsEmptySeat()) return;
                    }
                }

                IsGamePlaying = false;
                // Debug.Log("LOSE");
                LoseHandle();
            }

            return;

            Queue<Minion> GetQueue(int index)
            {
                return index switch
                {
                    0 => ParkingLot.cur.FirstMinionQueue,
                    1 => ParkingLot.cur.SecondMinionQueue,
                    2 => ParkingLot.cur.ThirdMinionQueue,
                };
            }
        }

        private static readonly WaitUntil condition = new WaitUntil(() => !cur.IsBusLeaving);

        public void WinHandle()
        {
            EmojiPopping.Activate(false);
            StartCoroutine(ie_Wait());
            return;

            IEnumerator ie_Wait()
            {
                yield return condition;
                yield return Yielders.Get(2.48f);

                const string key = "RATING_END_LEVEL";
                if (!PlayerPrefs.HasKey(key) && LevelDataFragment.cur.gameData.level == AdsManager.Ins.RatingLevel)
                {
                    PlayerPrefs.SetInt(key, 2710);
                    UIManager.ins.OpenUI<CanvasRating>();

                    yield return new WaitUntil(() => !CanvasRating.IsOpen);
                    yield return Yielders.Get(0.64f);
                }

                UIManager.ins.OpenUI<CanvasVictory>();
            }
        }

        public void LoseHandle()
        {
            EmojiPopping.Activate(false);
            StartCoroutine(ie_Wait());
            return;

            IEnumerator ie_Wait()
            {
                yield return Yielders.Get(.38f);
                CanvasGamePlay.PopOutOfSlotNoffStatic(CanvasGamePlay.OUT_OF_SLOT);

                yield return Yielders.Get(1.72f);

                UIManager.ins.OpenUI<CanvasLose>();
            }
        }

        public void ReviveTransport()
        {
            IsGamePlaying = true;

            ParkingLot.cur.UnlockPossibleAdsParkSlot();
            EmojiPopping.Activate(true);

            BusStation.cur.BoosterMinionsMix();

            var level = LevelDataFragment.cur.GetFireBaseLevel().ToString();
            FirebaseManager.Ins.g_gameplay_booster(level, "REVIVE");
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private const int ambulancePenaltyCount = 6;

        private const int fireTruckPenaltyCount = 10;

        private int ambulanceNum;
        private int fireTruckNum;

        private void ResetTriggerSpecialCar()
        {
            ambulanceNum = 0;
            fireTruckNum = 0;
        }

        public void TriggerAmbulance(Minion minion)
        {
            if (ambulanceNum == 0)
            {
                ambulanceNum = ambulancePenaltyCount;

                var variant = JunkPile.ins.GetCountDownNote();
                variant.Fetch(minion);
            }

            ambulanceNum--;
        }

        public void TriggerFireTruck()
        {
            if (fireTruckNum == 0)
            {
                fireTruckNum = fireTruckPenaltyCount;

                // var 
            }
        }
    }
}