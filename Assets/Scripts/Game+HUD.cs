namespace Assets.Scripts
{
    public partial class Game
    {
        private void InitHUD()
        {
            balance.SetBalance(balances.CurrentBalance(CollectableType.Coin));
            balance.CurrentWeapon = WeaponType.Shuriken;
            if (perks.MinigunUnlocked)
            {
                balance.StowedWeapon = WeaponType.Minigun;
                balance.SetStowedAmmo(balances.CurrentBalance(CollectableType.Minigun));
            }
            else
            {
                balance.StowedWeapon = WeaponType.NA;
                balance.SetStowedAmmo(-1);
            }
            balance.SetItemAmmo(CollectableType.Bomb, balances.CurrentBalance(CollectableType.Bomb));
            balance.SetItemAmmo(CollectableType.Landmine, balances.CurrentBalance(CollectableType.Landmine));
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

        private void SwitchHUDWeapon(WeaponType obj)
        {
            var stowedWeapon = balance.CurrentWeapon;
            balance.CurrentWeapon = obj;
            var ammoCollectable = PlayerBalanceService.CollectableForWeapon(balance.CurrentWeapon);
            if (ammoCollectable != CollectableType.NA)
                balance.SetAmmo(balances.CurrentBalance(ammoCollectable));

            var stowedAmmoCollectable = PlayerBalanceService.CollectableForWeapon(stowedWeapon);
            var stowedCount = stowedAmmoCollectable != CollectableType.NA ?
                balances.CurrentBalance(stowedAmmoCollectable) :
                -1;
            balance.SetStowedAmmo(stowedCount);
        }

        private void UpdateHUDPerk(PerkType perk)
        {
            switch (perk)
            {
                case PerkType.Shuriken:
                    break;
                case PerkType.Shield:
                    break;
                case PerkType.Minigun:
                    {
                        if (perks.MinigunUnlocked)
                            player.SelectWeapon(WeaponType.Minigun);
                    }
                    break;
                default:
                    break;

            };
        }



    }
}