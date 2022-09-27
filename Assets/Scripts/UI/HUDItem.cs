using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HUDItem : MonoBehaviour
    {
        [SerializeField] private Image bomb;
        [SerializeField] private Image landmine;

        [SerializeField] private TextMeshProUGUI balance;
        [SerializeField] protected bool showBalance = true;


        public void SetActiveItem(CollectableType collectableType)
        {
            bomb.gameObject.SetActive(collectableType == CollectableType.Bomb);
            landmine.gameObject.SetActive(collectableType == CollectableType.Landmine);
        }

        private void OnEnable()
        {
            balance.gameObject.SetActive(showBalance);
        }

        private int balanceValue;
        public int Balance { 
            get => balanceValue;
            set
            {
                balanceValue = value;
                balance.text = value >= 0 ? $"{value}" : "";
            }
        }
    }
}