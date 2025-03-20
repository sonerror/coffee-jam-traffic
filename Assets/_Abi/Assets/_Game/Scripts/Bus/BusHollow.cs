using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusHollow : Bus
    {
        public BusTunnel busTunnel;
        public BoxCollider[] busCollider;

        public void SetCollider(BusType busType)
        {
            boxCollider = busCollider[(int)busType];
        }

        public void SneakInBus(Bus bus)
        {
            bus.nextNodeList.Push(this);
            bus.nextNodeList.Sort((x, y) =>
            {
                Vector3 position = bus.transform.position;
                var xPoint = x.boxCollider.ClosestPoint(position);
                var yPoint = y.boxCollider.ClosestPoint(position);
                return (Vector3.Distance(position, xPoint) - Vector3.Distance(position, yPoint)) < 0 ? 1 : -1; //sort from low to high, but nextNodeList use as stack so invert to make closet bus is last element --> on top of stack
            });
            
            SetDetectableByBus(true);
            bus.Weight += busTunnel.AwaitDataCount;
        }

        public void SetDetectableByBus(bool isOn)
        {
            IsStayOnBusZone = isOn;
            IsOutOfTunnel = !isOn;
        }

        public override void GetHit(Transform hitterTrans, Vector3 surfacePoint)
        {
            busTunnel.CurrentLockBus.GetHit(hitterTrans, surfacePoint);
        }
    }
}