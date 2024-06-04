using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Tilemaps;


public enum States{
    idle,
    run,
    jump
}

// In dare need of recoding the gun behaviour
// Will probably remove gun_puckup

public class CharacterController2D : MonoBehaviour
{
    private bool isActive = false;

    public delegate void playerDeathEvent();
    public static event playerDeathEvent OnPlayerDeath;

    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravityScale = 2.5f;
    [SerializeField] private float lowJumpGravScale = 1f;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] private float autoFireCooldown = 0.125f;
    private float autoFireCounter;

    public bool facingRight = true;
    private Vector2 moveVector = Vector2.zero;
    private bool isGrounded = false;
    private bool isJumpPerforming = false;
    private Collider2D platformUnder = null;
    private bool isGunActive = false;
    private bool isFirePerforming = false;
    
    Transform tr;
    private Rigidbody2D body;
    private CapsuleCollider2D mainCollider;
    private Animator anim;

    public GameObject bulletPrefab;
    public GameObject gunPoint;
    public GameObject gunSprite;
    public GameObject followPoint;

    public PlayerAudio audioplayer;

    public CinemachineImpulseSource impulser;

    private GameObject[] bulletsArray;
    public int maxBulletCount;

    private Controls controls;

    private States State{
        get {return (States)anim.GetInteger("state");}
        set {anim.SetInteger("state", (int)value);}
    }

    public void SetGunActive(bool Flag){
        gunSprite.GetComponent<SpriteRenderer>().enabled = Flag;
        isGunActive = Flag;
        GameManager.Instance.isPlayerGunActive = Flag;
    }

    private void Start(){
        isActive = true;

        anim = GetComponent<Animator>();
        tr = transform;
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = gravityScale;
        mainCollider = GetComponent<CapsuleCollider2D>();
        mainCollider.excludeLayers = LayerMask.GetMask("Hazards", "Enemies");
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        facingRight = true;
    }

    void Awake(){
        SetGunActive(false);
        controls = new Controls();
    }

    void OnEnable(){
        controls.Enable();

        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
        controls.Player.Jump.canceled += OnJumpCanceled;
        controls.Player.Fire.performed += OnFirePerformed;
        controls.Player.Fire.canceled += OnFireCanceled;
    }

    void OnDisable(){
        controls.Disable();

        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Jump.canceled -= OnJumpCanceled;
        controls.Player.Fire.performed -= OnFirePerformed;
        controls.Player.Fire.canceled -= OnFireCanceled;
    }
    
    #region InputSystem events

    private void OnMovePerformed(InputAction.CallbackContext context){
        moveVector = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context){
        moveVector = Vector2.zero;
    }

    // I don't really like how i made this Jump here, but it implements new input system and works, so i'll let it be
    private void OnJumpPerformed(InputAction.CallbackContext context){
        // Dropping down from platforms works, but looks like it was welded by hopes and tape
        // Hope it won't break on me sometime

        // Probably should remake it into falling through platforms on down press
        if ((platformUnder != null) && (moveVector.y < 0)){
            Physics2D.IgnoreCollision(mainCollider, platformUnder, true);
            StartCoroutine(EnableCollider(platformUnder));
        }
        else{
            jumpBufferCounter = jumpBufferTime;
            isJumpPerforming = true;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context){
        isJumpPerforming = false;
    }

    private void OnFirePerformed(InputAction.CallbackContext context){
        if (isGunActive){
            isFirePerforming = true;
        }
    }

    private void OnFireCanceled(InputAction.CallbackContext context){
        isFirePerforming = false;
        autoFireCounter = 0;
    }

    // All i can say is, that if i did the inputs this way from the start it would've(probably)
    // been better than this
    #endregion All InputSystem events, methods that called by InputSystem in another place

    private IEnumerator EnableCollider(Collider2D cldr){
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreCollision(mainCollider, cldr, false);
    }

    public float terminalVelocity;
    private void Update(){
        if (!GameManager.Instance.isPaused){
            if (isActive){

                // Now somewhere here have to appear handle for dropping down on platforms
                // but i guess there is gonna be a big renovations anyways

                if (isGrounded){
                    if (moveVector.x == 0){
                        State = States.idle;
                    }
                    else{
                        State = States.run;
                    }
                }
                else{
                    State = States.jump;
                }

                if (moveVector.x != 0){
                    if ((moveVector.x > 0) && !facingRight){
                        facingRight = true;
                        tr.eulerAngles = new Vector3(0, 0, 0);
                    }
                    if ((moveVector.x < 0) && facingRight){
                        facingRight = false;
                        tr.eulerAngles = new Vector3(0, 180, 0);
                    }
                }

                if (isGrounded){
                    coyoteCounter = coyoteTime;
                }
                else{
                    coyoteCounter -= Time.deltaTime;
                }

                if (isJumpPerforming){
                    jumpBufferCounter -= Time.deltaTime;
                }
                else{
                    jumpBufferCounter = 0;
                }

                if ((jumpBufferCounter > 0) && (coyoteCounter > 0)){
                    body.velocity = new Vector2(body.velocity.x, jumpForce);
                    jumpBufferCounter = 0;
                    audioplayer.PlayJump();
                }
                if (isJumpPerforming && (body.velocity.y > 0)){
                    body.gravityScale = lowJumpGravScale;
                    coyoteCounter = 0;
                }
                else{
                    body.gravityScale = gravityScale;
                }

                // Trying to cap terminal velocity
                // seems like works
                if (body.velocity.y < terminalVelocity){
                    body.velocity = new Vector2(body.velocity.x, terminalVelocity);
                }

                if (isFirePerforming){
                    if (autoFireCounter <= 0){
                        GunShot(facingRight);
                        autoFireCounter = autoFireCooldown;
                    }
                    else{
                        autoFireCounter -= Time.deltaTime;
                    }
                }
            }
        }
        bulletsArray = GameObject.FindGameObjectsWithTag("PlayerBullet");
    }

    float currentVelocity = 0f;
    float currentAscendVelocity = 0f;
    private void FixedUpdate(){
        if (!GameManager.Instance.isPaused){
            if (isActive){
                Bounds colliderBounds = mainCollider.bounds;
                float colliderRadius = mainCollider.size.x * 0.1f * Mathf.Abs(tr.localScale.x);
                Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
                
                // Works, hope it will not cause unexpected behaviour
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
                // isGrounded = colliders.Length > 0;
                if (colliders.Length > 0){
                    isGrounded = true;
                    if (colliders[0].gameObject.layer == LayerMask.NameToLayer("Platforms")){
                        platformUnder = colliders[0];
                    }
                    else{
                        platformUnder = null;
                    }
                }
                else{
                    platformUnder = null;
                    isGrounded = false;
                }

                // Reworked movement works
                // hope it stays this way
                float newVelocity = moveVector.x * maxSpeed;
                body.velocity = new Vector2(Mathf.SmoothDamp(body.velocity.x, newVelocity, ref currentVelocity, 0.1f), body.velocity.y);
            }
            if (isFinished){
                // here be ascend code
                body.velocity = new Vector2(0f, Mathf.SmoothDamp(body.velocity.y, 10f, ref currentAscendVelocity, 2f)); // kinda ok
                body.AddTorque(50f * Time.deltaTime); // works
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        // Check isActive to prevent player jiggling by enemies
        // May revert, coz i think it's funny
        if (collision.CompareTag("Enemy")){
            PlayerDeath();
        }
        if (collision.CompareTag("Key")){
            collision.GetComponent<KeyBehaviour>().PickedUp(followPoint);
        }
    }

    private void GunShot(bool toRight){
        if (isGunActive){
            if (bulletsArray.Length < maxBulletCount){
                GameObject bulletInstance = Instantiate(bulletPrefab, gunPoint.transform.position, Quaternion.identity);
                BulletController Controller = bulletInstance.GetComponent<BulletController>();
                Controller.SetDirection(toRight);
                audioplayer.PlayShoot();
            }
        }
    }

    // Works good. Maybe add a screenshake on death later
    public void PlayerDeath(){
        if (isActive){
            isActive = false;
            mainCollider.enabled = false;
            body.freezeRotation = false;
            body.AddForce(new Vector2(Random.Range(-7f, 7f), Random.Range(8f, 12f)), ForceMode2D.Impulse);
            body.AddTorque(Random.Range(-30f, 30f), ForceMode2D.Impulse);
            impulser.GenerateImpulse();
            audioplayer.PlayDeath();
            audioplayer.DisableStep();  // Probably not the best way to fix postmortem stepping, but works. Hope it won't bite me some day;
            OnPlayerDeath();
        }
    }

    bool isFinished = false;
    public void PlayerFinish(){
        // here be code for player ascend after entering portal
        if (isActive){
            isActive = false;
            isFinished = true;
            mainCollider.enabled = false;
            body.freezeRotation = false;
            body.gravityScale = 0f;
            body.velocity = Vector3.zero;
            GameManager.Instance.DisableFollow();
        }
    }
}