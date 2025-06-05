using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{

    [Header("Required components")]
    [SerializeField] private Volume volume;
    [SerializeField] private RectTransform tabletTransform;
    [SerializeField] private PopupMessenger popupMessenger;
    [SerializeField] private Player player;
    [SerializeField] private UniversalControlHandler universalControlHandler;
    [SerializeField] private TrapdoorController trapdoorController;
    [SerializeField] private GameObject dividedFloor;
    [SerializeField] private GameObject normalFloor;
    [SerializeField] private GameObject levelSelectPointer;

    [Header("Lighting components")]
    [SerializeField] private GameObject artifialLighting;
    [SerializeField] private Light2D globalLight;

    [Header("Tutorial Ending Settings")]
    [SerializeField] private float vignetteFadeSpeed = 1f;
    [SerializeField] private float brightnessIncreaseSpeed = 1f;

    [Header("Tablet movement settings")]
    [SerializeField] private float _movementSpeed = 1f;
    [SerializeField] private float _movementRange = 20f;

    [Header("Initial tablet popup settings")]
    [SerializeField] private string title;
    [SerializeField, TextArea(3, 10)] private string[] pages;

    private bool tabletNotClicked = true;
    private Vector3 tabletStartPos;
    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (PlayerPrefs.GetInt("Tutorial progress"))
        {
            case 0:
                InitializeStart();
                break;
            case 1:
                InitializeEnd();
                break;
        }

        audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void InitializeStart()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = true;
            vignette.intensity.value = 1f;
        }

        dividedFloor.SetActive(true);
        normalFloor.SetActive(false);
        levelSelectPointer.SetActive(false);
        tabletTransform.localPosition = new Vector3(-2.6f, -70, 0);
        tabletStartPos = tabletTransform.localPosition;
        artifialLighting.SetActive(false);
        trapdoorController.gameObject.SetActive(true);
        universalControlHandler.pauseDisabled = true;
        globalLight.intensity = 0.1f;
        player.controlsEnabled = false;
        player.autoRespawn = false;
        StartCoroutine(MovetabletTransform());
        StartCoroutine(SendTabletPopup());
        PlayerPrefs.SetInt("Tutorial progress", 1);
    }

    private void InitializeEnd()
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = true;
            vignette.intensity.value = 1f;
        }

        player.controlsEnabled = false;
        artifialLighting.SetActive(false);
        globalLight.intensity = 0.1f;
        StartCoroutine(VignetteFadeOut());
        StartCoroutine(IncreaseBrightness());
        levelSelectPointer.SetActive(true);
        PlayerPrefs.SetInt("Tutorial progress", 2);
    }

    private IEnumerator IncreaseBrightness()
    {
        while (!Mathf.Approximately(globalLight.intensity, 0.2f))
        {
            globalLight.intensity = Mathf.MoveTowards(globalLight.intensity, 0.2f, Time.deltaTime * brightnessIncreaseSpeed);
            yield return null;
        }
    }

    private IEnumerator VignetteFadeOut()
    {
        yield return new WaitForSeconds(1.3f);

        if (volume.profile.TryGet(out Vignette vignette))
        {
            while (!Mathf.Approximately(vignette.intensity.value, 0f))
            {
                vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, 0f, Time.deltaTime * vignetteFadeSpeed);
                yield return null;
            }
        }

        vignette.active = false;

        yield return new WaitForSeconds(1f);
        player.controlsEnabled = true;
        audioManager.PlaySFX(audioManager.tubeLightOn);
        artifialLighting.SetActive(true);
    }

    private IEnumerator MovetabletTransform()
    {
        while (tabletNotClicked)
        {
            float verticalOffset = Mathf.Sin(Time.time * _movementSpeed) * _movementRange;
            tabletTransform.localPosition = tabletStartPos + new Vector3(0, verticalOffset, 0);
            yield return null;
        }
    }

    private IEnumerator SendTabletPopup()
    {
        yield return new WaitForSeconds(3f);
        popupMessenger.QueueTabletMessage(title, pages);
        StartCoroutine(WaitForPopupCompletion());
    }

    private IEnumerator WaitForPopupCompletion()
    {
        while (popupMessenger.popupQueued)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        trapdoorController.OpenTrapdoor(true);
        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(3);
    }
}
