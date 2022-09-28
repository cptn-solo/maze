using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{

    public class HUDCurrentWeapon : MonoBehaviour
    {
        [SerializeField] private HUDPerk[] perksViews;
        [SerializeField] private HUDPerk ammoView;

        public PerkInfo CurrentPerk
        {
            set
            {
                foreach (var item in perksViews)
                {
                    var rawValue = value.WeaponPerks.FirstOrDefault(x => x.Key == item.WeaponPerk);
                    item.ValueLabel.text = rawValue.Equals(default) ? "" : $"{rawValue.Value}";
                }
            }
        }

        public int CurrentWeaponAmmo
        { 
            set
            {
                ammoView.ValueLabel.text = value >= 0 ? $"{value}" : "";
                ammoView.Icon.gameObject.SetActive(value >= 0);
            } 
        }
    }
}