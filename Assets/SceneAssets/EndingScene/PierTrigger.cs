using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            collision.GetComponent<EndingPlayerBehaviour>().NextStageTrigger();
        }
    }
}
