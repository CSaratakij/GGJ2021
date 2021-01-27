using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class UIMainMenuController : MonoBehaviour
{
    [SerializeField]
    Button[] buttons;

    enum ButtonIndex
    {
        Start,
        HowToPlay,
        Credit,
        Quit
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        buttons
            [(int)ButtonIndex.Start].
            onClick.
            AddListener(() => {
                    GameController.Instance?.ChangeScene(SceneIndex.Level);
        });

        buttons
            [(int)ButtonIndex.HowToPlay].
            onClick.
            AddListener(() => {
                    //TODO
        });

        buttons
            [(int)ButtonIndex.Credit].
            onClick.
            AddListener(() => {
                    //TODO
        });

        buttons
            [(int)ButtonIndex.Quit].
            onClick.
            AddListener(() => {
                #if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
        });
    }
}

