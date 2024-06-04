using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpawnpointBehaviour : MonoBehaviour
{
    public static GunSpawnpointBehaviour Instance {get; private set;}

    public bool PickUpExists;

    public GameObject GunPickupPrefab;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
        PickUpExists = false;
    }

    public void SpawnGunPickup(){
        Instantiate(GunPickupPrefab, transform.position, Quaternion.identity);
        PickUpExists = true;
    }
}
