using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameMenu : MonoBehaviour
    {        
        [SerializeField] private Button[] levelButtons;

        public event Action<int> OnLevelSelected;

        public void LevelButtonPressed(int levelIdx) =>
            OnLevelSelected?.Invoke(levelIdx);
    }
}