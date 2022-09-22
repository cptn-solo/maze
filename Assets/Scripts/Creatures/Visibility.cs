using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Visibility : MonoBehaviour, IVisibleObject
    {
        private bool isVisible;

        private VisibilityChecker checker;
        private Battle battle;

        #region IVisibleObject

        public bool IsVisible => isVisible;
        public Transform Transform => transform;
        public event EventHandler<bool> OnVisibilityChanged;
        public event EventHandler<UnitInfo> OnInfoChanged;

        #endregion

        private UnitInfo unitInfo = default;

        private void Awake()
        {
            checker = GetComponent<VisibilityChecker>();
            battle = GetComponent<Battle>();
        }

        private void OnEnable()
        {
            if (checker != null)
                checker.OnVisibilityChanged += VisChecker_OnVisibilityChanged;

            if (battle != null)
                battle.OnBattleInfoChange += Battle_OnBattleInfoChange;
        }

        private void Battle_OnBattleInfoChange(BattleInfo obj)
        {
            unitInfo.Hp = obj.CurrentHP;
            unitInfo.Shield = obj.CurrentShield;

            OnInfoChanged?.Invoke(this, unitInfo);            
        }

        private void VisChecker_OnVisibilityChanged(object sender, bool e)
        {
            isVisible = e;
            OnVisibilityChanged?.Invoke(this, e);
            OnInfoChanged?.Invoke(this, unitInfo);
        }

        private void OnDisable()
        {
            if (checker != null)
                checker.OnVisibilityChanged -= VisChecker_OnVisibilityChanged;
        }

    }
}