using UnityEngine;

namespace Assets.Scripts
{

    public class Wallmart : MonoBehaviour
    {
        [SerializeField] protected WallmartItem itemType;
        [SerializeField] private LayerMask buyerMask;

        protected virtual void ShowUIWallmart(Player player) =>
            player.ShowWallmart(itemType);

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(buyerMask) && other.TryGetComponent<Player>(out var player))
                ShowUIWallmart(player);
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CheckColliderMask(buyerMask) && other.TryGetComponent<Player>(out var player))
                player.HideWallmart();
        }
    }
}