using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Chest : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private LayerMask openMask;

        private const string AnimOpenBool = "open";
        private bool nearby = false;
        private bool dropped = false;

        public event Action<Chest> OnChestOpened;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CheckColliderMask(openMask))
                return;

            if (!nearby && !dropped)
            {
                StartCoroutine(CheckNearby());
                StartCoroutine(SpawnCollectables());
            }

            nearby = true;
        }

        private IEnumerator CheckNearby()
        {
            nearby = true;
            animator.SetBool(AnimOpenBool, nearby);

            while (nearby)
            {
                nearby = false;
                yield return new WaitForSeconds(2.0f);
            }

            animator.SetBool(AnimOpenBool, nearby);
        }

        private IEnumerator SpawnCollectables()
        {
            yield return new WaitForSeconds(.3f);

            OnChestOpened?.Invoke(this);

            dropped = true;

            while (nearby)
                yield return new WaitForSeconds(1.0f);

            yield return new WaitForSeconds(5.0f);
            
            dropped = false;
            nearby = false;
        }


        private void OnDisable()
        {
            nearby = false;
            
            StopCoroutine(CheckNearby());
            
            animator.SetBool(AnimOpenBool, false);
        }

        private void OnEnable()
        {
            animator.SetBool(AnimOpenBool, false);
        }
    }
}