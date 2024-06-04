using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FinalBossTrigger : MonoBehaviour
{
    public FinalBossBehaviour finalBoss;

    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player") && !finalBoss.isActive && GameManager.Instance.isPlayerActive){
            finalBoss.StartBattle();
        }
    }
}