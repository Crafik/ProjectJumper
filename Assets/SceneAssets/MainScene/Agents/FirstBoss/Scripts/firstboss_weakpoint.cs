using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstboss_weakpoint : MonoBehaviour
{
    public FirstBossBehaviour boss;

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("PlayerBullet")){
            boss.GetDamage();
        }
    }
}
