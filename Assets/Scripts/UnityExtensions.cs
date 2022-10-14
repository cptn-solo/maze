using UnityEngine;

namespace Assets.Scripts
{
    public static class UnityExtensions
    {
        public static bool CheckColliderMask(this Collider other, LayerMask mask)
        {
            return (mask.value & (1 << other.transform.gameObject.layer)) > 0;
        }

        /// <summary> Converts given bitmask to layer number </summary>
        /// <returns> layer number </returns>
        public static int FirstSetLayer(this LayerMask mask)
        {
            var bitmask = mask.value;

            int result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1)
            {
                bitmask >>= 1;
                result++;
            }
            return result;
        }
    }
}