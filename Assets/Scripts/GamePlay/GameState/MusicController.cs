using System.Collections;
using System.Collections.Generic;
using Config;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField]
    private AudioClip battleMusic;

    [SerializeField]
    private AudioClip waitingMusic;

    [SerializeField]
    private AudioSource audioSource;

    private AudioClip nextClip;

    private AudioState audioState;

    private float volumeDelta;

	private float fullVolume;
    
    public enum AudioState
    {
        STARTING,
        PLAYING,
        PAUSING,
        PAUSED
    }
    
    // Start is called before the first frame update
    public void Awake()
    {
        GameController.onGameStateChanged.AddListener(OnGameStateChange);
        audioState = AudioState.PAUSED;
        audioSource.volume = 0f;
		fullVolume = 0f;
        nextClip = null;
        volumeDelta = 0.003f;
    }

	public void SetVolume(float volume) {
		fullVolume = volume;
	}

    public void Update()
    {
        switch (audioState)
        {
            case AudioState.PAUSED:
                Paused();
                break;
            case AudioState.PLAYING:
                Playing();
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
        if (audioSource.volume < fullVolume)
        {
            audioSource.volume += volumeDelta;
        }
        else
        {
            audioSource.volume = fullVolume;
            audioState = AudioState.PLAYING;
        }
    }

    private void Playing()
    {
        if (nextClip != null && nextClip != audioSource.clip)
        {
            audioState = AudioState.PAUSING; 
        }
    }

    private void Paused()
    {
        if (nextClip != null)
        {
            audioSource.clip = nextClip;
            audioSource.Play();
            nextClip = null;
            audioState = AudioState.STARTING;
        }
    }
    
    private void Pausing()
    {
        if (audioSource.volume > 0.0f)
        {
            audioSource.volume -= volumeDelta;
        }
        else
        {
            audioSource.volume = 0.0f;
            if (nextClip != null)
            {
                audioSource.clip = nextClip;
                nextClip = null;
                audioSource.Play();
                audioState = AudioState.STARTING;
                return;
            }
            audioSource.Pause();
            audioState = AudioState.PAUSED;
        }
    }
    
    public void OnGameStateChange(EGameState eGameState)
    {
        switch (eGameState)
        {
            case EGameState.FightingWave:
                nextClip = battleMusic;
                break;
            case EGameState.WaitingForNextWave:
                nextClip = waitingMusic;
                break;
            case EGameState.GameOver:
                nextClip = null;
                break;
            case EGameState.GameWon:
                nextClip = null;
                break;
        }        
    }

}