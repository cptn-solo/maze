using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Chest : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private LayerMask openMask;

        private const string AnimOpenBool = "open";
        private bool nearby = false;
        public IngameSoundEvents SoundEvents { get; set; }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (animator == null || !other.CheckColliderMask(openMask))
                return;

            if (!nearby)
                StartCoroutine(CheckNearby());

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