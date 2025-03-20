using System;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class AnimMesher : MonoBehaviour
    {
        public static AnimMesher cur;

        public Mesh[] idleMeshes;
        public Mesh[] runMeshes;
        public Mesh[] sitMeshes;

        public Mesh[] ambulanceIdleMeshes;
        public Mesh[] ambulanceRunMeshes;
        public Mesh[] ambulanceSitMeshes;

        public Mesh[] policeIdleMeshes;
        public Mesh[] policeRunMeshes;
        public Mesh[] policeSitMeshes;

        public Mesh[] fireTruckIdleMeshes;
        public Mesh[] fireTruckRunMeshes;
        public Mesh[] fireTruckSitMeshes;

        public Mesh[] vipIdleMeshes;
        public Mesh[] vipRunMeshes;
        public Mesh[] vipSitMeshes;

        private int idleCount;
        private int runCount;
        private int sitCount;

        [SerializeField] private float idleTimeToIndex; //fps
        [SerializeField] private float runTimeToIndex;
        [SerializeField] private float sitTimeToIndex;

        private void Awake()
        {
            cur = this;

            idleCount = idleMeshes.Length;
            runCount = runMeshes.Length;
            sitCount = sitMeshes.Length;
        }

        public Mesh GetIdle(float time)
        {
            var index = Mathf.FloorToInt(time * idleTimeToIndex);
            return idleMeshes[index % idleCount];
        }

        public Mesh GetRun(float time)
        {
            var index = Mathf.FloorToInt(time * runTimeToIndex);
            return runMeshes[index % runCount];
        }

        public Mesh GetSit(float time)
        {
            var index = Mathf.FloorToInt(time * sitTimeToIndex);
            return sitMeshes[index % sitCount];
        }

        public Mesh GetSpecialIdle(float time, int flag)
        {
            var index = Mathf.FloorToInt(time * idleTimeToIndex) % idleCount;
            return flag switch
            {
                1 => ambulanceIdleMeshes[index],
                2 => policeIdleMeshes[index],
                3 => fireTruckIdleMeshes[index],
                4 => vipIdleMeshes[index],
                _ => idleMeshes[index],
            };
        }

        public Mesh GetSpecialRun(float time, int flag)
        {
            var index = Mathf.FloorToInt(time * runTimeToIndex) % runCount;
            return flag switch
            {
                1 => ambulanceRunMeshes[index],
                2 => policeRunMeshes[index],
                3 => fireTruckRunMeshes[index],
                4 => vipRunMeshes[index],
                _ => runMeshes[index],
            };
        }

        public Mesh GetSpecialSit(float time, int flag)
        {
            var index = Mathf.FloorToInt(time * sitTimeToIndex) % sitCount;
            return flag switch
            {
                1 => ambulanceSitMeshes[index],
                2 => policeSitMeshes[index],
                3 => fireTruckSitMeshes[index],
                4 => vipSitMeshes[index],
                _ => sitMeshes[index],
            };
        }
    }
}