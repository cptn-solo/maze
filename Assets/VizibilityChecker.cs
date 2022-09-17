using System;
using UnityEngine;

public class VizibilityChecker : MonoBehaviour
{
    public event EventHandler<bool> OnVisibilityChanged;

    public void OnBecameInvisible()
    {
        OnVisibilityChanged?.Invoke(this, false);
    }

    public void OnBecameVisible()
    {
        OnVisibilityChanged?.Invoke(this, true);
    }

}
