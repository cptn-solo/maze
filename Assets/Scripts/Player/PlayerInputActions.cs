//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.2
//     from Assets/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""f42ba754-3fb3-433d-baec-270ca4d86685"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""cce2f2f4-b4ed-4869-93d7-c5c8f7ba3426"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""4dab0cdc-2e53-4a1c-b62b-37a5a649c848"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""2849989f-a590-4e67-bfa7-d159a0e0d0d9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Weapon"",
                    ""type"": ""Button"",
                    ""id"": ""d247cf25-185f-4b6e-b499-524e7a74525a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Item1"",
                    ""type"": ""Button"",
                    ""id"": ""4244bbaf-72a3-43f2-867d-4e04f74a7779"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Item2"",
                    ""type"": ""Button"",
                    ""id"": ""e7c5e091-cc2b-48a6-9cd5-7d7e03517f6f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""73a6dde1-fd18-49a4-bad8-e8ee16c73de7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b6fa36a5-e722-4396-992c-0aeb433f4ca1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6d4a7b82-97d7-4e2d-9e86-81efae0e513d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""97fb5a4a-bbb8-4b8c-b61f-d79956395eb8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ebd65dc9-a42e-4d6a-8403-cf5fb7f77737"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9a8630d8-bfed-44b0-8075-404b1e0cab42"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9a510bca-0cc5-4748-9cc5-5e3270220b45"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fa6a80a-0337-416f-ae83-55b19dda1634"",
                    ""path"": ""<Keyboard>/#(1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Weapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""64f70729-4dbd-4553-b067-354d9ca9d634"",
                    ""path"": ""<Keyboard>/#(2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3699cde6-0585-46fc-b44f-61ed3080f71b"",
                    ""path"": ""<Keyboard>/#(3)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Mobile"",
            ""id"": ""7027a820-b6f9-46f8-ba32-dbaf03497369"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""756e6a71-8f2e-4908-8947-a20fe6493587"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""df3903cb-5d2e-499f-82da-acf0598e04ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Weapon"",
                    ""type"": ""Button"",
                    ""id"": ""16cba9d7-dcc1-421c-974a-ac3392817f4f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Item1"",
                    ""type"": ""Button"",
                    ""id"": ""bdd29929-1a94-4c98-892b-52ba745044eb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Item2"",
                    ""type"": ""Button"",
                    ""id"": ""8a6fd51c-d59b-450e-9e46-f487e3b4ed39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""165e21ec-0110-49d8-83e4-d2af6580f34f"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5eacadc0-0080-40e4-8084-55f00ee89b96"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19c5047a-34c6-4e61-936f-c02797b8f9de"",
                    ""path"": ""<Keyboard>/#(1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Weapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""448c61f6-7fd0-4579-b215-7f3bafe649af"",
                    ""path"": ""<Keyboard>/#(2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ed9a6f28-d463-4329-abeb-fa6a1129eff2"",
                    ""path"": ""<Keyboard>/#(3)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Item2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_Move = m_Default.FindAction("Move", throwIfNotFound: true);
        m_Default_Jump = m_Default.FindAction("Jump", throwIfNotFound: true);
        m_Default_Attack = m_Default.FindAction("Attack", throwIfNotFound: true);
        m_Default_Weapon = m_Default.FindAction("Weapon", throwIfNotFound: true);
        m_Default_Item1 = m_Default.FindAction("Item1", throwIfNotFound: true);
        m_Default_Item2 = m_Default.FindAction("Item2", throwIfNotFound: true);
        // Mobile
        m_Mobile = asset.FindActionMap("Mobile", throwIfNotFound: true);
        m_Mobile_Jump = m_Mobile.FindAction("Jump", throwIfNotFound: true);
        m_Mobile_Attack = m_Mobile.FindAction("Attack", throwIfNotFound: true);
        m_Mobile_Weapon = m_Mobile.FindAction("Weapon", throwIfNotFound: true);
        m_Mobile_Item1 = m_Mobile.FindAction("Item1", throwIfNotFound: true);
        m_Mobile_Item2 = m_Mobile.FindAction("Item2", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_Move;
    private readonly InputAction m_Default_Jump;
    private readonly InputAction m_Default_Attack;
    private readonly InputAction m_Default_Weapon;
    private readonly InputAction m_Default_Item1;
    private readonly InputAction m_Default_Item2;
    public struct DefaultActions
    {
        private @PlayerInputActions m_Wrapper;
        public DefaultActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Default_Move;
        public InputAction @Jump => m_Wrapper.m_Default_Jump;
        public InputAction @Attack => m_Wrapper.m_Default_Attack;
        public InputAction @Weapon => m_Wrapper.m_Default_Weapon;
        public InputAction @Item1 => m_Wrapper.m_Default_Item1;
        public InputAction @Item2 => m_Wrapper.m_Default_Item2;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnMove;
                @Jump.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnAttack;
                @Weapon.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeapon;
                @Weapon.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeapon;
                @Weapon.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnWeapon;
                @Item1.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem1;
                @Item1.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem1;
                @Item1.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem1;
                @Item2.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem2;
                @Item2.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem2;
                @Item2.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnItem2;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Weapon.started += instance.OnWeapon;
                @Weapon.performed += instance.OnWeapon;
                @Weapon.canceled += instance.OnWeapon;
                @Item1.started += instance.OnItem1;
                @Item1.performed += instance.OnItem1;
                @Item1.canceled += instance.OnItem1;
                @Item2.started += instance.OnItem2;
                @Item2.performed += instance.OnItem2;
                @Item2.canceled += instance.OnItem2;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);

    // Mobile
    private readonly InputActionMap m_Mobile;
    private IMobileActions m_MobileActionsCallbackInterface;
    private readonly InputAction m_Mobile_Jump;
    private readonly InputAction m_Mobile_Attack;
    private readonly InputAction m_Mobile_Weapon;
    private readonly InputAction m_Mobile_Item1;
    private readonly InputAction m_Mobile_Item2;
    public struct MobileActions
    {
        private @PlayerInputActions m_Wrapper;
        public MobileActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Mobile_Jump;
        public InputAction @Attack => m_Wrapper.m_Mobile_Attack;
        public InputAction @Weapon => m_Wrapper.m_Mobile_Weapon;
        public InputAction @Item1 => m_Wrapper.m_Mobile_Item1;
        public InputAction @Item2 => m_Wrapper.m_Mobile_Item2;
        public InputActionMap Get() { return m_Wrapper.m_Mobile; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MobileActions set) { return set.Get(); }
        public void SetCallbacks(IMobileActions instance)
        {
            if (m_Wrapper.m_MobileActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_MobileActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MobileActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MobileActionsCallbackInterface.OnJump;
                @Attack.started -= m_Wrapper.m_MobileActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_MobileActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_MobileActionsCallbackInterface.OnAttack;
                @Weapon.started -= m_Wrapper.m_MobileActionsCallbackInterface.OnWeapon;
                @Weapon.performed -= m_Wrapper.m_MobileActionsCallbackInterface.OnWeapon;
                @Weapon.canceled -= m_Wrapper.m_MobileActionsCallbackInterface.OnWeapon;
                @Item1.started -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem1;
                @Item1.performed -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem1;
                @Item1.canceled -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem1;
                @Item2.started -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem2;
                @Item2.performed -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem2;
                @Item2.canceled -= m_Wrapper.m_MobileActionsCallbackInterface.OnItem2;
            }
            m_Wrapper.m_MobileActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Weapon.started += instance.OnWeapon;
                @Weapon.performed += instance.OnWeapon;
                @Weapon.canceled += instance.OnWeapon;
                @Item1.started += instance.OnItem1;
                @Item1.performed += instance.OnItem1;
                @Item1.canceled += instance.OnItem1;
                @Item2.started += instance.OnItem2;
                @Item2.performed += instance.OnItem2;
                @Item2.canceled += instance.OnItem2;
            }
        }
    }
    public MobileActions @Mobile => new MobileActions(this);
    public interface IDefaultActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnWeapon(InputAction.CallbackContext context);
        void OnItem1(InputAction.CallbackContext context);
        void OnItem2(InputAction.CallbackContext context);
    }
    public interface IMobileActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnWeapon(InputAction.CallbackContext context);
        void OnItem1(InputAction.CallbackContext context);
        void OnItem2(InputAction.CallbackContext context);
    }
}
