using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Data/Level", order = 2)]
public class Level : ScriptableObject
{
    public List<BoxData> boxData = new List<BoxData>();
}
[System.Serializable]
public class BoxData
{
    public PoolType boxType;
    public Vector3 tr;
    public Vector3 rotate;
    public BoxData(PoolType poolType,Vector3 tr, Vector3 rotate)
    {
        this.boxType = poolType;
        this.tr = tr;
        this.rotate = rotate;
    }
}

