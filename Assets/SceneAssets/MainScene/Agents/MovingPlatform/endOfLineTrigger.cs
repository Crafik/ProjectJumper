using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class endOfLineTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera roomCamera;
    public killTrigger killTrigger;

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player") && roomCamera.Follow != GameManager.Instance.Player.transform){
            roomCamera.Follow = GameManager.Instance.Player.transform;
            killTrigger.DeactivateTrigger();
        }
    }
}
