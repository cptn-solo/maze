using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class UziPerks
    {
        public static PerkInfo UziPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Uzi,
                Price = 150,
                WeaponType = WeaponType.Uzi,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 2),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 2),
                    KeyValuePair.Create(WeaponPerk.FireRate, 12),
                },
            };
        public static PerkInfo UziPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Uzi,
                Price = 300,
                WeaponType = WeaponType.Uzi,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 8),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 4),
                    KeyValuePair.Create(WeaponPerk.FireRate, 10),
                },
            };
        public static PerkInfo UziPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Uzi,
                Price = 1200,
                WeaponType = WeaponType.Uzi,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 10),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 6),
                    KeyValuePair.Create(WeaponPerk.FireRate, 12),
                },
            };
        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {
            1 => UziPerk1Info,
            2 => UziPerk2Info,
            3 => UziPerk3Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {
            0 => UziPerk1Info,
            1 => UziPerk2Info,
            2 => UziPerk3Info,
            _ => default
        };

    }
}
