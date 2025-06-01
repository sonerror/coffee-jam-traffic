using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshPoint : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Mesh mesh;
    public Vector3[] verticles;
    public int index;
    private int instanceIndex;
    public Transform indicator;
    public Transform oneindicator;
    public Transform twoindicator;

    private Vector3 one;
    private Vector3 two;
    public void Init()
    {
        mesh = meshFilter.mesh;
        verticles = mesh.vertices;
        index = 0;
        instanceIndex = 0;
    }
    public void DisPlay()
    {
        indicator.position = new Vector3(verticles[index].x, verticles[index].y, verticles[index].z);
    }
    public void DisplayNext()
    {
        index++;
        DisPlay();
    }

    public void DisPlayPrev()
    {
        index--;
        DisPlay();
    }

    public void Take1()
    {
        one = indicator.position;
        oneindicator.position = one;
    }

    public void Take2()
    {
        two = indicator.position;
        twoindicator.position = two;
    }

    public void GetPoint()
    {
        GameObject go = new GameObject("Point " + instanceIndex);
        go.transform.position = (one + two) / 2;
        instanceIndex++;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshPoint))]
public class MEshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (MeshPoint)target;

        if (GUILayout.Button("Init"))
        {
            script.Init();
        }
        if (GUILayout.Button("Display"))
        {
            script.DisPlay();
        }
        if (GUILayout.Button("Display Next"))
        {
            script.DisplayNext();
        }
        if (GUILayout.Button("Display Prev"))
        {
            script.DisPlayPrev();
        }
        if (GUILayout.Button("Take 1"))
        {
            script.Take1();
        }
        if (GUILayout.Button("Take 2"))
        {
            script.Take2();
        }
        if (GUILayout.Button("Get Point"))
        {
            script.GetPoint();
        }
    }
}
#endif

