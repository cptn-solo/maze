using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CardRow : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI valueLabel;

        public Image Icon => icon;
        public TextMeshProUGUI ValueLabel => valueLabel;
        public TextMeshProUGUI NameLabel => nameLabel;

        public PlayerPerk PlayerPerk { get; set; }
        public WeaponPerk WeaponPerk { get; set; }


    }
}