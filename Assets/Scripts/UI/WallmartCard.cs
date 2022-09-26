using Assets.Scripts;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class WallmartCard : MonoBehaviour
{
    [SerializeField] private WallmartItem itemType;

    private Button buyButton;
    private WallmartScreen wallmart;

    public string PlayerId { get; set; }

    private void OnBuyButtonClick()
    {
        wallmart.OnBuyPressed?.Invoke(itemType, PlayerId);
    }

    private void Awake()
    {
        wallmart = GetComponentInParent<WallmartScreen>();
        buyButton = GetComponentInChildren<Button>();
    }

    private void OnEnable()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
    }

    private void OnDisable()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClick);
        PlayerId = null;
    }
}
