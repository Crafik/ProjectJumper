using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCameraBehaviour : MonoBehaviour
{
    // Decided to make this into singleton, should be a good thing
    // Need to implement tracking of runtime spawned player
    public static MainCameraBehaviour Instance {get; private set;}

    public Animator switcher;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        }
        else{
            Instance = this;
        }
    }

    void Start()
    {
        // Looks stoopid, but more clean
        // Would be better to find another way to do so
        switcher.Play("startroom");
        GameManager.Instance.currentActiveRoom = "startroom";
    }
}
