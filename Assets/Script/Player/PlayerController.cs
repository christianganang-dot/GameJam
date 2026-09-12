using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null)
            return;

        // Kiri
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput.x = -1f;
        }

        // Kanan
        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput.x = 1f;
        }
    }

    private void MovePlayer()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(moveInput.x) > 0.01f;

        animator.SetBool("IsMoving", isMoving);

        // Menentukan arah hadap
        if (moveInput.x > 0)
        {
            animator.SetFloat("Direction", 1f);
        }
        else if (moveInput.x < 0)
        {
            animator.SetFloat("Direction", -1f);
        }
    }
}

