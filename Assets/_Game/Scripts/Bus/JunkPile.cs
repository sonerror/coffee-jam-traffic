using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    public class JunkPile : MonoBehaviour
    {
        public static JunkPile ins;
        public ShipperController ShipperPrefab;
        private readonly Stack<Bus> carPool = new();
        public Bus carPrefab;

        private readonly Stack<Bus> vanPool = new();
        public Bus vanPrefab;

        private readonly Stack<Bus> busPool = new();
        public Bus busPrefab;
        public Material[] busMats;
        public Material[] busMatCovers;

        private readonly Stack<BusAmbulance> ambulancePool = new();
        public Bus ambulancePrefabs;

        private readonly Stack<BusPolice> policePool = new();
        public Bus policePrefab;

        private readonly Stack<BusFireTruck> fireTruckPool = new();
        public Bus fireTruckPrefab;

        private readonly Stack<BusVipProMax> vipProMaxPool = new();
        public Bus vipProMaxPrefab;

        private readonly Stack<Minion> minionPool = new();
        public Minion minionPrefab;
        public MatrixPrime<Material> minionMats;

        private readonly Stack<BusTunnel> busTunnelPool = new();
        public BusTunnel busTunnelPrefab;

        private readonly Stack<CountDownNote> countDownNotePool = new();

        [FormerlySerializedAs("countDownNotePrefabs")]
        public CountDownNote countDownNotePrefab;

        private readonly List<Bus> activeBuses = new List<Bus>();
        private readonly List<Minion> activeMinions = new List<Minion>();
        private readonly List<BusTunnel> activeTunnels = new List<BusTunnel>();
        private readonly List<CountDownNote> activeCountDownNotes = new List<CountDownNote>();

        public List<Minion> ActiveMinions => activeMinions;

        public Material[] busGrayMats;
        public Material[] busRainbowMats;
        public Color NormalArrowColor;
        public Color CoverArrowColor;
        private void Awake()
        {
            ins = this;
        }

        public Bus GetBus(BusType type, JunkColor junkColor, Vector3 pos, Quaternion rotation,Vector3 localScale)
        {
            var variation = type switch
            {
                BusType.Car => carPool.Count > 0 ? carPool.Pop() : Instantiate(carPrefab),
                BusType.Van => vanPool.Count > 0 ? vanPool.Pop() : Instantiate(vanPrefab),
                BusType.Bus => busPool.Count > 0 ? busPool.Pop() : Instantiate(busPrefab),
            };
            variation.transform.SetPositionAndRotation(pos, rotation);
            variation.transform.localScale=localScale;
            variation.RootScale = localScale;
            variation.transform.SetParent(null);
            variation.gameObject.SetActive(true);
            /*variation.mainColorRend[0].sharedMaterial = busMats[(int)junkColor];
            variation.mainColorRend[1].sharedMaterial = busMats[(int)junkColor];*/
            SetRendersColor(variation.mainColorRend,busMats[(int)junkColor]);
            variation.color = junkColor;
            variation.IsPushed = false;

            if (!variation.IsInActiveList)
            {
                activeBuses.Add(variation);
                variation.IsInActiveList = true;
            }

            variation.OnInit();
            return variation;
        }

        public ShipperController GetShipper()
        {
            return Instantiate(ShipperPrefab);
        }
        private void SetRenderColor(Renderer renderer,Material mat)
        {
            var listColors = new List<Material>();
            for (int i = 0; i < renderer.sharedMaterials.Length;i++)
            {
                listColors.Add(mat);
            }

            renderer.sharedMaterials = listColors.ToArray();
        }
        
        private void SetRendersColor(IEnumerable<Renderer> renderers,Material mat)
        {
            foreach (var render in renderers)
            {
                SetRenderColor(render,mat);
            }
        }
        
        public Bus GetSpecialBus(JunkColor junkColor, Vector3 pos, Quaternion rotation,Vector3 localScale)
        {
            var variation = junkColor switch
            {
                JunkColor.Ambulance => ambulancePool.Count > 0 ? ambulancePool.Pop() : Instantiate(ambulancePrefabs),
                JunkColor.PoliceCar => policePool.Count > 0 ? policePool.Pop() : Instantiate(policePrefab),
                JunkColor.FireTruck => fireTruckPool.Count > 0 ? fireTruckPool.Pop() : Instantiate(fireTruckPrefab),
                JunkColor.VipProMax => vipProMaxPool.Count > 0 ? vipProMaxPool.Pop() : Instantiate(vipProMaxPrefab),
                _ => null,
            };

            if (variation == null) return variation;

            variation.transform.SetPositionAndRotation(pos, rotation);
            variation.transform.localScale = localScale;
            variation.RootScale = localScale;
            variation.transform.SetParent(null);
            variation.gameObject.SetActive(true);
            variation.color = junkColor;
            variation.IsPushed = false;

            if (!variation.IsInActiveList)
            {
                activeBuses.Add(variation);
                variation.IsInActiveList = true;
            }

            variation.OnInit();
            return variation;
        }

        public Minion GetMinion(JunkColor junkColor)
        {
            var variation = minionPool.Count > 0 ? minionPool.Pop() : Instantiate(minionPrefab);

            variation.transform.SetParent(null);
            variation.gameObject.SetActive(true);
            /*
            variation.rend.sharedMaterials = minionMats[(int)junkColor];
            */
            variation.SetRender(minionMats[(int)junkColor][0]);
            variation.Color = junkColor;
            variation.IsPushed = false;

            if (IsSpecialType(junkColor))
            {
                var targetType = junkColor switch
                {
                    JunkColor.Ambulance => AnimMeshPlayer.AnimSpecialType.Ambulance,
                    JunkColor.PoliceCar => AnimMeshPlayer.AnimSpecialType.Police,
                    JunkColor.FireTruck => AnimMeshPlayer.AnimSpecialType.FireTruck,
                    JunkColor.VipProMax => AnimMeshPlayer.AnimSpecialType.Vip,
                };

                variation.anim.SwitchType(targetType);
            }
            else
            {
                variation.anim.SwitchType(AnimMeshPlayer.AnimSpecialType.Normal);
            }

            if (!variation.IsInActiveList)
            {
                activeMinions.Add(variation);
                variation.IsInActiveList = true;
            }

            variation.OnInit();

            return variation;
        }

        public BusTunnel GetTunnel()
        {
            var variation = busTunnelPool.Count > 0 ? busTunnelPool.Pop() : Instantiate(busTunnelPrefab);

            variation.transform.SetParent(null);
            variation.gameObject.SetActive(true);
            variation.IsPushed = false;

            activeTunnels.Add(variation);

            variation.OnInit();

            return variation;
        }

        public CountDownNote GetCountDownNote()
        {
            var variation = countDownNotePool.Count > 0 ? countDownNotePool.Pop() : Instantiate(countDownNotePrefab);

            variation.gameObject.SetActive(true);
            variation.IsPushed = false;

            activeCountDownNotes.Add(variation);

            variation.OnInit();

            return variation;
        }

        public void Push(Bus bus)
        {
            if(bus==null) return;
            if (bus.IsPushed) return;
            bus.IsPushed = true;
            bus.transform.SetParent(null);
            bus.gameObject.SetActive(false);

            if (IsSpecialType(bus.color))
            {
                switch (bus.color)
                {
                    case JunkColor.Ambulance:
                        ambulancePool.Push(bus as BusAmbulance);
                        break;
                    case JunkColor.PoliceCar:
                        policePool.Push(bus as BusPolice);
                        break;
                    case JunkColor.FireTruck:
                        fireTruckPool.Push(bus as BusFireTruck);
                        break;
                    case JunkColor.VipProMax:
                        vipProMaxPool.Push(bus as BusVipProMax);
                        break;
                }
            }
            else
            {
                switch (bus.Type)
                {
                    case BusType.Car:
                        carPool.Push(bus);
                        break;
                    case BusType.Van:
                        vanPool.Push(bus);
                        break;
                    case BusType.Bus:
                        busPool.Push(bus);
                        break;
                }
            }
            bus.OnPush();
        }

        public void Push(Minion minion)
        {
            if (minion.IsPushed) return;
            minion.IsPushed = true;
            minion.gameObject.SetActive(false);
            minion.CurrentMoveToBusTween?.Kill();
            minionPool.Push(minion);
        }

        public void Push(BusTunnel busTunnel)
        {
            if (busTunnel.IsPushed) return;
            busTunnel.IsPushed = true;
            busTunnel.gameObject.SetActive(false);
            busTunnel.OnPush();
            busTunnelPool.Push(busTunnel);
        }

        public void Push(CountDownNote countDownNote)
        {
            if (countDownNote.IsPushed) return;
            countDownNote.IsPushed = true;
            countDownNote.gameObject.SetActive(false);
            countDownNote.OnPush();
            countDownNotePool.Push(countDownNote);
        }

        public void RecallAll()
        {
            activeBuses.Clear();;
            activeBuses.AddRange(FindObjectsByType<Bus>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList());
            for (int i = 0; i < activeBuses.Count; i++)
            {
                var active = activeBuses[i];
                if(active==null) continue;;
                Push(active);
                active.IsInActiveList = false;
            }

            for (int i = 0; i < activeMinions.Count; i++)
            {
                var active = activeMinions[i];
                Push(active);
                active.IsInActiveList = false;
            }

            for (int i = 0; i < activeTunnels.Count; i++) Push(activeTunnels[i]);
            for (int i = 0; i < activeCountDownNotes.Count; i++) Push(activeCountDownNotes[i]);

            activeBuses.Clear();
            activeMinions.Clear();
            activeTunnels.Clear();
            activeCountDownNotes.Clear();
        }

        public void ChangeColor(Bus bus, JunkColor color,bool isCovered=false)
        {
            bus.color = color;
            if (isCovered)
            {
                SetRendersColor(bus.mainColorRend,busMatCovers[(int)color]);
            }
            else
            {
                SetRendersColor(bus.mainColorRend,busMats[(int)color]);
            }
            /*
            bus.mainColorRend[0].sharedMaterial = busMats[(int)color];
            bus.mainColorRend[1].sharedMaterial = busMats[(int)color];*/
        }

        public void ChangeBusMat(Bus bus,Material mat)
        {
            SetRendersColor(bus.mainColorRend,mat);

        }
        public Material GetBusColorByJunkColor(JunkColor color)
        {
            return busMats[(int)color];
        }
        public Material GetBusCoverColorByJunkColor(JunkColor color)
        {
            return busMatCovers[(int)color];
        }
        public Material GetMininonColorByBus(Bus bus)
        {
            return minionMats[(int)bus.color][0];
        }
        public void ChangeColor(Minion minion, JunkColor color)
        {
            minion.Color = color;
            /*
            minion.rend.sharedMaterials = minionMats[(int)color];
            */
            minion.SetRender(minionMats[(int)color][0]);
            if (IsSpecialType(color))
            {
                var targetType = color switch
                {
                    JunkColor.Ambulance => AnimMeshPlayer.AnimSpecialType.Ambulance,
                    JunkColor.PoliceCar => AnimMeshPlayer.AnimSpecialType.Police,
                    JunkColor.FireTruck => AnimMeshPlayer.AnimSpecialType.FireTruck,
                    JunkColor.VipProMax => AnimMeshPlayer.AnimSpecialType.Vip,
                };

                minion.anim.SwitchType(targetType);
            }
            else
            {
                minion.anim.SwitchType(AnimMeshPlayer.AnimSpecialType.Normal);
            }
        }

        public void ChangeToGray(Bus bus, JunkColor color)
        {
            var mat = new Material(busGrayMats[(int)color]);
            bus.CurrentGrayMat = mat;
            
            /*bus.mainColorRend[0].sharedMaterial = mat;
            bus.mainColorRend[1].sharedMaterial = mat;*/
            SetRendersColor(bus.mainColorRend,mat);

            mat.SetFloat(Variables.FLOAT, 0);
        }

        public void ChangeToRainBow(Bus bus, JunkColor color)
        {
            var mat = busRainbowMats[(int)color];
            SetRendersColor(bus.mainColorRend,mat);

            /*
            bus.mainColorRend[0].sharedMaterial = mat;
        */
        }

        public float GetScaleFactor()
        {
            float GetScale()
            {
                return BusLevelSO.active.ScaleFactor>0?BusLevelSO.active.ScaleFactor:1;
            }

            if (LevelDataFragment.IsMultiLayer)
            {
                return LevelDataFragment.cur.gameData.level switch
                {
                    //Scale level hard/super hard 2 layer 
                     < 2 => 1.3f,
                     < 4 => 1.15f,
                     4 => 1.22f,
                     5 => 1.0f,
                     6 => 1.2f,
                     9 => 1.31f,
                     11 => 1.21f,
                     14 => 1.085f,
                     15 => 1.01f,
                     19 => 1.22f,
                     24 => 1.25f,
                     29 => 1.09f,
                     26 => 1.21f,
                     39 => 1.18f,
                     66 => 1.02f,
                    //Scale level full 1 layer
                    _ => GetScale(),
                };
            }
            else
            {
                return LevelDataFragment.cur.gameData.level switch
                {
                    < 2 => 1.3f,
                    < 4 => 1.15f,
                    5 => 1.0f,
                    6 => 1.2f,
                    9 => 1.14f,
                    11 => 1.21f,
                    15 => 1.01f,
                    24 => 1.06f,
                    29 => 1.09f,
                    26 => 1.21f,
                    39 => 1.18f,
               
                    66 => 1.02f,
                    _ => GetScale(),
                };
            }
            
           
            
            
        }

        public bool IsSpecialType(JunkColor color)
        {
            return (int)color > 10;
        }

        public static bool IsSpecialColorType(JunkColor color)
        {
            return (int)color > 10;
        }
    }

    public enum JunkColor
    {
        Blue = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Purple = 4,
        Pink = 5,
        Brown = 6,
        Cyan = 7,
        Orange = 8,
        Black = 9,
        White = 10,
        Ambulance = 11,
        PoliceCar = 12,
        FireTruck = 13,
        VipProMax = 14,
    }

    public enum BusType
    {
        Car,
        Van,
        Bus,
    }
}