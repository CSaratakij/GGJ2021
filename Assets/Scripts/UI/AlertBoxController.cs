using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertBoxController : MonoBehaviour
{
    [Header("General Setting")]
    [SerializeField]
    CanvasGroup[] dialogs;

    [SerializeField]
    TextMeshProUGUI[] lblMessages;

    [SerializeField]
    TextMeshProUGUI lblNpcName;

    [SerializeField]
    Image imgNpc;

    [SerializeField]
    Button[] simpleButtons;

    [SerializeField]
    Button[] npcDialogButtons;

    [Header("Dependencies")]
    [SerializeField]
    EventDirector eventDirector;

    [SerializeField]
    UIInGameController uiInGame;

    public enum MessageType
    {
        Simple,
        Npc
    }

    public Action<int> OnSelectButton;

    bool isShow = false;
    MessageType messageType;

    CanvasGroup currentDialog;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < simpleButtons.Length; ++i)
        {
            int id = i;
            simpleButtons[i].onClick.AddListener(() => {
                OnSelectButton?.Invoke(id);
            });
        }

        for (int i = 0; i < npcDialogButtons.Length; ++i)
        {
            int id = i;
            npcDialogButtons[i].onClick.AddListener(() => {
                OnSelectButton?.Invoke(id);
            });
        }
    }

    void SetDialogActive(MessageType messageType, bool isActive = true)
    {
        foreach (CanvasGroup dialog in dialogs) {
            dialog.alpha = 0;
            dialog.interactable = false;
            dialog.blocksRaycasts = false;
        }

        currentDialog = dialogs[(int)messageType];

        currentDialog.alpha = (isActive) ? 1.0f : 0.0f; // <--- this better be the animation
        currentDialog.interactable = isActive;
        currentDialog.blocksRaycasts = isActive;
    }

    public void SetMessageInfo(string message, string[] choices, NpcScriptableObject npcInfo = null)
    {
        messageType = (npcInfo == null) ? MessageType.Simple : MessageType.Npc;
        lblMessages[(int)messageType].SetText(message);

        if (MessageType.Npc == messageType) {
            lblNpcName.SetText(npcInfo.npcName);
            imgNpc.sprite = npcInfo.sprite;
        }

        bool useDefaultChoice = (choices == null) || (choices.Length <= 0);
        Button[] buttons = (MessageType.Simple == messageType) ? simpleButtons : npcDialogButtons;

        if (useDefaultChoice) {
            for (int i = 0; i < buttons.Length; ++i) {
                bool shouldShow = (i == 0);
                buttons[i].gameObject.SetActive(shouldShow);

                if (shouldShow) {
                    var label = buttons[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    label.SetText("OK");
                }
            }
        }
        else {
            for (int i = 0; i < buttons.Length; ++i) {
                bool shouldShow = (i < (choices.Length));
                buttons[i].gameObject.SetActive(shouldShow);

                if (shouldShow) {
                    string text = choices[i];
                    var label = buttons[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    label.SetText(text);
                }
            }
        }

        SetDialogActive(messageType);
    }

    public void Show(bool value = true)
    {
        isShow = value;
        uiInGame.ShowAlertBox(value);
    }
}

