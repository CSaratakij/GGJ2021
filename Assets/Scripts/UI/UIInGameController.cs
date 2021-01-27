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
        Pause,
        GoalNote,
        Alert,
        InGame
    }

    Panel currentPanel;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        InputHandler();
    }

    void Initialize()
    {
        if (GameState.Start != GameController.Instance?.State) {
            Debug.LogError("Game hasn't start properly...");
        }

        HideAll();
        Show(Panel.GoalNote);
    }

    void SubscribeEvent()
    {

    }

    void UnsubscribeEvent()
    {

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
        }
    }

    void Show(int id, bool isShow = true)
    {
        currentPanel = (Panel)id;
        var panel = panels[id];

        panel.alpha = (isShow) ? 1 : 0;
        panel.interactable = isShow;
    }

    void Show(Panel panel, bool isShow = true)
    {
        Show((int)panel, isShow);
    }
}

