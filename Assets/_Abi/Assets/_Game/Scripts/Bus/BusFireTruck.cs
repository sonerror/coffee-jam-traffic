using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusFireTruck : Bus
    {
        public override void OnInit()
        {
            base.OnInit();

            // BusStation.cur.groundBus.Remove(this);
        }

        public override void OnPostInit()
        {
            base.OnPostInit();

            // BusStation.cur.groundBus.Remove(this);
        }
    }
}