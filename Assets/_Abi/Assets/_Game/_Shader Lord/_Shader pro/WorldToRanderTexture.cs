// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class WorldToRanderTexture : MonoBehaviour
// {
//     public new Transform transform;
//     [SerializeField] private bool isSetOnSpawn;
//
//     private static Vector2Int renderCenter = new Vector2Int(Mathf.FloorToInt(1024f / 2), Mathf.FloorToInt(1024f / 2));
//     [SerializeField] private float radius = 10;
//     private static float XPosScale = 8;
//     private static float YPosScale = 8;
//     private static Color setupColor = Color.black;
//     private static int[,] baseBuffer = new int[124, 124];
//     private Vector2Int[] buffer;
//     private int bufferLength;
//     private static List<Vector2Int> tempPosList = new List<Vector2Int>();
//     private static Vector2Int tmp = new Vector2Int();
//     private static Vector2Int center;
//     private static Vector2Int curPixel;
//     private static Vector2Int curIrrator;
//     private void Awake()
//     {
//         tempPosList.Clear();
//
//         center = new Vector2Int(Mathf.FloorToInt((float)baseBuffer.GetLength(0) / 2), Mathf.FloorToInt((float)baseBuffer.GetLength(1) / 2));
//         for (int i = 0; i < baseBuffer.GetLength(0); i++)
//         {
//             for (int j = 0; j < baseBuffer.GetLength(1); j++)
//             {
//                 tmp.Set(i, j);
//                 if (Vector2Int.Distance(tmp, center) < radius) tempPosList.Add(new Vector2Int(i - center.x, j - center.y));
//             }
//         }
//
//         buffer = tempPosList.ToArray();
//         bufferLength = buffer.Length;
//     }
//
//     private void Start()
//     {
//         if (isSetOnSpawn) ConvertToRT();
//         MapController.renderTexture.Apply();
//     }
//     public void ConvertToRT()
//     {
//         // Debug.Log("SASA  " + bufferLength + "    " + baseBuffer.Length + "   " + MapController.renderTexture.GetPixel(curPixel.x, curPixel.y) + curPixel + "   " + renderCenter);
//         curPixel.Set(Mathf.FloorToInt(transform.position.x * XPosScale) + renderCenter.x, Mathf.FloorToInt(transform.position.z * YPosScale) + renderCenter.y);
//         for (int i = 0; i < bufferLength; i++)
//         {
//             curIrrator.Set(Mathf.Clamp(buffer[i].x + curPixel.x, 0, 1023), Mathf.Clamp(buffer[i].y + curPixel.y, 0, 1023));
//             if (MapController.markTexture[curIrrator.x, curIrrator.y])
//             {
//                 MapController.renderTexture.SetPixel(curIrrator.x, curIrrator.y, setupColor);
//                 MapController.markTexture[curIrrator.x, curIrrator.y] = false;
//             }
//         }
//
//         //apply and end of update behaviour in player con
//
//     }
// }
