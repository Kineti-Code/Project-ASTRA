using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 _normalScale = new(1, 2, 1);
    private Vector3 _slideScale = new(2, 1, 2);
    private Vector2 _spawnPoint;
    private float _gravityScale = 9;
    private float _jump = 20f;
    public float speed = 11f;
    private float _climbSpeed = 10f;
    private float _maxSlideTime = 0.5f;
    private float _slideMultiplier = 1.5f;
    [SerializeField] private bool _isGrounded = true;
    [SerializeField] private string groundType;
    private bool _canSlide = true;
    private bool _onLadder = false;
    private bool _onHook = false;
    private bool _isSliding = false;
    [SerializeField] private bool _isColliding = false;
    private bool _reduceFriction = false;
    private bool _slippyDippyCooldown = false;
    private int horizontalInput;
    public int _isFacing = 1;
    private float wKeyPressTime = -1f;
    private float sKeyPressTime = -1f;
    private float gracePeriod = 0.2f;
    private bool highSpeedMode = false;
    private BreakablePlatform[] breakablePlatforms;

    [Header("Artifacts Management")]
    public int artifactsCollected = 0;

    [Header("Physics materials")]
    [SerializeField] private PhysicsMaterial2D _normalMat;
    [SerializeField] private PhysicsMaterial2D _noFrictionmat;

    [Header("Player components")]
    [SerializeField] private GameObject _visualComponent;
    [SerializeField] private BoxCollider2D _hitboxComponent;
    [SerializeField] private Camera playercam;

    [Header("High Speed Mode Configuration")]
    [SerializeField] private float CamZoomoutFactor = 30f;

    [Header("Post Processing")]
    [SerializeField] private Volume vignetteVolume;

    [Header("Controls")]
    public KeyCode _upControl;
    public KeyCode _downControl;
    public KeyCode _leftControl;
    public KeyCode _rightControl;

    [Header("Advanced Controls")]
    public bool controlsEnabled = true;
    public bool autoRespawn = true;
    [HideInInspector] public bool inTutorialMode = false;
    private bool highspeedEnabled = false;

    [Header("Terus1 only")]
    public Flashlight flashlight; // assigned in choosechosenone script

    [Header("Magnus25 only")]
    [SerializeField] private GameObject dismountHookInfo;
    private Hook activeHook;

    [Header("Nivalis36 only")]
    [SerializeField] private ColdthManager coldthManager;
    [SerializeField] private PhysicsMaterial2D _slippyDippyMat;

    private GameObject _startPlatform;
    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        _startPlatform = GameObject.FindWithTag("Start");

        if (_startPlatform == null)
        {
            Debug.LogWarning("start platform is null");
            _spawnPoint = transform.position;
        }
        else
        {
            _spawnPoint = new Vector2(_startPlatform.transform.position.x, _startPlatform.transform.position.y + _startPlatform.transform.localScale.y / 2);
            transform.position = _spawnPoint;
        }

        Collider2D groundCheck = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Ground"));
        if (groundCheck != null)
        {
            _isGrounded = true;
        }

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            if (gameObject.GetComponentInChildren<Flashlight>() != null)
            {
                flashlight = gameObject.GetComponentInChildren<Flashlight>();
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            breakablePlatforms = FindObjectsByType<BreakablePlatform>(FindObjectsSortMode.None);
        }

        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            inTutorialMode = true;
        }

        if (dismountHookInfo != null)
        {
            dismountHookInfo.SetActive(false);
        }

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (audioManager == null)
        {
            Debug.LogWarning("AudioManager is null");
        }

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            if (vignetteVolume.profile.TryGet(out Vignette vignette))
            {
                vignette.intensity.value = 0;
                vignette.active = false;
                highspeedEnabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (_onLadder)
        {
            climbLadder();
        }

        if ((_onHook) && Input.GetKeyDown(_upControl)) 
        {
            activeHook.DetachPlayer();
        }

        if (controlsEnabled)
        {
            handleKeypresses();
        }

        if ((transform.position.y < -100 || transform.position.y > 240) && autoRespawn)
        {
            Respawn();
        }

        if ((rb.linearVelocity.x >= 40 || rb.linearVelocity.y >= 40) && !highSpeedMode && highspeedEnabled)
        {
            StartCoroutine(HighSpeed());
        }
    }

    private IEnumerator HighSpeed()
    {
        highSpeedMode = true;
        StartCoroutine(AdjustFOV(true));
        float OriginalZPosition = playercam.transform.position.z;

        while (rb.linearVelocity.x >= 40 || rb.linearVelocity.y >= 40)
        {
            float updatedZPosition = OriginalZPosition - CamZoomoutFactor;
            playercam.transform.position = new Vector3(transform.position.x, transform.position.y, updatedZPosition);
            yield return null;
        }

        StopCoroutine(AdjustFOV(true));
        StartCoroutine(AdjustFOV(false));

        yield return new WaitForSeconds(1f);
        playercam.transform.position = new Vector3(transform.position.x, transform.position.y, OriginalZPosition);
        highSpeedMode = false;
    }

    private IEnumerator AdjustFOV(bool direction)
    {
        if (vignetteVolume.profile.TryGet(out Vignette vignette))
        {
            vignette.active = true;
        }

        float targetValue = direction ? 0.5f : 0f;
        while (!Mathf.Approximately(vignette.intensity.value, targetValue))
        {
            vignette.intensity.value = Mathf.MoveTowards(vignette.intensity.value, targetValue, Time.deltaTime);
            yield return null;
        }

        if (!direction)
        {
            vignette.active = false;
        }
    }

    private void handleKeypresses()
    {
        if (Input.GetKey(_leftControl)) { horizontalInput = -1; }
        else if (Input.GetKey(_rightControl)) { horizontalInput = 1; }
        else { horizontalInput = 0; }

        // Ensure footsteps only play when actually moving
        if (horizontalInput != 0 && Mathf.Abs(rb.linearVelocity.x) > 0.1f && _isGrounded)
        {
            audioManager.PlayMovementSound(groundType, AudioManager.MovementType.Walking);
        }
        else
        {
            // If player is stopping OR in the air, fully fade out footsteps
            audioManager.CrossfadeFootstepSound(groundType, AudioManager.MovementType.Walking, !_isGrounded || Mathf.Abs(rb.linearVelocity.x) < 0.1f);
        }

        if (Input.GetKeyDown(_rightControl))
        {
            _isFacing = 1;
        }

        else if (Input.GetKeyDown(_leftControl))
        {
            _isFacing = -1;
        }

        if (Input.GetKey(_upControl) && _isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _jump);
        }

        if (Input.GetKeyDown(_downControl) && !inTutorialMode) 
        {
            sKeyPressTime = Time.time;

            if (_canSlide && _isGrounded)
            {
                StartCoroutine(PlayerSlide());
            }
        }

        if (Input.GetKeyDown(_upControl))
        {
            wKeyPressTime = Time.time;
        }

        if (!inTutorialMode && _isGrounded && _canSlide && wKeyPressTime > 0 && sKeyPressTime > 0 && (Mathf.Abs(wKeyPressTime - sKeyPressTime) <= gracePeriod))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _jump);
            StartCoroutine(PlayerSlide());

            wKeyPressTime = -1f;
            sKeyPressTime = -1f;
        }

        if (_reduceFriction && horizontalInput != 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);   
        }

        else if (!_reduceFriction)
        {
            rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
        }
      
    }

    IEnumerator PlayerSlide()
    {
        _isSliding = true;
        _visualComponent.transform.localScale = _slideScale;
        _hitboxComponent.size = new Vector2(2, 1);
        float slideStartTime = Time.time;

        while (_isSliding)
        {
            rb.linearVelocity = new Vector2(_isFacing * speed * _slideMultiplier, rb.linearVelocity.y);

            if (Input.GetKeyDown(_leftControl) || Input.GetKeyDown(_rightControl) || _canSlide == false)
            {
                StopSlide();
            }
            if (Time.time - slideStartTime >= _maxSlideTime && _isColliding)
            {
                StopSlide();
            }

            yield return null;
        }
    }

    IEnumerator WaitForGroundContact()
    {
        while (!_isGrounded)
        {
            yield return null;
        }

        _reduceFriction = true;
    }

    private void StopSlide()
    {
        _isSliding = false;
        _visualComponent.transform.localScale = _normalScale; // Always reset to normal scale
        _hitboxComponent.size = new Vector2(1, 2);
    }

    public void updateCheckpoint(Vector2 updated_spawnpoint)
    {
        _spawnPoint = updated_spawnpoint;
    }

    public void collectArtifact()
    {
        if (audioManager != null)
        {
            int randomNumber = Random.Range(0, audioManager.collectArtifact.Length);
            audioManager.PlaySFX(audioManager.collectArtifact[randomNumber]);
        }

        artifactsCollected++;
    }

    public void Respawn()
    {
        transform.position = _spawnPoint;
        rb.linearVelocity = Vector2.zero;
        wKeyPressTime = -1f;
        sKeyPressTime = -1f;
        controlsEnabled = true;

        if (SceneManager.GetActiveScene().buildIndex == 7 && coldthManager != null)
        {
            coldthManager.InitializeColdSystem();

            if (PlayerManager.Instance.NumOfPlayers == 1)
            {
                foreach (BreakablePlatform breakablePlatform in breakablePlatforms)
                {
                    breakablePlatform.EnablePlatform();
                    breakablePlatform.StopAllCoroutines();
                }
            }
        }
    }

    public void climbLadder()
    {
        if (Input.GetKey(_upControl))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _climbSpeed);
        }

        else if (Input.GetKey(_downControl))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -_climbSpeed);
        }

        else
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    private void SpeedIncrease()
    {
        if (Input.GetKeyDown(_downControl) && _isGrounded && speed <= 18)
        {
            speed += 5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _onLadder = true;
            rb.gravityScale = 0;
        }

        if (collision.CompareTag("NoSlide"))
        {
            _canSlide = false;
        }

        if (collision.CompareTag("Hook"))
        {
            activeHook = collision.gameObject.GetComponent<Hook>();
            _onHook = true;
            _reduceFriction = true;
            dismountHookInfo.SetActive(true);
        }

        if (collision.CompareTag("FlashlightLeaker") && flashlight != null)
        {
            flashlight.PunctureBattery();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("UltraSlippyDippy"))
        {
            SpeedIncrease();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _onLadder = false;
            rb.gravityScale = _gravityScale;
        }

        if (collision.CompareTag("Hook"))
        {
            _onHook = false;
            activeHook = null;
            dismountHookInfo.SetActive(false);
            StartCoroutine(WaitForGroundContact());
        }

        if (collision.CompareTag("UltraSlippyDippy"))
        {
            if (speed > 11)
            {
                speed = 11;
            }
        }
    }

    private IEnumerator SlippyDippyCooldown()
    {
        _slippyDippyCooldown = true;
        yield return new WaitForSeconds(0.75f);

        if (!_isGrounded || !_isColliding)
        {
            _hitboxComponent.sharedMaterial = _normalMat;
        }

        _slippyDippyCooldown = false;
        _reduceFriction = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("NoJump"))
        {
            bool groundContact = false;

            // Check each contact point.
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Only consider the player grounded if the contact normal is almost perfectly vertical
                // (i.e. the surface is essentially horizontal) and has little horizontal component.
                if (contact.normal.y > 0.95f && Mathf.Abs(contact.normal.x) < 0.2f)
                {
                    groundContact = true;
                    break; // We found a proper ground contact.
                }
            }

            _isGrounded = groundContact;

            // If not grounded, ensure the player doesn't "stick" by using the no friction material.
            if (!_isGrounded)
            {
                _hitboxComponent.sharedMaterial = _noFrictionmat;
            }
            else
            {
                // For platforms that aren't marked as "SlippyDippy", restore normal friction.
                if (!collision.collider.CompareTag("SlippyDippy"))
                {
                    if (_hitboxComponent.sharedMaterial != _normalMat)
                    {
                        StartCoroutine(SlippyDippyCooldown());
                    }
                }
                // For SlippyDippy platforms, apply the slippy material and reduce friction.
                else if (_hitboxComponent.sharedMaterial != _slippyDippyMat && collision.collider.CompareTag("SlippyDippy"))
                {
                    _hitboxComponent.sharedMaterial = _slippyDippyMat;
                    _reduceFriction = true;
                }
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Respawn"))
        {
            Respawn();
        }

        if (collision.collider.CompareTag("FlashlightCharger") && flashlight != null)
        {
            flashlight.RechargeFlashlight();
        }

        if (!collision.collider.CompareTag("SlippyDippy"))
        {
            StopCoroutine(SlippyDippyCooldown());
            _slippyDippyCooldown = false;
            _reduceFriction = false;
        }

        groundType = collision.collider.tag;
        if (groundType == "Untagged" || groundType == "Start") { groundType = "Normal"; }

        _isColliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isGrounded = false;
        _isColliding = false;

        if (collision.collider.CompareTag("SlippyDippy"))
        {
            StartCoroutine(SlippyDippyCooldown());
        }

        if (!_slippyDippyCooldown)
        {
            _reduceFriction = false;
            _hitboxComponent.sharedMaterial = _noFrictionmat;
        }

        _hitboxComponent.sharedMaterial = _noFrictionmat;

        if (!inTutorialMode)
        {
            _canSlide = true;
        }

        else
        {
            _canSlide = false;
        }
    }
}
