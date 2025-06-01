using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineAdjustTransformHelper : MonoBehaviour
{
    public SplineContainer SplineContainer;
    public float FirstOffset;
    public float OffsetBetween;
    public Transform[] Points;

    private void OnValidate()
    {
        AdjustPoints();
    }

    private void AdjustPoints()
    {
        if (SplineContainer == null || Points == null || Points.Length == 0)
        {
            return;
        }

        float totalDistance = FirstOffset;

        for (int i = 0; i < Points.Length; i++)
        {
            if (SplineContainer != null)
            {
                Points[i].position = SplineContainer.EvaluatePosition(totalDistance / this.SplineContainer.CalculateLength());
                totalDistance += OffsetBetween;
            }
        }
    }
}
