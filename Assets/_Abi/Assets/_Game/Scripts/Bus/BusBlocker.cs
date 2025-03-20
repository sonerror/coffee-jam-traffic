using System;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusBlocker : Bus
    {
        private void Start()
        {
            IsStayOnBusZone = true;
        }

        public override void GetHit(Transform hitterTrans, Vector3 surfacePoint)
        {
            AudioManager.ins.PlaySound(SoundType.BusCollide);
            ProjectileManager.ins.Pop(ProjectileType.HitSpark, surfacePoint, hitterTrans.rotation);
        }
    }
}