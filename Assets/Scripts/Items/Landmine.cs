using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Landmine : Bomb
    {
        private const string AnimSetup = "setup";
        private const string AnimArmed = "armed";


        [SerializeField] private float armedTime = 5.0f;
        [SerializeField] private float triggerDistance;
        [SerializeField] private GameObject redBeam;
        [SerializeField] private GameObject greenBeam;
        [SerializeField] private LayerMask playerMask;

        private Animator animator;

        public string OwnerPlayerId; // to check if a player is the one installed the landmine

        private readonly List<Hitbox> targets = new();

        protected override void OnAwake()
        {
            base.OnAwake();
            animator = GetComponent<Animator>();
        }

        protected override IEnumerator WaitForEngage()
        {
            yield return new WaitForSeconds(.2f);

            animator.SetBool(AnimSetup, true);
           
            while (PlayerInRange())
                yield return new WaitForSeconds(.3f);

            animator.SetBool(AnimSetup, false);
            animator.SetBool(AnimArmed, true);

            var elapsedTime = 0.0f;
            while (elapsedTime <= armedTime && targets.Count == 0)
            {
                TryGetAttackTarget();
                
                yield return new WaitForSeconds(.1f);
                elapsedTime += .1f;
            }

            animator.SetBool(AnimArmed, false);
            SoundEvents.BombExplode();

            foreach (var target in targets)
            {
                var distance = Vector3.Distance(transform.position, target.transform.position);
                target.DealDamage(distance < 0.1 ?
                    maxDamage :
                    Mathf.FloorToInt(maxDamage * (attackDistance - distance) / attackDistance));
            }

            yield return new WaitForSeconds(2.0f);

            Destroy(this.gameObject);
        }

        private bool PlayerInRange()
        {
            Array.Clear(hitColliders, 0, maxColliders - 1);

            var playerCnt = Physics.OverlapSphereNonAlloc(transform.position, triggerDistance, hitColliders, playerMask);
            return playerCnt > 0 && hitColliders
                .Where(x => x != null)
                .Select(x => x.GetComponent<Hitbox>())
                .Where(x => x.PlayerId == OwnerPlayerId)
                .Count() > 0;
        }

        private void TryGetAttackTarget()
        {
            var triggerCnt = Physics.OverlapSphereNonAlloc(transform.position, triggerDistance, hitColliders, targetMask);
            if (triggerCnt == 0)
                return;

            var cnt = Physics.OverlapSphereNonAlloc(transform.position, attackDistance, hitColliders, targetMask);

            for (int i = 0; i < cnt; i++)
            {
                var c = hitColliders[i];

                if (c.transform.position.y != Mathf.Clamp(c.transform.position.y, transform.position.y - .2f, transform.position.y + .2f))
                    continue;

                if (c.TryGetComponent<Hitbox>(out var hitbox) && hitbox.CurrentHP <= 0)
                    continue;

                targets.Add(hitbox);
            }

            Array.Clear(hitColliders, 0, maxColliders - 1);
        }
    }
}