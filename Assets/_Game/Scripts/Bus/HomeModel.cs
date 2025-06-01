using System;
using UnityEngine;

namespace _Game.Scripts.Bus
{
    public class HomeModel : MonoBehaviour
    {
        public static HomeModel cur;

        public new GameObject gameObject;

        public Transform homeCamRestPoint;

        public Transform homeCamRefPoint;

        public GameObject lightObject;

        public Transform tableRestPoint;
        public Transform tableRefPoint;

        private void Awake()
        {
            cur = this;

            var dir = homeCamRestPoint.position - homeCamRefPoint.position;

            float tarRatio = (float)Screen.height / Screen.width;
            float rootRatio = 1920f / 1080;

            var tableDir = tableRestPoint.position - tableRefPoint.position;

            if (tarRatio > rootRatio)
            {
                dir *= tarRatio / rootRatio;
                tableDir *= tarRatio / rootRatio;
            }

            /*homeCamRestPoint.position = homeCamRefPoint.position + dir;
            tableRestPoint.position = tableRefPoint.position + tableDir;*/
        }
    }
}