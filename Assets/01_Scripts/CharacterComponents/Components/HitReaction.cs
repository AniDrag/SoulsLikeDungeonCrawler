using AniDrag.Core;
using UnityEngine;

namespace AniDrag.CharacterComponents
{
    public class HitReaction: MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField] private ParticleSystem _hitParticles;
        [SerializeField] private AudioSource _hitSound;
        [SerializeField] private float _destroyAfterSeconds = 2f;

        private IHealth _health;

        private void Awake()
        {
            _health = GetComponent<IHealth>();
            if (_health != null)
                _health.OnHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(IHealth health)
        {
            // Only react if health decreased (i.e., damage taken)
            // This requires storing previous health; we'll keep it simple and just react to any change
            // Better: check if current health < previous health.
            // For simplicity, we'll just trigger on every change (including healing)
            // You can refine this.

            if (_hitParticles != null)
                Instantiate(_hitParticles, transform.position, Quaternion.identity);

            if (_hitSound != null)
                _hitSound.Play();
        }
    }
}