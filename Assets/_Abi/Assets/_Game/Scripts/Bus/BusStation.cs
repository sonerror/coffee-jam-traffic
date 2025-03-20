using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Bus
{
    public class BusStation : MonoBehaviour //also use for edit map
    {
        public static BusStation cur;

        public List<Bus> activeBus; //only remove bus when it move out park zone
        public List<Bus> groundBus; //same
        public List<BusTunnel> activeTunnels;

        public AnimationCurve spreadCurve;

        public List<BusColorData> AwaitBusDatas { get; set; } = new List<BusColorData>(); // use for data from tunnel buses

        public List<BusColorDataWithBackTrack> AwaitForFetchDataBus { get; set; } =
            new List<BusColorDataWithBackTrack>(); // use for store color data of bus being chosen out of plan then move to AwaitBusDatas above to fetch in // use that list for both purpose

        public List<BusColorData> PenaltyColorData { get; set; } = new List<BusColorData>();
        public bool IsVipBus { get; set; }
        public Bus CurrentVipBus { get; set; }

        public static int MIN_MINION_PER_CALL = 32;

        public int MovedBus { get; set; }
        public List<AnchorPoint> anchorPoints;

        public List<Bus> GrayBuses { get; set; } = new List<Bus>();

        private void Awake()
        {
            cur = this;
        }

        public bool CallOfDuty(out List<JunkColor> menAtWar)
        {
            if (activeBus.Count == 0 && AwaitForFetchDataBus.Count == 0 && IsTunnelRanOutData())
            {
                menAtWar = new List<JunkColor>();
                return false;
            }

            var busList = new List<Bus>();

            CheckAnchorPoint(busList);
            busList.AddRange(SearchForNodeAtLeast(0, MIN_MINION_PER_CALL));

            CheckForNull(ref busList);

            menAtWar = busList.Count == 0 && AwaitBusDatas.Count == 0 ? new List<JunkColor>() : ExtractColorData(busList);

            if (ProcessPenaltyColor(menAtWar) && menAtWar.Count == 0)
            {
                var isEnd = CallOfDuty(out var newList);
                menAtWar = newList;
                return isEnd;
            }

            return true;
        }

        private bool IsTunnelRanOutData()
        {
            for (int i = 0; i < activeTunnels.Count; i++)
            {
                if (!activeTunnels[i].IsOutOfData) return false;
            }

            return true;
        }

        // private int spawnNum = 0;

        private List<JunkColor> ExtractColorData(List<Bus> busList)
        {
            var list = new List<BusColorData>();
            var unMixableIndexs = new List<int>();
            var topLayer = 1;
            foreach (var bus in busList)
            {
                if (bus.Layer > topLayer)
                {
                    topLayer = bus.Layer;
                }
            }
            for (int i = 0; i < busList.Count; i++)
            {
                // Debug.Log("EXTRACT COLOR " + busList[i].GetColorData());
                // Debug.DrawRay(busList[i].RootPosition, Vector3.up * 10f, Color.magenta, 5);
                var bus = busList[i];
                bus.IsBusTaken = true;
                list.Add(bus.GetColorData());
                if (bus.Layer < topLayer)
                {
                    if (bus.Weight > 0)
                    {
                        unMixableIndexs.Add(i);
                    }
                }
            }

            for (int i = 0; i < AwaitBusDatas.Count; i++)
            {
                list.Add(AwaitBusDatas[i]);
            }

            AwaitBusDatas.Clear();

            /*
            list = MergeBusColorData(list);
            */

            // Debug.Log("DONE EXTRACT ");

            if (LevelDataFragment.cur.gameData.level == 0) return RawMinions(list);
            return MixMinions(list);
            if (unMixableIndexs.Count > 0)
            {
                var mixList = new List<JunkColor>();
                var startIndex = 0;
                foreach (var i in unMixableIndexs)
                {
                    var matchList = new List<BusColorData>();
                    for (var j = startIndex; j < i; j++)
                    {
                        matchList.Add(list[j]);
                    }
                    mixList.AddRange(MixMinions(matchList));
                    mixList.AddRange(MixMinions(new List<BusColorData>() { list[i] }));
                    startIndex = i + 1;
                }

                if (unMixableIndexs[^1] < (list.Count - 1))
                {
                    var matchList = new List<BusColorData>();

                    for (int i = unMixableIndexs[^1] + 1; i < list.Count; i++)
                    {
                        matchList.Add(list[i]);

                    }
                    mixList.AddRange(MixMinions(matchList));

                }

                return mixList;
            }
            else
            {
                return MixMinions(list);
            }
        }

        public void CheckAwaitData(List<Bus> list)
        {
            var num = GetMinionNumFromBuses(list);
            while (AwaitForFetchDataBus.Count > 0)
            {
                if (num > MIN_MINION_PER_CALL) break;
                var data = AwaitForFetchDataBus.Dequeue();
                num += data.num;
                data.DecreaseWeight();
                AwaitBusDatas.Add(data.GetColorData());
            }
        }

        public void CheckForNull(ref List<Bus> list)
        {
            CheckAwaitData(list);

            if (GetMinionNumFromBuses(list) <= MIN_MINION_PER_CALL)
            {
                list.AddRange(SearchForNodeAtLeastLastChance(0, 100, MIN_MINION_PER_CALL));
            }

            CheckFromTunnel(list);
        }

        private int GetMinionNumFromBuses(List<Bus> list)
        {
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                num += list[i].GetTrueMinionNum();
            }

            return num;
        }

        public void CheckAnchorPoint(List<Bus> list)
        {
            for (int i = 0; i < anchorPoints.Count; i++)
            {
                var data = anchorPoints[i];
                if (MovedBus >= data.anchorPoint)
                {
                    if (!data.bus.IsBusTaken)
                    {
                        list.Add(data.bus);
                        data.bus.IsBusTaken = true;
                    }

                    anchorPoints.Remove(data);
                    return;
                }
            }
        }

        public List<Bus> SearchForNodeAtLeast(int weight, int minNum)
        {
            var list = new List<Bus>();
            int num = 0;
            for (int i = 0; i < activeBus.Count; i++)
            {
                if (num > minNum) break;
                var bus = activeBus[i];
                if (bus.IsBusTaken) continue;
                if (bus.Weight == weight)
                {
                    list.Add(bus);
                    num += bus.GetTrueMinionNum();
                    bus.IsBusTaken = true;
                }
            }

            return list;
        }

        public List<Bus> SearchForNodeAtLeastLastChance(int weightMin, int weightMax, int minNum)
        {
            var list = new List<Bus>();
            int num = 0;
            for (int i = 0; i < activeBus.Count; i++)
            {
                if (num > minNum) break;
                var bus = activeBus[i];
                if (bus.IsBusTaken) continue;
                if (bus.Weight == 1)
                {
                    list.Add(bus);
                    num += bus.GetTrueMinionNum();
                    bus.IsBusTaken = true;
                }
            }

            for (int i = 0; i < activeBus.Count; i++)
            {
                if (num > minNum) break;
                var bus = activeBus[i];
                if (bus.IsBusTaken) continue;
                if (bus.Weight == 2)
                {
                    list.Add(bus);
                    num += bus.GetTrueMinionNum();
                    bus.IsBusTaken = true;
                }
            }

            for (int i = 0; i < activeBus.Count; i++)
            {
                if (num > minNum) break;
                var bus = activeBus[i];
                if (bus.IsBusTaken) continue;

                list.Add(bus);
                num += bus.GetTrueMinionNum();
                bus.IsBusTaken = true;
            }

            return list;
        }

        public void CheckFromTunnel(List<Bus> list)
        {
            if (activeTunnels.Count == 0) return;
            var checkNum = GetMinionNumFromBuses(list);
            while (checkNum < MIN_MINION_PER_CALL && !IsTunnelRanOutData())
            {
                for (int i = 0; i < activeTunnels.Count; i++)
                {
                    var data = activeTunnels[i].HandOutData(false);
                    if (data.num <= 0) continue;

                    checkNum += data.num;

                    // Debug.Log("CHECK NUM " + checkNum);

                    AwaitBusDatas.Add(data);
                    if (checkNum >= MIN_MINION_PER_CALL) break;
                }
            }
        }

        public void RemoveFromActiveBuses(Bus bus)
        {
            if (bus.IsBusTaken || !bus.IsStayOnBusZone) activeBus.Remove(bus);
        }

        public void RemoveFromGroundBuses(Bus bus)
        {
            if (!bus.IsStayOnBusZone)
            {
                groundBus.Remove(bus);
                if (false)
                {
                    ParkingLot.cur.ReCalMinAndMaxPoint();
                }
            }
        }

        public List<JunkColor> RawMinions(List<BusColorData> colorList)
        {
            var list = new List<JunkColor>();

            for (int i = 0; i < colorList.Count; i++)
            {
                var data = colorList[i];
                for (int j = 0; j < data.num; j++)
                {
                    list.Add(data.color);
                }
            }

            return list;
        }


        /*public List<JunkColor> MixMinions(List<BusColorData> colorList, int difficulty=0)
        {
            // Tạo danh sách chứa tất cả các màu từ colorList theo số lượng của chúng
            List<JunkColor> flatColorList = new List<JunkColor>();
            foreach (var item in colorList)
            {
                for (int i = 0; i < item.num; i++)
                {
                    flatColorList.Add(item.color);
                }
            }

            int baseRange = 3;
            int mixRange = baseRange + difficulty;

            // Tạo danh sách các chỉ số dựa trên `colorList` để xác định phạm vi hoán đổi hợp lý
            List<int> indexRanges = new List<int>();
            for (int i = 0; i < colorList.Count; i++)
            {
                for (int j = 0; j < colorList[i].num; j++)
                {
                    indexRanges.Add(colorList[i].num);
                }
            }

            // Trộn ngẫu nhiên các màu trong phạm vi ±mixRange quanh chỉ số ban đầu của chúng trong `indexRanges`
            for (int i = 0; i < flatColorList.Count; i++)
            {
                int start = Mathf.Max(0, indexRanges[i] - mixRange);
                int end = Mathf.Min(flatColorList.Count - 1, indexRanges[i] + mixRange);

                // Điều chỉnh `start` và `end` để không vượt quá giới hạn mảng
                if (start <= 0)
                {
                    start = 0;
                    end = Mathf.Min(flatColorList.Count - 1, mixRange * 2);
                }
                else if (end >= flatColorList.Count - 1)
                {
                    end = flatColorList.Count - 1;
                    start = Mathf.Max(0, end - mixRange * 2);
                }

                if (start <= end)
                {
                    int randomIndex = UnityEngine.Random.Range(start, end + 1);

                    // Hoán đổi các màu nếu các chỉ số nằm trong phạm vi hợp lý
                    (flatColorList[i], flatColorList[randomIndex]) = (flatColorList[randomIndex], flatColorList[i]);
                }
            }

            return flatColorList;
        }*/


        /*
        public List<JunkColor> MixMinions(List<BusColorData> colorList, int difficulty=3)
        {
            List<JunkColor> result = new List<JunkColor>();

            // Thêm các màu vào danh sách kết quả theo số lượng của chúng
            foreach (var item in colorList)
            {
                for (int i = 0; i < item.num; i++)
                {
                    result.Add(item.color);
                }
            }

            int baseRange = 4; // Phạm vi cơ bản cho việc hoán đổi
            int mixRange = baseRange + difficulty; // Điều chỉnh phạm vi hoán đổi dựa trên độ khó

            // Trộn ngẫu nhiên danh sách kết quả trong khoảng ±mixRange index
            for (int i = result.Count - 1; i > 0; i--)
            {
                int start = Mathf.Max(0, i - mixRange);
                int end = Mathf.Min(result.Count - 1, i + mixRange);

                if (start == 0)
                {
                    // Đảm bảo khoảng cách giữa start và end luôn bằng 2 lần mixRange
                    end = Mathf.Min(result.Count - 1, mixRange * 2);
                }
                else if (end == result.Count - 1)
                {
                    // Đảm bảo khoảng cách giữa start và end luôn bằng 2 lần mixRange
                    start = Mathf.Max(0, end - mixRange * 2);
                }

                // Chọn ngẫu nhiên một index trong khoảng ±mixRange
                if (start <= end)
                {
                    int randomIndex = UnityEngine.Random.Range(start, end + 1);
                    (result[i], result[randomIndex]) = (result[randomIndex], result[i]);
                }
            }

            return result;
        }
        */


        public List<JunkColor> MixMinions(List<BusColorData> colorList)
        {
            var colorNum = colorList.Count;
            if (colorList.Count == 0)
            {
                return new();
            }
            if (colorNum == 1)
            {
                var backupList = new List<JunkColor>();
                var backupData = colorList[0];
                var backupColor = backupData.color;

                for (int i = 0; i < backupData.num; i++)
                {
                    backupList.Add(backupColor);
                }

                // Debug.Log("PICKED COLOR MONO" + colorList[0].color + "  " + colorList[0].num);

                return backupList;
            }

            // var pickIndex = Random.Range(0, colorNum);
            var pickIndex = 0;
            int maxNUm = 0;

            for (int i = 0; i < colorList.Count; i++)
            {
                if (colorList[i].num > maxNUm && (int)colorList[i].color < (int)JunkColor.Ambulance)
                {
                    maxNUm = colorList[i].num;
                    // Debug.Log("PICKED " + colorList[i].color);
                    pickIndex = i;
                }
            }

            var pickedData = colorList[pickIndex];
            colorList.Remove(pickedData);
            var remainColor = colorNum - 1;


            float average = pickedData.num * 1f / remainColor;
            int portion = Mathf.FloorToInt(average);
            var portionList = new List<int>();
            for (int i = 0; i < remainColor; i++)
            {
                portionList.Add(portion);
            }

            portionList[^1] += pickedData.num - remainColor * portion;

            var list = new List<JunkColor>();

            for (int i = 0; i < remainColor; i++)
            {
                if (i == 0)
                {
                    list.AddRange(SpreadColor(0, colorList[i], new BusColorData() { color = pickedData.color, num = portionList[i] }));
                }
                else if (i == pickIndex - 1)
                {
                    list.AddRange(SpreadColor(1, colorList[i], new BusColorData() { color = pickedData.color, num = portionList[i] }));
                }
                else if (i == pickIndex)
                {
                    list.AddRange(SpreadColor(1, new BusColorData() { color = pickedData.color, num = portionList[i] }, colorList[i]));
                }
                else
                {
                    list.AddRange(SpreadColor(0, colorList[i], new BusColorData() { color = pickedData.color, num = portionList[i] }));
                }
            }

            // Debug.Log("PICKED COLOR " + pickedData.color + "  " + pickedData.num + "   " + list.Count);

            return list;
        }

        private List<JunkColor> SpreadColor(float spread, BusColorData firstData, BusColorData secondData)
        {
            var firstNum = firstData.num;
            var firstColor = firstData.color;
            var secondNum = secondData.num;
            var secondColor = secondData.color;
            var totalNum = firstNum + secondNum;

            var list = new List<JunkColor>();

            for (int i = 0; i < totalNum; i++)
            {
                var per = GetPer(i);

                if (((Random.Range(0, 1f) < per && firstNum > 0) || secondNum == 0))
                {
                    list.Add(firstColor);
                    firstNum--;
                }
                else if (secondNum > 0)
                {
                    list.Add(secondColor);
                    secondNum--;
                }
            }

            return list;

            float GetPer(int i)
            {
                if ((int)firstColor >= (int)JunkColor.Ambulance || (int)secondColor >= (int)JunkColor.Ambulance) return 2f;
                var rawPer = spreadCurve.Evaluate((float)i / totalNum);
                rawPer -= .5f;
                rawPer *= spread;
                return rawPer + .5f;
            }
        }

        public List<BusColorData> MergeBusColorData(List<BusColorData> list)
        {
            var finalList = new List<BusColorData>();
            for (int i = 0; i < list.Count; i++)
            {
                AddToList(finalList, list[i]);
            }

            // for (int i = 0; i < finalList.Count; i++) Debug.Log("MERGE " + finalList[i].color + "  " + finalList[i].num);

            return finalList;

            void AddToList(List<BusColorData> illit, BusColorData data)
            {
                for (int i = 0; i < illit.Count; i++)
                {
                    if (illit[i].color == data.color)
                    {
                        var tmp = illit[i];
                        tmp.num += data.num;
                        illit[i] = tmp;

                        return;
                    }
                }

                illit.Add(data);
            }
        }

        public void BoosterUltraRandomColor()
        {
            TransportCenter.cur.IsGamePlaying = false;

            SwitchData[] switchList = new SwitchData[groundBus.Count];
            var carVanIndexRawList = new int[24];
            var busRawList = new Stack<int>();

            for (int i = 0; i < 24; i++)
            {
                carVanIndexRawList[i] = -1;
            }

            for (int i = 0; i < switchList.Length; i++)
            {
                var bus = groundBus[i];
                switchList[i] = new SwitchData() { isBusTaken = bus.IsBusTaken, isBusCarTaken = bus.IsBusCarPartTaken, isBusVanTaken = bus.IsBusVanPartTaken, color = bus.color };
            }

            //try to trade car van with a bus if possible
            for (int i = 0; i < switchList.Length; i++)
            {
                var bus = groundBus[i];
                var suffixIndex = bus.Type switch
                {
                    BusType.Car => 0,
                    BusType.Van => 1,
                    BusType.Bus => 2,
                };

                if (bus.Type == BusType.Bus)
                {
                    busRawList.Push(i);
                }
                else
                {
                    carVanIndexRawList[(int)bus.color * 2 + suffixIndex] = i;
                }
            }

            for (int i = 0; i < 12; i++)
            {
                var rawCar = carVanIndexRawList[i * 2];
                var rawVan = carVanIndexRawList[i * 2 + 1];
                var rawBus = busRawList.Count == 0 ? -1 : busRawList.Pop();
                if (rawCar != -1 && rawVan != -1 && rawBus != -1)
                {
                    var busColor = switchList[rawBus].color;
                    var carTaken = switchList[rawBus].isBusCarTaken || switchList[rawBus].isBusTaken;
                    var vanTaken = switchList[rawBus].isBusVanTaken || switchList[rawBus].isBusTaken;

                    switchList[rawBus].color = switchList[rawCar].color;
                    switchList[rawBus].isBusTaken = switchList[rawCar].isBusTaken && switchList[rawVan].isBusTaken;
                    switchList[rawBus].isBusCarTaken = switchList[rawCar].isBusTaken;
                    switchList[rawBus].isBusVanTaken = switchList[rawVan].isBusTaken;

                    switchList[rawCar].color = busColor;
                    switchList[rawCar].isBusTaken = carTaken;
                    switchList[rawVan].color = busColor;
                    switchList[rawVan].isBusTaken = vanTaken;
                }
            }

            // do normal shuffle bus index now
            var carIndexList = new List<int>();
            var vanIndexList = new List<int>();
            var busIndexList = new List<int>();
            for (int i = 0; i < groundBus.Count; i++)
            {
                // if (groundBus[i].IsBusTaken) continue;
                switch (groundBus[i].Type)
                {
                    case BusType.Car:
                        carIndexList.Add(i);
                        break;
                    case BusType.Van:
                        vanIndexList.Add(i);
                        break;
                    case BusType.Bus:
                        busIndexList.Add(i);
                        break;
                }
            }

            var carShuffleList = new List<int>(carIndexList);
            var vanShuffleList = new List<int>(vanIndexList);
            var busShuffleList = new List<int>(busIndexList);

            carShuffleList.Shuffle();
            vanShuffleList.Shuffle();
            busShuffleList.Shuffle();

            CheckIfTheSame(carIndexList, carShuffleList);
            CheckIfTheSame(vanIndexList, vanShuffleList);
            CheckIfTheSame(busIndexList, busShuffleList);

            StartCoroutine(ie_Shuffle());
            return;

            IEnumerator ie_Shuffle()
            {
                yield return null;
                yield return null;
                yield return null;

                /*
                yield return Yielders.Get(0.18f);
                */

                /*
                AudioManager.ins.PlaySound(SoundType.BoosterRefreshStart);
                AudioManager.ins.MakeVibrate(HapticTypes.HeavyImpact);

                Shader.SetGlobalFloat(Variables.GLOBAL_RAINBOW_PER, 0);

                for (int i = 0; i < groundBus.Count; i++)
                {
                    var bus = groundBus[i];
                    JunkPile.ins.ChangeToRainBow(bus, bus.color);
                }

                DOVirtual.Float(0, 1, .82f, (fuk) => Shader.SetGlobalFloat(Variables.GLOBAL_RAINBOW_PER, fuk)).SetEase(Ease.Linear);

                yield return Yielders.Get(3.14f);*/

                var delay = Config.cur.delayBetweenSwapCarBooster;
                var wait = new WaitForSeconds(delay);
                for (int i = 0; i < carShuffleList.Count; i++)
                {
                    SwitchBus(carIndexList[i], carShuffleList[i]);
                    yield return wait;
                }

                for (int i = 0; i < vanShuffleList.Count; i++)
                {
                    SwitchBus(vanIndexList[i], vanShuffleList[i]);
                    yield return wait;
                }

                for (int i = 0; i < busShuffleList.Count; i++)
                {
                    SwitchBus(busIndexList[i], busShuffleList[i]);
                    yield return wait;
                }

                TransportCenter.cur.IsGamePlaying = true;
            }

            void SwitchBus(int busIndex, int dataIndex)
            {
                var switchBus = groundBus[busIndex];
                var data = switchList[dataIndex];

                switchBus.IsBusTaken = data.isBusTaken;
                switchBus.IsBusCarPartTaken = data.isBusCarTaken;
                switchBus.IsBusVanPartTaken = data.isBusVanTaken;

                JunkPile.ins.ChangeColor(switchBus, data.color, switchBus.IsCovering);
                switchBus.Jiggle();

                AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);
            }

            void CheckIfTheSame(List<int> list, List<int> shuffleList)
            {
                bool isTheSame = true;
                for (int i = 0; i < list.Count; i++)
                {
                    if (groundBus[list[i]].color != groundBus[shuffleList[i]].color)
                    {
                        isTheSame = false;
                        break;
                    }
                }

                if (isTheSame && list.Count > 0)
                {
                    var first = shuffleList[0];
                    for (int i = 0; i < shuffleList.Count - 1; i++)
                    {
                        shuffleList[i] = shuffleList[i + 1];
                    }

                    shuffleList[^1] = first;
                }
            }
        }

        private struct SwitchData
        {
            public bool isBusTaken;
            public bool isBusCarTaken;
            public bool isBusVanTaken;
            public JunkColor color;
        }

        public bool IsVipBusAble()
        {
            return (ParkingLot.cur.GetVipSlot().AssignedBus == null);
        }

        public void BoosterVipBus()
        {
            CanvasGamePlay.CheckBoosterButtonStatic(true);
            StartCoroutine(ie_CheckBus());
            return;

            IEnumerator ie_CheckBus()
            {
                IsVipBus = true;
                CurrentVipBus = null;

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                IsVipBus = false;

                CanvasGamePlay.CheckBoosterButtonStatic(false);

                if (CurrentVipBus != null)
                {
                    ResourcesDataFragment.cur.AddVipBus(-1, "GAME_PLAY");
                    CurrentVipBus.IsTouchable = false;

                    var parkSlot = ParkingLot.cur.GetVipSlot();
                    parkSlot.SetupVipSlot(true);
                    parkSlot.PreParkTheBus(CurrentVipBus);

                    //move out bus handle in helicoptar 

                    Helicoptar.cur.PickupBus(CurrentVipBus, () =>
                    {
                        parkSlot.IsBusArrive = true;
                        TransportCenter.cur.StartMinionTransport(CurrentVipBus);
                        CurrentVipBus.SetOnParkScale();
                        CurrentVipBus.SetBusOpenOrHide(false);
                        CurrentVipBus = null;
                    });
                }
            }
        }

        public void BoosterMinionsMix()
        {
            TransportCenter.cur.IsGamePlaying = false;
            //var for multi line
            var isSingleLine = ParkingLot.cur.isSingleLine;

            var currentMinions = new List<JunkColor>();
            int num = 0;

            if (isSingleLine)
            {
                foreach (var m in ParkingLot.cur.MinionsQueue)
                {
                    currentMinions.Add(m.Color);
                }
            }
            else
            {
                foreach (var m in ParkingLot.cur.FirstMinionQueue)
                {
                    currentMinions.Add(m.Color);
                }

                foreach (var m in ParkingLot.cur.SecondMinionQueue)
                {
                    currentMinions.Add(m.Color);
                }

                foreach (var m in ParkingLot.cur.ThirdMinionQueue)
                {
                    currentMinions.Add(m.Color);
                }
            }

            num = currentMinions.Count;

            var targetColor = new List<JunkColor>();

            var busList = TransportCenter.cur.BusOnDutyQueue;

            for (int i = 0; i < busList.Count; i++)
            {
                if (num <= 0) break;
                var data = busList[i].GetRequireColorData();
                if (data.num == 0) continue;
                num -= data.num;

                // Debug.Log("GET PEN " + data.color + "  " + data.num);

                PenaltyColorData.Add(data);
                FetchColor(data);
            }

            var newQueue = new Queue<JunkColor>();
            var finalList = new List<JunkColor>();
            var awaitQueue = new List<JunkColor>(ParkingLot.cur.AwaitMinions);
            var tmpRootQueue = isSingleLine ? new List<Minion>(ParkingLot.cur.MinionsQueue) : new List<Minion>();
            if (!isSingleLine)
            {
                tmpRootQueue.AddRange(ParkingLot.cur.FirstMinionQueue);
                tmpRootQueue.AddRange(ParkingLot.cur.SecondMinionQueue);
                tmpRootQueue.AddRange(ParkingLot.cur.ThirdMinionQueue);
            }

            var rootQueue = new List<JunkColor>();

            for (int i = 0; i < tmpRootQueue.Count; i++)
            {
                rootQueue.Add(tmpRootQueue[i].Color);
            }

            ProcessPenaltyColor(awaitQueue);
            ProcessPenaltyColor(rootQueue);

            for (int i = ParkingLot.cur.GetStandPosNum(); i < targetColor.Count; i++)
            {
                finalList.Add(targetColor[i]);
            }

            int interruptIndex = 0;
            for (int i = targetColor.Count; i < ParkingLot.cur.GetStandPosNum(); i++)
            {
                if (interruptIndex == awaitQueue.Count) break;
                targetColor.Add(awaitQueue[interruptIndex]);
                interruptIndex++;
            }

            // Debug.Log("INTERRUPT " + interruptIndex + "  " + targetColor.Count + "   " + ParkingLot.cur.GetStandPosNum() + "   " + rootQueue.Length);

            for (int i = interruptIndex; i < awaitQueue.Count; i++)
            {
                finalList.Add(awaitQueue[i]);
            }

            int subInterruptIndex = 0;
            for (int i = targetColor.Count; i < ParkingLot.cur.GetStandPosNum(); i++)
            {
                if (subInterruptIndex == rootQueue.Count) break;
                targetColor.Add(rootQueue[subInterruptIndex]);
                subInterruptIndex++;
            }

            for (int i = subInterruptIndex; i < rootQueue.Count; i++)
            {
                finalList.Add(rootQueue[i]);
            }

            ProcessPenaltyColor(finalList);

            for (int i = 0; i < finalList.Count; i++)
            {
                newQueue.Enqueue(finalList[i]);
            }

            ParkingLot.cur.AwaitMinions = newQueue;
            StartCoroutine(ie_SwitchMinion());
            return;

            IEnumerator ie_SwitchMinion()
            {
                int index = 0;

                // Debug.Log("SWITCH MINION " + ParkingLot.cur.MinionsQueue.Count + "  " + targetColor.Count);

                var delay = Config.cur.delayBetweenSwapMinionBooster;
                var wait = new WaitForSeconds(delay);

                if (isSingleLine)
                {
                    foreach (var m in ParkingLot.cur.MinionsQueue)
                    {
                        // Debug.Log("SWITCH " + index);
                        JunkPile.ins.ChangeColor(m, targetColor[index]);
                        m.PopOnColorChange();

                        AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                        AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                        index++;
                        yield return wait;
                    }
                }
                else if (ParkingLot.cur.isBarrier)
                {
                    // if (ParkingLot.cur.isContinueBarrier)
                    {
                        var currentIndex = TransportCenter.cur.GetTrueIndex();
                        var queueList = currentIndex switch
                        {
                            0 => new List<Queue<Minion>>() { ParkingLot.cur.FirstMinionQueue, ParkingLot.cur.SecondMinionQueue, ParkingLot.cur.ThirdMinionQueue },
                            1 => new List<Queue<Minion>>() { ParkingLot.cur.SecondMinionQueue, ParkingLot.cur.ThirdMinionQueue, ParkingLot.cur.FirstMinionQueue },
                            _ => new List<Queue<Minion>>() { ParkingLot.cur.ThirdMinionQueue, ParkingLot.cur.FirstMinionQueue, ParkingLot.cur.SecondMinionQueue },
                        };

                        foreach (var m in queueList[0])
                        {
                            // Debug.Log("SWITCH " + index);
                            JunkPile.ins.ChangeColor(m, targetColor[index]);
                            m.PopOnColorChange();

                            AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                            AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                            index++;
                            yield return wait;
                        }

                        foreach (var m in queueList[1])
                        {
                            // Debug.Log("SWITCH " + index);
                            JunkPile.ins.ChangeColor(m, targetColor[index]);
                            m.PopOnColorChange();

                            AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                            AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                            index++;
                            yield return wait;
                        }

                        foreach (var m in queueList[2])
                        {
                            // Debug.Log("SWITCH " + index);
                            JunkPile.ins.ChangeColor(m, targetColor[index]);
                            m.PopOnColorChange();

                            AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                            AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                            index++;
                            yield return wait;
                        }
                    }
                    // else
                    // {
                    //     var currentIndex = TransportCenter.cur.GetTrueIndex();
                    //     var queueList = currentIndex switch
                    //     {
                    //         0 => new List<Queue<Minion>>() { ParkingLot.cur.FirstMinionQueue, ParkingLot.cur.SecondMinionQueue, ParkingLot.cur.ThirdMinionQueue },
                    //         1 => new List<Queue<Minion>>() { ParkingLot.cur.SecondMinionQueue, ParkingLot.cur.ThirdMinionQueue, ParkingLot.cur.FirstMinionQueue },
                    //         _ => new List<Queue<Minion>>() { ParkingLot.cur.ThirdMinionQueue, ParkingLot.cur.FirstMinionQueue, ParkingLot.cur.SecondMinionQueue },
                    //     };
                    //
                    //     var newList = new List<Minion>();
                    //
                    //     var firstCount = queueList[0].Count;
                    //     var secondCount = queueList[1].Count;
                    //     var thirdCount = queueList[2].Count;
                    //
                    //     var firstList = new List<Minion>(queueList[0]);
                    //     var secondList = new List<Minion>(queueList[1]);
                    //     var thirdList = new List<Minion>(queueList[2]);
                    //
                    //     var firstIndex = 0;
                    //     var secondIndex = 0;
                    //     var thirdIndex = 0;
                    //     bool isAdded = true;
                    //     while (isAdded)
                    //     {
                    //         isAdded = false;
                    //         if (firstIndex < firstCount)
                    //         {
                    //             newList.Add(firstList[firstIndex]);
                    //             firstIndex++;
                    //             isAdded = true;
                    //         }
                    //
                    //         if (secondIndex < secondCount)
                    //         {
                    //             newList.Add(secondList[secondIndex]);
                    //             secondIndex++;
                    //             isAdded = true;
                    //         }
                    //
                    //         if (thirdIndex < thirdCount)
                    //         {
                    //             newList.Add(thirdList[thirdIndex]);
                    //             thirdIndex++;
                    //             isAdded = true;
                    //         }
                    //     }
                    //
                    //     foreach (var m in newList)
                    //     {
                    //         // Debug.Log("SWITCH " + index);
                    //         JunkPile.ins.ChangeColor(m, targetColor[index]);
                    //         m.PopOnColorChange();
                    //
                    //         AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                    //         AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);
                    //
                    //         index++;
                    //         yield return wait;
                    //     }
                    // }
                }
                else
                {
                    foreach (var m in ParkingLot.cur.FirstMinionQueue)
                    {
                        // Debug.Log("SWITCH " + index);
                        JunkPile.ins.ChangeColor(m, targetColor[index]);
                        m.PopOnColorChange();

                        AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                        AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                        index++;
                        yield return wait;
                    }

                    foreach (var m in ParkingLot.cur.SecondMinionQueue)
                    {
                        // Debug.Log("SWITCH " + index);
                        JunkPile.ins.ChangeColor(m, targetColor[index]);
                        m.PopOnColorChange();

                        AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                        AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                        index++;
                        yield return wait;
                    }

                    foreach (var m in ParkingLot.cur.ThirdMinionQueue)
                    {
                        // Debug.Log("SWITCH " + index);
                        JunkPile.ins.ChangeColor(m, targetColor[index]);
                        m.PopOnColorChange();

                        AudioManager.ins.PlaySound(SoundType.MinionOnBoard);
                        AudioManager.ins.MakeVibrate(HapticTypes.LightImpact);

                        index++;
                        yield return wait;
                    }
                }

                TransportCenter.cur.IsGamePlaying = true;
                TransportCenter.cur.StartMinionTransport();
            }

            void FetchColor(BusColorData data)
            {
                var fetchNum = data.num;
                var color = data.color;

                for (int i = 0; i < fetchNum; i++) targetColor.Add(color);

                // Debug.Log("FETCH COLOR " + fetchNum + "  " + color + "   " + targetColor.Count);
            }
        }

        public bool ProcessPenaltyColor(List<JunkColor> colorList) //return if penalty happen
        {
            if (PenaltyColorData.Count == 0) return false;
            bool isChange = false;
            for (int i = 0; i < PenaltyColorData.Count; i++)
            {
                var data = PenaltyColorData[i];
                var num = data.num;
                for (int j = 0; j < num; j++)
                {
                    if (!colorList.Contains(data.color)) break;
                    colorList.Remove(data.color);
                    data.num--;
                    isChange = true;
                }

                PenaltyColorData[i] = data;
            }

            for (int i = PenaltyColorData.Count - 1; i >= 0; i--)
            {
                if (PenaltyColorData[i].num <= 0) PenaltyColorData.Remove(PenaltyColorData[i]);
            }

            return isChange;
        }

        public void CheckGrayBus()
        {
            for (int i = 0; i < GrayBuses.Count; i++)
            {
                var bus = GrayBuses[i];
                Debug.Log("CHECK GRAY " + bus.IsClearAllNextNode(), bus.gameObject);
                if (!bus.IsGrayBus) continue;
                if (bus.IsClearAllNextNode())
                {
                    bus.TurnBackNormalFromGray();
                }
            }
        }

        public void CheckCoveredBuses()
        {
            foreach (var bus in groundBus)
            {
                if (bus.IsCoveredBus && !bus.IsCovering)
                {
                    bus.TurnToUnCovered();
                }
            }
        }

