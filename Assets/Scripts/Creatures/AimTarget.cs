using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class AimTarget : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float attackDistance = .5f;

        private Transform attackTarget;
        
        const int maxColliders = 10;
        
        readonly Collider[] hitColliders = new Collider[maxColliders];

        public Transform AttackTarget => attackTarget;

        public bool AttackTargetLost =>
            attackTarget == null ||
            transform.position.y != Mathf.Clamp(transform.position.y, attackTarget.position.y - .2f, attackTarget.position.y + .2f) ||
            Vector3.Distance(attackTarget.position, transform.position) > attackDistance;

        public Transform TryGetAttackTarget(bool recheck = false)
        {

            if (!recheck)
                attackTarget = null;

            var cnt = Physics.OverlapSphereNonAlloc(transform.position, attackDistance, hitColliders, targetMask);

            for (int i = 0; i < cnt; i++)
            {
                var c = hitColliders[i];

                if (c.transform.position.y != Mathf.Clamp(c.transform.position.y, transform.position.y - .2f, transform.position.y + .2f))
                    continue;

                if (c.TryGetComponent<Hitbox>(out var hitbox) && hitbox.CurrentHP <= 0)
                    continue;

                // check for walls removed to improve gameplay and QoL for the player ;)

                if (attackTarget == null ||
                    Vector3.SqrMagnitude(c.transform.position - transform.position) <
                    Vector3.SqrMagnitude(attackTarget.position - transform.position))
                    attackTarget = c.transform;
            }

            Array.Clear(hitColliders, 0, maxColliders - 1);
            
            return attackTarget;
        }

        internal void Engage(bool v)
        {
            attackTarget = v ? TryGetAttackTarget() : null;

        }
    }
}