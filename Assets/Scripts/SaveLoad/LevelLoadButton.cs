using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoadButton : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    
    [SerializeField] private Image[] collectedDuckImages;
    
    [Space]
    [SerializeField] private Sprite collectedDuckImage;
    [SerializeField] private Sprite uncollectedDuckImage;
    
    [Space, SerializeField]
    private Button button;
    

    // loads level in index
    public void LoadLevel()
    {
        GameManager.Instance.LoadPuzzleNumber(levelIndex);
    }

    public void LoadLevelData()
    {
        // reset duck icons
        for (int i = 0; i < collectedDuckImages.Length; i++)
        {
            SetDuckImage(i, false);
        }
        
        // update collected icons based on save data
        LevelSaveData thisLevelsSaveData = SaveLoadManager.Instance.SaveData.GetDataForLevel(levelIndex);
        Debug.Log("leveldata loaded", this);
        for (int i = 0; i < thisLevelsSaveData.numDucksCollected; i++)
        {
            SetDuckImage(i, true);
        }
        
        // check if previous level is beaten - level not available if not
        if (levelIndex >= 0)
        {
            LevelSaveData previousLevelData = SaveLoadManager.Instance.SaveData.GetDataForLevel(levelIndex - 1);
            if (!previousLevelData.levelCompleted)
            {
                button.interactable = false;
            }
        }
    }

    private void SetDuckImage(int index, bool collected)
    {
        collectedDuckImages[index].sprite = collected ? collectedDuckImage : uncollectedDuckImage;
    }
}