#if UNITY_EDITOR
        private bool isDrawGizmo;
        private bool isSpecificGizmo;
        private Bus specificGizmoBus;
        private List<GizmoData> gizmoDatas = new List<GizmoData>();
        [SerializeField] private Gradient gizmoGradient;
        private List<GizmoWeightData> gizmoWeightDatas = new List<GizmoWeightData>();
        private List<GizmoDreamyNumData> gizmoDreamyNumDatas = new List<GizmoDreamyNumData>();

        [SerializeField] private int colorToRandom = 6;

        private bool isShowRootNode;
        private List<Bus> rootNodeList = new List<Bus>();

        private bool isShowWeight;
        private bool isPositionRecored;
        private bool isShowDreamyNum;

        private List<Vector3> recordedBusPos;

        private const int BUS_LAYER = 1 << 8;

        [SerializeField] private Texture labelBg;

        public BusLevelSO levelSO;

        [SerializeField] private Bus trackBranchBus;
        private bool isTrackBranch;

        public Transform camTreePos;
        private Vector3 rootPos;
        private Vector3 rootEuler;
        private float rootOrthoSize;
        [SerializeField] private float camTreeOrthoSize;

        private void Start()
        {
            rootPos = CameraCon.ins.transform.position;
            rootEuler = CameraCon.ins.transform.eulerAngles;
            rootOrthoSize = CameraCon.ins.cam.orthographicSize;
        }

        [ContextMenu("HOW MANY COLOR IS IT")]
        public void CountColor()
        {
            var flag = new int[System.Enum.GetNames(typeof(JunkColor)).Length];
            for (int i = 0; i < activeBus.Count; i++)
            {
                flag[(int)activeBus[i].color]++;
            }

            int num = 0;
            for (int i = 0; i < flag.Length; i++)
            {
                if (flag[i] > 0) num++;
            }
            Debug.Log("FUCKFUCKFUCK NUM OF COLOR " + num);
        }
        [Space(20)]
        public int RandomColorLayer = -1;
        public int Level = 1;
        [ContextMenu("MEGA RANDOM COLOR")]
        public void MegaRandomColor()
        {
            if (!EditorApplication.isPlaying) return;
            var colorList = new List<JunkColor>();
            var targetColor = Mathf.Min(11, colorToRandom);
            for (int i = 0; i < targetColor; i++)
            {
                colorList.Add((JunkColor)i);
            }

            var basicColor = new List<JunkColor> { JunkColor.Blue, JunkColor.Red, JunkColor.Green, JunkColor.Yellow };
            var randomAbleBuses = new List<Bus>(activeBus);
            if (RandomColorLayer >= 1 && RandomColorLayer <= 4)
            {
                randomAbleBuses.Clear();
                foreach (var bus in activeBus)
                {
                    if (bus.Layer == RandomColorLayer)
                    {
                        randomAbleBuses.Add(bus);
                    }
                }
            }
            for (int i = 0; i < randomAbleBuses.Count; i++)
            {
                var bus = randomAbleBuses[i];
                JunkPile.ins.ChangeColor(bus, bus.GetBranchDepth() > 3 ? colorList.GetRandom() : basicColor.GetRandom());
            }
        }

        [ContextMenu("SAVE ANCHOR POINT")]
        public void SaveAnchorPoint()
        {
            if (!EditorApplication.isPlaying) return;
            var list = new List<AnchorPointData>();

            for (int i = 0; i < anchorPoints.Count; i++)
            {
                var data = anchorPoints[i];
                list.Add(new AnchorPointData() { busIndex = activeBus.IndexOf(data.bus), anchorPoint = data.anchorPoint });
            }

            levelSO.anchorPoints = list.ToArray();
        }

        [ContextMenu("SAVE LEVEL")]
        public void SaveMapToSO()
        {
            if (EditorApplication.isPlaying && activeBus.Count > 0)
            {
                SaveAnchorPoint();

                int num = 0;
                for (int i = 0; i < activeBus.Count; i++)
                {
                    num += activeBus[i].MaxMinion;
                }

                for (int i = 0; i < activeTunnels.Count; i++)
                {
                    activeTunnels[i].FetchLockBus();
                    num += activeTunnels[i].GetTotalMinionNum();
                }

                num += BusSlide.cur.GetMinionNum();

                levelSO.totalMinionNum = num;
                levelSO.SaveLevel();
                levelSO.SetDirtyPro();
            }
        }

        [ContextMenu("LOAD LEVEL")]
        public void LoadLevel()
        {
            if (EditorApplication.isPlaying)
            {
                if (Level > -1)
                {

                    LevelDataFragment.cur.gameData.level = Level;
                }
                else
                {
                    LevelDataFragment.cur.gameData.level = 1;

                }

                levelSO.LoadLevel();
                DrawBranch();
            }
        }

        public void LoadCurrentLevel()
        {
            if (EditorApplication.isPlaying)
            {
                LevelDataFragment.cur.LoadLevel();
                DrawBranch();
            }
        }

        public void LoadNExtLevel()
        {
            if (EditorApplication.isPlaying)
            {
                LevelDataFragment.cur.AscendLevel();
                LevelDataFragment.cur.LoadLevel();
                DrawBranch();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) DrawBranch();
            if (Input.GetKeyDown(KeyCode.S)) TurnOffBranchGizmo();
            if (Input.GetKeyDown(KeyCode.B)) ToggleTrackBranch();
            if (Input.GetKeyDown(KeyCode.N)) ToggleRootNode();
            if (Input.GetKeyDown(KeyCode.M)) ToggleWeight();
            if (Input.GetKeyDown(KeyCode.R)) SnapBackBus();
            if (Input.GetKeyDown(KeyCode.T)) ConstructTree();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = CameraCon.ins.cam.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray, 1000, BUS_LAYER);
                if (hits.Length == 0)
                {
                    isSpecificGizmo = false;
                    isTrackBranch = false;
                    return;
                }
                var bus = hits[0].transform.GetComponent<Bus>();

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    JunkPile.ins.Push(bus);
                    return;
                }

                if (isTrackBranch)
                {
                    isSpecificGizmo = true;
                    isTrackBranch = false;
                    specificGizmoBus = bus;
                }
                else
                {
                    trackBranchBus = bus;
                    isTrackBranch = true;
                    isSpecificGizmo = false;
                }
                // Debug.Log("CHANGE COLOR " + Input.GetKeyDown(KeyCode.Alpha1), bus.gameObject);

                if (Input.GetKey(KeyCode.BackQuote)) ChangeBusColor(bus, (JunkColor)0);
                if (Input.GetKey(KeyCode.Alpha1)) ChangeBusColor(bus, (JunkColor)1);
                if (Input.GetKey(KeyCode.Alpha2)) ChangeBusColor(bus, (JunkColor)2);
                if (Input.GetKey(KeyCode.Alpha3)) ChangeBusColor(bus, (JunkColor)3);
                if (Input.GetKey(KeyCode.Alpha4)) ChangeBusColor(bus, (JunkColor)4);
                if (Input.GetKey(KeyCode.Alpha5)) ChangeBusColor(bus, (JunkColor)5);
                if (Input.GetKey(KeyCode.Alpha6)) ChangeBusColor(bus, (JunkColor)6);
                if (Input.GetKey(KeyCode.Alpha7)) ChangeBusColor(bus, (JunkColor)7);
                if (Input.GetKey(KeyCode.Alpha8)) ChangeBusColor(bus, (JunkColor)8);
                if (Input.GetKey(KeyCode.Alpha9)) ChangeBusColor(bus, (JunkColor)9);
                if (Input.GetKey(KeyCode.Alpha0)) ChangeBusColor(bus, (JunkColor)10);
                if (Input.GetKey(KeyCode.Z)) ChangeBusColor(bus, (JunkColor)11);
                if (Input.GetKey(KeyCode.X)) ChangeBusColor(bus, (JunkColor)12);
                if (Input.GetKey(KeyCode.C)) ChangeBusColor(bus, (JunkColor)13);
                if (Input.GetKey(KeyCode.V)) ChangeBusColor(bus, (JunkColor)14);
                if (Input.GetKey(KeyCode.A))
                {
                    if (JunkPile.ins.IsSpecialType(bus.color)) return;
                    bus.TurnToGray();
                    bus.CurrentGrayMat.SetFloat(Variables.GRAY_PER, .35f);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (trackBranchBus != null)
                {
                    var index = activeBus.IndexOf(trackBranchBus) - 1;
                    trackBranchBus = activeBus[Mathf.Clamp(index, 0, activeBus.Count - 1)];
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (trackBranchBus != null)
                {
                    var index = activeBus.IndexOf(trackBranchBus) + 1;
                    trackBranchBus = activeBus[Mathf.Clamp(index, 0, activeBus.Count - 1)];
                }
            }

            return;

            void ChangeBusColor(Bus bus, JunkColor color)
            {
                Debug.LogError("okia");
                var isCurSpecial = JunkPile.ins.IsSpecialType(bus.color);
                var isTargetSpecial = JunkPile.ins.IsSpecialType(color);

                if (isCurSpecial != isTargetSpecial || isTargetSpecial)
                {
                    JunkPile.ins.Push(bus);
                    bus = isTargetSpecial ? JunkPile.ins.GetSpecialBus(color, bus.transform.position, bus.transform.rotation, bus.transform.localScale) :
                        JunkPile.ins.GetBus(bus.Type, color, bus.transform.position, bus.transform.rotation, bus.transform.localScale);
                }

                bus.IsGrayBus = false;
                bus.CurrentGrayMat = null;
                if (!isTargetSpecial) JunkPile.ins.ChangeColor(bus, color);
            }
        }

        private void OnDrawGizmos()
        {
            if (!isDrawGizmo) return;
            for (int i = 0; i < gizmoDatas.Count; i++)
            {
                var data = gizmoDatas[i];
                if (isSpecificGizmo && data.bus != specificGizmoBus) continue;
                if (isSpecificGizmo)
                {
                    isTrackBranch = false;
                }

                if (isTrackBranch && data.bus != trackBranchBus && (data.bus.nextNodeList.Contains(trackBranchBus) || !data.bus.backTrackNodeList.Contains(trackBranchBus))) continue;
                if (!data.bus.IsStayOnBusZone || !data.targetBus.IsStayOnBusZone) continue;

                Gizmos.color = GetColor(data.bus.nextNodeList.Count);
                var dir = data.end - data.start;
                dir.Normalize();
                Gizmos.DrawLine(data.start + dir * .34f, data.end - dir * .34f);

                var midpoint = (data.start + data.end) / 2;
                var offPoint = midpoint - dir * 0.14f;
                var cross = Vector3.Cross(Vector3.up, dir).normalized * .12f;
                var sidePoint = offPoint + cross;
                var subSidePoint = offPoint - cross;

                Gizmos.DrawLine(sidePoint, subSidePoint);
                Gizmos.DrawLine(midpoint, subSidePoint);
                Gizmos.DrawLine(sidePoint, midpoint);

                if (isSpecificGizmo)
                {
                    Gizmos.color = Color.red;
                    var matrix = Gizmos.matrix;
                    Gizmos.matrix = data.targetBus.boxCollider.transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(data.targetBus.boxCollider.center, data.targetBus.boxCollider.size);
                    Gizmos.matrix = matrix;
                }
            }

            if (isShowWeight)
            {
                var color = GUI.color;
                for (int i = 0; i < gizmoWeightDatas.Count; i++)
                {
                    var data = gizmoWeightDatas[i];

                    var labelPos = data.bus.transform.position + Vector3.up * 2.52f;
                    Handles.Label(labelPos + Vector3.down * .01f, labelBg);
                    GUI.color = Color.black;
                    Handles.Label(labelPos, data.weight.ToString());
                    GUI.color = color;
                }

                Handles.EndGUI();
            }

            if (isShowRootNode)
            {
                for (int i = 0; i < rootNodeList.Count; i++)
                {
                    Gizmos.DrawWireCube(rootNodeList[i].transform.position + Vector3.down * 4.34f, 2.12f * Vector3.one);
                }
            }

            if (isShowDreamyNum)
            {
                var color = GUI.color;
                for (int i = 0; i < gizmoDreamyNumDatas.Count; i++)
                {
                    var data = gizmoDreamyNumDatas[i];

                    var labelPos = data.pos;
                    Handles.Label(labelPos + Vector3.down * .01f, labelBg);
                    GUI.color = Color.black;
                    Handles.Label(labelPos, data.num.ToString());
                    GUI.color = color;
                }

                Handles.EndGUI();
            }

            return;

            Color GetColor(int degree)
            {
                return gizmoGradient.Evaluate(Mathf.Clamp01(degree / 10f));
            }
        }

        [ContextMenu("TAKE ALL BUS")]
        public void TakeAllBus()
        {
            var list = FindObjectsByType<Bus>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            activeBus = new List<Bus>(list);

            for (int i = activeBus.Count - 1; i >= 0; i--)
            {
                if (activeBus[i] is BusHollow or BusBlocker)
                {
                    activeBus.RemoveAt(i);
                }
            }
        }

        [ContextMenu("SORT ACTIVE BUS")]
        public void SortActiveBus()
        {
            activeBus.Sort((x, y) =>
                {
                    var rs = (x.GetBranchWeight(new List<Bus>()) - y.GetBranchWeight(new List<Bus>())) * 100;
                    return rs;
                }

            // + (x.GetBranchDepth() - y.GetBranchDepth()) * 10
            // + (x.color - y.color)
            );
            // activeBus.Shuffle();
            //
            // for (int i = 0; i < activeBus.Count; i++)
            // {
            //     Debug.Log("SORT " + activeBus[i].GetColorData() + "   " + activeBus[i].Weight);
            // }
        }

        [ContextMenu("Tempo Connect Node")]
        public void TempoConnectNode()
        {
            // AlignBus();

            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];

                ConnectNode(bus);
            }

            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];
                bus.directBackNodeList = new List<Bus>();
            }

            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];
                var list = bus.nextNodeList;
                for (int j = 0; j < list.Count; j++)
                {
                    list[j].directBackNodeList.Add(bus);
                }
            }

            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];

                bus.Weight = bus.GetBranchWeight(new List<Bus>()) - 1; //offset -1 for begin check
                bus.BackTrackAllNode();
            }

            DrawBranch();
            SortActiveBus();
        }

        public void ConnectNode(Bus bus)
        {
            var width = bus.boxCollider.size.x * bus.transform.localScale.x * 0.5f;
            var hit1s = Physics.BoxCastAll(bus.transform.position + Vector3.up * 0.35f,
                new Vector3(0, 0.02f, 0) + new Vector3(1, 0, 0.1f) * width, bus.transform.forward, bus.transform.rotation, 20, BUS_LAYER);
            var length = bus.boxCollider.size.z * bus.transform.localScale.z * 0.5f;
            var hit2s = Physics.BoxCastAll(bus.transform.position + Vector3.up * 0.35f,
                new Vector3(0, 0.02f, 0) + new Vector3(1, 0, 0) * width + new Vector3(0, 0, 1) * length,
                Vector3.up, bus.transform.rotation, 20, BUS_LAYER);
            var hitList = new List<RaycastHit>();
            foreach (var hit in hit1s)
            {
                hitList.Add(hit);
            }
            foreach (var hit in hit2s)
            {
                hitList.Add(hit);
            }
            var hits = hitList.ToArray();
            bus.nextNodeList.Clear();
            for (int j = 0; j < hits.Length; j++)
            {
                Debug.Log("HIT ", hits[j].transform.gameObject);
                var targetBus = hits[j].transform.GetComponent<Bus>();
                if (targetBus != null && targetBus != bus && targetBus is not BusBlocker) bus.nextNodeList.Add(targetBus);
            }

            bus.nextNodeList.Sort((x, y) =>
            {
                Vector3 position = bus.transform.position;
                var xPoint = x.boxCollider.ClosestPoint(position);
                var yPoint = y.boxCollider.ClosestPoint(position);
                return (Vector3.Distance(position, xPoint) - Vector3.Distance(position, yPoint)) < 0 ? 1 : -1; //sort from low to high, but nextNodeList use as stack so invert to make closet bus is last element --> on top of stack
            });
        }
        [ContextMenu("Construct tree")]
        public void ConstructTree()
        {
            RecordCurrentPos();
            SetCamTreePos(true);
            isShowDreamyNum = true;
            var indexArray = new int[100];
            const float yRoot = 20f;
            const float xDiff = 3.62f;
            const float zDiff = -6.42f;
            var subActiveBus = new List<Bus>(activeBus);
            while (subActiveBus.Count > 0)
            {
                var root = subActiveBus[^1];
                Handle(root);
                var checkList = root.nextNodeList;
                for (int i = 0; i < checkList.Count; i++)
                {
                    var bus = checkList[i];
                    Handle(bus);
                }
            }
            gizmoDreamyNumDatas.Clear();
            int recored = 0;
            for (int i = 0; i < indexArray.Length; i++)
            {
                if (indexArray[i] != 0)
                {
                    var pos = new Vector3(-2.5f, yRoot, zDiff * i) + Vector3.right * 14.86f;
                    recored += indexArray[i];
                    gizmoDreamyNumDatas.Add(new GizmoDreamyNumData() { num = recored, pos = pos });
                }
            }
            DrawBranch();
            return;
            void Handle(Bus bus)
            {
                if (!subActiveBus.Contains(bus)) return;
                subActiveBus.Remove(bus);
                var depthIndex = bus.GetBranchDepth();
                var sideIndex = indexArray[depthIndex];
                indexArray[depthIndex]++;
                var pos = new Vector3(sideIndex * xDiff * (1 + depthIndex * .28f) + depthIndex * depthIndex * .24f, yRoot, zDiff * depthIndex) + Vector3.right * 14.86f;
                bus.transform.position = pos;
            }
        }
        [ContextMenu("DRAW BRANCH")]
        public void DrawBranch()
        {
            gizmoDatas.Clear();
            gizmoWeightDatas.Clear();
            rootNodeList.Clear();

            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];

                var list = bus.nextNodeList;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] == null || !list[j].IsStayOnBusZone) continue;
                    gizmoDatas.Add(new GizmoData()
                    {
                        depth = j,
                        end = list[j].transform.position + Vector3.up * 1.12f,
                        start = bus.transform.position + Vector3.up * 1.12f,
                        bus = bus,
                        targetBus = list[j],
                    });
                }

                gizmoWeightDatas.Add(new GizmoWeightData()
                {
                    bus = bus,
                    weight = bus.Weight
                });

                if (bus.backTrackNodeList.Count == 0 && !rootNodeList.Contains(bus)) rootNodeList.Add(bus);
            }

            isDrawGizmo = true;
        }

        [ContextMenu("TOGGLE TRACK BRANCH")]
        public void ToggleTrackBranch()
        {
            isTrackBranch = !isTrackBranch;
        }

        [ContextMenu("TOGGLE ROOT NODE")]
        public void ToggleRootNode()
        {
            isShowRootNode = !isShowRootNode;
        }

        [ContextMenu("TOGGLE WEIGHT")]
        public void ToggleWeight()
        {
            isShowWeight = !isShowWeight;
        }

        [ContextMenu("TURN OFF BRANCH GIZMO")]
        public void TurnOffBranchGizmo()
        {
            isDrawGizmo = false;
            isTrackBranch = false;
        }

        private void RecordCurrentPos()
        {
            if (isPositionRecored) return;
            isPositionRecored = true;
            recordedBusPos = new List<Vector3>();

            for (int i = 0; i < activeBus.Count; i++)
            {
                recordedBusPos.Add(activeBus[i].transform.position);
            }
        }

        [ContextMenu("SNAP BACK BUS")]
        public void SnapBackBus()
        {
            for (int i = 0; i < activeBus.Count; i++)
            {
                activeBus[i].transform.position = recordedBusPos[i];
            }

            isPositionRecored = false;
            isDrawGizmo = false;
            isShowDreamyNum = false;

            SetCamTreePos(false);
        }

        private Vector3[] eulerAngle = new Vector3[] { new Vector3(0, 300, 0), 
        new Vector3(0, 30, 0), new Vector3(0, 120, 0), new Vector3(0, 210, 0) };
        [Space(20)]
        [SerializeField] private float jiggleAngle = 2;

        [ContextMenu("ALIGN BUS")]
        public void AlignBus()
        {
            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];
                bus.transform.eulerAngles = GetAlignAngle(bus.transform, false);
            }
        }

        [ContextMenu("JIGGLE BUS")]
        public void JiggleBus()
        {
            for (int i = 0; i < activeBus.Count; i++)
            {
                var bus = activeBus[i];
                bus.transform.eulerAngles = GetAlignAngle(bus.transform, true);
            }
        }

        private Vector3 GetAlignAngle(Transform trans, bool isJiggle)
        {
            var angle = trans.eulerAngles;
            var yAngle = angle.y;
            for (int i = 0; i < eulerAngle.Length; i++)
            {
                if (Mathf.Abs(yAngle - eulerAngle[i].y) < 10) return eulerAngle[i] + (isJiggle ? Random.Range(-jiggleAngle, jiggleAngle) : 0) * Vector3.up;
            }

            return eulerAngle[0] + (isJiggle ? Random.Range(-jiggleAngle, jiggleAngle) : 0) * Vector3.up;
        }

        private void SetCamTreePos(bool isTreeMode)
        {
            if (isTreeMode)
            {
                CameraCon.ins.SetRestPoint(camTreePos);
                CameraCon.ins.cam.orthographicSize = camTreeOrthoSize;
            }
            else
            {
                CameraCon.ins.transform.position = rootPos;
                CameraCon.ins.transform.eulerAngles = rootEuler;
                CameraCon.ins.cam.orthographicSize = rootOrthoSize;
            }
        }
#endif
    }

    [Serializable]
    public struct GizmoData
    {
        public int depth;
        public Vector3 start;
        public Vector3 end;
        public Bus bus;
        public Bus targetBus;
    }

    [Serializable]
    public struct GizmoWeightData
    {
        public int weight;
        public Bus bus;
    }

    [Serializable]
    public struct GizmoDreamyNumData
    {
        public Vector3 pos;
        public int num;
    }

    [Serializable]
    public struct BusColorData
    {
        public int num;
        public JunkColor color;

        public override string ToString()
        {
            return color.ToString() + "  " + num.ToString();
        }
    }

    [Serializable]
    public struct BusColorDataWithBackTrack
    {
        public int num;
        public JunkColor color;
        public List<Bus> backTrackList;

        public override string ToString()
        {
            return color.ToString() + "  " + num.ToString();
        }

        public BusColorData GetColorData()
        {
            return new BusColorData()
            {
                color = color,
                num = num,
            };
        }

        public void DecreaseWeight()
        {
            for (int i = 0; i < backTrackList.Count; i++)
            {
                var bus = backTrackList[i];
                if (!bus.IsOutOfTunnel) bus.Weight--;
            }
        }
    }
}