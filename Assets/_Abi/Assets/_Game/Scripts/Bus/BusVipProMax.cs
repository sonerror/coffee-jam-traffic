using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusVipProMax : Bus
    {
        public override void OnInit()
        {
            base.OnInit();

            // BusStation.cur.groundBus.Remove(this);
        }

        public override void OnPostInit()
        {
            base.OnPostInit();

            // Debug.Log("POST", gameObject);

            // BusStation.cur.groundBus.Remove(this);
        }
    }
}