using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IVisibleObject
    {
        bool IsVisible { get; }
        Transform Transform { get; }

        event EventHandler<bool> OnVisibilityChanged;
        event EventHandler<UnitInfo> OnInfoChanged;
    }
}