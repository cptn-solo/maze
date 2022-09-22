using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Battle : MonoBehaviour
    {
        private Hitbox hitbox;
        private MovableUnit unit;

        private BattleInfo battleInfo = default;

        public event Action<BattleInfo> OnBattleInfoChange;
        public event Action<bool> OnTakingDamage;

        private void Awake()
        {
            hitbox = GetComponentInChildren<Hitbox>();
            
            unit = GetComponent<MovableUnit>();            
            unit.OnUnitRespawned += Unit_OnUnitRespawned;
        }

        private void OnEnable()
        {
            if (hitbox != null)
            {
                hitbox.OnAddShield += Hitbox_OnAddShield;
                hitbox.OnAddHP += Hitbox_OnAddHP;
                hitbox.OnDamage += Hitbox_OnDamage;
                hitbox.OnShieldDamage += Hitbox_OnShieldDamage;
                hitbox.OnZeroHealthReached += Hitbox_OnZeroHealthReached;
                hitbox.OnCriticalDamage += Hitbox_OnCriticalDamage;
            }
        }
        private void OnDisable()
        {
            if (hitbox != null)
            {
                hitbox.OnAddShield -= Hitbox_OnAddShield;
                hitbox.OnAddHP -= Hitbox_OnAddHP;
                hitbox.OnDamage -= Hitbox_OnDamage;
                hitbox.OnShieldDamage -= Hitbox_OnShieldDamage;
                hitbox.OnZeroHealthReached -= Hitbox_OnZeroHealthReached;
                hitbox.OnCriticalDamage -= Hitbox_OnCriticalDamage;
            }
        }

        private void Unit_OnUnitRespawned(MovableUnit unit)
        {
            if (hitbox)
            {
                battleInfo = hitbox.ResetHP();
                
                battleInfo.CriticalHP = false;

                OnBattleInfoChange?.Invoke(battleInfo);
            }
        }
        private void Hitbox_OnAddHP(int currentHP)
        {
            battleInfo.CurrentHP = currentHP;
            OnBattleInfoChange?.Invoke(battleInfo);
        }

        private void Hitbox_OnAddShield(int currentShield)
        {
            battleInfo.CurrentShield = currentShield;
            OnBattleInfoChange?.Invoke(battleInfo);
        }
        private void Hitbox_OnShieldDamage(int currentShield)
        {
            battleInfo.CurrentShield = currentShield;

            OnTakingDamage?.Invoke(battleInfo.CriticalHP);
            OnBattleInfoChange?.Invoke(battleInfo);
        }

        private void Hitbox_OnDamage(int currentHP)
        {
            battleInfo.CurrentHP = currentHP;
            
            OnTakingDamage?.Invoke(battleInfo.CriticalHP);
            OnBattleInfoChange?.Invoke(battleInfo);
        }

        private void Hitbox_OnCriticalDamage(int currentHP)
        {
            battleInfo.CurrentHP = currentHP;
            battleInfo.CriticalHP = true;

            OnTakingDamage?.Invoke(battleInfo.CriticalHP);
            OnBattleInfoChange?.Invoke(battleInfo);
        }

        private void Hitbox_OnZeroHealthReached()
        {
            battleInfo.CurrentHP = 0;
            OnBattleInfoChange?.Invoke(battleInfo);
        }
    }
}