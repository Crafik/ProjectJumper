using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class LockBehaviour : MonoBehaviour
{
    private float unlockSpeed = 0.25f;
    private float unlockCounter;
    private bool isUnlocking;

    //renovations
    // Tilemap gates;
    // [Tooltip("Horizontal(Rightward) size of a gate from the point. [> 0]")]
    // public int sizeX;
    // [Tooltip("Vertical(Downward) size of a gate from the point. [> 0]")]
    // public int sizeY;
    // public RuleTile doorBlock;

    // Vector3Int startTilePos;

    public GateBehaviour m_gate;

    public CinemachineImpulseSource impulser;
    public AudioSource audioplayer;
    public AudioClip lock_open;
    public AudioClip door_open;

    void Awake(){
        isUnlocking = false;
        unlockCounter = unlockSpeed;
        rowCounter = 0;
    }

    public void ResetGate(){
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        isUnlocking = false;
        isFinished = false;
        unlockCounter = unlockSpeed;
        rowCounter = 0;
        m_gate.BuildGate();
    }

    public void Unlock(){
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        audioplayer.clip = lock_open;
        audioplayer.Play();
        isUnlocking = true;
    }

    int rowCounter;
    public bool isFinished;
    void Update(){
        if (isUnlocking && !isFinished){
            if (unlockCounter < 0f){
                // for (int i = 0; i < sizeX; ++i){
                //     gates.SetTile(new Vector3Int(startTilePos.x + i, startTilePos.y - rowCounter, startTilePos.z), null);
                // }
                m_gate.OpeningSequenceStep();
                rowCounter++;

                unlockCounter = unlockSpeed;
                Vector2 impDir = Random.insideUnitCircle.normalized;
                impulser.m_DefaultVelocity.x = impDir.x * 0.1f;
                impulser.m_DefaultVelocity.y = impDir.y * 0.1f;
                impulser.GenerateImpulse();
                audioplayer.clip = door_open;
                audioplayer.Play();
            }
            unlockCounter -= Time.deltaTime;
            if (rowCounter == m_gate.sizeY && unlockCounter < 0f){
                isFinished = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Key")){
            collision.GetComponent<KeyBehaviour>().Unlock(gameObject);
        }
    }
}
