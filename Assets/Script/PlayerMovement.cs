using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData playerData;

    [Header("Movement")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 35f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private float groundCheckDistance = 0.25f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private Transform groundCheckOrigin;

    private Rigidbody rb;

    private Vector2 moveInput;
    private bool isSprinting;

    private float currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (groundCheckOrigin == null)
            groundCheckOrigin = transform;

        if (playerData != null)
            currentHealth = playerData.maxHealth;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // -------------------- INPUT SYSTEM (Send Messages) --------------------
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    public void OnJump()
    {
        if (!IsGrounded()) return;

        // Reset Y velocity for consistent jump
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump");
    }

    public void OnPrimaryAttack()
    {
        Debug.Log("PrimaryAttack");
    }

    public void OnSecondaryAttack()
    {
        Debug.Log("SecondaryAttack");
    }

    public void OnObject()
    {
        Debug.Log("Object / Interact");
    }

    // -------------------- MOVEMENT --------------------
    private void HandleMovement()
    {
        if (playerData == null) return;

        float speed = isSprinting ? playerData.sprintSpeed : playerData.normalSpeed;

        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * speed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        float accel = (targetVelocity.sqrMagnitude > 0.0001f) ? acceleration : deceleration;

        Vector3 newHorizontal = Vector3.MoveTowards(
            currentHorizontal,
            targetVelocity,
            accel * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newHorizontal.x, currentVelocity.y, newHorizontal.z);
    }

    // -------------------- GROUND CHECK --------------------
    private bool IsGrounded()
    {
        Vector3 origin = groundCheckOrigin.position + Vector3.up * 0.05f;

        // SphereCast down for stable ground detection
        return Physics.SphereCast(
            origin,
            groundCheckRadius,
            Vector3.down,
            out _,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    // -------------------- HEALTH --------------------
    public void TakeDamage(float amount)
    {
        if (playerData == null) return;

        currentHealth -= amount;
        Debug.Log($"TakeDamage: -{amount} (HP: {currentHealth}/{playerData.maxHealth})");

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (playerData == null) return;

        currentHealth = Mathf.Min(currentHealth + amount, playerData.maxHealth);
        Debug.Log($"Heal: +{amount} (HP: {currentHealth}/{playerData.maxHealth})");
    }

    private void Die()
    {
        Debug.Log("mort");
        // TODO: animations / respawn / disable input
    }

    // Optionnel: pratique pour debug dans l’inspector
    public float GetCurrentHealth() => currentHealth;
}
