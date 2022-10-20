using UnityEngine;

namespace Assets.Scripts
{
    public class WallmartLevel : Wallmart
    {
        private Animator animator;
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();

        }
        private void Start()
        {
            if (animator)
            {
                animator.SetBool("mirror", false);
                animator.SetFloat("speed", 0.01f);
            }
        }

    }
}