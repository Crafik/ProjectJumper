using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Note to myself: All init should be done in Awake.(i guess)

public class RoomManager : MonoBehaviour
{
    private List<GameObject> Spawners;
    private List<GameObject> Cannons;
    private List<GameObject> KeySpawner;

    void Awake(){
        Spawners = new List<GameObject>();
        Cannons = new List<GameObject>();
        KeySpawner = new List<GameObject>();
        foreach (Transform child in transform){
            if (child.CompareTag("EnemySpawner"))
                Spawners.Add(child.gameObject);
            if (child.CompareTag("Cannon"))
                Cannons.Add(child.gameObject);
            if (child.CompareTag("KeySpawner"))
                KeySpawner.Add(child.gameObject);
        }
    }

    void Start(){
        //Empty for now
    }

    public void SetRoomActive(bool active){
        // There is a room for improvement, probably will do later
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0){
            foreach (GameObject enemy in enemies){
                Destroy(enemy);
            }
        }
        // Here be spawning enemies
        if (active){
            if (Spawners.Count > 0){
                foreach (GameObject spawner in Spawners){
                    spawner.GetComponent<GenericEnemySpawner>().SpawnEnemy();
                }
            }
        }
        if (Cannons.Count > 0){
            foreach (GameObject cannon in Cannons){
                cannon.SetActive(active);
            }
        }
    }
}
