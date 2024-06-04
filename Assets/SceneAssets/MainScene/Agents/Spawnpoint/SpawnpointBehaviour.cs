using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnpointBehaviour : MonoBehaviour
{
    public delegate void CheckpointEvent(GameObject toThis);
    public static event CheckpointEvent OnCheckpointEnter;

    public SpriteRenderer sprite;
    
    public AudioSource audioplayer;

    public Sprite FlagOff;
    public Sprite FlagOn;

    private bool isActive;

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player") && !isActive && GameManager.Instance.isPlayerActive){
            OnCheckpointEnter(this.gameObject);
            SetSpawnpointActive();
        }
    }

    void SetSpawnpointActive(){
        isActive = true;
        sprite.sprite = FlagOn;
        audioplayer.Play();
    }

    public void SetSpawnpointInactive(){
        isActive = false;
        sprite.sprite = FlagOff;
    }
}
