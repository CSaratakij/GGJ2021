using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class UIMainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    Button[] buttons;

    [Header("Credit UI")]
    [SerializeField]
    CanvasGroup creditUI;

    [SerializeField]
    TextMeshProUGUI lblCredit;

    [SerializeField]
    Button btnCreditBack;

    [SerializeField]
    CreditInfo[] creditInfo;

    [System.Serializable]
    struct CreditInfo
    {
        public string position;
        public string name;
    }

    enum ButtonIndex
    {
        Start,
        HowToPlay,
        Credit,
        Quit
    }

    void Awake()
    {
        ShowCreditUI(false);
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        bool shouldHideCreditUI = Input.GetButtonDown("Cancel") && (creditUI.alpha > 0.0f);

        if (shouldHideCreditUI) {

        }
    }

    void Initialize()
    {
        buttons
            [(int)ButtonIndex.Start].
            onClick.
            AddListener(() => {
                    GameController.Instance?.GameStart();
                    GameController.Instance?.ChangeScene(GameController.Instance.startScene);
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
                    ShowCreditUI(true);
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

        // Credit UI
        string creditText = "";

        foreach (var item in creditInfo)
        {
            creditText += $"{item.position} | {item.name}{System.Environment.NewLine}";
        }

        lblCredit.SetText(creditText);

        btnCreditBack.onClick.AddListener(() => {
            ShowCreditUI(false);
        });
    }

    void ShowCreditUI(bool value = true)
    {
        creditUI.alpha = (value) ? 1 : 0;
        creditUI.interactable = value;
        creditUI.blocksRaycasts = value;
    }
}

