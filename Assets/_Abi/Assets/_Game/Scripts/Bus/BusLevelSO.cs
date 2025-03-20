using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    [Serializable, CreateAssetMenu(menuName = "Bus Level", fileName = "Bus Level")]
    public class BusLevelSO : ScriptableObject
    {
        public static BusLevelSO active;
        public float ScaleFactor = -1;
        public BusMapHardness busMapHardness;
        public MapType mapType;

        public List<BusPosData> busPosDatas;
        public List<TunnelDataPack> tunnelDataPacks;

        public int totalMinionNum;

        public AnchorPointData[] anchorPoints;
        public AnchorPointData[] busSlideAnchorPoints;

        public List<int> grayBusIndexes;

        public List<SlideBusData> busSlideDataList;

        public bool OverrideMinMinionCount;
        public int MinMinioncount;
        public void LoadLevel()
        {
            active = this;
            Helicoptar.cur.Reposition();
            if (OverrideMinMinionCount)
            {
                BusStation.MIN_MINION_PER_CALL = MinMinioncount;
            }
            else
            {
                BusStation.MIN_MINION_PER_CALL = 24;
            }
            ParkingLot.cur.ResetAll();
            JunkPile.ins.RecallAll(); //temp

            var shippers = FindObjectsByType<ShipperController>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
            foreach (var shipper in shippers)
            {
                if (shipper.CurrentBus != null)
                {
                    if (!shipper.CurrentBus.IsInActiveList)
                    {
                        JunkPile.ins.Push(shipper.CurrentBus);
                    }
                }

                shipper.DOKill();
                Destroy(shipper.gameObject);
            }
            
            
            var busList = new List<Bus>();
            for (int i = 0; i < busPosDatas.Count; i++)
            {
                var data = busPosDatas[i];
                var localScale = data.localScale;
                if (localScale.magnitude < 1f)
                {
                    localScale=new Vector3(7.17600012f, 7.17600012f, 7.17600012f);
                }
                localScale=new Vector3(7.17600012f, 7.17600012f, 7.17600012f);

                var bus = JunkPile.ins.IsSpecialType(data.color) ? JunkPile.ins.GetSpecialBus(data.color, data.position, data.rotation,localScale) : 
                    JunkPile.ins.GetBus(data.type, data.color, data.position, data.rotation,localScale);
                bus.transform.SetParent(null);
                bus.Weight = data.weight;
                busList.Add(bus);
                // Debug.Log("LOAD BUS " + i + "  " + bus.Type + "   " + bus.color + "   " + bus.Weight);
            }

            for (int i = 0; i < busList.Count; i++)
            {
                var bus = busList[i];
                // Debug.Log("LOAD BUS " + i + "  " + bus.gameObject.name + "    " + bus.Type + "   " + bus.color + "   " + bus.Weight);
                busList[i].nextNodeList = ConvertToBusList(busList, busPosDatas[i].nodeIndexList);
                busList[i].backTrackNodeList = ConvertToBusList(busList, busPosDatas[i].backTrackList);
                busList[i].Setup();
            }

            BusStation.cur.activeBus = busList;
            BusStation.cur.groundBus = new List<Bus>(busList);

            for (int i = BusStation.cur.groundBus.Count - 1; i >= 0; i--)
            {
                var bus = BusStation.cur.groundBus[i];
                if (bus.color >= JunkColor.Ambulance) BusStation.cur.groundBus.Remove(bus);
            }

            var grayBuses = ConvertToBusList(busList, grayBusIndexes);
            BusStation.cur.GrayBuses = grayBuses;
            for (int i = 0; i < grayBuses.Count; i++)
            {
                grayBuses[i].TurnToGray();
            }

            foreach (var bus in  BusStation.cur.groundBus)
            {
                if (bus.IsCoveredBus)
                {
                    bus.TurnToCovered();
                }
            }
            var tunnelList = new List<BusTunnel>();
            for (int i = 0; i < tunnelDataPacks.Count; i++)
            {
                var data = tunnelDataPacks[i];
                var tunnel = JunkPile.ins.GetTunnel();
                tunnel.FetchData(data);
                tunnelList.Add(tunnel);
            }

            if (busSlideDataList.Count == 0)
            {
                BusSlide.cur.SetActive(false);
                BusSlide.cur.ClearSlide();
            }
            else
            {
                BusSlide.cur.SetActive(true);
                BusSlide.cur.FetchData(new List<SlideBusData>(busSlideDataList));
                BusSlide.cur.Running();
            }

            BusStation.cur.activeTunnels = tunnelList;
            BusStation.cur.anchorPoints = GetAnchorPoints();

            SettingMap();
            ParkingLot.cur.FetchMinionsNUm(totalMinionNum);

            TransportCenter.cur.InitializeMap();
        }

        // [ContextMenu("SAVE LEVEL")]
        public void SaveLevel()
        {
            var busList = BusStation.cur.activeBus;
            busPosDatas = new List<BusPosData>();
            grayBusIndexes = new List<int>();
            for (int i = 0; i < busList.Count; i++)
            {
                var bus = busList[i];
                busPosDatas.Add(new BusPosData()
                {
                    color = bus.color,
                    type = bus.Type,
                    position = bus.transform.position,
                    localScale = bus.transform.localScale,
                    rotation = bus.transform.rotation,
                    nodeIndexList = ConvertToIndexList(busList, bus.nextNodeList),
                    backTrackList = ConvertToIndexList(busList, bus.backTrackNodeList),
                    weight = bus.Weight,
                });

                if (bus.IsGrayBus) grayBusIndexes.Add(i);
            }

            tunnelDataPacks = new List<TunnelDataPack>();
            var tunnelList = BusStation.cur.activeTunnels;
            for (int i = 0; i < tunnelList.Count; i++)
            {
                tunnelDataPacks.Add(tunnelList[i].ExtractDataPack());
            }

            busSlideDataList = BusSlide.cur.GetData();
        }

        private List<int> ConvertToIndexList(List<Bus> busList, List<Bus> nodeList)
        {
            var list = new List<int>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                list.Add(busList.IndexOf(nodeList[i]));
            }

            return list;
        }

        private List<Bus> ConvertToBusList(List<Bus> busList, List<int> indexList)
        {
            var list = new List<Bus>();
            for (int i = 0; i < indexList.Count; i++)
            {
                list.Add(busList[indexList[i]]);
            }

            return list;
        }

        public List<AnchorPoint> GetAnchorPoints()
        {
            var list = new List<AnchorPoint>();

            for (int i = 0; i < anchorPoints.Length; i++)
            {
                var data = anchorPoints[i];
                var bus = BusStation.cur.activeBus[data.busIndex];
                list.Add(new AnchorPoint() { bus = bus, anchorPoint = data.anchorPoint });
            }

            for (int i = 0; i < busSlideAnchorPoints.Length; i++)
            {
                var data = busSlideAnchorPoints[i];
                var bus = BusSlide.cur.busList[data.busIndex];
                list.Add(new AnchorPoint() { bus = bus, anchorPoint = data.anchorPoint });
            }

            list.Sort((a, b) => a.anchorPoint - b.anchorPoint);

            return list;
        }

        private void SettingMap()
        {
            switch (mapType)
            {
                case MapType.SingleLine:
                    ParkingLot.cur.isSingleLine = true;
                    ParkingLot.cur.isThirdLineOpen = false;
                    ParkingLot.cur.isBarrier = false;
                    break;
                case MapType.DoubleLine:
                    ParkingLot.cur.isSingleLine = false;
                    ParkingLot.cur.isThirdLineOpen = false;
                    ParkingLot.cur.isBarrier = false;
                    break;
                case MapType.TripleLine:
                    ParkingLot.cur.isSingleLine = false;
                    ParkingLot.cur.isThirdLineOpen = true;
                    ParkingLot.cur.isBarrier = false;
                    break;
                case MapType.DoubleBarrier:
                    ParkingLot.cur.isSingleLine = false;
                    ParkingLot.cur.isThirdLineOpen = false;
                    ParkingLot.cur.isBarrier = true;
                    break;
                case MapType.TripleBarrier:
                    ParkingLot.cur.isSingleLine = false;
                    ParkingLot.cur.isThirdLineOpen = true;
                    ParkingLot.cur.isBarrier = true;
                    break;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("SET DIRTY")]
        public void SetDirtyPro()
        {
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Fix SpecialBuses")]
        public void FiFixSpecialBuses()
        {
            Debug.Log("Fix Special Color data");
            for (int i = 0; i < busPosDatas.Count; i++)
            {
                var busdata = busPosDatas[i];
                if (JunkPile.IsSpecialColorType(busdata.color))
                {
                    busdata.color = JunkColor.Blue;
                    busPosDatas[i] = busdata;
                }
            }
     
            for (int i = 0; i < busSlideDataList.Count; i++)
            {
                var busdata = busSlideDataList[i];
                if (JunkPile.IsSpecialColorType(busdata.color))
                {
                    busdata.color = JunkColor.Blue;
                    busSlideDataList[i] = busdata;
                }
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct BusPosData
    {
        public JunkColor color;
        public BusType type;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        public List<int> nodeIndexList;
        public List<int> backTrackList;
        public int weight;
    }

    [Serializable]
    public struct AnchorPointData
    {
        public int busIndex;
        public int anchorPoint;
    }

    [Serializable]
    public struct AnchorPoint
    {
        public Bus bus;
        public int anchorPoint;
    }

    public enum BusMapHardness
    {
        Easy,
        Hard,
        SuperHard
    }

    public enum MapType
    {
        SingleLine,
        DoubleLine,
        TripleLine,
        DoubleBarrier,
        TripleBarrier,
        DoubleBarrierContinue,
        TripleBarrierContinue,
    }
}