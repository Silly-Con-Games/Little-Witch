using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField]
    private AudioClip battleMusic;
    
    [SerializeField]
    private AudioSource audioSource;

    private AudioClip nextClip;

    private AudioState audioState;
    
    public enum AudioState
    {
        STARTING,
        PLAYING,
        PAUSING,
        PAUSED
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GameController.onGameStateChanged.AddListener(OnGameStateChange);
        audioState = AudioState.PAUSED;
    }

    void Update()
    {
        switch (audioState)
        {
            case AudioState.PAUSED:
                break;
            case AudioState.PLAYING:
                break;
            case AudioState.PAUSING:
                Pausing();
                break;
            case AudioState.STARTING:
                Starting();
                break;
        }   
    }

    private void Starting()
    {
        if (audioSource.volume < 1.0f)
        {
            audioSource.volume += 0.01f;
        }
        else
        {
            audioState = AudioState.PLAYING;
            audioSource.Play();
        }
    }
    
    private void Pausing()
    {
        if (audioSource.volume > 0.0f)
        {
            audioSource.volume -= 0.01f;
        }
        else
        {
            audioSource.Pause();
            audioState = AudioState.PAUSED;
        }
    }
    
    public void OnGameStateChange(EGameState eGameState)
    {
        switch (eGameState)
        {
            case EGameState.FightingWave:
                if (audioState == AudioState.PLAYING)
                {
                    nextClip = battleMusic;
                    audioState = AudioState.PAUSING;
                }
                else if (audioState == AudioState.PAUSED)
                {
                    audioSource.clip = battleMusic;
                    audioState = AudioState.STARTING;
                }
                break;
            case EGameState.WaitingForNextWave:
                break;
            case EGameState.GameOver:
                break;
            case EGameState.GameWon:
                break;
        }        
    }

}
