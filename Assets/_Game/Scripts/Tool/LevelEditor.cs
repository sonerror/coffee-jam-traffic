using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    private GameObject parentObject;
    private string levelName = "NewLevel";
    private string savePath = "Assets/_Game/OS/Levels"; // ???ng d?n l?u Level

    [MenuItem("Tools/Level Creator")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Creator", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object:", parentObject, typeof(GameObject), true);
        levelName = EditorGUILayout.TextField("Level Name:", levelName);

        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel();
        }
    }

    private void GenerateLevel()
    {
        if (parentObject == null)
        {
            Debug.LogWarning("Vui l�ng ch?n m?t GameObject cha ch?a c�c Box!");
            return;
        }

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            AssetDatabase.Refresh();
        }

        Box[] boxes = parentObject.GetComponentsInChildren<Box>(); // T�m t?t c? Box trong GameObject cha
        if (boxes.Length == 0)
        {
            Debug.LogWarning("Kh�ng t�m th?y h?p n�o trong GameObject cha!");
            return;
        }

        Level level = ScriptableObject.CreateInstance<Level>();
        level.boxData = new List<BoxData>();

        foreach (Box box in boxes)
        {
            Vector3 position = box.transform.localPosition; // L?u v? tr� t??ng ??i
            Vector3 rotation = box.transform.localRotation.eulerAngles; // L?u g�c quay t??ng ??i
            level.boxData.Add(new BoxData(box.poolType, position, rotation));
        }

        string fullPath = $"{savePath}/{levelName}.asset";
        AssetDatabase.CreateAsset(level, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = level;

        Debug.Log($"Level {levelName} ?� ???c t?o th�nh c�ng t?i {fullPath}!");
    }
}
