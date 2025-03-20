using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using _Game.Scripts.Bus;

public class BatchCloneAndRenameBusLevelSO : EditorWindow
{
    public List<BusLevelSO> busLevelList = new List<BusLevelSO>();

    public List<int> numbersList_5 = new List<int>
    {
        95, 250, 220, 235, 120, 230, 180, 75, 100, 80, 110, 60, 200, 70, 305, 300, 285, 265,
        270, 345, 255, 310, 85, 330, 130, 245, 155, 320, 340, 295, 165, 210, 160, 275, 225,
        150, 90, 335, 170, 105, 140, 115, 315, 240, 135, 125, 175, 195, 260, 55, 290, 65,
        205, 145, 280, 325, 190, 350, 185, 215
    };

    public List<int> numbersList_ko_5 = new List<int>
    {
        119, 147, 188, 196, 128, 237, 201, 136, 134, 124, 184, 68, 171, 189, 86, 256, 62, 238, 187, 107,
        67, 254, 113, 108, 241, 54, 51, 78, 234, 52, 146, 281, 138, 186, 198, 274, 93, 242, 91, 232,
        209, 58, 173, 272, 121, 99, 157, 261, 268, 211, 247, 179, 271, 139, 213, 277, 226, 116, 214, 123,
        223, 59, 144, 309, 252, 239, 72, 71, 249, 154, 118, 216, 122, 158, 284, 321, 218, 304, 227, 327,
        219, 64, 176, 77, 294, 61, 278, 333, 244, 273, 337, 248, 314, 174, 181, 133, 106, 76, 162, 66,
        142, 94, 156, 202, 317, 224, 112, 159, 329, 346, 183, 339, 143, 221, 246, 282, 263, 338, 324, 149,
        177, 269, 331, 204, 318, 319, 349, 129, 98, 199, 169, 262, 101, 74, 297, 178, 197, 88, 212, 104,
        316, 111, 148, 267, 299, 89, 312, 279, 168, 207, 293, 73, 288, 83, 182, 347, 114, 152, 264, 303,
        233, 167, 236, 257, 57, 276, 194, 141, 287, 291, 126, 348, 258, 92, 96, 344, 137, 229, 203, 127,
        172, 53, 161, 307, 109, 332, 132, 79, 56, 301, 228, 308, 251, 222, 298, 313, 231, 164, 82, 289,
        259, 97, 63, 283, 81, 151, 87, 306, 163, 153, 253, 342, 208, 286, 296, 206, 334, 243, 191, 326,
        192, 341, 84, 131, 69, 343, 102, 322, 103, 166, 193, 328, 217, 117, 336, 292, 266, 311, 302, 323
    };


    public string targetFolderPath = "Assets/ClonedBusLevelSO"; // Đường dẫn đến folder đích

    [MenuItem("Tools/Batch Clone and Rename BusLevelSO")]
    public static void ShowWindow()
    {
        GetWindow<BatchCloneAndRenameBusLevelSO>("Batch Clone and Rename BusLevelSO");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Clone and Rename BusLevelSO Files", EditorStyles.boldLabel);

        // Hiển thị danh sách các BusLevelSO
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty listProperty = serializedObject.FindProperty("busLevelList");

        EditorGUILayout.PropertyField(listProperty, new GUIContent("BusLevelSO List"), true);
        serializedObject.ApplyModifiedProperties();

        // Nhập đường dẫn thư mục đích
        targetFolderPath = EditorGUILayout.TextField("Target Folder Path", targetFolderPath);

        // Nút thực hiện clone và rename
        if (GUILayout.Button("Clone and Rename BusLevelSO Files"))
        {
            CloneAndRenameFiles();
        }
    }

    private void CloneAndRenameFiles()
    {
        // Tạo thư mục đích nếu chưa tồn tại
        if (!AssetDatabase.IsValidFolder(targetFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "ClonedBusLevelSO");
        }

        // int index = 351;  // Bắt đầu từ 1

        // foreach (BusLevelSO busLevelSO in busLevelList)
        // {
        //     if (busLevelSO != null)
        //     {
        //         string sourcePath = AssetDatabase.GetAssetPath(busLevelSO);
        //         string newFileName = $"Level {index}.asset";
        //         string destinationPath = Path.Combine(targetFolderPath, newFileName);

        //         // Copy file và rename
        //         if (AssetDatabase.CopyAsset(sourcePath, destinationPath))
        //         {
        //             Debug.Log($"Cloned and renamed {busLevelSO.name} to {newFileName}");
        //         }
        //         else
        //         {
        //             Debug.LogError($"Failed to clone and rename {busLevelSO.name}");
        //         }

        //         index++;
        //     }
        // }

        int index5 = 355;
        for (int i = 0; i < numbersList_5.Count; i++)
        {
            string sourcePath = AssetDatabase.GetAssetPath(busLevelList[numbersList_5[i] - 51]);
            string newFileName = $"Level {index5} tron {numbersList_5[i]}.asset";
            string destinationPath = Path.Combine(targetFolderPath, newFileName);

            // Copy file và rename
            if (AssetDatabase.CopyAsset(sourcePath, destinationPath))
            {
                // Debug.Log($"Cloned and renamed {busLevelList[numbersList_5[i]].name} to {newFileName}");
            }
            else
            {
                // Debug.LogError($"Failed to clone and rename {busLevelList[numbersList_5[i]].name}");
            }

            index5 += 5;
        }

        int index_ko_5 = 351;
        for (int i = 0; i < numbersList_ko_5.Count; i++)
        {
            string sourcePath = AssetDatabase.GetAssetPath(busLevelList[numbersList_ko_5[i] - 51]);
            string newFileName = $"Level {index_ko_5} tron {numbersList_ko_5[i]}.asset";
            string destinationPath = Path.Combine(targetFolderPath, newFileName);

            // Copy file và rename
            if (AssetDatabase.CopyAsset(sourcePath, destinationPath))
            {
                // Debug.Log($"Cloned and renamed {busLevelList[numbersList_ko_5[i]].name} to {newFileName}");
            }
            else
            {
                // Debug.LogError($"Failed to clone and rename {busLevelList[numbersList_ko_5[i]].name}");
            }

            index_ko_5 += 1;
            if (index_ko_5 % 5 == 0) index_ko_5 += 1;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}