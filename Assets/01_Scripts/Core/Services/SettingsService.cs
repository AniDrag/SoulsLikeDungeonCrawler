using UnityEngine;
using UnityEngine.Audio;

namespace AniDrag.Core
{
     public class SettingsService : MonoBehaviour, ISettingsService
    {
        [Header("References")]
        [SerializeField] private CameraSettings _cameraSettings;
        [SerializeField] private AudioMixerGroup _masterMixer;
        [SerializeField] private AudioMixerGroup _musicMixer;
        [SerializeField] private AudioMixerGroup _uiMixer;

        // Backing fields
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _uiVolume = 1f;
        private float _sensitivityVertical;
        private float _sensitivityHorizontal;
        private float _fov;
        private bool _invertVertical;

        public event System.Action OnSettingsChanged;

        private void Awake()
        {
            // Load saved values (example using PlayerPrefs)
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
            _sensitivityVertical = PlayerPrefs.GetFloat("SensVertical", _cameraSettings.SensitivityVertical);
            _sensitivityHorizontal = PlayerPrefs.GetFloat("SensHorizontal", _cameraSettings.SensitivityHorizontal);
            _fov = PlayerPrefs.GetFloat("FOV", _cameraSettings.FOV);
            _invertVertical = PlayerPrefs.GetInt("InvertVertical", 0) == 1;

            // Apply initial values
            ApplyMasterVolume();
            ApplyMusicVolume();
            ApplyUIVolume();
            ApplyCameraSettings();
        }

        public float MasterVolume
        {
            get => _masterVolume;
            set { _masterVolume = value; ApplyMasterVolume(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set { _musicVolume = value; ApplyMusicVolume(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public float UIVolume
        {
            get => _uiVolume;
            set { _uiVolume = value; ApplyUIVolume(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public float SensitivityVertical
        {
            get => _sensitivityVertical;
            set { _sensitivityVertical = value; ApplyCameraSettings(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public float SensitivityHorizontal
        {
            get => _sensitivityHorizontal;
            set { _sensitivityHorizontal = value; ApplyCameraSettings(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public float FOV
        {
            get => _fov;
            set { _fov = value; ApplyCameraSettings(); Save(); OnSettingsChanged?.Invoke(); }
        }

        public bool InvertVertical
        {
            get => _invertVertical;
            set { _invertVertical = value; ApplyCameraSettings(); Save(); OnSettingsChanged?.Invoke(); }
        }

        private void ApplyMasterVolume() => SetMixerVolume(_masterMixer, "Volume", _masterVolume);
        private void ApplyMusicVolume() => SetMixerVolume(_musicMixer, "Volume", _musicVolume);
        private void ApplyUIVolume() => SetMixerVolume(_uiMixer, "Volume", _uiVolume);

        private void SetMixerVolume(AudioMixerGroup mixer, string parameter, float volume)
        {
            if (mixer != null && mixer.audioMixer.GetFloat(parameter, out float current))
                mixer.audioMixer.SetFloat(parameter, ConvertToDecibel(volume));
            else
                Debug.LogWarning($"Parameter '{parameter}' not found in AudioMixer");
        }

        private void ApplyCameraSettings()
        {
            if (_cameraSettings != null)
            {
                _cameraSettings.SensitivityVertical = _sensitivityVertical;
                _cameraSettings.SensitivityHorizontal = _sensitivityHorizontal;
                _cameraSettings.FOV = _fov;
                _cameraSettings.InvertVertical = _invertVertical;
            }
        }

        private float ConvertToDecibel(float volume)
        {
            if (volume <= 0) return -80f;
            float linear = Mathf.Clamp(volume, 0.0001f, 1f); // assuming volume is 0-1
            return Mathf.Log10(linear) * 20f;
        }

        private void Save()
        {
            PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            PlayerPrefs.SetFloat("UIVolume", _uiVolume);
            PlayerPrefs.SetFloat("SensVertical", _sensitivityVertical);
            PlayerPrefs.SetFloat("SensHorizontal", _sensitivityHorizontal);
            PlayerPrefs.SetFloat("FOV", _fov);
            PlayerPrefs.SetInt("InvertVertical", _invertVertical ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}