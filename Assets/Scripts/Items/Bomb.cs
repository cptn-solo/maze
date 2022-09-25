using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bomb : MonoBehaviour
    {
        private Rigidbody rb;

        [SerializeField] private ParticleSystem smoke;
        [SerializeField] protected ParticleSystem flash;
        [SerializeField] protected GameObject modelVisual;
        [SerializeField] protected float attackDistance = .3f;
        [SerializeField] protected LayerMask targetMask;        
        [SerializeField] protected int maxDamage = 50;
        protected const int maxColliders = 10;
        protected readonly Collider[] hitColliders = new Collider[maxColliders];

        public IngameSoundEvents SoundEvents { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            modelVisual.SetActive(true);
            flash.gameObject.SetActive(false);
            rb.AddForce(transform.forward * .2f + transform.up * .2f, ForceMode.VelocityChange);
            StartCoroutine(WaitForEngage());
        }

        protected virtual IEnumerator WaitForEngage()
        {
            yield return new WaitForSeconds(1.0f);
            smoke.gameObject.SetActive(false);
            yield return new WaitForSeconds(.1f);
            modelVisual.SetActive(false);
            flash.gameObject.SetActive(true);
            SoundEvents.BombExplode();

            TryGetAttackTarget();
            
            yield return new WaitForSeconds(.2f);
            flash.gameObject.SetActive(false);

            yield return new WaitForSeconds(2.0f);

            Destroy(this.gameObject);
        }

        private void TryGetAttackTarget()
        {

            var cnt = Physics.OverlapSphereNonAlloc(transform.position, attackDistance, hitColliders, targetMask);

            for (int i = 0; i < cnt; i++)
            {
                var c = hitColliders[i];

                if (c.transform.position.y != Mathf.Clamp(c.transform.position.y, transform.position.y - .2f, transform.position.y + .2f))
                    continue;

                if (c.TryGetComponent<Hitbox>(out var hitbox) && hitbox.CurrentHP <= 0)
                    continue;

                // check for walls removed to improve gameplay and QoL for the player ;)
                var distance = Vector3.Distance(transform.position, c.transform.position);
                hitbox.DealDamage(distance < 0.1 ?
                    maxDamage : 
                    Mathf.FloorToInt(maxDamage * (attackDistance - distance) / attackDistance));
            }

            Array.Clear(hitColliders, 0, maxColliders - 1);
        }

    }
}