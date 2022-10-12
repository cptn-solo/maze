using System.Collections.Generic;

namespace Assets.Scripts
{
    public enum PerkType
    {
        NA,
        Shield,
        Power,
        Shuriken,
        Minigun,
        Shotgun,
        Uzi,
    }

    public enum PlayerPerk
    {
        NA,
        Shield,
        HP,
        Power,
    }

    public enum WeaponPerk
    {
        NA,
        ShieldDamage,
        HPDamage,
        Weight,
        FireRate,
    }

    public struct PerkInfo
    {
        public WallmartItem WallmartItem;
        public WeaponType WeaponType;

        public int Price;

        public KeyValuePair<PlayerPerk, int>[] PlayerPerks;
        public KeyValuePair<WeaponPerk, int>[] WeaponPerks;
    }


}
