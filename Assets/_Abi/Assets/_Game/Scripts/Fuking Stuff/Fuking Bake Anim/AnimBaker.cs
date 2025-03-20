using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Fuking_Stuff.Fuking_Bake_Anim
{
    public class AnimBaker : MonoBehaviour
    {
        public Animator animator;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public Transform skinnedMeshTrans;

        [SerializeField] private string animName;
        [SerializeField] private float step;
        [SerializeField] private string path;
        [SerializeField] private string preName;

        [SerializeField] private Animator subAnim;
        public SkinnedMeshRenderer subSkinnedMeshRenderer;
        public Transform subSkinMeshTrans;

        [SerializeField] private bool isSubMesh;

        [SerializeField] private Mesh[] modifyMeshes;

        private void Start()
        {
            animator.speed = 0;
            if (isSubMesh) subAnim.speed = 0;
        }
#if UNITY_EDITOR
        [ContextMenu("BAKE")]
        public void Bake()
        {
            StartCoroutine(ie_Bake());
            return;

            IEnumerator ie_Bake()
            {
                var anim = Animator.StringToHash(animName);
                int num = 0;

                if (isSubMesh)
                {
                    for (float i = 0; i < 1; i += step)
                    {
                        animator.Play(anim, 0, i);
                        subAnim.Play(anim, 0, i);
                        yield return null;
                        yield return null;

                        var mesh = new Mesh();
                        var subMesh = new Mesh();
                        skinnedMeshRenderer.BakeMesh(mesh, true);
                        subSkinnedMeshRenderer.BakeMesh(subMesh, true);

                        CombineInstance[] cb = new CombineInstance[2];

                        cb[0].mesh = subMesh;
                        cb[0].transform = subSkinMeshTrans.localToWorldMatrix;

                        cb[1].mesh = mesh;
                        cb[1].transform = skinnedMeshTrans.localToWorldMatrix;

                        var finalMesh = new Mesh();
                        finalMesh.CombineMeshes(cb);

                        AssetDatabase.CreateAsset(finalMesh, path + preName + " " + num + ".asset");
                        num++;

                        yield return Yielders.Get(0.25f);
                    }

                    AssetDatabase.SaveAssets();
                }
                else
                {
                    for (float i = 0; i < 1; i += step)
                    {
                        animator.Play(anim, 0, i);
                        yield return null;
                        yield return null;

                        var mesh = new Mesh();
                        skinnedMeshRenderer.BakeMesh(mesh, true);

                        var verts = mesh.vertices;
                        var normal = mesh.normals;

                        for (int j = 0; j < verts.Length; j++)
                        {
                            var v = verts[j];
                            verts[j] = skinnedMeshTrans.TransformPoint(v);
                            normal[j] = skinnedMeshTrans.TransformDirection(normal[j]);
                        }

                        mesh.vertices = verts;
                        mesh.normals = normal;

                        AssetDatabase.CreateAsset(mesh, path + preName + " " + num + ".asset");
                        num++;

                        yield return Yielders.Get(0.25f);
                    }

                    AssetDatabase.SaveAssets();
                }
            }
        }

        [ContextMenu("SET READ/WRITE FALSE")]
        public void SetReadWriteFalse()
        {
            for (int i = 0; i < modifyMeshes.Length; i++)
            {
                SerializedObject s = new SerializedObject(modifyMeshes[i]);
                s.FindProperty("m_IsReadable").boolValue = false;
                s.ApplyModifiedProperties();
            }
        }
#endif
    }
}