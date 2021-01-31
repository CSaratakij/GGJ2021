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

    [Header("Start scene")]
    [SerializeField]
    public SceneIndex startScene = SceneIndex.Level;

    PlayerProfilePreset playerPreset;
    PlayerProfile player;

    GameState currentState;
    GameState previousState;

    // Hacks;
    float happiness;

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
        player.cat = new NpcProfile(profile.haveCat);
        player.girl = new NpcProfile(profile.haveGirlFriend);

        happiness = profile.happiness;
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
        if (player == null) { 
            return;
        }

        var money = player.money;
        var amount = 0;

        if (money < 5000) {
            amount = 127;
        }
        else if (money < 15000) {
            amount = 183;
        }
        else if (money < 30000) {
            amount = 340;
        }
        else if (money < 50000) {
            amount = 466;
        }
        else {
            amount = 800;
        }

        money -= amount;
        player.money = money;
    }

    public void GainHappiness()
    {
        if (player == null)
            return;

        if (player.happiness > 0) {
            player.happiness -= 1;
        }
        else {
            player.happiness += 3;
        }
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
        ChangeScene(startScene);
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
        RandomPlayerProfile();
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

