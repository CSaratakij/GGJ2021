using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInGameController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup[] panels;

    [SerializeField]
    InnerTime innerTime;

    enum Panel
    {
        InGame,
        GoalNote,
        Alert,
        PlayerProfile,
        AdvanceTimeTransition,
        Pause
    }

    CanvasGroup currentPanel;

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
        innerTime.OnStartAdvanceTime += OnStartAdvanceTime;
        innerTime.OnFinishAdvanceTime += OnFinishAdvanceTime;
    }

    void UnsubscribeEvent()
    {
        GameController.Instance.OnGameStateChange -= OnGameStateChange;
        innerTime.OnStartAdvanceTime -= OnStartAdvanceTime;
        innerTime.OnFinishAdvanceTime -= OnFinishAdvanceTime;
    }

    void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.ShowProfile:
                HideAll();
                Show(Panel.PlayerProfile);
            break;

            case GameState.Normal:
                Show(Panel.InGame);
            break;

            case GameState.End:
                // Show game over ui here
                Debug.Log("Game Over UI..");
            break;

            default:
                break;
        }
    }

    void OnStartAdvanceTime()
    {
        FadeIn();
    }

    void OnFinishAdvanceTime()
    {
        FadeOut();
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

    void Show(Panel panel, bool isShow = true)
    {
        Show((int)panel, isShow);
    }

    void Show(int id, bool isShow = true)
    {
        var panel = panels[id];

        panel.alpha = (isShow) ? 1.0f : 0.0f;
        panel.interactable = isShow;
        panel.blocksRaycasts = isShow;

        currentPanel = panel;
    }

    void FadeIn()
    {
        //TODO : fade anim
        Show(Panel.AdvanceTimeTransition);
    }

    void FadeOut()
    {
        //TODO : fade anim
        Show(Panel.AdvanceTimeTransition, false);
    }

    public void ShowAlertBox(bool isShow = true)
    {
        Show((int)Panel.Alert, isShow);
    }

    public void ShowIngameUI(bool isShow = true)
    {
        Show((int)Panel.InGame, isShow);
    }
}

