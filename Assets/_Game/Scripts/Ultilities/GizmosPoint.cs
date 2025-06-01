using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosPoint : MonoBehaviour
{
    public Transform[] t;

    private void OnDrawGizmos()
    {
        for (var i = 0; i < t.Length; i++)
        {
            Gizmos.DrawWireSphere(t[i].position, 1f);
        }
    }
}
