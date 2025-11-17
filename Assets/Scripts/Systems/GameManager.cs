using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Handles game start and end states
 *
 * 
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private readonly string MainMenuSceneName = "MainMenu";
    
    [SerializeField] private bool startPlayerInactive;
    [SerializeField] private bool isMenu;
    
    [Space]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private CanvasGroup startScreen;
    [SerializeField] private CanvasGroup endScreen;
    [SerializeField] private CanvasGroup raceStartScreen;
    [SerializeField] private CanvasGroup raceResultsScreen;
    [SerializeField] private CanvasGroup lapCounterScreen;
    [SerializeField] private CanvasGroup racePlayerIndicator;
    [SerializeField] private TextMeshProUGUI raceCountdownText;
    [SerializeField] private TextMeshProUGUI raceResultsText;
    [SerializeField] private TextMeshProUGUI lapCounterText;
    [SerializeField] private LineDrawer playerLineDrawer;
    [SerializeField] private TextMeshProUGUI resultsText;
    
    [Space]
    [SerializeField] private Image[] duckCollectionImages;
    [SerializeField] private Sprite duckCollectedImage;
    
    [Space]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource waterDrainSound;
    [SerializeField] private AudioSource waterFillSound;
    [SerializeField] private AudioSource raceCountdownSound;
    [SerializeField] private AudioSource raceWinSound;
    [SerializeField] private AudioSource lapCompleteSound;
    private AudioMixer audioMixer;

    private LineFollower playerLineFollower;

    private LevelCompletionCollectible[] levelCollectibles;
    private int numCollectiblesCollected;
    
    private float fadeTime = 0.8f;

    public static string[] puzzleLevelNames = new string[]
    {
        "Easy1Final",
        "Easy2Final",
        "Easy3Final",
        "Easy4Final",
        "Easy5Final",
        "Med1Final",
        "Med2Final",
        "Med3Final",
        "Med4Final",
        "Med5Final",
        "Hard1Final",
        "Hard2Final",
        "Hard3Final",
        "Hard4Final",
        "Hard5Final",
        "16LevelFinal",
        "17LevelFinal",
        "18LevelFinal",
        "19LevelFinal",
        "20LevelFinal",
        "Bubble1Final",
        "Bubble2Final",
        "Bubble3Final",
        "Bubble4Final",
        "Bubble5Final"
    };

    public static string[] raceLevelNames = new string[]
    {
        "Race_1_Final"
    };
    
    public enum GameMode
    {
        Puzzle,
        Race
    }
    [Space]
    [SerializeField]
    public GameMode gameMode = GameMode.Puzzle;

    [Header("Race Mode Variables")]
    private int _totalNumRaceCheckpoints;

    [SerializeField] 
    private int numLaps = 3;

    private bool _raceOver;
    
    private RaceCheckpoint[] _raceCheckpoints;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        if (isMenu) return;
        
        playerLineFollower = playerLineDrawer.GetComponent<LineFollower>();
        
        // Turn player off if set to
        if (startPlayerInactive)
        {
            playerLineDrawer.SetLineDrawerActive(false);
        }
        
        // Find and subscribe to all collectible collected events
        levelCollectibles = FindObjectsByType<LevelCompletionCollectible>(FindObjectsSortMode.None);
        foreach (LevelCompletionCollectible collectible in levelCollectibles)
        {
            collectible.OnCollected += OnCollectibleCollected;
        }
        
        // Turn start screen canvas on and turn player hud off
        SetCanvasGroupActive(startScreen, true);
        SetCanvasGroupActive(playerHUD, false);
        SetCanvasGroupActive(endScreen, false);

        if (gameMode == GameMode.Puzzle)
        {
            // Subscribe to player end follow line state
            WaterDrain.Instance.OnWaterDrained += EndPuzzleGame;
        
            // Subscribe water drain sounds to water drain
            WaterDrain.Instance.OnWaterStartDraining += () => waterDrainSound.Play();
            WaterDrain.Instance.OnWaterDrained += () => waterDrainSound.DOFade(0, 0.5f);   
        }
        // race mode
        else
        {
            _raceCheckpoints = FindObjectsByType<RaceCheckpoint>(FindObjectsSortMode.None);
            _totalNumRaceCheckpoints = _raceCheckpoints.Length;
        }
    }
    // Transitions from start screen to gameplay
    public void StartGame()
    {
        // Fade out start screen canvas and fade in main screen
        FadeCanvasGroup(startScreen, false, () =>
        {
            if (gameMode == GameMode.Puzzle)
            {
                waterFillSound.Play();
                waterFillSound.DOFade(1, 0.5f);
                
                WaterDrain.Instance.FillUpBathtub(() =>
                {
                    FadeCanvasGroup(playerHUD, true, () =>
                        playerLineDrawer.SetLineDrawerActive(true));
                    // start music
                    music.Play();
                    waterFillSound.DOFade(0, 0.5f);
                });
            }
            else
            {
                UpdateLapCounterText();
                raceCountdownText.text = "";
                FadeCanvasGroup(raceStartScreen, true, () =>
                {
                    FadeCanvasGroup(racePlayerIndicator, true);
                    
                    StartCoroutine(StartRaceCountdown(() =>
                        {
                            FadeCanvasGroup(racePlayerIndicator, false);

                            FadeCanvasGroup(playerHUD, true, () =>
                            {
                                // enable player movement
                                playerLineDrawer.SetLineDrawerActive(true);

                                // start movement for enemy racer(s)
                                var opponentRacers =
                                    FindObjectsByType<RaceOpponentLineHandler>(FindObjectsSortMode.None);
                                foreach (var opponent in opponentRacers)
                                {
                                    opponent.StartMovement();
                                }

                                // start music
                                music.Play();
                            });

                            FadeCanvasGroup(lapCounterScreen, true);
                        }
                    ));
                });
            }
        });
    }

    // Fades a canvas group in or out
    private void FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn, Action onFadeComplete = null)
    {
        float endAlphaValue = fadeIn ? 1 : 0;
        
        canvasGroup.DOFade(endAlphaValue, fadeTime).onComplete += () =>
        {
            canvasGroup.interactable = fadeIn;
            canvasGroup.blocksRaycasts = fadeIn;
            onFadeComplete?.Invoke();
        };
    }
    
    // Sets a canvas group on or off immediately
    private void SetCanvasGroupActive(CanvasGroup canvasGroup, bool active)
    {
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
        canvasGroup.alpha = active? 1 : 0;
    }
    
    // activates a canvas group
    public void ActiveCanvasGroup(CanvasGroup canvasGroup)
    {
        SetCanvasGroupActive(canvasGroup, true);
    }
    
    // deactivates a canvas group
    public void DeactiveCanvasGroup(CanvasGroup canvasGroup)
    {
        SetCanvasGroupActive(canvasGroup, false);
    }
    
    // Increments collectibles collected and checks if the game is over
    private void OnCollectibleCollected()
    {
        numCollectiblesCollected++;
        if (numCollectiblesCollected >= levelCollectibles.Length)
        {
            Debug.Log("game done!");
        }
        UpdatePuzzleResultsScreen();
    }

    // Fades out game hud and fades in end screen
    public void EndPuzzleGame()
    {
        if (gameMode == GameMode.Puzzle)
        {
            UpdatePuzzleResultsScreen();
            playerLineDrawer.SetLineDrawerActive(false);
            FadeCanvasGroup(playerHUD, false, () =>
                FadeCanvasGroup(endScreen, true));
        }
    }
    
    // Updates the results text with player's score
    private void UpdatePuzzleResultsScreen()
    {
        for (int i = 0; i < numCollectiblesCollected; i++)
        {
            duckCollectionImages[i].sprite = duckCollectedImage;
        }
        
        //string results = "Collected " + numCollectiblesCollected + " out of " + levelCollectibles.Length + " collectibles!";
        //resultsText.text = results;
    }
    
    // Reloads the scene for game restart
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Loads a specified puzzle based on index - only used for early builds without menu's/level transitions
    public void LoadPuzzleNumber(int puzzleNumber)
    {
        SceneManager.LoadScene(puzzleLevelNames[puzzleNumber - 1]);
    }
    
    // Loads a specified racecourse scene
    public void LoadRaceCourseNumber(int courseNumber)
    {
        SceneManager.LoadScene(raceLevelNames[courseNumber - 1]);
    }
    
    // loads title screen
    public void QuitToTitle()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
    
    // quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // adjusts the audio mixer master slider
    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", volume);    
    }
    
    // adjusts the audio mixer master slider
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);    
    }
    
    // adjusts the audio mixer master slider
    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("soundVolume", volume);    
    }
    
    // saves level data to save data
    public void SaveLevelToSaveData()
    {
        // find index of level in save data - uses order of puzzle names as order
        int levelSaveIndex = 0;
        for (int i = 0; i < puzzleLevelNames.Length; i++)
        {
            if (puzzleLevelNames[i].Equals(SceneManager.GetActiveScene().name))
            {
                levelSaveIndex = i;
                break;
            }
        }
        
        // save to save data if puzzle level
        if (gameMode == GameMode.Puzzle)
        {
            LevelSaveData levelSaveData = new LevelSaveData(true, numCollectiblesCollected);
            SaveLoadManager.Instance.SaveData.SaveDataForLevel(levelSaveIndex, levelSaveData);
            SaveLoadManager.Instance.SaveGame();
        }

    }
    
    // checks if lap/race was completed by player when hitting finish line
    public void OnLapCompleted(LineFollower racer)
    {
        // check if lap was completed (all checkpoints hit)
        if (racer.GetNumberOfHitCheckpoints() >= _totalNumRaceCheckpoints)
        {
            racer.CompleteLap();
            if (racer.gameObject.CompareTag("Player"))
            {
                lapCompleteSound.Play();

                if (racer.GetNumberOfCompletedLaps() < numLaps)
                {
                    UpdateLapCounterText(racer);
                }
            }
        }
        
        // check if race over
        if (racer.GetNumberOfCompletedLaps() >= numLaps && !_raceOver)
        {
            bool didPlayerWinRace = racer.gameObject.CompareTag("Player");
            
            EndRaceGame(didPlayerWinRace);
        }
    }
    
    // ends race and fades in results ui
    private void EndRaceGame(bool didPlayerWinRace)
    {
        _raceOver = true;
        
        music.DOFade(0, 0.5f);
        
        UpdateRaceResultsScreen(didPlayerWinRace);
        playerLineDrawer.SetLineDrawerActive(false);
        FadeCanvasGroup(playerHUD, false, () =>
            FadeCanvasGroup(raceResultsScreen, true, () =>
                {
                    if (didPlayerWinRace) raceWinSound.Play();
                }));
    }
    
    // race start UI coroutine
    private IEnumerator StartRaceCountdown(Action onComplete)
    {
        float timeBetweenClicks = 0.56f;
        
        raceCountdownSound.Play();

        float initialSoundBufferTime = 0.3f;
        yield return new WaitForSeconds(initialSoundBufferTime);
        
        raceCountdownText.text = "3";
        
        yield return new WaitForSeconds(timeBetweenClicks);
        
        raceCountdownText.text = "2";
        
        yield return new WaitForSeconds(timeBetweenClicks);
        
        raceCountdownText.text = "1";
        
        yield return new WaitForSeconds(timeBetweenClicks);
        
        raceCountdownText.text = "<color=\"green\">GO!!!";

        float timeBeforeStart = 0.5f;
        yield return new WaitForSeconds(timeBeforeStart);
        
        FadeCanvasGroup(raceStartScreen, false, onComplete);
    }
    
    // Updates race results screen
    private void UpdateRaceResultsScreen(bool didPlayerWinRace)
    {
        if (didPlayerWinRace)
        {
            raceResultsText.text = "<color=\"green\">You won!";
        }
        else
        {
            raceResultsText.text = "<color=\"red\">You lost...";
        }
    }
    
    // update lap count text
    private void UpdateLapCounterText(LineFollower racer)
    {
        lapCounterText.text = "Lap " + (racer.GetNumberOfCompletedLaps() + 1) + " of " + numLaps;
    }
    
    private void UpdateLapCounterText()
    {
        lapCounterText.text = "Lap 1 of " + numLaps;
    }
}
