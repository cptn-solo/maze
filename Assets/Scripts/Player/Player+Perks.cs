using System.Linq;

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
            if (arg1 == PerkType.Minigun && Perks.MinigunLevel is int mgLevel && mgLevel > 0 && minigun != null)
            {
                minigun.PerkDamage = PerkDamage(WeaponType.Minigun, mgLevel);
                if (currentWeapon == WeaponType.Minigun)
                    PerkRateOfFire = PerkROF(currentWeapon, mgLevel);
            }

            if (arg1 == PerkType.Shuriken && Perks.ShurikenLevel is int shLevel && shLevel > 0 && shell != null)
            {
                shell.PerkDamage = PerkDamage(WeaponType.Shuriken, shLevel);
                if (currentWeapon == WeaponType.Shuriken)
                    PerkRateOfFire = PerkROF(currentWeapon, shLevel);
            }

            if (arg1 == PerkType.Shield && Perks.ShieldLevel is int sldLevel && sldLevel > 0 && hitbox != null)
            {
                hitbox.PerkMaxHP = PerkShield(PlayerPerk.HP, sldLevel);
                hitbox.PerkMaxShield = PerkShield(PlayerPerk.Shield, sldLevel);
                hitbox.ResetHP();
            }
        }
        private int PerkShield(PlayerPerk perk, int level) =>
            ShieldPerks.PerkForLevel(level)
                .PlayerPerks.Where(x => x.Key == perk)
                .Select(x => x.Value).FirstOrDefault();

        private int PerkDamage(WeaponType weapon, int level) => weapon switch
            {
                WeaponType.Shuriken =>
                    ShurikenPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x=>x.Key == WeaponPerk.HPDamage)
                    .Select(x => x.Value).FirstOrDefault(),
                WeaponType.Minigun =>
                    MinigunPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.HPDamage)
                    .Select(x => x.Value).FirstOrDefault(),
                _ => 0
            };

        private int PerkROF(WeaponType weapon, int level) => weapon switch
            {
                WeaponType.Shuriken =>
                    ShurikenPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.FireRate)
                    .Select(x => x.Value).FirstOrDefault(),
                WeaponType.Minigun =>
                    MinigunPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.FireRate)
                    .Select(x => x.Value).FirstOrDefault(),
                _ => 0
            };

    }
}