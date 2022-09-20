using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

namespace Assets.Scripts
{
    public class Zombie : MovableUnit
    {
        private const string AnimGoBool = "go";
        private const string AnimDieBool = "die";
        private const string AnimAttackBool = "attack";
        private const string AnimDamageBool = "damage";
        
        private Damage damage;
        private AimTarget aim;
        private bool scouting;

        protected override void OnAwake()
        {
            base.OnAwake();

            damage = GetComponentInChildren<Damage>();
            aim = GetComponent<AimTarget>();
        }

        private void Damage_OnDealingDamage(Hitbox obj)
        {
            SoundEvents.ZombieAttack();

            StartCoroutine(AttackAnimation());
        }

        protected override void OnTakingDamage(bool critical)
        {
            SoundEvents.ZombieDamaged();
            
            base.OnTakingDamage(critical);

            StartCoroutine(DamageAnimation());
        }

        private IEnumerator DamageAnimation()
        {
            animator.SetBool(AnimDamageBool, true);

            yield return new WaitForSeconds(.3f);

            animator.SetBool(AnimDamageBool, false);

        }
        private IEnumerator AttackAnimation()
        {
            animator.SetBool(AnimAttackBool, true);

            yield return new WaitForSeconds(.5f);

            animator.SetBool(AnimAttackBool, false);

        }


        private IEnumerator LookForTarget()
        {
            while (scouting)
            {                
                yield return new WaitForSeconds(1.0f);

                if (scouting && (aim.AttackTarget == null || aim.AttackTargetLost))
                    aim.Engage(true);
            }
        }

        protected override Vector3 CurrentMoveDir(Vector3 translatedDir)
        {
            if (aim.AttackTarget)
                return (aim.AttackTarget.position - transform.position).normalized;

            return base.CurrentMoveDir(translatedDir);
        }
        protected override Vector3 CurrentRotationDir(Vector3 translatedDir)
        {
            if (!scouting|| aim.AttackTarget == null)
                return base.CurrentRotationDir(translatedDir);

            return aim.AttackTarget.position - transform.position;
        }

        protected override float CurrentMoveSpeed(float speed)
        {
            var baseSpeed = base.CurrentMoveSpeed(speed);
            if (aim.AttackTarget != null)
                return baseSpeed * 3.0f;

            return baseSpeed;
        }
        protected override float CurrentRotationSpeed(float rotationSpeed)
        {
            var baseRotationSpeed = base.CurrentRotationSpeed(rotationSpeed);
            if (aim.AttackTarget != null)
                return baseRotationSpeed * 3.0f;

            return baseRotationSpeed;
        }

        protected override void OnGotKilled()
        {
            base.OnGotKilled();

            damage.Active = false;
            
            scouting = false;
            StopCoroutine(LookForTarget());

            animator.SetBool(AnimDieBool, true);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnResurrected()
        {
            base.OnResurrected();
            
            var dirRandom = Random.Range(0, 2);
            moveDir = dirRandom == 0 ? Vector3.right : Vector3.left;

            damage.Active = true;
            
            scouting = true;            
            StartCoroutine(LookForTarget());

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, true);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnObjDisable()
        {
            base.OnObjDisable();

            if (damage)
                damage.OnDealingDamage += Damage_OnDealingDamage;

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnObjEnable()
        {
            base.OnObjEnable();

            if (damage)
                damage.OnDealingDamage += Damage_OnDealingDamage;
            
            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, true);
            animator.SetBool(AnimAttackBool, false);
        }
    }
}