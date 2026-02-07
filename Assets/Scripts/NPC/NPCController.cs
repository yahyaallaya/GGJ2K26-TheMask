using UnityEngine;

public class NPCController : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public float patrolDistance = 4f;


    [Header("Detection Settings")]
    public float groundCheckDistance = 0.5f;
    public float obstacleCheckDistance = 0.2f;
    public float jumpCheckDistance = 1f;

    Vector2 leftPoint;
    Vector2 rightPoint;
    Vector2 startPos;
    public Transform player;

    private Rigidbody2D rb;
    private Vector2 currentTarget;
    private float lastAttackTime;


    [Header("Ground Check Settings")]
    public Transform groundCheckFront;
    public LayerMask groundLayer;


    private enum State { Patrol, Chase, Attack }
    private State state = State.Patrol;

    private bool movingToRight = true;

    bool shouldJump = false;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        leftPoint = new Vector2(startPos.x - patrolDistance, startPos.y);
        rightPoint = new Vector2(startPos.x + patrolDistance, startPos.y);
        currentTarget = leftPoint;

    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // if (distanceToPlayer <= attackRange)
        //     state = State.Attack;
        // else 
        if (distanceToPlayer <= detectionRange)
            state = State.Chase;
        else
            state = State.Patrol;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }

        if (IsGrounded() && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;
            // rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
            rb.linearVelocity = new Vector2(jumpDirection.x, jumpForce);
        }
        
    }

    void Patrol()
    {
        Debug.Log("NPC Patrol");
        Vector2 target = movingToRight ? rightPoint : leftPoint;

        // Move horizontally only
        Vector2 dir = (target - (Vector2)transform.position);
        dir.y = 0;
        float moveDir = Mathf.Sign(dir.x);

        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (moveDir != 0)
            transform.localScale = new Vector3(moveDir, 1, 1);

        // Switch direction if passed target
        if ((movingToRight && transform.position.x >= target.x) ||
            (!movingToRight && transform.position.x <= target.x))
        {
            movingToRight = !movingToRight;
        }

        // Check obstacles / ground
        bool groundAhead = IsGroundAhead();
        bool obstacleAhead = IsObstacleAhead();
        bool canJump = CanJumpOver();


        
        // if (obstacleAhead && canJump && IsGrounded())
        // {
        //     Jump();
        // }

        // if (groundAhead && canJump && IsGrounded())
        // {
        //     Jump();
        // }
    }

    void Chase()
    {
        Debug.Log("NPC Chase");

        Vector2 dirToPlayer = player.position - transform.position;

        // Ignore vertical
        dirToPlayer.y = 0;

        // Move towards player
        float moveDir = Mathf.Sign(dirToPlayer.x);
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);

        // Flip sprite
        if (moveDir != 0)
            transform.localScale = new Vector3(moveDir, 1, 1);

        // Check obstacles / ground
        bool groundAhead = IsGroundAhead();
        bool obstacleAhead = IsObstacleAhead();
        bool canJump = CanJumpOver();


        RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(moveDir, 0), 2f, groundLayer);
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(moveDir, 0, 0), Vector2.down, 2f, groundLayer);

        if (!groundInFront.collider && !gapAhead.collider && IsGrounded())
        {
            shouldJump = true;
            
        }
    

        // if (obstacleAhead && canJump && IsGrounded())
        // {
        //     Jump();
        // }

        // Optional: stop at edges
        if (!groundAhead)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }


    void Attack()
    {
        Debug.Log("NPC Attack");
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("NPC attacks player!");
            // Damage player here
        }
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }



    bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheckFront.position, Vector2.down, 0.2f, groundLayer);
    }

    bool IsGroundAhead()
    {
        // ray from front of feet, slightly ahead
        return Physics2D.Raycast(groundCheckFront.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    bool IsObstacleAhead()
    {
        Vector2 dir = Vector2.right * Mathf.Sign(transform.localScale.x);
        float distance = obstacleCheckDistance; // e.g., 0.3-0.5 units

        return Physics2D.Raycast(transform.position, dir, distance, groundLayer);
    }

    bool CanJumpOver()
    {
        Vector2 dir = Vector2.right * Mathf.Sign(transform.localScale.x);
        Vector2 origin = transform.position + Vector3.up * 0.5f; // head height
        float distance = jumpCheckDistance; // e.g., 0.5-1 units

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, groundLayer);
        return hit.collider == null; 
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

}

