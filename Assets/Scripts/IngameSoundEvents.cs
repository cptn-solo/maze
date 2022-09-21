using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class IngameSoundEvents : MonoBehaviour, IIngameSoundEvents
    {
        public event EventHandler OnPlayerAttack;
        public event EventHandler OnPlayerDamaged;
        public event EventHandler OnPlayerDamagedCritical;
        public event EventHandler OnPlayerWalk;
        public event EventHandler OnPlayerJump;
        public event EventHandler OnZombieAttack;
        public event EventHandler OnZombieDamaged;
        public event EventHandler OnZombieDamagedCritical;
        public event EventHandler OnZombieKilled;

        public void PlayerAttack() => OnPlayerAttack?.Invoke(this, null);
        public void PlayerDamaged() => OnPlayerDamaged?.Invoke(this, null);
        public void PlayerDamagedCritical() => OnPlayerDamagedCritical?.Invoke(this, null);
        public void PlayerWalk() => OnPlayerWalk?.Invoke(this, null);
        public void PlayerJump() => OnPlayerJump?.Invoke(this, null);
        public void ZombieAttack() => OnZombieAttack?.Invoke(this, null);
        public void ZombieDamaged() => OnZombieDamaged?.Invoke(this, null);
        public void ZombieDamagedCritical() => OnZombieDamagedCritical?.Invoke(this, null);
        public void ZombieKilled() => OnZombieKilled?.Invoke(this, null);

    }
}