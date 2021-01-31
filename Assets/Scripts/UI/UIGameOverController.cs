using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameOverController : MonoBehaviour
{
    [Header("Ending UI")]
    [SerializeField]
    GameObject endingCanvas;

    [SerializeField]
    Image endingImage;

    [SerializeField]
    TextMeshProUGUI lblEnding;

    [SerializeField]
    TextMeshProUGUI txtDescription;

    [SerializeField]
    EndingPreset defaultEndingPreset;

    private void Awake()
    {
        endingCanvas.SetActive(false);
    }

    private void Start()
    {
        EndingPreset endingPreset = GameController.Instance?.endingPreset;
        if(endingPreset != null)
        {
            endingPreset = defaultEndingPreset;
        }
        endingImage.sprite = endingPreset.sprite;
        lblEnding.text = endingPreset.endingName;
        txtDescription.text = endingPreset.description;
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
