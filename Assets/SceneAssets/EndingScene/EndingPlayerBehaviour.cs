using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingPlayerBehaviour : MonoBehaviour
{
    private delegate void ScriptDelegate();
    ScriptDelegate m_scriptDelegate;

    public EndingManager m_manager;

    public Animator m_anim;
    public GameObject camFollow;
    public CapsuleCollider2D mainCollider;
    public Rigidbody2D m_body;


    Bounds colliderBounds;
    float colliderRadius;

    bool isGrounded;
    bool isFacingRight = true;

    private States State{
        get {return (States)m_anim.GetInteger("state");}
        set {m_anim.SetInteger("state", (int)value);}
    }
    
    void Start(){
        colliderRadius = mainCollider.size.x * 0.1f * Mathf.Abs(transform.localScale.x);
        m_scriptDelegate = FallingStage;
    }

    void Update(){
        if (isGrounded){
            if (m_body.velocity.x != 0f){
                State = States.run;
            }
            else{
                State = States.idle;
            }
        }
        else{
            State = States.jump;
        }
    }

    void FixedUpdate(){
        Vector3 groundCheckPos = mainCollider.bounds.min + new Vector3(mainCollider.bounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius, LayerMask.GetMask("Ground"));
        isGrounded = colliders.Length > 0;

        if (m_body.velocity.y < -10f){
            m_body.velocity = new Vector2(m_body.velocity.x, -10f);
        }

        // here be script delegate
        m_scriptDelegate();
    }

    void FlipHorizontally(bool right){
        isFacingRight = right;
        transform.eulerAngles = new Vector3(0, right ? 0 : 180, 0);
    }

    int scriptStage = 0;
    void EndingScript(){
        switch (scriptStage){
            case 0:
                // falling in
                break;
            case 1:
                // landed, looking around
                break;
            case 2:
                // starts moving to the right
                break;
            case 3:
                // stops at pier, cam flies a bit further
                break;
        }
    }

    void FallingStage(){
        if (isGrounded){
            m_scriptDelegate = LandedStage;
        }
    }

    float lookAroundCounter = 0f;
    int LandedStageCounter = 0;
    void LandedStage(){
        lookAroundCounter += Time.deltaTime;
        if (lookAroundCounter > 0.7f){
            if (LandedStageCounter > 3){
                m_scriptDelegate = MovingStage;
            }
            else{
                FlipHorizontally(!isFacingRight);
                lookAroundCounter = 0f;
                LandedStageCounter += 1;
            }
        }
    }

    float currentVelocity = 0f;
    void MovingStage(){
        m_body.velocity = new Vector2(Mathf.SmoothDamp(m_body.velocity.x, 5f, ref currentVelocity, 0.1f), 0f);
    }
    public void NextStageTrigger(){
        startXPos = camFollow.transform.position.x;
        m_scriptDelegate = PierStage;
        m_body.velocity = Vector2.zero;
    }

    float startXPos;
    void PierStage(){
        if (camFollow.transform.position.x < startXPos + 12f){
            camFollow.GetComponent<Rigidbody2D>().velocity = new Vector3(5f, 0f, 0f);
        }
        else{
            camFollow.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_manager.SetFinTextActive();
            m_scriptDelegate = () => {};
        }
    }
}
