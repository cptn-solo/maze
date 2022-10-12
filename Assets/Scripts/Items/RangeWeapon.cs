using UnityEngine;

namespace Assets.Scripts
{
    public class RangeWeapon : MonoBehaviour
    {
        private const string AnimAttackBool = "attack";

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private int damagePerShot = 1;

        public int PerkDamage { get => damagePerShot; set => damagePerShot = value; }
        public IngameSoundEvents SoundEvents { get; set; }

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OutOfAmmo() =>
            SoundEvents.OutOfAmmo();

        protected virtual void ShotSound() =>
            SoundEvents.MinigunShot();
        public virtual void AfterEachShot() {}
        public void Attack(bool toggle)
        {
            animator.SetBool(AnimAttackBool, toggle);

            if (!toggle)
                return;
                
            ShotSound();

            if (!Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1.0f, targetMask))
                return;

            if (!hitInfo.collider.TryGetComponent<Hitbox>(out var hitbox))
                return;

            if (hitbox.CurrentHP <= 0)
                return;

            hitbox.DealDamage(PerkDamage);
        }
    }
}