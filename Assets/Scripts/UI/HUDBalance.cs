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
        [SerializeField] private TextMeshProUGUI item1ammo;
        [SerializeField] private TextMeshProUGUI item2ammo;
        [SerializeField] private TextMeshProUGUI stowedWeaponAmmo;
        [SerializeField] private GameObject weapon1image;
        [SerializeField] private GameObject weapon2image;

        private WeaponType currentWeapon = WeaponType.Shuriken;
        private WeaponType stowedWeapon = WeaponType.Minigun;

        private bool ShowAmmo => 
            PlayerBalanceService.CollectableForWeapon(currentWeapon) != CollectableType.NA;

        private GameObject WeaponImageObject(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Shuriken => weapon2image,
                _ => weapon1image,
            };
        }


        public CollectableType CurrentItem1 { get; set; } = CollectableType.Bomb;
        public CollectableType CurrentItem2 { get; set; } = CollectableType.Landmine;
        public WeaponType CurrentWeapon
        {
            get => currentWeapon;
            set 
            {
                stowedWeapon = currentWeapon;
                currentWeapon = value;
                
                ammoIcon.gameObject.SetActive(ShowAmmo);
                ammo.gameObject.SetActive(ShowAmmo);

                WeaponImageObject(stowedWeapon).SetActive(true);
                WeaponImageObject(currentWeapon).SetActive(false);

            }
        }

        public void SetAmmo(int value) =>
            ammo.text = $"{value}";

        public void SetBalance(int value) =>
            balance.text = $"{value}";

        internal void SetItemAmmo(CollectableType arg1, int arg2)
        {
            if (arg1 == CurrentItem1)
                item1ammo.text = $"{arg2}";
            else if (arg1 == CurrentItem2)
                item2ammo.text = $"{arg2}";
        }

        internal void SetStowedAmmo(int stowedCount)
        {
            stowedWeaponAmmo.text = stowedCount >= 0 ? $"{stowedCount}" : "";
        }
    }
}