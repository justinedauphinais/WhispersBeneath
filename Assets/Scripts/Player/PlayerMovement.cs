using UnityEngine;

/// <summary>
/// This script provides jumping and movement for the main character
/// </summary>

public class Player : MonoBehaviour
{
    [Header("Camera rotation")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Camera mainCamera;
    private float verticalRotation = 0f;

    [Header("Ground movement")]
    [SerializeField] private float MoveSpeed = 5f;
    private Rigidbody rb;
    private float moveHorizontal;
    private float moveForward;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 10f;         /// Force to apply when jumping
    [SerializeField] private float fallMultiplier = 2.5f;   /// Multiplies gravity when falling down
    [SerializeField] private float ascendMultiplier = 2f;   /// Multiplies gravity for ascending to peak of jump
    [SerializeField] private LayerMask groundLayer;         /// Layer of the ground
    private bool isGrounded = true;
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float raycastDistance;

    /// <summary>
    /// Initialize the parameters
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Set the raycast to be slightly beneath the player's feet
        raycastDistance = ((GetComponent<CapsuleCollider>().height * transform.localScale.y) / 2) + 0.2f;

        // Hides the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Movement detection & jumping logic
    /// </summary>
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward = Input.GetAxisRaw("Vertical");

        RotateCamera();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Checking when we're on the ground and keeping track of our ground check delay
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
        }
        else
        {
            groundCheckTimer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// Moving depending on the Update() calculations
    /// </summary>
    void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpPhysics();
    }

    /// <summary>
    /// Moving the player
    /// </summary>
    void MovePlayer()
    {
        rb.linearVelocity = new Vector3(((transform.right * moveHorizontal + transform.forward * moveForward).normalized * MoveSpeed).x,
                                        rb.linearVelocity.y,
                                        ((transform.right * moveHorizontal + transform.forward * moveForward).normalized * MoveSpeed).z);

        // If we aren't moving and are on the ground, stop velocity so we don't slide
        if (isGrounded && moveHorizontal == 0 && moveForward == 0)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    /// <summary>
    /// Rotation of the camera logic & application
    /// </summary>
    void RotateCamera()
    {
        transform.Rotate(0, (Input.GetAxis("Mouse X") * mouseSensitivity), 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -20f, 20f);

        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    /// <summary>
    /// Jump application
    /// </summary>
    void Jump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); // Initial burst for the jump
    }

    /// <summary>
    /// Gravity & jump force application
    /// </summary>
    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)        // Falling - Apply fall multiplier to make descent faster
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)   // Rising - Change multiplier to make player reach peak of jump faster
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime;
        }
    }
}
