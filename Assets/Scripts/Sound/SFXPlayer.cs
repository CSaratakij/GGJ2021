using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField]
    int totalSources = 10;

    [SerializeField]
    ButtonAudioInfo[] infos;

    AudioSource[] audioSources;

    [System.Serializable]
    public struct ButtonAudioInfo
    {
        public AudioClip clip;
        public Button[] buttons;
    }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        audioSources = new AudioSource[totalSources];

        for (int i = 0; i < audioSources.Length; ++i)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
        }

        foreach (var item in infos)
        {
            foreach (var button in item.buttons)
            {
                button?.onClick.AddListener(() => {
                    Play(item.clip);
                });
            }
        }
    }

    public void Play(AudioClip clip)
    {
        StartCoroutine(PlayCallback(clip));
    }

    IEnumerator PlayCallback(AudioClip clip)
    {
        bool canPlay = false;

        foreach (var source in audioSources)
        {
            if (source.isPlaying)
                continue;

            canPlay = true;
            source.PlayOneShot(clip);
            yield break;
        }

        if (!canPlay) {
            audioSources[0].Stop();
            audioSources[0].PlayOneShot(clip);
        }
    }
}

