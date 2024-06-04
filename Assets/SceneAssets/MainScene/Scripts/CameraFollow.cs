using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera Cam;

    void Awake(){
        Cam = GetComponent<CinemachineVirtualCamera>();

        GameManager.OnGameRestart += SetFollowTarget;
    }

    void SetFollowTarget(){
        Cam.Follow = GameManager.Instance.Player.transform;
    }
}
