using AniDrag.CharacterComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AniDrag.Core;
public class CharacterComponentUI : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;
    HealthComponent hp;
    StaminaComponent sp;
    XpComponent xp;
    //ManaComponent mp;
    public void Initialize()
    {
        Debug.Log("Initializing CharacterComponentUI...");
        if (owner == null)
            owner = GameManager.Instance.Players[0];
        hp = owner.GetComponent<HealthComponent>();
        //mp = owner.GetComponent<ManaComponent>();
        sp = owner.GetComponent<StaminaComponent>();
        xp = owner.GetComponent<XpComponent>();

        hp.onHealthChange.AddListener(OnHealthChanged);
        sp.updateStamina += OnStaminaChanged;
        xp.updateXp +=OnXpChanged;
        xp.onLevelUp.AddListener(OnLevelUp);

        OnLevelUp();
    }

    float timeNow = 0;
    float timeInterval = 0.1f;
    // Update is called once per frame
    void Update()
    {
        if (timeNow >= Time.time)
        {
            timeNow = Time.time + timeInterval;
            if (owner != null)
            {
                OnHealthChanged(hp.currentHealth);
                OnStaminaChanged(sp);
                OnXpChanged(xp);
            }
        }
    }
    void OnHealthChanged(int health)
    {
        healthSlider.value = health;
    }
    void OnStaminaChanged(StaminaComponent stamina)
    {
        staminaSlider.value = stamina.currentStamina;
    }
   void OnXpChanged(XpComponent xp)
    {
        xpSlider.value = xp.currentXP;
    }
     void OnLevelUp(int value = 0)
    {
        levelText.text = xp.GetLevel().ToString();
        healthSlider.maxValue = hp.maxHealth;
        staminaSlider.maxValue = sp.maxStamina;
        xpSlider.maxValue = xp.maxXP;
    }
}
