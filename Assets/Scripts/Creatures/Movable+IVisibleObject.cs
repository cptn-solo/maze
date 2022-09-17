using System;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Movable : IVisibleObject
    {
        private bool isVisible;

        #region IVisibleObject
        public bool IsVisible => isVisible;
        public Transform Transform => transform;
        public event EventHandler<bool> OnVisibilityChanged;

        #endregion

        private void VisChecker_OnVisibilityChanged(object sender, bool e)
        {
            isVisible = e;
            OnVisibilityChanged?.Invoke(this, e);
        }


    }
}