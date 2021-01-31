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

    float startPlayTime = 0.0f;
    bool shouldMoveToNextTrack = false;

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

    public void UnPause()
    {
        source.UnPause();
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

            startPlayTime = Time.time;
            source.PlayOneShot(clips[currentClipIndex]);

            yield return new WaitUntil(() => {
                if (GameState.Pause == GameController.Instance.State) {
                    return false;
                }
                return (!source.isPlaying);
            });

            shouldMoveToNextTrack = false;
            yield return waitForEachTrack;
        }
    }

    public void Stop()
    {
        if (playSongCoroutine != null) {
            StopCoroutine(playSongCoroutine);
            playSongCoroutine = null;
        }

        shouldMoveToNextTrack = false;
        startPlayTime = 0.0f;

        source.Stop();
    }
}

