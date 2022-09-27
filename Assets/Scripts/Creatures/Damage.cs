using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Damage : MonoBehaviour
    {
        [SerializeField] private LayerMask damageTo;
        [SerializeField] private int damagePerHit;
        [SerializeField] private float damageInterval; // 0 for single hit

        private Hitbox hitbox;

        public LayerMask DamageTo => damageTo;

        public int PerkAddedDamage { get; set; } = 0;

        public bool Active { get; set; }
        public float SizeScale { get; set; } = 1.0f;

        public event Action<Hitbox> OnDealingDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (Active &&
                other.CheckColliderMask(damageTo) && 
                other.gameObject.TryGetComponent<Hitbox>(out var hitbox) &&
                hitbox.CurrentHP > 0)
            {
                DealDamage(hitbox);

                if (damageInterval > 0)
                {
                    this.hitbox = hitbox;
                    this.hitbox.OnDestroyedOrDisabled += Hitbox_OnDestroyedOrDisabled;
                    this.hitbox.OnZeroHealthReached += Hitbox_OnZeroHealthReached;
                    StartCoroutine(RepeatDamage());
                }
            }
        }

        private void DealDamage(Hitbox hitbox)
        {
            OnDealingDamage?.Invoke(hitbox);
            hitbox.DealDamage(Mathf.FloorToInt(damagePerHit * SizeScale) + PerkAddedDamage);
        }

        private void Hitbox_OnZeroHealthReached()
        {
            if (this.hitbox != null)
                ReleaseHitbox();
        }
        private void Hitbox_OnDestroyedOrDisabled()
        {
            if (this.hitbox != null)
                ReleaseHitbox();
        }

        private void ReleaseHitbox()
        {
            this.hitbox.OnDestroyedOrDisabled -= Hitbox_OnDestroyedOrDisabled;
            this.hitbox.OnZeroHealthReached -= Hitbox_OnZeroHealthReached;
            this.hitbox = null;
        }


        private IEnumerator RepeatDamage()
        {
            while (Active && hitbox != null)
            {
                yield return new WaitForSeconds(damageInterval);
                if (Active && hitbox != null && hitbox.CurrentHP > 0)
                    DealDamage(hitbox);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CheckColliderMask(damageTo) &&
                other.gameObject.TryGetComponent<Hitbox>(out var hitbox) &&
                this.hitbox == hitbox)
                ReleaseHitbox();
        }

    }
}