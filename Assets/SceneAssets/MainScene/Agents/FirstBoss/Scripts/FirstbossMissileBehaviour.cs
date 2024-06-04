using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FirstbossMissileBehaviour : MonoBehaviour
{
    // Will be slow at start, accelerating to fast speed, slow to turn

    public float startSpeed;
    public float accel;
    public float turnSpeed;

    public Rigidbody2D rb;
    public SpriteRenderer m_sprite;

    void Start()
    {
        currentSpeed = startSpeed;
    }

    public float flickerPeriod;
    private float flickerCounter = 0;
    void Update(){
        if (flickerCounter < 0){
            m_sprite.flipY = !m_sprite.flipY;
            flickerCounter = flickerPeriod;
        }
        flickerCounter -= Time.deltaTime;
    }

    float currentSpeed;
    void FixedUpdate()
    {
        // here be movement calcs
        //transform.LookAt(GameManager.Instance.Player.transform);
        Vector2 playerDir = (GameManager.Instance.Player.transform.position - transform.position).normalized;
        float rotateAmount = -Vector3.Cross(playerDir, transform.right).z;

        rb.angularVelocity = turnSpeed * rotateAmount;
        rb.velocity = transform.right * currentSpeed;
        currentSpeed += accel;
    }

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.gameObject.GetComponent<CharacterController2D>().PlayerDeath();
        }
        // Kinda works
        Instantiate(GameManager.Instance.smallExplosionPrefab, transform.position + transform.right/2, Quaternion.identity);

        Destroy(gameObject);
    }
}
