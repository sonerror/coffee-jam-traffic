using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cam Angle", menuName = "SO/Cam Angle")]
public class CamAngleSO : ScriptableObject
{
    public float[] camMul;

    public float GetMaxMul()
    {
        var value = camMul[0];
        for (int i = 0; i < camMul.Length; i++)
            if (camMul[i] > value)
                value = camMul[i];
        return value;
    }

    public float GetMinMul()
    {
        var value = camMul[0];
        for (int i = 0; i < camMul.Length; i++)
            if (camMul[i] < value)
                value = camMul[i];
        return value;
    }
}