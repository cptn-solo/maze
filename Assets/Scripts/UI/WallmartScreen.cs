using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WallmartScreen : MonoBehaviour
    {
        internal Action<WallmartItem, string, PerkInfo> OnBuyPressed;
        internal Action OnCancelPressed;
        
        [SerializeField] private GameObject[] items;
        [SerializeField] private WallmartItem[] itemIndexes;

        [SerializeField] private GameObject successView;
        [SerializeField] private GameObject failureView;

        public PlayerPerkService Perks { get; set; }

        public void ShowItemCard(PerkInfo info, string playerId, int playerBalance)
        {
            var itemIndex = Array.IndexOf(itemIndexes, info.WallmartItem);
            var item = items[itemIndex];

            item.SetActive(true);
            var card = item.GetComponent<WallmartCard>();
            card.SetPurchaseInfo(playerId, info, info.Price <= playerBalance);
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