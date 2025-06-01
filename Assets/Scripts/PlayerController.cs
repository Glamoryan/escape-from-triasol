using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float energyCostPerSecond = 20f;
    public bool facingRight = true;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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

        isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero;
        if (isSprinting && EnergyBar.Instance != null)
        {
            float energyCost = energyCostPerSecond * Time.deltaTime;
            if (!EnergyBar.Instance.TryUseEnergy(energyCost))
            {
                isSprinting = false;
            }
        }

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        float speedForAnim = moveInput.magnitude * currentSpeed;
        animator.SetFloat("Speed", speedForAnim);
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

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
        transform.localScale = scale;
        }
    }

}
