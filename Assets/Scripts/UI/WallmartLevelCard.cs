using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class WallmartLevelCard : WallmartCard
{
    [SerializeField] private Image thumbnailImage;
    protected override void ApplyValues()
    {
        if (perkInfo.WallmartItem == WallmartItem.Level)
        {
            titleLabel.text = $"Level {perkInfo.LevelId}";
            
            var image = Resources.Load<Sprite>(SpriteNameForLevel(perkInfo.LevelId));

            thumbnailImage.sprite = image;
        }
    }
    private string SpriteNameForLevel(int level) =>
        $"level{level}";


}
