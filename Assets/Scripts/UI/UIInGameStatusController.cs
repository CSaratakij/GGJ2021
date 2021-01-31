using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInGameStatusController : MonoBehaviour
{
    [Header("Shop Setting")]
    [SerializeField]
    RectTransform shopListPrefabs;

    [SerializeField]
    CanvasGroup itemDialog;

    [SerializeField]
    Button btnItem;

    [SerializeField]
    Button[] btnBuy;

    [SerializeField]
    ItemScriptableObject[] shopDetails;

    [Header("PlayerStat Setting")]
    [SerializeField]
    TextMeshProUGUI lblStat;

    [Header("DayProgress Setting")]
    [SerializeField]
    Slider daySlider;

    [SerializeField]
    Color monthLableBGActiveColor;

    [SerializeField]
    Image[] monthLableBG;

    [Header("Dependencies")]
    [SerializeField]
    InnerTime innerTime;

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
        if (Time.frameCount % 3 == 0) {
            lblStat.SetText(
                $@"Happiness : {GameController.Instance.Player.happiness}
                Money : {GameController.Instance.Player.money}"
            );
        }
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void Initialize()
    {
        daySlider.value = 1;
        int id = 1;

        foreach (var item in monthLableBG)
        {
            var child = item.transform.GetChild(1);
            var label = child.GetComponent<TextMeshProUGUI>();

            label?.SetText($"{id}");
            id += 1;
        }
    }

    void SubscribeEvent()
    {
        innerTime.OnDayPass += OnDayPass;
        innerTime.OnMonthPass += OnMonthPass;
    }

    void UnsubscribeEvent()
    {
        innerTime.OnDayPass -= OnDayPass;
        innerTime.OnMonthPass -= OnMonthPass;
    }

    void OnDayPass(DateTime date)
    {
        daySlider.value += 1;
    }

    void OnMonthPass(DateTime date)
    {
        monthLableBG[date.Month - 1].color = monthLableBGActiveColor;
    }
}

