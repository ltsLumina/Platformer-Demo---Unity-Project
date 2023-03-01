#region
using UnityEngine;
#endregion

public class B_Enemy : MonoBehaviour
{
    // Configurable parameters
    [SerializeField] float health = 20f;
    [SerializeField] float moveSpeed = 1f;

    // Private variables
    float currentHealth;

    // Cached references
    Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        myRigidbody   = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() { myRigidbody.velocity = new Vector2(moveSpeed, 0f); }

    void OnTriggerExit2D(Collider2D other)
    {
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing() { transform.localScale = new Vector2(-Mathf.Sign(myRigidbody.velocity.x), 1f); }

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