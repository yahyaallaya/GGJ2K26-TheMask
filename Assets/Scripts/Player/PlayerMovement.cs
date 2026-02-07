using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    private float horizontalInput;

    private Rigidbody2D body;
    private Animator animator;
    private CapsuleCollider2D col;
    private float wallJumpCooldown;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // animator = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        float horizontalInput = Keyboard.current.leftArrowKey.isPressed ? -1 :
                     Keyboard.current.rightArrowKey.isPressed ? 1 : 0;

        body.linearVelocity = new Vector2(horizontalInput * moveSpeed, body.linearVelocity.y);


        // this is for the player vision direction
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;

        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // animator.SetBool("run", horizontalInput != 0);
        // animator.SetBool("grounded", isGrounded());


        if(Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Jump();
        }





        // this is an extra , maybe remove

        if (wallJumpCooldown > 0.2f)
        {
            body.linearVelocity = new Vector2(horizontalInput * jumpForce, body.linearVelocity.y);
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 1;
                body.linearVelocity = Vector2.zero;
            }
            else
                body.gravityScale = 2.5f;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame) // space
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            // animator.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            // if (horizontalInput == 0)
            // {
            //     body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
            //     transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // }
            // else
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 9);

            wallJumpCooldown = 0;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Ground"))
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}

