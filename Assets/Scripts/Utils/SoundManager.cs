using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField]
    float playTrackDelay = 8.0f;

    [SerializeField]
    AudioClip[] clips;

    int currentClipIndex = -1;
    AudioSource source;

    WaitForSeconds waitForEachTrack;
    Coroutine playSongCoroutine;

    void Awake()
    {
        waitForEachTrack = new WaitForSeconds(playTrackDelay);
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameController.Instance.OnGameStateChange += OnGameStateChange;
    }

    void OnDestroy()
    {
        GameController.Instance.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameState state)
    {
        if (!source.isPlaying) {
            return;
        }

        if (GameState.Pause == state) {
            source.Pause();
        }
        else {
            source.UnPause();
        }
    }

    public void Play()
    {
        if (playSongCoroutine != null) {
            StopCoroutine(playSongCoroutine);
        }

        playSongCoroutine = StartCoroutine(PlaySongCallback());
    }

    IEnumerator PlaySongCallback()
    {
        while (true)
        {
            if (source.isPlaying) {
                source.Stop();
            }

            currentClipIndex += 1;

            if (currentClipIndex >= clips.Length) {
                currentClipIndex = 0;
            }

            source.PlayOneShot(clips[currentClipIndex]);

            yield return new WaitUntil(() => {
                return (!source.isPlaying);
            });

            yield return waitForEachTrack;
        }
    }

    public void Stop()
    {
        if (playSongCoroutine != null) {
            StopCoroutine(playSongCoroutine);
            playSongCoroutine = null;
        }

        source.Stop();
    }
}

