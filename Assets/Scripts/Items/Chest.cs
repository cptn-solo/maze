using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Chest : MonoBehaviour
    {
        private Animator animator;
        
        [SerializeField] private LayerMask openMask;
        [SerializeField] private float dropCooldownTime = 60.0f;
        [SerializeField] private float dropPassableTime = 5.0f;

        [SerializeField] private LayerMask maskWhenOpen;
        [SerializeField] private LayerMask maskWhenClosed;
        [SerializeField] private Transform passableBody;

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

            passableBody.gameObject.layer = maskWhenOpen.FirstSetLayer();

            while (nearby)
                yield return new WaitForSeconds(1.0f);

            
            yield return new WaitForSeconds(dropPassableTime);
            
            passableBody.gameObject.layer = maskWhenClosed.FirstSetLayer();

            yield return new WaitForSeconds(dropCooldownTime - dropPassableTime);
            
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