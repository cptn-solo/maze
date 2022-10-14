using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts
{
    public class PlayerBalanceService : MonoBehaviour
    {
        public event Action<CollectableType, int> OnBalanceChanged;
        public event Action<int> OnPlayerScoreChanged;
        public event Action<int> OnEnemyScoreChanged;
        
        private int playerScore = 0;
        private int enemyScore = 0;
        public int PlayerScore {
            get => playerScore;
            set
            {
                playerScore = value;
                OnPlayerScoreChanged?.Invoke(playerScore);
            }
        }// zombies eliminated by player
        public int EnemyScore {
            get => enemyScore;
            set
            {
                enemyScore = value;
                OnEnemyScoreChanged?.Invoke(enemyScore);
            }

        }// player eliminated by zombies and other npc

        public int CurrentCoinX =>
            1 + Mathf.FloorToInt((PlayerScore / (1 + EnemyScore * .2f)) * .1f);

        public static CollectableType CollectableForWeapon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Minigun => CollectableType.Minigun,
                WeaponType.Shotgun => CollectableType.Minigun,
                WeaponType.Uzi => CollectableType.Minigun,
                _ => CollectableType.NA,
            };
        }

        public static string BalanceKey(CollectableType collectable) =>
            $"{PlayerPreferencesService.BalanceKey}_{collectable}";


        public int CurrentBalance(CollectableType collectableType) => 
            collectableType != CollectableType.NA ?
                PlayerPrefs.GetInt(BalanceKey(collectableType)) : 
                -1;

        public int CurrentBalance(WeaponType weaponType) =>
            weaponType != WeaponType.NA ?
                CurrentBalance(CollectableForWeapon(weaponType)) :
                -1;

        public int AddBalance(CollectableType collectableType, int count, bool applyX = true)
        {
            if (applyX && collectableType == CollectableType.Coin)
                count *= CurrentCoinX;

            var balance = CurrentBalance(collectableType) + count;
            SetBalance(collectableType, balance);
            return balance;
        }

        public void SetBalance(CollectableType collectableType, int balance)
        {
            PlayerPrefs.SetInt(BalanceKey(collectableType), balance);
            OnBalanceChanged?.Invoke(collectableType, balance);
        }
    }
}
