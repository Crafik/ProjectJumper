using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

// Notes to myself:
// It would be better to handle the gun in this script as well i think

// Modes:
// 0 - OnMove           - moving from side to side on a platform
// 1 - Careless         - moving on platform ignoring pits, but not walls
// 2 - Sleep            - waiting until player enters proxymity, then start OnMove in player's direction
// 3 - Sentry           - waiting until player enters proxymity, then start Pursuit
// 4 - Jumpy            - jumps in random intervals(or not so random)
// 5 - Pursuit          - pursuits player, jumps over pits and walls

// (?) 6 - summoned - ???

// Also: Need to add death from hazards

public class EnemyBehaviour : MonoBehaviour
{
    private bool isActive = false;
    // Would be better to remake it into enum, but me be lazy ass
    [SerializeField] private int actionMode;
    delegate void ModeDelegate();
    ModeDelegate currentModeAction;

    public bool startFacingRight;

    public int healthPoints = 3;

    public float moveSpeed;
    public float jumpForce;
    public float gravityScale;
    public float lowGravityScale;
    public float jumpInterval;
    private float jumpCounter;
    private float jumpMultiplier;
    private bool isGrounded = false;
    private bool facingRight = true;
    private float moveDirection;
    
    Transform tr;
    private Rigidbody2D body;
    private CapsuleCollider2D mainCollider;
    private Animator anim;
    private GameObject proxy;

    [SerializeField] private AudioSource audioplayer;

    private float despawnTimer = 1f;
    private float despawnCounter = -1f;

    // Physics
    float colliderRadius;
    
    private States State{
        get {return (States)anim.GetInteger("state");}
        set {anim.SetInteger("state", (int)value);}
    }

    void Awake(){
        jumpCounter = 0;
        anim = GetComponent<Animator>();
        tr = transform;
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = gravityScale;
        mainCollider = GetComponent<CapsuleCollider2D>();
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        facingRight = true;
        moveDirection = 1f;
        proxy = tr.GetChild(0).gameObject;
        proxy.SetActive(false);
        isActive = true;
        ChangeMode(actionMode);
        // Works, but if stops, then need to move this into physics checks
        colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(tr.localScale.x);

        FlipHorizontal(startFacingRight);
    }

    public void EnemyInit(int Mode, int Health, bool faceRight, float jumpInt, float jumpMult){
        ChangeMode(Mode);
        healthPoints = Health;
        FlipHorizontal(faceRight);
        jumpInterval = jumpInt;
        jumpMultiplier = jumpMult;
    }
    public void EnemyInit(int Mode, int Health, bool faceRight){
        EnemyInit(Mode, Health, faceRight, 1, 1);
    }

