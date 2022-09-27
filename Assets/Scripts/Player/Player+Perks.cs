namespace Assets.Scripts
{
    public partial class Player
    {
        public PlayerPerkService Perks { get; set; }

        public void InitPerkedItems()
        {
            foreach (var perk in new[] {
                PerkType.Minigun,
                PerkType.Shield,
                PerkType.Shuriken,
            })
                UpdatePerk(perk);
        }
        public void UpdatePerk(PerkType arg1)
        {
            if (arg1 == PerkType.Minigun && Perks.MinigunUnlocked && minigun != null)
                minigun.PerkAddedDamage = PerkAddedDamage(WeaponType.Minigun, Perks.MinigunLevel);

            if (arg1 == PerkType.Shuriken && Perks.ShurikenUnlocked && shell != null)
                shell.PerkAddedDamage = PerkAddedDamage(WeaponType.Shuriken, Perks.ShurikenLevel);
        }

        private int PerkAddedDamage(WeaponType weapon, int level)
        {
            return weapon switch
            {
                WeaponType.Shuriken =>
                level switch
                {
                    1 => 1,
                    2 => 2,
                    3 => 4,
                    4 => 5,
                    5 => 7,
                    6 => 8,
                    _ => 0
                },
                WeaponType.Minigun =>
                level switch
                {
                    1 => 3,
                    2 => 4,
                    3 => 6,
                    _ => 0
                },
                _ => 0
            };
        }

    }
}