 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCubeizer
{
    private static Vector3 A, B, C, D;

    //Display without having to press play
    public static void DrawCubeizerPath(Transform startPoint, Transform controlPointStart, Transform controlPointEnd, Transform endPoint)
    {
        A = startPoint.position;
        B = controlPointStart.position;
        C = controlPointEnd.position;
        D = endPoint.position;

        //The Bezier curve's color
        Gizmos.color = Color.white;

        //The start position of the line
        var lastPos = A;

        //The resolution of the line
        //Make sure the resolution is adding up to 1, so 0.3 will give a gap at the end, but 0.2 will work
        var resolution = 0.02f;

        //How many loops?
        var loops = Mathf.FloorToInt(1f / resolution);

        for (var i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            var t = i * resolution;

            //Find the coordinates between the control points with a Catmull-Rom spline
            var newPos = DeCasteljausAlgorithm(t);

            //Draw this line segment
            Gizmos.DrawLine(lastPos, newPos);

            //Save this pos so we can draw the next line segment
            lastPos = newPos;
        }

        //Also draw lines between the control points and endpoints
        Gizmos.color = Color.green;

        Gizmos.DrawLine(A, B);
        Gizmos.DrawLine(C, D);
    }

    //The De Casteljau's Algorithm
    public static Vector3 DeCasteljausAlgorithm(float t)
    {
        //Linear interpolation = lerp = (1 - t) * A + t * B
        //Could use Vector3.Lerp(A, B, t)

        //To make it faster
        var oneMinusT = 1f - t;

        //Layer 1
        var Q = oneMinusT * A + t * B;
        var R = oneMinusT * B + t * C;
        var S = oneMinusT * C + t * D;

        //Layer 2
        var P = oneMinusT * Q + t * R;
        var T = oneMinusT * R + t * S;

        //Final interpolated position
        var U = oneMinusT * P + t * T;

        return U;
    }
}
