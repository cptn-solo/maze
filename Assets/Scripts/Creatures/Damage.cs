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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(damageTo) && 
                other.gameObject.TryGetComponent<Hitbox>(out var hitbox) &&
                hitbox.CurrentHP > 0)
            {
                hitbox.DealDamage(damagePerHit);

                if (damageInterval > 0)
                {
                    this.hitbox = hitbox;
                    this.hitbox.OnDestroyedOrDisabled += Hitbox_OnDestroyedOrDisabled;
                    this.hitbox.OnZeroHealthReached += Hitbox_OnZeroHealthReached;
                    StartCoroutine(RepeatDamage());
                }
            }
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
            while (hitbox != null)
            {
                yield return new WaitForSeconds(damageInterval);
                if (hitbox != null && hitbox.CurrentHP > 0)
                    hitbox.DealDamage(damagePerHit);                
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