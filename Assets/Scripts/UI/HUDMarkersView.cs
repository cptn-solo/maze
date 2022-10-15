using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkersView : MonoBehaviour
    {
        [SerializeField] private GameObject markerPrefab;
        private readonly Dictionary<Transform, HUDMarkerView> markers = new();
        
        private Camera cam;

        private RectTransform rectTransform;
        public void AttachCamera(Camera cam) =>
            this.cam = cam;

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
            marker.Attach(rectTransform, transform, rectTransform.localScale.x, cam);
            markers.Add(transform, marker);

            return marker;
        }
        internal void RemoveUnit(IVisibleObject unit)
        {
            var transform = unit.Transform;
            RemoveMarker(transform);
            unit.OnVisibilityChanged -= Unit_OnVisibilityChanged;
            unit.OnInfoChanged -= Unit_OnInfoChanged;
        }
        internal void RemoveMarker(Transform unitTransform)
        {
            if (markers.TryGetValue(unitTransform, out var marker))
            {
                markers.Remove(unitTransform);
                marker.Detach();
                Destroy(marker.gameObject);
            }
        }


        private void Marker_OnMarkerBeingDestroyed(HUDMarkerView arg1, Transform arg2)
        {
            if (arg2 != null && markers.TryGetValue(arg2, out var _))
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

        private void Awake() => rectTransform = GetComponent<RectTransform>();
    }
}