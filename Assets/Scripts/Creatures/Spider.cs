using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Spider : Enemy
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private int beamDamage = 10;
        [SerializeField] private float beamRange = 1.0f;

        [SerializeField] private Transform beam;

        protected override IEnumerator RangeAttack(Transform target)
        {
            OnRangeAttack();

            StartCoroutine(ShootAnimation());
            
            yield return new WaitForSeconds(.08f);

            SoundEvents.SpiderBeamAttack();

            while (true)
            {
                if (!Physics.Raycast(beam.position, beam.forward, out var hitInfo, beamRange, targetMask))
                    break;

                if (!hitInfo.collider.TryGetComponent<Hitbox>(out var hitbox))
                    break;

                if (hitbox.CurrentHP <= 0)
                    break;

                hitbox.DealDamage(beamDamage);

                break;
            }

            yield return null;


        }

    }
}