    void Update(){
        if (!isActive){
            // Despawner looks strange but works
            if (despawnCounter > 0){
                if (despawnCounter < 1){
                    Destroy(gameObject);
                }
                despawnCounter -= Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (isActive){
            isGrounded = CheckIsGrounded();
            if (isGrounded){
                if (body.velocity.x == 0){
                    State = States.idle;
                }
                else{
                    State = States.run;
                }
            }
            else{
                State = States.jump;
            }
            currentModeAction();
        }
    }

    private void OnBecameInvisible(){
        despawnCounter = despawnTimer + 1f;
    }

    private void ChangeMode(int Mode){
        actionMode = Mode;
        // There be code to reassign delegate based on current Mode
        switch(actionMode){
            case 0: // OnMove
                isCareful = true;
                currentModeAction = MovementHandler;
                break;
            case 1: // Careless
                isCareful = false;
                currentModeAction = MovementHandler;
                break;
            case 2: // Sleep
                isCareful = false;
                proxy.SetActive(true);
                currentModeAction = () => {};
                break;
            case 3: // Sentry
                isCareful = true;
                proxy.SetActive(true);
                currentModeAction = () => {};
                break;
            case 4: // Jumpy
                body.gravityScale = lowGravityScale;
                currentModeAction = JumpingHandler;
                break;
            case 5: // Pursuit
                currentModeAction = PursuitHandler;
                break;
            case 6: // Summoned
                currentModeAction = SummonedHandler;
                break;
            default:
                break;
        }
    }

    private bool CheckIsGrounded(){
        Vector3 groundCheckPos = mainCollider.bounds.min + new Vector3(mainCollider.bounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        
        // Works, hope it will not cause unexpected behaviour
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
        return colliders.Length > 0;
    }

    private bool isCareful;
    // Checks if agent can go further. Returns true if not;
    private bool CheckInFront(){
        // Not sure what i did here, but hope it will work. It should. Probably
        Vector3 inFrontCheckPos = mainCollider.bounds.center + new Vector3(mainCollider.bounds.extents.x * moveDirection, 0, 0);
        Vector3 inFrontGroundCheckPos = mainCollider.bounds.min + new Vector3(mainCollider.bounds.size.x * 0.5f, colliderRadius * 0.9f, 0) + new Vector3(mainCollider.bounds.extents.x * moveDirection, 0, 0);
        
        Collider2D[] colliders;
        // Works, hope it will not cause unexpected behaviour
        colliders = Physics2D.OverlapCircleAll(inFrontCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
        if (colliders.Length > 0){
            return true;
        }
        // Addtional check for OnMove mode
        if (isCareful){
            colliders = Physics2D.OverlapCircleAll(inFrontGroundCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
            if (colliders.Length < 1){
                return true;
            }
        }
        return false;
    }

    private void FlipHorizontal(bool right){
        facingRight = right;
        tr.eulerAngles = new Vector3(0, right ? 0 : 180, 0);
    }

    private void MovementHandler(){
        if (CheckInFront() && isGrounded){
            moveDirection *= -1;
        }
        if (moveDirection != 0){
                if ((moveDirection > 0) && !facingRight){
                    FlipHorizontal(true);
                }
                if ((moveDirection < 0) && facingRight){
                    FlipHorizontal(false);
                }
            }
        body.velocity = new Vector2(moveDirection * moveSpeed, body.velocity.y);
    }

    private void JumpingHandler(){
        if (jumpCounter < 0){
            jumpCounter = jumpInterval;
            body.velocity = new Vector2(body.velocity.x, jumpForce * jumpMultiplier);
        }
        jumpCounter -= Time.deltaTime;
    }

    private void PursuitHandler(){
        // Good enough, may improve/remake it later
        if (!GameManager.Instance.isPlayerActive){
            ChangeMode(0);
            return;
        }
        
        moveDirection = (tr.position.x - GameManager.Instance.Player.transform.position.x) > 0 ? -1 : 1;
        // Here be altered movement + jump
        if (CheckInFront() && isGrounded){
            //here be jump
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }
        if (body.velocity.y > 0){
            body.gravityScale = lowGravityScale;
        }
        if (body.velocity.y < 0){
            body.gravityScale = gravityScale;
        }
        if (moveDirection != 0){
                if ((moveDirection > 0) && !facingRight){
                    FlipHorizontal(true);
                }
                if ((moveDirection < 0) && facingRight){
                    FlipHorizontal(false);
                }
            }
        body.velocity = new Vector2(moveDirection * moveSpeed, body.velocity.y);
    }

    private void SummonedHandler(){
        if (CheckIsGrounded()){
            ChangeMode(5);
        }
    }

    public void WakeUp(){
        moveDirection = (tr.position.x - GameManager.Instance.Player.transform.position.x) > 0 ? -1 : 1;
        if (actionMode == 2){
            ChangeMode(1);
        }
        if (actionMode == 3){
            ChangeMode(5);
        }
        proxy.SetActive(false);
    }

    public void GetDamage(){
        healthPoints--;
        if (healthPoints < 1){
            EnemyDeath();
        }
        else{
            audioplayer.volume = 0.5f;
            audioplayer.Play();
        }
    }

    public void EnemyDeath(){
        isActive = false;
        mainCollider.enabled = false;
        body.freezeRotation = false;
        body.AddForce(new Vector2(Random.Range(-7f, 7f), Random.Range(5f, 7f)), ForceMode2D.Impulse);
        body.AddTorque(Random.Range(-30f, 30f), ForceMode2D.Impulse);
        audioplayer.volume = 0.8f;
        audioplayer.Play();
    }
}
