using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertBoxController : MonoBehaviour
{
    [SerializeField]
    CanvasGroup[] messages;

    [SerializeField]
    TextMeshProUGUI lblMessage;

    [SerializeField]
    TextMeshProUGUI lblNpcName;

    [SerializeField]
    Image npcImage;

    [SerializeField]
    Button[] buttons;

    public enum MessageType
    {
        Simple,
        Npc
    }

    public Action<int> OnSelectButton;

    bool isShow = false;
    MessageType messageType;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i <  buttons.Length; ++i)
        {
            var id = i;
            buttons[i].onClick.AddListener(() => {
                OnSelectButton?.Invoke(id);
            });
        }
    }

    public void SetMessageInfo(string message, NpcScriptableObject npcInfo = null, MessageType messageType = MessageType.Simple)
    {
        // set canvas base on the message type
    }

    public void Show(bool value = true)
    {
        isShow = value;
    }
}

