using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Totally unfinished, need to implement enemy interaction
    // will do after i implement enemies

    public float bulletSpeed = 10f;
    public bool facingRight = false;

    public Rigidbody2D body;

    public void SetDirection(bool toRight){
        facingRight = toRight;
    }

    void Start(){
        body.gravityScale = 0f;
    }

    void Update()
    {
        transform.position += (facingRight ? transform.right : -transform.right) * Time.deltaTime * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Enemy")){
            collision.gameObject.GetComponent<EnemyBehaviour>().GetDamage();
        }
        
        Destroy(gameObject);
    }

    private void OnBecameInvisible(){
        // I hope this will work as needed and don't cause unexpexted behaviour
        Destroy(gameObject);
    }
}
