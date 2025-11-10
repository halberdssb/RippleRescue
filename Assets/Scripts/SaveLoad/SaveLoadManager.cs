using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    public bool playIntroCutscene = true;
    
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
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LevelLoadButton[] levelLoadButtons = FindObjectsByType<LevelLoadButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var levelLoadButton in levelLoadButtons)
        {
            levelLoadButton.LoadLevelData();
        }
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
