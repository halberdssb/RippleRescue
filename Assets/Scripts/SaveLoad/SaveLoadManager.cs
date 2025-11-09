using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    
    private readonly string SaveFileName = "SaveData.txt";
    
    public SaveData SaveData;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        
        if (File.Exists(GetSaveFilePath()))
        {
            SaveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(GetSaveFilePath()));
            Debug.Log("Game loaded");
        }
        else
        {
            SaveData = new SaveData();
        }
    }

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private string GetSaveFilePath()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, SaveFileName);
    }
    
    public void SaveGame()
    {
        File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(SaveData));
        Debug.Log(GetSaveFilePath());
    }
}
