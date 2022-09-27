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
                WeaponType.Minigun => PerkType.Minigun,
                WeaponType.Shuriken => PerkType.Shuriken,
                _ => PerkType.NA,
            };
        }
        public static PerkType PerkForWallmart(WallmartItem wallmartItem)
        {
            return wallmartItem switch
            {
                WallmartItem.Minigun => PerkType.Minigun,
                WallmartItem.Shuriken => PerkType.Shuriken,
                WallmartItem.Shield => PerkType.Shield,
                WallmartItem.Power => PerkType.Power,
                _ => PerkType.NA,
            };
        }

        public int CurrentPerk(PerkType perkType) =>
            PlayerPrefs.GetInt(PerkKey(perkType));

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

        public int ShurikenLevel => CurrentPerk(PerkForWeapon(WeaponType.Shuriken));
        public bool ShurikenUnlocked =>  ShurikenLevel > 0;

        public int MinigunLevel => CurrentPerk(PerkForWeapon(WeaponType.Minigun));
        public bool MinigunUnlocked => MinigunLevel > 0;

        public int ShieldLevel => CurrentPerk(PerkType.Shield);
        public bool ShieldUnlocked => ShieldLevel > 0;
    }
}
