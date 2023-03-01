#region
using UnityEngine;
#endregion

public class Enemy_Bullet : MonoBehaviour
{
    [SerializeField] float speed;

    Rigidbody2D bulletRB;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        bulletRB = GetComponent<Rigidbody2D>();
        target   = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDirection = (target.transform.position - transform.position).normalized * speed;
        bulletRB.velocity = new Vector2(moveDirection.x, moveDirection.y);
        Destroy(gameObject, 2);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    // {
    //  if (other.gameObject.tag == "Player")
    //  {
    //      other.gameObject.GetComponent<Player>().TakeDamage(damage);
    // }
    //  Destroy(gameObject);
    // }
}