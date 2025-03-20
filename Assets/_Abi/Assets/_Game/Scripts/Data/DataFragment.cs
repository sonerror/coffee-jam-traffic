using UnityEngine;

public abstract class DataFragment : ScriptableObject
{
    [SerializeField] protected string key;
    public abstract void Load(); //contain reset func
    public abstract void Save();
    public abstract void ResetData();

    public virtual void Update() //remember to check null
    {
    }

    protected void SaveData<T>(T obj, string uniqueKey)
    {
        string data = JsonUtility.ToJson(obj);
        PlayerPrefs.SetString(uniqueKey, data);

        // Debug.Log(data);
    }

    protected bool LoadData<T>(ref T data, string uniqueKey) where T : class //return if load success // class is take as value type ??? -> req ref
    {
        data = null;
        data = JsonUtility.FromJson<T>(PlayerPrefs.GetString(uniqueKey));
        // Debug.Log("LOAD " + PlayerPrefs.GetString(uniqueKey));
        // Debug.Log("CHECK " + (data != null));
        return data != null;
    }

    protected T LoadDataTest<T>(T data, string uniqueKey) where T : class //return if load success
    {
        data = null;
        data = JsonUtility.FromJson<T>(PlayerPrefs.GetString(uniqueKey));
        // Debug.Log("LOAD " + PlayerPrefs.GetString(uniqueKey));
        // Debug.Log("CHECK " + (data != null));
        return data;
    }
}