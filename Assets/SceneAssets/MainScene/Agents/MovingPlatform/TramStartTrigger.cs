using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TramStartTrigger : MonoBehaviour
{
    public MovingPlatformBehaviour platform;
    public killTrigger killTrigger;

    public Sprite switchOff;
    public Sprite switchOn;

    public SpriteRenderer m_sprite;
    public AudioSource m_audio;

    private bool isActivated;
    private bool isActive;

    void Awake(){
        isActivated = false;
        isActive = true;

        GameManager.OnGameRestart += PlatformReset;
        SpawnpointBehaviour.OnCheckpointEnter += StateSwitch;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("PlayerBullet") && !isActivated){
            isActivated = true;
            m_sprite.sprite = switchOn;
            m_audio.Play();
            platform.StartMoving();
            killTrigger.ActivateTrigger();
        }
    }

    void PlatformReset(){
        if (isActive){
            isActivated = false;
            m_sprite.sprite = switchOff;
            platform.ResetPlatform();
            killTrigger.DeactivateTrigger();
        }
    }

    void StateSwitch(GameObject omitted){
        if (isActivated){
            isActive = false;
        }
    }
}
