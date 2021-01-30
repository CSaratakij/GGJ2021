using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerProfileController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup dialog;

    [SerializeField]
    Button btnStart;

    [SerializeField]
    EventDirector eventDirector;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        btnStart.
            onClick.
            AddListener(() => {
                    ShowUI(false);
                    eventDirector.BeginPlay();
        });
    }

    void ShowUI(bool value = true)
    {
        dialog.alpha = (value) ? 1.0f : 0.0f;
        dialog.interactable = value;
        dialog.blocksRaycasts = value;
    }

    public void ShowProfile()
    {
        ShowUI();
    }
}

