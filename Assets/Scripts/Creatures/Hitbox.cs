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

        private int maxHPScaled;
        private int maxShieldScaled;

        public int PerkMaxHP { get => maxHP; set => maxHP = value; }
        public int PerkMaxShield { get => maxShield; set => maxShield = value; }

        public BattleInfo ResetHP()
        {
            maxHPScaled = Mathf.FloorToInt(PerkMaxHP * SizeScale);
            maxShieldScaled = PerkMaxShield > 0 ? Mathf.FloorToInt(PerkMaxShield * SizeScale) : 0;

            CurrentHP = maxHPScaled;
            CurrentShield = maxShieldScaled;

            BattleInfo battleInfo = default;
            battleInfo.CurrentHP = CurrentHP;
            battleInfo.CurrentShield = CurrentShield;
            battleInfo.MaxHP = maxHPScaled;
            battleInfo.MaxShield = maxShieldScaled;
            
            return battleInfo;
        }

        public void DealDamage(int damage)
        {
            CurrentShield = Mathf.Max(0, CurrentShield - damage);
            
            if (maxShieldScaled > 0 && CurrentShield > 0)
                damage = Mathf.FloorToInt(damage * (1 - CurrentShield/maxShieldScaled));

            CurrentHP = Mathf.Max(0, CurrentHP - damage);

            if (CurrentHP <= 0)
                OnZeroHealthReached?.Invoke();
            else
                (CurrentHP <= criticalHP ? OnCriticalDamage : OnDamage).Invoke(CurrentHP);
        }

        internal void AddHP(int cnt)
        {
            CurrentHP = Mathf.Min(CurrentHP + cnt, maxHPScaled);
            OnAddHP?.Invoke(CurrentHP);
        }

        internal void AddShield(int cnt)
        {
            CurrentShield = Mathf.Min(CurrentShield + cnt, maxShieldScaled);
            OnAddShield?.Invoke(CurrentShield);
        }
        private void OnDestroy() => OnDestroyedOrDisabled?.Invoke();
        private void OnDisable() => OnDestroyedOrDisabled?.Invoke();

    }
}