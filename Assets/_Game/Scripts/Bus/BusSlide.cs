using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    public class BusSlide : MonoBehaviour
    {
        public static BusSlide cur;

        public Vector3 checkBoxCenter;
        public Vector3 checkBoxHalfExtent;

        private Vector3 debugCheckBoxCenter;

        public Vector3 edgePointOnMap;
        public Vector2 touchableMinMaxX;

        public new GameObject gameObject;

        public List<Bus> busList;

        public List<SlideBusData> busDataList;

        public Vector3[] anchorPosList;
        public Transform[] movingTransList;

        public int ActiveMovingCount { get; set; }

        [SerializeField] private float movingSpeed = .32f;

        public bool IsActive { get; set; }
        public bool IsPlatformMoving { get; private set; }
        public bool IsBusDone { get; set; }

        private float movingPer;

        private float MovingPer
        {
            get => movingPer;
            set
            {
                movingPer = value;
                slideMat.SetFloat(OFFSET, value);
            }
        }

        private const int BUS_LAYER = 1 << 8;

        public TMP_Text countText;
        private int remainNum;

        [SerializeField] private Material slideMat;
        private static int OFFSET = Shader.PropertyToID("_Offset");

        private void Awake()
        {
            cur = this;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(edgePointOnMap + Vector3.left * 10f, edgePointOnMap + Vector3.right * 10);

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(debugCheckBoxCenter, checkBoxHalfExtent * 2);

            Gizmos.color = Color.red;
            for (int i = 0; i < anchorPosList.Length; i++)
            {
                Gizmos.DrawLine(anchorPosList[i], anchorPosList[i] + Vector3.up * 5f);
            }

            Gizmos.color = Color.magenta;
            var pos = edgePointOnMap + Vector3.back * .16f;
            var pos1 = pos;
            var pos2 = pos;
            pos1.x = touchableMinMaxX.x;
            pos2.x = touchableMinMaxX.y;
            Gizmos.DrawLine(pos1, pos2);
        }

        [ContextMenu("TOGGLE ENABLE")]
        public void ToggleEnable()
        {
            SetActive(!IsActive);
            ClearSlide();
        }
#endif

        private void Update()
        {
            if (!IsPlatformMoving) return;

            Move(MovingPer);

            MovingPer += Time.deltaTime * movingSpeed;
        }

        public void SetActive(bool isOn)
        {
            IsActive = isOn;
            gameObject.SetActive(isOn);
        }

        [ContextMenu("CLEAR SLIDE")]
        public void ClearSlideOnEdit()
        {
            for (int i = 0; i < busList.Count; i++)
            {
                JunkPile.ins.Push(busList[i]);
            }

            ClearSlide();
        }

        public void ClearSlide()
        {
            busList.Clear();

            IsPlatformMoving = false;
            IsBusDone = true;
            MovingPer = 0;

            for (int i = 0; i < movingTransList.Length; i++)
            {
                movingTransList[i].position = anchorPosList[0];
            }

            StopAllCoroutines();
        }

        public void Running()
        {
            IsPlatformMoving = true;
        }

        public void PauseRunning(bool isPause)
        {
            IsPlatformMoving = !isPause;
        }

        public List<SlideBusData> GetData()
        {
            return new List<SlideBusData>(busDataList);
        }

        public void FetchData(List<SlideBusData> dataList)
        {
            ClearSlide();

            var defaultPos = new Vector3(0, -500, 0);

            busDataList = dataList;

            ActiveMovingCount = dataList.Count;
            for (int i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                // var bus = JunkPile.ins.GetBus(data.busType, data.color, defaultPos, Quaternion.identity);
                var bus = JunkPile.ins.IsSpecialType(data.color) ? JunkPile.ins.GetSpecialBus(data.color, defaultPos, Quaternion.identity,
                    new Vector3(7.17600012f, 7.17600012f, 7.17600012f)) : 
                    JunkPile.ins.GetBus(data.busType, data.color, defaultPos, Quaternion.identity,new Vector3(7.17600012f, 7.17600012f, 7.17600012f));
                busList.Add(bus);

                bus.Weight = 999;
                bus.transform.SetParent(movingTransList[i]);
                bus.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

                bus.IsSlideBus = true;
            }

            BusStation.cur.activeBus.AddRange(busList);
            BusStation.cur.groundBus.AddRange(busList); // no changing color for bus on slide now

            remainNum = busList.Count;
            countText.text = remainNum.ToString();
        }

        public void Move(float per)
        {
            for (int i = 0; i < anchorPosList.Length - 1; i++)
            {
                var (index, percent) = TakeIndexAndPer(i);
                var movingTrans = movingTransList[index];
                movingTrans.transform.position = Vector3.Lerp(anchorPosList[i], anchorPosList[i + 1], percent);
            }

            return;

            (int, float) TakeIndexAndPer(int offset)
            {
                var check = per + offset;
                var rawIndex = Mathf.FloorToInt(check);
                int curIndex = rawIndex % ActiveMovingCount;

                var curPer = 1 - (check - rawIndex);

                return (curIndex, curPer);
            }
        }

        private readonly WaitUntil waitForBusDone = new WaitUntil(() => cur.IsBusDone);

        public bool WantToMove(Bus bus) // return if move-able
        {
            if (!IsPlatformMoving || !IsTouchable() || !IsBusDone) return false;
            PauseRunning(true);
            IsBusDone = false;

            var checkPoint = new Vector3(bus.transform.position.x, checkBoxCenter.y, checkBoxCenter.z);
            debugCheckBoxCenter = checkPoint;
            var list = Physics.OverlapBox(checkPoint, checkBoxHalfExtent, Quaternion.identity, BUS_LAYER);
            var nnl = new List<Bus>();
            for (int i = 0; i < list.Length; i++)
            {
                var b = Bus.GetBusFromCol(list[i]);
                if (b.IsStayOnBusZone && b != bus) nnl.Add(b);
            }

            var curParent = bus.transform.parent;
            bus.nextNodeList = nnl;

            bus.nextNodeList.Sort((x, y) =>
            {
                Vector3 position = bus.transform.position;
                var xPoint = x.boxCollider.ClosestPoint(position);
                var yPoint = y.boxCollider.ClosestPoint(position);
                return (Vector3.Distance(position, xPoint) - Vector3.Distance(position, yPoint)) < 0 ? 1 : -1; //sort from low to high, but nextNodeList use as stack so invert to make closet bus is last element --> on top of stack
            });

            bus.transform.SetParent(null, true);
            bus.RootPosition = bus.transform.position;
            StartCoroutine(ie_PostHandle());
            return true;

            IEnumerator ie_PostHandle()
            {
                yield return waitForBusDone;

                PauseRunning(false);

                if (!bus.IsStayOnBusZone)
                {
                    remainNum--;
                    countText.text = remainNum.ToString();
                }
                else
                {
                    bus.transform.SetParent(curParent, true);
                }
            }

            bool IsTouchable()
            {
                var curPosX = bus.transform.position.x;
                return curPosX > touchableMinMaxX.x && curPosX < touchableMinMaxX.y;
            }
        }

        public int GetMinionNum()
        {
            int num = 0;
            for (int i = 0; i < busDataList.Count; i++)
            {
                num += GetNum(busDataList[i].busType);
            }

            return num;

            int GetNum(BusType type)
            {
                return type switch
                {
                    BusType.Car => 4,
                    BusType.Van => 6,
                    BusType.Bus => 10,
                };
            }
        }
    }

    [Serializable]
    public struct SlideBusData
    {
        public BusType busType;
        public JunkColor color;
    }
}