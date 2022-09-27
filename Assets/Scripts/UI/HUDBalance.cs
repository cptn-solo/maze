using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HUDBalance : MonoBehaviour
    {
        // current coins and current weapon balances:
        [SerializeField] private TextMeshProUGUI balance;
        [SerializeField] private TextMeshProUGUI ammo;
        [SerializeField] private Image ammoIcon;

        // usable items and stowed weapon balances:
        [SerializeField] private HUDWeapon stowedWeapon;
        [SerializeField] private HUDWeapon currentWeapon;
        [SerializeField] private HUDItem bombItem;
        [SerializeField] private HUDItem landmineItem;

        private WeaponType currentWeaponType = WeaponType.Shuriken;
        private WeaponType stowedWeaponType = WeaponType.NA;

        private bool ShowAmmo => 
            PlayerBalanceService.CollectableForWeapon(currentWeaponType) != CollectableType.NA;

        public WeaponType StowedWeapon
        {
            get => stowedWeaponType;
            set
            {
                stowedWeaponType = value;
                stowedWeapon.SetActiveWeapon(stowedWeaponType);
                stowedWeapon.gameObject.SetActive(stowedWeaponType != WeaponType.NA);                    
            }
        } 
        public WeaponType CurrentWeapon
        {
            get => currentWeaponType;
            set 
            {
                if (currentWeaponType != value)
                {
                    StowedWeapon = currentWeaponType;
                    currentWeaponType = value;
                }

                ammoIcon.gameObject.SetActive(ShowAmmo);
                ammo.gameObject.SetActive(ShowAmmo);

                currentWeapon.SetActiveWeapon(currentWeaponType);
            }
        }

        public void SetAmmo(int value) =>
            ammo.text = $"{value}";

        public void SetBalance(int value) =>
            balance.text = $"{value}";

        internal void SetItemAmmo(CollectableType arg1, int arg2)
        {
            if (arg1 == CollectableType.Bomb)
                bombItem.Balance = arg2;
            else if (arg1 == CollectableType.Landmine)
                landmineItem.Balance = arg2;
        }

        internal void SetStowedAmmo(int stowedCount)
        {
            stowedWeapon.Balance = stowedCount;
        }
    }
}