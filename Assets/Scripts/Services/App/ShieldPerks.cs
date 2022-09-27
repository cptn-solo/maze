using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class ShieldPerks
    {
        public static PerkInfo ShieldPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 20,
                WeaponType = WeaponType.NA,                
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 100),
                    KeyValuePair.Create(PlayerPerk.HP, 100),
                },
            };
        public static PerkInfo ShieldPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Shield,
                Price = 150,
                WeaponType = WeaponType.NA,
                PlayerPerks = new[]
                {
                    KeyValuePair.Create(PlayerPerk.Shield, 130),
                    KeyValuePair.Create(PlayerPerk.HP, 150),
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
        public static PerkInfo PerkForWallmartItem(WallmartItem arg1, int currentPerk)
            => arg1 switch
            {
                WallmartItem.Shield => currentPerk switch
                {   // stock is 1 so start from 1
                    1 => ShieldPerk1Info,
                    2 => ShieldPerk2Info,
                    3 => ShieldPerk3Info,
                    _ => default
                },
                _ => default
            };

    }
}
