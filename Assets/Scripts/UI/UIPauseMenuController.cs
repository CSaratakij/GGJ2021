using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenuController : MonoBehaviour
{
    [SerializeField]
    Button[] buttons;

    [SerializeField]
    UIInGameController ingameUI;

    enum ButtonType
    {
        Resume,
        Restart,
        MainMenu
    }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        buttons[(int)ButtonType.Resume].
            onClick.
            AddListener(() => {
                    ingameUI.UnPause();
        });

        buttons[(int)ButtonType.Restart].
            onClick.
            AddListener(() => {
                    ingameUI.UnPause();
                    GameController.Instance?.RestartGame();
        });

        buttons[(int)ButtonType.MainMenu].
            onClick.
            AddListener(() => {
                    ingameUI.UnPause();
                    GameController.Instance?.BackToMainMenu();
        });
    }
}

