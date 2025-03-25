using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelOS", menuName = "ScriptableObjects/Data/LevelOS", order = 1)]
public class LevelOS : ScriptableObject
{
    public List<LevelData> levelOS;
    public LevelData GetLevelByID(int _id)
    {
        return levelOS.Find(id => id.levelID == _id);
    }
  
}
[System.Serializable]
public class LevelData
{
    public int levelID;
    public Level level;
}
