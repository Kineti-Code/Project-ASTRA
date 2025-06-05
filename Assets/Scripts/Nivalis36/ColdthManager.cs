using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColdthManager : MonoBehaviour
{
    [SerializeField] private Slider coldthIndicator;

    [Header("Properties")]

    [SerializeField] private float heatDistance = 10f;
    [SerializeField, Range(0.01f, 0.5f)] private float heatUpTime = 0.01f;
    [SerializeField, Range(0.01f, 0.5f)] private float coolDownTime = 0.01f;

    private GameObject[] heatSources;
    private Coroutine currentCoroutine;
    private Transform playerTransform;
    private Player playerScript;

    private bool isFullyHeated = false;
    private enum CoroutineType { None, Heating, Cooling }
    private CoroutineType currentCoroutineType = CoroutineType.None;
    void Start()
    {
        InitializeColdSystem();
    }

    public void InitializeColdSystem()
    {
        // Reset all states on respawn
        isFullyHeated = false;
        currentCoroutineType = CoroutineType.None;
        coldthIndicator.value = 0;

        // Refresh references
        playerTransform = transform.parent;
        playerScript = GetComponentInParent<Player>();
        heatSources = GameObject.FindGameObjectsWithTag("HeatSource");

        // Restart the initial coroutine
        SwitchCoroutine(PlayerCooling(), CoroutineType.Cooling);
    }

    void Update()
    {
        bool isNearHeatSource = CheckHeatProximity();

        // Reset isFullyHeated when moving away from the heat source
        if (!isNearHeatSource && isFullyHeated)
        {
            isFullyHeated = false;
        }

        bool shouldHeat = isNearHeatSource && coldthIndicator.value > 0;
        bool shouldCool = !isNearHeatSource && coldthIndicator.value < 1;

        // Only switch coroutines if state changes
        if (shouldHeat && currentCoroutineType != CoroutineType.Heating)
        {
            SwitchCoroutine(PlayerHeating(), CoroutineType.Heating);
        }
        else if (shouldCool && currentCoroutineType != CoroutineType.Cooling)
        {
            SwitchCoroutine(PlayerCooling(), CoroutineType.Cooling);
        }

        if (coldthIndicator.value >= 0.7f)
        {
            SlowDownPlayer();
        }
    }


    void SlowDownPlayer()
    {
        playerScript.speed = (-36.67f * coldthIndicator.value) + 36.67f;
    }

    bool CheckHeatProximity()
    {
        foreach (var heatSource in heatSources)
        {
            if (Vector3.Distance(playerTransform.position, heatSource.transform.position) <= heatDistance)
            {
                return true;
            }
        }
        return false;
    }

    void SwitchCoroutine(IEnumerator newCoroutine, CoroutineType newType)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(newCoroutine);
        currentCoroutineType = newType;
    }

    IEnumerator PlayerCooling()
    {
        while (coldthIndicator.value < 1 && !isFullyHeated)
        {
            coldthIndicator.value = Mathf.Min(coldthIndicator.value + coolDownTime, 1);
            yield return new WaitForSeconds(0.5f);

            if (Mathf.Approximately(coldthIndicator.value, 1f))
            {
                Debug.Log("Player frozen!");
                playerScript.Respawn();
                playerScript.speed = 11f;

                InitializeColdSystem();
                break;
            }
        }
    }

    IEnumerator PlayerHeating()
    {
        while (coldthIndicator.value > 0 && !isFullyHeated)
        {
            coldthIndicator.value = Mathf.Max(coldthIndicator.value - heatUpTime, 0);
            yield return new WaitForSeconds(0.5f);

            if (Mathf.Approximately(coldthIndicator.value, 0f))
            {
                isFullyHeated = true;
                if (currentCoroutine != null)
                    StopCoroutine(currentCoroutine);
                break;
            }
        }
    }
}