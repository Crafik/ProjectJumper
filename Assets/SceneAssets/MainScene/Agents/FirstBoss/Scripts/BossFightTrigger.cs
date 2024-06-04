using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(AudioSource))]

public class BossFightTrigger : MonoBehaviour
{
    public GateBehaviour gate1;
    public GateBehaviour gate2;

    public string bossName;

    Rigidbody2D body;
    BoxCollider2D box;
    CinemachineImpulseSource impulser;
    AudioSource audioPlayer;

    public GameObject bossPrefab;
    GameObject bossEntity;

    private bool isActivated = false;
    private bool isBossActive = false;
    private bool isBossDefeated = false;
    // Gonna be a pain in the arse to deal with manager i guess

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        impulser = GetComponent<CinemachineImpulseSource>();
        audioPlayer = GetComponent<AudioSource>();
        box.isTrigger = true;
        body.bodyType = RigidbodyType2D.Static;
        gameObject.layer = LayerMask.NameToLayer("triggers");

        GameManager.OnGameRestart += OnGameRestart;
    }

    void OnTriggerEnter2D(Collider2D collision){
        // Place code here
         if (collision.gameObject.CompareTag("Player") && GameManager.Instance.isPlayerActive && !isActivated){
            MainCameraBehaviour.Instance.switcher.Play(bossName);
            // Here be code for activation boss fight
            // and for creating gates
            gate1.BuildGate();
            gate2.BuildGate();
            
            impulser.GenerateImpulse();
            audioPlayer.Play();

            isActivated = true;
            // Works, now to spawning boss, and managing the afterfight
        }
    }

    void OnGameRestart(){
        if (!isBossDefeated){
            if (isBossActive){
                Destroy(bossEntity);
                isBossActive = false;
            }
            if (isActivated){
                isActivated = false;
                spawnDelayCounter = 1.5f;
                gate1.OpenGate();
                gate2.OpenGate();
            }
        }
    }

    public void OnBossDefeated(){
        MainCameraBehaviour.Instance.switcher.Play(GameManager.Instance.currentActiveRoom);
        gate1.OpenGate();
        gate2.OpenGate();
        impulser.GenerateImpulse();
        audioPlayer.Play();
        isBossDefeated = true;
    }

    float spawnDelayCounter = 1.5f;
    void Update(){
        if (isActivated && spawnDelayCounter > 0){
            spawnDelayCounter -= Time.deltaTime;
        }
        if (spawnDelayCounter < 0 && !isBossActive){
            bossEntity = Instantiate(bossPrefab, transform.GetChild(4).position, Quaternion.identity);
            bossEntity.GetComponent<FirstBossBehaviour>().init(transform.GetChild(0).position.x, transform.GetChild(2).position.x, transform.GetComponent<BossFightTrigger>());
            isBossActive = true;
        }
        // Don't know what will be better
        // Track status of the boss here or call an event
        // Probably latter
    }
}
