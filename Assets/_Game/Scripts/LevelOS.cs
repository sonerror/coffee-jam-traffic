using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Data/Level", order = 1)]
public class LevelOS : ScriptableObject
{
    public int idLevel;
    public List<LevelData> levelDatas = new List<LevelData>();
}
[System.Serializable]
public class LevelData
{
    public int id;
    public BoxType boxType;
    public Vector3 tr;
    public Vector3 rotate;
    public LevelData(int id, BoxType boxType, Vector3 tr, Vector3 rotate)
    {
        this.id = id;
        this.boxType = boxType;
        this.tr = tr;
        this.rotate = rotate;
    }
}

[System.Serializable]
public enum BoxType
{
    box4,
    box6,
    box10
}
