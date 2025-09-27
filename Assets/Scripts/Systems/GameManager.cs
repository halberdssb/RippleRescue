using System;
using DG.Tweening;
using UnityEngine;

/*
 * Handles game start and end states
 *
 * 
 */

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool startPlayerInactive;
    
    [Space]
    [SerializeField] private CanvasGroup playerHUD;
    [SerializeField] private CanvasGroup startScreen;
    [SerializeField] private LineDrawer playerLineDrawer;

    private Collectible[] levelCollectibles;
    private int numCollectiblesCollected;
    
    private float fadeTime = 0.8f;

    private void Start()
    {
        // Turn player off if set to
        if (startPlayerInactive)
        {
        playerLineDrawer.SetLineDrawerActive(false);
        }
        
        // Find and subscribe to all collectible collected events
        levelCollectibles = FindObjectsOfType<Collectible>();
        foreach (Collectible collectible in levelCollectibles)
        {
            collectible.OnCollected += OnCollectibleCollected;
        }
        
        // Turn start screen canvas on and turn player hud off
        SetCanvasGroupActive(startScreen, true);
        SetCanvasGroupActive(playerHUD, false);
    }
    // Transitions from start screen to gameplay
    public void StartGame()
    {
        // Fade out start screen canvas and fade in main screen
        FadeCanvasGroup(startScreen, false, () => 
            FadeCanvasGroup(playerHUD, true, () => 
                playerLineDrawer.SetLineDrawerActive(true)));
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
    
    // Increments collectibles collected and checks if the game is over
    private void OnCollectibleCollected()
    {
        numCollectiblesCollected++;
        if (numCollectiblesCollected >= levelCollectibles.Length)
        {
            Debug.Log("game done!");
        }
    }
}
