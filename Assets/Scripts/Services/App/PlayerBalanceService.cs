using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerBalanceService : MonoBehaviour
    {
        public event Action<CollectableType, int> OnBalanceChanged;

        public static CollectableType CollectableForWeapon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Minigun => CollectableType.Minigun,
                _ => CollectableType.NA,
            };
        }

        public static string BalanceKey(CollectableType collectable) =>
            $"{PlayerPreferencesService.BalanceKey}_{collectable}";

        public static string BalanceKey(WeaponType weapon) =>
            $"{PlayerPreferencesService.BalanceKey}_{CollectableForWeapon(weapon)}";


        public int CurrentBalance(CollectableType collectableType) =>
            PlayerPrefs.GetInt(BalanceKey(collectableType));
        
        public void AddBalance(CollectableType collectableType, int count)
        {
            var balance = CurrentBalance(collectableType) + count;
            SetBalance(collectableType, balance);
        }

        public void SetBalance(CollectableType collectableType, int balance)
        {
            PlayerPrefs.SetInt(BalanceKey(collectableType), balance);
            OnBalanceChanged?.Invoke(collectableType, balance);
        }
    }
}
