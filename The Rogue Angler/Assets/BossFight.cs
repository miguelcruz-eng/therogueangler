using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    [SerializeField] AudioClip backgroundMusic; // Música de fundo
    private AudioSource audioSource;

    void Awake()
    {
        // // Garante que o objeto persista entre cenas
        // DontDestroyOnLoad(gameObject);

        // Inicializa o AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // public void SetVolume(float volume)
    // {
    //     audioSource.volume = Mathf.Clamp01(volume);
    // }
}
