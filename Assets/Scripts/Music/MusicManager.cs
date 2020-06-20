using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource specialEffectAudioSource;

    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private AudioClip lostMusic;
    [SerializeField] private AudioClip menuMusic;

    [SerializeField] private AudioClip launchGame;
    
    private GameManager.GameState previousState;
    
    void Start()
    {
        musicAudioSource.clip = menuMusic;
        musicAudioSource.Play();

        previousState = GameManager.Instance.CurrentState;
    }
    
    void Update()
    {
        var newState = GameManager.Instance.CurrentState;

        if (newState != previousState)
        {
            if (newState == GameManager.GameState.GAME)
            {
                musicAudioSource.clip = clips[Random.Range(0, clips.Count)];
                musicAudioSource.Play();

                specialEffectAudioSource.clip = launchGame;
                specialEffectAudioSource.Play();
            }
            
            if (newState == GameManager.GameState.END)
            {
                musicAudioSource.clip = lostMusic;
                musicAudioSource.Play();
            }

            previousState = newState;
        }
    }
}
