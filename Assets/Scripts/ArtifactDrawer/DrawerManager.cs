using UnityEngine;
using UnityEngine.SceneManagement;

public class DrawerManager : MonoBehaviour
{

    [SerializeField] private GameObject[] minerals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        foreach (GameObject mineral in minerals)
        {
            mineral.SetActive(false);
        }

        for (int i = 1; i <= minerals.Length; i++)
        {
            if (PlayerPrefs.GetInt($"Level{i}") == 1)
            {
                minerals[i - 1].SetActive(true);
            }
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        SceneManager.LoadScene(1);
    //    }
    //}
}
