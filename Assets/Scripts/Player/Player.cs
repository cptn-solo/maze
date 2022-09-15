using System.Collections;
using UnityEngine;

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
        [SerializeField] private float fadeSpeed = 1.0f;
        
        [SerializeField] private float jumpImpulse = 2.0f;

        private Building building;

        private Vector3 localRight;
        private Vector3 localForward;

        private Vector3 translatedDir;
        private Vector3 inputDir;
        private Vector3 center = Vector3.zero;
        
        private Vector3 desiredRotVelocity;
        private bool rotationsEnabled = true;
        private Vector3 desiredVelocity;

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

            BindInputs();
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

            if (Mathf.Abs(localForward.x) > Mathf.Abs(localForward.z) * 1.2f)
                localForward = Mathf.Sign(localForward.x) * Vector3.right;
            else if (Mathf.Abs(localForward.z) > Mathf.Abs(localForward.x) * 1.2f)
                localForward = Mathf.Sign(localForward.z) * Vector3.forward;

            var r90 = Quaternion.AngleAxis(90.0f, Vector3.up);
            localRight = r90 * localForward;

            translatedDir = inputDir.x * localRight + inputDir.z * localForward;

        }

        private void OnMove(Vector3 inputDir)
        {
            this.inputDir = inputDir;
            var go = inputDir.sqrMagnitude > 0;
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

        private void Update()
        {
            if ((inputDir.sqrMagnitude != 0.0f || rb.velocity.x != 0.0f || rb.velocity.z != 0.0f) && !fadingOut)
                ReadLocalAxis();
            
            desiredRotVelocity = default;

            if (rotationsEnabled)
            {
                var rs = rotationSpeed * Mathf.Deg2Rad; // 3 ~ 180 deg/s
                var angleY = Vector3.SignedAngle(transform.forward, translatedDir, Vector3.up) * Mathf.Deg2Rad;
                desiredRotVelocity.y = rs * angleY;

                var angleX = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.right) * Mathf.Deg2Rad;
                desiredRotVelocity.x = rs * -angleX;

                var angleZ = Vector3.SignedAngle(Vector3.up, transform.up, Vector3.forward) * Mathf.Deg2Rad;
                desiredRotVelocity.z = rs * -angleZ;
            }

            var desiredSpeed = (inputDir.sqrMagnitude != 0.0f && !fadingOut) ? speed : 0;
            
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

            if (rotationsEnabled)
            {
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

        }

        private Vector3 Grounded(Vector3 position = default)
        {
            if (position == default)
                position = rb.position;

            return position - Vector3.up * position.y;
        }
    }
}