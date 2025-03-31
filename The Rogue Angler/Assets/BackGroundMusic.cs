using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    [SerializeField] AudioClip musicTheme; // Música de fundo
    [SerializeField] AudioClip bossTheme; // Música de fundo
    [SerializeField] AudioClip victoryTheme; // Música de fundo

    [SerializeField] private FadeUI endScreen;
    public AudioSource audioSource;

    void Awake()
    {
        // // Garante que o objeto persista entre cenas
        // DontDestroyOnLoad(gameObject);

        // Inicializa o AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicTheme;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        PlayMusic();
    }

    public void Victory()
    {
        audioSource.Stop(); // Para a música atual antes de tocar a nova
        audioSource.PlayOneShot(victoryTheme);
        StartCoroutine(WaitEnd());
    }

    IEnumerator WaitEnd()
    {
        yield return new WaitForSeconds(victoryTheme.length); // Espera até a música de vitória terminar
        audioSource.clip = bossTheme;
        audioSource.Play();
        SetVolume(0.3f);
        
        endScreen.FadeUIIn(0.5f);
        Time.timeScale = 0;
        GameManager.Instance.gameIsPaused = true;
    }
    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            SetVolume(0.3f);
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }
}
