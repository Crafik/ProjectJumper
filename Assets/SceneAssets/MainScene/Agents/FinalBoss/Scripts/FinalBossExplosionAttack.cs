using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossExplosionAttack : MonoBehaviour
{
    // looks good for now
    public SpriteRenderer m_sprite;
    public AnimationClip m_animClip;

    public float followTimer;
    public float detonationTimer;

    public float spriteOpacity;

    void Awake(){
        m_animClip.wrapMode = WrapMode.Once;
        m_sprite.color = new Color(1f, 1f, 1f, 0f);
        followCounter = followTimer;
        detonationCounter = detonationTimer;
    }

    float followCounter;
    float detonationCounter;
    void Update(){
        m_sprite.color = new Color(1f, 1f, 1f, spriteOpacity);
        if (followCounter > 0 && GameManager.Instance.isPlayerActive){
            transform.position = GameManager.Instance.Player.transform.position;
            followCounter -= Time.deltaTime;
        }
        if (followCounter < 0){
            detonationCounter -= Time.deltaTime;
        }
        if (detonationCounter < 0){
            Instantiate(GameManager.Instance.mediumExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
