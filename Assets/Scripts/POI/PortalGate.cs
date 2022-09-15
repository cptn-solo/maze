using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PortalGate : MonoBehaviour
    {
        public event Action<PortalGate, Movable> OnEnterGate;
        public LayerMask PassengerMask { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(PassengerMask) &&
                other.gameObject.TryGetComponent<Movable>(out var passenger))
                OnEnterGate?.Invoke(this, passenger);
        }

    }
}