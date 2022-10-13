using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerPerkService : MonoBehaviour
    {
        public event Action<PerkType, int> OnPerkChanged;

        public static string PerkKey(PerkType perk) =>
            $"{PlayerPreferencesService.PerkKey}_{perk}";
        
        public static PerkType PerkForWeapon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Uzi=> PerkType.Uzi,
                WeaponType.Shotgun => PerkType.Shotgun,
                WeaponType.Minigun => PerkType.Minigun,
                WeaponType.Shuriken => PerkType.Shuriken,
                _ => PerkType.NA,
            };
        }
        public static PerkType PerkForWallmart(WallmartItem wallmartItem)
        {
            return wallmartItem switch
            {
                WallmartItem.Uzi => PerkType.Uzi,
                WallmartItem.Shotgun => PerkType.Shotgun,
                WallmartItem.Minigun => PerkType.Minigun,
                WallmartItem.Shuriken => PerkType.Shuriken,
                WallmartItem.Shield => PerkType.Shield,
                WallmartItem.Power => PerkType.Power,
                _ => PerkType.NA,
            };
        }

        public KeyValuePair<WeaponType, int>[] UnlockedWeapons()
        {
            var list = new List<KeyValuePair<WeaponType, int>>();

            foreach (var wep in new[]{
                WeaponType.Shuriken,
                WeaponType.Uzi,
                WeaponType.Shotgun,
                WeaponType.Minigun,
             })
            {
                var weaponLevel = CurrentPerk(PerkForWeapon(wep));
                if (weaponLevel > 0)
                    list.Add(KeyValuePair.Create(wep, weaponLevel));
            }
            return list.ToArray();
        }

        public int CurrentPerk(PerkType perkType) =>
            PlayerPrefs.GetInt(PerkKey(perkType));
        
        public int CurrentPerk(WallmartItem arg1) =>
            PlayerPrefs.GetInt(PerkKey(PerkForWallmart(arg1)));
        public int CurrentPerk(WeaponType arg1) =>
            PlayerPrefs.GetInt(PerkKey(PerkForWeapon(arg1)));

        public void SetPerk(PerkType perkType, int value)
        {
            PlayerPrefs.SetInt(PerkKey(perkType), value);
            OnPerkChanged?.Invoke(perkType, value);

        }
        public int AddPerk(PerkType perkType, int value)
        {
            var newValue = CurrentPerk(perkType) + value;
            SetPerk(perkType, newValue);
            return newValue;
        }

        internal int AddPerk(WallmartItem item, int v)
        {
            return AddPerk(PerkForWallmart(item), v);
        }
        internal PerkInfo CurrentPerkInfo(PlayerPerk playerPerk)
        {
            var curPerkInfo = playerPerk switch
            {
                PlayerPerk.Shield => ShieldPerks.PerkForLevel(ShieldLevel),
                _ => default
            };

            return curPerkInfo;
        }

        internal PerkInfo CurrentPerkInfo(WeaponType weaponType)
        {
            var curPerkInfo = weaponType switch
            {
                WeaponType.Uzi => UziPerks.PerkForLevel(UziLevel),
                WeaponType.Shotgun => ShotgunPerks.PerkForLevel(ShotgunLevel),
                WeaponType.Minigun => MinigunPerks.PerkForLevel(MinigunLevel),
                _ => ShurikenPerks.PerkForLevel(ShurikenLevel)
            };

            return curPerkInfo;
        }
        public int ShotgunLevel => CurrentPerk(PerkForWeapon(WeaponType.Shotgun));
        public bool ShotgunUnlocked => ShotgunLevel > 0;
        public int UziLevel => CurrentPerk(PerkForWeapon(WeaponType.Uzi));
        public bool UziUnlocked => UziLevel > 0;

        public int ShurikenLevel => CurrentPerk(PerkForWeapon(WeaponType.Shuriken));
        public bool ShurikenUnlocked =>  ShurikenLevel > 0;

        public int MinigunLevel => CurrentPerk(PerkForWeapon(WeaponType.Minigun));
        public bool MinigunUnlocked => MinigunLevel > 0;

        public int ShieldLevel => CurrentPerk(PerkType.Shield);
        public bool ShieldUnlocked => ShieldLevel > 0;

    }
}
