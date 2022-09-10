using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PortalGate : MonoBehaviour
    {
        public event Action<PortalGate, Player> OnEnterGate;
        public LayerMask PlayerMask { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(PlayerMask) &&
                other.gameObject.TryGetComponent<Player>(out var player))
                OnEnterGate?.Invoke(this, player);
        }

    }
}