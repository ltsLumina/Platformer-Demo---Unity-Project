#region
using UnityEngine;
#endregion

public class B_Enemy3 : MonoBehaviour
{
    [SerializeField] Transform rayCast;
    //put player in raycastMask
    [SerializeField] LayerMask raycastMask;
    [SerializeField] float rayCastLength;
    [SerializeField] float attackDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] float attackTimer;
    [SerializeField] Transform leftLimit;
    [SerializeField] Transform rightLimit;

    bool attackMode;
    bool cooling;
    //private Animator anim;
    float distance;
    RaycastHit2D hit;
    bool inRange;
    float intTimer;
    Transform target;

    void Awake()
    {
        SelectTarget();
        intTimer = attackTimer;
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!attackMode) Move();

        if (!InsideofLimits() && !inRange) SelectTarget();

        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        if (hit.collider      != null) EnemyLogic();
        else if (hit.collider == null) inRange = false;

        if (inRange == false) StopAttack();
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            target  = trig.transform;
            inRange = true;
            Flip();
        }
    }
    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance) StopAttack();
        else if (attackDistance >= distance && cooling == false) Attack();

        if (cooling) cooldown();
        //anim.SetBool("Attack", false);
    }
    void Move()
    {
        Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    void Attack()
    {
        attackTimer = intTimer;
        attackMode  = true;

        //anim.SetBool("isMoving", false);
        //anim.SetBool("Attack", true);
    }
    void cooldown()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && cooling && attackMode)
        {
            cooling     = false;
            attackTimer = intTimer;
        }
    }
    void StopAttack()
    {
        cooling    = false;
        attackMode = false;
        //anim.SetBool("Attack", false);
    }
    void RaycastDebugger()
    {
        if (distance > attackDistance) Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        else if (attackDistance > distance)
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
    }

    public void TriggerCooling() { cooling = true; }

    bool InsideofLimits() =>
        transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;

    void SelectTarget()
    {
        float distanceToLeft  = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight) target = leftLimit;
        else target                                  = rightLimit;

        Flip();
    }
    void Flip()
    {
        Vector3 rotation = transform.eulerAngles;

        if (transform.position.x > target.position.x) rotation.y = 180f;
        else rotation.y                                          = 0f;

        transform.eulerAngles = rotation;
    }
    // private void ProcessHit(B_DamageDealer damageDealer)
    // {
    //     health -= damageDealer.GetDamage();
    //     damageDealer.Hit();
    //
    //     if (health <= 0)
    //     {
    //         Die();
    //     }
    // }

    void Die() { Destroy(gameObject); }
}