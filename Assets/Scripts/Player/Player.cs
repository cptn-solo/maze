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

        protected override void OnAwake()
        {
            base.OnAwake();
            BindInputs();
        }

        protected override void OnTakingDamage(bool critical)
        {
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
                StartCoroutine(AttackCoroutine());

            inAttackState = toggle;
        }

        private IEnumerator AttackCoroutine()
        {
            inAttackState = true;

            while (inAttackState && !fadingOut)
            {
                shell.transform.SetParent(null, true);
                shell.gameObject.SetActive(true);

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
                yield return new WaitForSeconds(.01f);
                animator.SetBool(AnimJumpBool, false);
            }

            yield return new WaitForSeconds(.1f);
            
            if (!fadingOut)
                rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }
    }
}