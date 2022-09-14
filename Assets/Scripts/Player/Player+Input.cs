
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public partial class Player
    {
        private void BindInputs()
        {
            PlayerInputActions actions = new PlayerInputActions();

            actions.Enable();
            
            actions.Default.Move.performed += Move_performed;
            actions.Default.Jump.performed += Jump_performed;
            actions.Default.Fire.performed += Fire_performed;

            actions.Mobile.LeftStick.performed += LeftStick_performed;
            actions.Mobile.LeftStick.canceled += LeftStick_canceled;
            actions.Mobile.Jump.performed += Jump_performed;
            actions.Mobile.Fire.performed += Fire_performed;
        }


        private void Fire_performed(InputAction.CallbackContext obj)
        {
            
        }

        private void LeftStick_canceled(InputAction.CallbackContext obj)
        {
            OnMove(Vector3.zero);
        }

        private void LeftStick_performed(InputAction.CallbackContext obj)
        {
            var rawInput = obj.ReadValue<Vector2>();
            Vector3 mappedInput = default;
            mappedInput.x = rawInput.x;
            mappedInput.y = 0.0f;
            mappedInput.z = rawInput.y;

            OnMove(mappedInput);
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            OnMove(obj.ReadValue<Vector3>());
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            OnJump();
        }


    }
}