using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkersView : MonoBehaviour
    {
        [SerializeField] private GameObject markerPrefab;
        private readonly Dictionary<Transform, HUDMarkerView> markers = new();

        private RectTransform rectTransform;

        internal HUDMarkerView AddEnemy(IVisibleObject enemy, EnemyType enemyType) => 
            AddUnit(enemy);

        internal HUDMarkerView AddUnit(IVisibleObject unit)
        {
            var transform = unit.Transform;

            if (markers.ContainsKey(transform))
                return markers[transform];

            unit.OnVisibilityChanged += Unit_OnVisibilityChanged;
            unit.OnInfoChanged += Unit_OnInfoChanged;

            var marker = Instantiate(markerPrefab).GetComponent<HUDMarkerView>();
            marker.OnMarkerBeingDestroyed += Marker_OnMarkerBeingDestroyed;
            marker.Attach(rectTransform, transform, rectTransform.localScale.x);
            markers.Add(transform, marker);

            return marker;
        }

        private void Marker_OnMarkerBeingDestroyed(HUDMarkerView arg1, Transform arg2)
        {
            markers.Remove(arg2);
        }
        private void Unit_OnInfoChanged(object sender, UnitInfo e)
        {
            var owner = (IVisibleObject)sender;
            if (markers.TryGetValue(owner.Transform, out var marker))
                marker.SetInfo(e);
        }

        private void Unit_OnVisibilityChanged(object sender, bool e)
        {
            var owner = (IVisibleObject)sender;
            if (markers.TryGetValue(owner.Transform, out var marker))
                marker.gameObject.SetActive(e);
        }

        internal void RemoveMarker(IVisibleObject unit)
        {
            var transform = unit.Transform;
            if (markers.TryGetValue(transform, out var marker))
            {
                unit.OnVisibilityChanged -= Unit_OnVisibilityChanged;
                marker.Detach();
                Destroy(marker.gameObject);
            }
        }

        private void Awake() => rectTransform = GetComponent<RectTransform>();
    }
}