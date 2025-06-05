using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MinimapGeneralController : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private GameObject minimapCamera;
    [SerializeField] private GameObject minimapFrame;
    [SerializeField] private GameObject toggleButton;

    [Header("Button settings")]
    [SerializeField] private Vector3 toggledOnCoordinates;
    [SerializeField] private Vector3 toggledOffCoordinates;

    private bool minimapOn = true;
    private RectTransform buttonTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonTransform = toggleButton.GetComponent<RectTransform>();
        buttonTransform.anchoredPosition = toggledOnCoordinates;
        SetMinimapState();
    }

    public void ToggleMinimap()
    {
        minimapOn = !minimapOn;

        if (minimapOn)
        {
            buttonTransform.anchoredPosition = toggledOnCoordinates;
            PlayerPrefs.SetInt("MinimapToggled", 1);
        }

        else
        {
            buttonTransform.anchoredPosition = toggledOffCoordinates;
            PlayerPrefs.SetInt("MinimapToggled", 0);
        }

        minimapFrame.SetActive(minimapOn);
        minimapCamera.SetActive(minimapOn);
    }

    private void SetMinimapState()
    {
        if (PlayerPrefs.GetInt("MinimapToggled") == 1)
        {
            minimapOn = true;
            buttonTransform.anchoredPosition = toggledOnCoordinates;
        }

        else
        {
            minimapOn = false;
            buttonTransform.anchoredPosition= toggledOffCoordinates;
        }

        minimapFrame.SetActive(minimapOn);
        minimapCamera.SetActive(minimapOn);
    }
}
