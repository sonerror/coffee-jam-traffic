using TMPro;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class BusAmbulance : Bus
    {
        public static BusAmbulance cur;

        private const int NUM_TILL_DEATH = 6;

        public Transform numBoardTrans;
        public TMP_Text numText;

        private int currentNum;
        private bool isStartTrigger;

        public override void OnInit()
        {
            base.OnInit();

            // BusStation.cur.groundBus.Remove(this);
            currentNum = NUM_TILL_DEATH;

            cur = this;
            isStartTrigger = false;
        }

        public override void OnPostInit()
        {
            base.OnPostInit();

            // BusStation.cur.groundBus.Remove(this);
        }

        public static void TriggerCountDown()
        {
            if (cur == null || cur.isStartTrigger) return;
            cur.isStartTrigger = true;
        }

        public void OnBusMoved()
        {
            if (IsStayOnBusZone && isStartTrigger)
            {
                currentNum--;
                if (currentNum <= 0) TransportCenter.cur.LoseHandle();
            }
        }

        public void UpdateNum()
        {
            numText.text = currentNum.ToString();
        }
    }
}