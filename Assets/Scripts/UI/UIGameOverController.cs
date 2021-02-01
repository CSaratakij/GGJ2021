using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIGameOverController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    GameObject dialogPopup;

    [SerializeField]
    Image endingImage;

    [SerializeField]
    TextMeshProUGUI lblEnding;

    [SerializeField]
    TextMeshProUGUI txtDescription;

    [SerializeField]
    EndingPreset defaultEndingPreset;

    AudioSource audioSource;
    AudioClip endingClip;

    public float autopopupdelay = 1f;

    void Awake()
    {
        dialogPopup.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        var endingPreset = GameController.Instance?.endingPreset;

        if (endingPreset == null)
        {
            endingPreset = defaultEndingPreset;
        }

        if (endingPreset.sprite != null)
        {
            endingImage.sprite = endingPreset.sprite;
        }

        lblEnding.text = endingPreset.endingName;
        txtDescription.text = endingPreset.description;
        endingClip = endingPreset.endingBGM;

        StartCoroutine(DelayPopup());
    }

    IEnumerator DelayPopup()
    {
        yield return new WaitForSeconds(autopopupdelay);

        dialogPopup.SetActive(true);
        audioSource.PlayOneShot(endingClip);
    }

    public void BackToMainmenu()
    {
        GameController gameController = GameController.Instance;

        if (gameController != null) {
            gameController.ChangeScene(SceneIndex.MainMenu);
        }
        else
        {
            SceneManager.LoadScene((int)SceneIndex.MainMenu);
        }
    }
}
