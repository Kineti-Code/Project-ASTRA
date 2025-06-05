using UnityEngine;
using UnityEngine.UI;

public class SplitscreenManager : MonoBehaviour
{
    [SerializeField] private GameObject borderRenderer;
    private RawImage borderImage;
    [SerializeField] private Texture[] borderLayouts;
    private Camera[] playerCameras;
    private int _numOfPlayers;

    public void AdjustSplitscreenConfig()
    {
        _numOfPlayers = PlayerManager.Instance.NumOfPlayers;
        borderImage = borderRenderer.GetComponent<RawImage>();
        AdjustCameraView();
        AdjustCameraBorders();
    }

    private Camera[] GetCameras()
    {
        playerCameras = null;

        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");
        playerCameras = new Camera[activePlayers.Length];

        for (int i = 0; i < activePlayers.Length; i++)
        {
            playerCameras[i] = activePlayers[i].GetComponentInChildren<Camera>();

            if (playerCameras[i] == null)
            {
                Debug.LogError($"Player {i} has no Camera component!");
            }
        }

        if (activePlayers.Length != _numOfPlayers)
        {
            Debug.LogWarning("length of active player list not equal to number of players in scene");
            Debug.Log("Length of active players list: " + activePlayers.Length);
            Debug.Log("Numofplayers integer: " + _numOfPlayers);
        }

        return playerCameras;
    }

    private void AdjustCameraView()
    {
        Camera[] activePlayerCameras = GetCameras();

        // Take active player cameras; adjust camera viewport rect x, y, w, & h values based on screen (change zoom if needed)

        switch (_numOfPlayers)
        {
            case 0:
                Debug.LogWarning("How are there 0 players wtf (adjustcameraview func.)");
                break;

            case 1:

                activePlayerCameras[0].rect = new Rect(0, 0, 1, 1);
                activePlayerCameras[0].transform.localPosition = new Vector3(activePlayerCameras[0].transform.localPosition.x, activePlayerCameras[0].transform.localPosition.y, -17);
                break;

            case 2:

                activePlayerCameras[0].rect = new Rect(0, 0, 0.5f, 1);
                activePlayerCameras[1].rect = new Rect(0.5f, 0, 0.5f, 1);

                for (int i = 0; i < activePlayerCameras.Length; i++)
                {
                    activePlayerCameras[i].transform.localPosition = new Vector3(activePlayerCameras[0].transform.localPosition.x, activePlayerCameras[0].transform.localPosition.y, -17);
                }

                break;

            case 3:

                activePlayerCameras[0].rect = new Rect(0, 0.67f, 1, 0.33f);
                activePlayerCameras[1].rect = new Rect(0, 0.34f, 1, 0.33f);
                activePlayerCameras[2].rect = new Rect(0, 0, 1, 0.34f);

                for (int i = 0; i < activePlayerCameras.Length; i++)
                {
                    activePlayerCameras[i].transform.localPosition = new Vector3(activePlayerCameras[0].transform.localPosition.x, activePlayerCameras[0].transform.localPosition.y, -5);
                }

                break;

            case 4:

                activePlayerCameras[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                activePlayerCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                activePlayerCameras[2].rect = new Rect(0, 0, 0.5f, 0.5f);
                activePlayerCameras[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);

                for (int i = 0; i < activePlayerCameras.Length; i++)
                {
                    activePlayerCameras[i].transform.localPosition = new Vector3(activePlayerCameras[0].transform.localPosition.x, activePlayerCameras[0].transform.localPosition.y, -10);
                }

                break;

            default:
                Debug.LogWarning("invalid num of players while adjusting cameras for splitscreen");
                Debug.Log("Num of player argument: " + _numOfPlayers);
                break;

        }

    }

    private void AdjustCameraBorders()
    {

        switch (_numOfPlayers)
        {
            case 0:
                Debug.LogWarning("How are there 0 players wtf (adjustcameraborders func.");
                break;

            case 1:

                if (borderRenderer.activeSelf)
                {
                    borderRenderer.SetActive(false);
                }
                break;

            case 2:

                if (!borderRenderer.activeSelf)
                {
                    borderRenderer.SetActive(true);
                }

                borderImage.texture = borderLayouts[0];
                break;

            case 3:

                if (!borderRenderer.activeSelf)
                {
                    borderRenderer.SetActive(true);
                }

                borderImage.texture = borderLayouts[1];
                break;

            case 4:

                if (!borderRenderer.activeSelf)
                {
                    borderRenderer.SetActive(true);
                }

                borderImage.texture = borderLayouts[2];
                break;

            default:
                Debug.LogWarning("invalid num of players while adjusting viewport borders for splitscreen");
                Debug.Log("Num of player argument: " + _numOfPlayers);
                break;
        }
    }
}
