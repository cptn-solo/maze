using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private LayerMask damagedBy;
        [SerializeField] private int maxHP;
        [SerializeField] private int maxShield;
        [SerializeField] private int criticalHP;

        public int CurrentHP { get; private set; }
        public int CurrentShield { get; private set; }

        public float SizeScale { get; set; } = 1.0f;

        public event Action<int> OnShieldDamage;
        public event Action<int> OnDamage;
        public event Action<int> OnCriticalDamage;
        public event Action<int> OnAddHP;
        public event Action<int> OnAddShield;

        public event Action OnZeroHealthReached;
        public event Action OnDestroyedOrDisabled;

        public BattleInfo ResetHP()
        {
            CurrentHP = Mathf.FloorToInt(maxHP * SizeScale);
            CurrentShield = Mathf.FloorToInt(maxShield * SizeScale);

            BattleInfo battleInfo = default;
            battleInfo.CurrentHP = CurrentHP;
            battleInfo.CurrentShield = CurrentShield;
            battleInfo.MaxHP = CurrentHP;
            battleInfo.MaxShield = CurrentShield;
            
            return battleInfo;
        }

        public void DealDamage(int damage)
        {
            if (CurrentShield >= damage)
            {
                CurrentShield -= damage;
                OnShieldDamage?.Invoke(CurrentShield);
                damage = 0;
            }
            else
            {
                CurrentShield = 0;
                OnShieldDamage?.Invoke(CurrentShield);
                damage -= CurrentShield;
            }

            if (CurrentHP <= damage)
            {
                CurrentHP = 0;
                OnZeroHealthReached?.Invoke();
            }
            else if (CurrentHP - damage <= criticalHP)
            {
                CurrentHP -= damage;
                OnCriticalDamage?.Invoke(CurrentHP);
            }
            else if (CurrentHP > damage)
            {
                CurrentHP -= damage;
                OnDamage?.Invoke(CurrentHP);
            }
        }

        internal void AddHP(int cnt)
        {
            if (CurrentHP + cnt <= maxHP * SizeScale)
            {
                CurrentHP += cnt;
                OnAddHP?.Invoke(CurrentHP);
            }
        }

        internal void AddShield(int cnt)
        {
            if (CurrentHP < maxHP * SizeScale)
            {
                CurrentHP += cnt;
                OnAddHP?.Invoke(CurrentHP);
            }
            else if (CurrentShield + cnt <= maxShield * SizeScale)
            {
                CurrentShield += cnt;
                OnAddShield?.Invoke(CurrentShield);
            }
        }
        private void OnDestroy() => OnDestroyedOrDisabled?.Invoke();
        private void OnDisable() => OnDestroyedOrDisabled?.Invoke();

    }
}