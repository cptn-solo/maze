using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private float worldYOffset = .17f;
        [SerializeField] private LayerMask targetLayerMask;

        private RectTransform rectTransform;

        private Camera cam;
        private Transform worldTarget;

        private MarkerInfo info = default;

        public MarkerInfo Info => info;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            cam = Camera.main;
        }

        public void Attach(RectTransform parent, Transform worldTarget, float scale)
        {
            rectTransform.SetParent(parent);
            rectTransform.localScale *= scale;

            this.worldTarget = worldTarget;
        }

        public void Detach()
        {
            rectTransform.parent = null;
            worldTarget = null;
            this.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (cam != null && worldTarget != null)
            {
                var pos = worldTarget.position + Vector3.up * worldYOffset;
                rectTransform.position = cam.WorldToScreenPoint(pos);
            }       
        }

        internal void SetInfo(string label, Color color, string info)
        {
            Info.SetInfo(label, color, info);

            titleLabel.text = label;
            titleLabel.color = color;
            infoLabel.text = info;
        }
    }
}