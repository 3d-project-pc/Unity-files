using UnityEngine;

public class PlayerAvatarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float speed = 8f;
    public float gravity = -19.62f;

    [Header("Physics")]
    private Vector3 velocity;

    void Update()
    {
        // 1. Get Input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 2. Calculate Direction
        // 'transform.forward' ensures you move relative to where the player faces
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Normalize so diagonal movement isn't faster
        if (move.magnitude > 1) move.Normalize();

        // 3. Apply Movement
        controller.Move(move * speed * Time.deltaTime);

        // 4. Gravity Logic
        // This keeps the player on the ground
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}