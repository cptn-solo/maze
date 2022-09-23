using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class HUDBalance : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balance;

        public int CurrentBalance
        {
            get => PlayerPrefs.GetInt(PlayerPreferencesService.BalanceKey);
            set
            {
                PlayerPrefs.SetInt(PlayerPreferencesService.BalanceKey, value);
                balance.text = $"{CurrentBalance}";
            }
        }

        private void Start()
        {
            balance.text = $"{CurrentBalance}";
        }
    }
}