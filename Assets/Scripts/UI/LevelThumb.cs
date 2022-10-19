using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelThumb : MonoBehaviour
    {
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private Image veilImage;
        [SerializeField] private Image lockImage;
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private Color lockedTextColor;
        [SerializeField] private Color unlockedTextColor;

        [SerializeField] private int levelId;

        public int LevelId => levelId;
       
        private bool unlocked = false;

        public bool Unlocked
        {
            get => unlocked;
            set
            {
                unlocked = value;
                veilImage.enabled = !unlocked;
                lockImage.enabled = !unlocked;
                levelLabel.color = unlocked ? unlockedTextColor : lockedTextColor;
            }

        }
    }
}