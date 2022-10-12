using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public partial class Player
    {
        public PlayerPerkService Perks { get; set; }

        public void InitPerkedItems()
        {

            var levels = new Dictionary<PerkType, int>()
            {
                { PerkType.Shield, Perks.ShieldLevel },
                { PerkType.Shuriken, Perks.ShurikenLevel},
                { PerkType.Minigun, Perks.MinigunLevel },
                { PerkType.Shotgun, Perks.ShotgunLevel },
                { PerkType.Uzi, Perks.UziLevel },
            };
            foreach (var perk in levels)
                UpdatePerk(perk.Key, perk.Value);
        }
        public void UpdatePerk(PerkType arg1, int level)
        {
            if (arg1 == PerkType.Minigun && level > 0 && minigun != null)
            {
                minigun.PerkDamage = PerkDamage(WeaponType.Minigun, level);
                if (currentWeapon == WeaponType.Minigun)
                    PerkRateOfFire = PerkROF(currentWeapon, level);
            }
            if (arg1 == PerkType.Shotgun && level > 0 && shotgun != null)
            {
                shotgun.PerkDamage = PerkDamage(WeaponType.Shotgun, level);
                if (currentWeapon == WeaponType.Shotgun)
                    PerkRateOfFire = PerkROF(currentWeapon, level);
            }
            if (arg1 == PerkType.Uzi && level > 0 && uzi != null)
            {
                uzi.PerkDamage = PerkDamage(WeaponType.Uzi, level);
                if (currentWeapon == WeaponType.Uzi)
                    PerkRateOfFire = PerkROF(currentWeapon, level);
            }

            if (arg1 == PerkType.Shuriken && level > 0 && shell != null)
            {
                shell.PerkDamage = PerkDamage(WeaponType.Shuriken, level);
                if (currentWeapon == WeaponType.Shuriken)
                    PerkRateOfFire = PerkROF(currentWeapon, level);
            }

            if (arg1 == PerkType.Shield && level > 0 && hitbox != null)
            {
                hitbox.PerkMaxHP = PerkShield(PlayerPerk.HP, level);
                hitbox.PerkMaxShield = PerkShield(PlayerPerk.Shield, level);
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
                WeaponType.Shotgun =>
                    ShotgunPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.HPDamage)
                    .Select(x => x.Value).FirstOrDefault(),
                WeaponType.Uzi =>
                    UziPerks.PerkForLevel(level)
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
                WeaponType.Shotgun =>
                    ShotgunPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.FireRate)
                    .Select(x => x.Value).FirstOrDefault(),
                WeaponType.Uzi =>
                    UziPerks.PerkForLevel(level)
                    .WeaponPerks.Where(x => x.Key == WeaponPerk.FireRate)
                    .Select(x => x.Value).FirstOrDefault(),
                _ => 0
            };

    }
}