using AniDrag.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AniDrag.CharacterComponents
{
    public class EntityUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _staminaSlider;
        [SerializeField] private Slider _manaSlider;
        [SerializeField] private Text _levelText;

        private IHealth _health;
        private IStamina _stamina;
        private IMana _mana;
        private IXp _xp;

        private void Awake()
        {
            // Find components on the same GameObject (or its parents/children as needed)
            _health = GetComponentInParent<IHealth>();
            _stamina = GetComponentInParent<IStamina>();
            _mana = GetComponentInParent<IMana>();
            _xp = GetComponentInParent<IXp>();

            // Disable UI elements for missing components
            if (_healthSlider != null && _health == null)
                _healthSlider.gameObject.SetActive(false);
            if (_staminaSlider != null && _stamina == null)
                _staminaSlider.gameObject.SetActive(false);
            if (_manaSlider != null && _mana == null)
                _manaSlider.gameObject.SetActive(false);
            if (_levelText != null && _xp == null)
                _levelText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (_health != null)
                _health.OnHealthChanged += UpdateHealth;
            if (_stamina != null)
                _stamina.OnStaminaChanged += UpdateStamina;
            if (_mana != null)
                _mana.OnManaChanged += UpdateMana;
            if (_xp != null)
            {
                _xp.OnXpChanged += UpdateXp;
                _xp.OnLevelUp += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
                _health.OnHealthChanged -= UpdateHealth;
            if (_stamina != null)
                _stamina.OnStaminaChanged -= UpdateStamina;
            if (_mana != null)
                _mana.OnManaChanged -= UpdateMana;
            if (_xp != null)
            {
                _xp.OnXpChanged -= UpdateXp;
                _xp.OnLevelUp -= UpdateLevel;
            }
        }

        private void UpdateHealth(IHealth health)
        {
            if (_healthSlider != null)
            {
                _healthSlider.maxValue = health.MaxHealth;
                _healthSlider.value = health.CurrentHealth;
            }
        }

        private void UpdateStamina(IStamina stamina)
        {
            if (_staminaSlider != null)
            {
                _staminaSlider.maxValue = stamina.MaxStamina;
                _staminaSlider.value = stamina.CurrentStamina;
            }
        }

        private void UpdateMana(IMana mana)
        {
            if (_manaSlider != null)
            {
                _manaSlider.maxValue = mana.MaxMana;
                _manaSlider.value = mana.CurrentMana;
            }
        }

        private void UpdateXp(IXp xp)
        {
            // Optional: update XP bar
        }

        private void UpdateLevel(int newLevel)
        {
            if (_levelText != null)
                _levelText.text = newLevel.ToString();
        }
    }
}