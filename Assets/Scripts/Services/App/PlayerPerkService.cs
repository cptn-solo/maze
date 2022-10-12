using System;
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
