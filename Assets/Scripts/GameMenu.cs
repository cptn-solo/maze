using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameMenu : MonoBehaviour
    {        
        [SerializeField] private LevelThumb[] levelButtons;

        public LevelThumb[] LevelButtons => levelButtons;

        public event Action<int> OnLevelSelected;

        public void LevelButtonPressed(int levelIdx) =>
            OnLevelSelected?.Invoke(LevelButtons[levelIdx].LevelId);
    }
}