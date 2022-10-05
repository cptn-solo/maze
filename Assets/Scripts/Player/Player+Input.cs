using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public partial class Player
    {
        private PlayerInputActions actions;

        public void ToggleInput(bool toggle)
        {
            if (toggle)
            {
                actions.Enable();
                touches.enabled = true;
            }                
            else
            {
                actions.Disable();
                touches.enabled = false;
            }
        }

        private void BindInputs()
        {
            actions = new PlayerInputActions();

            ToggleInput(true);
            
            actions.Default.Move.performed += Move_performed;
            actions.Default.Move.canceled += Move_canceled;
            actions.Default.Jump.performed += Jump_performed;
            actions.Default.Attack.performed += Attack_performed;
            actions.Default.Weapon.performed += Minigun_performed;
            actions.Default.Item1.performed += Item1_performed;
            actions.Default.Item2.performed += Item2_performed;

            actions.Mobile.Jump.performed += Jump_performed;
            actions.Mobile.Attack.performed += Attack_performed;
            actions.Mobile.Weapon.performed += Minigun_performed;
            actions.Mobile.Item1.performed += Item1_performed;
            actions.Mobile.Item2.performed += Item2_performed;
        }

        private void Move_performed(InputAction.CallbackContext obj) =>
            keyboardMoveDir = obj.ReadValue<Vector2>();

        private void Move_canceled(InputAction.CallbackContext obj) =>
            keyboardMoveDir = Vector2.zero;

        private void Jump_performed(InputAction.CallbackContext obj) =>
            OnJump();

        private void Attack_performed(InputAction.CallbackContext obj) =>
            OnAttack(obj.ReadValueAsButton());

        private void Minigun_performed(InputAction.CallbackContext obj) =>
            OnWeaponSelect();

        private void Item1_performed(InputAction.CallbackContext obj) =>
            OnItem1Select();

        private void Item2_performed(InputAction.CallbackContext obj) =>
            OnItem2Select();
    }
}