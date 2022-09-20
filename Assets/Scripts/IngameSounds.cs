using System;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IIngameSoundEvents
    {
        event EventHandler OnPlayerAttack;
        event EventHandler OnPlayerDamaged;
        event EventHandler OnPlayerDamagedCritical;
        event EventHandler OnPlayerWalk;
        event EventHandler OnPlayerJump;

        event EventHandler OnZombieAttack;
        event EventHandler OnZombieDamaged;
        event EventHandler OnZombieDamagedCritical;
        event EventHandler OnZombieKilled;
    }

    public class IngameSounds : MonoBehaviour
    {
        [SerializeField] private IngameSoundEvents soundEvents;

        [SerializeField] private AudioClip attackPlayer;
        [SerializeField] private AudioClip attackZombie;
        [SerializeField] private AudioClip killedPlayer;
        [SerializeField] private AudioClip damagePlayer;
        [SerializeField] private AudioClip damageZombie;
        [SerializeField] private AudioClip killedZombie;
        [SerializeField] private AudioClip walkPlayer;
        [SerializeField] private AudioClip jumpPlayer;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (soundEvents != null)
            {
                soundEvents.OnPlayerAttack += PlayerEvents_OnPlayerAttack;
                soundEvents.OnPlayerDamaged += PlayerEvents_OnPlayerDamaged;
                soundEvents.OnPlayerDamagedCritical += PlayerEvents_OnPlayerDamagedCritical;

                soundEvents.OnZombieAttack += ZombieEvents_OnZombieAttack;
                soundEvents.OnZombieDamaged += ZombieEvents_OnZombieDamaged;
                soundEvents.OnZombieDamagedCritical += ZombieEvents_OnZombieDamagedCritical;
                soundEvents.OnZombieKilled += SoundEvents_OnZombieKilled;
            }
        }

        private void SoundEvents_OnZombieKilled(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(killedZombie);
        }

        private void ZombieEvents_OnZombieAttack(object sender, EventArgs e)
        {
            if (attackZombie != null)
                audioSource.PlayOneShot(attackZombie);
        }

        private void ZombieEvents_OnZombieDamagedCritical(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(damageZombie);
        }

        private void ZombieEvents_OnZombieDamaged(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(damageZombie);
        }

        private void PlayerEvents_OnPlayerDamagedCritical(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(damagePlayer);
        }

        private void PlayerEvents_OnPlayerDamaged(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(damagePlayer);
        }

        private void PlayerEvents_OnPlayerAttack(object sender, EventArgs e)
        {
            audioSource.PlayOneShot(attackPlayer);
        }

        private void OnDisable()
        {
            if (soundEvents != null)
            {
                soundEvents.OnPlayerAttack -= PlayerEvents_OnPlayerAttack;
                soundEvents.OnPlayerDamaged -= PlayerEvents_OnPlayerDamaged;
                soundEvents.OnPlayerDamagedCritical -= PlayerEvents_OnPlayerDamagedCritical;

                soundEvents.OnZombieAttack -= ZombieEvents_OnZombieAttack;
                soundEvents.OnZombieDamaged -= ZombieEvents_OnZombieDamaged;
                soundEvents.OnZombieDamagedCritical -= ZombieEvents_OnZombieDamagedCritical;
                soundEvents.OnZombieKilled -= SoundEvents_OnZombieKilled;
            }

        }
    }
}