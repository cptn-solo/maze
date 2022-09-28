using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDPerk : MonoBehaviour
    {
        [SerializeField] private WeaponPerk weaponPerk;
        [SerializeField] private PlayerPerk playerPerk;

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI valueLabel;

        public Image Icon => icon;
        public TextMeshProUGUI ValueLabel => valueLabel;

        public WeaponPerk WeaponPerk => weaponPerk;
        public PlayerPerk PlayerPerk => playerPerk;

    }
}