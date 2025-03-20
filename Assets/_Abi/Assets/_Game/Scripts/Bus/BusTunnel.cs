using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusTunnel : MonoBehaviour
    {
        public new Transform transform;
        public new GameObject gameObject;

        public bool IsPushed { get; set; }
        public bool IsOutOfData { get; set; }

        public Transform busParkTrans;
        public Transform busbusTrans;
        public Transform busvanTrans;
        public Transform buscarTrans;
        
        public Transform busInitTrans;

        [SerializeField] private Bus targetLockBus;
        public Bus CurrentLockBus { get; private set; }
        private Tween moveBusTween;
        private Tween scaleBusTween;

        public List<TunnelBusData> awaitBuses;
        public int AwaitDataCount => awaitBuses.Count;
        private readonly List<Bus> linkedBusList = new List<Bus>();

        [SerializeField] private TMP_Text _countText;  
        [SerializeField] private Vector3 targetBarrierRotate;
        private Tween barrierTween;

        private List<Bus> blockBusList;

        private bool isSignalAccepted;

        [SerializeField] private Transform _door;
        [SerializeField] private Vector3 _rootDoorLocalPosition;
        [SerializeField] private Transform _bandgeTf;
        
#if UNITY_EDITOR
        [Header("EDITOR ONLY")] [SerializeField]
        private Vector3 gizmoOffsetFromCenter;

        [SerializeField] private Vector3 gizmoSize;

        private void OnDrawGizmos()
        {
            if (busParkTrans != null)
            {
                /*Gizmos.color = Color.green;
                var matrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(gizmoOffsetFromCenter, gizmoSize);
                Gizmos.matrix = matrix;*/
            }
        }

        [ContextMenu("FETCH LOCK BUS")]
        public void FetchLockBus()
        {
            if (targetLockBus != null) CurrentLockBus = targetLockBus;
        }
#endif
        private void Awake()
        {
            _rootDoorLocalPosition = _door.localPosition;
        }

        public void OnInit()
        {
            _door.localPosition = _rootDoorLocalPosition;
            linkedBusList.Clear();

            isSignalAccepted = false;
        }

        public void OnPush()
        {
            moveBusTween?.Kill();
            scaleBusTween?.Kill();
            barrierTween?.Kill();
        }

        public TunnelDataPack ExtractDataPack()
        {
            blockBusList = new List<Bus>(CurrentLockBus.directBackNodeList);
            return new TunnelDataPack()
            {
                datas = awaitBuses,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                lockBusIndex = GetBusIndex(),
                blockBusIndexList = GetIndexList(blockBusList),
            };

            int GetBusIndex()
            {
                var list = BusStation.cur.activeBus;
                return list.IndexOf(CurrentLockBus);
            }

            int GetSpecificBusIndex(Bus bus)
            {
                var list = BusStation.cur.activeBus;
                return list.IndexOf(bus);
            }

            List<int> GetIndexList(List<Bus> buses)
            {
                var list = new List<int>();
                for (int i = 0; i < buses.Count; i++)
                {
                    list.Add(GetSpecificBusIndex(buses[i]));
                }

                return list;
            }
        }

        public void FetchData(TunnelDataPack dataPack)
        {
            transform.position = dataPack.position;
            transform.rotation = dataPack.rotation;
            transform.localScale = dataPack.scale;
            
            _bandgeTf.LookAt(Camera.main.transform);

          
            
            CurrentLockBus = BusStation.cur.activeBus[dataPack.lockBusIndex];
            awaitBuses = new List<TunnelBusData>(dataPack.datas);

            _countText.text= awaitBuses.Count.ToString();


            var list = dataPack.blockBusIndexList;
            for (int i = 0; i < list.Count; i++)
            {
                var bus = BusStation.cur.activeBus[list[i]];
                linkedBusList.Add(bus);
            }

            IsOutOfData = false;
        }

        private void DecreaseWeight()
        {
            for (int i = 0; i < linkedBusList.Count; i++)
            {
                var bus = linkedBusList[i];
                if (bus.Weight > 0) bus.Weight--;
            }
        }

        public void OnBusMoveOutParkZone()
        {
             Debug.Log("ON BUS MOVE " + (CurrentLockBus == null));
            if (isSignalAccepted) return;
            if (CurrentLockBus == null) return;

            StartCoroutine(ie_PopNewBus());
            return;

        }

        private IEnumerator ie_PopNewBus()
        {
            if (CurrentLockBus.IsStayOnBusZone) yield break;

            isSignalAccepted = true;

            DecreaseWeight();

            if (awaitBuses.Count == 0)
            {
                CurrentLockBus = null;
                yield break;
            }

            barrierTween?.Kill();
            barrierTween = _door.DOLocalMove(_rootDoorLocalPosition + Vector3.down * 0.08f, .28f).SetEase(Ease.Linear);
            yield return Yielders.Get(0.32f);

            var currentNNL = new List<Bus>(CurrentLockBus.nextNodeList);

            CurrentLockBus = ConvertToBus(awaitBuses.Dequeue());
            CurrentLockBus.transform.SetPositionAndRotation(busInitTrans);
            CurrentLockBus.transform.localScale = CurrentLockBus.RootScale * 0.3f * JunkPile.ins.GetScaleFactor();
            CurrentLockBus.nextNodeList = currentNNL;
            CurrentLockBus.IsTouchable = false;
            var time = Time.time;
            var isSpawned = false;
            CurrentLockBus.transform.DOScale(CurrentLockBus.RootScale * JunkPile.ins.GetScaleFactor(), 0.25f);
            var targetTf = busbusTrans;
            if (CurrentLockBus.Type == BusType.Van)
            {
                targetTf = busvanTrans;
            }

            if (CurrentLockBus.Type == BusType.Car)
            {
                targetTf = buscarTrans;
            }

            moveBusTween = CurrentLockBus.transform.DOMove(targetTf.position, .32f).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    CurrentLockBus.IsTouchable = true;
                    CurrentLockBus.RootPosition = CurrentLockBus.transform.position;
                });

            
            isSignalAccepted = false;

            yield return Yielders.Get(0.18f);

            barrierTween = _door.DOLocalMove(_rootDoorLocalPosition, .36f).SetEase(Ease.Linear);
        }

        private void Update()
        {
            if (awaitBuses != null)
            {
                _countText.text= awaitBuses.Count.ToString();
            }
            
        }

        private Bus ConvertToBus(TunnelBusData data)
        {
            // var bus = JunkPile.ins.GetBus(data.busType, data.color, Vector3.zero, Quaternion.identity);
            var bus = JunkPile.ins.IsSpecialType(data.color) ? JunkPile.ins.GetSpecialBus(data.color, Vector3.zero, Quaternion.identity,new Vector3(7.17600012f, 7.17600012f, 7.17600012f)) :
                JunkPile.ins.GetBus(data.busType, data.color, Vector3.zero, Quaternion.identity,new Vector3(7.17600012f, 7.17600012f, 7.17600012f));
            bus.Weight = 0;
            bus.IsBusTaken = data.isTaken;
            bus.IsOutOfTunnel = true;

            AppendBus(bus);

            return bus;
        }

        private void AppendBus(Bus bus)
        {
            BusStation.cur.activeBus.Add(bus);
            BusStation.cur.groundBus.Add(bus);
        }

        public BusColorData HandOutData(bool isTakeFirstOnly)
        {
            if (awaitBuses.Count == 0 || IsOutOfData)
            {
                IsOutOfData = true;
                return new BusColorData();
            }

            if (isTakeFirstOnly)
            {
                var data = awaitBuses[0];
                if (data.isTaken) return new BusColorData();
                data.isTaken = true;
                awaitBuses[0] = data;
                return new BusColorData() { color = data.color, num = GetSeatNum(data.busType) };
            }

            for (int i = 0; i < awaitBuses.Count; i++)
            {
                var data = awaitBuses[i];
                if (data.isTaken) continue;
                data.isTaken = true;
                awaitBuses[i] = data;
                // Debug.DrawRay(transform.position, Vector3.up * 10f, Color.magenta);
                // Debug.Log("EXTRACT COLOR TUNNEL" + data.color + "   " + data.busType);

                return new BusColorData() { color = data.color, num = GetSeatNum(data.busType) };
            }

            IsOutOfData = true;
            return new BusColorData();
        }

        private int GetSeatNum(BusType busType)
        {
            return busType switch
            {
                BusType.Car => 4,
                BusType.Van => 6,
                BusType.Bus => 10,
            };
        }

        public int GetTotalMinionNum()
        {
            int num = 0;
            for (int i = 0; i < awaitBuses.Count; i++)
            {
                num += GetSeatNum(awaitBuses[i].busType);
            }

            return num;
        }
    }

    [Serializable]
    public struct TunnelDataPack
    {
        public List<TunnelBusData> datas;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int lockBusIndex;
        public List<int> blockBusIndexList;
    }

    [Serializable]
    public struct TunnelBusData
    {
        public BusType busType;
        public JunkColor color;
        public bool isTaken;
    }
}