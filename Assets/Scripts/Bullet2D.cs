using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        transform.Translate(direction * distanceThisFrame, Space.World);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Destroy(gameObject);
    }
}
