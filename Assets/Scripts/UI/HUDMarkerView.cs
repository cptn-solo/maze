using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDMarkerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private float worldYOffset = .17f;

        [SerializeField] private Image shieldImage;
        [SerializeField] private Image hpImage;

        [SerializeField] private float baseBarWidth = 150.0f;

        public event Action<HUDMarkerView, Transform> OnMarkerBeingDestroyed;

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

        internal void SetInfo(string label, Color color, string info)
        {
            Info.SetInfo(label, color, info);

            titleLabel.text = label;
            titleLabel.color = color;
            infoLabel.text = info;
        }

        internal void SetInfo(UnitInfo e)
        {
            infoLabel.text = $"{e.Score}";
            infoLabel.color = e.ScoreColor;

            SetImageWidth(hpImage, e.Hp);
            SetImageWidth(shieldImage, e.Shield);
        }

        private void SetImageWidth(Image image, int val)
        {
            image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, val);
        }

        private void LateUpdate()
        {
            if (cam != null && worldTarget != null)
            {
                var pos = worldTarget.position + worldTarget.localScale.y * worldYOffset * Vector3.up;
                rectTransform.position = cam.WorldToScreenPoint(pos);
            }
        }

        private void OnDestroy()
        {
            OnMarkerBeingDestroyed?.Invoke(this, worldTarget);
        }

    }
}