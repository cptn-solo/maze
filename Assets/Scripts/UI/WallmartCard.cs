using Assets.Scripts;
using Assets.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WallmartCard : MonoBehaviour
{
    [SerializeField] private WallmartItem itemType;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI priceLabel;

    private WallmartScreen wallmart;

    private string playerId;
    private int price;
    public void SetPurchaseEnabled(string playerId, int price, bool enabled) {
        this.playerId = playerId;
        this.price = price;

        priceLabel.color = enabled ? 
            Color.green :
            Color.red;
        priceLabel.text = $"{price}";

        buyButton.gameObject.SetActive(enabled);
        cancelButton.gameObject.SetActive(!enabled);
    }

    private void OnBuyButtonClick()
    {
        wallmart.OnBuyPressed?.Invoke(itemType, playerId, price);
    }

    private void OnCancelButtonClick()
    {
        wallmart.OnCancelPressed?.Invoke();
    }

    private void Awake()
    {
        wallmart = GetComponentInParent<WallmartScreen>();
    }

    private void OnEnable()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    private void OnDisable()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClick);
        cancelButton.onClick.RemoveListener(OnCancelButtonClick);

        playerId = null;
        price = 0;
    }
}
