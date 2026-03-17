using UnityEngine;
using AniDrag.Core;

namespace AniDrag.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _playerCam;
        [SerializeField] private Transform _playerOrientation;
        [SerializeField] private Transform _cameraTrackPosition;
        [SerializeField] private CameraSettings _settings;

        [Header("Sensitivity")]
        [SerializeField] private float _baseHorizontalSensitivity = 1f;
        [SerializeField] private float _baseVerticalSensitivity = 1f;

        [Header("Clamp")]
        [SerializeField] private float _verticalClamp = 85f;

        private float _xRotation;
        private float _yRotation;
        private bool _cameraEnabled = true;
        private int _invertFactor = 1;

        private void Start()
        {
            if (_playerCam == null)
                _playerCam = Camera.main;

            if (_settings != null)
                _playerCam.fieldOfView = _settings.FOV;
        }

        private void OnEnable()
        {
            Services.GameState.OnCameraControlAllowedChanged += SetCameraEnabled;
        }

        private void OnDisable()
        {
            Services.GameState.OnCameraControlAllowedChanged -= SetCameraEnabled;
        }

        private void Update()
        {
            UpdatePosition();

            if (!_cameraEnabled)
            {
                UpdateSettingsWhileDisabled();
                return;
            }

            Vector2 lookInput = Services.Input.LookInput;
            if (lookInput == Vector2.zero) return;

            ApplyRotation(lookInput);
        }

        private void UpdatePosition()
        {
            if (_cameraTrackPosition != null)
                _playerCam.transform.position = _cameraTrackPosition.position;
        }

        private void UpdateSettingsWhileDisabled()
        {
            if (_settings != null && Mathf.Abs(_playerCam.fieldOfView - _settings.FOV) > 0.01f)
                _playerCam.fieldOfView = Mathf.Lerp(_playerCam.fieldOfView, _settings.FOV, Time.deltaTime * 5f);

            _invertFactor = _settings.InvertVertical ? -1 : 1;
        }

        private void ApplyRotation(Vector2 lookInput)
        {
            _yRotation += lookInput.x * _baseHorizontalSensitivity * Time.deltaTime * _settings.SensitivityHorizontal * _invertFactor;
            _xRotation -= lookInput.y * _baseVerticalSensitivity * Time.deltaTime * _settings.SensitivityVertical;

            _xRotation = Mathf.Clamp(_xRotation, -_verticalClamp, _verticalClamp);

            _playerCam.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

            if (_playerOrientation != null)
                _playerOrientation.rotation = Quaternion.Euler(0f, _yRotation, 0f);
        }

        private void SetCameraEnabled(bool enabled) => _cameraEnabled = enabled;
    }
}