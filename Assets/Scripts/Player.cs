using System.Collections;
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
    private Vector3 closest1;
    private Vector3 closest2;

    private Vector3 localRight;
    private Vector3 localForward;

    [SerializeField] private float treshold = .05f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotationSpeed = 30.0f;

    private Vector3 translatedDir;
    private Vector3 inputDir;

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

        var toCenter = (Vector3.zero - closestPointOnSide).normalized;
        var toClosest1 = (closest1 - closestPointOnSide).normalized;
        var toClosest2 = (closest2 - closestPointOnSide).normalized;

        localRight = (Vector3.SignedAngle(toCenter, toClosest1, transform.up) > 0) ? toClosest1 : toClosest2;
        
        var r90 = Quaternion.AngleAxis(-90.0f, Vector3.up);
        
        localForward = r90 * localRight;
    }

    private void SwitchSideTo(Vector3 closest1, Vector3 closest2)
    {
        var grounded = Grounded();
        var closestPoint = GetClosestPoint(grounded, Vector3.zero, closest1);
        
        targetPosition = closestPoint + (closestPoint - grounded).normalized * treshold * 1.001f;
        sideSwitchPivot = closestPoint;
        
        this.closest1 = closest1;
        this.closest2 = closest2;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector3>();
        Debug.Log($"Move {dir}");
        
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

        if (inputDir.z < 0 && ToSide() < treshold)
        {
            if (inputDir.x != 0)
                inputDir.z = 0;
            else
                return;
        }

        if (inputDir.z > 0 && Vector3.Distance(grounded, Vector3.zero) < treshold * 4)
        {
            if (inputDir.x != 0)
                inputDir.z = 0;
            else
                return;
        }

        if ((ToRay(grounded, closest1) < treshold || ToRay(grounded, closest2) < treshold) &&
            targetPosition == default)
        {
            ReadPathSegment(true);
        }

        if (targetPosition != default)
        {
            if (Vector3.Distance(targetPosition, Vector3.zero) < treshold * 2)
            {
                targetPosition += 2 * treshold * (targetPosition - Vector3.zero).normalized;
            }

            if (Vector3.Distance(targetPosition, grounded) > 0.05f)
            {
                var moveDir = (targetPosition - grounded).normalized;
                transform.position += speed * Time.deltaTime * moveDir;

                var lookDir = (closest2 - grounded).normalized;
                var delta1 = Vector3.SignedAngle(
                    transform.forward,
                    lookDir,
                    transform.up) * Time.deltaTime * rotationSpeed;

                transform.RotateAround(transform.position, transform.up, delta1);
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
    
    private float ToSide(Vector3 grounded = default, Vector3 closest1 = default, Vector3 closest2 = default)
    {
        if (grounded == default)
            grounded = Grounded();

        if (closest1 == default)
            closest1 = this.closest1;

        if (closest2 == default)
            closest2 = this.closest2;

        return Vector3.Distance(grounded, GetClosestPoint(grounded, closest1, closest2));
    }

    private float ToRay(Vector3 grounded, Vector3 rayEndGrounded)
    {
        return Vector3.Distance(grounded, GetClosestPoint(grounded, Vector3.zero, rayEndGrounded));
    }
}
