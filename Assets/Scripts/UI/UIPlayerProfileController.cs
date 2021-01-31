using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerProfileController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI lblProfileInfo;

    [SerializeField]
    CanvasGroup dialog;

    [SerializeField]
    Button btnStart;

    [SerializeField]
    EventDirector eventDirector;

    void Awake()
    {
        Initialize();
        GameController.Instance?.RandomPlayerProfile();
    }

    void Start()
    {
        var remark = GameController.Instance?.Player.remark;
        lblProfileInfo.SetText(remark);
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

