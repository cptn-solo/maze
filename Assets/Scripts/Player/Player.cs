using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public partial class Player : MonoBehaviour
    {
        private const string AnimJumpBool = "jump";
        private const string AnimGoBool = "go";
        private const string AnimSpeedFloat = "speed";

        private Rigidbody rb;
        private Collider col;

        [SerializeField] private Renderer ren;
        [SerializeField] private Animator animator;

        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float rotationSpeed = 30.0f;
        [SerializeField] private float jumpImpulse = 2.0f;

        private Building building;

        private Vector3 localRight;
        private Vector3 localForward;

        private Vector3 translatedDir;
        private Vector3 inputDir;
        private Vector3 center = Vector3.zero;

        public Building Building
        {
            get => building;
            set
            {
                building = value;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();

            PlayerInputActions actions = new PlayerInputActions();

            actions.Enable();
            actions.Default.Jump.performed += Jump_performed;
            actions.Default.Move.performed += Move_performed;
        }

        public void OnRespawned(Vector3 position, Quaternion rotation)
        {
            TogglePhisBody(false);

            rb.velocity = Vector3.zero;

            rb.ResetInertiaTensor();
            rb.ResetCenterOfMass();

            rb.MovePosition(position);
            rb.MoveRotation(rotation);
            //player.transform.parent = building.transform;

            TogglePhisBody(true);
        }        

        private void ReadLocalAxis()
        {
            localForward = (center - Grounded()).normalized;

            if (Mathf.Abs(localForward.x) > Mathf.Abs(localForward.z) * 1f)
                localForward = Mathf.Sign(localForward.x) * Vector3.right;
            else if (Mathf.Abs(localForward.z) > Mathf.Abs(localForward.x) * 1f)
                localForward = Mathf.Sign(localForward.z) * Vector3.forward;

            var r90 = Quaternion.AngleAxis(90.0f, Vector3.up);
            localRight = r90 * localForward;
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            inputDir = obj.ReadValue<Vector3>();
            var go = inputDir.sqrMagnitude > 0;
            animator.SetBool(AnimGoBool, go);
            animator.SetFloat(AnimSpeedFloat, go ? 1 + speed : 1);
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            if (rb.velocity.y < .01f && !fadingOut)
                StartCoroutine(JumpCoroutine());
        }

        private IEnumerator JumpCoroutine()
        {
            animator.SetBool(AnimJumpBool, true);
            yield return new WaitForSeconds(.01f);
            animator.SetBool(AnimJumpBool, false);

            yield return new WaitForSeconds(.1f);
            
            if (!fadingOut)
                rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }

        private void Update()
        {
            if ((inputDir.sqrMagnitude != 0.0f || rb.velocity.x != 0.0f || rb.velocity.z != 0.0f) && !fadingOut)
                ReadLocalAxis();
        }

        private void FixedUpdate()
        {
            if (inputDir.sqrMagnitude != 0.0f && !fadingOut)
            {
                ReadLocalAxis();

                translatedDir = inputDir.x * localRight + inputDir.z * localForward;
                Vector3 rbHorizontalVelocity = default;
                rbHorizontalVelocity.x = rb.velocity.x;
                rbHorizontalVelocity.z = rb.velocity.z;

                var deltaPos = (speed - rbHorizontalVelocity.magnitude);
                if (deltaPos > 0)
                    rb.AddForce(translatedDir * deltaPos, ForceMode.VelocityChange);
            }

            var angle = Vector3.SignedAngle(transform.forward, translatedDir, Vector3.up) * Time.fixedDeltaTime;
            rb.ResetCenterOfMass();
            if (Mathf.Abs(angle) > 0.001f)
            {
                var torque = rotationSpeed * angle - rb.angularVelocity.y;
                rb.AddTorque(0, torque, 0, ForceMode.VelocityChange);
            }
        }

        private Vector3 Grounded(Vector3 position = default)
        {
            if (position == default)
                position = rb.position;

            return position - Vector3.up * position.y;
        }
    }
}