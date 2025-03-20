using System;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class CountDownNote : MonoBehaviour
    {
        public new GameObject gameObject;
        public new Transform transform;

        public bool IsPushed { get; set; }
        public Minion TargetMinion { get; set; }

        public bool isCount;

        private const int defaultCountNum = 6;
        private const int increaseStep = 5;
        private int countNum;

        [SerializeField] private Vector3 defaultEulerAngle;

        private void Awake()
        {
            transform.eulerAngles = defaultEulerAngle;
        }

        public void OnInit()
        {
        }

        public void OnPush()
        {
            isCount = false;
            TargetMinion = null;
        }

        public void Fetch(Minion minion)
        {
            TargetMinion = minion;
            isCount = true;

            countNum = defaultCountNum;
        }

        public void OnRevive()
        {
            IncreaseCountNum();
        }

        public void IncreaseCountNum()
        {
            countNum = Mathf.Min(countNum + increaseStep, defaultCountNum);
        }

        private void Update()
        {
            if (isCount)
            {
                transform.position = TargetMinion.transform.position + Vector3.up * 1.12f;
            }
        }

        public void OnBusMoveOut()
        {
            countNum--;
            // if()
        }
    }
}