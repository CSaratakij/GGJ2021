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

public class UIGameOverController : MonoBehaviour
{
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

    public float autopopupdelay = 1f;

    private void Awake()
    {
        dialogPopup.SetActive(false);
    }

    IEnumerable DelayPopup()
    {
        yield return new WaitForSeconds(autopopupdelay);
        dialogPopup.SetActive(true);
    }

    private void Start()
    {
        EndingPreset endingPreset = GameController.Instance?.endingPreset;
        if(endingPreset == null)
        {
            endingPreset = defaultEndingPreset;
        }
        if (endingPreset.sprite != null)
        {
            endingImage.sprite = endingPreset.sprite;
        }
        lblEnding.text = endingPreset.endingName;
        txtDescription.text = endingPreset.description;
        StartCoroutine("DelayPopup");
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
