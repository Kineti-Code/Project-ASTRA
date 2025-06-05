using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("Required Components")]
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private Slider _chargeSlider;

    [Header("Flashlight Settings")]
    [SerializeField] private float _dischargeRate = 0.3f;
    [SerializeField] private float _dischargeRateMultiplier = 2f;
    [SerializeField] private float _BrightnessReductionPoint = 0.7f;
    [SerializeField, Min(0)] private float _minIntensity = 0.7f;

    private float _OGflashlightIntensity;
    private bool _dischargeRateReduced = false;
    private Light2D _flashlightBeam;
    private bool _flashlightSetting = false;
    private Player _player;
    private float slope;
    private float yIntercept;
    private AudioManager audioManager;

    void Start()
    {
        _flashlight.SetActive(_flashlightSetting);
        _player = GetComponentInParent<Player>();
        _flashlightBeam = _flashlight.GetComponent<Light2D>();
        _OGflashlightIntensity = _flashlightBeam.intensity;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        CalculateLinearDecrease();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }

    void Update()
    {
        ToggleFlashlight();
        UpdateFlashlightDirection();
    }

    private IEnumerator ReduceCharge()
    {
        while (_flashlightSetting)
        {
            yield return new WaitForSeconds(_dischargeRate);
            _chargeSlider.value += 0.01f;

            if (_chargeSlider.value >= 0.7f)
            {
                _flashlightBeam.intensity = (slope * _chargeSlider.value) + yIntercept;
            }
        }
    }

    private void CalculateLinearDecrease()
    {
        slope = (_minIntensity - _OGflashlightIntensity) / (1 - _BrightnessReductionPoint);
        yIntercept = _minIntensity + Mathf.Abs(slope);
    }

    public void RechargeFlashlight()
    {
        _chargeSlider.value = 0;
        _flashlightBeam.intensity = _OGflashlightIntensity;
    }

    public void PunctureBattery()
    {
        if (!_dischargeRateReduced)
        {
            _dischargeRate /= _dischargeRateMultiplier;
            _dischargeRateReduced = true;
        }
    }

    private void ToggleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            audioManager.PlaySFX(audioManager.flashlightToggle);
            _flashlightSetting = !_flashlightSetting;
            _flashlight.SetActive(_flashlightSetting);

            if (_flashlightSetting)
            {
                StartCoroutine(ReduceCharge());
            }
        }
    }

    private void UpdateFlashlightDirection()
    {
        if (_player != null && _flashlight.activeSelf) // Update direction only if flashlight is on
        {
            float targetRotation = _player._isFacing == 1 ? -90f : -270f;
            if (_flashlight.transform.localRotation.eulerAngles.z != targetRotation)
            {
                _flashlight.transform.localRotation = Quaternion.Euler(0, 0, targetRotation);
            }
        }
    }
}