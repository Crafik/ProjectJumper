using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]

public class GateTrigger : MonoBehaviour
{
    public Sprite switchOff;
    public Sprite switchOn;

    public SpriteRenderer sprite;
    public AudioSource audioplayer;
    public CinemachineImpulseSource impulser;

    public GateBehaviour m_gate;

    private bool isActivated;
    private bool isActive;

    void Awake(){
        isActive = true;
        isActivated = false;
        GetComponent<BoxCollider2D>().isTrigger = true;

        GameManager.OnGameRestart += GateReset;
        SpawnpointBehaviour.OnCheckpointEnter += StateSwitch;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if ((collision.CompareTag("Player") || collision.CompareTag("PlayerBullet")) && !isActivated){
            m_gate.OpenGate();
            isActivated = true;
            sprite.sprite = switchOn;
            audioplayer.Play();
            Vector2 impDir = UnityEngine.Random.insideUnitCircle.normalized;
            impulser.m_DefaultVelocity.x = impDir.x * 0.1f;
            impulser.m_DefaultVelocity.y = impDir.y * 0.1f;
            impulser.GenerateImpulse();
        }
    }

    void GateReset(){
        if (isActive){
            m_gate.BuildGate();
            sprite.sprite = switchOff;
            isActivated = false;
        }
    }

    void StateSwitch(GameObject omitted){
        if (isActivated){
            isActive = false;
        }
    }
}
