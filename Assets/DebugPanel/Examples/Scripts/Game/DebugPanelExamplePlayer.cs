#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace DebugPanel.Examples
{
    public sealed class DebugPanelExamplePlayer : MonoBehaviour
    {
        public enum PlayerColor
        {
            Red,
            Blue,
            Purple,
        }
        
        [Header("References")]
        public SpriteRenderer? SpriteRenderer;

        [Header("Values")] 
        [Min(0)] public float Speed = 2;
        [Min(0)] public float Range = 4;
        public bool IsIa;
        public PlayerColor Color;
        public Color ReadColor;
        public Color BlueColor;
        public Color PurpleColor;
        
#if ENABLE_INPUT_SYSTEM 
        InputAction _actionUp;
        InputAction _actionDown;
#endif

        Func<IReadOnlyList<DebugPanelExampleBall>>? _getBallsFunc;
        int _direction;
        
        void Awake()
        {
            SetupInput();
            SetColor(Color);
        }
        
        void Update()
        {
            UpdateIa();
            UpdateInput();
            UpdateMovement();
        }

        public void Setup(Func<IReadOnlyList<DebugPanelExampleBall>> getBallsFunc)
        {
            _getBallsFunc = getBallsFunc;
        }

        public void SetColor(PlayerColor playerColor)
        {
            Color color = ReadColor;
            
            switch (playerColor)
            {
                case PlayerColor.Red:
                    color = ReadColor;
                    break;
                case PlayerColor.Blue:
                    color = BlueColor;
                    break;
                case PlayerColor.Purple:
                    color = PurpleColor;
                    break;
            }

            SpriteRenderer!.color = color;
        }

        void SetupInput()
        {
#if ENABLE_INPUT_SYSTEM 
            _actionUp = new InputAction(type: InputActionType.Button);
            _actionUp.AddBinding("<Keyboard>/w");
            _actionUp.Enable(); 
            
            _actionDown = new InputAction(type: InputActionType.Button);
            _actionDown.AddBinding("<Keyboard>/s");
            _actionDown.Enable(); 
#endif
        }

        void UpdateIa()
        {
            if (!IsIa)
            {
                return;
            }
            
            if (_getBallsFunc == null)
            {
                return;
            }

            IReadOnlyList<DebugPanelExampleBall> balls = _getBallsFunc.Invoke();

            float closestBallDistance = float.MaxValue;
            DebugPanelExampleBall? closestBall = balls.FirstOrDefault();

            if (closestBall == null)
            {
                return;
            }

            foreach (DebugPanelExampleBall ball in balls)
            {
                float distance = Mathf.Abs(ball.transform.position.x - transform.position.x);

                if (distance < closestBallDistance)
                {
                    closestBallDistance = distance;
                    closestBall = ball;
                }
            }
            
            bool insideMovementThreshold = Mathf.Abs(closestBall.transform.position.x - transform.position.x) > 1;

            if (!insideMovementThreshold)
            {
                return;
            }

            bool isAbove = closestBall.transform.localPosition.y > transform.localPosition.y;

            _direction = isAbove ? 1 : -1;
        }

        void UpdateInput()
        {
            if (IsIa)
            {
                return;
            }
            
#if ENABLE_INPUT_SYSTEM
            if (_actionUp.IsPressed())
            {
                _direction = 1;
            }

            if (_actionDown.IsPressed())
            {
                _direction = -1;
            }
#else
            if (UnityEngine.Input.GetKey("w"))
            {
                _direction = 1;
            }
            
            if (UnityEngine.Input.GetKey("s"))
            {
                _direction = -1;
            }
#endif
        }

        void UpdateMovement()
        {
            float deltaSpeed = Speed * Time.deltaTime;
            Vector2 newPositionToAdd = Vector2.up * (deltaSpeed * _direction);
            transform.localPosition += new Vector3(newPositionToAdd.x, newPositionToAdd.y);

            if (transform.localPosition.y > Range)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, Range);
            }
            
            if (transform.localPosition.y < -Range)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, -Range);
            }
            
            _direction = 0;
        }
    }
}