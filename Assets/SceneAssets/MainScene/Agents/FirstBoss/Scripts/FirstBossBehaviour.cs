using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstBossBehaviour : MonoBehaviour
{
    
    delegate void StateDelegate();
    StateDelegate currentStateAction;
    StateDelegate nextStateAction;


    public bool isInEnding;
    public bool endingFacingRight;


    public int healthPoints = 150;

    public float moveSpeed;
    public float dashSpeed;

    private float facingDirection; // Positive = right
    
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private CapsuleCollider2D mainCollider;
    [SerializeField] private BoxCollider2D legsCollider;
    [SerializeField] private Animator anim;
    [SerializeField] private CinemachineImpulseSource impulser;
    [SerializeField] private FirstBossAudioHandler Audioplayer;

    public GameObject missilePrefab;
    public GameObject bombPrefab;
    public GameObject genericEnemyPrefab;

    #region Animation states

    const string BOSS_IDLE = "firstboss_idle";
    const string BOSS_MOVE = "firstboss_move";
    const string BOSS_DASH_PREP = "firstboss_dash_prep";
    const string BOSS_DASH_ACT = "firstboss_dash_active";
    const string BOSS_FIRE = "firstboss_fire";
    const string BOSS_JUMP = "firstboss_jump";
    const string BOSS_AIR = "firstboss_airborn";
    const string BOSS_LAND = "firstboss_land";

    #endregion

    string currentAnimState;

    public float cycleDuration;
    public float cycleCounter;

    // Arena borders
    private float leftBorder;
    private float rightBorder;
    private float sixthOfArena;

    // Physics(?)
    private float colliderRadius;

    BossFightTrigger figthManager; // typo. let it be

    void Start(){
        colliderRadius = mainCollider.size.y * 0.5f * Mathf.Abs(transform.localScale.y);
        if (!isInEnding){
            FlipHorizontal(GameManager.Instance.Player.transform.position.x > transform.position.x);
            currentStateAction = StartHandler;
        }
        else{
            FlipHorizontal(endingFacingRight);
            ChangeAnimation(BOSS_IDLE);
        }
    }

    public void init(float leftB, float rightB, BossFightTrigger bft){
        leftBorder = leftB + 0.25f;
        rightBorder = rightB - 0.25f;
        sixthOfArena = (rightBorder - leftBorder) / 6;
        figthManager = bft;
    }

    void FixedUpdate(){
        // Physics
        if (isInEnding){
            return;
        }
        
        // Here be physics checks(probably)
        // Kinda have to check for active player for things to work properly
        if (GameManager.Instance.isPlayerActive){
            currentStateAction();
        }
    }
    
    public void GetDamage(){
        healthPoints--;

        Audioplayer.PlayHurt();

        if (healthPoints < 1){
            // Placeholder
            // here be death routine
            mainCollider.enabled = false;
            legsCollider.enabled = false;
            transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(Random.Range(-7f, 7f), Random.Range(5f, 7f)), ForceMode2D.Impulse);
            Audioplayer.PlayDeath();
            despawnTimer = 2f;
            currentStateAction = DeathHandler;
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.GetComponent<CharacterController2D>().PlayerDeath();
        }
    }

    private bool CheckIsGrounded(){
        Vector3 groundCheckPos = mainCollider.bounds.min + new Vector3(mainCollider.bounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        
        // Works, hope it will not cause unexpected behaviour
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
        return colliders.Length > 0;
    }

    private bool CheckInFront(){
        // Not sure what i did here, but hope it will work. It should. Probably
        Vector3 inFrontCheckPos = mainCollider.bounds.center + new Vector3(mainCollider.bounds.extents.x * facingDirection, mainCollider.bounds.extents.y, 0);

        Collider2D[] colliders;
        // Works, hope it will not cause unexpected behaviour
        colliders = Physics2D.OverlapCircleAll(inFrontCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
        if (colliders.Length > 0){
            return true;
        }
        return false;
    }

    void ChangeAnimation(string name){
        if (currentAnimState != name){
            anim.Play(name);
            currentAnimState = name;
        }
    }

    void FlipHorizontal(bool right){
        facingDirection = right ? 1f : -1f;
        transform.eulerAngles = new Vector3(0, right ? 0 : 180, 0);
    }

    #region Behaviour
    void StartHandler(){
        ChangeAnimation(BOSS_AIR);
        if (CheckIsGrounded()){
            LandFX();
            currentStateAction = LandHandler;
        }
    }

    void LandHandler(){
        // is stoopid but works, and "well" integrated
        ChangeAnimation(BOSS_LAND);
    }

    void LandTransition(){
        IdleCounter = Random.Range(1.5f, 2f);
        nextStateAction = Random.value > 0.5f ? DashPrepHandler : ShotHandler;
        currentStateAction = IdleHandler;
    }


    float IdleCounter;
    void IdleHandler(){
        // Idle:
        // Wait a bit in place "thinking" about next move
        //
        // This is the entry point of "statemachine"
        // Probably the central point of "statemachine".

        // Maybe i'll use a stack of last 5 actions to determine next action. Maybe
        ChangeAnimation(BOSS_IDLE);

        FlipHorizontal(GameManager.Instance.Player.transform.position.x > transform.position.x);

        if (IdleCounter < 0){

            // Probably works, but dunno
            currentStateAction = nextStateAction;
        }
        IdleCounter -= Time.deltaTime;
    }

    #region Move
    void NextDestDecision(){
        float playerX = GameManager.Instance.Player.transform.position.x;
        bool toRight = playerX > transform.position.x;
        List<float> positions = new List<float>();

        if (playerX - leftBorder > sixthOfArena * 2){
            positions.Add(((playerX - leftBorder) / 2) + leftBorder);
        }
        if (rightBorder - playerX > sixthOfArena * 2){
            positions.Add(((rightBorder - playerX) / 2) + leftBorder);
        }
        positions.Add(playerX);
        positions.Add((Mathf.Abs(playerX - transform.position.x) / 2) + (toRight ? transform.position.x : playerX));
        positions.Add(leftBorder + (rightBorder - leftBorder) / 2);
        positions.Add(leftBorder + sixthOfArena);
        positions.Add(leftBorder);
        positions.Add(rightBorder - sixthOfArena);
        positions.Add(rightBorder);

        moveDestination = positions[Random.Range(0, positions.Count)];
    }

    void NextMoveDesicion(){
        // Random bullshit go!

        StateDelegate[] actions = {
            ShotHandler,
            JumpHandler,
            DashPrepHandler
        };
        nextStateAction = actions[Random.Range(0, 3)];
    }

    void MoveStartHandler(){
        NextDestDecision();
        NextMoveDesicion();
        currentStateAction = MoveHandler;
    }

    float moveDestination;
    void MoveHandler(){
        // Move
        // Behaviour:
        // Either move to closest border, or towards player some distance
        // Decision made by random(probably);
        // Most likely to dictate next action
        // Desicion made in the IdleHandler

        ChangeAnimation(BOSS_MOVE);
        FlipHorizontal(moveDestination > transform.position.x);

        // here be direction
        if (Mathf.Abs(transform.position.x - moveDestination) < 2 ){
            // here be transition to next action
            IdleCounter = Random.Range(1f, 1.5f);
            currentStateAction = IdleHandler;
        }

        body.velocity = new Vector2(facingDirection * moveSpeed, body.velocity.y);
    }
    #endregion

    #region Dash
    // Distance from player when dash stops
    // Need to mind starting distance from player
    public float DashCancelDistance;
    bool DashPlayerDir;
    bool DashIsStopping;
    float DashCurrentSpeed;
    public float DashSlowdownRate;
    void DashPrepHandler(){
        ChangeAnimation(BOSS_DASH_PREP);
    }
    void DashStep(){
        DashCurrentSpeed = dashSpeed;
        DashIsStopping = false;
        DashPlayerDir = GameManager.Instance.Player.transform.position.x > transform.position.x;
        currentStateAction = DashActHandler;
    }
    void DashActHandler(){
        ChangeAnimation(BOSS_DASH_ACT);
        float distanceToPlayer = Vector2.Distance(GameManager.Instance.Player.transform.position, transform.position);
        if (CheckInFront()){
            impulser.m_DefaultVelocity = new Vector3(0.25f, 0, 0);
            impulser.GenerateImpulse();

            Audioplayer.PlayLand();

            IdleCounter = Random.Range(1.75f, 2.5f);
            currentStateAction = IdleHandler;
            nextStateAction = MoveStartHandler;
        }
        if ((DashPlayerDir != GameManager.Instance.Player.transform.position.x > transform.position.x && distanceToPlayer > DashCancelDistance) || DashIsStopping){
            DashIsStopping = true;
            if (DashCurrentSpeed - DashSlowdownRate < 0){
                if (Random.value > 0.5f){
                    body.velocity = new Vector2(0, body.velocity.y);
                    IdleCounter = Random.Range(0.5f, 1.25f);
                    currentStateAction = IdleHandler;
                    nextStateAction = MoveStartHandler;
                }
                else{
                    currentStateAction = ShotHandler;
                }
            }
            DashCurrentSpeed -= DashSlowdownRate;
        }

        body.velocity = new Vector2(facingDirection * DashCurrentSpeed, body.velocity.y);
    }
    #endregion

    #region Shot
    void ShotHandler(){
        // Shoot:
        // Need to properly telegraph the shot
        ChangeAnimation(BOSS_FIRE);
        FlipHorizontal(GameManager.Instance.Player.transform.position.x > transform.position.x);
    }
    public float bombUpForce;
    public float bombSideForce;
    public float bombSideForceStep;
    public float minionUpForce;
    public float minionSideForce;
    public float minionSideForceStep;
    void ShotEvent(){
        // will be called from animation(probably a good way to implement this)
        // here instantiate boolet(s)
        
        // Several types of attacks i guess, smth like:
        // 1 - (?) Spawn = spawn 2-4 generic enemies with pursuit behav. and 2 maybe 3 hp
        // 2 - Bomb throw = 3 bombs in arc towards player, explode after a while
        // 3 - Homing missile = nuff said. :: Homing missiles probably will be either accelerating and slow to turn, or destroyable

        // everything is to be tweaked in the future
        switch (Random.Range(0, 3)){
            case 0:
                // spawn
                for (int i = 0; i < 2; ++i){
                    var minion = Instantiate(genericEnemyPrefab, transform.GetChild(1).transform.position, Quaternion.identity);
                    minion.GetComponent<EnemyBehaviour>().EnemyInit(6, 3, facingDirection > 0);
                    minion.GetComponent<Rigidbody2D>().AddForce(new Vector2((facingDirection > 0 ? 1f : -1f) * (minionSideForce + (minionSideForceStep * i)), minionUpForce));
                }
                break;
            case 1:
                // bombs
                for (int i = 0; i < 3; ++i){
                    var bomb = Instantiate(bombPrefab, transform.GetChild(1).transform.position, Quaternion.identity);
                    bomb.GetComponent<Rigidbody2D>().AddForce(new Vector2((facingDirection > 0 ? 1f : -1f) * (bombSideForce + (bombSideForceStep * i)), bombUpForce));
                }
                break;
            case 2:
                // homing
                Instantiate(missilePrefab, transform.GetChild(1).transform.position, Quaternion.Euler(new Vector3(0f, 0f, facingDirection > 0 ? 0f : 180f)));
                break;
            default:
                break;
        }

        nextStateAction = MoveStartHandler;
        IdleCounter = Random.Range(1f, 2f);
        currentStateAction = IdleHandler;
    }
    #endregion

    #region Jump
    void JumpHandler(){
        // Jump:
        // Jump after a bit of preparation
        // Play Landing animation on land
        ChangeAnimation(BOSS_JUMP);
        FlipHorizontal(GameManager.Instance.Player.transform.position.x > transform.position.x);
    }

    public float jumpUpForce;
    public float jumpAirbornTime;
    float jumpSideForce;
    void JumpEvent(){
        // Here be jump
        // Probably make that UpForce is constant, but side force will depend on player's 'x' position
        // Will calculate the SideForce based on time entity spends airborn

        body.AddForce(Vector2.up * jumpUpForce);

        float distanceToPlayer = Mathf.Abs(GameManager.Instance.Player.transform.position.x - transform.position.x);
        jumpSideForce = distanceToPlayer / jumpAirbornTime;

        launchDelay = 0.5f;
        currentStateAction = AirbornHandler;
    }

    float launchDelay; // Cringe
    void AirbornHandler(){
        ChangeAnimation(BOSS_AIR);
        body.velocity = new Vector2(jumpSideForce * facingDirection, body.velocity.y);
        launchDelay -= Time.deltaTime;
        if (CheckIsGrounded() && launchDelay < 0){
            LandFX();
            currentStateAction = LandHandler;
        }
    }

    void LandFX(){
        impulser.m_DefaultVelocity = new Vector3(0, 0.25f, 0);
        impulser.GenerateImpulse();
        Audioplayer.PlayLand();
    }
    #endregion

    float despawnTimer;
    void DeathHandler(){
        ChangeAnimation(BOSS_IDLE);
        if (despawnTimer < 0){
            figthManager.OnBossDefeated();
            Destroy(gameObject);
        }
        despawnTimer -= Time.deltaTime;
    }

    #endregion
}

