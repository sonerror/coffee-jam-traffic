using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {
        [Header("Road settings")]
        public float roadWidth = .4f;
        // [Range(0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;
        public bool isCreateUnderMesh = true;
        public bool isShittyRoad;

        [Header("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;

        [SerializeField]
        public GameObject meshHolder;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        public Mesh mesh;

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                AssignMeshComponents();
                AssignMaterials();
                if (!isShittyRoad) CreateRoadMesh(); else CreateShittyMesh();
            }
        }

        void CreateRoadMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8 + 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];
            int[] backPlate = new int[6];
            int[] frontPlate = new int[6];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));


                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        if (isCreateUnderMesh)
                            underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }
                if (i == 0)
                {
                    verts[path.NumPoints * 8 + 0] = verts[vertIndex + 0];
                    verts[path.NumPoints * 8 + 1] = verts[vertIndex + 1];
                    verts[path.NumPoints * 8 + 2] = verts[vertIndex + 2];
                    verts[path.NumPoints * 8 + 3] = verts[vertIndex + 3];

                    Vector3 localBack = -Vector3.Cross(localRight, localUp);

                    normals[path.NumPoints * 8 + 0] = localBack;
                    normals[path.NumPoints * 8 + 1] = localBack;
                    normals[path.NumPoints * 8 + 2] = localBack;
                    normals[path.NumPoints * 8 + 3] = localBack;

                    int tmp = path.NumPoints * 8;
                    backPlate = new int[] { tmp + 0, tmp + 1, tmp + 2, tmp + 2, tmp + 1, tmp + 3 };
                }
                if (i == path.NumPoints - 1)
                {

                    verts[path.NumPoints * 8 + 4] = verts[vertIndex + 0];
                    verts[path.NumPoints * 8 + 5] = verts[vertIndex + 1];
                    verts[path.NumPoints * 8 + 6] = verts[vertIndex + 2];
                    verts[path.NumPoints * 8 + 7] = verts[vertIndex + 3];

                    Vector3 localFoward = Vector3.Cross(localRight, localUp);

                    normals[path.NumPoints * 8 + 4] = localFoward;
                    normals[path.NumPoints * 8 + 5] = localFoward;
                    normals[path.NumPoints * 8 + 6] = localFoward;
                    normals[path.NumPoints * 8 + 7] = localFoward;

                    int tmp = path.NumPoints * 8;
                    frontPlate = new int[] { tmp + 5, tmp + 4, tmp + 7, tmp + 7, tmp + 4, tmp + 6 };

                }

                vertIndex += 8;
                triIndex += 6;

            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = isCreateUnderMesh ? 5 : 4;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(sideOfRoadTriangles, 1);
            mesh.SetTriangles(backPlate, 2);
            mesh.SetTriangles(frontPlate, 3);
            if (isCreateUnderMesh)
                mesh.SetTriangles(underRoadTriangles, 4);
            mesh.RecalculateBounds();
        }

        public float shittyRadius = .3f;
        public float shittyOffset;
        void CreateShittyMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 6];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 12 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] backPlate = new int[12];
            int[] frontPlate = new int[12];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 1, 6, 6, 1, 7, 1, 2, 7, 7, 2, 8, 2, 3, 8, 8, 3, 9, 3, 4, 9, 9, 4, 10, 4, 5, 10, 10, 5, 11, 5, 0, 11, 11, 0, 6 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));
                Vector3 localForward = path.GetTangent(i);

                Vector3 localOff_0 = Quaternion.AngleAxis(30, localForward) * localUp;
                Vector3 localOff_1 = Quaternion.AngleAxis(90, localForward) * localUp;
                Vector3 localOff_2 = Quaternion.AngleAxis(150, localForward) * localUp;
                Vector3 localOff_3 = Quaternion.AngleAxis(210, localForward) * localUp;
                Vector3 localOff_4 = Quaternion.AngleAxis(270, localForward) * localUp;
                Vector3 localOff_5 = Quaternion.AngleAxis(330, localForward) * localUp;

                // Find center position of current path vertex
                Vector3 center = path.GetPoint(i) + localRight * shittyOffset - localUp * 0.86602540378f * shittyRadius;

                //Add suround points
                verts[vertIndex + 0] = center + localOff_0 * shittyRadius;
                verts[vertIndex + 1] = center + localOff_1 * shittyRadius;
                verts[vertIndex + 2] = center + localOff_2 * shittyRadius;
                verts[vertIndex + 3] = center + localOff_3 * shittyRadius;
                verts[vertIndex + 4] = center + localOff_4 * shittyRadius;
                verts[vertIndex + 5] = center + localOff_5 * shittyRadius;

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(0.2f, path.times[i]);
                uvs[vertIndex + 2] = new Vector2(0.4f, path.times[i]);
                uvs[vertIndex + 3] = new Vector2(0.6f, path.times[i]);
                uvs[vertIndex + 4] = new Vector2(0.8f, path.times[i]);
                uvs[vertIndex + 5] = new Vector2(1, path.times[i]);

                // Shit load of road normals
                normals[vertIndex + 0] = localOff_0;
                normals[vertIndex + 1] = localOff_1;
                normals[vertIndex + 2] = localOff_2;
                normals[vertIndex + 3] = localOff_3;
                normals[vertIndex + 4] = localOff_4;
                normals[vertIndex + 5] = localOff_5;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    }
                }
                if (i == 0)
                {
                    backPlate = new int[] { 0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 5, 4 };
                }
                if (i == path.NumPoints-1)
                {
                    frontPlate = new int[] { i * 6 + 0, i * 6 + 1, i * 6 + 2, i * 6 + 0, i * 6 + 2, i * 6 + 3, i * 6 + 0, i * 6 + 3, i * 6 + 4, i * 6 + 0, i * 6 + 4, i * 6 + 5 };
                }

                vertIndex += 6;
                triIndex += 36;
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 3;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(backPlate, 1);
            mesh.SetTriangles(frontPlate, 2);
            mesh.RecalculateBounds();
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {

            if (meshHolder == null)
            {
                meshHolder = new GameObject("Road ");
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            // Ensure mesh renderer and filter components are assigned
            if (!meshHolder.gameObject.GetComponent<MeshFilter>())
            {
                meshHolder.gameObject.AddComponent<MeshFilter>();
            }
            if (!meshHolder.GetComponent<MeshRenderer>())
            {
                meshHolder.gameObject.AddComponent<MeshRenderer>();
            }

            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            meshFilter = meshHolder.GetComponent<MeshFilter>();
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            meshFilter.mesh = mesh;
        }

        void AssignMaterials()
        {
            if (roadMaterial != null && undersideMaterial != null)
            {
                if (isCreateUnderMesh)
                    meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial, undersideMaterial, undersideMaterial };
                else
                    meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial, undersideMaterial };
                // meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
            }
        }

    }
}