using UnityEngine;

namespace Assets.Scripts
{
    public class RangeWeapon : MonoBehaviour
    {
        private const string AnimAttackBool = "attack";

        [SerializeField] private LayerMask targetMask;
        [SerializeField] private int damagePerShot = 1;
        [SerializeField] protected int ammoPerShot = 1; // patch for shotgun
        [SerializeField] protected float maxRange = 1.0f;
        [SerializeField] protected float effectiveRange = 1.0f;

        public int AmmoPerShot => ammoPerShot;
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

            if (!Physics.Raycast(transform.position, transform.forward, out var hitInfo, maxRange, targetMask))
                return;

            if (!hitInfo.collider.TryGetComponent<Hitbox>(out var hitbox))
                return;

            if (hitbox.CurrentHP <= 0)
                return;

            var factor = hitInfo.distance < effectiveRange ? 
                1 : 
                ((maxRange - hitInfo.distance + effectiveRange) / maxRange);
            
            hitbox.DealDamage(Mathf.FloorToInt(PerkDamage * factor));
        }
    }
}