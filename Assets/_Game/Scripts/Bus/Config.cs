using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Bus
{
    [CreateAssetMenu(menuName = "SO/Config", fileName = "Config")]
    public class Config : ScriptableObject
    {
        public static Config cur;

        [Header("Bus Map")] public float closeDistanceBetweenBuses;
        public float bounceBackDistance;

        public float busMoveSpeed;
        public float minionMoveSpeed;
        public float minionQueueSegmentMoveTime = .18f;

        public float busHeight;
        public float vanHeight;
        public float carHeight;

        public float delayBetweenBusLeave = .72f;

        public float averageBusParkPathLength = 15;
        public float averageBusLeavePathLength = 5.2f;

        public float delayBetweenSwapCarBooster = .024f;
        public float delayBetweenSwapMinionBooster = .018f;

        [Header("In Game")] public int goldPerWin = 10;
        public int goldPerRevive = 150;
        public int goldPerVip = 2;

        public static int SwapCarTutLevel => AdsManager.Ins.SwapCarTutLevel;
        public static int VipBusTutLevel => AdsManager.Ins.VipBUsTutLevel;
        public static int SwapMinionTutLevel => AdsManager.Ins.SwapMinionTutLEvel;

        [Header("Treasure Item")] public Vector3 treasureHomeDisplaySize;
        public Vector3 treasureHomeShowSize;
#if UNITY_EDITOR
        private void OnEnable()
        {
            cur = this;
        }
#else
    private void Awake()
    {
        cur = this;
    }
#endif
    }
}