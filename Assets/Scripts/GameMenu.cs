using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameMenu : MonoBehaviour
    {        
        [SerializeField] private Button[] levelButtons;

        public event Action<int> OnLevelSelected;

        private void OnEnable()
        {
            for (int i = 0; i < levelButtons.Length; i++)
                levelButtons[i].onClick.AddListener(() => OnLevelSelected.Invoke(i));
        }

        private void OnDisable()
        {
            for (int i = 0; i < levelButtons.Length; i++)
                levelButtons[i].onClick.RemoveListener(() => OnLevelSelected.Invoke(i));
        }
    }
}