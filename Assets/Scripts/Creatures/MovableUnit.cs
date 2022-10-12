using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class MovableUnit : MonoBehaviour
    {
        protected Rigidbody rb;
        protected Collider col;
        private Battle battle;
        private Camera sceneCamera;

        [SerializeField] protected Renderer ren;
        [SerializeField] protected Animator animator;

        protected Vector3 desiredRotVelocity;
        protected Vector3 desiredVelocity;

        [SerializeField] protected float speed = 1.5f;
        [SerializeField] protected float rotationSpeed = 30.0f;
        [SerializeField] protected float fadeSpeed = 1.0f;

        [SerializeField] protected float jumpImpulse = 2.0f;
        [SerializeField] private float outOfSceneYTreshold = -10.0f;

        [SerializeField] private float movePauseForRangeAttack = 2.0f;
        [SerializeField] private float movePauseForMeleeAttack = .5f;

        protected Building building;

        protected Vector2 moveDir;

        protected Vector3 localRight;
        protected Vector3 localForward;

        private Vector3 translatedDir;
        private Vector3 center = Vector3.zero;

        private bool translateDirActive = true;

        public event Action<MovableUnit> OnBeforeFadeOut;
        public event Action<MovableUnit> OnAfterFadeIn;
        public bool TranslateDirActive { 
            get => translateDirActive;
            set
            {
                var prev = translateDirActive;
                translateDirActive = value;
                if (prev != value)
                    OnTranslateDirActiveChange?.Invoke(value);
            }
        }

        public IngameSoundEvents SoundEvents { get; set; }

        public event Action<MovableUnit> OnUnitBeforeKilled;
        public event Action<MovableUnit> OnUnitRespawned;
        public event Action<MovableUnit> OnUnitKilled;
        public event Action<bool> OnTranslateDirActiveChange;

        private event Action OnAwakeAction;
        private event Action OnStartAction;

        private event Action OnEnabledAction;
        private event Action OnDisabledAction;
        private event Action OnDestroyAction;

        private float sizeScale = 1.0f;
        private bool isCancelMoveOnDealingDamage;
        private bool isCancelMoveOnRangeAttack;

        public float SizeScale
        {
            get => sizeScale;
            internal set
            {
                sizeScale = value;
                transform.localScale *= value;
            }
        }

        public Building Building
        {
            get => building;
            set
            {
                building = value;
            }
        }

        public void OnRespawned(Vector3 position, Quaternion rotation)
        {
            rb.velocity = Vector3.zero;

            rb.ResetInertiaTensor();
            rb.ResetCenterOfMass();

            rb.MovePosition(position);
            rb.MoveRotation(rotation);

            OnResurrected();

            OnUnitRespawned?.Invoke(this);
        }

        protected virtual void OnAwake()
        {
            sceneCamera = Camera.main;

            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            battle = GetComponent<Battle>();

            battle.OnBattleInfoChange += Battle_OnBattleInfoChange;
            battle.OnTakingDamage += Battle_OnTakingDamage;
            
            OnAwakeAction?.Invoke();
        }

        protected virtual void OnGotKilled() =>
            OnUnitBeforeKilled?.Invoke(this);

        protected virtual void OnResurrected() { }
        protected virtual void OnRangeAttack()
        {
            if (!isCancelMoveOnRangeAttack)
                StartCoroutine(CancelMoveOnRangeAttack());
        }

        protected virtual void OnDealingDamage(Hitbox hitbox) {
            if (!isCancelMoveOnDealingDamage)
                StartCoroutine(CancelMoveOnDealingDamage());
        }

        protected virtual void OnTakingDamage(bool critical) { }
        private void Battle_OnTakingDamage(bool critical) =>
            OnTakingDamage(critical);

        private void Battle_OnBattleInfoChange(BattleInfo obj)
        {
            if (obj.CurrentHP <= 0)
            {
                StartCoroutine(Killed());
            }
        }

        private IEnumerator Killed()
        {
            OnGotKilled();

            yield return new WaitForSeconds(3.0f);

            OnUnitKilled?.Invoke(this);

        }

        private IEnumerator CancelMoveOnDealingDamage()
        {
            isCancelMoveOnDealingDamage = true;
            yield return new WaitForSeconds(movePauseForMeleeAttack);
            isCancelMoveOnDealingDamage = false;
        }

        private IEnumerator CancelMoveOnRangeAttack()
        {
            isCancelMoveOnRangeAttack = true;
            yield return new WaitForSeconds(movePauseForRangeAttack);
            isCancelMoveOnRangeAttack = false;
        }

        protected virtual void OnStart() => OnStartAction?.Invoke();
        protected virtual void OnObjEnable() => OnEnabledAction?.Invoke();
        protected virtual void OnObjDisable() => OnDisabledAction?.Invoke();            
        protected virtual void OnObjDestroy() => OnDestroyAction?.Invoke();
        private void Awake() => OnAwake();
        private void Start() => OnStart();
        private void OnEnable() => OnObjEnable();        
        private void OnDisable() => OnObjDisable();
        private void OnDestroy() => OnObjDestroy();

        protected virtual float CurrentMoveSpeed(float speed) => speed;
        protected virtual float CurrentRotationSpeed(float rotationSpeed) => rotationSpeed;
        protected virtual Vector3 CurrentMoveDir(Vector3 translatedDir) => translatedDir;
        protected virtual Vector3 CurrentRotationDir(Vector3 translatedDir) => translatedDir;
        private Vector3 Grounded(Vector3 position = default)
        {
            if (position == default)
                position = rb.position;

            return position - Vector3.up * position.y;
        }

        private void ReadLocalAxis()
        {
            if (TranslateDirActive)
            {
                localForward = (center - Grounded()).normalized;

                if (Mathf.Abs(localForward.x) > Mathf.Abs(localForward.z) * 1.2f)
                    localForward = Mathf.Sign(localForward.x) * Vector3.right;
                else if (Mathf.Abs(localForward.z) > Mathf.Abs(localForward.x) * 1.2f)
                    localForward = Mathf.Sign(localForward.z) * Vector3.forward;

                var r90 = Quaternion.AngleAxis(90.0f, Vector3.up);
                localRight = r90 * localForward;
            }
            else
            {
                localForward = (Grounded() - Grounded(sceneCamera.transform.position)).normalized;
                var r90 = Quaternion.AngleAxis(90.0f, Vector3.up);
                localRight = r90 * localForward;
            }

            translatedDir = moveDir.x * localRight + moveDir.y * localForward;
        }
        public virtual void ToggleTranslateDir() { }
        protected virtual void ProcessMove() { }

        private void Update()
        {
            ProcessMove();

            ToggleTranslateDir();

            if ((moveDir.sqrMagnitude != 0.0f || rb.velocity.x != 0.0f || rb.velocity.z != 0.0f) && !fadingOut)
                ReadLocalAxis();

            desiredRotVelocity = default;

            var rotationDirCur = CurrentRotationDir(translatedDir);
            var rotSpeedCur = CurrentRotationSpeed(rotationSpeed);
            
            var speedCur = isCancelMoveOnDealingDamage || isCancelMoveOnRangeAttack ?
                0 : CurrentMoveSpeed(speed);

            var rs = rotSpeedCur * Mathf.Deg2Rad; // 3 ~ 180 deg/s
            var angleY = Vector3.SignedAngle(transform.forward, rotationDirCur, Vector3.up) * Mathf.Deg2Rad;
            desiredRotVelocity.y = rs * angleY;

            var angleX = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.right) * Mathf.Deg2Rad;
            desiredRotVelocity.x = rs * -angleX;

            var angleZ = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.forward) * Mathf.Deg2Rad;
            desiredRotVelocity.z = rs * -angleZ;

            var desiredSpeed = (moveDir.sqrMagnitude != 0.0f && !fadingOut) ? speedCur : 0;

            desiredVelocity = CurrentMoveDir(translatedDir) * desiredSpeed;
        }

        private void FixedUpdate()
        {
            if (rb.position.y < outOfSceneYTreshold)
            {
                OnUnitKilled.Invoke(this);
                return;
            }

            rb.ResetCenterOfMass();

            if (fadingOut)
                return;

            var velocity = rb.velocity;
            var speedCur = CurrentMoveSpeed(speed);

            float maxSpeedChange = speedCur * 4 * Time.fixedDeltaTime;
            velocity.x =
                Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z =
                Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            rb.velocity = velocity;

            var rotVelocity = rb.angularVelocity;
            var rotSpeedCur = CurrentRotationSpeed(rotationSpeed);

            float maxRotSpeedChange = rotSpeedCur * 4 * Time.fixedDeltaTime;
            rotVelocity.x =
                Mathf.MoveTowards(rotVelocity.x, desiredRotVelocity.x, maxRotSpeedChange);
            rotVelocity.y =
                Mathf.MoveTowards(rotVelocity.y, desiredRotVelocity.y, maxRotSpeedChange);
            rotVelocity.z =
                Mathf.MoveTowards(rotVelocity.z, desiredRotVelocity.z, maxRotSpeedChange);

            rb.angularVelocity = rotVelocity;

        }
    }
}