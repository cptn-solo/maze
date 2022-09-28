using Assets.Scripts.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HUDBalance : MonoBehaviour
    {
        [SerializeField] private HUDPlayerInfo playerInfoView;
        [SerializeField] private HUDCurrentWeapon currentWeaponView;
        
        // usable items and stowed weapon balances:
        [SerializeField] private HUDWeapon stowedWeapon;
        [SerializeField] private HUDWeapon currentWeapon;
        [SerializeField] private HUDItem bombItem;
        [SerializeField] private HUDItem landmineItem;

        private WeaponType currentWeaponType = WeaponType.NA;
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
                currentWeaponType = value;
                currentWeapon.SetActiveWeapon(currentWeaponType);
            }
        }

        public void SetAmmo(int value) =>
            currentWeaponView.CurrentWeaponAmmo = value;

        public void SetBalance(int value) =>
            playerInfoView.CurrenBalance = value;

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

        internal void SetPlayerInfo(PerkInfo perkInfo, int balance = 0)
        {
            playerInfoView.CurrentPerk = perkInfo;
            playerInfoView.CurrenBalance = balance;
        }

        internal void SetCurrentWeaponInfo(PerkInfo perkInfo, int ammo = -1)
        {
            CurrentWeapon = perkInfo.WeaponType;
            currentWeaponView.CurrentPerk = perkInfo;
            currentWeaponView.CurrentWeaponAmmo = ammo;
        }
    }
}