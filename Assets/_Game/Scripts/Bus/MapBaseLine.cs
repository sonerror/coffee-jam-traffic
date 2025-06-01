using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBaseLine : MonoBehaviour
{
    public Transform root;

    private Quaternion upLeftDir = Quaternion.Euler(0, -60, 0);
    private Quaternion upRightDir = Quaternion.Euler(0, 30, 0);

    [SerializeField] private float offsetAngle;

    [SerializeField] private Color gridColor;

    [SerializeField] private int res = 12;
    [SerializeField] private float width = .96f;

    [SerializeField] private bool isDraw;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isDraw = !isDraw;
        }
    }

    private void OnDrawGizmos()
    {
        if (root == null || !isDraw) return;
        var offset = Quaternion.Euler(0, offsetAngle, 0);

        for (int i = 0; i < res; i++)
        {
            var position = root.position;
            Draw(position + (offset * upLeftDir * Vector3.forward) * i * width, upRightDir);
            Draw(position + (offset * upRightDir * Vector3.forward) * i * width, upLeftDir);
            Draw(position - (offset * upLeftDir * Vector3.forward) * i * width, upRightDir);
            Draw(position - (offset * upRightDir * Vector3.forward) * i * width, upLeftDir);
        }

        return;

        void Draw(Vector3 pos, Quaternion dir)
        {
            var offset = Quaternion.Euler(0, offsetAngle, 0);
            var start = pos + offset * dir * Vector3.forward * 20;
            var end = pos - offset * dir * Vector3.forward * 20;

            Gizmos.color = gridColor;
            Gizmos.DrawLine(start, end);
        }
    }
}