using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FirstBossBombBehaviour : MonoBehaviour
{
    public Animator m_anim;
    public CircleCollider2D mainCollider;

    float colliderRadius;
    bool isAirborn;

    void Awake(){
        colliderRadius = mainCollider.radius * 0.4f * Mathf.Abs(transform.localScale.x);
        isAirborn = true;
    }

    void Update(){
        if (isAirborn){
            Vector3 groundCheckPos = mainCollider.bounds.min + new Vector3(mainCollider.bounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius, LayerMask.GetMask("Ground", "Platforms"));
            if (colliders.Length > 0){
                isAirborn = false;
                m_anim.Play("firstboss_bomb_timer");
            }
        }
    }

    void AnimFinished(){
        Instantiate(GameManager.Instance.mediumExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
