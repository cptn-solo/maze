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

        private const string AnimMinigunBool = "minigun";

        [SerializeField] private Transform launcher;
        [SerializeField] private Shell shell;
        [SerializeField] private Minigun minigun;

        public event Action<CollectableType, int> OnCollected;
        public event Action<WeaponType> OnWeaponSelected;

        private bool inAttackState;
        private AimTarget aim;
        private Collector collector;
        private Hitbox hitbox;
        private bool attackRunning;
        private bool weaponSelectRunning;
        private bool minigunEquipped;
        private bool item1SelectRunning;
        private bool item2SelectRunning;

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
                case CollectableType.HP:
                    {
                        hitbox.AddHP(cnt);
                        break;
                    }
                default:
                    {
                        OnCollected?.Invoke(collectableType, cnt);
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

        private void OnWeaponSelect()
        {
            if (!weaponSelectRunning)
                StartCoroutine(ToggleMinigun());
        }

        private void OnItem1Select()
        {
            if (!item1SelectRunning)
                StartCoroutine(UseItem1());
        }

        private void OnItem2Select()
        {
            if (!item2SelectRunning)
                StartCoroutine(UseItem2());
        }
        
        private IEnumerator UseItem1()
        {
            item1SelectRunning = true;
            yield return new WaitForSeconds(.3f);
            item1SelectRunning = true;
        }
        
        private IEnumerator UseItem2()
        {
            item2SelectRunning = true;
            yield return new WaitForSeconds(.3f);
            item2SelectRunning = true;
        }

        private IEnumerator ToggleMinigun()
        {
            weaponSelectRunning = true;

            minigunEquipped = !minigunEquipped;
            
            OnWeaponSelected?.Invoke(minigunEquipped ? WeaponType.Minigun : WeaponType.Shuriken);
            
            minigun.gameObject.SetActive(minigunEquipped);
            animator.SetBool(AnimMinigunBool, minigunEquipped);

            yield return new WaitForSeconds(.3f);

            weaponSelectRunning = false;
        }

        private void OnAttack(bool toggle)
        {
            Debug.Log($"OnAttack {toggle}");
            inAttackState = toggle;
            if (inAttackState && !attackRunning)
                StartCoroutine(AttackCoroutine());                
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
            attackRunning = true;
            aim.Engage(true);

            while (inAttackState && !fadingOut)
            {
                
                aim.TryGetAttackTarget(true);

                if (minigunEquipped)
                {
                    minigun.Attack(true);
                    SoundEvents.MinigunShot();
                }
                else
                {
                    shell.transform.SetParent(null, true);
                    shell.TargetDir = aim.AttackTarget != null ?
                        (aim.AttackTarget.transform.position - transform.position).normalized :
                        transform.forward;
                    shell.gameObject.SetActive(true);

                    SoundEvents.PlayerAttack();

                    yield return new WaitForSeconds(.3f);

                    shell.gameObject.SetActive(false);
                    shell.transform.SetParent(launcher, false);
                    shell.transform.localPosition = Vector3.zero;
                    shell.transform.localRotation = Quaternion.identity;
                }


                yield return new WaitForSeconds(.1f);
            }

            aim.Engage(false);
            attackRunning = false;
            if (minigunEquipped)
                minigun.Attack(false);

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