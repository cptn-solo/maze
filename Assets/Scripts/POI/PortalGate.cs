using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PortalGate : MonoBehaviour
    {
        public event Action<PortalGate, MovableUnit> OnEnterGate;
        public LayerMask PassengerMask { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(PassengerMask) &&
                other.gameObject.TryGetComponent<MovableUnit>(out var passenger))
                OnEnterGate?.Invoke(this, passenger);
        }

    }
}