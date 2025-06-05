using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Storm : MonoBehaviour
{
    [Header("Required gameobjects")]

    [SerializeField] private Image stormOverlay;
    [SerializeField] private UniversalControlHandler universalControlHandler;

    [Header("Storm controls")]

    [SerializeField] private float gracePeriod = 5f; // Time to fade the alpha value
    [SerializeField, Range(0, 1)] private float maxStormIntensity = 0.65f;

    [SerializeField] private Bunker[] bunkers;
    private Bunker currentBunker;
    private float timeToAlphaChange;

    private void OnEnable()
    {
        bunkers = FindObjectsByType<Bunker>(FindObjectsSortMode.None);

        if (stormOverlay != null)
        {
            stormOverlay.color = new Color(1, 1, 1, 0); // Start with alpha = 0 (fully transparent)
            timeToAlphaChange = gracePeriod / (maxStormIntensity * 100);
            StartCoroutine(FadeInStorm());
        }
        else
        {
            Debug.LogWarning("Storm Overlay Image not assigned!");
        }
    }

    private IEnumerator FadeInStorm()
    {
        for (float x = 0; x <= maxStormIntensity; x += 0.01f)
        {
            yield return new WaitForSeconds(timeToAlphaChange);
            stormOverlay.color = new Color(1, 1, 1, x);
        }

        yield return new WaitForSeconds(Random.Range(5, 10));
        currentBunker.SetPlayersSortingLayer(2);
        currentBunker.stormSurvived = true;
        currentBunker.TogglePlayerControls(true);
        currentBunker = null;
        gameObject.SetActive(false);
    }

    private bool CheckPlayerSafe()
    {
        bool notAllPlayersInABunker = true;

        foreach (var bunker in bunkers)
        {
            if (bunker.allPlayersInBunker)
            {
                notAllPlayersInABunker = false;
                currentBunker = bunker;
                break;
            }
        }

        return notAllPlayersInABunker;
    }

    public void ReactivateBunkers()
    {
        foreach (var bunker in bunkers)
        {
            bunker.stormActivated = false;
            bunker.stormSurvived = false;
            bunker.allPlayersInBunker = false;
        }
    }

    public void Reset()
    {
        stormOverlay.color = new Color(1, 1, 1, 0);

        if (currentBunker != null)
        {
            currentBunker.TogglePlayerControls(true);
            currentBunker.SetPlayersSortingLayer(2);
        }
    }

    private void Update()
    {

        if (Mathf.Abs(stormOverlay.color.a - maxStormIntensity) < 0.01f && CheckPlayerSafe())
        {
            Debug.Log("players aint safe");
            universalControlHandler.ReloadLevel();
        }

        // don't do anything if players are safe
    }
}
