using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;

public static class GameHelper
{

    /*
    * 1. Chuyển từ tọa độ màn hình (transform) sang tọa độ trên canvas (rect transform)
    */
    #region Canvas & Angle

    //Chuyển từ tọa độ màn hình (transform) sang tọa độ trên canvas (rect transform)
    public static Vector2 WorldToCanvas(this Canvas canvas, Vector3 world_position, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewport_position = camera.WorldToViewportPoint(world_position);
        var canvas_rect = canvas.GetComponent<RectTransform>();

        return new Vector2((viewport_position.x * canvas_rect.sizeDelta.x) - (canvas_rect.sizeDelta.x * 0.5f),
            (viewport_position.y * canvas_rect.sizeDelta.y) - (canvas_rect.sizeDelta.y * 0.5f));
    }

    //Lấy góc giữa vật thể và 1 vector
    public static float GetAngle(this Transform t, Vector3 target)
    {
        return (float)Mathf.Atan2(target.y - t.position.y, target.x - t.position.x) * Mathf.Rad2Deg - 90;
    }

    //Lấy góc giữa 2 vector
    public static float GetAngle(this Vector3 t, Vector3 target)
    {
        return (float)Mathf.Atan2(target.y - t.y, target.x - t.x) * Mathf.Rad2Deg - 90;
    }

    //Góc của 1 vector với vector X
    public static float GetAngle(this Vector3 t)
    {
        return (float)Mathf.Atan2(t.y, t.x) * Mathf.Rad2Deg;
    }

    //Góc của 1 vector với vector X
    public static float GetAngle(this Vector2 t)
    {
        return (float)Mathf.Atan2(t.y, t.x) * Mathf.Rad2Deg;
    }

    //Góc của 1 vector với trục hoành
    public static float GetAngleZero(this Vector3 t, Vector3 target)
    {
        return (float)Mathf.Atan2(target.y - t.y, target.x - t.x) * Mathf.Rad2Deg + 90;
    }

    public static float Lerp(this float t, float b, float s)
    {
        if(t < b)
        {
            return t + s < b ? t + s : b;
        }
        else
        {
            return t - s > b ? t - s : b;
        }
    }

    #endregion

    /*
    * 1. String to enum of Enumtype
    * 2. enum of EnumType to value (int, float ...)
    */
    #region Enum
    /// <summary>
    /// Extension method to return an enum value of type T for the given string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this string value)
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Extension method to return an enum value of type T for the given int.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this int value)
    {
        var name = System.Enum.GetName(typeof(T), value);
        return name.ToEnum<T>();
    }
    #endregion

    /*
    * Get & Set PlayerPrefs, create if don't have
    */
    #region PlayerPref
    public static float PlayerPref_float(string key, float v)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, v);
        }
        return PlayerPrefs.GetFloat(key);
    }

    public static float PlayerPref_float(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, 0);
        }
        return PlayerPrefs.GetFloat(key);
    }

    public static int PlayerPref_int(string key, int v)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, v);
        }
        return PlayerPrefs.GetInt(key);
    }

    public static int PlayerPref_int(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 0);
        }
        return PlayerPrefs.GetInt(key);
    }

    public static string PlayerPref_string(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetString(key, "");
        }
        return PlayerPrefs.GetString(key);
    }
    #endregion

    /*
    * 1. Get All Child Of Transform
    * 2. Remove All Child Of Transform
    * 3. Remove All Child Of Transform In Editor
    */
    #region Transform Children
    public static List<GameObject> GetAllChilds(this Transform t)
    {
        List<GameObject> childs = new List<GameObject>();
        int c = t.childCount;
        for (int i = 0; i < c; i++)
            childs.Add(t.GetChild(i).gameObject);
        return childs;
    }

    public static void RemoveAllChild(this Transform t)
    {
        int c = t.childCount;
        for (int i = 0; i < c; i++) 
            Object.DestroyImmediate(t.GetChild(0).gameObject);                 
    }

    public static void RemoveChildren(Transform t)
    {
        foreach(Transform child in t) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static void DisableAllChild(this Transform t)
    {
        int c = t.childCount;
        for (int i = 0; i < c; i++)
            t.GetChild(i).gameObject.SetActive(false);
    }

    public static void RemoveAllChildOnEditor(this Transform t)
    {
        int c = t.childCount;
        for (int i = 0; i < c; i++)
            Object.DestroyImmediate(t.GetChild(0).gameObject);
    }
    #endregion

    /*
    * 1. Random Element in Array
    * 2. Random Element in List
    * 3. Flip List
    * 4. Clone to new list <T>
    */
    #region Array
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this T[] array)
    {
        int i = array.Length;
        while (i > 1)
        {
            int num = rng.Next(i--);
            T t = array[i];
            array[i] = array[num];
            array[num] = t;
        }
    }
    public static void Shuffle<T>(this List<T> array)
    {
        int i = array.Count;
        while (i > 1)
        {
            int num = rng.Next(i--);
            T t = array[i];
            array[i] = array[num];
            array[num] = t;
        }
    }

    public static List<Vector3> FlipList(this List<Vector3> l)
    {
        return l.Select(s => new Vector3(-s.x, s.y)).ToList();
    }

    public static T DeepClone<T>(T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }

    #endregion

    /*
     * 1. Scale image for X size
     * 2. Scale image for X size3
     * 3. Get color with alpha
     * 4. Flip X (-1 <-> 1)
     * 5. Set Flip X (-1 / 1)
     */
    #region Spire + Image
    public static void SetSizeX(this SpriteRenderer render, int size)
    {
        float scale = size / render.sprite.rect.width;
        render.transform.localScale = new Vector3(scale, scale);
    }

    public static void SetSizeY(this SpriteRenderer render, int size)
    {
        float scale = size / render.sprite.rect.height;
        render.transform.localScale = new Vector3(scale, scale);
    }

    public static Color SetColorAlpha(Color c, float a)
    {
        c.a = a;
        return c;
    }

    public static void FlipX(this Transform t)
    {
        t.localScale = new Vector3(t.localScale.x * -1, t.localScale.y, t.localScale.z);
    }

    public static void FlipX(this Transform t, int dir)
    {
        if (dir < 0) t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
        else t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
    }
    #endregion

    /*
     * 1. Get current time (second)
     * 2. format time (HH:MM)
     * 3. format time (HH:MM:SS)
     */
    #region Time
    public static int CurrentTimeInSecond
    {
        get
        {
            return (int)(System.DateTime.Now - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    public static string FormatTimeMMSS(int second)
    {
        string s = "";
        var min = second / 60;
        s += min < 10 ? "0" + min : "" + min;
        s += ":";
        var sec = second % 60;
        s += sec < 10 ? "0" + sec : "" + sec;
        return s;
    }

    public static string FormatTimeHHMMSS(int second)
    {
        string s = "";
        var hour = second / 3600;
        s += hour < 10 ? "0" + hour : "" + hour;
        s += ":";

        var min = (second - hour * 3600) / 60;
        s += min < 10 ? "0" + min : "" + min;
        s += ":";

        var sec = second % 60;
        s += sec < 10 ? "0" + sec : "" + sec;
        return s;
    }

    public static int GetDayNow
    {
        get
        {
            return CurrentTimeInSecond / 86400;
        }
    }
    #endregion

#if UNITY_EDITOR

    public static List<Object> GetAllAssets(string path)
    {
        string[] paths = { path };
        var assets = UnityEditor.AssetDatabase.FindAssets(null, paths);
        var assetsObj = assets.Select(s => UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(UnityEditor.AssetDatabase.GUIDToAssetPath(s))).ToList();
        return assetsObj;
    }

#endif
}