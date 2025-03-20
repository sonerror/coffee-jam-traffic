#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;


public class ChangeScene : Editor
{
    [MenuItem("Open Scene/Loading #1")]
    public static void OpenLoading()
    {
        OpenScene("Loading");
    }

    [MenuItem("Open Scene/Game #2")]
    public static void OpenGame()
    {
        OpenScene("Game");
    }

    [MenuItem("Open Scene/Hex #3")]
    public static void OpenRoad()
    {
        OpenScene("Hex");
    }

    [MenuItem("Open Scene/Tutorial #4")]
    public static void OpenLevelEdit()
    {
        OpenScene("Tutorial");
    }

    [MenuItem("Open Scene/Builder #5")]
    public static void OpenStackRider()
    {
        OpenScene("Builder");
    }

    [MenuItem("Open Scene/Asset #6")]
    public static void OpenAsset()
    {
        OpenScene("Asset");
    }

    [MenuItem("Open Scene/Move Stop Move #q")]
    public static void OpenMoveStopMove()
    {
        OpenScene("MoveStopMove");
    }

    private static void OpenScene(string sceneName)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/_Game/Scenes/" + sceneName + ".unity");
        }
    }
}
#endif