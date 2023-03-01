#region
using UnityEngine;
#endregion

public class B_Enemy2 : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] float lineOfSite = 30;
    [SerializeField] float shootingRange = 15;
    [SerializeField] float fireRate = 1f;

    [SerializeField] float nextFireTime;

    [SerializeField] GameObject Enemy_Bullet;
    [SerializeField] GameObject bulletParent;

    Transform player;
    // Start is called before the first frame update
    void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer < lineOfSite && distanceFromPlayer > shootingRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distanceFromPlayer <= shootingRange && nextFireTime < Time.time)
        {
            Instantiate(Enemy_Bullet, bulletParent.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    // private void ProcessHit(B_DamageDealer damageDealer)
    // {
    //     health -= damageDealer.GetDamage();
    //     damageDealer.Hit();
    //
    //     if (health <= 0)
    //     {
    //        Die();
    //     }
    // }

    void Die() { Destroy(gameObject); }
}