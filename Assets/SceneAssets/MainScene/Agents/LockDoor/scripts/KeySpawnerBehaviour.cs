using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawnerBehaviour : MonoBehaviour
{
    // Need to save current status on OnCheckpointEnter event
    // Saving a door status here probably a good idea
    [SerializeField] private GameObject KeyPrefab;

    private bool isActive; // False if saved after key is used

    // Door variables
    public LockBehaviour m_lock;
    private GameObject m_key;

    void Awake(){
        isActive = true;

        GameManager.OnGameRestart += OnRestart;
        SpawnpointBehaviour.OnCheckpointEnter += DisableSpawner;
    }

    private void OnRestart(){
        Destroy(m_key);
        if (isActive){
            m_lock.ResetGate();
            m_key = Instantiate(KeyPrefab, transform.position, Quaternion.identity);
        }
    }

    private void DisableSpawner(GameObject omitted){
        if (m_key == null && m_lock.isFinished){
            isActive = false;
        }
    }
}
