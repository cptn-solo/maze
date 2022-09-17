using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Visibility : MonoBehaviour, IVisibleObject
    {
        private bool isVisible;

        private VisibilityChecker checker;

        #region IVisibleObject
        public bool IsVisible => isVisible;
        public Transform Transform => transform;
        public event EventHandler<bool> OnVisibilityChanged;

        #endregion

        private void Awake()
        {
            checker = GetComponent<VisibilityChecker>();
        }

        private void OnEnable()
        {
            if (checker != null)
                checker.OnVisibilityChanged += VisChecker_OnVisibilityChanged;
        }

        private void OnDisable()
        {
            if (checker != null)
                checker.OnVisibilityChanged -= VisChecker_OnVisibilityChanged;
        }

        private void VisChecker_OnVisibilityChanged(object sender, bool e)
        {
            isVisible = e;
            OnVisibilityChanged?.Invoke(this, e);
        }


    }
}