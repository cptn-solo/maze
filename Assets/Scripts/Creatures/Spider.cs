using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Spider : Enemy
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private int beamDamage = 2;
        [SerializeField] private float beamRange = 1.0f;

        [SerializeField] private Transform beam;
        [SerializeField] private Transform aimBeam;

        protected override IEnumerator RangeAttack(Transform target)
        {
            rangeAttackRunning = true;

            var damage = Mathf.FloorToInt(beamDamage * SizeScale);

            OnRangeAttack();

            yield return new WaitForSeconds(.08f);

            StartCoroutine(ShootAnimation());
            var attackWindow = 1.0f;
            while (rangeAttackRunning && attackWindow > 0f)
            {
                SoundEvents.SpiderBeamAttack();

                if (beam.gameObject.activeSelf && 
                    Physics.Raycast(beam.position, beam.forward, out var hitInfo, beamRange, targetMask) && 
                    hitInfo.collider.TryGetComponent<Hitbox>(out var hitbox) &&
                    hitbox.CurrentHP > 0)
                    hitbox.DealDamage(damage);

                yield return new WaitForSeconds(.1f);
                attackWindow -= .1f;
            }
            
            rangeAttackRunning = false;
        }

    }
}