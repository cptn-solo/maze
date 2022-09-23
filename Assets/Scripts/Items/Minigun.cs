using UnityEngine;

namespace Assets.Scripts
{
    public class Minigun : MonoBehaviour
    {
        private const string AnimAttackBool = "attack";

        private Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void Attack(bool toggle)
        {
            animator.SetBool(AnimAttackBool, toggle);
        }
    }
}