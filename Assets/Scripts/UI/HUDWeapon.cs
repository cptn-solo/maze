using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class HUDWeapon : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI balance;

        private int balanceValue;
        public int Balance { 
            get => balanceValue;
            set
            {
                balanceValue = value;
                balance.text = $"{value}";
            }
        }
    }
}