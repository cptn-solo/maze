namespace Assets.Scripts
{
    public static class LevelPerks
    {
        public static PerkInfo LevelPerk1Info
            => new()
            {
                WallmartItem = WallmartItem.Level,
                LevelId = 1,
                Price = 0,
            };
        public static PerkInfo LevelPerk2Info
            => new()
            {
                WallmartItem = WallmartItem.Level,
                LevelId = 2,
                Price = 10000,
            };
        public static PerkInfo LevelPerk3Info
            => new()
            {
                WallmartItem = WallmartItem.Level,
                LevelId = 3,
                Price = 20000,
            };
        public static PerkInfo LevelPerk4Info
            => new()
            {
                WallmartItem = WallmartItem.Level,
                LevelId = 4,
                Price = 50000,
            };
        public static PerkInfo LevelPerk5Info
            => new()
            {
                WallmartItem = WallmartItem.Level,
                LevelId = 5,
                Price = 100000,
            };

        public static PerkInfo PerkForLevel(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => LevelPerk1Info,
            2 => LevelPerk2Info,
            3 => LevelPerk3Info,
            4 => LevelPerk4Info,
            5 => LevelPerk5Info,
            _ => default
        };

        public static PerkInfo PerkForWallmartItem(int currentPerk) => currentPerk switch
        {   // stock is 1 so start from 1
            1 => LevelPerk1Info,
            2 => LevelPerk2Info,
            3 => LevelPerk3Info,
            4 => LevelPerk4Info,
            5 => LevelPerk5Info,
            _ => default
        };

    }
}
