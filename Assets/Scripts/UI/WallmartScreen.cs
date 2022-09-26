using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WallmartScreen : MonoBehaviour
    {
        internal Action<WallmartItem, string> OnBuyPressed;

        [SerializeField] private GameObject[] items;
        [SerializeField] private WallmartItem[] itemIndexes;

        [SerializeField] private GameObject successView;
        [SerializeField] private GameObject failureView;

        public void ShowItemCard(WallmartItem card, string playerId)
        {
            var item = items[Array.IndexOf(itemIndexes, card)];
            item.SetActive(true);
            item.GetComponent<WallmartCard>().PlayerId = playerId;
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