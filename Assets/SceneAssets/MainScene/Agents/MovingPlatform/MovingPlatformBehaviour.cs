using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MovingPlatformBehaviour : MonoBehaviour
{
    public GameObject path;
    GameObject start;
    GameObject end;

    public CinemachineVirtualCamera roomCamera;

    public float speedFactor;

    void Awake(){
        start = path.transform.GetChild(0).gameObject;
        end = path.transform.GetChild(1).gameObject;
        ResetPlatform();
    }

    bool isStarted = false;
    float currentVel = 0f;
    void FixedUpdate(){
        // here be lerp and actual trigger to start
        if(isStarted && transform.position != end.transform.position){
            //transform.position = new Vector2(Mathf.SmoothDamp(transform.position.x, end.transform.position.x, ref currentVel, 20f), transform.position.y);
            transform.position = Vector2.Lerp(start.transform.position, end.transform.position, currentVel);
            currentVel += Time.deltaTime * speedFactor;
        }
    }

    public void StartMoving(){
        roomCamera.Follow = transform;
        isStarted = true;
    }

    public void ResetPlatform(){
        transform.position = start.transform.position;
        currentVel = 0f;
        isStarted = false;
    }

    void OnCollisionEnter2D(Collision2D collision){
        collision.gameObject.transform.SetParent(transform);
    }

    // have to think is through
    void OnCollisionExit2D(Collision2D collision){
        if (gameObject.activeSelf){
            collision.gameObject.transform.parent = null;
        }
    }
}
