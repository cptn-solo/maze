using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WallmartScreen : MonoBehaviour
    {
        internal Action<WallmartItem, string, int> OnBuyPressed;
        internal Action OnCancelPressed;
        
        [SerializeField] private GameObject[] items;
        [SerializeField] private int[] itemsPrices;
        [SerializeField] private WallmartItem[] itemIndexes;

        [SerializeField] private GameObject successView;
        [SerializeField] private GameObject failureView;

        public void ShowItemCard(WallmartItem cardType, string playerId, int playerBalance)
        {
            var itemIndex = Array.IndexOf(itemIndexes, cardType);
            var item = items[itemIndex];
            var price = itemsPrices[itemIndex];

            item.SetActive(true);
            var card = item.GetComponent<WallmartCard>();
            card.SetPurchaseEnabled(playerId, price, price <= playerBalance);
        }

        public void HideActiveCard()
        {
            foreach (var card in items)
                card.SetActive(false);
        }

        internal void CompletePurchase(bool success)
        {
            HideActiveCard();
            StartCoroutine(ShowFinalView(success));
        }

        private IEnumerator ShowFinalView(bool success)
        {
            successView.SetActive(success);
            failureView.SetActive(!success);

            yield return new WaitForSeconds(2.0f);

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            successView.SetActive(false);
            failureView.SetActive(false);

            foreach (var card in items)
                card.SetActive(false);

        }
    }
}