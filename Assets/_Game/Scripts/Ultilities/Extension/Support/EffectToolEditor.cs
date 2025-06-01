#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EffectToolEditor : EditorWindow
{
    [MenuItem("MyTool/Effect Tool Window %q")]
    static void ShowWindow()
    {
        var editor = GetWindow(typeof(EffectToolEditor));
        editor.Show();
    }

    public int CurrentIndexLevel;

    void OnGUI()
    {
        GUILayout.Label("TimeScale");
        Time.timeScale = EditorGUILayout.Slider("Time Scale", Time.timeScale, 0, 10);
    }
}
#endif