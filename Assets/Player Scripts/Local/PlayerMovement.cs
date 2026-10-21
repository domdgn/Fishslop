using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveVector;
    public void OnMove(InputValue val)
    {
        if (!isLocalPlayer) return;
        moveVector = val.Get<Vector2>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
