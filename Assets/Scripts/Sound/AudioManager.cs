using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public float sliderValue;
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("MovedToNotDestroy");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("Music");
    }
    
    public float PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
            return 0f;
        }
        else
        {
            musicSource.clip = s.clip; 
            musicSource.Play();
            return s.clip.length;
        }
    }

    public float PlaySFX(string name, float pitch)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
            return 0f;
        }
        else
        {
            //UnityEngine.Random.Range(0.7f, 1.3f);
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(s.clip);
            return s.clip.length;
        }
    }
}
