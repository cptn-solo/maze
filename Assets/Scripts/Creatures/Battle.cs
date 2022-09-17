using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Battle : MonoBehaviour
    {
        private Hitbox hitbox;
        private MovableUnit unit;

        private void Awake()
        {
            hitbox = GetComponentInChildren<Hitbox>();
            
            unit = GetComponent<MovableUnit>();            
            unit.OnUnitRespawned += Unit_OnUnitRespawned;
        }

        private void Unit_OnUnitRespawned()
        {
            if (hitbox)
                hitbox.ResetHP();
        }

        private void OnEnable()
        {
            if (hitbox != null)
            {
                hitbox.OnDamage += Hitbox_OnDamage;
                hitbox.OnZeroHealthReached += Hitbox_OnZeroHealthReached;
            }
        }

        private void OnDisable()
        {
            if (hitbox != null)
            {
                hitbox.OnDamage -= Hitbox_OnDamage;
                hitbox.OnZeroHealthReached -= Hitbox_OnZeroHealthReached;
            }

        }

        private void Hitbox_OnZeroHealthReached()
        {
        }

        private void Hitbox_OnDamage()
        {

        }

    }
}