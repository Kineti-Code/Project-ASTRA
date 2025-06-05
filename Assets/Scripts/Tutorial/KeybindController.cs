using UnityEngine;
using UnityEngine.UI;

public class KeybindController : MonoBehaviour
{

    public bool slideEnabled = false;

    [Header("Control Objects")]
    [SerializeField] private GameObject leftKeyObject;
    [SerializeField] private GameObject rightKeyObject;
    [SerializeField] private GameObject upKeyObject;
    [SerializeField] private GameObject downKeyObject;

    private Image leftKeyRenderer;
    private Image rightKeyRenderer;
    private Image upKeyRenderer;
    private Image downKeyRenderer;

    [Header("Images")]
    [SerializeField] private Sprite[] leftKeyImages;
    [SerializeField] private Sprite[] rightKeyImages;
    [SerializeField] private Sprite[] upKeyImages;
    [SerializeField] private Sprite[] downKeyImages;

    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode upKey;
    private KeyCode downKey;

    private Player player;

    void Start()
    {
        player = GetComponentInParent<Player>();

        leftKey = player._leftControl;
        rightKey = player._rightControl;
        upKey = player._upControl;
        downKey = player._downControl;

        leftKeyRenderer = leftKeyObject.GetComponent<Image>();
        rightKeyRenderer = rightKeyObject.GetComponent<Image>();
        upKeyRenderer = upKeyObject.GetComponent<Image>();
        downKeyRenderer = downKeyObject.GetComponent<Image>();

        leftKeyRenderer.sprite = leftKeyImages[0];
        rightKeyRenderer.sprite = rightKeyImages[0];
        upKeyRenderer.sprite = upKeyImages[0];
        downKeyRenderer.sprite = downKeyImages[0];
        downKeyRenderer.color = Color.gray;
    }

    public void EnableSlide()
    {
        slideEnabled = true;
        downKeyRenderer.color = Color.white;
    }

    void Update()
    {
        if (Input.GetKey(leftKey))
        {
            leftKeyRenderer.sprite = leftKeyImages[1];
        }

        else
        {
            leftKeyRenderer.sprite = leftKeyImages[0];
        }



        if (Input.GetKey(rightKey))
        {
            rightKeyRenderer.sprite = rightKeyImages[1];
        }

        else
        {
            rightKeyRenderer.sprite = rightKeyImages[0];
        }



        if (Input.GetKey(upKey))
        {
            upKeyRenderer.sprite = upKeyImages[1];
        }

        else
        {
            upKeyRenderer.sprite = upKeyImages[0];
        }



        if (slideEnabled)
        {
            if (Input.GetKey(downKey))
            {
                downKeyRenderer.sprite = downKeyImages[1];
            }

            else
            {
                downKeyRenderer.sprite = downKeyImages[0];
            }
        }
    }
}
