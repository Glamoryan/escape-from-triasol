using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float energyCostPerSecond = 20f;
    public bool facingRight = true;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private bool isSprinting;

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

        if (moveInput.x != 0)
        {
            FlipCharacter(moveInput.x > 0);
        }

        // Koşma kontrolü
        isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        if (isSprinting && EnergyBar.Instance != null)
        {
            float energyCost = energyCostPerSecond * Time.deltaTime;
            if (!EnergyBar.Instance.TryUseEnergy(energyCost))
            {
                isSprinting = false;
            }
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        rb.MovePosition(rb.position + moveInput * currentSpeed * Time.fixedDeltaTime);
    }

    void FlipCharacter(bool faceRight)
    {
        if (facingRight != faceRight)
        {
            facingRight = faceRight;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !facingRight;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}
