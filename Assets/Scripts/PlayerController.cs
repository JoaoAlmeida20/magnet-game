using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [Header("Run")]
    public float runMaxSpeed;
    public float speedPower;
    public float accel;
    public float deccel;
    [Range(0, 1)] public float accelAirMultiplier;
    [Range(0, 1)] public float deccelAirMultiplier;

    [Header("Jump")]
    public float jumpStrength;
    [Range(0, 1)] public float jumpShorthopMultiplier;
    [Range(0, 0.5f)] public float jumpCoyoteTime;
    [Range(0, 0.5f)] public float jumpBufferTime;
    
    [Header("Physics")]
    public float friction;
    public float fallGravityMultiplier;
    public float maxFallSpeed;
    public float rotationSpeed;

    Rigidbody2D rigidbody2d;
    SpriteRenderer spriteRenderer;
    Animator animator;
    BoxCollider2D boxCollider2d;
    MagneticObject magneticObject;

    Quaternion defaultRotation;
    Vector2 playerSize;
    Vector2 groundCheckSize;
    float gravityScale;

    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    bool isJumping;
    bool jumpReleased;
    float lastJumpedTime;
    float lastGroundedTime;
    bool fire1;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        magneticObject = GetComponent<MagneticObject>();
        defaultRotation = transform.rotation;
        playerSize = boxCollider2d.size;
        gravityScale = rigidbody2d.gravityScale;
        groundCheckSize = new Vector2(playerSize.x * transform.localScale.x - 0.05f, 0.05f);
        isJumping = false;
        lastJumpedTime = jumpBufferTime + 1.0f;
        lastGroundedTime = jumpCoyoteTime + 1.0f;
        animator.SetBool("isRed", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused) {
            return;
        }
        
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump")) {
            lastJumpedTime = 0.0f;
            jumpReleased = false;
        }
        else {
            lastJumpedTime += Time.deltaTime;
        }
        if (Input.GetButtonUp("Jump")) {
            jumpReleased = true;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            transform.position = new Vector3(0, 5, 0);
        }

        magneticObject.isOn = Input.GetButton("Fire1");
        animator.SetBool("isMagnet", magneticObject.isOn);

        if (Input.GetButtonDown("Fire2") && SceneManager.GetActiveScene().name == "End") {
            magneticObject.isRed = !magneticObject.isRed;
            animator.SetBool("isRed", magneticObject.isRed);
        }
    }

    void FixedUpdate() {
        // Grounded Check
        lastGroundedTime += Time.fixedDeltaTime;
        Vector2 groundCheckCenter = (Vector2) transform.position + (Vector2.down * (transform.localScale.y + groundCheckSize.y) / 2.0f);
        if (Physics2D.OverlapBox(groundCheckCenter, groundCheckSize, 0.0f, LayerMask.GetMask("Default")) != null) {
            lastGroundedTime = 0.0f;
        }

        // Run
        float targetSpeed = horizontal * runMaxSpeed;
        float speedDiff = targetSpeed - rigidbody2d.velocity.x;
        float accelRate;
        if (lastGroundedTime == 0.0f) {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel : deccel;
        }
        else {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel * accelAirMultiplier : deccel * deccelAirMultiplier;
        }

        // Applies acceleration to speed difference, then raises to set power so acceleration increases with higher speeds
        // Multiplies by sign to reapply direction
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, speedPower) * Mathf.Sign(speedDiff);
        if (!magneticObject.isSuck && !(Mathf.Abs(targetSpeed) != 0 && Mathf.Sign(targetSpeed) == Mathf.Sign(rigidbody2d.velocity.x) && Mathf.Abs(targetSpeed) < Mathf.Abs(rigidbody2d.velocity.x))) {
            rigidbody2d.AddForce(movement * Vector2.right);
        }

        // Jump
        if (isJumping && rigidbody2d.velocity.y < 0.0f) {
            isJumping = false;
        }

        if (!isJumping && lastJumpedTime <= jumpBufferTime) {
            if (lastGroundedTime <= jumpCoyoteTime) {
                rigidbody2d.AddForce(Vector2.up * jumpStrength * (0.8f + Mathf.Abs(rigidbody2d.velocity.x) / (runMaxSpeed * 5)) , ForceMode2D.Impulse);
                isJumping = true;
            }
        }

        if (jumpReleased && isJumping) {
            rigidbody2d.AddForce(Vector2.down * rigidbody2d.velocity * (1 - jumpShorthopMultiplier), ForceMode2D.Impulse);
            jumpReleased = false;
        }

        // Friction
        if (lastGroundedTime == 0.0f && (Mathf.Abs(horizontal) < 0.01f || Mathf.Abs(rigidbody2d.velocity.x) > runMaxSpeed)) {
            float amount = Mathf.Min(Mathf.Abs(rigidbody2d.velocity.x), friction);
            amount *= Mathf.Sign(rigidbody2d.velocity.x);
            rigidbody2d.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        // Magnet
        if (!magneticObject.isSuck) {
            float rotationMultiplier = 1;
            if (lastGroundedTime == 0) {
                rotationMultiplier = 5;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, rotationMultiplier * rotationSpeed * Time.fixedDeltaTime);
        }

        // Increased falling gravity
        if (!magneticObject.isSuck) {
            if (rigidbody2d.velocity.y < 0.0f) {
                rigidbody2d.gravityScale = gravityScale * fallGravityMultiplier;
            }
            else {
                rigidbody2d.gravityScale = gravityScale;
            }
        }

        // Max fall speed
        if (rigidbody2d.velocity.y < -maxFallSpeed) {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, -maxFallSpeed);
        }
    }
}
