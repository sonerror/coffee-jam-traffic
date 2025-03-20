using System;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class AnimMeshPlayer : MonoBehaviour
    {
        public MeshFilter meshFilter;

        public static int ANIM_IDLE = Animator.StringToHash("IDLE");
        public static int ANIM_RUN = Animator.StringToHash("RUN");
        public static int ANIM_SEAT = Animator.StringToHash("SEAT");

        private int curAnim;
        private float time;

        [SerializeField] private AnimSpecialType animSpecialType;
        private int animFlag;

        public void PlayAnim(int anim)
        {
            // time = 0;
            curAnim = anim;
        }

        /*
        private void Update()
        {
            if (animFlag == 0)
            {
                if (curAnim == ANIM_IDLE)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetIdle(time);
                }
                else if (curAnim == ANIM_RUN)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetRun(time);
                }
                else if (curAnim == ANIM_SEAT)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetSit(time);
                }
            }
            else
            {
                if (curAnim == ANIM_IDLE)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetSpecialIdle(time, animFlag);
                }
                else if (curAnim == ANIM_RUN)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetSpecialRun(time, animFlag);
                }
                else if (curAnim == ANIM_SEAT)
                {
                    meshFilter.sharedMesh = AnimMesher.cur.GetSpecialSit(time, animFlag);
                }
            }

            time += Time.deltaTime;
        }
        */

        public void SwitchType(AnimSpecialType animSpecialType)
        {
            animFlag = (int)animSpecialType;
        }

        public enum AnimSpecialType
        {
            Normal,
            Ambulance,
            Police,
            FireTruck,
            Vip,
        }
    }
}