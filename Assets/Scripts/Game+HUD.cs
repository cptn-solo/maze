namespace Assets.Scripts
{
    public partial class Game
    {
        private void InitHUD()
        {
            balance.SetCoinX(balances.CurrentCoinX);

            balance.SetItemAmmo(CollectableType.Bomb, balances.CurrentBalance(CollectableType.Bomb));
            balance.SetItemAmmo(CollectableType.Landmine, balances.CurrentBalance(CollectableType.Landmine));

            balance.SetPlayerInfo(
                perks.CurrentPerkInfo(PlayerPerk.Shield),
                balances.CurrentBalance(CollectableType.Coin));

            UpdateHUDWeapon(player.CurrentWeapon, player.StowedWeapon);
        }

        private void UpdateHUDWeapon(WeaponType current, WeaponType stowed)
        {
            balance.SetCurrentWeaponInfo(
                perks.CurrentPerkInfo(current), 
                balances.CurrentBalance(current));
            balance.StowedWeapon = stowed;
            balance.SetStowedAmmo(balances.CurrentBalance(stowed));
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
                case PerkType.Shotgun:
                    if (WeaponType.Shotgun != balance.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Shotgun);
                    else
                        balance.SetCurrentWeaponInfo(
                            ShotgunPerks.PerkForLevel(level),
                            balances.CurrentBalance(WeaponType.Shotgun));
                    break;
                case PerkType.Uzi:
                    if (WeaponType.Uzi != balance.CurrentWeapon)
                        player.SelectWeapon(WeaponType.Uzi);
                    else
                        balance.SetCurrentWeaponInfo(
                            UziPerks.PerkForLevel(level),
                            balances.CurrentBalance(WeaponType.Uzi));
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