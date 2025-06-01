using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RaymarchCam : MonoBehaviour
{
    public Renderer rend;
    public Material raymarchMat;

    public new Transform transform;
    public Camera cam;

    public bool isRend;

    public Mesh mesh;

    public MeshFilter meshFilter;

    private static int CAM_FRUSTUM = Shader.PropertyToID("CamFrustum");
    private static int CAM_2_WORLD_MATRIX = Shader.PropertyToID("C2WMatrix");
    private static int CAM_WORLD_SPACE = Shader.PropertyToID("CamWorldSpace");

    private void Awake()
    {
        raymarchMat = rend.material;

        Mesh mesh = meshFilter.mesh;
        var vert = mesh.vertices;
        for (int i = 0; i < vert.Length; i++)
        {
            if (vert[i].x < 0 && vert[i].y > 0) vert[i].z = 0;
            else if (vert[i].x > 0 && vert[i].y > 0) vert[i].z = 1;
            else if (vert[i].x < 0 && vert[i].y < 0) vert[i].z = 2;
            else if (vert[i].x > 0 && vert[i].y < 0) vert[i].z = 3;
        }

        mesh.vertices = vert;
        meshFilter.mesh = mesh;

        RenderPipelineManager.endCameraRendering += OnEndRender;
    }

    private void Update()
    {
        if (isRend) return;
        raymarchMat.SetMatrix(CAM_FRUSTUM, Camfrustum());
        raymarchMat.SetMatrix(CAM_2_WORLD_MATRIX, cam.cameraToWorldMatrix);
    }


    private void OnEndRender(ScriptableRenderContext context, Camera camera)
    {
        if (camera != cam || !isRend) return;
        // Debug.Log("CAMMM");

        raymarchMat.SetMatrix(CAM_FRUSTUM, Camfrustum());
        raymarchMat.SetMatrix(CAM_2_WORLD_MATRIX, cam.cameraToWorldMatrix);

        this.DrawFullQuadIndex(raymarchMat, mesh);
    }

    private Matrix4x4 Camfrustum()
    {
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = -Vector3.forward - goRight + goUp;
        Vector3 TR = -Vector3.forward + goRight + goUp;
        Vector3 BL = -Vector3.forward - goRight - goUp;
        Vector3 BR = -Vector3.forward + goRight - goUp;

        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BL);
        frustum.SetRow(3, BR);

        return frustum;
    }
}