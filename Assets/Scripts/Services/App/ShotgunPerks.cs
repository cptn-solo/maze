using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class ShotgunPerks
    {
        public static PerkInfo ShotgunPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Shotgun,
                Price = 1000,
                WeaponType = WeaponType.Shotgun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 5),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 30),
                    KeyValuePair.Create(WeaponPerk.FireRate, 1),
                },
            };
        public static PerkInfo ShotgunPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Shotgun,
                Price = 3000,
                WeaponType = WeaponType.Shotgun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 8),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 50),
                    KeyValuePair.Create(WeaponPerk.FireRate, 1),
                },
            };
        public static PerkInfo ShotgunPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Shotgun,
                Price = 10000,
                WeaponType = WeaponType.Shotgun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 12),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 80),
                    KeyValuePair.Create(WeaponPerk.FireRate, 1),
                },
            };
        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {
            1 => ShotgunPerk1Info,
            2 => ShotgunPerk2Info,
            3 => ShotgunPerk3Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {
            0 => ShotgunPerk1Info,
            1 => ShotgunPerk2Info,
            2 => ShotgunPerk3Info,
            _ => default
        };

    }
}
