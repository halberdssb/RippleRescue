using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector director;
    [SerializeField]
    private CanvasGroup splashScreenCanvas;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveLoadManager.Instance.playIntroCutscene)
        {
            splashScreenCanvas.gameObject.SetActive(true);
            SaveLoadManager.Instance.playIntroCutscene = false;
        }
        else
        {
            splashScreenCanvas.gameObject.SetActive(false);
            director.Play();
            director.time = director.duration;
        }
    }
}
