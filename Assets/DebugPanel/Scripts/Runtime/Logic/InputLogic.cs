using DebugPanel.Behaviours;
using DebugPanel.Core;
using DebugPanel.Datas;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace DebugPanel.Logic
{
    public sealed class InputLogic
    {
        readonly InputData _inputData;

        public InputLogic(InputData inputData)
        {
            _inputData = inputData;
        }

        public void SetupToggleInput(
            KeyCode keyCode, 
            bool enableMiddleMouseButtonToggle, 
            bool enableThreeFingerTapToggle
            )
        {
            if (_inputData.ToggleSetUp)
            {
                return;
            }

            _inputData.ToggleSetUp = true;
            
#if ENABLE_INPUT_SYSTEM 
            if (enableThreeFingerTapToggle)
            {
                EnhancedTouchSupport.Enable();

                Touch.onFingerDown += _ =>
                {
                    if (Touch.activeFingers.Count == 3)
                    {
                        UDebugPanel.Toggle();
                    }
                };
            }

            InputAction toggleAction = new InputAction(type: InputActionType.Button);

            if (enableMiddleMouseButtonToggle)
            {
                toggleAction.AddBinding("Mouse/middleButton");                
            }

            if (keyCode != KeyCode.None)
            {
                toggleAction.AddBinding($"<Keyboard>/{keyCode}");   
            }
            
            toggleAction.Enable(); 
            toggleAction.performed += _ => UDebugPanel.Toggle();
#else
            _inputData.ToggleInputEnabled = true;
            _inputData.ToggleInputKeyCode = keyCode;
            _inputData.MiddleMouseButtonToggleEnabled = enableMiddleMouseButtonToggle;
            _inputData.ThreeFingerTapToggleEnabled = enableThreeFingerTapToggle;

            GameObject gameObject = new GameObject("DebugPanelToggleBehaviour");
            Object.DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<DebugPanelToggleBehaviour>();
#endif
        }

        public void TickToggleInput()
        {
#if !ENABLE_INPUT_SYSTEM 
            if (!_inputData.ToggleInputEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(_inputData.ToggleInputKeyCode))
            {
                UDebugPanel.Toggle();
            }

            if (_inputData.MiddleMouseButtonToggleEnabled)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    UDebugPanel.Toggle();
                }   
            }

            if (_inputData.ThreeFingerTapToggleEnabled)
            {
                if (Input.touchCount == 3)
                {
                    UDebugPanel.Toggle();
                }
            }
#endif
        }
    }
}