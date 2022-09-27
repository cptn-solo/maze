using UnityEngine;

namespace Assets.Scripts
{
    public class Minigun : MonoBehaviour
    {
        private const string AnimAttackBool = "attack";

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private int damagePerShot = 1;

        public int PerkAddedDamage { get; set; } = 0;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }


        public void Attack(bool toggle)
        {
            animator.SetBool(AnimAttackBool, toggle);

            if (!Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1.0f, targetMask))
                return;

            if (!hitInfo.collider.TryGetComponent<Hitbox>(out var hitbox))
                return;

            if (hitbox.CurrentHP <= 0)
                return;

            hitbox.DealDamage(damagePerShot + PerkAddedDamage);
        }
    }
}