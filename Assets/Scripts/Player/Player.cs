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

        private Rigidbody rb;
        private Collider col;

        [SerializeField] private Renderer ren;
        [SerializeField] private Animator animator;

        [SerializeField] private float tresholdSqr = .0025f;
        [SerializeField] private float treshold = .05f;
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float rotationSpeed = 30.0f;
        [SerializeField] private float apexSpeed = 15.0f;
        [SerializeField] private float jumpImpulse = 2.0f;

        private Building building;
        private Vector3[] pathPoints;

        private Vector3 targetPosition;
        private Vector3 sideSwitchPivot;
        private float sideSwitchRadius;
        private Vector3 closest1;
        private Vector3 closest2;

        private Vector3 localRight;
        private Vector3 localForward;

        private Vector3 translatedDir;
        private Vector3 inputDir;
        private Vector3 center = Vector3.zero;
        private Vector3 walkPosition;

        public Building Building
        {
            get => building;
            set
            {
                building = value;
                pathPoints = building.PathPoints.Select(x => x.position).ToArray();
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

        private void Start()
        {
            OnRespawned();
        }

        public void OnRespawned()
        {
            targetPosition = default;
            sideSwitchPivot = default;
            
            pathPoints = pathPoints.Select(Grounded).ToArray();

            rb.velocity = Vector3.zero;
            rb.MoveRotation(Quaternion.identity);

            ReadPathSegment();
            ReadLocalAxis();
        }

        public void ReadPathSegment(bool forceSwitchSide = false)
        {
            var grounded = Grounded();
            var ordered = pathPoints.OrderBy(x => (x - grounded).sqrMagnitude).ToArray();

            var closest = ordered.Take(2).ToArray();
            closest1 = closest[0];
            closest2 = closest[1];

            if (forceSwitchSide)
                SwitchSideTo(closest[0], ordered[2]);
        }

        private void ReadLocalAxis()
        {
            var closestPointOnSide = GetClosestPoint(Grounded(), closest1, closest2);

            var toCenter = (center - closestPointOnSide).normalized;
            var toClosest1 = (closest1 - closestPointOnSide).normalized;
            var toClosest2 = (closest2 - closestPointOnSide).normalized;

            localRight = Vector3.SignedAngle(toCenter, toClosest1, Vector3.up) > 0 ? toClosest1 : toClosest2;

            var r90 = Quaternion.AngleAxis(-90.0f, Vector3.up);

            localForward = r90 * localRight;
        }

        private void SwitchSideTo(Vector3 closest1, Vector3 closest2)
        {
            var grounded = Grounded();
            var closestPoint = GetClosestPoint(grounded, center, closest1);

            targetPosition = closestPoint + 1.01f * treshold * (closestPoint - grounded).normalized;
            sideSwitchPivot = closestPoint + (center - closest1).normalized * treshold;
            sideSwitchRadius = (sideSwitchPivot - targetPosition).magnitude;

            this.closest1 = closest1;
            this.closest2 = closest2;

            StartCoroutine(ApexMove());
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            inputDir = obj.ReadValue<Vector3>();
            animator.SetBool(AnimGoBool, inputDir.magnitude > 0);
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            if (rb.velocity.y < .01f)
            {
                StartCoroutine(JumpCoroutine());
            }
        }

        private IEnumerator JumpCoroutine()
        {
            animator.SetBool(AnimJumpBool, true);
            yield return new WaitForSeconds(.01f);
            animator.SetBool(AnimJumpBool, false);

            yield return new WaitForSeconds(.1f);
            rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);


        }

        Vector3 GetClosestPoint(Vector3 point, Vector3 line_start, Vector3 line_end)
        {
            return line_start + Vector3.Project(point - line_start, line_end - line_start);
        }

        private void FixedUpdate()
        {
            var grounded = Grounded();

            if (inputDir.z > 0 && (grounded - center).sqrMagnitude < tresholdSqr * 4)
            {
                if (inputDir.x != 0)
                    inputDir.z = 0;
                else
                    return;
            }

            if (targetPosition == default &&
                (ToRaySqr(grounded, closest1) < tresholdSqr || ToRaySqr(grounded, closest2) < tresholdSqr))
            {
                ReadPathSegment(true);
            }

            if (targetPosition == default && inputDir.magnitude != 0.0f && !fadingOut)
            {
                translatedDir = inputDir.x * localRight + inputDir.z * localForward;
                Vector3 rbHorizontalVelocity = default;
                rbHorizontalVelocity.x = rb.velocity.x;
                rbHorizontalVelocity.z = rb.velocity.z;

                var deltaPos = (speed - rbHorizontalVelocity.magnitude);
                if (deltaPos > 0)
                    rb.AddForce(translatedDir * deltaPos, ForceMode.VelocityChange);
            }
            var angle = Vector3.SignedAngle(transform.forward, translatedDir, Vector3.up) * Time.fixedDeltaTime;
            var torque = rotationSpeed * angle - rb.angularVelocity.y;
            rb.AddTorque(0, torque, 0, ForceMode.VelocityChange);            
        }

        private Vector3 Grounded(Vector3 position = default)
        {
            if (position == default)
                position = rb.position;

            return position - Vector3.up * position.y;
        }

        private float ToSideSqr(Vector3 grounded = default, Vector3 closest1 = default, Vector3 closest2 = default)
        {
            if (grounded == default)
                grounded = Grounded();

            if (closest1 == default)
                closest1 = this.closest1;

            if (closest2 == default)
                closest2 = this.closest2;

            return (grounded - GetClosestPoint(grounded, closest1, closest2)).sqrMagnitude;
        }

        private float ToRaySqr(Vector3 grounded, Vector3 rayEndGrounded)
        {
            return (grounded - GetClosestPoint(grounded, center, rayEndGrounded)).sqrMagnitude;
        }
    }
}