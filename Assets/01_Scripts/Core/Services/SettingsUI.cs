    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.UI;
namespace AniDrag.Core
{

   public class SettingsUI : MonoBehaviour
    {
        [Header("Audio Settings")]
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
        [SerializeField] Toggle invertVertical;
        private GameObject panel;
        private int retries = 0;
        private void OnEnable()
        {
            if (Services.Input != null)
            {
                Debug.LogWarning("SettingsUI: Input service ready.");
                Services.Input.OnMenuToggle += ToggleSettingsPanel;
            }
            else
            {
                Debug.LogWarning("SettingsUI: Input service not ready, will retry.");
                Invoke(nameof(RetrySubscribe), 0.1f);
            }
            panel = this.transform.GetChild(0).gameObject;
            SubscribeTo();
        }

        private void OnDisable()
        {
            if (Services.Input != null)
                Services.Input.OnMenuToggle -= ToggleSettingsPanel;

            UnsubscribeFrom();
            CancelInvoke(); // stop any pending retries
        }

        private void RetrySubscribe()
        {
            retries++;
            if (Services.Input != null)
            {
                Debug.LogWarning("SettingsUI: Input service found after retry: " + retries);
                Services.Input.OnMenuToggle += ToggleSettingsPanel;
            }
            else
                Invoke(nameof(RetrySubscribe), 0.1f);
        }

        private void ToggleSettingsPanel()
        {
            bool active = !panel.activeSelf;
            panel.SetActive(active);

            if (active)
            {
                Services.GameState.FreezeTime();
                Services.GameState.UnlockCursor();
                RefreshUIValues();
            }
            else
            {
                Services.GameState.UnfreezeTime();
                Services.GameState.LockCursor();
            }
        }

        private void RefreshUIValues()
        {
            verticalSens.value = Services.Settings.SensitivityVertical;
            horizontalSens.value = Services.Settings.SensitivityHorizontal;
            fovSlider.value = Services.Settings.FOV;
            invertVertical.isOn = Services.Settings.InvertVertical;
            masterMixerSlider.value = Services.Settings.MasterVolume;
            musicMixerSlider.value = Services.Settings.MusicVolume;
            uiMixerSlider.value = Services.Settings.UIVolume;
        }

        // Button callbacks
        void OnMainMenuClicked() => Services.Scene.LoadScene(0);
        void OnRestartClicked() => Services.Scene.ReloadCurrentScene();

        // Slider callbacks
        void OnVerticalSensChanged(float value) => Services.Settings.SensitivityVertical = value;
        void OnHorizontalSensChanged(float value) => Services.Settings.SensitivityHorizontal = value;
        void OnFovChanged(float value) => Services.Settings.FOV = value;
        void SetMasterVolume(float volume) => Services.Settings.MasterVolume = volume;
        void SetMusicVolume(float volume) => Services.Settings.MusicVolume = volume;
        void SetUIVolume(float volume) => Services.Settings.UIVolume = volume;
        void OnInvertVerticalChanged(bool value) => Services.Settings.InvertVertical = value;

        void SubscribeTo()
        {
            if (MainMenu != null) MainMenu.onClick.AddListener(OnMainMenuClicked);
            if (Restart != null) Restart.onClick.AddListener(OnRestartClicked);

            verticalSens.onValueChanged.AddListener(OnVerticalSensChanged);
            horizontalSens.onValueChanged.AddListener(OnHorizontalSensChanged);
            fovSlider.onValueChanged.AddListener(OnFovChanged);
            invertVertical.onValueChanged.AddListener(OnInvertVerticalChanged);

            masterMixerSlider.onValueChanged.AddListener(SetMasterVolume);
            musicMixerSlider.onValueChanged.AddListener(SetMusicVolume);
            uiMixerSlider.onValueChanged.AddListener(SetUIVolume);
        }

        void UnsubscribeFrom()
        {
            if (MainMenu != null) MainMenu.onClick.RemoveListener(OnMainMenuClicked);
            if (Restart != null) Restart.onClick.RemoveListener(OnRestartClicked);

            verticalSens.onValueChanged.RemoveListener(OnVerticalSensChanged);
            horizontalSens.onValueChanged.RemoveListener(OnHorizontalSensChanged);
            fovSlider.onValueChanged.RemoveListener(OnFovChanged);
            invertVertical.onValueChanged.RemoveListener(OnInvertVerticalChanged);

            masterMixerSlider.onValueChanged.RemoveListener(SetMasterVolume);
            musicMixerSlider.onValueChanged.RemoveListener(SetMusicVolume);
            uiMixerSlider.onValueChanged.RemoveListener(SetUIVolume);
        }
    }
}