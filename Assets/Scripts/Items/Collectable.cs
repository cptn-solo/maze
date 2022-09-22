using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public class Collectable : MonoBehaviour
    {
        private const string AnimDropBool = "drop";
        private const string AnimCollectBool = "collect";

        [SerializeField] private LayerMask collectorMask;
        [SerializeField] private CollectableType collectableType = CollectableType.HP;

        private Animator animator;
        private Rigidbody rb;

        public bool Collected { get; set; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }

        private void Release(Collectable collectable)
        {
            Destroy(this.gameObject); // until pool implementation
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!Collected &&
                other.CheckColliderMask(collectorMask) &&
                other.TryGetComponent<Collector>(out var collector))
            {
                StartCoroutine(Collect(collector));
            }
        }

        private IEnumerator WaitForDrop()
        {
            yield return new WaitForSeconds(.1f);

            if (!Time.inFixedTimeStep)
                yield return new WaitForFixedUpdate();

            animator.SetBool(AnimDropBool, true);

            rb.AddForce(Vector3.up * .8f, ForceMode.Impulse);
        }

        private IEnumerator WaitForDespawn()
        {
            yield return new WaitForSeconds(30.0f);

            Release(this);
        }

        private IEnumerator Collect(Collector collector)
        {
            Collected = true;

            if (collector != null)
                collector.Collect(collectableType);

            animator.SetBool(AnimDropBool, false);
            animator.SetBool(AnimCollectBool, true);

            yield return new WaitForSeconds(2.0f);

            Release(this);
        }

        private void OnEnable()
        {
            Collected = false;
            animator.SetBool(AnimCollectBool, false);

            StartCoroutine(WaitForDrop());
            StartCoroutine(WaitForDespawn());
        }
    }
}