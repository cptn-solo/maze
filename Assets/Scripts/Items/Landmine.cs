using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Landmine : Bomb
    {
        [SerializeField] private float armedTime = 10.0f;
        [SerializeField] private float triggerDistance;
        [SerializeField] private GameObject beam;


        private readonly List<Hitbox> targets = new();

        protected override IEnumerator WaitForEngage()
        {
            yield return new WaitForSeconds(1.0f);

            var elapsedTime = 0.0f;
            while (elapsedTime <= armedTime && targets.Count == 0)
            {
                TryGetAttackTarget();

                elapsedTime += Time.deltaTime;
                
                beam.SetActive(true);

                yield return null;
                
                beam.SetActive(false);
            }

            beam.SetActive(false);
            yield return new WaitForSeconds(.1f);
            modelVisual.SetActive(false);
            flash.gameObject.SetActive(true);
            SoundEvents.BombExplode();

            foreach (var target in targets)
            {
                var distance = Vector3.Distance(transform.position, target.transform.position);
                target.DealDamage(distance < 0.1 ?
                    maxDamage :
                    Mathf.FloorToInt(maxDamage * (attackDistance - distance) / attackDistance));
            }

            yield return new WaitForSeconds(.2f);
            flash.gameObject.SetActive(false);

            yield return new WaitForSeconds(2.0f);

            Destroy(this.gameObject);
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