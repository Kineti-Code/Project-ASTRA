using System.Collections;
using UnityEngine;

public class MainMenuParticleController : MonoBehaviour
{

    [SerializeField] private ParticleSystem cometShower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cometShower != null)
        {
            StartCoroutine(spawnCometShower());
        }

        else
        {
            Debug.LogWarning("Comet Shower system not defined");
        }
    }

    private IEnumerator spawnCometShower()
    {
        while (true)
        {
            int timeToNextShower = Random.Range(15, 45);
            yield return new WaitForSeconds(timeToNextShower);
            cometShower.Play();
        }
    }
}
