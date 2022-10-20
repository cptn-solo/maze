using UnityEngine;

namespace Assets.Scripts
{

    public class Wallmart : MonoBehaviour
    {
        [SerializeField] private WallmartItem itemType;
        [SerializeField] private LayerMask buyerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(buyerMask) && other.TryGetComponent<Player>(out var player))
                player.ShowWallmart(itemType);
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CheckColliderMask(buyerMask) && other.TryGetComponent<Player>(out var player))
                player.HideWallmart();
        }
    }
}