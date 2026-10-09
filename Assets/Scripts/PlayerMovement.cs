using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] float speed = 6f;
    [SerializeField] float jumpForce = 10f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        var moveX = Input.GetAxisRaw("Horizontal");
        var moveZ = Input.GetAxisRaw("Vertical");
        var moveY = Jump(Input.GetButton("Jump"));
        Vector3 move = new Vector3(moveX, moveY, moveZ).normalized;
        _rb.linearVelocity = move * speed;
    }
    private float Jump(bool inputJump)
    {
        if (inputJump)
        {
            return jumpForce;
        }
        else
        {
            return 0f;
        }
    }
}
