using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Handles game start and end states
 *
 * 
 */

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool startPlayerInactive;
    [SerializeField] private bool isMenu;
    
    [Space]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private CanvasGroup startScreen;
    [SerializeField] private CanvasGroup endScreen;
    [SerializeField] private LineDrawer playerLineDrawer;
    [SerializeField] private TextMeshProUGUI resultsText;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource waterDrainSound;

    private LineFollower playerLineFollower;

    private LevelCompletionCollectible[] levelCollectibles;
    private int numCollectiblesCollected;
    
    private float fadeTime = 0.8f;

    private string[] puzzleLevelNames = new string[]
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
        "20LevelFinal"
    };

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
        
        // Subscribe to player end follow line state
        WaterDrain.Instance.OnWaterDrained += EndGame;
        
        // Subscribe water drain sounds to water drain
        WaterDrain.Instance.OnWaterStartDraining += () => waterDrainSound.Play();
        WaterDrain.Instance.OnWaterDrained += () => waterDrainSound.DOFade(0, 0.5f);
    }
    // Transitions from start screen to gameplay
    public void StartGame()
    {
        // Fade out start screen canvas and fade in main screen
        FadeCanvasGroup(startScreen, false, () => 
            FadeCanvasGroup(playerHUD, true, () => 
                playerLineDrawer.SetLineDrawerActive(true)));
        
        // start music
        music.Play();
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
        UpdateResultsText();
    }

    // Fades out game hud and fades in end screen
    public void EndGame()
    {
        UpdateResultsText();
        playerLineDrawer.SetLineDrawerActive(false);
        FadeCanvasGroup(playerHUD, false, () =>
            FadeCanvasGroup(endScreen, true));
    }
    
    // Updates the results text with player's score
    private void UpdateResultsText()
    {
        string results = "Collected " + numCollectiblesCollected + " out of " + levelCollectibles.Length + " collectibles!";
        resultsText.text = results;
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
}
