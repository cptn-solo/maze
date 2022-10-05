using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class ShurikenPerks
    {
        public static PerkInfo ShurikenPerk0Info
            => new()
            {
                WallmartItem = WallmartItem.Shuriken,
                Price = 0,
                WeaponType = WeaponType.Shuriken,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 1),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 2),
                    KeyValuePair.Create(WeaponPerk.FireRate, 4),
                },
            };

        public static PerkInfo ShurikenPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Shuriken,
                Price = 50,
                WeaponType = WeaponType.Shuriken,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 2),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 3),
                    KeyValuePair.Create(WeaponPerk.FireRate, 5),
                },
            };
        public static PerkInfo ShurikenPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Shuriken,
                Price = 100,
                WeaponType = WeaponType.Shuriken,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 2),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 6),
                    KeyValuePair.Create(WeaponPerk.FireRate, 4),
                },
            };
        public static PerkInfo ShurikenPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Shuriken,
                Price = 300,
                WeaponType = WeaponType.Shuriken,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 3),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 10),
                    KeyValuePair.Create(WeaponPerk.FireRate, 4),
                },
            };
        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => ShurikenPerk0Info,
            2 => ShurikenPerk1Info,
            3 => ShurikenPerk2Info,
            4 => ShurikenPerk3Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => ShurikenPerk1Info,
            2 => ShurikenPerk2Info,
            3 => ShurikenPerk3Info,
            _ => default
        };
    }
}
