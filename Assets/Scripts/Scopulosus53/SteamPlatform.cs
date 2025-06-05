using System.Collections;
using UnityEngine;

public class SteamPlatform : MonoBehaviour
{

    [SerializeField] private GameObject _steamColumn;
    [SerializeField] private GameObject _steamCloud;
    [SerializeField] private float _steamDelay = 0f;

    private void Start()
    {
        _steamColumn.SetActive(false);
        _steamCloud.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (_steamDelay != 0)
            {
                StartCoroutine(SteamStartTime());
            }
            else
            {
                _steamColumn.SetActive(true);
                _steamCloud.SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(SteamCooldown());
        }
    }

    private IEnumerator SteamStartTime()
    {
        yield return new WaitForSeconds(_steamDelay);
        _steamColumn.SetActive(true);
        _steamCloud.SetActive(true);
    }
    private IEnumerator SteamCooldown()
    {
        yield return new WaitForSeconds(10f);
        _steamColumn.SetActive(false);
        _steamCloud.SetActive(false);
    }
}
