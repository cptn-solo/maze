using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Building building;
    private Vector3[] pathPoints;
    
    private Vector3 targetPosition;
    private Vector3 sideSwitchPivot;
    private float sideSwitchAngle;
    private float sideSwitchRadius;
    private Vector3 closest1;
    private Vector3 closest2;

    private Vector3 localRight;
    private Vector3 localForward;
    [SerializeField] private float tresholdSqr = .0025f;
    [SerializeField] private float treshold = .05f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotationSpeed = 30.0f;
    [SerializeField] private float apexSpeed = 15.0f;

    private Vector3 translatedDir;
    private Vector3 inputDir;
    private Vector3 center = Vector3.zero;

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

        PlayerInputActions actions = new PlayerInputActions();

        actions.Enable();
        actions.Default.Jump.performed += Jump_performed;
        actions.Default.Move.performed += Move_performed;
    }

    private void Start()
    {
        pathPoints = pathPoints.Select(Grounded).ToArray();

        OnRespawned();
    }

    public void OnRespawned()
    {
        targetPosition = default;
        sideSwitchPivot = default;

        rb.velocity = Vector3.zero;

        ReadPathSegment();
        ReadLocalAxis();
    }

    public void ReadPathSegment(bool forceSwitchSide = false)
    {
        var grounded = Grounded();
        var ordered = pathPoints.OrderBy(x => (x - grounded).sqrMagnitude).ToArray();
        
        var closest = ordered.Take(2).ToArray();
        this.closest1 = closest[0];
        this.closest2 = closest[1];
        
        if (forceSwitchSide)
            SwitchSideTo(closest[0], ordered[2]);
    }

    private void ReadLocalAxis()
    {
        var closestPointOnSide = GetClosestPoint(Grounded(), closest1, closest2);

        var toCenter = (center - closestPointOnSide).normalized;
        var toClosest1 = (closest1 - closestPointOnSide).normalized;
        var toClosest2 = (closest2 - closestPointOnSide).normalized;

        localRight = (Vector3.SignedAngle(toCenter, toClosest1, transform.up) > 0) ? toClosest1 : toClosest2;
        
        var r90 = Quaternion.AngleAxis(-90.0f, Vector3.up);
        
        localForward = r90 * localRight;
    }

    private void SwitchSideTo(Vector3 closest1, Vector3 closest2)
    {
        var grounded = Grounded();
        var closestPoint = GetClosestPoint(grounded, center, closest1);
        
        targetPosition = closestPoint + 1.01f * treshold * (closestPoint - grounded).normalized;
        sideSwitchPivot = closestPoint + (center - closest1).normalized * treshold;
        sideSwitchAngle = Vector3.SignedAngle(
            (grounded - sideSwitchPivot),
            (targetPosition - sideSwitchPivot),
            Vector3.up);
        sideSwitchRadius = (sideSwitchPivot - targetPosition).magnitude;

        this.closest1 = closest1;
        this.closest2 = closest2;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector3>();
        
        inputDir = dir;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        Debug.Log($"Jump {obj}");
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var old = Gizmos.color;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, localForward * 10.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, localRight * 10.0f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(closest1 + Vector3.up * transform.position.y, .1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(closest2 + Vector3.up * transform.position.y, .1f);

        if (targetPosition != default)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition + Vector3.up * transform.position.y, .02f);
        }
        if (sideSwitchPivot != default)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(sideSwitchPivot + Vector3.up * transform.position.y, .02f);
        }

        Gizmos.color = old;

    }
#endif
    Vector3 GetClosestPoint(Vector3 point, Vector3 line_start, Vector3 line_end)
    {
        return line_start + Vector3.Project(point - line_start, line_end - line_start);
    }

    private void Update()
    {
        var grounded = Grounded();

        if (inputDir.z < 0 && ToSideSqr() < tresholdSqr)
        {
            if (inputDir.x != 0)
                inputDir.z = 0;
            else
                return;
        }

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

        if (targetPosition != default)
        {
            if ((targetPosition - center).sqrMagnitude < tresholdSqr * 2)
            {
                targetPosition += 2 * treshold * (targetPosition - center).normalized;
            }

            if ((targetPosition - grounded).sqrMagnitude > tresholdSqr * .1f)
            {
                var current = (grounded - sideSwitchPivot).normalized;
                var target = (targetPosition - sideSwitchPivot).normalized;
                var step = sideSwitchPivot + sideSwitchRadius * Vector3.RotateTowards(
                    current, target, apexSpeed * Time.deltaTime, 0.0f);

                var lookDir = (step - grounded).normalized;

                var delta1 = Vector3.SignedAngle(
                    transform.forward,
                    lookDir,
                    transform.up);

                transform.RotateAround(transform.position, transform.up, delta1);

                transform.position = step + transform.up * transform.position.y;
            }
            else
            {
                transform.position = targetPosition + Vector3.up * transform.position.y;
                targetPosition = default;

                ReadLocalAxis();
            }

            return;
        }

        translatedDir = (inputDir.x * localRight + inputDir.z * localForward);

        transform.position += speed * Time.deltaTime * translatedDir;
        var delta2 = Vector3.SignedAngle(transform.forward, translatedDir, transform.up) * Time.deltaTime * rotationSpeed;
        transform.RotateAround(transform.position, transform.up, delta2);
    }

    private Vector3 Grounded(Vector3 position = default)
    {
        if (position == default)
            position = transform.position;
        
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
