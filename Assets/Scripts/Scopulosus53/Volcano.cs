using UnityEngine;

public class Volcano : MonoBehaviour
{
    [SerializeField] private GameObject _steamColumn;
    [SerializeField] private GameObject _steamCloud;

    private float _nextEruptionDelay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _steamColumn.SetActive(false);
        _steamCloud.SetActive(false);
        ScheduleNextEruption();
    }

    private void ScheduleNextEruption()
    {
        _nextEruptionDelay = Random.Range(5f, 8f); // Random delay for the next eruption
        Invoke(nameof(StartEruption), _nextEruptionDelay); // Trigger the eruption after the delay
    }

    private void StartEruption()
    {
        StartCoroutine(EruptionSequence());
    }

    private System.Collections.IEnumerator EruptionSequence()
    {
        // Activate steam column and cloud
        _steamColumn.SetActive(true);
        _steamCloud.SetActive(true);

        // Wait for 15 seconds
        yield return new WaitForSeconds(15f);

        // Deactivate steam column and cloud
        _steamColumn.SetActive(false);
        _steamCloud.SetActive(false);

        // Schedule the next eruption
        ScheduleNextEruption();
    }
}
