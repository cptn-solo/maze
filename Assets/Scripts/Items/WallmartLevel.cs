using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class WallmartLevel : Wallmart
    {
        private const string AnimSpeedFloat = "speed";

        private Animator animator;
        [SerializeField] private Animator thumbnailAnimator;

        [SerializeField] private Transform cellCube; // disable on unlock (purchase)
        [SerializeField] private TeleportPoint tp;
        [SerializeField] private int levelId = 1;
        
        public event Action<int> OnEnterLevelPortal;

        public int LevelId => levelId;

        private bool unlocked = false;
        public bool Unlocked {
            get => unlocked;
            set
            {
                unlocked = value;
                cellCube.gameObject.SetActive(!unlocked);
            }
        }

        protected override void ShowUIWallmart(Player player) =>
            player.ShowLevelWallmart(levelId);

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            tp.OnEnterPortal += Tp_OnEnterPortal;
        }
        private void OnDestroy()
        {
            tp.OnEnterPortal -= Tp_OnEnterPortal;
        }

        private void Tp_OnEnterPortal(Vector3 arg1, Vector3 arg2, MovableUnit arg3) =>
            OnEnterLevelPortal?.Invoke(LevelId);

        private void Start()
        {
            if (animator)
            {
                animator.SetFloat(AnimSpeedFloat, 0.1f);
                thumbnailAnimator.SetFloat(AnimSpeedFloat, 0.1f);
            }
        }

    }
}