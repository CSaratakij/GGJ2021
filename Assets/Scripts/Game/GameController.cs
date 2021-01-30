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

    [Header("Game time")]
    [SerializeField]
    float secondsUntilDay = 3.0f;

    public Action<GameState> OnGameStateChange;
    public GameState State => currentState;
    public bool IsGamePause => (GameState.Pause == currentState);
    public PlayerProfile Player => player;

    PlayerProfilePreset playerPreset;
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

        playerPreset = playerProfiles[id];
        PlayerProfilePreset profile = playerProfiles[id];

        player.happiness = profile.happiness;
        player.money = profile.money;
        player.salary = profile.salary;
        player.haveCat = profile.haveCat;
        player.haveGirlFriend = profile.haveGirlFriend;
        player.haveBeggar = true;
        player.haveSaleman = true;

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

    public void GetSalary()
    {
        if (playerPreset == null) {
            Debug.LogWarning("Should add salary, but no player preset found.");
            return;
        }

        player.money += playerPreset.salary;
    }

    public void RemoveSalaryPerDay()
    {
        if (playerPreset == null) { 
            Debug.LogWarning("Should remove salary, but no player preset found.");
            return;
        }

        double removeMaxAmountPerMonth = (playerPreset.salary * 35) / 100;
        player.money -= (int)(removeMaxAmountPerMonth / 30);
    }

    public void ResetGameState()
    {
        ChangeGameState(GameState.MainMenu);
    }

    public void RestartGame()
    {
        OnGameStateChange = null;
        ResetGameState();

        GameStart();
        ChangeScene(SceneIndex.Level);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;

        if (currentState == GameState.Pause)
        {
            currentState = previousState;
        }
        
        Debug.Log($"Current State {currentState}");
    }

    public void GameStart()
    {
        ResetGameState();
        ChangeGameState(GameState.Start);
    }

    public void ShowProfile()
    {
        ChangeGameState(GameState.ShowProfile);
    }

    public void BeginPlay()
    {
        ChangeGameState(GameState.Normal);
    }

    public void GameOver()
    {
        ChangeGameState(GameState.End);
    }

    public void ChangeScene(SceneIndex sceneIndex)
    {
        SceneManager.LoadScene((int)sceneIndex);
    }

    public void BackToMainMenu()
    {
        ResetGameState();
        ChangeScene(SceneIndex.MainMenu);
    }
}

