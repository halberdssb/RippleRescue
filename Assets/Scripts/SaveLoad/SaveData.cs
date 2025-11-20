using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]
public class SaveData
{
    [SerializeField]
    private LevelSaveData[] _levelData;

    public SaveData()
    {
        _levelData = new LevelSaveData[GameManager.puzzleLevelNames.Length];
    }

    public void SaveDataForLevel(int levelIndex, LevelSaveData inLevelSaveData)
    {
        LevelSaveData currentlySavedLevelData = _levelData[levelIndex];

        // prevent fewer number of ducks to be saved than highest number of ducks saved for level
        if (inLevelSaveData.numDucksCollected < currentlySavedLevelData.numDucksCollected)
        {
            inLevelSaveData.numDucksCollected = currentlySavedLevelData.numDucksCollected;
        }
        
        _levelData[levelIndex] = inLevelSaveData;
    }

    public LevelSaveData GetDataForLevel(int levelIndex)
    {
        return _levelData[levelIndex];
    }

    public void UnlockAllLevels()
    {
        for (int i = 0; i < _levelData.Length; i++)
        {
            _levelData[i].levelCompleted = true;
        }
    }
}

[System.Serializable]
public struct LevelSaveData
{
    public bool levelCompleted;
    public int numDucksCollected;

    public LevelSaveData(bool levelCompleted, int numDucksCollected)
    {
        this.levelCompleted = levelCompleted;
        this.numDucksCollected = numDucksCollected;
    }
}
