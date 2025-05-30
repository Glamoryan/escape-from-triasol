using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;

    [HideInInspector] public Vector2 direction;
    [HideInInspector] public GameObject owner;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (direction != Vector2.zero)
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
            return;

        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
