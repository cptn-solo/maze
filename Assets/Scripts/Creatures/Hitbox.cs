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

        public string PlayerId;
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

        private float shieldCellStrength = 1.0f;
        private int maxHPScaled;
        private int maxShieldScaled;

        public BattleInfo ResetHP()
        {
            maxHPScaled = Mathf.FloorToInt(maxHP * SizeScale);
            maxShieldScaled = maxShield > 0 ?  maxHPScaled : 0;

            CurrentHP = maxHPScaled;
            // just for alignment purposes, to set the initial widths same value for both HP and shields (if any):
            shieldCellStrength = maxHPScaled != 0 ? maxShieldScaled / maxHPScaled : 0f;
            CurrentShield = shieldCellStrength > 0 ? CurrentHP : 0;

            BattleInfo battleInfo = default;
            battleInfo.CurrentHP = CurrentHP;
            battleInfo.CurrentShield = CurrentShield;
            battleInfo.MaxHP = maxHPScaled;
            battleInfo.MaxShield = maxShieldScaled;
            
            return battleInfo;
        }

        public void DealDamage(int damage)
        {
            if (CurrentShield > 0)
                damage = Mathf.FloorToInt(damage / shieldCellStrength);

            var combined = CurrentHP + CurrentShield;

            combined -= damage;

            if (combined >= CurrentHP)
            {
                CurrentShield = combined - CurrentHP;
                OnShieldDamage?.Invoke(CurrentShield);
            }
            else if (combined > 0)
            {
                CurrentShield = 0;
                CurrentHP = combined;
                
                (CurrentHP <= criticalHP ? OnCriticalDamage : OnDamage).Invoke(CurrentHP);
            }
            else
            {
                CurrentShield = 0;
                CurrentHP = 0;
                OnZeroHealthReached?.Invoke();
            }
        }

        internal void AddHP(int cnt)
        {
            CurrentHP = Mathf.Min(CurrentHP + cnt, maxHPScaled);
            OnAddHP?.Invoke(CurrentHP);
        }

        internal void AddShield(int cnt)
        {
            var combined = CurrentHP + CurrentShield + Mathf.FloorToInt(cnt / shieldCellStrength);

            if (combined >= maxHPScaled + maxShieldScaled)
            {
                CurrentHP = maxHPScaled;
                CurrentShield = maxShieldScaled;
                OnAddShield?.Invoke(CurrentShield);
            }
            else if (combined >= maxHPScaled)
            {
                CurrentHP = maxHPScaled;
                CurrentShield = combined - maxHPScaled;
                OnAddShield?.Invoke(CurrentShield);
            }
            else
            {
                CurrentHP = combined;
                CurrentShield = 0;
                OnAddHP?.Invoke(CurrentHP);
            }
        }
        private void OnDestroy() => OnDestroyedOrDisabled?.Invoke();
        private void OnDisable() => OnDestroyedOrDisabled?.Invoke();

    }
}