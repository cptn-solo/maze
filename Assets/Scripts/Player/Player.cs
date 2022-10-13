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
        private const string AnimShotgunBool = "shotgun";
        private const string AnimUziBool = "uzi";

        [SerializeField] private Transform launcher;
        [SerializeField] private Shell shell;
        [SerializeField] private Minigun minigun;
        [SerializeField] private Shotgun shotgun;
        [SerializeField] private Uzi uzi;

        [SerializeField] private LayerMask translateDirMask;
        private readonly Collider[] translateDirBuff = new Collider[1];

        public event Action<CollectableType, int> OnCollected;
        public event Action<WeaponType, WeaponType> OnWeaponSelected;
        public event Action<WeaponType, int> OnActiveWeaponAttack;

        public PlayerBalanceService Balances { get; set; }
        public PlayerPreferencesService Prefs { get; set; }

        private bool inAttackState;
        private TouchInputProcessor touches;
        private PlayerCamera playerCamera;
        private AimTarget aim;
        private Collector collector;
        private Hitbox hitbox;

        private Vector2 keyboardMoveDir = Vector2.zero;
        private Vector2 touchMoveDir = Vector2.zero;
        
        protected override void ProcessMove()
        {
            base.ProcessMove();
            var moveVector = touches.LeftDelta;
            if (moveVector == default ||
                float.IsInfinity(moveVector.x) ||
                float.IsNaN(moveVector.x) ||
                moveVector.sqrMagnitude <= 400.0f)
            {
                touchMoveDir = Vector2.zero;
            }

            touchMoveDir = moveVector.normalized;
            OnMove((keyboardMoveDir.sqrMagnitude > 0) ? 
                keyboardMoveDir : 
                touchMoveDir
                );
        }

        public override void ToggleTranslateDir()
        {
            base.ToggleTranslateDir();
            TranslateDirActive = Physics.OverlapSphereNonAlloc(
                transform.position + transform.up * .04f, .01f, translateDirBuff, translateDirMask) == 1;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            touches = GetComponent<TouchInputProcessor>();
            playerCamera = GetComponent<PlayerCamera>();

            aim = GetComponent<AimTarget>();
            hitbox = GetComponentInChildren<Hitbox>();
            collector = GetComponentInChildren<Collector>();
            collector.OnCollected += Collector_OnCollected;

            hitbox.PlayerId = gameObject.ToString(); // to be replaced with network id
        }
        protected override void OnStart()
        {
            base.OnStart();

            ToggleCameraControl();

            Prefs.OnCameraControlChanged += Prefs_OnCameraControlChanged;
            Prefs.OnCameraSencitivityChanged += Prefs_OnCameraSencitivityChanged;
            OnTranslateDirActiveChange += Player_OnTranslateDirActiveChange;
        }

        private void ToggleCameraControl()
        {
            playerCamera.CameraControl = Prefs.CameraControl && !TranslateDirActive;
            if (playerCamera.CameraControl)
                playerCamera.CameraSencitivity = Prefs.CameraSencitivity;
        }

        private void Player_OnTranslateDirActiveChange(bool obj) =>
            ToggleCameraControl();

        private void Prefs_OnCameraControlChanged(bool obj) =>
            ToggleCameraControl();

        private void Prefs_OnCameraSencitivityChanged(float obj) =>
            playerCamera.CameraSencitivity = obj;

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
                        if (collectableType >= CollectableType.Coin)
                            Balances.AddBalance(collectableType, cnt);

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
            ToggleInput(false);

            OnMove(Vector2.zero);

            inAttackState = false;

            animator.SetBool(AnimDieBool, true);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimJumpBool, false);

            base.OnGotKilled();
        }

        protected override void OnResurrected()
        {
            base.OnResurrected();
                       
            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimJumpBool, false);
        }
        private void OnMove(Vector2 inputDir)
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