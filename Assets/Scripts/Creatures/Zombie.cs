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
        private Hitbox hitbox;
        private AimTarget aim;
        private ObstacleAvoidance pilot;

        private bool scouting;

        protected override void OnAwake()
        {
            base.OnAwake();

            damage = GetComponentInChildren<Damage>();
            hitbox = GetComponentInChildren<Hitbox>();
            aim = GetComponent<AimTarget>();
            pilot = GetComponent<ObstacleAvoidance>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            for (var i = 0; i < ren.materials.Length; i++)// in new[] {0, 4, 8 , 10})
            {
                var mat = ren.materials[i];
                Color.RGBToHSV(mat.color, out var h, out var s, out var v);
                h = Random.Range(.01f, .99f);
                mat.color =  Color.HSVToRGB(h, 1, v);
            }
        }
        private void Damage_OnDealingDamage(Hitbox obj)
        {
            SoundEvents.ZombieAttack();

            StartCoroutine(AttackAnimation());
        }

        private void Pilot_OnDeadlock(object sender, System.EventArgs e)
        {
            moveDir.x *= -1;

            StartCoroutine(StopScouting(Random.Range(3.0f, 5.0f)));
        }

        private IEnumerator StopScouting(float interval)
        {
            scouting = false;
         
            StopCoroutine(LookForTarget());
            aim.Engage(false);

            yield return new WaitForSeconds(interval);

            scouting = true;

            StartCoroutine(LookForTarget());
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
                return (pilot.SuggestedDir(aim.AttackTarget.position - transform.position).normalized);

            return base.CurrentMoveDir(pilot.SuggestedDir(translatedDir));
        }
        protected override Vector3 CurrentRotationDir(Vector3 translatedDir)
        {
            if (!scouting || aim.AttackTarget == null)
                return base.CurrentRotationDir(pilot.SuggestedDir(translatedDir));

            return aim.AttackTarget.position - transform.position;
        }

        protected override float CurrentMoveSpeed(float speed)
        {
            var baseSpeed = base.CurrentMoveSpeed(speed);
            if (aim.AttackTarget != null)
                return baseSpeed * 3.0f / SizeScale;

            return baseSpeed;
        }
        protected override float CurrentRotationSpeed(float rotationSpeed)
        {
            var baseRotationSpeed = base.CurrentRotationSpeed(rotationSpeed);
            if (aim.AttackTarget != null)
                return baseRotationSpeed * 3.0f / SizeScale;

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
            damage.SizeScale = SizeScale;
            hitbox.SizeScale = SizeScale;
            
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

            if (pilot)
                pilot.OnDeadlock -= Pilot_OnDeadlock;

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnObjEnable()
        {
            base.OnObjEnable();

            if (damage)
                damage.OnDealingDamage += Damage_OnDealingDamage;

            if (pilot)
                pilot.OnDeadlock += Pilot_OnDeadlock;
            
            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, true);
            animator.SetBool(AnimAttackBool, false);
        }

    }
}