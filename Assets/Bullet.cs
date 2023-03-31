#region
using System.Collections;
using UnityEngine;
#endregion

public class Bullet : MonoBehaviour
{
    new Collider2D collider2D;
    [SerializeField] AudioClip laughSound;

    void Awake()
    {
        collider2D = GetComponent<Collider2D>();

    }

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<AudioSource>().PlayOneShot(laughSound);
            Destroy(other.gameObject);

            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
        //if (collider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) Destroy(gameObject);
    }
}