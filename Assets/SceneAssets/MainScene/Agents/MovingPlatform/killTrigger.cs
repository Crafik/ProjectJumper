using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class killTrigger : MonoBehaviour
{
    public GameObject m_camera;
    
    bool isActive = false;

    void Update(){
        if(isActive){
            transform.position = m_camera.transform.position;
        }
    }

    public void ActivateTrigger(){
        isActive = true;
    }

    public void DeactivateTrigger(){
        isActive = false;
    }

    void OnTriggerExit2D(Collider2D collision){
        if (gameObject.activeSelf){
            if (collision.CompareTag("Player") && isActive){
                collision.GetComponent<CharacterController2D>().PlayerDeath();
            }
        }
    }
}
