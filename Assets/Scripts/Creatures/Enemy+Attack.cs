using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Enemy
    {
        [SerializeField] private bool rangeAttackEnabled;
        [SerializeField] private float rangeAttackInterval = 3.0f;

        protected bool onTargetRunning;
        protected bool rangeAttackRunning;

        private IEnumerator AttackAnimation()
        {
            animator.SetBool(AnimAttackBool, true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(AnimAttackBool, false);

        }

        protected IEnumerator ShootAnimation()
        {
            animator.SetBool(AnimShootBool, true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(AnimShootBool, false);

        }

        private IEnumerator LookForTarget()
        {
            while (scouting)
            {
                yield return new WaitForSeconds(1.0f);

                if (scouting && (aim.AttackTarget == null || aim.AttackTargetLost))
                {
                    aim.Engage(true);
                    if (rangeAttackEnabled && aim.AttackTarget != null && !onTargetRunning)
                        StartCoroutine(OnTarget());
                }
            }
        }

        private IEnumerator OnTarget()
        {
            onTargetRunning = true;
            while (onTargetRunning && aim.AttackTarget != null)
            {
                // delay before 1st attack
                yield return new WaitForSeconds(rangeAttackInterval);
                
                // double check
                if (onTargetRunning && !rangeAttackRunning && aim.AttackTarget != null)
                    StartCoroutine(RangeAttack(aim.AttackTarget));
            }

            onTargetRunning = false;
        }

        protected virtual IEnumerator RangeAttack(Transform target) {
            rangeAttackRunning = true;
            
            yield return null;
            
            rangeAttackRunning = false;
        }
    }
}