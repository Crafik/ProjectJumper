using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CannonballBehaviour : MonoBehaviour
{

    private float moveSpeed;

    private bool isActive;

    [SerializeField] private GameObject sprite;

    void Awake(){
        isActive = false;
    }

    public void Init(float Speed){
        moveSpeed = Speed;
        isActive = true;
    }

    void Update(){
        if (isActive)
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
            sprite.transform.Rotate(0f, 0f, 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.gameObject.GetComponent<CharacterController2D>().PlayerDeath();
        }

        Destroy(gameObject);
    }
}
