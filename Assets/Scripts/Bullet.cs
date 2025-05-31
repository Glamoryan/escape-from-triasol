using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifetime = 5f;

    [HideInInspector] public Vector2 direction;
    [HideInInspector] public GameObject owner;

    private void Start()
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

        if (collision.CompareTag("Item") || collision.CompareTag("Spaceship") || collision.CompareTag("Structure"))
            return;

        if (owner.CompareTag("Player") && collision.CompareTag("Structure"))
            return;

        if (owner.CompareTag("Structure") && collision.CompareTag("Player"))
            return;

        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
