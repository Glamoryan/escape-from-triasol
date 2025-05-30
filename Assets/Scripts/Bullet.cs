using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;

    public Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
