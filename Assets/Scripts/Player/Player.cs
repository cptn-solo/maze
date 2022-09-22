using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Player : MovableUnit
    {
        private const string AnimJumpBool = "jump";
        private const string AnimGoBool = "go";
        private const string AnimDieBool = "die";
        private const string AnimAttackBool = "attack";
        private const string AnimDamageBool = "damage";
        private const string AnimSpeedFloat = "speed";

        [SerializeField] private Transform launcher;
        [SerializeField] private Shell shell;
        
        private bool inAttackState;
        private AimTarget aim;
        private Collector collector;
        private Hitbox hitbox;

        protected override void OnAwake()
        {
            base.OnAwake();
            BindInputs();

            aim = GetComponent<AimTarget>();
            hitbox = GetComponentInChildren<Hitbox>();
            collector = GetComponentInChildren<Collector>();
            collector.OnCollected += Collector_OnCollected;
        }

        private void Collector_OnCollected(CollectableType collectableType, int cnt)
        {
            SoundEvents.CollectedItem();

            switch (collectableType)
            {
                case CollectableType.Shield:
                    {
                        hitbox.AddShield(cnt);
                        break;
                    }
                default:
                    {
                        hitbox.AddHP(cnt);
                        break;
                    }
            }

        }

        protected override void OnTakingDamage(bool critical)
        {
            SoundEvents.PlayerDamaged();

            base.OnTakingDamage(critical);
            StartCoroutine(DamageAnimation());
        }

        private IEnumerator DamageAnimation()
        {
            animator.SetBool(AnimDamageBool, true);

            yield return new WaitForSeconds(.3f);

            animator.SetBool(AnimDamageBool, false);

        }

        protected override void OnGotKilled()
        {
            base.OnGotKilled();
            
            ToggleInput(false);

            inAttackState = false;

            animator.SetBool(AnimDieBool, true);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimJumpBool, false);
        }

        protected override void OnResurrected()
        {
            base.OnResurrected();

            ToggleInput(true);

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimJumpBool, false);
        }
        private void OnMove(Vector3 inputDir)
        {
            moveDir = inputDir;
            var go = moveDir.sqrMagnitude > 0;
            if (animator != null)
            {
                animator.SetBool(AnimGoBool, go);
                animator.SetFloat(AnimSpeedFloat, go ? 1 + speed : 1);
            }
        }

        private void OnJump()
        {
            if (rb.velocity.y < .01f && !fadingOut)
                StartCoroutine(JumpCoroutine());
        }

        private void OnAttack(bool toggle)
        {
            if (toggle && !inAttackState)
            {
                aim.Engage(true);
                StartCoroutine(AttackCoroutine());
            }

            inAttackState = toggle;

            if (!inAttackState)
                aim.Engage(false);

        }


        protected override Vector3 CurrentRotationDir(Vector3 translatedDir)
        {
            if (!inAttackState || aim.AttackTarget == null)
                return base.CurrentRotationDir(translatedDir);

            return aim.AttackTarget.position - transform.position;
        }
        protected override float CurrentMoveSpeed(float speed)
        {
            var baseSpeed = base.CurrentMoveSpeed(speed);
            if (aim.AttackTarget != null)
                return baseSpeed * .8f;

            return baseSpeed;
        }

        protected override float CurrentRotationSpeed(float rotationSpeed)
        {
            var baseRotationSpeed = base.CurrentRotationSpeed(rotationSpeed);
            if (aim.AttackTarget != null)
                return baseRotationSpeed * 3.0f;

            return baseRotationSpeed;
        }



        private IEnumerator AttackCoroutine()
        {
            inAttackState = true;

            while (inAttackState && !fadingOut)
            {
                
                aim.TryGetAttackTarget(true);

                shell.transform.SetParent(null, true);
                shell.gameObject.SetActive(true);

                SoundEvents.PlayerAttack();

                yield return new WaitForSeconds(.3f);

                shell.gameObject.SetActive(false);
                shell.transform.SetParent(launcher, false);
                shell.transform.localPosition = Vector3.zero;
                shell.transform.localRotation = Quaternion.identity;
                
                yield return new WaitForSeconds(.1f);
            }
        }

        private IEnumerator JumpCoroutine()
        {
            if (animator != null)
            {
                animator.SetBool(AnimJumpBool, true);
                SoundEvents.PlayerJump();
                yield return new WaitForSeconds(.01f);
                animator.SetBool(AnimJumpBool, false);
            }

            yield return new WaitForSeconds(.1f);
            
            if (!fadingOut)
                rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }
    }
}