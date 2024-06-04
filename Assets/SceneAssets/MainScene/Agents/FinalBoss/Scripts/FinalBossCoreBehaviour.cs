using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FinalBossCoreBehaviour : MonoBehaviour
{
    public FinalBossBehaviour finalBoss;
    GameObject shutter;

    public AudioSource m_audio;
    public SpriteRenderer m_spriteRenderer;
    public CapsuleCollider2D m_collider;

    public int num_ID;
    public bool shutterDirection;   // true for Up

    public int health;
    public int currentHealth;

    Vector2 shutterStartPos;
    Vector2 shutterOpenPos;

    public bool isActive = false;

    void Awake(){
        shutter = transform.GetChild(0).gameObject;
        shutterStartPos = shutter.transform.position;
        shutterOpenPos = shutterStartPos + new Vector2(0, shutterDirection ? 3f : -3f);
        GameManager.OnGameRestart += CoreReset;
    }

    float shiftProgress;
    void Update(){
        if (isShutterSwitching){
            if (shiftProgress > 1f){
                isShutterSwitching = false;
            }
            // a bit confusing, but okay
            // have an idea how to shrink this a bit, but no
            if (isShutterOpen){
                shutter.transform.position = Vector2.Lerp(shutterStartPos, shutterOpenPos, shiftProgress);
            }
            else{
                shutter.transform.position = Vector2.Lerp(shutterOpenPos, shutterStartPos, shiftProgress);
            }
            shiftProgress += Time.deltaTime * 0.5f;
        }

        if (isDestroyed && smallBlastCounter < 12){
            smallBlastTimerCounter -= Time.deltaTime;
            if (smallBlastTimerCounter < 0f){
                Vector2 randomShift = Random.insideUnitCircle.normalized * 1.3f;
                Vector3 smallBlastPos = new Vector3(transform.position.x + randomShift.x, transform.position.y + randomShift.y, transform.position.z);
                Instantiate(GameManager.Instance.smallExplosionPrefab, smallBlastPos, Quaternion.identity);
                if (smallBlastCounter % 4 == 0){
                    Instantiate(GameManager.Instance.mediumExplosionPrefab, transform);
                }
                smallBlastCounter++;
                smallBlastTimerCounter = 0.25f;
            }
        }
        if (smallBlastCounter == 12){
            m_spriteRenderer.enabled = false;
            finalBoss.CoreIsDestroyed();
            smallBlastCounter += 1;
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.GetComponent<CharacterController2D>().PlayerDeath();
        }
        if (collision.CompareTag("PlayerBullet")){
            currentHealth--;
            if (currentHealth < 1){
                DestructionSequence();
            }
            else{
                m_audio.Play();
            }
        }
    }

    public bool isShutterOpen = false;
    bool isShutterSwitching = false;
    public void SwitchShutter(){
        if (!isShutterSwitching){
            if (isShutterOpen){
                isShutterOpen = false;
            }
            else{
                isShutterOpen = true;
            }
            shiftProgress = 0f;
            isShutterSwitching = true;
        }
    }

    public void CoreReset(){
        currentHealth = health;
        isShutterOpen = false;
        isShutterSwitching = false;
        smallBlastTimerCounter = 0.25f;
        smallBlastCounter = 0;
        m_spriteRenderer.enabled = true;
        m_collider.enabled = true;
        isActive = true;
        isDestroyed = false;
        shutter.transform.position = shutterStartPos;
    }

    int smallBlastCounter;
    float smallBlastTimerCounter;
    bool isDestroyed = false;
    void DestructionSequence(){
        isActive = false;
        isDestroyed = true;
        m_collider.enabled = false;
        finalBoss.CoreGotDestroyed(num_ID);
    }
}
