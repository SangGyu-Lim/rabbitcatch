using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum SoundType
{
    BGM,
    voice,
    effect,
    click,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Init();
    }
    void OnDestroy() { instance = null; }

    private AudioSource BGM;
    private AudioSource voice;
    private AudioSource effect;
    private AudioSource click;

    void Init()
    {
        BGM = gameObject.AddComponent<AudioSource>();
        BGM.playOnAwake = false;
        BGM.volume = 1.0f;
        BGM.loop = true;

        voice = gameObject.AddComponent<AudioSource>();
        voice.playOnAwake = false;
        voice.volume = 1.0f;
        voice.loop = false;

        effect = gameObject.AddComponent<AudioSource>();
        effect.playOnAwake = false;
        effect.volume = 1.0f;
        effect.loop = false;

        click = gameObject.AddComponent<AudioSource>();
        click.playOnAwake = false;
        click.volume = 1.0f;
        click.loop = false;
    }

    public void PlaySound(SoundType type, string fileName, float volume = 1.0f)
    {
        AudioClip tempSound = Resources.Load("Sound/" + fileName, typeof(AudioClip)) as AudioClip;
        if (tempSound == null)
            return;

        AudioSource audio = new AudioSource();
        switch(type)
        {
            case SoundType.BGM:
                {
                    audio = BGM;
                }
                break;
            case SoundType.voice:
                {
                    audio = voice;
                }
                break;
            case SoundType.effect:
                {
                    audio = effect;
                }
                break;
            case SoundType.click:
                {
                    audio = click;
                }
                break;
        }

        if (audio.isPlaying)
        {
            if (audio.clip == tempSound)
                audio.Stop();
        }

        audio.clip = tempSound;
        audio.Play();
        click.volume = volume;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
