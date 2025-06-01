using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Pot
{
    public static Vector3 one = Vector3.one;
    public static Vector3 zero = Vector3.zero;
    public static Vector3 up = Vector3.up;
    public static Vector3 right = Vector3.right;
    public static Vector3 forward = Vector3.forward;
    private static Vector3 tempVector3 = new Vector3();
    public static Vector3 vIdicator;
    public static Vector3 freeVec = new Vector3();

    public static Vector3 GetVector3(float x, float y, float z = 0)
    {
        tempVector3.x = x;
        tempVector3.y = y;
        tempVector3.z = z;

        return tempVector3;
    }
}
