using System;
using UnityEngine;
using Random = System.Random;

namespace DebugPanel.Examples
{
    public sealed class DebugPanelExampleBall : MonoBehaviour
    {
        [Header("Values")]
        public float Speed = 100;

        public event Action OnGoal;
        
        Vector2 _direction = Vector2.zero;

        void Start()
        {
            Reset();
        }

        void Update()
        {
            UpdateMovement();
        }

        public void Reset()
        {
            transform.localPosition = Vector2.zero;
            
            int randomDirection = UnityEngine.Random.Range(0, 2);

            if (randomDirection == 0)
            {
                randomDirection = -1;
            }
            
            _direction = new Vector2(randomDirection, UnityEngine.Random.Range(-0.4f, 0.4f));
        }

        void UpdateMovement()
        {
            float deltaSpeed = Time.deltaTime * Speed;
            Vector2 newPositionToAdd = _direction * deltaSpeed;
            transform.localPosition += new Vector3(newPositionToAdd.x, newPositionToAdd.y);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            bool isWall = IsWall(other);

            if (isWall)
            {
                BounceOnWall();
                return;
            }
            
            bool isPlayer = IsPlayer(other);

            if (isPlayer)
            {
                BounceOnPlayer();
                return;
            }
            
            bool isGoal = IsGoal(other);

            if (isGoal)
            {
                WhenGoal();
                return;
            }
        }

        bool IsWall(Collider2D other) => other.gameObject.CompareTag("Wall");
        
        bool IsPlayer(Collider2D other) => other.gameObject.CompareTag("Player");
        bool IsGoal(Collider2D other) => other.gameObject.CompareTag("Goal");
        void BounceOnWall() => _direction = new Vector2(_direction.x, -_direction.y);

        void BounceOnPlayer() => _direction = new Vector2(-_direction.x, _direction.y);

        void WhenGoal()
        {
            OnGoal?.Invoke();
        }
    }
}