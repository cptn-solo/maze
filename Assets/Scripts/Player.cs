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
        OnRespawned();
    }

    public void OnRespawned()
    {
        targetPosition = default;
        rb.velocity = Vector3.zero;
        StartCoroutine(InitPositionOnSpawn());
    }
    private IEnumerator InitPositionOnSpawn()
    {
        yield return new WaitForSeconds(2.0f);

        ReadPathSegment();
        ReadLocalAxis();

    }

    public void ReadPathSegment()
    {
        pathPoints = pathPoints.Select(x => {
            x.y = transform.position.y; return x;
        }).ToArray();
        
        var ordered = pathPoints.OrderBy(x => Vector3.Distance(x, transform.position)).ToArray();
        
        var closest = ordered.Take(2).ToArray();
        this.closest1 = closest[0];
        this.closest2 = closest[1];
        
        if (Vector3.Distance(closest[0], transform.position) < treshold)
            SwitchSideTo(closest[0], ordered[2]);
    }

    private void ReadLocalAxis()
    {
        var buildingFloorCenter = new Vector3(
            building.transform.position.x, transform.position.y, building.transform.position.z);
        var toCenter = (buildingFloorCenter - transform.position).normalized;
        var toClosest1 = (closest1 - transform.position).normalized;
        var toClosest2 = (closest2 - transform.position).normalized;

        localRight = (Vector3.SignedAngle(toCenter, toClosest1, transform.up) > 0) ? toClosest1 : toClosest2;
        var r90 = Quaternion.AngleAxis(-90.0f, Vector3.up);
        localForward = r90 * localRight;
    }

    private void SwitchSideTo(Vector3 closest1, Vector3 closest2)
    {
        targetPosition = closest1 + (closest2 - closest1).normalized * (treshold + .05f);
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
        Gizmos.DrawSphere(closest1, .1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(closest2, .1f);

        Gizmos.color = old;

    }
#endif

    private void Update()
    {
        if ((Vector3.Distance(closest1, transform.position) < treshold ||
            Vector3.Distance(closest2, transform.position) < treshold) &&
            targetPosition == default)
            ReadPathSegment();

        if (targetPosition != default)
        {
            if (Vector3.Distance(targetPosition, transform.position) > 0.05f)
            {
                var dir = (targetPosition - transform.position).normalized;
                transform.position += dir * Time.deltaTime * speed;
                
                var lookDir = (closest2 - transform.position).normalized;
                var delta1 = Vector3.SignedAngle(
                    transform.forward, 
                    lookDir, 
                    transform.up) * Time.deltaTime * rotationSpeed;
                
                transform.RotateAround(transform.position, transform.up, delta1);
            }
            else
            {
                transform.position = targetPosition;
                targetPosition = default;

                ReadLocalAxis();
            }

            return;
        }

        translatedDir = (inputDir.x * localRight + inputDir.z * localForward);

        transform.position += translatedDir * Time.deltaTime * speed;
        var delta2 = Vector3.SignedAngle(transform.forward, translatedDir, transform.up) * Time.deltaTime * rotationSpeed;
        transform.RotateAround(transform.position, transform.up, delta2);
    }
}
