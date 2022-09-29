using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public partial class Game
    {
        private void InitHUD()
        {
            balance.SetItemAmmo(CollectableType.Bomb, balances.CurrentBalance(CollectableType.Bomb));
            balance.SetItemAmmo(CollectableType.Landmine, balances.CurrentBalance(CollectableType.Landmine));

            var levels = new Dictionary<PerkType, int>() 
            {
                { PerkType.Shield, perks.ShieldLevel },
            };
            var info = new Dictionary<PerkType, PerkInfo>
            {
                { PerkType.Shield, ShieldPerks.PerkForLevel(levels[PerkType.Shield]) },
            };

            balance.SetPlayerInfo(info[PerkType.Shield], balances.CurrentBalance(CollectableType.Coin));

            SwitchHUDWeapon(WeaponType.Shuriken);
        }

        private void SwitchHUDWeapon(WeaponType obj)
        {
            var prevWeapon = balance.CurrentWeapon;
            var prevStowedWeapon = balance.StowedWeapon;

            var levels = new Dictionary<WeaponType, int>() {
                { WeaponType.Shuriken, perks.ShurikenLevel },
                { WeaponType.Minigun, perks.MinigunLevel },
            };
            var info = new Dictionary<WeaponType, PerkInfo>
            {
                { WeaponType.Shuriken, ShurikenPerks.PerkForLevel(levels[WeaponType.Shuriken]) },
                { WeaponType.Minigun, MinigunPerks.PerkForLevel(levels[WeaponType.Minigun]) },
            };

            balance.SetCurrentWeaponInfo(info[obj], balances.CurrentBalance(obj));

            var stowedWeapon = (prevWeapon == WeaponType.NA && 
                levels.FirstOrDefault(x => x.Value > 0 && x.Key != obj)
                    is KeyValuePair<WeaponType, int> option) ? option.Key : prevWeapon;

            balance.StowedWeapon = stowedWeapon;
            balance.SetStowedAmmo(balances.CurrentBalance(stowedWeapon));
        }

        private void UpdateHUDPerk(PerkType perk, int level)
        {
            switch (perk)
            {
                case PerkType.Shuriken:
                    if (WeaponType.Shuriken != balance.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Shuriken);
                    else
                        balance.SetCurrentWeaponInfo(
                            ShurikenPerks.PerkForLevel(level),
                            balances.CurrentBalance(WeaponType.Shuriken));
                    break;
                case PerkType.Shield:
                    balance.SetPlayerInfo(
                        ShieldPerks.PerkForLevel(level),
                        balances.CurrentBalance(CollectableType.Coin));
                    break;
                case PerkType.Minigun:
                    if (WeaponType.Minigun != balance.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Minigun);
                    else
                        balance.SetCurrentWeaponInfo(
                            MinigunPerks.PerkForLevel(level),
                            balances.CurrentBalance(WeaponType.Minigun));
                    break;
                default:
                    break;

            };
        }

        private void UpdateHUDBalances(CollectableType arg1, int arg2)
        {
            if (arg1 == CollectableType.Coin)
                balance.SetBalance(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(balance.CurrentWeapon) == arg1)
                balance.SetAmmo(arg2);
            else if (PlayerBalanceService.CollectableForWeapon(balance.StowedWeapon) == arg1)
                balance.SetStowedAmmo(arg2);
            else
                balance.SetItemAmmo(arg1, arg2);
        }
    }
}