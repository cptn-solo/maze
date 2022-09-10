using UnityEngine;

namespace Assets.Scripts
{
    public static class UnityExtensions
    {
        public static bool CheckColliderMask(this Collider other, LayerMask mask)
        {
            return (mask.value & (1 << other.transform.gameObject.layer)) > 0;
        }
    }
}