using UnityEngine;

public class MeleeNPC : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int damage;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    

    void Start()
    {
        

    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack only when play in sight
        if(PlayerInSight())
        {
            Debug.Log("aaaaaaaaaaaaaa   PlayerInSight");
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("MeleeAttack");
            }
        }
    }

    
    private bool PlayerInSight2()
    {
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
                                            new Vector3(col.bounds.size.x * range,col.bounds.size.y, col.bounds.size.z), 
                                            0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private bool PlayerInSight()
    {
        Vector2 center =
            (Vector2)col.bounds.center +
            (Vector2)transform.right * range * transform.localScale.x * colliderDistance;

        Vector2 size = new Vector2(col.bounds.size.x * range, col.bounds.size.y);

        Collider2D hit = Physics2D.OverlapBox(center, size, 0, playerLayer);

        return hit != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
                            new Vector3(col.bounds.size.x * range,col.bounds.size.y, col.bounds.size.z));
    }

}

