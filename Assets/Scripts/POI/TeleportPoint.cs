using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class TeleportPoint : MonoBehaviour
    {
        [SerializeField] private LayerMask playerMask;

        private Animator animator;
        private PortalGate[] tpGates;

        public event Action<Vector3, Vector3, MovableUnit> OnEnterPortal;

        public PortalGate ExitGate(Vector3 dir)
        {
            foreach (var gate in tpGates.Where(x => x.isActiveAndEnabled).ToArray())
            {
                var exitDir = (gate.transform.position - transform.position).normalized;
                if (dir == exitDir)
                    return gate;
            }
            return null;
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            tpGates = GetComponentsInChildren<PortalGate>();
        }
        private void OnDisable()
        {
            foreach (var tp in tpGates)
                tp.OnEnterGate -= Tp_OnEnterGate;
        }
        private void OnEnable()
        {
            foreach (var tp in tpGates)
            {
                tp.OnEnterGate += Tp_OnEnterGate;
                tp.PassengerMask = playerMask;
            }
        }

        private void Tp_OnEnterGate(PortalGate gate, MovableUnit passenger)
        {
            OnEnterPortal?.Invoke(
                     transform.position,
                     (transform.position - gate.transform.position).normalized,
                     passenger);
        }

        private void Start()
        {
            if (animator)
            {
                animator.SetBool("mirror", Random.Range(0, 2) == 1);
                animator.SetFloat("speed", Random.Range(0.02f, 0.05f));
            }
        }
    }
}