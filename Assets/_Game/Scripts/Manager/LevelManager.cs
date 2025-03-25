using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public LevelOS leveldata;
    public int levelCurrent = 0;
    public Level currentLevel;

    public void OnLoadLevel(int levelID)
    {
        currentLevel = leveldata.GetLevelByID(levelID).level;
        for (int i = 0; i < currentLevel.boxData.Count; i++)
        {
            Box newBox = SimplePool.Spawn<Box>((PoolType)currentLevel.boxData[i].boxType, currentLevel.boxData[i].tr, Quaternion.identity);
           // newBox.transform.parent = null;
            newBox.transform.rotation = Quaternion.Euler(currentLevel.boxData[i].rotate);
        }
    }
}
