using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterCrushTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player") && GameManager.Instance.isPlayerActive){
            GameManager.Instance.Player.GetComponent<CharacterController2D>().PlayerDeath();
        }
    }
}
