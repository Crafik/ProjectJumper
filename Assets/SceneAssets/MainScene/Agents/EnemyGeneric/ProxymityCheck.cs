using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxymityCheck : MonoBehaviour
{
    private EnemyBehaviour script;
    void Start()
    {
        script = GetComponentInParent<EnemyBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            script.WakeUp();
        }
    }
}
