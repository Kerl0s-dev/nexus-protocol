using UnityEngine;
using MiniTween.CameraTween;
using MiniTween;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody rb;

    [Header("Déplacement")]
    public float speed = 5f;
    public float jumpForce = 5f;
    Vector3 direction;

    [Header("Saut")]
    public bool canJump = true;
    public bool isGrounded = true;
    public LayerMask groundLayer;

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    float moveHorizontal, moveVertical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2 * 0.5f + 0.2f, groundLayer);

        GetInputs();
        SpeedControl();

        if (isGrounded)
            rb.linearDamping = 5f;
        else
            rb.linearDamping = 0f;
    }

    private void FixedUpdate()
    {
        if (!isDashing) // si on dash pas, déplacement normal
            rb.AddForce(direction * speed * 10, ForceMode.Force);
    }

    void GetInputs()
    {
        moveHorizontal = InputManager.Move.x;
        moveVertical = InputManager.Move.y;

        direction = transform.forward * moveVertical + transform.right * moveHorizontal;

        // Saut
        if (InputManager.Jump && isGrounded && canJump)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), 0.25f);
        }

        // Dash
        if (InputManager.Dash && canDash)
        {
            Camera.main.DoShake(ShakeType.Damage, EasingType.EaseInQuad);
            StartCoroutine(DoDash());
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        canJump = true;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > speed && !isDashing)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    // Coroutine du dash
    private System.Collections.IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        // Reset de la vitesse avant le dash pour plus de contrôle
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        // Dash dans la direction actuelle
        rb.AddForce(direction.normalized * dashForce, ForceMode.Impulse);

        Camera.main.DoFOV(80f, 0.1f, EasingType.EaseInQuad);

        yield return new WaitForSeconds(dashDuration);

        Camera.main.DoFOV(60f, 0.1f, EasingType.EaseInQuad);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}