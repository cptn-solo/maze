using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class TeleportPoint : MonoBehaviour
    {
        [SerializeField] private LayerMask playerMask;

        private Animator animator;
        private PortalGate[] tpGates;

        public event Action<Vector3, Vector3, Movable> OnEnterPortal;

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

        private void Tp_OnEnterGate(PortalGate gate, Movable passenger)
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
                animator.SetBool("mirror", Random.Range(0, 2) == 1 ? true : false);
                animator.SetFloat("speed", Random.Range(0.02f, 0.05f));
            }
        }
    }
}