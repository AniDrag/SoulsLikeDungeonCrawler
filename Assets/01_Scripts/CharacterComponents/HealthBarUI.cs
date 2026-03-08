using UnityEngine;
using UnityEngine.UI;

namespace AniDrag.CharacterComponents
{
    /// <summary>
    /// Simple UI health bar that follows a HealthComponent.
    /// Attach to a GameObject that has a Slider (or assign one in the inspector).
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HealthComponent healthComponent;   
        [SerializeField] private Slider healthSlider;               

        private void Start()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
                if (healthComponent == null)
                {
                    Debug.LogError("HealthBarUI: No HealthComponent assigned or found!", this);
                    return;
                }
            }

            if (healthSlider == null)
            {
                Debug.LogError("HealthBarUI: No Slider assigned!", this);
                return;
            }

            healthSlider.minValue = 0;
            healthSlider.maxValue = healthComponent.maxHealth;
            healthSlider.value = healthComponent.currentHealth;

            healthComponent.onHealthChanged += UpdateHealthBar;
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
                healthComponent.onHealthChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(HealthComponent comp)
        {
            healthSlider.maxValue = comp.maxHealth;
            healthSlider.value = comp.currentHealth;
        }
    }
}