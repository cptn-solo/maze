using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HUDWeapon : HUDItem
    {
        [SerializeField] private Image shuriken;
        [SerializeField] private Image minigun;
        [SerializeField] private Image shotgun;
        [SerializeField] private Image uzi;
        public void SetActiveWeapon(WeaponType weaponType)
        {
            shuriken.gameObject.SetActive(weaponType == WeaponType.Shuriken);
            minigun.gameObject.SetActive(weaponType == WeaponType.Minigun);
            shotgun.gameObject.SetActive(weaponType == WeaponType.Shotgun);
            uzi.gameObject.SetActive(weaponType == WeaponType.Uzi);
        }

    }
}