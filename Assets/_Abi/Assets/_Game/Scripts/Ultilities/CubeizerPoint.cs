using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeizerPoint : MonoBehaviour
{
        public Transform controlPointStart;
        public Transform controlPointEnd;
        public Transform endPoint;
        public Transform StartPoint;

        private void OnDrawGizmos()
        {
                DrawCubeizer.DrawCubeizerPath(StartPoint, controlPointStart, controlPointEnd, endPoint);
        }

        public void GetPathPoint(List<Vector3> path)
        {
                path.Add(StartPoint.position);
                path.Add(controlPointStart.position);
                path.Add(controlPointEnd.position);
        }

        public Vector3[] GetPath()
        {
                return new[] { endPoint.position, controlPointStart.position, controlPointEnd.position };
        }
}
