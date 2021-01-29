using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInGameController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup[] panels;

    enum Panel
    {
        InGame,
        GoalNote,
        Alert,
        PlayerProfile,
        Pause
    }

    Panel currentPanel;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        SubscribeEvent();
    }

    void Update()
    {
        InputHandler();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void Initialize()
    {
        if (GameState.Start != GameController.Instance?.State) {
            Debug.LogError("Game hasn't start properly...");
        }

        HideAll();
    }

    void SubscribeEvent()
    {
        GameController.Instance.OnGameStateChange += OnGameStateChange;
    }

    void UnsubscribeEvent()
    {
        GameController.Instance.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameState state)
    {
        if (GameState.ShowProfile == state) {
            HideAll();
            Show(Panel.PlayerProfile);
        }
        else if (GameState.Normal == state) {
            Show(Panel.InGame);
        }
    }

    void InputHandler()
    {
        if (Input.GetButtonDown("Cancel")) {
            ToggleShowInGamePanel();
        }
    }

    void ToggleShowInGamePanel()
    {
        if (GameController.Instance == null) {
            Debug.LogError("Cannot find the game controller instance...");
            return;
        }

        var shouldPause = GameController.Instance.IsGamePause;

        shouldPause = !shouldPause;
        Show(Panel.Pause, shouldPause);

        GameController.Instance?.Pause(shouldPause);
    }

    void HideAll()
    {
        foreach (CanvasGroup panel in panels) {
            panel.alpha = 0;
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }
    }

    void Show(int id, bool isShow = true)
    {
        currentPanel = (Panel)id;
        var panel = panels[id];

        panel.alpha = (isShow) ? 1 : 0;
        panel.interactable = isShow;
        panel.blocksRaycasts = isShow;
    }

    void Show(Panel panel, bool isShow = true)
    {
        Show((int)panel, isShow);
    }

    public void ShowAlertBox(bool isShow = true)
    {
        Show((int)Panel.Alert, isShow);
    }
}

