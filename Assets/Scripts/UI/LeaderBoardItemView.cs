using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LeaderBoardItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI balanceLabel;

        private string networkId;
        private Image bgImage;
        
        private RectTransform rectTransform;

        public UnitInfo Info { get; private set; }
        public void Attach(RectTransform parent, string networkId, float scale)
        {
            rectTransform.SetParent(parent);
            rectTransform.localScale *= scale;

            this.networkId = networkId;
        }

        internal void SetInfo(UnitInfo info, bool localPlayer)
        {
            nameLabel.text = info.NickName;
            nameLabel.color = info.BodyTintColor;
            balanceLabel.text = $"{info.Score}";
            bgImage.enabled = localPlayer;

            Info = info;
        }

        internal void Detach()
        {
            rectTransform.parent = null;
            networkId = default;
            Info = default;
            this.gameObject.SetActive(false);
        }
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            bgImage = GetComponent<Image>();
        }

        internal void SetListPosition(int pos)
        {
            rectTransform.SetSiblingIndex(pos);
            SetLeader(pos == 0);
        }
        internal void SetLeader(bool leader)
        {
            if (leader)
                rectTransform.SetAsFirstSibling();
            
            var fontStyle = leader ? FontStyles.Bold : FontStyles.Normal; ;
            
            nameLabel.fontStyle = fontStyle;
            balanceLabel.fontStyle = fontStyle;
        }
    }
}