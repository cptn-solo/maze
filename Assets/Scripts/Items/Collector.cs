using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Collector : MonoBehaviour
    {
        public event Action<CollectableType, int> OnCollected;

        internal void Collect(CollectableType collectableType, int val = 1) =>
            OnCollected?.Invoke(collectableType, val);
    }
}