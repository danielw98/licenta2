using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudio;
    public AudioSource ambientAudio;
    public AudioSource effectsAudio;
    public AudioSource secondaryEffectsAudio;
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public static AudioManager Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicAudio.clip = clip;
        musicAudio.Play();
    }
    public void PlayAmbient(AudioClip clip)
    {
        ambientAudio.clip = clip;
        ambientAudio.Play();
    }
    public void PlayEffect(AudioClip clip)
    {
        effectsAudio.clip = clip;
        effectsAudio.Play();
    }

    public void PlaySecondaryEffect(AudioClip clip)
    {
        secondaryEffectsAudio.clip = clip;
        secondaryEffectsAudio.Play();
    }

}
