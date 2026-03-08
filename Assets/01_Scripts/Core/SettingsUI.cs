    using AniDrag.Core;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class SettingsUI : MonoBehaviour
    {
        [Header("AudioMixers")]
        [SerializeField] PlayerInput inputs;
        [SerializeField] private Transform settingsUI;
        [Header("AudioMixers")]
        [SerializeField] AudioMixerGroup masterMixer;
        [SerializeField] AudioMixerGroup musicMixer;
        [SerializeField] AudioMixerGroup uiMixer;
        [SerializeField] Slider masterMixerSlider;
        [SerializeField] Slider musicMixerSlider;
        [SerializeField] Slider uiMixerSlider;


        [Header("Buttons")]
        [SerializeField] Button MainMenu;
        [SerializeField] Button Restart;

        [Header("Camera Settings")]
        [SerializeField] Slider verticalSens;
        [SerializeField] Slider horizontalSens;
        [SerializeField] Slider fovSlider;
        [SerializeField] CameraSettings cameraSettings;

        void Start()
        {
            //Buttons
            if(MainMenu != null)
                MainMenu.onClick?.AddListener(OnMainMenuClicked);
            if(Restart != null)
                Restart.onClick?.AddListener(OnRestartClicked);

            //Camera Settings
            verticalSens.value = cameraSettings.SensitivityVertical;
            horizontalSens.value = cameraSettings.SensitivityHorizontal;
            fovSlider.value = cameraSettings.FOV;
            verticalSens.onValueChanged.AddListener(OnVerticalSensChanged);
            horizontalSens.onValueChanged.AddListener(OnHorizontalSensChanged);
            fovSlider.onValueChanged.AddListener(OnFovChanged);

            //Sound
            masterMixerSlider.onValueChanged?.AddListener(SetMasterVolume);
            musicMixerSlider.onValueChanged?.AddListener(SetMusicVolume);
            uiMixerSlider.onValueChanged?.AddListener(SetUIVolume);
        }
        private void Update()
        {
            if (inputs.actions["Menu"].WasPressedThisFrame())
            {
                if (settingsUI.gameObject.activeSelf)
                {
                    settingsUI.gameObject?.SetActive(false);
                    ClosedSettings();
                }
                else
                {
                    settingsUI.gameObject?.SetActive(true);
                    OpenedSettings();
                }
            }
        }
        void OnMainMenuClicked()
        {
            Scene_Manager.Instance.SCENE_LoadScene(0);
        }
        void OnRestartClicked()
        {
            Scene_Manager.Instance.SCENE_ReloadScene();
        }
        void OnVerticalSensChanged(float value)
        {
            cameraSettings.SensitivityVertical = value;
        }
        void OnHorizontalSensChanged(float value)
        {
            cameraSettings.SensitivityHorizontal = value;
        }
        void OnFovChanged(float value)
        {
            cameraSettings.FOV = value;
        }
         void SetMasterVolume(float volume)
        {
            masterMixer.audioMixer?.SetFloat("MasterVolume", ConvertToDecibel(volume));
        }
         void SetMusicVolume(float volume)
        {
            musicMixer.audioMixer?.SetFloat("MusicVolume", ConvertToDecibel(volume));
        }
         void SetUIVolume(float volume)
        {
            uiMixer.audioMixer?.SetFloat("UIVolume", ConvertToDecibel(volume));
        }
        public void OpenedSettings()
        {
            cameraSettings.EnableMenuPanel();
        }
        public void ClosedSettings()
        {
            cameraSettings.DisableMenuPanel();
        }

        private float ConvertToDecibel(float volume)
        {
            if (volume <= 0)
                return -80f;
            float linear = Mathf.Clamp(volume, 0.0001f, 100f) / 100f;
            return Mathf.Log10(linear) * 20f;
        }
    }
