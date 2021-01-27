using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance = null;

    [Header("Players")]
    [SerializeField]
    PlayerProfilePreset[] playerProfiles;

    public Action<GameState> OnGameStateChange;
    public GameState State => currentState;
    public bool IsGamePause => (GameState.Pause == currentState);

    PlayerProfile player;

    GameState currentState;
    GameState previousState;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        MakeSingleton();
        Random.InitState(Random.Range(0, 100));
        player = new PlayerProfile();
    }

    void MakeSingleton()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    PlayerProfile RandomPlayerProfile()
    {
        Random.InitState(Random.Range(0, 100));

        int id = Random.Range(0, playerProfiles.Length);
        PlayerProfilePreset profile = playerProfiles[id];

        player.happiness = profile.happiness;
        player.money = profile.money;

        return player;
    }

    void ChangeGameState(GameState state)
    {
        if (State == state) {
            return;
        }

        previousState = currentState;
        currentState = state;

        OnGameStateChange?.Invoke(state);
    }

    public void Pause(bool value = true)
    {
        Time.timeScale = (value) ? 0 : 1;

        var state = (value) ? GameState.Pause : previousState;
        ChangeGameState(state);

        Debug.Log($"Current State {state}");
    }

    public void ResetGameState()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void GameStart()
    {
        RandomPlayerProfile();
        ChangeGameState(GameState.Start);
    }

    public void GameOver()
    {
        ChangeGameState(GameState.End);
    }

    public void ChangeScene(SceneIndex sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }
}

