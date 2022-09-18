using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private LayerMask damagedBy;
        [SerializeField] private int maxHP;
        [SerializeField] private int criticalHP;

        private int currentHP;
        public int CurrentHP => currentHP;

        public event Action<int> OnDamage;
        public event Action<int> OnCriticalDamage;
        public event Action OnZeroHealthReached;
        public event Action OnDestroyedOrDisabled;

        public void ResetHP()
        {
            currentHP = maxHP;
        }

        public void DealDamage(int damage)
        {
            if (currentHP <= damage)
            {
                currentHP = 0;
                OnZeroHealthReached?.Invoke();
            }
            else if (currentHP - damage <= criticalHP)
            {
                currentHP -= damage;
                OnCriticalDamage?.Invoke(currentHP);
            }
            else if (currentHP > damage)
            {
                currentHP -= damage;
                OnDamage?.Invoke(currentHP);
            }

            Debug.Log($"Damaged, current HP: {CurrentHP}");
        }

        private void OnDestroy()
        {
            OnDestroyedOrDisabled?.Invoke();
        }

        private void OnDisable()
        {
            OnDestroyedOrDisabled?.Invoke();
        }

    }
}