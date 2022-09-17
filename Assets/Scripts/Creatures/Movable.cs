using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Movable : MonoBehaviour
    {
        protected Rigidbody rb;
        protected Collider col;

        [SerializeField] protected Renderer ren;
        [SerializeField] protected Animator animator;

        protected Vector3 desiredRotVelocity;
        protected Vector3 desiredVelocity;

        [SerializeField] protected float speed = 1.5f;
        [SerializeField] protected float rotationSpeed = 30.0f;
        [SerializeField] protected float fadeSpeed = 1.0f;

        [SerializeField] protected float jumpImpulse = 2.0f;

        protected Building building;

        protected Vector3 moveDir;

        protected Vector3 localRight;
        protected Vector3 localForward;

        private Action OnAwakeAction;
        private Action OnStartAction;

        public Building Building
        {
            get => building;
            set
            {
                building = value;
            }
        }

        private Vector3 translatedDir;
        private Vector3 center = Vector3.zero;

        public void OnRespawned(Vector3 position, Quaternion rotation)
        {
            TogglePhisBody(false);

            rb.velocity = Vector3.zero;

            rb.ResetInertiaTensor();
            rb.ResetCenterOfMass();

            rb.MovePosition(position);
            rb.MoveRotation(rotation);

            TogglePhisBody(true);            
        }

        private void ReadLocalAxis()
        {
            localForward = (center - Grounded()).normalized;

            if (Mathf.Abs(localForward.x) > Mathf.Abs(localForward.z) * 1.2f)
                localForward = Mathf.Sign(localForward.x) * Vector3.right;
            else if (Mathf.Abs(localForward.z) > Mathf.Abs(localForward.x) * 1.2f)
                localForward = Mathf.Sign(localForward.z) * Vector3.forward;

            var r90 = Quaternion.AngleAxis(90.0f, Vector3.up);
            localRight = r90 * localForward;

            translatedDir = moveDir.x * localRight + moveDir.z * localForward;
        }

        protected virtual void OnAwake()
        {
            OnAwakeAction?.Invoke();
        }

        protected virtual void OnStart()
        {
            OnStartAction?.Invoke();
        }


        protected virtual void OnObjEnable()
        {
            var visChecker = GetComponentInChildren<VizibilityChecker>();
            if (visChecker)
                visChecker.OnVisibilityChanged += VisChecker_OnVisibilityChanged;
        }

        protected virtual void OnObjDisable()
        {
            var visChecker = GetComponentInChildren<VizibilityChecker>();
            if (visChecker)
                visChecker.OnVisibilityChanged -= VisChecker_OnVisibilityChanged;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();

            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void OnEnable()
        {
            OnObjEnable();

        }
        
        private void OnDisable()
        {
            OnObjDisable();
        }


        private void Update()
        {
            if ((moveDir.sqrMagnitude != 0.0f || rb.velocity.x != 0.0f || rb.velocity.z != 0.0f) && !fadingOut)
                ReadLocalAxis();

            desiredRotVelocity = default;

            var rs = rotationSpeed * Mathf.Deg2Rad; // 3 ~ 180 deg/s
            var angleY = Vector3.SignedAngle(transform.forward, translatedDir, Vector3.up) * Mathf.Deg2Rad;
            desiredRotVelocity.y = rs * angleY;

            var angleX = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.right) * Mathf.Deg2Rad;
            desiredRotVelocity.x = rs * -angleX;

            var angleZ = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.forward) * Mathf.Deg2Rad;
            desiredRotVelocity.z = rs * -angleZ;

            var desiredSpeed = (moveDir.sqrMagnitude != 0.0f && !fadingOut) ? speed : 0;

            desiredVelocity = translatedDir * desiredSpeed;
        }

        private void FixedUpdate()
        {
            rb.ResetCenterOfMass();

            if (fadingOut)
                return;

            var velocity = rb.velocity;
            float maxSpeedChange = speed * 4 * Time.fixedDeltaTime;
            velocity.x =
                Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z =
                Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
            rb.velocity = velocity;

            var rotVelocity = rb.angularVelocity;
            float maxRotSpeedChange = rotationSpeed * 4 * Time.fixedDeltaTime;
            rotVelocity.x =
                Mathf.MoveTowards(rotVelocity.x, desiredRotVelocity.x, maxRotSpeedChange);
            rotVelocity.y =
                Mathf.MoveTowards(rotVelocity.y, desiredRotVelocity.y, maxRotSpeedChange);
            rotVelocity.z =
                Mathf.MoveTowards(rotVelocity.z, desiredRotVelocity.z, maxRotSpeedChange);

            rb.angularVelocity = rotVelocity;

        }

        private Vector3 Grounded(Vector3 position = default)
        {
            if (position == default)
                position = rb.position;

            return position - Vector3.up * position.y;
        }
    }
}