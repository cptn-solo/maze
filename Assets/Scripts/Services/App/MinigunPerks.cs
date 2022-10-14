using System.Collections.Generic;

namespace Assets.Scripts
{

    public static class MinigunPerks
    {
        public static PerkInfo MinigunPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Minigun,
                Price = 500,
                WeaponType = WeaponType.Minigun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 1),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 5),
                    KeyValuePair.Create(WeaponPerk.FireRate, 7),
                },
            };
        public static PerkInfo MinigunPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Minigun,
                Price = 1000,
                WeaponType = WeaponType.Minigun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 2),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 6),
                    KeyValuePair.Create(WeaponPerk.FireRate, 8),
                },
            };
        public static PerkInfo MinigunPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Minigun,
                Price = 5000,
                WeaponType = WeaponType.Minigun,
                WeaponPerks = new[]
                {
                    KeyValuePair.Create(WeaponPerk.ShieldDamage, 3),
                    KeyValuePair.Create(WeaponPerk.HPDamage, 10),
                    KeyValuePair.Create(WeaponPerk.FireRate, 8),
                },
            };
        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {
            1 => MinigunPerk1Info,
            2 => MinigunPerk2Info,
            3 => MinigunPerk3Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {
            0 => MinigunPerk1Info,
            1 => MinigunPerk2Info,
            2 => MinigunPerk3Info,
            _ => default
        };
    }
}
