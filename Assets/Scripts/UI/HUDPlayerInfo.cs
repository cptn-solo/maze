using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDPlayerInfo : MonoBehaviour
    {
        [SerializeField] private HUDPerk[] perksViews;
        [SerializeField] private HUDPerk balanceView;
        [SerializeField] private HUDPerk coninXView;

        public PerkInfo CurrentPerk
        { 
            set
            {
                foreach (var item in perksViews)
                {
                    var rawValue = value.PlayerPerks.FirstOrDefault(x => x.Key == item.PlayerPerk);
                    item.ValueLabel.text = rawValue.Equals(default) ? "" : $"{rawValue.Value}";
                }
            }
        }
        public int CurrenCoinX
        {
            set
            {
                coninXView.gameObject.SetActive(value > 0);
                coninXView.ValueLabel.text = $"{value}";
            }
        }
        public int CurrenBalance
        {
            set
            {
                balanceView.ValueLabel.text = $"{value}";
            }
        }
    }
}