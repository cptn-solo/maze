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
    [SerializeField] private TextMeshProUGUI titleLabel;
    [SerializeField] private CardRow[] cardRows;

    private const string SpriteShield = "shield";
    private const string SpriteHP = "hp";
    private const string SpriteBullet = "bullet";
    private const string SpritePower = "power";

    private const string CaptionShield = "Shield";
    private const string CaptionHP = "HP";
    private const string CaptionROF = "Rate of fire/s";
    private const string CaptionPower = "Power";

    private WallmartScreen wallmart;

    private string playerId;
    
    private PerkInfo perkInfo;

    public void SetPurchaseInfo(string playerId, PerkInfo info, bool enabled) {
        this.playerId = playerId;
        perkInfo = info;

        priceLabel.color = enabled ? 
            Color.green :
            Color.red;
        priceLabel.text = $"{perkInfo.Price}";

        if (perkInfo.WeaponPerks != null)
            for (int i = 0; i < perkInfo.WeaponPerks.Length; i++)
            {
                var perk = perkInfo.WeaponPerks[i];
                var icon = Resources.Load<Sprite>(SpriteNameForWeaponPerk(perk.Key));
                var caption = CaptionForWeaponPerk(perk.Key);
                var row = cardRows[i];

                SetRowValues(perk.Value, icon, caption, row);
            }
        
        if (perkInfo.PlayerPerks != null)
            for (int i = 0; i < perkInfo.PlayerPerks.Length; i++)
            {
                var perk = perkInfo.PlayerPerks[i];
                var icon = Resources.Load<Sprite>(SpriteNameForPlayerPerk(perk.Key));
                var caption = CaptionForPlayerPerk(perk.Key);
                var row = cardRows[i];

                SetRowValues(perk.Value, icon, caption, row);
            }

        buyButton.gameObject.SetActive(enabled);
        cancelButton.gameObject.SetActive(!enabled);
    }

    private static void SetRowValues(int value, Sprite icon, string caption, CardRow row)
    {
        row.gameObject.SetActive(true);

        row.Icon.sprite = icon;
        row.NameLabel.text = caption;
        row.ValueLabel.text = $"{value}";
    }

    private string CaptionForWeaponPerk(WeaponPerk key) =>
        key switch
        {
            WeaponPerk.ShieldDamage => CaptionShield,
            WeaponPerk.HPDamage => CaptionHP,
            WeaponPerk.Weight => CaptionPower,
            WeaponPerk.FireRate => CaptionROF,
            _ => null
        };

    private string SpriteNameForWeaponPerk(WeaponPerk key) =>
        key switch
        {
            WeaponPerk.ShieldDamage => SpriteShield,
            WeaponPerk.HPDamage => SpriteHP,
            WeaponPerk.Weight => SpritePower,
            WeaponPerk.FireRate => SpriteBullet,
            _ => null
        };

    private string CaptionForPlayerPerk(PlayerPerk key) =>
        key switch
        {
            PlayerPerk.Shield => CaptionShield,
            PlayerPerk.HP => CaptionHP,
            PlayerPerk.Power => CaptionPower,
            _ => null
        };

    private string SpriteNameForPlayerPerk(PlayerPerk key) =>
        key switch
        {
            PlayerPerk.Shield => SpriteShield,
            PlayerPerk.HP => SpriteHP,
            PlayerPerk.Power => SpritePower,
            _ => null
        };

    private void OnBuyButtonClick() =>
        wallmart.OnBuyPressed?.Invoke(itemType, playerId, perkInfo);

    private void OnCancelButtonClick() =>
        wallmart.OnCancelPressed?.Invoke();

    private void Awake() =>
        wallmart = GetComponentInParent<WallmartScreen>();        

    private void OnEnable()
    {
        buyButton.onClick.AddListener(OnBuyButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    private void OnDisable()
    {
        buyButton.onClick.RemoveListener(OnBuyButtonClick);
        cancelButton.onClick.RemoveListener(OnCancelButtonClick);

        foreach (var row in cardRows)
            row.gameObject.SetActive(false);

        playerId = null;
        perkInfo = default;
    }
}
