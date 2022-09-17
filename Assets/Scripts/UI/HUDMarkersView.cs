using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkersView : MonoBehaviour
    {
        [SerializeField] private GameObject markerPrefab;
        private readonly Dictionary<Transform, HUDMarkerView> markers = new();

        private RectTransform rectTransform;

        internal HUDMarkerView AddEnemy(IVisibleObject enemy, EnemyType enemyType) => 
            AddPlayer(enemy);

        internal HUDMarkerView AddPlayer(IVisibleObject player)
        {
            var transform = player.Transform;

            if (markers.ContainsKey(transform))
                return markers[transform];

            var marker = Instantiate(markerPrefab).GetComponent<HUDMarkerView>();
            marker.Attach(rectTransform, transform, rectTransform.localScale.x);
            markers.Add(transform, marker);
            player.OnVisibilityChanged += Player_OnVisibilityChanged;
            return marker;
        }

        private void Player_OnVisibilityChanged(object sender, bool e)
        {
            var owner = (IVisibleObject)sender;
            if (markers.TryGetValue(owner.Transform, out var marker) && !marker.gameObject.IsDestroyed())
                marker.enabled = e;
        }

        internal void RemoveMarker(IVisibleObject owner)
        {
            var transform = owner.Transform;
            if (markers.TryGetValue(transform, out var marker) && !marker.gameObject.IsDestroyed())
            {
                owner.OnVisibilityChanged -= Player_OnVisibilityChanged;
                marker.Detach();
                Destroy(marker.gameObject);
            }
        }

        internal void UpdatePlayer(Transform transform, PlayerInfo playerInfo)
        {
            if (markers.TryGetValue(transform, out var marker) && !marker.gameObject.IsDestroyed())
                marker.SetInfo(playerInfo.NickName, playerInfo.BodyTintColor, $"{playerInfo.Score}");
        }

        private void Awake() => rectTransform = GetComponent<RectTransform>();
    }
}