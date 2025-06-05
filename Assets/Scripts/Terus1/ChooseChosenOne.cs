using UnityEngine;

public class ChooseChosenOne : MonoBehaviour
{

    private Player[] players;
    [SerializeField] private GameObject _flashlightPrefab;
    [SerializeField] private UniversalControlHandler universalControlHandler;

    void Start()
    {
        // get all players
        // choose random number from 1-4
        // give that player flashlight

        players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        int chosenNum = Random.Range(0, players.Length);
        Player chosenOne = players[chosenNum];
        GameObject flashlightInstance = Instantiate(_flashlightPrefab, chosenOne.transform.position, Quaternion.identity);
        flashlightInstance.transform.parent = chosenOne.transform;
        chosenOne.flashlight = flashlightInstance.GetComponent<Flashlight>();
        universalControlHandler.flashlight = chosenOne.flashlight;
    }
}
