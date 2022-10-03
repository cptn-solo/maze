﻿
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using static PlayerInputActions;

namespace Assets.Scripts
{
    public partial class Player
    {
        private PlayerInputActions actions;
        private DefaultInputActions defaultActions;

        private void ToggleInput(bool toggle)
        {
            if (toggle)
            {
                defaultActions.Enable();
                actions.Enable();
            }
                
            else
            {
                defaultActions.Disable();
                actions.Disable();
            }
        }

        private void BindInputs()
        {
            actions = new PlayerInputActions();
            defaultActions = new DefaultInputActions();

            ToggleInput(true);
            
            defaultActions.Player.Move.performed += LeftStick_performed;
            defaultActions.Player.Move.canceled += LeftStick_canceled;

            //actions.Default.Move.performed += Move_performed;
            actions.Default.Jump.performed += Jump_performed;
            actions.Default.Attack.performed += Attack_performed;
            actions.Default.Weapon.performed += Minigun_performed;
            actions.Default.Item1.performed += Item1_performed;
            actions.Default.Item2.performed += Item2_performed;

            //actions.Mobile.LeftStick.performed += LeftStick_performed;
            //actions.Mobile.LeftStick.canceled += LeftStick_canceled;
            actions.Mobile.Jump.performed += Jump_performed;
            actions.Mobile.Attack.performed += Attack_performed;
            actions.Mobile.Weapon.performed += Minigun_performed;
            actions.Mobile.Item1.performed += Item1_performed;
            actions.Mobile.Item2.performed += Item2_performed;
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
        private void LeftStick_canceled(InputAction.CallbackContext obj)
        {
            OnMove(Vector3.zero);
        }

        private void Move_performed(InputAction.CallbackContext obj)
        {
            OnMove(obj.ReadValue<Vector3>());
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            OnJump();
        }

        private void Attack_performed(InputAction.CallbackContext obj)
        {
            OnAttack(obj.ReadValueAsButton());
        }

        private void Minigun_performed(InputAction.CallbackContext obj)
        {
            OnWeaponSelect();
        }

        private void Item1_performed(InputAction.CallbackContext obj)
        {
            OnItem1Select();
        }

        private void Item2_performed(InputAction.CallbackContext obj)
        {
            OnItem2Select();
        }



    }
}