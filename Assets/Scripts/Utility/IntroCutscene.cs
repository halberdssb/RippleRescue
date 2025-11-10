using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector director;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SaveLoadManager.Instance.playIntroCutscene)
        {
            director.Play();
            SaveLoadManager.Instance.playIntroCutscene = false;
        }
        else
        {
            director.Play();
            director.time = director.duration;
        }
    }
}
