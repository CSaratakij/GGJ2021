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
    RectTransform shopShelf;

    [SerializeField]
    CanvasGroup itemDialog;

    [SerializeField]
    Button btnItem;

    [SerializeField]
    ItemScriptableObject[] shopDetails;

    [SerializeField]
    RectTransform[] shopItems;

    [Header("Notification Setting")]
    [SerializeField]
    TextMeshProUGUI lblNotificationCount;

    [SerializeField]
    Button btnNotification;

    [Header("PlayerStat Setting")]
    [SerializeField]
    TextMeshProUGUI lblStat;

    [SerializeField]
    TextMeshProUGUI lblSalary;

    [Header("DayProgress Setting")]
    [SerializeField]
    Slider daySlider;

    [SerializeField]
    Color monthLableBGActiveColor;

    [SerializeField]
    Image[] monthLableBG;

    [Header("Event Summary Setting")]
    [SerializeField]
    CanvasGroup[] eventSummaryImages;

    [Header("Dependencies")]
    [SerializeField]
    InnerTime innerTime;

    [Header("Dependencies")]
    [SerializeField]
    EventDirector eventDirector;

    Button[] btnBuys;

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

            lblSalary.SetText($"Salary : {GameController.Instance.Player.salary}");
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

        itemDialog.alpha = 0;
        itemDialog.blocksRaycasts = false;
        itemDialog.interactable = false;

        foreach (var item in monthLableBG)
        {
            var child = item.transform.GetChild(1);
            var label = child.GetComponent<TextMeshProUGUI>();

            label?.SetText($"{id}");
            id += 1;
        }

        monthLableBG[0].color = monthLableBGActiveColor;

        foreach (var item in eventSummaryImages)
        {
            item.alpha = 0;
        }

        btnItem.onClick.AddListener(() => {
            ToggleItemPanel();
        });

        btnNotification.onClick.AddListener(() => {
            ActivateLastOptionalEvent();
        });

        for (int i = 0; i < shopDetails.Length; ++i)
        {
            var item = shopDetails[i];
            var obj = shopItems[i];

            /* var obj = Instantiate(shopListPrefabs); */
            /* obj.SetParent(shopShelf.transform); */

            var image = obj.transform.GetChild(0).GetComponent<Image>();
            var text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var button = obj.transform.GetChild(2).GetComponent<Button>();

            image.sprite = item.sprite;
            text.SetText(item.itemName);

            button.onClick.AddListener(() => {
                GameController.Instance.Player.BuyItem(item);
                Debug.Log("Buy : " + item.itemName);

                button.interactable = false;

                var buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                buttonText.SetText("Sold out");
                buttonText.color = Color.white;
            });

            /* obj.gameObject.SetActive(true); */
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
        daySlider.value += 1.0f;
    }

    void OnMonthPass(DateTime date)
    {
        daySlider.value = (date.Month - 1) * 30;
        monthLableBG[date.Month - 1].color = monthLableBGActiveColor;
    }

    void ToggleItemPanel()
    {
        bool shouldShow = (itemDialog.alpha > 0);
        shouldShow = !shouldShow;

        itemDialog.alpha = shouldShow ? 1.0f : 0.0f;
        itemDialog.interactable = shouldShow;
        itemDialog.blocksRaycasts = shouldShow;

        innerTime.Pause(shouldShow);
        btnNotification.interactable = !shouldShow;
    }

    void ActivateLastOptionalEvent()
    {
        eventDirector.StartCurrentOptionalScenario();
    }

    public void NotifyNotification(bool isNotify = true)
    {
        var number = (isNotify) ? 1 : 0;
        lblNotificationCount.SetText($"{number}");
    }

    // TODO : need inventory
    public void UpdateItemSummary(ItemScriptableObject itemObject)
    {
        /* update item summary here */
    }

    // TODO : update if player have certain item
    // change certain button to sold-out and mark it to not interactable
    public void UpdateShoplist()
    {
        foreach (var item in shopDetails)
        {
            var player = GameController.Instance.Player;

            if (!player.HasItem(item))
            {
                // TODO : update ui state here
            }
        }
    }
}

