using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

public class KeyBehaviour : MonoBehaviour
{
    private bool isActive;
    private bool isUnlocking;

    private float speedHigh = 10f;
    private float speedMid = 7f;
    private float speedLow = 5f;

    private GameObject followPoint;

    void Awake(){
        isActive = false;
        isUnlocking = false;
    }

    void Update(){
        if (isActive){
            float currentSpeed;
            if (isUnlocking){
                if (Vector3.Distance(transform.position, followPoint.transform.position) > 5f)
                    currentSpeed = speedHigh;
                else if (Vector3.Distance(transform.position, followPoint.transform.position) > 3f)
                    currentSpeed = speedMid;
                else
                    currentSpeed = speedLow;
            }
            else{
                if (Vector3.Distance(transform.position, followPoint.transform.position) > 7f)
                    currentSpeed = speedHigh;
                else
                    currentSpeed = speedHigh - (speedHigh - Vector3.Distance(transform.position, followPoint.transform.position));
            }
            transform.position = Vector3.MoveTowards(transform.position, followPoint.transform.position, currentSpeed * Time.deltaTime);
            if (isUnlocking){
                if (transform.position == followPoint.transform.position){
                    followPoint.GetComponent<LockBehaviour>().Unlock();
                    Destroy(gameObject);
                }
            }
        }
    }

    public void PickedUp(GameObject follow){
        if (isActive){
            return;
        }
        followPoint = follow;
        isActive = true;
    }

    public void Unlock(GameObject unlockable){
        followPoint = unlockable;
        isUnlocking = true;
    }
}
