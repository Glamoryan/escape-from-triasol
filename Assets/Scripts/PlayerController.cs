using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool facingRight = true;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // Yatay hareket varsa karakterin yönünü değiştir
        if (moveInput.x != 0)
        {
            FlipCharacter(moveInput.x > 0);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void FlipCharacter(bool faceRight)
    {
        // Eğer mevcut yön ile istenen yön aynı değilse
        if (facingRight != faceRight)
        {
            facingRight = faceRight;
            
            // SpriteRenderer varsa onu kullan
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !facingRight;
            }
            else
            {
                // Yoksa transform'u kullan
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}
