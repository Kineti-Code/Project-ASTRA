using UnityEngine;

public class OpenSettings : MonoBehaviour
{
    [SerializeField] private GameObject _tabletMain;
    [SerializeField] private GameObject _settingsUI;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _tabletMain.SetActive(true);
        _settingsUI.SetActive(true);
    }
}
