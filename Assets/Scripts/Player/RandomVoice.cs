using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomVoice : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> voices;
    [SerializeField] private float minTime = 20;
    [SerializeField] private float maxTime = 40;

    private float timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            audioSource.clip = voices[Random.Range(0, voices.Count)];
            audioSource.Play();
            
            timer = Random.Range(minTime, maxTime);
        }
    }
}
