using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class ShieldPerks
    {
        public static PerkInfo ShieldPerk0Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 0,
                WeaponType = WeaponType.NA,
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 80),
                    KeyValuePair.Create(PlayerPerk.HP, 50),
                },
            };
        public static PerkInfo ShieldPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 100,
                WeaponType = WeaponType.NA,                
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 100),
                    KeyValuePair.Create(PlayerPerk.HP, 70),
                },
            };
        public static PerkInfo ShieldPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 250,
                WeaponType = WeaponType.NA,
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 120),
                    KeyValuePair.Create(PlayerPerk.HP, 100),
                },
            };
        public static PerkInfo ShieldPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 300,
                WeaponType = WeaponType.NA,
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 150),
                    KeyValuePair.Create(PlayerPerk.HP, 200),
                },
            };
        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => ShieldPerk0Info,
            2 => ShieldPerk1Info,
            3 => ShieldPerk2Info,
            4 => ShieldPerk3Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => ShieldPerk1Info,
            2 => ShieldPerk2Info,
            3 => ShieldPerk3Info,
            _ => default
        };

    }
}
